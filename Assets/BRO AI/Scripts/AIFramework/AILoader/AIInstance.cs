using UnityEngine;

namespace BRO.AI.Loader
{
    /// <summary>
    /// This class containts the relevant information of an AI, which is the AIBase class and an avatar sprite for the UI.
    /// </summary>
    public class AIInstance
    {
        /// <summary>
        /// Instance of the AIBase class.
        /// </summary>
        public AIBase Instance { get; set; }
        /// <summary>
        /// Avatar image of the AI, which is meant to be displayed on the UI.
        /// </summary>
        public Sprite AvatarSprite { get; set; }
    }
}
