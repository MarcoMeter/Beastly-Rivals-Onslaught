using Bolt;
using BRO.MatchAutomation;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BRO.Game
{
    /// <summary>
    /// Logic of the in-game menu UI.
    /// </summary>
    public class QuitUI : GlobalEventListener
    {
        #region Member
        [SerializeField]
        private GameObject m_menuGameObject;
        private const string m_MAIN_MENU_NAME = "MainMenu";
        #endregion

        #region Unity Lifecycle
        /// <summary>
        /// Update listens to Input Events to open or close the in-game menu.
        /// </summary>
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (m_menuGameObject.activeInHierarchy)
                {
                    m_menuGameObject.SetActive(false);
                }
                else
                {
                    m_menuGameObject.SetActive(true);
                }
            }
        }
        #endregion

        #region UI Events
        /// <summary>
        /// Shuts down bolt and loads the menu as soon as bolt is done shutting down.
        /// </summary>
        public void LoadMenu()
        {
            BoltLauncher.Shutdown();
        }

        /// <summary>
        /// Closes the game
        /// </summary>
        public void QuitGame()
        {
            Application.Quit();
        }
        #endregion

        #region Private Functions
        /// <summary>
        /// OnBoltShutDown is triggered by a Callback added by the BoltShutdownBegin event.
        /// Reloads the main menu.
        /// </summary>
        private void OnBoltShutDown()
        {
            LoadingFlags flags = new LoadingFlags() { Local = true, ShowInfo = false, UseFade = true, UseLoadingScreen = true, WaitForInput = false };
            LoadSceneManager.Instance.LoadScene(m_MAIN_MENU_NAME, flags);
        }
        #endregion

        #region Bolt Events
        /// <summary>
        /// OnBoltShutDown is triggered by a Callback added by the BoltShutdownBegin event.
        /// Loads the main menu.
        /// </summary>
        /// <param name="registerDoneCallback"></param>
        public override void BoltShutdownBegin(AddCallback registerDoneCallback)
        {
            if (!GameObject.Find("MatchAutomationManager"))
                registerDoneCallback(OnBoltShutDown);
        }
        #endregion
    }
}