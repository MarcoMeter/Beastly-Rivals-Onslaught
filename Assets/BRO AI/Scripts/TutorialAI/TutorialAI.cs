using BRO.AI.Framework.Events;
using System.Collections.Generic;
using UnityEngine;

namespace BRO.AI.Framework.Tutorial
{
    public class TutorialAI : AIEventListener
    {
        #region Member Fields
        private const float m_aiEngineInvokeTimeStep = 0.5f; // time intervals concerning the invocation of the function AIEngineUpdate
        #endregion

        #region Member Properties
        #endregion

        /// <summary>
        /// Functions and properties provided by the UnityEngine and its Monobehaviour are available.
        /// Check out Unity's documentation https://docs.unity3d.com/Manual/ExecutionOrder.html for further details
        /// </summary>
        #region Unity Lifecycle
        void Awake() { }
        void Start() { }
        void Update() { } // called every frame
        void LateUpdate() { } // called at the end of every frame

        /// <summary>
        /// Within OnDestroy we make sure that the invocation of AIEngineUpdate() gets cancelled.
        /// </summary>
        void OnDestroy()
        {
            CancelInvoke();
        }
        #endregion

        #region AI Scheduler
        /// <summary>
        /// AIEngineUpdate is called repeadetly as soon as the MatchStartEvent is received.
        /// It executes a logic for moving the player with the goal to increase the distance towards the closest foe.
        /// </summary>
        private void AIEngineUpdate()
        {
            if (!MyPlayer.HasBall)
            {
                List<PlayerDistance> distanceToPlayers = GetDistanceToRemainingPlayers();
                
                if (distanceToPlayers.Count > 0)
                {
                    // Request a list with the information about all distances to each player excluding this player, who requests the list for further functionalities
                    // The list is ordered by the closest player first
                    PlayerDistance closestPlayer = distanceToPlayers[0]; // closest player is in the first spot of the list

                    // Initialize direction vector which points in the opposite direction of the closest player
                    Vector3 oppositeDirectionToClosestPlayer = (transform.position - GameState.Players[closestPlayer.playerId].Position).normalized;
                    
                    // Execute movement
                    if (closestPlayer.distance < 20)
                    {
                        Move(transform.position + oppositeDirectionToClosestPlayer * 15);
                    }
                    else
                    {
                        Move(new Vector3(Random.Range(-40, 40), 0, Random.Range(-40, 40)));
                    }
                }
            }
        }
        #endregion

        #region AI Events
        /// <summary>
        /// On MatchStart, start invoking the AIEngineUpdate function.
        /// </summary>
        /// <param name="ev"></param>
        public override void OnEvent(MatchStartEvent ev)
        {
            InvokeRepeating("AIEngineUpdate", 0, m_aiEngineInvokeTimeStep); // InvokeRepeating requires the call of CancelInvoke() as soon as it is not needed anymore in order to avoid malfunctioning
        }

        public override void OnEvent(MatchDoneEvent ev)
        {
            
        }

        public override void OnEvent(PlayerKilledEvent ev)
        {

        }

        public override void OnEvent(PlayerBlinkedEvent ev)
        {

        }

        public override void OnEvent(BallPassedEvent ev)
        {

        }

        /// <summary>
        /// As soon as the AI catches the ball, it shall pass the ball to someone else.
        /// </summary>
        /// <param name="ev">Event information</param>
        public override void OnEvent(BallCaughtEvent ev)
        {
            if (MyPlayer.HasBall)
            {
                PassBall(GameState.GetRemainingEnemies(MyId)[Random.Range(0, GameState.GetRemainingEnemies(MyId).Count)].Id);
            }
        }
        #endregion

        #region Private Functions
        /// <summary>
        /// Computes a sorted list of players w.r.t. to the distance
        /// </summary>
        /// <returns>Returns a list of distances to every playing player (referenced by id), excluding the requesting player (referenced by id) himself.</returns>
        private List<PlayerDistance> GetDistanceToRemainingPlayers()
        {
            List<PlayerDistance> playerDistances = new List<PlayerDistance>();
            List<Player> remainingPlayers = GameState.RemainingPlayers;
            Vector3 requestingPlayerPos = transform.position;

            // Filter out MyPlayer and compute the distance
            for (int i = 0; i < remainingPlayers.Count; i++)
            {
                if (remainingPlayers[i].Id != MyId)
                {
                    playerDistances.Add(new PlayerDistance(remainingPlayers[i].Id, Vector3.Distance(requestingPlayerPos, GameState.Players[remainingPlayers[i].Id].Position)));
                }
            }

            // Sorts list by distance (closest player first)
            playerDistances.Sort((x, y) => x.distance.CompareTo(y.distance));

            return playerDistances;
        }
        #endregion
    }

    #region structs
    /// <summary>
    /// The PlayerDistance struct contains information about a player and the distance towards that player.
    /// </summary>
    public struct PlayerDistance
    {
        public int playerId;
        public float distance;

        public PlayerDistance(int id, float d)
        {
            playerId = id;
            distance = d;
        }
    }
    #endregion
}