using System.ComponentModel;
using BoreholeFeatures;

namespace FeatureAnnotationTool.PropertyGridWrappers
{
    public sealed class BoreholeLayerPropertyContainer : LayerPropertyContainer
    {
        private readonly Layer m_UnderlyingLayer;

        [Category("\tBottom Edge"), Description("The amplitude of the layer's bottom edge")]
        [DisplayName("Amplitude (mm)")]
        public int SecondSineAmplitude => m_UnderlyingLayer.BottomSineAmplitudeInMm;

        public BoreholeLayerPropertyContainer(object underlyingLayer) : base (underlyingLayer)
        {
            m_UnderlyingLayer = underlyingLayer as BoreholeLayer;
        }
    }
}