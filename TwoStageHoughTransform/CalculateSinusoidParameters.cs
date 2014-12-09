using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EdgeFitting;
using TwoStageHoughTransform.AccumulatorSpace;

namespace TwoStageHoughTransform
{
    /// <summary>
    /// Calculates the amplitude and dip direction of a sinusoid from it's 
    /// </summary>
    class CalculateSinusoidParameters
    {
        private Sine sine;

        private int depthOfSine;
        private int maxSineAmplitude;

        private EdgePointData edgePointData;
        private int imageHeight, imageWidth;

        private bool testing = false;

        # region Properties

        public Sine Sine
        {
            get
            {
                return sine;
            }
        }

        public int Depth
        {
            get
            {
                return depthOfSine;
            }
        }

        public int MaxSineAmplitude
        {
            get
            {
                return maxSineAmplitude;
            }
        }


        public EdgePointData EdgePointData
        {
            get
            {
                return edgePointData;
            }
        }

        public int ImageWidth
        {
            get
            {
                return imageWidth;
            }
        }

        public int ImageHeight
        {
            get
            {
                return imageHeight;
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

        public CalculateSinusoidParameters(int depthOfSine, int maxSineAmplitude, EdgePointData edgePointData, int imageWidth, int imageHeight)
        {
            this.depthOfSine = depthOfSine;
            this.maxSineAmplitude = maxSineAmplitude;

            this.edgePointData = edgePointData;
            this.imageWidth = imageWidth;
            this.imageHeight = imageHeight;
        }

        public void Run()
        {
            sine = CalculateSineParameters();
        }

        private Sine CalculateSineParameters()
        {
            Sine sine;
            int amplitude;
            int azimuth;

            double frequency = Math.PI * 2.0;

            AccumulatorSpace2D accumulatorSpace2d = new AccumulatorSpace2D(maxSineAmplitude + 1, 360);

            int numOfEdges = edgePointData.NumberOfEdges;
            
            for (int i = 0; i < numOfEdges; i++)
            {
                List<Point> pointsInEdge = edgePointData.GetPointsAtNum(i);

                //Maybe add check of edge bounds to speed up EdgePointData can calculate min and max values of each edge set on creation 
                int numOfPoints = pointsInEdge.Count;

                for (int j = 0; j < numOfPoints; j++)
                {
                    Point currentPoint = pointsInEdge[j];

                    if (IsEdgePointViable(currentPoint, depthOfSine))
                    {
                        int xPoint = currentPoint.X;
                        int yPoint = currentPoint.Y;

                        for (azimuth = 0; azimuth < 360; azimuth++)
                        {
                            double azimuthDisplacement = (azimuth * (imageWidth / 360)) - (imageWidth * 0.25);

                            double amplitudeDouble;
                            amplitudeDouble = ((double)yPoint - (double)depthOfSine) / (double)(Math.Sin((xPoint - azimuthDisplacement) * ((double)frequency / (Double)imageWidth)));

                            if (amplitudeDouble >= 0 && amplitudeDouble <= maxSineAmplitude)
                            {
                                //amplitude = Convert.ToInt32(amplitudeDouble);
                                amplitude = Convert.ToInt32(Math.Round(amplitudeDouble, MidpointRounding.AwayFromZero));
                                accumulatorSpace2d.Increment(amplitude, azimuth);
                            }
                        }
                    }
                }
            }
            
            accumulatorSpace2d.CalculateMax();

            amplitude = accumulatorSpace2d.Dimension1Max;
            azimuth = accumulatorSpace2d.Dimension2Max;

            sine = new Sine(depthOfSine, azimuth, amplitude, imageWidth);

            return sine;
        }

        /// <summary>
        /// Checks that an edge point is within the allowable distance from a sinusoid depth
        /// </summary>
        /// <param name="edgePointToCheck"></param>
        /// <param name="depthToCheck"></param>
        /// <returns></returns>
        private bool IsEdgePointViable(Point edgePointToCheck, int depthToCheck)
        {
            int pointYPos = edgePointToCheck.Y;

            if (Math.Max(pointYPos, depthToCheck) - Math.Min(pointYPos, depthToCheck) > maxSineAmplitude)
                return false;
            else
                return true;
        }
    }
}