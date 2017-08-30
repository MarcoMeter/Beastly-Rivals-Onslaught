using BRO.AI;
using UnityEngine;

namespace BRO.SequenceEditor
{
    /// <summary>
    /// The Match object stores match relevant information.
    /// </summary>
    [System.Serializable]
    public class Match
    {
        #region Member Fields
        private int m_id;
        private string m_name = string.Empty;
        private string m_tag;
        private AIBase[] m_aiPlayers;
        private int m_lives;
        private bool m_infiniteLives = false;
        private Color m_color;
        private string m_description;
        #endregion

        #region Member Properties
        public int Id
        {
            get { return m_id; }
            set { m_id = value; }
        }

        public string Name
        {
            get { return m_name; }
            set { m_name = value; }
        }

        public AIBase[] AIPlayers
        {
            get { return m_aiPlayers; }
            set { m_aiPlayers = value; }
        }

        public int Lives
        {
            get { return m_lives; }
            set { m_lives = value; }
        }

        public bool InfiniteLives
        {
            get { return m_infiniteLives; }
            set { m_infiniteLives = value; }
        }

        public string Tag
        {
            get { return m_tag; }
            set { m_tag = value; }
        }

        public Color Color
        {
            get { return m_color; }
            set { m_color = value; }
        }

        public string Description
        {
            get { return m_description; }
            set { m_description = value; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor for a Match object.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="tag"></param>
        /// <param name="aiPlayers"></param>
        /// <param name="lives"></param>
        /// <param name="infiniteLives"></param>
        /// <param name="color"></param>
        /// <param name="description"></param>
        public Match(string name, string tag, AIBase[] aiPlayers, int lives, bool infiniteLives,  Color color, string description)
        {
            m_name = name;
            m_aiPlayers = aiPlayers;
            m_lives = lives;
            m_infiniteLives = infiniteLives;
            m_tag = tag;
            m_color = color;
            m_description = description;
        }
        #endregion
    }
}