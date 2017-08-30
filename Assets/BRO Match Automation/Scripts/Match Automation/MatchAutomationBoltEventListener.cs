using UnityEngine;
using System.Collections;
using Bolt;

namespace BRO.MatchAutomation
{
    public class MatchAutomationBoltEventListener : Bolt.GlobalEventListener
    {
        #region Member Fields
        private const float m_BOLT_SHUTDOWN_DELAY = 0.5f;
        private MatchAutomationManager m_automationManager;
        #endregion

        #region Unity Lifecycle
        public void Start()
        {
            m_automationManager = GetComponent<MatchAutomationManager>();
        }
        #endregion

        #region Bolt Events
        public override void BoltStartDone()
        {
            m_automationManager.InitializeMatch();
        }

        public override void BoltShutdownBegin(AddCallback registerDoneCallback)
        {
            registerDoneCallback(OnBoltShutdown);
        }

        private void OnBoltShutdown()
        {
            m_automationManager.ConcludeMatch();
        }
        #endregion

        #region Custom Events
        public override void OnEvent(WinnerEvent evnt)
        {
            StartCoroutine(ShutdownBolt());
        }
        #endregion

        #region Local Functions
        private IEnumerator ShutdownBolt()
        {
            yield return new WaitForSeconds(m_BOLT_SHUTDOWN_DELAY);
            BoltLauncher.Shutdown();
        }
        #endregion
    }
}