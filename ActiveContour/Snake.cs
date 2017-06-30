using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace ActiveContour
{
    /// <summary>
    /// Implements an active contour based on Chan and Vese - Active Contours Without Edges
    /// </summary>
    public class Snake
    {
        private const bool TimeTest = false;
        private const int DrawingTestCounter = 10;
        private const bool DrawingTest = false;

        private int m_NumOfIterationsWithoutChange;

        private DateTime m_StartTime;
        private DateTime m_EndTime;
        private DateTime m_TotalStart;
        private DateTime m_TotalEnd;
        private DateTime m_CheckStart;
        private DateTime m_CheckEnd;

        private readonly Bitmap m_OriginalImage;
        private readonly int m_ImageHeight;
        private readonly int m_ImageWidth;

        private int m_Counter;

        private readonly double m_AlphaValue;

        private double m_NarrowBandSize = 1.2;

        private double[,] m_Sdf;
        private double[,] m_SdfOld;
        private List<Point> m_NarrowBand;
        private double[,] m_ImageForce;
        private double[,] m_CurvaturePenalty;
        private double[,] m_GradientDescentValues;

        private bool[,] m_FinalCurve;

        private double m_ExteriorMean;
        private double m_InteriorMean;

        private bool m_SolutionFound;

        private double m_MaxGradientDescentValue;

        private List<Point> m_CurvePoints = new List<Point>();
        private readonly bool[,] m_CheckedPoints;

        private int m_SimplifyTolerance = 5;

        # region check stopping condition variables

        private int m_NumOfSimilarIterations;
        private int m_DifferenceThreshold = 5;
        private int m_DifferenceCountThreshold = 10;

        # endregion

        public Snake(Bitmap image, List<Point> initialPoints, double alphaValue)
        {
            m_OriginalImage = image;
            m_AlphaValue = alphaValue;

            m_ImageWidth = m_OriginalImage.Width;
            m_ImageHeight = m_OriginalImage.Height;

            m_CheckedPoints = new bool[m_ImageWidth, m_ImageHeight];

            CalculateSignedDistanceMap(initialPoints);
        }

        public Snake(Bitmap image, List<Point> initialPoints)
        {
            m_OriginalImage = image;

            m_ImageWidth = m_OriginalImage.Width;
            m_ImageHeight = m_OriginalImage.Height;

            m_CheckedPoints = new bool[m_ImageWidth, m_ImageHeight];

            CalculateSignedDistanceMap(initialPoints);
        }

        /// <summary>
        /// Creates a 2 dimensional boolean array from a list of points. 
        /// Points appear as 1 and non-points appear as 0
        /// </summary>
        /// <param name="initialPoints">The initial points</param>
        private void CalculateSignedDistanceMap(List<Point> initialPoints)
        {
            DateTime setUpStart;
            DateTime setUpEnd;

            if (TimeTest == true)
            {
                setUpStart = DateTime.Now;
            }

            SignedDistanceMap signedDistanceMap = new SignedDistanceMap(initialPoints, m_ImageWidth, m_ImageHeight);

            m_Sdf = signedDistanceMap.GetSignedDistanceMap();

            if (TimeTest == true)
            {
                setUpEnd = DateTime.Now;

                var timeSpan = setUpEnd.Subtract(setUpStart);
                var timeDifferenceMilliseconds = timeSpan.Milliseconds;
                var timeDifferenceSeconds = timeSpan.Seconds;
                var timeDifferenceMinutes = timeSpan.Minutes;

                string iterationText = "Setup time(mm:ss:mm): " + timeDifferenceMinutes + ":" + timeDifferenceSeconds + ":" + timeDifferenceMilliseconds;

                using (StreamWriter sw = File.AppendText(@"Times.txt"))
                {
                    sw.WriteLine();
                    sw.WriteLine(iterationText);
                    sw.WriteLine();
                }
            }
        }

        public void RunActiveContour()
        {
            if (TimeTest == true)
            {
                string iterationText = "Iteration - " + m_Counter;
                //System.IO.File.WriteAllText(@"FitnessTimes.txt", generationText);

                using (StreamWriter sw = File.AppendText(@"Times.txt"))
                {
                    sw.WriteLine();
                    sw.WriteLine(iterationText);
                    sw.WriteLine();

                    m_TotalStart = DateTime.Now;
                }
            }

            while (!m_SolutionFound)
            {
                m_Counter++;

                if (TimeTest == true)
                {
                    m_StartTime = DateTime.Now;
                }

                CalculateMeanImageBrightness();

                CalculateCurvesNarrowBand();

                CalculateImageForce();

                CalculateCurvaturePenalty();

                CalculateGradientDescent();

                EvolveCurve();

                m_Sdf = SmoothSDF();

                m_CheckStart = DateTime.Now;

                if (m_Counter == 1)
                    m_SdfOld = (double[,])m_Sdf.Clone();
                else
                {
                    CheckIfSnakeHasConverged();
                    m_SdfOld = (double[,])m_Sdf.Clone();
                }

                m_CheckEnd = DateTime.Now;

                if (DrawingTest && (m_Counter % DrawingTestCounter == 0 || m_Counter == 1))
                {
                    DrawCurve();

                    ThinCurve();

                    DrawThinnedCurve();
                }

                if (TimeTest == true)
                {
                    m_EndTime = DateTime.Now;

                    WriteTimes(m_Counter);
                }
            }

            if (DrawingTest)
            {
                DrawCurve();
            }

            ThinCurve();

            if (DrawingTest)
            {
                DrawThinnedCurve();
            }
            //Calculate points
            CalculateCurvePoints();

            if (DrawingTest)
            {
                DrawFinalPoints();
            }
        }

        # region Brightness methods

        /// <summary>
        /// Calculates the mean interior and exterior brightness of the entire image
        /// </summary>
        private void CalculateMeanImageBrightness()
        {
            double interiorTotal = 0;
            double exteriorTotal = 0;

            int interiorNumber = 0;
            int exteriorNumber = 0;

            m_InteriorMean = 0;
            m_ExteriorMean = 0;

            int sampleRate = 4;

            for (int yPos = 0; yPos < m_ImageHeight; yPos += sampleRate)
            {
                for (int xPos = 0; xPos < m_ImageWidth; xPos += sampleRate)
                {
                    if (m_Sdf[xPos, yPos] < 0)         //Exterior
                    {
                        exteriorTotal += GetBrightnessAt(xPos, yPos);

                        exteriorNumber++;
                    }
                    else
                    {
                        interiorTotal += GetBrightnessAt(xPos, yPos);

                        interiorNumber++;
                    }
                }
            }

            m_InteriorMean = interiorTotal / (double)interiorNumber;
            m_ExteriorMean = exteriorTotal / (double)exteriorNumber;
        }

        /// <summary>
        /// Returns the brightness of the original image at the given location
        /// </summary>
        /// <param name="xPos"></param>
        /// <param name="yPos"></param>
        /// <returns></returns>
        private int GetBrightnessAt(int xPos, int yPos)
        {
            int brightness;

            int pixelValue = m_OriginalImage.GetPixel(xPos, yPos).ToArgb();

            double r = (pixelValue & 0xff0000) >> 16;
            double g = (pixelValue & 0xff00) >> 8;
            double b = pixelValue & 0xff;

            brightness = CalculateBrightness(r, g, b);

            return brightness;
        }

        /**
         * Returns the brightness value of a given RGB
         * 
         * @param r The red value of the pixel
         * @param g The green value of the pixel
         * @param b The blue value of the pixel
         * 
         * @return The brightness value associated with the given RGB values
         */
        private int CalculateBrightness(double r, double g, double b)
        {
            int brightness = (int)Math.Floor(0.334 * r + 0.333 * g + 0.333 * b);

            return brightness;
        }

        # endregion

        /// <summary>
        /// Calculates the points which fall within the curves narrow band (areas which are below narrowBandSize and above -narrowBandSize)
        /// </summary>
        /// <returns></returns>
        private void CalculateCurvesNarrowBand()
        {
            //narrowBand.Clear();

            m_NarrowBand = new List<Point>();

            for (int yPos = 0; yPos < m_ImageHeight; yPos++)
            {
                for (int xPos = 0; xPos < m_ImageWidth; xPos++)
                {
                    double sdfValue = m_Sdf[xPos, yPos];

                    if (sdfValue <= m_NarrowBandSize && sdfValue >= (0 - m_NarrowBandSize))
                        m_NarrowBand.Add(new Point(xPos, yPos));
                }
            }

            //DrawCurvesNarrowBand();
        }

        /// <summary>
        /// Calculates the image force at each point in the narrow band
        /// </summary>
        private void CalculateImageForce()
        {
            m_ImageForce = new double[m_ImageWidth, m_ImageHeight];

            foreach (Point point in m_NarrowBand)
            {
                int xPoint = point.X;
                int yPoint = point.Y;

                int pointBrightness = GetBrightnessAt(xPoint, yPoint);

                m_ImageForce[xPoint, yPoint] = Math.Pow((pointBrightness - m_ExteriorMean), 2) - Math.Pow((pointBrightness - m_InteriorMean), 2);
            }
        }

        /// <summary>
        /// Calculates the curvature penalty
        /// </summary>
        /// <returns></returns>
        private void CalculateCurvaturePenalty()
        {
            CurvaturePenalty calcCurvaturePenalty = new CurvaturePenalty(m_Sdf, m_NarrowBand);

            m_CurvaturePenalty = calcCurvaturePenalty.GetCurvaturePenalty();
        }

        private void CalculateGradientDescent()
        {
            m_GradientDescentValues = new double[m_ImageWidth, m_ImageHeight];

            double maximumForce = GetMaximumForce();

            m_MaxGradientDescentValue = 0;

            double currentGradientDescentValue = 0;

            foreach (Point point in m_NarrowBand)
            {
                int xPos = point.X;
                int yPos = point.Y;

                double force = m_ImageForce[xPos, yPos];

                currentGradientDescentValue = (force / maximumForce) + m_AlphaValue + m_CurvaturePenalty[xPos, yPos];

                m_GradientDescentValues[xPos, yPos] = currentGradientDescentValue;

                if (m_MaxGradientDescentValue < currentGradientDescentValue)
                    m_MaxGradientDescentValue = currentGradientDescentValue;
            }
        }

        private double GetMaximumForce()
        {
            double maximumForce = 0;

            foreach (Point point in m_NarrowBand)
            {
                int xPos = point.X;
                int yPos = point.Y;

                double currentForce = Math.Abs(m_ImageForce[xPos, yPos]);

                if (currentForce > maximumForce)
                    maximumForce = currentForce;
            }

            return maximumForce;
        }

        private void EvolveCurve()
        {
            //double maxValue = 0;

            //foreach (double value in gradientDescentValues)
            //{
            //    if (maxValue < value)
            //        maxValue = value;
            //}

            double dt = 0.45 / (m_MaxGradientDescentValue + double.Epsilon);

            foreach (Point point in m_NarrowBand)
            {
                int xPos = point.X;
                int yPos = point.Y;

                m_Sdf[xPos, yPos] = m_Sdf[xPos, yPos] + (dt * m_GradientDescentValues[xPos, yPos]);
            }
        }

        private double[,] SmoothSDF()
        {
            double[,] smootheSdf = new double[m_ImageWidth, m_ImageHeight];

            //foreach (Point point in narrowBand)
            //{
            for (int yPos = 0; yPos < m_ImageHeight; yPos++)
            {
                for (int xPos = 0; xPos < m_ImageWidth; xPos++)
                {
                    //int xPos = point.X;
                    //int yPos = point.Y;

                    smootheSdf[xPos, yPos] = Sussman(xPos, yPos, 0.5);
                }
            }

            return smootheSdf;
        }

        # region Sussman methods

        private double Sussman(int xPos, int yPos, double timeStep)
        {
            double startValue = m_Sdf[xPos, yPos];
            double endValue;

            double shiftRightValue;

            if (xPos - 1 < 0)
                shiftRightValue = 0;
            else
                shiftRightValue = startValue - ShiftRight(xPos, yPos);

            double shiftLeftValue;

            if (xPos + 1 > m_ImageWidth - 1)
                shiftLeftValue = 0;
            else
                shiftLeftValue = ShiftLeft(xPos, yPos) - startValue;


            double shiftDownValue;

            if (yPos - 1 < 0)
                shiftDownValue = 0;
            else
                shiftDownValue = startValue - ShiftDown(xPos, yPos);

            double shiftUpValue;

            if (yPos + 1 > m_ImageHeight - 1)
                shiftUpValue = 0;
            else
                shiftUpValue = ShiftUp(xPos, yPos) - startValue;

            double rightPos = shiftRightValue;
            double rightNeg = shiftRightValue;

            if (rightPos < 0)
                rightPos = 0;
            if (rightNeg > 0)
                rightNeg = 0;

            double leftPos = shiftLeftValue;
            double leftNeg = shiftLeftValue;

            if (leftPos < 0)
                leftPos = 0;
            if (leftNeg > 0)
                leftNeg = 0;

            double downPos = shiftDownValue;
            double downNeg = shiftDownValue;

            if (downPos < 0)
                downPos = 0;
            if (downNeg > 0)
                downNeg = 0;

            double upPos = shiftUpValue;
            double upNeg = shiftUpValue;

            if (upPos < 0)
                upPos = 0;
            if (upNeg > 0)
                upNeg = 0;

            double db;

            if (startValue > 0)
            {
                db = Math.Sqrt(Math.Max(Math.Pow(rightPos, 2), Math.Pow(leftNeg, 2))) + Math.Sqrt(Math.Max(Math.Pow(downPos, 2), Math.Pow(upNeg, 2))) - 1;
            }
            else if (startValue < 0)
            {
                db = Math.Sqrt(Math.Max(Math.Pow(rightNeg, 2), Math.Pow(leftPos, 2))) + Math.Sqrt(Math.Max(Math.Pow(downNeg, 2), Math.Pow(upPos, 2))) - 1;
            }
            else
                db = 0;

            endValue = startValue - timeStep * SussmanSign(startValue) * db;

            return endValue;
        }

        private double ShiftRight(int xPos, int yPos)
        {
            int shiftX = xPos - 1;
            int shiftY = yPos;

            double rightValue;

            //if (shiftX < 0)
            //    rightValue = 0;
            //else
            rightValue = m_Sdf[shiftX, shiftY];

            return rightValue;
        }

        private double ShiftLeft(int xPos, int yPos)
        {
            int shiftX = xPos + 1;
            int shiftY = yPos;

            double leftValue;

            //if (shiftX > imageWidth-1)
            //    leftValue = 0;
            //else
            leftValue = m_Sdf[shiftX, shiftY];

            return leftValue;
        }

        private double ShiftDown(int xPos, int yPos)
        {
            int shiftX = xPos;
            int shiftY = yPos - 1;

            double downValue;

            downValue = m_Sdf[shiftX, shiftY];

            return downValue;
        }

        private double ShiftUp(int xPos, int yPos)
        {
            int shiftX = xPos;
            int shiftY = yPos + 1;

            double upValue;

            upValue = m_Sdf[shiftX, shiftY];

            return upValue;
        }

        private double SussmanSign(double value)
        {
            double sussmanValue = value / Math.Sqrt(Math.Pow(value, 2) + 1);

            return sussmanValue;
        }

        # endregion

        private void CheckIfSnakeHasConverged()
        {
            int diffCount = 0;

            for (int yPos = 0; yPos < m_ImageHeight; yPos++)
            {
                for (int xPos = 0; xPos < m_ImageWidth; xPos++)
                {
                    if (m_SdfOld[xPos, yPos] < 0 && m_Sdf[xPos, yPos] > 0 || m_SdfOld[xPos, yPos] > 0 && m_Sdf[xPos, yPos] < 0)
                    {
                        diffCount++;
                    }
                }
            }

            if (diffCount < m_DifferenceThreshold)
            {
                m_NumOfSimilarIterations++;
            }
            else
            {
                m_NumOfSimilarIterations = 0;

                m_NumOfIterationsWithoutChange++;
            }

            if (m_NumOfSimilarIterations >= m_DifferenceCountThreshold)
            {
                m_SolutionFound = true;
            }

            if (m_NumOfIterationsWithoutChange > 50)
            {
                m_DifferenceThreshold++;
                m_NumOfIterationsWithoutChange = 0;
            }

            if (TimeTest)
            {
                using (StreamWriter sw = File.AppendText(@"Times.txt"))
                {
                    sw.Write("Difference from last iteration: " + diffCount + " --- ");
                }
            }
        }

        /// <summary>
        /// Calculates the line around the shape
        /// </summary>
        private void ThinCurve()
        {
            m_FinalCurve = new bool[m_ImageWidth, m_ImageHeight];

            for (int yPos = 0; yPos < m_ImageHeight; yPos++)
            {
                for (int xPos = 0; xPos < m_ImageWidth; xPos++)
                {
                    if (m_Sdf[xPos, yPos] > 0)
                    {
                        m_FinalCurve[xPos, yPos] = false;
                    }
                    else if (IsPointAtBoundary(xPos, yPos))
                    {
                        m_FinalCurve[xPos, yPos] = true;
                    }
                    else
                    {
                        m_FinalCurve[xPos, yPos] = false;
                    }
                }
            }
        }

        # region Boundary methods

        /// <summary>
        /// Checks if any of the given points neighbours are background pixels. 
        /// If so it marks the point as an edge.
        /// </summary>
        /// <param name="xPos"></param>
        /// <param name="yPos"></param>
        /// <returns></returns>
        private bool IsPointAtBoundary(int xPos, int yPos)
        {
            if (CheckNorth(xPos, yPos) > 0)
                return true;

            if (CheckNorthEast(xPos, yPos) > 0)
                return true;

            if (CheckEast(xPos, yPos) > 0)
                return true;

            if (CheckSouthEast(xPos, yPos) > 0)
                return true;

            if (CheckSouth(xPos, yPos) > 0)
                return true;

            if (CheckSouthWest(xPos, yPos) > 0)
                return true;

            if (CheckWest(xPos, yPos) > 0)
                return true;

            if (CheckNorthWest(xPos, yPos) > 0)
                return true;

            return false;
        }

        private double CheckNorth(int xPos, int yPos)
        {
            if (yPos > 0)
                return m_Sdf[xPos, yPos - 1];
            else
                return 1;
        }

        private double CheckNorthEast(int xPos, int yPos)
        {
            if (yPos > 0 && xPos < m_ImageWidth - 1)
                return m_Sdf[xPos + 1, yPos - 1];
            else
                return 1;
        }

        private double CheckEast(int xPos, int yPos)
        {
            if (xPos < m_ImageWidth - 1)
                return m_Sdf[xPos + 1, yPos];
            else
                return 1;
        }

        private double CheckSouthEast(int xPos, int yPos)
        {
            if (xPos < m_ImageWidth - 1 && yPos < m_ImageHeight - 1)
                return m_Sdf[xPos + 1, yPos + 1];
            else
                return 1;
        }

        private double CheckSouth(int xPos, int yPos)
        {
            if (yPos < m_ImageHeight - 1)
                return m_Sdf[xPos, yPos + 1];
            else
                return 1;
        }

        private double CheckSouthWest(int xPos, int yPos)
        {
            if (yPos < m_ImageHeight - 1 && xPos > 0)
                return m_Sdf[xPos - 1, yPos + 1];
            else
                return 1;
        }

        private double CheckWest(int xPos, int yPos)
        {
            if (xPos > 0)
                return m_Sdf[xPos - 1, yPos];
            else
                return 1;
        }

        private double CheckNorthWest(int xPos, int yPos)
        {
            if (xPos > 0 && yPos > 0)
                return m_Sdf[xPos - 1, yPos - 1];
            else
                return 1;
        }

        # endregion

        # region Calculate curve points methods

        /// <summary>
        /// Since the curve can not be represented by all it's points it must be smoothed and 
        /// </summary>
        private void CalculateCurvePoints()
        {
            List<Point> allPoints = new List<Point>();

            ExtractLine extractLine = new ExtractLine(m_FinalCurve);
            extractLine.Extract();

            allPoints = extractLine.GetLine();

            if(DrawingTest)
                DrawPoints(allPoints);

            if (allPoints.Count > 0)
            {
                SimplifyShape simplifyShape = new SimplifyShape(allPoints, m_SimplifyTolerance);

                simplifyShape.Simplify();

                m_CurvePoints = simplifyShape.GetSimplifiedPoints();
            }
            
        }

        #endregion

        # region FindnextPoint methods

        private Point FindNextPoint(Point currentPoint)
        {
            var xPoint = currentPoint.X;
            var yPoint = currentPoint.Y;

            if (CheckIfPointIsNorth(xPoint, yPoint) == true)
            {
                return (new Point(xPoint, yPoint - 1));
            }
            else if (CheckIfPointIsNorthEast(xPoint, yPoint))
            {
                return (new Point(xPoint + 1, yPoint - 1));
            }
            else if (CheckIfPointIsEast(xPoint, yPoint))
            {
                return (new Point(xPoint + 1, yPoint));
            }
            else if (CheckIfPointIsSouthEast(xPoint, yPoint))
            {
                return (new Point(xPoint + 1, yPoint + 1));
            }
            else if (CheckIfPointIsSouth(xPoint, yPoint))
            {
                return (new Point(xPoint, yPoint + 1));
            }
            else if (CheckIfPointIsSouthWest(xPoint, yPoint))
            {
                return (new Point(xPoint - 1, yPoint + 1));
            }
            else if (CheckIfPointIsWest(xPoint, yPoint))
            {
                return (new Point(xPoint - 1, yPoint));
            }
            else if (CheckIfPointIsNorthWest(xPoint, yPoint))
            {
                return (new Point(xPoint - 1, yPoint - 1));
            }

            return new Point(-1, -1);
        }

        private bool CheckIfPointIsNorth(int xPoint, int yPoint)
        {
            if (yPoint > 0)
            {
                if (m_FinalCurve[xPoint, yPoint - 1] == true && m_CheckedPoints[xPoint, yPoint - 1] == false)
                    return true;
            }

            return false;
        }

        private bool CheckIfPointIsNorthEast(int xPoint, int yPoint)
        {
            if (yPoint > 0 && xPoint < m_ImageWidth - 1)
            {
                if (m_FinalCurve[xPoint + 1, yPoint - 1] == true && m_CheckedPoints[xPoint + 1, yPoint - 1] == false)
                    return true;
            }

            return false;
        }

        private bool CheckIfPointIsEast(int xPoint, int yPoint)
        {
            if (xPoint < m_ImageWidth - 1)
            {
                if (m_FinalCurve[xPoint + 1, yPoint] == true && m_CheckedPoints[xPoint + 1, yPoint] == false)
                    return true;
            }

            return false;
        }

        private bool CheckIfPointIsSouthEast(int xPoint, int yPoint)
        {
            if (xPoint < m_ImageWidth - 1 && yPoint < m_ImageHeight - 1)
            {
                if (m_FinalCurve[xPoint + 1, yPoint + 1] == true && m_CheckedPoints[xPoint + 1, yPoint + 1] == false)
                    return true;
            }

            return false;
        }

        private bool CheckIfPointIsSouth(int xPoint, int yPoint)
        {
            if (yPoint < m_ImageHeight - 1)
            {
                if (m_FinalCurve[xPoint, yPoint + 1] == true && m_CheckedPoints[xPoint, yPoint + 1] == false)
                    return true;
            }

            return false;
        }

        private bool CheckIfPointIsSouthWest(int xPoint, int yPoint)
        {
            if (xPoint > 0 && yPoint < m_ImageHeight - 1)
            {
                if (m_FinalCurve[xPoint - 1, yPoint + 1] == true && m_CheckedPoints[xPoint - 1, yPoint + 1] == false)
                    return true;
            }

            return false;
        }

        private bool CheckIfPointIsWest(int xPoint, int yPoint)
        {
            if (xPoint > 0)
            {
                if (m_FinalCurve[xPoint - 1, yPoint] == true && m_CheckedPoints[xPoint - 1, yPoint] == false)
                    return true;
            }

            return false;
        }

        private bool CheckIfPointIsNorthWest(int xPoint, int yPoint)
        {
            if (xPoint > 0 && yPoint > 0)
            {
                if (m_FinalCurve[xPoint - 1, yPoint - 1] == true && m_CheckedPoints[xPoint - 1, yPoint - 1] == false)
                    return true;
            }

            return false;
        }

        # endregion

        # region Get methods

        public List<Point> GetCurvePoints()
        {
            return m_CurvePoints;
        }

        //public Point[,] GetCurvePoints()
        //{
        //    return sdf;
        //}

        # endregion

        # region Set methods

        public void SetNarrowBandSize(int narrowBandSize)
        {
            this.m_NarrowBandSize = narrowBandSize;
        }

        public void SetSimplifyTolerance(int simplifyTolerance)
        {
            this.m_SimplifyTolerance = simplifyTolerance;
        }

        /// <summary>
        /// Sets the threshold for how many pixel differences are allowed for an iteration to be 
        /// considered different to the last
        /// </summary>
        /// <param name="differenceThreshold"></param>
        public void SetStopCheckDifferenceThreshold(int differenceThreshold)
        {
            this.m_DifferenceThreshold = differenceThreshold;
        }

        /// <summary>
        /// Sets the number of similar iterations are allowed before the snake is classed as converged 
        /// </summary>
        /// <param name="differenceCountThreshold"></param>
        public void SetStopCheckDifferenceCounter(int differenceCountThreshold)
        {
            this.m_DifferenceCountThreshold = differenceCountThreshold;
        }

        # endregion

        # region Testing  methods

        private void DrawCurvesNarrowBand()
        {
            if (m_Counter % 10 == 0)
            {
                Bitmap image = new Bitmap(m_OriginalImage);

                foreach (Point point in m_NarrowBand)
                {
                    image.SetPixel(point.X, point.Y, Color.LawnGreen);
                }

                image.Save("Snake - NarrowBand - " + m_Counter + ".bmp");
            }
        }

        private void WriteTimes(int counter)
        {
            TimeSpan timeSpan = m_EndTime.Subtract(m_StartTime);
            int timeDifferenceMilliseconds = timeSpan.Milliseconds;
            int timeDifferenceSeconds = timeSpan.Seconds;
            int timeDifferenceMinutes = timeSpan.Minutes;


            TimeSpan stopCheckTimeSpan = m_CheckEnd.Subtract(m_CheckStart);
            int stopCheckTimeDifferenceMilliseconds = stopCheckTimeSpan.Milliseconds;
            int stopCheckTimeDifferenceSeconds = stopCheckTimeSpan.Seconds;
            int stopChecTimeDifferenceMinutes = stopCheckTimeSpan.Minutes;


            string time = "Iteration - " + counter + ", Total run time(mm:ss:mm): " + timeDifferenceMinutes + ":" + timeDifferenceSeconds + ":" + timeDifferenceMilliseconds + ", StopCheck time(mm:ss:mm): " + stopChecTimeDifferenceMinutes + ":" + stopCheckTimeDifferenceSeconds + ":" + stopCheckTimeDifferenceMilliseconds;
            //System.IO.File.AppendText(@"FitnessTimes.txt", time);
            //System.IO.File.AppendText

            using (StreamWriter sw = File.AppendText(@"Times.txt"))
            {
                sw.WriteLine(time);

                if (m_SolutionFound)
                {
                    m_TotalEnd = DateTime.Now;

                    TimeSpan totalTimeSpan = m_TotalEnd.Subtract(m_TotalStart);
                    int totalTimeMinutes = totalTimeSpan.Minutes;
                    int totalTimeSeconds = totalTimeSpan.Seconds;

                    sw.WriteLine();
                    sw.WriteLine("Total time(mm:ss) - " + totalTimeMinutes + ":" + totalTimeSeconds);
                    sw.WriteLine();
                }
            }
        }

        /// <summary>
        /// Draws the curve over the original image and outputs it (for debugging/testing purposes)
        /// </summary>
        private void DrawCurve()
        {
            Bitmap image = new Bitmap(m_OriginalImage);

            //Graphics g = Graphics.FromImage(image);

            for (int yPos = 0; yPos < m_ImageHeight; yPos++)
            {
                for (int xPos = 0; xPos < m_ImageWidth; xPos++)
                {
                    if (m_Sdf[xPos, yPos] < 0)
                    {
                        image.SetPixel(xPos, yPos, Color.LawnGreen);
                    }
                }
            }

            image.Save("Snake - curve - " + m_Counter + ".bmp");
        }

        private void DrawThinnedCurve()
        {
            Bitmap image = new Bitmap(m_OriginalImage);

            //Graphics g = Graphics.FromImage(image);

            for (int yPos = 0; yPos < m_ImageHeight; yPos++)
            {
                for (int xPos = 0; xPos < m_ImageWidth; xPos++)
                {
                    if (m_FinalCurve[xPos, yPos])
                    {
                        image.SetPixel(xPos, yPos, Color.LawnGreen);
                    }
                }
            }

            image.Save("Snake - Thinned curve - " + m_Counter + ".bmp");
        }

        private void DrawPoints(List<Point> allPoints)
        {
            Bitmap image = new Bitmap(m_OriginalImage);

            for (int i = 0; i < allPoints.Count; i++)
            {
                image.SetPixel(allPoints[i].X, allPoints[i].Y, Color.LawnGreen);
            }

            image.Save("Snake - All Points.bmp");
        }

        private void DrawFinalPoints()
        {
            Bitmap image = new Bitmap(m_OriginalImage);

            Graphics g = Graphics.FromImage(image);
            Pen pen = new Pen(Color.LawnGreen);
            g.DrawPolygon(pen, m_CurvePoints.ToArray());

            image.Save("Snake - Final Points (tolerance=" + m_SimplifyTolerance + ").bmp");
        }

        # endregion
    }
}
