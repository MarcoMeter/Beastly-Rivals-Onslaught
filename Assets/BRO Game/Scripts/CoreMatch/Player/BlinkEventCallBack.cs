using UnityEngine;
using System.Collections;
using BRO.AI.Framework.Events;

namespace BRO.Game
{
    /// <summary>
    /// It listens to the player's blink actions and processes the cooldown of these abilities.
    /// This class is executed on the Server only.
    /// </summary>
    public class BlinkEventCallBack : Bolt.GlobalEventListener
    {
        #region Member Fields
        private IGameControllerState m_gmcState;
        #endregion

        #region Unity Lifecycle
        /// <summary>
        /// Disables this component on the clients.
        /// Initializes member fields.
        /// </summary>
        private void Start()
        {
            if (!BoltNetwork.isServer)
                enabled = false;

            m_gmcState = GameController.Instance.state;
        }
        #endregion

        #region Bolt Events
        // The client raises the BlinkEvent if he performs the input for a blink.
        // The cool down of the blink ability is processed on the server (Coroutine)
        // As soon as the cooldown is done, the server raises the BlickCooldownDoneEvent on the client who was calling the BlinkEvent
        // The server is just controlling the cooldown. The positoning is performed by the client
        public override void OnEvent(BlinkEvent evnt)
        {
            // raise AI event
            var aiEvent = new PlayerBlinkedEvent();
            aiEvent.Player = evnt.playerId;
            aiEvent.Destination = evnt.destination;
            aiEvent.Send();

            // process blink cooldown
            m_gmcState.players[evnt.playerId].blinkOnCooldown = true;
            StartCoroutine(ProcessCooldown(evnt.playerId));

            // Trigger blink sound
            var soundEvent = PlaySoundEvent.Create();
            soundEvent.audioClipIndex = (int)SoundClip.BlinkAbility;
            soundEvent.Send();
        }
        #endregion

        #region Local Functions
        /// <summary>
        /// This Coroutine decreases the cooldown of the blink ability of the concerned player.
        /// </summary>
        /// <param name="playerId">The player to set his ability on cool down</param>
        /// <returns></returns>
        IEnumerator ProcessCooldown(int playerId)
        {
            yield return new WaitForSeconds(m_gmcState.players[playerId].blinkCoolDown);
            m_gmcState.players[playerId].blinkOnCooldown = false;
        }
        #endregion
    }
}