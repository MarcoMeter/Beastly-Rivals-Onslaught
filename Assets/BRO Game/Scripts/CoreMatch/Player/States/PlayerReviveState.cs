using UnityEngine;
using UnityEngine.AI;

namespace BRO.Game
{
    /// <summary>
    /// The PlayerReviveState executes the logics for reviving the player object.
    /// </summary>
    public class PlayerReviveState : Bolt.EntityEventListener<IPlayerState>
    {
        #region Member Fields
        [SerializeField]
        private PlayerStateController m_playerStateController;
        private IGameControllerState m_gmcState;
        private const float m_REVIVAL_SPEED = 30f;
        private const float m_DESTINATION_DISTANCE_THRESHOLD = 0.55f;
        #endregion

        #region Unity Lifecycle
        /// <summary>
        /// Initializes members and sets the player state's speed to 0.
        /// </summary>
        private void OnEnable()
        {
            m_gmcState = GameController.Instance.state;
            state.Speed = 0;
        }

        /// <summary>
        /// Executes Revive logic.
        /// </summary>
        private void Update()
        {
            Revive();
        }
        #endregion

        #region Local Functions
        /// <summary>
        /// When revives, the player is back to his original spawn location and is facing the center of the field.
        /// After that the server gets notified about the revival based on the PlayerAliveEvent.
        /// </summary>
        private void Revive()
        {
            // Make players respawn at original spawn location
            //transform.position = m_gmcState.players[state.playerId].spawnPoint;
            // Make players respawn at random location
            var respawnPosition = new Vector3(Random.Range(-45, 45), 0, Random.Range(-40, 40));
            NavMeshHit hit;
            NavMesh.SamplePosition(respawnPosition, out hit, 30, NavMesh.AllAreas);
            transform.position = hit.position;
            transform.LookAt(Vector3.zero);

            var ev = PlayerAliveEvent.Create(GameController.Instance.GetComponent<BoltEntity>());
            ev.playerId = state.playerId;
            ev.Send();
            
            // Raise Sound Event
            var soundEvent = PlaySoundEvent.Create();
            soundEvent.audioClipIndex = (int)SoundClip.PlayerRevive;
            soundEvent.Send();

            m_playerStateController.PlayerRevived();    // inform the StateController
        }
        #endregion
    }
}