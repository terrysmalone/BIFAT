using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BoreholeFeatures;
using System.IO;
using System.Drawing;
using RGReferencedData;
using System.Windows.Forms;

namespace SaveLoadBoreholeData
{
    /// <summary>
    /// Class which implements all the actions associated with saving and loading boreholedata
    /// 
    /// Author - Terry Malone (trm8@aber.ac.uk)
    /// Version 1.1 - Refactored
    /// </summary>
    public class SaveLoadData
    {
        private string projectLocation, projectName;
        private string featuresFilePath, layerGroupsPath, clusterGroupsPath, inclusionGroupsPath;

        private LayerTypeSelector layerTypeSelector = new LayerTypeSelector("Borehole");

        /// <summary>
        /// Constructor method
        /// </summary>
        /// <param name="projectLocationPath">The path of the location of the project</param>
        /// <param name="projectName">The project name</param>
        public SaveLoadData(string projectLocationPath, string projectName)
        {
            this.projectLocation = projectLocationPath;
            this.projectName = projectName;

            featuresFilePath = projectLocationPath + "\\" + projectName + ".features";
            layerGroupsPath = projectLocation + "\\features\\layerGroups";
            clusterGroupsPath = projectLocation + "\\features\\clusterGroups";
            inclusionGroupsPath = projectLocation + "\\features\\inclusionGroups";
        }

        # region initialise directory methods

        /// <summary>
        /// Creates the project folders
        /// </summary>
        public void createDirectories()
        {
            if (Directory.Exists(projectLocation))
                Directory.Delete(projectLocation, true);

            System.IO.Directory.CreateDirectory(projectLocation);

            string newPath = System.IO.Path.Combine(projectLocation, "data");
            System.IO.Directory.CreateDirectory(newPath);

            newPath = System.IO.Path.Combine(projectLocation, "source");
            System.IO.Directory.CreateDirectory(newPath);

            newPath = System.IO.Path.Combine(projectLocation, "features");
            System.IO.Directory.CreateDirectory(newPath);
        }

        /// <summary>
        /// Copies a given file to the source folder
        /// </summary>
        /// <param name="sourceFilePath">The path of the file to copy</param>
        public void copySourceFile(string sourceFilePath)
        {
            GC.Collect();

            string fileName = extractFileName(sourceFilePath);

            File.Copy(sourceFilePath, projectLocation + "\\source\\" + fileName, true);
        }

        /// <summary>
        /// Extracts the file name from a given path string
        /// </summary>
        /// <param name="sourceFilePath">The path of the file</param>
        /// <returns>The name of the file</returns>
        private string extractFileName(string sourceFilePath)
        {
            string name;
            int startPos = sourceFilePath.LastIndexOf('\\');

            name = sourceFilePath.Substring(startPos);

            return name;
        }

        # endregion

        # region image data methods

        /// <summary>
        /// Creates an imageData file from the source image file
        /// </summary>
        public void CreateImageDataFromBitmap()
        {
            ImageData imageData = new ImageData(projectLocation, "Bitmap");

            imageData.Write();
        }

        /// <summary>
        /// Creates a scrollImageData image file from the source image file
        /// </summary>
        public void CreateFullLengthImagePreviewData()
        {
            FullLengthScrollerData scrollerData = new FullLengthScrollerData(projectLocation);

            scrollerData.Write();
        }

        public bool DoesFullLengthScrollerDataExist()
        {
            if (File.Exists(projectLocation + "\\data\\scrollImageData"))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Creates an imageData file from the source otv and hed files
        /// </summary>
        public void createImageDataFromOTV()
        {
            ImageData imageData = new ImageData(projectLocation, "OTV");

            imageData.Write();
        }

        /// <summary>
        /// Creates an imageData file from a given ReferencedChannel
        /// </summary>
        /// <param name="optvChannel"></param>
        public void createImageDataFromChannel(ReferencedChannel optvChannel)
        {
            ImageData imageData = new ImageData(projectLocation, "Channel");

            imageData.Write();
        }

        # endregion

        # region save borehole details & features methods

        /// <summary>
        /// Saves a given string to the projects .FeaturesImageTiler file
        /// </summary>
        /// <param name="boreholeDetails">The string representation of the borehole details</param>
        public void saveBoreholeDetails(string boreholeDetails)
        {
            string path = projectLocation + "\\" + projectName + ".features";

            using (StreamWriter sw = new StreamWriter(path))
            {
                sw.Write(boreholeDetails);
            }
        }

        /// <summary>
        /// Saves a given List of Layers to the projects layers file
        /// </summary>
        /// <param name="layers">A List of Layers to save</param>
        public void saveLayers(List<Layer> layers)
        {
            string path = projectLocation + "\\features\\layers";

            using (StreamWriter sw = new StreamWriter(path))
            {
                for (int i = 0; i < layers.Count; i++)
                {
                    sw.WriteLine(layers[i].GetDetails());
                }
            }
        }

        /// <summary>
        /// Saves a given List of Clusters to the projects clusters file
        /// </summary>
        /// <param name="clusters">A List of Clusters to save</param>
        public void saveClusters(List<Cluster> clusters)
        {
            string path = projectLocation + "\\features\\clusters";

            using (StreamWriter sw = new StreamWriter(path))
            {
                for (int i = 0; i < clusters.Count; i++)
                {
                    sw.WriteLine(clusters[i].GetDetails());
                }
            }
        }

        /// <summary>
        /// Saves a given List of Inclusions to the projects inclusions file
        /// </summary>
        /// <param name="inclusions">A List of Inclusions to save</param>
        public void saveInclusions(List<Inclusion> inclusions)
        {
            string path = projectLocation + "\\features\\inclusions";

            using (StreamWriter sw = new StreamWriter(path))
            {
                for (int i = 0; i < inclusions.Count; i++)
                {
                    sw.WriteLine(inclusions[i].getDetails());
                }
            }
        }

        /// <summary>
        /// Saves a given fluid level to the projects 'fluidLevel file
        /// </summary>
        /// <param name="fluidLevel">The fluid level</param>
        public void saveFluidLevel(int fluidLevel)
        {
            string path = projectLocation + "\\features\\fluidLevel";

            using (StreamWriter sw = new StreamWriter(path))
            {
                sw.WriteLine(fluidLevel);
            }
        }

        # endregion

        # region Layer Groups methods

        public void SaveLayerGroups(List<Tuple<string, Color>> layerGroupsToSave)
        {
            using (StreamWriter sw = new StreamWriter(layerGroupsPath))
            {
                for (int i = 0; i < layerGroupsToSave.Count; i++)
                {
                    sw.WriteLine(layerGroupsToSave[i].Item1);
                    sw.WriteLine(layerGroupsToSave[i].Item2.ToArgb());
                }
            }
        }

        public void SaveClusterGroups(List<Tuple<string, Color>> clusterGroupsToSave)
        {
            using (StreamWriter sw = new StreamWriter(clusterGroupsPath))
            {
                for (int i = 0; i < clusterGroupsToSave.Count; i++)
                {
                    sw.WriteLine(clusterGroupsToSave[i].Item1);
                    sw.WriteLine(clusterGroupsToSave[i].Item2.ToArgb());
                }
            }
        }

        public void SaveInclusionGroups(List<Tuple<string, Color>> inclusionGroupsToSave)
        {
            using (StreamWriter sw = new StreamWriter(inclusionGroupsPath))
            {
                for (int i = 0; i < inclusionGroupsToSave.Count; i++)
                {
                    sw.WriteLine(inclusionGroupsToSave[i].Item1);
                    sw.WriteLine(inclusionGroupsToSave[i].Item2.ToArgb());
                }
            }
        }

        public List<Tuple<string, Color>> GetLayerGroupsList()
        {
            List<Tuple<string, Color>> groups = new List<Tuple<string, Color>>();

            if (File.Exists(layerGroupsPath))
            {
                using (StreamReader reader = new StreamReader(layerGroupsPath))
                {
                    string line;

                    string groupName;
                    Color groupColour;

                    while ((line = reader.ReadLine()) != null)
                    {
                        groupName = line;
                        groupColour = ColorFrom(reader.ReadLine());

                        groups.Add(Tuple.Create(groupName, groupColour));
                    }
                }
            }

            return groups;
        }

        public List<Tuple<string, Color>> GetClusterGroupsList()
        {
            List<Tuple<string, Color>> groups = new List<Tuple<string, Color>>();

            if (File.Exists(clusterGroupsPath))
            {
                using (StreamReader reader = new StreamReader(clusterGroupsPath))
                {
                    string line;

                    string groupName;
                    Color groupColour;

                    while ((line = reader.ReadLine()) != null)
                    {
                        groupName = line;
                        groupColour = ColorFrom(reader.ReadLine());

                        groups.Add(Tuple.Create(groupName, groupColour));
                    }
                }
            }

            return groups;
        }

        public List<Tuple<string, Color>> GetInclusionGroupsList()
        {
            List<Tuple<string, Color>> groups = new List<Tuple<string, Color>>();

            if (File.Exists(inclusionGroupsPath))
            {
                using (StreamReader reader = new StreamReader(inclusionGroupsPath))
                {
                    string line;

                    string groupName;
                    Color groupColour;

                    while ((line = reader.ReadLine()) != null)
                    {
                        groupName = line;
                        groupColour = ColorFrom(reader.ReadLine());

                        groups.Add(Tuple.Create(groupName, groupColour));
                    }
                }
            }

            return groups;
        }

        private Color ColorFrom(string colourAsString)
        {
            Color colour = Color.FromArgb(System.Convert.ToInt32(colourAsString));

            return colour;
        }

        # endregion

        public void RemoveFluidLevel()
        {
            string fluidLevelPath = projectLocation + "\\features\\fluidLevel";

            if (File.Exists(fluidLevelPath))
                File.Delete(fluidLevelPath);

        }

        # region get methods

        /// <summary>
        /// Returns the imageData file path
        /// </summary>
        /// <returns>The imageData file path </returns>
        public string getImageDataFilePath()
        {
            return projectLocation + "\\data\\imageData";
        }

        /// <summary>
        /// Returns the .features file path
        /// </summary>
        /// <returns>The .features file path</returns>
        public string getFeaturesFilePath()
        {
            return featuresFilePath;
        }

        public Bitmap GetFullPreviewImage()
        {            
            FullLengthScrollerData scrollerData = new FullLengthScrollerData(projectLocation);
            Bitmap fullPreviewImage = scrollerData.GetFullPreviewImage();

            return fullPreviewImage;
        }

        # region borehole details get methods

        /// <summary>
        /// Gets and Returns the name of the borehole from the .features file 
        /// </summary>
        /// <returns>The borehole name</returns>
        public string GetBoreholeName()
        {
            string boreholeName;

            using (StreamReader reader = new StreamReader(featuresFilePath))
            {
                boreholeName = reader.ReadLine();
            }

            return boreholeName;
        }

        /// <summary>
        /// Gets and Returns the width of the borehole from the .features file 
        /// </summary>
        /// <returns>The borehole width</returns>
        public int GetBoreholeWidth()
        {
            int boreholeWidth;

            using (StreamReader reader = new StreamReader(featuresFilePath))
            {
                reader.ReadLine();
                boreholeWidth = Int32.Parse(reader.ReadLine());
            }

            return boreholeWidth;
        }

        /// <summary>
        /// Gets and Returns the height of the borehole from the .features file 
        /// </summary>
        /// <returns>The borehole height</returns>
        public int GetBoreholeHeight()
        {
            int boreholeHeight;

            using (StreamReader reader = new StreamReader(featuresFilePath))
            {
                reader.ReadLine();
                reader.ReadLine();

                boreholeHeight = Int32.Parse(reader.ReadLine());
            }

            return boreholeHeight;
        }

        /// <summary>
        /// Gets and Returns the start depth of the borehole from the .features file 
        /// </summary>
        /// <returns>The borehole start depth</returns>
        public int GetBoreholeStartDepth()
        {
            int boreholeStartDepth;

            using (StreamReader reader = new StreamReader(featuresFilePath))
            {
                reader.ReadLine();
                reader.ReadLine();
                reader.ReadLine();

                boreholeStartDepth = Int32.Parse(reader.ReadLine());
            }

            return boreholeStartDepth;
        }

        /// <summary>
        /// Gets and Returns the end depth of the borehole from the .features file 
        /// </summary>
        /// <returns>The borehole end depth</returns>
        public int GetBoreholeEndDepth()
        {
            int boreholeEndDepth;

            using (StreamReader reader = new StreamReader(featuresFilePath))
            {
                reader.ReadLine();
                reader.ReadLine();
                reader.ReadLine();
                reader.ReadLine();

                boreholeEndDepth = Int32.Parse(reader.ReadLine());
            }

            return boreholeEndDepth;
        }

        /// <summary>
        /// Gets and Returns the depth resolution of the borehole from the .features file 
        /// </summary>
        /// <returns>The borehole depth resolution</returns>
        public int GetDepthResolution()
        {
            int depthResolution;

            using (StreamReader reader = new StreamReader(featuresFilePath))
            {
                reader.ReadLine();
                reader.ReadLine();
                reader.ReadLine();
                reader.ReadLine();
                reader.ReadLine();

                depthResolution = Int32.Parse(reader.ReadLine());
            }

            return depthResolution;
        }

        public string GetType()
        {
            string type = "";

            try
            {
                StreamReader reader = new StreamReader(featuresFilePath);
                
                reader.ReadLine();
                reader.ReadLine();
                reader.ReadLine();
                reader.ReadLine();
                reader.ReadLine();
                reader.ReadLine();

                type = reader.ReadLine();
                    
                reader.Close();

                if (type == null || type == "")
                {
                    AddTypeToFile("Borehole");  //Default type

                    type = "Borehole";
                }
               
            }
            catch (Exception e)
            {
                MessageBox.Show("There was an error reading the features file: " + e.Message);
            }

            return type;
        }

        private void AddTypeToFile(string type)
        {
            string path = projectLocation + "\\" + projectName + ".features";

            using (StreamWriter sw = new StreamWriter(path, true))
            {
                sw.WriteLine(type);
            }
        }

        # endregion

        # region get features methods

        /// <summary>
        /// Returns whether the fluid level has been set.
        /// Returns 'true' if it is set, 'false' if not
        /// </summary>
        /// <returns></returns>
        public bool getIsFluidLevelSet()
        {
            string fluidLevelPath = projectLocation + "\\features\\fluidLevel";
            bool isFluidLevelSet = false;

            if (File.Exists(fluidLevelPath))
            {
                using (StreamReader reader = new StreamReader(fluidLevelPath))
                {
                    if (reader.ReadLine() != null)
                    {
                        isFluidLevelSet = true;
                    }
                }
            }

            return isFluidLevelSet;
        }

        /// <summary>
        /// Gets and returns the fluid level from the file 'features\\fluidLevel'
        /// </summary>
        /// <returns>The fluid level</returns>
        public int getFluidLevel()
        {
            int fluidLevel = -1;

            string fluidLevelPath = projectLocation + "\\features\\fluidLevel";

            if (File.Exists(fluidLevelPath))
            {
                using (StreamReader reader = new StreamReader(fluidLevelPath))
                {
                    fluidLevel = Int32.Parse(reader.ReadLine());
                }
            }

            return fluidLevel;
        }

        /// <summary>
        /// Gets and returns a List of layers from the file 'features\\layers'
        /// </summary>
        /// <returns>The List of Layers</returns>
        public List<Layer> getLayers(string type)
        {
            layerTypeSelector = new LayerTypeSelector(type);
            
            List<Layer> layers = new List<Layer>();

            string layersPath = projectLocation + "\\features\\layers";

            if (File.Exists(layersPath))
            {
                int azimuthResolution = GetBoreholeWidth();
                int depthResolution = GetDepthResolution();
                int boreholeStartDepth = GetBoreholeStartDepth();

                using (StreamReader reader = new StreamReader(layersPath))
                {
                    string currentLayerText;

                    int layerNum = 0;

                    while ((currentLayerText = reader.ReadLine()) != null)
                    {
                        try
                        {
                            layers.Add(createLayer(type, currentLayerText, azimuthResolution, depthResolution, boreholeStartDepth));
                        }
                        catch (Exception e)
                        {
                            MessageBox.Show("Could not load layer " + layerNum, "Error");
                        }

                        layerNum++;
                    }
                }
            }

            return layers;
        }

        /// <summary>
        /// Creates and returns a layer from a given string
        /// </summary>
        /// <param name="layerText">The string representation of the layer</param>
        /// <returns>The layer</returns>
        private Layer createLayer(string imageType, string layerText, int azimuthResolution, int depthResolution, int boreholeStartDepth)
        {
            Layer layer = null;

            String[] details = null;

            if (layerText != null || !layerText.Equals(""))
            {
                details = layerText.Split(',');
            }

            String type = "";
            String desc = "";
            int quality = 0;

            DateTime added = new DateTime();
            DateTime lastModified = new DateTime();
            
            if (imageType == "Borehole")
            {
                int firstDepth = System.Convert.ToInt32(details[2]);
                int firstAz = System.Convert.ToInt32(details[3]);
                int firstAmp = System.Convert.ToInt32(details[4]);
                int secondDepth = System.Convert.ToInt32(details[5]);
                int secondAz = System.Convert.ToInt32(details[6]);
                int secondAmp = System.Convert.ToInt32(details[7]);
                type = details[8];
                desc = details[9];
                quality = System.Convert.ToInt32(details[10]);

                added = System.Convert.ToDateTime(details[11]);
                lastModified = System.Convert.ToDateTime(details[12]);

                layer = layerTypeSelector.setUpLayer(firstDepth, firstAmp, firstAz, secondDepth, secondAmp, secondAz, azimuthResolution, depthResolution);

                //Add group
                if (details.Length > 13)
                {
                    layer.Group = details[13];
                }
            }
            else if (imageType == "Core")
            {
                double firstSlope = System.Convert.ToDouble(details[3]);
                int firstIntercept = System.Convert.ToInt32(details[2]);
                double secondSlope = System.Convert.ToDouble(details[5]);
                int secondIntercept = System.Convert.ToInt32(details[4]);
                                
                type = details[6];
                desc = details[7];
                quality = System.Convert.ToInt32(details[8]);

                added = System.Convert.ToDateTime(details[9]);
                lastModified = System.Convert.ToDateTime(details[10]);

                layer = layerTypeSelector.setUpLayer(firstSlope, firstIntercept, secondSlope, secondIntercept, azimuthResolution, depthResolution);

                //Add group
                if (details.Length > 11)
                {
                    layer.Group = details[11];
                }
            }            

            setLayerTypes(layer, type);

            layer.SetBoreholeStartDepth(boreholeStartDepth);

            layer.Description = desc;

            layer.SetQuality(quality);

            layer.TimeAdded = added;
            layer.TimeLastModified = lastModified;

            layer.CalculateStartY();
            layer.CalculateEndY();

            return layer;
        }

        /// <summary>
        /// Sets the type of a given layer based on the content of a given string
        /// </summary>
        /// <param name="layer">The Layer of which the type is to be set</param>
        /// <param name="typesString">The text representing the layers type/s</param>
        private void setLayerTypes(Layer layer, string typesString)
        {
            if (typesString.Contains("clean"))
                layer.Clean = true;

            if (typesString.Contains("void"))
                layer.LayerVoid = true;

            if (typesString.Contains("smallBubbles"))
                layer.SmallBubbles = true;

            if (typesString.Contains("largeBubbles"))
                layer.LargeBubbles = true;

            if (typesString.Contains("fineDebris"))
                layer.FineDebris = true;

            if (typesString.Contains("coarseDebris"))
                layer.CoarseDebris = true;

            if (typesString.Contains("diamicton"))
                layer.Diamicton = true;
        }

        /// <summary>
        /// Gets and returns a List of Clusters from the file 'features\\clusters'
        /// </summary>
        /// <returns>The List of Clusters</returns>
        public List<Cluster> GetClusters()
        {
            List<Cluster> clusters = new List<Cluster>();

            string clustersPath = projectLocation + "\\features\\clusters";

            if (File.Exists(clustersPath))
            {
                int azimuthResolution = GetBoreholeWidth();
                int depthResolution = GetDepthResolution();
                int boreholeStartDepth = GetBoreholeStartDepth();

                using (StreamReader reader = new StreamReader(clustersPath))
                {
                    string currentClusterText;

                    while ((currentClusterText = reader.ReadLine()) != null)
                    {
                        clusters.Add(createCluster(currentClusterText, azimuthResolution, depthResolution, boreholeStartDepth));
                    }
                }
            }

            return clusters;
        }

        /// <summary>
        /// Creates a cluster from given text details
        /// </summary>
        /// <param name="currentClusterText">The cluster details</param>
        /// <param name="azimuthResolution">The azimuth resolution of the borehole</param>
        /// <param name="depthResolution">The depth resolution of the borehole</param>
        /// <param name="boreholeStartDepth">The borehole start depth</param>
        /// <returns></returns>
        private Cluster createCluster(string currentClusterText, int azimuthResolution, int depthResolution, int boreholeStartDepth)
        {
            String[] details = null;
            Cluster cluster;

            if (currentClusterText != null || !currentClusterText.Equals(""))
            {
                details = currentClusterText.Split(',');
            }

            int startY = System.Convert.ToInt32(details[0]);
            int endY = System.Convert.ToInt32(details[1]);
            int startX = System.Convert.ToInt32(details[2]);
            int endX = System.Convert.ToInt32(details[3]);
            string points = details[4];
            String type = details[5];
            String desc = details[6];
            DateTime added = System.Convert.ToDateTime(details[7]);
            DateTime lastModified = System.Convert.ToDateTime(details[8]);

            cluster = new Cluster(azimuthResolution, depthResolution);

            setClusterTypes(cluster, type);

            cluster.SourceStartDepth = boreholeStartDepth;

            cluster.Description = desc;

            List<Point> clusterPoints = pointsFromString(points);

            for (int i = 0; i < clusterPoints.Count; i++)
                cluster.AddPoint(clusterPoints[i]);

            cluster.LeftXBoundary = startX;
            cluster.RightXBoundary = endX;

            cluster.TopYBoundary = startY;
            cluster.BottomYBoundary = endY;

            cluster.TimeAdded = added;
            cluster.TimeLastModified = lastModified;

            //Add group
            if (details.Length > 9)
            {
                cluster.Group = details[9];
            }

            cluster.IsComplete = true;

            return cluster;
        }

        /// <summary>
        /// Sets the type of a specific cluster
        /// </summary>
        /// <param name="clusterPosition">The position of the cluster</param>
        /// <param name="typesString">The cluster type</param>
        private void setClusterTypes(Cluster cluster, string typesString)
        {
            if (typesString.Contains("smallBubbles"))
                cluster.SmallBubbles = true;

            if (typesString.Contains("largeBubbles"))
                cluster.LargeBubbles = true;

            if (typesString.Contains("fineDebris"))
                cluster.FineDebris = true;

            if (typesString.Contains("coarseDebris"))
                cluster.CoarseDebris = true;

            if (typesString.Contains("diamicton"))
                cluster.Diamicton = true;
        }

        /// <summary>
        /// Returns a List of Points from a string 
        /// </summary>
        /// <param name="stringToConvert">The string to convert to Points</param>
        /// <returns>A List of Points</returns>
        private List<Point> pointsFromString(string stringToConvert)
        {
            stringToConvert = stringToConvert.TrimEnd();
            String[] points = null;

            if (stringToConvert != null || !stringToConvert.Equals(""))
            {
                points = stringToConvert.Split(' ');
            }

            List<Point> pointsInString = new List<Point>();

            int currentPoint = 0;
            int first, second;

            for (int i = 0; i < points.Length; i += 2)
            {
                first = System.Convert.ToInt32(points[i]);
                second = System.Convert.ToInt32(points[i + 1]);

                pointsInString.Add(new Point(first, second));
                currentPoint++;
            }

            return pointsInString;
        }

        /// <summary>
        /// Gets and returns the List of Inclusions from the projects 'features\\inclusions folder
        /// </summary>
        /// <returns>The List of Inclusions</returns>
        public List<Inclusion> GetInclusions()
        {
            List<Inclusion> inclusions = new List<Inclusion>();

            string inclusionsPath = projectLocation + "\\features\\inclusions";

            if (File.Exists(inclusionsPath))
            {
                int azimuthResolution = GetBoreholeWidth();
                int depthResolution = GetDepthResolution();
                int boreholeStartDepth = GetBoreholeStartDepth();

                using (StreamReader reader = new StreamReader(inclusionsPath))
                {
                    string currentInclusionText;

                    while ((currentInclusionText = reader.ReadLine()) != null)
                    {
                        inclusions.Add(createInclusion(currentInclusionText, azimuthResolution, depthResolution, boreholeStartDepth));
                    }
                }
            }

            return inclusions;
        }

        /// <summary>
        /// Creates an Inclusion from given text details
        /// </summary>
        /// <param name="currentClusterText">The cluster details</param>
        /// <param name="azimuthResolution">The azimuth resolution of the borehole</param>
        /// <param name="depthResolution">The depth resolution of the borehole</param>
        /// <param name="boreholeStartDepth">The borehole start depth</param>
        /// <returns></returns>
        private Inclusion createInclusion(string currentInclusionText, int azimuthResolution, int depthResolution, int boreholeStartDepth)
        {
            String[] details = null;
            Inclusion inclusion;

            if (currentInclusionText != null || !currentInclusionText.Equals(""))
            {
                details = currentInclusionText.Split(',');
            }

            int startY = System.Convert.ToInt32(details[0]);
            int endY = System.Convert.ToInt32(details[1]);
            int startX = System.Convert.ToInt32(details[2]);
            int endX = System.Convert.ToInt32(details[3]);
            string points = details[4];
            String type = details[5];
            String desc = details[6];
            DateTime added = System.Convert.ToDateTime(details[7]);
            DateTime lastModified = System.Convert.ToDateTime(details[8]);

            inclusion = new Inclusion(azimuthResolution, depthResolution);

            inclusion.SourceStartDepth = boreholeStartDepth;

            inclusion.Description = desc;

            inclusion.InclusionType = type;

            List<Point> inclusionPoints = pointsFromString(points);

            for (int i = 0; i < inclusionPoints.Count; i++)
                inclusion.AddPoint(inclusionPoints[i]);

            inclusion.LeftXBoundary = startX;
            inclusion.RightXBoundary = endX;

            inclusion.TopYBoundary = startY;
            inclusion.BottomYBoundary = endY;

            inclusion.TimeAdded = added;
            inclusion.TimeLastModified = lastModified;

            //Add group
            if (details.Length > 9)
            {
                inclusion.Group = details[9];
            }

            inclusion.IsComplete = true;

            return inclusion;
        }

        # endregion

        # endregion
    }
}
