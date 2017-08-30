/// <summary>
/// The PlayerID component helps out to distribute and replicate the id to the player's state.
/// </summary>
public class PlayerId : Bolt.EntityBehaviour<IPlayerState>
{
    #region Member Properties
    /// <summary>
    /// Set the IsAi property and the value for the player's state.
    /// </summary>
    public bool IsAi
    {
        get { return state.isAi; }
        set
        {
            state.isAi = value;
        }
    }

    /// <summary>
    /// Sets and gets the player's id.
    /// </summary>
    public int Id
    {
        get { return state.playerId; }
        set
        {
            state.playerId = value;
        }
    }
    #endregion
}