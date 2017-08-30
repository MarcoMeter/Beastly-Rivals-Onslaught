using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The PlayerSlot is a collection of UI elements to define a PlayerSlot component.
/// </summary>
public class PlayerSlot : MonoBehaviour
{
    #region Member Fields
    public Image readyImage;
    public Image notReadyImage;
    public Text playerName;
    public Image playerColor;
    public Button killSlotButton;
    public Button addSlotButton;
    #endregion

    #region Unity Lifecycle
    /// <summary>
    /// Disables buttons, which are not supposed to be used by the clients.
    /// </summary>
    void Start()
    {
        if(BoltNetwork.isClient)
        {
            if(killSlotButton)
                killSlotButton.gameObject.SetActive(false);
            if (addSlotButton)
                addSlotButton.gameObject.SetActive(false);
        }
    }
    #endregion
}