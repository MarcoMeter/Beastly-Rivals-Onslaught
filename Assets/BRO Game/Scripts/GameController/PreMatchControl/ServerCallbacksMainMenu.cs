using Bolt;
using UdpKit;
using UnityEngine;

namespace BRO.Game.PreMatch
{
    /// <summary>
    /// This script, which is attached to the GameController, listens to Bolt Events and custom events which occur during the MainMenu on the server side.
    /// </summary>
    public class ServerCallbacksMainMenu : Bolt.GlobalEventListener
    {
        #region Member Fields
        private IGameControllerState m_gmcState;
        private PlayerRegistrationService m_playerRegistration;
        private PlayerReadyController m_playerReadyController;
        private const int m_MAXPLAYERS = 8;
        #endregion

        #region Unity Lifecycle
        /// <summary>
        /// Disables this component on the client. The game controller is executed on the server.
        /// Gathers the instances of the GameControllerState, PlayerRegistrationService and PreMatchPlayerManagement.
        /// </summary>
        void Start()
        {
            if (!BoltNetwork.isServer)
                enabled = false;

            m_gmcState = GameController.Instance.state;
            m_playerRegistration = GetComponent<PlayerRegistrationService>();
            m_playerReadyController = GetComponent<PlayerReadyController>();
        }
        #endregion

        #region Bolt Events
        // Manually refuse and accept connections
        /// <summary>
        /// ConnectRequest has to be enabled inside the bolt settings. This allows a custom logic for accepting or declining incoming connections.
        /// </summary>
        /// <param name="endpoint">The endpoint of the client's connection</param>
        /// <param name="token">Currently no token is sent during this event</param>
        public override void ConnectRequest(UdpEndPoint endpoint, IProtocolToken token)
        {
            if (m_gmcState.playersConnected < m_MAXPLAYERS)
            {
                BoltNetwork.Accept(endpoint);
            }
            else
            {
                BoltNetwork.Refuse(endpoint);
            }

        }

        /// <summary>
        /// First handshake betweem Server and client. it tirggers the id assignment functionality.
        /// </summary>
        /// <param name="connection">Connection of the connected player</param>
        public override void Connected(BoltConnection connection)
        {
            m_playerRegistration.RegisterPlayerConnection(connection);
            m_playerReadyController.InterruptGameLaunchProcess();
        }

        /// <summary>
        /// Deregister a player who left or got kicked
        /// </summary>
        /// <param name="connection">The player's connection</param>
        public override void Disconnected(BoltConnection connection)
        {
            m_playerRegistration.DeregisterPlayer(connection);
            m_playerReadyController.InterruptGameLaunchProcess(true);
        }
        #endregion

        #region Custom Bolt Events
        /// <summary>
        /// Receives the player's name and triggers the official player enrollment process. This event is raised after the client received his ID.
        /// </summary>
        /// <param name="evnt">Event data which carries the player's name</param>
        public override void OnEvent(EnrollPlayerName evnt)
        {
            m_playerRegistration.RegisterPlayer(evnt);
        }

        /// <summary>
        /// If a player clicked ready button inside the lobby, the PreMatchPlayerManagement will assign the ready status to the player's information.
        /// </summary>
        /// <param name="evnt"></param>
        public override void OnEvent(PlayerReadyLobbyEvent evnt)
        {
            m_playerReadyController.SetPlayerLobbyReady(evnt.playerId);
        }
        #endregion
    }
}