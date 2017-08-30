using UnityEngine;

namespace BRO.AI.Framework.Events
{
    public class MatchStartEvent : AIEvent
    {

    }

    public class MatchDoneEvent : AIEvent
    {
        public int Winner
        {
            get; set;
        }
    }

    public class PlayerKilledEvent : AIEvent
    {
        public bool Humilated
        {
            get; set;
        }

        public int Killer
        {
            get; set;
        }

        public int Victim
        {
            get; set;
        }
    }

    public class BallPassedEvent : AIEvent
    {
        public bool InitalBall      // if the match starts or continues (after a player died), then there is no source
        {
            get; set;
        }

        public int Source
        {
            get; set;
        }

        public int Target
        {
            get; set;
        }

        public bool IsPowerShot
        {
            get; set;
        }

        public float PowerShotCharge
        {
            get; set;
        }
    }

    public class BallCaughtEvent : AIEvent
    {
        public int Catcher
        {
            get; set;
        }
    }

    public class PlayerBlinkedEvent : AIEvent
    {
        public int Player
        {
            get; set;
        }

        public Vector3 Destination
        {
            get; set;
        }
    }
}
