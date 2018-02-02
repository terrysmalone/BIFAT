using System.ComponentModel;
using BoreholeFeatures;
using FeatureAnnotationTool.Converters;

namespace FeatureAnnotationTool
{
    /// <summary>
    /// Contains an inclusion so that its propertiesd can be displayed, 
    /// and changed, on a property grid 
    /// </summary>
    [DefaultProperty("Description")]
    public sealed class InclusionPropertyContainer
    {
        private readonly Inclusion m_underlyingInclusion;

        [Category("Depth"),
         Description("The inclusions start depth in millimetres"),
         DisplayName("Start Depth (mm)")]
        public int StartDepth => m_underlyingInclusion.StartDepth;

        [Category("Depth"),
         Description("The inclusions end depth in millimetres"),
         DisplayName("End Depth (mm)")]
        public int EndDepth => m_underlyingInclusion.EndDepth;

        [Category("\t\tDescription"),
         Description("Additional information")]
        public string Description
        {
            get => m_underlyingInclusion.Description;

            set => m_underlyingInclusion.Description = value;
        }

        [Browsable(true),
         DefaultValue("entry1"),
         Category("\tDescription"),
         Description("The group this inclusion belongs to (To " +
                     "change group display colours go to 'Features>Inclusions>Groups'"),
         TypeConverter(typeof(InclusionGroupConverter))]
        public string Group
        {
            get => m_underlyingInclusion.Group;

            set => m_underlyingInclusion.Group = value;
        }

        [Category("Points"),
         Description("The clusters points")]
        public string PointsString => m_underlyingInclusion.PointsString;
        
        public InclusionPropertyContainer(object underlyingInclusion)
        {
            m_underlyingInclusion = underlyingInclusion as Inclusion;
        }
    }
}