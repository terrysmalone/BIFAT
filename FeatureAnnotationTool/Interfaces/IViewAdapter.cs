﻿using System.Collections.Generic;
using System.Drawing;
using BoreholeFeatures;
using ImageTiler;
using RGReferencedData;

namespace FeatureAnnotationTool.Interfaces
{
    /// <summary>
    /// Deals with all calls from the View to the controller
    /// </summary>
    public interface IViewAdapter
    {
        void OpenBoreholeImageFile(string fileName, string boreholeName);
        
        void OpenOtvFile(string fileName);

        void DeSelectFeature();
        
        void CreateNewLayer(int firstDepth, int firstAmplitude, int firstAzimuth);

        void CreateNewCluster();

        List<Point> GetLayer1(int count);

        List<Point> GetLayer2(int count);

        List<Point> GetClusterPoints(int count);

        List<Point> GetInclusion(int count);

        int GetNumOfLayers();

        int GetNumOfClusters();

        int GetNumOfInclusions();

        bool OverCurrentFeature(int p, int p2);

        int GetTopAzimuthOfSelectedLayer();

        int GetBottomAzimuthOfSelectedLayer();

        object SelectedFeature { get; }

        string CurrentFeatureType { get; }

        void DeleteCurrentFeature();

        void PointClicked(int xPos, int yPos);

        void MoveCurrentFeature(int sineToMove, int xMove, int yMove);

        void ChangeTopAmplitudeOfCurrentLayer(int yMove);

        void ChangeBottomAmplitudeOfCurrentLayer(int yMove);
        
        void AddToCurrentCluster(int xPos, int yPos);

        void AddToCurrentInclusion(int xPos, int yPos);

        void ClusterComplete();

        void DeleteCluster(Cluster cluster);

        void MovePoint(Point movingPoint, Point destination);

        void DeletePoint(Point deletePoint);

        void DeleteInclusion(Inclusion inclusion);

        void CreateNewInclusion();

        void InclusionComplete();
        
        void Reset();

        string BoreholeName { get; }

        void SaveFeatures();

        void LoadFeaturesFile(string p);

        bool IsFluidLevelSet();

        int FluidLevel { get; set; }
        
        void WriteAllFeaturesToExcel(string fileName, List<string> layerPropertiesToInclude, List<string> clusterPropertiesToInclude, List<string> inclusionPropertiesToInclude);

        void AddCompleteLayer(Layer layerToAdd);

        void LoadPreviousBoreholeSection();

        void LoadNextBoreholeSection();

        int GetCurrentSectionStart();
        int GetCurrentSectionEnd();


        void LoadFirstBoreholeSection();

        FileTiler GetCurrentTiler();

        Bitmap GetWholeBoreholeImage();

        void CreateDepthLuminosityProfileInExcel();

        void OpenChannel(ReferencedChannel optvChannel, string workingPath);

        void DeleteLayersInRange(int startDepth, int endDepth);

        FileTiler GetNewTiler(int sectionHeight);

        void JoinEdges(Layer previouslySelectedFeature, Layer currentlySelectedFeature);

        void WriteLayersToText(string fileName, List<string> layerPropertiesToInclude);

        void LoadLastBoreholeSection();

        void LoadSection(int p);
        
        int GetLayerMax(int i);

        int GetLayerMin(int currentLayer);

        int BoreholeStartDepth { get; }

        int BoreholeEndDepth { get; }

        int DepthResolution { get; }

        void DeleteAllLayers();

        void DeleteAllInclusions();

        void DeleteAllClusters();

        void SplitLayer(Layer currentLayer);

        Color GetLayerColour(int layerToDraw);
        
        Color GetClusterColour(int clusterNum);

        Color GetInclusionColour(int inclusionNum);

        void SetupLayerGroupsForm();

        Color GetLayerColour(Layer layer);

        Color GetClusterColour(Cluster cluster);

        Color GetInclusionColour(Inclusion inclusion);

        string GetProjectLocation();

        void CreateDepthLuminosityProfileInText();
                
        void WriteClustersToText(string fileName, List<string> clusterProperties);

        void WriteInclusionsToText(string p, List<string> inclusionProperties);

        bool LayerAtLastClick { get; }

        List<string> GetLayerGroupsList();

        void ChangeSelectedFeaturesGroup(string p);

        string GetCurrentFeaturesGroup();

        int AzimuthResolution { get; }

        int BoreholeDepth { get; }

        void AddLayerGroup(string p);

        void AddClusterGroup(string p);

        void AddInclusionGroup(string p);
        
        string ImageType { get; }

        #region project methods

        void ExportImage(string fileName, bool withFeatures, bool withRuler, int currentSectionheight);
        
        string ProjectType { get; set; }

        void WriteLayersForWellCad(string fileName);

        void SaveGroups();

        #endregion project methods

        #region borehole methods

        void LoadSectionAt(int p);

        void RemoveFluidLevel();

        #endregion borehole methods

        #region layer methods

        void CreateNewLayer(double layerSlope, int layerIntercept);

        void RemoveLayersWithQualityLessThan(int quality);
        
        #endregion layer methods

        #region cluster methods

        void AddPoint(Point addPoint, int addAfter);
        
        List<string> GetClusterGroupsList();

        void SetupClusterGroupsForm();

        bool ClusterAtLastClick { get; }

        #endregion cluster methods

        #region inclusion methods

        List<string> GetInclusionGroupsList();

        void SetupInclusionGroupsForm();

        bool InclusionAtLastClick { get; }

        #endregion inclusion methods

    }
}
