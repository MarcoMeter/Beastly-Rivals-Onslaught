using BRO.Game;
using BRO.Game.PreMatch;
using UnityEngine;

namespace BRO.AI.Framework
{
    /// <summary>
    /// The AIController is in charge of adding the loaded AI logic to the AI player's GameObject.
    /// </summary>
    public class AIController : MonoBehaviour
    {
        #region Member Fields
        [SerializeField]
        private PlayerId m_playerIdComponent;
        [SerializeField]
        private AIDebug m_aiDebug;
        private AIBehaviour[] m_aiBehaviours;
        private IGameControllerState m_gmcState;
        #endregion

        #region Unity Lifecycle
        /// <summary>
        /// Initialize AI Logic and disable this component if this is not an AI
        /// </summary>
        private void Start()
        {
            if (!m_playerIdComponent.IsAi || BoltNetwork.isClient)
            {
                m_aiDebug.enabled = false;
                enabled = false;
            }
            else
            {
                m_gmcState = GameController.Instance.state;
                var m_playerRegistration = GameController.Instance.GetComponentInChildren<PlayerRegistrationService>();
                var m_aiBase = m_playerRegistration.GetAIBase(m_playerIdComponent.Id);
                m_aiBase.AddAIComponents(gameObject);
                // This loop ensures, that all AIComponents get initialized right after their instantiation on their GameObject
                m_aiBehaviours = GetComponents<AIBehaviour>();
                foreach (var behaviour in m_aiBehaviours)
                {
                    behaviour.Init();
                }
            }
        }

        /// <summary>
        /// The players information are updated by the end of every frame using the LateUpdate of the AIController
        /// </summary>
        private void LateUpdate()
        {
            if (m_gmcState.gameState != (int)MatchState.PreMatchState &&
                m_gmcState.gameState != (int)MatchState.MatchEndState)
            {
                foreach (var behaviour in m_aiBehaviours)
                {
                    behaviour.MyPlayerUpdate();
                }
            }
        }
        #endregion
    }
}