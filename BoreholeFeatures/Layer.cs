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
        protected int sourceAzimuthResolution;
        protected int depthResolution;

        protected int layerStartY, layerEndY;
        protected int sourceStartDepth, sourceEndDepth;
        protected string description = "";
        protected string layerType = "";
        protected int quality = 4;

        //Borehole Layer
        //Top sine attributes
        protected int topAzimuth, topAmplitude;
        protected int topDepthPixels;
        
        //Bottom sine attributes
        protected int bottomAzimuth, bottomAmplitude;
        protected int bottomDepthPixels;

        //Core Layer
        //Top line 
        protected int topEdgeIntercept;
        protected double topEdgeSlope;

        //Bottom line
        protected int bottomEdgeIntercept;
        protected double bottomEdgeSlope;

        protected int FIRST_EDGE = 1;
        protected int SECOND_EDGE = 2;
        protected int BOTH_EDGES = 3;
        
        private LayerProperties m_layerProperties;

        protected DateTime timeAdded, timeLastModified;
        
        protected string[] allGroupNames;
        
        #region properties
        
        public int StartDepth => (int)(layerStartY * (double)depthResolution + sourceStartDepth);
        
        public int EndDepth => (int)((layerEndY * (double)depthResolution) + sourceStartDepth);
        
        public string Group { get; set; }
        
        public int Quality
        {
            get => quality;

            set
            {
                quality = value;
                timeLastModified = DateTime.Now; 
            }

        }

        public string Description
        {
            get => description;

            set 
            { 
                description = value;
                timeLastModified = DateTime.Now;
            }
        }

        #region Layer properties

        public bool LayerVoid
        {
            get => m_layerProperties.HasFlag(LayerProperties.LayerVoid);
            set
            {
                if(value) m_layerProperties |= LayerProperties.LayerVoid; 
                else m_layerProperties &= ~LayerProperties.LayerVoid;
            }
        }

        public bool Clean
        {
            get => m_layerProperties.HasFlag(LayerProperties.Clean);
            set
            {
                if (value) m_layerProperties |= LayerProperties.Clean;
                else m_layerProperties &= ~LayerProperties.Clean;
            }
        }

        public bool SmallBubbles
        {
            get => m_layerProperties.HasFlag(LayerProperties.SmallBubbles);
            set
            {
                if (value) m_layerProperties |= LayerProperties.SmallBubbles;
                else m_layerProperties &= ~LayerProperties.SmallBubbles;
            }
        }
        
        public bool LargeBubbles
        {
            get => m_layerProperties.HasFlag(LayerProperties.LargeBubbles);
            set
            {
                if (value) m_layerProperties |= LayerProperties.LargeBubbles;
                else m_layerProperties &= ~LayerProperties.LargeBubbles;
            }
        }
        
        public bool FineDebris
        {
            get => m_layerProperties.HasFlag(LayerProperties.FineDebris);
            set
            {
                if (value) m_layerProperties |= LayerProperties.FineDebris;
                else m_layerProperties &= ~LayerProperties.FineDebris;
            }
        }
       
        public bool CoarseDebris
        {
            get => m_layerProperties.HasFlag(LayerProperties.CoarseDebris);
            set
            {
                if (value) m_layerProperties |= LayerProperties.CoarseDebris;
                else m_layerProperties &= ~LayerProperties.CoarseDebris;
            }
        }

        public bool Diamicton
        {
            get => m_layerProperties.HasFlag(LayerProperties.Diamicton);
            set
            {
                if (value) m_layerProperties |= LayerProperties.Diamicton;
                else m_layerProperties &= ~LayerProperties.Diamicton;
            }
        }

        #endregion Layer properties

        /// <summary>
        /// Returns the depth of the layers top edge
        /// </summary>
        /// <returns>The depth of the top sine</returns>
        public int TopEdgeDepth => topDepthPixels;

        /// <summary>
        /// The depth of the layers bottom edge
        /// </summary>
        public int BottomEdgeDepth => bottomDepthPixels;

        /// <summary>
        /// Returns the depth in millimetres of the top sine of the layer
        /// </summary>
        /// <returns>The depth in millimetres of the top sine</returns>
        public int TopEdgeDepthMm => Convert.ToInt32(topDepthPixels 
                                                     * (double)depthResolution 
                                                     + sourceStartDepth);

        /// <summary>
        /// Returns the depth in millimetres of the bottom sine of the layer
        /// </summary>
        /// <returns>The depth in millimetres of the bottom sine</returns>
        public int BottomEdgeDepthMm => Convert.ToInt32(bottomDepthPixels 
                                                        * (double)depthResolution 
                                                        + sourceStartDepth);

        /// <summary>
        /// Returns the amplitude of the top edge (lowest y co-ordinates) in mm
        /// </summary>
        /// <returns>The top edges amplitude</returns>
        public int TopSineAmplitudeInMm => (int)(topAmplitude * (double)depthResolution);

        /// <summary>
        /// The azimuth of the top sine (lowest y co-ordinates)
        /// </summary>
        public int TopSineAzimuth => topAzimuth;

        /// <summary>
        /// The azimuth of the bottom sine (highest y co-ordinates)
        /// </summary>
        public int BottomSineAzimuth => bottomAzimuth;

        /// <summary>
        /// The amplitude of the top sine (lowest y co-ordinates)
        /// </summary>
        public int TopSineAmplitude => topAmplitude;


        /// <summary>
        /// The amplitude of the bottom edge (highest y co-ordinates)
        /// </summary>
        public int BottomSineAmplitude => bottomAmplitude;

        /// <summary>
        /// The amplitude of the bottom edge (highest y co-ordinates)
        /// </summary>
        public int BottomSineAmplitudeInMm => (int)(bottomAmplitude * (double)depthResolution);

        public double TopEdgeSlope => topEdgeSlope;

        public int TopEdgeIntercept => topEdgeIntercept;

        public double BottomEdgeSlope => bottomEdgeSlope;

        public int BottomEdgeIntercept => bottomEdgeIntercept;

        /// <summary>
        /// The layer's lowest y-point
        /// </summary>
        public int StartY => layerStartY;

        /// <summary>
        /// The layer's highest y-point
        /// </summary>
        public int EndY => layerEndY;

        /// <summary>
        /// The borehole's start depth
        /// </summary>
        public int SourceStartDepth => sourceStartDepth;

        /// <summary>
        /// The borehole's end depth
        /// </summary>
        public int SourceEndDepth => sourceEndDepth;

        /// <summary>
        /// The layer's type
        /// </summary>
        public string LayerType
        {
            get
            {
                WriteLayerProperties();
                return layerType;
            }
        }

        /// <summary>
        /// The time the layer was added
        /// </summary>
        public DateTime TimeAdded
        {
            get => timeAdded;
            set => timeAdded = value;
        }

        /// <summary>
        /// The time the layer was last modified
        /// </summary>
        public DateTime TimeLastModified
        {
            get => timeLastModified;
            set => timeLastModified = value;
        }

        // The depth resolution (mm per pixel) of the borehole image
        public int SourceDepthResolution => depthResolution;

        //  The azimuth resolution (number of pixels horizontally) of the borehole image
        public int SourceAzimuthResolution => sourceAzimuthResolution;

        public int TopEdgeInterceptMm => Convert.ToInt32(topEdgeIntercept 
                                                         * (double)depthResolution 
                                                         + sourceStartDepth);

        public int BottomEdgeInterceptMm => Convert.ToInt32(bottomEdgeIntercept 
                                                            * (double)depthResolution 
                                                            + sourceStartDepth);

        public int DepthResolution
        {
            get => depthResolution;

            set
            {
                depthResolution = value;

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
            
            layerStartY = (int)topEdgePoints[0].Y;

            for (var sinePointPos = 1; sinePointPos < topEdgePoints.Count; sinePointPos++)
            {
                var currentYPoint = (int)topEdgePoints[sinePointPos].Y;

                if (currentYPoint < layerStartY)
                    layerStartY = currentYPoint;
            }
        }

        /// <summary>
        /// Calculates the lowest Y-point (hisghest value) in the layer
        /// </summary>
        public void CalculateEndY()
        {
            var bottomEdgePoints = GetBottomEdgePoints();

            layerEndY = bottomEdgePoints[0].Y;
            int currentYPoint;

            for (int sinePointPos = 1; sinePointPos < bottomEdgePoints.Count; sinePointPos++)
            {
                currentYPoint = (int)bottomEdgePoints[sinePointPos].Y;

                if (currentYPoint > layerEndY)
                    layerEndY = currentYPoint;
            }
        }

        public abstract void ChangeTopAmplitudeBy(int changeAmplitudeBy);

        public abstract void ChangeBottomAmplitudeBy(int changeAmplitudeBy);

        public abstract void MoveEdge(int edgeToMove, int xMoveBy, int yMoveBy);

        /// <summary>
        /// Method which turns the layer type into a string from the selected bool variables
        /// </summary>
        internal void WriteLayerProperties()
        {
            layerType = "";

            if(m_layerProperties.HasFlag(LayerProperties.LayerVoid))
            {
                layerType = string.Concat(layerType, "void ");
            }

            if (m_layerProperties.HasFlag(LayerProperties.Clean))
            {
                layerType = string.Concat(layerType, "clean ");
            }

            if(m_layerProperties.HasFlag(LayerProperties.SmallBubbles))
            {
                layerType = string.Concat(layerType, "smallBubbles ");
            }

            if (m_layerProperties.HasFlag(LayerProperties.LargeBubbles))
            {
                layerType = string.Concat(layerType, "largeBubbles ");
            }

            if (m_layerProperties.HasFlag(LayerProperties.FineDebris))
            {
                layerType = string.Concat(layerType, "fineDebris ");
            }

            if (m_layerProperties.HasFlag(LayerProperties.CoarseDebris))
            {
                layerType = string.Concat(layerType, "coarseDebris ");
            }

            if (m_layerProperties.HasFlag(LayerProperties.Diamicton))
            {
                layerType = string.Concat(layerType, "diamicton ");
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

        public abstract int getBottomYPoint(int xPoint);

        public abstract string GetDetails();

        /// <summary>
        /// Returns how many visible edges the layer has
        /// </summary>
        /// <returns></returns>
        public abstract int GetNumOfEdges();        

        public string[] GetLayerGroupNames()
        {
            return allGroupNames;
        }

        # endregion

        # region set methods
        
        /// <summary>
        /// Sets the start depth of the borehole
        /// </summary>
        /// <param name="sourceStartDepth">The borehole's start depth</param>
        public void SetBoreholeStartDepth(int sourceStartDepth)
        {
            this.sourceStartDepth = sourceStartDepth;

            CalculateStartY();
            CalculateEndY();
        }

        /// <summary>
        /// Sets the end depth of the borehole
        /// </summary>
        /// <param name="endDepth">The borehole's end depth</param>
        public void SetBoreholeEndDepth(int endDepth)
        {
            this.sourceEndDepth = endDepth;
        }

        public abstract void SetTopEdgeDepth(int topDepthPixels);

        public abstract void SetBottomEdgeDepth(int bottomDepthPixels);

        public abstract void SetTopSineAmplitude(int topAmplitude);

        public abstract void SetBottomSineAmplitude(int bottomAmplitude);

        public abstract void SetTopSineAzimuth(int topAzimuth);

        public abstract void SetBottomSineAzimuth(int bottomAzimuth);

        public void SetAllGroupNames(string[] allGroupNames)
        {
            this.allGroupNames = allGroupNames;
        }

        public abstract void SetTopEdgeSlope(double firstEdgeSlope);

        public abstract void SetTopEdgeIntercept(int firstEdgeIntercept);

        public abstract void SetBottomEdgeSlope(double secondEdgeSlope);

        public abstract void SetBottomEdgeIntercept(int secondEdgeIntercept);

        # endregion          
    }

}
