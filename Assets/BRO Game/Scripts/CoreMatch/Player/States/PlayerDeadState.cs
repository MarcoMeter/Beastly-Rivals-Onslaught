namespace BRO.Game
{
    /// <summary>
    /// The PlayerDeadState is present if the player got killed by the beast.
    /// It takes care of playing the dead animation and setting the player state's speed back to 0.
    /// </summary>
    public class PlayerDeadState : Bolt.EntityEventListener<IPlayerState>
    {
        #region Member Fields
        #endregion

        #region Unity Lifecycle
        /// <summary>
        /// Plays the dead animation and sets the player state's speed back to 0.
        /// </summary>
        private void OnEnable()
        {
            state.IsAlive = false;
            state.Speed = 0;
        }
        #endregion

        #region Public Functions
        #endregion

        #region Local Functions
        #endregion
    }
}