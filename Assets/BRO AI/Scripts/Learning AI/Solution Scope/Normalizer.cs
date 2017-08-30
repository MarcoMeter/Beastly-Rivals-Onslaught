namespace BRO.AI.Learning
{
    /// <summary>
    /// This class provides static functions for each individual input information to normalize (range 0 - 1).
    /// </summary>
    public class Normalizer
    {
        /// <summary>
        /// This function normalizes the time elapsed after an initial ball pass by the environment
        /// </summary>
        /// <param name="currentTimeElapsed">Elapsed time</param>
        /// <returns>Returns the normalized value of the time elapsed after the initial ball</returns>
        public static double NormalizeElapsedTime(double currentTimeElapsed)
        {
            const double minTimeElapsed = 0;
            const double maxTimeElapsed = 60;
            return ((double)currentTimeElapsed - minTimeElapsed) / (maxTimeElapsed - minTimeElapsed);
        }

        /// <summary>
        /// This function normalizes the beast's speed
        /// </summary>
        /// <param name="currentSpeed">Current speed of the beast</param>
        /// <returns>Returns the normalized value of the beast's speed</returns>
        public static double NormalizeBeastSpeed(float currentSpeed)
        {
            const double minSpeed = 0;
            const double maxSpeed = 75;
            return ((double)currentSpeed - minSpeed) / (maxSpeed - minSpeed);
        }

        /// <summary>
        /// This function normalizes the beast's rotation speed
        /// </summary>
        /// <param name="currentRotationSpeed">Current rotation speed of the beast</param>
        /// <returns>Returns the normalized value of the beast's rotation speed</returns>
        public static double NormalizeBeastRotationSpeed(float currentRotationSpeed)
        {
            const double minSpeed = 50;
            const double maxSpeed = 360;
            return ((double)currentRotationSpeed - minSpeed) / (maxSpeed - minSpeed);
        }

        /// <summary>
        /// This function normalizes the beast's position x
        /// </summary>
        /// <param name="currentBeastPositionX">Current position x of the beast</param>
        /// <returns>Returns the normalized value of the beast's position x</returns>
        public static double NormalizeBeastPositionX(float currentBeastPositionX)
        {
            const double minBeastPositionX = -70;
            const double maxBeastPositionX = 70;
            return ((double)currentBeastPositionX - minBeastPositionX) / (maxBeastPositionX - minBeastPositionX);
        }

        /// <summary>
        /// This function normalizes the beast's position y
        /// </summary>
        /// <param name="currentBeastPositionZ">Current position y of the beast</param>
        /// <returns>Returns the normalized value of the beast's position y</returns>
        public static double NormalizeBeastPositionZ(float currentBeastPositionZ)
        {
            const double minBeastPositionZ = -70;
            const double maxBeastPositionZ = 70;
            return ((double)currentBeastPositionZ - minBeastPositionZ) / (maxBeastPositionZ - minBeastPositionZ);
        }

        /// <summary>
        /// This function normalizes the grid position of an entity
        /// </summary>
        /// <param name="currentGridPosition">The current position of the entity according to the grid</param>
        /// <returns>Returns the normalized value of the entity's grid postion</returns>
        public static double NormalizeGridPosition(int currentGridPosition)
        {
            const double minGridPosition = 0;
            const double maxGridPosition = 159;
            return ((double)currentGridPosition - minGridPosition) / (maxGridPosition - minGridPosition);
        }

        /// <summary>
        /// This function normalizes the ID of the ball carrier
        /// </summary>
        /// <param name="ballCarrier">Current ball carrier, the value shall be -1 if there is no ball carrier</param>
        /// <returns>Returns the normalized value of the ID of the player who possesses the ball currently</returns>
        public static double NormalizeBallCarrier(int ballCarrier)
        {
            const double minBallPossession = -1;
            const double maxBallPossession = 7;
            return ((double)ballCarrier - minBallPossession) / (maxBallPossession - minBallPossession);
        }       

        /// <summary>
        /// This function normalizes an entity's current position x
        /// </summary>
        /// <param name="currentPositionX">Entity's current position x </param>
        /// <returns>Returns the normalized value of the entity's position x</returns>
        public static double NormalizePositionX(float currentPositionX)
        {
            const double minPositionX = -60;
            const double maxPositionX = 60;
            return ((double)currentPositionX - minPositionX) / (maxPositionX - minPositionX);
        }

        /// <summary>
        /// This function normalizes an entity's current position z
        /// </summary>
        /// <param name="currentsPositionZ">Entity's current position z</param>
        /// <returns>Returns the normalized value of the entity's position z</returns>
        public static double NormalizePositionZ(float currentPositionZ)
        {
            const double minPositionZ = -50;
            const double maxPositionZ = 50;
            return ((double)currentPositionZ - minPositionZ) / (maxPositionZ - minPositionZ);
        }

        /// <summary>
        /// This function normalizes the current distance to the beast or to a foe
        /// </summary>
        /// <param name="currentDistance">Current distance to the beast or to a foe</param>
        /// <returns>Returns the normalized value of the distance to the beast or to a foe</returns>
        public static double NormalizeDistance(float currentDistance)
        {
            const double minDistanceToBeast = 0;
            const double maxDistanceToBeast = 130;
            return ((double)currentDistance - minDistanceToBeast) / (maxDistanceToBeast - minDistanceToBeast);
        }

        /// <summary>
        /// This function normalizes an entity's current rotation
        /// </summary>
        /// <param name="currentMyRotation">Entity's current rotation</param>
        /// <returns>Returns the normalized value of the entity's rotation</returns>
        public static double NormalizeRotation(float currentMyRotation)
        {
            const double minMyRotation = -1;
            const double maxMyRotation = 360;
            return ((double)currentMyRotation - minMyRotation) / (maxMyRotation - minMyRotation);
        }
        
        /// <summary>
        /// This function normalizes the number of remaining lives
        /// </summary>
        /// <param name="currentLives">My current number of remaining lives</param>
        /// <returns>Returns the normalized value of the number of remaining lives</returns>
        public static double NormalizeLives(int currentLives)
        {
            const double minLives = -1;
            const double maxMyLives = 9;
            return (currentLives - minLives) / (maxMyLives - minLives);
        }
        
        /// <summary>
        /// This function normalizes an entity's current score
        /// </summary>
        /// <param name="currentScore">Entity's current score </param>
        /// <returns>Returns the normalized value of the entity's current score</returns>
        public static double NormalizeScore(int currentScore)
        {
            const double minScore = -11;
            const double maxScore = 70;
            return ((double)currentScore - minScore) / (maxScore - minScore);
        }
    }
}