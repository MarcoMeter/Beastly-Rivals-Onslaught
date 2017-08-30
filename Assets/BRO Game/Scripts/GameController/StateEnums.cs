namespace BRO.Game
{
    /// <summary>
    /// All possible states of the game (basically during the match).
    /// </summary>
    public enum MatchState { PreMatchState, MatchStartState, PlayerDeadState, PlayerReviveState, PlayerAliveState, MatchEndState };

    /// <summary>
    /// All possible states of each player during the game.
    /// </summary>
    public enum PlayerState { MatchStartState, DeadState, ReviveState, AliveState };
}