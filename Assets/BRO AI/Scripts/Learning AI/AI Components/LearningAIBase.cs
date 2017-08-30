using UnityEngine;

namespace BRO.AI.Learning
{
    public class LearningAIBase : AIBase
    {
        #region Instructions!!!
        // Instructions for developing a new AI:
        //
        // First of all you have to create a class which derives from AIBaseClass.
        // This abstract class makes sure that on run-time your AI can be found with its distinct information and logic.
        // These information describe your AI. This information has to be supplied using GetAIInformation().
        // The logic for your AI has to be referenced in AddAIComponent(GameObject).
        // Within that function, use AddComponent<T>() (T a type of Monobehaviour, AIBehaviour or AIEventListener) to add your AI's logic.
        // This functions is in charge of adding the AI logic to the player's GameObject.
        // For the sake of ease, you can add these information as member variables like below.
        // You may use this example as a boilerplate.
        #endregion

        #region Member Fields
        private const string m_name = "Learning AI #1";
        private const string m_author = "ARP Team";
        private const string m_affinity = "Hochschule Rhein-Waal";
        private const string m_description = "This AI is intended for trying out learning behaviors´.";
        private const AITag m_tag = AITag.LearningAI;
        private const string m_version = "1.0";
        private const string m_date = "May 2017";
        private const string m_pictureFileName = "noImage.jpg";
        #endregion

        #region Abstract Functions
        /// <summary>
        /// Makes an instance of the stored AI information.
        /// </summary>
        /// <returns>Returns the information of this particular AI instance.</returns>
        public override AIInformation GetAIInformation()
        {
            return new AIInformation(m_name, m_author, m_affinity, m_description, m_tag, m_version, m_date, m_pictureFileName);
        }

        /// <summary>
        /// Adds the Monobehaviors containing the AI logic to the player's GameObject.
        /// </summary>
        /// <param name="go">GameObject of the Player.</param>
        public override void AddAIComponents(GameObject go)
        {
            //go.AddComponent<OldLearningAIBehaviour>();
            go.AddComponent<LearningAgent>();
        }
        #endregion
    }
}
