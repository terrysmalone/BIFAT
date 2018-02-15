using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.IO;
using FeatureAnnotationTool.Interfaces;
using BoreholeFeatures;

namespace FeatureAnnotationTool.Model
{
    /// <summary>
    /// A class which deals with all the featuresfound in the borehole image
    /// </summary>
    internal class Features
    {
        private int m_FluidLevel;

        private List<Cluster> m_Clusters;
        private List<Inclusion> m_Inclusions;

        private int m_SelectedLayerPosition;

        private bool m_LayerAtCurrentClick;
        private bool m_ClusterAtCurrentClick;
        private bool m_InclusionAtCurrentClick;

        private int[] m_LayerBrightnesses;
        private int[] m_ClusterBrightnesses;
        private int[] m_InclusionBrightnesses;

        private readonly IModel m_Model;

        private string[] m_AllLayerGroups;
        private string[] m_AllClusterGroups;
        private string[] m_AllInclusionGroups;
        
        private string m_Type;        //"Borehole" or "Core"

        #region properties
        
        public int SourceStartDepth { get; set; }

        public int DepthResolution { get; set; }

        public int AzimuthResolution { get; set; }

        /// <summary>
        /// The boreholes fluid level
        /// </summary>
        /// <returns>The fluid level</returns>
        public int FluidLevel
        {
            get => m_FluidLevel;

            set
            {
                m_FluidLevel = value;
                FluidLevelSet = true;
            }
        }
        
        public bool FluidLevelSet { get; private set; }
        
        public object SelectedFeature { get; private set; } = "";
        
        public string SelectedFeatureType { get; private set; } = "";

        /// <summary>
        /// The name of the borehole 
        /// </summary>
        public string BoreholeName { get; set; } = "";

        internal string CurrentFeaturesGroup
        {
            get
            {
                var group = "";

                switch(SelectedFeatureType)
                {
                    case "Layer":
                        @group = ((Layer)SelectedFeature).Group;
                        break;
                    case "Cluster":
                        @group = ((Cluster)SelectedFeature).Group;
                        break;
                    case "Inclusion":
                        @group = ((Inclusion)SelectedFeature).Group;
                        break;
                }

                return group;
            }
        }

        #region Layer properties

        public List<Layer> Layers { get; private set; }
        

        /// <summary>
        /// The top sines (lowest y value) azimuth of the selected layer
        /// </summary>
        public int GetTopAzimuthOfSelectedLayer()
        {
                var selectedLayer = (Layer)SelectedFeature;

                return selectedLayer.TopSineAzimuth;
        }

        /// <summary>
        /// The bottom sines (lowest y value) azimuth of the selected layer
        /// </summary>
        public int GetBottomAzimuthOfSelectedLayer()
        {
            var selectedLayer = (Layer)SelectedFeature;

            return selectedLayer.BottomSineAzimuth;
            
        }

        internal bool LayerAtLastClick
        {
            get { return m_LayerAtCurrentClick; }
        }

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
            get { return m_ClusterAtCurrentClick; }
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
            get { return m_InclusionAtCurrentClick; }
        }

        #endregion Inclusion properties

        #endregion properties

        #region constructor

        /// <summary>
        /// Constructor method
        /// </summary>
        public Features(IModel model)
        {
            this.m_Model = model;

            Layers = new List<Layer>();
            m_Clusters = new List<Cluster>();
            m_Inclusions = new List<Inclusion>();

            FluidLevelSet = false;
        }

        #endregion constructor

        /// <summary>
        /// Deselects the selected feature
        /// </summary>
        public void DeSelectFeature()
        {
            SelectedFeature = "";
            SelectedFeatureType = "";
        }
        
        /// <summary>
        /// Deletes the current feature
        /// </summary>
        public void DeleteActiveFeature()
        {
            if (SelectedFeatureType.Equals("Layer"))
            {
                Layer activeLayer = (Layer)SelectedFeature;
                Layers.Remove(activeLayer);

            }
            else if (SelectedFeatureType.Equals("Cluster"))
            {
                Cluster activeCluster = (Cluster)SelectedFeature;
                m_Clusters.Remove(activeCluster);
            }
            else if (SelectedFeatureType.Equals("Inclusion"))
            {
                Inclusion activeInclusion = (Inclusion)SelectedFeature;
                m_Inclusions.Remove(activeInclusion);
            }

            SelectedFeatureType = "";
            SelectedFeature = "";
        }

        #region Write features methods

        /// <summary>
        /// Writes the current features to a specified excel document
        /// </summary>
        /// <param name="fileName">The name of the excel file to write to</param>
        public void WriteAllFeaturesToExcel(string fileName, List<string> layerPropertiesToInclude, List<string> clusterPropertiesToInclude, List<string> inclusionPropertiesToInclude)
        {
            WriteFeaturesToExcel writeFeatures = new WriteFeaturesToExcel(fileName, Layers, m_Clusters, m_Inclusions);

            writeFeatures.SetLayerBrightnesses(m_LayerBrightnesses);
            writeFeatures.SetClusterBrightnesses(m_ClusterBrightnesses);
            writeFeatures.SetInclusionBrightnesses(m_InclusionBrightnesses);

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

                for (int i = 0; i < Layers.Count; i++)
                {
                    if (layerPropertiesToInclude.Contains("Start depth (mm)"))
                    {
                        stream.Write(Layers[i].StartDepth);

                        if(lastItem != "Start depth (mm)")
                            stream.Write(", ");
                    }

                    if (layerPropertiesToInclude.Contains("End depth (mm)"))
                    {
                        stream.Write(Layers[i].EndDepth);

                        if (lastItem != "End depth (mm)")
                            stream.Write(", ");
                    }

                    if (layerPropertiesToInclude.Contains("Top sine depth (mm)"))
                    {
                        stream.Write(Layers[i].TopEdgeDepthMm);

                        if (lastItem != "Top sine depth (mm)") 
                            stream.Write(", ");
                    }

                    if (layerPropertiesToInclude.Contains("Top sine azimuth"))
                    {
                        stream.Write(Layers[i].TopSineAzimuth);

                        if (lastItem != "Top sine azimuth") 
                            stream.Write(", ");
                    }

                    if (layerPropertiesToInclude.Contains("Top sine amplitude (mm)"))
                    {
                        stream.Write(Layers[i].TopSineAmplitudeInMm);

                        if (lastItem != "Top sine amplitude (mm)")
                            stream.Write(", ");
                    }

                    if (layerPropertiesToInclude.Contains("Bottom sine depth (mm)"))
                    {
                        stream.Write(Layers[i].BottomEdgeDepthMm);

                        if (lastItem != "Bottom sine depth (mm)")
                            stream.Write(", ");
                    }

                    if (layerPropertiesToInclude.Contains("Bottom sine azimuth"))
                    {
                        stream.Write(Layers[i].BottomSineAzimuth);

                        if (lastItem != "Bottom sine azimuth")
                            stream.Write(", ");
                    }

                    if (layerPropertiesToInclude.Contains("Bottom sine amplitude (mm)"))
                    {
                        stream.Write(Layers[i].BottomSineAmplitudeInMm);

                        if (lastItem != "Bottom sine amplitude (mm)")
                            stream.Write(", ");
                    }

                    # region Core only properties

                    if (layerPropertiesToInclude.Contains("Top edge intercept (mm)"))
                    {
                        stream.Write(Layers[i].TopEdgeInterceptMm);

                        if (lastItem != "Top edge intercept (mm)")
                            stream.Write(", ");
                    }

                    if (layerPropertiesToInclude.Contains("Top edge slope"))
                    {
                        stream.Write(Layers[i].TopEdgeSlope);

                        if (lastItem != "Top edge slope")
                            stream.Write(", ");
                    }

                    if (layerPropertiesToInclude.Contains("Bottom edge intercept (mm)"))
                    {
                        stream.Write(Layers[i].BottomEdgeInterceptMm);

                        if (lastItem != "Bottom edge intercept (mm)")
                            stream.Write(", ");
                    }

                    if (layerPropertiesToInclude.Contains("Bottom edge slope"))
                    {
                        stream.Write(Layers[i].BottomEdgeSlope);

                        if (lastItem != "Bottom edge slope")
                            stream.Write(", ");
                    }

                    # endregion

                    if (layerPropertiesToInclude.Contains("Group"))
                    {
                        stream.Write(Layers[i].Group);

                        if (lastItem != "Group")
                            stream.Write(", ");
                    }

                    if (layerPropertiesToInclude.Contains("Layer type"))
                    {
                        stream.Write(Layers[i].LayerType);

                        if (lastItem != "Layer type")
                            stream.Write(", ");
                    }

                    if (layerPropertiesToInclude.Contains("Layer description"))
                    {
                        stream.Write(Layers[i].Description);

                        if (lastItem != "Layer description")
                            stream.Write(", ");
                    }

                    if (layerPropertiesToInclude.Contains("Layer quality"))
                    {
                        stream.Write(Layers[i].Quality);

                        if (lastItem != "Layer quality")
                            stream.Write(", ");
                    }

                    if (layerPropertiesToInclude.Contains("Mean layer brightness"))
                    {
                        stream.Write(m_LayerBrightnesses[i]);

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
        public void WriteLayersForWellCad(string fileName)
        {
            using (StreamWriter stream = new StreamWriter(fileName))
            {
                stream.WriteLine("Depth,Azimuth,Dip,Aperture");
                stream.WriteLine("m,deg,deg,mm");
                
                for (int i = 0; i < Layers.Count; i++)
                {
                    WriteEdgeDetailsForWellCad(Layers[i], stream);
                }
            }
        }

        private void WriteEdgeDetailsForWellCad(Layer layer, StreamWriter stream)
        {
            double wellDepth;
            double wellAzimuth;
            double wellDip;
            double wellThickness;
                        
            wellDepth = (double)layer.TopEdgeDepthMm / 1000.0;
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
            FluidLevelSet = false;
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

            var insertPosition = CalculateLayerInsertPosition(depth);

            var layerTypeSelector = new LayerTypeSelector("Borehole");

            var layer = layerTypeSelector.setUpLayer(depth, 
                                                     amplitude, 
                                                     azimuth, 
                                                     depth, 
                                                     amplitude, 
                                                     azimuth, 
                                                     AzimuthResolution, 
                                                     DepthResolution);
         
            Layers.Insert(insertPosition, layer);

            m_SelectedLayerPosition = insertPosition;
                        
            SelectedFeature = Layers[m_SelectedLayerPosition];
            SelectedFeatureType = "Layer";

            
            SetAllLayerGroupNames(m_AllLayerGroups);
            ((Layer)SelectedFeature).Group = "Unspecified";

            SetStartDepth();
        }

        public void AddNewLayer(double slope, int intercept)
        {
            var insertPosition = CalculateLayerInsertPosition(intercept);

            var layerTypeSelector = new LayerTypeSelector("Core");

            var layer = layerTypeSelector.setUpLayer(slope, 
                                                     intercept, 
                                                     slope, 
                                                     intercept, 
                                                     AzimuthResolution, 
                                                     DepthResolution);

            Layers.Insert(insertPosition, layer);

            m_SelectedLayerPosition = insertPosition;
            
            SelectedFeature = Layers[m_SelectedLayerPosition];
            SelectedFeatureType = "Layer";


            SetAllLayerGroupNames(m_AllLayerGroups);
            ((Layer)SelectedFeature).Group = "Unspecified";
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
                m_Model.AddLayerGroup(layerToAdd.Group);
                m_Model.SaveGroups();
            }

            var layerPosition = CalculateLayerInsertPosition(layerToAdd.TopEdgeDepth);
            Layers.Insert(layerPosition, layerToAdd);

            SetAllLayerGroupNames(m_AllLayerGroups);
        }

        private int CalculateLayerInsertPosition(int depth)
        {
            switch(Layers.Count)
            {
                case 0:
                    return 0;
                case 1:
                    if (depth < Layers[0].TopEdgeDepth)
                        return 0;
                    else 
                        return 1;
            }
            
            for (var i = 0; i < Layers.Count; i++)
            {
                if(depth < Layers[i].TopEdgeDepth)
                    return i;
            }

            return Layers.Count;
        }

        /// <summary>
        /// Returns a List of Points from the top edge of a specified layer
        /// </summary>
        /// <param name="layerNum">The position of the layer</param>
        /// <returns>A List of layer points</returns>
        public List<Point> GetLayerPoints1(int layerNum)
        {
            return Layers[layerNum].GetTopEdgePoints();
        }

        /// <summary>
        /// Returns a List of Points from the bottom edge of a specified layer
        /// </summary>
        /// <param name="layerNum">The position of the layer</param>
        /// <returns>A List of layer points</returns>
        public List<Point> GetLayerPoints2(int layerNum)
        {
            return Layers[layerNum].GetBottomEdgePoints();
        }

        /// <summary>
        /// Returns a string representation of the group the layer at the given position belongs to
        /// </summary>
        /// <param name="layerNum">The positioin of the layer in the List</param>
        /// <returns>The group the layer belongs to</returns>
        public string GetLayerGroup(int layerNum)
        {
            return Layers[layerNum].Group;
        }

        public int GetLayerMin(int layerNum)
        {
            return Layers[layerNum].StartY;
        }

        public int GetLayerMax(int layerNum)
        {
            return Layers[layerNum].EndY;
        }

        internal void DeleteLayersInRange(int startDepth, int endDepth)
        {
            int count = 0;
            bool stop = false;

            while (count < Layers.Count && stop == false)
            {
                Layer currentLayer = Layers[count];

                if (currentLayer.TopEdgeDepth >= startDepth && currentLayer.BottomEdgeDepth <= endDepth)
                {
                    Layers.Remove(currentLayer);
                    count--;
                }
                else if (currentLayer.BottomEdgeDepth > endDepth)
                    stop = true;

                count++;
            }
        }

        internal void DeleteAllLayers()
        {
            Layers.Clear();
        }

        public void RemoveLayersWithQualityLessThan(int quality)
        {
            for (int i = 0; i < Layers.Count; i++)
            {
                if(Layers[i].Quality < quality)                
                {
                    Layers.RemoveAt(i);
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
                if (m_Type == "Borehole")
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
                if (m_Type == "Borehole")
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

            Layers.Remove(currentlySelectedFeature);

            SelectedFeature = previouslySelectedFeature;

            previouslySelectedFeature.MoveEdge(3, 0, 0);
        }

        /// <summary>
        /// Splits a two-edge layer into two one edge layers
        /// </summary>
        /// <param name="layerToSplit">The two edge layer to be split</param>
        public void SplitLayer(Layer layerToSplit)
        {
            int position = Layers.IndexOf(layerToSplit);
            //int position = layers.FindIndex();

            LayerTypeSelector layerTypeSelector;

            layerTypeSelector = new LayerTypeSelector(m_Type);

            Layer firstLayer, secondLayer;

            if (m_Type == "Borehole")
            {
                firstLayer = layerTypeSelector.setUpLayer(layerToSplit.TopEdgeDepth, layerToSplit.TopSineAmplitude, layerToSplit.TopSineAzimuth, layerToSplit.TopEdgeDepth, layerToSplit.TopSineAmplitude, layerToSplit.TopSineAzimuth, AzimuthResolution, DepthResolution);
                secondLayer = layerTypeSelector.setUpLayer(layerToSplit.BottomEdgeDepth, layerToSplit.BottomSineAmplitude, layerToSplit.BottomSineAzimuth, layerToSplit.BottomEdgeDepth, layerToSplit.BottomSineAmplitude, layerToSplit.BottomSineAzimuth, AzimuthResolution, DepthResolution);
            }
            else
            {
                firstLayer = layerTypeSelector.setUpLayer(layerToSplit.TopEdgeSlope, layerToSplit.TopEdgeIntercept, layerToSplit.TopEdgeSlope, layerToSplit.TopEdgeIntercept, AzimuthResolution, DepthResolution);
                secondLayer = layerTypeSelector.setUpLayer(layerToSplit.BottomEdgeSlope, layerToSplit.BottomEdgeIntercept, layerToSplit.BottomEdgeSlope, layerToSplit.BottomEdgeIntercept, AzimuthResolution, DepthResolution);
            
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

            Layers.Insert(position, secondLayer);
            Layers.Insert(position, firstLayer);

            Layers.Remove(layerToSplit);

            SelectedFeature = Layers[position];
        }

        # endregion

        # region Group methods

        internal void ChangeLayerGroupName(string groupNameBefore, string groupNameAfter)
        {
            for (int i = 0; i < Layers.Count; i++)
            {
                if (Layers[i].Group == groupNameBefore)
                    Layers[i].Group = groupNameAfter;
            }
        }

        internal void ChangeClusterGroupName(string groupNameBefore, string groupNameAfter)
        {
            for (int i = 0; i < m_Clusters.Count; i++)
            {
                if (m_Clusters[i].Group == groupNameBefore)
                    m_Clusters[i].Group = groupNameAfter;
            }
        }

        internal void ChangeInclusionGroupName(string groupNameBefore, string groupNameAfter)
        {
            for (int i = 0; i < m_Inclusions.Count; i++)
            {
                if (m_Inclusions[i].Group == groupNameBefore)
                    m_Inclusions[i].Group = groupNameAfter;
            }
        }

        internal int GetLayerGroupCount(string groupName)
        {
            int count = 0;

            for (int i = 0; i < Layers.Count; i++)
            {
                string layerName = Layers[i].Group;

                if (layerName == groupName)
                    count++;
            }

            return count;
        }
        

        internal int GetClusterGroupCount(string groupName)
        {
            int count = 0;

            for (int i = 0; i < m_Clusters.Count; i++)
            {
                string clusterName = m_Clusters[i].Group;

                if (clusterName == groupName)
                    count++;
            }

            return count;
        }

        internal int GetInclusionGroupCount(string groupName)
        {
            int count = 0;

            for (int i = 0; i < m_Inclusions.Count; i++)
            {
                string inclusionName = m_Inclusions[i].Group;

                if (inclusionName == groupName)
                    count++;
            }

            return count;
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
            SelectedFeature = m_Clusters[m_Clusters.Count - 1];
            SelectedFeatureType = "Cluster";

            SetAllClusterGroupNames(m_AllClusterGroups);
            ((Cluster)SelectedFeature).Group = "Unspecified";

            SetStartDepth();
        }

        /// <summary>
        /// Deletes a specified cluster from the features
        /// </summary>
        /// <param name="clusterToDelete">The cluster to delete</param>
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
            SelectedFeature = m_Inclusions[m_Inclusions.Count - 1];
            SelectedFeatureType = "Inclusion";

            SetAllInclusionGroupNames(m_AllInclusionGroups);
            ((Inclusion)SelectedFeature).Group = "Unspecified";
            
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
            int position = Layers.IndexOf(feature);

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

            for(int i=0; i<Layers.Count; i++)
            {
                for (int j = 0; j < groupNames.Count; j++)
                {
                    if (Layers[i].Group == groupNames[j])
                    {
                        layersInGroups.Add(Layers[i]);
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
            if (SelectedFeatureType.Equals("Layer"))
            {
                Layer currentLayer = (Layer)SelectedFeature;
                currentLayer.SetBoreholeStartDepth(SourceStartDepth);
                Console.WriteLine("In FeaturesList - start depth being added: " + SourceStartDepth);
            }
            else if (SelectedFeatureType.Equals("Cluster"))
            {
                Cluster currentCluster = (Cluster)SelectedFeature;
                currentCluster.SourceStartDepth = SourceStartDepth;
            }
            else if (SelectedFeatureType.Equals("Inclusion"))
            {
                Inclusion currentInclusion = (Inclusion)SelectedFeature;
                currentInclusion.SourceStartDepth = SourceStartDepth;
            }

        }
        
        # region Set active feature type methods

        /// <summary>
        /// Checks if there is a feature at the selected co-ordinates and if so sets the equivalent feature type
        /// </summary>
        /// <param name="xPoint">The x position to check</param>
        /// <param name="yPoint">The y position to check</param>
        public void SetActiveFeatureType(int xPoint, int yPoint)
        {
            m_LayerAtCurrentClick = false;
            m_ClusterAtCurrentClick = false;
            m_InclusionAtCurrentClick = false;

            CheckIfLayerIsAtPoint(xPoint, yPoint);

            CheckIfClusterIsAtPoint(xPoint, yPoint);

            CheckIfInclusionIsAtPoint(xPoint, yPoint);            
        }
        
        public void CheckIfLayerIsAtPoint(int xPoint, int yPoint)
        {
            int high, low;

            //Check layers
            for (int i = 0; i < Layers.Count; i++)
            {
                low = Layers[i].GetTopEdgePoints()[xPoint].Y;
                high = Layers[i].GetBottomEdgePoints()[xPoint].Y;

                if (yPoint > low - 6 && yPoint < high + 6)
                {
                    if (SelectedFeatureType.Equals("Cluster") && m_Clusters[m_Clusters.Count - 1].IsComplete == false)
                    {
                        m_Clusters.Remove((Cluster)SelectedFeature);
                    }
                    else if (SelectedFeatureType.Equals("Inclusion") && m_Inclusions[m_Inclusions.Count - 1].IsComplete == false)
                    {
                        m_Inclusions.Remove((Inclusion)SelectedFeature);
                    }

                    //Same for inclusion

                    SelectedFeatureType = "Layer";
                    SelectedFeature = Layers[i];

                    m_LayerAtCurrentClick = true;
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

                if (IsPointWithinPolygon(xPoint, yPoint, clusterPoints))
                {

                    if (SelectedFeatureType.Equals("Inclusion") && m_Inclusions[m_Inclusions.Count - 1].IsComplete == false)
                    {
                        m_Inclusions.Remove((Inclusion)SelectedFeature);
                    }

                    SelectedFeatureType = "Cluster";
                    SelectedFeature = m_Clusters[i];

                    m_ClusterAtCurrentClick = true;

                    return true;
                }
                else if (PointsAreNegative(clusterPoints))
                {
                    clusterPoints = MoveAllPointsXparameter(clusterPoints, AzimuthResolution);

                    if (IsPointWithinPolygon(xPoint, yPoint, clusterPoints))
                    {

                        if (SelectedFeatureType.Equals("Inclusion") && m_Inclusions[m_Inclusions.Count - 1].IsComplete == false)
                        {
                            m_Inclusions.Remove((Inclusion)SelectedFeature);
                        }

                        SelectedFeatureType = "Cluster";
                        SelectedFeature = m_Clusters[i];

                        m_ClusterAtCurrentClick = true;

                        return true;
                    }
                }
                else if (PointsArePositive(clusterPoints))
                {
                    clusterPoints = MoveAllPointsXparameter(clusterPoints, -AzimuthResolution);

                    if (IsPointWithinPolygon(xPoint, yPoint, clusterPoints))
                    {

                        if (SelectedFeatureType.Equals("Inclusion") && m_Inclusions[m_Inclusions.Count - 1].IsComplete == false)
                        {
                            m_Inclusions.Remove((Inclusion)SelectedFeature);
                        }

                        SelectedFeatureType = "Cluster";
                        SelectedFeature = m_Clusters[i];

                        m_ClusterAtCurrentClick = true;
                        
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

        public bool CheckIfInclusionIsAtPoint(int xPoint, int yPoint)
        {
            for (int i = 0; i < m_Inclusions.Count; i++)
            {
                List<Point> inclusionPoints = m_Inclusions[i].Points;

                if (IsPointWithinPolygon(xPoint, yPoint, inclusionPoints))
                {
                    if (SelectedFeatureType.Equals("Cluster") && m_Clusters[m_Clusters.Count - 1].IsComplete == false)
                    {
                        m_Clusters.Remove((Cluster)SelectedFeature);
                    }

                    SelectedFeatureType = "Inclusion";
                    SelectedFeature = m_Inclusions[i];

                    m_InclusionAtCurrentClick = true;

                    return true;
                }
                else if (PointsAreNegative(inclusionPoints))
                {
                    inclusionPoints = MoveAllPointsXparameter(inclusionPoints, AzimuthResolution);

                    if (IsPointWithinPolygon(xPoint, yPoint, inclusionPoints))
                    {
                        if (SelectedFeatureType.Equals("Cluster") && m_Clusters[m_Clusters.Count - 1].IsComplete == false)
                        {
                            m_Clusters.Remove((Cluster)SelectedFeature);
                        }

                        SelectedFeatureType = "Inclusion";
                        SelectedFeature = m_Inclusions[i];

                        m_InclusionAtCurrentClick = true;

                        return true;
                    }
                }
                else if (PointsArePositive(inclusionPoints))
                {
                    inclusionPoints = MoveAllPointsXparameter(inclusionPoints, -AzimuthResolution);

                    if (IsPointWithinPolygon(xPoint, yPoint, inclusionPoints))
                    {
                        if (SelectedFeatureType.Equals("Cluster") && m_Clusters[m_Clusters.Count - 1].IsComplete == false)
                        {
                            m_Clusters.Remove((Cluster)SelectedFeature);
                        }

                        SelectedFeatureType = "Inclusion";
                        SelectedFeature = m_Inclusions[i];

                        m_InclusionAtCurrentClick = true;

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
        private bool IsPointWithinPolygon(int xPoint, int yPoint, List<Point> polygonPoints)
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
            this.Layers = layers;

            for (int i = 0; i < layers.Count; i++)
            {
                if (layers[i].Group == null)
                {
                    layers[i].Group = "Unspecified";
                    m_Model.AddLayerGroup(layers[i].Group);
                    m_Model.SaveGroups();
                }

                layers[i].SetAllGroupNames(m_AllLayerGroups);
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
                    m_Model.AddClusterGroup(clusters[i].Group);
                    m_Model.SaveGroups();
                }

                clusters[i].SetAllGroupNames(m_AllClusterGroups);
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
                    m_Model.AddInclusionGroup(inclusions[i].Group);
                    m_Model.SaveGroups();
                }

                inclusions[i].SetAllGroupNames(m_AllInclusionGroups);
            }
        }

        internal void SetAllLayerGroupNames(string[] allGroups)
        {
            this.m_AllLayerGroups = allGroups;

            for (int i = 0; i < Layers.Count; i++)
            {
                Layers[i].SetAllGroupNames(allGroups);
            }
        }

        internal void SetAllClusterGroupNames(string[] allGroups)
        {
            this.m_AllClusterGroups = allGroups;

            for (int i = 0; i < m_Clusters.Count; i++)
            {
                m_Clusters[i].SetAllGroupNames(allGroups);
            }
        }

        internal void SetAllInclusionGroupNames(string[] allGroups)
        {
            this.m_AllInclusionGroups = allGroups;

            for (int i = 0; i < m_Inclusions.Count; i++)
            {
                m_Inclusions[i].SetAllGroupNames(allGroups);
            }
        }

        internal void SetImageType(string type)
        {
            this.m_Type = type;
        }

        internal void SetLayerBrightnesses(int[] layerBrightnesses)
        {
            this.m_LayerBrightnesses = layerBrightnesses;
        }

        internal void SetClusterBrightnesses(int[] clusterBrightnesses)
        {
            this.m_ClusterBrightnesses = clusterBrightnesses;
        }

        internal void SetInclusionBrightnesses(int[] inclusionBrightnesses)
        {
            this.m_InclusionBrightnesses = inclusionBrightnesses;
        }
        
        # endregion set methods

        internal void ChangeSelectedFeaturesGroup(string group)
        {
            if(SelectedFeatureType == "Layer")
                ((Layer)SelectedFeature).Group = group;
            else if (SelectedFeatureType == "Cluster")
                ((Cluster)SelectedFeature).Group = group;
            else if (SelectedFeatureType == "Inclusion")
                ((Inclusion)SelectedFeature).Group = group;
        }
    }
}
