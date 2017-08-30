using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Bolt;
using BRO.MatchAutomation;

namespace BRO.Game
{
    /// <summary>
    /// The GameOverUI coves functionality its UI events and receives the event about the winner of the game.
    /// </summary>
    public class GameOverUI : GlobalEventListener
    {
        #region Member
        [SerializeField]
        private GameObject m_menuGameObject;
        [SerializeField]
        private Text m_winnerText;
        private const string m_MAIN_MENU_NAME = "MainMenu";
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

        #region Bolt Events
        /// <summary>
        /// Enables the game over ui game object
        /// </summary>
        /// <param name="evnt">Event data with knowledge of the winner's id</param>
        public override void OnEvent(WinnerEvent evnt)
        {
            Invoke("PopUpGameOverUI", 3.0f);
            m_winnerText.text += evnt.winnerName + " !!!";
        }

        /// <summary>
        /// OnBoltShutDown is triggered by a Callback added by the BoltShutdownBegin event.
        /// Loads the main menu.
        /// </summary>
        /// <param name="registerDoneCallback"></param>
        public override void BoltShutdownBegin(AddCallback registerDoneCallback)
        {
            if(!GameObject.Find("MatchAutomationManager"))
                registerDoneCallback(OnBoltShutDown);
        }
        #endregion

        #region Private Functions
        /// <summary>
        /// Displays the UI for the match's conclusion
        /// </summary>
        private void PopUpGameOverUI()
        {
            m_menuGameObject.SetActive(true);
        }

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
    }
}