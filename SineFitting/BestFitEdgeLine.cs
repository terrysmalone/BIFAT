using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Edges;

namespace EdgeFitting
{
    /// <summary>
    /// A class which, given a series of edge points attempts to find a best fit EdgeLine
    /// 
    /// Author Terry Malone (trm8@aber.ac.uk)
    /// Version 1.0
    /// </summary>
    public class BestFitEdgeLine
    {
        private Edge edge;
        private EdgeLine currentEdgeLine;
        private List<Point> points = new List<Point>();

        private int imageWidth;
        private double imageHeight;
        private double error, totalError;

        private int edgeQuality;
        private double lowestError;

        public BestFitEdgeLine(Edge edge, int imageWidth, int imageHeight)
        {
            this.edge = edge;
            this.imageWidth = imageWidth;
            this.imageHeight = imageHeight;

            edge.CalculateEndPoints();

            points = edge.Points;

            calculateEdgeQuality();
        }

        /// <summary>
        /// Calculates edge quality based on the number of points in comparison with the width of the image
        /// </summary>
        private void calculateEdgeQuality()
        {
            if ((float)points.Count / (float)imageWidth >= 0.9)
                edgeQuality = 4;
            else if ((float)points.Count / (float)imageWidth >= 0.7)
                edgeQuality = 3;
            else if ((float)points.Count / (float)imageWidth >= 0.55)
                edgeQuality = 2;
            else
                edgeQuality = 1;
        }

        /// <summary>
        /// Finds the best fit EdgeLine to the List of edge points
        /// </summary>
        public void FindBestFit()
        {
            CalculateParameters(); 
        }

        private void CalculateParameters()
        {
            lowestError = 1000000000;

            int lowestIntercept = 0;
            double lowestSlope = 0;

            double startDepth = CalculateStartDepth();
            double endDepth = CalculateEndDepth();

            for (double intercept = startDepth; intercept < endDepth; intercept++)
            {
                for (double slope = -1; slope < 1; slope += 0.01)
                {
                    totalError = 0;

                    for (int i = 0; i < points.Count; i++)
                    {
                        int xPoint = (int)points[i].X;
                        int yPoint = (int)points[i].Y;

                        error = ((slope * (double)xPoint) + intercept) - yPoint;

                        if (error < 0)
                            error = 0 - error;

                        totalError += error;
                    }

                    if (totalError < lowestError)
                    {

                        lowestError = Math.Max(totalError, 0) - Math.Min(totalError, 0);

                        lowestSlope = slope;
                        lowestIntercept = (int)intercept;            
                    }
                }
            }

            currentEdgeLine = new EdgeLine(lowestSlope, lowestIntercept, imageWidth, edgeQuality);
        }

        /// <summary>
        /// Calculates the search start depth based on the positions of the edge points
        /// </summary>
        /// <returns>The search start depth</returns>
        private double CalculateStartDepth()
        {
            double startDepth;

            if (edge.LowestYPoint - (edge.HighestYPoint-edge.LowestYPoint) > 0)
                startDepth = edge.LowestYPoint - (edge.HighestYPoint-edge.LowestYPoint);
            else
                startDepth = 0;

            return startDepth;
        }

        /// <summary>
        /// Calculates the search end depth based on the positions of the edge points
        /// </summary>
        /// <returns>The search end depth</returns>

        private double CalculateEndDepth()
        {
            double endDepth;

            if (edge.HighestYPoint + (edge.HighestYPoint - edge.LowestYPoint) < imageHeight)
                endDepth = edge.HighestYPoint + (edge.HighestYPoint - edge.LowestYPoint);
            else
                endDepth = imageHeight - 1;

            return endDepth;
        }

        # region Get methods

        public EdgeLine GetEdgeLine()
        {
            return currentEdgeLine;
        }

        public double GetLowestError()
        {
            return lowestError;
        }

        # endregion
    }
}
