using System;
using System.Collections.Generic;
using System.Drawing;

namespace ActiveContour
{
    /// <summary>
    /// Creates a signed distance map from a binary image mask
    /// </summary>
    public class SignedDistanceMap
    {
        private double[,] m_SignedDistanceMap;

        # region Constructors
        
        /// <summary>
        /// Initialises the signed distance map from a list of points which, when joined create the masks shape.
        /// There is no narrow band
        /// </summary>
        /// <param name="imageWidth"></param>
        /// <param name="imageHeight"></param>
        /// <param name="initialPoints"></param>
        public SignedDistanceMap(List<Point> initialPoints, int imageWidth, int imageHeight)
        {
            CreateSignedDistanceMap(initialPoints, imageWidth, imageHeight);
        }

        # endregion

        private void CreateSignedDistanceMap(List<Point> points, int imageWidth, int imageHeight)
        {
            var maskPoints = new bool[imageWidth, imageHeight];

            m_SignedDistanceMap = new double[imageWidth, imageHeight];

            for (var yPos = 0; yPos < imageHeight; yPos++)
            {
                for (var xPos = 0; xPos < imageWidth; xPos++)
                {
                    if (IsPointWithinPolygon(xPos, yPos, points))
                        maskPoints[xPos, yPos] = true;
                    else
                        maskPoints[xPos, yPos] = false;
                }
            }

            
            //Bitmap maskImage = new Bitmap(maskPoints.GetLength(0), maskPoints.GetLength(1));

            //for (int y = 0; y < maskPoints.GetLength(1); y++)
            //{
            //    for (int x = 0; x < maskPoints.GetLength(0); x++)
            //    {
            //        if (maskPoints[x, y] == true)
            //            maskImage.SetPixel(x, y, Color.Black);
            //        else
            //            maskImage.SetPixel(x, y, Color.White);
            //    }
            //}

            ////maskImage.Save("MaskBitmap.BMP");
            
            //Calculate euclidean distance to border for each point
            for (var yPos = 0; yPos < imageHeight; yPos++)
            {
                for (var xPos = 0; xPos < imageWidth; xPos++)
                {
                    var currentPoint = new Point(xPos, yPos);

                    var shortestEuclideanDistanceToCurve = CalculateShortestDistance(points[points.Count - 1], points[0], currentPoint); //Check last line

                    for (var i = 0; i < points.Count - 1; i++)
                    {
                        var distance = CalculateShortestDistance(points[i], points[i + 1], currentPoint);

                        if (distance < shortestEuclideanDistanceToCurve)
                            shortestEuclideanDistanceToCurve = distance;
                    }

                    //Points are negative outside the curve and positive inside
                    if (IsPointWithinPolygon(xPos, yPos, points))
                        m_SignedDistanceMap[xPos, yPos] = 0 - shortestEuclideanDistanceToCurve - 0.5;
                    else
                        m_SignedDistanceMap[xPos, yPos] = shortestEuclideanDistanceToCurve + 0.5;
                }
            }
        }

        /// <summary>
        /// Calculates the shortest distance between a point and a line 
        /// </summary>
        /// <param name="end1">Start of the line</param>
        /// <param name="end2">End of the line</param>
        /// <param name="point">Point</param>
        /// <returns></returns>
        private static double CalculateShortestDistance(Point end1, Point end2, Point point)
        {
            var pointsWidth = end2.X - end1.X;
            var pointsHeight = end2.Y - end1.Y;

            var dist = Math.Pow(pointsWidth, 2) + Math.Pow(pointsHeight, 2);

            var u = (((point.X - end1.X) * pointsWidth) + ((point.Y - end1.Y)) * pointsHeight) / dist;

            if (u > 1)
                u = 1;
            else if (u < 0)
                u = 0;

            var x = end1.X + u * pointsWidth;
            var y = end1.Y + u * pointsHeight;

            var dx = x - point.X;
            var dy = y - point.Y;

            var shortestDistance = Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2));

            return shortestDistance;
        }
        
        private static bool IsPointWithinPolygon(int xPoint, int yPoint, List<Point> polygonPoints)
        {
            if (polygonPoints == null) throw new ArgumentNullException(nameof(polygonPoints));

            var inside = false;

            if (polygonPoints.Count < 3)
            {
                return false;
            }

            var oldPoint = new Point(polygonPoints[polygonPoints.Count - 1].X, 
                                       polygonPoints[polygonPoints.Count - 1].Y);

            foreach (var polygonPoint in polygonPoints)
            {
                var newPoint = new Point(polygonPoint.X, polygonPoint.Y);

                Point p1;
                Point p2;

                if (newPoint.X > oldPoint.X)
                {
                    p1 = oldPoint;
                    p2 = newPoint;
                }
                else
                {
                    p1 = newPoint;
                    p2 = oldPoint;
                }

                if ((newPoint.X < xPoint) == (xPoint <= oldPoint.X) 
                    && ((long)yPoint - (long)p1.Y) * (long)(p2.X - p1.X) 
                    < ((long)p2.Y - (long)p1.Y) * (long)(xPoint - p1.X))
                {
                    inside = !inside;
                }

                oldPoint = newPoint;
            }

            return inside;
        }

        # region Get methods

        public double[,] GetSignedDistanceMap()
        {
            return m_SignedDistanceMap;
        }

        # endregion
    }
}
