using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BRO.Game.PreMatch
{   
    public class PlayerInitialization : Bolt.GlobalEventListener
    {
        #region Member Fields
        IGameControllerState m_gmcState;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            SceneManager.sceneLoaded += OnLevelFinishedLoading;
        }
        #endregion

        #region Local Functions
        private void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
        {
            if(scene.name.Equals("CoreMatch"))
            {
                SceneLoadLocalDone(scene.name);
                SceneManager.sceneLoaded -= OnLevelFinishedLoading;
            }
        }
        #endregion

        #region Bolt Events
        // SceneLoadLocalDone is invoked for each client.
        // Each client is spawned dependant on his id.
        public override void SceneLoadLocalDone(string map)
        {
            m_gmcState = GameController.Instance.state;

            // Prepare the PlayerIdToken to provide the id for the server on the CallBack EntityAttached()
            var token = new PlayerIdToken();
            token.PlayerID = PlayerInformation.myPlayer.id;

            if (BoltNetwork.isClient || m_gmcState.serverHasPlayer)
            {
                // The LookRotation of each player is set towards the middle of the pitch
                // Could still save the reference to the returned GameObject
                GameObject playerGo = BoltNetwork.Instantiate(BoltPrefabs.ProtoPowerBotPlayer, token,
                m_gmcState.players[PlayerInformation.myPlayer.id].spawnPoint, Quaternion.LookRotation(Vector3.zero - m_gmcState.players[PlayerInformation.myPlayer.id].spawnPoint));

                playerGo.GetComponent<PlayerId>().Id = PlayerInformation.myPlayer.id;
            }

            // Spawn ai players
            if (BoltNetwork.isServer)
            {
                for (int i = 0; i < m_gmcState.players.Length; i++)
                {
                    if (m_gmcState.players[i].isAi)
                    {
                        token.PlayerID = i;
                        GameObject aiGo = BoltNetwork.Instantiate(BoltPrefabs.ProtoPowerBotPlayer, token, m_gmcState.players[i].spawnPoint, Quaternion.LookRotation(Vector3.zero - m_gmcState.players[i].spawnPoint));
                        aiGo.GetComponent<PlayerId>().Id = i;
                        aiGo.GetComponent<PlayerId>().IsAi = true;
                    }
                }
            }
        }
        #endregion
    }
}