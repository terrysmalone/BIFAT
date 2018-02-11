using System.ComponentModel;
using BoreholeFeatures;

namespace FeatureAnnotationTool.PropertyGridWrappers
{
    public sealed class CoreLayerPropertyContainer : LayerPropertyContainer
    {
        private readonly CoreLayer m_UnderlyingLayer;

        [Category("Top Edge"),
         Description("The intercept of the layer's top edge in millimetres"), 
         DisplayName("Intercept (mm)")]
        public int TopEdgeIntercept => m_UnderlyingLayer.TopEdgeIntercept;

        [Category("Top Edge"),
         Description("The slope of the layer's top edge"),
         DisplayName("Slope")]
        public double TopEdgeSlope => m_UnderlyingLayer.TopEdgeSlope;

        [Category("Bottom Edge"),
         Description("The intercept of the layer's bottom edge in millimetres"),
         DisplayName("Intercept (mm)")]
        public int BottomEdgeIntercept => m_UnderlyingLayer.BottomEdgeIntercept;

        [Category("Bottom Edge"),
         Description("The slope of the layer's bottom edge"),
         DisplayName("Slope")]
        public double BottomEdgeSlope => m_UnderlyingLayer.BottomEdgeSlope;


        public CoreLayerPropertyContainer(object underlyingLayer) : base(underlyingLayer)
        {
            m_UnderlyingLayer = underlyingLayer as CoreLayer;
        }
    }
}