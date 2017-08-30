using UnityEngine;

namespace BRO.Game.PreMatch
{
    /// <summary>
    /// Nothing to do.
    /// </summary>
    public class PreMatchControl : Bolt.EntityEventListener<IGameControllerState>
    {
        #region Member Fields
        [SerializeField]
        private PlayerRegistrationService m_registrationService;
        [SerializeField]
        private PlayerReadyController m_playerReadyController;
        [SerializeField]
        private PlayerCallbacksMainMenu m_playerCallbacks;
        [SerializeField]
        private ServerCallbacksMainMenu m_serverCallbacks;
        #endregion
    }
}