using System;

namespace BoreholeFeatures
{
    [Flags]
    public enum ClusterProperties
    {
        None         = 0,
        SmallBubbles = 1,
        LargeBubbles = 2,
        FineDebris   = 4,
        CoarseDebris = 8,
        Diamicton    = 16
    }
}