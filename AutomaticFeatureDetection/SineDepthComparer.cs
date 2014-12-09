using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdgeFitting;

namespace AutomaticFeatureDetection
{
    /// <summary>
    /// Class which allows the sorting of sines by depth in ascending order
    /// Overrides IComparer
    /// Author - Terry Malone (trm8@aber.ac.uk)
    /// Version 1.1
    /// </summary>
    public class SineDepthComparer : IComparer<Sine>
    {
        public int Compare(Sine one, Sine two)
        {
            if (one.Depth < two.Depth)
                return -1;
            else if (one.Depth > two.Depth)
                return 1;
            else
                return 0;
        }
    }
}
