using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Edges;
using System.Drawing;

namespace EdgeFitting
{
    /// <summary>
    /// A class which, given a series of edge points attempts to find a best fit sine
    /// 
    /// Author Terry Malone (trm8@aber.ac.uk)
    /// Version 1.1
    /// </summary>
    public class BestFitSine
    {
        private Edge edge;
        private Sine currentSine;
        private List<Point> points = new List<Point>();

        private int imageWidth;
        private double imageHeight, maxAmplitude;
        private double error, totalError;

        private int edgeQuality;
        private double lowestError;

        /// <summary>
        /// Constructor method
        /// </summary>
        /// <param name="edge">The edge to fit a sine to</param>
        /// <param name="imageWidth">The width of the source image</param>
        /// <param name="imageHeight">The height of the source image</param>
        public BestFitSine(Edge edge, int imageWidth, int imageHeight)
        {
            this.edge = edge;
            this.imageWidth = imageWidth;
            this.imageHeight = imageHeight;

            edge.CalculateEndPoints();

            points = edge.Points;

            CalculateEdgeQuality();

            maxAmplitude = 30;
        }

        /// <summary>
        /// Calculates edge quality based on the number of points in comparison with the width of the image
        /// </summary>
        private void CalculateEdgeQuality()
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
        /// Finds the best fit sine to the List of edge points
        /// </summary>
        public void FindBestFit()
        {
            //CalculateParameters();  //Accurate but slow

            //fastCalculateParameters();  //Fast but less accurate

            CalculateParametersUsingALGLIB();

            //CalculateParametersUsingLeastSquareFitting();
        }

        private void CalculateParameters()
        {
            lowestError = 1000000000;
            int lowestAmp = 0, lowestAz = 0, lowestDepth = 0;

            double azimuthStep = imageWidth / 360;
            double frequency = Math.PI * 2.0;

            double startDepth = calculateStartDepth();
            double endDepth = calculateEndDepth();

            for (double depth = startDepth; depth < endDepth; depth++)
            {
                for (double azimuth = 0; azimuth < 360; azimuth++)
                {
                    //double azimuthDisplacement = (imageWidth * 0.25) - (azimuth * (imageWidth / 360));
                    double azimuthDisplacement = (azimuth * (imageWidth / 360)) - (imageWidth * 0.25);

                    for (double amplitude = 0; amplitude <= maxAmplitude; amplitude++)
                    {
                        totalError = 0;

                        for (int i = 10; i < points.Count - 10; i += (points.Count - 20) / 10)
                        {
                            int xPoint = (int)points[i].X;
                            int yPoint = (int)points[i].Y;

                            //error = (depth + (Math.Sin((xPoint + azimuthDisplacement) * ((frequency) / (Double)imageWidth)) * amplitude)) - yPoint;
                            error = (depth + (Math.Sin((xPoint - azimuthDisplacement) * ((frequency) / (Double)imageWidth)) * amplitude)) - yPoint;

                            if (error < 0)
                                error = 0 - error;

                            totalError += error;
                        }

                        if (totalError < lowestError)
                        {

                            lowestError = Math.Max(totalError, 0) - Math.Min(totalError, 0);

                            lowestDepth = (int)depth;
                            lowestAmp = (int)amplitude;
                            lowestAz = (int)azimuth;
                        }
                    }
                }
            }

            currentSine = new Sine(lowestDepth, lowestAz, lowestAmp, imageWidth, edgeQuality);
        }

        private void fastCalculateParameters()
        {
            lowestError = 1000000000;
            int lowestAmp = 0, lowestAz = 0, lowestDepth = 0;

            double azimuthStep = imageWidth / 360;
            double frequency = Math.PI * 2.0;

            edge.CalculateEndPoints();

            double startDepth = calculateStartDepth();
            double endDepth = calculateEndDepth();

            //Find nearest to 10
            for (double depth = startDepth; depth < endDepth; depth += 10)
            {
                for (double azimuth = 0; azimuth < 360; azimuth += 10)
                {
                    double azimuthDisplacement = (imageWidth * 0.25) - (azimuth * (imageWidth / 360));

                    for (double amplitude = 0; amplitude <= maxAmplitude; amplitude += 10)
                    {
                        totalError = 0;

                        for (int i = 10; i < points.Count - 10; i += (points.Count - 20) / 10)
                        {
                            int xPoint = (int)points[i].X;
                            int yPoint = (int)points[i].Y;

                            error = (depth + (Math.Sin((xPoint + azimuthDisplacement) * ((frequency) / (Double)imageWidth)) * amplitude)) - yPoint;

                            if (error < 0)
                                error = 0 - error;

                            totalError += error;
                        }

                        if (totalError < lowestError)
                        {

                            lowestError = Math.Max(totalError, 0) - Math.Min(totalError, 0);

                            lowestDepth = (int)depth;
                            lowestAmp = (int)amplitude;
                            lowestAz = (int)azimuth;
                        }
                    }
                }
            }

            lowestError = 1000000000;

            int lowestPossibleDepth = lowestDepth - 5;
            int highestPossibleDepth = lowestDepth + 5;
            int lowestPossibleAmp = lowestAmp - 5;
            int highestPossibleAmp = lowestAmp + 5;
            int lowestPossibleAz = lowestAz - 5;
            int highestPossibleAz = lowestAz + 5;

            if (lowestPossibleDepth < 0)
                lowestPossibleDepth = 0;

            if (highestPossibleDepth >= imageHeight)
                highestPossibleDepth = (int)imageHeight - 1;

            if (lowestPossibleAmp < 0)
                lowestPossibleAmp = 0;

            //Narrow it down
            for (double depth = lowestPossibleDepth; depth <= highestPossibleDepth; depth++)
            {
                for (double az = lowestPossibleAz; az <= highestPossibleAz; az++)
                {
                    double azimuth = az;

                    if (az < 0)
                        azimuth = 360 + lowestPossibleAz;

                    if (az > 359)
                        azimuth = highestPossibleAz - 360;

                    double azimuthDisplacement = (imageWidth * 0.25) - (azimuth * (imageWidth / 360));

                    for (double amplitude = lowestPossibleAmp; amplitude <= highestPossibleAmp; amplitude++)
                    {
                        totalError = 0;

                        for (int i = 10; i < points.Count - 10; i += (points.Count - 20) / 10)
                        {
                            int xPoint = (int)points[i].X;
                            int yPoint = (int)points[i].Y;

                            error = (depth + (Math.Sin((xPoint + azimuthDisplacement) * ((frequency) / (Double)imageWidth)) * amplitude)) - yPoint;

                            if (error < 0)
                                error = 0 - error;

                            totalError += error;
                        }

                        if (totalError < lowestError)
                        {

                            lowestError = Math.Max(totalError, 0) - Math.Min(totalError, 0);

                            lowestDepth = (int)depth;
                            lowestAmp = (int)amplitude;
                            lowestAz = (int)azimuth;
                        }
                    }
                }
            }

            currentSine = new Sine(lowestDepth, lowestAz, lowestAmp, imageWidth, edgeQuality);
        }

        /// <summary>
        /// c[0] = Amplitude
        /// c[1] = shift
        /// c[2] = depth
        /// </summary>
        /// <param name="c"></param>
        /// <param name="x"></param>
        /// <param name="func"></param>
        /// <param name="obj"></param>
        public void function_cx_1_func(double[] c, double[] x, ref double func, object obj)
        {
            double frequency = Math.PI * 2.0;

            func = (c[0] * Math.Sin((x[0] - c[1]) * (frequency / (double)imageWidth))) + c[2];
        }

        /// <summary>
        /// Calculates parameters using nonlinear least square regression.
        /// Calculations are carried out using AlgLib (http://www.alglib.net), an open source numerical analysis package
        /// </summary>
        private void CalculateParametersUsingALGLIB()
        {
            double[,] x = new double[points.Count, 1];
            double[] y = new double[points.Count];

            for (int i = 0; i < points.Count; i++)
            {
                x[i, 0] = points[i].X;
                y[i] = points[i].Y;
            }

            int ampGuess = (edge.HighestYPoint - edge.LowestYPoint) / 2;
            
            int azGuess = edge.AzimuthGuess;

            int displacementGuess = (int)((double)azGuess * (double)(imageWidth / 360) - (double)(imageWidth * 0.25));

            int depthGuess = edge.LowestYPoint + ampGuess;

            double[] c = new double[] { ampGuess, displacementGuess, depthGuess };

            double[] s = new double[] { 1, 1, 1 };
            double[] lowerBound = new double[] { 0, -imageWidth, 0 };
            double[] upperBound = new double[] { 100, imageWidth, imageHeight };

            double epsx = 0.1;
            int maxits = 100;
            int info;

            alglib.lsfitstate state;
            alglib.lsfitreport rep;
            double diffstep = 0.0001;

            //
            // Fitting without weights
            //
            alglib.lsfitcreatef(x, y, c, diffstep, out state);
            alglib.lsfitsetcond(state, epsx, maxits);

            alglib.lsfitsetbc(state, lowerBound, upperBound);
            alglib.lsfitsetscale(state, s);

            alglib.lsfitfit(state, function_cx_1_func, null, null);
            alglib.lsfitresults(state, out info, out c, out rep);
            
            int depth = (int)c[2];
            int amplitude = (int)c[0];
            int azimuth = (int)((((double)imageWidth * 0.25) + c[1]) / ((double)imageWidth / 360.0));
            
            if (azimuth < 0)
                azimuth = azimuth + 359;

            if (azimuth > 359)
                azimuth = azimuth - 359;

            currentSine = new Sine(depth, azimuth, amplitude, imageWidth, edgeQuality);
        }

        /// <summary>
        /// Calculates the search start depth based on the maximum amplitude and position of the lowest edge points
        /// </summary>
        /// <returns>The search start depth</returns>
        private double calculateStartDepth()
        {
            double startDepth;

            if (edge.LowestYPoint - (maxAmplitude) > 0)
                startDepth = edge.LowestYPoint - (maxAmplitude * 2);
            else
                startDepth = 0;

            return startDepth;
        }

        /// <summary>
        /// Calculates the search end depth based on the maximum amplitude and position of the highest edge points
        /// </summary>
        /// <returns>The search end depth</returns>

        private double calculateEndDepth()
        {
            double endDepth;

            if (edge.HighestYPoint + (maxAmplitude) < imageHeight)
                endDepth = edge.HighestYPoint + (maxAmplitude * 2);
            else
                endDepth = imageHeight - 1;

            return endDepth;
        }

        /// <summary>
        /// Sets the maximum amplitude to search
        /// </summary>
        /// <param name="maxAmplitude">The maximum amplitude</param>
        public void setMaxAmplitude(int maxAmplitude)
        {
            this.maxAmplitude = maxAmplitude;
        }

        # region get methods

        /**
        * Method which returns the best fit sine
        *
        * @return currentSine The Sine that best fits the given edge
        */
        public Sine GetSine()
        {
            return currentSine;
        }

        /// <summary>
        /// Returns the lowest error found (Tells how close the best fit was to being 'perfect')
        /// </summary>
        /// <returns>The lowest error</returns>
        public double getLowestError()
        {
            return lowestError;
        }

        # endregion
    }
}
