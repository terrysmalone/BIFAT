using System.ComponentModel;
using BoreholeFeatures;
using FeatureAnnotationTool.Converters;

namespace FeatureAnnotationTool.PropertyGridWrappers
{
    /// <summary>
    /// Contains a cluster so that its properties can be displayed, 
    /// and changed, on a property grid 
    /// </summary>
    public sealed class ClusterPropertyContainer
    {
        private readonly Cluster m_UnderlyingCluster;
        
        [Category("Depth"),
         Description("The clusters start depth in millimetres"),
         DisplayName("Start Depth (mm)")]
        public int StartDepth => m_UnderlyingCluster.StartDepth;

        [Category("Depth"),
         Description("The clusters end depth in millimetres"),
         DisplayName("End Depth (mm)")]
        public int EndDepth => m_UnderlyingCluster.EndDepth;

        [Category("\t\tDescription"),
         Description("Additional information")]
        public string Description
        {
            get => m_UnderlyingCluster.Description;

            set => m_UnderlyingCluster.Description = value;
        }

        [Browsable(true),
         DefaultValue("entry1"),
         Category("\tDescription"),
         Description("The group this cluster belongs to (To " +
                     "change group display colours go to 'Features>Clusters>Groups'"),
         TypeConverter(typeof(ClusterGroupConverter))]
        public string Group
        {
            get => m_UnderlyingCluster.Group;

            set => m_UnderlyingCluster.Group = value;
        }

        [Category("Points"),
         Description("The clusters points")]
        public string PointsString => m_UnderlyingCluster.PointsString;

        [Category("\tCluster Properties"),
         Description("Does the cluster contain small bubbles?"),
         DisplayName("Small Bubbles")]
        public bool SmallBubbles
        {
            get => m_UnderlyingCluster.SmallBubbles;

            set => m_UnderlyingCluster.SmallBubbles = value;
        }

        [Category("\tCluster Properties"),
         Description("Does the cluster contain large bubbles?"),
         DisplayName("Large Bubbles")]
        public bool LargeBubbles
        {
            get => m_UnderlyingCluster.LargeBubbles;

            set => m_UnderlyingCluster.LargeBubbles = value;
        }

        [Category("\tCluster Properties"),
         Description("Does the cluster contain fine debris?"),
         DisplayName("Fine Debris")]
        public bool FineDebris
        {
            get => m_UnderlyingCluster.FineDebris;

            set => m_UnderlyingCluster.FineDebris = value;
        }

        [Category("\tCluster Properties"),
         Description("Does the cluster contain coarse debris?"),
         DisplayName("Coarse Debris")]
        public bool CoarseDebris
        {
            get => m_UnderlyingCluster.CoarseDebris;

            set => m_UnderlyingCluster.CoarseDebris = value;
        }

        [Category("\tCluster Properties"),
         Description("Does the cluster contain debris of varying grain size?")]
        public bool Diamicton
        {
            get => m_UnderlyingCluster.Diamicton;

            set => m_UnderlyingCluster.Diamicton = value;
        }

        public ClusterPropertyContainer(object underlyingCluster)
        {
            m_UnderlyingCluster = underlyingCluster as Cluster;
        }
    }
}