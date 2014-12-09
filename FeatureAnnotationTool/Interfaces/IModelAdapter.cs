using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace FeatureAnnotationTool.Interfaces
{
    /// <summary>
    /// Deals with calls from the model to the controller
    /// </summary>
    public interface IModelAdapter
    {
        void displayImage(Bitmap image);
        void displayPreview(Bitmap image);
        void DisplayFullPreview(Bitmap image);
        // Any functions the model needs to call to be propogated to the view

        void setTitleName(string boreholeName);

        void setRulerImage(Bitmap ruler);
                
       //void setBoreholeSectionEnds(int p, int p_2);

        void SetFileName(string fileName);

        void enableNextSectionButton();

        void disableNextSectionButton();

        void disablePreviousSectionButton();

        void disableFirstSectionButton();

        void enablePreviousSectionButton();

        void enableFirstSectionButton();

        void setBoreholeSectionsComboBoxItems(int boreholeStartDepth, int boreholeEndDepth);

        void disableLastSectionButton();

        void enableLastSectionButton();

        void SetCurrentSectionComboBox(int p);

        void RefreshBoreholeImage();

        void DrawAllFeatures(object sender, PaintEventArgs e);

        void StartProgressReport(string title);

        void EndProgressReport();

        void UpdateProgress(int percentComplete);

        void DrawOrientationRuler();

        void HideOrientationRuler();

        void ShowWellCADExport(bool show);
    }
}
