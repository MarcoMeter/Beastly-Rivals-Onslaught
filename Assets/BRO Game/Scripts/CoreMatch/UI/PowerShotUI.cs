using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BRO.Game
{
    /// <summary>
    /// This Component is in charge of activating the text objects for the power shot.
    /// </summary>
    public class PowerShotUI : MonoBehaviour
    {
        #region Member Fields
        [SerializeField]
        private GameObject m_textHolderGameObject;
        [SerializeField]
        private Text m_shotNameText;
        [SerializeField]
        private Text m_shotX;
        [SerializeField]
        private Text m_shotChargeValue;

        IGameControllerState m_gmcState;
        #endregion

        #region Unity LifeCycle
        /// <summary>
        /// Initializes the GameController state and adds a callback to listen to the event of the occurance of a charging power shot.
        /// </summary>
        private void Start()
        {
            m_gmcState = GameController.Instance.state;
            m_gmcState.AddCallback("isPowerShot", TogglePowerShotUI);
        }

        /// <summary>
        /// Updates the UI during the power shot.
        /// </summary>
        private void Update()
        {
            if(m_gmcState.isPowerShot)
            {
                m_shotChargeValue.text = m_gmcState.powerShotCharge.ToString("0.00");
            }
        }
        #endregion

        #region Local Functions
        /// <summary>
        /// Toggles the GameObject, which carries the text, on and off depening on the presence of a charging power shot.
        /// </summary>
        private void TogglePowerShotUI()
        {
            m_textHolderGameObject.SetActive(m_gmcState.isPowerShot);
        }
        #endregion
    }
}