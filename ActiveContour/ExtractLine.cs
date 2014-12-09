using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace ActiveContour
{
    /// <summary>
    /// Extracts a line as a List<Point> from a two-dimensional boolean array
    /// </summary>
    class ExtractLine
    {
        private bool[,] pointsArray;

        private bool[,] addedPoints;

        private List<Point> line;

        private Point startPoint;

        private Point[] neighbours;

        private int xLength, yLength;

        public ExtractLine(bool[,] pointsArray)
        {
            this.pointsArray = pointsArray;

            xLength = pointsArray.GetLength(0);
            yLength = pointsArray.GetLength(1);

            addedPoints = new bool[xLength, yLength];

            line = new List<Point>();

            neighbours = new Point[8];
            neighbours[0] = new Point(0, -1);       //North
            neighbours[1] = new Point(1, -1);        //North East
            neighbours[2] = new Point(1, 0);         //East
            neighbours[3] = new Point(1, 1);         //South East
            neighbours[4] = new Point(0, 1);         //South
            neighbours[5] = new Point(-1, 1);        //South west
            neighbours[6] = new Point(-1, 0);        //West
            neighbours[7] = new Point(-1, -1);       //North West          
        }

        # region process line methods

        /// <summary>
        /// Checks if any points should be removed and if so removes them.
        /// Points are removed if they touch maore than 2 other points and removing them won't break a line
        /// </summary>
        private void ProcessPoints()
        {
            for (int yPos = 0; yPos < yLength; yPos++)
            {
                for (int xPos = 0; xPos < xLength; xPos++)
                {
                    if (pointsArray[xPos, yPos] == true)
                    {
                        Point[] connectedPoints = GetConnectedPoints(new Point(xPos, yPos));

                        if (ArePointsConnected(connectedPoints))
                        {
                            pointsArray[xPos, yPos] = false;
                        }
                    }
                }
            }

            ReomveOutliers();
        }

        /// <summary>
        /// Iteratively checks if points are outliers and if so removes them
        /// </summary>
        private void ReomveOutliers()
        {
            bool cleanRun = false;

            while (cleanRun == false)
            {
                cleanRun = true;

                for (int yPos = 0; yPos < yLength; yPos++)
                {
                    for (int xPos = 0; xPos < xLength; xPos++)
                    {
                        if (pointsArray[xPos, yPos] == true && GetConnectedPoints(new Point(xPos, yPos)).Length < 2)
                        {
                            pointsArray[xPos, yPos] = false;
                            cleanRun = false;
                        }
                    }

                }
            }
        }

        /// <summary>
        /// Checks if the given points are connected to each other
        /// </summary>
        /// <param name="connectedPoints"></param>
        /// <returns></returns>
        private bool ArePointsConnected(Point[] connectedPoints)
        {
            for (int i = 0; i < connectedPoints.Length; i++)
            {
                bool connected = false;

                for (int j = 0; j < connectedPoints.Length; j++)
                {
                    if (i != j)
                    {
                        if (IsConnected(connectedPoints[i], connectedPoints[j]) == true)
                            connected = true;
                    }
                }

                if (connected == false)
                    return false;
            }

            return true;
        }

        private bool IsConnected(Point firstPoint, Point secondPoint)
        {
            if (Math.Max(firstPoint.X, secondPoint.X) - Math.Min(firstPoint.X, secondPoint.X) > 1)
                return false;

            if (Math.Max(firstPoint.Y, secondPoint.Y) - Math.Min(firstPoint.Y, secondPoint.Y) > 1)
                return false;

            return true;

        }

        # endregion

        public void Extract()
        {
            ProcessPoints();

            //DrawPoints();

            startPoint = FindStartPoint();

            if(startPoint != new Point(-1, -1))
            {
                 AddToLine(startPoint);

                Point nextPoint = GetConnectedPoints(startPoint)[0];
                Point lastPoint = GetConnectedPoints(startPoint)[1];

                AddToLine(nextPoint);

                bool pointsLeft = true;

                Point currentPoint;

                while (pointsLeft)
                {
                    currentPoint = nextPoint;

                    nextPoint = GetNextPoint(currentPoint);

                    AddToLine(nextPoint);

                    if (nextPoint.X == lastPoint.X && nextPoint.Y == lastPoint.Y)
                        pointsLeft = false;
                }
            }
            
        }

        private Point GetNextPoint(Point currentPoint)
        {
            Point[] touchingPoints = GetConnectedPoints(currentPoint);

            //if (touchingPoints.Length > 1)
            //{
            //    for (int i = 1; i < touchingPoints.Length; i++)
            //    {
            //        addedPoints[touchingPoints[i].X, touchingPoints[i].Y] = true;
            //    }
            //}

            return touchingPoints[0];
        }

        private void AddToLine(Point startPoint)
        {
            line.Add(startPoint);
            addedPoints[startPoint.X, startPoint.Y] = true;
        }

        private Point[] GetConnectedPoints(Point startPoint)
        {
            List<Point> touchingPoints = new List<Point>();

            for (int i = 0; i < neighbours.Length; i++)
            {
                int neighbourX = startPoint.X + neighbours[i].X;
                int neighbourY = startPoint.Y + neighbours[i].Y;

                if (neighbourX >= 0 && neighbourX < xLength && neighbourY >= 0 && neighbourY < yLength)
                {
                    if (pointsArray[neighbourX, neighbourY] == true && addedPoints[neighbourX, neighbourY] == false)
                        touchingPoints.Add(new Point(neighbourX, neighbourY));
                }
            }

            return touchingPoints.ToArray();

        }

        private Point FindStartPoint()
        {
            bool startFound = false;
            int xStart = 0;
            int yStart = 0;

            for (int yPos = 0; yPos < yLength; yPos++)
            {
                for (int xPos = 0; xPos < xLength; xPos++)
                {
                    //checkedPoints[xPos, yPos] = true;

                    if (pointsArray[xPos, yPos] == true && GetConnectedPoints(new Point(xPos, yPos)).Length == 2)
                    {
                        startFound = true;

                        xStart = xPos;
                        yStart = yPos;

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
            int connectedNeighbours = 0;

            for (int i = 0; i < neighbours.Length; i++)
            {
                int neighbourX = xPos + neighbours[i].X;
                int neighbourY = yPos + neighbours[i].Y;

                if (neighbourX >= 0 && neighbourX < xLength && neighbourY >= 0 && neighbourY < yLength)
                {
                    if (pointsArray[neighbourX, neighbourY] == true)
                        connectedNeighbours++;
                }
            }

            return connectedNeighbours;
        }

        # region Get methods

        public List<Point> GetLine()
        {
            return line;
        }

        # endregion

        private void DrawPoints()
        {
            Bitmap image = new Bitmap(xLength, yLength);

            //Graphics g = Graphics.FromImage(image);

            for (int yPos = 0; yPos < yLength; yPos++)
            {
                for (int xPos = 0; xPos < xLength; xPos++)
                {
                    if (pointsArray[xPos, yPos])
                    {
                        image.SetPixel(xPos, yPos, Color.LawnGreen);
                    }
                    else
                        image.SetPixel(xPos, yPos, Color.Black);
                }
            }

            image.Save("Snake - ProcessedCurve.bmp");
        }
    }
}
