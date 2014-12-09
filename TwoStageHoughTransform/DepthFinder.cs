using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwoStageHoughTransform.AccumulatorSpace;

namespace TwoStageHoughTransform
{
    /// <summary>
    /// Class which calculate the likely depths of fixed period sinusoids in an image
    /// </summary>
    public class DepthFinder
    {
        private int imageHeight, imageWidth;
        private EdgePointData edgePointData;
        private bool[] imageData;

        private VoteTracker voteTracker;
        private int numOfEdges;

        private AccumulatorSpace1D accumulatorSpace;

        private int scanLineXValue;
        private int accumulatorValue;

        private int maxSineAmplitude = 20;

        private double edgeJoinBonus = 1;  //1 makes bonus ineffective
        private double edgeLengthBonus = 1;    //o makes bonus ineffective

        private bool testing = false;

        #region Properties
        public double EdgeJoinBonus
        {
            get
            {
                return edgeJoinBonus;
            }
            set
            {
                edgeJoinBonus = value;
            }
        }

        public double EdgeLengthBonus
        {
            get
            {
                return edgeLengthBonus;
            }
            set
            {
                edgeLengthBonus = value;
            }
        }

        public int MaxSineAmplitude
        {
            get
            {
                return maxSineAmplitude;
            }
            set
            {
                maxSineAmplitude = value;
            }
        }

        public bool Testing
        {
            get
            {
                return testing;
            }
            set
            {
                testing = value;
            }
        }
        #endregion

        /// <summary>
        /// Constructor method
        /// </summary>
        public DepthFinder(bool[] imageData, EdgePointData edgePointData, int numOfEdges, int imageWidth, int imageHeight)
        {
            this.imageData = imageData;
            this.edgePointData = edgePointData;
            this.numOfEdges = numOfEdges;

            this.imageWidth = imageWidth;
            this.imageHeight = imageHeight;

            accumulatorSpace = new AccumulatorSpace1D(imageHeight);

            voteTracker = new VoteTracker(numOfEdges, imageHeight);
        }

        public void Run()
        {
            CheckEdgePoints();
        }

        private void CheckEdgePoints()
        {
            int scanWidth = imageWidth / 2;

            //Check the left half of the image
            for (int xValue = 0; xValue < scanWidth; xValue++)
            {
                for (int yValue = 0; yValue < imageHeight; yValue++)
                {
                    //If there is an edge point check for corresponding points
                    if (imageData[xValue + (yValue * imageWidth)])
                    {
                        //EdgePoint edgePoint1 = edgePoints.Find(f => f.GetPos().Equals(new Point(xValue, yValue)));

                        //int point1Num = edgePoint1.GetEdgeSet();
                        //int point1Length = edgePoint1.EdgeLength;
                        int point1Num = edgePointData.GetEdgeNumAt(xValue, yValue);
                        int point1Length = edgePointData.GetEdgeLengthAt(xValue, yValue);

                        scanLineXValue = xValue + (imageWidth / 2);

                        //How far to check based on the maximum amplitude
                        int start = yValue - maxSineAmplitude;
                        int finish = yValue + maxSineAmplitude;

                        if (start < 0)
                            start = 0;

                        if (finish > imageHeight)
                            finish = imageHeight;

                        for (int verticalScanLine = start; verticalScanLine < finish; verticalScanLine++)
                        {
                            if (imageData[scanLineXValue + (imageWidth * verticalScanLine)])
                            {
                                accumulatorValue = Convert.ToInt32(Math.Round((double)((yValue + verticalScanLine) / 2.0), 0, MidpointRounding.AwayFromZero));

                                double accumulatorAmount = 1;

                                //EdgePoint edgePoint2 = edgePoints.Find(f => f.GetPos().Equals(new Point(scanLineXValue, verticalScanLine)));
                                //int point2Num = edgePoint2.GetEdgeSet();
                                //int point2Length = edgePoint2.EdgeLength;

                                int point2Num = edgePointData.GetEdgeNumAt(scanLineXValue, verticalScanLine);
                                int point2Length = edgePointData.GetEdgeLengthAt(scanLineXValue, verticalScanLine);


                                //Length bonus

                                double lengthBonus = (0.5 * ((point1Length + point2Length) / imageWidth)) * edgeLengthBonus;

                                accumulatorAmount = accumulatorAmount + lengthBonus;

                                //Join bonus
                                //if (point1Num == point2Num)
                                    accumulatorAmount = accumulatorAmount + edgeJoinBonus;

                                accumulatorSpace.IncrementBy(accumulatorValue, accumulatorAmount);

                                //Update votes tracker
                                voteTracker.AddVotes(point1Num, accumulatorValue, accumulatorAmount);

                                if (point1Num != point2Num)
                                    voteTracker.AddVotes(point2Num, accumulatorValue, accumulatorAmount);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Removes all votes which were cast by a given Edge
        /// </summary>
        /// <param name="currentEdge"></param>
        internal void RemoveVotesFromAccumulatorSpace(int currentEdge)
        {
            double[] votesFromEdge = voteTracker.GetVotesFromEdge(currentEdge);

            for (int i = 0; i < votesFromEdge.Length; i++)
            {
                accumulatorSpace.DecrementBy(i, votesFromEdge[i]);
            }
        }

        #region Get methods

        /*public List<int> GetPeaks(int threshold)
        {
            List<int> peaksFound = accumulatorSpace.GetPeaks(threshold);
            //List<int> peaksFound = accumulatorSpace.GetNeighbourCheckPeaks(threshold);  //For testing (does same as above except how it is displayed visually
            return peaksFound;
        }*/

        public int GetTopPeak(int peakThreshold)
        {
            int topPeak = accumulatorSpace.GetTopPeak(peakThreshold);

            return topPeak;
        }

        public List<double> GetAccumulatorSpace()
        {
            return accumulatorSpace.GetSpace().ToList();
        }

        #endregion
    }
}
