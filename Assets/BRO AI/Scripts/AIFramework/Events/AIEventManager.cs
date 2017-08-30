using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BRO.AI.Framework.Events
{
    /// <summary>
    /// The EventManager (Singleton) guides raised events to their final destinations (i.e. AIEventListener).
    /// </summary>
    public class AIEventManager : GenericSingleton<AIEventManager>
    {
        #region Member Fields
        private List<AIEventListener> m_observers = new List<AIEventListener>();        // A list with observers that are notified if an event is raised
        #endregion 

        #region Public Functions
        /// <summary>
        /// Distributes AI events as soon as the end of the frame is reached.
        /// Events are fired by their Send() function.
        /// </summary>
        /// <param name="ev">The event to be fired and its information</param>
        public void Raise(AIEvent ev)
        {
            StartCoroutine(NotifyObservers(ev));
        }

        /// <summary>
        /// Adds an observer to listen to events
        /// </summary>
        /// <param name="observer">The observer which is supposed to listen</param>
        public void AddListener(AIEventListener observer)
        {
            m_observers.Add(observer);
        }

        /// <summary>
        /// Remove observer from the list
        /// </summary>
        /// <param name="observer">The observer to remove</param>
        public void RemoveListener(AIEventListener observer)
        {
            m_observers.Remove(observer);
        }
        #endregion

        #region Local Functions
        /// <summary>
        /// Triggers the event on the observers by the end of the frame.
        /// </summary>
        /// <param name="ev">AI Event information</param>
        /// <returns>WaitForEndOfFrame</returns>
        private IEnumerator NotifyObservers(AIEvent ev)
        {
            yield return new WaitForEndOfFrame();
            foreach (var observer in m_observers)
            {
                if (ev.GetType() == typeof(MatchStartEvent))
                {
                    observer.OnEvent((MatchStartEvent)ev);
                }

                else if (ev.GetType() == typeof(MatchDoneEvent))
                {
                    observer.OnEvent((MatchDoneEvent)ev);
                }

                else if (ev.GetType() == typeof(PlayerKilledEvent))
                {
                    observer.OnEvent((PlayerKilledEvent)ev);
                }

                else if (ev.GetType() == typeof(BallPassedEvent))
                {
                    observer.OnEvent((BallPassedEvent)ev);
                }

                else if (ev.GetType() == typeof(PlayerBlinkedEvent))
                {
                    observer.OnEvent((PlayerBlinkedEvent)ev);
                }

                else if (ev.GetType() == typeof(BallCaughtEvent))
                {
                    observer.OnEvent((BallCaughtEvent)ev);
                }
            }
        }
        #endregion
    }
}
