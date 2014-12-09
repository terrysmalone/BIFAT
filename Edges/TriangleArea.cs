using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace Edges
{
    /// <summary>
    /// Calculate the are of a triangle given two points and their directions.
    /// Steps:
    /// 1. Calculates the equation of each line
    /// 2. Calculates the point of intersection of the above lines
    /// 3. Calculates the area of the triangle from the 2 given points and the point of intersection
    /// </summary>
    public class TriangleArea
    {
        private Point point1;
        private int point1Direction;
        private Point point2;
        private int point2Direction;

        private Point pointOfIntersection;

        private double triangleArea;

        private bool triangleNotPossible;

        public TriangleArea(Point point1, int point1Direction, Point point2, int point2Direction)
        {
            this.point1 = point1;
            this.point1Direction = point1Direction;

            this.point2 = point2;
            this.point2Direction = point2Direction;

            triangleNotPossible = false;
        }

        public void CalculateArea()
        {
            if (point1Direction != point2Direction && Math.Abs(180 - Math.Abs(point1Direction - point2Direction)) != 0)
            {
                Point point3 = CalculateThirdPoint(point1, point1Direction, point2, point2Direction);

                triangleArea = CalculateTriangleArea(point1, point2, point3);

                triangleNotPossible = false;

                //MessageBox.Show("Point1: " + point1 +
                //    "\nPoint2: " + point2 +
                //    "\nIntersectingPoint: " + point3);
            }
            else
            {
                triangleNotPossible = true;
                //MessageBox.Show("triangleNotPossible");
            }

            
        }

        private Point CalculateThirdPoint(Point point1, int point1Direction, Point point2, int point2Direction)
        {
            pointOfIntersection = new Point(1,1);

            double line1Slope = 0;
            double line2Slope = 0;
            double line1Intercept = 0;
            double line2Intercept = 0;

            CalculateLineEquation calculateLine1 = new CalculateLineEquation(point1, point1Direction);
            line1Slope = calculateLine1.Slope;
            line1Intercept = calculateLine1.Intercept;

            CalculateLineEquation calculateLine2 = new CalculateLineEquation(point2, point2Direction);
            line2Slope = calculateLine2.Slope;
            line2Intercept = calculateLine2.Intercept;

            //MessageBox.Show("Line1Slope: " + line1Slope);
            //MessageBox.Show("Line2Slope: " + line2Slope);

            //MessageBox.Show("line1Intercept: " + line1Intercept);
            //MessageBox.Show("line2Intercept: " + line2Intercept);

            CalculateIntersectingPoint calculateIntersection = new CalculateIntersectingPoint(line1Slope, line1Intercept, line2Slope, line2Intercept);
            
            pointOfIntersection = calculateIntersection.GetIntersectingPoint();
            //MessageBox.Show("pointOfIntersection: " + pointOfIntersection);
            return pointOfIntersection;
        }

        /// <summary>
        /// Calculates the area of a triangle from three given points
        /// </summary>
        /// <returns></returns>
        private double CalculateTriangleArea(Point point1, Point point2, Point point3)
        {
            double area = 0;

            double val1, val2, val3;

            val1 = point1.X * (point2.Y - point3.Y);
            val2 = point2.X * (point3.Y - point1.Y);
            val3 = point3.X * (point1.Y - point2.Y);

            area = Math.Abs((val1 + val2 + val3) / 2.0);

            return area;
        }

        public double GetArea()
        {
            return triangleArea;
        }

        public bool GetTriangleNotPossible()
        {
            return triangleNotPossible;
        }

        # region Test method

        public void DrawTriangle(Bitmap imageToDrawOn, string fileName)
        {
            //MessageBox.Show("Point1: " + point1 + "\nPoint 2: " + point2 + "\nIntersecting point: " + pointOfIntersection);

            Bitmap draw = (Bitmap)imageToDrawOn.Clone();

            Graphics graphics = Graphics.FromImage(draw);

            Pen pen = new Pen(Color.Red);
            Point[] points = new Point[] { point1, point2, pointOfIntersection, point1 };

            graphics.DrawLines(pen, points);

            draw.Save(fileName);
        }

        # endregion
    }
}
