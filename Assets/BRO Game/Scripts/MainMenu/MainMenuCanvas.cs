using BRO.Game.PreMatch;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The MainMenuCanvas, as root of the UI, is in charge of posting messages such as errors.
/// Usage: transform.root.GetComponent<MainMenuCanvas>().PostMessage("Bolt Start failed", "Bolt wasn't able to launch the server.");
/// The Bolt.GlobalEventListener inherits from Monobehavior. Documentation: https://doc.photonengine.com/en-us/bolt/current/in-depth/global-callbacks
/// </summary>
public class MainMenuCanvas : Bolt.GlobalEventListener
{
    #region Member Fields
    [Header("Warning Message Window")]
    [Tooltip("UI window which is capable of displaying a warning or error message.")]
    public GameObject warningMessage;
    [Tooltip("Title text component of the warning message")]
    public Text title;
    [Tooltip("Content text component of the warning message")]
    public Text message;
    #endregion

    #region Public Functions
    /// <summary>
    /// This function pops up a window with information related to a failure.
    /// </summary>
    /// <param name="title">Title of the warning or error message</param>
    /// <param name="message">Content of the warning or error message </param>
    public void PostMessage(string title, string message)
    {
        warningMessage.SetActive(true);
        this.title.text = title;
        this.message.text = message;
    }

    /// <summary>
    /// This function pops up a window with information related to a failure. It'll be closed automatically based on the displayTime parameter.
    /// </summary>
    /// <param name="title">Title of the warning or error message</param>
    /// <param name="message">Content of the warning or error message </param>
    /// <param name="displayTime">Duration of displaying the message</param>
    public void PostMessage(string title, string message, float displayTime)
    {
        warningMessage.SetActive(true);
        this.title.text = title;
        this.message.text = message;
        Invoke("DisableMessageWindow", displayTime);
    }
    #endregion

    #region Private Functions
    /// <summary>
    /// Disables the game object of the warning message to hide the pop up window.
    /// </summary>
    private void DisableMessageWindow()
    {
        warningMessage.SetActive(false);
    }
    #endregion

    #region Bolt Events
    /// <summary>
    /// This is the Bolt Event which is called right after the Bolt socket was started successfully. This event is used to register token classes.
    /// Documentation: https://doc.photonengine.com/en-us/bolt/current/in-depth/protocol-tokens
    /// </summary>
    public override void BoltStartBegin()
    {
        BoltNetwork.RegisterTokenClass<PlayerIdToken>();            // The PlayerIdToken carries the player's id for identification during the MainMenu/Lobby
    }
    #endregion
}