namespace BRO.Game.PreMatch
{
    /// <summary>
    /// The PlayerInformation stores connections to all other players on the server site.
    /// On the client site the local player's information is getting stored in here.
    /// </summary>
    public class PlayerInformation
    {
        #region Member Fields
        public static BoltConnection[] playerConnections = new BoltConnection[8];
        public static Player myPlayer = new Player();
        #endregion
    }
}