using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BRO.SequenceEditor
{
    /// <summary>
    /// Match item that display on the screen.
    /// </summary>
    public class MatchItem : EventTrigger
    {
        #region Member Fields
        private Match m_myMatch;
        [SerializeField]
        private Text m_idText;
        [SerializeField]
        private Text m_nameText;
        [SerializeField]
        private Text m_tagText;
        [SerializeField]
        private Button m_deleteButton;
        [SerializeField]
        private Image m_colorImage;
        [SerializeField]
        private Text[] m_aiPlayerTexts;
        [SerializeField]
        private GameObject m_matchItemShadow;
        private GameObject m_matchShadowInstance;
        private MatchSequenceEditor m_sequenceEditor;
        #endregion

        #region Member Properties
        public int Id
        {
            get { return m_myMatch.Id; }
        }
        #endregion

        #region Unity Lifecycle
        /// <summary>
        /// Initializes the MatchItem.
        /// </summary>
        private void Start()
        {
            m_sequenceEditor = gameObject.transform.root.GetComponent<MatchSequenceEditor>();
        }
        #endregion

        #region UI EventTrigger
        /// <summary>
        /// When pointer enters the match item, display Preview.
        /// </summary>
        /// <param name="eventData"></param>
        public override void OnPointerEnter(PointerEventData eventData)
        {
            m_sequenceEditor.DisplayMatchPreview(m_myMatch);
        }

        /// <summary>
        /// When pointer exits the match item, hide the Preview.
        /// </summary>
        /// <param name="eventData"></param>
        public override void OnPointerExit(PointerEventData eventData)
        {
            m_sequenceEditor.StopDisplayMatchPreview();
        }

        /// <summary>
        /// When pointer clicks the match item, show Modify UI through OnMatchItemClick.
        /// </summary>
        /// <param name="eventData"></param>
        public override void OnPointerClick(PointerEventData eventData)
        {
            m_sequenceEditor.OnMatchItemClick(m_myMatch);
        }

        /// <summary>
        /// When the pointer begins dragging, instantiate the shadow of the match item and make the shadow follows the mouse position.
        /// </summary>
        /// <param name="eventData"></param>
        public override void OnBeginDrag(PointerEventData eventData)
        {
            m_matchShadowInstance = Instantiate(m_matchItemShadow);
            m_matchShadowInstance.transform.SetParent(transform.root);
            m_matchShadowInstance.transform.position = transform.position;
            m_matchShadowInstance.GetComponent<FollowMousePosition>().DefineOffset(transform.position);
        }

        /// <summary>
        /// When the pointer stops dragging, destroy the shadow/
        /// Check if the object below is a different match item and re-order the match sequence.
        /// </summary>
        /// <param name="eventData"></param>
        public override void OnEndDrag(PointerEventData eventData)
        {
            GameObject eventGameObject = eventData.pointerCurrentRaycast.gameObject; // the object which is below the mouse right on this event (the match item is able to receive this kind of raycast)
            Destroy(m_matchShadowInstance); // destroy the match shadow instance
                                            // Check if the object below is a different match item GameObject
            if (eventGameObject != gameObject && eventGameObject.tag == gameObject.tag)
            {
                m_sequenceEditor.ReOrderMatches(m_myMatch.Id, eventGameObject.GetComponent<MatchItem>().Id); // tell the sequence controller where to place the dragged match
            }
        }
        #endregion

        #region Public Functions
        /// <summary>
        /// Add the received match data (ID, Name and Tag) to each text fields.
        /// Get information for info box.
        /// Mouse over: match information display.
        /// Mouse exit: match information not display.
        /// </summary>
        /// <param name="myMatch"></param>
        public void ReceiveMatchData(Match myMatch)
        {
            m_myMatch = myMatch;
            UpdateMatchData();
        }

        /// <summary>
        /// Updates the visual representation of the match UI
        /// </summary>
        public void UpdateMatchData()
        {
            m_idText.text = m_myMatch.Id.ToString();
            m_nameText.text = m_myMatch.Name;
            m_tagText.text = m_myMatch.Tag;
            m_colorImage.color = m_myMatch.Color;
            for (int i = 0; i < m_myMatch.AIPlayers.Length; i++)
            {
                if (m_myMatch.AIPlayers[i] != null)
                {
                    m_aiPlayerTexts[i].text = m_myMatch.AIPlayers[i].GetAIInformation().GetName();
                }
                else
                {
                    m_aiPlayerTexts[i].text = "-";
                }
            }
        }

        /// <summary>
        /// Triggers the deletion process for this particular match on the MatchSequenceController
        /// </summary>
        public void OnDelete()
        {
            m_sequenceEditor.DeleteMatch(m_myMatch);
        }
        #endregion
    }
}