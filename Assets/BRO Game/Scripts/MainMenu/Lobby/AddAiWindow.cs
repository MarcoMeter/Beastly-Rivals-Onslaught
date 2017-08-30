using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using BRO.AI.Loader;

namespace BRO.Game.PreMatch
{
    /// <summary>
    /// UI functionality for the AddAiWindoiw, which is capable of adding AI to a match.
    /// </summary>
    public class AddAiWindow : MonoBehaviour
    {
        #region Member Fields
        // UI Objects
        [SerializeField]
        private Dropdown m_aiDropdown;
        [SerializeField]
        private Image m_aiAvatarImage;
        [SerializeField]
        private Text m_aiVersionText;
        [SerializeField]
        private Text m_aiBuildDateText;
        [SerializeField]
        private Text m_aiDescriptionText;
        [SerializeField]
        private Text m_aiAuthorText;
        [SerializeField]
        private Text m_aiAffinityText;
        [SerializeField]
        private Sprite m_placeHolderAvatar;
        private const string m_versionPrefix = "Version ";
        [SerializeField]
        private LobbyPanel m_lobbyPanel;

        // AI Fields Objects
        private List<AIInstance> m_availableAIs;
        #endregion

        #region Unity Lifecycle
        /// <summary>
        /// OnEnable is triggered as soon as the gameobject got enabled. It tirggers the dropdown initialization.
        /// </summary>
        void OnEnable()
        {
            InitAIDropdown();
        }
        #endregion

        #region UI Events
        /// <summary>
        /// UI Event which is triggered as soon as soon as the selected option of the dropdown got changed.
        /// </summary>
        public void DropdownValueChanged()
        {
            UpdateUI();
        }

        /// <summary>
        /// Starts the process of adding AI to the match by informing the lobby
        /// </summary>
        public void AddAI()
        {
            m_lobbyPanel.AddAI(m_availableAIs[m_aiDropdown.value].Instance);
        }
        #endregion

        #region Private Functions
        /// <summary>
        /// Initiates the dropdown menu based on available AIs
        /// </summary>
        private void InitAIDropdown()
        {
            // Clear dropdown options
            m_aiDropdown.ClearOptions();
            // Retrieve AIs
            var aiLoader = new AILoader();
            m_availableAIs = aiLoader.GetAIInstances();

            foreach (var ai in m_availableAIs)
            {
                m_aiDropdown.options.Add(new Dropdown.OptionData(ai.Instance.GetAIInformation().GetName()));
            }

            UpdateUI();
        }

        /// <summary>
        /// Updates the UI based on the selected option of the dropdown menu.
        /// </summary>
        private void UpdateUI()
        {
            m_aiDropdown.captionText.text = m_availableAIs[m_aiDropdown.value].Instance.GetAIInformation().GetName();
            m_aiVersionText.text = m_versionPrefix + m_availableAIs[m_aiDropdown.value].Instance.GetAIInformation().GetVersion();
            m_aiBuildDateText.text = m_availableAIs[m_aiDropdown.value].Instance.GetAIInformation().GetDate(); ;
            m_aiAvatarImage.sprite = m_placeHolderAvatar;
            m_aiDescriptionText.text = m_availableAIs[m_aiDropdown.value].Instance.GetAIInformation().GetDescription();
            m_aiAuthorText.text = m_availableAIs[m_aiDropdown.value].Instance.GetAIInformation().GetAuthor();
            m_aiAffinityText.text = m_availableAIs[m_aiDropdown.value].Instance.GetAIInformation().GetAffinity();

            // Display avatar sprite if available
            if (m_availableAIs[m_aiDropdown.value].AvatarSprite)
            {
                m_aiAvatarImage.sprite = m_availableAIs[m_aiDropdown.value].AvatarSprite;
            }
            else
            {
                m_aiAvatarImage.sprite = m_placeHolderAvatar;
            }
        }
        #endregion
    }
}