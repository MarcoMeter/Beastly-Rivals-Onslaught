using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BRO.Game
{
    /// <summary>
    /// The LoadSceneManager transitions scenes using optionally a loading screen and a fading transition effect.
    /// </summary>
    public class LoadSceneManager : GenericSingleton<LoadSceneManager>
    {
        #region Member Fields
        [SerializeField]
        private LoadSceneEventReceiver m_eventReceiver;
        [SerializeField]
        private GameObject m_fadeGameObject;
        [SerializeField]
        private Image m_fadeImage;
        [SerializeField]
        private Color m_fadeInColor;
        [SerializeField]
        private Color m_fadeOutColor;
        [SerializeField]
        private float m_fadeSpeed = 1f;
        private const string m_LOADING_SCREEN_SCENE_NAME = "LoadingScreen";
        
        private Text m_LoadingText;
        private Image m_loadingAnimationImage;
        private Animator m_loadingAnimationAnimator;
        [SerializeField]
        private Color m_loadingStartColor = Color.red;
        [SerializeField]
        private Color m_loadingEndColor = Color.green;
        private const string m_LOAD_DONE_TEXT = "Loading Done! Press Any Key To Continue";
        private AsyncOperation m_asyncOperation;
        private string m_destinationScene = "";
        private const float m_MAX_LOAD_PROGRESS = 0.9f;
        private bool m_waitForInputKey = true;
        private bool m_showInfo = true;
        private GameObject m_infoGameObject;
        #endregion

        #region Unity Lifecycle
        /// <summary>
        /// Initializes the LoadSceneManager
        /// </summary>
        private void Start()
        {
            // Add a callback to the sceneLoaded field of the SceneManager
            SceneManager.sceneLoaded += OnLevelFinishedLoading;

            // Attach a LoadSceneEventReceiver if not present
            if (!m_eventReceiver)
            {
                m_eventReceiver = gameObject.AddComponent<LoadSceneEventReceiver>();
            }

            // Keep the LoadSceneManager persistent
            DontDestroyOnLoad(gameObject);
        }
        #endregion

        #region Public Functions
        /// <summary>
        /// Triggers the process of loading the specified scene based on its name.
        /// </summary>
        /// <param name="sceneName">The destination scene.</param>
        /// <param name="local">Shall the scene be loaded across the network?</param>
        /// <param name="useFade">Shall the scene transition be faded or not?</param>
        /// <param name="useLoadingScreen">Shall be a loading screen used?</param>
        public void LoadScene(string sceneName, LoadingFlags flags)
        {
            m_destinationScene = sceneName;
            m_waitForInputKey = flags.WaitForInput;
            m_showInfo = flags.ShowInfo;
            if (!flags.Local)
            {
                // Trigger the network event for loading the scene on all connections
                var ev = LoadSceneEvent.Create(Bolt.GlobalTargets.Everyone);
                ev.sceneName = sceneName;
                ev.useFade = flags.UseFade;
                ev.useLoadingScreen = flags.UseLoadingScreen;
                ev.Send();
            }
            else
            {
                // Load the destination scene locally
                StartCoroutine(ProcessLoadScene(sceneName, flags.UseFade, flags.UseLoadingScreen));
            }
        }

        /// <summary>
        /// Loads the scene which is specified by the LoadSceneEvent.
        /// </summary>
        /// <param name="ev">Event data about how to load the destination scene</param>
        public void LoadScene(LoadSceneEvent ev)
        {
            StartCoroutine(ProcessLoadScene(ev.sceneName, ev.useFade, ev.useLoadingScreen));
            m_destinationScene = ev.sceneName;
        }

        /// <summary>
        /// Initialize members of the loading screen for displaying updates.
        /// Triggers the process of loading the final destination scene, if the loading screen is used for scene transitioning.
        /// </summary>
        /// <param name="loadingText">Text for displaying loading information</param>
        /// <param name="loadingAnimationImage">Loading animation image</param>
        /// <param name="loadingAnimationAnimator">Animator of the loading image</param>
        public void InitializeLoadingScreen(Text loadingText, Image loadingAnimationImage, Animator loadingAnimationAnimator, GameObject infoGameObject)
        {
            m_LoadingText = loadingText;
            m_loadingAnimationImage = loadingAnimationImage;
            m_loadingAnimationAnimator = loadingAnimationAnimator;
            m_infoGameObject = infoGameObject;
            StartCoroutine(LoadDestinationScene(m_destinationScene));
        }
        #endregion

        #region Local Functions
        /// <summary>
        /// This Coroutine runs the process of loading the desire scene based on the parameters.
        /// </summary>
        /// <param name="sceneName">Final scene to load</param>
        /// <param name="useFade">Shall the scene be transitioned using a fade?</param>
        /// <param name="useLoadingScreen">Shall the loading screen be used for transitioning?</param>
        /// <returns></returns>
        private IEnumerator ProcessLoadScene(string sceneName, bool useFade, bool useLoadingScreen)
        {
            if(useFade)
            {
                yield return StartCoroutine(FadeInOut(false));
            }

            if (useLoadingScreen)
            {
                SceneManager.LoadScene(m_LOADING_SCREEN_SCENE_NAME);
            }
            else
            {
                SceneManager.LoadScene(sceneName);
            }

            if(useFade)
            {
                yield return StartCoroutine(FadeInOut(true));
            }

            yield return null;
        }

        /// <summary>
        /// Actually loads the final scene and updates the loading screen GameObjects.
        /// </summary>
        /// <param name="sceneName">Name of the destination scene</param>
        /// <returns></returns>
        private IEnumerator LoadDestinationScene(string sceneName)
        {
            m_infoGameObject.SetActive(m_showInfo);
            yield return new WaitForSeconds(0.5f);
            // Trigger scene loading and initialize the AsyncOperation for progress information
            m_asyncOperation = SceneManager.LoadSceneAsync(sceneName);
            m_asyncOperation.allowSceneActivation = false;

            while (!m_asyncOperation.isDone)
            {
                // Update color of the loading animation based on the loading progress
                m_loadingAnimationImage.color = Color.Lerp(m_loadingStartColor, m_loadingEndColor, m_asyncOperation.progress / m_MAX_LOAD_PROGRESS);

                // The progess of the loading won't exceed 90%
                if (m_asyncOperation.progress >= m_MAX_LOAD_PROGRESS)
                {
                    // Update the loading screen scene
                    m_LoadingText.text = m_LOAD_DONE_TEXT;

                    if (m_waitForInputKey)
                    {
                        // Wait for input to finally activate the desired scene
                        if (Input.anyKeyDown && !Input.GetKeyDown(KeyCode.Mouse0))
                        {
                            yield return StartCoroutine(FadeInOut(false));
                            m_asyncOperation.allowSceneActivation = true;
                            yield return StartCoroutine(FadeInOut(true));
                        }
                    }
                    else
                    {
                        yield return StartCoroutine(FadeInOut(false));
                        m_asyncOperation.allowSceneActivation = true;
                        yield return StartCoroutine(FadeInOut(true));
                    }
                }
                yield return null; // yield return has to be called each iteration, otherwise this will make the application freeze
            }
        }

        /// <summary>
        /// Fades between two colors of the fade image.
        /// </summary>
        /// <param name="fadeIn">Fade out if false.</param>
        /// <returns></returns>
        private IEnumerator FadeInOut(bool fadeIn)
        {
            float lerpProgress = 0;
            m_fadeGameObject.SetActive(true);
            while (lerpProgress <= 1)
            {
                lerpProgress += Time.deltaTime * m_fadeSpeed;
                if (fadeIn)
                {
                    m_fadeImage.color = Color.Lerp(m_fadeInColor, m_fadeOutColor, lerpProgress);
                }
                else
                {
                    m_fadeImage.color = Color.Lerp(m_fadeOutColor, m_fadeInColor, lerpProgress);
                }
                yield return null; // yield return has to be called each iteration, otherwise this will make the application freeze
            }
            m_fadeGameObject.SetActive(false);
        }

        /// <summary>
        /// Callback to get notified upon the done loading of a scene.
        /// Destroys and reinstantiates the LoadSceneEventReceiver.
        /// If the LoadSceneEventReceiver is treated as persistent, it won't be able to receive further events as soon as Bolt got shutdown and started again.
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="mode"></param>
        private void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
        {
            if (m_eventReceiver)
            {
                Destroy(m_eventReceiver);
                m_eventReceiver = gameObject.AddComponent<LoadSceneEventReceiver>();
            }
        }
        #endregion
    }
}