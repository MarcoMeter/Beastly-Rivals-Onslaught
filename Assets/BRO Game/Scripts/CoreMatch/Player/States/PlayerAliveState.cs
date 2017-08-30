using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using BRO.Game.PreMatch;

#pragma warning disable 0414

namespace BRO.Game
{
    /// <summary>
    /// The PlayerAliveState takes care of executing the actual action of the player object.It covers physics, animations and actions such as throwing the ball.
    /// </summary>
    public class PlayerAliveState : Bolt.EntityEventListener<IPlayerState>
    {
        #region Member Fields
        [SerializeField]
        private PlayerStateController m_playerStateController;
        [SerializeField]
        private PlayerInput m_playerInput;
        [SerializeField]
        private NavMeshAgent m_navMeshAgent;
        [SerializeField]
        private Animator m_playerAnim;
        [SerializeField]
        private float m_defaultSpeed = 15.0f;

        private IGameControllerState m_gmcState;

        private BoltEntity m_targetEntity;
        private Vector3 m_targetEntityPos;
        private float m_moveSpeed = 0;
        private Vector3 m_tempDestination;
        private float m_destinationDistance;
        private bool m_destinationSet = false;
        private bool m_passBall = false;
        private bool m_isTauntingOnce = false;

        // Animator States
        private int idleState = Animator.StringToHash("Base Layer.Idle");
        private int runState = Animator.StringToHash("Base Layer.Run");
        private int blinkState = Animator.StringToHash("Base Layer.Blink");
        private int throwState = Animator.StringToHash("Base Layer.Throw");
        private int deathState = Animator.StringToHash("Base Layer.Death");
        private int noMotionStateLayer2 = Animator.StringToHash("Taunts.No Motion");

        // Destination delegate
        public delegate void DestinationDelegate(bool isDestinationSet, Vector3 destination);
        #endregion

        #region Member Properties
        public DestinationDelegate DestinationCallback
        {
            get; set;
        }
        #endregion

        #region Unity Lifecycle
        /// <summary>
        /// As soon as the PlayerAlive states gets enabled, it enables the PlayerInput component and saves a reference to the GameController's state.
        /// </summary>
        private void OnEnable()
        {
            m_gmcState = GameController.Instance.state;
            TogglePlayerInput();

            // Pass this script reference to the StateMachineBehaviour of the player's animator
            // This ensures that this script is notified on the end of a taunt animation
            m_playerAnim.GetBehaviour<PlayerAnimatorStateMachineBehaviour>().Script = this;
        }

        /// <summary>
        /// OnDisable disables the PlayerInput, stops the NavMeshAgent and resets the player's speed to 0.
        /// </summary>
        private void OnDisable()
        {
            if (entity.isAttached) // If the scene got quit, the state and the NavMeshAgent cannot be accessed anymore.
            {
                TogglePlayerInput();
                StopAgent();
                state.Speed = 0;
            }
        }

        /// <summary>
        /// Update processes the animations and the physical movement.
        /// </summary>
        private void Update()
        {
            // Update the player state's movement speed
            state.Speed = m_moveSpeed;

            // Processes Movement
            if (m_destinationSet)
            {
                MoveTowards(m_tempDestination);
            }

            // Processes Passing the Ball
            if (m_passBall)
            {
                PassBallRotationLogic();
            }
        }
        #endregion

        #region Public Functions
        /// <summary>
        /// Command the player object to move to the specified destination.
        /// </summary>
        /// <param name="destination">The destination to move to.</param>
        public void MoveForward(Vector3 destination)
        {
            // Set members
            m_tempDestination = destination;
            m_destinationSet = true;            // the actual movement logic is processed inside Update()
            if(DestinationCallback != null)
                DestinationCallback(m_destinationSet, destination);
        }

        /// <summary>
        /// Commands the player object to pass the ball to the specified target in the case of possessing the ball.
        /// </summary>
        /// <param name="target">The target to throw the ball to.</param>
        public void PassBallToPlayer(BoltEntity target)
        {
            if (m_gmcState.players[state.playerId].hasBall)
            {
                // Stop the player's movement
                StopAgent();

                // Set members
                m_targetEntity = target;
                m_targetEntityPos = target.transform.position;
                m_passBall = true;              // the actual ball pass logic is processed inside Update()
            }
        }

        /// <summary>
        /// Commands the player object to teleport to the specified destination.
        /// </summary>
        /// <param name="destination">The destination to teleport to.</param>
        public void Blink(Vector3 destination)
        {
            // Blinking is only available if the ability is not on cooldown
            if (!m_gmcState.players[state.playerId].blinkOnCooldown)
            {
                // Stop the player's movement
                StopAgent();

                // The BlinkEvent is raised on the server because the server handles the cooldown of the blink ability. The server raises BlinkCooldownDoneEvent inside this script to enable the blink again
                var ev = BlinkEvent.Create(Bolt.GlobalTargets.OnlyServer);
                ev.destination = destination;
                ev.playerId = state.playerId;
                ev.Send();

                StartCoroutine(ProcessBlink(destination)); // ProcessBlink executes the rotation, animation and position with slight delays
            }
        }

        /// <summary>
        /// Plays triggers the specified taunt animation if this player killed someone.
        /// Disables the player input.
        /// </summary>
        /// <param name="tauntId">Id of the taunt animation.</param>
        public void PlayTauntAnimation(int tauntId)
        {
            if (m_gmcState.players[state.playerId].hasKilled)
            {
                StopAgent();
                state.TauntId = tauntId;
                m_isTauntingOnce = true;
                TogglePlayerInput();
            }
        }

        /// <summary>
        /// Stops the taunt animation and enables the PlayerInput again.
        /// </summary>
        public void TauntAnimationIsDone()
        {
            m_isTauntingOnce = false;
            TogglePlayerInput();
            state.TauntId = 0;
        }

        /// <summary>
        /// Stops the movement of player object.
        /// </summary>
        public void StopAgent()
        {
            m_moveSpeed = 0;
            m_destinationSet = false;
            m_navMeshAgent.angularSpeed = 0;
            m_navMeshAgent.ResetPath();
            if (DestinationCallback != null)
                DestinationCallback(m_destinationSet, m_tempDestination);
        }
        #endregion

        #region Local Functions
        /// <summary>
        /// Executes the movement functionality during update until the destination is reached.
        /// </summary>
        /// <param name="destination">The destionation location to move to.</param>
        private void MoveTowards(Vector3 destination)
        {
            m_destinationDistance = Vector3.Distance(destination, transform.position);

            if (m_destinationDistance < 2.5f && m_destinationDistance > 0.5f)
            {
                m_moveSpeed = 0;
            }
            else if (m_destinationDistance < 0.5f)
            {
                StopAgent();
            }
            else if (m_destinationDistance > 0.5f) // All distance querys inside this function avoid malfunctioning of the NavMeshAgent and the animations
            {
                m_moveSpeed = m_defaultSpeed;
                m_navMeshAgent.angularSpeed = 600;
                m_navMeshAgent.SetDestination(destination);
            }
        }

        /// <summary>
        /// Executes the rotation of the player object during Update to meet the requirements for passing the ball.
        /// </summary>
        private void PassBallRotationLogic()
        {
            // Basically the player object is rotating towards the position where the player clicked on an enemy to pass the ball
            // It won't try keep following the target if it is moving elsewhere
            Quaternion finalLookRotation = Quaternion.LookRotation(m_targetEntityPos - transform.position);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, finalLookRotation, Time.deltaTime * 250);

            if (Quaternion.Angle(finalLookRotation, transform.rotation) <= 10)
            {
                StartCoroutine(ProcessPassBall()); // ProcessPassBall takes care of the animation and adds a slight delay before the PassBallEvent is raised

                m_passBall = false;
            }
        }
        
        /// <summary>
        /// Executes the blink logics, which start with playing the animation and then setting the player to the desired location.
        /// </summary>
        /// <param name="destination"></param>
        /// <returns></returns>
        private IEnumerator ProcessBlink(Vector3 destination)
        {
            state.Blink = true;
            yield return new WaitForSeconds(0.1f);
            state.Blink = false;
            yield return new WaitForSeconds(0.2f);

            Vector3 lookDir = destination - transform.position;
            transform.position = destination;
            transform.rotation = Quaternion.LookRotation(lookDir);
        }

        /// <summary>
        /// Executes the logic of passing the ball by starting to play the animation. As soon as a certain time is passed, the actual event is raised to pass the ball, which is done by the server.
        /// </summary>
        /// <returns></returns>
        private IEnumerator ProcessPassBall()
        {
            state.Throw = true;
            yield return new WaitForSeconds(0.3f);
            state.Throw = false;
            yield return new WaitForSeconds(0.3f);

            var ev = PassBallEvent.Create(GameController.Instance.entity);
            ev.actingPlayer = entity;
            ev.targetPlayer = m_targetEntity;
            ev.playerId = state.playerId;
            ev.targetId = m_targetEntity.GetState<IPlayerState>().playerId;
            ev.Send();
        }

        /// <summary>
        /// Enables/disables the PlayerInput.
        /// Keeps the PlayerInput disabled for AIs.
        /// </summary>
        private void TogglePlayerInput()
        {
            if(state.isAi)
            {
                m_playerInput.enabled = false;
            }
            else
            {
                m_playerInput.enabled = !m_playerInput.enabled;
            }
        }
        #endregion
    }
}