using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FeatureAnnotationTool.Controller;
using System.Windows.Forms;
using FeatureAnnotationTool.Model;

namespace FeatureAnnotationTool
{
    /// <summary>
    /// Author - Terry Malone (trm8@aber.ac.uk)
    /// Version 1.1
    ///  
    ///This program is free software; you can redistribute it and/or
    ///modify it under the terms of the GNU General Public License
    ///as published by the Free Software Foundation; either version 2
    ///of the License, or (at your option) any later version.
    ///
    ///This program is distributed in the hope that it will be useful,
    ///but WITHOUT ANY WARRANTY; without even the implied warranty of
    ///MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ///GNU General Public License for more details.
    /// </summary>
    class BIFAT
    {
        //public delegate IModel ModelFactory();
        //public delegate IView ViewFactory();

        internal static void Start()
        {
            AnnotationToolController controller = null;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            AnnotationToolModel model = new AnnotationToolModel();
            AnnotationToolForm view = new AnnotationToolForm();

            controller = new AnnotationToolController(model, view);

            Application.Run(view);
        }
    }
}
