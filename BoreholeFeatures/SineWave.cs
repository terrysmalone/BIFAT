using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace BoreholeFeatures
{
    /// <summary>
    /// A class which calculates the points of a sine wave
    /// 
    /// Author - Terry Malone (trm8@aber.ac.uk)
    /// version 1.1 - Refactored
    /// </summary>
    public class SineWave
    {
        private int depth;
        private int azimuth;
        private int amplitude;
        private int sourceAzimuthResolution;

        List<Point> sinePoints = new List<Point>();

        /// <summary>
        /// Constructor method
        /// </summary>
        /// <param name="depth">The depth of the centre point of the sine wave</param>
        /// <param name="azimuth">Where the lowest point of the sine wave falls horizontally in degrees</param>
        /// <param name="amplitude">The distance vertically from the depth point to the azimuth point</param>
        /// <param name="sourceAzimuthResolution">sourceAzimuthResolution The resolution (number of pixels) in the x-axis</param>
        public SineWave(int depth, int azimuth, int amplitude, int sourceAzimuthResolution)
        {
            this.depth = depth;
            this.azimuth = azimuth;
            this.amplitude = amplitude;
            this.sourceAzimuthResolution = sourceAzimuthResolution;

            calculatePoints();
        }

        /// <summary>
        /// Method which calculates all the points of the sine wave
        /// </summary>
        public void calculatePoints()
        {
            sinePoints.Clear();
            int xPoint, yPoint;
            double azimuthDisplacement = ((double)sourceAzimuthResolution * 0.25) - ((double)azimuth * ((double)sourceAzimuthResolution / 360.0));

            double frequency = (Math.PI * 2.0) / (double)sourceAzimuthResolution;

            for (int i = 0; i < sourceAzimuthResolution; i++)
            {
                xPoint = i;

                yPoint = (int)((double)depth + ((double)Math.Sin(((double)xPoint + (double)azimuthDisplacement) * ((double)frequency)) * (double)amplitude));

                sinePoints.Add(new Point(xPoint, yPoint));
            }
        }

        /// <summary>
        /// Method which changes the attributes of the SIneWave and recalculates its points
        /// </summary>
        /// <param name="depth">The depth of the centre point of the sine wave</param>
        /// <param name="azimuth">Where the lowest point of the sine wave falls horizontally in degrees</param>
        /// <param name="amplitude">The distance vertically from the depth point to the azimuth point</param>
        public void change(int depth, int azimuth, int amplitude)
        {
            this.depth = depth;
            this.azimuth = azimuth;
            this.amplitude = amplitude;

            calculatePoints();
        }

        # region Get methods

        /// <summary>
        /// Returns the Y value at a given x point
        /// </summary>
        /// <param name="x">The x value of which the corresponding y value is wanted</param>
        /// <returns>The y value at the given x position</returns>
        public int getY(int x)
        {
            int y = (int)sinePoints[x].Y;
            return y;
        }

        /// <summary>
        /// Returns an individual point on the sine wave
        /// </summary>
        /// <param name="point">The point to be returned</param>
        /// <returns>The Point at the given position</returns>
        public Point getSinepoint(int point)
        {
            return sinePoints[point];
        }

        /// <summary>
        /// Method which returns all of the points on the sine wave
        /// </summary>
        /// <returns>The array of points representing the sine wave</returns>
        public List<Point> getSinePoints()
        {
            return sinePoints;
        }

        /// <summary>
        /// Method which returns the amplitude of the sine wave
        /// </summary>
        /// <returns>The amplitude of the sine wave</returns>
        public int getAmplitude()
        {
            return amplitude;
        }

        /// <summary>
        /// Method which returns the azimuth of the sine wave
        /// </summary>
        /// <returns>The azimuth of the sine wave</returns>
        public int getAzimuth()
        {
            return azimuth;
        }

        /// <summary>
        /// Method which returns the depth of the sine wave
        /// </summary>
        /// <returns>The depth of the sine wave</returns>
        public int getDepth()
        {
            return depth;
        }

        # endregion
    }

}
