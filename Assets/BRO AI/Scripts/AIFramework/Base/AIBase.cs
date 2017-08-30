using UnityEngine;

/// <summary>
/// This is the AIBase class that needs to be inherited by any new AI
/// </summary>
namespace BRO.AI
{
    [System.Serializable]
    public abstract class AIBase
    {

        /// <summary>
        /// This method must be implemented by each AI!
        /// should return information about the AI
        /// e.g.: Name, difficulty, ...
        /// </summary>
        /// <returns></returns>

        public abstract AIInformation GetAIInformation();
        public abstract void AddAIComponents(GameObject go);
    }
}