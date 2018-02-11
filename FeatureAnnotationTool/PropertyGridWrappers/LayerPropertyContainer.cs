using System.ComponentModel;
using BoreholeFeatures;
using FeatureAnnotationTool.Converters;

namespace FeatureAnnotationTool.PropertyGridWrappers
{
    public class LayerPropertyContainer
    {
        private readonly Layer m_UnderlyingLayer;

        [Category("Depth"),
         Description("The layers start depth in millimetres"),
         DisplayName("Start Depth (mm)")]
        public int StartDepth => m_UnderlyingLayer.StartDepth;

        [Category("Depth"),
         Description("The layers end depth in millimetres"),
         DisplayName("End Depth (mm)")]
        public int EndDepth => m_UnderlyingLayer.EndDepth;

        [Browsable(true),
         DefaultValue("entry1"),
         Category("Description"),
         Description("The group this layer belongs to (To change group display colours go to 'Features>Layers>Groups'"),
         TypeConverter(typeof(LayerGroupConverter))]
        public string Group
        {
            get => m_UnderlyingLayer.Group;

            set => m_UnderlyingLayer.Group = value;
        }

        [Browsable(true),
         DefaultValue("entry1"),
         Category("Description"), Description("The quality of the layer (1-very poor, 2-poor, 3-good, 4-very good)"),
         TypeConverter(typeof(QualityConverter))]
        public int Quality
        {
            get => m_UnderlyingLayer.Quality;

            set => m_UnderlyingLayer.Quality = value;
        }

        [Category("Description"), 
         Description("Additional information")]
        public string Description
        {
            get => m_UnderlyingLayer.Description;

            set => m_UnderlyingLayer.Description = value;
        }

        [Category("Layer Properties"), 
         Description("Is the layer a void?"),
         DisplayName("Void")]
        public bool LayerVoid
        {
            get => m_UnderlyingLayer.LayerVoid;

            set => m_UnderlyingLayer.LayerVoid = value;
        }

        [Category("Layer Properties"),
         Description("Is the layer bubble/debris free?")]
        public bool Clean
        {
            get => m_UnderlyingLayer.Clean;

            set => m_UnderlyingLayer.Clean = value;
        }

        [Category("Layer Properties"),
         Description("Does the layer contain small bubbles?"),
         DisplayName("Small Bubbles")]
        public bool SmallBubbles
        {
            get => m_UnderlyingLayer.SmallBubbles;

            set => m_UnderlyingLayer.SmallBubbles = value;
        }

        [Category("Layer Properties"),
         Description("Does the layer contain large bubbles?"),
         DisplayName("Large Bubbles")]
        public bool LargeBubbles
        {
            get => m_UnderlyingLayer.LargeBubbles;

            set => m_UnderlyingLayer.LargeBubbles = value;
        }

        [Category("Layer Properties"),
         Description("Does the layer contain fine debris?"),
         DisplayName("Fine Debris")]
        public bool FineDebris
        {
            get => m_UnderlyingLayer.FineDebris;

            set => m_UnderlyingLayer.FineDebris = value;
        }

        [Category("Layer Properties"),
         Description("Does the layer contain coarse debris?"),
         DisplayName("Coarse Debris")]
        public bool CoarseDebris
        {
            get => m_UnderlyingLayer.CoarseDebris;

            set => m_UnderlyingLayer.CoarseDebris = value;
        }

        [Category("Layer Properties"),
         Description("Does the layer contain debris of varying grain size?")]
        public bool Diamicton
        {
            get => m_UnderlyingLayer.Diamicton;

            set => m_UnderlyingLayer.Diamicton = value;
        }

        public LayerPropertyContainer(object underlyingLayer)
        {
            m_UnderlyingLayer = underlyingLayer as Layer;
        }
    }
}