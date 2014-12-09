using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FeatureAnnotationTool.DialogBoxes
{
    public partial class HoughTransformAutoDetectParametersForm : Form
    {
        private bool advancedOptions = false;
        private bool runEdgeDetection = false;

        private int imageWidth;

        private int filterSize;

        private int processingStart, processingEnd;

        private float cannyLow, cannyHigh;
        private int gaussianWidth, gaussianAngle;

        private float verticalWeight, horizontalWeight;
        private int edgeJoinBonus, edgeLengthBonus, maxLayerAmplitude, peakThreshold;

        private int boreholeStartDepth, boreholeEndDepth;
        private int depthResolution;

        private int sectionStartDepth, sectionEndDepth;
        private int viewingAreaStart, viewingAreaEnd;

        private int layerSensitivity;

        private const int ADVANCED_HEIGHT = 720;
        private const int SIMPLE_HEIGHT = 510;


        # region Properties
        
        /// <summary>
        /// Returns whether to run the edge detection or not
        /// </summary>
        /// <returns></returns>
        public bool Run
        {
            get
            {
                return runEdgeDetection;
            }
        }

        public float CannyLow
        {
            get
            {
                if (!advancedOptions)
                    return 0.01f;
                else
                    return cannyLow;
            }
            set
            {
                cannyLow = value;
                setUpValues();
            }
        }

        public float CannyHigh
        {
            get
            {
                if (!advancedOptions)
                    return 0.02f;
                else
                    return cannyHigh;
            }
            set
            {
                cannyHigh = value;
                setUpValues();
            }
        }

        public int GaussianWidth
        {
            get
            {
                if (!advancedOptions)
                    return filterSize;
                else
                    return gaussianWidth;
            }
            set
            {
                gaussianWidth = value;
                setUpValues();
            }
        }

        public int GaussianAngle
        {
            get
            {
                if (!advancedOptions)
                    return (int)((double)filterSize / 3.0);
                else
                    return gaussianAngle;
            }
            set
            {
                gaussianAngle = value;
                setUpValues();
            }
        }

        public float VerticalWeight
        {
            get
            {
                return verticalWeight;
            }
            set
            {
                verticalWeight = value;
                setUpValues();
            }
        }

        public float HorizontalWeight
        {
            get
            {
                return horizontalWeight;
            }
            set
            {
                horizontalWeight = value;
                setUpValues();
            }
        }

        public int EdgeJoinBonus
        {
            get
            {
                return edgeJoinBonus;
            }
            set
            {
                edgeJoinBonus = value;
                setUpValues();
            }
        }

        public int EdgeLengthBonus
        {
            get
            {
                return edgeLengthBonus;
            }
            set
            {
                edgeLengthBonus = value;
                setUpValues();
            }
        }

        public int MaxLayerAmplitude
        {
            get
            {
                return maxLayerAmplitude;
            }
            set
            {
                maxLayerAmplitude = value;
                setUpValues();
            }
        }

        public int PeakThreshold
        {
            get
            {
                return peakThreshold;
            }
            set
            {
                peakThreshold = value;
                setUpValues();
            }
        }

        public int LayerSensitivity
        {
            get
            {
                return layerSensitivity;
            }
            set
            {
                layerSensitivity = value;
                setUpValues();

            }
        }

        public bool DisableLayerDetection
        {
            get
            {
                return disableLayerDetectionCheckBox.Checked;
            }
        }

        /// <summary>
        /// Returns the start processing depth in pixels
        /// </summary>
        /// <returns></returns>
        public int StartProcessingDepth
        {
            get
            {
                return (int)(((double)processingStart - (double)boreholeStartDepth) / (double)depthResolution);
            }
        }

        /// <summary>
        /// Returns the end processing depth in pixels
        /// </summary>
        /// <returns></returns>
        public int EndProcessingDepth
        {
            get
            {
                return (int)(((double)processingEnd - (double)boreholeStartDepth) / (double)depthResolution);
            }
        }

        # endregion

        # region Constructor

        public HoughTransformAutoDetectParametersForm(int imageWidth)
        {
            this.imageWidth = imageWidth;

            InitializeComponent();

            this.Height = SIMPLE_HEIGHT;

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

                simpleMaxLayerAmplitudeTrackBar.Value = maxLayerAmplitude;
                simpleMaxLayerAmplitudeTextBox.Text = maxLayerAmplitude.ToString();

                simplePeakThresholdTrackBar.Value = peakThreshold;
                simplePeakThresholdTextBox.Text = peakThreshold.ToString();
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

                advancedEdgeJoinBonusTrackBar.Value = edgeJoinBonus;
                advancedEdgeJoinBonusTextBox.Text = edgeJoinBonus.ToString();

                advancedEdgeLengthBonusTrackBar.Value = edgeLengthBonus;
                advancedEdgeLengthBonusTextBox.Text = edgeLengthBonus.ToString();

                advancedMaxLayerAmplitudeTrackBar.Value = maxLayerAmplitude;
                advancedMaxLayerAmplitudeTextBox.Text = maxLayerAmplitude.ToString();

                peakThresholdTrackBar.Value = peakThreshold;
                peakThresholdTextBox.Text = peakThreshold.ToString();
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
                this.Height = ADVANCED_HEIGHT;
                //simplifiedEdgeDetectionGroupBox.Location = new Point(12, 434);
                //processAreaGroupBox.Location = new Point(12, 391);

                advancedOptions = true;
                parametersTypeButton.Text = "Simplified Options...";

                //disableLayerDetectionCheckBox.Location = new Point(15, 529);
                checkIfAdvanced();
            }
            else
            {
                this.Height = SIMPLE_HEIGHT;
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


        private void simpleMaxLayerAmplitudeTrackBar_Scroll(object sender, EventArgs e)
        {
            maxLayerAmplitude = simpleMaxLayerAmplitudeTrackBar.Value;
            simpleMaxLayerAmplitudeTextBox.Text = maxLayerAmplitude.ToString();
        }

        private void simplePeakThresholdTrackBar_Scroll(object sender, EventArgs e)
        {
            peakThreshold = simplePeakThresholdTrackBar.Value;
            simplePeakThresholdTextBox.Text = peakThreshold.ToString();
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

        private void advancedEdgeJoinBonusTrackBar_Scroll(object sender, EventArgs e)
        {
            edgeJoinBonus = advancedEdgeJoinBonusTrackBar.Value;
            advancedEdgeJoinBonusTextBox.Text = edgeJoinBonus.ToString();
        }

        private void advancedEdgeLengthBonusTrackBar_Scroll(object sender, EventArgs e)
        {
            edgeLengthBonus = advancedEdgeLengthBonusTrackBar.Value;
            advancedEdgeLengthBonusTextBox.Text = edgeLengthBonus.ToString();
        }

        private void advancedMaxLayerAmplitudeTrackBar_Scroll(object sender, EventArgs e)
        {
            maxLayerAmplitude = advancedMaxLayerAmplitudeTrackBar.Value;
            advancedMaxLayerAmplitudeTextBox.Text = maxLayerAmplitude.ToString();
        }

        private void peakThresholdTrackBar_Scroll(object sender, EventArgs e)
        {
            peakThreshold = peakThresholdTrackBar.Value;
            peakThresholdTextBox.Text = peakThreshold.ToString();
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

            startTextBox.Text = ((double)processingStart / (double)1000).ToString("N2");

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
            endTextBox.Text = ((double)processingEnd / (double)1000).ToString("N2");

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
            viewingAreaStart = boreholeStartDepth + (viewingStart * depthResolution);
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

            edgeJoinBonus = 2;
            edgeLengthBonus = 1;
            maxLayerAmplitude = 30;
            peakThreshold = 250;

            layerSensitivity = 1;
        }

        # endregion

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

                if (!advancedOptions)
                {
                    writer.WriteLine("Canny Horizontal weighting = \"" + 0.0 + "\"");
                    writer.WriteLine("Canny Vertical weighting = \"" + 1.0 + "\"");
                }
                else
                {
                    writer.WriteLine("Canny Horizontal weighting = \"" + horizontalWeight + "\"");
                    writer.WriteLine("Canny Vertical weighting = \"" + verticalWeight + "\"");
                }

                if (!advancedOptions)
                {
                    writer.WriteLine("Edge join bonus = \"" + 3 + "\"");
                    writer.WriteLine("Edge length bonus = \"" + 2 + "\"");
                }
                else
                {
                    writer.WriteLine("Edge join bonus = \"" + edgeJoinBonus + "\"");
                    writer.WriteLine("Edge length bonus = \"" + edgeLengthBonus + "\"");
                }                 

                writer.WriteLine("Maximum layer amplitude = \"" + maxLayerAmplitude + "\"");

                if (!advancedOptions)
                    writer.WriteLine("Hough space peak threshold = \"" + 200 + "\"");
                else
                    writer.WriteLine("Hough space peak threshold = \"" + peakThreshold + "\"");

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

                        CalculateEdgeJoinBonus(sr.ReadLine());
                        CalculateEdgeLengthBonus(sr.ReadLine());

                        CalculateMaxLayerAmplitude(sr.ReadLine());
                        CalculatePeakThreshold(sr.ReadLine());
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

        private void CalculateEdgeJoinBonus(string line)
        {
            edgeJoinBonus = IntFromLine(line);

            advancedEdgeJoinBonusTrackBar.Value = edgeJoinBonus;
            advancedEdgeJoinBonusTextBox.Text = edgeJoinBonus.ToString();
        }

        private void CalculateEdgeLengthBonus(string line)
        {
            edgeLengthBonus = IntFromLine(line);

            advancedEdgeLengthBonusTrackBar.Value = edgeLengthBonus;
            advancedEdgeLengthBonusTextBox.Text = edgeLengthBonus.ToString();
        }

        private void CalculateMaxLayerAmplitude(string line)
        {
            maxLayerAmplitude = IntFromLine(line);

            advancedMaxLayerAmplitudeTrackBar.Value = maxLayerAmplitude;
            advancedMaxLayerAmplitudeTextBox.Text = maxLayerAmplitude.ToString();
        }

        private void CalculatePeakThreshold(string line)
        {
            peakThreshold = IntFromLine(line);

            peakThresholdTrackBar.Value = peakThreshold;
            peakThresholdTextBox.Text = peakThreshold.ToString();
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
                this.Height = 750;
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
