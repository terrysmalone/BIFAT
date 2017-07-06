using System;
using System.Collections.Generic;
using EdgeFitting;

namespace AutomaticFeatureDetection
{
    public class LineDepthComparer:IComparer<EdgeLine>
    {
        public int Compare(EdgeLine one, EdgeLine two)
        {
            if (one == null) { throw new ArgumentNullException(nameof(one)); }

            if (two == null) { throw new ArgumentNullException(nameof(one)); }

            if (one.Intercept < two.Intercept)
            {
                return -1;
            }
            else if (one.Intercept > two.Intercept)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }
}
