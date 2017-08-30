using UnityEngine;
using UnityEngine.UI;

namespace BRO.AI.Learning
{
    /// <summary>
    /// This Canvas visualizes Q-Learning data.
    /// </summary>
    public class DeepQCanvas : MonoBehaviour
    {
        #region Member Fields
        [SerializeField]
        private Text m_currentAction;
        [SerializeField]
        private Text m_age;
        [SerializeField]
        private Text m_experienceReplaySize;
        [SerializeField]
        private Text m_explorationEpsilon;
        [SerializeField]
        private Text m_averageQLoss;
        [SerializeField]
        private Text m_smoothReward;
        #endregion

        #region Public Functions
        /// <summary>
        /// Updates the UI according to the Q-Learning properties.
        /// </summary>
        /// <param name="age"></param>
        /// <param name="replaySize"></param>
        /// <param name="explorationEpsilon"></param>
        /// <param name="loss"></param>
        /// <param name="reward"></param>
        public void SetValues(int age, int replaySize, double explorationEpsilon, double loss, double reward)
        {
            m_age.text = "Age: " + age;
            m_experienceReplaySize.text = "Experience Replay Size: " + replaySize;
            m_explorationEpsilon.text = "Exploration Epsilon: " + explorationEpsilon;
            m_averageQLoss.text = "Average Q-Learning Loss: " + loss;
            m_smoothReward.text = "Smooth-ish Reward: " + reward;
        }

        public void SetValue(OutputAction action)
        {
            m_currentAction.text = action.ToString();
        }
        #endregion
    }
}