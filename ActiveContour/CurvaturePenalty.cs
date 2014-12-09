using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace ActiveContour
{
    /// <summary>
    /// Calculates the curvature penalty of a curve from its narrow band
    /// </summary>
    public class CurvaturePenalty
    {
        private double[,] sdf;
        private List<Point> narrowBand;

        private double[,] curvaturePenalty;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sdf">The signed distance function values of the entire image</param>
        /// <param name="narrowBand">A list of points that are within the curves narrow band</param>
        public CurvaturePenalty(double[,] sdf, List<Point> narrowBand)
        {
            this.sdf = sdf;
            this.narrowBand = narrowBand;

            int width = sdf.GetLength(0);
            int height = sdf.GetLength(1);

            curvaturePenalty = new double[width, height];

            foreach (Point point in narrowBand)
            {
                int x = point.X;
                int y = point.Y;

                int left = x - 1;

                if (x == 0)
                    left = 0;

                int right = x + 1;

                if (x == width - 1)
                    right = width - 1;

                int up = y - 1;

                if (y == 0)
                    up = 0;

                int down = y + 1;

                if (y == height - 1)
                    down = height - 1;


                //Get central derivatives of SDF at x,y
                double sdfLeft = sdf[left, y];
                double sdfRight = sdf[right, y];
                double sdfUp = sdf[x, up];
                double sdfDown = sdf[x, down];

                double sdfCentral = (2 * sdf[x, y]);

                double sdfX = -sdfLeft + sdfRight;
                double sdfY = -sdfDown + sdfUp;

                double sdfXX = sdfLeft - sdfCentral + sdfRight;
                double sdfYY = sdfDown - sdfCentral + sdfUp;

                double sdfXY = -0.25 * sdf[left, down] - 0.25 * sdf[right, up] + +0.25 * sdf[right, down] + 0.25 * sdf[left, up];

                double sdfX2 = Math.Pow(sdfX, 2);
                double sdfY2 = Math.Pow(sdfY, 2);

                curvaturePenalty[x, y] = (((sdfX2 * sdfYY) + (sdfY2 * sdfXX) - (2 * sdfX * sdfY * sdfXY)) / Math.Pow((sdfX2 + sdfY2 + double.Epsilon), 2)) * Math.Pow((sdfX2 + sdfY2), 0.5);
            }
        }

        public double[,] GetCurvaturePenalty()
        {
            return curvaturePenalty;
        }
    }
}
