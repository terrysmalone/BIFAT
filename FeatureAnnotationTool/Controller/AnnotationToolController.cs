using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using FeatureAnnotationTool.AutoDetect;
using System.ComponentModel;
using System.Windows.Forms;
using BoreholeFeatures;
using FeatureAnnotationTool.Interfaces;
using ImageTiler;
using RGReferencedData;

namespace FeatureAnnotationTool.Controller
{
    /// <summary>
    /// Controls the interactions between the model and view
    /// </summary>
    public class AnnotationToolController : IModelAdapter, IViewAdapter
    {
        private IModel _model;
        private IView _view;

        #region properties

        /// <summary>
        /// Returns the azimuth of the selected layers top (lowest y-value) sine 
        /// </summary>
        /// <returns>The azimuth of the selected layers top sine</returns>
        public int TopAzimuthOfSelectedLayer
        {
            get { return _model.TopAzimuthOfSelectedLayer; }
        }

        /// <summary>
        /// Returns the azimuth of the selected layers bottom (highest y-value) sine 
        /// </summary>
        /// <returns>The azimuth of the selected layers bottom sine</returns>
        public int BottomAzimuthOfSelectedLayer
        {
            get { return _model.BottomAzimuthOfSelectedLayer; }
        }

        # region Borehole details properties

        public string BoreholeName
        {
            get { return _model.BoreholeName ;}
        }

        public int BoreholeStartDepth
        {
            get { return _model.BoreholeStartDepth; }
        }

        public int BoreholeEndDepth
        {
            get { return _model.BoreholeEndDepth; }
        }

        public int DepthResolution
        {
            get { return _model.DepthResolution;}
        }

        public int AzimuthResolution
        {
            get { return _model.AzimuthResolution; }
        }

        public int BoreholeDepth
        {
            get { return _model.BoreholeDepth; }
        }

        # endregion Borehole details properties

        public string ProjectType
        {
            get { return _model.ProjectType; }
            set { _model.ProjectType = value; }
        }

        public string ImageType
        {
            get { return _model.ImageType; }
        }

        #endregion properties

        /// <summary>
        /// Constructor method
        /// </summary>
        /// <param name="m">Model interface</param>
        /// <param name="v">View Interface</param>
        public AnnotationToolController(IModel m, IView v)
        {
            _model = m;
            _view = v;

            _model.setAdapter(this);
            _view.SetAdapter(this);
        }

        public void RefreshBoreholeImage()
        {
            _view.RefreshBoreholeImage();
        }

        public void DrawOrientationRuler()
        {
            _view.DrawOrientationRuler();
        }

        public void HideOrientationRuler()
        {
            _view.HideOrientationRuler();
        }

        # region Calls from View

        #region Export features methods

        public void WriteAllFeaturesToExcel(string fileName, List<string> layerPropertiesToInclude, List<string> clusterPropertiesToInclude, List<string> inclusionPropertiesToInclude)
        {
            _model.WriteAllFeaturesToExcel(fileName, layerPropertiesToInclude, clusterPropertiesToInclude, inclusionPropertiesToInclude);
        }

        public void writeLayersToText(string fileName, List<string> layerPropertiesToIncude)
        {
            _model.WriteLayersToText(fileName, layerPropertiesToIncude);
        }

        public void WriteLayersForWellCAD(string fileName)
        {
            _model.WriteLayersForWellCAD(fileName);
        }

        public void WriteClustersToText(string fileName, List<string> clusterPropertiesToIncude)
        {
            _model.WriteClustersToText(fileName, clusterPropertiesToIncude);
        }

        public void WriteInclusionsToText(string fileName, List<string> inclusionPropertiesToIncude)
        {
            _model.WriteInclusionsToText(fileName, inclusionPropertiesToIncude);
        }

        # endregion

        # region Group methods

        public void SaveGroups()
        {
            _model.SaveGroups();
        }

        public void AddLayerGroup(string groupName)
        {
            _model.AddLayerGroup(groupName);
        }

        public void AddClusterGroup(string groupName)
        {
            _model.AddClusterGroup(groupName);
        }

        public void AddInclusionGroup(string groupName)
        {
            _model.AddInclusionGroup(groupName);
        }

        # endregion
        
        # region Features methods

        public void RemoveFluidLevel()
        {
            _model.RemoveFluidLevel();
        }

        public void DeleteAllLayers()
        {
            _model.DeleteAllLayers();
        }

        public void DeleteAllClusters()
        {
            _model.DeleteAllClusters();
        }

        public void DeleteAllInclusions()
        {
            _model.DeleteAllInclusions();
        }

        public void DeleteLayersInRange(int startDepth, int endDepth)
        {
            _model.DeleteLayersInRange(startDepth, endDepth);
        }

        # endregion

        # region Borehole section methods

        public void LoadFirstBoreholeSection()
        {
            _model.LoadFirstBoreholeSection();
        }

        public void LoadLastBoreholeSection()
        {
            _model.LoadLastBoreholeSection();
        }

        public void LoadPreviousBoreholeSection()
        {
            _model.LoadPreviousBoreholeSection();
        }

        public void LoadNextBoreholeSection()
        {
            _model.LoadNextBoreholeSection();
        }

        # endregion

        public FileTiler getNewTiler(int sectionHeight)
        {
            return _model.GetNewTiler(sectionHeight);
        }

        public void ExportImage(string fileName, bool withFeatures, bool withRuler, int currentSectionHeight)
        {
            _model.ExportImage(fileName, withFeatures, withRuler, currentSectionHeight);
        }
        
        # endregion

        public void StartProgressReport(string title)
        {
            _view.StartProgressReport(title);
        }

        public void UpdateProgress(int percentComplete)
        {
            _view.UpdateProgress(percentComplete);
        }

        public void EndProgressReport()
        {
            _view.EndProgressReport();
        }

        public string GetProjectLocation()
        {
            return _model.GetProjectLocation();
        }

        public void SetupLayerGroupsForm()
        {
            _model.SetupLayerGroupsForm();
        }

        public void SetupClusterGroupsForm()
        {
            _model.SetupClusterGroupsForm();
        }

        public void SetupInclusionGroupsForm()
        {
            _model.SetupInclusionGroupsForm();
        }

        public Color GetLayerColour(int layer)
        {
            return _model.GetLayerColour(layer);
        }

        public Color GetLayerColour(Layer layer)
        {
            return _model.GetLayerColour(layer);
        }

        public Color GetClusterColour(int cluster)
        {
            return _model.GetClusterColour(cluster);
        }

        public Color GetClusterColour(Cluster cluster)
        {
            return _model.GetClusterColour(cluster);
        }

        public Color GetInclusionColour(int inclusion)
        {
            return _model.GetInclusionColour(inclusion);
        }

        public Color GetInclusionColour(Inclusion inclusion)
        {
            return _model.GetInclusionColour(inclusion);
        }

        public void SetCurrentSectionComboBox(int currentSection)
        {
            _view.SetCurrentSectionComboBox(currentSection);
        }

        public void DrawAllFeatures(object sender, PaintEventArgs e)
        {
            _view.DrawAllFeatures(sender, e);
        }

        #region IModelAdapter Members

        public void enableNextSectionButton()
        {
            _view.EnableNextSectionButton();
        }

        public void disableNextSectionButton()
        {
            _view.DisableNextSectionButton();
        }

        public void enablePreviousSectionButton()
        {
            _view.EnablePreviousSectionButton();
        }

        public void disablePreviousSectionButton()
        {
            _view.DisablePreviousSectionButton();
        }

        public void disableFirstSectionButton()
        {
            _view.DisableFirstSectionButton();
        }

        public void enableFirstSectionButton()
        {
            _view.EnableFirstSectionButton();
        }

        public void disableLastSectionButton()
        {
            _view.DisableLastSectionButton();
        }

        public void enableLastSectionButton()
        {
            _view.EnableLastSectionButton();
        }   

        public void setTitleName(string boreholeName)
        {
            _view.SetTitleName(boreholeName);
        }

        public Bitmap getWholeBoreholeImage()
        {
            return _model.getWholeBoreholeImage();
        }
        
        public void displayImage(Bitmap boreholeImage)
        {
            _view.DisplayImage(boreholeImage);
        }

        /// <summary>
        /// Tells the view to display a borehole preview image
        /// </summary>
        /// <param name="boreholePreviewImage"></param>
        public void displayPreview(Bitmap boreholePreviewImage)
        {
            _view.changeBoreholeScroller(0.0);
            _view.DisplayPreview(boreholePreviewImage);            
        }

        /// <summary>
        /// Tells the view to display an entire borehole preview image
        /// </summary>
        /// <param name="boreholePreviewImage"></param>
        public void DisplayFullPreview(Bitmap fullPreviewImage)
        {
            _view.DisplayFullPreview(fullPreviewImage);
        }

        #endregion

        #region IViewAdapter Members

        public void OpenBoreholeImageFile(string fileName, string boreholeName)
        {
            _model.OpenBoreholeImageFile(fileName, boreholeName);
        }

        //public void otvFileOpened()
        //{
        //    _model.otvFileOpened();
        //}

        public void openOTVFile(string fileName)
        {            
                _model.OpenOTVFile(fileName);
        }

        public void openChannel(ReferencedChannel optvChannel, string workingPath)
        {
            _model.OpenChannel(optvChannel, workingPath);
        }

        public void setRulerImage(Bitmap ruler)
        {
            _view.SetRulerImage(ruler);
        }

        public FileTiler getCurrentTiler()
        {
            return _model.getCurrentTiler();
        }

        public void DeSelectFeature()
        {
            _model.deSelectFeature();
            _view.HideFeatureDetailsPropertyGrid();
        }

        /*public void setBoreholeSectionEnds(int sectionStartDepth, int sectionEndDepth)
        {
            _view.setBoreholeSectionEnds(sectionStartDepth, sectionEndDepth); 
        }*/

        public void LoadSection(int section)
        {
            _model.LoadSection(section);
        }

        public void LoadSectionAt(int yPos)
        {
            _model.LoadSectionAt(yPos);
        }

        public void setBoreholeSectionsComboBoxItems(int boreholeStartDepth, int boreholeEndDepth)
        {
            _view.SetBoreholeSectionsComboBoxItems(boreholeStartDepth, boreholeEndDepth);
        }

        public void createNewLayer(int depth, int amplitude, int azimuth)
        {
            _model.addNewLayer(depth, amplitude, azimuth);
        }

        public void CreateNewLayer(double slope, int intercept)
        {
            _model.AddNewLayer(slope, intercept);
        }

        public void createNewCluster()
        {
            _model.addNewCluster();
        }

        public void CreateNewInclusion()
        {
            _model.createNewInclusion();
        }
        
        public void AddToCurrentCluster(int xPoint, int yPoint)
        {
            _model.addToCurrentCluster(xPoint, yPoint);
        }

        public void AddToCurrentInclusion(int xPoint, int yPoint)
        {
            _model.addToCurrentInclusion(xPoint, yPoint);
        }        

        public void MovePoint(Point movingPoint, Point destination)
        {
            _model.movePoint(movingPoint, destination);
        }

        public void AddPoint(Point addPoint, int addAfter)
        {
            _model.AddPoint(addPoint, addAfter);
        }

        public void DeletePoint(Point deletePoint)
        {
            _model.deletePoint(deletePoint);
        }

        public int getNumOfLayers()
        {
            return _model.getNumOfLayers();
        }

        public int getNumOfClusters()
        {
            return _model.getNumOfClusters();
        }

        public int getNumOfInclusions()
        {
            return _model.getNumOfInclusions();
        }

        public bool IsFluidLevelSet()
        {
            return _model.getIsFluidLevelSet();
        }

        public int FluidLevel
        {
            get { return _model.FluidLevel; }
            set { _model.FluidLevel = value; }
        }

        public List<Point> getLayer1(int layerNum)
        {
            List<Point> points = _model.getLayerPoints1(layerNum);

            return points;
        }

        public List<Point> getLayer2(int layerNum)
        {
            List<Point> points = _model.getLayerPoints2(layerNum);

            return points;
        }

        public int GetLayerMin(int currentLayer)
        {
            return _model.GetLayerMin(currentLayer);
        }

        public int GetLayerMax(int currentLayer)
        {
            return _model.GetLayerMax(currentLayer);
        }

        public List<Point> GetClusterPoints(int clusterNum)
        {
            List<Point> points = _model.GetClusterPoints(clusterNum);

            return points;
        }

        public List<Point> getInclusion(int inclusionNum)
        {
            List<Point> points = _model.getInclusionPoints(inclusionNum);

            return points;
        }

        public bool OverCurrentFeature(int xPoint, int yPoint)
        {
            return _model.getIsOverCurrentFeature(xPoint, yPoint);
        }        
        
        public void DeleteCurrentFeature()
        {
            _model.deleteCurrentFeature();
            _view.HideFeatureDetailsPropertyGrid();
        }

        public object SelectedFeature
        {
            get { return _model.SelectedFeature; }
        }

        public string CurrentFeatureType
        {
            get { return _model.CurrentFeatureType; }
        }

        public void ChangeSelectedFeaturesGroup(string group)
        {
            _model.ChangeSelectedFeaturesGroup(group);
        }

        public void PointClicked(int xPoint, int yPoint)
        {
            _model.setActiveFeatureType(xPoint, yPoint);
        }

        public bool LayerAtLastClick
        {
            get { return _model.LayerAtLastClick; } 
        }

        public bool ClusterAtLastClick
        {
            get { return _model.ClusterAtLastClick; }
        }

        public bool InclusionAtLastClick
        {
            get { return _model.InclusionAtLastClick;}
        }

        public void DeleteCluster(Cluster clusterToDelete)
        {
            _model.deleteCluster(clusterToDelete);
        }

        public void DeleteInclusion(Inclusion inclusionToDelete)
        {
            _model.deleteInclusion(inclusionToDelete);
        }

        public void ClusterComplete()
        {
            _model.setLastClusterAsComplete();
        }

        public void InclusionComplete()
        {
            _model.setLastInclusionAsComplete();
        }

        public void MoveCurrentFeature(int sine, int xMove, int yMove)
        {
            _model.moveCurrentFeature(sine,xMove, yMove);
        }

        public void ChangeTopAmplitudeOfCurrentLayer(int yMove)
        {
            _model.changeTopAmplitudeOfSelectedLayer(yMove);
        }

        public void ChangeBottomAmplitudeOfCurrentLayer(int yMove)
        {
            _model.changeBottomAmplitudeOfSelectedLayer(yMove);
        }

        public void Reset()
        {
            _model.Reset();
        }

        public void SaveFeatures()
        {
            _model.saveFeatures();
        }

        public void LoadFeaturesFile(string fileName)
        {
            _model.LoadFeaturesFile(fileName);
        }

        public void SetFileName(string fileName)
        {
            _view.SetFileName(fileName);
        }

        /// <summary>
        /// Sent from AutoDetect via AnnotationToolForm
        /// </summary>
        /// <param name="layerToAdd"></param>
        public void AddCompleteLayer(Layer layerToAdd)
        {
            _model.AddCompleteLayer(layerToAdd);
            _view.RefreshBoreholeImage();
        }

        public int GetCurrentSectionStart()
        {
            return _model.GetCurrentSectionStart();
        }

        public int GetCurrentSectionEnd()
        {
            return _model.GetCurrentSectionEnd();
        }

        public void CreateDepthLuminosityProfileInExcel()
        {
            _model.CreateDepthLuminosityProfileInExcel();
        }

        public void CreateDepthLuminosityProfileInText()
        {
            _model.CreateDepthLuminosityProfileInText();
        }        

        public void JoinEdges(Layer previouslySelectedFeature, Layer currentlySelectedFeature)
        {
            _model.JoinEdges(previouslySelectedFeature, currentlySelectedFeature);
        }

        public void SplitLayer(Layer layerToSplit)
        {
            _model.SplitLayer(layerToSplit);
        }

        public List<string> GetLayerGroupsList()
        {
            return _model.GetLayerGroupsList();
        }

        public List<string> GetClusterGroupsList()
        {
            return _model.GetClusterGroupsList();
        }

        public List<string> GetInclusionGroupsList()
        {
            return _model.GetInclusionGroupsList();
        }

        public string GetCurrentFeaturesGroup()
        {
            return _model.GetCurrentFeaturesGroup();
        }

        public void RemoveLayersWithQualityLessThan(int quality)
        {
            _model.RemoveLayersWithQualityLessThan(quality);
        }

        #endregion

        public void ShowWellCADExport(bool show)
        {
            _view.ShowWellCADExport(show);
        }

    }
}
