using System;
using System.Collections.Generic;
using System.Drawing;

namespace BoreholeFeatures
{
    /// <summary>
    /// Represents a sinusoidal layer and all the actions that can be applied to it
    /// </summary>
    public abstract class Layer
    {
        // ReSharper disable InconsistentNaming
        protected int m_SourceAzimuthResolution;
        protected int m_DepthResolution;

        protected int m_LayerStartY;
        protected int m_LayerEndY;

        protected int m_SourceStartDepth;
        protected int m_SourceEndDepth;
        protected string m_Description = "";
        protected string m_LayerType = "";
        protected int m_Quality = 4;
        
        protected int m_TopDepthPixels;
        
        protected int m_BottomDepthPixels;

        protected int m_FirstEdge = 1;
        protected int m_SecondEdge = 2;
        protected int m_BothEdges = 3;

        protected DateTime m_TimeAdded;
        protected DateTime m_TimeLastModified;

        protected string[] m_AllGroupNames;

        private LayerProperties m_LayerProperties;

        // ReSharper restore InconsistentNaming

        #region properties

        public int StartDepth => (int)(m_LayerStartY * (double)m_DepthResolution + m_SourceStartDepth);
        
        public int EndDepth => (int)((m_LayerEndY * (double)m_DepthResolution) + m_SourceStartDepth);
        
        public string Group { get; set; }
        
        public int Quality
        {
            get => m_Quality;

            set
            {
                m_Quality = value;
                m_TimeLastModified = DateTime.Now; 
            }

        }

        public string Description
        {
            get => m_Description;

            set 
            { 
                m_Description = value;
                m_TimeLastModified = DateTime.Now;
            }
        }

        #region Layer properties

        public bool LayerVoid
        {
            get => m_LayerProperties.HasFlag(LayerProperties.LayerVoid);
            set
            {
                if(value) m_LayerProperties |= LayerProperties.LayerVoid; 
                else m_LayerProperties &= ~LayerProperties.LayerVoid;
            }
        }

        public bool Clean
        {
            get => m_LayerProperties.HasFlag(LayerProperties.Clean);
            set
            {
                if (value) m_LayerProperties |= LayerProperties.Clean;
                else m_LayerProperties &= ~LayerProperties.Clean;
            }
        }

        public bool SmallBubbles
        {
            get => m_LayerProperties.HasFlag(LayerProperties.SmallBubbles);
            set
            {
                if (value) m_LayerProperties |= LayerProperties.SmallBubbles;
                else m_LayerProperties &= ~LayerProperties.SmallBubbles;
            }
        }
        
        public bool LargeBubbles
        {
            get => m_LayerProperties.HasFlag(LayerProperties.LargeBubbles);
            set
            {
                if (value) m_LayerProperties |= LayerProperties.LargeBubbles;
                else m_LayerProperties &= ~LayerProperties.LargeBubbles;
            }
        }
        
        public bool FineDebris
        {
            get => m_LayerProperties.HasFlag(LayerProperties.FineDebris);
            set
            {
                if (value) m_LayerProperties |= LayerProperties.FineDebris;
                else m_LayerProperties &= ~LayerProperties.FineDebris;
            }
        }
       
        public bool CoarseDebris
        {
            get => m_LayerProperties.HasFlag(LayerProperties.CoarseDebris);
            set
            {
                if (value) m_LayerProperties |= LayerProperties.CoarseDebris;
                else m_LayerProperties &= ~LayerProperties.CoarseDebris;
            }
        }

        public bool Diamicton
        {
            get => m_LayerProperties.HasFlag(LayerProperties.Diamicton);
            set
            {
                if (value) m_LayerProperties |= LayerProperties.Diamicton;
                else m_LayerProperties &= ~LayerProperties.Diamicton;
            }
        }

        #endregion Layer properties

        /// <summary>
        /// Returns the depth of the layers top edge
        /// </summary>
        /// <returns>The depth of the top sine</returns>
        public int TopEdgeDepth => m_TopDepthPixels;

        /// <summary>
        /// The depth of the layers bottom edge
        /// </summary>
        public int BottomEdgeDepth => m_BottomDepthPixels;

        /// <summary>
        /// Returns the depth in millimetres of the top sine of the layer
        /// </summary>
        /// <returns>The depth in millimetres of the top sine</returns>
        public int TopEdgeDepthMm => Convert.ToInt32(m_TopDepthPixels 
                                                     * (double)m_DepthResolution 
                                                     + m_SourceStartDepth);

        /// <summary>
        /// Returns the depth in millimetres of the bottom sine of the layer
        /// </summary>
        /// <returns>The depth in millimetres of the bottom sine</returns>
        public int BottomEdgeDepthMm => Convert.ToInt32(m_BottomDepthPixels 
                                                        * (double)m_DepthResolution 
                                                        + m_SourceStartDepth);

        /// <summary>
        /// The layer's lowest y-point
        /// </summary>
        public int StartY => m_LayerStartY;

        /// <summary>
        /// The layer's highest y-point
        /// </summary>
        public int EndY => m_LayerEndY;

        /// <summary>
        /// The borehole's start depth
        /// </summary>
        public int SourceStartDepth => m_SourceStartDepth;

        /// <summary>
        /// The borehole's end depth
        /// </summary>
        public int SourceEndDepth => m_SourceEndDepth;

        /// <summary>
        /// The layer's type
        /// </summary>
        public string LayerType
        {
            get
            {
                WriteLayerProperties();
                return m_LayerType;
            }
        }

        /// <summary>
        /// The time the layer was added
        /// </summary>
        public DateTime TimeAdded
        {
            get => m_TimeAdded;
            set => m_TimeAdded = value;
        }

        /// <summary>
        /// The time the layer was last modified
        /// </summary>
        public DateTime TimeLastModified
        {
            get => m_TimeLastModified;
            set => m_TimeLastModified = value;
        }
        
       
        //  The azimuth resolution (number of pixels horizontally) of the borehole image
        public int SourceAzimuthResolution => m_SourceAzimuthResolution;

        // The depth resolution (mm per pixel) of the borehole image
        public int SourceDepthResolution
        {
            get => m_DepthResolution;

            set
            {
                m_DepthResolution = value;

                CalculateStartY();
                CalculateEndY();
            }
        }

        #endregion properties


        /// <summary>
        /// Calculates the highest Y-point (lowest value) in the layer
        /// </summary>
        public void CalculateStartY()
        {
            var topEdgePoints = GetTopEdgePoints();
            
            m_LayerStartY = topEdgePoints[0].Y;

            for (var sinePointPos = 1; sinePointPos < topEdgePoints.Count; sinePointPos++)
            {
                var currentYPoint = topEdgePoints[sinePointPos].Y;

                if (currentYPoint < m_LayerStartY)
                    m_LayerStartY = currentYPoint;
            }
        }

        /// <summary>
        /// Calculates the lowest Y-point (hisghest value) in the layer
        /// </summary>
        public void CalculateEndY()
        {
            var bottomEdgePoints = GetBottomEdgePoints();

            m_LayerEndY = bottomEdgePoints[0].Y;

            for (var sinePointPos = 1; sinePointPos < bottomEdgePoints.Count; sinePointPos++)
            {
                var currentYPoint = bottomEdgePoints[sinePointPos].Y;

                if (currentYPoint > m_LayerEndY)
                {
                    m_LayerEndY = currentYPoint;
                }
            }
        }

        public abstract void MoveEdge(int edgeToMove, int xMoveBy, int yMoveBy);

        /// <summary>
        /// Method which turns the layer type into a string from the selected bool variables
        /// </summary>
        internal void WriteLayerProperties()
        {
            m_LayerType = "";

            if(m_LayerProperties.HasFlag(LayerProperties.LayerVoid))
            {
                m_LayerType = string.Concat(m_LayerType, "void ");
            }

            if (m_LayerProperties.HasFlag(LayerProperties.Clean))
            {
                m_LayerType = string.Concat(m_LayerType, "clean ");
            }

            if(m_LayerProperties.HasFlag(LayerProperties.SmallBubbles))
            {
                m_LayerType = string.Concat(m_LayerType, "smallBubbles ");
            }

            if (m_LayerProperties.HasFlag(LayerProperties.LargeBubbles))
            {
                m_LayerType = string.Concat(m_LayerType, "largeBubbles ");
            }

            if (m_LayerProperties.HasFlag(LayerProperties.FineDebris))
            {
                m_LayerType = string.Concat(m_LayerType, "fineDebris ");
            }

            if (m_LayerProperties.HasFlag(LayerProperties.CoarseDebris))
            {
                m_LayerType = string.Concat(m_LayerType, "coarseDebris ");
            }

            if (m_LayerProperties.HasFlag(LayerProperties.Diamicton))
            {
                m_LayerType = string.Concat(m_LayerType, "diamicton ");
            }
        }

        # region get methods

        /// <summary>
        /// Returns the points of the layers first edge
        /// </summary>
        /// <returns>A List of the points along the first edge</returns>
        public abstract List<Point> GetTopEdgePoints();

        /// <summary>
        /// Returns the points of the layers second edge
        /// </summary>
        /// <returns>A List of the points along the second edge</returns>
        public abstract List<Point> GetBottomEdgePoints();

        public abstract int GetTopYPoint(int xPoint);

        public abstract int GetBottomYPoint(int xPoint);

        public abstract string GetDetails();

        /// <summary>
        /// Returns how many visible edges the layer has
        /// </summary>
        /// <returns></returns>
        public abstract int GetNumOfEdges();        

        public string[] GetLayerGroupNames()
        {
            return m_AllGroupNames;
        }

        # endregion

        # region set methods
        
        /// <summary>
        /// Sets the start depth of the borehole
        /// </summary>
        public void SetBoreholeStartDepth(int sourceStart)
        {
            m_SourceStartDepth = sourceStart;

            CalculateStartY();
            CalculateEndY();
        }

        /// <summary>
        /// Sets the end depth of the borehole
        /// </summary>
        public void SetBoreholeEndDepth(int endDepth)
        {
            m_SourceEndDepth = endDepth;
        }
        
        public void SetAllGroupNames(string[] groupNames)
        {
            m_AllGroupNames = groupNames;
        }

        # endregion          
    }

}
