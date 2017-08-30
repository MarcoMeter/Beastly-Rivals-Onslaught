namespace BRO.AI.Framework.Events
{
    /// <summary>
    /// Base class for AI related events, which are supposed to be fired by the game logic itself.
    /// </summary>
    public class AIEvent
    {
        /// <summary>
        /// Create a blank instance of the event
        /// </summary>
        public AIEvent()
        {
            
        }
        
        /// <summary>
        /// Fire the event. The event is broadcasted to the AIs logic components (i.e. AIEventListener)
        /// </summary>
        public void Send()
        {
            AIEventManager.Instance.Raise(this);
        }
    }
}