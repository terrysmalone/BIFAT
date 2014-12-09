using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Drawing;

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

        private List<Point> inclusionPoints;

        private int leftXBoundary, rightXBoundary;
        private int topYBoundary, bottomYBoundary;
        private int startDepth, endDepth;
        private String description = "";
        private String inclusionType = "";
        private string pointsString = "";

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
                return startDepth;
            }
        }

        [CategoryAttribute("Depth"), DescriptionAttribute("The clusters end depth in millimetres")]
        [DisplayName("End Depth")]
        public int EndDepth
        {
            get
            {
                return endDepth;
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
                return pointsString;
            }
        }

        [CategoryAttribute("\tDescription"), DescriptionAttribute("Additional information")]
        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        [Browsable(true)]
        [DefaultValue("entry1")]
        [CategoryAttribute("\tDescription"), DescriptionAttribute("The type of inclusion")]
        [TypeConverter(typeof(InclusionTypeConverter))]
        [DisplayName("Inclusion Type")]
        public string InclusionType
        {
            get { return inclusionType; }
            set { inclusionType = value; }
        }

        # endregion property grid methods

        public List<Point> Points
        {
            get
            {
                return inclusionPoints;
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
            get { return leftXBoundary; }
            set { leftXBoundary = value; }
        }

        /// <summary>
        /// The left boundary of the cluster
        /// </summary>
        public int RightXBoundary
        {
            get { return rightXBoundary; }
            set { rightXBoundary = value; }
        }

        /// <summary>
        /// The y position of the top boundary of the cluster
        /// </summary>
        public int TopYBoundary
        {
            get { return topYBoundary; }
            set { topYBoundary = value; }
        }

        /// <summary>
        /// The bottom boundary of the cluster
        /// </summary>
        public int BottomYBoundary
        {
            get { return bottomYBoundary; }
            set { bottomYBoundary = value; }
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
            leftXBoundary = startX;
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

            inclusionPoints = new List<Point>();

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
            inclusionPoints.Add(newPoint);

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
            inclusionPoints.Insert(addAfter+1, newPoint);

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
            if (inclusionPoints.Contains(deletePoint))
            {
                inclusionPoints.Remove(deletePoint);

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

            if (inclusionPoints.Contains(oldPosition))
            {
                index = inclusionPoints.IndexOf(oldPosition);

                inclusionPoints.Remove(oldPosition);
                inclusionPoints.Insert(index, newPosition);

                timeLastModified = DateTime.Now;
            }

            CheckFeatureIsWithinImageBounds();

            calculateXBounds();
            calculateYBounds();
        }

        private void ShiftAllXPoints(int amountToShift)
        {
            for (int i = 0; i < inclusionPoints.Count; i++)
            {
                Point currentPoint = inclusionPoints[i];
                inclusionPoints[i] = new Point(currentPoint.X + amountToShift, currentPoint.Y);
            }

            leftXBoundary += amountToShift;
            rightXBoundary += amountToShift;
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

            for (int i = 0; i < inclusionPoints.Count; i++)
            {
                currentPoint = inclusionPoints[i];

                currentPoint.X += xMoveBy;
                currentPoint.Y += yMoveBy;

                inclusionPoints[i] = currentPoint;
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
            startDepth = (int)((double)sourceStartDepth + (double)((double)topYBoundary * (double)depthResolution));
        }

        /// <summary>
        /// Calculates the end depth of the inclusion in millimetres
        /// </summary>
        private void CalculateEndDepthInMM()
        {
            endDepth = (int)((double)sourceStartDepth + (double)((double)bottomYBoundary * (double)depthResolution));
        }

        /// <summary>
        /// Calculates the y boundaries of the cluster
        /// </summary>
        private void calculateYBounds()
        {
            CalculateStartDepthInMM();
            CalculateEndDepthInMM();

            for (int i = 1; i < inclusionPoints.Count; i++)
            {
                if (inclusionPoints[i].Y > bottomYBoundary)
                {
                    bottomYBoundary = inclusionPoints[i].Y;

                    CalculateEndDepthInMM();
                }

                if (inclusionPoints[i].Y < topYBoundary)
                {
                    topYBoundary = inclusionPoints[i].Y;

                    CalculateStartDepthInMM();
                }
            }
        }

        /// <summary>
        /// Calculates the x boundaries of the cluster
        /// </summary>
        private void calculateXBounds()
        {
            if (inclusionPoints.Count > 0)
            {
                leftXBoundary = inclusionPoints[0].X;
                rightXBoundary = inclusionPoints[0].X;
                topYBoundary = inclusionPoints[0].Y;
                bottomYBoundary = inclusionPoints[0].Y;



                for (int i = 1; i < inclusionPoints.Count; i++)
                {
                    if (inclusionPoints[i].X > rightXBoundary)
                        rightXBoundary = inclusionPoints[i].X;

                    if (inclusionPoints[i].X < leftXBoundary)
                        leftXBoundary = inclusionPoints[i].X;

                    if (inclusionPoints[i].Y > bottomYBoundary)
                    {
                        bottomYBoundary = inclusionPoints[i].Y;

                        CalculateEndDepthInMM();
                    }

                    if (inclusionPoints[i].Y < topYBoundary)
                    {
                        topYBoundary = inclusionPoints[i].Y;

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
            if (inclusionPoints.Count > 0)
            {
                pointsString = "";
                int xPoint, yPoint;

                for (int i = 0; i < inclusionPoints.Count; i++)
                {
                    xPoint = inclusionPoints[i].X;
                    yPoint = inclusionPoints[i].Y;

                    pointsString = String.Concat(pointsString, "(", xPoint, ", ", yPoint, ") ");
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

            description = description.Replace(',', ' ');

            String points = "";

            for (int i = 0; i < inclusionPoints.Count; i++)
            {
                points = String.Concat(points, inclusionPoints[i].X, " ", inclusionPoints[i].Y, " ");
            }

            details = topYBoundary + "," + bottomYBoundary + "," + leftXBoundary + "," + rightXBoundary + "," + points + "," + inclusionType + "," + description + "," + timeAdded + "," + timeLastModified + "," + group;

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

            for (int i = 0; i < inclusionPoints.Count; i++)
            {
                if (inclusionPoints[i].X > 0)
                    allBelow0 = false;

                if (inclusionPoints[i].X < azimuthResolution)
                    allAboveWidth = false;
            }

            if (allBelow0)
                ShiftAllXPoints(azimuthResolution);
            else if (allAboveWidth)
                ShiftAllXPoints(-azimuthResolution);
        }        

        public void SetPoints(List<Point> points)
        {
            inclusionPoints.Clear();

            inclusionPoints = points;

            timeLastModified = DateTime.Now;

            calculateXBounds();
            calculateYBounds();
            CalculatePointsString();
        }
    }

}
