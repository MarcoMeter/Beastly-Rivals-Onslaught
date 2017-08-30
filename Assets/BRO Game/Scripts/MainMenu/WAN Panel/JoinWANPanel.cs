using UnityEngine;
using System.Collections.Generic;
using UdpKit;
using UnityEngine.UI;
using Bolt;

namespace BRO.Game.PreMatch
{
    /// <summary>
    /// The JoinWANPanel provides logic for connecting to an online match.
    /// </summary>
    public class JoinWANPanel : GlobalEventListener
    {
        #region Member Fields
        [SerializeField]
        private GameObject m_lobbyPanel;
        [SerializeField]
        private GameObject m_mainPanel;
        [SerializeField]
        private GameObject m_gameFoundText;
        [SerializeField]
        private InputField m_playerNameInput;
        [SerializeField]
        private GameObject m_joinGameButtonPrefab;

        private List<UdpSession> m_sessions = new List<UdpSession>();
        private List<GameObject> m_joinGameButtons = new List<GameObject>();
        private float m_buttonPosX = 0;
        private float m_buttonPosY = 250;
        #endregion

        #region Unity Lifecycle
        /// <summary>
        /// Start Bolt client
        /// Load player name from PlayerPrefs.
        /// </summary>
        private void Start()
        {
            BoltLauncher.StartClient(UdpEndPoint.Any);
            m_playerNameInput.text = PlayerPrefs.GetString("playerNameKey");
        }
        #endregion

        #region UI Events
        /// <summary>
        /// Connect to a session based on the Button's ID.
        /// Save the player's name to the PlayerPrefs and assign the PlayerInformation.
        /// </summary>
        /// <param name="gameIdComponent"></param>
        public void Connect(JoinGameButtonId gameIdComponent)
        {
            PlayerPrefs.SetString("playerNameKey", m_playerNameInput.text);
            PlayerInformation.myPlayer.playerName = m_playerNameInput.text;
            if (m_sessions.Count > 0)
            {
                BoltNetwork.Connect(m_sessions[gameIdComponent.gameId]);
            }
        }

        /// <summary>
        /// Request a new SessionList of matches.
        /// </summary>
        public void Refresh()
        {
            Bolt.Zeus.RequestSessionList();
        }

        /// <summary>
        /// In order to leave the JoinWANPanel, Bolt has to be shut down.
        /// </summary>
        public void OnCancel()
        {
            m_lobbyPanel.SetActive(false);
            m_mainPanel.SetActive(true);
            BoltLauncher.Shutdown();
        }

        /// <summary>
        /// On submitting the name inside the InputField, it gets assigned to the PlayerInformation and saved inside the PlayerPrefs.
        /// </summary>
        public void OnNameSubmit()
        {
            PlayerInformation.myPlayer.playerName = m_playerNameInput.text;
            PlayerPrefs.SetString("playerNameKey", m_playerNameInput.text);
        }
        #endregion

        #region Bolt Events
        /// <summary>
        /// Connect to Zeus as soon as the Bolt completed starting.
        /// </summary>
        public override void BoltStartDone()
        {
            Bolt.Zeus.Connect();
        }

        /// <summary>
        /// As soon as the connection to Zeus got established, request a session list.
        /// </summary>
        /// <param name="endpoint"></param>
        public override void ZeusConnected(UdpKit.UdpEndPoint endpoint)
        {
            Zeus.RequestSessionList(); // As soon as the connection to Zeus got established, request the available game sessions.
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionList"></param>
        public override void SessionListUpdated(UdpKit.Map<System.Guid, UdpKit.UdpSession> sessionList)
        {
            InstantiateSessionButtons();
        }

        /// <summary>
        /// On completed connection, enable the lobby.
        /// </summary>
        /// <param name="connection"></param>
        public override void Connected(BoltConnection connection)
        {
            ClearJoinButtons();
            m_lobbyPanel.SetActive(true);
            gameObject.SetActive(false);
        }

        public override void ConnectFailed(UdpEndPoint endpoint, IProtocolToken token)
        {
            transform.root.GetComponent<MainMenuCanvas>().PostMessage("Connection failed", "Couldn't establish connection to server."); // Display error on connection failure
        }

        public override void ZeusConnectFailed(UdpEndPoint endpoint)
        {
            transform.root.GetComponent<MainMenuCanvas>().PostMessage("Connection failed", "Couldn't establish connection to matchmaking server Zeus."); // Display error on connection failure
        }

        public override void SessionConnectFailed(UdpSession session, IProtocolToken token)
        {
            transform.root.GetComponent<MainMenuCanvas>().PostMessage("Connection failed", "Couldn't establish connection to session."); // Display error on connection failure
        }
        #endregion

        #region Local Functions
        /// <summary>
        /// ArrangeSessions instantiates Buttons for each game sessions available. This function is triggered as Message by CallBacksWAN.
        /// </summary>
        private void InstantiateSessionButtons()
        {
            m_gameFoundText.GetComponent<Text>().text = "Games found = " + BoltNetwork.SessionList.Count;
            ClearJoinButtons();
            if (BoltNetwork.SessionList.Count > 0)
            {
                // Re-initialize member
                int i = 0;
                m_buttonPosY = 250;
                m_sessions = new List<UdpSession>();
                foreach (var session in BoltNetwork.SessionList)
                {
                    // Save each session
                    m_sessions.Add(session.Value);
                    // Instantiate Buttons which make you join a game/session
                    GameObject joinButton = (GameObject)Instantiate(m_joinGameButtonPrefab, new Vector3(m_buttonPosX, m_buttonPosY, 0), Quaternion.identity);
                    joinButton.transform.SetParent(transform);
                    joinButton.transform.localPosition = new Vector3(m_buttonPosX, m_buttonPosY, 0);
                    // The gameId corresponds to the session
                    JoinGameButtonId gameIdComponent = joinButton.GetComponent<JoinGameButtonId>();
                    gameIdComponent.gameId = i;
                    joinButton.GetComponent<Button>().onClick.AddListener(() => { Connect(gameIdComponent); });
                    joinButton.GetComponentInChildren<Text>().text = m_sessions[i].HostName;
                    m_joinGameButtons.Add(joinButton);
                    // Update counter
                    m_buttonPosY -= 40;
                    i++;
                }
            }
        }

        /// <summary>
        /// Deletes all existing join buttons and initialize their list again.
        /// </summary>
        private void ClearJoinButtons()
        {
            if (m_joinGameButtons.Count > 0)
            {
                for (int i = 0; i < m_joinGameButtons.Count; i++)
                {
                    Destroy(m_joinGameButtons[i]);
                }
            }
            m_joinGameButtons = new List<GameObject>();
        }
        #endregion
    }
}