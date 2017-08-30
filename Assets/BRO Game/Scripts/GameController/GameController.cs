using UnityEngine;
using BRO.Game.PreMatch;
using BRO.Game.Conclusion;
using UnityEngine.SceneManagement;

namespace BRO.Game
{
    /// <summary>
    /// The game controller takes care of launching and processing the match.
    /// </summary>
    public class GameController : GameControllerSingleton<GameController>
    {
        #region Member Fields
        [SerializeField]
        PreMatchControl m_preMatchControl;
        [SerializeField]
        MatchFlowControl m_matchFlowControl;
        [SerializeField]
        MatchConclusionControl m_matchConclusionControl;
        private const string m_CORE_MATCH_SCENE_NAME = "CoreMatch";
        #endregion

        #region Unity Lifecycle
        /// <summary>
        /// Disables this component on the client. The game controller is executed on the server.
        /// </summary>
        private void Awake()
        {
            if (!BoltNetwork.isServer)
                enabled = false;
        }

        /// <summary>
        /// Disables all components of the match flow control.
        /// </summary>
        private void Start()
        {
            foreach (var component in m_matchFlowControl.GetComponents<MonoBehaviour>())
            {
                component.enabled = false;
            }
        }
        #endregion

        #region Public Functions
        /// <summary>
        /// Enables all components of the match flow control and launches the match.
        /// </summary>
        public void TransitionToMatchFlow()
        {
            // Disable all PreMatchControl components as soon as the CoreMatch got loaded.
            foreach (var component in m_preMatchControl.GetComponents<MonoBehaviour>())
            {
                component.enabled = false;
            }

            // Enable all components for the match in the CoreScene
            foreach (var component in m_matchFlowControl.GetComponents<MonoBehaviour>())
            {
                component.enabled = true;
            }

            // Start running the match logic
            m_matchFlowControl.GetComponent<MatchFlowControl>().StartMatch();
        }
        #endregion
    }
}