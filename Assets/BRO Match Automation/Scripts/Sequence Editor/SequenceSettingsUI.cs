using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace BRO.SequenceEditor
{
    /// <summary>
    /// The UI for Sequence Settings.
    /// </summary>
    public class SequenceSettingsUI : MonoBehaviour
    {

        #region Member Fields
        [SerializeField]
        private MatchSequenceEditor m_sequenceEditor;
        [SerializeField]
        private InputField m_loopInput;
        [SerializeField]
        private Toggle m_infiniteToggle;
        [SerializeField]
        private Toggle m_shuffleToggle;
        #endregion

        #region Member Properties
        #endregion

        #region UI Toggle Events
        /// <summary>
        /// If "Infinite" Toggle is on, loopInput input field is not interactable. Vice versa.
        /// </summary>
        public void isInfinite()
        {
            m_loopInput.interactable = !m_infiniteToggle.isOn;
        }
        #endregion

        #region UI Events
        /// <summary>
        /// Show the information of the match sequence settings on the Sequence Settings UI.
        /// </summary>
        /// <param name="sequence"></param>
        public void OnShowSequenceSettings(SequenceSettings sequence)
        {
            m_loopInput.text = sequence.Laps.ToString();
            m_infiniteToggle.isOn = sequence.Infinite;
            m_shuffleToggle.isOn = sequence.Shuffle;
        }

        /// <summary>
        /// If the Confirm button on the Sequence Settings UI is clicked.
        /// Save the sequence information and hide the Sequence Settings UI.
        /// </summary>
        public void OnConfirmSettings()
        {
            m_sequenceEditor.SetMatchSequence(int.Parse(m_loopInput.text), m_infiniteToggle.isOn, m_shuffleToggle.isOn);
            gameObject.SetActive(false);
        }

        /// <summary>
        /// If the Cancel button on the Sequence Settings UI is clicked, hide the Sequence Settings UI.
        /// </summary>
        public void OnCancel()
        {
            gameObject.SetActive(false);
        }
        #endregion

        #region Unity Lifecycle
        void Start()
        {

        }

        void Update()
        {

        }
        #endregion

        #region Private Functions
        #endregion
    }
}