using UnityEngine;

/// <summary>
/// This component modifies the color of the gameObject's renderer component randomly over time.
/// </summary>
public class BackgroundColorChanger : MonoBehaviour
{
    #region Member Fields
    private Color m_colorStart = Color.black;
    private Color m_colorEnd;
    private const float rate = 0.15f;
    private float m_timeCount = 0;
    private Renderer m_renderer;
    private bool m_changeColor = true;
    #endregion

    #region Unity Lifecycle
    /// <summary>
    /// Initialization of member fields
    /// </summary>
    private void Start()
    {
        m_colorEnd = new Color(Random.value, Random.value, Random.value);
        m_renderer = GetComponent<Renderer>();
    }

    /// <summary>
    /// Lerps the color
    /// </summary>
    private void Update()
    {
        if (m_changeColor)
        {
            m_timeCount += Time.deltaTime * rate;
            m_renderer.material.SetColor("_Color", Color.Lerp(m_colorStart, m_colorEnd, m_timeCount));

            if (m_timeCount >= 1)
            {
                m_timeCount = 0;
                m_colorStart = m_renderer.material.GetColor("_Color");
                m_colorEnd = new Color(Random.value, Random.value, Random.value);
            }
        }

        // Press space to disable the color changing
        if (Input.GetKeyDown(KeyCode.Space))
        {
            m_changeColor = !m_changeColor;
        }
    }
    #endregion
}