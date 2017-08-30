using UnityEngine;

namespace BRO.SequenceEditor
{
    /// <summary>
    /// Allow object to follow the mouse position.
    /// </summary>
    public class FollowMousePosition : MonoBehaviour
    {
        #region Member Fields
        private Vector3 m_offset;
        private bool m_ifOffset = false;
        #endregion

        #region Unity Lifecycle
        void Update()
        {
            if (m_ifOffset)
            {
                transform.position = Input.mousePosition - m_offset;
            }
            else
            {
                transform.position = Input.mousePosition;
            }
        }
        #endregion

        #region Public Functions
        public void DefineOffset(Vector3 origin)
        {
            m_offset = Input.mousePosition - origin;
            m_ifOffset = true;
        }
        #endregion
    }
}