using System;
using System.Drawing;

namespace Edges
{
    /// <summary>
    /// Calculate the area of a triangle given two points and their directions.
    /// Steps:
    /// 1. Calculates the equation of each line
    /// 2. Calculates the point of intersection of the above lines
    /// 3. Calculates the area of the triangle from the 2 given points and the point of intersection
    /// </summary>
    public class TriangleArea
    {
        private readonly Point m_Point1;
        private readonly int m_Point1Direction;
        private readonly Point m_Point2;
        private readonly int m_Point2Direction;

        private Point m_PointOfIntersection;

        private double m_TriangleArea;

        private bool m_TriangleNotPossible;

        public TriangleArea(Point point1, int point1Direction, Point point2, int point2Direction)
        {
            m_Point1 = point1;
            m_Point1Direction = point1Direction;

            m_Point2 = point2;
            m_Point2Direction = point2Direction;

            m_TriangleNotPossible = false;
        }

        public void CalculateArea()
        {
            if (m_Point1Direction != m_Point2Direction 
                && Math.Abs(180 - Math.Abs(m_Point1Direction - m_Point2Direction)) != 0)
            {
                var point3 = CalculateThirdPoint(m_Point1, m_Point1Direction, m_Point2, m_Point2Direction);

                m_TriangleArea = CalculateTriangleArea(m_Point1, m_Point2, point3);

                m_TriangleNotPossible = false;
            }
            else
            {
                m_TriangleNotPossible = true;
            }

            
        }

        private Point CalculateThirdPoint(Point point1, 
                                          int point1Direction, 
                                          Point point2, 
                                          int point2Direction)
        {
            m_PointOfIntersection = new Point(1,1);

            var calculateLine1 = new CalculateLineEquation(point1, point1Direction);

            var line1Slope = calculateLine1.Slope;
            var line1Intercept = calculateLine1.Intercept;

            var calculateLine2 = new CalculateLineEquation(point2, point2Direction);

            var line2Slope = calculateLine2.Slope;
            var line2Intercept = calculateLine2.Intercept;

            var calculateIntersection = new CalculateIntersectingPoint(line1Slope, line1Intercept, line2Slope, line2Intercept);
            
            m_PointOfIntersection = calculateIntersection.GetIntersectingPoint();

            return m_PointOfIntersection;
        }

        /// <summary>
        /// Calculates the area of a triangle from three given points
        /// </summary>
        /// <returns></returns>
        private static double CalculateTriangleArea(Point point1, Point point2, Point point3)
        {
            double val1 = point1.X * (point2.Y - point3.Y);

            double val2 = point2.X * (point3.Y - point1.Y);

            double val3 = point3.X * (point1.Y - point2.Y);

            var area = Math.Abs((val1 + val2 + val3) / 2.0);

            return area;
        }

        public double GetArea()
        {
            return m_TriangleArea;
        }

        public bool GetTriangleNotPossible()
        {
            return m_TriangleNotPossible;
        }

        # region Test method

        public void DrawTriangle(Bitmap imageToDrawOn, string fileName)
        {
            var draw = (Bitmap)imageToDrawOn.Clone();

            var graphics = Graphics.FromImage(draw);

            var pen = new Pen(Color.Red);
            var points = new[] { m_Point1, m_Point2, m_PointOfIntersection, m_Point1 };

            graphics.DrawLines(pen, points);

            draw.Save(fileName);
        }

        # endregion
    }
}
