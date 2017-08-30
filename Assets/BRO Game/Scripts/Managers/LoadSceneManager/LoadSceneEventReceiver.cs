using UnityEngine;

namespace BRO.Game
{
    [RequireComponent(typeof(LoadSceneManager))]
    public class LoadSceneEventReceiver : Bolt.GlobalEventListener
    {
        #region Bolt Events
        public override void OnEvent(LoadSceneEvent evnt)
        {
            LoadSceneManager.Instance.LoadScene(evnt);
        }
        #endregion
    }
}