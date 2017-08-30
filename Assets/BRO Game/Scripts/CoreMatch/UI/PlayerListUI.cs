using UnityEngine;
using UnityEngine.UI;

namespace BRO.Game
{
    /// <summary>
    /// This is the behaviour for updating the Player List UI.
    /// </summary>
    public class PlayerListUI : MonoBehaviour
    {
        #region Member Fields
        [SerializeField]
        private Text[] m_playerNames;
        [SerializeField]
        private Text[] m_playerLifes;
        [SerializeField]
        private Text[] m_playerKills;
        [SerializeField]
        private GameObject[] m_playerDeadImage;
        private IGameControllerState m_gmcState;
        #endregion

        #region Unity Lifecycle
        void Start()
        {
            m_gmcState = GameController.Instance.state;
            m_gmcState.AddCallback("players[]", OnStatsChanged);
        }
        #endregion

        #region Callback
        /// <summary>
        /// Updates the player list UI.
        /// </summary>
        private void OnStatsChanged()
        {
            for (int i = 0; i < 8; i++)
            {
                if (m_gmcState.players[i].matchReady)
                {
                    m_playerNames[i].text = m_gmcState.players[i].playerName;
                    m_playerLifes[i].text = m_gmcState.players[i].lifes.ToString();
                    m_playerKills[i].text = m_gmcState.players[i].kills.ToString();
                    if(m_gmcState.players[i].isGameOver && (PlayerState)m_gmcState.players[i].playerGameState == PlayerState.DeadState)
                    {
                        m_playerDeadImage[i].SetActive(true);
                    }
                }
            }
        }
        #endregion
    }
}