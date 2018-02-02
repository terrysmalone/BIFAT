using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BoreholeFeatures
{
    [Flags]
    public enum LayerProperties
    {
        None = 0,
        LayerVoid = 1,
        Clean = 2,
        SmallBubbles = 4,
        LargeBubbles = 8,
        FineDebris = 16,
        CoarseDebris = 32,
        Diamicton = 64
    }
}
