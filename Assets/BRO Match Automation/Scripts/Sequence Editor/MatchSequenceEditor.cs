using UnityEngine;
using System.Collections.Generic;
using BRO.MatchAutomation;
using BRO.Game;

namespace BRO.SequenceEditor
{
    /// <summary>
    /// The controller for the match automation tool.
    /// </summary>
    public class MatchSequenceEditor : MonoBehaviour
    {
        #region Member Fields
        public List<Match> m_matchList = new List<Match>();
        public List<GameObject> m_matchButtonList = new List<GameObject>();
        public SequenceSettings m_sequenceSettings = new SequenceSettings(0, false, false);
        [SerializeField]
        private GameObject m_gridLayout;
        [SerializeField]
        private GameObject m_matchItemPrefab;
        [SerializeField]
        private MatchSettingsUI m_matchSettingsUI;
        [SerializeField]
        private MatchPreviewInformation m_previewMatchInfo;
        [SerializeField]
        private GameObject m_matchPreviewUI;
        [SerializeField]
        private SequenceSettingsUI m_sequenceSettingsUI;
        [SerializeField]
        private MatchAutomationManager m_automationManager;
        private const string m_SCENE_NAME_MAIN_MENU = "MainMenu";
        #endregion

        #region Member Properties

        #endregion

        #region UI Events
        /// <summary>
        /// Clears the sequence.
        /// </summary>
        public void OnNewClick()
        {
            ClearMatchSequence();
        }

        /// <summary>
        /// Hide Sequence Settings UIs.
        /// Display the UI for adding match item with assigned match information.
        /// </summary>
        public void OnAddMatchClick()
        {
            m_sequenceSettingsUI.gameObject.SetActive(false);
            m_matchSettingsUI.OnShowAdd();
            m_matchSettingsUI.gameObject.SetActive(true);
        }

        /// <summary>
        /// Hiide Match (add and modify) and Sequence Settings UIs.
        /// Save match sequence data, including matchList, matchButtonList, matchSequence.
        /// </summary>
        public void OnSaveClick()
        {
            m_matchSettingsUI.gameObject.SetActive(false);
            m_sequenceSettingsUI.gameObject.SetActive(false);

            // Make sure it is not possible to save empty list.
            if (m_matchList.Count == 0)
            {
                Debug.Log("Cannot save empty sequence.");
            }
            else
            {
                MatchSequenceData newMatchSequence = new MatchSequenceData(m_matchList, m_sequenceSettings);
                SaveLoad.Save(newMatchSequence);
            }
        }

        /// <summary>
        /// Hide Match (add and modify) and Sequence Settings UIs.
        /// First "clear" the match list and the on-screen match items.
        /// Then load the saved data and create the match items.
        /// </summary>
        public void OnLoadClick()
        {
            m_matchSettingsUI.gameObject.SetActive(false);
            m_sequenceSettingsUI.gameObject.SetActive(false);

            MatchSequenceData loadedMatchSequence = SaveLoad.Load();

            // Check if there is data before the process.
            // If there is data, clear the screen and display the saved match sequence.
            if (loadedMatchSequence != null)
            {
                ClearMatchSequence();

                for (int i = 0; i < loadedMatchSequence.MatchList.Count; i++)
                {
                    CreateMatch(loadedMatchSequence.MatchList[i]);
                }
                SetMatchSequence(loadedMatchSequence.Settings.Laps, loadedMatchSequence.Settings.Infinite, loadedMatchSequence.Settings.Shuffle);
            }
        }

        /// <summary>
        /// Hide Match Settings UI.
        /// Display the UI for match sequence settings.
        /// </summary>
        public void OnSequenceSettingsClick()
        {
            m_matchSettingsUI.gameObject.SetActive(false);
            m_sequenceSettingsUI.gameObject.SetActive(true);
            m_sequenceSettingsUI.OnShowSequenceSettings(m_sequenceSettings);
        }

        /// <summary>
        /// Hide Sequence Settings and Preview UIs.
        /// Display the UI with corresponding match information and allow modification.
        /// </summary>
        /// <param name="match"></param>
        public void OnMatchItemClick(Match match)
        {
            m_sequenceSettingsUI.gameObject.SetActive(false);
            m_previewMatchInfo.gameObject.SetActive(false);
            m_matchSettingsUI.OnShowModify(match);
            m_matchSettingsUI.gameObject.SetActive(true);
        }

        /// <summary>
        /// Starts running the matches of the sequences automaitcally.
        /// </summary>
        public void OnRunSequence()
        {
            if(m_matchList.Count > 0)
                m_automationManager.StartMatchAutomation(m_matchList, m_sequenceSettings);
        }

        /// <summary>
        /// Loads the main menu.
        /// </summary>
        public void OnBackToMenu()
        {
            LoadSceneManager.Instance.LoadScene(m_SCENE_NAME_MAIN_MENU,
                new LoadingFlags() { UseFade = false, Local = true, ShowInfo = false, UseLoadingScreen = false, WaitForInput = false });
        }
        #endregion

        #region Unity Lifecycle
        void Awake()
        {

        }

        void Start()
        {

        }

        void OnEnable()
        {

        }

        void Update()
        {

        }
        #endregion

        #region Public Functions
        /// <summary>
        /// Creates a match button and object based on match information, which gets added to a list.
        /// </summary>
        /// <param name="newMatch">Match object without id</param>
        public void CreateMatch(Match newMatch)
        {
            newMatch.Id = AssignMatchId();
            m_matchList.Add(newMatch);

            GameObject go = Instantiate(m_matchItemPrefab);
            go.transform.SetParent(m_gridLayout.transform);
            go.GetComponent<MatchItem>().ReceiveMatchData(newMatch);
            m_matchButtonList.Add(go);

        }

        /// <summary>
        /// Hide Preview and Match Settings UIs.
        /// Deletes a match (i.e. removing list references, destroying its gameObject)
        /// </summary>
        /// <param name="match">Match to be deleted</param>
        public void DeleteMatch(Match match)
        {
            m_previewMatchInfo.gameObject.SetActive(false);
            m_matchSettingsUI.gameObject.SetActive(false);
            m_matchList.RemoveAt(match.Id);
            Destroy(m_matchButtonList[match.Id]);
            m_matchButtonList.RemoveAt(match.Id);
            UpdateIds();
        }

        /// <summary>
        /// Display the match inforamtion when Match (add and modify) or Sequence Settings UIs are not displayed.
        /// </summary>
        /// <param name="match"></param>
        public void DisplayMatchPreview(Match match)
        {
            if ((m_matchSettingsUI.gameObject.activeInHierarchy == false) &&
                (m_sequenceSettingsUI.gameObject.activeInHierarchy == false))
            {
                m_previewMatchInfo.UpdateUI(match);
                m_matchPreviewUI.SetActive(true);
            }
        }

        /// <summary>
        /// Stop display the match information.
        /// </summary>
        public void StopDisplayMatchPreview()
        {
            m_matchPreviewUI.SetActive(false);
        }

        /// <summary>
        /// Moves the dragged match to the spot of a different match in order to adjust the order of the sequence.
        /// </summary>
        /// <param name="matchToMove">Moving match</param>
        /// <param name="targetMatch">Match with the taget slot</param>
        public void ReOrderMatches(int matchToMove, int targetMatch)
        {
            // 1. assign the match to move the new id
            m_matchList[matchToMove].Id = targetMatch;

            // 2. update all the other affected ids
            if (matchToMove < targetMatch)
            {
                for (int i = targetMatch; i > matchToMove; i--)
                {
                    m_matchList[i].Id = i - 1;
                }
            }
            else if (matchToMove > targetMatch)
            {
                for (int i = targetMatch; i < matchToMove; i++)
                {
                    m_matchList[i].Id = i + 1;
                }
            }

            // 3. sort the match list
            m_matchList = OrderById(m_matchList);

            // 4. instantiate new match item buttons
            m_matchButtonList = InstantiateMatchItems(m_matchList);
        }

        /// <summary>
        /// Update the match sequence settings accordingly.
        /// </summary>
        /// <param name="loop"></param>
        /// <param name="infinite"></param>
        /// <param name="shuffle"></param>
        public void SetMatchSequence(int loop, bool infinite, bool shuffle)
        {
            m_sequenceSettings.Laps = loop;
            m_sequenceSettings.Infinite = infinite;
            m_sequenceSettings.Shuffle = shuffle;
        }
        #endregion

        #region Private Functions
        /// <summary>
        /// Assigns an initial ID to a created match.
        /// </summary>
        /// <returns></returns>
        private int AssignMatchId()
        {
            return m_matchList.Count;
        }

        /// <summary>
        /// Assigns new IDs to all matches.
        /// </summary>
        private void UpdateIds()
        {
            for (int i = 0; i < m_matchList.Count; i++)
            {
                m_matchList[i].Id = i;
                m_matchButtonList[i].GetComponent<MatchItem>().UpdateMatchData();
            }
        }

        /// <summary>
        /// Sort the match list according to their IDs after moving any match item.
        /// </summary>
        /// <param name="matchList"></param>
        /// <returns></returns>
        private List<Match> OrderById(List<Match> matchList)
        {
            matchList.Sort((x, y) => x.Id.CompareTo(y.Id));
            return matchList;
        }

        /// <summary>
        /// Instantiate new match items after moving any match item.
        /// </summary>
        /// <param name="matchList"></param>
        /// <returns></returns>
        private List<GameObject> InstantiateMatchItems(List<Match> matchList)
        {
            List<GameObject> newMatchButtons = new List<GameObject>();

            DestroyMatchButtons();

            foreach (Match match in matchList)
            {
                GameObject go = Instantiate(m_matchItemPrefab);
                go.transform.SetParent(m_gridLayout.transform);
                go.GetComponent<MatchItem>().ReceiveMatchData(match);
                newMatchButtons.Add(go);
            }
            return newMatchButtons;
        }

        /// <summary>
        /// Empties the match sequence and its related objects.
        /// </summary>
        private void ClearMatchSequence()
        {
            DestroyMatchButtons();
            m_matchList = new List<Match>();
        }

        /// <summary>
        /// Destroys all GameObjects of the match button list field.
        /// </summary>
        private void DestroyMatchButtons()
        {
            foreach (GameObject go in m_matchButtonList)
            {
                Destroy(go);
            }
            m_matchButtonList = new List<GameObject>();
        }
        #endregion
    }
}