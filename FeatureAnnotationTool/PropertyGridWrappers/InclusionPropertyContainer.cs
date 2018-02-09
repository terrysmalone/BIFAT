using System.ComponentModel;
using BoreholeFeatures;
using FeatureAnnotationTool.Converters;

namespace FeatureAnnotationTool.PropertyGridWrappers
{
    /// <summary>
    /// Contains an inclusion so that its propertiesd can be displayed, 
    /// and changed, on a property grid 
    /// </summary>
    public sealed class InclusionPropertyContainer
    {
        private readonly Inclusion m_UnderlyingInclusion;

        [Category("Depth"),
         Description("The inclusions start depth in millimetres"),
         DisplayName("Start Depth (mm)")]
        public int StartDepth => m_UnderlyingInclusion.StartDepth;

        [Category("Depth"),
         Description("The inclusions end depth in millimetres"),
         DisplayName("End Depth (mm)")]
        public int EndDepth => m_UnderlyingInclusion.EndDepth;

        [Category("\t\tDescription"),
         Description("Additional information")]
        public string Description
        {
            get => m_UnderlyingInclusion.Description;

            set => m_UnderlyingInclusion.Description = value;
        }

        [Browsable(true),
         DefaultValue("entry1"),
         Category("\tDescription"),
         Description("The group this inclusion belongs to (To " +
                     "change group display colours go to 'Features>Inclusions>Groups'"),
         TypeConverter(typeof(InclusionGroupConverter))]
        public string Group
        {
            get => m_UnderlyingInclusion.Group;

            set => m_UnderlyingInclusion.Group = value;
        }

        [Category("Points"),
         Description("The clusters points")]
        public string PointsString => m_UnderlyingInclusion.PointsString;
        
        public InclusionPropertyContainer(object underlyingInclusion)
        {
            m_UnderlyingInclusion = underlyingInclusion as Inclusion;
        }
    }
}