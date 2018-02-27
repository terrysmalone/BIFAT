using System.ComponentModel;
using BoreholeFeatures;

namespace FeatureAnnotationTool.PropertyGridWrappers
{
    public sealed class BoreholeLayerPropertyContainer : LayerPropertyContainer
    {
        private readonly BoreholeLayer m_UnderlyingLayer;

        [Category("\t\tTop Edge"), 
         Description("The depth of the layer's top edge"), 
         DisplayName("Depth (mm)")]
        public int FirstSineDepth => m_UnderlyingLayer.TopEdgeDepthMm;

        [Category("\t\tTop Edge"), 
         Description("The amplitude of the layer's top edge"),
         DisplayName("Amplitude (mm)")]
        public int FirstSineAmplitude => m_UnderlyingLayer.TopSineAmplitudeInMm;

        [Category("\t\tTop Edge"),
         Description("The azimuth of the layer's top edge"),
         DisplayName("Azimuth (mm)")]
        public int FirstSineAzimuth => m_UnderlyingLayer.TopSineAzimuthInMm;

        [Category("\tBottom Edge"),
         Description("The depth of the layer's bottom edge"),
         DisplayName("Depth (mm)")]
        public int SecondSineDepth => m_UnderlyingLayer.SecondSineDepth;

        [Category("\tBottom Edge"),
         Description("The amplitude of the layer's bottom edge"),
         DisplayName("Amplitude (mm)")]
        public int SecondSineAmplitude => m_UnderlyingLayer.BottomSineAmplitudeInMm;

        [Category("\tBottom Edge"),
         Description("The azimuth of the layer's bottom edge"),
         DisplayName("Azimuth (mm)")]
        public int SecondSineAzimuth => m_UnderlyingLayer.BottomSineAzimuthInMm;

        public BoreholeLayerPropertyContainer(object underlyingLayer) : base (underlyingLayer)
        {
            m_UnderlyingLayer = underlyingLayer as BoreholeLayer;
        }
    }
}