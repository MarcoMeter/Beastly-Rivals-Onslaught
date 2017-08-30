using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;

namespace BRO
{
    /// <summary>
    /// The DebugManager is in charge of enabling and disabling visual debugging.
    /// </summary>
    public class DebugManager : GenericSingleton<DebugManager>
    {
        #region Member Fields
        [SerializeField]
        private bool m_isDebugMode = true;

        // Line Rendering
        [SerializeField]
        private Material m_lineMaterial;


        // Delegate definition
        public delegate void DebugModeDelegate(bool isDebug);
        #endregion

        #region Member Proberties
        /// <summary>
        /// Returns true if debugging mode is enabled.
        /// Read-only
        /// </summary>
        public bool IsDebugMode
        {
            get { return m_isDebugMode; }
        }

        /// <summary>
        /// Add methods to the delegate to get notified upon on changes of the debug mode.
        /// </summary>
        public DebugModeDelegate DebugModeCallback
        {
            get; set;
        }
        #endregion

        #region Unity Lifecycle
        /// <summary>
        /// Start ensures that this object will stay persistent.
        /// </summary>
        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// Checks if m_isDebugMode has changed.
        /// OnValidate gets called if a value in the inspector changed andas soon as the script gets loaded (including loaded scenes).
        /// Editor-only function.
        /// </summary>
        private void OnValidate()
        {
            if(DebugModeCallback != null)
                DebugModeCallback(m_isDebugMode);
        }
        #endregion

        #region Public Functions
        /// <summary>
        /// Renders a line based on two positions.
        /// </summary>
        /// <param name="start">Start position</param>
        /// <param name="end">End position</param>
        /// <param name="color">Color of the line</param>
        /// <returns></returns>
        public LineRenderer DrawLine(Vector3 start, Vector3 end, Color color)
        {
            GameObject go = new GameObject("Line");
            LineRenderer lineRenderer = go.AddComponent<LineRenderer>();
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, start);
            lineRenderer.SetPosition(1, end);
            Material lineMaterial = new Material(m_lineMaterial);
            lineMaterial.SetColor("_TintColor", color);
            lineRenderer.material = lineMaterial;
            lineRenderer.useWorldSpace = false;
            lineRenderer.shadowCastingMode = ShadowCastingMode.Off;
            return lineRenderer;
        }

        public LineRenderer DrawCircle(Vector3 position, float radius,int segments, Color color)
        {
            GameObject go = new GameObject("Circle");
            LineRenderer lineRenderer = go.AddComponent<LineRenderer>();
            lineRenderer.positionCount = segments + 1;
            Material lineMaterial = new Material(m_lineMaterial);
            lineMaterial.SetColor("_TintColor", color);
            lineRenderer.material = lineMaterial;
            lineRenderer.useWorldSpace = true;
            lineRenderer.shadowCastingMode = ShadowCastingMode.Off;

            // Compute positions
            float x, z;
            float y = 0.0f;

            float angle = (360f / segments);

            for(int i =0; i < lineRenderer.positionCount; i++)
            {
                x = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
                z = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;
                lineRenderer.SetPosition(i, new Vector3(x, y, z) + position);
                angle += (360f / segments);
            }

            return lineRenderer;
        }
        #endregion

        #region Local Functions
        #endregion
    }
}