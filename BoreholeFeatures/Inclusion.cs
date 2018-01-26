using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Drawing;
using BoreholeFeatures.Converters;

namespace BoreholeFeatures
{
    /// <summary>
    /// A class which represents an inclusion and all the actions that can be applied to it
    /// 
    /// Author - Terry Malone
    /// Version 1.1 Refactored
    /// </summary>
    [DefaultPropertyAttribute("Description")]
    public class Inclusion
    {
        private int azimuthResolution, depthResolution;

        private List<Point> m_InclusionPoints;

        private int m_LeftXBoundary, m_RightXBoundary;
        private int m_TopYBoundary, m_BottomYBoundary;
        private int m_StartDepth, m_EndDepth;
        private String m_Description = "";
        private String m_InclusionType = "";
        private string m_PointsString = "";

        protected string group;
        protected string[] allGroupNames;

        private bool isComplete;

        private DateTime timeAdded, timeLastModified;
        private int sourceStartDepth;

        #region properties

        # region Property grid methods

        [CategoryAttribute("Depth"), DescriptionAttribute("The clusters start depth in millimetres")]
        [DisplayName("Start Depth")]
        public int StartDepth
        {
            get
            {
                return m_StartDepth;
            }
        }

        [CategoryAttribute("Depth"), DescriptionAttribute("The clusters end depth in millimetres")]
        [DisplayName("End Depth")]
        public int EndDepth
        {
            get
            {
                return m_EndDepth;
            }
        }

        [Browsable(true)]
        [DefaultValue("entry1")]
        [CategoryAttribute("\tDescription"), DescriptionAttribute("The group this inclusion belongs to (To change group display colours go to 'Features>Inclusions>Groups'")]
        [TypeConverter(typeof(InclusionGroupConverter))]
        public string Group
        {
            get { return group; }
            set
            {
                //group = value;

                //if(_model != null && group!=null)
                //    _model.AddGroup(group);
                group = value;
            }
        }

        [CategoryAttribute("Points"), DescriptionAttribute("The clusters points")]
        public string PointsString
        {
            get
            {
                CalculatePointsString();
                return m_PointsString;
            }
        }

        [CategoryAttribute("\tDescription"), DescriptionAttribute("Additional information")]
        public string Description
        {
            get { return m_Description; }
            set { m_Description = value; }
        }

        [Browsable(true)]
        [DefaultValue("entry1")]
        [CategoryAttribute("\tDescription"), DescriptionAttribute("The type of inclusion")]
        [TypeConverter(typeof(InclusionTypeConverter))]
        [DisplayName("Inclusion Type")]
        public string InclusionType
        {
            get { return m_InclusionType; }
            set { m_InclusionType = value; }
        }

        # endregion property grid methods

        public List<Point> Points
        {
            get
            {
                return m_InclusionPoints;
            }
        }

        /// <summary>
        /// Has the cluster has been completely drawn or not
        /// </summary>
        public bool IsComplete
        {
            get { return isComplete; }
            set { isComplete = value; }
        }

        /// <summary>
        /// The left boundary of the cluster
        /// </summary>
        public int LeftXBoundary
        {
            get { return m_LeftXBoundary; }
            set { m_LeftXBoundary = value; }
        }

        /// <summary>
        /// The left boundary of the cluster
        /// </summary>
        public int RightXBoundary
        {
            get { return m_RightXBoundary; }
            set { m_RightXBoundary = value; }
        }

        /// <summary>
        /// The y position of the top boundary of the cluster
        /// </summary>
        public int TopYBoundary
        {
            get { return m_TopYBoundary; }
            set { m_TopYBoundary = value; }
        }

        /// <summary>
        /// The bottom boundary of the cluster
        /// </summary>
        public int BottomYBoundary
        {
            get { return m_BottomYBoundary; }
            set { m_BottomYBoundary = value; }
        }

        /// <summary>
        /// The time the cluster was added
        /// </summary>
        public DateTime TimeAdded
        {
            get { return timeAdded; }
            set { timeAdded = value; }
        }

        /// <summary>
        /// The time the cluster was last modified
        /// </summary>
        public DateTime TimeLastModified
        {
            get { return timeLastModified; }
            set { timeLastModified = value; }
        }

        public void XStartBoundary(int startX)
        {
            m_LeftXBoundary = startX;
        }

        public int SourceStartDepth
        {
            get { return sourceStartDepth; }
            set
            {
                sourceStartDepth = value;

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
            this.azimuthResolution = azimuthResolution;
            this.depthResolution = depthResolution;

            m_InclusionPoints = new List<Point>();

            timeAdded = DateTime.Now;
            timeLastModified = DateTime.Now;
        }

        # region Point operations

        /// <summary>
        /// Adds a point to the end of the list of inclusion points
        /// </summary>
        /// <param name="newPoint">The new point to add</param>
        public void AddPoint(Point newPoint)
        {
            m_InclusionPoints.Add(newPoint);

            timeLastModified = DateTime.Now;

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
            m_InclusionPoints.Insert(addAfter+1, newPoint);

            timeLastModified = DateTime.Now;

            calculateXBounds();
            calculateYBounds();
        }

        /// <summary>
        /// Removes a point from the list of inclusion points
        /// </summary>
        /// <param name="deletePoint">The point to remove</param>
        public void RemovePoint(Point deletePoint)
        {
            if (m_InclusionPoints.Contains(deletePoint))
            {
                m_InclusionPoints.Remove(deletePoint);

                CheckFeatureIsWithinImageBounds();

                calculateXBounds();
                calculateYBounds();
                timeLastModified = DateTime.Now;
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

            if (m_InclusionPoints.Contains(oldPosition))
            {
                index = m_InclusionPoints.IndexOf(oldPosition);

                m_InclusionPoints.Remove(oldPosition);
                m_InclusionPoints.Insert(index, newPosition);

                timeLastModified = DateTime.Now;
            }

            CheckFeatureIsWithinImageBounds();

            calculateXBounds();
            calculateYBounds();
        }

        private void ShiftAllXPoints(int amountToShift)
        {
            for (int i = 0; i < m_InclusionPoints.Count; i++)
            {
                Point currentPoint = m_InclusionPoints[i];
                m_InclusionPoints[i] = new Point(currentPoint.X + amountToShift, currentPoint.Y);
            }

            m_LeftXBoundary += amountToShift;
            m_RightXBoundary += amountToShift;
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

            for (int i = 0; i < m_InclusionPoints.Count; i++)
            {
                currentPoint = m_InclusionPoints[i];

                currentPoint.X += xMoveBy;
                currentPoint.Y += yMoveBy;

                m_InclusionPoints[i] = currentPoint;
            }

            CheckFeatureIsWithinImageBounds();

            calculateXBounds();
            calculateYBounds();


            timeLastModified = DateTime.Now;
        }

        /// <summary>
        /// Calculates the start depth of the inclusion in millimetres
        /// </summary>
        private void CalculateStartDepthInMM()
        {
            m_StartDepth = (int)((double)sourceStartDepth + (double)((double)m_TopYBoundary * (double)depthResolution));
        }

        /// <summary>
        /// Calculates the end depth of the inclusion in millimetres
        /// </summary>
        private void CalculateEndDepthInMM()
        {
            m_EndDepth = (int)((double)sourceStartDepth + (double)((double)m_BottomYBoundary * (double)depthResolution));
        }

        /// <summary>
        /// Calculates the y boundaries of the cluster
        /// </summary>
        private void calculateYBounds()
        {
            CalculateStartDepthInMM();
            CalculateEndDepthInMM();

            for (int i = 1; i < m_InclusionPoints.Count; i++)
            {
                if (m_InclusionPoints[i].Y > m_BottomYBoundary)
                {
                    m_BottomYBoundary = m_InclusionPoints[i].Y;

                    CalculateEndDepthInMM();
                }

                if (m_InclusionPoints[i].Y < m_TopYBoundary)
                {
                    m_TopYBoundary = m_InclusionPoints[i].Y;

                    CalculateStartDepthInMM();
                }
            }
        }

        /// <summary>
        /// Calculates the x boundaries of the cluster
        /// </summary>
        private void calculateXBounds()
        {
            if (m_InclusionPoints.Count > 0)
            {
                m_LeftXBoundary = m_InclusionPoints[0].X;
                m_RightXBoundary = m_InclusionPoints[0].X;
                m_TopYBoundary = m_InclusionPoints[0].Y;
                m_BottomYBoundary = m_InclusionPoints[0].Y;



                for (int i = 1; i < m_InclusionPoints.Count; i++)
                {
                    if (m_InclusionPoints[i].X > m_RightXBoundary)
                        m_RightXBoundary = m_InclusionPoints[i].X;

                    if (m_InclusionPoints[i].X < m_LeftXBoundary)
                        m_LeftXBoundary = m_InclusionPoints[i].X;

                    if (m_InclusionPoints[i].Y > m_BottomYBoundary)
                    {
                        m_BottomYBoundary = m_InclusionPoints[i].Y;

                        CalculateEndDepthInMM();
                    }

                    if (m_InclusionPoints[i].Y < m_TopYBoundary)
                    {
                        m_TopYBoundary = m_InclusionPoints[i].Y;

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
            if (m_InclusionPoints.Count > 0)
            {
                m_PointsString = "";
                int xPoint, yPoint;

                for (int i = 0; i < m_InclusionPoints.Count; i++)
                {
                    xPoint = m_InclusionPoints[i].X;
                    yPoint = m_InclusionPoints[i].Y;

                    m_PointsString = String.Concat(m_PointsString, "(", xPoint, ", ", yPoint, ") ");
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

            m_Description = m_Description.Replace(',', ' ');

            String points = "";

            for (int i = 0; i < m_InclusionPoints.Count; i++)
            {
                points = String.Concat(points, m_InclusionPoints[i].X, " ", m_InclusionPoints[i].Y, " ");
            }

            details = m_TopYBoundary + "," + m_BottomYBoundary + "," + m_LeftXBoundary + "," + m_RightXBoundary + "," + points + "," + m_InclusionType + "," + m_Description + "," + timeAdded + "," + timeLastModified + "," + group;

            return details;
        }

        public string[] GetInclusionGroupNames()
        {
            return allGroupNames;
        }

        # endregion

        # region Set methods

        public void SetAllGroupNames(string[] allGroupNames)
        {
            this.allGroupNames = allGroupNames;
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

            for (int i = 0; i < m_InclusionPoints.Count; i++)
            {
                if (m_InclusionPoints[i].X > 0)
                    allBelow0 = false;

                if (m_InclusionPoints[i].X < azimuthResolution)
                    allAboveWidth = false;
            }

            if (allBelow0)
                ShiftAllXPoints(azimuthResolution);
            else if (allAboveWidth)
                ShiftAllXPoints(-azimuthResolution);
        }        

        public void SetPoints(List<Point> points)
        {
            m_InclusionPoints.Clear();

            m_InclusionPoints = points;

            timeLastModified = DateTime.Now;

            calculateXBounds();
            calculateYBounds();
            CalculatePointsString();
        }
    }

}
