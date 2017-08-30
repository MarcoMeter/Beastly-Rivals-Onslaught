namespace BRO.Game
{
    public class PlayerEntityAttacher : Bolt.EntityEventListener<IGameControllerState>
    {
        #region Public Functions
        /// <summary>
        /// SetPlayerMatchReady finalizes the player list right before the match starts.
        /// As soon as each player spawned his virtual representation, the player is ready for the match.
        /// This is run in the CoreMatch scene.
        /// </summary>
        /// <param name="playerId">ID of the player</param>
        /// <param name="playerEntity">Entity of the virtual presentation of the player</param>
        public void SetPlayerMatchReady(int playerId, BoltEntity playerEntity)
        {
            state.players[playerId].playerEntity = playerEntity;
            state.players[playerId].matchReady = true;
            state.players[playerId].isGameOver = false;

            int countReady = 0;
            for (int i = 0; i < state.players.Length; i++)
            {
                if (state.players[i].matchReady)
                {
                    countReady++;
                }
            }

            if (countReady == state.playersConnected)
            {
                for (int i = 0; i < state.players.Length; i++)
                {
                    if (state.players[i].available)
                        PlayersMatchReadyEvent.Create(state.players[i].playerEntity).Send();
                }
                GameController.Instance.TransitionToMatchFlow();
            }
        }
        #endregion
    }
}