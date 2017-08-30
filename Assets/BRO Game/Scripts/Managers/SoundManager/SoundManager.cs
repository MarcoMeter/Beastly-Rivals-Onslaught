using UnityEngine;
using BRO.Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;

namespace BRO.Game
{
    /// <summary>
    /// The SoundManager is in charge of playing back all sounds for all connections.
    /// </summary>
    public class SoundManager : GenericSingleton<SoundManager>
    {
        #region Member Fields
        [SerializeField]
        private AudioSource m_backgroundAudioSource;
        [SerializeField]
        private AudioClip[] m_audioClips;
        private Dictionary<SoundClip, AudioClip> m_soundDictionary = new Dictionary<SoundClip, AudioClip>();
        private const int m_RESET_CLIP_VALUE = int.MaxValue;
        private SoundEventReceiver m_eventReceiver;
        private const string m_CORE_MATCH_SCENE_NAME = "CoreMatch";
        private const string m_MAIN_MENU_SCENE_NAME = "MainMenu";
        #endregion

        #region Member Properties
        #endregion

        #region Unity Lifecycle
        private void Start()
        {
            InitializeSoundDictionary();

            // Attach a SoundEventReceiver if not present
            if (!m_eventReceiver)
            {
                m_eventReceiver = gameObject.AddComponent<SoundEventReceiver>();
            }

            // Add a callback to the sceneLoaded field
            SceneManager.sceneLoaded += OnLevelFinishedLoading;
            DontDestroyOnLoad(gameObject);
        }
        #endregion

        #region Public Functions
        /// <summary>
        /// Plays the desired sound effect.
        /// </summary>
        /// <param name="clipIndex"></param>
        public void PlayAudioClip(SoundClip clipEnum)
        {
            AudioClip audioClip;

            if (m_soundDictionary.TryGetValue(clipEnum, out audioClip))
            {
                AudioSource audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.clip = audioClip;
                audioSource.Play();
                StartCoroutine(DestroyAudioSource(audioSource));
            }
            else
            {
                Debug.LogWarning("No Audio Clip found for " + clipEnum.ToString());
            }
        }

        /// <summary>
        /// Starts playing the backgorund music.
        /// </summary>
        public void PlayBackgroundMusic()
        {
            m_backgroundAudioSource.Play();
        }

        /// <summary>
        /// Stops playing the background music.
        /// </summary>
        public void StopBackgroundMusic()
        {
            m_backgroundAudioSource.Stop();
        }
        #endregion

        #region Local Functions
        /// <summary>
        /// Pairs the SoundClip enums to the AudioClips referenced inside the array.
        /// </summary>
        private void InitializeSoundDictionary()
        {
            for(int i = 0; i < m_audioClips.Length; i++)
            {
                m_soundDictionary.Add((SoundClip)i, m_audioClips[i]);
            }
        }

        /// <summary>
        /// Destroys the AudioSource as soon as it is done playing the sound.
        /// </summary>
        /// <param name="audioSource">AudioSource to destroy after playing the audio clip.</param>
        /// <returns></returns>
        private IEnumerator DestroyAudioSource(AudioSource audioSource)
        {
            if(audioSource)
            {
                yield return new WaitForSeconds(audioSource.clip.length);
                Destroy(audioSource);
            }
        }

        /// <summary>
        /// Destroys and reinstantiates the SoundEventReceiver.
        /// If the SoundEventReceiver is treated as persistent, it won't be able to receive further events as soon as Bolt got shutdown and started again.
        /// Based on the scene, stop or start playing the background music.
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="mode"></param>
        private void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
        {
            if (m_eventReceiver)
            {
                Destroy(m_eventReceiver);
                m_eventReceiver = gameObject.AddComponent<SoundEventReceiver>();
            }

            if (scene.name.Equals(m_MAIN_MENU_SCENE_NAME))
            {
                StopBackgroundMusic();
            }
            else if(scene.name.Equals(m_CORE_MATCH_SCENE_NAME))
            {
                PlayBackgroundMusic();
            }
        }
        #endregion
    }
}