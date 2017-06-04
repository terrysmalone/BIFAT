using System;
using System.Collections.Generic;
using System.Drawing;

namespace ActiveContour
{
    /// <summary>
    /// Calculates the curvature penalty of a curve from its narrow band
    /// </summary>
    public class CurvaturePenalty
    {
        private readonly double[,] _curvaturePenalty;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sdf">The signed distance function values of the entire image</param>
        /// <param name="narrowBand">A list of points that are within the curves narrow band</param>
        public CurvaturePenalty(double[,] sdf, List<Point> narrowBand)
        {
            var width = sdf.GetLength(0);
            var height = sdf.GetLength(1);

            _curvaturePenalty = new double[width, height];

            foreach (Point point in narrowBand)
            {
                var x = point.X;
                var y = point.Y;

                var left = x - 1;

                if (x == 0)
                    left = 0;

                var right = x + 1;

                if (x == width - 1)
                    right = width - 1;

                var up = y - 1;

                if (y == 0)
                    up = 0;

                var down = y + 1;

                if (y == height - 1)
                    down = height - 1;


                //Get central derivatives of SDF at x,y
                var sdfLeft = sdf[left, y];
                var sdfRight = sdf[right, y];
                var sdfUp = sdf[x, up];
                var sdfDown = sdf[x, down];

                var sdfCentral = (2 * sdf[x, y]);

                var sdfX = -sdfLeft + sdfRight;
                var sdfY = -sdfDown + sdfUp;

                var sdfXx = sdfLeft - sdfCentral + sdfRight;
                var sdfYy = sdfDown - sdfCentral + sdfUp;

                var sdfXy = -0.25 * sdf[left, down] - 0.25 * sdf[right, up] + 0.25 
                               * sdf[right, down] + 0.25 * sdf[left, up];

                var sdfX2 = Math.Pow(sdfX, 2);
                var sdfY2 = Math.Pow(sdfY, 2);

                _curvaturePenalty[x, y] = (((sdfX2 * sdfYy) + (sdfY2 * sdfXx) - (2 * sdfX * sdfY * sdfXy)) 
                                            / Math.Pow((sdfX2 + sdfY2 + double.Epsilon), 2)) 
                                          * Math.Pow((sdfX2 + sdfY2), 0.5);
            }
        }

        public double[,] GetCurvaturePenalty()
        {
            return _curvaturePenalty;
        }
    }
}
