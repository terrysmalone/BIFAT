using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace BoreholeFeatures
{
    /// <summary>
    /// A class which represents a cluster and all the actions that can be applied to it
    /// 
    /// Author - Terry Malone
    /// Version 1.1 Refactored
    /// </summary>
    public sealed class Cluster
    {
        private readonly int m_azimuthResolution;
        private readonly int m_depthResolution;

        private string m_description = "";

        private string m_clusterType = "";

        private string m_pointsString = "";

        private string[] m_allGroupNames;

        private int m_sourceStartDepth;

        private  ClusterProperties m_clusterProperties;

        #region Properties
        
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

        #region cluster properties

        public bool SmallBubbles
        {
            get => m_clusterProperties.HasFlag(ClusterProperties.SmallBubbles);

            set
            {
                if (value) m_clusterProperties |= ClusterProperties.SmallBubbles;
                else m_clusterProperties &= ~ClusterProperties.SmallBubbles;
            }
        }

        public bool LargeBubbles
        {
            get => m_clusterProperties.HasFlag(ClusterProperties.LargeBubbles);

            set
            {
                if (value) m_clusterProperties |= ClusterProperties.LargeBubbles;
                else m_clusterProperties &= ~ClusterProperties.LargeBubbles;
            }
        }

        
        public bool FineDebris
        {
            get => m_clusterProperties.HasFlag(ClusterProperties.FineDebris);

            set
            {
                if (value) m_clusterProperties |= ClusterProperties.FineDebris;
                else m_clusterProperties &= ~ClusterProperties.FineDebris;
            }
        }

        public bool CoarseDebris
        {
            get => m_clusterProperties.HasFlag(ClusterProperties.CoarseDebris);

            set
            {
                if (value) m_clusterProperties |= ClusterProperties.CoarseDebris;
                else m_clusterProperties &= ~ClusterProperties.CoarseDebris;
            }
        }

        public bool Diamicton
        {
            get => m_clusterProperties.HasFlag(ClusterProperties.Diamicton);

            set
            {
                if (value) m_clusterProperties |= ClusterProperties.Diamicton;
                else m_clusterProperties &= ~ClusterProperties.Diamicton;
            }
        }

        #endregion cluster properties

        public List<Point> Points { get; }

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

        public string ClusterType
        {
            get
            {
                WriteClusterType();
                return m_clusterType;
            }
        }

        /// <summary>
        /// The time the cluster was added
        /// </summary>
        public DateTime TimeAdded { get; set; }

        /// <summary>
        /// The time the cluster was last modified
        /// </summary>
        public DateTime TimeLastModified { get; set; }

        public int SourceStartDepth
        {
            get => m_sourceStartDepth;

            set
            {
                m_sourceStartDepth = value;

                CalculateStartDepthInMm();
                CalculateEndDepthInMm();

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
            m_azimuthResolution = azimuthResolution;
            m_depthResolution = depthResolution;

            Points = new List<Point>();

            TimeAdded = DateTime.Now;
            TimeLastModified = DateTime.Now;
        }

        #endregion constructor

        # region Point operation methods

        /// <summary>
        /// Adds a point to the end of the list of cluster point
        /// </summary>
        /// <param name="newPoint">The point to add</param>
        public void AddPoint(Point newPoint)
        {
            Points.Add(newPoint);
            TimeLastModified = DateTime.Now;

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
            Points.Insert(addAfter+1, newPoint);

            TimeLastModified = DateTime.Now;

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
            if (!Points.Contains(deletePoint)) return;

            Points.Remove(deletePoint);

            CheckFeatureIsWithinImageBounds();

            CalculateXBounds();
            CalculateYBounds();

            CalculatePointsString();
            TimeLastModified = DateTime.Now;
        }

        /// <summary>
        /// Method which moves one of the points of the Cluster
        /// </summary>
        /// <param name="oldPosition">The current position of the point</param>
        /// <param name="newPosition">The destination of the point</param>
        public void MovePoint(Point oldPosition, Point newPosition)
        {
            if (Points.Contains(oldPosition))
            {
                var oldPositionIndex = Points.IndexOf(oldPosition);

                Points.Remove(oldPosition);
                Points.Insert(oldPositionIndex, newPosition);

                TimeLastModified = DateTime.Now;
            }

            CheckFeatureIsWithinImageBounds();

            CalculateXBounds();
            CalculateYBounds();

            CalculatePointsString();
        }

        # endregion Point operation methods

        /// <summary>
        /// Moves the cluster a specified amount in the x and y direction
        /// </summary>
        /// <param name="xMoveBy">Amout to move cluster along the x axis</param>
        /// <param name="yMoveBy">Amout to move cluster along the y axis</param>
        public void MoveCluster(int xMoveBy, int yMoveBy)
        {
            for (var position = 0; position < Points.Count; position++)
            {
                var currentPoint = Points[position];

                currentPoint.X += xMoveBy;
                currentPoint.Y += yMoveBy;

                Points[position] = currentPoint;
            }

            CheckFeatureIsWithinImageBounds();

            CalculateXBounds();
            CalculateYBounds();

            CalculatePointsString();

            TimeLastModified = DateTime.Now;
        }

        /// <summary>
        /// Calculates the y boundaries of the cluster
        /// </summary>
        private void CalculateYBounds()
        {
            CalculateStartDepthInMm();
            CalculateEndDepthInMm();

            for (var i = 1; i < Points.Count; i++)
            {
                if (Points[i].Y > BottomYBoundary)
                {
                    BottomYBoundary = Points[i].Y;

                    CalculateEndDepthInMm();
                }

                if (Points[i].Y >= TopYBoundary) continue;

                TopYBoundary = Points[i].Y;

                CalculateStartDepthInMm();
            }
        }

        /// <summary>
        /// Calculates the x boundaries of the cluster
        /// </summary>
        private void CalculateXBounds()
        {
            if (Points.Count <= 0) return;

            LeftXBoundary = Points[0].X;

            RightXBoundary = Points[0].X;

            TopYBoundary = Points[0].Y;

            BottomYBoundary = Points[0].Y;
            

            for (var i = 1; i < Points.Count; i++)
            {
                if (Points[i].X > RightXBoundary)
                    RightXBoundary = Points[i].X;

                if (Points[i].X < LeftXBoundary)
                    LeftXBoundary = Points[i].X;

                if (Points[i].Y > BottomYBoundary)
                {
                    BottomYBoundary = Points[i].Y;

                    CalculateEndDepthInMm();
                }

                if (Points[i].Y >= TopYBoundary) continue;

                TopYBoundary = Points[i].Y;

                CalculateStartDepthInMm();
            }
        }

        /// <summary>
        /// Calculates the start depth of the cluster in millimetres
        /// </summary>
        private void CalculateStartDepthInMm()
        {
            StartDepth = Convert.ToInt32(m_sourceStartDepth + TopYBoundary * (double)m_depthResolution);
        }

        /// <summary>
        /// Calculates the end depth of the cluster in millimetres
        /// </summary>
        private void CalculateEndDepthInMm()
        {
            EndDepth = Convert.ToInt32(m_sourceStartDepth + BottomYBoundary * (double)m_depthResolution);
        }

        /// <summary>
        /// Converts the list of points to a string
        /// </summary>
        private void CalculatePointsString()
        {
            if (Points.Count > 0)
            {
                m_pointsString = "";

                foreach (var clusterPoint in Points)
                {
                    var xPoint = clusterPoint.X;
                    var yPoint = clusterPoint.Y;

                    m_pointsString = string.Concat(m_pointsString, "(", xPoint, ", ", yPoint, ") ");
                }
            }
        }

        /// <summary>
        /// Creates a string of the type of cluster
        /// </summary>
        private void WriteClusterType()
        {
            m_clusterType = "";

            if (SmallBubbles)
                m_clusterType = string.Concat(m_clusterType, "smallBubbles ");

            if (LargeBubbles)
                m_clusterType = string.Concat(m_clusterType, "largeBubbles ");

            if (FineDebris)
                m_clusterType = string.Concat(m_clusterType, "fineDebris ");

            if (CoarseDebris)
                m_clusterType = string.Concat(m_clusterType, "coarseDebris ");

            if (Diamicton)
                m_clusterType = string.Concat(m_clusterType, "diamicton ");
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
        public string GetDetails()
        {
            CalculatePointsString();

            m_description = m_description.Replace(',', ' ');

            WriteClusterType();

            var points = Points.Aggregate("", (current, t) => string.Concat(current, t.X, " ", t.Y, " "));

            var details = TopYBoundary + "," 
                          + BottomYBoundary + "," 
                          + LeftXBoundary + "," 
                          + RightXBoundary + "," 
                          + points + "," 
                          + m_clusterType + "," 
                          + m_description + "," 
                          + TimeAdded + "," 
                          + TimeLastModified + "," 
                          + Group;

            return details;
        }

        public string[] GetClusterGroupNames()
        {
            return m_allGroupNames;
        }

        # endregion

        # region set methods

        public void SetAllGroupNames(string[] allGroupNames)
        {
            m_allGroupNames = allGroupNames;
        }

        # endregion

        /// <summary>
        /// Checks that image is not completely below 0 or completely above boreholeWidth
        /// This could happen due to rotation
        /// </summary>
        public void CheckFeatureIsWithinImageBounds()
        {
            var allBelow0 = true;
            var allAboveWidth = true;

            foreach (var point in Points)
            {
                if (point.X > 0)
                {
                    allBelow0 = false;
                }

                if (point.X < m_azimuthResolution)
                {
                    allAboveWidth = false;
                }
            }

            if (allBelow0)
            {
                ShiftAllXPoints(m_azimuthResolution);
            }
            else if (allAboveWidth)
            {
                ShiftAllXPoints(-m_azimuthResolution);
            }
        }

        private void ShiftAllXPoints(int amountToShift)
        {
            for (var i = 0; i < Points.Count; i++)
            {
                var currentPoint = Points[i];
                Points[i] = new Point(currentPoint.X + amountToShift, 
                                      currentPoint.Y);
            }

            LeftXBoundary += amountToShift;
            RightXBoundary += amountToShift;
        }
    } 
}
