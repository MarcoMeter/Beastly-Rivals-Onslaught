using BRO.AI.Framework.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;  

namespace BRO.Game
{
    /// <summary>
    /// The MatchFlowControl component is in charge of starting the match logics and to keep them going until someone has won.
    /// </summary>
    public class MatchFlowControl : Bolt.EntityEventListener<IGameControllerState>
    {
        #region Member Fields
        [SerializeField]
        private float m_startDelay = 5f;                                    // The time (seconds), which have to pass, before the match initially commences
        
        private BoltEntity m_ball;                                          // The entity of the ball (can be null)
        private BeastBehaviour m_beastBehavior;
        [SerializeField]
        private PowerShotControl m_powerShotControl;
        [SerializeField]
        private Vector3 m_beastSpawnLocation = new Vector3(0, 0.5f, 0);
        [SerializeField]
        private Vector3 m_initialBallSpawn = new Vector3(0, 8f, 0);

        private List<int> m_remainingPlayers = new List<int>();             // This list is used to let the MatchFlowControl know which player is still participating/alive

        private bool m_infiniteLives = false;                               // This can be set by the match automation to ensure, that a single match will not conclude
        #endregion

        #region Member Properties
        /// <summary>
        /// Called by the Match Automation Manager to prevent players from loosing lives at all.
        /// </summary>
        public bool InfiniteLives
        {
            get { return m_infiniteLives; }
            set { m_infiniteLives = value; }
        }
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
        /// Informs the MatchFlowControl that the players are ready to start playing.
        /// </summary>
        public void StartMatch()
        {
            state.gameState = (int)MatchState.MatchStartState;
            StartCoroutine(LaunchInitialEntities());
        }

        /// <summary>
        /// Inform the MatchFlowControl, that the Beast killed a player.
        /// </summary>
        /// <param name="playerId">The id of the player who got killed by the beast</param>
        public void PlayerKilledByBeast(int playerId)
        {
            // Update game state and trigger player revival
            state.players[playerId].playerGameState = (int)PlayerState.DeadState;
            state.gameState = (int)MatchState.PlayerDeadState;
            state.players[playerId].hasBall = false;
            state.ballCarrier = null;
            if (state.players[playerId].lifes > 0)
            {
                if(!m_infiniteLives)
                    state.players[playerId].lifes--;

                StartCoroutine(TriggerPlayerRevival(playerId));
            }
            else if (state.players[playerId].lifes == 0)
            {
                state.players[playerId].isGameOver = true;
                m_remainingPlayers.RemoveAt(m_remainingPlayers.IndexOf(playerId));
                state.remainingPlayerCount = m_remainingPlayers.Count;
                if (m_remainingPlayers.Count > 1)
                {
                    StartCoroutine(ContinuoMatch());
                }
                else // this is the case of the game being done, the winner is then determined and the states get set accordingly
                {
                    ConcludeMatch(m_remainingPlayers[0]);
                }
            }

            // Determine the killer, update the game state and raise AI event
            if (state.lastPassByPlayer > -1)
            {
                state.players[state.lastPassByPlayer].hasKilled = true;
                state.players[state.lastPassByPlayer].kills++;  // increase player kill score

                // Raise AI Event
                var aiEvent = new PlayerKilledEvent();
                aiEvent.Killer = state.lastPassByPlayer;
                aiEvent.Victim = playerId;
                aiEvent.Humilated = false;
                aiEvent.Send();
            }
            else
            {
                // Raise AI Event
                var aiEvent = new PlayerKilledEvent();
                aiEvent.Humilated = true;
                aiEvent.Victim = playerId;
                aiEvent.Send();

                if (!m_infiniteLives)
                    state.players[playerId].kills--;            // being humilated decreases your kill count by one
            }
        }
        #endregion

        #region Local Functions
        /// <summary>
        /// Launches the first entities of the game starting with the beast and then instantiates the ball.
        /// </summary>
        /// <returns>yield wait for seconds</returns>
        private IEnumerator LaunchInitialEntities()
        {
            // wait one second, so that everything is ready, like the scripts of the AI players
            yield return new WaitForSeconds(1f);
            InitializeRemainingPlayerList();

            // Spawn Beast
            state.beastEntity = BoltNetwork.Instantiate(BoltPrefabs.Beast, m_beastSpawnLocation, Quaternion.identity);
            m_beastBehavior = state.beastEntity.GetComponent<BeastBehaviour>();
            // Raise AI Event: Trigger start match event on the AIs logic components
            new MatchStartEvent().Send();
            yield return new WaitForSeconds(m_startDelay);
            m_beastBehavior.IdleAgressive();

            // Launch ball and pass it to a random player
            int randomPlayerId = RandomPlayer();
            PassBall(true, state.players[randomPlayerId].playerEntity, null, randomPlayerId, -1);
            yield return new WaitForSeconds(m_startDelay);
        }

        /// <summary>
        /// Initialize remainingPlayer list based on playerIds.
        /// </summary>
        private void InitializeRemainingPlayerList()
        {
            for (int i = 0; i < state.players.Length; i++)
            {
                if (!state.players[i].isGameOver && state.players[i].matchReady)
                {
                    m_remainingPlayers.Add(state.players[i].playerId);
                    state.players[i].playerGameState = (int)PlayerState.AliveState;
                }
                else
                {
                    state.players[i].isGameOver = true;
                }
            }
            state.remainingPlayerCount = m_remainingPlayers.Count;
        }

        /// <summary>
        /// Brings the ball back into the game and resets the beast properties.
        /// </summary>
        private void RelaunchEntities()
        {
            // Update game state
            state.gameState = (int)MatchState.PlayerAliveState;

            // Reset beast properties
            m_beastBehavior.ResetBeast();

            // Launch the ball and pass it to a random player
            int randomPlayerId = RandomPlayer();
            PassBall(true, state.players[randomPlayerId].playerEntity, null, randomPlayerId, -1);
        }

        /// <summary>
        /// Affects the beast and the game state.
        /// Relaunches the entities to move on with the flow of the match.
        /// </summary>
        /// <returns>Delay purposes</returns>
        private IEnumerator ContinuoMatch()
        {
            yield return new WaitForSeconds(3f);
            state.gameState = (int)MatchState.PlayerReviveState;
            m_beastBehavior.Idle();
            yield return new WaitForSeconds(3f);
            RelaunchEntities();
        }

        /// <summary>
        /// Triggers the revive process of the player who just got killed and has lifes left.
        /// </summary>
        /// <param name="playerId">The player to revive</param>
        /// <returns>Is used to wait for seconds</returns>
        private IEnumerator TriggerPlayerRevival(int playerId)
        {
            yield return new WaitForSeconds(3f);
            m_beastBehavior.Idle();
            state.gameState = (int)MatchState.PlayerReviveState;
            state.players[playerId].playerGameState = (int)PlayerState.ReviveState;
            yield return new WaitForSeconds(3f);
            RevivePlayer.Create(state.players[playerId].playerEntity).Send();
        }

        /// <summary>
        /// ConcludeMatch determines the winner and raises events for that purpose.
        /// </summary>
        /// <param name="winnerId">The id of the player whon won.</param>
        private void ConcludeMatch(int winnerId)
        {
            // Finalize GameState
            state.players[winnerId].isWinner = true;
            state.players[winnerId].isGameOver = true;
            state.gameState = (int)MatchState.MatchEndState;
            // Raise Bolt Event
            var ev = WinnerEvent.Create();
            ev.winnerId = winnerId;
            ev.winnerName = state.players[winnerId].playerName;
            ev.Send();
            // Raise AI Event
            var aiEvent = new MatchDoneEvent();
            aiEvent.Winner = winnerId;
            aiEvent.Send();
        }

        /// <summary>
        /// Triggers the passing of the ball.
        /// </summary>
        /// <param name="initialBall">States if the ball is launched by the server</param>
        /// <param name="targetPlayer">The entity of the player who is targeted</param>
        /// <param name="actingPlayer">The player who is passing the ball. This should be null if the server launches the ball</param>
        /// <param name="targetId">The id of the target player</param>
        /// <param name="actingPlayerId">The id of the acting player who passes the ball. This should be -1 if the server passes the ball.</param>
        private void PassBall(bool initialBall, BoltEntity targetPlayer, BoltEntity actingPlayer, int targetId, int actingPlayerId)
        {
            Vector3 ballSpawnLocation = m_initialBallSpawn;

            // Update game state info
            if (initialBall)
            {
                if (state.lastPassByPlayer > -1)
                    state.players[state.lastPassByPlayer].hasKilled = false;
                state.lastPassByPlayer = actingPlayerId;
            }
            else
            {
                // Override the spawn location to fit the acting player's throwing position
                ballSpawnLocation = new Vector3(actingPlayer.transform.position.x, 8f, actingPlayer.transform.position.z);
                state.players[actingPlayerId].hasBall = false;
                state.lastPassByPlayer = actingPlayerId;
            }

            // Instantiate the ball
            m_ball = BoltNetwork.Instantiate(BoltPrefabs.Ball, ballSpawnLocation, Quaternion.identity);

            // Pass the ball normally, or as Power Shot
            if (!state.isPowerShot)
            {
                m_ball.GetComponent<BallBehaviour>().SetTarget(targetPlayer.transform);
                m_beastBehavior.SetTarget(targetPlayer);
            }
            else
            {
                m_ball.GetComponent<BallBehaviour>().SetPowerShotCharge(targetPlayer.transform, m_powerShotControl.PowerShotCharge);
                m_beastBehavior.SetPowerShotTarget(targetPlayer, m_powerShotControl.PowerShotCharge);
            }

            // Raise AI Event
            var aiEvent = new BallPassedEvent();
            if (actingPlayer)
            {
                aiEvent.Source = actingPlayerId;
            }
            aiEvent.InitalBall = initialBall;
            aiEvent.Target = targetId;
            aiEvent.IsPowerShot = state.isPowerShot;
            aiEvent.PowerShotCharge = m_powerShotControl.PowerShotCharge;
            aiEvent.Send();

            // Raise Sound Event
            var soundEvent = PlaySoundEvent.Create();
            // Play the end of kamehameha if PowerShot
            if (!state.isPowerShot)
            {
                soundEvent.audioClipIndex = (int)SoundClip.PassBall;
            }
            else
            {
                soundEvent.audioClipIndex = (int)SoundClip.PowerShotEnd;
            }
            soundEvent.Send();
        }

        /// <summary>
        /// Randomizing a player.
        /// </summary>
        /// <returns>Returns a random player ID of the remaining players.</returns>
        private int RandomPlayer()
        {
            return m_remainingPlayers[Random.Range(0, m_remainingPlayers.Count)];
        }
        #endregion

        #region Bolt Events
        /// <summary>
        /// This event is received as soon as a Player was brought back to life.
        /// </summary>
        /// <param name="evnt">Holds the ID of the player who was just revived</param>
        public override void OnEvent(PlayerAliveEvent evnt)
        {
            state.players[evnt.playerId].playerGameState = (int)PlayerState.AliveState;
            RelaunchEntities();
        }

        /// <summary>
        /// This event is raised by players who pass the ball in order to actually instantiate the ball and to make the beast chase.
        /// </summary>
        /// <param name="evnt">Event information including information about the acting and the target player.</param>
        public override void OnEvent(PassBallEvent evnt)
        {
            if (state.players[evnt.playerId].hasBall)
            {
                // Instantiate and pass the ball
                PassBall(false, evnt.targetPlayer, evnt.actingPlayer, evnt.targetId, evnt.playerId);
                state.ballCarrier = null;
            }
        }
        #endregion
    }
}