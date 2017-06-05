using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace ActiveContour
{
    /// <summary>
    /// Extracts a line as a List of Points from a two-dimensional boolean array
    /// </summary>
    internal class ExtractLine
    {
        private readonly bool[,] m_PointsArray;

        private readonly bool[,] m_AddedPoints;

        private readonly List<Point> m_Line;

        private Point m_StartPoint;

        private readonly Point[] m_Neighbours;

        private readonly int m_XLength;
        private readonly int m_YLength;

        public ExtractLine(bool[,] pointsArray)
        {
            m_PointsArray = pointsArray;

            m_XLength = pointsArray.GetLength(0);
            m_YLength = pointsArray.GetLength(1);

            m_AddedPoints = new bool[m_XLength, m_YLength];

            m_Line = new List<Point>();

            m_Neighbours = new Point[8];
            m_Neighbours[0] = new Point(0, -1);       //North
            m_Neighbours[1] = new Point(1, -1);        //North East
            m_Neighbours[2] = new Point(1, 0);         //East
            m_Neighbours[3] = new Point(1, 1);         //South East
            m_Neighbours[4] = new Point(0, 1);         //South
            m_Neighbours[5] = new Point(-1, 1);        //South west
            m_Neighbours[6] = new Point(-1, 0);        //West
            m_Neighbours[7] = new Point(-1, -1);       //North West          
        }

        # region process line methods

        /// <summary>
        /// Checks if any points should be removed and if so removes them.
        /// Points are removed if they touch maore than 2 other points and 
        /// removing them won't break a line
        /// </summary>
        private void ProcessPoints()
        {
            for (var yPos = 0; yPos < m_YLength; yPos++)
            {
                for (var xPos = 0; xPos < m_XLength; xPos++)
                {
                    // ReSharper disable once InvertIf
                    if (m_PointsArray[xPos, yPos])
                    {
                        var connectedPoints = GetConnectedPoints(new Point(xPos, yPos));

                        if (ArePointsConnected(connectedPoints))
                        {
                            m_PointsArray[xPos, yPos] = false;
                        }
                    }
                }
            }

            ReomveOutliers();
        }
        
        private Point[] GetConnectedPoints(Point startPoint)
        {
            var touchingPoints = new List<Point>();

            foreach (var neighbour in m_Neighbours)
            {
                var neighbourX = startPoint.X + neighbour.X;
                var neighbourY = startPoint.Y + neighbour.Y;

                if (neighbourX >= 0 && neighbourX < m_XLength && neighbourY >= 0 && neighbourY < m_YLength)
                {
                    if (m_PointsArray[neighbourX, neighbourY] == true && m_AddedPoints[neighbourX, neighbourY] == false)
                        touchingPoints.Add(new Point(neighbourX, neighbourY));
                }
            }

            return touchingPoints.ToArray();

        }

        /// <summary>
        /// Checks if the given points are connected to each other
        /// </summary>
        /// <param name="connectedPoints"></param>
        /// <returns></returns>
        private bool ArePointsConnected(IList<Point> connectedPoints)
        {
            for (var i = 0; i < connectedPoints.Count; i++)
            {
                var connected = false;

                for (var j = 0; j < connectedPoints.Count; j++)
                {
                    if (i != j)
                    {
                        if (IsConnected(connectedPoints[i], connectedPoints[j]))
                            connected = true;
                    }
                }

                if (connected == false)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Iteratively checks if points are outliers and if so removes them
        /// </summary>
        private void ReomveOutliers()
        {
            var cleanRun = false;

            while (cleanRun == false)
            {
                cleanRun = true;

                for (var yPos = 0; yPos < m_YLength; yPos++)
                {
                    for (var xPos = 0; xPos < m_XLength; xPos++)
                    {
                        if (m_PointsArray[xPos, yPos] == true && GetConnectedPoints(new Point(xPos, yPos)).Length < 2)
                        {
                            m_PointsArray[xPos, yPos] = false;
                            cleanRun = false;
                        }
                    }

                }
            }
        }

        private static bool IsConnected(Point firstPoint, Point secondPoint)
        {
            if (Math.Max(firstPoint.X, secondPoint.X) - Math.Min(firstPoint.X, secondPoint.X) > 1)
                return false;

            if (Math.Max(firstPoint.Y, secondPoint.Y) - Math.Min(firstPoint.Y, secondPoint.Y) > 1)
                return false;

            return true;

        }

        # endregion process line methods

        public void Extract()
        {
            ProcessPoints();

            m_StartPoint = FindStartPoint();

            if(m_StartPoint != new Point(-1, -1))
            {
                 AddToLine(m_StartPoint);

                var nextPoint = GetConnectedPoints(m_StartPoint)[0];
                var lastPoint = GetConnectedPoints(m_StartPoint)[1];

                AddToLine(nextPoint);

                var pointsLeft = true;

                while (pointsLeft)
                {
                    var currentPoint = nextPoint;

                    nextPoint = GetNextPoint(currentPoint);

                    AddToLine(nextPoint);

                    if (nextPoint.X == lastPoint.X && nextPoint.Y == lastPoint.Y)
                        pointsLeft = false;
                }
            }
            
        }

        private Point GetNextPoint(Point currentPoint)
        {
            var touchingPoints = GetConnectedPoints(currentPoint);

            return touchingPoints[0];
        }

        private void AddToLine(Point startPoint)
        {
            m_Line.Add(startPoint);
            m_AddedPoints[startPoint.X, startPoint.Y] = true;
        }

        private Point FindStartPoint()
        {
            for (var yPos = 0; yPos < m_YLength; yPos++)
            {
                for (var xPos = 0; xPos < m_XLength; xPos++)
                {
                    if (m_PointsArray[xPos, yPos] == true && GetConnectedPoints(new Point(xPos, yPos)).Length == 2)
                    {
                        var xStart = xPos;
                        var yStart = yPos;

                        return new Point(xStart, yStart);
                    }
                }
            }

            return new Point(-1, -1);
        }

        /// <summary>
        /// Calculates how many points are connected to the given point in the given boolean array
        /// </summary>
        /// <param name="xPos"></param>
        /// <param name="yPos"></param>
        /// <returns></returns>
        private int PointsTouching(int xPos, int yPos)
        {
            var connectedNeighbours = 0;

            for (int i = 0; i < m_Neighbours.Length; i++)
            {
                var neighbourX = xPos + m_Neighbours[i].X;
                var neighbourY = yPos + m_Neighbours[i].Y;

                if (neighbourX >= 0 && neighbourX < m_XLength && neighbourY >= 0 && neighbourY < m_YLength)
                {
                    if (m_PointsArray[neighbourX, neighbourY] == true)
                        connectedNeighbours++;
                }
            }

            return connectedNeighbours;
        }

        private void DrawPoints()
        {
            var image = new Bitmap(m_XLength, m_YLength);
            
            for (var yPos = 0; yPos < m_YLength; yPos++)
            {
                for (var xPos = 0; xPos < m_XLength; xPos++)
                {
                    image.SetPixel(xPos, yPos, m_PointsArray[xPos, yPos] 
                                    ? Color.LawnGreen 
                                    : Color.Black);
                }
            }

            image.Save("Snake - ProcessedCurve.bmp");
        }

        # region Get methods

        public List<Point> GetLine()
        {
            return m_Line;
        }

        # endregion  Get methods
    }
}
