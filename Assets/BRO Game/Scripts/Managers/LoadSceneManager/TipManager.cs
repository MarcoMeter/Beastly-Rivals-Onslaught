using UnityEngine;

namespace BRO.Game
{
    /// <summary>
    /// The TipManager is in charge of displaying the tip objects.
    /// </summary>
    public class TipManager : MonoBehaviour
    {
        #region Member Fields
        [SerializeField]
        private GameObject[] m_tipObjects;
        private int m_currentIndex = 0;
        #endregion

        #region UI Event
        /// <summary>
        /// Displays the next tip object.
        /// </summary>
        public void NextItem()
        {
            m_tipObjects[m_currentIndex].SetActive(false);
            m_currentIndex++;
            if (m_currentIndex >= m_tipObjects.Length)
                m_currentIndex = 0;
            m_tipObjects[m_currentIndex].SetActive(true);
        }

        /// <summary>
        /// Displays the previous tip object.
        /// </summary>
        public void PreviousItem()
        {
            m_tipObjects[m_currentIndex].SetActive(false);
            m_currentIndex--;
            if (m_currentIndex < 0)
                m_currentIndex = m_tipObjects.Length-1;
            m_tipObjects[m_currentIndex].SetActive(true);
        }
        #endregion
    }
}