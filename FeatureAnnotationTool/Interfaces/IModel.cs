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
    /// </summary>
    public interface IModel
    {        
        void SetAdapter(IModelAdapter a);

        void OpenBoreholeImageFile(string fileName, string boreholeName);

        void OpenOtvFile(string fileName);

        void DeSelectFeature();

        void AddNewLayer(int depth, int amplitude, int azimuth);

        List<Point> GetLayerPoints1(int layerNum);
        List<Point> GetLayerPoints2(int layerNum);

        List<Point> GetClusterPoints(int clusterNum);

        List<Point> GetInclusionPoints(int inclusionNum);

        int GetNumberOfLayers();

        int GetNumOfClusters();

        int GetNumOfInclusions();
        
        bool GetIsOverCurrentFeature(int xPoint, int yPoint);

        int GetTopAzimuthOfSelectedLayer();
        int GetBottomAzimuthOfSelectedLayer();

        void DeleteCurrentFeature();

        string CurrentFeatureType { get; }

        void SetActiveFeatureType(int xPoint, int yPoint);

        object SelectedFeature { get; }

        void MoveCurrentFeature(int sine, int xMove, int yMove);

        void ChangeTopAmplitudeOfSelectedLayer(int yMove);
        
        void ChangeBottomAmplitudeOfSelectedLayer(int yMove);
        
        void AddNewCluster();

        void AddToCurrentCluster(int xPoint, int yPoint);

        void SetLastClusterAsComplete();

        void DeleteCluster(Cluster clusterToDelete);

        void MovePoint(Point movingPoint, Point destination);

        void DeletePoint(Point deletePoint);

        void DeleteInclusion(Inclusion inclusionToDelete);

        void CreateNewInclusion();

        void AddToCurrentInclusion(int xPoint, int yPoint);

        void SetLastInclusionAsComplete();

        void Reset();

        string BoreholeName { get; }

        void SaveFeatures();

        void LoadFeaturesFile(string fileName);

        bool GetIsFluidLevelSet();

        int FluidLevel{ get; set; }

        # region Export features methods

        void WriteAllFeaturesToExcel(string fileName, List<string> layerPropertiesToInclude, List<string> clusterPropertiesToInclude, List<string> inclusionPropertiesToInclude);

        void WriteLayersForWellCad(string fileName);

        # endregion

        void AddCompleteLayer(Layer layerToAdd);

        void LoadPreviousBoreholeSection();

        void LoadNextBoreholeSection();

        int GetCurrentSectionStart();

        int GetCurrentSectionEnd();

        void LoadFirstBoreholeSection();

        FileTiler GetCurrentTiler();

        Bitmap GetWholeBoreholeImage();

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

        void RenameLayerGroup(string p, string p2);

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

        void RenameClusterGroup(string p, string p2);

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

        void RenameInclusionGroup(string p, string p2);

        int GetInclusionGroupCount(string p);

        bool ClusterAtLastClick { get; }

        bool InclusionAtLastClick { get; }

        List<string> GetInclusionGroupsList();

        List<string> GetClusterGroupsList();

        void RemoveLayersWithQualityLessThan(int quality);

    }
}
