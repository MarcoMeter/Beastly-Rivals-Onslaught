using BRO.Game;
using UnityEngine;

/// <summary>
/// Provides Functions for UI Events
/// </summary>
public class MainPanel : MonoBehaviour 
{
    #region Member Fields
    private const string m_SEQUENCE_EDITOR_SCENE_NAME = "MatchSequenceEditor";
    #endregion

    #region OnClick
    /// <summary>
    /// Exits the game.
    /// </summary>
    public void ExitGame()
    {
        Application.Quit();
    }

    /// <summary>
    /// Loads the MatchSequenceEditor.
    /// </summary>
    public void LoadMatchSequenceEditor()
    {
        LoadingFlags flags = new LoadingFlags() { Local = true, ShowInfo = false, UseFade = false, UseLoadingScreen = false, WaitForInput = false };
        LoadSceneManager.Instance.LoadScene(m_SEQUENCE_EDITOR_SCENE_NAME, flags);
    }

    /// <summary>
    /// Button event to open the related asset store page of the PowerBot asset.
    /// </summary>
    public void OnPowerBotClick()
    {
        Application.OpenURL("https://www.assetstore.unity3d.com/en/#!/content/18136");
    }
    #endregion
}
