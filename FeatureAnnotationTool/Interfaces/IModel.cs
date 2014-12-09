using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using BoreholeFeatures;
using ImageTiler;

namespace FeatureAnnotationTool.Interfaces
{
    /// <summary>
    /// Deals with calls from the controller to the model
    /// 
    /// Author - Terry Malone (trm8@aber.ac.uk)
    /// </summary>
    public interface IModel
    {        
        void setAdapter(IModelAdapter a);

        void OpenBoreholeImageFile(string fileName, string boreholeName);
        //void otvFileOpened();
        void OpenOTVFile(string fileName);

        void deSelectFeature();

        void addNewLayer(int depth, int amplitude, int azimuth);

        List<Point> getLayerPoints1(int layerNum);
        List<Point> getLayerPoints2(int layerNum);

        List<Point> GetClusterPoints(int clusterNum);

        List<Point> getInclusionPoints(int inclusionNum);

        int getNumOfLayers();

        int getNumOfClusters();

        int getNumOfInclusions();
        
        bool getIsOverCurrentFeature(int xPoint, int yPoint);

        int TopAzimuthOfSelectedLayer { get; }
        int BottomAzimuthOfSelectedLayer { get; }

        void deleteCurrentFeature();

        string CurrentFeatureType { get; }

        void setActiveFeatureType(int xPoint, int yPoint);

        object SelectedFeature { get; }

        void moveCurrentFeature(int sine, int xMove, int yMove);

        void changeTopAmplitudeOfSelectedLayer(int yMove);
        
        void changeBottomAmplitudeOfSelectedLayer(int yMove);
        
        void addNewCluster();

        void addToCurrentCluster(int xPoint, int yPoint);

        void setLastClusterAsComplete();

        void deleteCluster(Cluster clusterToDelete);

        void movePoint(Point movingPoint, Point destination);

        void deletePoint(Point deletePoint);

        void deleteInclusion(Inclusion inclusionToDelete);

        void createNewInclusion();

        void addToCurrentInclusion(int xPoint, int yPoint);

        void setLastInclusionAsComplete();

        void Reset();

        string BoreholeName { get; }

        void saveFeatures();

        void LoadFeaturesFile(string fileName);

        bool getIsFluidLevelSet();

        int FluidLevel{ get; set; }

        # region Export features methods

        void WriteAllFeaturesToExcel(string fileName, List<string> layerPropertiesToInclude, List<string> clusterPropertiesToInclude, List<string> inclusionPropertiesToInclude);

        void WriteLayersForWellCAD(string fileName);

        # endregion

        void AddCompleteLayer(Layer layerToAdd);

        void LoadPreviousBoreholeSection();

        void LoadNextBoreholeSection();

        int GetCurrentSectionStart();

        int GetCurrentSectionEnd();

        void LoadFirstBoreholeSection();

        FileTiler getCurrentTiler();

        Bitmap getWholeBoreholeImage();

        void CreateDepthLuminosityProfileInExcel();

        void OpenChannel(RGReferencedData.ReferencedChannel optvChannel, string workingPath);

        void DeleteAllLayers();

        FileTiler GetNewTiler(int sectionHeight);

        void JoinEdges(Layer previouslySelectedFeature, Layer currentlySelectedFeature);

        void WriteLayersToText(string fileName, List<string> layerPropertiesToInclude);

        void LoadLastBoreholeSection();

        void LoadSection(int section);
        
        void LoadSectionAt(int yPos);

        int GetLayerMin(int currentLayer);

        int GetLayerMax(int currentLayer);

        int BoreholeStartDepth { get; }

        int BoreholeEndDepth { get; }

        int DepthResolution { get; }

        void DeleteLayersInRange(int startDepth, int endDepth);

        void DeleteAllClusters();

        void DeleteAllInclusions();

        void SplitLayer(Layer layerToSplit);

        string[] GetLayerGroupNames();

        string[] GetClusterGroupNames();

        string[] GetInclusionGroupNames();

        void AddLayerGroup(string group);

        void AddClusterGroup(string group);

        void AddInclusionGroup(string group);

        Color GetLayerColour(int layer);

        List<Tuple<string, Color>> GetLayerGroups();

        void SetupLayerGroupsForm();

        Color GetLayerColour(Layer layer);

        void ChangeLayerGroupColour(string p, Color newColour);

        void RenameLayerGroup(string p, string p_2);

        int GetLayerGroupCount(string groupName);

        void DeleteLayerGroup(string groupName);

        string GetProjectLocation();

        void CreateDepthLuminosityProfileInText();
                
        void WriteClustersToText(string fileName, List<string> clusterPropertiesToIncude);

        void WriteInclusionsToText(string fileName, List<string> inclusionPropertiesToIncude);

        bool LayerAtLastClick { get; }

        List<string> GetLayerGroupsList();

        void ChangeSelectedFeaturesGroup(string group);

        string GetCurrentFeaturesGroup();

        int AzimuthResolution { get; }

        int BoreholeDepth { get; }

        string ProjectType { get; set; }

        string ImageType { get; }

        void AddNewLayer(double slope, int intercept);

        void ExportImage(string fileName, bool withFeatures, bool withRuler, int currentSectionHeight);

        void SaveGroups();

        void AddPoint(Point addPoint, int addAfter);

        void RemoveFluidLevel();

        void RenameClusterGroup(string p, string p_2);

        void DeleteClusterGroup(string p);

        List<Tuple<string, Color>> GetClusterGroups();

        int GetClusterGroupCount(string p);

        void ChangeClusterGroupColour(string p, Color newColour);

        void SetupClusterGroupsForm();

        void SetupInclusionGroupsForm();

        Color GetClusterColour(int cluster);

        Color GetInclusionColour(int inclusion);

        Color GetClusterColour(Cluster cluster);

        Color GetInclusionColour(Inclusion inclusion);

        List<Tuple<string, Color>> GetInclusionGroups();

        void ChangeInclusionGroupColour(string p, Color newColour);

        void DeleteInclusionGroup(string p);

        void RenameInclusionGroup(string p, string p_2);

        int GetInclusionGroupCount(string p);

        bool ClusterAtLastClick { get; }

        bool InclusionAtLastClick { get; }

        List<string> GetInclusionGroupsList();

        List<string> GetClusterGroupsList();

        void RemoveLayersWithQualityLessThan(int quality);

    }
}
