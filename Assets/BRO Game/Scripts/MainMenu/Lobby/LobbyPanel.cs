using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using BRO.AI;
using Bolt;

namespace BRO.Game.PreMatch
{
    /// <summary>
    /// The lobby panel gathers all the connected clients and the host.
    /// It provides logic for the host to kick players and to make changes to the game settings.
    /// </summary>
    public class LobbyPanel : Bolt.GlobalEventListener
    {
        #region Member Fields
        public PlayerSlot[] playerSlots = new PlayerSlot[8];
        public GameObject[] hostButtons;
        public InputField playerLifesInput;
        public Text placeHolderText;

        [SerializeField]
        private GameObject m_AiWindowGo;
        [SerializeField]
        private AddAiWindow m_aiWindow;

        private BoltEntity m_gameController;
        private IGameControllerState m_gmcState;
        private PlayerRegistrationService m_playerRegistration;
        #endregion

        #region Unity Lifecycle
        /// <summary>
        /// Assigns the instance of the GameController and adds a callback to update the UI frequently.
        /// It also disables some buttons on the client side.
        /// </summary>
        void Start()
        {
            // Get the reference of the GameController. The server is probably the only who is getting the reference at this point
            if (GameObject.FindGameObjectWithTag("GameController"))
            {
                m_gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<BoltEntity>();
                m_gmcState = m_gameController.GetState<IGameControllerState>();
                m_gmcState.AddCallback("players[]", OnPlayerStatsChanged); // add callback to update the Lobby's ui on event
            }

            // Disable host buttons on clients
            if (BoltNetwork.isClient)
            {
                for (int i = 0; i < hostButtons.Length; i++)
                {
                    hostButtons[i].SetActive(false);
                }
                playerLifesInput.gameObject.SetActive(false);
            }

            if (BoltNetwork.isServer)
            {
                m_playerRegistration = m_gameController.GetComponentInChildren<PlayerRegistrationService>();
                m_playerRegistration.RegisterHostPlayer();
                placeHolderText.text = m_playerRegistration.GetPlayerLifes().ToString();
            }
        }

        /// <summary>
        /// It might occur that the GameController is not found during Start, so Update makes sure that it keeps looking for it untill it's found.
        /// </summary>
        void Update()
        {
            // Seek for the GameController and add a Callback to the players' properties
            if (GameObject.FindGameObjectWithTag("GameController") && !m_gameController)
            {
                m_gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<BoltEntity>();
                m_gmcState = m_gameController.GetState<IGameControllerState>();
                m_gmcState.AddCallback("players[]", OnPlayerStatsChanged); // add callback to update the Lobby's ui on event
            }
        }
        #endregion

        #region State Callbacks
        /// <summary>
        /// Only called on clients, it updates the UI of the Lobby corresponding to the GameController's state
        /// </summary>
        /// <param name="stateToCallback"></param>
        /// <param name="path"></param>
        /// <param name="indices"></param>
        public void OnPlayerStatsChanged(Bolt.IState stateToCallback, string path, Bolt.ArrayIndices indices)
        {
            if (m_gameController)
            {
                stateToCallback = (IGameControllerState)stateToCallback;

                for (int i = 0; i < playerSlots.Length; i++)
                {
                    if (playerSlots[i])
                    {
                        playerSlots[i].playerName.text = m_gmcState.players[i].playerName;

                        if (m_gmcState.players[i].lobbyReady)
                        {
                            playerSlots[i].readyImage.gameObject.SetActive(true);
                            playerSlots[i].notReadyImage.gameObject.SetActive(false);
                        }

                        if (!m_gmcState.players[i].lobbyReady)
                        {
                            playerSlots[i].readyImage.gameObject.SetActive(false);
                            playerSlots[i].notReadyImage.gameObject.SetActive(true);
                        }
                    }
                }
            }
        }
        #endregion

        #region UI Events
        /// <summary>
        /// Closes the Bolt socket in order to leave the lobby.
        /// </summary>
        public void Leave()
        {
            BoltLauncher.Shutdown();
            transform.root.GetComponent<MainMenuCanvas>().PostMessage("Leaving the lobby", "Hold on ...");
        }

        /// <summary>
        /// Sends the PlayerReadyLobbyEvent.
        /// </summary>
        public void Ready()
        {
            var ev = PlayerReadyLobbyEvent.Create(Bolt.GlobalTargets.OnlyServer);
            ev.playerId = PlayerInformation.myPlayer.id;
            ev.Send();
        }

        /// <summary>
        /// Kicks a player by his id.
        /// </summary>
        /// <param name="id">Player's id.</param>
        public void KillSlot(int id)
        {
            m_playerRegistration.DeregisterPlayerKicked(id);
        }

        /// <summary>
        /// As soon as the InputField for the player lifes changed, update the player's information inside the gameController.
        /// </summary>
        public void UpdatePlayerLifes()
        {
            if (playerLifesInput.text.Length > 0)
                m_playerRegistration.SetPlayerLifes(int.Parse(playerLifesInput.text));
        }
        #endregion

        #region Bolt Events
        /// <summary>
        /// Only client-side.
        /// Posts a message and lets the client load the MainMenu scene again.
        /// </summary>
        /// <param name="connection"></param>
        public override void Disconnected(BoltConnection connection)
        {
            if (BoltNetwork.isClient)
            {
                transform.root.GetComponent<MainMenuCanvas>().PostMessage("Disconnected", "You got kicked or the host closed the session.");
                Invoke("LoadMainMenu", 2.0f);
            }
        }

        /// <summary>
        /// As soon as Bolt is demanded to shutdow, a callback is added to get notified upon on the completion of shutting down Bolt.
        /// </summary>
        /// <param name="registerDoneCallback">Delegate callback</param>
        public override void BoltShutdownBegin(AddCallback registerDoneCallback)
        {
            registerDoneCallback(OnBoltShutDown);
        }
        #endregion

        #region Private Functions
        /// <summary>
        /// This is invoked by the Disconnected Bolt Event.
        /// </summary>
        private void LoadMainMenu()
        {
            SceneManager.LoadScene(0);
        }

        /// <summary>
        /// OnBoltShutDown is triggered by a Callback added by the BoltShutdownBegin event.
        /// Reloads the main menu.
        /// </summary>
        private void OnBoltShutDown()
        {
            LoadMainMenu();
        }
        #endregion

        #region Public Functions
        /// <summary>
        /// The lobby panel passes the AddAI event to the PlayerRegistrationService.
        /// </summary>
        /// <param name="aiBase">The AI to be added</param>
        public void AddAI(AIBase aiBase)
        {
            m_playerRegistration.RegisterAiPlayer(aiBase); // GameController takes care of adding another player
        }
        #endregion
    }
}