using System;
using System.Collections.Generic;
using System.Drawing;
using BoreholeFeatures;
using DrawEdges;
using DrawEdges.DrawEdgesFactory;
using EdgeDetector;
using EdgeFitting;
using LayerDetection;
using TwoStageHoughTransform;

namespace AutomaticFeatureDetection
{
    public class HoughLayerDetection
    {
        //parameters

        //Data
        private int m_ImageWidth;
        private int m_ImageHeight;
        private readonly int m_AzimuthResolution;
        private int[] m_ImageData;

        private readonly Bitmap m_OriginalImage;
        
        private List<Sine> m_DetectedFitSines = new List<Sine>();
        
        //Detected features
        private readonly LayerTypeSelector m_LayerTypeSelector = new LayerTypeSelector("Borehole");


        # region properties

        public List<Layer> DetectedLayers { get; private set; } = new List<Layer>();
        
        public int StartHeight { get; }

        public float CannyLow { get; set; } = 0.01f;

        public float CannyHigh { get; set; } = 0.016f;

        public int GaussianWidth { get; set; } = 20;

        public int GaussianAngle { get; set; } = 6;

        public float VerticalWeight { get; set; }

        public float HorizontalWeight { get; set; }

        public int EdgeJoinBonus { get; set; }

        public int EdgeLengthBonus { get; set; }

        public int MaxLayerAmplitude { get; set; }

        public int PeakThreshold { get; set; }

        public int LayerSensitivity { get; set; } = 1;

        public bool DisableLayerDetection { get; set; } = false;

        public string ImageType { get; set; } = "Borehole";

        public int DepthResolution { get; set; }

        # region Test properties

        public bool DrawTestImages { get; set; } = false;

        public Color TestDrawingBackgroundColour { get; set; }

        public Color TestDrawingEdgeColour { get; set; }

        public Color TestDrawingOverBackgroundColour { get; set; }

        public bool TestDrawMultiColouredEdges { get; set; } = false;

        # endregion

        # endregion

        /// <summary>
        /// Constructor method
        /// </summary>
        public HoughLayerDetection(Bitmap originalImage, int startHeight)
        {            
            m_OriginalImage = originalImage;
            StartHeight = startHeight;

            m_AzimuthResolution = originalImage.Width;
            DepthResolution = 1;

            SetUpImageData();
        }

        /// <summary>
        /// Sets up the imageData byte array from the given Bitmap
        /// </summary>
        private void SetUpImageData()
        {
            m_ImageData = BitmapConverter.GetRgbFromBitmap(m_OriginalImage);

            m_ImageHeight = m_OriginalImage.Height;
            m_ImageWidth = m_OriginalImage.Width;
        }

        /// <summary>
        /// Calls all other methods and runs the feature detection
        /// </summary>
        public void DetectFeatures()
        {
            bool[] cannyData = PerformCannyEdgeDetection();

            if (ImageType == "Borehole")
            {
                m_DetectedFitSines = PerformHoughTransform(cannyData);


                m_DetectedFitSines.Sort(new SineDepthComparer());

                if (DrawTestImages)
                {
                    DrawSinesImage drawSines = new DrawSinesImage(m_OriginalImage, m_DetectedFitSines);
                    drawSines.DrawnImage.Save("06 - Image with fit sines.bmp");
                }

                //Layer detection
                if (DisableLayerDetection == false)
                {
                    DetectLayers detectLayers = new DetectLayers(m_DetectedFitSines, m_OriginalImage, DepthResolution);

                    detectLayers.LayerBrightnessSensitivity = LayerSensitivity;

                    detectLayers.Run();

                    DetectedLayers = detectLayers.DetectedLayers;

                    //detectedLayers.Sort(new LayerDepthComparer());
                }
                else
                {
                    for (var sinePlace = 0; sinePlace < m_DetectedFitSines.Count; sinePlace++)
                    {
                        var currentSineToAdd = m_DetectedFitSines[sinePlace];

                        var currentLayerToAdd = m_LayerTypeSelector.setUpLayer(currentSineToAdd.Depth + StartHeight, currentSineToAdd.Amplitude, currentSineToAdd.Azimuth, currentSineToAdd.Depth + StartHeight, currentSineToAdd.Amplitude, currentSineToAdd.Azimuth, m_AzimuthResolution, DepthResolution);

                        DetectedLayers.Add(currentLayerToAdd);
                    }
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private List<Sine> PerformHoughTransform(bool[] cannyData)
        {
            var houghTransform =
                new HoughTransform(cannyData, m_ImageWidth, m_ImageHeight)
                {
                    EdgeJoinBonus = EdgeJoinBonus,
                    EdgeLengthBonus = EdgeLengthBonus,
                    MaxSineAmplitude = MaxLayerAmplitude,
                    PeakThreshold = PeakThreshold,
                    Testing = DrawTestImages
                };

            if (DrawTestImages)
            {
                houghTransform.SetOriginalTestImage(m_OriginalImage);
            }

            houghTransform.RunHoughTransform();

            return houghTransform.Sines;
        }

        private bool[] PerformCannyEdgeDetection()
        {
            CannyDetector detector = new CannyDetector(m_ImageData, m_ImageWidth, m_ImageHeight);

            detector.WrapHorizontally = true;
            detector.WrapVertically = true; 

            detector.LowThreshold = CannyLow;
            detector.HighThreshold = CannyHigh;
            detector.GaussianWidth = GaussianWidth;
            detector.GaussianSigma = GaussianAngle;

            detector.HorizontalWeight = HorizontalWeight;
            detector.VerticalWeight = VerticalWeight;

            

            detector.DetectEdges();

            bool[] edgeData = detector.GetEdgeData();

            if (DrawTestImages)
            {
                DrawEdgesImageFactory factory = new DrawEdgesImageFactory("Bool");
                DrawEdgesImage drawImage = factory.setUpDrawEdges(edgeData, m_ImageWidth, m_ImageHeight);

                drawImage.setBackgroundColour(TestDrawingBackgroundColour);
                drawImage.setEdgeColour(TestDrawingEdgeColour);
                drawImage.SetDrawMultiColouredEdges(TestDrawMultiColouredEdges);

                drawImage.drawEdgesImage();
                drawImage.getDrawnEdges().Save("01a - Canny(" + detector.LowThreshold + "," + detector.HighThreshold + "," + detector.GaussianWidth + ", " + (int)detector.GaussianSigma + ").bmp");

                drawImage.setEdgeColour(TestDrawingOverBackgroundColour);
                drawImage.drawEdgesOverBackgroundImage(m_OriginalImage);
                drawImage.getDrawnEdges().Save("01b - Canny over original(" + detector.LowThreshold + "," + detector.HighThreshold + "," + detector.GaussianWidth + ", " + (int)detector.GaussianSigma + ").bmp");
            }

            return edgeData;
        }
    }
}
