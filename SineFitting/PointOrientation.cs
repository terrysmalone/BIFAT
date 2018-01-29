using System;
using System.Drawing;

namespace EdgeFitting
{
    /// <summary>
    /// Class which calculates the orientation of a point from the two points surrounding it.  
    /// Possible orientations are:
    /// 
    /// Horizontal   HorizontalLeadingOffset   LeadingDiagonal   VerticalLeadingOffset 
    ///  |-|-|-|        |X|-|-|  |-|-|-|          |X|-|-|          |X|-|-|  |-|X|-|   
    ///  |X|X|X|        |-|X|X|  |X|X|-|          |-|X|-|          |-|X|-|  |-|X|-|   
    ///  |-|-|-|        |-|-|-|  |-|-|X|          |-|-|X|          |-|X|-|  |-|-|X|   
    /// 
    /// Vertical     VerticalCounterOffset    CounterDiagonal    HorizontalCounterOffset
    ///  |-|X|-|        |-|-|X|   |-|X|-|         |-|-|X|          |-|-|X|   |-|-|-|
    ///  |-|X|-|        |-|X|-|   |-|X|-|         |-|X|-|          |X|X|-|   |-|X|X|
    ///  |-|X|-|        |-|X|-|   |X|-|-|         |X|-|-|          |-|-|-|   |X|-|-|
    /// 
    ///  Author - Terry Malone (terrysmalone@hotmail.com)
    /// </summary>
    public class PointOrientation
    {
        private readonly int m_ImageWidth;

        private int m_BeforeX, m_AfterX;

        private readonly int m_BeforeY;
        private readonly int m_CheckX;
        private readonly int m_CheckY;
        private readonly int m_AfterY;

        #region properties

        /// <summary>
        /// Returns the orientation of the given point
        /// </summary>
        /// <returns></returns>
        public Orientation Orientation { get; private set; }

        #endregion properties
        
        public PointOrientation(Point beforePoint, Point checkPoint, Point afterPoint, int imageWidth)
        {
            m_BeforeX = beforePoint.X;
            m_BeforeY = beforePoint.Y;

            m_CheckX = checkPoint.X;
            m_CheckY = checkPoint.Y;

            m_AfterX = afterPoint.X;
            m_AfterY = afterPoint.Y;
            
            this.m_ImageWidth = imageWidth;

            CheckForEdgeValues();

            CalculatePointOrientation();
        }

        /// <summary>
        /// If the edges are at the edge of the image wrap them
        /// </summary>
        private void CheckForEdgeValues()
        {
            CheckIfPointIsAtLeftBoundary();

            CheckIfPointIsAtRightBoundary();
        }

        /// <summary>
        /// If point to check is at left boundary beforeX is set to minus 1.  
        /// </summary>
        private void CheckIfPointIsAtLeftBoundary()
        {
            if (m_CheckX == 0 && m_BeforeX != 0)
            {
                m_BeforeX = -1;
            }
        }

        /// <summary>
        /// If point to check is at left boundary afterX is set to image width.  
        /// </summary>
        private void CheckIfPointIsAtRightBoundary()
        {
            if (m_CheckX == m_ImageWidth - 1 && m_AfterX != m_ImageWidth - 1)
            {
                m_AfterX = m_ImageWidth;
            }
        }

        private void CalculatePointOrientation()
        {
            var columnCount = Convert.ToInt32(Math.Abs(m_BeforeX - m_AfterX) + 1);
            var rowCount = Convert.ToInt32(Math.Abs(m_BeforeY - m_AfterY) + 1);

            if(columnCount == 1)
            {
                Orientation = Orientation.Vertical;
            }
            else if(rowCount == 1)
            {
                Orientation = Orientation.Horizontal;
            }
            else if(columnCount == 2)
            {
                if(m_BeforeX == m_AfterX - 1 && m_BeforeY < m_AfterY)
                {
                    Orientation = Orientation.VerticalLeadingOffset;
                }
                else
                {
                    Orientation = Orientation.VerticalCounterOffset;
                }

            }
            else if(rowCount == 2)
            {
                Orientation = m_BeforeY == m_AfterY - 1 ? Orientation.HorizontalLeadingOffset 
                                                             : Orientation.HorizontalCounterOffset;
            }
            else if(columnCount == 3 && rowCount == 3)
            {
                Orientation = m_BeforeY < m_AfterY ? Orientation.LeadingDiagonal 
                                                        : Orientation.CounterDiagonal;
            }
        }

        /// <summary>
        /// Checks if points span one row
        /// </summary>
        /// <returns>True if all points are in same row</returns>
        private bool DoPointsSpan1Column()
        {
            return m_BeforeX == m_AfterX 
                   && m_BeforeX == m_CheckX;
        }

        /// <summary>
        /// Checks if points span one column
        /// </summary>
        /// <returns>True if all points are in same column</returns>
        private bool DoPointsSpan1Row()
        {
            return m_BeforeY == m_AfterY
                   && m_BeforeY == m_CheckY;
        }

        /// <summary>
        /// Checks if points span 3 columns
        /// </summary>
        /// <returns>True if points span 3 columns</returns>
        private bool DoPointsSpan3Columns()
        {
            return m_BeforeX < m_CheckX 
                   && m_AfterX > m_CheckX;
        }
    }

}
