using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BRO.MatchAutomation
{
    public class MatchAutomationUI : MonoBehaviour
    {
        #region Member Fields
        [SerializeField]
        private Toggle m_speedTogggle;
        [SerializeField]
        private Slider m_speedSlider;
        [SerializeField]
        private InputField m_speedInput;
        [SerializeField]
        private MatchAutomationManager m_automationManager;

        [SerializeField]
        private Text m_matchId;
        [SerializeField]
        private Text m_matchTag;
        [SerializeField]
        private Text m_matchName;
        [SerializeField]
        private Text m_matchProgess;
        [SerializeField]
        private Text m_lapProgess;
        [SerializeField]
        private Text m_progress;
        [SerializeField]
        private Text m_elapsedTime;

        private DateTime m_startTime;
        #endregion

        #region Unity LifeCycle
        /// <summary>
        /// Initialization of member fields.
        /// </summary>
        private void Start()
        {
            m_startTime = m_automationManager.StartTime;
        }

        /// <summary>
        /// Updates the UI.
        /// </summary>
        private void Update()
        {
            if(!m_speedSlider.interactable)
                m_speedSlider.value = Time.timeScale;

            if (!m_speedInput.interactable)
                m_speedInput.text = Time.timeScale.ToString();

            TimeSpan elapsedTime = DateTime.Now - m_startTime;
            m_elapsedTime.text = elapsedTime.Days + "d " + elapsedTime.Hours + "h " + elapsedTime.Minutes + "m " + elapsedTime.Seconds + "s";

            m_matchId.text = "#" + m_automationManager.CurrentMantch.Id.ToString();
            m_matchName.text = m_automationManager.CurrentMantch.Name;
            m_matchTag.text = "#" + m_automationManager.CurrentMantch.Tag;
            m_matchProgess.text = (m_automationManager.CurrentMatchIndex + 1) + "/" + m_automationManager.MatchCount;
            if (!m_automationManager.InfiniteLaps)
            {
                m_lapProgess.text = (m_automationManager.CurrentLap + 1) + "/" + m_automationManager.Laps;
                m_progress.text = (m_automationManager.ProcessedMatchCount + 1) + "/" + (m_automationManager.MatchCount * m_automationManager.Laps) +
                    "  " + String.Format("{0:P2}.", ((float)(m_automationManager.ProcessedMatchCount) / (float)(m_automationManager.MatchCount * m_automationManager.Laps)));
            }
            else
            {
                m_lapProgess.text = (m_automationManager.CurrentLap + 1) + "/Inf.";
                m_progress.text = (m_automationManager.ProcessedMatchCount + 1) + "/Inf.";
            }
        }
        #endregion

        #region UI Events
        /// <summary>
        /// Turns on/off the manual simulation speed control.
        /// </summary>
        public void ToggleSpeedUI()
        {
            m_automationManager.ToggleSimulationSpeed();
            m_speedSlider.interactable = m_speedTogggle.isOn;
            m_speedInput.interactable = m_speedTogggle.isOn;
        }

        /// <summary>
        /// Sets the simulation speed aka timeScale.
        /// </summary>
        public void OnSliderValueChanged()
        {
            Time.timeScale = m_speedSlider.value;
            m_speedInput.text = m_speedSlider.value.ToString();
        }

        /// <summary>
        /// Sets the simulation speed aka timeScale.
        /// </summary>
        public void OnInputChanged()
        {
            m_speedSlider.value = float.Parse(m_speedInput.text);
            Time.timeScale = m_speedSlider.value;
        }
        #endregion
    }
}