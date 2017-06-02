using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DrawEdges;
using DrawEdges.DrawEdgesFactory;
using EdgeFitting;

namespace TwoStageHoughTransform
{
    public class HoughTransform
    {
        private readonly bool[] m_EdgeData;
        private readonly int m_ImageWidth;
        private readonly int m_ImageHeight;

        private EdgePointData m_EdgePointData;

        private DepthFinder depthCheck;

        private int numOfEdges;

        private Bitmap afterCannyImage, originalImage; //For testing

        # region properties

        public List<Sine> Sines { get; } = new List<Sine>();

        public double EdgeJoinBonus { get; set; } = 0;

        public double EdgeLengthBonus { get; set; } = 0;

        public int PeakThreshold { get; set; } = 100;

        public int MaxSineAmplitude { get; set; } = 20;

        public bool Testing { get; set; }

        #endregion properties

        public HoughTransform(bool[] edgeData, int imageWidth, int imageHeight)
        {
            this.m_EdgeData = edgeData;
            this.m_ImageWidth = imageWidth;
            this.m_ImageHeight = imageHeight;

            BuildEdgePointsList();
        }

        private void BuildEdgePointsList()
        {
            var edgePointsFinder = new EdgePointsFinder(m_EdgeData, 
                                                        m_ImageWidth, 
                                                        m_ImageHeight);

            edgePointsFinder.RunFinder();

            m_EdgePointData = edgePointsFinder.EdgePointData;
            numOfEdges = edgePointsFinder.NumOfEdges;
        }

        public void RunHoughTransform()
        {
            var stage = 0;
            var allPeaksFound = false;

            const int sizeToCheckEachWay = 2; //For testing

            RunDepthChecker();

            while (!allPeaksFound)
            {
                var depthOfSine = depthCheck.GetTopPeak(PeakThreshold);

                if (depthOfSine < m_ImageHeight)
                {
                    if (Testing)
                        DrawAccumulatorSpaceToImage(stage, sizeToCheckEachWay);

                    CalculateSinusoidParameters calculateParameters = new CalculateSinusoidParameters(depthOfSine, MaxSineAmplitude, m_EdgePointData, m_ImageWidth, m_ImageHeight);
                    calculateParameters.Testing = Testing;
                    calculateParameters.Run();

                    Sine currentSine = calculateParameters.Sine;

                    Sines.Add(currentSine);

                    if (Testing)
                        DrawSinesToImage(stage);

                    RemoveSine(currentSine);

                    if (Testing)
                        DrawEdgePointsImage(stage);

                    stage++;
                }
                else
                    allPeaksFound = true;
            }

            if (Testing)
                DrawAccumulatorSpaceToImage(stage, sizeToCheckEachWay);
            //CheckForDepths();             
        }

        # region Remove Sine methods

        /// <summary>
        /// Removes traces of this sine from edges, votetracker and the depth accumulator space
        /// </summary>
        /// <param name="currentSine"></param>
        private void RemoveSine(Sine currentSine)
        {
            //Get edges on sine 
            List<Point> sinePoints = currentSine.Points;
            List<int> edgeSetsOnSine = FindEdgeSetsOnSine(sinePoints);

            for (int i = 0; i < edgeSetsOnSine.Count; i++)
            {
                int currentEdge = edgeSetsOnSine[i];

                RemoveVotesFromAccumulatorSpace(currentEdge);
            }
        }

        private List<int> FindEdgeSetsOnSine(List<Point> sinePoints)
        {
            List<int> edgeSetsOnSine = new List<int>();

            for (int i = 0; i < sinePoints.Count; i++)
            {
                Point currentPoint = sinePoints[i];
                int xPoint = currentPoint.X;
                int yPoint = currentPoint.Y;

                //if (edgePoints.Contains(new EdgePoint(currentPoint.X, currentPoint.Y)))
                if (m_EdgePointData.IsEdgePointAt(xPoint, yPoint))
                {
                    //EdgePoint foundPoint = edgePoints.Find(point => point.GetXPos() == currentPoint.X && point.GetYPos() == currentPoint.Y);

                    //int edgeNumber = foundPoint.GetEdgeSet();
                    int edgeNumber = m_EdgePointData.GetEdgeNumAt(xPoint, yPoint);

                    if (!edgeSetsOnSine.Contains(edgeNumber))
                    {
                        edgeSetsOnSine.Add(edgeNumber);
                    }
                }
            }

            return edgeSetsOnSine;
        }

        /// <summary>
        /// Removes votes cast from this edge from the accumulator space
        /// </summary>
        /// <param name="currentEdge"></param>
        private void RemoveVotesFromAccumulatorSpace(int currentEdge)
        {
            depthCheck.RemoveVotesFromAccumulatorSpace(currentEdge);
        }

        # endregion

        private void RunDepthChecker()
        {
            depthCheck = new DepthFinder(m_EdgeData, m_EdgePointData, numOfEdges, m_ImageWidth, m_ImageHeight);

            depthCheck.MaxSineAmplitude = MaxSineAmplitude;

            depthCheck.EdgeJoinBonus = EdgeJoinBonus;
            depthCheck.EdgeLengthBonus = EdgeLengthBonus;

            depthCheck.Testing = Testing;

            depthCheck.Run();
        }

        # region Test methods

        private void DrawAccumulatorSpaceToImage(int stage, int sizeToCheckEachWay)
        {
            List<Point> points = new List<Point>();

            List<double> accSpace = depthCheck.GetAccumulatorSpace();

            int accSize = accSpace.Count;

            //Check if scaling is needed
            int maxCell = 0;

            for (int i = 0; i < accSize; i++)
            {
                double total = 0;

                total += accSpace[i];

                for (int check = 1; check <= sizeToCheckEachWay; check++)
                {
                    //Check before
                    if (i - check >= 0)
                        total += accSpace[i - check];

                    //Check after
                    if (i + check < accSpace.Count)
                        total += accSpace[i + check];
                }

                if (total > maxCell)
                    maxCell = Convert.ToInt32(total);
            }

            double scaleFactor = 1.0;

            if (maxCell >= m_ImageWidth)
            {
                scaleFactor = (double)m_ImageWidth / (double)maxCell;
                //scaleFactor += 0.1;
            }

            for (int i = 0; i < accSize; i++)
            {
                double total = 0;

                total += accSpace[i];

                for (int check = 1; check <= sizeToCheckEachWay; check++)
                {
                    //Check before
                    if (i - check >= 0)
                        total += accSpace[i - check];

                    //Check after
                    if (i + check < accSpace.Count)
                        total += accSpace[i + check];
                }

                int peakHeight = (int)(total * scaleFactor);

                for (int j = 0; j < peakHeight; j++)
                {
                    points.Add(new Point(j, i));
                }
            }

            DrawEdgesImageFactory factory = new DrawEdgesImageFactory("Point");
            DrawEdgesImage drawImage = factory.setUpDrawEdges(points, m_ImageWidth, m_ImageHeight);

            drawImage.setBackgroundColour(Color.White);
            drawImage.setEdgeColour(Color.LawnGreen);
            drawImage.SetDrawMultiColouredEdges(false);

            drawImage.setEdgeColour(Color.Red);
            //drawImage.drawEdgesOverBackgroundImage(afterCannyImage);
            drawImage.drawEdgesOverBackgroundImage(originalImage);

            drawImage.getDrawnEdges().Save("AccumulatorSpace- " + stage.ToString() + ".bmp");
        }

        private void DrawEdgePointsImage(int stage)
        {
            List<Point> points = new List<Point>();

            int numOfEdges = m_EdgePointData.NumberOfEdges;

            for (int i = 0; i < numOfEdges; i++)
            {
                List<Point> pointsInEdge = m_EdgePointData.GetPointsAtNum(i);

                //Maybe add check of edge bounds to speed up EdgePointData can calculate min and max values of each edge set on creation 
                int numOfPoints = pointsInEdge.Count;

                for (int j = 0; j < numOfPoints; j++)
                {
                    points.Add(pointsInEdge[j]);
                }
            }

            DrawEdgesImageFactory factory = new DrawEdgesImageFactory("Point");
            DrawEdgesImage drawImage = factory.setUpDrawEdges(points, m_ImageWidth, m_ImageHeight);

            drawImage.setBackgroundColour(Color.Black);
            drawImage.setEdgeColour(Color.White);
            drawImage.SetDrawMultiColouredEdges(false);

            drawImage.setEdgeColour(Color.White);
            //drawImage.drawEdgesOverBackgroundImage(afterCannyImage);
            drawImage.drawEdgesImage();
            drawImage.getDrawnEdges().Save("EdgePoints- " + stage.ToString() + ".bmp");
        }

        /*public void SetCannyTestImage(Bitmap afterCannyImage)
        {
            this.afterCannyImage = afterCannyImage;
        }

        private void DrawPeaksToImage()
        {
            List<Point> points = new List<Point>();

            for (int i = 0; i < depthPeaks.Count; i++)
            {
                int depth = depthPeaks[i];

                for (int j = 0; j < imageWidth; j++)
                {
                    points.Add(new Point(j, depth));
                }
            }

            DrawEdgesImageFactory factory = new DrawEdgesImageFactory("Point");
            DrawEdgesImage drawImage = factory.setUpDrawEdges(points, imageWidth, imageHeight);

            drawImage.setBackgroundColour(Color.White);
            drawImage.setEdgeColour(Color.Red);
            drawImage.SetDrawMultiColouredEdges(false);

            drawImage.setEdgeColour(Color.Red);
            drawImage.drawEdgesOverBackgroundImage(afterCannyImage);
            drawImage.getDrawnEdges().Save("Found peaks.bmp");
        }*/

        public void SetOriginalTestImage(Bitmap originalImage)
        {
            this.originalImage = originalImage;
        }

        private void DrawSinesToImage(int stage)
        {
            //DrawSinesImage drawSinesOverCanny = new DrawSinesImage(afterCannyImage, sines);
            //drawSinesOverCanny.DrawnImage.Save("Sines over Canny - " + stage + ".bmp");

            DrawSinesImage drawSinesOverOriginal = new DrawSinesImage(originalImage, Sines);
            drawSinesOverOriginal.DrawnImage.Save("Sines over original - " + stage + ".bmp");
        }

        private void writeSpace()
        {
            List<double> depthSpace = depthCheck.GetAccumulatorSpace();

            //for (int i = 0; i < depthSpace.Count; i++)
            //{
            //    Console.WriteLine(
            //}

            Microsoft.Office.Interop.Excel.Application app = null;
            Microsoft.Office.Interop.Excel.Workbook workbook = null;
            Microsoft.Office.Interop.Excel.Worksheet depthWorksheet = null;

            app = new Microsoft.Office.Interop.Excel.Application();

            workbook = app.Workbooks.Add(1);

            depthWorksheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Sheets[1];

            depthWorksheet.Cells[1, 2] = "Test";

            for (int i = 0; i < depthSpace.Count; i++)
            {
                depthWorksheet.Cells[1, i + 1] = depthSpace[i];
            }

            workbook.SaveAs("depth.xlsx");
            workbook.Close();
        }

        #endregion
    }
}
