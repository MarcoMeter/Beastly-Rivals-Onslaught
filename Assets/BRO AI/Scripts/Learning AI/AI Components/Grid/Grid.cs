using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0219, 0414
namespace BRO.AI.Learning
{
    /// <summary>
    /// A Grid object maps a 2D dimensional space into the 3D world using two points. This is used to approximate the players' and the beast's position.
    /// </summary>
    public class Grid
    {
        #region Member Fields
        private List<GridCell> m_cells = new List<GridCell>();
        private Vector2 m_topLeftCorner;
        private Vector2 m_bottomRightCorner;
        private Vector2 m_centerPoint;
        private float m_cellWidth;
        private float m_cellHeight;
        private float m_gridWidth;
        private float m_gridHeight;
        private int m_rowCount;
        private int m_columnCount;
        #endregion

        #region Member Properties
        /// <summary>
        /// Read-only count of cells
        /// </summary>
        public int CellCount
        {
            get { return m_cells.Count; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructs a grid based on two points and the dimensions of the cells.
        /// </summary>
        /// <param name="topLeftCorner">Top left corner of the grid</param>
        /// <param name="bottomRightCorner">Bottom right corner of the grid</param>
        /// <param name="cellWidth">Width for each cell</param>
        /// <param name="cellHeight">Height for each cell</param>
        public Grid(Vector2 topLeftCorner, Vector2 bottomRightCorner, float cellWidth, float cellHeight)
        {
            // Initializing properties
            m_topLeftCorner = topLeftCorner;
            m_bottomRightCorner = bottomRightCorner;
            m_cellWidth = cellWidth;
            m_cellHeight = cellHeight;
            m_gridWidth = Mathf.Abs(topLeftCorner.x) + Mathf.Abs(bottomRightCorner.x);
            m_gridHeight = Mathf.Abs(topLeftCorner.y) + Mathf.Abs(bottomRightCorner.y);
            m_centerPoint = new Vector2(topLeftCorner.x + m_gridWidth / 2, topLeftCorner.y - m_gridHeight / 2);
            m_rowCount = (int)(m_gridHeight / cellHeight);
            m_columnCount = (int)(m_gridWidth / cellWidth);

            // Preparing grid cell generation
            float currX = topLeftCorner.x;
            float currY = topLeftCorner.y;
            int currIndex = 0;

            // Nested for-loop to instantiate and position grid cells
            for (int i = 0; i < m_rowCount; i++)
            {
                for (int j = 0; j < m_columnCount; j++)
                {
                    var centerPoint = new Vector2(currX + cellWidth / 2, currY - cellHeight / 2);
                    var cTopLeftCorner = new Vector2(currX, currY);
                    var cBottomRightCorner = new Vector2(currX + cellWidth, currY - cellHeight);
                    m_cells.Add(new GridCell(this, currIndex, cellWidth, cellHeight, centerPoint, cTopLeftCorner, cBottomRightCorner));
                    currX += cellWidth;
                    currIndex++;
                }
                currX = topLeftCorner.x;
                currY -= cellHeight;
            }
        }
        #endregion

        #region Public Functions
        /// <summary>
        /// Serches through all grid cells in order to pinpoint one cell, which containts the position.
        /// </summary>
        /// <param name="position">Position to search for in the grid</param>
        /// <returns></returns>
        public int FindGridIndex(Vector2 position)
        {
            // Iterate over each cell and check if the position is inside the cell's boundary
            foreach (var cell in m_cells)
            {
                if(position.x >= cell.TopLeftCorner.x && position.x <= cell.BottomRightCorner.x &&
                    position.y <= cell.TopLeftCorner.y && position.y >= cell.BottomRightCorner.y)
                {
                    return cell.Id;
                }
            }
            throw new System.Exception("Position is out of the grid's bounds, Pos: " + position);
        }
        #endregion

        #region Local Functions
        #endregion
    }
}