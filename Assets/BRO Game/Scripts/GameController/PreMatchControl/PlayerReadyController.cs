using System.Collections;
using UnityEngine;

namespace BRO.Game.PreMatch
{
    /// <summary>
    /// The PlayerReadyController keeps track of the players being ready or not. If all players are readey, the PlayerReadyController is supposed to load the CoreGame scene.
    /// </summary>
    public class PlayerReadyController : Bolt.EntityEventListener<IGameControllerState>
    {
        #region Member Fields
        private Coroutine m_launchRoutine;          // Stores a references of the game launch process (countdown), so that it can be stopped externally.
        private bool m_isLaunching = false;
        private const string m_CORE_MATCH_SCENE_NAME = "CoreMatch";
        #endregion

        #region Unity Lifecycle
        /// <summary>
        /// Disables this component on the client. The game controller is executed on the server.
        /// </summary>
        void Awake()
        {
            if (!BoltNetwork.isServer)
                enabled = false;
        }
        #endregion

        #region Public Functions
        /// <summary>
        /// This event is triggered if a client clicked on the ready button inside the lobby. This will update his ready status.
        /// Upon this event, it is checked if all players are ready. If so, the game will commence.
        /// </summary>
        /// <param name="playerId">The ID of the player, who clicked ready.</param>
        public void SetPlayerLobbyReady(int playerId)
        {
            // set everybody else ready
            if (state.players[playerId].lobbyReady)
            {
                state.players[playerId].lobbyReady = false;
            }
            else if (!state.players[playerId].lobbyReady)
            {
                state.players[playerId].lobbyReady = true;
            }

            // Meanwhile the server is checking, if players are ready. If all available players are ready, the server loads the game level.
            int availablePlayers = 0;
            int readyPlayers = 0;
            for (int i = 0; i < state.players.Length; i++)
            {
                if (state.players[i].available)
                {
                    availablePlayers++;
                    if (state.players[i].lobbyReady)
                    {
                        readyPlayers++;
                    }
                }
            }

            // if all players are ready, start the launch countdown
            if (availablePlayers > 1 && availablePlayers == readyPlayers)
            {
                m_launchRoutine = StartCoroutine(LaunchGameCountdown());
                m_isLaunching = true;
            }
            // if someone is not ready anymore, stop the launch process
            else
            {
                InterruptGameLaunchProcess();
            }

            availablePlayers = readyPlayers = 0;
        }
        
        /// <summary>
        /// If a player connected, disconnneted or is not ready anymore, stop the launching process.
        /// </summary>
        public void InterruptGameLaunchProcess()
        {
            if (m_isLaunching && m_launchRoutine != null)
            {
                StopCoroutine(m_launchRoutine);
                m_isLaunching = false;
                RaiseChatMessageEvent("Launch Aborted.", "Server");
            }
        }

        /// <summary>
        /// If a player connected, disconnneted or is not ready anymore, stop the launching process.
        /// </summary>
        public void InterruptGameLaunchProcess(bool disconnected)
        {
            InterruptGameLaunchProcess();

            // Set players back to not ready if someone disconnected
            if(disconnected)
            {
                foreach(var player in state.players)
                {
                    if(!player.isAi)
                        player.lobbyReady = false;
                }
            }
        }
        #endregion

        #region Local Functions
        /// <summary>
        /// This Coroutine uses a countdown to load the CoreMatch scene.
        /// </summary>
        /// <returns></returns>
        IEnumerator LaunchGameCountdown()
        {

            yield return new WaitForSeconds(1);
            RaiseChatMessageEvent("LAUNCHING GAME !", "Server");
            yield return new WaitForSeconds(1);
            RaiseChatMessageEvent(". . . 3", "Server");
            yield return new WaitForSeconds(1);
            RaiseChatMessageEvent(". . . 2", "Server");
            yield return new WaitForSeconds(1);
            RaiseChatMessageEvent(". . . 1", "Server");
            yield return new WaitForSeconds(1);

            // Config and execute scene loading
            LoadingFlags flags = new LoadingFlags() { Local = false, ShowInfo = true, UseFade = true, UseLoadingScreen = true, WaitForInput = true };
            LoadSceneManager.Instance.LoadScene(m_CORE_MATCH_SCENE_NAME, flags);
        }

        /// <summary>
        /// Creates a ChatMessageEvent for the countdown.
        /// </summary>
        /// <param name="message">Content of the chat message</param>
        /// <param name="sender">Sender of the chat message</param>
        void RaiseChatMessageEvent(string message, string sender)
        {
            var ev = ChatMessageEvent.Create(Bolt.GlobalTargets.Everyone);
            ev.sender = sender;
            ev.message = message;
            ev.Send();
        }
        #endregion
    }
}