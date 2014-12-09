﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace BoreholeFeatures
{
    public class LayerGroupConverter : StringConverter
    {
        //private IModel _model;

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
        public override System.ComponentModel.TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            Layer refLayer = context.Instance as Layer;
            return new StandardValuesCollection(refLayer.GetLayerGroupNames());
        }
    }
}