using UnityEngine.UI;

namespace BRO.Game.PreMatch
{
    /// <summary>
    /// The chat listens to one custom Event retrieved by Bolt.GlobalEventListener. It also provides functionality to send events related to the chat.
    /// </summary>
    public class Chat : Bolt.GlobalEventListener
    {
        #region Member Fields
        public Text chatText;
        public InputField chatInput;
        private string m_myName;
        #endregion

        #region Unity Lifecycle
        /// <summary>
        /// Loads the player's name.
        /// </summary>
        void Start()
        {
            m_myName = PlayerInformation.myPlayer.playerName;
        }
        #endregion

        #region UI Events
        /// <summary>
        /// As soon as the content of the input field gets submitted, a ChatMessageEvent gets raised, so that everybody, who is listening on ChatMessageEvent, can receive the chat message.
        /// </summary>
        public void SubmitChatMessage()
        {
            if (chatInput.text.Length > 0)
            {
                var ev = ChatMessageEvent.Create(Bolt.GlobalTargets.Others);
                ev.message = chatInput.text;
                ev.sender = m_myName;
                ev.Send();
                chatText.text += System.Environment.NewLine + m_myName + " : " + chatInput.text;
                chatInput.text = "";
                chatInput.ActivateInputField();
            }
        }
        #endregion

        #region Custom Bolt Events
        /// <summary>
        /// Updates the UI text of the chat UI based on the incoming chat message.
        /// </summary>
        /// <param name="evnt">The ChatMessageEvents contains a message represented by a string.</param>
        public override void OnEvent(ChatMessageEvent evnt)
        {
            chatText.text += System.Environment.NewLine + evnt.sender + " : " + evnt.message;

            // Raise Sound Event
            var soundEvent = PlaySoundEvent.Create();
            soundEvent.audioClipIndex = (int)SoundClip.ChatMessage;
            soundEvent.Send();
        }
        #endregion
    }
}