using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FeatureAnnotationTool;
using FeatureAnnotationTool.Model;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using FeatureAnnotationTool.Interfaces;
using BoreholeFeatures;
//using Microsoft.Office.Core;

namespace FeatureAnnotationTool.Model
{
    /// <summary>
    /// A class which deals with all the featuresfound in the borehole image
    /// 
    /// Author - Terry Malone (trm8@aber.ac.uk)
    /// Version - 1.1 refactored
    /// </summary>
    class Features
    {
        object selectedFeature = "";
        string selectedFeatureType = "";
        private string boreholeName = "";

        private bool fluidLevelSet;
        private int fluidLevel;

        private int sourceStartDepth = 0;

        List<Layer> layers;
        List<Cluster> clusters;
        List<Inclusion> inclusions;

        private int selectedLayerPosition;

        private int depthResolution, azimuthResolution;

        private bool layerAtCurrentClick = false;
        private bool clusterAtCurrentClick = false;
        private bool inclusionAtCurrentClick = false;

        private int[] layerBrightnesses = null;
        private int[] clusterBrightnesses = null;
        private int[] inclusionBrightnesses = null;

        private IModel _model;

        private string[] allLayerGroups;
        private string[] allClusterGroups;
        private string[] allInclusionGroups;
        
        private string type;        //"Borehole" or "Core"

        #region properties
        
        public int SourceStartDepth
        {
            get { return sourceStartDepth; }
            set { sourceStartDepth = value; }
        }

        public int DepthResolution
        {
            get { return depthResolution; }
            set { depthResolution = value; }
        }

        public int AzimuthResolution
        {
            get { return azimuthResolution; }
            set { azimuthResolution = value; }
        }

        /// <summary>
        /// The boreholes fluid level
        /// </summary>
        /// <returns>The fluid level</returns>
        public int FluidLevel
        {
            get { return fluidLevel; }
            set
            {
                fluidLevel = value;
                fluidLevelSet = true;
            }
        }

        /// <summary>
        /// Whether the fluid level is set or not
        /// </summary>
        public bool FluidLevelSet
        {
            get { return fluidLevelSet; }
        }

        /// <summary>
        /// Method which returns the currently active feature
        /// </summary>
        /// <returns>The selected feature</returns>
        public object SelectedFeature
        {
            get { return selectedFeature; }
        }

        /// <summary>
        /// String representing the currently selected feature type
        /// </summary>
        /// <returns>The type of feature which is currently selected</returns>
        public string SelectedType
        {
            get { return selectedFeatureType; }
        }

        /// <summary>
        /// The name of the borehole 
        /// </summary>
        public string BoreholeName
        {
            get { return boreholeName; }
            set { boreholeName = value; }
        }

        internal string CurrentFeaturesGroup
        {
            get
            {
                string group = "";

                if (selectedFeatureType == "Layer")
                    group = ((Layer)selectedFeature).Group;
                else if (selectedFeatureType == "Cluster")
                    group = ((Cluster)selectedFeature).Group;
                else if (selectedFeatureType == "Inclusion")
                    group = ((Inclusion)selectedFeature).Group;

                return group;
            }
        }

        #region Layer properties

        public List<Layer> Layers
        {
            get { return layers; }
        }

        /// <summary>
        /// The number of layers in the feature list
        /// </summary>
        public int NumOfLayers
        {
            get { return layers.Count(); }
        }

        /// <summary>
        /// The top sines (lowest y value) azimuth of the selected layer
        /// </summary>
        public int TopAzimuthOfSelectedLayer
        {
            get
            {
                Layer selectedLayer = (Layer)selectedFeature;
                return selectedLayer.TopSineAzimuth;
            }
        }

        /// <summary>
        /// The bottom sines (lowest y value) azimuth of the selected layer
        /// </summary>
        public int BottomAzimuthOfSelectedLayer
        {
            get
            {
                Layer selectedLayer = (Layer)selectedFeature;
                return selectedLayer.BottomSineAzimuth;
            }
        }

        internal bool LayerAtLastClick
        {
            get { return layerAtCurrentClick; }
        }

        #endregion Layer properties

        #region Cluster properties

        public List<Cluster> Clusters
        {
            get { return clusters; }
        }

        public int NumOfClusters
        {
            get { return clusters.Count(); }
        }

        internal bool ClusterAtLastClick
        {
            get { return clusterAtCurrentClick; }
        }
        
        #endregion Cluster properties

        #region Inclusion properties

        public List<Inclusion> Inclusions
        {
            get { return inclusions; }
        }

        public int NumOfInclusions
        {
            get { return inclusions.Count(); }
        }

        internal bool InclusionAtLastClick
        {
            get { return inclusionAtCurrentClick; }
        }

        #endregion Inclusion properties

        #endregion properties

        #region constructor

        /// <summary>
        /// Constructor method
        /// </summary>
        public Features(IModel _model)
        {
            this._model = _model;

            layers = new List<Layer>();
            clusters = new List<Cluster>();
            inclusions = new List<Inclusion>();

            fluidLevelSet = false;
        }

        #endregion constructor

        /// <summary>
        /// Deselects the selected feature
        /// </summary>
        public void DeSelectFeature()
        {
            selectedFeature = "";
            selectedFeatureType = "";
        }
        
        /// <summary>
        /// Deletes the current feature
        /// </summary>
        public void DeleteActiveFeature()
        {
            if (selectedFeatureType.Equals("Layer"))
            {
                Layer activeLayer = (Layer)selectedFeature;
                layers.Remove(activeLayer);

            }
            else if (selectedFeatureType.Equals("Cluster"))
            {
                Cluster activeCluster = (Cluster)selectedFeature;
                clusters.Remove(activeCluster);
            }
            else if (selectedFeatureType.Equals("Inclusion"))
            {
                Inclusion activeInclusion = (Inclusion)selectedFeature;
                inclusions.Remove(activeInclusion);
            }

            selectedFeatureType = "";
            selectedFeature = "";
        }

        #region Write features methods

        /// <summary>
        /// Writes the current features to a specified excel document
        /// </summary>
        /// <param name="fileName">The name of the excel file to write to</param>
        public void WriteAllFeaturesToExcel(string fileName, List<string> layerPropertiesToInclude, List<string> clusterPropertiesToInclude, List<string> inclusionPropertiesToInclude)
        {
            WriteFeaturesToExcel writeFeatures = new WriteFeaturesToExcel(fileName, layers, clusters, inclusions);

            writeFeatures.SetLayerBrightnesses(layerBrightnesses);
            writeFeatures.SetClusterBrightnesses(clusterBrightnesses);
            writeFeatures.SetInclusionBrightnesses(inclusionBrightnesses);

            writeFeatures.AddLayerPropertiesToinclude(layerPropertiesToInclude);
            writeFeatures.AddClusterPropertiesToinclude(clusterPropertiesToInclude);
            writeFeatures.AddInclusionPropertiesToinclude(inclusionPropertiesToInclude);
            
            writeFeatures.WriteFile();
        }


        public void WriteLayersToText(string fileName, List<string> layerPropertiesToInclude)
        {
            using (StreamWriter stream = new StreamWriter(fileName))
            {
                stream.Write("# ");

                for (int i = 0; i < layerPropertiesToInclude.Count; i++)
                {
                    if(i == layerPropertiesToInclude.Count-1)
                        stream.Write(layerPropertiesToInclude[i]);
                    else
                        stream.Write(layerPropertiesToInclude[i] + ", ");
                }

                stream.WriteLine();

                string lastItem = layerPropertiesToInclude[layerPropertiesToInclude.Count - 1];

                for (int i = 0; i < layers.Count; i++)
                {
                    if (layerPropertiesToInclude.Contains("Start depth (mm)"))
                    {
                        stream.Write(layers[i].StartDepth);

                        if(lastItem != "Start depth (mm)")
                            stream.Write(", ");
                    }

                    if (layerPropertiesToInclude.Contains("End depth (mm)"))
                    {
                        stream.Write(layers[i].EndDepth);

                        if (lastItem != "End depth (mm)")
                            stream.Write(", ");
                    }

                    if (layerPropertiesToInclude.Contains("Top sine depth (mm)"))
                    {
                        stream.Write(layers[i].TopEdgeDepthMM);

                        if (lastItem != "Top sine depth (mm)") 
                            stream.Write(", ");
                    }

                    if (layerPropertiesToInclude.Contains("Top sine azimuth"))
                    {
                        stream.Write(layers[i].TopSineAzimuth);

                        if (lastItem != "Top sine azimuth") 
                            stream.Write(", ");
                    }

                    if (layerPropertiesToInclude.Contains("Top sine amplitude (mm)"))
                    {
                        stream.Write(layers[i].TopSineAmplitudeInMM);

                        if (lastItem != "Top sine amplitude (mm)")
                            stream.Write(", ");
                    }

                    if (layerPropertiesToInclude.Contains("Bottom sine depth (mm)"))
                    {
                        stream.Write(layers[i].BottomEdgeDepthMM);

                        if (lastItem != "Bottom sine depth (mm)")
                            stream.Write(", ");
                    }

                    if (layerPropertiesToInclude.Contains("Bottom sine azimuth"))
                    {
                        stream.Write(layers[i].BottomSineAzimuth);

                        if (lastItem != "Bottom sine azimuth")
                            stream.Write(", ");
                    }

                    if (layerPropertiesToInclude.Contains("Bottom sine amplitude (mm)"))
                    {
                        stream.Write(layers[i].BottomSineAmplitudeInMM);

                        if (lastItem != "Bottom sine amplitude (mm)")
                            stream.Write(", ");
                    }

                    # region Core only properties

                    if (layerPropertiesToInclude.Contains("Top edge intercept (mm)"))
                    {
                        stream.Write(layers[i].TopEdgeInterceptMm);

                        if (lastItem != "Top edge intercept (mm)")
                            stream.Write(", ");
                    }

                    if (layerPropertiesToInclude.Contains("Top edge slope"))
                    {
                        stream.Write(layers[i].TopEdgeSlope);

                        if (lastItem != "Top edge slope")
                            stream.Write(", ");
                    }

                    if (layerPropertiesToInclude.Contains("Bottom edge intercept (mm)"))
                    {
                        stream.Write(layers[i].BottomEdgeInterceptMm);

                        if (lastItem != "Bottom edge intercept (mm)")
                            stream.Write(", ");
                    }

                    if (layerPropertiesToInclude.Contains("Bottom edge slope"))
                    {
                        stream.Write(layers[i].BottomEdgeSlope);

                        if (lastItem != "Bottom edge slope")
                            stream.Write(", ");
                    }

                    # endregion

                    if (layerPropertiesToInclude.Contains("Group"))
                    {
                        stream.Write(layers[i].Group);

                        if (lastItem != "Group")
                            stream.Write(", ");
                    }

                    if (layerPropertiesToInclude.Contains("Layer type"))
                    {
                        stream.Write(layers[i].LayerType);

                        if (lastItem != "Layer type")
                            stream.Write(", ");
                    }

                    if (layerPropertiesToInclude.Contains("Layer description"))
                    {
                        stream.Write(layers[i].Description);

                        if (lastItem != "Layer description")
                            stream.Write(", ");
                    }

                    if (layerPropertiesToInclude.Contains("Layer quality"))
                    {
                        stream.Write(layers[i].Quality);

                        if (lastItem != "Layer quality")
                            stream.Write(", ");
                    }

                    if (layerPropertiesToInclude.Contains("Mean layer brightness"))
                    {
                        stream.Write(layerBrightnesses[i]);

                        if (lastItem != "Mean layer brightness")
                            stream.Write(", ");
                    }
                    
                    stream.WriteLine();
                }
            }
        }

        /// <summary>
        /// Writes a .war file containing all layers for use in WellCAD as a structure log
        /// </summary>
        /// <param name="fileName"></param>
        public void WriteLayersForWellCAD(string fileName)
        {
            using (StreamWriter stream = new StreamWriter(fileName))
            {
                stream.WriteLine("Depth,Azimuth,Dip,Aperture");
                stream.WriteLine("m,deg,deg,mm");
                
                for (int i = 0; i < layers.Count; i++)
                {
                    WriteEdgeDetailsForWellCAD(layers[i], stream);
                }
            }
        }

        private void WriteEdgeDetailsForWellCAD(Layer layer, StreamWriter stream)
        {
            double wellDepth;
            double wellAzimuth;
            double wellDip;
            double wellThickness;
                        
            wellDepth = (double)layer.TopEdgeDepthMM / 1000.0;
            wellAzimuth = (double)layer.TopSineAzimuth;
            wellDip = (double)layer.TopSineAmplitudeInMM;
            wellThickness = Math.Max((double)layer.TopEdgeDepthMM,(double)layer.BottomEdgeDepthMM) - Math.Min((double)layer.TopEdgeDepthMM, (double)layer.BottomEdgeDepthMM);

            stream.WriteLine(wellDepth + "," + wellAzimuth + "," + wellDip + "," + wellThickness);
        }

        public void WriteClustersToText(string fileName, List<string> clusterPropertiesToInclude)
        {
            using (StreamWriter stream = new StreamWriter(fileName))
            {
                stream.Write("# ");

                for (int i = 0; i < clusterPropertiesToInclude.Count; i++)
                {
                    if (i == clusterPropertiesToInclude.Count - 1)
                        stream.Write(clusterPropertiesToInclude[i]);
                    else
                        stream.Write(clusterPropertiesToInclude[i] + ", ");
                }

                stream.WriteLine();

                string lastItem = clusterPropertiesToInclude[clusterPropertiesToInclude.Count - 1];

                for (int i = 0; i < clusters.Count; i++)
                {
                    if (clusterPropertiesToInclude.Contains("Start depth (mm)"))
                    {
                        stream.Write(clusters[i].StartDepth);

                        if (lastItem != "Start depth (mm)")
                            stream.Write(", ");
                    }

                    if (clusterPropertiesToInclude.Contains("End depth (mm)"))
                    {
                        stream.Write(clusters[i].EndDepth);

                        if (lastItem != "End depth (mm)")
                            stream.Write(", ");
                    }

                    if (clusterPropertiesToInclude.Contains("Top Y boundary"))
                    {
                        stream.Write(clusters[i].TopYBoundary);

                        if (lastItem != "Top Y boundary")
                            stream.Write(", ");
                    }

                    if (clusterPropertiesToInclude.Contains("Bottom Y boundary"))
                    {
                        stream.Write(clusters[i].BottomYBoundary);

                        if (lastItem != "Bottom Y boundary")
                            stream.Write(", ");
                    }

                    if (clusterPropertiesToInclude.Contains("Left X boundary"))
                    {
                        stream.Write(clusters[i].LeftXBoundary);

                        if (lastItem != "Left X boundary")
                            stream.Write(", ");
                    }

                    if (clusterPropertiesToInclude.Contains("Right X boundary"))
                    {
                        stream.Write(clusters[i].RightXBoundary);

                        if (lastItem != "Right X boundary")
                            stream.Write(", ");
                    }

                    if (clusterPropertiesToInclude.Contains("Points"))
                    {
                        stream.Write(clusters[i].Points);

                        if (lastItem != "Points")
                            stream.Write(", ");
                    }

                    if (clusterPropertiesToInclude.Contains("Cluster type"))
                    {
                        stream.Write(clusters[i].ClusterType);

                        if (lastItem != "Cluster type")
                            stream.Write(", ");
                    }

                    if (clusterPropertiesToInclude.Contains("Cluster description"))
                    {
                        stream.Write(clusters[i].Description);

                        if (lastItem != "Cluster description")
                            stream.Write(", ");
                    }

                    if (clusterPropertiesToInclude.Contains("Mean cluster brightness"))
                    {
                        stream.Write(clusters[i].Description);

                        if (lastItem != "Mean cluster brightness")
                            stream.Write(", ");
                    }

                    stream.WriteLine();
                }
            }
        }

        public void WriteInclusionsToText(string fileName, List<string> inclusionPropertiesToInclude)
        {
            using (StreamWriter stream = new StreamWriter(fileName))
            {
                stream.Write("# ");

                for (int i = 0; i < inclusionPropertiesToInclude.Count; i++)
                {
                    if (i == inclusionPropertiesToInclude.Count - 1)
                        stream.Write(inclusionPropertiesToInclude[i]);
                    else
                        stream.Write(inclusionPropertiesToInclude[i] + ", ");
                }

                stream.WriteLine();

                string lastItem = inclusionPropertiesToInclude[inclusionPropertiesToInclude.Count - 1];

                for (int i = 0; i < inclusions.Count; i++)
                {
                    if (inclusionPropertiesToInclude.Contains("Start depth (mm)"))
                    {
                        stream.Write(inclusions[i].StartDepth);

                        if (lastItem != "Start depth (mm)")
                            stream.Write(", ");
                    }

                    if (inclusionPropertiesToInclude.Contains("End depth (mm)"))
                    {
                        stream.Write(inclusions[i].EndDepth);

                        if (lastItem != "End depth (mm)")
                            stream.Write(", ");
                    }

                    if (inclusionPropertiesToInclude.Contains("Top Y boundary"))
                    {
                        stream.Write(inclusions[i].TopYBoundary);

                        if (lastItem != "Top Y boundary")
                            stream.Write(", ");
                    }

                    if (inclusionPropertiesToInclude.Contains("Bottom Y boundary"))
                    {
                        stream.Write(inclusions[i].BottomYBoundary);

                        if (lastItem != "Bottom Y boundary")
                            stream.Write(", ");
                    }

                    if (inclusionPropertiesToInclude.Contains("Left X boundary"))
                    {
                        stream.Write(inclusions[i].LeftXBoundary);

                        if (lastItem != "Left X boundary")
                            stream.Write(", ");
                    }

                    if (inclusionPropertiesToInclude.Contains("Right X boundary"))
                    {
                        stream.Write(inclusions[i].RightXBoundary);

                        if (lastItem != "Right X boundary")
                            stream.Write(", ");
                    }

                    if (inclusionPropertiesToInclude.Contains("Points"))
                    {
                        stream.Write(inclusions[i].PointsString);

                        if (lastItem != "Points")
                            stream.Write(", ");
                    }

                    if (inclusionPropertiesToInclude.Contains("Inclusion type"))
                    {
                        stream.Write(inclusions[i].InclusionType);

                        if (lastItem != "Inclusion Type")
                            stream.Write(", ");
                    }

                    if (inclusionPropertiesToInclude.Contains("Inclusion description"))
                    {
                        stream.Write(inclusions[i].Description);

                        if (lastItem != "Inclusion Description")
                            stream.Write(", ");
                    }

                    if (inclusionPropertiesToInclude.Contains("Mean inclusion brightness"))
                    {
                        stream.Write(inclusions[i].Description);

                        if (lastItem != "Mean inclusion brightness")
                            stream.Write(", ");
                    }

                    stream.WriteLine();
                }
            }
        }

        # endregion

        public void RemoveFluidLevel()
        {
            fluidLevelSet = false;
        } 

        # region Layer methods

        /// <summary>
        /// Adds a new layer to the list of features
        /// </summary>
        /// <param name="depth">The depth of the first edge of the layer</param>
        /// <param name="amplitude">The amplitude of the first edge of the layer</param>
        /// <param name="azimuth">The azimuth of the first edge of the layer</param>
        public void AddNewLayer(int depth, int amplitude, int azimuth)
        {
            azimuth = (int)((float)azimuth / ((float)azimuthResolution / 360.0f));

            //layers.Add(new Layer(depth, amplitude, azimuth, depth, amplitude, azimuth, sourceAzimuthResolution, depthResolution));

            int insertPosition = CalculateLayerInsertPosition(depth);

            LayerTypeSelector layerTypeSelector;

            layerTypeSelector = new LayerTypeSelector("Borehole");
            Layer layer = layerTypeSelector.setUpLayer(depth, amplitude, azimuth, depth, amplitude, azimuth, azimuthResolution, depthResolution);
         
            layers.Insert(insertPosition, layer);

            selectedLayerPosition = insertPosition;
                        
            //selectedLayerPosition = layers.Count - 1;
            selectedFeature = layers[selectedLayerPosition];
            selectedFeatureType = "Layer";

            
            SetAllLayerGroupNames(allLayerGroups);
            ((Layer)selectedFeature).Group = "Unspecified";
            //layers[selectedLayerPosition].InitialiseModelAdapter(_model);
            SetStartDepth();
        }

        public void AddNewLayer(double slope, int intercept)
        {
            int insertPosition = CalculateLayerInsertPosition(intercept);

            LayerTypeSelector layerTypeSelector;
            
            layerTypeSelector = new LayerTypeSelector("Core");
            Layer layer = layerTypeSelector.setUpLayer(slope, intercept, slope, intercept, azimuthResolution, depthResolution);

            layers.Insert(insertPosition, layer);

            selectedLayerPosition = insertPosition;


            //selectedLayerPosition = layers.Count - 1;
            selectedFeature = layers[selectedLayerPosition];
            selectedFeatureType = "Layer";


            SetAllLayerGroupNames(allLayerGroups);
            ((Layer)selectedFeature).Group = "Unspecified";
            //layers[selectedLayerPosition].InitialiseModelAdapter(_model);
            SetStartDepth();  
        }

        /// <summary>
        /// Adds a completed layer to the list of features
        /// </summary>
        /// <param name="layerToAdd">The layer to add</param>
        public void AddCompleteLayer(Layer layerToAdd)
        {
            if (layerToAdd.Group == null)
            {
                layerToAdd.Group = "Unspecified";
                _model.AddLayerGroup(layerToAdd.Group);
                _model.SaveGroups();
            }

            int layerPosition = CalculateLayerInsertPosition(layerToAdd.TopEdgeDepth);
            layers.Insert(layerPosition, layerToAdd);

            SetAllLayerGroupNames(allLayerGroups);

            //layers[layerPosition].InitialiseModelAdapter(_model);
            //if(layerPosition <= selectedLayerPosition)
            //{
            //    selectedLayerPosition++;
            //    selectedFeature = layers[selectedLayerPosition];
            //}

            //layers.Add(layerToAdd);
        }

        private int CalculateLayerInsertPosition(int depth)
        {
            if (layers.Count == 0)
                return 0;

            if (layers.Count == 1)
            {
                if (depth < layers[0].TopEdgeDepth)
                    return 0;
                else 
                    return 1;
            }

            for (int i = 0; i < layers.Count; i++)
            {
                if(depth < layers[i].TopEdgeDepth)
                    return i;
            }

            return layers.Count;
        }

        /// <summary>
        /// Returns a List of Points from the top edge of a specified layer
        /// </summary>
        /// <param name="layerNum">The position of the layer</param>
        /// <returns>A List of layer points</returns>
        public List<Point> GetLayerPoints1(int layerNum)
        {
            return layers[layerNum].GetTopEdgePoints();
        }

        /// <summary>
        /// Returns a List of Points from the bottom edge of a specified layer
        /// </summary>
        /// <param name="layerNum">The position of the layer</param>
        /// <returns>A List of layer points</returns>
        public List<Point> GetLayerPoints2(int layerNum)
        {
            return layers[layerNum].GetBottomEdgePoints();
        }

        /// <summary>
        /// Returns a string representation of the group the layer at the given position belongs to
        /// </summary>
        /// <param name="layerNum">The positioin of the layer in the List</param>
        /// <returns>The group the layer belongs to</returns>
        public string GetLayerGroup(int layerNum)
        {
            return layers[layerNum].Group;
        }

        public int GetLayerMin(int layerNum)
        {
            return layers[layerNum].StartY;
        }

        public int GetLayerMax(int layerNum)
        {
            return layers[layerNum].EndY;
        }

        internal void DeleteLayersInRange(int startDepth, int endDepth)
        {
            int count = 0;
            bool stop = false;

            while (count < layers.Count && stop == false)
            {
                Layer currentLayer = layers[count];

                if (currentLayer.TopEdgeDepth >= startDepth && currentLayer.BottomEdgeDepth <= endDepth)
                {
                    layers.Remove(currentLayer);
                    count--;
                }
                else if (currentLayer.BottomEdgeDepth > endDepth)
                    stop = true;

                count++;
            }
        }

        internal void DeleteAllLayers()
        {
            layers.Clear();
        }

        public void RemoveLayersWithQualityLessThan(int quality)
        {
            for (int i = 0; i < layers.Count; i++)
            {
                if(layers[i].Quality < quality)                
                {
                    layers.RemoveAt(i);
                    i--;
                }
            }
        }

        public void JoinEdges(Layer previouslySelectedFeature, Layer currentlySelectedFeature)
        {
            /*if (currentlySelectedFeature.getTopDepth() < previouslySelectedFeature.getTopDepth())
            {
                currentlySelectedFeature.setBottomDepth(previouslySelectedFeature.getTopDepth());
                currentlySelectedFeature.setBottomAmplitude(previouslySelectedFeature.TopSineAmplitude);
                currentlySelectedFeature.setBottomAzimuth(previouslySelectedFeature.TopSineAzimuth);
            }
            else
            {
                currentlySelectedFeature.setTopDepth(previouslySelectedFeature.getTopDepth());
                currentlySelectedFeature.setTopAmplitude(previouslySelectedFeature.TopSineAmplitude);
                currentlySelectedFeature.setTopAzimuth(previouslySelectedFeature.TopSineAzimuth);
            }

            currentlySelectedFeature.Group = previouslySelectedFeature.Group);

            layers.Remove(previouslySelectedFeature);

            currentlySelectedFeature.moveSine(3, 0, 0);*/

            if (previouslySelectedFeature.TopEdgeDepth < currentlySelectedFeature.TopEdgeDepth)
            {
                if (type == "Borehole")
                {
                    previouslySelectedFeature.SetBottomEdgeDepth(currentlySelectedFeature.TopEdgeDepth);
                    previouslySelectedFeature.SetBottomSineAmplitude(currentlySelectedFeature.TopSineAmplitude);
                    previouslySelectedFeature.SetBottomSineAzimuth(currentlySelectedFeature.TopSineAzimuth);
                }
                else
                {
                    previouslySelectedFeature.SetBottomEdgeSlope(currentlySelectedFeature.TopEdgeSlope);
                    previouslySelectedFeature.SetBottomEdgeIntercept(currentlySelectedFeature.TopEdgeIntercept);
                }
            }
            else
            {
                if (type == "Borehole")
                {
                    previouslySelectedFeature.SetTopEdgeDepth(currentlySelectedFeature.TopEdgeDepth);
                    previouslySelectedFeature.SetTopSineAmplitude(currentlySelectedFeature.TopSineAmplitude);
                    previouslySelectedFeature.SetTopSineAzimuth(currentlySelectedFeature.TopSineAzimuth);
                }
                else
                {
                    previouslySelectedFeature.SetTopEdgeSlope(currentlySelectedFeature.TopEdgeSlope);
                    previouslySelectedFeature.SetTopEdgeIntercept(currentlySelectedFeature.TopEdgeIntercept);
                }
            }

            layers.Remove(currentlySelectedFeature);

            selectedFeature = previouslySelectedFeature;

            previouslySelectedFeature.MoveEdge(3, 0, 0);
        }

        /// <summary>
        /// Splits a two-edge layer into two one edge layers
        /// </summary>
        /// <param name="layerToSplit">The two edge layer to be split</param>
        public void SplitLayer(Layer layerToSplit)
        {
            int position = layers.IndexOf(layerToSplit);
            //int position = layers.FindIndex();

            LayerTypeSelector layerTypeSelector;

            layerTypeSelector = new LayerTypeSelector(type);

            Layer firstLayer, secondLayer;

            if (type == "Borehole")
            {
                firstLayer = layerTypeSelector.setUpLayer(layerToSplit.TopEdgeDepth, layerToSplit.TopSineAmplitude, layerToSplit.TopSineAzimuth, layerToSplit.TopEdgeDepth, layerToSplit.TopSineAmplitude, layerToSplit.TopSineAzimuth, azimuthResolution, depthResolution);
                secondLayer = layerTypeSelector.setUpLayer(layerToSplit.BottomEdgeDepth, layerToSplit.BottomSineAmplitude, layerToSplit.BottomSineAzimuth, layerToSplit.BottomEdgeDepth, layerToSplit.BottomSineAmplitude, layerToSplit.BottomSineAzimuth, azimuthResolution, depthResolution);
            }
            else
            {
                firstLayer = layerTypeSelector.setUpLayer(layerToSplit.TopEdgeSlope, layerToSplit.TopEdgeIntercept, layerToSplit.TopEdgeSlope, layerToSplit.TopEdgeIntercept, azimuthResolution, depthResolution);
                secondLayer = layerTypeSelector.setUpLayer(layerToSplit.BottomEdgeSlope, layerToSplit.BottomEdgeIntercept, layerToSplit.BottomEdgeSlope, layerToSplit.BottomEdgeIntercept, azimuthResolution, depthResolution);
            
            }
            //firstLayer.InitialiseModelAdapter(_model);
            //secondLayer.InitialiseModelAdapter(_model);

            firstLayer.Group = layerToSplit.Group;
            secondLayer.Group = layerToSplit.Group;
            
            /*
            firstLayer.Clean = layerToSplit.Clean;
            firstLayer.CoarseDebris = layerToSplit.CoarseDebris;
            firstLayer.Diamicton = layerToSplit.Diamicton;
            firstLayer.FineDebris = layerToSplit.FineDebris;
            firstLayer.LargeBubbles = layerToSplit.LargeBubbles;
            firstLayer.LayerVoid = layerToSplit.LayerVoid;
            firstLayer.SmallBubbles = layerToSplit.SmallBubbles;

            secondLayer.Clean = layerToSplit.Clean;
            secondLayer.CoarseDebris = layerToSplit.CoarseDebris;
            secondLayer.Diamicton = layerToSplit.Diamicton;
            secondLayer.FineDebris = layerToSplit.FineDebris;
            secondLayer.LargeBubbles = layerToSplit.LargeBubbles;
            secondLayer.LayerVoid = layerToSplit.LayerVoid;
            secondLayer.SmallBubbles = layerToSplit.SmallBubbles;
            */

            layers.Insert(position, secondLayer);
            layers.Insert(position, firstLayer);

            layers.Remove(layerToSplit);

            selectedFeature = layers[position];
        }

        # endregion

        # region Group methods

        internal void ChangeLayerGroupName(string groupNameBefore, string groupNameAfter)
        {
            for (int i = 0; i < layers.Count; i++)
            {
                if (layers[i].Group == groupNameBefore)
                    layers[i].Group = groupNameAfter;
            }
        }

        internal void ChangeClusterGroupName(string groupNameBefore, string groupNameAfter)
        {
            for (int i = 0; i < clusters.Count; i++)
            {
                if (clusters[i].Group == groupNameBefore)
                    clusters[i].Group = groupNameAfter;
            }
        }

        internal void ChangeInclusionGroupName(string groupNameBefore, string groupNameAfter)
        {
            for (int i = 0; i < inclusions.Count; i++)
            {
                if (inclusions[i].Group == groupNameBefore)
                    inclusions[i].Group = groupNameAfter;
            }
        }

        internal int GetLayerGroupCount(string groupName)
        {
            int count = 0;

            for (int i = 0; i < layers.Count; i++)
            {
                string layerName = layers[i].Group;

                if (layerName == groupName)
                    count++;
            }

            return count;
        }
        

        internal int GetClusterGroupCount(string groupName)
        {
            int count = 0;

            for (int i = 0; i < clusters.Count; i++)
            {
                string clusterName = clusters[i].Group;

                if (clusterName == groupName)
                    count++;
            }

            return count;
        }

        internal int GetInclusionGroupCount(string groupName)
        {
            int count = 0;

            for (int i = 0; i < inclusions.Count; i++)
            {
                string inclusionName = inclusions[i].Group;

                if (inclusionName == groupName)
                    count++;
            }

            return count;
        }

        # endregion

        # region Cluster methods

        public string GetClusterGroup(int clusterNum)
        {
            return clusters[clusterNum].Group;
        }

        /// <summary>
        /// Adds a cluster to the features
        /// </summary>
        public void AddCluster()
        {
            clusters.Add(new Cluster(azimuthResolution, depthResolution));
            selectedFeature = clusters[clusters.Count - 1];
            selectedFeatureType = "Cluster";

            SetAllClusterGroupNames(allClusterGroups);
            ((Cluster)selectedFeature).Group = "Unspecified";

            SetStartDepth();
        }

        /// <summary>
        /// Deletes a specified cluster from the features
        /// </summary>
        /// <param name="clusterToDelete">The cluster to delete</param>
        public void DeleteCluster(Cluster clusterToDelete)
        {
            clusters.Remove(clusterToDelete);
        }

        /// <summary>
        /// Removes all items from the Clusters List
        /// </summary>
        public void DeleteAllClusters()
        {
            clusters.Clear();
        }

        /// <summary>
        /// Sets the last added cluster as complete
        /// </summary>
        public void SetLastClusterAsComplete()
        {
            clusters[clusters.Count - 1].CheckFeatureIsWithinImageBounds();
            clusters[clusters.Count - 1].IsComplete = true;
        }

        /// <summary>
        /// Returns the number of clusters in the feature list
        /// </summary>
        /// <returns>The number of clusters</returns>
        public int GetNumOfClusters()
        {
            return clusters.Count();
        }

        /// <summary>
        /// Returns the List of points of a specified Cluster 
        /// </summary>
        /// <param name="clusterNum">The position of the Cluster</param>
        /// <returns>The List of Cluster Points</returns>
        public List<Point> GetClusterPoints(int clusterNum)
        {
            return clusters[clusterNum].Points;
        }

        # endregion

        # region Inclusion methods

        public string GetInclusionGroup(int inclusionNum)
        {
            return inclusions[inclusionNum].Group;
        }

        /// <summary>
        /// Adds an inclusion to the features
        /// </summary>
        public void AddInclusion()
        {
            inclusions.Add(new Inclusion(azimuthResolution, depthResolution));
            selectedFeature = inclusions[inclusions.Count - 1];
            selectedFeatureType = "Inclusion";

            SetAllInclusionGroupNames(allInclusionGroups);
            ((Inclusion)selectedFeature).Group = "Unspecified";
            
            SetStartDepth();
        }

        /// <summary>
        /// Deletes a specified inclusion
        /// </summary>
        /// <param name="inclusionToDelete">The inclusion to delete</param>
        public void DeleteInclusion(Inclusion inclusionToDelete)
        {
            inclusions.Remove(inclusionToDelete);
        }

        /// <summary>
        /// Removes all items from the Inclusions List
        /// </summary>
        public void DeleteAllInclusions()
        {
            inclusions.Clear();
        }

        /// <summary>
        /// Sets the last added inclusion as complete
        /// </summary>
        public void SetLastInclusionAsComplete()
        {
            inclusions[inclusions.Count - 1].CheckFeatureIsWithinImageBounds();
            inclusions[inclusions.Count - 1].IsComplete = true;
        }

        /// <summary>
        /// Returns the number of inclusions in the feature list
        /// </summary>
        /// <returns>The number of inclusions</returns>
        public int GetNumOfInlcusions()
        {
            return inclusions.Count();
        }

        /// <summary>
        /// Returns a List of a specified inclusions Points
        /// </summary>
        /// <param name="inclusionNum">The position of the inclusion to return</param>
        /// <returns>The List of Points</returns>
        public List<Point> GetInclusionPoints(int inclusionNum)
        {
            return inclusions[inclusionNum].Points;
        }

        # endregion
     
        # region Get methods

        internal int GetPositionOf(Layer feature)
        {
            int position = layers.IndexOf(feature);

            return position;
        }

        internal int GetPositionOf(Cluster feature)
        {
            int position = clusters.IndexOf(feature);

            return position;
        }

        internal int GetPositionOf(Inclusion feature)
        {
            int position = inclusions.IndexOf(feature);

            return position;
        }

        /// <summary>
        /// Returns all layers from the given list of group names
        /// </summary>
        /// <param name="excludeGroups"></param>
        /// <returns></returns>
        internal List<Layer> GetLayersInGroups(List<string> groupNames)
        {
            List<Layer> layersInGroups = new List<Layer>();

            for(int i=0; i<layers.Count; i++)
            {
                for (int j = 0; j < groupNames.Count; j++)
                {
                    if (layers[i].Group == groupNames[j])
                    {
                        layersInGroups.Add(layers[i]);
                        break;
                    }
                }
            }

            return layersInGroups;
        }

        # endregion

        # region Set methods
        
        /// <summary>
        /// Sets the start depth of the currently selected feature
        /// </summary>
        public void SetStartDepth()
        {
            if (selectedFeatureType.Equals("Layer"))
            {
                Layer currentLayer = (Layer)selectedFeature;
                currentLayer.SetBoreholeStartDepth(sourceStartDepth);
                Console.WriteLine("In FeaturesList - start depth being added: " + sourceStartDepth);
            }
            else if (selectedFeatureType.Equals("Cluster"))
            {
                Cluster currentCluster = (Cluster)selectedFeature;
                currentCluster.SourceStartDepth = sourceStartDepth;
            }
            else if (selectedFeatureType.Equals("Inclusion"))
            {
                Inclusion currentInclusion = (Inclusion)selectedFeature;
                currentInclusion.SourceStartDepth = sourceStartDepth;
            }

        }
        
        # region Set active feature type methods

        /// <summary>
        /// Checks if there is a feature at the selected co-ordinates and if so sets the equivalent feature type
        /// </summary>
        /// <param name="xPoint">The x position to check</param>
        /// <param name="yPoint">The y position to check</param>
        public void setActiveFeatureType(int xPoint, int yPoint)
        {
            layerAtCurrentClick = false;
            clusterAtCurrentClick = false;
            inclusionAtCurrentClick = false;

            checkIfLayerIsAtPoint(xPoint, yPoint);

            CheckIfClusterIsAtPoint(xPoint, yPoint);

            checkIfInclusionIsAtPoint(xPoint, yPoint);            
        }
        
        public void checkIfLayerIsAtPoint(int xPoint, int yPoint)
        {
            int high, low;

            //Check layers
            for (int i = 0; i < layers.Count; i++)
            {
                low = layers[i].GetTopEdgePoints()[xPoint].Y;
                high = layers[i].GetBottomEdgePoints()[xPoint].Y;

                if (yPoint > low - 6 && yPoint < high + 6)
                {
                    if (selectedFeatureType.Equals("Cluster") && clusters[clusters.Count - 1].IsComplete == false)
                    {
                        clusters.Remove((Cluster)selectedFeature);
                    }
                    else if (selectedFeatureType.Equals("Inclusion") && inclusions[inclusions.Count - 1].IsComplete == false)
                    {
                        inclusions.Remove((Inclusion)selectedFeature);
                    }

                    //Same for inclusion

                    selectedFeatureType = "Layer";
                    selectedFeature = layers[i];

                    layerAtCurrentClick = true;
                }
            }
        }

        /// <summary>
        /// Checks if the given point is within one of the clusters
        /// </summary>
        /// <param name="xPoint"></param>
        /// <param name="yPoint"></param>
        public bool CheckIfClusterIsAtPoint(int xPoint, int yPoint)
        {
            for (int i = 0; i < clusters.Count; i++)
            {
                List<Point> clusterPoints = clusters[i].Points;

                if (isPointWithinPolygon(xPoint, yPoint, clusterPoints))
                {

                    if (selectedFeatureType.Equals("Inclusion") && inclusions[inclusions.Count - 1].IsComplete == false)
                    {
                        inclusions.Remove((Inclusion)selectedFeature);
                    }

                    selectedFeatureType = "Cluster";
                    selectedFeature = clusters[i];

                    clusterAtCurrentClick = true;

                    return true;
                }
                else if (PointsAreNegative(clusterPoints))
                {
                    clusterPoints = MoveAllPointsXparameter(clusterPoints, azimuthResolution);

                    if (isPointWithinPolygon(xPoint, yPoint, clusterPoints))
                    {

                        if (selectedFeatureType.Equals("Inclusion") && inclusions[inclusions.Count - 1].IsComplete == false)
                        {
                            inclusions.Remove((Inclusion)selectedFeature);
                        }

                        selectedFeatureType = "Cluster";
                        selectedFeature = clusters[i];

                        clusterAtCurrentClick = true;

                        return true;
                    }
                }
                else if (PointsArePositive(clusterPoints))
                {
                    clusterPoints = MoveAllPointsXparameter(clusterPoints, -azimuthResolution);

                    if (isPointWithinPolygon(xPoint, yPoint, clusterPoints))
                    {

                        if (selectedFeatureType.Equals("Inclusion") && inclusions[inclusions.Count - 1].IsComplete == false)
                        {
                            inclusions.Remove((Inclusion)selectedFeature);
                        }

                        selectedFeatureType = "Cluster";
                        selectedFeature = clusters[i];

                        clusterAtCurrentClick = true;
                        
                        return true;
                    }
                }
            }

            return false;
        }

        private bool PointsAreNegative(List<Point> points)
        {
            for (int i = 0; i < points.Count; i++)
            {
                if (points[i].X < 0)
                    return true;
            }

            return false;
        }

        private bool PointsArePositive(List<Point> points)
        {
            for (int i = 0; i < points.Count; i++)
            {
                if (points[i].X >= azimuthResolution)
                    return true;
            }

            return false;
        }

        private List<Point> MoveAllPointsXparameter(List<Point> points, int amountToMove)
        {
            List<Point> pointsAfterMove = new List<Point>();

            for (int i = 0; i < points.Count; i++)
            {
                pointsAfterMove.Add(new Point(points[i].X + amountToMove, points[i].Y));
            }

            return pointsAfterMove;
        }

        public bool checkIfInclusionIsAtPoint(int xPoint, int yPoint)
        {
            for (int i = 0; i < inclusions.Count; i++)
            {
                List<Point> inclusionPoints = inclusions[i].Points;

                if (isPointWithinPolygon(xPoint, yPoint, inclusionPoints))
                {
                    if (selectedFeatureType.Equals("Cluster") && clusters[clusters.Count - 1].IsComplete == false)
                    {
                        clusters.Remove((Cluster)selectedFeature);
                    }

                    selectedFeatureType = "Inclusion";
                    selectedFeature = inclusions[i];

                    inclusionAtCurrentClick = true;

                    return true;
                }
                else if (PointsAreNegative(inclusionPoints))
                {
                    inclusionPoints = MoveAllPointsXparameter(inclusionPoints, azimuthResolution);

                    if (isPointWithinPolygon(xPoint, yPoint, inclusionPoints))
                    {
                        if (selectedFeatureType.Equals("Cluster") && clusters[clusters.Count - 1].IsComplete == false)
                        {
                            clusters.Remove((Cluster)selectedFeature);
                        }

                        selectedFeatureType = "Inclusion";
                        selectedFeature = inclusions[i];

                        inclusionAtCurrentClick = true;

                        return true;
                    }
                }
                else if (PointsArePositive(inclusionPoints))
                {
                    inclusionPoints = MoveAllPointsXparameter(inclusionPoints, -azimuthResolution);

                    if (isPointWithinPolygon(xPoint, yPoint, inclusionPoints))
                    {
                        if (selectedFeatureType.Equals("Cluster") && clusters[clusters.Count - 1].IsComplete == false)
                        {
                            clusters.Remove((Cluster)selectedFeature);
                        }

                        selectedFeatureType = "Inclusion";
                        selectedFeature = inclusions[i];

                        inclusionAtCurrentClick = true;

                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Checks if a given point is within the bounds of a given polygon
        /// </summary>
        /// <param name="xPoint">The x position of the point to check</param>
        /// <param name="yPoint">The y position of the point to check</param>
        /// <param name="polygonPoints">A List of all the corner Points of the polgon</param>
        /// <returns></returns>
        private bool isPointWithinPolygon(int xPoint, int yPoint, List<Point> polygonPoints)
        {
            Point p1, p2;

            bool inside = false;

            if (polygonPoints.Count < 3)
            {
                return inside;
            }

            Point oldPoint = new Point(polygonPoints[polygonPoints.Count - 1].X, polygonPoints[polygonPoints.Count - 1].Y);

            for (int i = 0; i < polygonPoints.Count; i++)
            {
                Point newPoint = new Point(polygonPoints[i].X, polygonPoints[i].Y);

                if (newPoint.X > oldPoint.X)
                {
                    p1 = oldPoint;
                    p2 = newPoint;
                }
                else
                {
                    p1 = newPoint;
                    p2 = oldPoint;
                }

                if ((newPoint.X < xPoint) == (xPoint <= oldPoint.X) && ((long)yPoint - (long)p1.Y) * (long)(p2.X - p1.X) < ((long)p2.Y - (long)p1.Y) * (long)(xPoint - p1.X))
                {
                    inside = !inside;
                }

                oldPoint = newPoint;
            }

            return inside;
        }

        # endregion

        public void SetLayers(List<Layer> layers)
        {
            this.layers = layers;

            for (int i = 0; i < layers.Count; i++)
            {
                if (layers[i].Group == null)
                {
                    layers[i].Group = "Unspecified";
                    _model.AddLayerGroup(layers[i].Group);
                    _model.SaveGroups();
                }

                layers[i].SetAllGroupNames(allLayerGroups);
            }
        }

        public void SetClusters(List<Cluster> clusters)
        {
            this.clusters = clusters;
            
            for (int i = 0; i < clusters.Count; i++)
            {
                if (clusters[i].Group == null)
                {
                    clusters[i].Group = "Unspecified";
                    _model.AddClusterGroup(clusters[i].Group);
                    _model.SaveGroups();
                }

                clusters[i].SetAllGroupNames(allClusterGroups);
            }
        }

        public void SetInclusions(List<Inclusion> inclusions)
        {
            this.inclusions = inclusions;

            for (int i = 0; i < inclusions.Count; i++)
            {
                if (inclusions[i].Group == null)
                {
                    inclusions[i].Group = "Unspecified";
                    _model.AddInclusionGroup(inclusions[i].Group);
                    _model.SaveGroups();
                }

                inclusions[i].SetAllGroupNames(allInclusionGroups);
            }
        }

        internal void SetAllLayerGroupNames(string[] allGroups)
        {
            this.allLayerGroups = allGroups;

            for (int i = 0; i < layers.Count; i++)
            {
                layers[i].SetAllGroupNames(allGroups);
            }
        }

        internal void SetAllClusterGroupNames(string[] allGroups)
        {
            this.allClusterGroups = allGroups;

            for (int i = 0; i < clusters.Count; i++)
            {
                clusters[i].SetAllGroupNames(allGroups);
            }
        }

        internal void SetAllInclusionGroupNames(string[] allGroups)
        {
            this.allInclusionGroups = allGroups;

            for (int i = 0; i < inclusions.Count; i++)
            {
                inclusions[i].SetAllGroupNames(allGroups);
            }
        }

        internal void SetImageType(string type)
        {
            this.type = type;
        }

        internal void SetLayerBrightnesses(int[] layerBrightnesses)
        {
            this.layerBrightnesses = layerBrightnesses;
        }

        internal void SetClusterBrightnesses(int[] clusterBrightnesses)
        {
            this.clusterBrightnesses = clusterBrightnesses;
        }

        internal void SetInclusionBrightnesses(int[] inclusionBrightnesses)
        {
            this.inclusionBrightnesses = inclusionBrightnesses;
        }
        
        # endregion set methods

        internal void ChangeSelectedFeaturesGroup(string group)
        {
            if(selectedFeatureType == "Layer")
                ((Layer)selectedFeature).Group = group;
            else if (selectedFeatureType == "Cluster")
                ((Cluster)selectedFeature).Group = group;
            else if (selectedFeatureType == "Inclusion")
                ((Inclusion)selectedFeature).Group = group;
        }
    }
}
