namespace BRO.Game.PreMatch
{
    /// <summary>
    /// The PlayerCallbacksMainMenu receives events for the player. It is in charge of distributing the player's id.
    /// </summary>
    [BoltGlobalBehaviour(BoltNetworkModes.Client, BoltScenes.MainMenu)]
    public class PlayerCallbacksMainMenu : Bolt.GlobalEventListener
    {
        #region Custom Bolt Events
        /// <summary>
        /// First, the player triggers the connection process to the server. The server accepts the connection.
        /// Upon a successful connection on the client side, the lobby panel is displayed.
        /// On the server side, the Connected event stores the client's connection and assigns an ID, which is distributed using this event.
        /// So that the player (client) knows his ID for future communication relevance.
        /// </summary>
        /// <param name="evnt">Event data which holds the player's id</param>
        public override void OnEvent(AssignPlayerID evnt)
        {
            PlayerInformation.myPlayer.id = evnt.playerId; // store the assigned ID

            // After storing the ID, tell the server the player's name
            var ev = EnrollPlayerName.Create(Bolt.GlobalTargets.OnlyServer);
            ev.name = PlayerInformation.myPlayer.playerName;
            ev.playerId = PlayerInformation.myPlayer.id;
            ev.Send();
        }

        /// <summary>
        /// Enroll player entities to the player's list with the help of the PlayerIdToken
        /// </summary>
        /// <param name="entity">Any bolt entity which gets attached, but this function only affects player entities</param>
        public override void EntityAttached(BoltEntity entity)
        {
            if (entity.GetComponent<PlayerId>()) // this statement makes sure that only Player entities go through this logic, otherwise the GameController, which is a BoltEntity as well, will cause a NullRef
            {
                var playerIdToken = (PlayerIdToken)entity.attachToken;
                GetComponent<PlayerEntityAttacher>().SetPlayerMatchReady(playerIdToken.PlayerID, entity); // set the player's entity based on his id, a player is only match ready if his entity is stored in the player list
            }
        }
        #endregion
    }
}