using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdgeFitting;

namespace AutomaticFeatureDetection
{
    public class LineDepthComparer:IComparer<EdgeLine>
    {
        public int Compare(EdgeLine one, EdgeLine two)
        {
            if (one.Intercept < two.Intercept)
                return -1;
            else if (one.Intercept > two.Intercept)
                return 1;
            else
                return 0;
        }
    }
}
