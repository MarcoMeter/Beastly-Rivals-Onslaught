using UnityEngine;
using BRO.Game;
using UnityEngine.AI;

namespace BRO.AI.Framework
{
    /// <summary>
    /// This class reveals information about the AI's controller player. This class provides actions to the AI.
    /// </summary>
    [RequireComponent(typeof(AIController))]
    public class AIBehaviour : MonoBehaviour, IInitAI
    {
        #region Member Fields
        private int m_myId;
        private Player m_myPlayer;
        private PlayerAliveState m_aliveState;                // This component executes actions such as movement and blinking
        private IGameControllerState m_gmcState;                // The actual networked state is used to find the BoltEntities of the participating players
        private const float m_MAX_NAVMESH_SAMPLE_RANGE = 100f;
        #endregion

        #region Member Properties
        /// <summary>
        /// The player's information
        /// Read-only
        /// </summary>
        protected Player MyPlayer
        {
            get { return m_myPlayer; }
        }

        /// <summary>
        /// The id of this player
        /// Read-only
        /// </summary>
        protected int MyId
        {
            get { return m_myId; }
        }
        #endregion

        #region Interface IInitAI
        /// <summary>
        /// Initializes members of the AIBehaviour.
        /// </summary>
        public virtual void Init()
        {
            m_aliveState = GetComponent<PlayerAliveState>();
            m_myId = GetComponent<PlayerId>().Id;
            m_gmcState = GameController.Instance.state;
        }

        /// <summary>
        /// MyPlayerUpdate gathers the player's information
        /// </summary>
        public virtual void MyPlayerUpdate()
        {
            if (m_gmcState.gameState != (int)MatchState.PreMatchState &&
                m_gmcState.gameState != (int)MatchState.MatchEndState)
            {
                if (m_gmcState.beastEntity != null)
                {
                    m_myPlayer = GameState.GetPlayer(m_myId);
                }
            }
        }
        #endregion

        #region AI Actions
        /// <summary>
        /// Action: Telepots the player to a target location
        /// </summary>
        /// <param name="destination">The destination location</param>
        protected bool Blink(Vector3 destination)
        {
            if (MyPlayer.BlinkOnCooldown)
            {
                return false;
            }
            else
            {
                // Sample the desired movement destination in order to clamp it to a valid position located on the NavMesh
                NavMeshHit hit;
                bool legitHit = NavMesh.SamplePosition(destination, out hit, m_MAX_NAVMESH_SAMPLE_RANGE, NavMesh.AllAreas);
                if (legitHit)
                {
                    m_aliveState.Blink(hit.position);
                    return true;
                }
                else
                {
                    Debug.LogWarning("AI Player: False destination for blinking.");
                    return false;
                }
            }
        }

        /// <summary>
        /// Action: Starts moving the player towards the destination
        /// </summary>
        /// <param name="destination">The destination to move to</param>
        protected bool Move(Vector3 destination)
        {
            // Sample the desired movement destination in order to clamp it to a valid position located on the NavMesh
            NavMeshHit hit;
            bool legitHit = NavMesh.SamplePosition(destination, out hit, m_MAX_NAVMESH_SAMPLE_RANGE, NavMesh.AllAreas);

            if (legitHit)
            {
                if (MyPlayer.State == PlayerState.AliveState)
                {
                    m_aliveState.MoveForward(hit.position);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                Debug.LogWarning("AI Player: False destination for moving.");
                return false;
            }
        }

        /// <summary>
        /// Action: Stops the movement of the player
        /// </summary>
        protected void StopMove()
        {
            m_aliveState.StopAgent();
        }

        /// <summary>
        /// Action: While possessing the ball, the player can throw the ball to another player
        /// </summary>
        /// <param name="targetPlayerId">The target player</param>
        protected bool PassBall(int targetPlayerId)
        {
            if (MyPlayer.HasBall)
            {
                m_aliveState.PassBallToPlayer(m_gmcState.players[targetPlayerId].playerEntity);
                return true;
            }
            else
            {
                Debug.LogWarning("AI Player: Not in possession of the ball to pass it.");
                return false;
            }
        }

        /// <summary>
        /// Action: If the player scored a kill, a taunt animation can be played to make fun of the victim.
        /// </summary>
        /// <param name="tauntId">The taunt animation to play (id shall be between 1 and 8)</param>
        protected void Taunt(int tauntId)
        {
            if (tauntId >= 1 && tauntId <= 8)
            {
                m_aliveState.PlayTauntAnimation(tauntId);
            }
        }
        #endregion
    }
}