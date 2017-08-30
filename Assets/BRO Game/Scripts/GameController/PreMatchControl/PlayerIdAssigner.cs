namespace BRO.Game.PreMatch
{
    /// <summary>
    /// The PlasyerIdAssigner manages the process of assigning and freeing IDs for the players.
    /// </summary>
    public class PlayerIdAssigner
    {
        #region Member Fields
        private bool[] m_availableId = new bool[8];
        #endregion

        #region Constructor
        /// <summary>
        /// Creates an instance of the PlayerIdAssigner, which initializes all available IDs.
        /// </summary>
        public PlayerIdAssigner()
        {
            // initialize all ids as true
            for (int i = 0; i < m_availableId.Length; i++)
            {
                m_availableId[i] = true;
            }
        }
        #endregion

        #region Public Functions
        /// <summary>
        /// Find the first available id.
        /// </summary>
        /// <returns>Returns the first available id to the server</returns>
        public int RequestPlayerId()
        {
            int idToAssign = 0;

            for (int i = 0; i < m_availableId.Length; i++)
            {
                if (m_availableId[i])
                {
                    idToAssign = i;
                    m_availableId[idToAssign] = false;
                    break;
                }
            }
            return idToAssign;
        }

        /// <summary>
        /// Makes a previous distributed id available again. e.g. a player disconnected or got kicked
        /// </summary>
        /// <param name="idToFree">The id which has to be freed</param>
        public void FreePlayerId(int idToFree)
        {
            m_availableId[idToFree] = true;
        }
        #endregion
    }
}