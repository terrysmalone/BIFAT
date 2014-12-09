using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BoreholeFeatures
{
    public class LayerTypeSelector
    {
        protected String type;

        public LayerTypeSelector(String type)
        {
            this.type = type;
        }

        public Layer setUpLayer(int firstDepth, int firstAmplitude, int firstAzimuth, int secondDepth, int secondAmplitude, int secondAzimuth, int azimuthResolution, int depthResolution)
        {
            if (type.Equals("Borehole"))
                return new BoreholeLayer(firstDepth, firstAmplitude, firstAzimuth, secondDepth, secondAmplitude, secondAzimuth, azimuthResolution, depthResolution);
            else
                return new BoreholeLayer(firstDepth, firstAmplitude, firstAzimuth, secondDepth, secondAmplitude, secondAzimuth, azimuthResolution, depthResolution);
        }

        public Layer setUpLayer(double firstSlope, int firstIntercept, double secondSlope, int secondIntercept, int azimuthResolution, int depthResolution)
        {
            if (type.Equals("Core"))
                return new CoreLayer(firstSlope, firstIntercept, secondSlope, secondIntercept, azimuthResolution, depthResolution);
            else
                return new CoreLayer(firstSlope, firstIntercept, secondSlope, secondIntercept, azimuthResolution, depthResolution);
        }
    }
}
