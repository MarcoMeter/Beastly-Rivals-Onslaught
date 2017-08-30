using UnityEngine;

// I basically want to use this object to 

namespace BRO.Game.PreMatch
{

    /// <summary>
    /// This class gathers all information about the local player to transfer them to the replication OnConnected.
    /// </summary>
    public class Player
    {
        #region Member Fields
        public BoltEntity entity;
        public int id;
        public string playerName;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a player object with a default name.
        /// </summary>
        public Player()
        {
            this.playerName = "UnnamedPlayer" + Random.Range(0, 100);
        }
        #endregion
    }
}