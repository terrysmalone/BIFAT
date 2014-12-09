using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BoreholeFeatures;

namespace LayerDetection
{
    class LayerDepthComparer : IComparer<Layer>
    {
        public int Compare(Layer one, Layer two)
        {
            if (one.TopEdgeDepth < two.TopEdgeDepth)
                return -1;
            else if (one.TopEdgeDepth > two.TopEdgeDepth)
                return 1;
            else
                return 0;
        }
    }
}
