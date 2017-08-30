using UnityEngine;

namespace BRO.Game
{
    /// <summary>
    /// The PlayerVisuals component is in charge of displaying any aesthetics related to the player of this instance of the script.
    /// </summary>
    public class PlayerVisuals : Bolt.EntityBehaviour<IPlayerState>
    {
        #region Member Fields
        [SerializeField]
        private GameObject m_controlledPlayerFeedback;
        [SerializeField]
        private GameObject m_ballFeedback;
        [SerializeField]
        private GameObject m_reviveFeedback;
        [SerializeField]
        private GameObject m_powerbotModel;
        [SerializeField]
        private GameObject m_blinkParticles;
        [SerializeField]
        private Collider m_collider;
        [SerializeField]
        private Texture[] m_playerTextures;

        private ParticleSystem m_blinkParticleSystem;
        private GameObject m_blinkIcon;

        IGameControllerState m_gmcState;
        #endregion

        #region Unity Lifecycle
        /// <summary>
        /// Initialize members and callbacks/delegates.
        /// </summary>
        private void Start()
        {
            m_ballFeedback.SetActive(false);
            m_reviveFeedback.SetActive(false);
            m_blinkParticleSystem = m_blinkParticles.GetComponent<ParticleSystem>();
            m_blinkIcon = GameObject.Find("FlashIcon");
            m_gmcState = GameController.Instance.state;

            if (entity.isOwner)
            {
                // Enable the player feedback, which visualizes the controlled player to the user
                if (!state.isAi)
                {
                    m_controlledPlayerFeedback.SetActive(true);
                }

                // Add Callbacks to GameController state
                m_gmcState.AddCallback("players[].hasBall", OnHasBallChanged);
                m_gmcState.AddCallback("players[].playerGameState", OnPlayerGameStateChanged);
                m_gmcState.AddCallback("players[].blinkOnCooldown", OnBlickCooldownChanged);
            }

            // Add Callbacks to player state
            state.AddCallback("ballFeedback", OnBallFeedbackChanged);
            state.AddCallback("reviveParticles", OnReviveFeedbackChanged);
            state.AddCallback("Blink", OnBlinkParticlesChanged);
            state.AddCallback("IsAlive", OnIsAliveChanged);

            // Assign the players' textures
            m_powerbotModel.GetComponent<SkinnedMeshRenderer>().material.SetTexture("_DIF", m_playerTextures[state.playerId]);
        }
        #endregion

        #region Local Functions
        /// <summary>
        /// Sets the players state according to the possession of the ball.
        /// </summary>
        private void OnHasBallChanged()
        {
            if (m_gmcState.players[state.playerId].hasBall)
            {
                state.ballFeedback = true;
            }
            else
            {
                state.ballFeedback = false;
            }
        }

        /// <summary>
        /// Sets the revive state.
        /// </summary>
        private void OnPlayerGameStateChanged()
        {
            if (m_gmcState.players[state.playerId].playerGameState == (int)PlayerState.ReviveState)
            {
                state.reviveParticles = true;
            }
            else
            {
                state.reviveParticles = false;
            }
        }
        
        /// <summary>
        /// Enables/disblaes the blink ability's icon.
        /// </summary>
        private void OnBlickCooldownChanged()
        {
            if (!m_gmcState.players[state.playerId].isAi)
            {
                // Ai doesn't use flashicon
                if (!m_gmcState.players[state.playerId].blinkOnCooldown)
                {
                    m_blinkIcon.SetActive(true);
                }
                else
                {
                    m_blinkIcon.SetActive(false);
                }
            }
        }

        /// <summary>
        /// Enables/disables the visual feedback indicting the possession of the ball.
        /// </summary>
        private void OnBallFeedbackChanged()
        {
            if (state.ballFeedback)
            {
                m_ballFeedback.SetActive(true);
            }
            else
            {
                m_ballFeedback.SetActive(false);
            }
        }

        /// <summary>
        /// Plays revive feedback when the player is revived.
        /// </summary>
        private void OnReviveFeedbackChanged()
        {
            if (state.reviveParticles)
            {
                m_reviveFeedback.SetActive(true);
            }
            else
            {
                m_reviveFeedback.SetActive(false);
            }
        }

        /// <summary>
        /// Plays the blink ability's particles.
        /// </summary>
        private void OnBlinkParticlesChanged()
        {
            if (state.Blink)
            {
                m_blinkParticles.SetActive(true);
                m_blinkParticleSystem.Play();
            }
        }
        
        /// <summary>
        /// Enables/disables the player's collider.
        /// </summary>
        private void OnIsAliveChanged()
        {
            if (state.IsAlive)
            {
                m_collider.enabled = true;
            }
            else
            {
                m_collider.enabled = false;
            }
        }
        #endregion
    }
}