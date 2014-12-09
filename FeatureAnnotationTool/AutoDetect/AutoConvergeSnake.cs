using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using ActiveContour;

namespace FeatureAnnotationTool.AutoDetect
{
    class AutoConvergeSnake
    {
        private Bitmap section;

        private List<Point> initialPoints;

        private int bitmapXOffset;
        private int bitmapYOffset;

        private Snake snake;

        private int stopCheckDifferenceThreshold = 5;
        private int stopCheckDifferenceCounter = 10;

        private List<Point> curvePoints;
        
        public AutoConvergeSnake(Bitmap section, List<Point> initialPoints, int bitmapXOffset, int bitmapYOffset)
        {
            this.section = section;
            this.initialPoints = initialPoints;
            this.bitmapXOffset = bitmapXOffset;
            this.bitmapYOffset = bitmapYOffset;

            List<Point> correctedPoints = new List<Point>();

            foreach (Point point in initialPoints)
            {
                Point correctedPoint = new Point(point.X - bitmapXOffset, point.Y - bitmapYOffset);
                correctedPoints.Add(correctedPoint);
            }

            snake = new Snake(section, correctedPoints, 0.0);

            snake.SetStopCheckDifferenceThreshold(stopCheckDifferenceThreshold);
            snake.SetStopCheckDifferenceCounter(stopCheckDifferenceCounter);

            curvePoints = new List<Point>();
        }

        public AutoConvergeSnake(Bitmap section, List<Point> initialPoints)
        {
            this.section = section;
            this.initialPoints = initialPoints;

            bitmapXOffset = 0;
            bitmapYOffset = 0;

            snake = new Snake(section, initialPoints, 0.1);

            snake.SetStopCheckDifferenceThreshold(5);
            snake.SetStopCheckDifferenceCounter(10);

            curvePoints = new List<Point>();
        }

        public void Run()
        {
            snake.RunActiveContour();
            
            curvePoints = CalculateWithoutOffset(snake.GetCurvePoints());
        }

        private List<Point> CalculateWithoutOffset(List<Point> curvePointsWithoutOffSet)
        {
            List<Point> curvePointsWithOffset = new List<Point>();

            foreach (Point point in curvePointsWithoutOffSet)
            {
                Point offsetPoint = new Point(point.X + bitmapXOffset, point.Y + bitmapYOffset);
                curvePointsWithOffset.Add(offsetPoint);
            }

            return curvePointsWithOffset;
        }

        # region Get methods

        public List<Point> GetCurvePoints()
        {
            return curvePoints;
        }

        # endregion

        # region Set methods

        public void SetStopCheckDifferenceThreshold(int stopCheckDifferenceThreshold)
        {
            this.stopCheckDifferenceThreshold = stopCheckDifferenceThreshold;
        }

        public void SetStopCheckDifferenceCounter(int stopCheckDifferenceCounter)
        {
            this.stopCheckDifferenceCounter = stopCheckDifferenceCounter;
        }

        # endregion
    }
}
