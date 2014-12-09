using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Drawing;

namespace BoreholeFeatures
{
    /// <summary>
    /// A class which represents a cluster and all the actions that can be applied to it
    /// 
    /// Author - Terry Malone
    /// Version 1.1 Refactored
    /// </summary>
    [DefaultPropertyAttribute("Description")]
    public class Cluster
    {
        private int azimuthResolution, depthResolution;

        private List<Point> clusterPoints;

        private int leftXBoundary, rightXBoundary;
        private int topYBoundary, bottomYBoundary;
        private int startDepth, endDepth;
        private String description = "";
        private String clusterType = "";
        private string pointsString = "";

        protected string group;
        protected string[] allGroupNames;

        private Boolean smallBubbles, largeBubbles, fineDebris, coarseDebris, diamicton;    //Cluster type properties

        private bool isComplete;

        private DateTime timeAdded, timeLastModified;
        private int sourceStartDepth;

        # region Properties

        #region property grid properties

        [CategoryAttribute("Depth"), DescriptionAttribute("The clusters start depth in millimetres")]
        [DisplayName("Start Depth (mm)")]
        public int StartDepth
        {
            get
            {
                return startDepth;
            }
        }

        [CategoryAttribute("Depth"), DescriptionAttribute("The clusters end depth in millimetres")]
        [DisplayName("End Depth (mm)")]
        public int EndDepth
        {
            get
            {
                return endDepth;
            }
        }

        [Browsable(true)]
        [DefaultValue("entry1")]
        [CategoryAttribute("\tDescription"), DescriptionAttribute("The group this cluster belongs to (To change group display colours go to 'Features>Clusters>Groups'")]
        [TypeConverter(typeof(ClusterGroupConverter))]
        public string Group
        {
            get { return group; }
            set
            {
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

        [CategoryAttribute("\t\tDescription"), DescriptionAttribute("Additional information")]
        public string Description
        {
            get { return description; }
            set 
            { 
                description = value;
                timeLastModified = DateTime.Now;
            }
        }

        [CategoryAttribute("\tCluster Type"), DescriptionAttribute("Does the cluster contain small bubbles?")]
        [DisplayName("Small Bubbles")]
        public bool SmallBubbles
        {
            get { return smallBubbles; }
            set { smallBubbles = value; }
        }

        [CategoryAttribute("\tCluster Type"), DescriptionAttribute("Does the cluster contain large bubbles?")]
        [DisplayName("Large Bubbles")]
        public bool LargeBubbles
        {
            get { return largeBubbles; }
            set { largeBubbles = value; }
        }

        [CategoryAttribute("\tCluster Type"), DescriptionAttribute("Does the cluster contain fine debris?")]
        [DisplayName("Fine Debris")]
        public bool FineDebris
        {
            get { return fineDebris; }
            set { fineDebris = value; }
        }

        [CategoryAttribute("\tCluster Type"), DescriptionAttribute("Does the cluster contain coarse debris?")]
        [DisplayName("Coarse Debris")]
        public bool CoarseDebris
        {
            get { return coarseDebris; }
            set { coarseDebris = value; }
        }

        [CategoryAttribute("\tCluster Type"), DescriptionAttribute("Does the cluster contain debris of varying grain size?")]
        public bool Diamicton
        {
            get { return diamicton; }
            set { diamicton = value; }
        }

        #endregion property grid properties

        public List<Point> Points
        {
            get
            {
                return clusterPoints;
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

        public string ClusterType
        {
            get
            {
                WriteClusterType();
                return clusterType;
            }
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

        public int SourceStartDepth
        {
            get { return sourceStartDepth; }
            set
            {
                sourceStartDepth = value;

                CalculateStartDepthInMM();
                calculateEndDepthInMM();

                CalculateYBounds();
                CalculatePointsString();
            }
        }

        # endregion properties

        #region constructor

        /// <summary>
        /// Constructor method
        /// </summary>
        /// <param name="azimuthResolution">The azimuth resolution of the borehole image</param>
        /// <param name="depthResolution">The depth resolution of the borehole image</param>
        public Cluster(int azimuthResolution, int depthResolution)
        {
            this.azimuthResolution = azimuthResolution;
            this.depthResolution = depthResolution;

            clusterPoints = new List<Point>();

            timeAdded = DateTime.Now;
            timeLastModified = DateTime.Now;
        }

        #endregion constructor

        # region Point operation methods

        /// <summary>
        /// Adds a point to the end of the list of cluster point
        /// </summary>
        /// <param name="newPoint">The point to add</param>
        public void AddPoint(Point newPoint)
        {
            clusterPoints.Add(newPoint);
            timeLastModified = DateTime.Now;

            CalculateXBounds();
            CalculateYBounds();

            CalculatePointsString();
        }

        /// <summary>
        /// Adds a point after the given position
        /// </summary>
        /// <param name="newPoint"></param>
        /// <param name="addAfter"></param>
        public void AddPoint(Point newPoint, int addAfter)
        {
            clusterPoints.Insert(addAfter+1, newPoint);

            timeLastModified = DateTime.Now;

            CalculateXBounds();
            CalculateYBounds();

            CalculatePointsString();
        }

        /// <summary>
        /// Removes a point from the cluster
        /// </summary>
        /// <param name="deletePoint">The point to delete</param>
        public void RemovePoint(Point deletePoint)
        {
            if (clusterPoints.Contains(deletePoint))
            {
                clusterPoints.Remove(deletePoint);

                CheckFeatureIsWithinImageBounds();

                CalculateXBounds();
                CalculateYBounds();

                CalculatePointsString();
                timeLastModified = DateTime.Now;
            }
        }

        /// <summary>
        /// Method which moves one of the points of the Cluster
        /// </summary>
        /// <param name="oldPosition">The current position of the point</param>
        /// <param name="newPosition">The destination of the point</param>
        public void MovePoint(Point oldPosition, Point newPosition)
        {
            int oldPositionIndex;

            if (clusterPoints.Contains(oldPosition))
            {
                oldPositionIndex = clusterPoints.IndexOf(oldPosition);

                clusterPoints.Remove(oldPosition);
                clusterPoints.Insert(oldPositionIndex, newPosition);

                timeLastModified = DateTime.Now;
            }

            CheckFeatureIsWithinImageBounds();

            CalculateXBounds();
            CalculateYBounds();

            CalculatePointsString();
        }

        private void ShiftAllXPoints(int amountToShift)
        {
            for (int i = 0; i < clusterPoints.Count; i++)
            {
                Point currentPoint = clusterPoints[i];
                clusterPoints[i] = new Point(currentPoint.X + amountToShift, currentPoint.Y);
            }

            leftXBoundary += amountToShift;
            rightXBoundary += amountToShift;
        }

        # endregion Point operation methods

        /// <summary>
        /// Moves the cluster a specified amount in the x and y direction
        /// </summary>
        /// <param name="xMoveBy">Amout to move cluster along the x axis</param>
        /// <param name="yMoveBy">Amout to move cluster along the y axis</param>
        public void MoveCluster(int xMoveBy, int yMoveBy)
        {
            Point currentPoint;

            for (int position = 0; position < clusterPoints.Count; position++)
            {
                currentPoint = clusterPoints[position];

                currentPoint.X += xMoveBy;
                currentPoint.Y += yMoveBy;

                clusterPoints[position] = currentPoint;
            }

            CheckFeatureIsWithinImageBounds();

            CalculateXBounds();
            CalculateYBounds();

            CalculatePointsString();

            timeLastModified = DateTime.Now;
        }

        /// <summary>
        /// Calculates the y boundaries of the cluster
        /// </summary>
        private void CalculateYBounds()
        {
            CalculateStartDepthInMM();
            calculateEndDepthInMM();

            for (int i = 1; i < clusterPoints.Count; i++)
            {
                if (clusterPoints[i].Y > bottomYBoundary)
                {
                    bottomYBoundary = clusterPoints[i].Y;

                    calculateEndDepthInMM();
                }

                if (clusterPoints[i].Y < topYBoundary)
                {
                    topYBoundary = clusterPoints[i].Y;

                    CalculateStartDepthInMM();
                }
            }
        }

        /// <summary>
        /// Calculates the x boundaries of the cluster
        /// </summary>
        private void CalculateXBounds()
        {
            if (clusterPoints.Count > 0)
            {
                leftXBoundary = clusterPoints[0].X;
                rightXBoundary = clusterPoints[0].X;
                topYBoundary = clusterPoints[0].Y;
                bottomYBoundary = clusterPoints[0].Y;

                

                for (int i = 1; i < clusterPoints.Count; i++)
                {
                    if (clusterPoints[i].X > rightXBoundary)
                        rightXBoundary = clusterPoints[i].X;

                    if (clusterPoints[i].X < leftXBoundary)
                        leftXBoundary = clusterPoints[i].X;

                    if (clusterPoints[i].Y > bottomYBoundary)
                    {
                        bottomYBoundary = clusterPoints[i].Y;

                        calculateEndDepthInMM();
                    }

                    if (clusterPoints[i].Y < topYBoundary)
                    {
                        topYBoundary = clusterPoints[i].Y;

                        CalculateStartDepthInMM();

                    }
                }
            }
        }

        /// <summary>
        /// Calculates the start depth of the cluster in millimetres
        /// </summary>
        private void CalculateStartDepthInMM()
        {
            startDepth = (int)((double)sourceStartDepth + (double)((double)topYBoundary * (double)depthResolution));
        }

        /// <summary>
        /// Calculates the end depth of the cluster in millimetres
        /// </summary>
        private void calculateEndDepthInMM()
        {
            endDepth = (int)((double)sourceStartDepth + (double)((double)bottomYBoundary * (double)depthResolution));
        }

        /// <summary>
        /// Converts the list of points to a string
        /// </summary>
        private void CalculatePointsString()
        {
            if (clusterPoints.Count > 0)
            {

                pointsString = "";
                int xPoint, yPoint;

                for (int i = 0; i < clusterPoints.Count; i++)
                {
                    xPoint = clusterPoints[i].X;
                    yPoint = clusterPoints[i].Y;

                    pointsString = String.Concat(pointsString, "(", xPoint, ", ", yPoint, ") ");
                }
            }
        }

        /// <summary>
        /// Creates a string of the type of cluster
        /// </summary>
        private void WriteClusterType()
        {
            clusterType = "";

            if (smallBubbles == true)
                clusterType = String.Concat(clusterType, "smallBubbles ");

            if (largeBubbles == true)
                clusterType = String.Concat(clusterType, "largeBubbles ");

            if (fineDebris == true)
                clusterType = String.Concat(clusterType, "fineDebris ");

            if (coarseDebris == true)
                clusterType = String.Concat(clusterType, "coarseDebris ");

            if (diamicton == true)
                clusterType = String.Concat(clusterType, "diamicton ");
        }

        # region get methods

        /// <summary>
        /// Returns text representing all of the clusters properties
        /// topYBoundary        - top Y boundary in pixels
        /// bottomYBoundary     - bottom Y boundary in pixels
        /// leftXBoundary       - X position of left boundary in pixels
        /// rightXBoundary      - X position of right boundary in pixels
        /// points              - List of cluster points with commas removed
        /// clusterType         - List of contents of cluster (space separated)
        /// Description         - Description of cluster
        /// Time added          - Time the cluster was first added
        /// Time last modified  - Time the cluster was last modified
        /// </summary>
        /// <returns>The cluster properties</returns>
        public String GetDetails()
        {
            CalculatePointsString();

            description = description.Replace(',', ' ');

            WriteClusterType();

            String points = "";

            for (int i = 0; i < clusterPoints.Count; i++)
            {
                points = String.Concat(points, clusterPoints[i].X, " ", clusterPoints[i].Y, " ");
            }

            String details;

            details = topYBoundary + "," + bottomYBoundary + "," + leftXBoundary + "," + rightXBoundary + "," + points + "," + clusterType + "," + description + "," + timeAdded + "," + timeLastModified + "," + group;

            return details;
        }

        public string[] GetClusterGroupNames()
        {
            return allGroupNames;
        }

        # endregion

        # region set methods

        public void SetAllGroupNames(string[] allGroupNames)
        {
            this.allGroupNames = allGroupNames;
        }

        # endregion

        /// <summary>
        /// Checks that image is not completely below 0 or completely above boreholeWidth
        /// This could happen due to rotation
        /// </summary>
        public void CheckFeatureIsWithinImageBounds()
        {
            bool allBelow0 = true;
            bool allAboveWidth = true;

            for (int i = 0; i < clusterPoints.Count; i++)
            {
                if (clusterPoints[i].X > 0)
                    allBelow0 = false;

                if (clusterPoints[i].X < azimuthResolution)
                    allAboveWidth = false;
            }

            if (allBelow0)
                ShiftAllXPoints(azimuthResolution);
            else if (allAboveWidth)
                ShiftAllXPoints(-azimuthResolution);
            
        }
    } 

}
