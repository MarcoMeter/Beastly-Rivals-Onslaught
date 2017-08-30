namespace BRO.Game.PreMatch
{
    /// <summary>
    /// The PlayerIDToken ensures the proper distribution of the player's designated id during the prematch phase.
    /// </summary>
    public class PlayerIdToken : Bolt.IProtocolToken
    {
        #region Member Fields
        private int m_playerId = int.MaxValue;
        #endregion

        #region Member Properties
        /// <summary>
        /// Gets/sets the playerId for the token distribution.
        /// </summary>
        public int PlayerID
        {
            get{ return m_playerId; }
            set{ m_playerId = value; }
        }
        #endregion

        #region IProtocolToken Interface
        /// <summary>
        /// Writes the information to the packet of the token.
        /// </summary>
        /// <param name="packet">The packet which stores the information.</param>
        public void Write(UdpKit.UdpPacket packet)
        {
            packet.WriteInt(m_playerId);
        }

        /// <summary>
        /// Reads the token data of the packet.
        /// </summary>
        /// <param name="packet">Packet with the token data.</param>
        public void Read(UdpKit.UdpPacket packet)
        {
            m_playerId = packet.ReadInt();
        }
        #endregion
    }
}