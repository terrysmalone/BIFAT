using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwoStageHoughTransform
{
    public class EdgePointData
    {
        private int imageHeight, imageWidth;
        private bool[,] isEdgePointHere;

        int[,] edgeNums;
        int[,] edgeLengths;

        private List<List<Point>> edgeNumPoints;

        # region Properties

        public int NumberOfEdges
        {
            get
            {
                return edgeNumPoints.Count;
            }
        }

        # endregion

        public EdgePointData(int imageWidth, int imageHeight)
        {
            this.imageWidth = imageWidth;
            this.imageHeight = imageHeight;

            isEdgePointHere = new bool[imageWidth, imageHeight];
            edgeNums = new int[imageWidth, imageHeight];
            edgeLengths = new int[imageWidth, imageHeight];

            edgeNumPoints = new List<List<Point>>();
        }

        public void AddEdgePoint(int xPos, int yPos, int edgeNum, int edgeLength)
        {
            isEdgePointHere[xPos, yPos] = true;

            edgeNums[xPos, yPos] = edgeNum;
            edgeLengths[xPos, yPos] = edgeLength;

            if (edgeNum >= edgeNumPoints.Count)
            {
                //Create new list
                List<Point> points = new List<Point>();
                edgeNumPoints.Add(points);
            }

            edgeNumPoints[edgeNum].Add(new Point(xPos, yPos));
        }

        public void RemoveEdgesAtNum(int edgeNum)
        {
            List<Point> pointsToRemove = edgeNumPoints[edgeNum];

            int numOfPointsToRemove = pointsToRemove.Count;

            for (int i = 0; i < numOfPointsToRemove; i++)
            {
                Point pointToRemove = pointsToRemove[i];
                
                int x = pointToRemove.X;
                int y = pointToRemove.Y;

                isEdgePointHere[x, y] = false;
            }

            edgeNumPoints[edgeNum].Clear();
        }

        public bool IsEdgePointAt(int x, int y)
        {
            return isEdgePointHere[x, y];

        }

        # region Get methods

        public int GetEdgeNumAt(int x, int y)
        {
            return edgeNums[x, y];
        }

        public int GetEdgeLengthAt(int x, int y)
        {
            return edgeLengths[x, y];
        }

        public List<Point> GetPointsAtNum(int num)
        {
            return edgeNumPoints[num];
        }

        # endregion
    }
}
