using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace BoreholeFeatures
{
    /// <summary>
    /// Modifies the input of the quality field for a feature in the property grid to show a 
    /// drop-down box with choices 1-4
    /// </summary>
    public class QualityConverter : Int32Converter
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
        public override System.ComponentModel.TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(
            new int[] { 1, 2, 3, 4 });
        }
    }

}
