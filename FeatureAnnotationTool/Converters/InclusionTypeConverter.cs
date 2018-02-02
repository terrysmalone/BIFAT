using System.ComponentModel;

namespace FeatureAnnotationTool.Converters
{
    /// <summary>
    /// Modifies the input type for the inclsuion type on the propertygrid to show a drop-down list
    /// </summary>
    internal class InclusionTypeConverter : StringConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            //true means show a combobox
            return true;
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            //true will limit to list. false will show the list, but allow free-form entry
            return true;
        }

        /// <summary>
        /// Overrides the standard values
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(new[] { "Clast", "Bubble", "Void" });
        }
    }

}
