using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edges;

namespace TwoStageHoughTransform
{
    /// <summary>
    /// Locates the EdgePoints in an image and creates a List 
    /// 
    /// </summary>
    class EdgePointsFinder
    {
        private int edgesInImage;

        private bool[] edgeData;
        private int imageWidth, imageHeight;

        private EdgePointData edgePointData;

        # region properties

        public EdgePointData EdgePointData
        {
            get
            {
                return edgePointData;
            }
        }

        public int NumOfEdges
        {
            get
            {
                return edgesInImage;
            }
        }

        # endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="edgeData"></param>
        /// <param name="imageWidth"></param>
        /// <param name="imageHeight"></param>
        public EdgePointsFinder(bool[] edgeData, int imageWidth, int imageHeight)
        {
            this.edgeData = edgeData;
            this.imageWidth = imageWidth;
            this.imageHeight = imageHeight;
        }

        #endregion

        /// <summary>
        /// Runs the EdgePointsFinder
        /// </summary>
        public void RunFinder()
        {
            edgePointData = new EdgePointData(imageWidth, imageHeight);
            
            List<Edge> edges = FindEdgesInImage();

            edgesInImage = edges.Count;

            CheckEgeLengths(edges);

            CreateEdgePoints(edges);
        }

        private void CheckEgeLengths(List<Edge> edges)
        {
            for (int i = 0; i < edges.Count; i++)
            {
                if (edges[i].EdgeLength > imageWidth)
                {
                    Console.WriteLine("TOO LONG");
                }
            }
        }

        /// <summary>
        /// Finds and returns all edges in the given edge detected image
        /// </summary>
        private List<Edge> FindEdgesInImage()
        {
            List<Edge> edgesInImage = new List<Edge>();

            FindEdges findEdges = new FindEdges(edgeData, imageWidth, imageHeight);
            findEdges.SetHorizontalWrap(true);
            findEdges.find();
            edgesInImage = findEdges.getEdges();

            return edgesInImage;
        }

        /// <summary>
        /// Converts Edges in EdgePoints
        /// </summary>
        /// <param name="edges"></param>
        /// <returns>List of EdgePoints</returns>
        private void CreateEdgePoints(List<Edge> edges)
        {
            for (int edgeCounter = 0; edgeCounter < edges.Count; edgeCounter++)
            {
                Edge edge = edges[edgeCounter];

                List<Point> currentEdgePoints = edge.Points;

                int edgeLength = currentEdgePoints.Count;

                for (int pointCounter = 0; pointCounter < edgeLength; pointCounter++)
                {
                    Point currentPoint = currentEdgePoints[pointCounter];

                    int xPos = currentPoint.X;
                    int yPos = currentPoint.Y;

                    //EdgePoint edgePoint = new EdgePoint(xPos, yPos, edgeCounter, edgeLength);

                    //createdEdgePoints.Add(edgePoint);

                    edgePointData.AddEdgePoint(xPos, yPos, edgeCounter, edgeLength);
                }
            }
        }
    }
}
