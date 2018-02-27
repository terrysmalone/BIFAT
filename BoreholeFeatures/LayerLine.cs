using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace BoreholeFeatures
{
    /// <summary>
    /// A class which calculates the points in a layer line
    /// 
    /// Author - Terry Malone (trm8@aber.ac.uk)
    /// version 1.0
    /// </summary>
    internal class LayerLine
    {
        private int sourceAzimuthResolution;

#region properties

        public List<Point> Points { get; } = new List<Point>();

        public int Intercept { get; private set; }

        public double Slope { get; private set; }

#endregion properties

        #region constructor

        /// <summary>
        /// Constructor method to create a LayerLine from two points along the line
        /// </summary>
        /// <param name="point1">The first point</param>
        /// <param name="point2">The second point</param>
        public LayerLine(Point point1, Point point2, int sourceAzimuthResolution)
        {
            this.sourceAzimuthResolution = sourceAzimuthResolution;

            Slope = (double)(point1.Y - point2.Y) / (double)(point1.X - point2.X);

            //intercept = y - (slope*x)
            double interceptDouble = point1.Y - (Slope * point1.X);

            Intercept = (int)Math.Round(interceptDouble, MidpointRounding.AwayFromZero);

            CalculatePoints();
        }

        /// <summary>
        /// Constructor method to create a LayerLine from a given slope and intercept value
        /// </summary>
        /// <param name="intercept">The lines intercept</param>
        /// <param name="slope">The lines slope</param>
        public LayerLine(double slope, int intercept, int sourceAzimuthResolution)
        {
            this.sourceAzimuthResolution = sourceAzimuthResolution;
            this.Intercept = intercept;
            this.Slope = slope;

            CalculatePoints();
        }

        #endregion constructor

        /// <summary>
        /// Method which calculates all the points of the line
        /// </summary>
        public void CalculatePoints()
        {
            Points.Clear();
            int xPoint, yPoint; 

            for (int i = 0; i < sourceAzimuthResolution; i++)
            {
                xPoint = i;
                yPoint = (int)Math.Round(Intercept + ((double)Slope * (double)xPoint));

                Points.Add(new Point(xPoint, yPoint));
            }
        } 
        
        # region Get methods

        /// <summary>
        /// Returns the y co-ordinate of the given x-point in the line
        /// </summary>
        /// <param name="atX"></param>
        /// <returns></returns>
        public int GetY(int atX)
        {
            int y = (int)Points[atX].Y;
            return y;
        }

        # endregion

        public void Change(double slope, int intercept)
        {
            this.Slope = slope;
            this.Intercept = intercept;

            CalculatePoints();
        }
    }
}
