using System.ComponentModel;
using BoreholeFeatures;

namespace FeatureAnnotationTool.Converters
{
    internal class ClusterGroupConverter : StringConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            //true means show a combobox
            return true;
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            //true will limit to list. false will show the list, but allow free-form entry
            return false;
        }

        /// <summary>
        /// Overrides the standard values
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            var refCluster = context.Instance as Cluster;

            // ReSharper disable once PossibleNullReferenceException
            return new StandardValuesCollection(refCluster.GetClusterGroupNames());
        }
    }
}