using Bolt;
using BRO.Game.PreMatch;
using UdpKit;

namespace BRO.Game
{
    /// <summary>
    /// This class listens to Bolt events and distributes that kind of information to other compontents, which have to make use of that.
    /// </summary>
    public class ServerCallbacksCoreMatch : Bolt.GlobalEventListener
    {
        #region Bolt Events
        /// <summary>
        /// Refuse any incoming connection during the CoreMatch, because the game is already going on. SO no connections on the fly.
        /// </summary>
        /// <param name="endpoint">Endpoint of the connection who tried to connect</param>
        /// <param name="token">No token</param>
        public override void ConnectRequest(UdpEndPoint endpoint, IProtocolToken token)
        {
            BoltNetwork.Refuse(endpoint);
        }

        /// <summary>
        /// TODO: The match flow is affected by players who disconnect during the match.
        /// </summary>
        /// <param name="connection">Player who disconnected</param>
        public override void Disconnected(BoltConnection connection)
        {
            // send a message to the match flow
        }
        #endregion
    }
}