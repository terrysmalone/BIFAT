using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using BoreholeFeatures;

namespace FeatureAnnotationTool.Interfaces
{
    /// <summary>
    /// Deals with calls to the view
    /// </summary>
    public interface IView
    {
        void SetAdapter(IViewAdapter a);

        void DisplayImage(Bitmap image);
        void DisplayPreview(Bitmap image);
        void DisplayFullPreview(Bitmap image);
        

        void changeBoreholeScroller(double position);
        
        void HideFeatureDetailsPropertyGrid();

        void ShowFeatureDetailsPropertyGrid();

        void SetTitleName(string boreholeName);

        void SetRulerImage(Bitmap ruler);

        void AddCompleteLayer(Layer layerToAdd);

        void OpenImageFile();
        void OpenOTVFile();

        void openExistingFile();

        //void setBoreholeSectionEnds(int sectionStartDepth, int sectionStartDepth_2);

        void SetFileName(string fileName);

        void EnableNextSectionButton();

        void DisableNextSectionButton();

        void EnablePreviousSectionButton();

        void DisablePreviousSectionButton();

        void DisableFirstSectionButton();

        void EnableFirstSectionButton();

        void UpdateProgress(int currentProgress, int totalProgress);

        void RefreshBoreholeImage();

        void StopEdgeDetection(bool showMessage);

        void SetBoreholeSectionsComboBoxItems(int boreholeStartDepth, int boreholeEndDepth);

        void DisableLastSectionButton();

        void EnableLastSectionButton();

        void SetCurrentSectionComboBox(int currentSection);

        void DrawAllFeatures(object sender, PaintEventArgs e);

        void StartProgressReport(string title);

        void EndProgressReport();

        void UpdateProgress(int percentComplete);

        void SetProjectType(string p);

        void DrawOrientationRuler();

        void HideOrientationRuler();

        void ShowWellCADExport(bool show);
    }
}
