using System.Collections;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 0108

namespace BRO.Game
{
    /// <summary>
    /// Behaviour of the beast. It takes care of physics, animations, visualisations and gameplay logic.
    /// </summary>
    public class BeastBehaviour : Bolt.EntityBehaviour<IBeastState>
    {
        #region Member Fields
        // Speed vaules
        private const float m_MAX_SPEED = 75f;
        private const float m_MAX_ROTATION_SPEED = 360f;
        private const float m_INIT_SPEED = 10f;
        private const float m_INIT_ROTATION_SPEED = 50f;
        private const float m_SPEED_MULTIPLIER = 0.6f;
        private const float m_ROTATION_SPEED_MULTIPLIER = 0.0005f;
        private float m_speed = m_INIT_SPEED;                       // current speed
        private float m_rotationSpeed = m_INIT_ROTATION_SPEED;      // current  rotation speed
        // Power Shot values
        private const float m_POWER_SHOT_DURATION = 2.1f;
        private const float m_POWER_SHOT_MULTIPLIER = 0.15f;

        // Animation values
        [SerializeField]
        private Animator m_anim;
        private const float m_RUN_ANIM_THERSHOLD = 13f;
        private const int m_IDLE_ANIM = 0;
        private const int m_WALK_ANIM = 1;
        private const int m_RUN_ANIM = 2;

        // Components
        [SerializeField]
        private GameObject m_child; // The child GameObject of the beast is the visual representation
        [SerializeField]
        private Rigidbody m_rigidbody;
        private BoltEntity m_target;
        private IGameControllerState m_gmcState;
        private MatchFlowControl m_matchFlowControl;

        // Target & Hunt values
        private bool m_targetSet = false;
        private const string m_PLAYER_TAG = "Player";

        #endregion

        #region Member Properties
        /// <summary>
        /// Ready-only speed of the beast
        /// </summary>
        public float Speed
        {
            get
            {
                return m_speed;
            }
        }

        /// <summary>
        /// Ready-only rotation speed of the beast (indicates the accuracy of the beast)
        /// </summary>
        public float RotationSpeed
        {
            get
            {
                return m_rotationSpeed;
            }
        }
        #endregion

        #region Unity Lifecycle
        /// <summary>
        /// Initialize members and disable this component on the clients.
        /// </summary>
        private void Start()
        {
            if (!entity.isOwner)
            {
                enabled = false;
            }
            m_gmcState = GameController.Instance.state;
            m_matchFlowControl = GameController.Instance.GetComponentInChildren<MatchFlowControl>();
        }

        /// <summary>
        /// Physical behaviour of the beast
        /// </summary>
        private void FixedUpdate()
        {
            if (m_targetSet)
            {
                // Force the beast (the child is the visual representation) to look at the player
                Vector3 targetRotation = m_target.transform.position - transform.position;
                m_child.transform.rotation = Quaternion.LookRotation(targetRotation);

                // Rotate the beast to the LookRotation of the child
                transform.rotation = Quaternion.RotateTowards(transform.rotation, m_child.transform.rotation, Time.deltaTime * m_rotationSpeed);
                m_rigidbody.velocity = transform.forward * m_speed;

                if (m_speed <= m_MAX_SPEED)
                {
                    m_speed += Time.deltaTime * m_SPEED_MULTIPLIER;
                }
                else
                {
                    m_speed = m_MAX_SPEED;
                }

                if (m_rotationSpeed <= m_MAX_ROTATION_SPEED)
                {
                    m_rotationSpeed += Time.deltaTime * m_rotationSpeed * m_rotationSpeed * m_ROTATION_SPEED_MULTIPLIER;
                    // Exponential growth of the rotation speed is needed to achieve the distinctive movement of the beast.
                    // At first the rotationSpped is supposed to grow very slow to get a huge drifting effect.
                    // If it doesn't grow fast at the end the beast will be moving on an orbital path forever, because he will never meet the rotation query's demands ( too fast position changing )
                }
                else
                {
                    m_rotationSpeed = m_MAX_ROTATION_SPEED;
                }

                // Uncomment if you want to see the actual route of the beast
                //m_child.transform.rotation = transform.rotation;

                // Run animations based on the beast's speed
                if (m_speed < m_RUN_ANIM_THERSHOLD)
                {
                    state.Speed = m_WALK_ANIM;
                }
                else if (m_speed >= m_RUN_ANIM_THERSHOLD)
                {
                    state.Speed = m_RUN_ANIM;
                }
            }
        }
        
        /// <summary>
        /// Collision Detection to determine if the beast gets to kill someone
        /// </summary>
        /// <param name="coll"></param>
        private void OnTriggerEnter(Collider coll)
        {
            // Check if the collision occured with a player
            if (m_targetSet && coll.tag.Equals(m_PLAYER_TAG))
            {
                // Identify the player's ID
                int playerId = coll.gameObject.GetComponent<BoltEntity>().GetState<IPlayerState>().playerId;
                // Check if the player is in possession of the ball
                if (m_gmcState.players[playerId].hasBall)
                {
                    // Stop movement
                    m_targetSet = false;
                    m_rigidbody.velocity = Vector3.zero;
                    state.Speed = m_IDLE_ANIM;

                    // Raise BeastKillsPlayerEvent to kill the ball carrier (determined by ID)
                    BeastKillsPlayerEvent.Create(m_gmcState.players[playerId].playerEntity).Send();
                    // Raise Sound Event
                    var soundEvent = PlaySoundEvent.Create();
                    if(m_gmcState.lastPassByPlayer == -1)
                    {
                        soundEvent.audioClipIndex = (int)SoundClip.PlayerHumilated;
                    }
                    else
                    {
                        soundEvent.audioClipIndex = (int)SoundClip.PlayerKilled;
                    }
                    soundEvent.Send();

                    // Notify the GameController
                    m_matchFlowControl.PlayerKilledByBeast(playerId);

                    // Kill Animation
                    state.KillPlayer = true;
                }
            }
        }
        #endregion

        #region Public Functions
        /// <summary>
        /// Makes the beast hunt the target player.
        /// </summary>
        /// <param name="targetPlayer">Entity of the player with the ball</param>
        public void SetTarget(BoltEntity targetPlayer)
        {
            m_target = targetPlayer;
            m_targetSet = true;
        }

        /// <summary>
        /// Makes the beast hunt the target player in affection of the power shot.
        /// </summary>
        /// <param name="targetPlayer">Entity of the player with the ball</param>
        /// <param name="powerShotCharge">Charge amount of the power shot</param>
        public void SetPowerShotTarget(BoltEntity targetPlayer, float powerShotCharge)
        {
            SetTarget(targetPlayer);
            StartCoroutine(ManagePowerShotBehaviour(powerShotCharge));
        }

        /// <summary>
        /// Plays the IdleAgressive animation.
        /// </summary>
        public void IdleAgressive()                // Triggered by MatchFlowController
        {
            state.IdleAgressive = true;
        }

        /// <summary>
        /// Plays the normal Idle animation.
        /// </summary>
        public void Idle()                   // Triggered by MatchFlowController
        {
            state.IdleAgressive = false;
            state.KillPlayer = false;
            state.BackToIdle = true;
        }

        /// <summary>
        /// Resets the beast's speed values and sets the animation to IdleAgressive.
        /// </summary>
        public void ResetBeast()
        {
            m_speed = m_INIT_SPEED;
            m_rotationSpeed = m_INIT_ROTATION_SPEED;
            IdleAgressive();
        }
        #endregion

        #region Local Functions
        /// <summary>
        /// This Coroutine computes the new speed and rotation speed of the beast, which gets reverted after a certain duration.
        /// </summary>
        /// <param name="powerShotCharge">The charge of the power shot.</param>
        /// <returns></returns>
        private IEnumerator ManagePowerShotBehaviour(float powerShotCharge)
        {
            // Compute speed
            float saveSpeed = m_speed * 1.05f;
            float saveRotationSpeed = m_rotationSpeed * 1.1f;
            m_speed *= powerShotCharge * m_POWER_SHOT_MULTIPLIER;
            m_rotationSpeed *= powerShotCharge * (m_POWER_SHOT_MULTIPLIER);
            yield return new WaitForSeconds(m_POWER_SHOT_DURATION);
            // Reset speed
            m_speed = saveSpeed;
            m_rotationSpeed = saveRotationSpeed;
        }
        #endregion

        #region Bolt Events
        /// <summary>
        /// In order to replicate the beast across the network, the Animator and the transforms have to be attached to the state of the beast.
        /// </summary>
        public override void Attached()
        {
            state.SetTransforms(state.transformChild, transform.GetChild(0).transform);
            state.SetTransforms(state.transform, transform);
            state.SetAnimator(m_anim);
        }
        #endregion
    }
}