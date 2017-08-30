using UnityEngine;
using UdpKit;
using UnityEngine.UI;

namespace BRO.Game.PreMatch
{
    /// <summary>
    /// UI logic for the LaunchServerPanel.
    /// </summary>
    public class LaunchServerPanel : Bolt.GlobalEventListener
    {
        #region Member Fields
        [SerializeField]
        private int m_serverPort = 4000;
        [SerializeField]
        private InputField m_gameNameInput;
        [SerializeField]
        private InputField m_playerNameInput;
        [SerializeField]
        private GameObject m_lobbyPanel;
        [SerializeField]
        private string m_defaultGameName = "BRO Session";
        private bool m_useZeus = false; // use Bolt's master server for online match making, if true
        #endregion

        #region Unity Lifecycle
        /// <summary>
        /// Initialize members
        /// </summary>
        void Start()
        {
            // If a player's name has been stored, load it.
            m_playerNameInput.text = PlayerPrefs.GetString("playerNameKey");
        }
        #endregion

        #region UI Events
        /// <summary>
        /// Hosts a match for the local network and adds the host's player information.
        /// </summary>
        public void HostLAN()
        {
            PlayerInformation.myPlayer.playerName = m_playerNameInput.text; // Name of the Host's player.
            PlayerPrefs.SetString("playerNameKey", m_playerNameInput.text); // The name gets stored within the PlayerPrefs for reuse.
            BoltLauncher.StartServer(new UdpEndPoint(UdpIPv4Address.Any, (ushort)m_serverPort)); // Starts the Bolt Server.
            m_useZeus = false; // Zeus is not used for LAN matches.
            DisableChildren();
        }

        /// <summary>
        /// Hosts an online match and adds the host's player information.
        /// </summary>
        public void HostWAN()
        {
            PlayerInformation.myPlayer.playerName = m_playerNameInput.text; // Name of the Host's player
            PlayerPrefs.SetString("playerNameKey", m_playerNameInput.text);
            BoltLauncher.StartServer();
            m_useZeus = true;
            DisableChildren();
        }
        #endregion

        #region Public Functions
        #endregion

        #region Bolt Events
        /// <summary>
        /// If Bolt started appropiately, open the lobby panel and instnatiate the GameController.
        /// </summary>
        public override void BoltStartDone()
        {
            if (!m_useZeus)
            {
                //BoltNetwork.EnableLanBroadcast();
                SetHostInfo();
            }
            
            // The server instatiates the Bolt Entity GameController
            BoltNetwork.Instantiate(BoltPrefabs.GameController);
            // Activate lobby if Bolt started
            m_lobbyPanel.SetActive(true);

            if(!m_useZeus)
            {
                gameObject.SetActive(false);
            }
            else
            {
                Bolt.Zeus.Connect();
            }
        }

        /// <summary>
        /// If ZeusConnected got triggered, the host info has to be passed to Zeus.
        /// </summary>
        /// <param name="endpoint"></param>
        public override void ZeusConnected(UdpEndPoint endpoint)
        {
            // If the match has to take place across the internet, set the Host Info for Zeus
            if (m_useZeus)
            {
                SetHostInfo();
            }
            gameObject.SetActive(false);
        }

        /// <summary>
        /// If Bolt did not start appropiately, post an error message.
        /// </summary>
        public override void BoltStartFailed()
        {
            transform.root.GetComponent<MainMenuCanvas>().PostMessage("Bolt Start failed", "Bolt wasn't able to launch the server.");
            m_lobbyPanel.SetActive(false);
        }
        #endregion

        #region Local Functions
        /// <summary>
        /// Sets the information of the to be hosted game.
        /// This concerns LanBroadcasting and Zeus.
        /// </summary>
        private void SetHostInfo()
        {
            if (m_gameNameInput.text.Length > 0)
            {
                BoltNetwork.SetHostInfo(m_gameNameInput.text, null);
            }
            else
            {
                BoltNetwork.SetHostInfo(m_defaultGameName, null);
            }
        }

        /// <summary>
        /// Disables the children of the LaunchServerPanel.
        /// </summary>
        private void DisableChildren()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
        }
        #endregion
    }
}