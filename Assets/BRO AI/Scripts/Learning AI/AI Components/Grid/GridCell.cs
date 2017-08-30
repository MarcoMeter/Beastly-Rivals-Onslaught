using UnityEngine;

#pragma warning disable 0219, 0414
namespace BRO.AI.Learning
{
    /// <summary>
    /// Single element of a grid object.
    /// </summary>
    public class GridCell
    {
        #region Member Fields
        private Grid m_parentGrid;
        private int m_cellId;
        private float m_width;
        private float m_height;
        private Vector2 m_centerPoint;
        private Vector2 m_topLeftCorner;
        private Vector2 m_bottomRightCorner;
        #endregion

        #region Member Properties
        /// <summary>
        /// Read-only index of the cell
        /// </summary>
        public int Id
        {
            get { return m_cellId; }
        }

        /// <summary>
        /// Read-only top left corner position
        /// </summary>
        public Vector2 TopLeftCorner
        {
            get { return m_topLeftCorner; }
        }

        /// <summary>
        /// Read-only bottom right corner position
        /// </summary>
        public Vector2 BottomRightCorner
        {
            get { return m_bottomRightCorner; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Creates an cell object which is a single entity of the grid.
        /// </summary>
        /// <param name="parent">Grid which this cell belongs to</param>
        /// <param name="id">Id of the cell</param>
        /// <param name="width">Width of the cell</param>
        /// <param name="height">Height of the cell</param>
        /// <param name="centerPoint">Center point of the cell</param>
        /// <param name="topLeftCorner">Top left corner position of the cell</param>
        /// <param name="bottomRightCorner">Bottom right corner position of the cell</param>
        public GridCell(Grid parent, int id, float width, float height, Vector2 centerPoint, Vector2 topLeftCorner, Vector2 bottomRightCorner)
        {
            m_parentGrid = parent;
            m_cellId = id;
            m_width = width;
            m_height = height;
            m_centerPoint = centerPoint;
            m_topLeftCorner = topLeftCorner;
            m_bottomRightCorner = bottomRightCorner;
        }
        #endregion

        #region Public Functions
        #endregion

        #region Local Functions
        #endregion
    }
}