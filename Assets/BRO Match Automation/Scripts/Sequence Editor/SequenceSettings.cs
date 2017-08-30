namespace BRO.SequenceEditor
{
    /// <summary>
    /// Stores the sequence settings.
    /// </summary>
    [System.Serializable]
    public class SequenceSettings
    {
        #region Member Fields
        private int m_laps = 1;
        private bool m_infinite = false;
        private bool m_shuffle = false;
        private float m_timeLimit = 0;
        private bool m_useTimeLimit = false;
        private float m_simulationSpeed = 1;
        #endregion

        #region Member Properties
        public int Laps
        {
            get { return m_laps; }
            set { m_laps = value; }
        }

        public bool Infinite
        {
            get { return m_infinite; }
            set { m_infinite = value; }
        }

        public bool Shuffle
        {
            get { return m_shuffle; }
            set { m_shuffle = value; }
        }

        public float TimeLimit
        {
            get { return m_timeLimit; }
            set { m_timeLimit = value; }
        }

        public float SimulationSpeed
        {
            get { return m_simulationSpeed; }
            set { m_simulationSpeed = value; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor for Sequence Settings.
        /// </summary>
        /// <param name="laps">The amount of laps of running the all matches</param>
        /// <param name="infinite">Let the automation run until it gets aborted</param>
        /// <param name="shuffle">Randomize the order of the matches each lap</param>
        public SequenceSettings(int laps, bool infinite, bool shuffle)
        {
            m_laps = laps;
            m_infinite = infinite;
            m_shuffle = shuffle;
        }

        /// <summary>
        /// Constructor for Sequence Settings.
        /// </summary>
        /// <param name="laps">The amount of laps of running the all matches</param>
        /// <param name="infinite">Let the automation run until it gets aborted</param>
        /// <param name="shuffle">Randomize the order of the matches each lap</param>
        /// <param name="timeLimit">Specify the duration of running the sequence in seconds</param>
        public SequenceSettings(int laps, bool infinite, bool shuffle, float timeLimit)
        {
            m_laps = laps;
            m_infinite = infinite;
            m_shuffle = shuffle;
            m_timeLimit = timeLimit;
            m_useTimeLimit = true;
        }

        /// <summary>
        /// Constructor for Sequence Settings, which leaves all fields at default.
        /// </summary>
        public SequenceSettings() { }
        #endregion
    }
}