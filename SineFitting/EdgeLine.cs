using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace EdgeFitting
{
    public class EdgeLine
    {
        private double slope;
        private int intercept;
        private int sourceAzimuthResolution;

        private int quality = 1;

        private List<Point> linePoints = new List<Point>();

        #region properties

        /// <summary>
        /// The List of all sine points 
        /// </summary>
        public List<Point> Points
        {
            get { return linePoints; }
        }

        /// <summary>
        /// The slope of the line
        /// </summary>
        public double Slope
        {
            get { return slope; }
        }

        /// <summary>
        /// The intercept of the line
        /// </summary>
        public int Intercept
        {
            get { return intercept; }
        }

        /// <summary>
        /// The quality of the sinusoid
        /// </summary>
        public int Quality
        {
            get { return quality; }
        }

        #endregion properties

        #region constructor

        public EdgeLine(double slope, int intercept, int sourceAzimuthResolution)
        {
            this.slope = slope;
            this.intercept = intercept;
            this.sourceAzimuthResolution = sourceAzimuthResolution;

            CalculatePoints();
        }

        public EdgeLine(double slope, int intercept, int sourceAzimuthResolution, int initialQuality)
        {
            this.quality = initialQuality;

            this.slope = slope;
            this.intercept = intercept;
            this.sourceAzimuthResolution = sourceAzimuthResolution;

            CalculatePoints();
        }

        #endregion constructor

        private void CalculatePoints()
        {
            linePoints.Clear();
            int xPoint, yPoint;

            for (int i = 0; i < sourceAzimuthResolution; i++)
            {
                xPoint = i;
                yPoint = (int)Math.Round(intercept + ((double)slope * (double)xPoint));

                linePoints.Add(new Point(xPoint, yPoint));
            }
        }
        /// <summary>
        /// Changes the values of the line and recalculates it's points
        /// </summary>
        /// <param name="depth">The sines depth</param>
        /// <param name="azimuth">The sines azimuth</param>
        /// <param name="amplitude">The sines amplitude</param>
        public void Change(double slope, int intercept)
        {
            this.slope = slope;
            this.intercept = intercept;

            CalculatePoints();
        }

        # region Get methods

        /// <summary>
        /// Gets the Y value of a given point along the line
        /// </summary>
        /// <param name="x">The x position of the point</param>
        /// <returns></returns>
        public int GetY(int x)
        {
            int y = (int)linePoints[x].Y;
            return y;
        }

        /// <summary>
        /// Returns the Point at a given position in the List of sine points
        /// </summary>
        /// <param name="position">The position along the sinusoid</param>
        /// <returns>The sine point</returns>
        public Point GetPoint(int position)
        {
            return linePoints[position];
        }
        
        # endregion
    }
}
