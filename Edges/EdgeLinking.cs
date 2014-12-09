using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace Edges
{
    /// <summary>
    /// Class which checks List of edges and links smaller edges onto larger ones
    /// 
    /// Author - Terry Malone (trm8@aber.ac.uk)
    /// version 1.1
    /// </summary>
    public class EdgeLinking
    {
        private int imageWidth, imageHeight;

        private List<Edge> edgesBeforeLinking;

        private List<Edge> linkedEdges = new List<Edge>();

        private bool[] checkedEdge;

        private List<int> edgesCloseEnough = new List<int>();
        private List<double> distanceToEdge = new List<double>();
        private List<int> edgeDistanceScores = new List<int>(); 


        private Edge currentEdge;
        private Edge edgeToAdd;

        private int minimumLength = 200;
        private int maxNeighbourDistance = 5;

        private bool horizontalWrap = true;

        private int overlapTolerance = 2;

        #region properties

        public int MaximumNeighbourDistance
        {
            get { return maxNeighbourDistance; }
            set
            {
                maxNeighbourDistance = value;
            }
        }

        public bool HorizontalWrap
        {
            get { return horizontalWrap; }
            set { horizontalWrap = value; }
        }

        /// <summary>
        /// The minimum length for edges to be considered strong eneough to keep
        /// </summary>
        public int MinimumLength
        {
            get { return minimumLength; }
            set { minimumLength = value; }

        }

        /// <summary>
        /// Returns the List of linked edges
        /// </summary>
        /// <returns>The linked edges</returns>
        public List<Edge> LinkedEdges
        {
            get { return linkedEdges; }
        }

        #endregion properties

        # region Constructor methods

        /// <summary>
        /// Consructor method
        /// </summary>
        /// <param name="edgesBeforeLinking">The edges to be checked for linking</param>
        public EdgeLinking(List<Edge> edgesBeforeLinking, int imageWidth, int imageHeight)
        {
            this.edgesBeforeLinking = edgesBeforeLinking;
            this.imageWidth = imageWidth;
            this.imageHeight = imageHeight;
        }

        /// <summary>
        /// Consructor method which accepts a minimum length 
        /// </summary>
        /// <param name="edgesBeforeLinking">The edges to be checked for linking</param>
        /// <param name="minimumLength">Edges smaller than this are not considered</param>
        public EdgeLinking(List<Edge> edgesBeforeLinking, int imageWidth, int imageHeight, int minimumLength)
        {
            this.edgesBeforeLinking = edgesBeforeLinking;
            this.minimumLength = minimumLength;
            this.imageWidth = imageWidth;
            this.imageHeight = imageHeight;
        }

        # endregion
        
        /// <summary>
        /// Method which implements the edge linking process
        /// </summary>
        public void Link()
        {
            if (edgesBeforeLinking.Count > 1)
            {
                edgesBeforeLinking.Sort(new EdgesLengthComparer());
                
                checkedEdge = new bool[edgesBeforeLinking.Count];
                
                int edge = 0;
                currentEdge = edgesBeforeLinking[edge];

                while (currentEdge.EdgeLength > minimumLength && edge < edgesBeforeLinking.Count)      //Check edges while there are still edges longer than minimumLength to check
                {
                    if (checkedEdge[edge] == false)     //If edge hasn't already been checked
                    {
                        checkedEdge[edge] = true;

                        CheckEnds(currentEdge);

                        linkedEdges.Add(currentEdge);
                    }

                    edge++;

                    if(edge < edgesBeforeLinking.Count)             
                        currentEdge = edgesBeforeLinking[edge];
                }
            }
            else if (edgesBeforeLinking.Count == 1) 
                linkedEdges.Add(edgesBeforeLinking[0]);
        }

        /// <summary>
        /// Checks both ends of an edge to see if any other edges can be joined
        /// </summary>
        /// <param name="currentEdge"></param>
        private void CheckEnds(Edge edgeToJoinTo)
        {
            CheckIfEndCanBeJoinedTo(edgeToJoinTo, 1);
            CheckIfEndCanBeJoinedTo(edgeToJoinTo, 2);
        }

        /// <summary>
        /// Checks if another edge is to be joined to this one
        /// </summary>
        /// <param name="edgeToJoinTo"></param>
        /// <param name="edgeEnd"></param>
        private void CheckIfEndCanBeJoinedTo(Edge edgeToJoinTo, int edgeEnd)
        {
            edgesCloseEnough.Clear();
            distanceToEdge.Clear();
            edgeDistanceScores.Clear();
            
            bool outlierFound = false;

            FindCloseEdges(edgeToJoinTo.GetEdgeEnd(edgeEnd), edgeEnd);
            
            if (edgesCloseEnough.Count > 0)
            {
                outlierFound = CheckIfOutlierCanBeJoinedTo(edgeToJoinTo, edgeEnd);

                if (outlierFound == false)
                {
                    CheckIfEdgeEndCanBeJoinedTo(edgeToJoinTo, edgeEnd);
                }
            }
        }

        /// <summary>
        /// Checks all edges to see if either of their ends are closer than minimumThreshold to the given point
        /// </summary>
        /// <param name="point"></param>
        private void FindCloseEdges(Point point, int edgeEnd)
        {
            Point checkPoint;

            for (int i = 0; i < edgesBeforeLinking.Count; i++)
            {
                if (checkedEdge[i] == false)
                {
                    if (edgeEnd == 1)
                        checkPoint = edgesBeforeLinking[i].EdgeEnd2;
                    else
                        checkPoint = edgesBeforeLinking[i].EdgeEnd1;

                    double distance = CalculateEuclideanDistance(point, checkPoint);

                    if (distance < maxNeighbourDistance && !PointsOverLap(point, edgeEnd, checkPoint))
                    {
                        AddToFoundEdges(i, distance);
                        //edgesCloseEnough.Add(position);
                        //distanceToEdge.Add(distance);
                    }
                }
            }
        }

        private void AddToFoundEdges(int position, double distance)
        {
            bool positionFound = false;
            int currentCount = 0;

            while (positionFound == false && currentCount < edgesCloseEnough.Count)
            {
                if (distance > distanceToEdge[currentCount])
                {
                    edgesCloseEnough.Insert(currentCount, position);
                    distanceToEdge.Insert(currentCount, distance);

                    positionFound = true;
                }

                currentCount++;
            }

            if (positionFound == false)
            {
                edgesCloseEnough.Add(position);
                distanceToEdge.Add(distance);
            }
        }

        /// <summary>
        /// Calculates the distance between 2 points
        /// </summary>
        private double CalculateEuclideanDistance(Point point1, Point point2)
        {
            double xDistance = Math.Max(point1.X, point2.X) - Math.Min(point1.X, point2.X);

            if (horizontalWrap && imageWidth - xDistance < xDistance)
                xDistance = imageWidth - xDistance;

            double yDistance = Math.Max(point1.Y, point2.Y) - Math.Min(point1.Y, point2.Y);

            double distance = Math.Sqrt((xDistance * xDistance) + (yDistance * yDistance));

            return distance;
        }

        /// <summary>
        /// Checks if any small outliers can be joined to an edge
        /// </summary>
        /// <param name="edgeToJoinTo">The main edge to check</param>
        /// <param name="edgeEnd">The edge end to join to</param>
        /// <returns>True if an outlier was added</returns>
        private bool CheckIfOutlierCanBeJoinedTo(Edge edgeToJoinTo, int edgeEnd)
        {
            //if (currentEdge.EdgeEnd2.Equals(new Point(227, 194)))
            //{
           //     MessageBox.Show("Stopping");
           // }

            bool outlierFound = false;
            double outlierMinimum = 5.0;

            int lowest = 100000;      //Default to high value
            int lowestPos = 0;
            int outlierMinimumAngle = 45;

            for (int i = 0; i < edgesCloseEnough.Count; i++)
            {
                int checkEdgePosition = edgesCloseEnough[i];

                double distance = distanceToEdge[i];

                if (distance <= outlierMinimum)
                {
                    int currentOutlierPosition = checkEdgePosition;

                    int currentOutlierScore = CalculateOutlierDirectionScore(edgeToJoinTo, edgeEnd, edgesBeforeLinking[currentOutlierPosition]);
                    
                    if (currentOutlierScore <= outlierMinimumAngle)
                    {
                        currentOutlierScore = Convert.ToInt32(currentOutlierScore * distance);

                        if (currentOutlierScore < lowest)
                        {
                            lowest = currentOutlierScore;
                            lowestPos = currentOutlierPosition;

                            outlierFound = true;
                        }
                    }
                }
            }

            //int combinedLength = currentEdge.EdgeLength + edgesBeforeLinking[lowestPos].EdgeLength;
            bool edgesOverlap = CheckIfEdgesOverlap(currentEdge, edgesBeforeLinking[lowestPos]);

            //if (lowest < outlierMinimumScore && combinedLength <= imageWidth)
            if (outlierFound == true && edgesOverlap == false)
            {
                outlierFound = true;

                edgeToAdd = edgesBeforeLinking[lowestPos];

                if(edgeEnd == 1)
                    currentEdge.AddEdgeToStart(edgeToAdd);
                else
                    currentEdge.AddEdge(edgeToAdd);

                checkedEdge[lowestPos] = true;

                ChangeEdgeEnd(edgeEnd, edgeToAdd);

                if (edgeEnd == 1)
                    CheckIfEndCanBeJoinedTo(currentEdge, 1);
                else
                    CheckIfEndCanBeJoinedTo(currentEdge, 2);
            }
            else
            {
                outlierFound = false;
            }

            return outlierFound;
        }

        private void CheckIfEdgeEndCanBeJoinedTo(Edge edgeToJoinTo, int edgeEnd)
        {
            int minimumEdgeScore = 1500;

            int lowest = 100000;     //default to high value
            int lowestPos = 0;

            if(currentEdge.EdgeEnd2.Equals(new Point(227,194)))
            {
                MessageBox.Show("Stopping");
            }
                

            for (int i = 0; i < edgesCloseEnough.Count; i++)
            {
                double areaScore;
                int directionScore;

                if (edgeEnd == 1)
                {
                    areaScore = CalculateAreaScore(edgeToJoinTo, edgeEnd, edgesBeforeLinking[edgesCloseEnough[i]], 2);
                    directionScore = CalculateEdgeDirectionScore(edgeToJoinTo, edgeEnd, edgesBeforeLinking[edgesCloseEnough[i]], 2);
                }
                else
                {
                    areaScore = CalculateAreaScore(edgeToJoinTo, edgeEnd, edgesBeforeLinking[edgesCloseEnough[i]], 1);
                    directionScore = CalculateEdgeDirectionScore(edgeToJoinTo, edgeEnd, edgesBeforeLinking[edgesCloseEnough[i]], 1);
                }

                long totalScore = Convert.ToInt32(areaScore * directionScore);

                if (totalScore > int.MaxValue)
                    totalScore = int.MaxValue;

                edgeDistanceScores.Add((int)totalScore);

                if (directionScore <= 45 && edgeDistanceScores[i] < lowest)
                {
                    lowest = edgeDistanceScores[i];
                    lowestPos = edgesCloseEnough[i];
                }
            }

            bool edgesOverlap = CheckIfEdgesOverlap(currentEdge, edgesBeforeLinking[lowestPos]);

            if (lowest < minimumEdgeScore && edgesOverlap == false)
            {
                edgeToAdd = edgesBeforeLinking[lowestPos];

                if (edgeEnd == 1)
                    currentEdge.AddEdgeToStart(edgeToAdd);
                else
                    currentEdge.AddEdge(edgeToAdd);

                checkedEdge[lowestPos] = true;

                ChangeEdgeEnd(edgeEnd, edgeToAdd);

                CheckIfEndCanBeJoinedTo(currentEdge, edgeEnd);
            }
        }

        /// <summary>
        /// Checks if the adding edge overlaps the main edge
        /// </summary>
        /// <param name="currentEdge"></param>
        /// <param name="addingEdge"></param>
        /// <returns></returns>
        private bool CheckIfEdgesOverlap(Edge currentEdge, Edge addingEdge)
        {
            bool edgesOverlap = false;

            int mainMin = currentEdge.EdgeEnd1.X + overlapTolerance;
            int mainMax = currentEdge.EdgeEnd2.X - overlapTolerance;

            int addingEdge1 = addingEdge.EdgeEnd1.X;
            int addingEdge2 = addingEdge.EdgeEnd2.X;
            
            if (horizontalWrap == false || mainMin < mainMax)   //If there is no horizontal wrap or the currentEdge doesn't wrap
            {
                if (addingEdge1 > mainMin && addingEdge1 < mainMax)
                    edgesOverlap = true;
                else if (addingEdge2 > mainMin && addingEdge2 < mainMax)
                    edgesOverlap = true;
            }
            else         //if edges wrap Edges wrap
            {
                if((addingEdge1 > mainMin && addingEdge1 < imageWidth) || (addingEdge1 > 0 && addingEdge1 < mainMax))
                    edgesOverlap = true;
                else if ((addingEdge2 > mainMin && addingEdge2 < imageWidth) || (addingEdge2 > 0 && addingEdge2 < mainMax))
                    edgesOverlap = true;
            }
            
            return edgesOverlap;
        }

        private double CalculateAreaScore(Edge edge1, int edge1End, Edge edge2, int edge2End)
        {
            double areaScore = 0;

            int edge1Direction = CalculateEdgeDirection(edge1, edge1End);
            int edge2Direction = CalculateEdgeDirection(edge2, edge2End);

            TriangleArea triangleArea = new TriangleArea(edge1.GetEdgeEnd(edge1End), edge1Direction, edge2.GetEdgeEnd(edge2End), edge2Direction);

            triangleArea.CalculateArea();

            if (triangleArea.GetTriangleNotPossible() == false)
                areaScore = triangleArea.GetArea();
            else
                areaScore = 1000;    //Large number so that this will not be chosen
            
            return areaScore;
        }

        /// <summary>
        /// Updates the edge end based on the new added edge
        /// </summary>
        /// <param name="edgeEnd"></param>
        /// <param name="edgeToAdd"></param>
        private void ChangeEdgeEnd(int edgeEnd, Edge edgeToAdd)
        {
            if (edgeEnd == 1)
            {
                if (Math.Max(edgeToAdd.EdgeEnd1.X, currentEdge.EdgeEnd1.X) - Math.Min(edgeToAdd.EdgeEnd1.X, currentEdge.EdgeEnd1.X) >
                    Math.Max(edgeToAdd.EdgeEnd2.X, currentEdge.EdgeEnd1.X) - Math.Min(edgeToAdd.EdgeEnd2.X, currentEdge.EdgeEnd1.X))
                {
                    currentEdge.EdgeEnd1 = edgeToAdd.EdgeEnd1;
                }
                else
                    currentEdge.EdgeEnd1 = edgeToAdd.EdgeEnd2;

            }
            else
            {
                if (Math.Max(edgeToAdd.EdgeEnd1.X, currentEdge.EdgeEnd2.X) - Math.Min(edgeToAdd.EdgeEnd1.X, currentEdge.EdgeEnd2.X) >
                    Math.Max(edgeToAdd.EdgeEnd2.X, currentEdge.EdgeEnd2.X) - Math.Min(edgeToAdd.EdgeEnd2.X, currentEdge.EdgeEnd2.X))
                {
                    currentEdge.EdgeEnd2 = edgeToAdd.EdgeEnd1;
                }
                else
                    currentEdge.EdgeEnd2 = edgeToAdd.EdgeEnd2;
            }
        }

        /// <summary>
        /// Checks if the checkPoint overlaps the main point
        /// </summary>
        /// <param name="point">The main point</param>
        /// <param name="edgeEnd">Which end the point belongs to</param>
        /// <param name="checkPoint"></param>
        /// <returns></returns>
        private bool PointsOverLap(Point point, int edgeEnd, Point checkPoint)
        {
            bool overlap = false;

            if (edgeEnd == 1)
            {
                if (point.X + overlapTolerance <= checkPoint.X && (checkPoint.X - point.X) < maxNeighbourDistance)
                    overlap = true;
            }
            else
            {
                if (point.X - overlapTolerance >= checkPoint.X && (point.X - checkPoint.X) < maxNeighbourDistance)
                    overlap = true;
            }

            return overlap;
        }

        # region Direction methods

        /// <summary>
        /// Scores two edge end points based on their relative directions
        /// A score of 0 is best and will occur if ends going in opposite directions score highest (i.e. 0-180 degrees)
        /// </summary>
        /// <param name="point"></param>
        /// <param name="point_2"></param>
        private int CalculateEdgeDirectionScore(Edge edge1, int edge1End, Edge edge2, int edge2End)
        {
            int direction1, direction2;

            direction1 = CalculateEdgeDirection(edge1, edge1End);
            direction2 = CalculateEdgeDirection(edge2, edge2End);
            
            int score = Math.Abs(180 - Math.Abs(direction1-direction2));
            
            return score;
        }

        /// <summary>
        /// Scores an edge end and an outlier based on their relative directions
        /// A score of 0 is best and will occur if ends going in opposite directions score highest (i.e. 0-180 degrees)
        /// </summary>
        private int CalculateOutlierDirectionScore(Edge edge1, int edgeEnd, Edge outlier)
        {
            int direction1, direction2;

            direction1 = CalculateEdgeDirection(edge1, edgeEnd);
            
            if(edgeEnd == 1)
                direction2 = CalculateDirection(outlier.Points[0], edge1.EdgeEnd1);
            else
                direction2 = CalculateDirection(outlier.Points[0], edge1.EdgeEnd2);

            int score = Math.Abs(180 - Math.Abs(direction1 - direction2));

            return score;
        }

        /// <summary>
        /// Returns the direction of the edge (0=North moving clockwise to
        /// 7=North west) 
        /// </summary>
        /// <param name="edge">The Edge to check</param>
        /// <param name="edgeEnd">The end to check 1 or 2</param>
        /// <returns></returns>
        private int CalculateEdgeDirection(Edge edge, int edgeEnd)
        {
            int direction = 0;

            Point startPoint;
            Point endPoint;

            if(edge.EdgeLength >= 5)
            {
                int endLength = 5;

                if (edgeEnd == 1)
                {
                    startPoint = edge.Points[endLength-1];
                    endPoint = edge.EdgeEnd1;
                }
                else
                {
                    int count = edge.EdgeLength;

                    startPoint = edge.Points[count - endLength];
                    endPoint = edge.EdgeEnd2;
                }

                //If points cross border change one
                if (horizontalWrap && (Math.Max(startPoint.X, endPoint.X) - Math.Min(startPoint.X, endPoint.X) > endLength))
                {
                    if (Math.Min(startPoint.X, endPoint.X) == startPoint.X)
                        startPoint.X += imageWidth;
                    else
                        endPoint.X += imageWidth;
                }

                direction = CalculateDirection(startPoint, endPoint);
            }
            else if (edge.EdgeLength < 5 && edge.EdgeLength > 2)
            {
                int endLength = edge.EdgeLength;

                if (edgeEnd == 1)
                {
                    startPoint = edge.Points[endLength - 1];
                    endPoint = edge.Points[0];
                }
                else
                {
                    startPoint = edge.Points[0];
                    endPoint = edge.Points[endLength - 1];
                }

                direction = CalculateDirection(startPoint, endPoint);
            }
            else
            {
                direction = 0;
            }

            return direction;
        }

        private int CalculateDirection(Point startPoint, Point endPoint)
        {
            int direction;

            //If points cross border change one
            if(horizontalWrap && (Math.Max(startPoint.X, endPoint.X) - Math.Min(startPoint.X, endPoint.X) > 100))    //100 is chosen since images are likely to be wider than this length and edges much smaller
            {
                if (Math.Min(startPoint.X, endPoint.X) == startPoint.X)
                    startPoint.X += imageWidth;
                else
                    endPoint.X += imageWidth;
            }

            LineDirection lineDirection = new LineDirection(startPoint, endPoint);
            lineDirection.Calculate();
            direction = lineDirection.GetDirection();

            return direction;
        }

        # endregion
    }

}
