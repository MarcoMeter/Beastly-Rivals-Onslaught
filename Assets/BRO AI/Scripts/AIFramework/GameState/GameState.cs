using BRO.Game;
using System;
using System.Collections.Generic;

#pragma warning disable 0114 // `BRO.AI.Framework.GameState.ToString()' hides inherited member

namespace BRO.AI.Framework
{
    /// <summary>
    /// The static GameState object contains by the end of each frame the most recent information of the current state of the game.
    /// The information provided is designed for the AI framework.
    /// </summary>
    public class GameState
    {
        #region Member Fields
        private static List<Player> m_players;
        private static List<Player> m_remainingPlayers;
        private static Player m_ballCarrier;
        private static Player m_lastPassingPlayer;
        private static Beast m_beast;
        private static bool m_isPowerShot;
        private static float m_powerShotCharge;
        private static MatchState m_state;
        #endregion

        #region Member Properties
        /// <summary>
        /// Returns a list of all players who initially started the match
        /// </summary>
        public static List<Player> Players
        {
            get { return m_players; }
        }

        /// <summary>
        /// Returns a list of all players who are still participating in the match
        /// </summary>
        public static List<Player> RemainingPlayers
        {
            get { return m_remainingPlayers; }
        }

        /// <summary>
        /// Returns the player who is in possession of the ball
        /// </summary>
        public static Player BallCarrier
        {
            get { return m_ballCarrier; }
        }

        /// <summary>
        /// Returns the player who passed the ball last
        /// </summary>
        public static Player LastPassingPlayer
        {
            get { return m_lastPassingPlayer; }
        }

        /// <summary>
        /// Returns the beast which provides further information
        /// </summary>
        public static Beast Beast
        {
            get { return m_beast; }
        }

        /// <summary>
        /// Returns true if a power shot is being charged.
        /// </summary>
        public static bool IsPowerShot
        {
            get { return m_isPowerShot; }
        }

        /// <summary>
        /// Returns the amount of charge of a charging power shor.
        /// </summary>
        public static float PowerShotCharge
        {
            get { return m_powerShotCharge; }
        }

        /// <summary>
        /// Returns the state of the match itself
        /// </summary>
        public static MatchState State
        {
            get { return m_state; }
        }
        #endregion

        #region Public Functions
        /// <summary>
        /// Updates the whole GameState information. This functions is triggered by the GameStateDistributionService, which is part of the GameController.
        /// </summary>
        /// <param name="players">All players, who started the match</param>
        /// <param name="remainingPlayers">Players who remain in the match</param>
        /// <param name="ballCarrier">Player in possession of the ball</param>
        /// <param name="lastPassingPlayer">Player who passed the ball last</param>
        /// <param name="beast">Information baout the beast</param>
        /// <param name="matchState">The current state of the match</param>
        public static void UpdateGameStateInfo(List<Player> players, List<Player> remainingPlayers, Player ballCarrier, Player lastPassingPlayer, Beast beast, bool isPowerShot, float powerShotCharge, int matchState)
        {
            // Apply information
            m_players = players;
            m_remainingPlayers = remainingPlayers;
            m_ballCarrier = ballCarrier;
            m_lastPassingPlayer = lastPassingPlayer;
            m_beast = beast;
            m_isPowerShot = isPowerShot;
            m_powerShotCharge = powerShotCharge;
            m_state = (MatchState)matchState;
        }

        /// <summary>
        /// Searches through the players list to return the player with the entered id.
        /// </summary>
        /// <param name="playerId">The id of the desired player</param>
        /// <returns>The player found by his id</returns>
        public static Player GetPlayer(int playerId)
        {
            Player player = null;

            foreach (var p in m_players)
            {
                if (p.Id == playerId)
                {
                    player = p;
                    break;
                }
            }
            return player;
        }

        /// <summary>
        /// Selects all the enemies of one player.
        /// </summary>
        /// <param name="excludingPlayerId">Player to exclude from list (player who calls the function)</param>
        /// <returns>Returns a list of all enemies who are still participating in the match.</returns>
        public static List<Player> GetRemainingEnemies(int excludingPlayerId)
        {
            List<Player> enemyList = new List<Player>();

            foreach(var player in RemainingPlayers)
            {
                if(player.Id != excludingPlayerId)
                {
                    enemyList.Add(player);
                }
            }
            return enemyList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Info about the game state</returns>
        public static string ToString()
        {
            // Add beast and match info to the string
            string value =  "//// General ////" + Environment.NewLine +
                            "Match State: " + m_state.ToString() + Environment.NewLine +
                            "Player Count:" + m_players.Count + Environment.NewLine +
                            "Rem. Player Count: " + m_remainingPlayers.Count + Environment.NewLine +
                            "//// Beast ////" + Environment.NewLine +
                            m_beast.ToString() + Environment.NewLine;

            // Add player information to the string
            foreach (var p in m_players)
            {
                value +=    "//// Player " + p.Name + " ////" + Environment.NewLine + 
                            p.ToString() + Environment.NewLine;
            }

            return value;
        }
        #endregion
    }
}