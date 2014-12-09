using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace ActiveContour
{
    /// <summary>
    /// Simplifies the points of a closed shape using a given tolerance using the Douglas-Peucker Algorithm 
    /// </summary>
    class SimplifyShape
    {
        private List<Point> originalPoints;
        private int tolerance;

        private List<Point> simplifiedPoints;

        private int numberOfPoints;

        private double furthestPointDistance;

        public SimplifyShape(List<Point> originalPoints, int tolerance)
        {
            this.originalPoints = originalPoints;
            this.tolerance = tolerance;

            simplifiedPoints = new List<Point>();

            numberOfPoints = originalPoints.Count;

            simplifiedPoints.Add(originalPoints[0]);
        }

        public void Simplify()
        {
            SimplifyLine(0, numberOfPoints - 1);
        }

        public void SimplifyLine(int startPos, int endPos)
        {
            int furthestPointPosition = CalculateFurthestPoint(startPos, endPos);

            if (furthestPointDistance < tolerance)
            {
                if (!simplifiedPoints.Contains(new Point(originalPoints[furthestPointPosition].X, originalPoints[furthestPointPosition].Y)))
                    simplifiedPoints.Add(originalPoints[furthestPointPosition]);
            }
            else
            {
                SimplifyLine(startPos, furthestPointPosition);

                if (!simplifiedPoints.Contains(new Point(originalPoints[furthestPointPosition].X, originalPoints[furthestPointPosition].Y)))
                    simplifiedPoints.Add(originalPoints[furthestPointPosition]);

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

            furthestPointDistance = maxDistance;

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
            double distance;

            int startX = originalPoints[startPos].X;
            int startY = originalPoints[startPos].Y;

            int endX = originalPoints[endPos].X;
            int endY = originalPoints[endPos].Y;

            int distX = originalPoints[distancePos].X;
            int distY = originalPoints[distancePos].Y;

            double top = ((endX - startX) * (startY - distY)) - ((startX - distX) * (endY - startY));
            double bottom = Math.Sqrt(Math.Pow(endX - startX, 2) + Math.Pow(endY - startY, 2));

            distance = top / bottom;


            if (distance < 0)
                distance = 0 - distance;

            return distance;

        }

        public List<Point> GetSimplifiedPoints()
        {
            return simplifiedPoints;
        }
    }
}
