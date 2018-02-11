 using System;
using System.Collections.Generic;
using System.Drawing;

namespace BoreholeFeatures
{
    /// <summary>
    /// A class which represents an inclusion and all the actions that can be applied to it
    /// 
    /// Author - Terry Malone
    /// Version 1.1 Refactored
    /// </summary>
    public class Inclusion
    {
        private readonly int m_azimuthResolution;
        private readonly int m_depthResolution;

        private string m_description = "";
        private string m_pointsString = "";
        
        protected string[] AllGroupNames;

        private int m_sourceStartDepth;

        #region properties

        public int StartDepth { get; private set; }

        public int EndDepth { get; private set; }
        
        public string Group { get; set; }

        public string PointsString
        {
            get
            {
                CalculatePointsString();
                return m_pointsString;
            }
        }

        public string Description
        {
            get => m_description;

            set
            {
                m_description = value;
                TimeLastModified = DateTime.Now;
            }
        }
        
        public string InclusionType { get; set; } = "";
        
        public List<Point> Points { get; private set; }

        /// <summary>
        /// Has the cluster has been completely drawn or not
        /// </summary>
        public bool IsComplete { get; set; }

        /// <summary>
        /// The left boundary of the cluster
        /// </summary>
        public int LeftXBoundary { get; set; }

        /// <summary>
        /// The left boundary of the cluster
        /// </summary>
        public int RightXBoundary { get; set; }

        /// <summary>
        /// The y position of the top boundary of the cluster
        /// </summary>
        public int TopYBoundary { get; set; }

        /// <summary>
        /// The bottom boundary of the cluster
        /// </summary>
        public int BottomYBoundary { get; set; }

        /// <summary>
        /// The time the cluster was added
        /// </summary>
        public DateTime TimeAdded { get; set; }

        /// <summary>
        /// The time the cluster was last modified
        /// </summary>
        public DateTime TimeLastModified { get; set; }

        public void XStartBoundary(int startX)
        {
            LeftXBoundary = startX;
        }

        public int SourceStartDepth
        {
            get => m_sourceStartDepth;
            set
            {
                m_sourceStartDepth = value;

                CalculateStartDepthInMM();
                CalculateEndDepthInMM();

                calculateYBounds();
                CalculatePointsString();
            }
        }

        #endregion properties

        /// <summary>
        /// Constructor method
        /// </summary>
        /// <param name="azimuthResolution">The vertical resolution of the borehole image</param>
        /// <param name="depthResolution">The depth resolution of the borehole image</param>
        public Inclusion(int azimuthResolution, int depthResolution)
        {
            this.m_azimuthResolution = azimuthResolution;
            this.m_depthResolution = depthResolution;

            Points = new List<Point>();

            TimeAdded = DateTime.Now;
            TimeLastModified = DateTime.Now;
        }

        # region Point operations

        /// <summary>
        /// Adds a point to the end of the list of inclusion points
        /// </summary>
        /// <param name="newPoint">The new point to add</param>
        public void AddPoint(Point newPoint)
        {
            Points.Add(newPoint);

            TimeLastModified = DateTime.Now;

            calculateXBounds();
            calculateYBounds();
        }

        /// <summary>
        /// Adds a point after the given position
        /// </summary>
        /// <param name="newPoint"></param>
        /// <param name="addAfter"></param>
        public void AddPoint(Point newPoint, int addAfter)
        {
            Points.Insert(addAfter+1, newPoint);

            TimeLastModified = DateTime.Now;

            calculateXBounds();
            calculateYBounds();
        }

        /// <summary>
        /// Removes a point from the list of inclusion points
        /// </summary>
        /// <param name="deletePoint">The point to remove</param>
        public void RemovePoint(Point deletePoint)
        {
            if (Points.Contains(deletePoint))
            {
                Points.Remove(deletePoint);

                CheckFeatureIsWithinImageBounds();

                calculateXBounds();
                calculateYBounds();
                TimeLastModified = DateTime.Now;
            }
        }

        /// <summary>
        /// Moves a point in the list of inclusion points
        /// </summary>
        /// <param name="oldPosition">The position of the point to move</param>
        /// <param name="newPosition">The position to move the point to</param>
        public void MovePoint(Point oldPosition, Point newPosition)
        {
            int index;

            if (Points.Contains(oldPosition))
            {
                index = Points.IndexOf(oldPosition);

                Points.Remove(oldPosition);
                Points.Insert(index, newPosition);

                TimeLastModified = DateTime.Now;
            }

            CheckFeatureIsWithinImageBounds();

            calculateXBounds();
            calculateYBounds();
        }

        private void ShiftAllXPoints(int amountToShift)
        {
            for (int i = 0; i < Points.Count; i++)
            {
                Point currentPoint = Points[i];
                Points[i] = new Point(currentPoint.X + amountToShift, currentPoint.Y);
            }

            LeftXBoundary += amountToShift;
            RightXBoundary += amountToShift;
        }

        # endregion Point operations
        
        /// <summary>
        /// Moves the position of the entire inclusion
        /// </summary>
        /// <param name="xMoveBy">Amout to move inclusion along the x axis</param>
        /// <param name="yMoveBy">Amout to move inclusion along the y axis</param>
        public void MoveInclusion(int xMoveBy, int yMoveBy)
        {
            Point currentPoint;

            for (int i = 0; i < Points.Count; i++)
            {
                currentPoint = Points[i];

                currentPoint.X += xMoveBy;
                currentPoint.Y += yMoveBy;

                Points[i] = currentPoint;
            }

            CheckFeatureIsWithinImageBounds();

            calculateXBounds();
            calculateYBounds();


            TimeLastModified = DateTime.Now;
        }

        /// <summary>
        /// Calculates the start depth of the inclusion in millimetres
        /// </summary>
        private void CalculateStartDepthInMM()
        {
            StartDepth = (int)((double)m_sourceStartDepth + (double)((double)TopYBoundary * (double)m_depthResolution));
        }

        /// <summary>
        /// Calculates the end depth of the inclusion in millimetres
        /// </summary>
        private void CalculateEndDepthInMM()
        {
            EndDepth = (int)((double)m_sourceStartDepth + (double)((double)BottomYBoundary * (double)m_depthResolution));
        }

        /// <summary>
        /// Calculates the y boundaries of the cluster
        /// </summary>
        private void calculateYBounds()
        {
            CalculateStartDepthInMM();
            CalculateEndDepthInMM();

            for (int i = 1; i < Points.Count; i++)
            {
                if (Points[i].Y > BottomYBoundary)
                {
                    BottomYBoundary = Points[i].Y;

                    CalculateEndDepthInMM();
                }

                if (Points[i].Y < TopYBoundary)
                {
                    TopYBoundary = Points[i].Y;

                    CalculateStartDepthInMM();
                }
            }
        }

        /// <summary>
        /// Calculates the x boundaries of the cluster
        /// </summary>
        private void calculateXBounds()
        {
            if (Points.Count > 0)
            {
                LeftXBoundary = Points[0].X;
                RightXBoundary = Points[0].X;
                TopYBoundary = Points[0].Y;
                BottomYBoundary = Points[0].Y;



                for (int i = 1; i < Points.Count; i++)
                {
                    if (Points[i].X > RightXBoundary)
                        RightXBoundary = Points[i].X;

                    if (Points[i].X < LeftXBoundary)
                        LeftXBoundary = Points[i].X;

                    if (Points[i].Y > BottomYBoundary)
                    {
                        BottomYBoundary = Points[i].Y;

                        CalculateEndDepthInMM();
                    }

                    if (Points[i].Y < TopYBoundary)
                    {
                        TopYBoundary = Points[i].Y;

                        CalculateStartDepthInMM();

                    }
                }
            }
        }

        /// <summary>
        /// Converts the list of points to a string
        /// </summary>
        public void CalculatePointsString()
        {
            if (Points.Count > 0)
            {
                m_pointsString = "";
                int xPoint, yPoint;

                for (int i = 0; i < Points.Count; i++)
                {
                    xPoint = Points[i].X;
                    yPoint = Points[i].Y;

                    m_pointsString = String.Concat(m_pointsString, "(", xPoint, ", ", yPoint, ") ");
                }
            }
        }

        # region Get methods

        /// <summary>
        /// Returns text representing all of the inclusions properties
        /// 
        /// topYBoundary        - top Y boundary in pixels
        /// bottomYBoundary     - bottom Y boundary in pixels
        /// leftXBoundary       - X position of left boundary in pixels
        /// rightXBoundary      - X position of right boundary in pixels
        /// points              - List of inclusion points with commas removed
        /// inclusionType       - Contents of inclusion
        /// Description         - Description of inclusion
        /// Time added          - Time the inclusion was first added
        /// Time last modified  - Time the inclusion was last modified
        /// </summary>
        /// <returns>The inclusion properties</returns>
        public String getDetails()
        {
            //calculateBoundaries();
            CalculatePointsString();

            String details;

            m_description = m_description.Replace(',', ' ');

            String points = "";

            for (int i = 0; i < Points.Count; i++)
            {
                points = String.Concat(points, Points[i].X, " ", Points[i].Y, " ");
            }

            details = TopYBoundary + "," + 
                      BottomYBoundary + "," + 
                      LeftXBoundary + "," + 
                      RightXBoundary + "," + 
                      points + "," + 
                      InclusionType + "," + 
                      m_description + "," + 
                      TimeAdded + "," + 
                      TimeLastModified + "," + 
                      Group;

            return details;
        }

        public string[] GetInclusionGroupNames()
        {
            return AllGroupNames;
        }

        # endregion

        # region Set methods

        public void SetAllGroupNames(string[] allGroupNames)
        {
            this.AllGroupNames = allGroupNames;
        }

        # endregion Set methods

        /// <summary>
        /// Checks that image is not completely below 0 or completely above boreholeWidth
        /// This could happen due to rotation
        /// </summary>
        public void CheckFeatureIsWithinImageBounds()
        {
            bool allBelow0 = true;
            bool allAboveWidth = true;

            for (int i = 0; i < Points.Count; i++)
            {
                if (Points[i].X > 0)
                    allBelow0 = false;

                if (Points[i].X < m_azimuthResolution)
                    allAboveWidth = false;
            }

            if (allBelow0)
                ShiftAllXPoints(m_azimuthResolution);
            else if (allAboveWidth)
                ShiftAllXPoints(-m_azimuthResolution);
        }        

        public void SetPoints(List<Point> points)
        {
            Points.Clear();

            Points = points;

            TimeLastModified = DateTime.Now;

            calculateXBounds();
            calculateYBounds();
            CalculatePointsString();
        }
    }
}
