using System;
using System.Collections.Generic;
using System.Drawing;

namespace ActiveContour
{
    /// <summary>
    /// Simplifies the points of a closed shape using a given 
    /// tolerance using the Douglas-Peucker Algorithm 
    /// </summary>
    class SimplifyShape
    {
        private readonly List<Point> m_OriginalPoints;
        private readonly int m_Tolerance;

        private readonly List<Point> m_SimplifiedPoints;

        private readonly int m_NumberOfPoints;

        private double m_FurthestPointDistance;

        #region constructor

        public SimplifyShape(List<Point> originalPoints, int tolerance)
        {
            m_OriginalPoints = originalPoints;
            m_Tolerance = tolerance;

            m_SimplifiedPoints = new List<Point>();

            m_NumberOfPoints = originalPoints.Count;

            m_SimplifiedPoints.Add(originalPoints[0]);
        }

        #endregion constructor

        public void Simplify()
        {
            SimplifyLine(0, m_NumberOfPoints - 1);
        }

        public void SimplifyLine(int startPos, int endPos)
        {
            var furthestPointPosition = CalculateFurthestPoint(startPos, endPos);

            if (m_FurthestPointDistance < m_Tolerance)
            {
                if (!m_SimplifiedPoints.Contains(new Point(m_OriginalPoints[furthestPointPosition].X,
                                                           m_OriginalPoints[furthestPointPosition].Y)))
                {
                    m_SimplifiedPoints.Add(m_OriginalPoints[furthestPointPosition]);
                }
            }
            else
            {
                SimplifyLine(startPos, furthestPointPosition);

                if (!m_SimplifiedPoints.Contains(new Point(m_OriginalPoints[furthestPointPosition].X,
                                                           m_OriginalPoints[furthestPointPosition].Y)))
                {
                    m_SimplifiedPoints.Add(m_OriginalPoints[furthestPointPosition]);
                }

                SimplifyLine(furthestPointPosition, endPos);
            }
        }

        /// <summary>
        /// Finds the point with the largest perpendicular distance between the two given points
        /// </summary>
        /// <returns></returns>
        private int CalculateFurthestPoint(int startPos, int endPos)
        {
            double maxDistance = 0;
            int furthestPoint = 0;

            for (int i = startPos; i < endPos; i++)
            {
                double distance = CalculateDistance(startPos, endPos, i);

                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    furthestPoint = i;
                }
            }

            m_FurthestPointDistance = maxDistance;

            return furthestPoint;
        }

        /// <summary>
        /// Calculates the perpendicular distance of point distancePos to the line startPos-endPos
        /// </summary>
        /// <param name="startPos"></param>
        /// <param name="endPos"></param>
        /// <param name="distancePos"></param>
        /// <returns></returns>
        private double CalculateDistance(int startPos, int endPos, int distancePos)
        {
            var startX = m_OriginalPoints[startPos].X;
            var startY = m_OriginalPoints[startPos].Y;

            var endX = m_OriginalPoints[endPos].X;
            var endY = m_OriginalPoints[endPos].Y;

            var distX = m_OriginalPoints[distancePos].X;
            var distY = m_OriginalPoints[distancePos].Y;

            var top = ((endX - startX) * (startY - distY)) - ((startX - distX) * (endY - startY));
            var bottom = Math.Sqrt(Math.Pow(endX - startX, 2) + Math.Pow(endY - startY, 2));

            var distance = top / bottom;


            if (distance < 0)
            {
                distance = 0 - distance;
            }

            return distance;
        }

        public List<Point> GetSimplifiedPoints()
        {
            return m_SimplifiedPoints;
        }
    }
}
