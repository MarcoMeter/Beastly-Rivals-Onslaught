using UnityEngine;

namespace BRO.Game
{
    /// <summary>
    /// The SoundEventReceiver requires the SoundManager (Singleton).
    /// It listens to the Bolt events, which are related to playing back sounds.
    /// </summary>
    [RequireComponent(typeof(SoundManager))]
    public class SoundEventReceiver : Bolt.GlobalEventListener
    {
        #region Bolt Events
        /// <summary>
        /// Upon the PlaySoundEvent, play the sound stated within the event data.
        /// </summary>
        /// <param name="evnt">Event data containing information about the sound to be played.</param>
        public override void OnEvent(PlaySoundEvent evnt)
        {
            SoundManager.Instance.PlayAudioClip((SoundClip)evnt.audioClipIndex);
        }
        #endregion
    }
}