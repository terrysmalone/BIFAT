using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Edges
{
    /// <summary>
    ///  Class which overrides IComparer in order to allow the sorting of edges by length
    ///  in descending order
    /// 
    /// Author - Terry Malone (trm8@aber.ac.uk)
    /// Version 1.1
    /// </summary>
    public class EdgesLengthComparer : IComparer<Edge>
    {
        public int Compare(Edge one, Edge two)
        {
            if (one.EdgeLength < two.EdgeLength)
                return 1;
            else if (one.EdgeLength > two.EdgeLength)
                return -1;
            else
                return 0;
        }
    }

}
