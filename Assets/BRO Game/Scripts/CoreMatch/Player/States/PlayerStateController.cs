using UnityEngine;

namespace BRO.Game
{
    /// <summary>
    /// The PlayerStateController is in charge of managing the states of the players. Each state contains distintive functionalities which are required for each state.
    /// </summary>
    public class PlayerStateController : Bolt.EntityEventListener<IPlayerState>
    {
        #region Member Fields
        // State Components
        [SerializeField]
        private PlayerReadyState m_playerReadyState;
        [SerializeField]
        private PlayerAliveState m_playerAliveState;
        [SerializeField]
        private PlayerDeadState m_playerDeadState;
        [SerializeField]
        private PlayerReviveState m_playerReviveState;
        [SerializeField]
        private PlayerGameOverState m_playerGameOverState;
        
        [SerializeField]
        private Animator m_playerAnim;
        #endregion

        #region Unity Lifecycle
        /// <summary>
        /// Disables all the components, which shall not be executed by other clients or the server. Enables the initial state components and updates the player's state.
        /// </summary>
        void Start()
        {
            if (!entity.isOwner)
            {
                m_playerReadyState.enabled = false;
                m_playerAliveState.enabled = false;
                m_playerDeadState.enabled = false;
                m_playerReviveState.enabled = false;
                m_playerGameOverState.enabled = false;
                enabled = false;
            }
            else if (entity.isOwner)
            {
                m_playerReadyState.enabled = true;
                state.IsAlive = true;
            }
        }
        #endregion

        #region Public Functions
        /// <summary>
        /// Updates the player state after the player's revival. The PlayerRevive states gets disabled whereas the PlayerAliveState gets enabled.
        /// </summary>
        public void PlayerRevived()
        {
            m_playerReviveState.enabled = false;
            m_playerAliveState.enabled = true;
        }
        #endregion

        #region Local Functions
        #endregion

        #region Bolt Events
        /// <summary>
        /// Attaches the transform and the animator to the Bolt Entity for the network replication.
        /// </summary>
        public override void Attached()
        {
            state.SetTransforms(this.state.transform, this.transform);
            state.SetAnimator(m_playerAnim);
        }

        /// <summary>
        /// Disables the PlayerReadyState and enables the PlayerAliveState.
        /// </summary>
        /// <param name="evnt">The event does not contain any special information.</param>
        public override void OnEvent(PlayersMatchReadyEvent evnt)
        {
            m_playerReadyState.enabled = false;
            m_playerAliveState.enabled = true;

        }

        /// <summary>
        /// Disables the PlayerAliveState and enables the PlayerDeadState.
        /// </summary>
        /// <param name="evnt">The event does not contain any special information.</param>
        public override void OnEvent(BeastKillsPlayerEvent evnt)
        {
            m_playerAliveState.enabled = false;
            m_playerDeadState.enabled = true;
        }

        /// <summary>
        /// This event is triggered as soon as the player got revived. It updates the players state, disables the PlayerDeadState and enables the PlayerReviveState.
        /// </summary>
        /// <param name="evnt"></param>
        public override void OnEvent(RevivePlayer evnt)
        {
            state.IsAlive = true;
            state.Speed = 0;
            m_playerReviveState.enabled = true;
            m_playerDeadState.enabled = false;
        }
        #endregion
    }
}