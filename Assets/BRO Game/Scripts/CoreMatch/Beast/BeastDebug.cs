using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BRO.Game
{
    /// <summary>
    /// Takes care of visualizing debug information.
    /// </summary>
    public class BeastDebug : MonoBehaviour
    {
        #region Member Fields
        [SerializeField]
        private BeastBehaviour m_beastBehaviour;
        [SerializeField]
        private GameObject m_debuggingCanvas;
        [SerializeField]
        private Text m_speedText;
        [SerializeField]
        private Text m_rotationSpeedText;
        [SerializeField]
        private GameObject m_actualRotationIndicator;
        [SerializeField]
        private GameObject m_lookRotationIndicator;
        [SerializeField]
        private GameObject m_collisionBoxVisualization;
        #endregion

        #region Unity Lifecycle
        /// <summary>
        /// Adds the DebugModeChanged function to the DebugModeDelegate.
        /// </summary>
        private void Start()
        {
            DebugManager.Instance.DebugModeCallback += OnDebugModeChanged;
            SetDebugGameObjectsActive(DebugManager.Instance.IsDebugMode);
        }

        /// <summary>
        /// Updates the information on the text objects.
        /// </summary>
        private void FixedUpdate()
        {
            m_speedText.text = "Speed: " + m_beastBehaviour.Speed.ToString("0.00");
            m_rotationSpeedText.text = "RSpeed: " + m_beastBehaviour.RotationSpeed.ToString("0.00");
        }
        #endregion

        #region Local Functions
        /// <summary>
        /// If the DebugMode changed on the DebugManager, this function is triggered.
        /// </summary>
        /// <param name="isDebugMode">True, if the debug mode is enabled.</param>
        private void OnDebugModeChanged(bool isDebugMode)
        {
            SetDebugGameObjectsActive(isDebugMode);
        }

        /// <summary>
        /// Enable/Disable the visual debugging GameObjects.
        /// </summary>
        /// <param name="isDebugMode">GameObjects will be activated if true</param>
        private void SetDebugGameObjectsActive(bool isDebugMode)
        {
            m_debuggingCanvas.SetActive(isDebugMode);
            m_actualRotationIndicator.SetActive(isDebugMode);
            m_lookRotationIndicator.SetActive(isDebugMode);
            m_collisionBoxVisualization.SetActive(isDebugMode);
        }
        #endregion
    }
}