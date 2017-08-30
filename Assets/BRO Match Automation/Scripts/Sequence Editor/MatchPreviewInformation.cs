using UnityEngine;
using UnityEngine.UI;

namespace BRO.SequenceEditor
{
    /// <summary>
    /// The Match Preview UI.
    /// </summary>
    public class MatchPreviewInformation : MonoBehaviour
    {
        #region Member Fields
        [SerializeField]
        private Text m_matchName;
        [SerializeField]
        private Text m_tag;
        [SerializeField]
        private Text[] m_playerTexts;
        [SerializeField]
        private Text m_lifes;
        [SerializeField]
        private Text m_description;
        #endregion

        #region Member Properties
        #endregion

        #region Unity Lifecycle
        void Start()
        {

        }

        void Update()
        {

        }
        #endregion

        #region UI Events
        #endregion

        #region Public Functions
        /// <summary>
        /// Update the Preview according to the match information.
        /// </summary>
        /// <param name="match"></param>
        public void UpdateUI(Match match)
        {
            m_matchName.text = match.Name;
            m_tag.text = match.Tag;
            for (int i = 0; i < match.AIPlayers.Length; i++)
            {
                if (match.AIPlayers[i] != null)
                {
                    m_playerTexts[i].text = match.AIPlayers[i].GetAIInformation().GetName();
                }
                else
                {
                    m_playerTexts[i].text = "-";
                }
            }

            if (!match.InfiniteLives)
            {
                m_lifes.text = match.Lives.ToString();
            }
            else
            {
                m_lifes.text = "Infinite Lives";
            }
            m_description.text = match.Description;
        }
        #endregion

        #region Private Functions
        #endregion
    }
}