using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Edges
{
    /// <summary>
    /// A class which checks a List of Edge objects and joins those whose end
    /// points are closer than a given value
    /// 
    /// Author - Terry Malone (trm8@aber.ac.uk)
    /// version 1.1
    /// </summary>
    public class JoinEdges
    {
        private List<Edge> allEdges, removedEdges, newEdges;
        private int distanceToBridge = 10;       //Joins edges if their gaps are smaller than this value
        private int imageHeight, imageWidth;

        private bool horizontalWrap = true;

        /// <summary>
        /// Constructor methods
        /// </summary>
        /// <param name="allEdges">A List of all the Edges to be checked</param>
        /// <param name="distanceToBridge">The maximum size gap to bridge between edges</param>
        public JoinEdges(List<Edge> allEdges, int distanceToBridge, int imageWidth, int imageheight)
        {
            this.allEdges = allEdges;
            this.distanceToBridge = distanceToBridge;
            this.imageWidth = imageWidth;
            this.imageHeight = imageheight;

            newEdges = new List<Edge>();
            removedEdges = new List<Edge>();
        }

        /// <summary>
        /// Method which joins edges that are within a given distance of each other
        /// </summary>
        public void join()
        {
            allEdges.Sort(new EdgesLengthComparer());

            newEdges.Clear();
            removedEdges.Clear();

            checkAllEdgesForJoin();

            createNewArray();
        }

        /// <summary>
        /// Checks all edges to see if there is a join
        /// </summary>
        private void checkAllEdgesForJoin()
        {
            Edge thisEdge, compareEdge;

            int thisEdgeMid, compareEdgeMid;

            for (int i = 0; i < allEdges.Count() - 1; i++)
            {
                thisEdge = allEdges[i];
                thisEdge.CalculateEndPoints();

                thisEdgeMid = thisEdge.LowestYPoint + (thisEdge.HighestYPoint - thisEdge.LowestYPoint);

                for (int j = i + 1; j < allEdges.Count(); j++)
                {
                    compareEdge = allEdges[j];
                    compareEdge.CalculateEndPoints();

                    compareEdgeMid = compareEdge.LowestYPoint + (compareEdge.HighestYPoint - compareEdge.LowestYPoint);

                    if (areEdgesVerticallyClose(thisEdgeMid, compareEdgeMid, 50))
                    {
                        if (compareEdges(thisEdge, compareEdge) == true && checkOverlap(thisEdge, compareEdge) == false)
                        {
                            thisEdge.AddEdge(compareEdge);

                            if (thisEdge.EdgeEnd1.X > compareEdge.EdgeEnd1.X)
                                thisEdge.EdgeEnd1 = compareEdge.EdgeEnd1;

                            if (thisEdge.EdgeEnd2.X < compareEdge.EdgeEnd2.X)
                                thisEdge.EdgeEnd2 = compareEdge.EdgeEnd2;

                            removedEdges.Add(compareEdge);

                            i++;    //We do not check compareEdge on the next iteration because it has been removed
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Checks if two positions are vertically close
        /// </summary>
        /// <param name="firstEdge">The first edge to compare</param>
        /// <param name="secondEdge">The second edge to compare</param>
        /// <param name="distance">Edges are considered close if they are closer than this distance</param>
        /// <returns>True if edges are close, ffalse if not</returns>
        private bool areEdgesVerticallyClose(int firstEdge, int secondEdge, int distance)
        {
            if (Math.Max(firstEdge, secondEdge) - Math.Min(firstEdge, secondEdge) <= distance)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Method which compares 2 edges and returns true if they should be joined
        /// </summary>
        /// <param name="edge1">The first Edge to compare</param>
        /// <param name="edge2">The second edge to compare</param>
        /// <returns>True if they should be joined</returns>
        private bool compareEdges(Edge edge1, Edge edge2)
        {
            double firstDistance = calculateDistance(edge1.EdgeEnd2, edge2.EdgeEnd1);
            double secondDistance = calculateDistance(edge1.EdgeEnd1, edge2.EdgeEnd2);

            //MessageBox.Show("Edge 1: " + edge1.EdgeEnd1 + ", " + edge1.EdgeEnd2 + "\nEdge 2: " + edge2.EdgeEnd1 + ", " + edge2.EdgeEnd2 + "\nfirstDistance: " + firstDistance + " - secondDistance: " + secondDistance);

            if ((firstDistance <= distanceToBridge && firstDistance >= 0) || (secondDistance <= distanceToBridge && secondDistance >= 0))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Checks if two given edges overlap along the horizontal (If they do it is unlikely that they belong
        /// to the same edge)
        /// </summary>
        /// <param name="edge1">The first edge</param>
        /// <param name="edge2">The second edge</param>
        /// <returns>True if edges overlap</returns>
        private bool checkOverlap(Edge edge1, Edge edge2)
        {
            int numberOfOverlaps = 0;       //Edges sometimes overlap by a few pixels

            List<Point> edge1Points = edge1.Points;
            List<Point> edge2Points = edge2.Points;

            for (int edge1Point = 0; edge1Point < edge1Points.Count; edge1Point++)
            {
                for (int edge2Point = 0; edge2Point < edge2Points.Count; edge2Point++)
                {
                    if (edge1Points[edge1Point].X.Equals(edge2Points[edge2Point].X))
                        numberOfOverlaps++;
                }
            }

            if (numberOfOverlaps > 10)
                return true;
            else
                return false;

        }

        /// <summary>
        /// Method which calculates the distance between 2 points
        /// </summary>
        /// <param name="firstPoint">The first point</param>
        /// <param name="secondPoint">The second point</param>
        /// <returns>The distance between the two given points</returns>
        private double calculateDistance(Point firstPoint, Point secondPoint)
        {
            double xDist, wrapXDist, yDist;
            double smallestXDist;

            xDist = Math.Max(firstPoint.X, secondPoint.X) - Math.Min(firstPoint.X, secondPoint.X);

            if (horizontalWrap)
            {
                wrapXDist = (imageWidth - Math.Max(firstPoint.X, secondPoint.X)) + Math.Min(firstPoint.X, secondPoint.X);

                smallestXDist = Math.Min(xDist, wrapXDist);
            }
            else
                smallestXDist = xDist;

            yDist = Math.Max(firstPoint.Y, secondPoint.Y) - Math.Min(firstPoint.Y, secondPoint.Y);

            double hypotenuse = Math.Sqrt((smallestXDist * smallestXDist) + (yDist * yDist));
            return hypotenuse;
        }

        /// <summary>
        /// Method which creates a new array of the old edges minus the edges that
        /// are to be removed
        /// </summary>
        private void createNewArray()
        {
            for (int i = 0; i < allEdges.Count(); i++)
            {

                if (!removedEdges.Contains(allEdges[i]))
                {

                    newEdges.Add(allEdges[i]);
                }
            }
        }

        # region Set methods

        /// <summary>
        /// Method which sets the distanceToBridge
        /// </summary>
        /// <param name="distanceToBridge">The minimum distance to bridge between edges</param>
        public void setDistanceToBridge(int distanceToBridge)
        {
            this.distanceToBridge = distanceToBridge;
        }

        public void SetHorizontalWrap(bool horizontalWrap)
        {
            this.horizontalWrap = horizontalWrap;
        }

        # endregion

        # region Get methods

        /// <summary>
        /// Method which returns the joined edges
        /// </summary>
        /// <returns>A List of joined edges</returns>
        public List<Edge> getJoinedEdges()
        {
            return newEdges;
        }

        /// <summary>
        /// Returns the number of edges
        /// </summary>
        /// <returns>An int representing the number of edges</returns>
        public int getNumberOfEdges()
        {
            return newEdges.Count();
        }

        /// <summary>
        /// Returns the distance to bridge between edges
        /// </summary>
        /// <returns>The distance to bridge</returns>
        public int getDistanceToBridge()
        {
            return distanceToBridge;
        }

        # endregion
    }

}
