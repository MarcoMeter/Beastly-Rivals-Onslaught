using System;
using System.Collections.Generic;

namespace BRO.SequenceEditor
{
    /// <summary>
    /// The MatchSequenceData enables the serialization of match sequences.
    /// </summary>
    [Serializable]
    public class MatchSequenceData : Object
    {
        #region Member Fields
        private List<Match> m_matchList;
        private SequenceSettings m_settings;
        #endregion

        #region Member Properties
        /// <summary>
        /// List of matches.
        /// Read-only.
        /// </summary>
        public List<Match> MatchList
        {
            get { return m_matchList; }
        }

        /// <summary>
        /// Match sequence settings.
        /// Read-only.
        /// </summary>
        public SequenceSettings Settings
        {
            get { return m_settings; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// The constructor of the Match Sequence data.
        /// </summary>
        /// <param name="matchList"></param>
        /// <param name="matchSequence"></param>
        public MatchSequenceData(List<Match> matchList, SequenceSettings matchSequence)
        {
            m_matchList = matchList;
            m_settings = matchSequence;
        }
        #endregion
    }
}