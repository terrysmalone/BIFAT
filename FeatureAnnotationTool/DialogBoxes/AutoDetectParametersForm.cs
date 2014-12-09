using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace FeatureAnnotationTool.DialogBoxes
{
    public partial class AutoDetectParametersForm : Form
    {
        private bool advancedOptions = false;
        private bool runEdgeDetection = false;

        private int imageWidth;

        private int filterSize;

        private int processingStart, processingEnd;

        private float cannyLow, cannyHigh;
        private int gaussianWidth, gaussianAngle;

        private float verticalWeight, horizontalWeight;
        public int edgeLinkingLength, edgeLinkingDistance, edgeRemovalThreshold, edgeJoiningThreshold;

        private int maxSineAmplitude;

        private int boreholeStartDepth, boreholeEndDepth;
        private int depthResolution;

        private int sectionStartDepth, sectionEndDepth;
        private int viewingAreaStart, viewingAreaEnd;

        private int layerSensitivity;
        
        # region Constructor

        public AutoDetectParametersForm(int imageWidth)
        {
            this.imageWidth = imageWidth;

            InitializeComponent();

            this.Height = 500;

            advancedOptions = false;
            SetDefaultValues();

            setUpValues();

            checkIfAdvanced();

            viewingAreaRadioButton.Checked = true;
        }

        # endregion

        # region Set up

        private void setUpValues()
        {
            //Simplified
            if (!advancedOptions)
            {
                filterSize = gaussianWidth;

                filterSizeTrackBar.Value = filterSize;
                filterSizeTextBox.Text = filterSize.ToString();
                
                simpleRemoveEdgesTrackBar.Maximum = imageWidth;
                simpleRemoveEdgesTrackBar.Value = edgeRemovalThreshold;
                simpleRemoveEdgesTextBox.Text = edgeRemovalThreshold.ToString();

                simpleEdgeLinkingTrackBar.Maximum = imageWidth;
                simpleEdgeLinkingTrackBar.Value = edgeLinkingLength;
                simpleEdgeLinkingTextBox.Text = edgeLinkingLength.ToString();


                //maximumAmplitudeTrackBar.Value = maxSineAmplitude;
                //maxAmplitudeTextBox.Text = maxSineAmplitude.ToString();
            }
            else
            {
                //Advanced
                if (cannyLow * 100.0f >= cannyLowTrackBar.Maximum)
                    cannyLowTrackBar.Value = 999;
                else
                    cannyLowTrackBar.Value = (int)(cannyLow * 100.0f) + 1;

                cannyLowTextBox.Text = cannyLow.ToString();

                if (cannyHigh * 100.0f >= cannyHighTrackBar.Maximum)
                    cannyHighTrackBar.Value = 1000;
                else
                    cannyHighTrackBar.Value = (int)(cannyHigh * 100.0f) + 1;

                cannyHighTextBox.Text = cannyHigh.ToString();
                gaussianWidthTrackBar.Value = gaussianWidth;
                gaussianWidthTextBox.Text = gaussianWidth.ToString();

                gaussianAngle = (int)((double)filterSize / 3.0);
                gaussianAngleTrackBar.Value = gaussianAngle;
                gaussianAngleTextBox.Text = gaussianAngle.ToString();

                verticalWeightTrackBar.Value = (int)(verticalWeight * 100.0f);
                verticalWeightTextBox.Text = verticalWeight.ToString();
                horizontalWeightTrackBar.Value = (int)(horizontalWeight * 100.0f);
                horizontalWeightTextBox.Text = horizontalWeight.ToString();

                advancedEdgeLinkingTrackBar.Maximum = imageWidth;
                advancedEdgeLinkingTrackBar.Value = edgeLinkingLength;
                advancedEdgeLinkingTextBox.Text = edgeLinkingLength.ToString();

                advancedEdgeLinkingDistanceTrackBar.Value = edgeLinkingDistance;
                advancedEdgeLinkingDistanceTextBox.Text = edgeLinkingDistance.ToString();

                advancedEdgeRemovalTrackBar.Maximum = imageWidth;
                advancedEdgeRemovalTrackBar.Value = edgeRemovalThreshold;
                advancedEdgeRemovalTextBox.Text = edgeRemovalThreshold.ToString();

                joinEdgesTrackBar.Value = edgeJoiningThreshold;
                joinEdgesTextBox.Text = edgeJoiningThreshold.ToString();
            }

            layerBrightnessSensitivityTrackBar.Value = layerSensitivity;
            layerBrightnessSensitivityTextBox.Text = layerBrightnessSensitivityTrackBar.Value.ToString();
        }
        
        # endregion

        private void runButton_Click(object sender, EventArgs e)
        {
            runEdgeDetection = true;

            this.Visible = false;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            runEdgeDetection = false;

            this.Visible = false;
        }

        # region Simple or Advanced options

        private void parametersTypeButton_Click(object sender, EventArgs e)
        {
            //setDefaults();
            
            if (advancedOptions == false)
            {
                this.Height = 720;
                //simplifiedEdgeDetectionGroupBox.Location = new Point(12, 434);
                //processAreaGroupBox.Location = new Point(12, 391);

                advancedOptions = true;
                parametersTypeButton.Text = "Simplified Options...";

                //disableLayerDetectionCheckBox.Location = new Point(15, 529);
                checkIfAdvanced();
            }
            else
            {
                this.Height = 500;
                //simplifiedEdgeDetectionGroupBox.Location = new Point(12, 207);
                //processAreaGroupBox.Location = new Point(12, 207);

                advancedOptions = false;
                parametersTypeButton.Text = "Advanced Options...";

                //disableLayerDetectionCheckBox.Location = new Point(15, 429);

                checkIfAdvanced();
            }

            setUpValues();
        }

        private void checkIfAdvanced()
        {
            if (advancedOptions)
            {
                hideSimpleOptions();
                showAdvancedOptions();
            }
            else
            {
                showSimpleOptions();
                hideAdvancedOptions();
            }
        }
                
        private void hideSimpleOptions()
        {
            simplifiedCannyDetectorGroupBox.Visible = false;
            simplifiedEdgeProcessingGroupBox.Visible = false;
            //simplifiedEdgeDetectionGroupBox.Visible = false;
        }

        private void showSimpleOptions()
        {
            simplifiedCannyDetectorGroupBox.Visible = true;
            simplifiedEdgeProcessingGroupBox.Visible = true;
            //simplifiedEdgeDetectionGroupBox.Visible = true;
        }
        
        private void hideAdvancedOptions()
        {
            advancedCannyDetectorGroupBox.Visible = false;
            advancedEdgeProcessingGroupBox.Visible = false;
        }
        
        private void showAdvancedOptions()
        {
            advancedCannyDetectorGroupBox.Visible = true;
            advancedEdgeProcessingGroupBox.Visible = true;
        }
        
        # endregion
                
        # region Simple Trackbars

        private void filterSizeTrackBar_Scroll(object sender, EventArgs e)
        {
            filterSize = filterSizeTrackBar.Value;
            filterSizeTextBox.Text = filterSize.ToString();

            gaussianWidth = filterSize;
            gaussianAngle = (int)((double)filterSize / 3.0);
        }

        private void simpleLinkEdgesTrackBar_Scroll(object sender, EventArgs e)
        {
            edgeLinkingLength = simpleEdgeLinkingTrackBar.Value;
            simpleEdgeLinkingTextBox.Text = edgeLinkingLength.ToString();
        }

        private void simpleRemoveEdgesTrackBar_Scroll(object sender, EventArgs e)
        {
            edgeRemovalThreshold = simpleRemoveEdgesTrackBar.Value;
            simpleRemoveEdgesTextBox.Text = edgeRemovalThreshold.ToString();
        }

        # endregion

        # region Advanced Trackbars

        private void gaussianWidthTrackBar_Scroll(object sender, EventArgs e)
        {
            gaussianWidth = gaussianWidthTrackBar.Value;
            gaussianWidthTextBox.Text = gaussianWidth.ToString();

            filterSize = gaussianWidth;
            gaussianAngle = (int)((double)filterSize / 3.0);
        }

        private void gaussianAngleTrackBar_Scroll(object sender, EventArgs e)
        {
            gaussianAngle = gaussianAngleTrackBar.Value;
            gaussianAngleTextBox.Text = gaussianAngle.ToString();
        }

        private void cannyLowTrackBar_Scroll(object sender, EventArgs e)
        {
            cannyLow = cannyLowTrackBar.Value / 100.0f;

            if (cannyLowTrackBar.Value >= cannyHighTrackBar.Value)
            {
                cannyHighTrackBar.Value = cannyLowTrackBar.Value + 1;
                cannyHigh = cannyHighTrackBar.Value / 100.0f;
                cannyHighTextBox.Text = cannyHigh.ToString();
            }

            cannyLowTextBox.Text = cannyLow.ToString();
        }

        private void cannyHighTrackBar_Scroll(object sender, EventArgs e)
        {
            cannyHigh = cannyHighTrackBar.Value / 100.0f;

            if (cannyHighTrackBar.Value <= cannyLowTrackBar.Value)
            {
                cannyLowTrackBar.Value = cannyHighTrackBar.Value - 1;
                cannyLow = cannyLowTrackBar.Value / 100.0f;
                cannyLowTextBox.Text = cannyLow.ToString();
            }

            cannyHighTextBox.Text = cannyHigh.ToString();
        }

        private void horizontalWeightTrackBar_Scroll(object sender, EventArgs e)
        {
            horizontalWeight = horizontalWeightTrackBar.Value / 100.0f;
            horizontalWeightTextBox.Text = horizontalWeight.ToString();
        }

        private void verticalWeightTrackBar_Scroll(object sender, EventArgs e)
        {
            verticalWeight = verticalWeightTrackBar.Value / 100.0f;
            verticalWeightTextBox.Text = verticalWeight.ToString();
        }

        private void advancedEdgeLinkingTrackBar_Scroll(object sender, EventArgs e)
        {
            edgeLinkingLength = advancedEdgeLinkingTrackBar.Value;
            advancedEdgeLinkingTextBox.Text = edgeLinkingLength.ToString();
        }

        private void advancedEdgeLinkingDistanceTrackBar_Scroll(object sender, EventArgs e)
        {
            edgeLinkingDistance = advancedEdgeLinkingDistanceTrackBar.Value;
            advancedEdgeLinkingDistanceTextBox.Text = edgeLinkingDistance.ToString();
        }

        private void advancedEdgeRemovalTrackBar_Scroll(object sender, EventArgs e)
        {
            edgeRemovalThreshold = advancedEdgeRemovalTrackBar.Value;
            advancedEdgeRemovalTextBox.Text = edgeRemovalThreshold.ToString();
        }

        private void joinEdgesTrackBar_Scroll(object sender, EventArgs e)
        {
            edgeJoiningThreshold = joinEdgesTrackBar.Value;
            joinEdgesTextBox.Text = edgeJoiningThreshold.ToString();
        }

        # endregion

        # region scroll bars

        private void layerBrightnessSensitivityTrackBar_Scroll(object sender, EventArgs e)
        {
            layerSensitivity = layerBrightnessSensitivityTrackBar.Value;

            layerBrightnessSensitivityTextBox.Text = layerSensitivity.ToString();
        }

        private void startTrackBar_Scroll(object sender, EventArgs e)
        {
            processingStart = startTrackBar.Value;
            
            startTextBox.Text = ((double)processingStart/(double)1000).ToString("N2");

            if (processingStart >= endTrackBar.Value)
            {
                endTrackBar.Value = processingStart;
                processingEnd = endTrackBar.Value;
                endTextBox.Text = ((double)processingEnd / (double)1000).ToString("N2");
            }
        }

        private void endTrackBar_Scroll(object sender, EventArgs e)
        {
            processingEnd = endTrackBar.Value;
            endTextBox.Text = ((double)processingEnd/(double)1000).ToString("N2");

            if (processingEnd <= startTrackBar.Value)
            {
                startTrackBar.Value = processingEnd;
                processingStart = startTrackBar.Value;
                startTextBox.Text = ((double)processingStart / (double)1000).ToString("N2");
            }
        }

        # endregion

        # region Set method

        public void SetStartDepth(int boreholeStartDepth)
        {
            this.boreholeStartDepth = boreholeStartDepth;
            processingStart = boreholeStartDepth;
            //setUpValues();
        }

        public void SetEndDepth(int boreholeEndDepth)
        {
            this.boreholeEndDepth = boreholeEndDepth;
            processingEnd = boreholeEndDepth;
            //setUpValues();
        }

        public void SetDepthResolution(int depthResolution)
        {
            this.depthResolution = depthResolution;
        }

        public void SetSectionStartDepth(int sectionStartDepth)
        {
            this.sectionStartDepth = boreholeStartDepth + (sectionStartDepth * depthResolution);
        }

        public void SetSectionEndDepth(int sectionEndDepth)
        {
            this.sectionEndDepth = boreholeStartDepth + (sectionEndDepth * depthResolution);
        }

        public void SetViewingAreaStart(int viewingStart)
        {
            viewingAreaStart = boreholeStartDepth + (viewingStart*depthResolution);
            processingStart = viewingAreaStart;            
        }

        public void SetViewingAreaEnd(int viewingEnd)
        {
            viewingAreaEnd = boreholeStartDepth + (viewingEnd * depthResolution);
            processingEnd = viewingAreaEnd;
        }

        /// <summary>
        /// Sets the initial section to be analysed
        /// </summary>
        public void SetSection()
        {
            startTrackBar.Minimum = boreholeStartDepth;
            startTrackBar.Maximum = boreholeEndDepth;

            endTrackBar.Minimum = boreholeStartDepth;
            endTrackBar.Maximum = boreholeEndDepth;
            
            startTrackBar.Value = processingStart;
            startTextBox.Text = ((double)processingStart / (double)1000).ToString("N2");

            if (processingEnd > endTrackBar.Maximum)
                processingEnd = endTrackBar.Maximum;

            endTrackBar.Value = processingEnd;

            endTextBox.Text = ((double)processingEnd / (double)1000).ToString("N2");
        }

        # region Set paramaters methods

        public void SetDefaultValues()
        {
            filterSize = 20;

            cannyLow = 0.01f;
            cannyHigh = 0.02f;
            gaussianWidth = 20;
            gaussianAngle = 7;

            verticalWeight = 1.0f;
            horizontalWeight = 0.0f;

            edgeLinkingLength = (int)((double)imageWidth / (double)3.0);
            edgeLinkingDistance = (int)((double)(imageWidth / 360.0) * 7.0);
            edgeRemovalThreshold = (int)((double)imageWidth / (double)2.0);
            edgeJoiningThreshold = (int)((double)(imageWidth / 360.0) * 10.0);

            maxSineAmplitude = 30;

            layerSensitivity = 1;
        }

        internal void SetEdgeJoiningDistance(int edgeJoiningThreshold)
        {
            this.edgeJoiningThreshold = edgeJoiningThreshold;
            setUpValues();
        }

        internal void SetEdgeRemovalThreshold(int edgeRemovalThreshold)
        {
            this.edgeRemovalThreshold = edgeRemovalThreshold;
            setUpValues();
        }

        internal void SetEdgeLinkingDistance(int edgeLinkingDistance)
        {
            this.edgeLinkingDistance = edgeLinkingDistance;
            setUpValues();
        }

        internal void SetEdgeLinkingThreshold(int edgeLinkingThreshold)
        {
            this.edgeLinkingLength = edgeLinkingThreshold;
            setUpValues();
        }

        internal void SetVerticalWeight(float verticalWeight)
        {
            this.verticalWeight = verticalWeight;
            setUpValues();
        }

        internal void SetHorizontalWeight(float horizontalWeight)
        {
            this.horizontalWeight = horizontalWeight;
            setUpValues();
        }

        internal void SetGaussianSigma(int gaussianAngle)
        {
            this.gaussianAngle = gaussianAngle;
            setUpValues();
        }

        internal void SetGaussianWidth(int gaussianWidth)
        {
            this.gaussianWidth = gaussianWidth;
            filterSize = gaussianWidth;

            setUpValues();
        }

        internal void SetCannyHigh(float cannyHigh)
        {
            this.cannyHigh = cannyHigh;
            setUpValues();
        }

        internal void SetCannyLow(float cannyLow)
        {
            this.cannyLow = cannyLow;
            setUpValues();
        }

        internal void SetLayerSensitivity(int layerSensitivity)
        {
            this.layerSensitivity = layerSensitivity;
            setUpValues();
        }

        # endregion

        # endregion

        # region Get methods

        /// <summary>
        /// Returns whether to run the edge detection or not
        /// </summary>
        /// <returns></returns>
        public bool getRun()
        {
            return runEdgeDetection;
        }

        public float getCannyLow()
        {
            if (!advancedOptions)
                return 0.01f;
            else
                return cannyLow;
        }

        public float getCannyHigh()
        {
            if (!advancedOptions)
                return 0.02f;
            else
                return cannyHigh;
        }

        public int getGaussianWidth()
        {
            if (!advancedOptions)
                return filterSize;
            else
                return gaussianWidth;
        }

        public int getGaussianAngle()
        {
            if (!advancedOptions)
                return (int)((double)filterSize / 3.0);
            else
                return gaussianAngle;
        }

        public float getVerticalWeight()
        {
            return verticalWeight;
        }

        public float getHorizontalWeight()
        {
            return horizontalWeight;
        }

        public int getEdgeLinkingThreshold()
        {
            return edgeLinkingLength;
        }
        
        public int getEdgeLinkingMaxDistance()
        {
            if (!advancedOptions)
                return (int)((double)(imageWidth / 360.0) * 7.0);
            else
                return edgeLinkingDistance;
        }

        public int getEdgeRemovalThreshold()
        {
            return edgeRemovalThreshold;
        }

        public int getEdgeJoiningThreshold()
        {
            if (!advancedOptions)
                return edgeJoiningThreshold = (int)((double)(imageWidth / 360.0) * 10.0);
            else
                return edgeJoiningThreshold;
        }

        public int getMaxSineAmplitude()
        {
            return maxSineAmplitude;
        }

        public int GetLayerSensitivity()
        {
            return layerSensitivity;
        }

        public bool GetDisableLayerDetection()
        {
            return disableLayerDetectionCheckBox.Checked;
        }

        /// <summary>
        /// Returns the start processing depth in pixels
        /// </summary>
        /// <returns></returns>
        public int GetStartProcessingDepth()
        {
            return (int)(((double)processingStart - (double)boreholeStartDepth)/(double)depthResolution);
        }

        /// <summary>
        /// Returns the end processing depth in pixels
        /// </summary>
        /// <returns></returns>
        public int GetEndProcessingDepth()
        {
            return (int)(((double)processingEnd - (double)boreholeStartDepth ) / (double)depthResolution);
        }

        # endregion   

        # region Radio buttons

        private void wholeBoreholeRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            processingStart = boreholeStartDepth;
            processingEnd = boreholeEndDepth;

            SetSection();
        }

        private void currentSectionRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            processingStart = sectionStartDepth;
            processingEnd = sectionEndDepth;

            SetSection();
        }

        private void viewingAreaRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            processingStart = viewingAreaStart;
            processingEnd = viewingAreaEnd;

            SetSection();
        }

        # endregion        

        # region Save/Load parameters methods

        private void saveParametersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();

            saveDialog.Filter = "Edge detection parameter file (*.edp)|*.edp";

            saveDialog.Title = "Save parameters to text file";

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                FileStream fs = new FileStream(saveDialog.FileName, FileMode.Create);

                StreamWriter writer = new StreamWriter(fs);

                if (!advancedOptions)
                {
                    writer.WriteLine("Canny low threshold = \"" + 0.01 + "\"");
                    writer.WriteLine("Canny high threshold = \"" + 0.02 + "\"");
                }
                else
                {
                    writer.WriteLine("Canny low threshold = \"" + cannyLow + "\"");
                    writer.WriteLine("Canny high threshold = \"" + cannyHigh + "\"");
                }

                writer.WriteLine("Canny gaussian width = \"" + gaussianWidth + "\"");
                writer.WriteLine("Canny gaussian sigma value = \"" + gaussianAngle + "\"");

                writer.WriteLine("Canny Horizontal weighting = \"" + horizontalWeight + "\"");
                writer.WriteLine("Canny Vertical weighting = \"" + verticalWeight + "\"");

                writer.WriteLine("Edge linking threshold = \"" + edgeLinkingLength + "\"");

                if (!advancedOptions)
                    writer.WriteLine("Edge linking distance = \"" + (int)((double)(imageWidth / 360.0) * 7.0) + "\"");
                else
                    writer.WriteLine("Edge linking distance = \"" + edgeLinkingDistance + "\"");

                writer.WriteLine("Edge removal threshold = \"" + edgeRemovalThreshold + "\"");

                if (!advancedOptions)
                    writer.WriteLine("Edge joining distance = \"" + (int)((double)(imageWidth / 360.0) * 10.0) + "\"");
                else
                    writer.WriteLine("Edge joining distance = \"" + edgeJoiningThreshold + "\"");

                writer.Close();
                fs.Close();
            }
        }

        private void loadParametersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Filter = "Edge detection parameter file (*.edp)|*.edp";

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                SwitchToAdvancedMode();

                try
                {
                    using (StreamReader sr = new StreamReader(openFileDialog.FileName))
                    {
                        CalculateCannyLow(sr.ReadLine());
                        CalculateCannyHigh(sr.ReadLine());

                        CalculateGaussianWidth(sr.ReadLine());
                        CalculateGaussianSigma(sr.ReadLine());

                        CalculateHorizontalWeight(sr.ReadLine());
                        CalculateVerticalWeight(sr.ReadLine());

                        CalculateEdgeLinkingThreshold(sr.ReadLine());
                        CalculateEdgeLinkingDistance(sr.ReadLine());

                        CalculateEdgeRemovalThreshold(sr.ReadLine());
                        CalculateEdgeJoiningDistance(sr.ReadLine());
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("The file could not be read:");
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private void CalculateCannyLow(string line)
        {
            cannyLow = FloatFromLine(line);

            if (cannyLow * 100.0f >= cannyLowTrackBar.Maximum)
                cannyLowTrackBar.Value = 999;
            else
                cannyLowTrackBar.Value = (int)(cannyLow * 100.0f) + 1;

            cannyLowTextBox.Text = cannyLow.ToString();
        }

        private void CalculateCannyHigh(string line)
        {
            cannyHigh = FloatFromLine(line);

            if (cannyHigh * 100.0f >= cannyHighTrackBar.Maximum)
                cannyHighTrackBar.Value = 1000;
            else
                cannyHighTrackBar.Value = (int)(cannyHigh * 100.0f) + 1;

            cannyHighTextBox.Text = cannyHigh.ToString();
        }

        private void CalculateGaussianWidth(string line)
        {
            gaussianWidth = IntFromLine(line);

            gaussianWidthTrackBar.Value = gaussianWidth;
            gaussianWidthTextBox.Text = gaussianWidth.ToString();
        }

        private void CalculateGaussianSigma(string line)
        {
            gaussianAngle = IntFromLine(line);

            gaussianAngleTrackBar.Value = gaussianAngle;
            gaussianAngleTextBox.Text = gaussianAngle.ToString();
        }

        private void CalculateHorizontalWeight(string line)
        {
            horizontalWeight = IntFromLine(line);

            horizontalWeightTrackBar.Value = (int)(horizontalWeight * 100.0f);
            horizontalWeightTextBox.Text = horizontalWeight.ToString();
        }

        private void CalculateVerticalWeight(string line)
        {
            verticalWeight = IntFromLine(line);

            verticalWeightTrackBar.Value = (int)(verticalWeight * 100.0f);
            verticalWeightTextBox.Text = verticalWeight.ToString();               
        }

        private void CalculateEdgeLinkingThreshold(string line)
        {
            edgeLinkingLength = IntFromLine(line);

            advancedEdgeLinkingTrackBar.Value = edgeLinkingLength;
            advancedEdgeLinkingTextBox.Text = edgeLinkingLength.ToString();
        }

        private void CalculateEdgeLinkingDistance(string line)
        {
            edgeLinkingDistance = IntFromLine(line);

            advancedEdgeLinkingDistanceTrackBar.Value = edgeLinkingDistance;
            advancedEdgeLinkingDistanceTextBox.Text = edgeLinkingDistance.ToString();
        }

        private void CalculateEdgeRemovalThreshold(string line)
        {
            edgeRemovalThreshold = IntFromLine(line);

            advancedEdgeRemovalTrackBar.Value = edgeRemovalThreshold;
            advancedEdgeRemovalTextBox.Text = edgeRemovalThreshold.ToString();
        }

        private void CalculateEdgeJoiningDistance(string line)
        {
            edgeJoiningThreshold = IntFromLine(line);

            joinEdgesTrackBar.Value = edgeJoiningThreshold;
            joinEdgesTextBox.Text = edgeJoiningThreshold.ToString();
        }

        private float FloatFromLine(string line)
        {
            char[] delim = { '"' };
            string[] strArr;
            strArr = line.Split(delim);

            return float.Parse(strArr[1]);
        }

        private int IntFromLine(string line)
        {
            char[] delim = { '"' };
            string[] strArr;
            strArr = line.Split(delim);

            return Int32.Parse(strArr[1]);
        }

        private void SwitchToAdvancedMode()
        {
            if (advancedOptions == false)
            {
                this.Height = 720;
                //simplifiedEdgeDetectionGroupBox.Location = new Point(12, 434);
                //processAreaGroupBox.Location = new Point(12, 391);

                advancedOptions = true;
                parametersTypeButton.Text = "Simplified Options...";

                //disableLayerDetectionCheckBox.Location = new Point(15, 529);
                checkIfAdvanced();
            }
        }

        # endregion                
    }
}
