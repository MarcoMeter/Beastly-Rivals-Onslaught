using UnityEngine;
using BRO.Game;
using BRO;

/// <summary>
/// Visualizes debug information of an AI player.
/// </summary>
public class AIDebug : MonoBehaviour
{
    #region Member Fields
    [SerializeField]
    private PlayerId m_playerIdComponent;
    [SerializeField]
    private GameObject m_crossPrefab;
    private GameObject m_placedCross;
    private LineRenderer m_movementLine;
    private PlayerAliveState m_aliveState;
    private Color m_playerColor;
    #endregion

    #region Unity Lifecycle
    /// <summary>
    /// Initializes callback delegates.
    /// </summary>
    private void Start()
    {
        m_aliveState = GetComponent <PlayerAliveState>();
        m_aliveState.DestinationCallback += OnDestinationChanged;
        DebugManager.Instance.DebugModeCallback += OnDebugModeChanged;
        m_playerColor = GameController.Instance.state.players[m_playerIdComponent.Id].playerColor;
    }
    #endregion

    #region Local Functions
    /// <summary>
    /// Destroys the destination GameObject if one is present in the case of being not in debug mode.
    /// </summary>
    /// <param name="isDebug">True, if debug mode is enabled</param>
    private void OnDebugModeChanged(bool isDebug)
    {
        if (!isDebug)
        {
            if (m_placedCross)
            {
                Destroy(m_placedCross);
            }
            if (m_movementLine)
            {
                Destroy(m_movementLine.gameObject);
            }
        }
    }

    /// <summary>
    /// Places a cross as desired destination to walk to.
    /// </summary>
    /// <param name="isDestinationSet">True if a destination is set</param>
    /// <param name="destination">Location of the desifnated movement destination</param>
    private void OnDestinationChanged(bool isDestinationSet, Vector3 destination)
    {
        // Delete previously existing crosses
        if (m_placedCross)
        {
            Destroy(m_placedCross);
        }

        // Delete existing lines
        if(m_movementLine)
        {
            Destroy(m_movementLine.gameObject);
        }

        // In debug mode, place a cross on the destination location.
        if (DebugManager.Instance.IsDebugMode && isDestinationSet)
        {
            m_placedCross = Instantiate(m_crossPrefab, destination, Quaternion.Euler(90.0f, 0.0f, 0.0f));
            m_placedCross.GetComponent<SpriteRenderer>().color = m_playerColor;
            m_movementLine = DebugManager.Instance.DrawLine(transform.position, destination, m_playerColor);
        }
    }
    #endregion
}
