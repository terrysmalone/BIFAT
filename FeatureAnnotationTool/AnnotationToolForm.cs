using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Threading;
using FeatureAnnotationTool.DialogBoxes;
using FeatureAnnotationTool.Interfaces;
using FeatureAnnotationTool.AutoDetect;
using ImageTiler;
using RGReferencedData;
using BoreholeFeatures;
using System.IO;

namespace FeatureAnnotationTool
{
    /// <summary>
    /// Author - Terry Malone (trm8@aber.ac.uk)
    /// Version 1.1
    ///  
    ///This program is free software; you can redistribute it and/or
    ///modify it under the terms of the GNU General Public License
    ///as published by the Free Software Foundation; either version 2
    ///of the License, or (at your option) any later version.
    ///
    ///This program is distributed in the hope that it will be useful,
    ///but WITHOUT ANY WARRANTY; without even the implied warranty of
    ///MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ///GNU General Public License for more details.
    /// </summary>
    public partial class AnnotationToolForm : Form, IView
    {
        private bool drawTestImages = true; //This sets whether test images are drawn or not ***drawTestImages***

        # region variables

        private bool isExternal = false;

        private IViewAdapter _viewAdapter;
        private TableLayoutSettings tableLayoutSettings;
        private OptionsForm optionsForm;

        private Bitmap boreholeSectionImage, boreholePreviewImage, fullPreviewImage;

        private string boreholeName;
        private int boreholeWidth, boreholeSectionHeight;

        private Point firstLayerPoint, secondLayerPoint;
        private bool firstLayerPointSelected;

        private string selectedButton = "";

        private int sectionStart, sectionEnd;

        private long totalToAutoDetect = 0;

        private bool previewMouseDown;
        private bool movingWholeLayer, alteringWholeAmplitude, alteringCluster, alteringInclusion, movingTopEdge, alteringTopAmplitude, movingBottomEdge, alteringBottomAmplitude, alteringThickness, alteringClusterPoint, alteringInclusionPoint;

        //Preview variables
        private float scrollSize;
        private float scrollPosition;

        private int previewHeight;

        //Full preview variables
        int fullPreviewScrollPosition;
        int fullPreviewScrollSize;

        private int previousX, previousY;
        //private int xMove, yMove;
        private int xMousePos, yMousePos;

        private Point movingPoint;

        private bool creatingCluster, creatingInclusion;
        private Point clusterStartPoint, inclusionStartPoint;

        private int numberofPointsInCluster = 0;
        private int numberofPointsInInclusion = 0;

        private bool autoDetectRunning = false;
        private bool mousePressed;
        
        //private bool previouslySaved = false;
        private string fileName;

        private object previouslySelectedFeature;
        private object currentlySelectedFeature;

        private bool changedSinceLastSave;

        private ReferencedChannel optvChannel;
        private string workingPath;

        private int featureOpacity;

        //Graphics variables
        private Pen layerPen = new Pen(Color.LawnGreen, 1);
        private Pen clusterPen = new Pen(Color.Purple, 1);
        private Pen inclusionPen = new Pen(Color.Blue, 1);
        private Pen selectedPen = new Pen(Color.White, 3);
        private Pen fluidLevelPen = new Pen(Color.Blue, 2);

        private AutoDetectParametersForm autoDetectForm;
        private HoughTransformAutoDetectParametersForm houghAutoDetectForm;
        private ProgressReportForm progressReportForm;

        private int currentAutoDetectProgress;
        private int totalAutoDetectProgress;

        private Thread autoDetectThread;
        private Thread progressReportThread;
        private string progressBarTitle;
        private bool userClickedComboBox = false;

        private bool drawingFeaturesImage = false;

        private bool autoSaveEnabled = true;

        private int rotation = 0;
        private int currentImageRotation = 0;

        private int rotationShift = 0;          //The amount of pixels images are shifted by

        private List<Rectangle> autoConvergingRegions = new List<Rectangle>();

        # region Autodetect parameters 

        private float cannyLow, cannyHigh;
        private int gaussianWidth, gaussianAngle;

        private float verticalWeight, horizontalWeight;
        private int edgeLinkingThreshold, edgeLinkingDistance, edgeRemovalThreshold, edgeJoiningThreshold;

        private int edgeJoinBonus, edgeLengthBonus, peakThreshold, maxSineAmplitude;

        private int layerSensitivity;
        private bool disableLayerDetection;

        private string autoDetectType;
        
        # endregion

        # endregion

        /// <summary>
        /// Constructor method
        /// </summary>
        public AnnotationToolForm()
        {
            isExternal = false;

            InitializeComponent();
            SetToolStripLayout();

            toolStripStatusLabel.Text = "Select 'New' and choose borehole image or select 'Open' to load previously created '.features' file";

            featureDetailsPropertyGrid.Visible = false;
            firstLayerPointSelected = false;

            DisableRotationButtons();
            DisableActiveBoreholeMenuItems();

            optionsForm = new OptionsForm(this);
            DisableSectionChangeObjects();
            DisableAutoDetectToolStripItems();

            this.KeyPreview = true;

            AddPictureBoxToolTips();
        }

        private void AddPictureBoxToolTips()
        {
            // Create the ToolTip and associate with the Form container.
            ToolTip toolTip1 = new ToolTip();

            // Set up the delays for the ToolTip.
            toolTip1.AutoPopDelay = 5000;
            toolTip1.InitialDelay = 600;
            toolTip1.ReshowDelay = 500;
            // Force the ToolTip text to be displayed whether or not the form is active.
            toolTip1.ShowAlways = true;

            // Set up the ToolTip text for the Button and Checkbox.
            toolTip1.SetToolTip(this.fullPreviewPictureBox, "Entire borehole");
            toolTip1.SetToolTip(this.boreholePreviewPictureBox, "Current borehole section");
            
        }

        # region Robertson Geologging calls

        /// <summary>
        /// FOR USE WITH ROBERTSON GEOLLOGGING SOFTWARE ONLY
        /// Constructor which allows for the software to be opened a borehole image loaded straight away
        /// </summary>
        /// <param name="optvChannel"></param>
        /// <param name="workingPath"></param>
        public AnnotationToolForm(ReferencedChannel optvChannel, string workingPath)
        {
            this.optvChannel = optvChannel;
            this.workingPath = workingPath;

            isExternal = true;

            InitializeComponent();
            SetToolStripLayout();

            toolStripStatusLabel.Text = "Select 'New' and choose borehole image or select 'Open' to load previously created '.features' file";

            featureDetailsPropertyGrid.Visible = false;
            firstLayerPointSelected = false;

            DisableSectionChangeObjects();
        }

        /// <summary>
        /// FOR USE WITH ROBERTSON GEOLLOGGING SOFTWARE ONLY
        /// </summary>
        public void openChannel()
        {
            _viewAdapter.openChannel(optvChannel, workingPath);

            boreholePictureBox.Refresh();
            changedSinceLastSave = false;

            ShowFeatureVisibilityItems(true);
        }

        # endregion

        public void SetAdapter(IViewAdapter _viewAdapter)
        {
            this._viewAdapter = _viewAdapter;
        }

        /// <summary>
        /// Set up the tool strip layout
        /// </summary>
        private void SetToolStripLayout()
        {
            featuresToolStrip.LayoutStyle = ToolStripLayoutStyle.Table;
            tableLayoutSettings = featuresToolStrip.LayoutSettings as TableLayoutSettings;

            tableLayoutSettings.ColumnCount = 5;
            tableLayoutSettings.RowCount = 6;

            tableLayoutSettings.SetColumnSpan(featuresToolStripLabel, 5);
            tableLayoutSettings.SetColumnSpan(layersToolStripLabel, 5);
            tableLayoutSettings.SetColumnSpan(clustersToolStripLabel, 5);
            tableLayoutSettings.SetColumnSpan(inclusionsToolStripLabel, 5);
            tableLayoutSettings.SetColumnSpan(fluidLevelToolStripLabel, 5);
        }

        /// <summary>
        /// Resets all GUI items 
        /// </summary>
        private void ResetAll()
        {
            DisableSectionChangeObjects();
            DisableRotationButtons();
            DisableActiveBoreholeMenuItems();
            DisableFeaturesButtons();

            _viewAdapter.DeleteCurrentFeature();
            _viewAdapter.DeSelectFeature();
            _viewAdapter.Reset();

            boreholePictureBox.Visible = false;
            rulerPictureBox.Visible = false;
            boreholePreviewPictureBox.Visible = false;
            fullPreviewPictureBox.Visible = false;

            this.leftPanel.Visible = false;
            this.boreholeImagePanel.Visible = false;
            this.orientationPictureBox.Visible = false;

            toolStripStatusLabel.Text = "Select 'New' and choose borehole image or select 'Open' to load previously created '.features' file";
            this.Text = "Borehole Feature Annotation Tool";
            featureDetailsPropertyGrid.Visible = false;
            creatingCluster = false;
            creatingInclusion = false;

            changedSinceLastSave = false;

            ShowFeatureVisibilityItems(false);

            opacityTrackbar.Value = 100;
            featureOpacity = 100;

            if (autoDetectRunning)
                StopEdgeDetection(false);

            currentImageRotation = 0;
            rotation = 0;
            rotationShift = 0;

            cannyHigh = 0;        
        }

        private void ShowFeatureVisibilityItems(bool visible)
        {
            showFeaturesCheckBox.Visible = visible;
            opacityTrackbar.Visible = visible;
            opacityLabel.Visible = visible;
            opacityMinLabel.Visible = visible;
            opacityMaxLabel.Visible = visible;
            opacityMinLabel.BringToFront();
            opacityMaxLabel.BringToFront();
        }        

        /// <summary>
        /// Opens a modal dialog box which asks the user what action they would like to take:
        /// Create a new file from an image
        /// Create a new file from OTV and HED files
        /// Open an existing .features file
        /// </summary>
        private void OpenSelectOptionDialogBox()
        {
            this.optionsForm.ShowDialog();
        }

        # region Forms change methods

        private void Form1_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;

            if (!isExternal)
                OpenSelectOptionDialogBox();
        }

        /// <summary>
        /// Called when the form is resized to update the position and size of the borehole scroller
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Resized(object sender, EventArgs e)
        {
            previewHeight = boreholePreviewPictureBox.Height;
            int boreholeImagepanelHeight = boreholeImagePanel.Height;

            double position = scrollPosition / ((double)previewHeight / (double)boreholePictureBox.Height);

            if (boreholeSectionHeight != 0 && previewHeight != 0 && boreholeImagepanelHeight != 0 && boreholeSectionHeight > boreholeImagepanelHeight)
                scrollSize = (int)((double)previewHeight / ((double)boreholeSectionHeight / (double)boreholeImagepanelHeight));
            else
                scrollSize = 0;

            Console.WriteLine();
        }

        # endregion

        # region Menu items methods
        
        private void fromImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (changedSinceLastSave == true)
            {
                if (showSaveOrExitDialog() != "Cancel")
                {
                    OpenImageFile();
                }
            }
            else
            {
                OpenImageFile();
            }
        }

        private void fromOtvhedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (changedSinceLastSave == true)
            {
                if (showSaveOrExitDialog() != "Cancel")
                {
                    OpenOTVFile();
                }
            }
            else
            {
                OpenOTVFile();
            }
        }

        # endregion

        # region Display image methods

        /// <summary>
        /// Displays the borehole image in boreholePictureBox
        /// </summary>
        /// <param name="boreholeImage">The image to display</param>

        public void DisplayImage(Bitmap boreholeImage)
        {
            this.boreholeSectionImage = boreholeImage;

            CheckImageRotation();

            //boreholePictureBox.Visible = true;
            //boreholePictureBox.Image = boreholeImage;

            boreholeSectionHeight = boreholeImage.Height;
            boreholeWidth = boreholeImage.Width;

            boreholeImagePanel.Width = boreholePictureBox.Width + 78;
            boreholeImagePanel.VerticalScroll.Value = 0;
            boreholeImagePanel.Update();
            boreholeImagePanel.VerticalScroll.Value = 0;

            orientationPictureBox.Width = boreholeImagePanel.Width;
            rotateRightButton.Location = new Point(boreholeImagePanel.Location.X + boreholeImagePanel.Width, rotateRightButton.Location.Y);

            this.addLayerToolStripButton.Enabled = true;
            this.addClusterToolStripButton.Enabled = true;
            this.addInclusionToolStripButton.Enabled = true;
            this.fluidLevelToolStripButton.Enabled = true;

            this.leftPanel.Visible = true;
            this.boreholeImagePanel.Visible = true;
            this.orientationPictureBox.Visible = true;

            toolStripStatusLabel.Text = "Select features to add";

            DisableFeaturesButtons();
            //enableRotationButtons();
            EnableActiveBoreholeMenuItems();

            featureDetailsPropertyGrid.Visible = false;
            creatingCluster = false;
            creatingInclusion = false;

            EnableSectionChangeObjects();
        }

        private void CheckImageRotation()
        {
            if (currentImageRotation != rotation)
            {
                Bitmap rotatedImage = new Bitmap(boreholeWidth, boreholeSectionHeight);

                ImageRotation imageRotation = new ImageRotation(boreholeSectionImage);
                int rotationAmount = rotation - currentImageRotation;

                if (rotationAmount < 0)
                    rotationAmount = 360 + rotationAmount;

                imageRotation.RotateImageBy(rotationAmount);

                boreholeSectionImage = imageRotation.GetRotatedImage();

                currentImageRotation = rotation;
            }

            boreholePictureBox.Visible = true;
            boreholePictureBox.Image = boreholeSectionImage;
        }        

        /// <summary>
        /// Displays the scaled down version of the image in fullBoreholePictureBox
        /// </summary>
        /// <param name="boreholePreviewImage">The scaled down version of the borehole image</param>
        public void DisplayPreview(Bitmap boreholePreviewImage)
        {
            boreholePreviewPictureBox.Visible = true;

            this.boreholePreviewImage = boreholePreviewImage;
            boreholePreviewPictureBox.Image = boreholePreviewImage;

            scrollSize = (int)((double)boreholePreviewPictureBox.Height / ((double)boreholeSectionHeight / (double)boreholeImagePanel.Height));
        }

        /// <summary>
        /// Displays the preview of the entire borehole
        /// </summary>
        /// <param name="boreholePreviewImage">The scaled down version of the borehole image</param>
        public void DisplayFullPreview(Bitmap fullPreviewImage)
        {
            fullPreviewPictureBox.Visible = true;

            this.fullPreviewImage = fullPreviewImage;
            fullPreviewPictureBox.Image = fullPreviewImage;
        }

        public void SetRulerImage(Bitmap ruler)
        {
            rulerPictureBox.Visible = true;

            rulerPictureBox.Height = ruler.Height + 1;

            rulerPictureBox.Image = ruler;
            rulerPictureBox.Refresh();
        }

        public void DrawOrientationRuler()
        {
            OrientationRuler orientationRuler = new OrientationRuler(rulerPictureBox.Width, boreholeSectionImage.Width, rotation);

            EnableRotationButtons();

            orientationPictureBox.Image = orientationRuler.GetOrientationRulerImage();
            orientationPictureBox.Refresh();
        }

        public void HideOrientationRuler()
        {
            orientationPictureBox.Visible = false;
            rotateLeftButton.Visible = false;
            rotateRightButton.Visible = false;
        }

        # endregion

        /// <summary>
        /// Checks if two points are close togeteher
        /// </summary>
        /// <param name="firstPoint">The first point to compare</param>
        /// <param name="secondPoint">The second point to compare</param>
        /// <returns>True if points are close, false if not</returns>
        private bool PointsAreClose(Point firstPoint, Point secondPoint)
        {
            if (secondPoint.X >= firstPoint.X - 5 && secondPoint.X <= firstPoint.X + 5 &&
                secondPoint.Y >= firstPoint.Y - 5 && secondPoint.Y <= firstPoint.Y + 5)
            {
                return true;
            }
            else
                return false;
        }

        # region Show features methods

        private void showFeaturesCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            boreholePictureBox.Refresh();

            if (showFeaturesCheckBox.Checked == false)
            {
                cancelCreatingOtherFeatures();
                _viewAdapter.DeSelectFeature();
                selectedButton = "";

                DisableFeaturesButtons();

                opacityTrackbar.Value = 0;
            }
            else
            {
                opacityTrackbar.Value = featureOpacity;
            }
        }

        # endregion

        # region Features Details methods

        public void HideFeatureDetailsPropertyGrid()
        {
            featureDetailsPropertyGrid.Visible = false;
        }

        public void ShowFeatureDetailsPropertyGrid()
        {
            object selectedFeature = _viewAdapter.SelectedFeature;

            if (selectedFeature != null)
            {
                featureDetailsPropertyGrid.SelectedObject = selectedFeature;
                featureDetailsPropertyGrid.Visible = true;
                this.Refresh();
            }
        }

        private void featureDetailsPropertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            changedSinceLastSave = true;

            if (e.ChangedItem.Label == "Group")
            {
                if (e.ChangedItem.Parent.Parent.Label == "BoreholeFeatures.BoreholeLayer" || e.ChangedItem.Parent.Parent.Label == "BoreholeFeatures.CoreLayer")
                    _viewAdapter.AddLayerGroup(e.ChangedItem.Value.ToString());
                else if (e.ChangedItem.Parent.Parent.Label == "BoreholeFeatures.Cluster")
                    _viewAdapter.AddClusterGroup(e.ChangedItem.Value.ToString());
                else if (e.ChangedItem.Parent.Parent.Label == "BoreholeFeatures.Inclusion")
                    _viewAdapter.AddInclusionGroup(e.ChangedItem.Value.ToString());

            
                _viewAdapter.SaveGroups();
            } 
            
        }

        # endregion

        private void AutoSave()
        {
            if (autoSaveEnabled)
                Save();
        }

        private void Save()
        {
            _viewAdapter.SaveFeatures();
            changedSinceLastSave = false;
        }

        # region Paint Methods

        /// <summary>
        /// Called whenever the screen is repainted.  It draws all features to
        /// the transparent overlay panel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void DrawAllFeatures(object sender, PaintEventArgs e)
        {
            checkWhetherToDrawAutoDetectArea(e);
            
            if (showFeaturesCheckBox.Checked == true)
            {
                sectionStart = _viewAdapter.GetCurrentSectionStart();
                sectionEnd = _viewAdapter.GetCurrentSectionEnd();

                if (boreholePictureBox.Image != null)
                {
                    DrawFluidLevel(e);

                    DrawAllLayers(e);

                    DrawAllClusters(e);

                    DrawAllInclusions(e);

                    DrawSelectedFeature(e);
                }

                featureDetailsPropertyGrid.SelectedObject = _viewAdapter.SelectedFeature;

            }

            DrawConvergingRegions(e);

            featureDetailsPropertyGrid.Update();
            changeBoreholeScroller(boreholeImagePanel.VerticalScroll.Value);
            boreholePreviewPictureBox.Refresh();
            rulerPictureBox.Refresh();
        }

        private void DrawFluidLevel(PaintEventArgs e)
        {
            if (_viewAdapter.IsFluidLevelSet())
            {
                int fluidLevel = _viewAdapter.FluidLevel;

                //if(fluidLevel >= sectionStart && fluidLevel <= sectionEnd)
                e.Graphics.DrawLine(OpacityPen(fluidLevelPen), 0, fluidLevel - sectionStart, boreholeWidth, fluidLevel - sectionStart);
            }
        }

        # region Draw layers methods

        private void DrawAllLayers(PaintEventArgs e)
        {
            List<Point> pointsToDraw;
            List<Point> pointsToDraw2;

            int numberOfLayers = _viewAdapter.getNumOfLayers();

            int layerToDraw;

            if (drawingFeaturesImage)
            {
                layerToDraw = 0;
            }
            else
            {
                layerToDraw = CalculateFirstLayerToDraw(numberOfLayers);
            }

            bool stopDrawing = false;

            while (stopDrawing == false && layerToDraw < numberOfLayers)
            {
                //Draw the layers first sine
                pointsToDraw = _viewAdapter.getLayer1(layerToDraw);

                layerPen.Color = _viewAdapter.GetLayerColour(layerToDraw);

                DrawSine(e, pointsToDraw, layerPen);

                //Draw the layers second sine
                pointsToDraw2 = _viewAdapter.getLayer2(layerToDraw);

                DrawSine(e, pointsToDraw2, layerPen);

                //Draw filling
                DrawFilling(e, pointsToDraw, pointsToDraw2);
                
                if (!drawingFeaturesImage)
                    stopDrawing = CheckWhetherToStop(layerToDraw);

                layerToDraw++;
            }
        }        

        /// <summary>
        /// Checks the position, in the List of features, of the first layer to be drawn
        /// </summary>
        /// <returns></returns>
        private int CalculateFirstLayerToDraw(int numberOfLayers)
        {
            int scrollStart = boreholeImagePanel.VerticalScroll.Value - 50;

            scrollStart += sectionStart;

            int maxLayerPosition;

            for (int i = 0; i < numberOfLayers; i++)
            {
                maxLayerPosition = _viewAdapter.GetLayerMax(i);

                if (maxLayerPosition > scrollStart)
                    return i;
            }

            return numberOfLayers;
        }

        private bool CheckWhetherToStop(int currentLayer)
        {
            bool stopDrawing;

            int minLayerPosition = _viewAdapter.GetLayerMin(currentLayer);

            int scrollEnd = boreholeImagePanel.VerticalScroll.Value + boreholeImagePanel.Height + 50;

            scrollEnd += sectionStart;

            if (minLayerPosition > scrollEnd)
                stopDrawing = true;
            else
                stopDrawing = false;

            return stopDrawing;
        }

        private void DrawSine(PaintEventArgs e, List<Point> pointsToDraw, Pen layerPen)
        {
            Point firstPoint, secondPoint;
            int firstX, secondX;

            List<Point> points;

            if (rotation != 0)
                points = ShiftSineForRotation(pointsToDraw);
            else
                points = pointsToDraw;

            for (int j = 0; j < points.Count - 5; j += 5)
            {
                firstPoint = points[j];
                secondPoint = points[j + 5];

                firstX = firstPoint.X - rotationShift;

                if (firstX < 0)
                    firstX += boreholeWidth;
                else if (firstX >= boreholeWidth)
                    firstX -= boreholeWidth;

                secondX = secondPoint.X - rotationShift;

                if (secondX < 0)
                    secondX += boreholeWidth;
                else if (secondX >= boreholeWidth)
                    secondX -= boreholeWidth;

                e.Graphics.DrawLine(OpacityPen(layerPen), firstX, firstPoint.Y - sectionStart, secondX, secondPoint.Y - sectionStart);
            }

            firstPoint = points[pointsToDraw.Count - 5];
            secondPoint = points[pointsToDraw.Count - 1];

            firstX = firstPoint.X - rotationShift;

            if (firstX < 0)
                firstX += boreholeWidth;
            else if (firstX >= boreholeWidth)
                firstX -= boreholeWidth;

            secondX = secondPoint.X - rotationShift;

            if (secondX < 0)
                secondX += boreholeWidth;
            else if (secondX >= boreholeWidth)
                secondX -= boreholeWidth;

            e.Graphics.DrawLine(OpacityPen(layerPen), firstX, firstPoint.Y - sectionStart, secondX, secondPoint.Y - sectionStart);
        }

        private Pen OpacityPen(Pen originalPen)
        {
            Color color = originalPen.Color;

            int opacity = Convert.ToInt32(((double)color.A / 100) * (double)featureOpacity);

            Color opacityColor = Color.FromArgb(opacity, color.R, color.G, color.B);
            Pen opacityPen = new Pen(opacityColor, originalPen.Width);

            return opacityPen;
        }

        private Brush OpacityBrush(SolidBrush originalBrush)
        {
            Color color = originalBrush.Color;

            int opacity = Convert.ToInt32(((double)color.A / 100) * (double)featureOpacity);

            Color opacityColor = Color.FromArgb(opacity, color.R, color.G, color.B);
            SolidBrush opacityBrush = new SolidBrush(opacityColor);

            return opacityBrush;
        }

        private void DrawFilling(PaintEventArgs e, List<Point> pointsToDraw, List<Point> pointsToDraw2)
        {
            Color color = layerPen.Color;
            Color semiTransparentColor = Color.FromArgb(70, color.R, color.G, color.B);
            Pen fillingPen = new Pen(semiTransparentColor, 1);

            for (int column = 0; column < pointsToDraw.Count; column += 1)
            {
                int firstX = pointsToDraw[column].X - rotationShift;

                if (firstX < 0)
                    firstX += boreholeWidth;
                else if (firstX >= boreholeWidth)
                    firstX -= boreholeWidth;
                
                int secondX = pointsToDraw2[column].X - rotationShift;

                if (secondX < 0)
                    secondX = secondX + boreholeWidth;
                else if (secondX >= boreholeWidth)
                    secondX -= boreholeWidth;

                e.Graphics.DrawLine(OpacityPen(fillingPen), firstX, pointsToDraw[column].Y - sectionStart, secondX, pointsToDraw2[column].Y - sectionStart);
            }
        }

        private List<Point> ShiftSineForRotation(List<Point> pointsToDraw)
        {
            List<Point> points = new List<Point>();

            for (int i = 0; i < pointsToDraw.Count; i++)
            {
                int oldPosition = i + rotationShift;

                if(oldPosition >= boreholeWidth)
                    oldPosition = oldPosition - boreholeWidth;

                points.Add(pointsToDraw[oldPosition]);
            }

            return points;
        }

        # endregion

        private void DrawAllClusters(PaintEventArgs e)
        {
            int numberOfClusters = _viewAdapter.getNumOfClusters();
            List<Point> pointsToDraw;

            for (int clusterNum = 0; clusterNum < numberOfClusters; clusterNum++)
            {
                pointsToDraw = GetPointToDraw(_viewAdapter.GetClusterPoints(clusterNum));

                clusterPen.Color = _viewAdapter.GetClusterColour(clusterNum);
                
                DrawClusterLines(e, pointsToDraw, clusterNum, numberOfClusters);

                //drawClusterShape(e, pointsToDraw, clusterNum, numberOfClusters);
            }
        }

        private List<Point> GetPointToDraw(List<Point> pointsBeforeConversion)
        {
            List<Point> pointsAfterConversion = new List<Point>();

            foreach (Point beforeConversion in pointsBeforeConversion)
            {
                Point afterConversion = new Point(beforeConversion.X, beforeConversion.Y - sectionStart);
                pointsAfterConversion.Add(afterConversion);
            }

            return pointsAfterConversion;
        }

        private void DrawClusterLines(PaintEventArgs e, List<Point> pointsToDraw, int currentCluster, int numberOfClusters)
        {
            if (currentCluster == numberOfClusters - 1 && creatingCluster)  //Currently drawing cluster
            {
                //draw lines
                if (pointsToDraw.Count > 1)
                {
                    pointsToDraw = ShiftPointsForRotation(pointsToDraw);

                    //e.Graphics.DrawLines(clusterPen, pointsToDraw.ToArray());
                    for (int i = 0; i < pointsToDraw.Count - 1; i++)
                    {
                        e.Graphics.DrawLine(OpacityPen(clusterPen), pointsToDraw[i].X, pointsToDraw[i].Y, pointsToDraw[i + 1].X, pointsToDraw[i + 1].Y);
                    }
                }
            }
            else
            {
                Color color = clusterPen.Color;
                Color transparentClusterColour = Color.FromArgb(70, color.R, color.G, color.B);

                Point[] pointsArray = ShiftPointsForRotation(pointsToDraw.ToArray());
                
                //if feature crosses over current view draw two polygons
                e.Graphics.DrawPolygon(OpacityPen(clusterPen), pointsArray);
                e.Graphics.FillPolygon(OpacityBrush(new SolidBrush(transparentClusterColour)), pointsArray);

                //if any points occur outside range  draw secondary shape
                if (pointsArray.Min(p => p.X) < 0)
                {
                    Point[] movedPoints = AddAmountToXPoints(pointsArray, boreholeWidth);
                    //Draw shape at end as well
                    e.Graphics.DrawPolygon(OpacityPen(clusterPen), movedPoints);
                    e.Graphics.FillPolygon(OpacityBrush(new SolidBrush(transparentClusterColour)), movedPoints);
                }
                else if (pointsArray.Max(p => p.X) > boreholeWidth)
                {
                    Point[] movedPoints = AddAmountToXPoints(pointsArray, -boreholeWidth);
                    
                    //Draw shape at start as well
                    e.Graphics.DrawPolygon(OpacityPen(clusterPen), movedPoints);
                    e.Graphics.FillPolygon(OpacityBrush(new SolidBrush(transparentClusterColour)), movedPoints);
                }
            }
        }

        private Point[] AddAmountToXPoints(Point[] points, int amount)
        {
            Point[] movedPoints = new Point[points.Length];

            for (int i = 0; i < points.Length; i++)
            {
                movedPoints[i] = new Point(points[i].X + amount, points[i].Y);
            }

            return movedPoints;
        }

        private List<Point> AddAmountToXPoints(List<Point> points, int amount)
        {
            List<Point> movedPoints = new List<Point>();

            for (int i = 0; i < points.Count; i++)
            {
                movedPoints.Add(new Point(points[i].X + amount, points[i].Y));
            }

            return movedPoints;
        }

        private Point[] ShiftPointsForRotation(Point[] points)
        {
            Point[] shiftedPoints = new Point[points.Length];

            int xMin = points.Min(p => p.X);
            int xMax = points.Max(p => p.X);

            int moveAmount = rotationShift;

            if (xMin - rotationShift < 0 && xMax - rotationShift < 0)
                moveAmount = 0-(boreholeWidth-rotationShift); 

            for (int i = 0; i < points.Length; i++)
            {
                int xPos = points[i].X - moveAmount;

                //If point is negative and crosses North add 720
                
                shiftedPoints[i] = new Point(xPos, points[i].Y);
            }

            return shiftedPoints;
        }

        private List<Point> ShiftPointsForRotation(List<Point> points)
        {
            List<Point> shiftedPoints = new List<Point>();

            int xMin = points.Min(p => p.X);
            int xMax = points.Max(p => p.X);

            int moveAmount = rotationShift;

            if (xMin - rotationShift < 0 && xMax - rotationShift < 0)
                moveAmount = 0 - (boreholeWidth - rotationShift); 

            for (int i = 0; i < points.Count; i++)
            {
                int xPos = points[i].X - moveAmount;

                //if (xPos >= boreholeWidth)
                //    xPos = xPos - boreholeWidth;
                //else if (xPos < 0)
                //    xPos += boreholeWidth;

                shiftedPoints.Add(new Point(xPos, points[i].Y));
            }

            return shiftedPoints;
        }

        /*
        private void drawClusterLines(PaintEventArgs e, List<Point> pointsToDraw, int currentCluster, int numberOfClusters)
        {
            if (pointsToDraw.Count > 1)
            {
                //e.Graphics.DrawLines(clusterPen, pointsToDraw.ToArray());
                for (int i = 0; i < pointsToDraw.Count - 1; i++)
                {
                    e.Graphics.DrawLine(clusterPen, pointsToDraw[i].X, pointsToDraw[i].Y - sectionStart, pointsToDraw[i + 1].X, pointsToDraw[i + 1].Y - sectionStart);
                }
            }

            if (currentCluster == numberOfClusters - 1 && creatingCluster)
            {
                //Don't join last line as cluster is in the middle of being created
            }
            else
            {
                //Join last line
                e.Graphics.DrawLine(clusterPen, pointsToDraw[pointsToDraw.Count - 1].X, pointsToDraw[pointsToDraw.Count - 1].Y - sectionStart, pointsToDraw[0].X, pointsToDraw[0].Y - sectionStart);
            }
        }
         * */

        private void DrawAllInclusions(PaintEventArgs e)
        {
            int numberOfInclusions = _viewAdapter.getNumOfInclusions();
            List<Point> pointsToDraw;

            for (int inclusionNum = 0; inclusionNum < numberOfInclusions; inclusionNum++)
            {
                inclusionPen.Color = _viewAdapter.GetInclusionColour(inclusionNum);
                
                //pointsToDraw = _viewAdapter.getInclusion(inclusionNum);
                pointsToDraw = GetPointToDraw(_viewAdapter.getInclusion(inclusionNum));
                //drawPointRectangles(e, pointsToDraw, inclusionPen);

                drawInclusionLines(e, pointsToDraw, inclusionNum, numberOfInclusions);
            }
        }

        private void drawInclusionLines(PaintEventArgs e, List<Point> pointsToDraw, int inclusionNumber, int numberOfInclusions)
        {
            if (inclusionNumber == numberOfInclusions - 1 && creatingInclusion)
            {
                if (pointsToDraw.Count > 1)
                {
                    pointsToDraw = ShiftPointsForRotation(pointsToDraw);

                    for (int i = 0; i < pointsToDraw.Count - 1; i++)
                    {
                        e.Graphics.DrawLine(OpacityPen(inclusionPen), pointsToDraw[i].X, pointsToDraw[i].Y, pointsToDraw[i + 1].X, pointsToDraw[i + 1].Y);
                    }
                }
            }
            else
            {
                Color color = inclusionPen.Color;
                Color transparentInclusionColour = Color.FromArgb(70, color.R, color.G, color.B);
                
                Point[] pointsArray = ShiftPointsForRotation(pointsToDraw.ToArray());
                
                e.Graphics.DrawPolygon(OpacityPen(inclusionPen), pointsArray);
                e.Graphics.FillPolygon(OpacityBrush(new SolidBrush(transparentInclusionColour)), pointsArray);

                //if any points occur outside range  draw secondary shape
                if (pointsArray.Min(p => p.X) < 0)
                {
                    Point[] movedPoints = AddAmountToXPoints(pointsArray, boreholeWidth);
                    //Draw shape at end as well
                    e.Graphics.DrawPolygon(OpacityPen(inclusionPen), movedPoints);
                    e.Graphics.FillPolygon(OpacityBrush(new SolidBrush(transparentInclusionColour)), movedPoints);
                }
                else if (pointsArray.Max(p => p.X) > boreholeWidth)
                {
                    Point[] movedPoints = AddAmountToXPoints(pointsArray, -boreholeWidth);

                    //Draw shape at start as well
                    e.Graphics.DrawPolygon(OpacityPen(inclusionPen), movedPoints);
                    e.Graphics.FillPolygon(OpacityBrush(new SolidBrush(transparentInclusionColour)), movedPoints);
                }
            }
        }

        /*
        private void drawInclusionLines(PaintEventArgs e, List<Point> pointsToDraw, int inclusionNumber, int numberOfInclusions)
        {
            if (pointsToDraw.Count > 1)
            {
                for (int i = 0; i < pointsToDraw.Count - 1; i++)
                {
                    e.Graphics.DrawLine(inclusionPen, pointsToDraw[i].X, pointsToDraw[i].Y - sectionStart, pointsToDraw[i + 1].X, pointsToDraw[i + 1].Y - sectionStart);
                }
            }

            if (inclusionNumber == numberOfInclusions - 1 && creatingInclusion)
            {
                //Don't join last line as cluster is in the middle of being created
            }
            else
            {
                //Join last line
                e.Graphics.DrawLine(inclusionPen, pointsToDraw[pointsToDraw.Count - 1].X, pointsToDraw[pointsToDraw.Count - 1].Y - sectionStart, pointsToDraw[0].X, pointsToDraw[0].Y - sectionStart);
            }
        }*/

        private void drawPointRectangles(PaintEventArgs e, List<Point> pointsToDraw, Pen pen)
        {
            for (int point = 0; point < pointsToDraw.Count; point++)
            {
                e.Graphics.DrawRectangle(OpacityPen(pen), pointsToDraw[point].X - 4, pointsToDraw[point].Y - 4 - sectionStart, 8, 8);
            }
        }

        private void DrawSelectedFeature(PaintEventArgs e)
        {
            if (!drawingFeaturesImage)
            {
                if (_viewAdapter.CurrentFeatureType.Equals("Layer"))
                {
                    drawSelectedLayer(e);
                }
                else if (_viewAdapter.CurrentFeatureType.Equals("Cluster"))
                {
                    drawSelectedCluster(e);
                }
                else if (_viewAdapter.CurrentFeatureType.Equals("Inclusion"))
                {
                    DrawSelectedInclusion(e);
                }
            }
        }

        private void drawSelectedLayer(PaintEventArgs e)
        {
            object currentFeature = _viewAdapter.SelectedFeature;

            layerPen.Color = _viewAdapter.GetLayerColour((Layer)currentFeature);

            List<Point> pointsToDraw;
            pointsToDraw = ((Layer)currentFeature).GetTopEdgePoints();

            DrawSine(e, pointsToDraw, selectedPen);
            DrawSine(e, pointsToDraw, layerPen);

            pointsToDraw = ((Layer)currentFeature).GetBottomEdgePoints();

            DrawSine(e, pointsToDraw, selectedPen);
            DrawSine(e, pointsToDraw, layerPen);
        }

        private void drawSelectedCluster(PaintEventArgs e)
        {
            if (!creatingCluster)
            {
                object currentFeature = _viewAdapter.SelectedFeature;

                clusterPen.Color = _viewAdapter.GetClusterColour((Cluster)currentFeature);

                List<Point> pointsToDraw;

                pointsToDraw = ShiftPointsForRotation(((Cluster)currentFeature).Points);
                //pointsToDraw = ((Cluster)currentFeature).Points;

                DrawSelectedClusterShape(e, pointsToDraw, selectedPen);

                //if any points occur outside range  draw secondary shape
                if (pointsToDraw.Min(p => p.X) < 0)
                {
                    List<Point> movedPoints = AddAmountToXPoints(pointsToDraw, boreholeWidth);
                    //Draw shape at end as well
                    DrawSelectedClusterShape(e, movedPoints, selectedPen);

                }
                else if (pointsToDraw.Max(p => p.X) > boreholeWidth)
                {
                    List<Point> movedPoints = AddAmountToXPoints(pointsToDraw, -boreholeWidth);

                    //Draw shape at start as well
                    DrawSelectedClusterShape(e, movedPoints, selectedPen);
                }

            }
        }

        private void DrawSelectedClusterShape(PaintEventArgs e, List<Point> pointsToDraw, Pen selectedPen)
        {
            //Draw the outline
            for (int point = 0; point < pointsToDraw.Count; point++)
            {
                e.Graphics.DrawRectangle(selectedPen, pointsToDraw[point].X - 4, pointsToDraw[point].Y - 4 - sectionStart, 8, 8);
            }

            if (pointsToDraw.Count > 1)
            {
                //e.Graphics.DrawLines(selectedPen, pointsToDraw.ToArray());
                for (int i = 0; i < pointsToDraw.Count - 1; i++)
                {
                    e.Graphics.DrawLine(OpacityPen(selectedPen), pointsToDraw[i].X, pointsToDraw[i].Y - sectionStart, pointsToDraw[i + 1].X, pointsToDraw[i + 1].Y - sectionStart);
                }
            }

            //Join last line
            e.Graphics.DrawLine(selectedPen, pointsToDraw[pointsToDraw.Count - 1].X, pointsToDraw[pointsToDraw.Count - 1].Y - sectionStart, pointsToDraw[0].X, pointsToDraw[0].Y - sectionStart);

            //Draw the cluster
            for (int point = 0; point < pointsToDraw.Count; point++)
            {
                e.Graphics.DrawRectangle(OpacityPen(clusterPen), pointsToDraw[point].X - 4, pointsToDraw[point].Y - 4 - sectionStart, 8, 8);
            }

            if (pointsToDraw.Count > 1)
            {
                //e.Graphics.DrawLines(clusterPen, pointsToDraw.ToArray());
                for (int i = 0; i < pointsToDraw.Count - 1; i++)
                {
                    e.Graphics.DrawLine(OpacityPen(clusterPen), pointsToDraw[i].X, pointsToDraw[i].Y - sectionStart, pointsToDraw[i + 1].X, pointsToDraw[i + 1].Y - sectionStart);
                }
            }

            //Join last line
            e.Graphics.DrawLine(OpacityPen(clusterPen), pointsToDraw[pointsToDraw.Count - 1].X, pointsToDraw[pointsToDraw.Count - 1].Y - sectionStart, pointsToDraw[0].X, pointsToDraw[0].Y - sectionStart);
            
        }

        private void DrawSelectedInclusion(PaintEventArgs e)
        {
            if (!creatingInclusion)
            {
                object currentFeature = _viewAdapter.SelectedFeature;

                inclusionPen.Color = _viewAdapter.GetInclusionColour((Inclusion)currentFeature);

                List<Point> pointsToDraw;

                pointsToDraw = ShiftPointsForRotation(((Inclusion)currentFeature).Points);

                DrawSelectedInclusionShape(e, pointsToDraw, selectedPen);

                //if any points occur outside range  draw secondary shape
                if (pointsToDraw.Min(p => p.X) < 0)
                {
                    List<Point> movedPoints = AddAmountToXPoints(pointsToDraw, boreholeWidth);
                    //Draw shape at end as well
                    DrawSelectedInclusionShape(e, movedPoints, selectedPen);

                }
                else if (pointsToDraw.Max(p => p.X) > boreholeWidth)
                {
                    List<Point> movedPoints = AddAmountToXPoints(pointsToDraw, -boreholeWidth);

                    //Draw shape at start as well
                    DrawSelectedInclusionShape(e, movedPoints, selectedPen);
                }
            }
        }

        private void DrawSelectedInclusionShape(PaintEventArgs e, List<Point> pointsToDraw, Pen selectedPen)
        {
            //Draw the outline
            for (int point = 0; point < pointsToDraw.Count; point++)
            {
                e.Graphics.DrawRectangle(OpacityPen(selectedPen), pointsToDraw[point].X - 4, pointsToDraw[point].Y - 4 - sectionStart, 8, 8);
            }

            if (pointsToDraw.Count > 1)
            {
                //e.Graphics.DrawLines(selectedPen, pointsToDraw.ToArray());
                for (int i = 0; i < pointsToDraw.Count - 1; i++)
                {
                    e.Graphics.DrawLine(OpacityPen(selectedPen), pointsToDraw[i].X, pointsToDraw[i].Y - sectionStart, pointsToDraw[i + 1].X, pointsToDraw[i + 1].Y - sectionStart);
                }
            }

            //Join last line
            e.Graphics.DrawLine(OpacityPen(selectedPen), pointsToDraw[pointsToDraw.Count - 1].X, pointsToDraw[pointsToDraw.Count - 1].Y - sectionStart, pointsToDraw[0].X, pointsToDraw[0].Y - sectionStart);

            //Draw the inclusion
            for (int point = 0; point < pointsToDraw.Count; point++)
            {
                e.Graphics.DrawRectangle(OpacityPen(inclusionPen), pointsToDraw[point].X - 4, pointsToDraw[point].Y - 4 - sectionStart, 8, 8);
            }

            if (pointsToDraw.Count > 1)
            {
                for (int i = 0; i < pointsToDraw.Count - 1; i++)
                {
                    e.Graphics.DrawLine(OpacityPen(inclusionPen), pointsToDraw[i].X, pointsToDraw[i].Y - sectionStart, pointsToDraw[i + 1].X, pointsToDraw[i + 1].Y - sectionStart);
                }
            }

            //Join last line
            e.Graphics.DrawLine(OpacityPen(inclusionPen), pointsToDraw[pointsToDraw.Count - 1].X, pointsToDraw[pointsToDraw.Count - 1].Y - sectionStart, pointsToDraw[0].X, pointsToDraw[0].Y - sectionStart);

        }

        /// <summary>
        /// Repaints the preview image and its scroll box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void paintPreview(object sender, PaintEventArgs e)
        {
            if (autoDetectRunning)
            {
                int start, end, height;

                int test = _viewAdapter.GetCurrentSectionStart();

                start = (int)((double)(currentAutoDetectProgress - _viewAdapter.GetCurrentSectionStart()) / ((double)boreholeSectionHeight / (double)previewHeight));
                end = (int)((double)(totalAutoDetectProgress - _viewAdapter.GetCurrentSectionStart()) / ((double)boreholeSectionHeight / (double)previewHeight));

                //end = boreholePreviewPictureBox.Height;
                height = end - start;

                //_viewAdapter.getCurrentSectionStart

                //e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(128, 0, 0, 0)), this.ClientRectangle);
                e.Graphics.FillRectangle(OpacityBrush(new SolidBrush(Color.FromArgb(128, 0, 0, 0))), new Rectangle(0, start, boreholeWidth, height));
            }

            Pen p = new Pen(Color.Red, 2);
            e.Graphics.DrawRectangle(p, 0, scrollPosition, boreholePreviewPictureBox.Size.Width - 2, scrollSize);
        }

        private void paintFullPreview(object sender, PaintEventArgs e)
        {
            double fullHeight = (double)_viewAdapter.BoreholeDepth;

            fullPreviewScrollSize = (int)((double)fullPreviewPictureBox.Height * ((double)boreholeSectionHeight / fullHeight));
            
            double sectionPosition = _viewAdapter.GetCurrentSectionStart();
            
            fullPreviewScrollPosition = (int)((double)fullPreviewPictureBox.Height * (sectionPosition / fullHeight));               

            Pen p = new Pen(Color.Red, 2);
            e.Graphics.DrawRectangle(p, 0, fullPreviewScrollPosition, fullPreviewPictureBox.Size.Width - 2, fullPreviewScrollSize);
        } 

        /// <summary>
        /// Checks if any of the current picture box is within the auto detect are 
        /// and if so draws it grayed out
        /// </summary>
        private void checkWhetherToDrawAutoDetectArea(PaintEventArgs e)
        {
            if (autoDetectRunning)
            {
                int start, end, height;

                start = currentAutoDetectProgress - _viewAdapter.GetCurrentSectionStart();
                end = totalAutoDetectProgress - _viewAdapter.GetCurrentSectionStart();
                //end = boreholePictureBox.Height;
                height = end - start;

                //_viewAdapter.getCurrentSectionStart

                //e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(128, 0, 0, 0)), this.ClientRectangle);
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(128, 0, 0, 0)), new Rectangle(0, start, boreholeWidth, height));
            }
        }

        /// <summary>
        /// Draws regions which are currently undergoing semi-automatic convergance
        /// </summary>
        /// <param name="e"></param>
        private void DrawConvergingRegions(PaintEventArgs e)
        {
            foreach (Rectangle region in autoConvergingRegions)
            {
                Rectangle shiftedRegion = ConvertForRotation(region);

                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(128, 0, 0, 0)), shiftedRegion);

                if (shiftedRegion.X < 0)
                {
                    shiftedRegion.Offset(boreholeWidth, 0);
                    e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(128, 0, 0, 0)), shiftedRegion);
                }
                else if (shiftedRegion.X + region.Width >= boreholeWidth)
                {
                    shiftedRegion.Offset(-boreholeWidth, 0);
                    e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(128, 0, 0, 0)), shiftedRegion);
                }
            }
        }

        /// <summary>
        /// Converts the points in a region to account for rotation
        /// </summary>
        /// <param name="region"></param>
        /// <returns></returns>
        private Rectangle ConvertForRotation(Rectangle region)
        {
            Rectangle shifted;


            shifted = new Rectangle(region.X - rotationShift, region.Y - _viewAdapter.GetCurrentSectionStart(), region.Width, region.Height);
            //int xPos = e.X + rotationShift;

            //if (xPos < 0)
            //{
            //    xPos += boreholeWidth;
            //}
            //else if (xPos >= boreholeWidth)
           // {
           //     xPos -= boreholeWidth;
           // }

            return shifted;
        }

        # endregion

        # region Feature button clicks

        #region Layer Button Clicks

        /// <summary>
        /// Called when the add layer button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addLayerButtonClicked(object sender, EventArgs e)
        {
            firstLayerPointSelected = false;

            cancelCreatingOtherFeatures();

            DisableFeaturesButtons();
            _viewAdapter.DeSelectFeature();
            selectedButton = "addLayerToolStripButton";
            toolStripStatusLabel.Text = "Select top of layer edge";
            addLayerToolStripButton.Checked = true;

            showFeaturesCheckBox.Checked = true;
            
            boreholePictureBox.Refresh();
        }

        private void cancelCreatingOtherFeatures()
        {
            if (_viewAdapter.CurrentFeatureType.Equals("Cluster") && creatingCluster)
            {
                _viewAdapter.DeleteCurrentFeature();
                creatingCluster = false;
            }
            else if (_viewAdapter.CurrentFeatureType.Equals("Inclusion") && creatingInclusion)
            {
                _viewAdapter.DeleteCurrentFeature();
                creatingInclusion = false;
            }
        }

        /// <summary>
        /// Called when the layer thickness button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void layerThicknessButtonClicked(object sender, EventArgs e)
        {
            selectedButton = "layerThicknessToolStripButton";
            toolStripStatusLabel.Text = "Click on layer and drag to change its thickness";
            DisableFeaturesButtons();
            enableLayerButtons();
            layerThicknessToolStripButton.Checked = true;
        }

        /// <summary>
        /// Called when the alter layer button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void alterLayerButtonClicked(object sender, EventArgs e)
        {
            selectedButton = "alterLayerToolStripButton";
            toolStripStatusLabel.Text = "Click on the layer to change its position or amplitude";
            DisableFeaturesButtons();
            enableLayerButtons();
            alterLayerToolStripButton.Checked = true;
        }

        /// <summary>
        /// Called when the alter one edge button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void alterEdgeButton(object sender, EventArgs e)
        {
            selectedButton = "alterEdgeToolStripButton";
            toolStripStatusLabel.Text = "Click on an edge to change its position or amplitude";
            DisableFeaturesButtons();
            enableLayerButtons();
            alterEdgeToolStripButton.Checked = true;
        }

        /// <summary>
        /// Called when the delete layer button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteButtonClicked(object sender, EventArgs e)
        {
            _viewAdapter.DeleteCurrentFeature();
            DisableFeaturesButtons();
            selectedButton = "";
            toolStripStatusLabel.Text = "Select feature to add";

            AutoSave();
            changedSinceLastSave = true;

            //boreholeImagePanel.Refresh();
            boreholePictureBox.Refresh();
            //boreholeFeaturesPanel.Refresh();
        }

        # endregion

        # region Cluster Button Clicks

        /// <summary>
        /// Called when the add cluster button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addClusterButtonClicked(object sender, EventArgs e)
        {
            if (_viewAdapter.CurrentFeatureType.Equals("Cluster") && creatingCluster)
            {
                _viewAdapter.DeleteCurrentFeature();
            }
            else if (_viewAdapter.CurrentFeatureType.Equals("Inclusion") && creatingInclusion)
            {
                _viewAdapter.DeleteCurrentFeature();
            }

            DisableFeaturesButtons();       //Disables all except 'add' feature buttons
            _viewAdapter.DeSelectFeature();

            selectedButton = "addClusterToolStripButton";
            toolStripStatusLabel.Text = "Select cluster points.  Click on first point to complete cluster";
            addClusterToolStripButton.Checked = true;
            creatingCluster = false;
            creatingInclusion = false;

            showFeaturesCheckBox.Checked = true;

            boreholePictureBox.Refresh();
        }

        /// <summary>
        /// Called when the edit cluster button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void editClusterButtonClicked(object sender, EventArgs e)
        {
            DisableFeaturesButtons();
            EnableClusterButtons();
            editClusterToolStripButton.Checked = true;
            selectedButton = "editClusterToolStripButton";
            toolStripStatusLabel.Text = "Move cluster or points";
            
        }

        private void addClusterPointToolStripButton_Click(object sender, EventArgs e)
        {
            DisableFeaturesButtons();
            EnableClusterButtons();
            addClusterPointToolStripButton.Checked = true;
            selectedButton = "addClusterPointToolStripButton";
            toolStripStatusLabel.Text = "Select the position to add the point to";
        }   

        /// <summary>
        /// Called when the remove cluster point button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void removeClusterPointButtonClicked(object sender, EventArgs e)
        {
            DisableFeaturesButtons();
            EnableClusterButtons();
            removeClusterPointToolStripButton.Checked = true;
            selectedButton = "removeClusterPointToolStripButton";
            toolStripStatusLabel.Text = "Select point to remove";
        }

        /// <summary>
        /// Called when the delete cluster button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteClusterButtonClicked(object sender, EventArgs e)
        {
            _viewAdapter.DeleteCurrentFeature();
            DisableFeaturesButtons();
            selectedButton = "";
            toolStripStatusLabel.Text = "Select feature to add";

            boreholePictureBox.Refresh();

            AutoSave();
            changedSinceLastSave = true;
        }

        # endregion

        # region Inclusion Button Clicks

        /// <summary>
        /// Called when the add inclusion button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addInclusionButtonClicked(object sender, EventArgs e)
        {
            if (_viewAdapter.CurrentFeatureType.Equals("Cluster") && creatingCluster)
            {
                _viewAdapter.DeleteCurrentFeature();
            }
            else if (_viewAdapter.CurrentFeatureType.Equals("Inclusion") && creatingInclusion)
            {
                _viewAdapter.DeleteCurrentFeature();
            }

            DisableFeaturesButtons();       //Disables all except 'add' feature buttons
            _viewAdapter.DeSelectFeature();

            selectedButton = "addInclusionToolStripButton";
            toolStripStatusLabel.Text = "Select inclusion points.  Click on first point to complete inclusion";
            addInclusionToolStripButton.Checked = true;
            creatingInclusion = false;
            creatingCluster = false;

            showFeaturesCheckBox.Checked = true;

            boreholePictureBox.Refresh();
        }

        /// <summary>
        /// Called when the edit inclusion button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void editInclusionButtonClicked(object sender, EventArgs e)
        {
            DisableFeaturesButtons();
            EnableInclusionButtons();
            editInclusionToolStripButton.Checked = true;
            selectedButton = "editInclusionToolStripButton";
            toolStripStatusLabel.Text = "Move inclusion or points";
            
        }

        private void addInclusionPointToolStripButton_Click(object sender, EventArgs e)
        {
            DisableFeaturesButtons();
            EnableInclusionButtons();
            addInclusionPointToolStripButton.Checked = true;
            selectedButton = "addInclusionPointToolStripButton";
            toolStripStatusLabel.Text = "Select position to add point to";
        }   

        /// <summary>
        /// Called when the remove inclusion point button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void removeInclusionPointButtonClicked(object sender, EventArgs e)
        {
            DisableFeaturesButtons();
            EnableInclusionButtons();
            removeInclusionPointToolStripButton.Checked = true;
            selectedButton = "removeInclusionPointToolStripButton";
            toolStripStatusLabel.Text = "Select point to remove";
        }

        /// <summary>
        /// Called when the delete inclusion button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteInclusionButtonClicked(object sender, EventArgs e)
        {
            _viewAdapter.DeleteCurrentFeature();
            DisableFeaturesButtons();
            selectedButton = "";
            toolStripStatusLabel.Text = "Select feature to add";

            boreholePictureBox.Refresh();

            AutoSave();
            changedSinceLastSave = true;
        }

        # endregion

        # region Fluid Level Button Clicks

        private void fluidLevelButtonClicked(object sender, EventArgs e)
        {
            if (_viewAdapter.CurrentFeatureType.Equals("Cluster") && creatingCluster)
            {
                //Console.WriteLine("Disabling feature");
                _viewAdapter.DeleteCurrentFeature();
                creatingCluster = false;
            }
            else if (_viewAdapter.CurrentFeatureType.Equals("Inclusion") && creatingInclusion)
            {
                //Console.WriteLine("Disabling feature");
                _viewAdapter.DeleteCurrentFeature();
                creatingInclusion = false;
            }

            DisableFeaturesButtons();       //Disables all except 'add' feature buttons
            _viewAdapter.DeSelectFeature();
            selectedButton = "fluidLevelToolStripButton";
            toolStripStatusLabel.Text = "Select fluid level position";
            fluidLevelToolStripButton.Checked = true;

            showFeaturesCheckBox.Checked = true;

            boreholePictureBox.Refresh();
        }

        # endregion

        # endregion

        # region Borehole preview scroller methods

        /// <summary>
        /// Updates the position of the scroller rectangle's position and refreshes fullBoreholePictureBox
        /// to repaint the scroller
        /// </summary>
        /// <param name="position">The position of the currently visible area of the borehole image</param>
        public void changeBoreholeScroller(double position)
        {
            previewHeight = boreholePreviewPictureBox.Height;

            scrollPosition = (float)(((double)previewHeight / (double)boreholeSectionHeight) * position);

            boreholePreviewPictureBox.Refresh();
        }

        private void boreholeScrolled(object sender, ScrollEventArgs e)
        {
            changeBoreholeScroller(e.NewValue);
        }

        /// <summary>
        /// Called when the preview is clicked.
        /// Updates the scroll value of the boreholeImagePanel so that it matches the new preview position
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">The MouseEvent of the click</param>
        private void boreholePreviewClicked(object sender, MouseEventArgs e)
        {
            updateScrollerPosition(e);
        }

        private void boreholePreviewPictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            previewMouseDown = true;
        }

        private void boreholePreviewPictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            previewMouseDown = false;
        }

        private void boreholePreviewPictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (previewMouseDown)
            {
                updateScrollerPosition(e);
            }
        }

        private void updateScrollerPosition(MouseEventArgs e)
        {
            int previewPosition = e.Y;

            int boreholePosition = (int)((((double)previewPosition - ((double)scrollSize / 2.0)) / (double)boreholePreviewPictureBox.Height) * (double)boreholeSectionHeight);

            
            if (boreholePosition > boreholeSectionHeight - boreholeImagePanel.Height)
            {
                boreholePosition = boreholeSectionHeight - boreholeImagePanel.Height;
            }

            if (boreholePosition < 0)
            {
                boreholePosition = 0;
            }

            boreholeImagePanel.VerticalScroll.Value = boreholePosition;
            boreholeImagePanel.Update();

            changeBoreholeScroller(boreholePosition);
            boreholeImagePanel.VerticalScroll.Value = boreholePosition;

            RefreshBoreholeImage();
        }

        # endregion

        # region Mouse events

        # region borehole image clicked methods

        /// <summary>
        /// Called when the borehole image is clicked.
        /// Checks if a feature is being selected and if not checks if a new feature is being added.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">The MouseEvent associated with the click</param>
        private void boreholeImageClicked(object sender, MouseEventArgs e)
        {
            if (isClickWithinGrayedArea(e.Y) || IsClickWithinConvergingRegion(e.X, e.Y))
            {
                displayAutoDetectWarning();
            }
            else if (showFeaturesCheckBox.Checked == true)
            {
                sectionStart = _viewAdapter.GetCurrentSectionStart();

                int xPos = e.X + rotationShift;

                if(xPos < 0)
                {
                    xPos += boreholeWidth;
                }
                else if (xPos >= boreholeWidth)
                {
                    xPos -= boreholeWidth;
                }

                int yPos = e.Y + sectionStart;

                previouslySelectedFeature = _viewAdapter.SelectedFeature;
                _viewAdapter.PointClicked(xPos, yPos);
                currentlySelectedFeature = _viewAdapter.SelectedFeature;

                if (e.Button == System.Windows.Forms.MouseButtons.Right)
                {
                    DisableFeaturesButtons();

                    checkWhatTypeOfFeatureIsBeingSelected();

                    if (_viewAdapter.LayerAtLastClick)      
                    {  
                        ShowFeatureDetailsPropertyGrid();

                        RefreshBoreholeImage(); //Called so that if new layer is selected it will show

                        rightClickOnLayerContextMenuStrip.Show(boreholePictureBox, e.Location);
                    }
                    else if (_viewAdapter.ClusterAtLastClick)
                    {
                        ShowFeatureDetailsPropertyGrid();

                        RefreshBoreholeImage(); //Called so that if new layer is selected it will show

                        rightClickOnClusterContextMenuStrip.Show(boreholePictureBox, e.Location);
                    }
                    else if (_viewAdapter.InclusionAtLastClick)
                    {
                        ShowFeatureDetailsPropertyGrid();

                        RefreshBoreholeImage(); //Called so that if new layer is selected it will show

                        rightClickOnInclusionContextMenuStrip.Show(boreholePictureBox, e.Location);
                    }
                }
                else
                {
                    checkIfFeatureIsBeingSelected();

                    checkIfFeatureIsBeingAdded(xPos, yPos);
                }
            }
        }

        /// <summary>
        /// Checks if a mouse click is within the current grayed out area
        /// </summary>
        /// <returns>True if mouse click is within area</returns>
        private bool isClickWithinGrayedArea(int yPos)
        {
            if (autoDetectRunning)
            {
                int start = currentAutoDetectProgress - _viewAdapter.GetCurrentSectionStart();
                int end = totalAutoDetectProgress - _viewAdapter.GetCurrentSectionStart();

                if (yPos >= start && yPos <= end)
                    return true;
                else
                    return false;
            }

            return false;
        }

        private bool IsClickWithinConvergingRegion(int xPoint, int yPoint)
        {
            foreach (Rectangle region in autoConvergingRegions)
            {                
                int xRight = region.Right-rotationShift;
                int xLeft = region.Left-rotationShift;
                                
                if (yPoint <= (region.Bottom+sectionStart) && yPoint >= (region.Top+sectionStart))
                {
                    if(xPoint <= xRight && xPoint >= xLeft)
                        return true;

                    if (xRight > boreholeWidth)
                    {
                        int correctedRight = xRight - boreholeWidth;
                        int correctedLeft = xLeft - boreholeSectionHeight;

                        if (xPoint <= correctedRight && xPoint >= correctedLeft)
                            return true;
                    }

                    if (xLeft < 0)
                    {
                        int correctedRight = xRight + boreholeWidth;
                        int correctedLeft = xLeft + boreholeSectionHeight;

                        if (xPoint <= correctedRight && xPoint >= correctedLeft)
                            return true;
                    }
                }

            }

            return false;
        }

        private void displayAutoDetectWarning()
        {
            MessageBox.Show("You can not edit an area which is undergoing automatic analysis.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /// <summary>
        /// Checks if a previously added feature is being selected
        /// </summary>
        /// <param name="xPos">The x position of the click</param>
        /// <param name="yPos">The y position of the click</param>
        private void checkIfFeatureIsBeingSelected()
        {
            if (!currentlySelectedFeature.Equals(previouslySelectedFeature))
            {
                DisableFeaturesButtons();

                checkWhatTypeOfFeatureIsBeingSelected();

                ShowFeatureDetailsPropertyGrid();
                boreholePictureBox.Refresh();
            }
            else
            {
                checkIfLayersAreToBeSplit();
            }
        }

        private void checkWhatTypeOfFeatureIsBeingSelected()
        {
            string currentFeatureType = _viewAdapter.CurrentFeatureType;

            if (currentFeatureType.Equals("Layer"))
            {
                checkIfLayersAreToBeJoined();
                checkIfLayersAreToBeSplit();

                selectedLayerActions();
            }
            else if (currentFeatureType.Equals("Cluster"))
            {
                selectedClusterActions();
            }
            else if (currentFeatureType.Equals("Inclusion"))
            {
                selectInclusionActions();
            }
        }

        private void checkIfLayersAreToBeJoined()
        {
            string previousFeature = previouslySelectedFeature.GetType().ToString();

            if ((previouslySelectedFeature.GetType().ToString().Equals("BoreholeFeatures.BoreholeLayer") || previouslySelectedFeature.GetType().ToString().Equals("BoreholeFeatures.CoreLayer")) && Control.ModifierKeys == Keys.Control)
            {
                Layer previousLayer = (Layer)previouslySelectedFeature;
                Layer currentLayer = (Layer)currentlySelectedFeature;

                if (previousLayer.GetNumOfEdges() == 1 && currentLayer.GetNumOfEdges() == 1)
                {
                    _viewAdapter.JoinEdges(previousLayer, currentLayer);
                }
            }
        }

        private void checkIfLayersAreToBeSplit()
        {
            if ((previouslySelectedFeature.GetType().ToString().Equals("BoreholeFeatures.BoreholeLayer") || previouslySelectedFeature.GetType().ToString().Equals("BoreholeFeatures.CoreLayer")) && Control.ModifierKeys == Keys.Shift)
            {
                Layer currentLayer = (Layer)currentlySelectedFeature;

                if (currentLayer.GetNumOfEdges() == 2)
                {
                    _viewAdapter.SplitLayer(currentLayer);
                }
            }
        }


        /// <summary>
        /// Called when a layer is selected
        /// </summary>
        private void selectedLayerActions()
        {
            creatingCluster = false;
            creatingInclusion = false;
            enableLayerButtons();
            alterLayerToolStripButton.Checked = true;
            selectedButton = "alterLayerToolStripButton";
        }

        /// <summary>
        /// Called when a cluster is selected
        /// </summary>
        private void selectedClusterActions()
        {
            if (creatingCluster == true)
                _viewAdapter.DeleteCluster((Cluster)previouslySelectedFeature);

            creatingCluster = false;
            creatingInclusion = false;

            EnableClusterButtons();
            editClusterToolStripButton.Checked = true;
            selectedButton = "editClusterToolStripButton";
        }

        /// <summary>
        /// Called when an inclusion is selected
        /// </summary>
        private void selectInclusionActions()
        {
            if (creatingInclusion == true)
                _viewAdapter.DeleteInclusion((Inclusion)previouslySelectedFeature);

            creatingCluster = false;
            creatingInclusion = false;

            EnableInclusionButtons();
            editInclusionToolStripButton.Checked = true;
            selectedButton = "editInclusionToolStripButton";
        }

        private void checkIfFeatureIsBeingAdded(int xPos, int yPos)
        {
            if (selectedButton.Equals("addLayerToolStripButton"))
            {
                checkWhichPartOfLayerToAdd(xPos, yPos);
                //changedSinceLastSave = true;
            }
            else if (selectedButton.Equals("addClusterToolStripButton"))
            {
                checkWhichPartOfClusterToAdd(xPos, yPos);
                AutoSave();
            }
            else if (selectedButton.Equals("addInclusionToolStripButton"))
            {
                checkWhichPartOfInclusionToAdd(xPos, yPos);
                AutoSave();
            }
            else if (selectedButton.Equals("fluidLevelToolStripButton"))
            {
                _viewAdapter.FluidLevel = yPos;
                boreholePictureBox.Refresh();
                AutoSave();
            }
        }

        /// <summary>
        /// Checks whether layers first or second point is to be added
        /// </summary>
        /// <param name="xPos">The x position of the click</param>
        /// <param name="yPos">The y position of the click</param>
        private void checkWhichPartOfLayerToAdd(int xPos, int yPos)
        {
            if (firstLayerPointSelected == false)
            {
                addFirstLayerPointValues(xPos, yPos);
            }
            else if (firstLayerPointSelected)
            {
                secondLayerPoint = new Point(xPos, yPos);

                calculateLayerAttributes();
                enableLayerButtons();

                //UnCheckButton and set alter Layer as active button
                addLayerToolStripButton.Checked = false;
                alterLayerToolStripButton.Checked = true;
                selectedButton = "alterLayerToolStripButton";
                toolStripStatusLabel.Text = "Click on the layer to change its position or amplitude";

                ShowFeatureDetailsPropertyGrid();
                boreholePictureBox.Refresh();

                firstLayerPointSelected = false;

                AutoSave();
            }
        }

        private void addFirstLayerPointValues(int xPos, int yPos)
        {
            firstLayerPoint = new Point(xPos, yPos);
            firstLayerPointSelected = true;

            toolStripStatusLabel.Text = "Select the bottom of the edge";
        }

        /// <summary>
        /// Calculates the sinusoid attributes based on which click was highest
        /// </summary>
        private void calculateLayerAttributes()
        {
            if (_viewAdapter.ImageType == "Borehole")
            {
                int layerAzimuth;
                int layerAmplitude;
                int layerDepth;

                if (firstLayerPoint.Y <= secondLayerPoint.Y)
                {
                    layerAzimuth = secondLayerPoint.X;
                    layerAmplitude = (secondLayerPoint.Y - firstLayerPoint.Y) / 2;
                    layerDepth = secondLayerPoint.Y - layerAmplitude;
                }
                else
                {
                    layerAzimuth = firstLayerPoint.X;
                    layerAmplitude = (firstLayerPoint.Y - secondLayerPoint.Y) / 2;
                    layerDepth = firstLayerPoint.Y - layerAmplitude;
                }

                _viewAdapter.createNewLayer(layerDepth, layerAmplitude, layerAzimuth);
            }
            else
            {
                double layerSlope;
                int layerIntercept;

                layerSlope = (double)(firstLayerPoint.Y - secondLayerPoint.Y) / (double)(firstLayerPoint.X - secondLayerPoint.X);

                //intercept = y - (slope*x)
                double interceptDouble = firstLayerPoint.Y - (layerSlope * firstLayerPoint.X);

                layerIntercept = (int)Math.Round(interceptDouble, MidpointRounding.AwayFromZero);

                _viewAdapter.CreateNewLayer(layerSlope, layerIntercept);
            }
        }

        private void checkWhichPartOfClusterToAdd(int xPos, int yPos)
        {
            if (creatingCluster == false)
            {
                addFirstClusterPoint(xPos, yPos);
            }
            else
            {
                checkWhetherToAddPointOrCompleteCluster(xPos, yPos);
            }

            ShowFeatureDetailsPropertyGrid();
        }

        private void addFirstClusterPoint(int xPos, int yPos)
        {
            _viewAdapter.createNewCluster();
            creatingCluster = true;

            if (xPos < boreholeWidth && xPos > rotationShift && rotationShift != 0)
            {
                xPos -= boreholeWidth;
            }
            _viewAdapter.AddToCurrentCluster(xPos, yPos);
            clusterStartPoint = new Point(xPos, yPos);

            numberofPointsInCluster = 1;

            boreholePictureBox.Refresh();
        }

        private void checkWhetherToAddPointOrCompleteCluster(int xPos, int yPos)
        {
            if (xPos < boreholeWidth && xPos > rotationShift && rotationShift != 0)
            {
                xPos -= boreholeWidth;
            }

            if (!PointsAreClose(clusterStartPoint, new Point(xPos, yPos)))
            {  
                _viewAdapter.AddToCurrentCluster(xPos, yPos);
                numberofPointsInCluster++;
                boreholePictureBox.Refresh();
            }
            else
            {
                if (numberofPointsInCluster > 2)
                {
                    completeCluster();

                    boreholePictureBox.Refresh();
                }
            }
        }

        private void completeCluster()
        {
            creatingCluster = false;
            DisableFeaturesButtons();
            EnableClusterButtons();
            _viewAdapter.ClusterComplete();

            selectedButton = "editClusterToolStripButton";
            editClusterToolStripButton.Checked = true;
        }

        private void checkWhichPartOfInclusionToAdd(int xPos, int yPos)
        {
            if (creatingInclusion == false)
            {
                addFirstInclusionPoint(xPos, yPos);
            }
            else
            {
                checkWhetherToAddPointOrCompleteInclusion(xPos, yPos);
            }

            ShowFeatureDetailsPropertyGrid();
        }
        private void addFirstInclusionPoint(int xPos, int yPos)
        {
            if (xPos < boreholeWidth && xPos > rotationShift && rotationShift != 0)
            {
                xPos -= boreholeWidth;
            }

            _viewAdapter.CreateNewInclusion();
            creatingInclusion = true;
            _viewAdapter.AddToCurrentInclusion(xPos, yPos);
            inclusionStartPoint = new Point(xPos, yPos);

            numberofPointsInInclusion = 1;
            boreholePictureBox.Refresh();
        }

        private void checkWhetherToAddPointOrCompleteInclusion(int xPos, int yPos)
        {
            if (xPos < boreholeWidth && xPos > rotationShift && rotationShift != 0)
            {
                xPos -= boreholeWidth;
            }

            if (!PointsAreClose(inclusionStartPoint, new Point(xPos, yPos)))
            {
                _viewAdapter.AddToCurrentInclusion(xPos, yPos);
                numberofPointsInInclusion++;
                boreholePictureBox.Refresh();
            }
            else
            {
                if (numberofPointsInInclusion > 2)
                {
                    completeInclusion();

                    boreholePictureBox.Refresh();
                }
            }
        }

        private void completeInclusion()
        {
            creatingInclusion = false;
            DisableFeaturesButtons();
            EnableInclusionButtons();
                        
            _viewAdapter.InclusionComplete();

            RefreshBoreholeImage();

            if (automaticallyConvergeInclusionsToolStripMenuItem.Checked == true)
            {
                Inclusion selectedFeature = (Inclusion)_viewAdapter.SelectedFeature;

                Rectangle region;

                Bitmap subImage = GetInclusionBitmap(selectedFeature, out region);

                List<object> callbackItems = new List<object>();

                callbackItems.Add((object)selectedFeature);
                callbackItems.Add((object)subImage);
                callbackItems.Add((object)region);

                //Auto converge
                ThreadPool.QueueUserWorkItem(new WaitCallback(AutoConvergeInclusion), callbackItems);

                _viewAdapter.DeSelectFeature();
            }

            selectedButton = "editInclusionToolStripButton";
            editInclusionToolStripButton.Checked = true;
        }

        private Bitmap GetInclusionBitmap(Inclusion inclusion, out Rectangle region)
        {
            int xOffset, yOffset;

            int sectionStartX, sectionEndX, sectionWidth;
            int sectionStartY, sectionEndY;

            sectionStartX = inclusion.LeftXBoundary - 10;
            sectionEndX = inclusion.RightXBoundary + 10;

            xOffset = sectionStartX;

            sectionWidth = sectionEndX - sectionStartX;

            //Correct for rotation
            sectionStartX += ((int)((double)boreholeWidth / (double)360) - rotationShift);
            sectionEndX += ((int)((double)boreholeWidth / (double)360) - rotationShift);

            if (sectionStartX < 0)
            {
                sectionStartX += boreholeWidth;
            }
            else if (sectionStartX >= boreholeWidth)
            {
                sectionStartX -= boreholeWidth;
            }

            sectionStartY = inclusion.TopYBoundary - 10;
            sectionEndY = inclusion.BottomYBoundary + 10;

            if (sectionStartY < 0)
                sectionStartY = 0;

            yOffset = sectionStartY;

            //Correct for section start
            sectionStartY -= sectionStart;
            sectionEndY -= sectionStart;

            if (sectionEndY >= boreholeSectionHeight)
                sectionEndY = boreholeSectionHeight - 1;

            Bitmap section = new Bitmap(sectionWidth, sectionEndY - sectionStartY);

            Graphics g = Graphics.FromImage(section);

            region = new Rectangle(sectionStartX, sectionStartY, sectionWidth, sectionEndY - sectionStartY);

            g.DrawImage(boreholeSectionImage, 0, 0, region, GraphicsUnit.Pixel);

            //if image goes off left add left end of region to image
            if (sectionStartX < 0)
            {
                //    Rectangle shiftedRegion = new Rectangle(0, sectionStartY, (sectionStartX - boreholeWidth) + sectionWidth, sectionEndY - sectionStartY);

                //    g.DrawImage(boreholeImage, boreholeWidth - sectionStartX, 0, shiftedRegion, GraphicsUnit.Pixel);
            }

            //if image goes off right add right of region to image
            if (sectionStartX + sectionWidth > boreholeWidth)
            {
                Rectangle shiftedRegion = new Rectangle(0, sectionStartY, (sectionStartX - boreholeWidth) + sectionWidth, sectionEndY - sectionStartY);

                g.DrawImage(boreholeSectionImage, boreholeWidth - sectionStartX, 0, shiftedRegion, GraphicsUnit.Pixel);
            }

            g.Dispose();

            //section.Save("TestSection.bmp");

            region.Offset(rotationShift, sectionStart);
            autoConvergingRegions.Add(region);

            RefreshBoreholeImage();

            return section;
        }

        private void AutoConvergeInclusion(object items)
        {
            List<object> callbackItems = (List<object>)items;

            Inclusion inclusion = (Inclusion)callbackItems[0];
            Bitmap section = (Bitmap)callbackItems[1];
            Rectangle region = (Rectangle)callbackItems[2];

            int sectionStartY = inclusion.TopYBoundary - 10;
            
            if (sectionStartY < 0)
                sectionStartY = 0;

            int yOffset = sectionStartY;
            
            AutoConvergeSnake autoConverge = new AutoConvergeSnake(section, inclusion.Points, inclusion.LeftXBoundary - 10, yOffset);

            autoConverge.Run();

            List<Point> convergedPoints = autoConverge.GetCurvePoints();

            if(convergedPoints.Count > 0)
                inclusion.SetPoints(convergedPoints);

            autoConvergingRegions.Remove(region);

            RefreshBoreholeImage();
            AutoSave();
        }

        # endregion

        # region mouseOverImage methods

        private void mouseOverImage(object sender, MouseEventArgs e)
        {
            boreholeImagePanel.Focus();

            if (mousePressed && (isClickWithinGrayedArea(e.Y) || IsClickWithinConvergingRegion(e.X, e.Y)))
            {
                mousePressed = false;
                _viewAdapter.DeSelectFeature();
                displayAutoDetectWarning();
            }
            else if (showFeaturesCheckBox.Checked == true)
            {
                sectionStart = _viewAdapter.GetCurrentSectionStart();

                xMousePos = calculateBoundedMouseXPos(e.X);

                xMousePos += rotationShift;

                if (xMousePos < 0)
                {
                    xMousePos += boreholeWidth;
                }
                else if (xMousePos >= boreholeWidth)
                {
                    xMousePos -= boreholeWidth;
                }

                yMousePos = calculateBoundedMouseYPos(e.Y);

                yMousePos += sectionStart;

                checkIfCursorShouldBeChanged();

                checkIfLayerIsToChange();

                checkIfClusterIsToChange();

                checkIfInclusionIsToChange();
            }
        }

        private int calculateBoundedMouseXPos(int pos)
        {
            int xPos;

            if (pos < 0)
                xPos = 0;
            else if (pos >= boreholePictureBox.Width)
                xPos = boreholePictureBox.Width - 1;
            else
                xPos = pos;

            return xPos;
        }

        private int calculateBoundedMouseYPos(int pos)
        {
            int yPos;

            if (pos < 0)
                yPos = 0;
            else if (pos >= boreholePictureBox.Height)
                yPos = boreholePictureBox.Height - 1;
            else
                yPos = pos;

            return yPos;
        }

        private void checkIfCursorShouldBeChanged()
        {
            if (_viewAdapter.OverCurrentFeature(xMousePos, yMousePos) == true)
            {
                checkIfAlterLayerButtonIsSelected();
            }
            else
                this.Cursor = Cursors.Default;
        }

        private void checkIfAlterLayerButtonIsSelected()
        {
            if (selectedButton.Equals("alterLayerToolStripButton"))
            {
                changeToAlterLayerCursor();
            }
            else if (selectedButton.Equals("alterEdgeToolStripButton"))
            {
                changeToAlterEdgeCursor();
            }
            else if (selectedButton.Equals("layerThicknessToolStripButton"))
            {
                changeToAlterThicknessCursor();
            }
        }

        private void changeToAlterLayerCursor()
        {
            int currentAzimuth = (int)(((double)_viewAdapter.TopAzimuthOfSelectedLayer / (double)360) * (double)boreholeWidth);

            if (_viewAdapter.ImageType == "Borehole")
            {
                if (xMousePos < currentAzimuth + 50 && xMousePos > currentAzimuth - 50)
                {
                    changeToChangeAmplitudeCursor();
                }
                else
                {
                    changeToMoveLayerCursor();
                }
            }
            else
            {
                changeToMoveLayerCursor();
            }
        }

        private void changeToAlterEdgeCursor()
        {
            if (_viewAdapter.ImageType == "Borehole")
            {
                int currentAzimuth = calculateAzimuthOfClosestEdge();

                if (xMousePos < currentAzimuth + 50 && xMousePos > currentAzimuth - 50)
                {
                    changeToChangeAmplitudeCursor();
                }
                else
                {
                    changeToMoveLayerCursor();
                }
            }
            else
            {
                changeToMoveLayerCursor();
            }
        }

        private int calculateAzimuthOfClosestEdge()
        {
            int currentAzimuth;

            Layer selectedLayer = (Layer)_viewAdapter.SelectedFeature;
            int firstEdgePoint = selectedLayer.GetTopEdgePoints()[xMousePos].Y;
            int secondEdgePoint = selectedLayer.GetBottomEdgePoints()[xMousePos].Y;

            if (secondEdgePoint - yMousePos > yMousePos - firstEdgePoint)
                currentAzimuth = (int)(((double)_viewAdapter.TopAzimuthOfSelectedLayer / (double)360) * (double)boreholeWidth);
            else
                currentAzimuth = (int)(((double)_viewAdapter.BottomAzimuthOfSelectedLayer / (double)360) * (double)boreholeWidth);

            return currentAzimuth;
        }

        private void changeToChangeAmplitudeCursor()
        {
            this.Cursor = Cursors.SizeNS;
        }

        private void changeToMoveLayerCursor()
        {
            this.Cursor = Cursors.SizeAll;
        }

        private void changeToAlterThicknessCursor()
        {
            this.Cursor = Cursors.HSplit;
        }

        private void checkIfLayerIsToChange()
        {
            if (movingWholeLayer)
            {
                moveWholeLayer();
            }
            else if (alteringWholeAmplitude)
            {
                alterLayerAmplitude();
            }
            else if (movingTopEdge)
            {
                moveTopEdge();
            }
            else if (movingBottomEdge)
            {
                moveBottomEdge();
            }
            else if (alteringTopAmplitude)
            {
                alterTopAmplitude();
            }
            else if (alteringBottomAmplitude)
            {
                alterBottomAmplitude();
            }
            else if (alteringThickness)
            {
                alterThickness();
            }
        }

        private void moveWholeLayer()
        {
            int xMove = xMousePos - previousX;
            int yMove = yMousePos - previousY;
            _viewAdapter.MoveCurrentFeature(3, xMove, yMove);

            boreholePictureBox.Refresh();

            changedSinceLastSave = true;

            previousX = xMousePos;
            previousY = yMousePos;
        }

        private void alterLayerAmplitude()
        {
            int yMove = yMousePos - previousY;

            _viewAdapter.ChangeTopAmplitudeOfCurrentLayer(yMove);
            _viewAdapter.ChangeBottomAmplitudeOfCurrentLayer(yMove);

            boreholePictureBox.Refresh();

            changedSinceLastSave = true;
            
            previousY = yMousePos;
        }

        private void moveTopEdge()
        {
            int xMove = xMousePos - previousX;
            int yMove = yMousePos - previousY;

            _viewAdapter.MoveCurrentFeature(1, xMove, yMove);

            boreholePictureBox.Refresh();

            changedSinceLastSave = true;

            previousX = xMousePos;
            previousY = yMousePos;
        }

        private void moveBottomEdge()
        {
            int xMove = xMousePos - previousX;
            int yMove = yMousePos - previousY;

            _viewAdapter.MoveCurrentFeature(2, xMove, yMove);

            boreholePictureBox.Refresh();

            changedSinceLastSave = true;

            previousX = xMousePos;
            previousY = yMousePos;
        }

        private void alterTopAmplitude()
        {
            int yMove = yMousePos - previousY;
            _viewAdapter.ChangeTopAmplitudeOfCurrentLayer(yMove);

            boreholePictureBox.Refresh();

            changedSinceLastSave = true;

            previousY = yMousePos;
        }

        private void alterBottomAmplitude()
        {
            int yMove = yMousePos - previousY;
            _viewAdapter.ChangeBottomAmplitudeOfCurrentLayer(yMove);

            boreholePictureBox.Refresh();

            changedSinceLastSave = true;

            previousY = yMousePos;
        }

        private void alterThickness()
        {
            Layer selectedLayer = (Layer)_viewAdapter.SelectedFeature;
            
            int firstEdgePoint = selectedLayer.GetTopEdgePoints()[previousX].Y;
            int secondEdgePoint = selectedLayer.GetBottomEdgePoints()[previousX].Y;

            int edgeToMove = 1;

            if (secondEdgePoint - previousY > previousY - firstEdgePoint)
            {
                edgeToMove = 1;
            }
            else
            {
                edgeToMove = 2;
            }

            int yMove = yMousePos - previousY;
            _viewAdapter.MoveCurrentFeature(edgeToMove, 0, yMove);

            boreholePictureBox.Refresh();

            changedSinceLastSave = true;

            previousY = yMousePos;
        }

        private void checkIfClusterIsToChange()
        {
            if (alteringClusterPoint)
            {
                moveClusterPoint();
                changedSinceLastSave = true;
            }
            else if (alteringCluster)
            {
                moveCluster();
                changedSinceLastSave = true;
            }

            boreholePictureBox.Refresh();
        }

        private void moveClusterPoint()
        {
            Point destination = new Point(xMousePos, yMousePos);

            if (xMousePos == 0)
            {
                Console.WriteLine("");
            }

            if (movingPoint.X < 0)
                destination = new Point(destination.X - boreholeWidth, destination.Y);
            else if (movingPoint.X >= boreholeWidth)
                destination = new Point(destination.X + boreholeWidth, destination.Y);

            if (destination.X - movingPoint.X > (boreholeWidth / 4))
                destination.X -= boreholeWidth;

            if (movingPoint.X - destination.X > (boreholeWidth / 4))
                destination.X += boreholeWidth;

            _viewAdapter.MovePoint(movingPoint, destination);

            changedSinceLastSave = true;

            movingPoint = destination;

            previousX = xMousePos;
            previousY = yMousePos;
        }

        private void moveCluster()
        {
            if (xMousePos < 0)
            {
                Console.WriteLine("");
            } 
           
            int xMove = xMousePos - previousX;
            int yMove = yMousePos - previousY;

            if(xMove < -180)
            {
                xMove += boreholeWidth;
            }

            if (xMove > 180)
            {
                xMove -= boreholeWidth;
            }

            _viewAdapter.MoveCurrentFeature(0, xMove, yMove);

            changedSinceLastSave = true;

            previousX = xMousePos;
            previousY = yMousePos;
        }

        private void checkIfInclusionIsToChange()
        {
            if (alteringInclusionPoint)
            {
                MoveInclusionPoint();
            }
            else if (alteringInclusion)
            {
                MoveInclusion();
            }

            boreholePictureBox.Refresh();
        }

        private void MoveInclusionPoint()
        {
            Point destination = new Point(xMousePos, yMousePos);

            if (movingPoint.X < 0)
                destination = new Point(destination.X - boreholeWidth, destination.Y);
            else if (movingPoint.X >= boreholeWidth)
                destination = new Point(destination.X + boreholeWidth, destination.Y);

            if (destination.X - movingPoint.X > (boreholeWidth / 4))
                destination.X -= boreholeWidth;

            if (movingPoint.X - destination.X > (boreholeWidth / 4))
                destination.X += boreholeWidth;

            _viewAdapter.MovePoint(movingPoint, destination);

            changedSinceLastSave = true;

            movingPoint = destination;

            previousX = xMousePos;
            previousY = yMousePos;
        }

        private void MoveInclusion()
        {
            int xMove = xMousePos - previousX;
            int yMove = yMousePos - previousY;

            if (xMove < -180)
            {
                xMove += boreholeWidth;
            }

            if (xMove > 180)
            {
                xMove -= boreholeWidth;
            }

            _viewAdapter.MoveCurrentFeature(0, xMove, yMove);

            changedSinceLastSave = true;

            previousX = xMousePos;
            previousY = yMousePos;
        }

        # endregion

        # region mouse pressed methods

        private void MousePressed(object sender, MouseEventArgs e)
        {
            if (isClickWithinGrayedArea(e.Y) || IsClickWithinConvergingRegion(e.X, e.Y))
            {
                displayAutoDetectWarning();
            }
            else if (showFeaturesCheckBox.Checked == true)
            {
                sectionStart = _viewAdapter.GetCurrentSectionStart();

                previousX = e.X + rotationShift;

                if (previousX < 0)
                {
                    previousX += boreholeWidth;
                }
                else if (previousX >= boreholeWidth)
                {
                    previousX -= boreholeWidth;
                }

                previousY = e.Y + sectionStart;

                checkIfMouseIsOverAFeature();

                mousePressed = true;
            }
        }

        private void checkIfMouseIsOverAFeature()
        {
            if (_viewAdapter.OverCurrentFeature(previousX, previousY))
            {
                if (selectedButton.Equals("alterLayerToolStripButton"))
                {
                    checkWhatTypeOfAlterToPerformOnLayer();
                }
                else if (selectedButton.Equals("alterEdgeToolStripButton"))
                {
                    checkWhichEdgeToAlter();
                }
                else if (selectedButton.Equals("layerThicknessToolStripButton"))
                {
                    alteringThickness = true;
                }
                else if (selectedButton.Equals("editClusterToolStripButton"))
                {
                    alteringCluster = true;

                    checkIfMouseIsOverClusterPoint();
                }
                else if (selectedButton.Equals("addClusterPointToolStripButton"))
                {
                    CheckIfClusterPointCanBeAdded();
                }
                else if (selectedButton.Equals("removeClusterPointToolStripButton"))
                {
                    checkIfClusterPointCanBeRemoved();
                }
                else if (selectedButton.Equals("editInclusionToolStripButton"))
                {
                    alteringInclusion = true;

                    checkIfMouseIsOverInclusionPoint();
                }
                else if (selectedButton.Equals("addInclusionPointToolStripButton"))
                {
                    CheckIfInclusionPointCanBeAdded();
                }
                else if (selectedButton.Equals("removeInclusionPointToolStripButton"))
                {
                    checkIfMouseWasPressedOverInclusionPoint();
                }
            }
        }

        private void checkWhatTypeOfAlterToPerformOnLayer()
        {
            int currentAzimuth = (int)(((double)_viewAdapter.TopAzimuthOfSelectedLayer / (double)360) * (double)boreholeWidth);

            if (previousX < currentAzimuth + 50 && previousX > currentAzimuth - 50)
                alteringWholeAmplitude = true;
            else
                movingWholeLayer = true;
        }

        private void checkWhichEdgeToAlter()
        {
            Layer selectedLayer = (Layer)_viewAdapter.SelectedFeature;

            int firstEdgePoint = selectedLayer.GetTopEdgePoints()[previousX].Y;
            int secondEdgePoint = selectedLayer.GetBottomEdgePoints()[previousX].Y;

            if (secondEdgePoint - previousY > previousY - firstEdgePoint)
            {
                checkWhatTypeOfAlterToPerformOnTopEdge();
            }
            else
            {
                checkWhatTypeOfAlterToPerformOnBottomEdge();
            }
        }

        private void checkWhatTypeOfAlterToPerformOnTopEdge()
        {
            int currentAzimuth = (int)(((double)_viewAdapter.TopAzimuthOfSelectedLayer / (double)360) * (double)boreholeWidth);

            if (previousX < currentAzimuth + 50 && previousX > currentAzimuth - 50)
                alteringTopAmplitude = true;
            else
                movingTopEdge = true;
        }

        private void checkWhatTypeOfAlterToPerformOnBottomEdge()
        {
            int currentAzimuth = (int)(((double)_viewAdapter.BottomAzimuthOfSelectedLayer / (double)360) * (double)boreholeWidth);

            if (previousX < currentAzimuth + 50 && previousX > currentAzimuth - 50)
                alteringBottomAmplitude = true;
            else
                movingBottomEdge = true;
        }

        private void checkIfMouseIsOverClusterPoint()
        {
            Cluster selectedCluster = (Cluster)_viewAdapter.SelectedFeature;
            List<Point> points = selectedCluster.Points;

            for (int i = 0; i < points.Count; i++)
            {
                Point currentPoint = points[i];

                if (currentPoint.X < 0)
                    currentPoint = new Point(currentPoint.X + boreholeWidth, currentPoint.Y);
                else if (currentPoint.X >= boreholeWidth)
                    currentPoint = new Point(currentPoint.X - boreholeWidth, currentPoint.Y);

                if (PointsAreClose(new Point(previousX, previousY), currentPoint))
                {
                    alteringClusterPoint = true;
                    movingPoint = points[i];
                }
            }
        }

        /// <summary>
        /// Checks if the current click is over a cluster line and if so adds a new point
        /// </summary>
        private void CheckIfClusterPointCanBeAdded()
        {
            bool isPointToBeAdded = false;
            int addAfter = 0;

            Cluster selectedCluster = (Cluster)_viewAdapter.SelectedFeature;
            List<Point> points = selectedCluster.Points;
            Point addPoint = new Point(-1, -1);
            Point clickedPoint = new Point(previousX, previousY);

            Point leftWrapClickPoint = new Point(clickedPoint.X - boreholeWidth, clickedPoint.Y);
            Point rightWrapClickPoint = new Point(clickedPoint.X + boreholeWidth, clickedPoint.Y);
            
            //check if click is over line
            for (int i = 0; i < points.Count-1; i++)
            {
                addPoint = CheckIfPointCanBeAdded(clickedPoint, points[i], points[i+1]);

                if (addPoint.X != -1 && addPoint.X != 1)
                {
                    isPointToBeAdded = true;
                    addAfter = i;
                    break;
                }

                //Also check for if image is wrapped at ends
                //clickedPoint.X += boreholeWidth;
                addPoint = CheckIfPointCanBeAdded(rightWrapClickPoint, points[i], points[i + 1]);
                if (addPoint.X != -1 && addPoint.X != 1)
                {
                    isPointToBeAdded = true;
                    addAfter = i;
                    break;
                }

                //clickedPoint.X -= boreholeWidth;
                //clickedPoint.X -= boreholeWidth;
                addPoint = CheckIfPointCanBeAdded(leftWrapClickPoint, points[i], points[i + 1]);

                if (addPoint.X != -1 && addPoint.X != 1)
                {
                    isPointToBeAdded = true;
                    addAfter = i;
                    break;
                }
            }

            //Check last to first line
            if (isPointToBeAdded == false)
            {
                addPoint = CheckIfPointCanBeAdded(clickedPoint, points[points.Count - 1], points[0]);

                if (addPoint.X != -1 && addPoint.X != 1)
                {
                    isPointToBeAdded = true;
                    addAfter = points.Count - 1;
                }

                //Check last to first line
                if(!isPointToBeAdded)
                {
                    //Also check for if image is wrapped at other end
                    //clickedPoint.X += boreholeWidth;
                    addPoint = CheckIfPointCanBeAdded(rightWrapClickPoint, points[points.Count - 1], points[0]);


                    if (addPoint.X != -1 && addPoint.X != 1)
                    {
                        isPointToBeAdded = true;
                        addAfter = points.Count - 1;

                    }

                    if(!isPointToBeAdded)
                    {

                        //clickedPoint.X -= boreholeWidth;
                        //clickedPoint.X -= boreholeWidth;
                        addPoint = CheckIfPointCanBeAdded(leftWrapClickPoint, points[points.Count - 1], points[0]);

                        if (addPoint.X != -1 && addPoint.X != 1)
                        {
                            isPointToBeAdded = true;
                            addAfter = points.Count - 1;
                        }
                    }
                }
            }
            
            if (isPointToBeAdded)
            {
                _viewAdapter.AddPoint(addPoint, addAfter);
                AutoSave();
            }
        }

        /// <summary>
        /// Checks if the given clicked point is on the line denoted by startOfLine and endOfLine (within a small error rate).
        /// If so the position on the line where the point is to be added is returned. If not the point -1,-1 is returned.
        /// </summary>
        /// <param name="clickedPoint"></param>
        /// <param name="startOfLine"></param>
        /// <param name="endOfLine"></param>
        /// <returns></returns>
        private Point CheckIfPointCanBeAdded(Point clickedPoint, Point startOfLine, Point endOfLine)
        {
            Point addPoint = new Point(-1, -1);
            double distance;

            if (ClickIsWithinBounds(startOfLine, endOfLine, clickedPoint))
            {
                if ((distance = CalculateDistance(startOfLine, endOfLine, clickedPoint)) < 5.0)
                {
                    addPoint = GetClosestPointToClick(startOfLine, endOfLine, clickedPoint);
                }
            }
            return addPoint;

        }

        private bool ClickIsWithinBounds(Point startOfLine, Point endOfLine, Point clickedPoint)
        {
            if (clickedPoint.X > Math.Min(startOfLine.X, endOfLine.X) - 5
                && clickedPoint.X < Math.Max(startOfLine.X, endOfLine.X) + 5
                && clickedPoint.Y > Math.Min(startOfLine.Y, endOfLine.Y) - 5
                && clickedPoint.Y < Math.Max(startOfLine.Y, endOfLine.Y) + 5)
                return true;

            else
                return false;
        }        
        
        /// <summary>
        /// Calculates the perpendicular distance of point distancePos to the line startPos-endPos
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="endPoint"></param>
        /// <param name="clickedPoint"></param>
        /// <returns></returns>
        private double CalculateDistance(Point startPoint, Point endPoint, Point clickedPoint)
        {
            double distance;

            int startX = startPoint.X;
            int startY = startPoint.Y;

            int endX = endPoint.X;
            int endY = endPoint.Y;

            int distX = clickedPoint.X;
            int distY = clickedPoint.Y;

            double top = ((endX - startX) * (startY - distY)) - ((startX - distX) * (endY - startY));
            double bottom = Math.Sqrt(Math.Pow(endX - startX, 2) + Math.Pow(endY - startY, 2));

            distance = top / bottom;

            if (distance < 0)
                distance = 0 - distance;

            return distance;
        }

        /// <summary>
        /// Calculates the closest point on the given line (startOfLine-endOfLine) to the given point (clickedPoint)
        /// </summary>
        /// <param name="startOfLine"></param>
        /// <param name="endOfLine"></param>
        /// <param name="clickedPoint"></param>
        /// <returns></returns>
        private Point GetClosestPointToClick(Point startOfLine, Point endOfLine, Point clickedPoint)
        {
            Point closestPoint;

            int startToClickX = clickedPoint.X - startOfLine.X;
            int startToClickY = clickedPoint.Y - startOfLine.Y;

            int startToEndX = endOfLine.X - startOfLine.X;
            int startToEndY = endOfLine.Y - startOfLine.Y;

            double startToEndSquared = Math.Pow(startToEndX, 2) + Math.Pow(startToEndY, 2);                     //Square magnitude of start to end

            double startToClickDotStartToEnd = (startToClickX * startToEndX) + (startToClickY * startToEndY);   //Dot product of lines start to end and start to click

            double normalised = startToClickDotStartToEnd / startToEndSquared;                                  //Normalised distance from a to closest point
        
        
            int xPoint = (int)(startOfLine.X + ((double)startToEndX * normalised));                             
            int yPoint = (int)(startOfLine.Y + ((double)startToEndY * normalised));


            closestPoint = new Point(xPoint, yPoint);

            return closestPoint;
        }

        private void checkIfClusterPointCanBeRemoved()
        {
            bool isPointToBeDeleted = false;

            Cluster selectedCluster = (Cluster)_viewAdapter.SelectedFeature;
            List<Point> points = selectedCluster.Points;
            Point deletePoint = new Point(0, 0);

            for (int i = 0; i < points.Count; i++)
            {
                Point currentPoint = points[i];

                if (currentPoint.X < 0)
                    currentPoint.X += boreholeWidth;
                else if (currentPoint.X >= boreholeWidth)
                    currentPoint.X -= boreholeWidth;

                if (PointsAreClose(new Point(previousX, previousY), currentPoint))
                {
                    deletePoint = points[i];
                    isPointToBeDeleted = true;
                }
            }

            if (isPointToBeDeleted == true && points.Count > 3)
            {
                _viewAdapter.DeletePoint(deletePoint);
                boreholePictureBox.Refresh();
                changedSinceLastSave = true;
                AutoSave();
            }
        }

        private void checkIfMouseIsOverInclusionPoint()
        {
            Inclusion selectedInclusion = (Inclusion)_viewAdapter.SelectedFeature;
            List<Point> points = selectedInclusion.Points;

            for (int i = 0; i < points.Count; i++)
            {
                Point currentPoint = points[i];

                if (currentPoint.X < 0)
                    currentPoint = new Point(currentPoint.X + boreholeWidth, currentPoint.Y);
                else if (currentPoint.X >= boreholeWidth)
                    currentPoint = new Point(currentPoint.X - boreholeWidth, currentPoint.Y);

                if (PointsAreClose(new Point(previousX, previousY), currentPoint))
                {
                    alteringInclusionPoint = true;
                    movingPoint = points[i];
                }
            }
        }

        /// <summary>
        /// Checks if the current click is over a cluster line and if so adds a new point
        /// </summary>
        private void CheckIfInclusionPointCanBeAdded()
        {
            bool isPointToBeAdded = false;
            int addAfter = 0;

            Inclusion selectedInclusion = (Inclusion)_viewAdapter.SelectedFeature;
            List<Point> points = selectedInclusion.Points;
            Point addPoint = new Point(-1, -1);
            Point clickedPoint = new Point(previousX, previousY);

            Point leftWrapClickPoint = new Point(clickedPoint.X - boreholeWidth, clickedPoint.Y);
            Point rightWrapClickPoint = new Point(clickedPoint.X + boreholeWidth, clickedPoint.Y);
            

            //check if click is over line
            for (int i = 0; i < points.Count - 1; i++)
            {
                addPoint = CheckIfPointCanBeAdded(clickedPoint, points[i], points[i + 1]);

                if (addPoint.X != -1 && addPoint.X != 1)
                {
                    isPointToBeAdded = true;
                    addAfter = i;
                    break;
                }

                //Also check for if image is wrapped at other end
                //clickedPoint.X += boreholeWidth;
                addPoint = CheckIfPointCanBeAdded(rightWrapClickPoint, points[i], points[i + 1]);
                if (addPoint.X != -1 && addPoint.X != 1)
                {
                    isPointToBeAdded = true;
                    addAfter = i;
                    break;
                }

                //clickedPoint.X -= boreholeWidth;
                //clickedPoint.X -= boreholeWidth;
                addPoint = CheckIfPointCanBeAdded(leftWrapClickPoint, points[i], points[i + 1]);

                if (addPoint.X != -1 && addPoint.X != 1)
                {
                    isPointToBeAdded = true;
                    addAfter = i;
                    break;
                }
            }

            //Check last to first line
            if (isPointToBeAdded == false)
            {
                addPoint = CheckIfPointCanBeAdded(clickedPoint, points[points.Count - 1], points[0]);

                if (addPoint.X != -1 && addPoint.X != 1)
                {
                    isPointToBeAdded = true;
                    addAfter = points.Count - 1;
                }

                if (!isPointToBeAdded)
                {
                    //Also check for if image is wrapped at other end
                    //clickedPoint.X += boreholeWidth;
                    addPoint = CheckIfPointCanBeAdded(rightWrapClickPoint, points[points.Count - 1], points[0]);

                    if (addPoint.X != -1 && addPoint.X != 1)
                    {
                        isPointToBeAdded = true;
                        addAfter = points.Count - 1;

                    }

                    if (!isPointToBeAdded)
                    {

                        //clickedPoint.X -= boreholeWidth;
                        //clickedPoint.X -= boreholeWidth;
                        addPoint = CheckIfPointCanBeAdded(leftWrapClickPoint, points[points.Count - 1], points[0]);

                        if (addPoint.X != -1 && addPoint.X != 1)
                        {
                            isPointToBeAdded = true;
                            addAfter = points.Count - 1;
                        }
                    }
                }
            }

            if (isPointToBeAdded)
            {
                _viewAdapter.AddPoint(addPoint, addAfter);
                AutoSave();
            }
        }

        private void checkIfMouseWasPressedOverInclusionPoint()
        {
            bool pointToDelete = false;

            Inclusion selectedInclusion = (Inclusion)_viewAdapter.SelectedFeature;
            List<Point> points = selectedInclusion.Points;
            Point deletePoint = new Point(0, 0);

            for (int i = 0; i < points.Count; i++)
            {
                Point currentPoint = points[i];

                if (currentPoint.X < 0)
                    currentPoint.X += boreholeWidth;
                else if (currentPoint.X >= boreholeWidth)
                    currentPoint.X -= boreholeWidth;

                if (PointsAreClose(new Point(previousX, previousY), currentPoint))
                {
                    deletePoint = points[i];
                    pointToDelete = true;
                }
            }

            if (pointToDelete == true && points.Count > 3)
            {
                _viewAdapter.DeletePoint(deletePoint);
                boreholePictureBox.Refresh();
                changedSinceLastSave = true;
                AutoSave();
            }
        }

        # endregion

        private void mouseLeftImage(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Default;
        }

        private void MouseReleased(object sender, MouseEventArgs e)
        {
            if (showFeaturesCheckBox.Checked == true)
            {
                movingWholeLayer = false;
                alteringWholeAmplitude = false;
                movingTopEdge = false;
                alteringTopAmplitude = false;
                movingBottomEdge = false;
                alteringBottomAmplitude = false;
                alteringThickness = false;
                alteringCluster = false;
                alteringInclusion = false;
                alteringClusterPoint = false;
                alteringInclusionPoint = false;

                mousePressed = false;

                //save();
            }
        }

        # endregion

        # region Menu strip methods

        private void boreholeImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetProjectType("Borehole");
            newFromImageToolStripMenuItem_Click(sender, e);
        }

        private void coreImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetProjectType("Core");
            newFromImageToolStripMenuItem_Click(sender, e);
        }

        public void SetProjectType(string type)
        {
            _viewAdapter.ProjectType = type;
        }

        /// <summary>
        /// Instructs the controller to create a new image from an image file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newFromImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (changedSinceLastSave == true)
            {
                if (showSaveOrExitDialog() != "Cancel")
                {
                    OpenImageFile();
                }
            }
            else
            {
                OpenImageFile();                
            }

            if (autoDetectRunning)
                StopEdgeDetection(false);
        }

        private void SetDetailsDialogTitle()
        {
            if (_viewAdapter.ImageType == "Borehole")
            {
                boreholeDetailsToolStripMenuItem.Text = "Borehole details";
            }
            else
            {
                boreholeDetailsToolStripMenuItem.Text = "Core details";
            }
        }

        private void newFromOtvHedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (changedSinceLastSave == true)
            {
                if (showSaveOrExitDialog() != "Cancel")
                {
                    OpenOTVFile();
                }
            }
            else
            {
                OpenOTVFile();
            }            

            if (autoDetectRunning)
                StopEdgeDetection(false);
        }

        /// <summary>
        /// Method which loads the features
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loadFeatures(object sender, EventArgs e)
        {
            if (changedSinceLastSave == true)
            {
                if (showSaveOrExitDialog() != "Cancel")
                {
                    openExistingFile();
                }
            }
            else
            {
                openExistingFile();
            }

            if (autoDetectRunning)
                StopEdgeDetection(false);
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (changedSinceLastSave == true)
            {
                DialogResult result = MessageBox.Show("Would you like to save before closing?", "Feature Annotation Tool", MessageBoxButtons.YesNoCancel);
                
                if (result == DialogResult.Yes)
                {
                    //if (previouslySaved)
                    Save();
                    //else
                    //    saveAs();

                    ResetAll();

                }
                else if (result == DialogResult.No)
                {
                    ResetAll();
                }
                else if (result == DialogResult.Cancel)
                {

                }
            }
            else
                ResetAll();

            if (autoDetectRunning)
                StopEdgeDetection(false);
        }

        /// <summary>
        /// Method which calls the save features method of the model
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveFeatures(object sender, EventArgs e)
        {
            Save();

            changedSinceLastSave = false;
        }

        # region Export image methods

        private void withFeaturesImageOnlyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool withFeatures = true;
            bool withRuler = false;
            ExportImage(withFeatures, withRuler);
        }

        private void withFeaturesAddRulerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool withFeatures = true;
            bool withRuler = true;
            ExportImage(withFeatures, withRuler);
        }

        private void withoutFeaturesImageOnlyToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            bool withFeatures = false;
            bool withRuler = false;
            ExportImage(withFeatures, withRuler);
        }

        private void withoutFeaturesAddRulerToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            bool withFeatures = false;
            bool withRuler = true;
            ExportImage(withFeatures, withRuler);
        }

        private void ExportImage(bool withFeatures, bool withRuler)
        {
            try
            {
                SaveFileDialog saveImageFileDialog = new SaveFileDialog();
                saveImageFileDialog.AddExtension = true;
                saveImageFileDialog.DefaultExt = "bmp";
                saveImageFileDialog.Filter = "Bitmap Images|*.bmp";

                if(withFeatures)
                    saveImageFileDialog.FileName = boreholeName + " - features";
                else
                    saveImageFileDialog.FileName = boreholeName;

                saveImageFileDialog.OverwritePrompt = true;
                saveImageFileDialog.InitialDirectory = _viewAdapter.GetProjectLocation();

                if (saveImageFileDialog.ShowDialog() == DialogResult.OK)
                {
                    int exportSectionHeight;

                    int boreholeDepthInPixels = _viewAdapter.BoreholeDepth;

                    bool okClicked = true;

                    if (boreholeDepthInPixels > 10000)
                    {
                        SectionSizeDialogBox sectionSizeBox = new SectionSizeDialogBox();
                        sectionSizeBox.SetMaximum(boreholeDepthInPixels);
                        sectionSizeBox.ShowDialog();

                        exportSectionHeight = sectionSizeBox.GetSectionHeight();

                        if (sectionSizeBox.OkClicked() == true)
                        {
                            okClicked = true;
                        }
                        else
                            okClicked = false;

                        sectionSizeBox.Dispose();
                    }
                    else
                        exportSectionHeight = boreholeDepthInPixels;

                    if (okClicked)
                    {
                        StartProgressReport("Writing image");

                        drawingFeaturesImage = true;
                        bool isChecked = showFeaturesCheckBox.Checked;
                        showFeaturesCheckBox.Checked = true;

                        string fileName = checkExtensionIsCorrect(saveImageFileDialog.FileName, "bmp");

                        _viewAdapter.ExportImage(fileName, withFeatures, withRuler, exportSectionHeight);

                        showFeaturesCheckBox.Checked = isChecked;
                        drawingFeaturesImage = false;

                        EndProgressReport();
                    }
                }
            }
            catch (PathTooLongException ptle)
            {
                EndProgressReport();
                MessageBox.Show("Error - The file path is too long: " + ptle.Message, "File path too long exception");
                //resetAll();
            }
            catch (Exception ex)
            {
                EndProgressReport();
                MessageBox.Show("Error creating image: " + ex.Message, "Error");
            }
        }

        # endregion

        /// <summary>
        /// Saves the annotated borehole image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveAsImageWithFeatures(object sender, EventArgs e)
        {
        }

        private string checkExtensionIsCorrect(string fileName, string extension)
        {
            string correctedString;

            char[] text = fileName.ToCharArray();
            char[] ext = extension.ToCharArray();

            if (fileName.EndsWith("." + extension))
                return fileName;
            else
            {
                correctedString = fileName + "." + extension;
                return correctedString;
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        # region Write Features methods

        private void writeAllFeaturesToExcel(object sender, EventArgs e)
        {
            try
            {
                LayerPropertiesOptionsForm layerPropertiesForm = new LayerPropertiesOptionsForm(_viewAdapter.ImageType);
                ClusterPropertiesOptionsForm clusterPropertiesForm = new ClusterPropertiesOptionsForm();
                InclusionPropertiesOptionsForm inclusionPropertiesForm = new InclusionPropertiesOptionsForm();

                bool cancelExport = false;

                if (boreholeSectionImage != null)
                {
                    List<string> layerProperties = null;
                    List<string> clusterProperties = null;
                    List<string> inclusionProperties = null;

                    if (_viewAdapter.getNumOfLayers() > 0)
                    {
                        layerProperties = RequestLayerProperties(layerPropertiesForm);

                        if (!layerPropertiesForm.IsOkayClicked())
                            cancelExport = true;
                    }

                    if (_viewAdapter.getNumOfClusters() > 0 && cancelExport == false)
                    {
                        clusterProperties = RequestClusterProperties(clusterPropertiesForm);

                        if (!clusterPropertiesForm.IsOkayClicked())
                            cancelExport = true;
                    }

                    if (_viewAdapter.getNumOfInclusions() > 0 && cancelExport == false)
                    {
                        inclusionProperties = RequestInclusionProperties(inclusionPropertiesForm);

                        if (!inclusionPropertiesForm.IsOkayClicked())
                            cancelExport = true;
                    }

                    if (cancelExport == false && (_viewAdapter.getNumOfLayers() > 0 || _viewAdapter.getNumOfClusters() > 0 || _viewAdapter.getNumOfInclusions() > 0))
                    {
                        SaveFileDialog saveDialog = new SaveFileDialog();
                        saveDialog.DefaultExt = "*.xlsx";
                        saveDialog.Filter = "Excel Files|*.xlsx";
                        saveDialog.InitialDirectory = _viewAdapter.GetProjectLocation();

                        saveDialog.FileName = boreholeName + " - features";

                        if (saveDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK && saveDialog.FileName.Length > 0)
                        {
                            StartProgressReport("Writing features file");
                            _viewAdapter.WriteAllFeaturesToExcel(saveDialog.FileName, layerProperties, clusterProperties, inclusionProperties);
                            EndProgressReport();
                        }
                    }
                }
            }
            catch (PathTooLongException ptle)
            {
                EndProgressReport();
                MessageBox.Show("Error - The file path is too long: " + ptle.Message, "File path too long exception");
                //resetAll();
            }
            catch (Exception ex)
            {
                EndProgressReport();
                MessageBox.Show("Error creating file: " + ex.Message, "Error");
            }
        }

        private void WriteAllLayersToExcel(object sender, EventArgs e)
        {
            try
            {
                LayerPropertiesOptionsForm layerPropertiesForm = new LayerPropertiesOptionsForm(_viewAdapter.ImageType);

                bool cancelExport = false;

                if (boreholeSectionImage != null)
                {
                    List<string> layerProperties = null;
                    List<string> clusterProperties = null;
                    List<string> inclusionProperties = null;

                    if (_viewAdapter.getNumOfLayers() > 0)
                    {
                        layerProperties = RequestLayerProperties(layerPropertiesForm);

                        if (!layerPropertiesForm.IsOkayClicked())
                            cancelExport = true;
                    }

                    if (cancelExport == false)
                    {
                        SaveFileDialog saveDialog = new SaveFileDialog();
                        saveDialog.DefaultExt = "*.xlsx";
                        saveDialog.Filter = "Excel Files|*.xlsx";
                        saveDialog.FileName = boreholeName + " - layers";
                        saveDialog.InitialDirectory = _viewAdapter.GetProjectLocation();

                        if (saveDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK && saveDialog.FileName.Length > 0)
                        {
                            StartProgressReport("Writing features file");
                            _viewAdapter.WriteAllFeaturesToExcel(saveDialog.FileName, layerProperties, clusterProperties, inclusionProperties);
                            EndProgressReport();
                        }
                    }
                }
            }
            catch (PathTooLongException ptle)
            {
                EndProgressReport();
                MessageBox.Show("Error - The file path is too long: " + ptle.Message, "File path too long exception");
                //resetAll();
            }
            catch (Exception ex)
            {
                EndProgressReport();
                MessageBox.Show("Error creating file: " + ex.Message, "Error");
            }
        }

        private void WriteAllClustersToExcel(object sender, EventArgs e)
        {
            try
            {
                ClusterPropertiesOptionsForm clusterPropertiesForm = new ClusterPropertiesOptionsForm();

                bool cancelExport = false;

                if (boreholeSectionImage != null)
                {
                    List<string> layerProperties = null;
                    List<string> clusterProperties = null;
                    List<string> inclusionProperties = null;

                    if (_viewAdapter.getNumOfClusters() > 0)
                    {
                        clusterProperties = RequestClusterProperties(clusterPropertiesForm);

                        if (!clusterPropertiesForm.IsOkayClicked())
                            cancelExport = true;
                    }

                    if (cancelExport == false)
                    {
                        SaveFileDialog saveDialog = new SaveFileDialog();
                        saveDialog.DefaultExt = "*.xlsx";
                        saveDialog.Filter = "Excel Files|*.xlsx";
                        saveDialog.FileName = boreholeName + " - clusters";
                        saveDialog.InitialDirectory = _viewAdapter.GetProjectLocation();

                        if (saveDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK && saveDialog.FileName.Length > 0)
                        {
                            StartProgressReport("Writing features file");
                            _viewAdapter.WriteAllFeaturesToExcel(saveDialog.FileName, layerProperties, clusterProperties, inclusionProperties);
                            EndProgressReport();
                        }
                    }
                }
            }
            catch (PathTooLongException ptle)
            {
                EndProgressReport();
                MessageBox.Show("Error - The file path is too long: " + ptle.Message, "File path too long exception");
                //resetAll();
            }
            catch (Exception ex)
            {
                EndProgressReport();
                MessageBox.Show("Error creating file: " + ex.Message, "Error");
            }
        }

        private void WriteAllInclusionsToExcel(object sender, EventArgs e)
        {
            try
            {
                InclusionPropertiesOptionsForm inclusionPropertiesForm = new InclusionPropertiesOptionsForm();

                bool cancelExport = false;

                if (boreholeSectionImage != null)
                {
                    List<string> layerProperties = null;
                    List<string> clusterProperties = null;
                    List<string> inclusionProperties = null;

                    if (_viewAdapter.getNumOfInclusions() > 0)
                    {
                        inclusionProperties = RequestInclusionProperties(inclusionPropertiesForm);

                        if (!inclusionPropertiesForm.IsOkayClicked())
                            cancelExport = true;
                    }

                    if (cancelExport == false)
                    {
                        SaveFileDialog saveDialog = new SaveFileDialog();
                        saveDialog.DefaultExt = "*.xlsx";
                        saveDialog.Filter = "Excel Files|*.xlsx";
                        saveDialog.FileName = boreholeName + " - inclusions";
                        saveDialog.InitialDirectory = _viewAdapter.GetProjectLocation();

                        if (saveDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK && saveDialog.FileName.Length > 0)
                        {
                            StartProgressReport("Writing features file");
                            _viewAdapter.WriteAllFeaturesToExcel(saveDialog.FileName, layerProperties, clusterProperties, inclusionProperties);
                            EndProgressReport();
                        }
                    }
                }
            }
            catch (PathTooLongException ptle)
            {
                EndProgressReport();
                MessageBox.Show("Error - The file path is too long: " + ptle.Message, "File path too long exception");
                //resetAll();
            }
            catch (Exception ex)
            {
                EndProgressReport();
                MessageBox.Show("Error creating file: " + ex.Message, "Error");
            }
        }

        private void LayersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                LayerPropertiesOptionsForm layerPropertiesForm = new LayerPropertiesOptionsForm(_viewAdapter.ImageType);

                bool cancelExport = false;

                if (boreholeSectionImage != null)
                {
                    List<string> layerProperties = null;

                    if (_viewAdapter.getNumOfLayers() > 0)
                    {
                        layerProperties = RequestLayerProperties(layerPropertiesForm);

                        if (!layerPropertiesForm.IsOkayClicked())
                            cancelExport = true;
                    }

                    if (cancelExport == false)
                    {
                        SaveFileDialog saveDialog = new SaveFileDialog();
                        saveDialog.DefaultExt = "*.txt";
                        saveDialog.Filter = "Text Files|*.txt";
                        saveDialog.FileName = boreholeName + " - layers";
                        saveDialog.InitialDirectory = _viewAdapter.GetProjectLocation();

                        if (saveDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK &&
                        saveDialog.FileName.Length > 0)
                        {
                            StartProgressReport("Writing layers to file");
                            _viewAdapter.writeLayersToText(saveDialog.FileName, layerProperties);
                            EndProgressReport();
                        }
                    }
                }
            }
            catch (PathTooLongException ptle)
            {
                EndProgressReport();
                MessageBox.Show("Error - The file path is too long: " + ptle.Message, "File path too long exception");
                //resetAll();
            }
            catch (Exception ex)
            {
                EndProgressReport();
                MessageBox.Show("Error creating file: " + ex.Message, "Error");
            }
        }

        private void WriteAllClustersToText(object sender, EventArgs e)
        {
            try
            {
                ClusterPropertiesOptionsForm clusterPropertiesForm = new ClusterPropertiesOptionsForm();

                bool cancelExport = false;

                if (boreholeSectionImage != null)
                {
                    List<string> clusterProperties = null;

                    if (_viewAdapter.getNumOfClusters() > 0)
                    {
                        clusterProperties = RequestClusterProperties(clusterPropertiesForm);

                        if (!clusterPropertiesForm.IsOkayClicked())
                            cancelExport = true;
                    }

                    if (cancelExport == false)
                    {
                        SaveFileDialog saveDialog = new SaveFileDialog();
                        saveDialog.DefaultExt = "*.xlsx";
                        saveDialog.Filter = "Excel Files|*.xlsx";
                        saveDialog.FileName = boreholeName + " - clusters";
                        saveDialog.InitialDirectory = _viewAdapter.GetProjectLocation();

                        if (saveDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK && saveDialog.FileName.Length > 0)
                        {
                            StartProgressReport("Writing features file");
                            _viewAdapter.WriteClustersToText(saveDialog.FileName, clusterProperties);
                            EndProgressReport();
                        }
                    }
                }
            }
            catch (PathTooLongException ptle)
            {
                EndProgressReport();
                MessageBox.Show("Error - The file path is too long: " + ptle.Message, "File path too long exception");
                //resetAll();
            }
            catch (Exception ex)
            {
                EndProgressReport();
                MessageBox.Show("Error creating file: " + ex.Message, "Error");
            }
        }

        private void WriteAllInclusionsToText(object sender, EventArgs e)
        {
            try
            {
                InclusionPropertiesOptionsForm inclusionPropertiesForm = new InclusionPropertiesOptionsForm();

                bool cancelExport = false;

                if (boreholeSectionImage != null)
                {
                    List<string> inclusionProperties = null;

                    if (_viewAdapter.getNumOfInclusions() > 0)
                    {
                        inclusionProperties = RequestInclusionProperties(inclusionPropertiesForm);

                        if (!inclusionPropertiesForm.IsOkayClicked())
                            cancelExport = true;
                    }

                    if (cancelExport == false)
                    {
                        SaveFileDialog saveDialog = new SaveFileDialog();
                        saveDialog.DefaultExt = "*.xlsx";
                        saveDialog.Filter = "Excel Files|*.xlsx";
                        saveDialog.FileName = boreholeName + " - inclusions";
                        saveDialog.InitialDirectory = _viewAdapter.GetProjectLocation();

                        if (saveDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK && saveDialog.FileName.Length > 0)
                        {
                            StartProgressReport("Writing features file");
                            _viewAdapter.WriteInclusionsToText(saveDialog.FileName, inclusionProperties);
                            EndProgressReport();
                        }
                    }
                }
            }
            catch (PathTooLongException ptle)
            {
                EndProgressReport();
                MessageBox.Show("Error - The file path is too long: " + ptle.Message, "File path too long exception");
                //resetAll();
            }
            catch (Exception ex)
            {
                EndProgressReport();
                MessageBox.Show("Error creating file: " + ex.Message, "Error");
            }
        }

        private List<string> RequestLayerProperties(LayerPropertiesOptionsForm layerPropertiesForm)
        {
            List<string> layerPropertiesToExport = null;

            layerPropertiesForm.ShowDialog();

            if (layerPropertiesForm.IsOkayClicked())
            {
                layerPropertiesToExport = layerPropertiesForm.GetSelectedItems();
            }

            return layerPropertiesToExport;
        }

        private List<string> RequestInclusionProperties(InclusionPropertiesOptionsForm inclusionPropertiesForm)
        {
            List<string> inclusionPropertiesToExport = null;

            inclusionPropertiesForm.ShowDialog();

            if (inclusionPropertiesForm.IsOkayClicked())
            {
                inclusionPropertiesToExport = inclusionPropertiesForm.GetSelectedItems();
            }

            return inclusionPropertiesToExport;
        }

        private List<string> RequestClusterProperties(ClusterPropertiesOptionsForm clusterPropertiesForm)
        {
            List<string> clusterPropertiesToExport = null;

            clusterPropertiesForm.ShowDialog();

            if (clusterPropertiesForm.IsOkayClicked())
            {
                clusterPropertiesToExport = clusterPropertiesForm.GetSelectedItems();
            }

            return clusterPropertiesToExport;
        }

        private void WriteLayersForWellCAD(object sender, EventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.DefaultExt = "*.war";
            saveDialog.Filter = "WellCAD structure log ascii file|*.war";
            saveDialog.InitialDirectory = _viewAdapter.GetProjectLocation();

            saveDialog.FileName = boreholeName + " - structures";

            if (saveDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK && saveDialog.FileName.Length > 0)
            {
                StartProgressReport("Writing features file");
                _viewAdapter.WriteLayersForWellCAD(saveDialog.FileName);
                EndProgressReport();
            }
        }

        # endregion

        # region EdgeProcessing auto detect

        private void detectLayersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (autoDetectRunning == false)
            {
                autoDetectType = "EdgeProcessing";

                autoDetectForm = new AutoDetectParametersForm(boreholeSectionImage.Width);

                /**if (cannyHigh != 0)
                {
                    autoDetectForm.SetCannyLow(cannyLow);
                    autoDetectForm.SetCannyHigh(cannyHigh);

                    autoDetectForm.SetGaussianWidth(gaussianWidth);
                    autoDetectForm.SetGaussianSigma(gaussianAngle);

                    autoDetectForm.SetHorizontalWeight(horizontalWeight);
                    autoDetectForm.SetVerticalWeight(verticalWeight);

                    autoDetectForm.SetEdgeLinkingThreshold(edgeLinkingThreshold);
                    autoDetectForm.SetEdgeLinkingDistance(edgeLinkingDistance);

                    autoDetectForm.SetEdgeRemovalThreshold(edgeRemovalThreshold);
                    autoDetectForm.SetEdgeJoiningDistance(edgeJoiningThreshold);
                }**/
                
                autoDetectForm.SetStartDepth(_viewAdapter.BoreholeStartDepth);
                autoDetectForm.SetEndDepth(_viewAdapter.BoreholeEndDepth);

                autoDetectForm.SetDepthResolution(_viewAdapter.DepthResolution);

                autoDetectForm.SetSectionStartDepth(_viewAdapter.GetCurrentSectionStart());
                autoDetectForm.SetSectionEndDepth(_viewAdapter.GetCurrentSectionEnd());

                autoDetectForm.SetViewingAreaStart(_viewAdapter.GetCurrentSectionStart() + boreholeImagePanel.VerticalScroll.Value);
                autoDetectForm.SetViewingAreaEnd(_viewAdapter.GetCurrentSectionStart() + boreholeImagePanel.VerticalScroll.Value + boreholeImagePanel.Height);

                autoDetectForm.SetSection();

                autoDetectForm.ShowDialog();

                GetEdgeProcessingAutoDetectValues();

                if (autoDetectForm.getRun() == true)
                {
                    autoDetectRunning = true;

                    checkWhetherToRemoveEdges(autoDetectForm.GetStartProcessingDepth(), autoDetectForm.GetEndProcessingDepth());

                    detectLayersToolStripMenuItem.Enabled = false;

                    EnableAutoDetectToolStripItems();

                    startThread();
                }

                autoDetectForm.Dispose();
            }
        }

        private void GetEdgeProcessingAutoDetectValues()
        {
            cannyLow = autoDetectForm.getCannyLow();
            cannyHigh = autoDetectForm.getCannyHigh();
            gaussianWidth = autoDetectForm.getGaussianWidth();
            gaussianAngle = autoDetectForm.getGaussianAngle();
            verticalWeight = autoDetectForm.getVerticalWeight();
            horizontalWeight = autoDetectForm.getHorizontalWeight();
            edgeLinkingThreshold = autoDetectForm.getEdgeLinkingThreshold();
            edgeLinkingDistance = autoDetectForm.getEdgeLinkingMaxDistance();
            edgeRemovalThreshold = autoDetectForm.getEdgeRemovalThreshold();
            edgeJoiningThreshold = autoDetectForm.getEdgeJoiningThreshold();
            layerSensitivity = autoDetectForm.GetLayerSensitivity();

            disableLayerDetection = autoDetectForm.GetDisableLayerDetection();
        }

        private void houghTransformToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (autoDetectRunning == false)
            {
                autoDetectType = "Hough";

                houghAutoDetectForm = new HoughTransformAutoDetectParametersForm(boreholeSectionImage.Width);

                /**if (cannyHigh != 0)
                {
                    houghAutoDetectForm.CannyLow = cannyLow;
                    houghAutoDetectForm.CannyHigh = cannyHigh;

                    houghAutoDetectForm.GaussianWidth = gaussianWidth;
                    houghAutoDetectForm.GaussianAngle = gaussianAngle;

                    houghAutoDetectForm.HorizontalWeight = horizontalWeight;
                    houghAutoDetectForm.VerticalWeight = verticalWeight;

                    houghAutoDetectForm.EdgeJoinBonus = edgeJoinBonus;
                    houghAutoDetectForm.EdgeLengthBonus = edgeJoinBonus;
                    houghAutoDetectForm.MaxLayerAmplitude = maxSineAmplitude;
                    houghAutoDetectForm.PeakThreshold = peakThreshold;
                }**/

                houghAutoDetectForm.SetStartDepth(_viewAdapter.BoreholeStartDepth);
                houghAutoDetectForm.SetEndDepth(_viewAdapter.BoreholeEndDepth);

                houghAutoDetectForm.SetDepthResolution(_viewAdapter.DepthResolution);

                houghAutoDetectForm.SetSectionStartDepth(_viewAdapter.GetCurrentSectionStart());
                houghAutoDetectForm.SetSectionEndDepth(_viewAdapter.GetCurrentSectionEnd());

                houghAutoDetectForm.SetViewingAreaStart(_viewAdapter.GetCurrentSectionStart() + boreholeImagePanel.VerticalScroll.Value);
                houghAutoDetectForm.SetViewingAreaEnd(_viewAdapter.GetCurrentSectionStart() + boreholeImagePanel.VerticalScroll.Value + boreholeImagePanel.Height);

                houghAutoDetectForm.SetSection();

                houghAutoDetectForm.ShowDialog();

                GetHoughAutoDetectValues();

                if (houghAutoDetectForm.Run == true)
                {
                    autoDetectRunning = true;

                    checkWhetherToRemoveEdges(houghAutoDetectForm.StartProcessingDepth, houghAutoDetectForm.EndProcessingDepth);

                    detectLayersToolStripMenuItem.Enabled = false;

                    EnableAutoDetectToolStripItems();

                    startThread();
                }

                houghAutoDetectForm.Dispose();
            }
        }

        private void GetHoughAutoDetectValues()
        {
            cannyLow = houghAutoDetectForm.CannyLow;
            cannyHigh = houghAutoDetectForm.CannyHigh;
            gaussianWidth = houghAutoDetectForm.GaussianWidth;
            gaussianAngle = houghAutoDetectForm.GaussianAngle;
            verticalWeight = houghAutoDetectForm.VerticalWeight;
            horizontalWeight = houghAutoDetectForm.HorizontalWeight;
            edgeJoinBonus = houghAutoDetectForm.EdgeJoinBonus;
            edgeLengthBonus = houghAutoDetectForm.EdgeLengthBonus;
            maxSineAmplitude = houghAutoDetectForm.MaxLayerAmplitude;
            peakThreshold = houghAutoDetectForm.PeakThreshold;

            layerSensitivity = houghAutoDetectForm.LayerSensitivity;

            disableLayerDetection = houghAutoDetectForm.DisableLayerDetection;
        }

        # endregion

        private void startThread()
        {
            // Initialize the object that the background worker calls.
            FileTiler tiler = _viewAdapter.getNewTiler(1000);

            if (autoDetectType.Equals("EdgeProcessing"))
            {
                AutomaticDetectUsingEdgeProcessing featureDetection = new AutomaticDetectUsingEdgeProcessing(tiler, this, _viewAdapter.ImageType);

                featureDetection.CannyLow = cannyLow;
                featureDetection.CannyHigh = cannyHigh;
                featureDetection.GaussianWidth = gaussianWidth;
                featureDetection.GaussianAngle = gaussianAngle;

                featureDetection.VerticalWeight = verticalWeight;
                featureDetection.HorizontalWeight = horizontalWeight;

                featureDetection.EdgeLinkingThreshold = edgeLinkingThreshold;
                featureDetection.EdgeLinkingDistance = edgeLinkingDistance;

                featureDetection.EdgeRemovalThreshold = edgeRemovalThreshold;
                featureDetection.EdgeJoiningThreshold = edgeJoiningThreshold;

                //featureDetection.SetMaxSineAmplitude(autoDetectForm.getMaxSineAmplitude());
                featureDetection.LayerSensitivity = layerSensitivity;

                featureDetection.DisableLayerDetection = disableLayerDetection;

                int startAnalysis = autoDetectForm.GetStartProcessingDepth();
                int endAnalysis = autoDetectForm.GetEndProcessingDepth();

                featureDetection.StartAnalysisDepth = startAnalysis;
                featureDetection.EndAnalysisDepth = endAnalysis;

                totalToAutoDetect = endAnalysis - startAnalysis;

                featureDetection.DrawTestImages = drawTestImages;

                // Start the asynchronous operation.
                //backgroundWorker1.RunWorkerAsync(featureDetection);
                autoDetectThread = new Thread(featureDetection.Detect);

            }
            else
            {
                AutomaticDetectUsingHough featureDetection = new AutomaticDetectUsingHough(tiler, this, _viewAdapter.ImageType);

                featureDetection.CannyLow = cannyLow;
                featureDetection.CannyHigh = cannyHigh;
                featureDetection.GaussianWidth = gaussianWidth;
                featureDetection.GaussianAngle = gaussianAngle;

                featureDetection.VerticalWeight = verticalWeight;
                featureDetection.HorizontalWeight = horizontalWeight;

                featureDetection.EdgeJoinBonus = edgeJoinBonus;
                featureDetection.EdgeLengthBonus = edgeLengthBonus;
                featureDetection.PeakThreshold = peakThreshold;
                featureDetection.MaxLayerAmplitude = maxSineAmplitude;

                //featureDetection.SetMaxSineAmplitude(autoDetectForm.getMaxSineAmplitude());
                featureDetection.LayerSensitivity = layerSensitivity;

                featureDetection.DisableLayerDetection = disableLayerDetection;

                int startAnalysis = houghAutoDetectForm.StartProcessingDepth;
                int endAnalysis = houghAutoDetectForm.EndProcessingDepth;

                featureDetection.StartAnalysisDepth = startAnalysis;
                featureDetection.EndAnalysisDepth = endAnalysis;

                totalToAutoDetect = endAnalysis - startAnalysis;

                featureDetection.DrawTestImages = drawTestImages;

                // Start the asynchronous operation.
                //backgroundWorker1.RunWorkerAsync(featureDetection);
                autoDetectThread = new Thread(featureDetection.Detect);
 
            }

            autoDetectThread.Start();
                
            
        }

        /// <summary>
        /// Asks user if they want to remove edges between two points and if so deletes them
        /// </summary>
        private void checkWhetherToRemoveEdges(int startDepth, int endDepth)
        {
            if (MessageBox.Show("Remove previously annotated layers?", "Remove Layers", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                _viewAdapter.DeSelectFeature();
                //_viewAdapter.deleteLayers();

                _viewAdapter.DeleteLayersInRange(startDepth, endDepth);
                boreholePictureBox.Refresh();
            }
        }

        private void AnnotationToolForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (autoDetectRunning)
            {
                StopEdgeDetection(false);
            }

            if (changedSinceLastSave == true)
            {
                showSaveOrExitDialog(e);
            }


        }

        private void showSaveOrExitDialog(FormClosingEventArgs e)
        {
            DialogResult result = MessageBox.Show("Would you like to save before quitting?", "Feature Annotation Tool", MessageBoxButtons.YesNoCancel);
            if (result == DialogResult.Yes)
            {
                //if (previouslySaved)
                Save();
                //else
                //    saveAs();

            }
            else if (result == DialogResult.No)
            {

            }
            else if (result == DialogResult.Cancel)
            {
                e.Cancel = true;
            }
        }

        private string showSaveOrExitDialog()
        {
            DialogResult result = MessageBox.Show("Would you like to save before quitting?", "Feature Annotation Tool", MessageBoxButtons.YesNoCancel);

            if (result == DialogResult.Yes)
            {
                Save();

            }
            else if (result == DialogResult.No)
            {

            }
            else if (result == DialogResult.Cancel)
            {
                return "Cancel";
            }


            return null;
        }

        public void openExistingFile()
        {
            try
            {
                ResetAll();

                OpenFileDialog openDialog = new OpenFileDialog();
                openDialog.DefaultExt = "*.features";
                openDialog.Filter = "Features Files|*.features";

                if (openDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    StartProgressReport("Opening Project");

                    DisableFeaturesButtons();
                    _viewAdapter.DeleteCurrentFeature();
                    _viewAdapter.DeSelectFeature();

                    _viewAdapter.LoadFeaturesFile(openDialog.FileName);
                    boreholePictureBox.Refresh();

                    EnableActiveBoreholeMenuItems();
                    //enableRotationButtons();

                    fileName = openDialog.FileName;

                    changedSinceLastSave = false;

                    ShowFeatureVisibilityItems(true);

                    if (autoDetectRunning)
                        StopEdgeDetection(false);

                    SetDetailsDialogTitle();

                    SetAvailableEdgeDetectionTypes();

                    EndProgressReport();
                }
            }
            catch (PathTooLongException ptle)
            {
                EndProgressReport();
                MessageBox.Show("Error - The file path is too long: " + ptle.Message, "File path too long exception");
                //resetAll();
            }
            catch (Exception e)
            {
                EndProgressReport();
                MessageBox.Show("Error creating project: " + e.Message, "Project creation error - 3");
            }
        }

        public void OpenImageFile()
        {
            try
            {
                ResetAll();

                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "Bitmap Images|*.bmp";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    Cursor.Current = Cursors.WaitCursor;

                    fileName = dialog.FileName.ToString();
                    boreholeName = dialog.SafeFileName.Remove(dialog.SafeFileName.Length - 4);

                    _viewAdapter.OpenBoreholeImageFile(fileName, boreholeName);

                    boreholePictureBox.Refresh();
                    changedSinceLastSave = false;

                    //showFeaturesCheckBox.Visible = true;

                    SetDetailsDialogTitle();

                    Cursor.Current = Cursors.Default;

                    SetAvailableEdgeDetectionTypes();
                }
            }
            catch (PathTooLongException ptle)
            {
                EndProgressReport();
                MessageBox.Show("Error - The file path is too long: " + ptle.Message, "File path too long exception");
                //resetAll();
            }
            catch (Exception e)
            {
                EndProgressReport();
                MessageBox.Show("Error creating project: " + e.Message, "Project creation error - 1");
            }
        }

        private void SetAvailableEdgeDetectionTypes()
        {
            if (_viewAdapter.ProjectType.Equals("Borehole"))
                houghTransformToolStripMenuItem.Enabled = true;
            else
                houghTransformToolStripMenuItem.Enabled = false;
        }

        public void OpenOTVFile()
        {
            try
            {
                ResetAll();

                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "HED file|*.HED";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    Cursor.Current = Cursors.WaitCursor;

                    fileName = dialog.FileName.ToString();
                    //boreholeName = dialog.SafeFileName.Remove(dialog.SafeFileName.Length - 4);

                    _viewAdapter.openOTVFile(fileName);

                    boreholePictureBox.Refresh();
                    changedSinceLastSave = false;

                    //showFeaturesCheckBox.Visible = true;

                    boreholeDetailsToolStripMenuItem.Text = "Borehole details";

                    SetAvailableEdgeDetectionTypes();

                    Cursor.Current = Cursors.Default;
                }
            }
            catch (PathTooLongException ptle)
            {
                EndProgressReport();
                MessageBox.Show("Error - The file path is too long: " + ptle.Message, "File path too long exception");
                //resetAll();
            }
            catch (Exception e)
            {
                EndProgressReport();
                MessageBox.Show("Error creating project: " + e.Message, "Project creation error - 2");
            }

            /*_viewAdapter.otvFileOpened();

            boreholePictureBox.Refresh();
            changedSinceLastSave = false;

            showFeaturesCheckBox.Visible = true;*/
        }

        # endregion

        # region enable/disable buttons

        /// <summary>
        /// Enables the menu items for an active opened borehole image
        /// </summary>
        private void EnableActiveBoreholeMenuItems()
        {
            closeToolStripMenuItem.Enabled = true;
            exportAsImageToolStripmenuItem.Enabled = true;

            //if (previouslySaved)
            //    saveToolStripButton.Enabled = true;
            //else
            //    saveToolStripButton.Enabled = false;

            //saveAsToolStripMenuItem.Enabled = true;

            saveToolStripButton.Enabled = true;
            saveToolStripMenuItem.Enabled = true;

            exportToExcelToolStripMenuItem.Enabled = true;

            featuresToolStripMenuItem.Enabled = true;
            toolsToolStripMenuItem.Enabled = true;
            automaticDetectionToolStripMenuItem.Enabled = true;
            
            ShowFeatureVisibilityItems(true);
        }

        /// <summary>
        /// Disables the menu items for an active opened borehole image
        /// </summary>
        private void DisableActiveBoreholeMenuItems()
        {
            closeToolStripMenuItem.Enabled = false;
            exportAsImageToolStripmenuItem.Enabled = false;

            saveToolStripButton.Enabled = false;
            saveToolStripMenuItem.Enabled = false;
            //saveAsToolStripMenuItem.Enabled = false;

            exportToExcelToolStripMenuItem.Enabled = false;

            featuresToolStripMenuItem.Enabled = false;
            toolsToolStripMenuItem.Enabled = false;
            automaticDetectionToolStripMenuItem.Enabled = false;

            ShowFeatureVisibilityItems(false);
        }

        /// <summary>
        /// Disables all buttons in the features tool strip except
        /// the 'add' buttons
        /// </summary>
        public void DisableFeaturesButtons()
        {
            layerThicknessToolStripButton.Enabled = false;
            alterLayerToolStripButton.Enabled = false;
            alterEdgeToolStripButton.Enabled = false;
            deleteLayerToolStripButton.Enabled = false;
            //addClusterToolStripButton.Enabled = false;
            editClusterToolStripButton.Enabled = false;
            addClusterPointToolStripButton.Enabled = false;
            removeClusterPointToolStripButton.Enabled = false;
            deleteClusterToolStripButton.Enabled = false;
            //addInclusionToolStripButton.Enabled = false;
            editInclusionToolStripButton.Enabled = false;
            addInclusionPointToolStripButton.Enabled = false;
            removeInclusionPointToolStripButton.Enabled = false;
            deleteInclusionToolStripButton.Enabled = false;
            //fluidLevelToolStripButton.Enabled = false;

            //Deselect all buttons
            addLayerToolStripButton.Checked = false;
            layerThicknessToolStripButton.Checked = false;
            alterLayerToolStripButton.Checked = false;
            alterEdgeToolStripButton.Checked = false;
            deleteLayerToolStripButton.Checked = false;
            addClusterToolStripButton.Checked = false;
            editClusterToolStripButton.Checked = false;
            addClusterPointToolStripButton.Checked = false;
            removeClusterPointToolStripButton.Checked = false;
            deleteClusterToolStripButton.Checked = false;
            addInclusionToolStripButton.Checked = false;
            editInclusionToolStripButton.Checked = false;
            addInclusionPointToolStripButton.Checked = false;
            removeInclusionPointToolStripButton.Checked = false;
            deleteInclusionToolStripButton.Checked = false;
            fluidLevelToolStripButton.Checked = false;
        }

        /// <summary>
        /// Enables all the layer buttons in the features toolstrip
        /// </summary>
        public void enableLayerButtons()
        {
            layerThicknessToolStripButton.Enabled = true;
            alterLayerToolStripButton.Enabled = true;
            alterEdgeToolStripButton.Enabled = true;
            deleteLayerToolStripButton.Enabled = true;
        }

        /// <summary>
        /// Enables all the cluster buttons in the features toolstrip
        /// </summary>
        public void EnableClusterButtons()
        {
            editClusterToolStripButton.Enabled = true;
            addClusterPointToolStripButton.Enabled = true;
            removeClusterPointToolStripButton.Enabled = true;
            deleteClusterToolStripButton.Enabled = true;
        }

        /// <summary>
        /// Enables all the inclusion buttons in the features toolstrip
        /// </summary>
        public void EnableInclusionButtons()
        {
            editInclusionToolStripButton.Enabled = true;
            addInclusionPointToolStripButton.Enabled = true;
            removeInclusionPointToolStripButton.Enabled = true;
            deleteInclusionToolStripButton.Enabled = true;
        }

        private void EnableRotationButtons()
        {
            rotateLeftButton.Enabled = true;
            rotateRightButton.Enabled = true;
            rotateLeftButton.Visible = true;
            rotateRightButton.Visible = true;
        }

        private void DisableRotationButtons()
        {
            rotateLeftButton.Enabled = false;
            rotateRightButton.Enabled = false;
            rotateLeftButton.Visible = false;
            rotateRightButton.Visible = false;
        }

        # endregion

        # region backgroundWorker events

        # endregion

        # region Previous/Next section methods

        private void previousSectionToolStripButton_Click(object sender, EventArgs e)
        {
            currentImageRotation = 0;
            
            _viewAdapter.LoadPreviousBoreholeSection();
            _viewAdapter.DeSelectFeature();

            boreholePictureBox.Refresh();

            fullPreviewPictureBox.Refresh();
        }

        private void nextSectionToolStripButton_Click(object sender, EventArgs e)
        {
            currentImageRotation = 0;

            _viewAdapter.LoadNextBoreholeSection();
            _viewAdapter.DeSelectFeature();
            
            boreholePictureBox.Refresh();

            fullPreviewPictureBox.Refresh();
        }

        private void firstSectionToolStripButton_Click(object sender, EventArgs e)
        {
            currentImageRotation = 0;
            
            _viewAdapter.LoadFirstBoreholeSection();
            _viewAdapter.DeSelectFeature();

            boreholePictureBox.Refresh();
            fullPreviewPictureBox.Refresh();
        }

        private void lastSectionToolStripButton_Click(object sender, EventArgs e)
        {
            currentImageRotation = 0;

            _viewAdapter.LoadLastBoreholeSection();
            _viewAdapter.DeSelectFeature();

            boreholePictureBox.Refresh();
            fullPreviewPictureBox.Refresh();
        } 

        public void EnableNextSectionButton()
        {
            nextSectionToolStripButton.Enabled = true;
        }

        public void DisableNextSectionButton()
        {
            nextSectionToolStripButton.Enabled = false;
        }

        public void EnablePreviousSectionButton()
        {
            previousSectionToolStripButton.Enabled = true;
        }

        public void DisablePreviousSectionButton()
        {
            previousSectionToolStripButton.Enabled = false;
        }

        public void DisableFirstSectionButton()
        {
            firstSectionToolStripButton.Enabled = false;
        }

        public void EnableFirstSectionButton()
        {
            firstSectionToolStripButton.Enabled = true;
        }

        public void DisableLastSectionButton()
        {
            lastSectionToolStripButton.Enabled = false;
        }

        public void EnableLastSectionButton()
        {
            lastSectionToolStripButton.Enabled = true;
        }

        # endregion

        # region Set methods

        public void SetTitleName(string boreholeName)
        {
            this.boreholeName = boreholeName;
            this.Text = "Borehole Feature Annotation Tool - " + boreholeName;
        }

        public void SetFileName(string fileName)
        {
            this.fileName = fileName;
        }

        public void SetBoreholeSectionsComboBoxItems(int boreholeStartDepth, int boreholeEndDepth)
        {
            int depthRes = _viewAdapter.DepthResolution;
            boreholeSectionToolStripComboBox.Items.Clear();

            decimal start = (decimal)boreholeStartDepth / (decimal)1000;
            decimal end = ((decimal)start + ((decimal)10 * (decimal)depthRes));

            decimal boreholeEnd = boreholeEndDepth / (decimal)1000;

            while (end < boreholeEnd)
            {

                string formattedStartDepth = start.ToString("N2");
                string formattedEndDepth = end.ToString("N2");

                boreholeSectionToolStripComboBox.Items.Add(formattedStartDepth + " - " + formattedEndDepth);

                start = end;
                end = start + ((decimal)10 * (decimal)depthRes);
            }

            if (start < boreholeEnd)
            {
                end = boreholeEnd;

                string formattedStartDepth = start.ToString("N2");
                string formattedEndDepth = end.ToString("N2");

                boreholeSectionToolStripComboBox.Items.Add(formattedStartDepth + " - " + formattedEndDepth);
            }
        }

        /// <summary>
        /// Sets the current section in the boreholeSectionToolStripComboBox
        /// </summary>
        /// <param name="currentSection"></param>
        public void SetCurrentSectionComboBox(int currentSection)
        {
            boreholeSectionToolStripComboBox.SelectedIndex = currentSection;
        }

        # endregion

        # region AutoDetect methods

        public void EnableAutoDetectToolStripItems()
        {
            autoDetectToolStripStatusLabel.Visible = true;
            autoDetectProgressToolStripStatusLabel.Text = "";
            autoDetectProgressToolStripStatusLabel.Visible = true;

            autoDetectProgressBar.Visible = true;
            stopAutoDetectionToolStripButton.Visible = true;

            toolStripStatusLabel.Visible = false;

        }

        public void DisableAutoDetectToolStripItems()
        {
            autoDetectToolStripStatusLabel.Visible = false;
            autoDetectProgressToolStripStatusLabel.Text = "";
            autoDetectProgressToolStripStatusLabel.Visible = false;

            autoDetectProgressBar.Visible = false;
            stopAutoDetectionToolStripButton.Visible = false;

            toolStripStatusLabel.Visible = true;

            detectLayersToolStripMenuItem.Enabled = true;
        }

        private void stopAutoDetectionToolStripButton_MouseEnter(object sender, EventArgs e)
        {
            stopAutoDetectionToolStripButton.BackColor = SystemColors.ControlDark;
        }

        private void stopAutoDetectionToolStripButton_MouseLeave(object sender, EventArgs e)
        {
            stopAutoDetectionToolStripButton.BackColor = SystemColors.Control;
        }

        private void stopAutoDetectionToolStripButton_Click(object sender, EventArgs e)
        {
            StopEdgeDetection(true);
        }

        # region AutoDetect Thread methods

        public void AddCompleteLayer(Layer newLayer)
        {
            _viewAdapter.AddCompleteLayer(newLayer);

            RefreshBoreholeImage();
            RefreshBoreholeImage();

            AutoSave();
        }

        public void UpdateProgress(int currentProgress, int totalProgress)
        {
            currentAutoDetectProgress = currentProgress;
            totalAutoDetectProgress = totalProgress;

            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(UpdateAutoDetectProgress));
            }
            else
            {
                UpdateAutoDetectProgress();
            }
        }

        private void UpdateAutoDetectProgress()
        {
            int startDepth = _viewAdapter.BoreholeStartDepth;
            int depthRes = _viewAdapter.DepthResolution;

            string start = (((double)startDepth + ((double)currentAutoDetectProgress * (double)depthRes)) / 1000.0).ToString("N2");
            string end = (((double)startDepth + ((double)totalAutoDetectProgress * (double)depthRes)) / (double)1000).ToString("N2");

            autoDetectProgressToolStripStatusLabel.Text = start + "/" + end;

            //autoDetectProgressBar.Value = (int)((100.0 / (double)totalAutoDetectProgress) * (double)currentAutoDetectProgress);
            autoDetectProgressBar.Value = (int)((100.0 / (double)totalToAutoDetect) * (totalToAutoDetect - ((double)(totalAutoDetectProgress - currentAutoDetectProgress))));

            RefreshBoreholeImage();

            //autoDetectProgressBar.Value = 
        }

        public void StopEdgeDetection(bool showMessage)
        {
            if (InvokeRequired)
            {
                autoDetectRunning = false;
                RefreshBoreholeImage();

                Invoke(new MethodInvoker(DisableAutoDetectToolStripItems));

                if (showMessage)
                    MessageBox.Show("Automatic feature detection complete.", "Automatic Detection");

                autoDetectThread.Abort();
            }
            else
            {
                autoDetectRunning = false;
                RefreshBoreholeImage();

                DisableAutoDetectToolStripItems();

                autoDetectThread.Abort();

                if (showMessage)
                    MessageBox.Show("Automatic feature detection cancelled.", "Automatic Detection");
            }
        }

        # endregion

        # endregion

        # region Borehole section methods

        private void BoreholeSectionToolStripComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (userClickedComboBox == true)
            {
                userClickedComboBox = false;
                currentImageRotation = 0;
                _viewAdapter.LoadSection(boreholeSectionToolStripComboBox.SelectedIndex);
                fullPreviewPictureBox.Refresh();
            }
        }

        private void BoreholeSectionToolStripComboBox_Click(object sender, EventArgs e)
        {
            userClickedComboBox = true;
        }

        private void BoreholeSectionToolStripComboBox_Leave(object sender, EventArgs e)
        {
            userClickedComboBox = false;
        }

        private void EnableSectionChangeObjects()
        {
            firstSectionToolStripButton.Visible = true;
            previousSectionToolStripButton.Visible = true;
            nextSectionToolStripButton.Visible = true;
            lastSectionToolStripButton.Visible = true;

            boreholeSectionToolStripComboBox.Visible = true;
            toolStripSeparator4.Visible = true;
        }

        private void DisableSectionChangeObjects()
        {
            firstSectionToolStripButton.Visible = false;
            previousSectionToolStripButton.Visible = false;
            nextSectionToolStripButton.Visible = false;
            lastSectionToolStripButton.Visible = false;
            
            boreholeSectionToolStripComboBox.Visible = false;
            toolStripSeparator4.Visible = false;
        }

        # endregion

        public void RefreshBoreholeImage()
        {
            if (boreholePictureBox.InvokeRequired)
            {
                boreholePictureBox.Invoke(new MethodInvoker(RefreshBoreholeImage));
            }
            else
            {
                boreholePictureBox.Refresh();
            }
        }

        # region Remove Features Methods

        private void removeAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _viewAdapter.DeSelectFeature();

            _viewAdapter.DeleteAllLayers();
            _viewAdapter.DeleteAllClusters();
            _viewAdapter.DeleteAllInclusions();
            
            _viewAdapter.RemoveFluidLevel();
            
            RefreshBoreholeImage();
            AutoSave();
        }

        private void removeAllInclusionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _viewAdapter.DeSelectFeature();

            _viewAdapter.DeleteAllInclusions();

            RefreshBoreholeImage();
            AutoSave();
        }

        private void removeAllClustersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _viewAdapter.DeSelectFeature();

            _viewAdapter.DeleteAllClusters();

            RefreshBoreholeImage();
            AutoSave();
        }

        private void removeAllLayersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _viewAdapter.DeSelectFeature();

            _viewAdapter.DeleteAllLayers();

            RefreshBoreholeImage();
            AutoSave();
        }

        private void removeFluidLevelToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            _viewAdapter.RemoveFluidLevel();
            RefreshBoreholeImage();
            AutoSave();
        }    

        # endregion

        # region Key Events

        private void AnnotationToolForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                _viewAdapter.DeleteCurrentFeature();
                AutoSave();
                changedSinceLastSave = true;

                creatingCluster = false;
                creatingInclusion = false;
                DisableFeaturesButtons();

                RefreshBoreholeImage();
            }
        }

        # endregion

        private void drawTestImagesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (drawTestImagesToolStripMenuItem.Checked == true)
            {
                drawTestImages = true;
            }
            else
                drawTestImages = false;
        }

        private void groupsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _viewAdapter.SetupLayerGroupsForm();
        }

        private void clustersToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            _viewAdapter.SetupClusterGroupsForm();
        }

        private void inclusionsToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            _viewAdapter.SetupInclusionGroupsForm();
        }  

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox about = new AboutBox();

            about.ShowDialog();
        }

        private void excelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                _viewAdapter.CreateDepthLuminosityProfileInExcel();
            }
            catch (PathTooLongException ptle)
            {
                EndProgressReport();
                MessageBox.Show("Error - The file path is too long: " + ptle.Message, "File path too long exception");
                //resetAll();
            }
            catch (Exception ex)
            {
                EndProgressReport();
                MessageBox.Show("Error creating profile: " + ex.Message, "Error");
            }
        }


        private void writeToTextFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                _viewAdapter.CreateDepthLuminosityProfileInText();
            }
            catch (PathTooLongException ptle)
            {
                EndProgressReport();
                MessageBox.Show("Error - The file path is too long: " + ptle.Message, "File path too long exception");
                //resetAll();
            }
            catch (Exception ex)
            {
                EndProgressReport();
                MessageBox.Show("Error creating profile: " + ex.Message, "Error");
            }
        }

        # region Progress Report Dialog

        public void StartProgressReport(string title)
        {
            progressBarTitle = title;
            //progressReportForm.Show();
            progressReportThread = new Thread(ShowProgressBar);
            progressReportThread.Start();
            //progressReportForm.Show();                
        }

        private void ShowProgressBar()
        {
            progressReportForm = new ProgressReportForm(progressBarTitle);

            progressReportForm.StartPosition = FormStartPosition.CenterParent;
            progressReportForm.ShowDialog();
        }

        public void UpdateProgress(int percentComplete)
        {

        }

        public void EndProgressReport()
        {
            progressReportThread.Abort();
        }

        # endregion

        # region Right-clcik on feature methods

        private void rightClickOnLayerContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            List<string> groups = _viewAdapter.GetLayerGroupsList();
            ToolStripMenuItem[] groupItems = new ToolStripMenuItem[groups.Count];

            for (int i = 0; i < groups.Count; i++)
            {
                groupItems[i] = new ToolStripMenuItem(groups[i]);

                groupItems[i].Click += new System.EventHandler(this.groupToolStripMenuItem_Click);

                if (_viewAdapter.GetCurrentFeaturesGroup() == groups[i])
                {
                    groupItems[i].Checked = true;
                }
            }

            selectGroupToolStripMenuItem.DropDownItems.Clear();

            selectGroupToolStripMenuItem.DropDownItems.AddRange(groupItems);
            selectGroupToolStripMenuItem.Name = "selectGroupToolStripMenuItem";
            selectGroupToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            selectGroupToolStripMenuItem.Text = "Select group";
        }

        private void rightClickOnLayerContextMenuStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Text == "Delete")
            {
                _viewAdapter.DeleteCurrentFeature();
                AutoSave();
                changedSinceLastSave = true;
            }
        }

        private void rightClickOnClusterContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            List<string> groups = _viewAdapter.GetClusterGroupsList();
            ToolStripMenuItem[] groupItems = new ToolStripMenuItem[groups.Count];

            for (int i = 0; i < groups.Count; i++)
            {
                groupItems[i] = new ToolStripMenuItem(groups[i]);

                groupItems[i].Click += new System.EventHandler(this.clusterGroupToolStripMenuItem_Click);

                if (_viewAdapter.GetCurrentFeaturesGroup() == groups[i])
                {
                    groupItems[i].Checked = true;
                }
            }

            selectClusterGroupToolStripMenuItem.DropDownItems.Clear();

            selectClusterGroupToolStripMenuItem.DropDownItems.AddRange(groupItems);
            selectClusterGroupToolStripMenuItem.Name = "selectGroupToolStripMenuItem";
            selectClusterGroupToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            selectClusterGroupToolStripMenuItem.Text = "Select group";
        }

        private void rightClickOnClusterContextMenuStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Text == "Delete")
            {
                _viewAdapter.DeleteCurrentFeature();
                AutoSave();
                changedSinceLastSave = true;
            }
        }

        private void rightClickOnInclusionContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            List<string> groups = _viewAdapter.GetInclusionGroupsList();
            ToolStripMenuItem[] groupItems = new ToolStripMenuItem[groups.Count];

            for (int i = 0; i < groups.Count; i++)
            {
                groupItems[i] = new ToolStripMenuItem(groups[i]);

                groupItems[i].Click += new System.EventHandler(this.inclusionGroupToolStripMenuItem_Click);

                if (_viewAdapter.GetCurrentFeaturesGroup() == groups[i])
                {
                    groupItems[i].Checked = true;
                }
            }

            selectInclusionGroupToolStripMenuItem.DropDownItems.Clear();

            selectInclusionGroupToolStripMenuItem.DropDownItems.AddRange(groupItems);
            selectInclusionGroupToolStripMenuItem.Name = "selectGroupToolStripMenuItem";
            selectInclusionGroupToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            selectInclusionGroupToolStripMenuItem.Text = "Select group";
        }

        private void rightClickOnInclusionContextMenuStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Text == "Delete")
            {
                _viewAdapter.DeleteCurrentFeature();
                AutoSave();
                changedSinceLastSave = true;
            }
        }

        # endregion

        private void groupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<string> groups = _viewAdapter.GetLayerGroupsList();

            //for (int i = 0; i < groups.Count; i++)
            //{
            if (groups.Contains(sender.ToString()))
            {
                _viewAdapter.ChangeSelectedFeaturesGroup(sender.ToString());
            }
            // }

            RefreshBoreholeImage();
        }

        private void clusterGroupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<string> groups = _viewAdapter.GetClusterGroupsList();

            //for (int i = 0; i < groups.Count; i++)
            //{
            if (groups.Contains(sender.ToString()))
            {
                _viewAdapter.ChangeSelectedFeaturesGroup(sender.ToString());
            }
            // }

            RefreshBoreholeImage();
        }

        private void inclusionGroupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<string> groups = _viewAdapter.GetInclusionGroupsList();

            //for (int i = 0; i < groups.Count; i++)
            //{
            if (groups.Contains(sender.ToString()))
            {
                _viewAdapter.ChangeSelectedFeaturesGroup(sender.ToString());
            }
            // }

            RefreshBoreholeImage();
        }

        private void boreholeDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DisplayDetailsForm boreholeDetailsForm = new DisplayDetailsForm();

            boreholeDetailsForm.SetImageType(_viewAdapter.ImageType);

            boreholeDetailsForm.SetBoreholeName(_viewAdapter.BoreholeName);
            boreholeDetailsForm.SetAzimuthResolution(_viewAdapter.AzimuthResolution);
            boreholeDetailsForm.SetDepthResolution(_viewAdapter.DepthResolution);
            boreholeDetailsForm.SetBoreholeDepth(_viewAdapter.BoreholeDepth);
            boreholeDetailsForm.SetStartDepth(_viewAdapter.BoreholeStartDepth);
            boreholeDetailsForm.SetEndDepth(_viewAdapter.BoreholeEndDepth);

            boreholeDetailsForm.ShowDialog();
        }

        private void viewHelpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Help.ShowHelp(this, HelpProvider.   
        }

        # region Options menu

        private void autoSaveEnabledToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (autoSaveEnabledToolStripMenuItem.Checked)
                autoSaveEnabled = true;
            else
                autoSaveEnabled = false;
        }

        # endregion        

        private void rotateLeftButton_Click(object sender, EventArgs e)
        {
            rotation += 90;

            if (rotation > 270)
                rotation = 0;

            rotationShift = (int)((double)rotation * ((double)boreholeWidth / (double)360));

            CheckImageRotation();
            DrawOrientationRuler();
        }

        private void rotateRightButton_Click(object sender, EventArgs e)
        {
            rotation -= 90;

            if (rotation < 0)
                rotation = 270;

            rotationShift = (int)((double)rotation * ((double)boreholeWidth / (double)360));

            CheckImageRotation();
            DrawOrientationRuler();
        }

        private void AutoConvergeInclusionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Inclusion selectedFeature = (Inclusion)_viewAdapter.SelectedFeature;

            Rectangle region;

            Bitmap subImage = GetInclusionBitmap(selectedFeature, out region);

            List<object> callbackItems = new List<object>();

            callbackItems.Add((object)selectedFeature);
            callbackItems.Add((object)subImage);
            callbackItems.Add((object)region);

            //Auto converge
            ThreadPool.QueueUserWorkItem(new WaitCallback(AutoConvergeInclusion), callbackItems);


            _viewAdapter.DeSelectFeature();
        }

        # region Remove Layers methods

        private void removeLayersLessThan4ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _viewAdapter.DeSelectFeature();
            _viewAdapter.RemoveLayersWithQualityLessThan(4);
        }

        private void removeLayersLessThan3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _viewAdapter.DeSelectFeature();
            _viewAdapter.RemoveLayersWithQualityLessThan(3);
        }

        private void removeLayersLessThan2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _viewAdapter.DeSelectFeature();
            _viewAdapter.RemoveLayersWithQualityLessThan(2);
        }

        # endregion 
    
        /// <summary>
        /// Shows or hides the export layers for wellCAD menu item
        /// </summary>
        /// <param name="show"></param>
        public void ShowWellCADExport(bool show)
        {
            exportLayersToWellCADFileToolStripMenuItem.Visible = show;
        }

        private void opacityTrackbar_Scroll(object sender, EventArgs e)
        {
            featureOpacity = opacityTrackbar.Value;

            if (opacityTrackbar.Value > 0 && showFeaturesCheckBox.Checked == false)
            {
                showFeaturesCheckBox.Checked = true;
            }

            RefreshBoreholeImage();
        }

        private void fullPreviewPictureBox_MouseClick(object sender, MouseEventArgs e)
        {       
            int yPositionInBorehole = (int)(((double)e.Y / (double)fullPreviewPictureBox.Height) * (double)_viewAdapter.BoreholeDepth);

            _viewAdapter.LoadSectionAt(yPositionInBorehole);

            fullPreviewPictureBox.Refresh();
        }
    }
}
