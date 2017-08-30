using UnityEngine;
using System.Collections;
using BRO.AI.Framework.Events;

namespace BRO.Game
{
    /// <summary>
    /// This script defines the ball's movement towards its target
    /// </summary>
    public class BallBehaviour : Bolt.EntityBehaviour<IBallState>
    {
        #region Member Fields
        [SerializeField]
        private float m_applyVelocityTime = 0.5f;
        [SerializeField]
        private float m_defaultBallSpeed = 45f;
        [SerializeField]
        private float m_ballSpeedLimit = 200f;
        [SerializeField]
        private float m_ballChargeMultiplier = 0.3f;
        private float m_ballSpeed;

        private Transform m_targetPlayer;
        private float m_distanceToTarget;
        private float m_heightAdjustment = 20f;
        private float m_countTime = 0;
        private bool m_targetSet = false;

        IGameControllerState m_gmcState;
        #endregion

        #region Unity Lifecycle
        /// <summary>
        /// Initialize components and members
        /// </summary>
        void Start()
        {
            m_gmcState = GameController.Instance.state;
        }

        /// <summary>
        /// Process ball movement if the target is set.
        /// </summary>
        void Update()
        {
            if (m_targetSet)
            {
                m_countTime += Time.deltaTime;
                if (m_countTime > m_applyVelocityTime)
                {
                    m_heightAdjustment -= Time.deltaTime * 75;

                    if (m_heightAdjustment <= 0)
                        m_heightAdjustment = 0;
                }
                else
                {
                    m_heightAdjustment -= Time.deltaTime * 5;
                }

                Vector3 target = new Vector3(m_targetPlayer.position.x, m_targetPlayer.position.y + 8 + m_heightAdjustment, m_targetPlayer.position.z);
                transform.position = Vector3.MoveTowards(transform.position, target, Time.deltaTime * m_ballSpeed);

                if (Vector3.Distance(transform.position, new Vector3(m_targetPlayer.position.x, m_targetPlayer.position.y + 8f, m_targetPlayer.position.z)) < 0.05f)
                {
                    // if the ball has almost reached its final destination, the PassBallPlayerAttachEvent is raised on the entity which is supposed to catch the ball
                    m_gmcState.players[m_targetPlayer.GetComponent<BoltEntity>().GetState<IPlayerState>().playerId].hasBall = true;
                    BoltEntity entitiyTarget = m_targetPlayer.GetComponent<BoltEntity>();
                    PassBallPlayerAttachEvent.Create(entitiyTarget).Send();
                    m_gmcState.ballCarrier = entitiyTarget;
                    // Raise AI Event, the event gets called by the end of the frame
                    var aiEvent = new BallCaughtEvent();
                    aiEvent.Catcher = entitiyTarget.GetState<IPlayerState>().playerId;
                    aiEvent.Send();
                    BoltNetwork.Destroy(entity);
                }
            }
        }
        #endregion

        #region Public Functions
        /// <summary>
        /// Sets a target for the ball and processes the throwing curve.
        /// </summary>
        /// <param name="target">The ball's target</param>
        public void SetTarget(Transform target)
        {
            m_targetPlayer = target;
            // get distance between the target and the ball in the very first place
            m_distanceToTarget = Vector3.Distance(transform.position, m_targetPlayer.position);
            // compute the time there the ball is going upwards
            m_applyVelocityTime = m_applyVelocityTime * ((m_distanceToTarget / 100));
            m_targetSet = true;
            m_ballSpeed = m_defaultBallSpeed;
        }

        /// <summary>
        /// Sets a target for the ball and processes the throwing curve.
        /// </summary>
        /// <param name="target">The ball's target</param>
        public void SetPowerShotCharge(Transform target, float powerShotCharge)
        {
            // Set the target
            SetTarget(target);

            // Compute Ball Speed
            float multiplier = powerShotCharge * m_ballChargeMultiplier;
            if(multiplier < 1)
            {
                multiplier = 1;
            }
            m_ballSpeed = m_defaultBallSpeed * multiplier;

            // Limit Ball Speed
            if(m_ballSpeed > m_ballSpeedLimit)
            {
                m_ballSpeed = m_ballSpeedLimit;
            }

            // Compute Velocity Time
            m_applyVelocityTime = m_applyVelocityTime * ((m_distanceToTarget / (m_ballSpeed * 2.5f)));
        }
        #endregion

        #region Local Functions
        #endregion

        #region Bolt Events
        /// <summary>
        /// Replicates the transform component across the network by attaching it to the Ball's state.
        /// </summary>
        public override void Attached()
        {
            state.SetTransforms(this.state.transform, this.transform);

            // After attaching the transform, disable this component on the clients
            if (!BoltNetwork.isServer)
            {
                enabled = false; // only the server is supposed to use this BallMovement component
            }
        }
        #endregion
    }
}