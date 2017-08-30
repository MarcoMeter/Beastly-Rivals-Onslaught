using UnityEngine;
using UdpKit;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

namespace BRO.Game.PreMatch
{
    /// <summary>
    /// The JoinLANPanel provides UI logic for connecting to a local match.
    /// </summary>
    public class JoinLANPanel : Bolt.GlobalEventListener
    {
        #region Member Fields
        [SerializeField]
        private int m_serverPort = 4000;
        [SerializeField]
        private InputField m_ipInput;
        [SerializeField]
        private GameObject m_lobbyPanel;
        [SerializeField]
        private GameObject m_mainPanel;
        [SerializeField]
        private InputField m_playerNameInput;
        [SerializeField]
        private GameObject m_joinGameButtonPrefab;
        private List<GameObject> m_joinGameButtons = new List<GameObject>();
        private float m_buttonPosX = 0;
        private float m_buttonPosY = 250;
        private List<UdpSession> m_sessions = new List<UdpSession>();
        private string m_serverAdress = "127.0.0.1";
        #endregion

        #region Unity Lifecycle
        /// <summary>
        /// Sets a default server adress and loads the player's name of the PlayerPrefs.
        /// </summary>
        void Start()
        {
            BoltLauncher.StartClient(UdpEndPoint.Any);
            m_ipInput.text = m_serverAdress;
            m_playerNameInput.text = PlayerPrefs.GetString("playerNameKey");
        }
        #endregion

        #region Bolt Events
        /// <summary>
        /// As soon as Bolt started enable lan broadcasting
        /// </summary>
        public override void BoltStartDone()
        {
            BoltNetwork.EnableLanBroadcast();
        }

        /// <summary>
        /// If the connection was succesfull, open the lobby panel.
        /// </summary>
        /// <param name="connection">Informaiton about the connection.</param>
        public override void Connected(BoltConnection connection)
        {
            ClearJoinButtons();
            m_lobbyPanel.SetActive(true);
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Updates the list of sessions broadcasted via LAN
        /// </summary>
        /// <param name="sessionList">List containing all available sessions.</param>
        public override void SessionListUpdated(Map<Guid, UdpSession> sessionList)
        {
            ClearJoinButtons();
            InstantiateSessionButtons();
        }
        #endregion

        #region UI Events
        /// <summary>
        /// Set the player's information and store the name inside the PlayerPrefs.
        /// Starts the Bolt Client.
        /// </summary>
        public void DirectConnect()
        {
            PlayerInformation.myPlayer.playerName = m_playerNameInput.text;
            PlayerPrefs.SetString("playerNameKey", m_playerNameInput.text);
            BoltNetwork.Connect(new UdpEndPoint(UdpIPv4Address.Parse(m_ipInput.text), (ushort)m_serverPort));
        }

        /// <summary>
        /// Set the player's information and store the name inside the PlayerPrefs.
        /// This event is triggered on submitting the input of the input field.
        /// </summary>
        public void OnNameSubmit()
        {
            PlayerInformation.myPlayer.playerName = m_playerNameInput.text;
            PlayerPrefs.SetString("playerNameKey", m_playerNameInput.text);
        }

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
        /// Turns off Bolt.
        /// </summary>
        public void OnCancel()
        {
            ClearJoinButtons();
            BoltLauncher.Shutdown();
        }
        #endregion

        #region Local Functions
        /// <summary>
        /// Instantiates sessions buttons.
        /// </summary>
        private void InstantiateSessionButtons()
        {
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
        /// Destroys all join buttons.
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