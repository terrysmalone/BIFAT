using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.IO;
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
    internal class Features
    {
        private object m_SelectedFeature = "";
        private string m_SelectedFeatureType = "";
        private string m_BoreholeName = "";

        private bool fluidLevelSet;
        private int fluidLevel;

        List<Layer> m_Layers;
        List<Cluster> m_Clusters;
        List<Inclusion> m_Inclusions;

        private int selectedLayerPosition;

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
        
        public int SourceStartDepth { get; set; } = 0;

        public int DepthResolution { get; set; }

        public int AzimuthResolution { get; set; }

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
            get { return m_SelectedFeature; }
        }

        /// <summary>
        /// String representing the currently selected feature type
        /// </summary>
        /// <returns>The type of feature which is currently selected</returns>
        public string SelectedType
        {
            get { return m_SelectedFeatureType; }
        }

        /// <summary>
        /// The name of the borehole 
        /// </summary>
        public string BoreholeName
        {
            get { return m_BoreholeName; }
            set { m_BoreholeName = value; }
        }

        internal string CurrentFeaturesGroup
        {
            get
            {
                string group = "";

                if (m_SelectedFeatureType == "Layer")
                    group = ((Layer)m_SelectedFeature).Group;
                else if (m_SelectedFeatureType == "Cluster")
                    group = ((Cluster)m_SelectedFeature).Group;
                else if (m_SelectedFeatureType == "Inclusion")
                    group = ((Inclusion)m_SelectedFeature).Group;

                return group;
            }
        }

        #region Layer properties

        public List<Layer> Layers
        {
            get { return m_Layers; }
        }

        /// <summary>
        /// The number of layers in the feature list
        /// </summary>
        public int NumOfLayers
        {
            get { return m_Layers.Count(); }
        }

        /// <summary>
        /// The top sines (lowest y value) azimuth of the selected layer
        /// </summary>
        public int TopAzimuthOfSelectedLayer
        {
            get
            {
                var selectedLayer = (BoreholeLayer)m_SelectedFeature;

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
                var selectedLayer = (BoreholeLayer)m_SelectedFeature;

                return selectedLayer.BottomSineAzimuth;
            }
        }

        internal bool LayerAtLastClick { get; private set; }

#endregion Layer properties

        #region Cluster properties

        public List<Cluster> Clusters
        {
            get { return m_Clusters; }
        }

        public int NumOfClusters
        {
            get { return m_Clusters.Count(); }
        }

        internal bool ClusterAtLastClick
        {
            get { return clusterAtCurrentClick; }
        }
        
        #endregion Cluster properties

        #region Inclusion properties

        public List<Inclusion> Inclusions
        {
            get { return m_Inclusions; }
        }

        public int NumOfInclusions
        {
            get { return m_Inclusions.Count(); }
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

            m_Layers = new List<Layer>();
            m_Clusters = new List<Cluster>();
            m_Inclusions = new List<Inclusion>();

            fluidLevelSet = false;
        }

        #endregion constructor

        /// <summary>
        /// Deselects the selected feature
        /// </summary>
        public void DeSelectFeature()
        {
            m_SelectedFeature = "";
            m_SelectedFeatureType = "";
        }
        
        /// <summary>
        /// Deletes the current feature
        /// </summary>
        public void DeleteActiveFeature()
        {
            if (m_SelectedFeatureType.Equals("Layer"))
            {
                Layer activeLayer = (Layer)m_SelectedFeature;
                m_Layers.Remove(activeLayer);

            }
            else if (m_SelectedFeatureType.Equals("Cluster"))
            {
                Cluster activeCluster = (Cluster)m_SelectedFeature;
                m_Clusters.Remove(activeCluster);
            }
            else if (m_SelectedFeatureType.Equals("Inclusion"))
            {
                Inclusion activeInclusion = (Inclusion)m_SelectedFeature;
                m_Inclusions.Remove(activeInclusion);
            }

            m_SelectedFeatureType = "";
            m_SelectedFeature = "";
        }

        #region Write features methods

        /// <summary>
        /// Writes the current features to a specified excel document
        /// </summary>
        /// <param name="fileName">The name of the excel file to write to</param>
        public void WriteAllFeaturesToExcel(string fileName, List<string> layerPropertiesToInclude, List<string> clusterPropertiesToInclude, List<string> inclusionPropertiesToInclude)
        {
            WriteFeaturesToExcel writeFeatures = new WriteFeaturesToExcel(fileName, m_Layers, m_Clusters, m_Inclusions);

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

                for (int i = 0; i < m_Layers.Count; i++)
                {
                    if (layerPropertiesToInclude.Contains("Start depth (mm)"))
                    {
                        stream.Write(m_Layers[i].StartDepth);

                        if(lastItem != "Start depth (mm)")
                            stream.Write(", ");
                    }

                    if (layerPropertiesToInclude.Contains("End depth (mm)"))
                    {
                        stream.Write(m_Layers[i].EndDepth);

                        if (lastItem != "End depth (mm)")
                            stream.Write(", ");
                    }

                    if (layerPropertiesToInclude.Contains("Top sine depth (mm)"))
                    {
                        stream.Write(m_Layers[i].TopEdgeDepthMm);

                        if (lastItem != "Top sine depth (mm)") 
                            stream.Write(", ");
                    }

                    if (layerPropertiesToInclude.Contains("Top sine azimuth"))
                    {
                        stream.Write(((BoreholeLayer)m_Layers[i]).TopSineAzimuth);

                        if (lastItem != "Top sine azimuth") 
                            stream.Write(", ");
                    }

                    if (layerPropertiesToInclude.Contains("Top sine amplitude (mm)"))
                    {
                        stream.Write(((BoreholeLayer)m_Layers[i]).TopSineAmplitudeInMm);

                        if (lastItem != "Top sine amplitude (mm)")
                            stream.Write(", ");
                    }

                    if (layerPropertiesToInclude.Contains("Bottom sine depth (mm)"))
                    {
                        stream.Write(m_Layers[i].BottomEdgeDepthMm);

                        if (lastItem != "Bottom sine depth (mm)")
                            stream.Write(", ");
                    }

                    if (layerPropertiesToInclude.Contains("Bottom sine azimuth"))
                    {
                        stream.Write(((BoreholeLayer)m_Layers[i]).BottomSineAzimuth);

                        if (lastItem != "Bottom sine azimuth")
                            stream.Write(", ");
                    }

                    if (layerPropertiesToInclude.Contains("Bottom sine amplitude (mm)"))
                    {
                        stream.Write(((BoreholeLayer)m_Layers[i]).BottomSineAmplitudeInMm);

                        if (lastItem != "Bottom sine amplitude (mm)")
                            stream.Write(", ");
                    }

                    # region Core only properties

                    if (layerPropertiesToInclude.Contains("Top edge intercept (mm)"))
                    {
                        stream.Write(((CoreLayer)m_Layers[i]).TopEdgeInterceptMm);

                        if (lastItem != "Top edge intercept (mm)")
                            stream.Write(", ");
                    }

                    if (layerPropertiesToInclude.Contains("Top edge slope"))
                    {
                        stream.Write(((CoreLayer)m_Layers[i]).TopEdgeSlope);

                        if (lastItem != "Top edge slope")
                            stream.Write(", ");
                    }

                    if (layerPropertiesToInclude.Contains("Bottom edge intercept (mm)"))
                    {
                        stream.Write(((CoreLayer)m_Layers[i]).BottomEdgeInterceptMm);

                        if (lastItem != "Bottom edge intercept (mm)")
                            stream.Write(", ");
                    }

                    if (layerPropertiesToInclude.Contains("Bottom edge slope"))
                    {
                        stream.Write(((CoreLayer)m_Layers[i]).BottomEdgeSlope);

                        if (lastItem != "Bottom edge slope")
                            stream.Write(", ");
                    }

                    # endregion

                    if (layerPropertiesToInclude.Contains("Group"))
                    {
                        stream.Write(m_Layers[i].Group);

                        if (lastItem != "Group")
                            stream.Write(", ");
                    }

                    if (layerPropertiesToInclude.Contains("Layer type"))
                    {
                        stream.Write(m_Layers[i].LayerType);

                        if (lastItem != "Layer type")
                            stream.Write(", ");
                    }

                    if (layerPropertiesToInclude.Contains("Layer description"))
                    {
                        stream.Write(m_Layers[i].Description);

                        if (lastItem != "Layer description")
                            stream.Write(", ");
                    }

                    if (layerPropertiesToInclude.Contains("Layer quality"))
                    {
                        stream.Write(m_Layers[i].Quality);

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
                
                for (int i = 0; i < m_Layers.Count; i++)
                {
                    WriteEdgeDetailsForWellCAD(m_Layers[i] as BoreholeLayer, stream);
                }
            }
        }

        private void WriteEdgeDetailsForWellCAD(BoreholeLayer layer, StreamWriter stream)
        {
            double wellDepth;
            double wellAzimuth;
            double wellDip;
            double wellThickness;
                        
            wellDepth = layer.TopEdgeDepthMm / 1000.0;
            wellAzimuth = (double)layer.TopSineAzimuth;
            wellDip = (double)layer.TopSineAmplitudeInMm;
            wellThickness = Math.Max((double)layer.TopEdgeDepthMm,(double)layer.BottomEdgeDepthMm) - Math.Min((double)layer.TopEdgeDepthMm, (double)layer.BottomEdgeDepthMm);

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

                for (int i = 0; i < m_Clusters.Count; i++)
                {
                    if (clusterPropertiesToInclude.Contains("Start depth (mm)"))
                    {
                        stream.Write(m_Clusters[i].StartDepth);

                        if (lastItem != "Start depth (mm)")
                            stream.Write(", ");
                    }

                    if (clusterPropertiesToInclude.Contains("End depth (mm)"))
                    {
                        stream.Write(m_Clusters[i].EndDepth);

                        if (lastItem != "End depth (mm)")
                            stream.Write(", ");
                    }

                    if (clusterPropertiesToInclude.Contains("Top Y boundary"))
                    {
                        stream.Write(m_Clusters[i].TopYBoundary);

                        if (lastItem != "Top Y boundary")
                            stream.Write(", ");
                    }

                    if (clusterPropertiesToInclude.Contains("Bottom Y boundary"))
                    {
                        stream.Write(m_Clusters[i].BottomYBoundary);

                        if (lastItem != "Bottom Y boundary")
                            stream.Write(", ");
                    }

                    if (clusterPropertiesToInclude.Contains("Left X boundary"))
                    {
                        stream.Write(m_Clusters[i].LeftXBoundary);

                        if (lastItem != "Left X boundary")
                            stream.Write(", ");
                    }

                    if (clusterPropertiesToInclude.Contains("Right X boundary"))
                    {
                        stream.Write(m_Clusters[i].RightXBoundary);

                        if (lastItem != "Right X boundary")
                            stream.Write(", ");
                    }

                    if (clusterPropertiesToInclude.Contains("Points"))
                    {
                        stream.Write(m_Clusters[i].Points);

                        if (lastItem != "Points")
                            stream.Write(", ");
                    }

                    if (clusterPropertiesToInclude.Contains("Cluster type"))
                    {
                        stream.Write(m_Clusters[i].ClusterType);

                        if (lastItem != "Cluster type")
                            stream.Write(", ");
                    }

                    if (clusterPropertiesToInclude.Contains("Cluster description"))
                    {
                        stream.Write(m_Clusters[i].Description);

                        if (lastItem != "Cluster description")
                            stream.Write(", ");
                    }

                    if (clusterPropertiesToInclude.Contains("Mean cluster brightness"))
                    {
                        stream.Write(m_Clusters[i].Description);

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

                for (int i = 0; i < m_Inclusions.Count; i++)
                {
                    if (inclusionPropertiesToInclude.Contains("Start depth (mm)"))
                    {
                        stream.Write(m_Inclusions[i].StartDepth);

                        if (lastItem != "Start depth (mm)")
                            stream.Write(", ");
                    }

                    if (inclusionPropertiesToInclude.Contains("End depth (mm)"))
                    {
                        stream.Write(m_Inclusions[i].EndDepth);

                        if (lastItem != "End depth (mm)")
                            stream.Write(", ");
                    }

                    if (inclusionPropertiesToInclude.Contains("Top Y boundary"))
                    {
                        stream.Write(m_Inclusions[i].TopYBoundary);

                        if (lastItem != "Top Y boundary")
                            stream.Write(", ");
                    }

                    if (inclusionPropertiesToInclude.Contains("Bottom Y boundary"))
                    {
                        stream.Write(m_Inclusions[i].BottomYBoundary);

                        if (lastItem != "Bottom Y boundary")
                            stream.Write(", ");
                    }

                    if (inclusionPropertiesToInclude.Contains("Left X boundary"))
                    {
                        stream.Write(m_Inclusions[i].LeftXBoundary);

                        if (lastItem != "Left X boundary")
                            stream.Write(", ");
                    }

                    if (inclusionPropertiesToInclude.Contains("Right X boundary"))
                    {
                        stream.Write(m_Inclusions[i].RightXBoundary);

                        if (lastItem != "Right X boundary")
                            stream.Write(", ");
                    }

                    if (inclusionPropertiesToInclude.Contains("Points"))
                    {
                        stream.Write(m_Inclusions[i].PointsString);

                        if (lastItem != "Points")
                            stream.Write(", ");
                    }

                    if (inclusionPropertiesToInclude.Contains("Inclusion type"))
                    {
                        stream.Write(m_Inclusions[i].InclusionType);

                        if (lastItem != "Inclusion Type")
                            stream.Write(", ");
                    }

                    if (inclusionPropertiesToInclude.Contains("Inclusion description"))
                    {
                        stream.Write(m_Inclusions[i].Description);

                        if (lastItem != "Inclusion Description")
                            stream.Write(", ");
                    }

                    if (inclusionPropertiesToInclude.Contains("Mean inclusion brightness"))
                    {
                        stream.Write(m_Inclusions[i].Description);

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
            azimuth = (int)((float)azimuth / ((float)AzimuthResolution / 360.0f));

            //layers.Add(new Layer(depth, amplitude, azimuth, depth, amplitude, azimuth, sourceAzimuthResolution, depthResolution));

            int insertPosition = CalculateLayerInsertPosition(depth);

            LayerTypeSelector layerTypeSelector;

            layerTypeSelector = new LayerTypeSelector("Borehole");
            Layer layer = layerTypeSelector.setUpLayer(depth, amplitude, azimuth, depth, amplitude, azimuth, AzimuthResolution, DepthResolution);
         
            m_Layers.Insert(insertPosition, layer);

            selectedLayerPosition = insertPosition;
                        
            //selectedLayerPosition = layers.Count - 1;
            m_SelectedFeature = m_Layers[selectedLayerPosition];
            m_SelectedFeatureType = "Layer";

            
            SetAllLayerGroupNames(allLayerGroups);
            ((Layer)m_SelectedFeature).Group = "Unspecified";
            //layers[selectedLayerPosition].InitialiseModelAdapter(_model);
            SetStartDepth();
        }

        public void AddNewLayer(double slope, int intercept)
        {
            int insertPosition = CalculateLayerInsertPosition(intercept);

            LayerTypeSelector layerTypeSelector;
            
            layerTypeSelector = new LayerTypeSelector("Core");
            Layer layer = layerTypeSelector.setUpLayer(slope, intercept, slope, intercept, AzimuthResolution, DepthResolution);

            m_Layers.Insert(insertPosition, layer);

            selectedLayerPosition = insertPosition;


            //selectedLayerPosition = layers.Count - 1;
            m_SelectedFeature = m_Layers[selectedLayerPosition];
            m_SelectedFeatureType = "Layer";


            SetAllLayerGroupNames(allLayerGroups);
            ((Layer)m_SelectedFeature).Group = "Unspecified";
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
            m_Layers.Insert(layerPosition, layerToAdd);

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
            if (m_Layers.Count == 0)
                return 0;

            if (m_Layers.Count == 1)
            {
                if (depth < m_Layers[0].TopEdgeDepth)
                    return 0;
                else 
                    return 1;
            }

            for (int i = 0; i < m_Layers.Count; i++)
            {
                if(depth < m_Layers[i].TopEdgeDepth)
                    return i;
            }

            return m_Layers.Count;
        }

        /// <summary>
        /// Returns a List of Points from the top edge of a specified layer
        /// </summary>
        /// <param name="layerNum">The position of the layer</param>
        /// <returns>A List of layer points</returns>
        public List<Point> GetLayerPoints1(int layerNum)
        {
            return m_Layers[layerNum].GetTopEdgePoints();
        }

        /// <summary>
        /// Returns a List of Points from the bottom edge of a specified layer
        /// </summary>
        /// <param name="layerNum">The position of the layer</param>
        /// <returns>A List of layer points</returns>
        public List<Point> GetLayerPoints2(int layerNum)
        {
            return m_Layers[layerNum].GetBottomEdgePoints();
        }

        /// <summary>
        /// Returns a string representation of the group the layer at the given position belongs to
        /// </summary>
        /// <param name="layerNum">The positioin of the layer in the List</param>
        /// <returns>The group the layer belongs to</returns>
        public string GetLayerGroup(int layerNum)
        {
            return m_Layers[layerNum].Group;
        }

        public int GetLayerMin(int layerNum)
        {
            return m_Layers[layerNum].StartY;
        }

        public int GetLayerMax(int layerNum)
        {
            return m_Layers[layerNum].EndY;
        }

        internal void DeleteLayersInRange(int startDepth, int endDepth)
        {
            int count = 0;
            bool stop = false;

            while (count < m_Layers.Count && stop == false)
            {
                Layer currentLayer = m_Layers[count];

                if (currentLayer.TopEdgeDepth >= startDepth && currentLayer.BottomEdgeDepth <= endDepth)
                {
                    m_Layers.Remove(currentLayer);
                    count--;
                }
                else if (currentLayer.BottomEdgeDepth > endDepth)
                    stop = true;

                count++;
            }
        }

        internal void DeleteAllLayers()
        {
            m_Layers.Clear();
        }

        public void RemoveLayersWithQualityLessThan(int quality)
        {
            for (var i = 0; i < m_Layers.Count; i++)
            {
                if(m_Layers[i].Quality < quality)                
                {
                    m_Layers.RemoveAt(i);
                    i--;
                }
            }
        }

        public void JoinEdges(Layer previouslySelectedFeature, Layer currentlySelectedFeature)
        {
            if (previouslySelectedFeature.TopEdgeDepth < currentlySelectedFeature.TopEdgeDepth)
            {
                if (type == "Borehole")
                {
                    (previouslySelectedFeature as BoreholeLayer)?
                        .SetBottomEdgeDepth(currentlySelectedFeature.TopEdgeDepth);

                    (previouslySelectedFeature as BoreholeLayer)?
                        .SetBottomSineAmplitude(((BoreholeLayer)currentlySelectedFeature).TopSineAmplitude);

                    (previouslySelectedFeature as BoreholeLayer)?
                        .SetBottomSineAzimuth(((BoreholeLayer)currentlySelectedFeature).TopSineAzimuth);
                }
                else
                {
                    (previouslySelectedFeature as CoreLayer)?
                        .SetBottomEdgeSlope(((CoreLayer)currentlySelectedFeature).TopEdgeSlope);

                    (previouslySelectedFeature as CoreLayer)?
                        .SetBottomEdgeIntercept(((CoreLayer)currentlySelectedFeature).TopEdgeIntercept);
                }
            }
            else
            {
                if (type == "Borehole")
                {
                    (previouslySelectedFeature as BoreholeLayer)?
                        .SetTopEdgeDepth(currentlySelectedFeature.TopEdgeDepth);

                    (previouslySelectedFeature as BoreholeLayer)?
                        .SetTopSineAmplitude(((BoreholeLayer)currentlySelectedFeature).TopSineAmplitude);

                    (previouslySelectedFeature as BoreholeLayer)?
                        .SetTopSineAzimuth(((BoreholeLayer)currentlySelectedFeature).TopSineAzimuth);
                }
                else
                {
                    (previouslySelectedFeature as CoreLayer)?
                        .SetTopEdgeSlope(((CoreLayer)currentlySelectedFeature).TopEdgeSlope);

                    (previouslySelectedFeature as CoreLayer)?
                        .SetTopEdgeIntercept(((CoreLayer)currentlySelectedFeature).TopEdgeIntercept);
                }
            }

            m_Layers.Remove(currentlySelectedFeature);

            m_SelectedFeature = previouslySelectedFeature;

            previouslySelectedFeature.MoveEdge(3, 0, 0);
        }

        /// <summary>
        /// Splits a two-edge layer into two one edge layers
        /// </summary>
        /// <param name="layerToSplit">The two edge layer to be split</param>
        public void SplitLayer(Layer layerToSplit)
        {
            var position = m_Layers.IndexOf(layerToSplit);
            
            var layerTypeSelector = new LayerTypeSelector(type);

            Layer firstLayer, secondLayer;

            if (type == "Borehole")
            {
                var layer = layerToSplit as BoreholeLayer;

                firstLayer = layerTypeSelector.setUpLayer(layer.TopEdgeDepth, 
                                                          layer.TopSineAmplitude, 
                                                          layer.TopSineAzimuth, 
                                                          layer.TopEdgeDepth, 
                                                          layer.TopSineAmplitude, 
                                                          layer.TopSineAzimuth, 
                                                          AzimuthResolution, 
                                                          DepthResolution);

                secondLayer = layerTypeSelector.setUpLayer(layer.BottomEdgeDepth, 
                                                           layer.BottomSineAmplitude, 
                                                           layer.BottomSineAzimuth, 
                                                           layer.BottomEdgeDepth, 
                                                           layer.BottomSineAmplitude, 
                                                           layer.BottomSineAzimuth, 
                                                           AzimuthResolution, 
                                                           DepthResolution);
            }
            else
            {
                var layer = layerToSplit as CoreLayer;

                firstLayer = layerTypeSelector.setUpLayer(layer.TopEdgeSlope, 
                                                          layer.TopEdgeIntercept, 
                                                          layer.TopEdgeSlope, 
                                                          layer.TopEdgeIntercept, 
                                                          AzimuthResolution, 
                                                          DepthResolution);

                secondLayer = layerTypeSelector.setUpLayer(layer.BottomEdgeSlope, 
                                                           layer.BottomEdgeIntercept, 
                                                           layer.BottomEdgeSlope, 
                                                           layer.BottomEdgeIntercept, 
                                                           AzimuthResolution, 
                                                           DepthResolution);
            
            }
            
            firstLayer.Group = layerToSplit.Group;
            secondLayer.Group = layerToSplit.Group;
            
            m_Layers.Insert(position, secondLayer);
            m_Layers.Insert(position, firstLayer);

            m_Layers.Remove(layerToSplit);

            m_SelectedFeature = m_Layers[position];
        }

        # endregion

        # region Group methods

        internal void ChangeLayerGroupName(string groupNameBefore, string groupNameAfter)
        {
            foreach (var layer in m_Layers)
            {
                if (layer.Group == groupNameBefore)
                {
                    layer.Group = groupNameAfter;
                }
            }
        }

        internal void ChangeClusterGroupName(string groupNameBefore, string groupNameAfter)
        {
            foreach (var t in m_Clusters)
            {
                if (t.Group == groupNameBefore)
                {
                    t.Group = groupNameAfter;
                }
            }

        }

        internal void ChangeInclusionGroupName(string groupNameBefore, string groupNameAfter)
        {
            foreach (var t in m_Inclusions)
            {
                if (t.Group == groupNameBefore)
                {
                    t.Group = groupNameAfter;
                }
            }
        }

        internal int GetLayerGroupCount(string groupName)
        {
            var count = 0;

            foreach (var layer in m_Layers)
            {
                var layerName = layer.Group;

                if (layerName == groupName)
                {
                    count++;
                }
            }

            return count;
        }
        

        internal int GetClusterGroupCount(string groupName)
        {
            return m_Clusters.Select(t => t.Group).Count(clusterName => clusterName == groupName);
        }

        internal int GetInclusionGroupCount(string groupName)
        {
            return m_Inclusions.Select(t => t.Group).Count(inclusionName => inclusionName == groupName);
        }

        # endregion

        # region Cluster methods

        public string GetClusterGroup(int clusterNum)
        {
            return m_Clusters[clusterNum].Group;
        }

        /// <summary>
        /// Adds a cluster to the features
        /// </summary>
        public void AddCluster()
        {
            m_Clusters.Add(new Cluster(AzimuthResolution, DepthResolution));

            m_SelectedFeature = m_Clusters[m_Clusters.Count - 1];

            m_SelectedFeatureType = "Cluster";

            SetAllClusterGroupNames(allClusterGroups);

            ((Cluster)m_SelectedFeature).Group = "Unspecified";

            SetStartDepth();
        }

        /// <summary>
        /// Deletes a specified cluster from the features
        /// </summary>
        public void DeleteCluster(Cluster clusterToDelete)
        {
            m_Clusters.Remove(clusterToDelete);
        }

        /// <summary>
        /// Removes all items from the Clusters List
        /// </summary>
        public void DeleteAllClusters()
        {
            m_Clusters.Clear();
        }

        /// <summary>
        /// Sets the last added cluster as complete
        /// </summary>
        public void SetLastClusterAsComplete()
        {
            m_Clusters[m_Clusters.Count - 1].CheckFeatureIsWithinImageBounds();
            m_Clusters[m_Clusters.Count - 1].IsComplete = true;
        }

        /// <summary>
        /// Returns the number of clusters in the feature list
        /// </summary>
        /// <returns>The number of clusters</returns>
        public int GetNumOfClusters()
        {
            return m_Clusters.Count();
        }

        /// <summary>
        /// Returns the List of points of a specified Cluster 
        /// </summary>
        /// <param name="clusterNum">The position of the Cluster</param>
        /// <returns>The List of Cluster Points</returns>
        public List<Point> GetClusterPoints(int clusterNum)
        {
            return m_Clusters[clusterNum].Points;
        }

        # endregion

        # region Inclusion methods

        public string GetInclusionGroup(int inclusionNum)
        {
            return m_Inclusions[inclusionNum].Group;
        }

        /// <summary>
        /// Adds an inclusion to the features
        /// </summary>
        public void AddInclusion()
        {
            m_Inclusions.Add(new Inclusion(AzimuthResolution, DepthResolution));
            m_SelectedFeature = m_Inclusions[m_Inclusions.Count - 1];
            m_SelectedFeatureType = "Inclusion";

            SetAllInclusionGroupNames(allInclusionGroups);
            ((Inclusion)m_SelectedFeature).Group = "Unspecified";
            
            SetStartDepth();
        }

        /// <summary>
        /// Deletes a specified inclusion
        /// </summary>
        /// <param name="inclusionToDelete">The inclusion to delete</param>
        public void DeleteInclusion(Inclusion inclusionToDelete)
        {
            m_Inclusions.Remove(inclusionToDelete);
        }

        /// <summary>
        /// Removes all items from the Inclusions List
        /// </summary>
        public void DeleteAllInclusions()
        {
            m_Inclusions.Clear();
        }

        /// <summary>
        /// Sets the last added inclusion as complete
        /// </summary>
        public void SetLastInclusionAsComplete()
        {
            m_Inclusions[m_Inclusions.Count - 1].CheckFeatureIsWithinImageBounds();
            m_Inclusions[m_Inclusions.Count - 1].IsComplete = true;
        }

        /// <summary>
        /// Returns the number of inclusions in the feature list
        /// </summary>
        /// <returns>The number of inclusions</returns>
        public int GetNumOfInlcusions()
        {
            return m_Inclusions.Count();
        }

        /// <summary>
        /// Returns a List of a specified inclusions Points
        /// </summary>
        /// <param name="inclusionNum">The position of the inclusion to return</param>
        /// <returns>The List of Points</returns>
        public List<Point> GetInclusionPoints(int inclusionNum)
        {
            return m_Inclusions[inclusionNum].Points;
        }

        # endregion
     
        # region Get methods

        internal int GetPositionOf(Layer feature)
        {
            int position = m_Layers.IndexOf(feature);

            return position;
        }

        internal int GetPositionOf(Cluster feature)
        {
            int position = m_Clusters.IndexOf(feature);

            return position;
        }

        internal int GetPositionOf(Inclusion feature)
        {
            int position = m_Inclusions.IndexOf(feature);

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

            for(int i=0; i<m_Layers.Count; i++)
            {
                for (int j = 0; j < groupNames.Count; j++)
                {
                    if (m_Layers[i].Group == groupNames[j])
                    {
                        layersInGroups.Add(m_Layers[i]);
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
            if (m_SelectedFeatureType.Equals("Layer"))
            {
                Layer currentLayer = (Layer)m_SelectedFeature;
                currentLayer.SetBoreholeStartDepth(SourceStartDepth);
                Console.WriteLine("In FeaturesList - start depth being added: " + SourceStartDepth);
            }
            else if (m_SelectedFeatureType.Equals("Cluster"))
            {
                Cluster currentCluster = (Cluster)m_SelectedFeature;
                currentCluster.SourceStartDepth = SourceStartDepth;
            }
            else if (m_SelectedFeatureType.Equals("Inclusion"))
            {
                Inclusion currentInclusion = (Inclusion)m_SelectedFeature;
                currentInclusion.SourceStartDepth = SourceStartDepth;
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
            LayerAtLastClick = false;
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
            for (int i = 0; i < m_Layers.Count; i++)
            {
                low = m_Layers[i].GetTopEdgePoints()[xPoint].Y;
                high = m_Layers[i].GetBottomEdgePoints()[xPoint].Y;

                if (yPoint > low - 6 && yPoint < high + 6)
                {
                    if (m_SelectedFeatureType.Equals("Cluster") && m_Clusters[m_Clusters.Count - 1].IsComplete == false)
                    {
                        m_Clusters.Remove((Cluster)m_SelectedFeature);
                    }
                    else if (m_SelectedFeatureType.Equals("Inclusion") && m_Inclusions[m_Inclusions.Count - 1].IsComplete == false)
                    {
                        m_Inclusions.Remove((Inclusion)m_SelectedFeature);
                    }

                    //Same for inclusion

                    m_SelectedFeatureType = "Layer";
                    m_SelectedFeature = m_Layers[i];

                    LayerAtLastClick = true;
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
            for (int i = 0; i < m_Clusters.Count; i++)
            {
                List<Point> clusterPoints = m_Clusters[i].Points;

                if (isPointWithinPolygon(xPoint, yPoint, clusterPoints))
                {

                    if (m_SelectedFeatureType.Equals("Inclusion") && m_Inclusions[m_Inclusions.Count - 1].IsComplete == false)
                    {
                        m_Inclusions.Remove((Inclusion)m_SelectedFeature);
                    }

                    m_SelectedFeatureType = "Cluster";
                    m_SelectedFeature = m_Clusters[i];

                    clusterAtCurrentClick = true;

                    return true;
                }
                else if (PointsAreNegative(clusterPoints))
                {
                    clusterPoints = MoveAllPointsXparameter(clusterPoints, AzimuthResolution);

                    if (isPointWithinPolygon(xPoint, yPoint, clusterPoints))
                    {

                        if (m_SelectedFeatureType.Equals("Inclusion") && m_Inclusions[m_Inclusions.Count - 1].IsComplete == false)
                        {
                            m_Inclusions.Remove((Inclusion)m_SelectedFeature);
                        }

                        m_SelectedFeatureType = "Cluster";
                        m_SelectedFeature = m_Clusters[i];

                        clusterAtCurrentClick = true;

                        return true;
                    }
                }
                else if (PointsArePositive(clusterPoints))
                {
                    clusterPoints = MoveAllPointsXparameter(clusterPoints, -AzimuthResolution);

                    if (isPointWithinPolygon(xPoint, yPoint, clusterPoints))
                    {

                        if (m_SelectedFeatureType.Equals("Inclusion") && m_Inclusions[m_Inclusions.Count - 1].IsComplete == false)
                        {
                            m_Inclusions.Remove((Inclusion)m_SelectedFeature);
                        }

                        m_SelectedFeatureType = "Cluster";
                        m_SelectedFeature = m_Clusters[i];

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
                if (points[i].X >= AzimuthResolution)
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
            for (int i = 0; i < m_Inclusions.Count; i++)
            {
                List<Point> inclusionPoints = m_Inclusions[i].Points;

                if (isPointWithinPolygon(xPoint, yPoint, inclusionPoints))
                {
                    if (m_SelectedFeatureType.Equals("Cluster") && m_Clusters[m_Clusters.Count - 1].IsComplete == false)
                    {
                        m_Clusters.Remove((Cluster)m_SelectedFeature);
                    }

                    m_SelectedFeatureType = "Inclusion";
                    m_SelectedFeature = m_Inclusions[i];

                    inclusionAtCurrentClick = true;

                    return true;
                }
                else if (PointsAreNegative(inclusionPoints))
                {
                    inclusionPoints = MoveAllPointsXparameter(inclusionPoints, AzimuthResolution);

                    if (isPointWithinPolygon(xPoint, yPoint, inclusionPoints))
                    {
                        if (m_SelectedFeatureType.Equals("Cluster") && m_Clusters[m_Clusters.Count - 1].IsComplete == false)
                        {
                            m_Clusters.Remove((Cluster)m_SelectedFeature);
                        }

                        m_SelectedFeatureType = "Inclusion";
                        m_SelectedFeature = m_Inclusions[i];

                        inclusionAtCurrentClick = true;

                        return true;
                    }
                }
                else if (PointsArePositive(inclusionPoints))
                {
                    inclusionPoints = MoveAllPointsXparameter(inclusionPoints, -AzimuthResolution);

                    if (isPointWithinPolygon(xPoint, yPoint, inclusionPoints))
                    {
                        if (m_SelectedFeatureType.Equals("Cluster") && m_Clusters[m_Clusters.Count - 1].IsComplete == false)
                        {
                            m_Clusters.Remove((Cluster)m_SelectedFeature);
                        }

                        m_SelectedFeatureType = "Inclusion";
                        m_SelectedFeature = m_Inclusions[i];

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
            this.m_Layers = layers;

            for (var i = 0; i < layers.Count; i++)
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
            this.m_Clusters = clusters;
            
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
            this.m_Inclusions = inclusions;

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

            for (int i = 0; i < m_Layers.Count; i++)
            {
                m_Layers[i].SetAllGroupNames(allGroups);
            }
        }

        internal void SetAllClusterGroupNames(string[] allGroups)
        {
            this.allClusterGroups = allGroups;

            for (int i = 0; i < m_Clusters.Count; i++)
            {
                m_Clusters[i].SetAllGroupNames(allGroups);
            }
        }

        internal void SetAllInclusionGroupNames(string[] allGroups)
        {
            this.allInclusionGroups = allGroups;

            for (int i = 0; i < m_Inclusions.Count; i++)
            {
                m_Inclusions[i].SetAllGroupNames(allGroups);
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
            if(m_SelectedFeatureType == "Layer")
                ((Layer)m_SelectedFeature).Group = group;
            else if (m_SelectedFeatureType == "Cluster")
                ((Cluster)m_SelectedFeature).Group = group;
            else if (m_SelectedFeatureType == "Inclusion")
                ((Inclusion)m_SelectedFeature).Group = group;
        }
    }
}
