using System.Collections.Generic;
using UnityEngine.SceneManagement;
using BRO.AI.Framework;
using UnityEngine;

namespace BRO.Game.Controller
{
    /// <summary>
    /// The GameStateDistributionService prepares all the available GameState data and organizes them for the AI Framework.
    /// </summary>
    public class GameStateDistributionService : Bolt.EntityEventListener<IGameControllerState>
    {
        #region Unity Lifecycle
        /// <summary>
        /// Disables this component on the client. The game controller is executed on the server. It also initializes the spawn points.
        /// </summary>
        void Awake()
        {
            if (!BoltNetwork.isServer)
                enabled = false;
        }

        /// <summary>
        /// LateUpdate is used to gather all the information of the GameState in order to make them available to the AI Framework.
        /// </summary>
        void LateUpdate()
        {
            if (SceneManager.GetActiveScene().name == "CoreMatch") // Only process this component in the CoreMatch scene
            {
                // Update the AI framework's GameState
                if (state.gameState != (int)MatchState.PreMatchState &&
                    state.gameState != (int)MatchState.MatchEndState)
                {
                    // Start the information procedure as soon as the beast's entity is available (it takes a few frames for spawning all the game entities)
                    if (state.beastEntity)
                    {
                        // Prepare beast information
                        BeastBehaviour beastBehavior = state.beastEntity.GetComponent<BeastBehaviour>();
                        Beast beast = new Beast(beastBehavior.Speed,
                                                beastBehavior.RotationSpeed,
                                                beastBehavior.transform.position,
                                                beastBehavior.transform.rotation);

                        // Prepare player information
                        List<Player> players = new List<Player>();              // all players of the match
                        List<Player> remainingPlayers = new List<Player>();     // players who remain in the match
                        Player ballCarrier = null;
                        Player lastPassingPlayer = null;
                        foreach (var p in state.players)
                        {
                            // Add all participating players to the list
                            if (p.matchReady)
                            {
                                Player player = new Player(p.playerId,
                                                        p.playerName,
                                                        p.playerGameState,
                                                        p.lifes,
                                                        p.kills,
                                                        p.hasBall,
                                                        p.blinkOnCooldown,
                                                        p.blinkCoolDown,
                                                        p.playerEntity.transform.position,
                                                        p.playerEntity.transform.rotation);
                                players.Add(player);
                                // Add all remaining players to the other list
                                if (!p.isGameOver)
                                {
                                    remainingPlayers.Add(player);
                                }
                                // Determine player with the ball
                                if (p.hasBall)
                                {
                                    ballCarrier = player;
                                }
                                // Determine the player who passed the ball last
                                if (p.playerId == state.lastPassByPlayer)
                                {
                                    lastPassingPlayer = player;
                                }
                            }
                        }
                        GameState.UpdateGameStateInfo(players, remainingPlayers, ballCarrier, lastPassingPlayer, beast, state.isPowerShot, state.powerShotCharge, state.gameState);
                    }
                }
            }
        }
        #endregion
    }
}