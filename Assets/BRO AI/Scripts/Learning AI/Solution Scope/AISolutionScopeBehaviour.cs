using BRO.AI.Framework;
using BRO.AI.Framework.Events;
using BRO.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BRO.AI.Learning
{
    /// <summary>
    /// This class extends all the AI components and Monobehaviour.
    /// It shall provide the complete output actions of the solution scope for the learning AI.
    /// </summary>
    public class AISolutionScopeBehaviour : AIEventListener
    {
        #region Member Fields
        // Output Action related
        // Movement
        private const float m_behindBeastDistance = 8;
        private const float m_beforePlayerDistance = 5;

        #region Input Headings
        protected string[] m_inputHeadings = new string[] { "Elapsed Time",
                                                            "Beast Speed",
                                                            "Beast Rotation Speed",
                                                            "Beast Grid Position",
                                                            "Beast Rotation",
                                                            "Ball Carrier",
                                                            "My Grid Position",
                                                            "My Distance To Beast",
                                                            "My Rotation",
                                                            "My Remaining Lives",
                                                            "My Score",
                                                            "1 Enemy Grid Position", "2 Enemy Grid Position", "3 Enemy Grid Position", "4 Enemy Grid Position", "5 Enemy Grid Position", "6 Enemy Grid Position", "7 Enemy Grid Position",
                                                            "1 Distance To Enemy", "2 Distance To Enemy", "3 Distance To Enemy", "4 Distance To Enemy", "5 Distance To Enemy" ,"6 Distance To Enemy", "7 Distance To Enemy",
                                                            "1 Enemy Rotation", "2 Enemy Rotation", "3 Enemy Rotation", "4 Enemy Rotation", "5 Enemy Rotation", "6 Enemy Rotation", "7 Enemy Rotation",
                                                            "1 Enemy Remaining Lives", "2 Enemy Remaining Lives", "3 Enemy Remaining Lives", "4 Enemy Remaining Lives", "5 Enemy Remaining Lives", "6 Enemy Remaining Lives", "7 Enemy Remaining Lives",
                                                            "1 Enemy Score", "2 Enemy Score", "3 Enemy Score", "4 Enemy Score", "5 Enemy Score", "6 Enemy Score", "7 Enemy Score"};

        protected string[] m_inputHeadingsSimplified = new string[] {
                                                            "Beast Speed",
                                                            "Beast Rotation Speed",
                                                            "Beast Position X",
                                                            "Beast Position Z",
                                                            "Beast Rotation",
                                                            "Ball Carrier",
                                                            "My Position X",
                                                            "My Position Z",
                                                            "My Rotation",
                                                            "Enemy Position X",
                                                            "Enemy Position X",
                                                            "Enemy Rotation",
                                                            };
        #endregion
        protected DateTime m_ballSequenceStartTime; // Start time at which the initial ball was passed.
        private Grid m_environmentGrid = new Grid(new Vector2(-80, 80), new Vector2(80, -80), 10, 10);
        #endregion

        #region Movement Actions
        /// <summary>
        /// Stops any movement. No further action is carried out.
        /// </summary>
        protected void DoNothing()
        {
            StopMove();
        }

        /// <summary>
        /// Starts moving up.
        /// </summary>
        /// <param name="distance">The distance is used to specify the target location</param>
        protected void MoveUp(float distance)
        {
            Move(MyPlayer.Position + Vector3.forward * distance);
        }

        /// <summary>
        /// Starts moving down.
        /// </summary>
        /// <param name="distance">The distance is used to specify the target location</param>
        protected void MoveDown(float distance)
        {
            Move(MyPlayer.Position + (Vector3.back * distance));
        }

        /// <summary>
        /// Starts moving left.
        /// </summary>
        /// <param name="distance">The distance is used to specify the target location</param>
        protected void MoveLeft(float distance)
        {
            Move(MyPlayer.Position + Vector3.left * distance);
        }

        /// <summary>
        /// Starts moving right.
        /// </summary>
        /// <param name="distance">The distance is used to specify the target location</param>
        protected void MoveRight(float distance)
        {
            Move(MyPlayer.Position + (Vector3.right * distance));
        }

        /// <summary>
        /// Moves in a way to keep dodging the beast for the purpose of survival and loading the power shot.
        /// </summary>
        protected void MoveToDodgeBeast()
        {
            // Compute beast forward direction
            Vector3 beastForward = (GameState.Beast.Rotation * Vector3.forward).normalized;
            // Compute position behindbeast based on some certain distance
            Vector3 behindBeastPos = GameState.Beast.Position + (beastForward * -m_behindBeastDistance);

            // Move to position behind the beast, which has a certain distance to the beast's position
            Move(behindBeastPos);
        }

        /// <summary>
        /// Runs into the player, who is in possession of the ball, to block him.
        /// </summary>
        protected void BlockBallCarrier()
        {
            if (GameState.BallCarrier != null)
            {
                // Compute ball carrier forward direction
                Vector3 playerForward = (GameState.BallCarrier.Rotation * Vector3.forward).normalized;
                // Compute position in front of the ball carrier based on some certain distance
                Vector3 beforePlayerPos = GameState.BallCarrier.Position + (playerForward * m_beforePlayerDistance);

                // Move to position behind the beast, which has a certain distance to the beast's position
                Move(beforePlayerPos);
            }
        }

        /// <summary>
        /// Moves away from the beast to gain more distance.
        /// </summary>
        protected void GainDistanceToBeast()
        {
            // Compute beast forward direction
            Vector3 beastForward = (GameState.Beast.Rotation * Vector3.forward).normalized;
            // Compute position behindbeast based on some certain distance
            Vector3 behindBeastPos = GameState.Beast.Position + (beastForward * -m_behindBeastDistance * 4);

            // Move to position behind the beast, which has a certain distance to the beast's position
            Move(behindBeastPos);
        }
        #endregion

        #region Throw Actions
        /// <summary>
        /// Passes the ball to a foe, who is closest to the direction faced by the AI.
        /// That's the fastest possibility to pass the ball.
        /// </summary>
        protected void PassToEasiestTarget()
        {
            // finding an easy target is about computing an angle to all players (based on face direction). The smallest angle guides to the easiest target.
            throw new NotImplementedException();
        }

        /// <summary>
        /// Passes the ball to the foe with the highest score.
        /// If there are foes, which share the same score, then ...
        /// </summary>
        protected void PassToHighestScore()
        {
            // find the player with the highest score GameState.GetPlayer(i).Kills
            // if there are players with the same score, then come up with a further querry to select one of them (not random!)
            // maybe trigger PassToEasiestTarget in that case
            // think about it and question it
            throw new NotImplementedException();
        }

        /// <summary>
        /// Passes the ball to the foe with the lowest amount of remaining lives.
        /// If there are foes, which share the same amount of lives, then ...
        /// </summary>
        protected void PassToLowestLife()
        {
            // find the enemy with the lowest remaining lives
            // if there are players with the same amount of lifes, then come up with a further querry to select one of them (not random!)
            // maybe trigger PassToEasiestTarget in that case
            // think about it and question it
            throw new NotImplementedException();
        }

        /// <summary>
        /// Passes the ball to the foe, who is located on the moving direction of the beast.
        /// This target has the highest probability of getting a foe killed.
        /// </summary>
        protected void PassToPrey()
        {
            // Probably! compute angles and distances from the enemies to the beast and base your decision on that
            // think about it and question it
            throw new NotImplementedException();
        }

        /// <summary>
        /// This Pass Ball function selects a random target for passing the ball.
        /// This is not supposed to be a serious output action for the learning AI.
        /// </summary>
        protected void PassBallRandomly()
        {
            if (MyPlayer.HasBall)
            {
                PassBall(GameState.GetRemainingEnemies(MyId)[UnityEngine.Random.Range(0, GameState.GetRemainingEnemies(MyId).Count)].Id);
            }
        }
        #endregion

        #region Blink Actions
        #endregion

        #region Input Information
        /// <summary>
        /// Gathers all input information of the current state of the game.
        /// </summary>
        /// <returns>Returns all normalized and numeric values of the game state.</returns>
        protected double[] GetInputData()
        {
            // Retrieve and setup state values
            // Beast values
            double elapsedTime = Math.Round((DateTime.Now - m_ballSequenceStartTime).TotalSeconds * Time.timeScale, 1); // Round to one decimal digit
            float beastSpeed = GameState.Beast.Speed;
            float beastRotationSpeed = GameState.Beast.RotationSpeed;
            //float beastPositionX = GameState.Beast.Position.x;
            //float beastPostitionZ = GameState.Beast.Position.z;
            int beastGridPosition = m_environmentGrid.FindGridIndex(new Vector2(GameState.Beast.Position.x, GameState.Beast.Position.z));
            float beastRotation = GameState.Beast.Rotation.eulerAngles.y;
            int ballCarrierId;
            if (GameState.BallCarrier != null)
            {
                ballCarrierId = GameState.BallCarrier.Id;
            }
            // if there is no ball carrier assign -1
            else
            {
                ballCarrierId = -1;
            }
            // Normalize and concat beast/environment values
            double[] beastValues = new double[] { //Normalizer.NormalizeElapsedTime(elapsedTime),
                                                  Normalizer.NormalizeBeastSpeed(beastSpeed),
                                                  Normalizer.NormalizeBeastRotationSpeed(beastRotationSpeed),
                                                  Normalizer.NormalizeGridPosition(beastGridPosition),
                                                  Normalizer.NormalizeRotation(beastRotation),
                                                  Normalizer.NormalizeBallCarrier(ballCarrierId) };

            // Player values
            //float myPositionX = MyPlayer.Position.x;
            //float myPositionZ = MyPlayer.Position.z;
            int myGridPosition = m_environmentGrid.FindGridIndex(new Vector2(MyPlayer.Position.x, MyPlayer.Position.z));
            float distanceToBeast = (MyPlayer.Position - GameState.Beast.Position).magnitude;
            float myRotation = MyPlayer.Rotation.eulerAngles.y;
            int myRemainingLives = MyPlayer.Lives;
            int myScore = MyPlayer.Kills;
            // Normalize and concat player values
            double[] playerValues = new double[] { Normalizer.NormalizeGridPosition(myGridPosition),
                                                   Normalizer.NormalizeDistance(distanceToBeast),
                                                   Normalizer.NormalizeRotation(myRotation),
                                                   //Normalizer.NormalizeLives(myRemainingLives),
                                                   //Normalizer.NormalizeScore(myScore) 
                                                  };

            // Enemy values
            //List<double> enemyPositionX = new List<double>();
            //List<double> enemyPositionZ = new List<double>();
            List<double> enemyGridPosition = new List<double>();
            List<double> enemyDistances = new List<double>();
            List<double> enemyRotations = new List<double>();
            List<double> enemyRemainingLives = new List<double>();
            List<double> enemyScores = new List<double>();
            double[] enemyValues = new double[] { };

            // Loop over all enemys and compute the individual values while checking if the potential enemy player is actually playing.
            for (int i = 0; i <= 7; i++)
            {
                // Check if the current id is not belonging to this player
                if (i != MyPlayer.Id)
                {
                    // Retrieve player based on id (highest possible id is 7)
                    var enemy = GameState.GetPlayer(i);

                    // Deal with alive players
                    if (enemy != null && enemy.State == PlayerState.AliveState)
                    {
                        //enemyPositionX.Add(enemy.Position.x);
                        //enemyPositionZ.Add(enemy.Position.z);
                        enemyGridPosition.Add(Normalizer.NormalizeGridPosition(m_environmentGrid.FindGridIndex(new Vector2(enemy.Position.x, enemy.Position.z))));
                        enemyDistances.Add(Normalizer.NormalizeDistance((MyPlayer.Position - enemy.Position).magnitude));
                        enemyRotations.Add(Normalizer.NormalizeRotation(enemy.Rotation.y));
                        enemyRemainingLives.Add(Normalizer.NormalizeLives(enemy.Lives));
                        enemyScores.Add(Normalizer.NormalizeScore(enemy.Kills));
                    }
                    // Deal with dead players and players who are actually not present in the match
                    else
                    {
                        //(enemyPositionX.Add(40);
                        //(enemyPositionZ.Add(50);
                        enemyGridPosition.Add(Normalizer.NormalizeGridPosition(0));           
                        enemyDistances.Add(Normalizer.NormalizeDistance(130));            
                        enemyRotations.Add(Normalizer.NormalizeRotation(-1));            
                        enemyRemainingLives.Add(Normalizer.NormalizeLives(-1));                                                          
                        enemyScores.Add(Normalizer.NormalizeScore(-11));
                    }
                }
            }

            // Concat enemy values
            enemyValues = enemyValues.Concat(enemyGridPosition.ToArray())
                .Concat(enemyDistances.ToArray())
                .Concat(enemyRotations.ToArray())
                //.Concat(enemyRemainingLives.ToArray())
                //.Concat(enemyScores.ToArray())
                .ToArray();

            // Concat all values and return them
            return beastValues.Concat(playerValues).Concat(enemyValues).ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Returns the least necessary normalized and numeric values of the game state.</returns>
        protected double[] GetSimplifiedInputData(bool normalize)
        {
            #region Beast Values
            // Retrieve and setup state values
            // Beast values
            float beastSpeed = GameState.Beast.Speed;
            float beastRotationSpeed = GameState.Beast.RotationSpeed;
            float beastPositionX = GameState.Beast.Position.x;
            float beastPostitionZ = GameState.Beast.Position.z;
            //float beastPrevPositionX;
            //float beastPrevPositionZ;
            float beastRotation = GameState.Beast.Rotation.eulerAngles.y;
            int ballCarrierId;
            if (GameState.BallCarrier != null)
            {
                ballCarrierId = GameState.BallCarrier.Id;
            }
            // if there is no ball carrier assign -1
            else
            {
                ballCarrierId = -1;
            }
            // Normalize and concat beast/environment values
            double[] beastValuesNormalized = new double[] { 
                                                  Normalizer.NormalizeBeastSpeed(beastSpeed),
                                                  Normalizer.NormalizeBeastRotationSpeed(beastRotationSpeed),
                                                  Normalizer.NormalizeBeastPositionX(beastPositionX),
                                                  Normalizer.NormalizeBeastPositionZ(beastPostitionZ),
                                                  Normalizer.NormalizeRotation(beastRotation),
                                                  Normalizer.NormalizeBallCarrier(ballCarrierId)
                                                };

            double[] beastValues = new double[] {
                                                  beastSpeed,
                                                  beastRotationSpeed,
                                                  beastPositionX,
                                                  beastPostitionZ,
                                                  beastRotation,
                                                  ballCarrierId,
                                                };
            #endregion

            #region Player Values
            // Player values
            float myPositionX = MyPlayer.Position.x;
            float myPositionZ = MyPlayer.Position.z;
            //float myPrevPositionX;
            //float myPrevPositionZ;
            float myRotation = MyPlayer.Rotation.eulerAngles.y;
            // Normalize and concat player values
            double[] playerValuesNormalized = new double[] { Normalizer.NormalizePositionX(myPositionX),
                                                   Normalizer.NormalizePositionZ(myPositionZ),
                                                   Normalizer.NormalizeRotation(myRotation),
                                                 };

            double[] playerValues = new double[] { myPositionX,
                                                   myPositionZ,
                                                   myRotation,
                                                 };
            #endregion

            #region Enemy Values
            // Enemy values
            Player enemy = GameState.GetRemainingEnemies(MyPlayer.Id)[0];
            float enemyPositionX = enemy.Position.x;
            float enemyPositionZ = enemy.Position.z;
            float enemyRotation = enemy.Rotation.eulerAngles.y;
            double[] enemyValuesNormalized = new double[] { Normalizer.NormalizePositionX(enemyPositionX),
                                                  Normalizer.NormalizePositionZ(enemyPositionZ),
                                                  Normalizer.NormalizeRotation(enemyRotation),
                                                };

            double[] enemyValues = new double[] { enemyPositionX,
                                                  enemyPositionZ,
                                                  enemyRotation,
                                                };
            #endregion

            // Concat all values and return them
            if (normalize)
            {
                return beastValuesNormalized.Concat(playerValuesNormalized).Concat(enemyValuesNormalized).ToArray();
            }
            else
            {
                return beastValues.Concat(playerValues).Concat(enemyValues).ToArray();
            }
        }
        #endregion
    }
}