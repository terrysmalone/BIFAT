using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Edges
{
    public class CalculateIntersectingPoint
    {
        private Point pointOfIntersection;
        private bool noIntersectingPoint;

        public CalculateIntersectingPoint(double line1Slope, double line1Intercept, double line2Slope, double line2Intercept)
        {
            double xPoint, yPoint;
            noIntersectingPoint = false;

            // Line1:
            // y = [line1Slope]x + line1Intercept
            // Line2:
            // y = [line2Slope]x + line2Intercept
            //
            // so,
            // [line1Slope]x + line1Intercept = [line2Slope]x + line2Intercept
            //
            // 1. Remove X from one side:
            //      line1Intercept = ([line2Slope-line1Slope]x) + line2Intercept
            // 2. Remove intercept from other side
            //      line1Intercept-line2Intercept = ([line2Slope-line1Slope]x)
            // 3. divide by slope 
            //      (line1Intercept-line2Intercept) / [line2Slope-line1Slope] = x  

            if (line1Slope == line2Slope)
                noIntersectingPoint = true;
            else
            {
                xPoint = (line1Intercept - line2Intercept) / (line2Slope - line1Slope);

                yPoint = (line1Slope * xPoint) + line1Intercept;

                pointOfIntersection = new Point(Convert.ToInt32(xPoint), Convert.ToInt32(yPoint));

                noIntersectingPoint = true;
            }
        }

        public Point GetIntersectingPoint()
        {
            return pointOfIntersection;
        }

        public bool GetNoIntersectingPoint()
        {
            return noIntersectingPoint;
        }
    }
}
