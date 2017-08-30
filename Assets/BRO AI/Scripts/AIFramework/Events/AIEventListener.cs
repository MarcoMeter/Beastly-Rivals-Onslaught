namespace BRO.AI.Framework.Events
{
    /// <summary>
    /// The AIEventListener allows any subclass to override certain events, which may occur during the match.
    /// </summary>
    public abstract class AIEventListener : AIBehaviour
    {
        #region Interface IInitAI
        /// <summary>
        /// Adds the observer to the EventManager in order to listen to events.
        /// </summary>
        public override void Init()
        {
            base.Init();
            AIEventManager.Instance.AddListener(this);
        }
        #endregion

        #region Event Functions
        public virtual void OnEvent(MatchStartEvent e)
        {

        }

        public virtual void OnEvent(MatchDoneEvent e)
        {

        }

        public virtual void OnEvent(PlayerKilledEvent e)
        {

        }

        public virtual void OnEvent(BallPassedEvent e)
        {

        }

        public virtual void OnEvent(BallCaughtEvent e)
        {

        }

        public virtual void OnEvent(PlayerBlinkedEvent e)
        {

        }
        #endregion
    }
}