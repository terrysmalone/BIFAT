using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace EdgeFitting
{
    /// <summary>
    /// A class which defines a sinusoid
    /// 
    /// Author Terry Malone (trm8@aber.ac.uk)
    /// Version 1.1
    /// </summary>
    public class Sine
    {
        private int depth;
        private int azimuth;
        private int amplitude;
        private int sourceAzimuthResolution;
        private int quality;

        private List<Point> sinePoints = new List<Point>();

        private Point orientationBeforePoint, orientationCheckPoint, orientationAfterPoint;
                
        # region Properties

        public List<Point> Points
        {
            get { return sinePoints; }
        }
        public int Amplitude
        {
            get { return amplitude; }
        }

        public int Azimuth
        {
            get { return azimuth; }
        }

        public int Depth
        {
            get { return depth; }
        }

        public int Quality
        {
            get { return quality; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor method
        /// </summary>
        /// <param name="depth">The sines depth</param>
        /// <param name="azimuth">The sines azimuth</param>
        /// <param name="amplitude">The sines amplitude</param>
        /// <param name="sourceAzimuthResolution">The source image resolution</param>
        public Sine(int depth, int azimuth, int amplitude, int sourceAzimuthResolution)
        {
            this.depth = depth;
            this.azimuth = azimuth;
            this.amplitude = amplitude;
            this.sourceAzimuthResolution = sourceAzimuthResolution;

            quality = 1;

            if (azimuth >= 360)
                azimuth = 0;

            calculatePoints();
        }

        /// <summary>
        /// Constructor method
        /// </summary>
        /// <param name="depth">The sines depth</param>
        /// <param name="azimuth">The sines azimuth</param>
        /// <param name="amplitude">The sines amplitude</param>
        /// <param name="sourceAzimuthResolution">The source image resolution</param>
        /// <param name="initialQuality">The quality of the sinusoid</param>
        public Sine(int depth, int azimuth, int amplitude, int sourceAzimuthResolution, int initialQuality)
        {
            this.depth = depth;
            this.azimuth = azimuth;
            this.amplitude = amplitude;
            this.sourceAzimuthResolution = sourceAzimuthResolution;

            quality = initialQuality;

            if (azimuth >= 360)
                azimuth = 0;

            calculatePoints();
        }

        #endregion

        /// <summary>
        /// Calculates all points of the sinusoid
        /// </summary>
        private void calculatePoints()
        {
            sinePoints.Clear();
            int xPoint, yPoint;
            int azimuthDisplacement = (int)(((double)sourceAzimuthResolution * (double)0.25) - ((double)azimuth * ((double)sourceAzimuthResolution / (double)360)));

            double frequency = (Math.PI * 2.0) / sourceAzimuthResolution;

            for (int i = 0; i < sourceAzimuthResolution; i++)
            {
                xPoint = i;

                yPoint = (int)(amplitude * (Math.Sin((xPoint + azimuthDisplacement) * (frequency))) + depth);

                sinePoints.Add(new Point(xPoint, yPoint));
            }
        }

        /// <summary>
        /// Changes the values of the sinusoid and recalculates it's points
        /// </summary>
        /// <param name="depth">The sines depth</param>
        /// <param name="azimuth">The sines azimuth</param>
        /// <param name="amplitude">The sines amplitude</param>
        public void change(int depth, int azimuth, int amplitude)
        {
            this.depth = depth;
            this.azimuth = azimuth;
            this.amplitude = amplitude;

            calculatePoints();
        }

        /// <summary>
        /// Calculates the given positions left and right Point based on whether it is at an edge
        /// </summary>
        /// <param name="position">The position of the point to check</param>
        private void calculatePointsPosition(int position)
        {
            if (position == 0)                                         //If point is at left end of edge
            {
                pointIsAtLeftEdge();
            }
            else if (position == sourceAzimuthResolution - 1)        //If point is at right edge
            {
                pointIsAtRightEdge();
            }
            else
            {
                pointIsAtPosition(position);
            }
        }

        /// <summary>
        /// Adds the points left and right position 
        /// </summary>
        private void pointIsAtLeftEdge()
        {
            orientationBeforePoint = sinePoints[sourceAzimuthResolution - 1];
            orientationCheckPoint = sinePoints[0];
            orientationAfterPoint = sinePoints[1];
        }

        /// <summary>
        /// Adds the points left and right position 
        /// </summary>
        private void pointIsAtRightEdge()
        {
            orientationBeforePoint = sinePoints[sourceAzimuthResolution - 2];
            orientationCheckPoint = sinePoints[sourceAzimuthResolution - 1];
            orientationAfterPoint = sinePoints[0];
        }

        /// <summary>
        /// Adds the points left and right position based on the given position
        /// </summary>
        /// <param name="position"></param>
        private void pointIsAtPosition(int position)
        {
            orientationBeforePoint = sinePoints[position - 1];
            orientationCheckPoint = sinePoints[position];
            orientationAfterPoint = sinePoints[position + 1];
        }

        # region get methods

        /// <summary>
        /// Gets the Y value of a given point along the sinusoid
        /// </summary>
        /// <param name="x">The x position of the point</param>
        /// <returns></returns>
        public int GetY(int x)
        {
            int y = (int)sinePoints[x].Y;
            return y;
        }

        /// <summary>
        /// Returns the Point at a given position in the List of sine points
        /// </summary>
        /// <param name="position">The position along the sinusoid</param>
        /// <returns>The sine point</returns>
        public Point GetPoint(int position)
        {
            return sinePoints[position];
        }

        /// <summary>
        /// Returns the orientation of the point at the given position
        /// 1:       2:                3:       4:                5:       6:                7:       8:
        /// |-|-|-|  |X|-|-|  |-|-|-|  |X|-|-|  |X|-|-|  |-|X|-|  |-|X|-|  |-|-|X|  |-|X|-|  |-|-|X|  |-|-|X|  |-|-|-|
        /// |X|X|X|  |-|X|X|  |X|X|-|  |-|X|-|  |-|X|-|  |-|X|-|  |-|X|-|  |-|X|-|  |-|X|-|  |-|X|-|  |X|X|-|  |-|X|X|
        /// |-|-|-|  |-|-|-|  |-|-|X|  |-|-|X|  |-|X|-|  |-|-|X|  |-|X|-|  |-|X|-|  |X|-|-|  |X|-|-|  |-|-|-|  |X|-|-|
        /// </summary>
        /// <param name="position">The position of the point to check</param>
        /// <returns>The orientation of the point</returns>
        public int GetPointOrientation(int position)
        {
            int pointOrientation = 0;

            calculatePointsPosition(position);

            PointOrientation orientation = new PointOrientation(orientationBeforePoint, orientationCheckPoint, orientationAfterPoint, sourceAzimuthResolution);

            pointOrientation = orientation.Orientation;

            return pointOrientation;
        }

        # endregion
    }
}
