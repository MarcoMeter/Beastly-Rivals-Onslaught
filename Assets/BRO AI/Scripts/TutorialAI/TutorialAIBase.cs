using UnityEngine;

namespace BRO.AI.Framework.Tutorial
{
    [System.Serializable]
    public class TutorialAIBase : AIBase
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
        private const string m_name = "Tutorial AI";
        private const string m_author = "Marco Pleines";
        private const string m_affinity = "Hochschule Rhein-Waal";
        private const string m_description =    "This is the final AI which is created throughout the tutorial, which can be retrieved from the git repository's wiki."
                                                + "It provides more detailed information in comparison to the code snippets of the tutorial.";
        private const AITag m_tag = AITag.Tutorial;
        private const string m_version = "1.0";
        private const string m_date = "March 2017";
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
            go.AddComponent<TutorialAI>();
        }
        #endregion
    }
}