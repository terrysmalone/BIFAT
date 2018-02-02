using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using BoreholeFeatures.Converters;

namespace BoreholeFeatures
{
    /// <summary>
    /// Represents a sinusoidal layer and all the actions that can be applied to it
    /// 
    /// Author - Terry Malone (trm8@aber.ac.uk)
    /// Version 1.1 Refactored
    /// </summary>
    [DefaultPropertyAttribute("Description")]
    public abstract class Layer
    {
        protected int sourceAzimuthResolution, depthResolution;

        protected int layerStartY, layerEndY;
        protected int sourceStartDepth, sourceEndDepth;
        protected String description = "";
        protected String layerType = "";
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
        
        //Layer properties
        private LayerProperties m_LayerProperties;

        protected DateTime timeAdded, timeLastModified;
        
        protected string[] allGroupNames;

        //private IModel _model;

        private LayerGroupConverter groupConverter = new LayerGroupConverter();

        #region properties

        # region property grid attributes

        [CategoryAttribute("\t\t\tDepth"), DescriptionAttribute("The layers start depth in millimetres")]
        [DisplayName("\tStart Depth (mm)")]
        public int StartDepth => (int)(layerStartY * (double)depthResolution + sourceStartDepth);

        [CategoryAttribute("\t\t\tDepth"), DescriptionAttribute("The layers end depth in millimetres")]
        [DisplayName("End Depth (mm)")]
        public int EndDepth => (int)((layerEndY * (double)depthResolution) + sourceStartDepth);

        [Browsable(true)]
        [DefaultValue("entry1")]
        [CategoryAttribute("\t\t\t\t\tDescription"), DescriptionAttribute("The group this layer belongs to (To change group display colours go to 'Features>Layers>Groups'")]
        [TypeConverter(typeof(LayerGroupConverter))]
        public string Group { get; set; }
        
        [Browsable(true)]
        [DefaultValue("entry1")]
        [CategoryAttribute("\t\t\t\t\tDescription"), DescriptionAttribute("The quality of the layer (1-very poor, 2-poor, 3-good, 4-very good)")]
        [TypeConverter(typeof(QualityConverter))]
        public int Quality
        {
            get { return quality; }
            set { quality = value; }
        }

        [CategoryAttribute("\t\t\t\t\tDescription"), DescriptionAttribute("Additional information")]
        public string Description
        {
            get { return description; }
            set 
            { 
                description = value;
                timeLastModified = DateTime.Now;
            }
        }

        [CategoryAttribute("\t\t\t\tLayer Properties"), DescriptionAttribute("Is the layer a void?")]
        [DisplayName("Void")]
        public bool LayerVoid
        {
            get => m_LayerProperties.HasFlag(LayerProperties.LayerVoid);
            set
            {
                if(value) m_LayerProperties |= LayerProperties.LayerVoid; 
                else m_LayerProperties &= ~LayerProperties.LayerVoid;
            }
        }

        [CategoryAttribute("\t\t\t\tLayer Properties"), DescriptionAttribute("Is the layer bubble/debris free?")]
        public bool Clean
        {
            get => m_LayerProperties.HasFlag(LayerProperties.Clean);
            set
            {
                if (value) m_LayerProperties |= LayerProperties.Clean;
                else m_LayerProperties &= ~LayerProperties.Clean;
            }
        }

        [CategoryAttribute("\t\t\t\tLayer Properties"), DescriptionAttribute("Does the layer contain small bubbles?")]
        [DisplayName("Small Bubbles")]
        public bool SmallBubbles
        {
            get => m_LayerProperties.HasFlag(LayerProperties.SmallBubbles);
            set
            {
                if (value) m_LayerProperties |= LayerProperties.SmallBubbles;
                else m_LayerProperties &= ~LayerProperties.SmallBubbles;
            }
        }

        [CategoryAttribute("\t\t\t\tLayer Properties"), DescriptionAttribute("Does the layer contain large bubbles?")]
        [DisplayName("Large Bubbles")]
        public bool LargeBubbles
        {
            get => m_LayerProperties.HasFlag(LayerProperties.LargeBubbles);
            set
            {
                if (value) m_LayerProperties |= LayerProperties.LargeBubbles;
                else m_LayerProperties &= ~LayerProperties.LargeBubbles;
            }
        }

        [CategoryAttribute("\t\t\t\tLayer Properties"), DescriptionAttribute("Does the layer contain fine debris?")]
        [DisplayName("Fine Debris")]
        public bool FineDebris
        {
            get => m_LayerProperties.HasFlag(LayerProperties.FineDebris);
            set
            {
                if (value) m_LayerProperties |= LayerProperties.FineDebris;
                else m_LayerProperties &= ~LayerProperties.FineDebris;
            }
        }

        [CategoryAttribute("\t\t\t\tLayer Properties"), DescriptionAttribute("Does the layer contain coarse debris?")]
        [DisplayName("Coarse Debris")]
        public bool CoarseDebris
        {
            get => m_LayerProperties.HasFlag(LayerProperties.CoarseDebris);
            set
            {
                if (value) m_LayerProperties |= LayerProperties.CoarseDebris;
                else m_LayerProperties &= ~LayerProperties.CoarseDebris;
            }
        }

        [CategoryAttribute("\t\t\t\tLayer Properties"), DescriptionAttribute("Does the layer contain debris of varying grain size?")]
        public bool Diamicton
        {
            get => m_LayerProperties.HasFlag(LayerProperties.Diamicton);
            set
            {
                if (value) m_LayerProperties |= LayerProperties.Diamicton;
                else m_LayerProperties &= ~LayerProperties.Diamicton;
            }
        }

        # endregion property grid properties

        /// <summary>
        /// Returns the depth of the layers top edge
        /// </summary>
        /// <returns>The depth of the top sine</returns>
        public int TopEdgeDepth
        {
            get { return topDepthPixels; }
        }

        /// <summary>
        /// The depth of the layers bottom edge
        /// </summary>
        public int BottomEdgeDepth
        {
            get { return bottomDepthPixels; }
        }

        /// <summary>
        /// Returns the depth in millimetres of the top sine of the layer
        /// </summary>
        /// <returns>The depth in millimetres of the top sine</returns>
        public int TopEdgeDepthMM
        {
            get { return System.Convert.ToInt32(((double)topDepthPixels * (double)depthResolution) + (double)sourceStartDepth); }
        }

        /// <summary>
        /// Returns the depth in millimetres of the bottom sine of the layer
        /// </summary>
        /// <returns>The depth in millimetres of the bottom sine</returns>
        public int BottomEdgeDepthMM
        {
            get { return System.Convert.ToInt32(((double)bottomDepthPixels * (double)depthResolution) + (double)sourceStartDepth); }
        }

        /// <summary>
        /// Returns the amplitude of the top edge (lowest y co-ordinates) in mm
        /// </summary>
        /// <returns>The top edges amplitude</returns>
        public int TopSineAmplitudeInMM
        {
            get { return (int)((double)topAmplitude * (double)depthResolution); }
        }

        /// <summary>
        /// The azimuth of the top sine (lowest y co-ordinates)
        /// </summary>
        public int TopSineAzimuth
        {
            get { return topAzimuth; }
        }

        /// <summary>
        /// The azimuth of the bottom sine (highest y co-ordinates)
        /// </summary>
        public int BottomSineAzimuth
        {
            get { return bottomAzimuth; }
        }

        /// <summary>
        /// The amplitude of the top sine (lowest y co-ordinates)
        /// </summary>
        public int TopSineAmplitude
        {
            get { return topAmplitude; }
        }


        /// <summary>
        /// The amplitude of the bottom edge (highest y co-ordinates)
        /// </summary>
        public int BottomSineAmplitude
        {
            get { return bottomAmplitude; }
        }

        /// <summary>
        /// The amplitude of the bottom edge (highest y co-ordinates)
        /// </summary>
        public int BottomSineAmplitudeInMM
        {
            get {return (int)((double)bottomAmplitude * (double)depthResolution);}
        }

        public double TopEdgeSlope
        {
            get { return topEdgeSlope; }
        }

        public int TopEdgeIntercept
        {
            get { return topEdgeIntercept; }
        }

        public double BottomEdgeSlope
        {
            get { return bottomEdgeSlope; }
        }

        public int BottomEdgeIntercept
        {
            get { return bottomEdgeIntercept; }
        }
        
        /// <summary>
        /// The layer's lowest y-point
        /// </summary>
        public int StartY
        {
            get { return layerStartY; }
        }

        /// <summary>
        /// The layer's highest y-point
        /// </summary>
        public int EndY
        {
            get { return layerEndY; }
        }

        /// <summary>
        /// The borehole's start depth
        /// </summary>
        public int SourceStartDepth
        {
            get { return sourceStartDepth; }
        }

        /// <summary>
        /// The borehole's end depth
        /// </summary>
        public int SourceEndDepth
        {
            get { return sourceEndDepth; }
        }

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
            get { return timeAdded; }
            set { timeAdded = value; }
        }

        /// <summary>
        /// The time the layer was last modified
        /// </summary>
        public DateTime TimeLastModified
        {
            get { return timeLastModified; }
            set { timeLastModified = value; }
        }

        // The depth resolution (mm per pixel) of the borehole image
        public int SourceDepthResolution
        {
            get { return depthResolution; }
        }

        //  The azimuth resolution (number of pixels horizontally) of the borehole image
        public int SourceAzimuthResolution => sourceAzimuthResolution;

        public int TopEdgeInterceptMm => Convert.ToInt32(((double)topEdgeIntercept * (double)depthResolution) + (double)sourceStartDepth);

        public int BottomEdgeInterceptMm => Convert.ToInt32(((double)bottomEdgeIntercept * (double)depthResolution) + (double)sourceStartDepth);

        public int DepthResolution
        {
            get { return depthResolution; }

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
            List<Point> topEdgePoints = GetTopEdgePoints();
            
            layerStartY = (int)topEdgePoints[0].Y;
            int currentYPoint;

            for (int sinePointPos = 1; sinePointPos < topEdgePoints.Count; sinePointPos++)
            {
                currentYPoint = (int)topEdgePoints[sinePointPos].Y;

                if (currentYPoint < layerStartY)
                    layerStartY = currentYPoint;
            }
        }

        /// <summary>
        /// Calculates the lowest Y-point (hisghest value) in the layer
        /// </summary>
        public void CalculateEndY()
        {
            List<Point> bottomEdgePoints = GetBottomEdgePoints();

            layerEndY = (int)bottomEdgePoints[0].Y;
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

            if(m_LayerProperties.HasFlag(LayerProperties.LayerVoid))
            {
                layerType = string.Concat(layerType, "void ");
            }

            if (m_LayerProperties.HasFlag(LayerProperties.Clean))
            {
                layerType = string.Concat(layerType, "clean ");
            }

            if(m_LayerProperties.HasFlag(LayerProperties.SmallBubbles))
            {
                layerType = string.Concat(layerType, "smallBubbles ");
            }

            if (m_LayerProperties.HasFlag(LayerProperties.LargeBubbles))
            {
                layerType = string.Concat(layerType, "largeBubbles ");
            }

            if (m_LayerProperties.HasFlag(LayerProperties.FineDebris))
            {
                layerType = string.Concat(layerType, "fineDebris ");
            }

            if (m_LayerProperties.HasFlag(LayerProperties.CoarseDebris))
            {
                layerType = string.Concat(layerType, "coarseDebris ");
            }

            if (m_LayerProperties.HasFlag(LayerProperties.Diamicton))
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
        /// Sets the quality of the layer
        /// </summary>
        /// <param name="quality">The quality of the layer</param>
        public void SetQuality(int quality)
        {
            this.quality = quality;
            timeLastModified = DateTime.Now;
        }

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
