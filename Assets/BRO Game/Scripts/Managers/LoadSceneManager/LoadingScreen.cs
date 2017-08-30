using UnityEngine;
using UnityEngine.UI;

namespace BRO.Game
{
    public class LoadingScreen : Bolt.GlobalEventListener
    {
        #region Member Fields
        [SerializeField]
        private Text m_LoadingText;
        [SerializeField]
        private Image m_loadingAnimationImage;
        [SerializeField]
        private Animator m_loadingAnimationAnimator;
        [SerializeField]
        private GameObject m_infoGameObject;
        #endregion

        #region Unity Lifecycle
        private void Start()
        {
            LoadSceneManager.Instance.InitializeLoadingScreen(m_LoadingText, m_loadingAnimationImage, m_loadingAnimationAnimator, m_infoGameObject);
        }
        #endregion
    }

    public class LoadingFlags
    {
        public LoadingFlags() { }

        public bool Local { get; set; }
        public bool UseFade { get; set; }
        public bool UseLoadingScreen { get; set; }
        public bool ShowInfo { get; set; }
        public bool WaitForInput { get; set; }
    }
}