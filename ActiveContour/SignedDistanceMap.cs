using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace ActiveContour
{
    /// <summary>
    /// Creates a signed distance map from a binary image mask
    /// </summary>
    public class SignedDistanceMap
    {
        private double narrowBandSize = 0.0;

        double[,] signedDistanceMap;

        # region Constructors

        /// <summary>
        /// Initialises the signed distance map from a list of points which, when joined create the masks shape
        /// </summary>
        /// <param name="initialPoints"></param>
        /// <param name="imageWidth"></param>
        /// <param name="imageHeight"></param>
        /// <param name="narrowBandSize">The cut off size of the curves narrow band</param>
        public SignedDistanceMap(List<Point> initialPoints, int imageWidth, int imageHeight, double narrowBandSize)
        {
            signedDistanceMap = CreateSignedDistanceMap(initialPoints, imageWidth, imageHeight);

            this.narrowBandSize = narrowBandSize;
        }

        /// <summary>
        /// Initialises the signed distance map from a list of points which, when joined create the masks shape.
        /// There is no narrow band
        /// </summary>
        /// <param name="imageWidth"></param>
        /// <param name="imageHeight"></param>
        /// <param name="initialPoints"></param>
        public SignedDistanceMap(List<Point> initialPoints, int imageWidth, int imageHeight)
        {
            signedDistanceMap = CreateSignedDistanceMap(initialPoints, imageWidth, imageHeight);
        }

        # endregion

        private double[,] CreateSignedDistanceMap(List<Point> points, int imageWidth, int imageHeight)
        {
            bool[,] maskPoints = new bool[imageWidth, imageHeight];

            double[,] sdfPoints = new double[imageWidth, imageHeight];

            for (int yPos = 0; yPos < imageHeight; yPos++)
            {
                for (int xPos = 0; xPos < imageWidth; xPos++)
                {
                    if (IsPointWithinPolygon(xPos, yPos, points))
                        maskPoints[xPos, yPos] = true;
                    else
                        maskPoints[xPos, yPos] = false;
                }
            }

            /**
            Bitmap maskImage = new Bitmap(maskPoints.GetLength(0), maskPoints.GetLength(1));

            for (int y = 0; y < maskPoints.GetLength(1); y++)
            {
                for (int x = 0; x < maskPoints.GetLength(0); x++)
                {
                    if (maskPoints[x, y] == true)
                        maskImage.SetPixel(x, y, Color.Black);
                    else
                        maskImage.SetPixel(x, y, Color.White);
                }
            }

            //maskImage.Save("MaskBitmap.BMP");

             * */

            //Calculate euclidean distance to border for each point
            for (int yPos = 0; yPos < imageHeight; yPos++)
            {
                for (int xPos = 0; xPos < imageWidth; xPos++)
                {
                    Point currentPoint = new Point(xPos, yPos);

                    double shortestEuclideanDistanceToCurve = CalculateShortestDistance(points[points.Count - 1], points[0], currentPoint); //Check last line

                    for (int i = 0; i < points.Count - 1; i++)
                    {
                        double distance = CalculateShortestDistance(points[i], points[i + 1], currentPoint);

                        if (distance < shortestEuclideanDistanceToCurve)
                            shortestEuclideanDistanceToCurve = distance;
                    }

                    //Points are negative outside the curve and positive inside
                    if (IsPointWithinPolygon(xPos, yPos, points))
                        sdfPoints[xPos, yPos] = 0 - shortestEuclideanDistanceToCurve - 0.5;
                    else
                        sdfPoints[xPos, yPos] = shortestEuclideanDistanceToCurve + 0.5;
                }
            }

            return sdfPoints;
        }

        /// <summary>
        /// Calculates the euclidean distance between to points
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <returns></returns>
        private double DistanceBetweenPoints(Point point1, Point point2)
        {
            double distance = 0.0;

            double length = Math.Pow((point2.X - point1.X), 2);
            double height = Math.Pow((point2.Y - point1.Y), 2);

            distance = Math.Sqrt(length + height);

            return distance;
        }

        /// <summary>
        /// Calculates the shortest distance between a point and a line 
        /// </summary>
        /// <param name="end1">Start of the line</param>
        /// <param name="end2">End of the line</param>
        /// <param name="point">Point</param>
        /// <returns></returns>
        private double CalculateShortestDistance(Point end1, Point end2, Point point)
        {
            double shortestDistance = 0.0;

            double pointsWidth = end2.X - end1.X;
            double pointsHeight = end2.Y - end1.Y;

            double dist = Math.Pow(pointsWidth, 2) + Math.Pow(pointsHeight, 2);

            double u = (((point.X - end1.X) * pointsWidth) + ((point.Y - end1.Y)) * pointsHeight) / dist;

            if (u > 1)
                u = 1;
            else if (u < 0)
                u = 0;

            double x = end1.X + u * pointsWidth;
            double y = end1.Y + u * pointsHeight;

            double dx = x - point.X;
            double dy = y - point.Y;

            shortestDistance = Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2));

            return shortestDistance;
        }

        /// <summary>
        /// Checks if a given point is within the bounds of a given polygon
        /// </summary>
        /// <param name="xPoint">The x position of the point to check</param>
        /// <param name="yPoint">The y position of the point to check</param>
        /// <param name="polygonPoints">A List of all the corner Points of the polgon</param>
        /// <returns></returns>
        private bool IsPointWithinPolygon(int xPoint, int yPoint, List<Point> polygonPoints)
        {
            Point p1, p2;

            bool inside = false;

            if (polygonPoints.Count < 3)
            {
                return inside;
            }

            Point oldPoint = new Point(polygonPoints[polygonPoints.Count - 1].X, polygonPoints[polygonPoints.Count - 1].Y);

            for (int i = 0; i < polygonPoints.Count; i++)
            {
                Point newPoint = new Point(polygonPoints[i].X, polygonPoints[i].Y);

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

                if ((newPoint.X < xPoint) == (xPoint <= oldPoint.X) && ((long)yPoint - (long)p1.Y) * (long)(p2.X - p1.X) < ((long)p2.Y - (long)p1.Y) * (long)(xPoint - p1.X))
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
            return signedDistanceMap;
        }

        # endregion

        # region Set methods

        public void SetNarrowBandSize(double narrowBandSize)
        {
            this.narrowBandSize = narrowBandSize;
        }

        # endregion
    }
}
