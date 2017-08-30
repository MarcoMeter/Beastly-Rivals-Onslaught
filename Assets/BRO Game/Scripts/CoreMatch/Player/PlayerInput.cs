using UnityEngine;

namespace BRO.Game
{
    /// <summary>
    /// The PlayerInput component is charge of listening to human player inputs in order to interpret them and to trigger the desired action.
    /// </summary>
    public class PlayerInput : Bolt.EntityBehaviour<IPlayerState>
    {
        #region Member Fields
        [SerializeField]
        PlayerAliveState m_playerAliveState;
        [SerializeField]
        private GameObject m_walkFeedback;
        [SerializeField]
        private LayerMask m_layerMask;
        private const string m_PLAYER_TAG = "Player";
        private const string m_GROUND_TAG = "Ground";
        [SerializeField]
        private float m_clickTimeSpan = 0.2f;
        private int m_clickCount = 0;
        private float m_trackClickTime = 0;
        private bool m_trackTime = false;
        #endregion

        #region Unity Lifecycle
        /// <summary>
        /// Just keep this script executing on the bolt entity owner's instance,
        /// </summary>
        void Start()
        {
            if (!entity.isOwner)
            {
                enabled = false;
            }
        }
        
        /// <summary>
        /// Update processes the user's input seeking for single left clicks and double clicks.
        /// </summary>
        void Update()
        {
            // Mouse Input Logics
            if (Input.GetMouseButtonDown(0))
            {
                LeftMouseButtonInput();
            }
            TrackMouseClicks();

            TauntInput();
        }
        #endregion

        #region Local Functions
        /// <summary>
        /// Upon hitting the left mouse button, it is checked if a player or the floor got hit. Based on that information player actions are triggered.
        /// </summary>
        private void LeftMouseButtonInput()
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, m_layerMask.value))
            {
                // Determine if the click hit the ground or another player
                if (hit.transform.tag.Equals(m_PLAYER_TAG) && hit.transform.gameObject != gameObject)
                {
                    m_playerAliveState.PassBallToPlayer(hit.transform.gameObject.GetComponent<BoltEntity>()); // Trigger pass ball
                    m_clickCount = 0;
                    m_trackTime = false;
                }
                else if (hit.transform.tag.Equals(m_GROUND_TAG))
                {
                    m_clickCount++; // Start tracking time to check for a double click, these values are processed below
                    m_trackTime = true;
                }
            }
        }

        /// <summary>
        /// TrackMouseClick keeps track of the time and determines if a single or a double click occured.
        /// </summary>
        private void TrackMouseClicks()
        {
            // Increase timer
            if (m_trackTime)
                m_trackClickTime += Time.deltaTime;

            if (m_trackClickTime >= m_clickTimeSpan)
            {
                // If one single click
                if (m_clickCount == 1)
                    SingleClickAction();

                // If one double click
                if (m_clickCount >= 2)
                    DoubleClickAction();

                // Reset click/timer member
                m_trackClickTime = 0;
                m_clickCount = 0;
                m_trackTime = false;
            }
        }

        /// <summary>
        /// A SingleClick on the floor means that the player desires to move to that position.
        /// </summary>
        private void SingleClickAction()
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.tag == m_GROUND_TAG)
                {
                    m_playerAliveState.MoveForward(hit.point);
                    Instantiate(m_walkFeedback, new Vector3(hit.point.x, hit.point.y + 0.75f, hit.point.z), Quaternion.Euler(new Vector3(90, 0, 0)));
                }
            }
        }

        /// <summary>
        /// A double click triggers the blink ability, which teleporets the player to the double clicked location.
        /// </summary>
        private void DoubleClickAction()
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.tag == m_GROUND_TAG)
                    m_playerAliveState.Blink(hit.point);
            }
        }

        /// <summary>
        /// Listens to the keys F1 - F8 in order to play taunt animations.
        /// </summary>
        private void TauntInput()
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                m_playerAliveState.PlayTauntAnimation(1);
            }

            if (Input.GetKeyDown(KeyCode.F2))
            {
                m_playerAliveState.PlayTauntAnimation(2);
            }

            if (Input.GetKeyDown(KeyCode.F3))
            {
                m_playerAliveState.PlayTauntAnimation(3);
            }

            if (Input.GetKeyDown(KeyCode.F4))
            {
                m_playerAliveState.PlayTauntAnimation(4);
            }

            if (Input.GetKeyDown(KeyCode.F5))
            {
                m_playerAliveState.PlayTauntAnimation(5);
            }

            if (Input.GetKeyDown(KeyCode.F6))
            {
                m_playerAliveState.PlayTauntAnimation(6);
            }

            if (Input.GetKeyDown(KeyCode.F7))
            {
                m_playerAliveState.PlayTauntAnimation(7);
            }

            if (Input.GetKeyDown(KeyCode.F8))
            {
                m_playerAliveState.PlayTauntAnimation(8);
            }
        }
        #endregion
    }
}