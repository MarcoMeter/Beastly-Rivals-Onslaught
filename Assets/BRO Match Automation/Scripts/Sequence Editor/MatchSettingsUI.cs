using BRO.AI;
using BRO.AI.Loader;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace BRO.SequenceEditor
{
    /// <summary>
    /// The UI for adding and modifying match of the match sequence.
    /// </summary>
    public class MatchSettingsUI : MonoBehaviour
    {
        #region Member Fields
        private Color m_color = new Color(245.0f / 255.0f, 245.0f / 255.0f, 245.0f / 255.0f);

        [SerializeField]
        private MatchSequenceEditor m_sequenceEditor;
        [SerializeField]
        private MatchItem m_matchItem;
        [SerializeField]
        private GameObject m_addMatchTitle;
        [SerializeField]
        private GameObject m_modifyMatchTitle;
        [SerializeField]
        private GameObject m_modifyMatchTitleId;
        [SerializeField]
        private GameObject m_createMatchButton;
        [SerializeField]
        private GameObject m_modifyMatchButton;
        [SerializeField]
        private Text m_IdText;
        [SerializeField]
        private InputField m_nameInput;
        [SerializeField]
        private Dropdown m_tagInput;
        [SerializeField]
        private Image[] m_aiAvatarImages;
        [SerializeField]
        private Text[] m_aiNameTexts;
        private AIBase[] m_aiBases = new AIBase[8];
        [SerializeField]
        private Slider m_lifeCountSlider;
        [SerializeField]
        private Text m_lifeCountText;
        [SerializeField]
        private Toggle m_infLivesToggle;
        [SerializeField]
        private InputField m_descriptionInput;
        [SerializeField]
        private Button m_colorRed;
        [SerializeField]
        private Button m_colorYellow;
        [SerializeField]
        private Button m_colorGreen;
        [SerializeField]
        private Button m_colorBlue;
        [SerializeField]
        private Button m_colorGrey;
        [SerializeField]
        private Button m_colorOrigin;
        [SerializeField]
        private GameObject m_editAIWindow;

        private int m_slotToEdit = 0;
        [SerializeField]
        private Sprite m_placeHolderAvatar;
        private Match m_matchBackup;
        #endregion

        #region Member Properties
        #endregion

        #region Select Color
        /// <summary>
        /// The Select Color region contains the onClick function of the colored button in changing the color of the match button.
        /// </summary>
        /// <param name="presetColorGo">The gameObject, which holds the button with the color information</param>
        public void OnColorClick(GameObject presetColorGo)
        {
            m_color = presetColorGo.GetComponent<Button>().colors.normalColor;
        }
        #endregion

        #region UI Events
        /// <summary>
        /// Initializes the AddMatchSettingsUI.
        /// Adds initial AIs to the first two slots.
        /// </summary>
        public void OnShowAdd()
        {
            // Hide the title and button for modification
            m_modifyMatchTitle.SetActive(false);
            m_modifyMatchTitleId.SetActive(false);
            m_modifyMatchButton.SetActive(false);

            // Show the title and button for add match
            m_addMatchTitle.SetActive(true);
            m_createMatchButton.SetActive(true);

            m_nameInput.text = "Match Name";

            for (int i = 0; i < m_tagInput.options.Count; i++)
            {
                if (m_tagInput.options[i].text == "Training")
                {
                    m_tagInput.value = i;
                }
            }

            m_aiBases = new AIBase[8];
            var aiLoader = new AILoader();
            var availableAIs = aiLoader.GetAIInstances();
            m_slotToEdit = 0;
            AddAI(availableAIs[0].Instance, availableAIs[0].AvatarSprite);
            m_slotToEdit = 1;
            AddAI(availableAIs[0].Instance, availableAIs[0].AvatarSprite);
            for (int i = 2; i < m_aiBases.Length; i++)
            {
                OnDeleteAISlot(i);
            }

            m_lifeCountSlider.value = 2;
            m_lifeCountText.text = m_lifeCountSlider.value.ToString();
            m_infLivesToggle.isOn = false;

            m_color = new Color(245, 245, 245);
            m_descriptionInput.text = "";
        }

        /// <summary>
        /// Show the information of the match on the Modify UI.
        /// </summary>
        /// <param name="match"></param>
        public void OnShowModify(Match match)
        {
            m_matchBackup = match;

            // Hide the title and button for add match.
            m_addMatchTitle.SetActive(false);
            m_createMatchButton.SetActive(false);

            // Show the title and button for modification.
            m_modifyMatchTitle.SetActive(true);
            m_modifyMatchTitleId.SetActive(true);
            m_modifyMatchButton.SetActive(true);

            m_IdText.text = m_matchBackup.Id.ToString();
            m_nameInput.text = m_matchBackup.Name;

            for (int i = 0; i < m_tagInput.options.Count; i++)
            {
                if (m_tagInput.options[i].text == m_matchBackup.Tag)
                {
                    m_tagInput.value = i;
                }
            }

            m_aiBases = (AIBase[])m_matchBackup.AIPlayers.Clone();
            for (int i = 0; i < m_matchBackup.AIPlayers.Length; i++)
            {
                if (m_matchBackup.AIPlayers[i] != null)
                {
                    m_aiNameTexts[i].text = m_matchBackup.AIPlayers[i].GetAIInformation().GetName();
                }
                else
                {
                    OnDeleteAISlot(i);
                }
            }

            m_lifeCountSlider.value = m_matchBackup.Lives;
            m_lifeCountText.text = m_lifeCountSlider.value.ToString();
            m_infLivesToggle.isOn = m_matchBackup.InfiniteLives;

            m_color = m_matchBackup.Color;
            m_descriptionInput.text = m_matchBackup.Description;
        }

        /// <summary>
        /// Updates the text next to the slider to show the current value of the slider.
        /// </summary>
        public void OnLifeCountChanged()
        {
            m_lifeCountText.text = m_lifeCountSlider.value.ToString();
        }

        /// <summary>
        /// Sets the slot to edit.
        /// </summary>
        /// <param name="slot">Slot to edit</param>
        public void OnEditAISlot(int slot)
        {
            m_editAIWindow.SetActive(true);
            m_slotToEdit = slot;
        }

        /// <summary>
        /// Removes an AI from the specified AI slot.
        /// </summary>
        /// <param name="slot">AI slot to remove.</param>
        public void OnDeleteAISlot(int slot)
        {
            m_aiBases[slot] = null;
            m_aiNameTexts[slot].text = "";
            m_aiAvatarImages[slot].sprite = null;
            m_aiAvatarImages[slot].color = new Color(238.0f / 255.0f, 238.0f / 255.0f, 238.0f / 255.0f);
        }

        /// <summary>
        /// If the Create button on the Add UI is clicked, create match according to the information on the Add UI then hide the Add UI.
        /// </summary>
        public void OnCreateMatch()
        {
            m_sequenceEditor.CreateMatch(new Match(m_nameInput.text, m_tagInput.options[m_tagInput.value].text, m_aiBases, (int)m_lifeCountSlider.value, m_infLivesToggle.isOn, m_color, m_descriptionInput.text));
            gameObject.SetActive(false);
        }

        /// <summary>
        /// If the Modify button on the Modify UI is clicked.
        /// First, get the match ID through the match item and select the match item from matchList.
        /// Then, update the information of the match according to the Modify UI and display the new modified match item.
        /// Hide the Modify UI.
        /// </summary>
        public void OnModifyMatch()
        {
            // get the match id of the match item
            int myMatchId = Int32.Parse(m_IdText.text);

            // get the MatchItem from the list according to the id
            Match myMatch = m_sequenceEditor.m_matchList[myMatchId];

            // update information in the MatchItem of the list
            myMatch.Name = m_nameInput.text;
            myMatch.Tag = m_tagInput.options[m_tagInput.value].text;
            myMatch.AIPlayers = m_aiBases;
            myMatch.Color = m_color;
            myMatch.Lives = (int)m_lifeCountSlider.value;
            myMatch.InfiniteLives = m_infLivesToggle.isOn;
            myMatch.Description = m_descriptionInput.text;

            // update MatchItem display
            m_matchItem.ReceiveMatchData(myMatch);
            for (int i = 0; i < m_sequenceEditor.m_matchList.Count; i++)
            {
                m_sequenceEditor.m_matchButtonList[i].GetComponent<MatchItem>().UpdateMatchData();
            }

            // hide modify match UI
            gameObject.SetActive(false);
            m_aiBases = new AIBase[8];
        }

        /// <summary>
        /// If the Cancel button on the Add UI is clicked, hide Add UI. Same for the Modify UI.
        /// </summary>
        public void OnCancel()
        {
            int myMatchId = Int32.Parse(m_IdText.text);
            if (m_sequenceEditor.m_matchList.Count > 0)
            {
                m_sequenceEditor.m_matchList[myMatchId] = m_matchBackup;
            }
            gameObject.SetActive(false);
            m_aiBases = new AIBase[8];
        }
        #endregion

        #region Unity Lifecycle
        #endregion

        #region Public Functions
        /// <summary>
        /// Adds or modifies an AI to the slot which has been selected to edit by the edit button.
        /// </summary>
        /// <param name="ai">AI to assign</param>
        /// <param name="avatar">Avatar of the AI to assign</param>
        public void AddAI(AIBase ai, Sprite avatar)
        {
            m_aiBases[m_slotToEdit] = ai;
            m_aiNameTexts[m_slotToEdit].text = ai.GetAIInformation().GetName();
            if (avatar != null)
            {
                m_aiAvatarImages[m_slotToEdit].sprite = avatar;
            }
            else
            {
                m_aiAvatarImages[m_slotToEdit].sprite = m_placeHolderAvatar;
            }
            m_aiAvatarImages[m_slotToEdit].color = Color.white;
        }
        #endregion

        #region Private Functions
        #endregion
    }
}