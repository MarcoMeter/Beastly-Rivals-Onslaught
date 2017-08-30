using System.Collections.Generic;
using UnityEngine;
using BRO.AI;

namespace BRO.Game.PreMatch
{
    /// <summary>
    /// The player registration service is in charge of establishing and mainting a list of all players, especially during the lobby to get everything prepared for the actual match.
    /// It also references the player's spawn points.
    /// </summary>
    public class PlayerRegistrationService : Bolt.EntityEventListener<IGameControllerState>
    {
        #region Member Fields
        private int m_playerLifes = 2;
        private BoltConnection[] m_playerConnections = new BoltConnection[8];
        private PlayerIdAssigner m_idAssigner = new PlayerIdAssigner();
        private Vector3[] m_spawnPoints = new Vector3[8];
        private List<AIBaseTupel> m_aiTupels = new List<AIBaseTupel>();
        private Color[] m_playerColors = new Color[]
        {
            Color.red,
            Color.blue,
            Color.green,
            Color.yellow,
            Color.magenta,
            new Color(255.0f/255.0f,120.0f/255.0f,0.0f/255.0f),
            new Color(15.0f/255.0f,25.0f/255.0f,25.0f/255.0f),
            Color.white
        };
        #endregion

        #region Unity Lifecycle
        /// <summary>
        /// Disables this component on the client. The game controller is executed on the server. It also initializes the spawn points.
        /// </summary>
        void Awake()
        {
            if (!BoltNetwork.isServer)
                enabled = false;

            // spawnPoints for all eight players
            m_spawnPoints[0] = new Vector3(0, 0, 25f);
            m_spawnPoints[1] = new Vector3(24.5f, 0, 19.5f);
            m_spawnPoints[2] = new Vector3(35f, 0, 0);
            m_spawnPoints[3] = new Vector3(24.5f, 0, -19.5f);
            m_spawnPoints[4] = new Vector3(0f, 0, -25f);
            m_spawnPoints[5] = new Vector3(-24.5f, 0, -19.5f);
            m_spawnPoints[6] = new Vector3(-35f, 0, 0);
            m_spawnPoints[7] = new Vector3(-24.5f, 0, 19.5f);
        }

        /// <summary>
        /// Initializes important members and the player list's properties.
        /// Registers a host player if host has player.
        /// </summary>
        void Start()
        {
            // Initialize state
            state.playersConnected = 0; // On Initialization there are no players connected (well, the host is, but doesn't have a player yet).
            for (int i = 0; i < state.players.Length; i++)
            {
                ResetPlayerState(i);
            }
        }
        #endregion

        #region Getter/Setter
        /// <summary>
        /// Sets the life count of all players.
        /// </summary>
        /// <param name="lifeCount">Specifies the number of lifesö</param>
        public void SetPlayerLifes(int lifeCount)
        {
            m_playerLifes = lifeCount;

            for (int i = 0; i < state.players.Length; i++) // update each player
            {
                state.players[i].lifes = lifeCount;
            }
        }

        /// <summary>
        /// Getter for the number of lifes of the players.
        /// </summary>
        /// <returns>Returns the life count of all players before the match.</returns>
        public int GetPlayerLifes()
        {
            return m_playerLifes;
        }
        #endregion

        #region Player re/deregister Functions
        /// <summary>
        /// At first the connection of the new client gets related to an ID, which gets forwarded to the client.
        /// </summary>
        /// <param name="connection"></param>
        public void RegisterPlayerConnection(BoltConnection connection)
        {
            int newPlayerId = m_idAssigner.RequestPlayerId(); // obtain free ID
            PlayerInformation.playerConnections[newPlayerId] = connection; // store id and connection information
            m_playerConnections[newPlayerId] = connection;

            var ev = AssignPlayerID.Create(connection); // raise event to distribute the assigned ID to the connected connection
            ev.playerId = newPlayerId;
            ev.Send();
            // In comparison to the host, the EnrollPlayerName Event is fired by the client
        }

        /// <summary>
        /// As soon as the client received his ID, he sends his name for being enrolled to the player's list.
        /// </summary>
        /// <param name="evnt">Event data which contains the player's ID and name</param>
        public void RegisterPlayer(EnrollPlayerName evnt)
        {
            state.players[evnt.playerId].playerName = evnt.name;
            state.players[evnt.playerId].playerId = evnt.playerId;
            state.players[evnt.playerId].available = true;
            state.playersConnected++;
            RaiseChatMessage(evnt.name + " entered the lobby!"); // Raise a chat message to introduce the new player to the lobby
        }

        /// <summary>
        /// Register non-human players (AIs) to the match.
        /// </summary>
        public void RegisterAiPlayer(AIBase aiBase)
        {
            if (state.playersConnected < 8) // if there is an open player slot, Ai can be added
            {
                int newAiId = m_idAssigner.RequestPlayerId();
                PlayerInformation.playerConnections[newAiId] = null; // AIs don't have connections like the host
                AIInformation aiInfo = aiBase.GetAIInformation(); // retrieve information about the AI

                state.players[newAiId].playerName = aiInfo.GetName() + " " + newAiId;
                state.players[newAiId].playerId = newAiId;
                state.players[newAiId].lobbyReady = true;
                state.players[newAiId].available = true;
                state.players[newAiId].isAi = true;
                state.playersConnected++;
                RaiseChatMessage(aiInfo.GetName() + " " + newAiId + " entered the lobby!");

                // save AIBase and id information as tupel, so that the AI Player GameObject can retrieve it's information by using GetAIBase(int playerId)
                AIBaseTupel tupel;
                tupel.aiBase = aiBase;
                tupel.aiId = newAiId;
                m_aiTupels.Add(tupel);
            }
        }

        /// <summary>
        /// If a player disconnected or got kicked, this function deregisters the player.
        /// </summary>
        /// <param name="connection">The connection of the disconnected player.</param>
        public void DeregisterPlayer(BoltConnection connection)
        {
            for (int i = 0; i < state.playersConnected; i++)
            {
                if (state.players[i].available && connection == PlayerInformation.playerConnections[i]) // find the disconnected player's id
                {
                    RaiseChatMessage(state.players[i].playerName + " left the lobby."); // inform the lobby via chat message event
                    ResetPlayerState(i); // reset the state of the left player
                    m_idAssigner.FreePlayerId(i); // free the player's id
                }
            }
            state.playersConnected--;
        }

        /// <summary>
        /// Kicks and disconnects a player from the match during the lobby. This is triggered by an OnCliock event of the lobby.
        /// </summary>
        /// <param name="toKickId">The id of the player who is supposed to be kicked.</param>
        public void DeregisterPlayerKicked(int toKickId)
        {
            if (state.players[toKickId].available) // can't kick nothing
            {
                if (!state.players[toKickId].isAi)
                {
                    m_playerConnections[toKickId].Disconnect(); // ServerCallbackMainMenu listens to Disconnected and triggers DeregisterPlayer here
                }
                else
                {
                    DeregisterAiPlayer(toKickId); // can't disconnect ai
                }
            }
        }

        /// <summary>
        /// Register the host's player to the player list.
        /// </summary>
        public void RegisterHostPlayer()
        {
            int newPlayerId = m_idAssigner.RequestPlayerId();

            PlayerInformation.myPlayer.id = newPlayerId;
            state.players[newPlayerId].playerName = PlayerInformation.myPlayer.playerName;
            state.players[newPlayerId].playerId = newPlayerId;
            state.players[newPlayerId].available = true;
            state.playersConnected++;
            state.serverHasPlayer = true;
        }

        /// <summary>
        /// Deregisters a non-human player (AI) and frees its ID. It als takes care to remove the AIBaseTupel from the list.
        /// </summary>
        /// <param name="aiId">ID of the AI player</param>
        private void DeregisterAiPlayer(int aiId)
        {
            // Find and remove AIBase tupel
            foreach (var tupel in m_aiTupels)
            {
                if (aiId == tupel.aiId)
                {
                    m_aiTupels.Remove(tupel);
                    break;
                }
            }

            RaiseChatMessage(state.players[aiId].playerName + " left the lobby.");
            ResetPlayerState(aiId);
            m_idAssigner.FreePlayerId(aiId);
            state.playersConnected--;
        }
        #endregion

        #region Local Functions
        /// <summary>
        /// Re-initializes a player's information inside the player list.
        /// </summary>
        /// <param name="id">ID of the player who has to be reinitialized.</param>
        private void ResetPlayerState(int id)
        {
            state.players[id].playerName = null;
            state.players[id].lifes = m_playerLifes;
            state.players[id].kills = 0;
            state.players[id].lobbyReady = false;
            state.players[id].matchReady = false;
            state.players[id].hasBall = false;
            state.players[id].isGameOver = true;
            state.players[id].blinkCoolDown = 15f;
            state.players[id].blinkOnCooldown = false;
            state.players[id].available = false;
            state.players[id].hasKilled = false;
            state.players[id].playerGameState = (int)PlayerState.MatchStartState;
            state.players[id].isAi = false;
            state.players[id].spawnPoint = m_spawnPoints[id];
            state.players[id].playerColor = m_playerColors[id];
        }

        /// <summary>
        /// Raise a chat message event by the server.
        /// </summary>
        /// <param name="message">Content of the chat message.</param>
        private void RaiseChatMessage(string message)
        {
            var ev = ChatMessageEvent.Create(Bolt.GlobalTargets.Everyone);
            ev.message = message;
            ev.sender = "Server";
            ev.Send();
        }
        #endregion

        #region Public Functions
        /// <summary>
        /// On player request, this function returns the AIBase instance related to the player id.
        /// </summary>
        /// <param name="playerId">The requesting AI player's ID</param>
        /// <returns>Returns the player related AIBase</returns>
        public AIBase GetAIBase(int playerId)
        {
            foreach (var tupel in m_aiTupels)
            {
                if (tupel.aiId == playerId)
                {
                    return tupel.aiBase;
                }
            }
            return null;
        }
        #endregion
    }

    /// <summary>
    /// Stores the AI player's id and relates it to the desired AIBase, which consists of its AI Logic for the actual match.
    /// </summary>
    struct AIBaseTupel
    {
        public AIBase aiBase;
        public int aiId;
    }
}