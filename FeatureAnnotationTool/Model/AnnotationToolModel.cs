using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Imaging;
using FeatureAnnotationTool.DialogBoxes;
using FeatureAnnotationTool.Interfaces;
using BoreholeFeatures;
using ImageTiler;
using RGHeader;
using BrightnessAnalysis;
using LargeBitmapFileCreator;
using RGReferencedData;
using SaveLoadBoreholeData;

namespace FeatureAnnotationTool.Model
{
    /// <summary>
    /// Controls the model and its calls to and from the controller
    /// 
    /// Author - Terry Malone (trm8@aber.ac.uk)
    /// Version - 1.1 Refactored
    /// </summary>
    public class AnnotationToolModel:IModel
    {
        # region variables

        Features features;

        private Header header;

        private string type = "Borehole";   //Borehole or Core
        private string projectName;
        private int boreholeStartDepth, boreholeEndDepth, boreholeWidth, boreholeHeight;

        private int sectionStart = 0;
        private int sectionEnd = 0;
        private string imageDataFile;

        private int sectionHeight = 10000;
        
        private int depthResolution;
        private int azimuthResolution;

        private Bitmap boreholeImage, boreholePreviewImage, fullPreviewImage;

        private string boreholeName;

        private IModelAdapter _modelAdapter;

        private FileTilerSelector fileTilerSelector;
        private FileTiler tiler;

        private string projectFileRoot;
        private SaveLoadData saveLoadData;

        private List<Tuple<string, Color>> layerGroups = new List<Tuple<string, Color>>();
        private List<Tuple<string, Color>> clusterGroups = new List<Tuple<string, Color>>();
        private List<Tuple<string, Color>> inclusionGroups = new List<Tuple<string, Color>>();
        
        # endregion

        #region properties

        /// <summary>
        /// Returns the type of the currently selected feature
        /// </summary>
        /// <returns>The ffeature type</returns>
        public string SelectedFeatureType
        {
            get { return features.SelectedType; }
        }

        /// <summary>
        /// Returns the currently selected feature
        /// </summary>
        /// <returns>The selected feature</returns>
        public object SelectedFeature
        {
            get { return features.SelectedFeature; }
        }

        public string ProjectType
        {
            get { return type; }
            set
            {
                type = value;

                if (type == "Borehole")
                    _modelAdapter.ShowWellCADExport(true);
                else
                    _modelAdapter.ShowWellCADExport(false);
            }
        }
        /// <summary>
        /// Returns the azimuth of the selected layers top (lowest y value) sine 
        /// </summary>
        /// <returns>The azimuth of the layers top sine</returns>
        public int TopAzimuthOfSelectedLayer
        {
            get
            {
                if (features.SelectedType.Equals("Layer"))
                    return features.TopAzimuthOfSelectedLayer;
                else
                    return 0;
            }
        }

        /// <summary>
        /// Returns the azimuth of the selected layers bottom (highest y value) sine 
        /// </summary>
        /// <returns>The azimuth of the layers bottom sine</returns>
        public int BottomAzimuthOfSelectedLayer
        {
            get
            {
                if (features.SelectedType.Equals("Layer"))
                    return features.BottomAzimuthOfSelectedLayer;
                else
                    return 0;
            }
        }

        /// <summary>
        /// Returns the type of the currently selected feature
        /// </summary>
        /// <returns>The feature type</returns>
        public string CurrentFeatureType
        {
            get { return features.SelectedType; }
        }

        /// <summary>
        /// Returns the current borehole name
        /// </summary>
        /// <returns>The borehole name</returns>
        public string BoreholeName
        {
            get {return features.BoreholeName; }
        }

        public int BoreholeStartDepth
        {
            get { return boreholeStartDepth; }
        }

        public int BoreholeEndDepth
        {
            get { return boreholeEndDepth; }
        }

        public int DepthResolution
        {
            get { return saveLoadData.GetDepthResolution(); }
        }

        public int AzimuthResolution
        {
            get { return saveLoadData.GetBoreholeWidth(); }
        }

        public int BoreholeDepth
        {
            get { return saveLoadData.GetBoreholeHeight(); }
        }

        #endregion properties

        /// <summary>
        /// Constructor method
        /// </summary>
        public AnnotationToolModel()
        {
            depthResolution = 1;
            features = new Features(this);
        }

        # region new methods

        # region new from Bitmap methods

        public void OpenBoreholeImageFile(string fileName, string boreholeName)
        {
            //try
            //{
                this.boreholeName = boreholeName;

                //type = "Borehole";

                openCreateNewProjectFormFromBitmap(fileName);

                _modelAdapter.StartProgressReport("Creating project");

                //ClearGroups();

                createInitialFilesFromBitmap(fileName);

                SetUpImageDataTiler();

                saveBoreholeDetails();

                setUpFeatures();

                loadBoreholeAndScroller();
                LoadFullLengthPreviewScroller();
                loadBoreholeName();

                sectionStart = tiler.SectionStartHeight;
                sectionEnd = tiler.SectionEndHeight;

                CreateRuler(sectionStart, sectionEnd);
            //}
            //catch (PathTooLongException ptle)
            //{
            //    _modelAdapter.EndProgressReport();
            //    MessageBox.Show("Error - The file path is too long: " + ptle.Message, "File path too long exception");

            //}

            _modelAdapter.EndProgressReport();
        }

        /// <summary>
        /// Opens the create new project form and gets relevant information from it
        /// </summary>
        /// <param name="filePath">The source image file path</param>
        private void openCreateNewProjectFormFromBitmap(string filePath)
        {
            CreateNewProjectFromBitmapForm newProjectForm = new CreateNewProjectFromBitmapForm();

            newProjectForm.SetProjectType(type);

            newProjectForm.setHeight((int)Bitmap.FromFile(filePath).Height);

            int separator_index = filePath.LastIndexOf(@"\");

            if (separator_index != -1 && separator_index < filePath.Length - 1)
                newProjectForm.setProjectLocation(filePath.Substring(0, separator_index));

            newProjectForm.BoreholeName = boreholeName;

            newProjectForm.ShowDialog();

            depthResolution = newProjectForm.DepthResolution;
            boreholeStartDepth = newProjectForm.StartDepth;
            boreholeEndDepth = newProjectForm.EndDepth;
            boreholeName = newProjectForm.BoreholeName;
            projectName = newProjectForm.ProjectName;
            projectFileRoot = newProjectForm.ProjectRoot;

            loadBoreholeName();

            GC.Collect();
        }

        /// <summary>
        /// Creates the initial project folder directory
        /// </summary>
        /// <param name="fileName">The name of the project</param>
        private void createInitialFilesFromBitmap(string fileName)
        {
            saveLoadData = new SaveLoadData(projectFileRoot, projectName);
            saveLoadData.createDirectories();
            saveLoadData.copySourceFile(fileName);

            saveLoadData.CreateImageDataFromBitmap();
            saveLoadData.CreateFullLengthImagePreviewData();

        }

        # endregion

        # region new from OTV/HED methods

        /// <summary>
        /// Method which opens A .HED and .OTV file and gets the required information from it
        /// </summary>
        public void OpenOTVFile(string fileName)
        {
            Cursor.Current = Cursors.WaitCursor;
            type = "Borehole";

            header = new Header(fileName);

            loadBoreholeNameFromHED();

            depthResolution = Convert.ToInt32(header.Well.GetKeyValue("step"));

            //LogMode 11=up, 12=down
            if (header.Well.GetKeyValue("LogMode").Equals("11"))
            {
                boreholeStartDepth = Convert.ToInt32(header.Well.GetKeyValue("enddepth"));
                boreholeEndDepth = Convert.ToInt32(header.Well.GetKeyValue("startdepth"));
            }
            else
            {
                boreholeStartDepth = Convert.ToInt32(header.Well.GetKeyValue("startdepth"));
                boreholeEndDepth = Convert.ToInt32(header.Well.GetKeyValue("enddepth"));
            }

            openCreateNewProjectFormFromOTV(fileName);

            _modelAdapter.StartProgressReport("Creating Project");

            createInitialFilesFromOTV(fileName);

            SetUpImageDataTiler();

            saveBoreholeDetails();

            setUpFeatures();

            loadBoreholeAndScroller();
            LoadFullLengthPreviewScroller();
            loadBoreholeName();

            sectionStart = tiler.SectionStartHeight;
            sectionEnd = tiler.SectionEndHeight;

            CreateRuler(sectionStart, sectionEnd);

            _modelAdapter.EndProgressReport();
        }

        /// <summary>
        /// Gets the filename from the .HED file
        /// </summary>
        private void loadBoreholeNameFromHED()
        {
            boreholeName = (string)header.HedFileName.TrimEnd('.', 'H', 'E', 'D');
            //boreholeName = (string)header.HedFileName.TrimEnd('.', 'H', 'E', 'D');

            int separator_index = boreholeName.LastIndexOf(@"\");

            if (separator_index != -1 && separator_index < boreholeName.Length - 1)
                boreholeName = boreholeName.Substring(separator_index + 1);

            loadBoreholeName();
        }

        /// <summary>
        /// Creates the initial project folder directory
        /// </summary>
        /// <param name="fileName">The name of the project</param>
        private void createInitialFilesFromOTV(string fileName)
        {
            saveLoadData = new SaveLoadData(projectFileRoot, projectName);
            saveLoadData.createDirectories();

            //.HED file
            saveLoadData.copySourceFile(fileName);

            //.OTV file
            string otvPath = (string)header.HedFileName.TrimEnd('.', 'H', 'E', 'D');
            
            
            otvPath += ".otv";

            //otvPath += ".lox";

            saveLoadData.copySourceFile(otvPath);

            saveLoadData.createImageDataFromOTV();

            saveLoadData.CreateFullLengthImagePreviewData();
        }

        private void openCreateNewProjectFormFromOTV(string filePath)
        {
            CreateNewProjectFromOTVForm newProjectForm = new CreateNewProjectFromOTVForm();

            int separator_index = filePath.LastIndexOf(@"\");

            if (separator_index != -1 && separator_index < filePath.Length - 1)
                newProjectForm.setProjectLocation(filePath.Substring(0, separator_index));

            newProjectForm.setBoreholeName(boreholeName);

            newProjectForm.ShowDialog();

            projectName = newProjectForm.getProjectName();
            projectFileRoot = newProjectForm.getProjectRoot();

            loadBoreholeName();

            GC.Collect();
        }

        # endregion

        # region new from optvChannel methods

        public void OpenChannel(ReferencedChannel optvChannel, string workingPath)
        {
            type = "Borehole";

            Cursor.Current = Cursors.WaitCursor;

            saveLoadData = new SaveLoadData(workingPath, "manualPicking");

            if (!Directory.Exists(workingPath))
            {
                saveLoadData.createDirectories();

                saveLoadData.createImageDataFromChannel(optvChannel);

                depthResolution = optvChannel.Session.Step;

                boreholeStartDepth = optvChannel.Session.Top;
                boreholeEndDepth = optvChannel.Session.Bottom;

                SetUpImageDataTiler();

                saveBoreholeDetails();

                setUpFeatures();

                loadBoreholeAndScroller();
                LoadFullLengthPreviewScroller();
                sectionStart = tiler.SectionStartHeight;
                sectionEnd = tiler.SectionEndHeight;

                CreateRuler(sectionStart, sectionEnd);
            }
            else
            {
                saveLoadData.createImageDataFromChannel(optvChannel);

                LoadFeaturesFile(workingPath + "\\manualPicking.features");
            }

            saveLoadData.CreateFullLengthImagePreviewData();
        }

        # endregion

        private void setUpFeatures()
        {
            features = new Features(this);

            features.AzimuthResolution = azimuthResolution;

            features.SourceStartDepth = boreholeStartDepth;
            features.DepthResolution = depthResolution;

            features.SetImageType(type);
            ClearGroups();

            AddLayerGroup("Unspecified");
            AddClusterGroup("Unspecified");
            AddInclusionGroup("Unspecified");
            SaveGroups();
        }

        public void SetupLayerGroupsForm()
        {
            LayerGroupsForm layersForm = new LayerGroupsForm(this);

            layersForm.ShowDialog();
        }

        public void SetupClusterGroupsForm()
        {
            ClusterGroupsForm clustersForm = new ClusterGroupsForm(this);

            clustersForm.ShowDialog();
        }

        public void SetupInclusionGroupsForm()
        {
            InclusionGroupsForm inclusionsForm = new InclusionGroupsForm(this);

            inclusionsForm.ShowDialog();
        }

        /// <summary>
        /// Sets the borehole scroller thumbnail and calls the controller to set up the image
        /// </summary>
        private void loadBoreholeAndScroller()
        {
            boreholePreviewImage = (Bitmap)boreholeImage.GetThumbnailImage(30, 400, null, IntPtr.Zero);

            _modelAdapter.displayImage(boreholeImage);
            _modelAdapter.displayPreview(boreholePreviewImage);
        }

        private void LoadFullLengthPreviewScroller()
        {
            fullPreviewImage = saveLoadData.GetFullPreviewImage();

            _modelAdapter.DisplayFullPreview(fullPreviewImage);
        }

        /// <summary>
        /// Method which opens a modal popup form and requests Borehole details
        /// </summary>
        private void createHeaderInformation()
        {
            BoreholeDetailsForm boreholeDetailsForm = new BoreholeDetailsForm(boreholeHeight);

            boreholeDetailsForm.setDefaultName(boreholeName);
            boreholeDetailsForm.ShowDialog();

            depthResolution = boreholeDetailsForm.DepthResolution;
            boreholeStartDepth = boreholeDetailsForm.StartDepth;
            boreholeEndDepth = boreholeDetailsForm.EndDepth;

            boreholeName = boreholeDetailsForm.BoreholeName;

            loadBoreholeName();
        }

        /// <summary>
        /// Calls methods to set the borehole name in the Features, and the controller
        /// </summary>
        private void loadBoreholeName()
        {
            features.BoreholeName = boreholeName;
            _modelAdapter.setTitleName(boreholeName);
        }

        /// <summary>
        /// Creates the ruler and sends it to the controller
        /// </summary>
        private void CreateRuler(int start, int end)
        {
            start = (start * depthResolution) + boreholeStartDepth;
            end = (end * depthResolution) + boreholeStartDepth;

            _modelAdapter.setBoreholeSectionsComboBoxItems(boreholeStartDepth, boreholeEndDepth);

            Ruler ruler = new Ruler(start, end, depthResolution);

            int height = ruler.GetRulerHeight();

            _modelAdapter.setRulerImage(ruler.GetRulerImage());

            if (ImageType == "Borehole")
                _modelAdapter.DrawOrientationRuler();
            else
                _modelAdapter.HideOrientationRuler();

            _modelAdapter.SetCurrentSectionComboBox(tiler.CurrentSectionNumber);
        }

        # endregion

        # region loadFeaturesFile methods

        /// <summary>
        /// Loads features and image data from a .features file project
        /// </summary>
        /// <param name="fileName"></param>
        public void LoadFeaturesFile(string fileName)
        {
            features = new Features(this);

            int separator_index = fileName.LastIndexOf(@"\");

            if (separator_index != -1 && separator_index < fileName.Length - 1)
            {
                projectFileRoot = fileName.Substring(0, separator_index);

                int fileNameLength = fileName.Length - projectFileRoot.Length - 10;

                projectName = fileName.Substring(separator_index + 1, fileNameLength);
            }
            saveLoadData = new SaveLoadData(projectFileRoot, projectName);

            loadBoreholeDetails();

            SetUpImageDataTiler();

            LoadGroups();

            LoadFeatures();

            loadBoreholeAndScroller();

            if (!saveLoadData.DoesFullLengthScrollerDataExist())
            {
                saveLoadData.CreateFullLengthImagePreviewData();
                
                LoadFullLengthPreviewScroller();
            }
            else
            {
                LoadFullLengthPreviewScroller();
            }

            CreateRuler(tiler.SectionStartHeight, tiler.SectionEndHeight);

        }

        private void LoadGroups()
        {
            layerGroups.Clear();
            layerGroups = saveLoadData.GetLayerGroupsList();
            features.SetAllLayerGroupNames(GetLayerGroupNames());

            clusterGroups.Clear();
            clusterGroups = saveLoadData.GetClusterGroupsList();
            features.SetAllClusterGroupNames(GetClusterGroupNames());

            inclusionGroups.Clear();
            inclusionGroups = saveLoadData.GetInclusionGroupsList();            
            features.SetAllInclusionGroupNames(GetInclusionGroupNames());
        }

        private void loadBoreholeDetails()
        {
            boreholeName = saveLoadData.GetBoreholeName();
            boreholeWidth = saveLoadData.GetBoreholeWidth();
            boreholeHeight = saveLoadData.GetBoreholeHeight();
            boreholeStartDepth = saveLoadData.GetBoreholeStartDepth();
            boreholeEndDepth = saveLoadData.GetBoreholeEndDepth();
            depthResolution = saveLoadData.GetDepthResolution();

            type = saveLoadData.GetType();

            features.SetImageType(type);

            azimuthResolution = boreholeWidth;

            features.AzimuthResolution = azimuthResolution;
            features.DepthResolution = depthResolution;

            features.SourceStartDepth = boreholeStartDepth;

            features.BoreholeName = boreholeName;
            _modelAdapter.setTitleName(boreholeName);
        }

        private void LoadFeatures()
        {
            int fluidLevel = saveLoadData.getFluidLevel();
            List<Layer> layers = saveLoadData.getLayers(type);
            List<Cluster> clusters = saveLoadData.GetClusters();
            List<Inclusion> inclusions = saveLoadData.GetInclusions();

            if (saveLoadData.getIsFluidLevelSet())
            {
                features.FluidLevel = fluidLevel;
            }

            features.SetLayers(layers);
            features.SetClusters(clusters);
            features.SetInclusions(inclusions);
        }

        # endregion

        # region Borehole Tile methods

        private void SetUpImageDataTiler()
        {
            imageDataFile = saveLoadData.getImageDataFilePath();

            fileTilerSelector = new FileTilerSelector("FeaturesFile");
            tiler = fileTilerSelector.setUpTiler(imageDataFile, sectionHeight);

            boreholeHeight = tiler.BoreholeHeight;
            boreholeWidth = tiler.BoreholeWidth;

            azimuthResolution = boreholeWidth;

            tiler.GoToFirstSection();

            boreholeImage = tiler.GetCurrentSectionAsBitmap();


            sectionStart = tiler.SectionStartHeight;
            sectionEnd = tiler.SectionEndHeight;

            checkPreviousNextStatus();
        }

        public void LoadFirstBoreholeSection()
        {
            tiler.GoToFirstSection();

            boreholeImage = tiler.GetCurrentSectionAsBitmap();

            sectionStart = tiler.SectionStartHeight;
            sectionEnd = tiler.SectionEndHeight;

            CreateRuler(tiler.SectionStartHeight, tiler.SectionEndHeight);

            loadBoreholeAndScroller();

            checkPreviousNextStatus();
        }

        public void LoadLastBoreholeSection()
        {
            tiler.GoToLastSection();

            boreholeImage = tiler.GetCurrentSectionAsBitmap();

            sectionStart = tiler.SectionStartHeight;
            sectionEnd = tiler.SectionEndHeight;

            CreateRuler(tiler.SectionStartHeight, tiler.SectionEndHeight);

            loadBoreholeAndScroller();

            checkPreviousNextStatus();
        }

        public void LoadPreviousBoreholeSection()
        {
            tiler.GoToPreviousSection();
            boreholeImage = tiler.GetCurrentSectionAsBitmap();

            sectionStart = tiler.SectionStartHeight;
            sectionEnd = tiler.SectionEndHeight;

            CreateRuler(sectionStart, sectionEnd);

            loadBoreholeAndScroller();

            checkPreviousNextStatus();
        }

        public void LoadNextBoreholeSection()
        {
            if (tiler.GoToNextSection())
            {
                boreholeImage = tiler.GetCurrentSectionAsBitmap();

                sectionStart = tiler.SectionStartHeight;
                sectionEnd = tiler.SectionEndHeight;

                CreateRuler(sectionStart, sectionEnd);

                loadBoreholeAndScroller();
                
                checkPreviousNextStatus();
            }
        }

        public void LoadSection(int section)
        {
            tiler.GoToSection(section);

            boreholeImage = tiler.GetCurrentSectionAsBitmap();

            sectionStart = tiler.SectionStartHeight;
            sectionEnd = tiler.SectionEndHeight;

            CreateRuler(sectionStart, sectionEnd);

            loadBoreholeAndScroller();

            checkPreviousNextStatus();
        }

        public void LoadSectionAt(int yPos)
        {
            int sectionToLoad = 0;

            int sectionHeight = tiler.DefaultSectionHeight;
            int currentSectionStart = 0;

            bool found = false;

            while (found == false)
            {
                if (yPos > currentSectionStart && yPos < (currentSectionStart + sectionHeight))
                    found = true;
                else
                {
                    sectionToLoad++;
                    currentSectionStart += sectionHeight;
                }
            }

            LoadSection(sectionToLoad);
        }

        private void checkPreviousNextStatus()
        {
            if (tiler.GetIsNextSection())
            {
                _modelAdapter.enableNextSectionButton();
                _modelAdapter.enableLastSectionButton();
            }
            else
            {
                _modelAdapter.disableNextSectionButton();
                _modelAdapter.disableLastSectionButton();
            }

            if (tiler.GetIsPreviousSection())
            {
                _modelAdapter.enablePreviousSectionButton();
                _modelAdapter.enableFirstSectionButton();
            }
            else
            {
                _modelAdapter.disablePreviousSectionButton();
                _modelAdapter.disableFirstSectionButton();
            }
        }

        public Bitmap getWholeBoreholeImage()
        {
            return tiler.GetWholeBoreholeImage();
        }

        # endregion

        # region Write data files methods

        # region Write features methods

        /// <summary>
        /// Calls the function which writes the features to an excel document
        /// </summary>
        /// <param name="fileName">The name of the file to write</param>
        public void WriteAllFeaturesToExcel(string fileName, List<string> layerPropertiesToInclude, List<string> clusterPropertiesToInclude, List<string> inclusionPropertiesToInclude)
        {
            int[] layerBrightnesses = null;
            int[] clusterBrightnesses = null;
            int[] inclusionBrightnesses = null;

            if (layerPropertiesToInclude != null)
            {
                if (layerPropertiesToInclude.Contains("Mean layer brightness"))
                {
                    layerBrightnesses = CalculateLayerBrightnesses();
                    features.SetLayerBrightnesses(layerBrightnesses);
                }
            }

            if (clusterPropertiesToInclude != null)
            {
                if (clusterPropertiesToInclude.Contains("Mean cluster brightness"))
                {
                    clusterBrightnesses = CalculateClusterBrightnesses();
                    features.SetClusterBrightnesses(clusterBrightnesses);
                }
            }

            if (inclusionPropertiesToInclude != null)
            {
                if (inclusionPropertiesToInclude.Contains("Mean inclusion brightness"))
                {
                    inclusionBrightnesses = CalculateInclusionBrightnesses();
                    features.SetInclusionBrightnesses(inclusionBrightnesses);
                }
            }

            features.WriteAllFeaturesToExcel(fileName, layerPropertiesToInclude, clusterPropertiesToInclude, inclusionPropertiesToInclude);
        }

        public void WriteLayersToText(string fileName, List<string> layerPropertiesToInclude)
        {
            int[] layerBrightnesses = null;

            if (layerPropertiesToInclude.Contains("Mean layer brightness"))
                layerBrightnesses = CalculateLayerBrightnesses();

            features.SetLayerBrightnesses(layerBrightnesses);

            features.WriteLayersToText(fileName, layerPropertiesToInclude);
        }

        # region CalculateLayerBrightness methods

        /// <summary>
        /// Calculates the brightnesses of all annotated layers in the borehole
        /// </summary>
        /// <returns></returns>
        private int[] CalculateLayerBrightnesses()
        {
            List<Layer> layers = features.Layers;

            int[] layerBrightnesses = new int[layers.Count];

            int sectionStartHeightMM = 0;
            int sectionEndHeightMM = 0;

            int currentTilerPosition = tiler.CurrentSectionNumber;
            int currentLayerBrightness;

            AverageBrightness layerBrightness = new AverageBrightness();
            layerBrightness.SetStartDepth(boreholeStartDepth);
            layerBrightness.SetSectionStartDepth(boreholeStartDepth);
            layerBrightness.SetDepthResolution(depthResolution);

            tiler.GoToFirstSection();

            sectionStartHeightMM = boreholeStartDepth;
            sectionEndHeightMM = sectionStartHeightMM + (tiler.CurrentSectionHeight * depthResolution) - 1;

            Bitmap currentSectionImage = tiler.GetCurrentSectionAsBitmap();
            layerBrightness.SetCurrentSectionImage(currentSectionImage);

            for (int i = 0; i < layers.Count; i++)
            {
                currentLayerBrightness = 0;

                int total1 = 0;
                int total2 = 0;
                int number1 = 0;
                int number2 = 0;

                bool layerDone = false;
                //bool sectionDone = false;
                Layer currentLayer = layers[i];

                while (layerDone == false)
                {
                    if (currentLayer.EndDepth < sectionStartHeightMM)   //This should not happen unless there is an error
                    {
                        layerDone = true;
                    }
                    else if (IsLayerCompletelyWithinSection(currentLayer, sectionStartHeightMM, sectionEndHeightMM))    //Layer falls wholly within section
                    {
                        currentLayerBrightness = layerBrightness.GetBrightnessOfLayer(layers[i]);
                        layerDone = true;
                    }
                    else if (isLayerPartiallyWithinSection(currentLayer, sectionStartHeightMM, sectionEndHeightMM))  //Layer falls partially within section
                    {
                        if (IsFirstPartOfLayerWithinSection(currentLayer, sectionStartHeightMM, sectionEndHeightMM))          //First part of layer is in section
                        {
                            layerBrightness.GetBrightnessOfLayer(layers[i]);

                            total1 = layerBrightness.GetTotalLayerBrightness();
                            number1 = layerBrightness.GetNumberOfLayerPointsUsed();

                            //load next section
                            tiler.GoToNextSection();
                            sectionStartHeightMM = sectionEndHeightMM + 1;
                            sectionEndHeightMM = sectionStartHeightMM + (tiler.CurrentSectionHeight * depthResolution);

                            layerBrightness.SetSectionStartDepth(sectionStartHeightMM);
                            layerBrightness.SetCurrentSectionImage(tiler.GetCurrentSectionAsBitmap());
                        }
                        else
                        {
                            //currentLayerBrightness = (int)(((double)currentLayerBrightness + (double)layerBrightness.GetBrightnessOfLayer(layers[i])) / 2.0);
                            layerBrightness.GetBrightnessOfLayer(layers[i]);

                            total2 = layerBrightness.GetTotalLayerBrightness();
                            number2 = layerBrightness.GetNumberOfLayerPointsUsed();

                            currentLayerBrightness = (int)((double)(total1 + total2) / (double)(number1 + number2));
                            layerDone = true;
                        }
                    }
                    else                                    //Layer is in later section
                    {
                        //load next section
                        tiler.GoToNextSection();
                        sectionStartHeightMM = sectionEndHeightMM + 1;
                        sectionEndHeightMM = sectionStartHeightMM + (tiler.CurrentSectionHeight * depthResolution);

                        layerBrightness.SetSectionStartDepth(sectionStartHeightMM);
                        layerBrightness.SetCurrentSectionImage(tiler.GetCurrentSectionAsBitmap());
                    }
                }

                layerBrightnesses[i] = currentLayerBrightness;
            }

            tiler.GoToSection(currentTilerPosition);

            return layerBrightnesses;
        }

        /// <summary>
        /// Checks if a given layer is completely within the given setion bounds
        /// </summary>
        /// <param name="currentLayer">The layer to check</param>
        /// <param name="sectionStartMM">The current section start depth</param>
        /// <param name="sectionEndMM">The current section end depth</param>
        /// <returns></returns>
        private bool IsLayerCompletelyWithinSection(Layer currentLayer, int sectionStartMM, int sectionEndMM)
        {
            bool inSection = false;

            int currentLayerStart = currentLayer.StartDepth;
            int currentLayerEnd = currentLayer.EndDepth;

            if (currentLayerStart >= sectionStartMM && currentLayerStart <= sectionEndMM && currentLayerEnd >= sectionStartMM && currentLayerEnd <= sectionEndMM)
                inSection = true;
            else
                inSection = false;

            return inSection;
        }

        private bool isLayerPartiallyWithinSection(Layer currentLayer, int sectionStartMM, int sectionEndMM)
        {
            bool inSection = false;

            int currentLayerStart = currentLayer.StartDepth;
            int currentLayerEnd = currentLayer.EndDepth;

            if ((currentLayerStart >= sectionStartMM && currentLayerStart <= sectionEndMM) && !(currentLayerEnd >= sectionStartMM && currentLayerEnd <= sectionEndMM))
                inSection = true;
            else if (!(currentLayerStart >= sectionStartMM && currentLayerStart <= sectionEndMM) && (currentLayerEnd >= sectionStartMM && currentLayerEnd <= sectionEndMM))
                inSection = true;
            else
                inSection = false;

            return inSection;
        }

        private bool IsFirstPartOfLayerWithinSection(Layer currentLayer, int sectionStartMM, int sectionEndMM)
        {
            bool inSection = false;

            int currentLayerStart = currentLayer.StartDepth;
            int currentLayerEnd = currentLayer.EndDepth;

            if ((currentLayerStart >= sectionStartMM && currentLayerStart <= sectionEndMM) && !(currentLayerEnd >= sectionStartMM && currentLayerEnd <= sectionEndMM))
                inSection = true;
            else
                inSection = false;

            return inSection;
        }

        # endregion

        /// <summary>
        /// Writes a .war file containing all layers for use in WellCAD as a structure log
        /// </summary>
        /// <param name="fileName"></param>
        public void WriteLayersForWellCAD(string fileName)
        {
            features.WriteLayersForWellCAD(fileName);
        }

        public void WriteClustersToText(string fileName, List<string> clusterPropertiesToInclude)
        {
            int[] clusterBrightnesses = null;

            if (clusterPropertiesToInclude != null)
            {
                if (clusterPropertiesToInclude.Contains("Mean cluster brightness"))
                {
                    clusterBrightnesses = CalculateClusterBrightnesses();
                    features.SetClusterBrightnesses(clusterBrightnesses);
                }
            }

            features.WriteClustersToText(fileName, clusterPropertiesToInclude);
        }

        public void WriteInclusionsToText(string fileName, List<string> inclusionPropertiesToInclude)
        {
            int[] inclusionBrightnesses = null;

            if (inclusionPropertiesToInclude != null)
            {
                if (inclusionPropertiesToInclude.Contains("Mean inclusion brightness"))
                {
                    inclusionBrightnesses = CalculateInclusionBrightnesses();
                    features.SetInclusionBrightnesses(inclusionBrightnesses);
                }
            }

            features.WriteInclusionsToText(fileName, inclusionPropertiesToInclude);
        }

        private int[] CalculateClusterBrightnesses()
        {
            List<Cluster> clusters = features.Clusters;

            int[] featureBrightnesses = new int[clusters.Count];

            int sectionStartHeightMM = 0;
            int sectionEndHeightMM = 0;

            int currentTilerPosition = tiler.CurrentSectionNumber;
            int currentFeatureBrightness;

            AverageBrightness featureBrightness = new AverageBrightness();
            featureBrightness.SetStartDepth(boreholeStartDepth);
            featureBrightness.SetSectionStartDepth(boreholeStartDepth);
            featureBrightness.SetDepthResolution(depthResolution);

            tiler.GoToFirstSection();

            sectionStartHeightMM = boreholeStartDepth;
            sectionEndHeightMM = sectionStartHeightMM + (tiler.CurrentSectionHeight * depthResolution) - 1;

            Bitmap currentSectionImage = tiler.GetCurrentSectionAsBitmap();
            featureBrightness.SetCurrentSectionImage(currentSectionImage);

            for (int i = 0; i < clusters.Count; i++)
            {
                currentFeatureBrightness = 0;

                int total1 = 0;
                int total2 = 0;
                int number1 = 0;
                int number2 = 0;

                bool featureDone = false;

                Cluster currentFeature = clusters[i];

                while (featureDone == false)
                {
                    if (currentFeature.EndDepth < sectionStartHeightMM)   //This should not happen unless there is an error
                    {
                        featureDone = true;
                    }
                    else if (IsFeatureCompletelyWithinSection(currentFeature.StartDepth, currentFeature.EndDepth, sectionStartHeightMM, sectionEndHeightMM))    //Feature falls wholly within section
                    {
                        currentFeatureBrightness = featureBrightness.GetBrightnessOfCluster(clusters[i]);
                        featureDone = true;
                    }
                    else if (isFeaturePartiallyWithinSection(currentFeature.StartDepth, currentFeature.EndDepth, sectionStartHeightMM, sectionEndHeightMM))  //Feature falls partially within section
                    {
                        if (IsFirstPartOfFeatureWithinSection(currentFeature.StartDepth, currentFeature.EndDepth, sectionStartHeightMM, sectionEndHeightMM))          //First part of feature is in section
                        {
                            featureBrightness.GetBrightnessOfCluster(clusters[i]);

                            total1 = featureBrightness.GetTotalFeatureBrightness();
                            number1 = featureBrightness.GetNumberOfFeaturePointsUsed();

                            //load next section
                            tiler.GoToNextSection();
                            sectionStartHeightMM = sectionEndHeightMM + 1;
                            sectionEndHeightMM = sectionStartHeightMM + (tiler.CurrentSectionHeight * depthResolution);

                            featureBrightness.SetSectionStartDepth(sectionStartHeightMM);
                            featureBrightness.SetCurrentSectionImage(tiler.GetCurrentSectionAsBitmap());
                        }
                        else
                        {
                            featureBrightness.GetBrightnessOfCluster(clusters[i]);

                            total2 = featureBrightness.GetTotalFeatureBrightness();
                            number2 = featureBrightness.GetNumberOfFeaturePointsUsed();

                            currentFeatureBrightness = (int)((double)(total1 + total2) / (double)(number1 + number2));
                            featureDone = true;
                        }
                    }
                    else                                    //feature is in later section
                    {
                        //load next section
                        tiler.GoToNextSection();
                        sectionStartHeightMM = sectionEndHeightMM + 1;
                        sectionEndHeightMM = sectionStartHeightMM + (tiler.CurrentSectionHeight * depthResolution);

                        featureBrightness.SetSectionStartDepth(sectionStartHeightMM);
                        featureBrightness.SetCurrentSectionImage(tiler.GetCurrentSectionAsBitmap());
                    }
                }

                featureBrightnesses[i] = currentFeatureBrightness;
            }

            tiler.GoToSection(currentTilerPosition);

            return featureBrightnesses;
        }

        private int[] CalculateInclusionBrightnesses()
        {
            List<Inclusion> inclusions = features.Inclusions;

            int[] featureBrightnesses = new int[inclusions.Count];

            int sectionStartHeightMM = 0;
            int sectionEndHeightMM = 0;

            int currentTilerPosition = tiler.CurrentSectionNumber;
            int currentFeatureBrightness;

            AverageBrightness featureBrightness = new AverageBrightness();
            featureBrightness.SetStartDepth(boreholeStartDepth);
            featureBrightness.SetSectionStartDepth(boreholeStartDepth);
            featureBrightness.SetDepthResolution(depthResolution);

            tiler.GoToFirstSection();

            sectionStartHeightMM = boreholeStartDepth;
            sectionEndHeightMM = sectionStartHeightMM + (tiler.CurrentSectionHeight * depthResolution) - 1;

            Bitmap currentSectionImage = tiler.GetCurrentSectionAsBitmap();
            featureBrightness.SetCurrentSectionImage(currentSectionImage);

            for (int i = 0; i < inclusions.Count; i++)
            {
                currentFeatureBrightness = 0;

                int total1 = 0;
                int total2 = 0;
                int number1 = 0;
                int number2 = 0;

                bool featureDone = false;

                Inclusion currentFeature = inclusions[i];

                while (featureDone == false)
                {
                    if (currentFeature.EndDepth < sectionStartHeightMM)   //This should not happen unless there is an error
                    {
                        featureDone = true;
                    }
                    else if (IsFeatureCompletelyWithinSection(currentFeature.StartDepth, currentFeature.EndDepth, sectionStartHeightMM, sectionEndHeightMM))    //Feature falls wholly within section
                    {
                        currentFeatureBrightness = featureBrightness.GetBrightnessOfInclusion(inclusions[i]);
                        featureDone = true;
                    }
                    else if (isFeaturePartiallyWithinSection(currentFeature.StartDepth, currentFeature.EndDepth, sectionStartHeightMM, sectionEndHeightMM))  //Feature falls partially within section
                    {
                        if (IsFirstPartOfFeatureWithinSection(currentFeature.StartDepth, currentFeature.EndDepth, sectionStartHeightMM, sectionEndHeightMM))          //First part of feature is in section
                        {
                            featureBrightness.GetBrightnessOfInclusion(inclusions[i]);

                            total1 = featureBrightness.GetTotalFeatureBrightness();
                            number1 = featureBrightness.GetNumberOfFeaturePointsUsed();

                            //load next section
                            tiler.GoToNextSection();
                            sectionStartHeightMM = sectionEndHeightMM + 1;
                            sectionEndHeightMM = sectionStartHeightMM + (tiler.CurrentSectionHeight * depthResolution);

                            featureBrightness.SetSectionStartDepth(sectionStartHeightMM);
                            featureBrightness.SetCurrentSectionImage(tiler.GetCurrentSectionAsBitmap());
                        }
                        else
                        {
                            featureBrightness.GetBrightnessOfInclusion(inclusions[i]);

                            total2 = featureBrightness.GetTotalFeatureBrightness();
                            number2 = featureBrightness.GetNumberOfFeaturePointsUsed();

                            currentFeatureBrightness = (int)((double)(total1 + total2) / (double)(number1 + number2));
                            featureDone = true;
                        }
                    }
                    else                                    //feature is in later section
                    {
                        //load next section
                        tiler.GoToNextSection();
                        sectionStartHeightMM = sectionEndHeightMM + 1;
                        sectionEndHeightMM = sectionStartHeightMM + (tiler.CurrentSectionHeight * depthResolution);

                        featureBrightness.SetSectionStartDepth(sectionStartHeightMM);
                        featureBrightness.SetCurrentSectionImage(tiler.GetCurrentSectionAsBitmap());
                    }
                }

                featureBrightnesses[i] = currentFeatureBrightness;
            }

            tiler.GoToSection(currentTilerPosition);

            return featureBrightnesses;
        }

        private bool IsFeatureCompletelyWithinSection(int featureStartMM, int featureEndMM, int sectionStartMM, int sectionEndMM)
        {
            bool inSection = false;

            if (featureStartMM >= sectionStartMM && featureStartMM <= sectionEndMM && featureEndMM >= sectionStartMM && featureEndMM <= sectionEndMM)
                inSection = true;
            else
                inSection = false;

            return inSection;
        }

        private bool isFeaturePartiallyWithinSection(int featureStartMM, int featureEndMM, int sectionStartMM, int sectionEndMM)
        {
            bool inSection = false;

            if ((featureStartMM >= sectionStartMM && featureStartMM <= sectionEndMM) && !(featureEndMM >= sectionStartMM && featureEndMM <= sectionEndMM))
                inSection = true;
            else if (!(featureStartMM >= sectionStartMM && featureStartMM <= sectionEndMM) && (featureEndMM >= sectionStartMM && featureEndMM <= sectionEndMM))
                inSection = true;
            else
                inSection = false;

            return inSection;
        }

        private bool IsFirstPartOfFeatureWithinSection(int featureStartMM, int featureEndMM, int sectionStartMM, int sectionEndMM)
        {
            bool inSection = false;

            if ((featureStartMM >= sectionStartMM && featureStartMM <= sectionEndMM) && !(featureEndMM >= sectionStartMM && featureEndMM <= sectionEndMM))
                inSection = true;
            else
                inSection = false;

            return inSection;
        }

        # endregion

        public void CreateDepthLuminosityProfileInExcel()
        {
            DepthLuminosityForm depthLuminosityForm = new DepthLuminosityForm();

            //depthLuminosityForm.SetDefaultName(projectFileRoot + "\\" + projectName + " - Depth-Luminosty profile.xlsx");

            List<string> groupNames = new List<string>();

            for (int i = 0; i < layerGroups.Count; i++)
            {
                groupNames.Add(layerGroups[i].Item1);
            }

            depthLuminosityForm.SetGroupsList(groupNames);

            depthLuminosityForm.ShowDialog();

            if (depthLuminosityForm.IsOKClicked())
            {
                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.DefaultExt = "*.xlsx";
                saveDialog.Filter = "Excel Files|*.xlsx";
                saveDialog.FileName = projectName + " - Depth-Luminosty profile";
                saveDialog.InitialDirectory = GetProjectLocation();

                if (saveDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK && saveDialog.FileName.Length > 0)
                {
                    string fileName = saveDialog.FileName;

                    _modelAdapter.StartProgressReport("Creating depth/luminosity profile");

                    AverageBrightness averageBrightness = new AverageBrightness();

                    averageBrightness.SetSamplingRate(depthLuminosityForm.getSamplingRate());
                    averageBrightness.SetDepthResolution(depthResolution);
                    averageBrightness.SetStartDepth(boreholeStartDepth);

                    averageBrightness.SetIncludeBackground(depthLuminosityForm.GetIncludeBackground());
                    averageBrightness.SetAllLayers(features.Layers);
                    averageBrightness.SetAllClusters(features.Clusters);
                    averageBrightness.SetAllInclusions(features.Inclusions);

                    if (depthLuminosityForm.GetAllFeaturesChecked())
                    {
                        averageBrightness.AddLayersToIncludeList(features.Layers);
                        averageBrightness.AddClustersToIncludeList(features.Clusters);
                        averageBrightness.AddInclusionsToIncludeList(features.Inclusions);
                    }
                    else
                    {
                        if (depthLuminosityForm.GetAllLayersChecked() == true)
                            averageBrightness.AddLayersToIncludeList(features.Layers);
                        else
                        {
                            List<string> includeGroups = depthLuminosityForm.GetCheckedGroups();

                            if (includeGroups.Count > 0)
                            {
                                List<Layer> excludeLayers = CalculateExcludeLayers(includeGroups);

                                averageBrightness.AddLayersToIncludeList(excludeLayers);
                            }
                        }

                        if (depthLuminosityForm.GetAllClustersChecked() == true)
                            averageBrightness.AddClustersToIncludeList(features.Clusters);

                        if (depthLuminosityForm.GetAllInclusionsChecked() == true)
                            averageBrightness.AddInclusionsToIncludeList(features.Inclusions);

                    }

                    int current = tiler.CurrentSectionNumber;

                    tiler.GoToFirstSection();

                    int sectionStart = 0;

                    averageBrightness.ProcessSection(tiler.GetCurrentSectionAsBitmap(), sectionStart);
                    
                    while (tiler.GetIsNextSection())
                    {
                        sectionStart += tiler.CurrentSectionHeight;

                        tiler.GoToNextSection();
                        averageBrightness.ProcessSection(tiler.GetCurrentSectionAsBitmap(), sectionStart);
                    }

                    tiler.GoToSection(current);

                    averageBrightness.WriteToExcel(fileName);
                    depthLuminosityForm.Close();

                    _modelAdapter.EndProgressReport();
                }
            }
        }

        public void CreateDepthLuminosityProfileInText()
        {
            DepthLuminosityForm depthLuminosityForm = new DepthLuminosityForm();

            List<string> groupNames = new List<string>();

            for (int i = 0; i < layerGroups.Count; i++)
            {
                groupNames.Add(layerGroups[i].Item1);
            }

            depthLuminosityForm.SetGroupsList(groupNames);

            depthLuminosityForm.ShowDialog();

            if (depthLuminosityForm.IsOKClicked())
            {
                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.DefaultExt = "*.txt";
                saveDialog.Filter = "Notepad Files|*.txt";
                saveDialog.FileName = projectName + " - Depth-Luminosty profile";
                saveDialog.InitialDirectory = GetProjectLocation();

                if (saveDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK && saveDialog.FileName.Length > 0)
                {
                    string fileName = saveDialog.FileName;

                    _modelAdapter.StartProgressReport("Creating depth/luminosity profile");
                    AverageBrightness averageBrightness = new AverageBrightness();

                    averageBrightness.SetSamplingRate(depthLuminosityForm.getSamplingRate());
                    averageBrightness.SetDepthResolution(depthResolution);
                    averageBrightness.SetStartDepth(boreholeStartDepth);

                    averageBrightness.SetIncludeBackground(depthLuminosityForm.GetIncludeBackground());
                    averageBrightness.SetAllLayers(features.Layers);
                    averageBrightness.SetAllClusters(features.Clusters);
                    averageBrightness.SetAllInclusions(features.Inclusions);

                    if (depthLuminosityForm.GetAllFeaturesChecked())
                    {
                        averageBrightness.AddLayersToIncludeList(features.Layers);
                        averageBrightness.AddClustersToIncludeList(features.Clusters);
                        averageBrightness.AddInclusionsToIncludeList(features.Inclusions);
                    }
                    else
                    {
                        if (depthLuminosityForm.GetAllLayersChecked() == true)
                            averageBrightness.AddLayersToIncludeList(features.Layers);
                        else
                        {
                            List<string> includeGroups = depthLuminosityForm.GetCheckedGroups();

                            if (includeGroups.Count > 0)
                            {
                                List<Layer> includeLayers = CalculateExcludeLayers(includeGroups);

                                averageBrightness.AddLayersToIncludeList(includeLayers);
                            }
                        }

                        if (depthLuminosityForm.GetAllClustersChecked() == true)
                            averageBrightness.AddClustersToIncludeList(features.Clusters);

                        if (depthLuminosityForm.GetAllInclusionsChecked() == true)
                            averageBrightness.AddInclusionsToIncludeList(features.Inclusions);

                    }

                    int current = tiler.CurrentSectionNumber;

                    tiler.GoToFirstSection();

                    int sectionStartDepth = 0;

                    averageBrightness.ProcessSection(tiler.GetCurrentSectionAsBitmap(), sectionStartDepth);

                    while (tiler.GetIsNextSection())
                    {
                        sectionStartDepth += tiler.CurrentSectionHeight;

                        tiler.GoToNextSection();
                        averageBrightness.ProcessSection(tiler.GetCurrentSectionAsBitmap(), sectionStartDepth);
                    }

                    tiler.GoToSection(current);

                    //string fileName = depthLuminosityForm.getFileName();

                    //if (fileName.Substring(fileName.Length - 4) != ".txt")
                    //{
                    //    fileName = fileName + ".txt";
                    // }

                    averageBrightness.WriteToTxt(fileName);
                    depthLuminosityForm.Close();

                    _modelAdapter.EndProgressReport();
                }
            }
        }

        /// <summary>
        /// Calculates which layers to exclude from a given list of group names
        /// </summary>
        /// <param name="excludeGroups"></param>
        /// <returns></returns>
        private List<Layer> CalculateExcludeLayers(List<string> excludeGroups)
        {
            return features.GetLayersInGroups(excludeGroups);
        }

        # endregion

        # region Export Image methods

        public void ExportImage(string fileName, bool withFeatures, bool withRuler, int exportSectionHeight)
        {
            int width = tiler.BoreholeWidth;
            int height = tiler.BoreholeHeight;

            LargeBMPFileCreator fileCreator;

            if(!withRuler)
                fileCreator = new LargeBMPFileCreator(fileName, width, height, exportSectionHeight);
            else
                fileCreator = new LargeBMPFileCreator(fileName, width + 60, height, exportSectionHeight);

            //fileCreator.WriteFileHeader();
            
            int initialSectionStart = sectionStart;
            int initialSectionEnd = sectionEnd;

            int currentSection = tiler.CurrentSectionNumber;

            tiler.GoToLastSection();

            do
            {
                using (Bitmap imageSection = tiler.GetCurrentSectionAsBitmap())
                {
                    byte[] sectionBytes;

                    sectionStart = tiler.SectionStartHeight;
                    sectionEnd = tiler.SectionEndHeight;

                    if (withFeatures)
                    {
                        using (Graphics g = Graphics.FromImage(imageSection))
                        {
                            //draw all features
                            PaintEventArgs paintEvent = new PaintEventArgs(g, new Rectangle(0, 0, imageSection.Width, imageSection.Height));
                            _modelAdapter.DrawAllFeatures(null, paintEvent);

                            //sectionBytes = GetBytesFromBitmap(imageSection);
                            //fileCreator.AddByteData(sectionBytes);
                        }
                    }

                    if (withRuler)
                    {
                        //Combine ruler with image section
                        int start = (sectionStart * depthResolution) + boreholeStartDepth;
                        int end = (sectionEnd * depthResolution) + boreholeStartDepth;

                        Ruler rulerImage = new Ruler(start, end, depthResolution);
                        
                        Bitmap rulerBitmap = rulerImage.GetRulerImage();

                        //Bitmap rulerBitmap = new Bitmap(rulerImage.GetRulerWidth(), rulerImage.GetRulerHeight(), PixelFormat.Format24bppRgb);

                        //MessageBox.Show("Section image format: " + imageSection.PixelFormat);
                        //MessageBox.Show("rulerBitmap image format: " + rulerBitmap.PixelFormat);

                       
                        Bitmap finalImageSection = CombineBitmaps(rulerBitmap, imageSection);

                        //MessageBox.Show("Section image format: " + imageSection.PixelFormat);
                        //MessageBox.Show("rulerBitmap image format: " + rulerBitmap.PixelFormat);
                        //MessageBox.Show("finalImageSection image format: " + finalImageSection.PixelFormat);

                            //new Bitmap(fileCreator.GetImageWidth(), fileCreator.GetImageHeight());

                        //finalImageSection.Save("testcombinedimageAfter.BMP");

                        sectionBytes = GetBytesFromBitmap(finalImageSection);
                        fileCreator.AddByteData(sectionBytes);
                    }
                    else
                    {
                        sectionBytes = GetBytesFromBitmap(imageSection);
                        fileCreator.AddByteData(sectionBytes);
                    }

                    if (sectionBytes != null)
                        sectionBytes = null;
                }
            } while (tiler.GoToPreviousSection());

            tiler.GoToSection(currentSection);

            sectionStart = initialSectionStart;
            sectionEnd = initialSectionEnd;
        }

        private Bitmap CombineBitmaps(Bitmap firstImage, Bitmap secondImage)
        {
            Bitmap finalImage = null;

            //firstImage.Save("testfirstimage.BMP");
            //secondImage.Save("testsecondimage.BMP");

            try
            {
                int width = 0;
                int height = 0;

                width += firstImage.Width;
                height = firstImage.Height > height ? firstImage.Height : height;

                width += secondImage.Width;
                height = secondImage.Height > height ? secondImage.Height : height;

                //create a bitmap to hold the combined image
                finalImage = new Bitmap(width, height, PixelFormat.Format24bppRgb);

                //get a graphics object from the image so we can draw on it
                using (Graphics g = Graphics.FromImage(finalImage))
                {
                    //set background color
                    g.Clear(System.Drawing.Color.White);

                    //go through each image and draw it on the final image
                    int offset = 0;

                    g.DrawImage(firstImage, new System.Drawing.Rectangle(offset, 0, firstImage.Width, firstImage.Height));
                    offset += firstImage.Width;

                    g.DrawImage(secondImage, new System.Drawing.Rectangle(offset, 0, secondImage.Width, secondImage.Height));
                }

                //finalImage.Save("testcombinedimage.BMP");
                return finalImage;
            }
            catch (Exception ex)
            {
                if (finalImage != null)
                    finalImage.Dispose();

                throw ex;
            }
            finally
            {
                firstImage.Dispose();
                secondImage.Dispose();
            }
        }

        private byte[] GetBytesFromBitmap(Bitmap image)
        {
            //Create section bytes
            image.RotateFlip(RotateFlipType.RotateNoneFlipY);

            byte[] bytes = null;

            using (MemoryStream ms = new MemoryStream())
            {
                // Save to memory using the Jpeg format
                image.Save(ms, ImageFormat.Bmp);

                byte[] bmpBytes = ms.GetBuffer();

                bytes = new byte[bmpBytes.Length - 54];
                
                for (int i = 0; i < bytes.Length; i++)
                {
                    bytes[i] = bmpBytes[i + 54];
                }

                if (bmpBytes != null)
                    bmpBytes = null;
            }

            return bytes;
        }

        # endregion

        # region save features methods

        public void saveFeatures()
        {
            Cursor.Current = Cursors.WaitCursor;

            saveBoreholeDetails();

            saveLoadData.saveLayers(features.Layers);
            saveLoadData.saveClusters(features.Clusters);
            saveLoadData.saveInclusions(features.Inclusions);

            if (features.FluidLevelSet)
                saveLoadData.saveFluidLevel(features.FluidLevel);
            else
                saveLoadData.RemoveFluidLevel();

            SaveGroups();
        }

        private void ClearGroups()
        {
            layerGroups.Clear();
        }

        public void SaveGroups()
        {
            try
            {
                saveLoadData.SaveLayerGroups(layerGroups);
                saveLoadData.SaveClusterGroups(clusterGroups);
                saveLoadData.SaveInclusionGroups(inclusionGroups);

            }
            catch (Exception e)
            {
            }
        }

        public List<string> GetLayerGroupsList()
        {
            List<string> groupsList = new List<string>();

            for (int i = 0; i < layerGroups.Count; i++)
            {
                groupsList.Add(layerGroups[i].Item1);
            }

            return groupsList;
        }

        public List<string> GetClusterGroupsList()
        {
            List<string> groupsList = new List<string>();

            for (int i = 0; i < clusterGroups.Count; i++)
            {
                groupsList.Add(clusterGroups[i].Item1);
            }

            return groupsList;
        }

        public List<string> GetInclusionGroupsList()
        {
            List<string> groupsList = new List<string>();

            for (int i = 0; i < inclusionGroups.Count; i++)
            {
                groupsList.Add(inclusionGroups[i].Item1);
            }

            return groupsList;
        }

        public string GetCurrentFeaturesGroup()
        {
            return features.CurrentFeaturesGroup;
        }

        private void saveBoreholeDetails()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(boreholeName);
            stringBuilder.AppendLine();
            stringBuilder.Append(boreholeWidth);
            stringBuilder.AppendLine();
            stringBuilder.Append(boreholeHeight);
            stringBuilder.AppendLine();
            stringBuilder.Append(boreholeStartDepth);
            stringBuilder.AppendLine();
            stringBuilder.Append(boreholeEndDepth);
            stringBuilder.AppendLine();
            stringBuilder.Append(depthResolution);
            stringBuilder.AppendLine();
            stringBuilder.Append(type);
            stringBuilder.AppendLine();


            string boreholeDetails = stringBuilder.ToString();

            saveLoadData.saveBoreholeDetails(boreholeDetails);
        }

        # endregion

        # region Features methods

        /// <summary>
        /// Restarts the Features
        /// </summary>
        public void Reset()
        {
            features = new Features(this);

            layerGroups.Clear();
            AddLayerGroup("Unspecified");
            AddClusterGroup("Unspecified");
            AddInclusionGroup("Unspecified");
        }

        /// <summary>
        /// Deletes the current feature
        /// </summary>
        public void deleteCurrentFeature()
        {
            features.DeleteActiveFeature();
        }

        /// <summary>
        /// Calls the features method which deselects the currently selected feature
        /// </summary>
        public void deSelectFeature()
        {
            if (features.SelectedFeature != null)
                features.DeSelectFeature();
        }

        /// <summary>
        /// Moves a point on the currently selected feature (cluster or inclusion only)
        /// </summary>
        /// <param name="movingPoint">The point position before move</param>
        /// <param name="destination">The point position after move</param>
        public void movePoint(Point movingPoint, Point destination)
        {
            if (features.SelectedType.Equals("Cluster"))
            {
                Cluster currentCluster = (Cluster)features.SelectedFeature;
                currentCluster.MovePoint(movingPoint, destination);
            }
            else if (features.SelectedType.Equals("Inclusion"))
            {
                Inclusion currentInclusion = (Inclusion)features.SelectedFeature;
                currentInclusion.MovePoint(movingPoint, destination);
            }
        }

        /// <summary>
        /// Move the position of the currently selected feature
        /// </summary>
        /// <param name="edgeToMove">First or Second edge (Layer only)</param>
        /// <param name="xMove">The x position to move the feature to</param>
        /// <param name="yMove">The y position to move the feature to</param>
        public void moveCurrentFeature(int edgeToMove, int xMove, int yMove)
        {
            if (features.SelectedType.Equals("Layer"))
            {
                Layer currentLayer = (Layer)features.SelectedFeature;
                currentLayer.MoveEdge(edgeToMove, xMove, yMove);
            }
            else if (features.SelectedType.Equals("Cluster"))
            {
                Cluster currentCluster = (Cluster)features.SelectedFeature;
                currentCluster.MoveCluster(xMove, yMove);
            }
            else if (features.SelectedType.Equals("Inclusion"))
            {
                Inclusion currentInclusion = (Inclusion)features.SelectedFeature;
                currentInclusion.MoveInclusion(xMove, yMove);
            }
        }

        /// <summary>
        /// Adds a point to the currently selected cluster/incusion after the given position
        /// </summary>
        /// <param name="selectedCluster"></param>
        /// <param name="addAfter"></param>
        /// <param name="addPoint"></param>
        public void AddPoint(Point addPoint, int addAfter)
        {
            if (features.SelectedType.Equals("Cluster"))
            {
                Cluster currentCluster = (Cluster)features.SelectedFeature;
                currentCluster.AddPoint(addPoint, addAfter);
            }
            else if (features.SelectedType.Equals("Inclusion"))
            {
                Inclusion currentInclusion = (Inclusion)features.SelectedFeature;
                currentInclusion.AddPoint(addPoint, addAfter);
            }
        }

        /// <summary>
        /// Deletes a point from the currently selected feature (Cluster or inclusion only)
        /// </summary>
        /// <param name="deletePoint">The p[osition of the point to delete</param>
        public void deletePoint(Point deletePoint)
        {
            if (features.SelectedType.Equals("Cluster"))
            {
                Cluster currentCluster = (Cluster)features.SelectedFeature;
                currentCluster.RemovePoint(deletePoint);
            }
            else if (features.SelectedType.Equals("Inclusion"))
            {
                Inclusion currentInclusion = (Inclusion)features.SelectedFeature;
                currentInclusion.RemovePoint(deletePoint);
            }
        }

        # region Layer methods

        /// <summary>
        /// Adds a new boreholelayer to the Features
        /// </summary>
        /// <param name="depth">The depth of the layer</param>
        /// <param name="amplitude">The amplitude of the layer</param>
        /// <param name="azimuth">The azimuth of the layer</param>
        public void addNewLayer(int depth, int amplitude, int azimuth)
        {
            features.AddNewLayer(depth, amplitude, azimuth);
        }

        public void AddNewLayer(double slope, int intercept)
        {
            features.AddNewLayer(slope, intercept);
        }

        /// <summary>
        /// Adds a layer to the Features
        /// </summary>
        /// <param name="layerToAdd">The layer to add</param>
        public void AddCompleteLayer(Layer layerToAdd)
        {
            layerToAdd.SetBoreholeStartDepth(boreholeStartDepth);
            layerToAdd.DepthResolution = depthResolution;

            features.AddCompleteLayer(layerToAdd);
        }

        /// <summary>
        /// Changes the amplitude of the top sine of the current layer by a specified amount
        /// </summary>
        /// <param name="changeBy">The amount to change the amplitude by</param>
        public void changeTopAmplitudeOfSelectedLayer(int changeBy)
        {
            if (features.SelectedType.Equals("Layer"))
            {
                Layer currentLayer = (Layer)features.SelectedFeature;

                currentLayer.ChangeTopAmplitudeBy(changeBy);
            }
        }

        /// <summary>
        /// Changes the amplitude of the bottom sine of the current layer by a specified amount
        /// </summary>
        /// <param name="changeBy">The amount to change the amplitude by</param>
        public void changeBottomAmplitudeOfSelectedLayer(int changeBy)
        {
            if (features.SelectedType.Equals("Layer"))
            {
                Layer currentLayer = (Layer)features.SelectedFeature;

                currentLayer.ChangeBottomAmplitudeBy(changeBy);
            }
        }

        public void DeleteLayersInRange(int startDepth, int endDepth)
        {
            features.DeleteLayersInRange(startDepth, endDepth);
        }

        /// <summary>
        /// Deletes all layers in features list
        /// </summary>
        public void DeleteAllLayers()
        {
            features.DeleteAllLayers();
        }

        public void RemoveLayersWithQualityLessThan(int quality)
        {
            features.RemoveLayersWithQualityLessThan(quality);
        }

        public void JoinEdges(Layer previouslySelectedFeature, Layer currentlySelectedFeature)
        {
            features.JoinEdges(previouslySelectedFeature, currentlySelectedFeature);
        }

        public void SplitLayer(Layer layerToSplit)
        {
            features.SplitLayer(layerToSplit);
        }

        # endregion

        # region Group methods

        public void AddLayerGroup(string group)
        {
            if (NotInLayerGroups(group))
            {
                layerGroups.Add(Tuple.Create(group, Color.LawnGreen));

                features.SetAllLayerGroupNames(GetLayerGroupNames());
            }

            if (boreholeImage != null)
                _modelAdapter.RefreshBoreholeImage();
        }

        private bool NotInLayerGroups(string group)
        {
            for (int i = 0; i < layerGroups.Count; i++)
            {
                if (group == layerGroups[i].Item1)
                    return false;
            }

            return true;
        }

        public void AddClusterGroup(string group)
        {
            if (NotInClusterGroups(group))
            {
                clusterGroups.Add(Tuple.Create(group, Color.Purple));

                features.SetAllClusterGroupNames(GetClusterGroupNames());
            }

            if (boreholeImage != null)
                _modelAdapter.RefreshBoreholeImage();
        }

        private bool NotInClusterGroups(string group)
        {
            for (int i = 0; i < clusterGroups.Count; i++)
            {
                if (group == clusterGroups[i].Item1)
                    return false;
            }

            return true;
        }

        public void AddInclusionGroup(string group)
        {
            if (NotInInclusionGroups(group))
            {
                inclusionGroups.Add(Tuple.Create(group, Color.Blue));

                features.SetAllInclusionGroupNames(GetInclusionGroupNames());
            }

            if (boreholeImage != null)
                _modelAdapter.RefreshBoreholeImage();
        }

        private bool NotInInclusionGroups(string group)
        {
            for (int i = 0; i < inclusionGroups.Count; i++)
            {
                if (group == inclusionGroups[i].Item1)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Calculates and returns the colour of the layer at the given position
        /// </summary>
        /// <param name="layer"></param>
        public Color GetLayerColour(Layer layer)
        {
            int position = features.GetPositionOf(layer);

            Color layerColour = new Color();

            layerColour = GetLayerColour(position);

            return layerColour;
        }

        /// <summary>
        /// Calculates and returns the colour of the layer at the given position
        /// </summary>
        /// <param name="layer"></param>
        public Color GetLayerColour(int layerNum)
        {
            Color layerColor = new Color();

            if (features.NumOfLayers > layerNum)
            {
                string group = features.GetLayerGroup(layerNum);

                for (int i = 0; i < layerGroups.Count; i++)
                {
                    if (layerGroups[i].Item1 == group)
                    {
                        layerColor = layerGroups[i].Item2;
                        return layerColor;
                    }
                }
            }
            else
                layerColor = Color.LawnGreen;

            return layerColor;
        }

        /// <summary>
        /// Calculates and returns the colour of the given cluster
        /// </summary>
        /// <param name="layer"></param>
        public Color GetClusterColour(Cluster cluster)
        {
            int position = features.GetPositionOf(cluster);

            Color clusterColour = new Color();

            clusterColour = GetClusterColour(position);

            return clusterColour;
        }

        public Color GetClusterColour(int clusterNum)
        {
            Color clusterColour = new Color();

            if (features.NumOfClusters > clusterNum)
            {
                string group = features.GetClusterGroup(clusterNum);

                for (int i = 0; i < clusterGroups.Count; i++)
                {
                    if (clusterGroups[i].Item1 == group)
                    {
                        clusterColour = clusterGroups[i].Item2;
                        return clusterColour;
                    }
                }
            }
            else
                clusterColour = Color.Purple;

            return clusterColour;
        }

        /// <summary>
        /// Calculates and returns the colour of the given cluster
        /// </summary>
        /// <param name="layer"></param>
        public Color GetInclusionColour(Inclusion inlcusion)
        {
            int position = features.GetPositionOf(inlcusion);

            Color inclusionColour = new Color();

            inclusionColour = GetInclusionColour(position);

            return inclusionColour;
        }

        public Color GetInclusionColour(int inlcusionNum)
        {
            Color inlcusionColour = new Color();

            if (features.GetNumOfInlcusions() > inlcusionNum)
            {
                string group = features.GetInclusionGroup(inlcusionNum);

                for (int i = 0; i < inclusionGroups.Count; i++)
                {
                    if (inclusionGroups[i].Item1 == group)
                    {
                        inlcusionColour = inclusionGroups[i].Item2;
                        return inlcusionColour;
                    }
                }
            }
            else
                inlcusionColour = Color.Blue;

            return inlcusionColour;
        }

        public void ChangeLayerGroupColour(string groupName, Color newColour)
        {
            for (int i = 0; i < layerGroups.Count; i++)
            {
                if (layerGroups[i].Item1 == groupName)
                {
                    layerGroups[i] = Tuple.Create(groupName, newColour);

                    _modelAdapter.RefreshBoreholeImage();
                    SaveGroups();
                    break;
                }
            }
        }

        public void RenameLayerGroup(string groupNameBefore, string groupNameAfter)
        {
            for (int i = 0; i < layerGroups.Count; i++)
            {
                if (layerGroups[i].Item1 == groupNameBefore)
                {
                    Color currentColour = layerGroups[i].Item2;

                    layerGroups[i] = Tuple.Create(groupNameAfter, currentColour);

                    break;
                }
            }

            ChangeLayerGroupName(groupNameBefore, groupNameAfter);

            SaveGroups();
        }

        /// <summary>
        /// Changes the groupName of every layer with a given groupname
        /// </summary>
        /// <param name="groupNameBefore"></param>
        /// <param name="groupNameAfter"></param>
        private void ChangeLayerGroupName(string groupNameBefore, string groupNameAfter)
        {
            features.SetAllLayerGroupNames(GetLayerGroupNames());
            features.ChangeLayerGroupName(groupNameBefore, groupNameAfter);
        }

        public void RenameClusterGroup(string groupNameBefore, string groupNameAfter)
        {
            for (int i = 0; i < clusterGroups.Count; i++)
            {
                if (clusterGroups[i].Item1 == groupNameBefore)
                {
                    Color currentColour = clusterGroups[i].Item2;

                    clusterGroups[i] = Tuple.Create(groupNameAfter, currentColour);

                    break;
                }
            }

            ChangeClusterGroupName(groupNameBefore, groupNameAfter);

            SaveGroups();
        }

        /// <summary>
        /// Changes the groupName of every cluster with a given groupname
        /// </summary>
        /// <param name="groupNameBefore"></param>
        /// <param name="groupNameAfter"></param>
        private void ChangeClusterGroupName(string groupNameBefore, string groupNameAfter)
        {
            features.SetAllClusterGroupNames(GetClusterGroupNames());
            features.ChangeClusterGroupName(groupNameBefore, groupNameAfter);
        }

        public void RenameInclusionGroup(string groupNameBefore, string groupNameAfter)
        {
            for (int i = 0; i < inclusionGroups.Count; i++)
            {
                if (inclusionGroups[i].Item1 == groupNameBefore)
                {
                    Color currentColour = inclusionGroups[i].Item2;

                    inclusionGroups[i] = Tuple.Create(groupNameAfter, currentColour);

                    break;
                }
            }

            ChangeInclusionGroupName(groupNameBefore, groupNameAfter);

            SaveGroups();
        }

        public void ChangeClusterGroupColour(string groupName, Color newColour)
        {
            for (int i = 0; i < clusterGroups.Count; i++)
            {
                if (clusterGroups[i].Item1 == groupName)
                {
                    clusterGroups[i] = Tuple.Create(groupName, newColour);

                    _modelAdapter.RefreshBoreholeImage();
                    SaveGroups();
                    break;
                }
            }
        }

        public void ChangeInclusionGroupColour(string groupName, Color newColour)
        {
            for (int i = 0; i < inclusionGroups.Count; i++)
            {
                if (inclusionGroups[i].Item1 == groupName)
                {
                    inclusionGroups[i] = Tuple.Create(groupName, newColour);

                    _modelAdapter.RefreshBoreholeImage();
                    SaveGroups();
                    break;
                }
            }
        }

        /// <summary>
        /// Changes the groupName of every cluster with a given groupname
        /// </summary>
        /// <param name="groupNameBefore"></param>
        /// <param name="groupNameAfter"></param>
        private void ChangeInclusionGroupName(string groupNameBefore, string groupNameAfter)
        {
            features.SetAllInclusionGroupNames(GetClusterGroupNames());
            features.ChangeInclusionGroupName(groupNameBefore, groupNameAfter);
        }

        public void DeleteLayerGroup(string groupName)
        {
            features.ChangeLayerGroupName(groupName, "Unspecified");

            for (int i = 0; i < layerGroups.Count; i++)
            {
                if (layerGroups[i].Item1 == groupName)
                    layerGroups.RemoveAt(i);
            }

            features.SetAllLayerGroupNames(GetLayerGroupNames());

            _modelAdapter.RefreshBoreholeImage();

            SaveGroups();
        }

        public void DeleteClusterGroup(string groupName)
        {
            features.ChangeClusterGroupName(groupName, "Unspecified");

            for (int i = 0; i < clusterGroups.Count; i++)
            {
                if (clusterGroups[i].Item1 == groupName)
                    clusterGroups.RemoveAt(i);
            }

            features.SetAllClusterGroupNames(GetClusterGroupNames());

            _modelAdapter.RefreshBoreholeImage();

            SaveGroups();
        }

        public void DeleteInclusionGroup(string groupName)
        {
            features.ChangeInclusionGroupName(groupName, "Unspecified");

            for (int i = 0; i < inclusionGroups.Count; i++)
            {
                if (inclusionGroups[i].Item1 == groupName)
                    inclusionGroups.RemoveAt(i);
            }

            features.SetAllInclusionGroupNames(GetInclusionGroupNames());

            _modelAdapter.RefreshBoreholeImage();

            SaveGroups();
        }

        /// <summary>
        /// Returns the number of layers in the given group
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public int GetLayerGroupCount(string groupName)
        {
            int count = 0;

            count = features.GetLayerGroupCount(groupName);

            return count;
        }

        public int GetClusterGroupCount(string groupName)
        {
            int count = 0;

            count = features.GetClusterGroupCount(groupName);

            return count;
        }

        public int GetInclusionGroupCount(string groupName)
        {
            int count = 0;

            count = features.GetInclusionGroupCount(groupName);

            return count;
        }

        public void ChangeSelectedFeaturesGroup(string group)
        {
            features.ChangeSelectedFeaturesGroup(group);
        }

        # endregion

        # region Cluster methods

        /// <summary>
        /// Adds a new cluster to the Features
        /// </summary>
        public void addNewCluster()
        {
            features.AddCluster();
        }

        /// <summary>
        /// Adds a point to the currently selected cluster
        /// </summary>
        /// <param name="xPoint">The x position of the point to add</param>
        /// <param name="yPoint">The y position of the point to add</param>
        public void addToCurrentCluster(int xPoint, int yPoint)
        {
            Cluster currentCluster = (Cluster)features.SelectedFeature;
            currentCluster.AddPoint(new Point(xPoint, yPoint));
        }

        /// <summary>
        /// Deletes the specified cluster from the Features
        /// </summary>
        /// <param name="clusterToDelete">The number of the cluster to delete</param>
        public void deleteCluster(Cluster clusterToDelete)
        {
            features.DeleteCluster(clusterToDelete);
        }

        public void DeleteAllClusters()
        {
            features.DeleteAllClusters();
        }

        # endregion

        # region Inclusion methods

        /// <summary>
        /// Adds a new inclusion to the Features
        /// </summary>
        public void createNewInclusion()
        {
            features.AddInclusion();
        }

        /// <summary>
        /// Adds a new point to the inclusion
        /// </summary>
        /// <param name="xPoint">The x position of the point to add</param>
        /// <param name="yPoint">The y position of the point to add</param>
        public void addToCurrentInclusion(int xPoint, int yPoint)
        {
            Inclusion currentInclusion = (Inclusion)features.SelectedFeature;
            currentInclusion.AddPoint(new Point(xPoint, yPoint));
        }

        /// <summary>
        /// Deletes the specified inclusion from the Features
        /// </summary>
        /// <param name="inclusionToDelete">The number of the inclusion to delete</param>
        public void deleteInclusion(Inclusion inclusionToDelete)
        {
            features.DeleteInclusion(inclusionToDelete);
        }

        public void DeleteAllInclusions()
        {
            features.DeleteAllInclusions();
        }

        # endregion

        # endregion

        # region set methods

        /// <summary>
        /// Sets the model adapter
        /// </summary>
        /// <param name="a"></param>
        public void setAdapter(IModelAdapter _modelAdapter)
        {
            this._modelAdapter = _modelAdapter;
        }

        public void RemoveFluidLevel()
        {
            features.RemoveFluidLevel();
        }

        /// <summary>
        /// Checks if there is a feature at the selected co-ordinates and if so sets the equivalent feature type
        /// </summary>
        /// <param name="xPoint">The x position to check</param>
        /// <param name="yPoint">The y position to check</param>
        public void setActiveFeatureType(int xPoint, int yPoint)
        {
            features.setActiveFeatureType(xPoint, yPoint);
        }

        public bool LayerAtLastClick
        {
            get { return features.LayerAtLastClick; }
        }

        public bool ClusterAtLastClick
        {
            get { return features.ClusterAtLastClick; }
        }

        public bool InclusionAtLastClick
        {
            get { return features.InclusionAtLastClick; }
        }


        /// <summary>
        /// Sets the last added cluster as completed
        /// </summary>
        public void setLastClusterAsComplete()
        {
            features.SetLastClusterAsComplete();
        }

        /// <summary>
        /// Sets the last added inclusion as completed
        /// </summary>
        public void setLastInclusionAsComplete()
        {
            features.SetLastInclusionAsComplete();
        }

        # endregion

        # region get methods

        public string ImageType
        {
            get { return type; }
        }
        
        /// <summary>
        /// Returns the number of layers after getting it from the Features
        /// </summary>
        /// <returns>The number of layers in the Features</returns>
        public int getNumOfLayers()
        {
            return features.NumOfLayers;
        }

        /// <summary>
        /// Returns the number of clusters after getting it from the Features
        /// </summary>
        /// <returns>The number of layers in the Features</returns>
        public int getNumOfClusters()
        {
            return features.GetNumOfClusters();
        }

        /// <summary>
        /// Returns the number of inclusions after getting it from the Features
        /// </summary>
        /// <returns>The number of inclusions in the Features</returns>
        public int getNumOfInclusions()
        {
            return features.NumOfInclusions;
        }

        /// <summary>
        /// Method which returns a bool value representing whether the fluid level of the borehole
        /// is set or not
        /// </summary>
        /// <returns>True if level is set false if not</returns>
        public bool getIsFluidLevelSet()
        {
            return features.FluidLevelSet;
        }

        /// <summary>
        /// Returns a List of Points from the first sine wave of the specified layer
        /// </summary>
        /// <param name="layerNum">The layer number</param>
        /// <returns>The List of Points</returns>
        public List<Point> getLayerPoints1(int layerNum)
        {
            if (features.NumOfLayers > layerNum)
                return features.GetLayerPoints1(layerNum);
            else
                return null;
        }

        /// <summary>
        /// Returns a List of Points from the second sine wave of the specified layer
        /// </summary>
        /// <param name="layerNum">The layer number</param>
        /// <returns>The List of Points</returns>
        public List<Point> getLayerPoints2(int layerNum)
        {
            if (features.NumOfLayers > layerNum)
                return features.GetLayerPoints2(layerNum);
            else
                return null;
        }

        public int GetLayerMin(int layerNum)
        {
            return features.GetLayerMin(layerNum);
        }

        public int GetLayerMax(int layerNum)
        {
            return features.GetLayerMax(layerNum);
        }


        /// <summary>
        /// Returns a List of Points representing the specified cluster
        /// </summary>
        /// <param name="clusterNum">The cluster number</param>
        /// <returns>The List of Points</returns>
        public List<Point> GetClusterPoints(int clusterNum)
        {
            if (features.GetNumOfClusters() > clusterNum)
                return features.GetClusterPoints(clusterNum);
            else
                return null;
        }

        /// <summary>
        /// Returns a List of Points representing the specified inclusion
        /// </summary>
        /// <param name="clusterNum">The inclusion number</param>
        /// <returns>The List of Points</returns>
        public List<Point> getInclusionPoints(int inclusionNum)
        {
            if (features.NumOfInclusions > inclusionNum)
                return features.GetInclusionPoints(inclusionNum);
            else
                return null;
        }

        /// <summary>
        /// Method which checks if there is a feature at the given co-ordinates
        /// </summary>
        /// <param name="xPoint">The x position to check</param>
        /// <param name="yPoint">The y posiition to check</param>
        /// <returns>True if there is a feature at the given point, false if there is not</returns>
        public bool getIsOverCurrentFeature(int xPoint, int yPoint)
        {
            object selectedFeature = features.SelectedFeature;

            if (features.SelectedType.Equals("Layer"))
            {
                Layer selectedLayer = (Layer)selectedFeature;
                int low, high;

                low = selectedLayer.GetTopEdgePoints()[xPoint].Y;
                high = selectedLayer.GetBottomEdgePoints()[xPoint].Y;

                if (yPoint > low - 6 && yPoint < high + 6)
                    return true;
                else
                    return false;
            }
            else if (features.SelectedType.Equals("Cluster") || features.SelectedType.Equals("Inclusion"))
            {
                if (CheckIfPointIsWithinShapeBounds(xPoint, yPoint, selectedFeature) == true)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            //else if (features.SelectedType.Equals("Cluster"))
            //{
                //Cluster selectedCluster = (Cluster)selectedFeature;
                //if (xPoint >= selectedCluster.LeftXBoundary - 3 && xPoint <= selectedCluster.RightXBoundary + 3
                //    && yPoint >= selectedCluster.TopYBoundary - 3 && yPoint <= selectedCluster.BottomYBoundary + 3)
                //{
                //    return true;
                //}

                //if (features.checkIfClusterIsAtPoint(xPoint, yPoint) == true)
                //    return true;

            //}
            //else if (features.SelectedType.Equals("Inclusion"))
            //{
                //Inclusion selectedInclusion = (Inclusion)selectedFeature;
                //if (xPoint >= selectedInclusion.LeftXBoundary - 3 && xPoint <= selectedInclusion.RightXBoundary + 3
                //    && yPoint >= selectedInclusion.TopYBoundary - 3 && yPoint <= selectedInclusion.BottomYBoundary + 3)
                //{
                //    return true;
                //}

                //if (features.checkIfInclusionIsAtPoint(xPoint, yPoint) == true)
                //     return true;

            //}

            return false;
        }

        private bool CheckIfPointIsWithinShapeBounds(int xPoint, int yPoint, object selectedFeature)
        {
            //Inclusion selectedInclusion = (Inclusion)selectedFeature;

            int leftXBoundary = 0;
            int rightXBoundary = 0;
            int topYBoundary = 0;
            int bottomYBoundary = 0;

            if (features.SelectedType.Equals("Cluster"))
            {
                leftXBoundary = ((Cluster)selectedFeature).LeftXBoundary;
                rightXBoundary = ((Cluster)selectedFeature).RightXBoundary;
                topYBoundary = ((Cluster)selectedFeature).TopYBoundary;
                bottomYBoundary = ((Cluster)selectedFeature).BottomYBoundary;                
            }
            else if (features.SelectedType.Equals("Inclusion"))
            {
                leftXBoundary = ((Inclusion)selectedFeature).LeftXBoundary;
                rightXBoundary = ((Inclusion)selectedFeature).RightXBoundary;
                topYBoundary = ((Inclusion)selectedFeature).TopYBoundary;
                bottomYBoundary = ((Inclusion)selectedFeature).BottomYBoundary;
            }

            if (xPoint >= leftXBoundary - 3 && xPoint <= rightXBoundary + 3
                && yPoint >= topYBoundary - 3 && yPoint <= bottomYBoundary + 3)
            {
                return true;
            }

            if (BoundsAreNegative(leftXBoundary))
            {
                leftXBoundary += azimuthResolution;
                rightXBoundary += azimuthResolution;

                if (xPoint >= leftXBoundary - 3 && xPoint <= rightXBoundary + 3
                && yPoint >= topYBoundary - 3 && yPoint <= bottomYBoundary + 3)
                {
                    return true;
                }
            }
            else if (BoundsArePositive(rightXBoundary))
            {
                leftXBoundary -= azimuthResolution;
                rightXBoundary -= azimuthResolution;

                if (xPoint >= leftXBoundary - 3 && xPoint <= rightXBoundary + 3
                && yPoint >= topYBoundary - 3 && yPoint <= bottomYBoundary + 3)
                {
                    return true;
                }
            }

            return false;
        }

        private bool BoundsAreNegative(int leftXBound)
        {
            if (leftXBound < 0)
                return true;
            else
                return false;
        }

        private bool BoundsArePositive(int rightXBound)
        {
            if (rightXBound >= azimuthResolution)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Returns the fluid level in the borehole
        /// </summary>
        /// <returns>The fluid level</returns>
        public int FluidLevel
        {
            get { return features.FluidLevel; }
            set { features.FluidLevel = value; }
        }     

        public int GetCurrentSectionStart()
        {
            return sectionStart;
        }

        public int GetCurrentSectionEnd()
        {
            return sectionEnd;
        }

        public FileTiler getCurrentTiler()
        {
            return tiler;
        }

        /// <summary>
        /// Returns an instance of FileTiler
        /// </summary>
        /// <param name="sectionHeight">The tiler section size</param>
        /// <returns></returns>
        public FileTiler GetNewTiler(int sectionHeight)
        {
            FileTilerSelector factory = new FileTilerSelector("FeaturesFile");

            string imageFilePath = saveLoadData.getImageDataFilePath();
            FileTiler fileTiler = factory.setUpTiler(imageFilePath, sectionHeight);

            return fileTiler;
        }

        public string[] GetLayerGroupNames()
        {
            string[] layerGroupNames = new string[layerGroups.Count];

            for (int i = 0; i < layerGroupNames.Length; i++)
            {
                layerGroupNames[i] = layerGroups[i].Item1;
            }

            return layerGroupNames;
        }

        public string[] GetClusterGroupNames()
        {
            string[] clusterGroupNames = new string[clusterGroups.Count];

            for (int i = 0; i < clusterGroupNames.Length; i++)
            {
                clusterGroupNames[i] = clusterGroups[i].Item1;
            }

            return clusterGroupNames;
        }

        public string[] GetInclusionGroupNames()
        {
            string[] inclusionGroupNames = new string[inclusionGroups.Count];

            for (int i = 0; i < inclusionGroupNames.Length; i++)
            {
                inclusionGroupNames[i] = inclusionGroups[i].Item1;
            }

            return inclusionGroupNames;
        }

        public List<Tuple<string, Color>> GetLayerGroups()
        {
            return layerGroups;
        }

        public List<Tuple<string, Color>> GetClusterGroups()
        {
            return clusterGroups;
        }

        public List<Tuple<string, Color>> GetInclusionGroups()
        {
            return inclusionGroups;
        }

        public string GetProjectLocation()
        {
            return projectFileRoot;
        }

        

        # endregion
    }
}
