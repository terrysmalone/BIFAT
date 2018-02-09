using BoreholeFeatures;

namespace FeatureAnnotationTool.PropertyGridWrappers
{
    public sealed class CoreLayerPropertyContainer : LayerPropertyContainer
    {
        private readonly Layer m_UnderlyingLayer;

        public CoreLayerPropertyContainer(object underlyingLayer) : base(underlyingLayer)
        {
            m_UnderlyingLayer = underlyingLayer as CoreLayer;
        }
    }
}