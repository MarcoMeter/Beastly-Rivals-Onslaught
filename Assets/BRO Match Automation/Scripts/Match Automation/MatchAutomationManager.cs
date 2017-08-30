using BRO.AI;
using BRO.AI.Loader;
using BRO.Game;
using BRO.Game.PreMatch;
using BRO.SequenceEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BRO.MatchAutomation
{
    /// <summary>
    /// The MatchAutomationManager controlls the whole flow of processing all querried matches from start to end.
    /// </summary>
    public class MatchAutomationManager : GenericSingleton<MatchAutomationManager>
    {
        #region Member Fields
        [SerializeField]
        private GameObject m_overlayCanvas;
        [SerializeField]
        private GameObject m_coreMatchCanvas;
        private const string m_SCENE_NAME_CORE_MATCH = "CoreMatch";
        private const string m_SCENE_NAME_LOAD_SCREEN = "LoadingScreen";
        private const string m_SCENE_NAME_MAIN_MENU = "MainMenu";
        private MatchAutomationBoltEventListener m_boltEventListener;

        private bool m_stop = false;
        private List<Match> m_matchSequence = new List<Match>();
        private SequenceSettings m_sequenceSettings;
        private int m_currentLap = 0;
        private int m_currentMatchIndex = 0;
        private int m_countMatches = 0;
        private PlayerRegistrationService m_playerRegistration;
        private bool m_manualTimeScale = false;

        private DateTime m_startTime;
        private DateTime m_endTime;
        private TimeSpan m_elapsedTime;
        #endregion

        #region Member Properties
        public DateTime StartTime
        {
            get { return m_startTime; }
        }

        public Match CurrentMantch
        {
            get { return m_matchSequence[m_currentMatchIndex]; }
        }

        public int MatchCount
        {
            get { return m_matchSequence.Count; }
        }

        public int CurrentMatchIndex
        {
            get { return m_currentMatchIndex; }
        }

        public int CurrentLap
        {
            get { return m_currentLap; }
        }

        public int Laps
        {
            get {  return m_sequenceSettings.Laps; }
        }

        public int ProcessedMatchCount
        {
            get { return m_countMatches; }
        }

        public bool InfiniteLaps
        {
            get { return m_sequenceSettings.Infinite; }
        }
        #endregion

        #region Unity Lifecycle
        /// <summary>
        /// Initializes the MatchAutomationController.
        /// </summary>
        private void Start()
        {
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnLevelFinishedLoading;
        }

        /// <summary>
        /// Removes the listener to the sceneLoaded field.
        /// </summary>
        private void OnDestroy()
        {
            Time.timeScale = 1;
            SceneManager.sceneLoaded -= OnLevelFinishedLoading;
        }
        #endregion

        #region Public Functions
        /// <summary>
        /// Launches the procedure of playing a list of matches automatically based on certain settings.
        /// </summary>
        /// <param name="matchSequence">List of matches which are supposed to be automatically played.</param>
        /// <param name="sequenceSettings">Setting for processing the match sequence.</param>
        public void StartMatchAutomation(List<Match> matchSequence, SequenceSettings sequenceSettings)
        {
            m_matchSequence = matchSequence;
            m_sequenceSettings = sequenceSettings;
            m_overlayCanvas.SetActive(true);
            m_startTime = DateTime.Now;

            // Shuffle list
            if (m_sequenceSettings.Shuffle)
            {
                m_matchSequence.Shuffle();
            }

            // Start the first match
            CommenceMatch();
        }

        /// <summary>
        /// Triggers the process for running the next match.
        /// </summary>
        public void CommenceMatch()
        {
            if (m_boltEventListener)
            {
                Destroy(m_boltEventListener);
            }
            m_boltEventListener = gameObject.AddComponent<MatchAutomationBoltEventListener>();
            BoltLauncher.StartSinglePlayer();
        }

        /// <summary>
        /// Initializes all relevant information for the current match.
        /// </summary>
        public void InitializeMatch()
        {
            StartCoroutine(InitializeGameController());
        }

        /// <summary>
        /// Concludes the finished match. Starts a new match if conditions are met.
        /// </summary>
        public void ConcludeMatch()
        {
            m_countMatches++;

            if(m_stop)
            {
                FinishMatchAutomation();
                return;
            }

            // Process sequence
            m_currentMatchIndex++;

            // If all matches are processed within one lap
            if (m_currentMatchIndex >= m_matchSequence.Count)
            {
                // Increment lap
                m_currentLap++;
                // Reset index to 0
                m_currentMatchIndex = 0;

                // Shuffle list
                if(m_sequenceSettings.Shuffle)
                {
                    m_matchSequence.Shuffle();
                }

                // If infinite laps are enabled, just commence the next match
                if (m_sequenceSettings.Infinite)
                {
                    CommenceMatch();
                }
                else
                {
                    // Finish automation if no laps are left
                    if (m_currentLap >= m_sequenceSettings.Laps)
                    {
                        FinishMatchAutomation();
                    }
                    else
                    {
                        CommenceMatch();
                    }
                }
            }
            else
            {
                CommenceMatch();
            }
        }

        /// <summary>
        /// Toogles on/off the simulation speed manual modification.
        /// </summary>
        public void ToggleSimulationSpeed()
        {
            m_manualTimeScale = !m_manualTimeScale;
        }
        #endregion

        #region UI Events
        /// <summary>
        /// Stops the match immediately without finishing the ongoing match.
        /// </summary>
        public void ForceStop()
        {
            BoltLauncher.Shutdown();
            m_countMatches--;           // subtract the current ongoing match
            m_stop = true;
        }

        /// <summary>
        /// Stops the automation after the ongoing match concluded.
        /// </summary>
        public void Stop()
        {
            m_stop = true;
        }
        #endregion

        #region Local Functions
        /// <summary>
        /// Initializes the GameController, registers AI players and loads the CoreMatch.
        /// </summary>
        /// <returns></returns>
        private IEnumerator InitializeGameController()
        {
            BoltNetwork.Instantiate(BoltPrefabs.GameController);
            yield return new WaitForSeconds(0.5f);

            // Register AI players
            m_playerRegistration = GameController.Instance.GetComponentInChildren<PlayerRegistrationService>();

            foreach (var ai in m_matchSequence[m_currentMatchIndex].AIPlayers)
            {
                if (ai != null)
                {
                    m_playerRegistration.RegisterAiPlayer(ai);
                }
            }
            m_playerRegistration.SetPlayerLifes(m_matchSequence[m_currentMatchIndex].Lives);

            // If the players shall not loose lives, then apply that property to the MatchFlowControl
            if (m_matchSequence[m_currentMatchIndex].InfiniteLives)
            {
                var flowControl = GameController.Instance.GetComponentInChildren<MatchFlowControl>();
                flowControl.InfiniteLives = true;
            }

            // Load level
            LoadingFlags flags = new LoadingFlags() {Local = true, ShowInfo = false, UseFade = true, UseLoadingScreen = true, WaitForInput = false};
            LoadSceneManager.Instance.LoadScene(m_SCENE_NAME_CORE_MATCH, flags);
        }

        /// <summary>
        /// Finishes the automation and loads the Main Menu.
        /// </summary>
        private void FinishMatchAutomation()
        {
            m_endTime = DateTime.Now;
            m_elapsedTime = m_endTime - m_startTime;
            Debug.Log("Start Time: " + m_startTime);
            Debug.Log("End Time: " + m_endTime);
            Debug.Log("Matches played: " + m_countMatches);
            Debug.Log("Processed laps: " + m_currentLap);
            Debug.Log("Elapsed time: " + m_elapsedTime.Days + "d " + m_elapsedTime.Hours + "h " + m_elapsedTime.Minutes + "min " + m_elapsedTime.Seconds +"s");

            // Load level
            LoadingFlags flags = new LoadingFlags() { Local = true, ShowInfo = false, UseFade = true, UseLoadingScreen = true, WaitForInput = false };
            LoadSceneManager.Instance.LoadScene(m_SCENE_NAME_MAIN_MENU, flags);
        }

        /// <summary>
        /// Desotrys the MatchAutomationManager, if it is not inside a valid scene.
        /// Sets the time scale.
        /// </summary>
        /// <param name="scene">Loaded scene</param>
        /// <param name="loadSceneMode"></param>
        private void OnLevelFinishedLoading(Scene scene, LoadSceneMode loadSceneMode)
        {
            m_overlayCanvas.SetActive(false);
            if (!scene.name.Equals(m_SCENE_NAME_CORE_MATCH) && !scene.name.Equals(m_SCENE_NAME_LOAD_SCREEN))
            {
                Destroy(gameObject);
            }

            if(scene.name.Equals(m_SCENE_NAME_CORE_MATCH))
            {
                m_coreMatchCanvas.SetActive(true);
                if (!m_manualTimeScale)
                {
                    Time.timeScale = m_sequenceSettings.SimulationSpeed;
                }
            }
            else
            {
                if (!m_manualTimeScale)
                {
                    Time.timeScale = 1;
                }
            }
        }
        #endregion
    }
}