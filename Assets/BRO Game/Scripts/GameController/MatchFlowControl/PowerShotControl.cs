using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BRO.Game
{
    /// <summary>
    /// The PowerShotControl observes the player with the ball. After a certain time, the next pass will be accelerated based on the passsed time while carryingn the ball.
    /// </summary>
    public class PowerShotControl : Bolt.EntityBehaviour<IGameControllerState>
    {
        #region Member Fields
        [SerializeField]
        private float m_chargeTimeThreshold = 7.5f;
        [SerializeField]
        private float m_chargeMultiplier = 2f;
        private float m_powerShotCharge = 0;
        private float m_countTime = 0;
        private bool m_charge = false;
        private bool m_soundEvent = false;
        #endregion

        #region Member Properties
        /// <summary>
        /// Read-only charge of the Power Shot.
        /// </summary>
        public float PowerShotCharge
        {
            get { return m_powerShotCharge; }
        }
        #endregion

        #region Unity Lifecycle
        /// <summary>
        /// Keeps this component disabled on clients.
        /// </summary>
        private void OnEnable()
        {
            if (!BoltNetwork.isServer)
            {
                enabled = false;
            }
        }

        /// <summary>
        /// Update runs the charge logic based on a existing ball carrier and the passed time.
        /// </summary>
        private void Update()
        {
            if(m_charge)
            {
                m_countTime += Time.deltaTime;
                if(m_countTime >= m_chargeTimeThreshold)
                {
                    if(!m_soundEvent)
                    {
                        // Player the beginning of the power shot only once
                        m_soundEvent = true;
                        var soundEvent = PlaySoundEvent.Create();
                        soundEvent.audioClipIndex = (int)SoundClip.PowerShotBegin;
                        soundEvent.Send();
                    }
                    m_powerShotCharge += Time.deltaTime * m_chargeMultiplier;
                    state.powerShotCharge = m_powerShotCharge;
                    state.isPowerShot = true;
                }
            }
        }
        #endregion

        #region Bolt Events
        /// <summary>
        /// Attached adds a callback to the ball carrier property of the GameController's state.
        /// </summary>
        public override void Attached()
        {
            state.AddCallback("ballCarrier", OnBallCarrierChanged);
        }
        #endregion

        #region Public Functions
        #endregion

        #region Local Functions
        /// <summary>
        /// Callback, which initializes members and triggers the charging logic of the Update() function.
        /// </summary>
        private void OnBallCarrierChanged()
        {
            // Initialize member
            m_powerShotCharge = 0;
            m_countTime = 0;
            state.isPowerShot = false;
            state.powerShotCharge = 0f;
            m_soundEvent = false;

            // Enable/Disable charge
            if (state.ballCarrier != null)
            {
                m_charge = true;
            }
            else
            {
                m_charge = false;
            }
        }
        #endregion
    }
}