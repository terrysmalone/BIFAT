using System.ComponentModel;
using BoreholeFeatures;
using FeatureAnnotationTool.Converters;

namespace FeatureAnnotationTool
{
    /// <summary>
    /// Contains a cluster so that its propertiesd can be displayed, 
    /// and changed, on a property grid 
    /// </summary>
    [DefaultProperty("Description")]
    public sealed class ClusterPropertyContainer
    {
        private readonly Cluster m_underlyingCluster;
        
        [Category("Depth"),
         Description("The clusters start depth in millimetres"),
         DisplayName("Start Depth (mm)")]
        public int StartDepth => m_underlyingCluster.StartDepth;

        [Category("Depth"),
         Description("The clusters end depth in millimetres"),
         DisplayName("End Depth (mm)")]
        public int EndDepth => m_underlyingCluster.EndDepth;

        [Category("\t\tDescription"),
         Description("Additional information")]
        public string Description
        {
            get => m_underlyingCluster.Description;

            set => m_underlyingCluster.Description = value;
        }

        [Browsable(true),
         DefaultValue("entry1"),
         Category("\tDescription"),
         Description("The group this cluster belongs to (To " +
                     "change group display colours go to 'Features>Clusters>Groups'"),
         TypeConverter(typeof(ClusterGroupConverter))]
        public string Group
        {
            get => m_underlyingCluster.Group;

            set => m_underlyingCluster.Group = value;
        }

        [Category("Points"),
         Description("The clusters points")]
        public string PointsString => m_underlyingCluster.PointsString;

        [Category("\tCluster Properties"),
         Description("Does the cluster contain small bubbles?"),
         DisplayName("Small Bubbles")]
        public bool SmallBubbles
        {
            get => m_underlyingCluster.SmallBubbles;

            set => m_underlyingCluster.SmallBubbles = value;
        }

        [Category("\tCluster Properties"),
         Description("Does the cluster contain large bubbles?"),
         DisplayName("Large Bubbles")]
        public bool LargeBubbles
        {
            get => m_underlyingCluster.LargeBubbles;

            set => m_underlyingCluster.LargeBubbles = value;
        }

        [Category("\tCluster Properties"),
         Description("Does the cluster contain fine debris?"),
         DisplayName("Fine Debris")]
        public bool FineDebris
        {
            get => m_underlyingCluster.FineDebris;

            set => m_underlyingCluster.FineDebris = value;
        }

        [Category("\tCluster Properties"),
         Description("Does the cluster contain coarse debris?"),
         DisplayName("Coarse Debris")]
        public bool CoarseDebris
        {
            get => m_underlyingCluster.CoarseDebris;

            set => m_underlyingCluster.CoarseDebris = value;
        }

        [Category("\tCluster Properties"),
         Description("Does the cluster contain debris of varying grain size?")]
        public bool Diamicton
        {
            get => m_underlyingCluster.Diamicton;

            set => m_underlyingCluster.Diamicton = value;
        }

        public ClusterPropertyContainer(object underlyingCluster)
        {
            m_underlyingCluster = underlyingCluster as Cluster;
        }
    }
}