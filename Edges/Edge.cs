using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Drawing;

namespace Edges
{
    /// <summary>
    /// A class which defines an edge and it's points
    /// 
    /// Author - Terry Malone (trm8@aber.ac.uk)
    /// version 1.1
    /// </summary>
    [DefaultPropertyAttribute("Edge Name")]
    public class Edge
    {
        private List<Point> points = new List<Point>();
        private int highestXPoint, lowestXPoint, highestYPoint, lowestYPoint;
        private Point edgeEnd1, edgeEnd2;

        private int azimuthGuess;

        private int imageWidth;

        private string edgeName;

        #region properties

        # region PropertyGrid attributes

        [CategoryAttribute("Edge Name"), DescriptionAttribute("The name of the edge")]
        public string EdgeName
        {
            get { return edgeName; }
            set { edgeName = value; }
        }

        [CategoryAttribute("Edge Length"), DescriptionAttribute("The length of the edge")]
        public string EdgeLengthString
        {
            get { return points.Count.ToString(); }
        }

        # endregion
        
        /// <summary>
        /// The width of the image the edge is from
        /// </summary>
        /// <returns></returns>
        public int ImageWidth
        {
            get { return imageWidth; }
        }

        /// <summary>
        /// Method which returns an ArrayList of all the points in the edge
        /// </summary>
        /// <returns>The ArrayList of Points</returns>
        public List<Point> Points
        {
            get { return points; }
        }

        /// <summary>
        /// The x position of the highest edge point
        /// </summary>
        public int HighestXPoint
        {
            get { return highestXPoint; }
        }

        /// <summary>
        /// The x position of the lowest edge point
        /// </summary>
        public int LowestXPoint
        {
            get { return lowestXPoint; }
        }

        /// <summary>
        /// The y position of the highest edge point
        /// </summary>
        public int HighestYPoint
        {
            get { return highestYPoint; }
        }

        /// <summary>
        /// The y position of the lowest edge point
        /// </summary>
        public int LowestYPoint
        {
            get { return lowestYPoint; }
        }

        /// <summary>
        /// The length of the edge
        /// </summary>
        public int EdgeLength
        {
            get { return points.Count; }
        }

        /// <summary>
        /// Method which returns the first end of the edge
        /// </summary>
        /// <returns>Point representing the first edge</returns>
        public Point EdgeEnd1
        {
            get { return edgeEnd1; }
            set
            {
                if (points.Contains(value))
                    edgeEnd1 = value;
            }
        }

        /// <summary>
        /// Method which returns the second end of the edge
        /// </summary>
        /// <returns>Point representing the second edge</returns>
        public Point EdgeEnd2
        {
            get { return edgeEnd2; }
            set
            {
                if (points.Contains(value))
                    edgeEnd2 = value;
            }
        }
        /// <summary>
        /// Returns an estimation of the edges resultant sinusoid's azimuth
        /// </summary>
        /// <returns></returns>
        public int AzimuthGuess
        {
            get { return azimuthGuess; }
        }

        #endregion properties

        /// <summary>
        /// Constructor method
        /// </summary>
        public Edge(int imageWidth)
        {
            lowestXPoint = 2147483647;
            highestXPoint = 0;
            lowestYPoint = 2147483647;
            highestYPoint = 0;

            this.imageWidth = imageWidth;
        }

        /// <summary>
        /// Method which adds a new point to the edge
        /// </summary>
        /// <param name="point">The point to add</param>
        public void AddPoint(Point point)
        {
            AddPoint(point.X, point.Y);
        }

        /// <summary>
        /// Method which adds a new point to the edge
        /// </summary>
        /// <param name="xPoint">The x position of the new point</param>
        /// <param name="yPoint">The y position of the new point</param>
        public void AddPoint(int xPoint, int yPoint)
        {
            points.Add(new Point(xPoint, yPoint));

            CheckBounds(xPoint, yPoint);
        }

        public void RemovePoint(Point pointToRemove)
        {
            if (points.Contains(pointToRemove))
                points.Remove(pointToRemove);

        }

        /// <summary>
        /// Checks if given point is the lowest or highest
        /// </summary>
        /// <param name="xPoint">The x position of the given point</param>
        /// <param name="yPoint">The y position of the given point</param>
        private void CheckBounds(int xPoint, int yPoint)
        {
            if (yPoint < lowestYPoint)
                lowestYPoint = yPoint;

            if (yPoint > highestYPoint)
            {
                highestYPoint = yPoint;
                azimuthGuess = (int)((double)xPoint / ((double)imageWidth / 360.0));
            }

            if (xPoint < lowestXPoint)
                lowestXPoint = xPoint;

            if (xPoint > highestXPoint)
                highestXPoint = xPoint;
        }

        /// <summary>
        /// Method which combines this edge and another edge
        /// </summary>
        /// <param name="newEdge">The Edge to combine with this edge</param>
        public void AddEdge(Edge newEdge)
        {
            points.AddRange(newEdge.Points);

            CalculateEndPoints();
        }

        /// <summary>
        /// Method which combines this edge and another edge
        /// </summary>
        /// <param name="newEdge">The Edge to combine with this edge</param>
        public void AddEdgeToStart(Edge newEdge)
        {
            points.InsertRange(0, newEdge.Points);

            CalculateEndPoints();
        }

        /// <summary>
        /// Method which recalculates the high and low x and y points of the edge
        /// </summary>
        public void CalculateEndPoints()
        {
            lowestXPoint = 2147483647;
            highestXPoint = 0;
            lowestYPoint = 2147483647;
            highestYPoint = 0;

            Point currentPoint;

            for (int i = 0; i < points.Count; i++)
            {
                currentPoint = points[i];

                CheckBounds(currentPoint.X, currentPoint.Y);
            }
        }

        /// <summary>
        /// Sorts the edge from left to right and assigns the edge ends
        /// </summary>
        public void SortEdge()
        {
            SortPointsHorizontally();

            SortPointsVertically();
            
            ChecksIfEdgesWrap();
        }

        /// <summary>
        /// Sorts all the points in a line from left to right based on their x co-ordinate
        /// </summary>
        private void SortPointsHorizontally()
        {
            var sortedPoints = points.OrderBy(p => p.X);
            points = sortedPoints.ToList();
        }

        /// <summary>
        /// Ensures that points with the same x coordinate appear in the correct order vertically
        /// </summary>
        private void SortPointsVertically()
        {
            int startSort = 0;
            int endSort = 0;
            int previousPoint = points[0].X;
            bool started = false;
            bool ended = false;

            for (int i = 1; i < points.Count; i++)
            {
                if (started)
                {
                    if (points[i].X != previousPoint)
                    {
                        endSort = i;
                        ended = true;
                    }
                }

                if (!started && points[i].X != previousPoint)
                {
                    started = true;
                    startSort = i;
                }

                if (started && ended)
                {
                    SortLine(startSort, endSort);
                    started = false;
                    ended = false;
                }

                previousPoint = points[i].X;
            }

            edgeEnd1 = points[0];
            edgeEnd2 = points[points.Count - 1];
        }

        /// <summary>
        /// Sorts a list of points based on the order they appear along a line 
        /// </summary>
        /// <param name="startSort">The start point to sort</param>
        /// <param name="endSort">The end point to sort</param>
        private void SortLine(int startSort, int endSort)
        {
            bool swapped;
            int beforeY = points[startSort - 1].Y;

            do
            {
                swapped = false;

                for (int i = startSort; i < endSort - 1; i++)
                {
                    if (Math.Max(points[i].Y, beforeY) - Math.Min(points[i].Y, beforeY) > Math.Max(points[i + 1].Y, beforeY) - Math.Min(points[i + 1].Y, beforeY))
                    {
                        Point temp = points[i];
                        points[i] = points[i + 1];
                        points[i + 1] = temp;

                        swapped = true;
                    }
                }
            } while (swapped);
        }

        /// <summary>
        /// Checks if edges wrap over image and if so adjusts edgeEnd1 and edgeEnd2
        /// </summary>
        private void ChecksIfEdgesWrap()
        {
            Point firstPoint = points[0];
            Point lastPoint = points[points.Count - 1];

            int end1Index = 0;

            if (firstPoint.X == 0)                                                              //If first point goes to the left edge check if it wraps 
            {
                if (lastPoint.X == imageWidth - 1)                                                //If last point goes to the right edge 
                {
                    if (lastPoint.Y >= firstPoint.Y - 1 && lastPoint.Y <= firstPoint.Y + 1)     //If points wrap calculate new edge ends 
                    {
                        int firstPos = points.Count - 1;

                        bool endFound = false;

                        //Move left until an edge is found
                        int nextPos = firstPos - 1;

                        do
                        {
                            if (nextPos >= 0 && (points[nextPos].X == points[firstPos].X || points[nextPos].X == points[firstPos].X - 1))
                            {
                                firstPos = nextPos;
                                nextPos--;
                            }
                            else
                            {
                                endFound = true;
                                edgeEnd1 = points[firstPos];

                                end1Index = firstPos;
                            }


                        } while (endFound == false);

                        //Move right until an edge is found
                        endFound = false;
                        int lastPos = 0;
                        nextPos = lastPos + 1;

                        do
                        {
                            if (nextPos <= imageWidth - 1 && (points[nextPos].X == points[lastPos].X || points[nextPos].X == points[lastPos].X + 1))
                            {
                                lastPos = nextPos;
                                nextPos++;
                            }
                            else
                            {
                                endFound = true;
                                edgeEnd2 = points[lastPos];
                            }

                        } while (endFound == false);
                    }

                    //Re arrange points from end1 to end2
                    points = points.Skip(end1Index).Concat(points.Take(end1Index)).ToList();

                }
            }
        }
        
        # region get methods

        public Point GetEdgeEnd(int edgeToReturn)
        {
            if (edgeToReturn == 1)
                return edgeEnd1;
            else
                return edgeEnd2;
        }

        # endregion
    }

}
