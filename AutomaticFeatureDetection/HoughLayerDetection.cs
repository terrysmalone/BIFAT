using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using BoreholeFeatures;
using DrawEdges;
using DrawEdges.DrawEdgesFactory;
using EdgeDetector;
using EdgeFitting;
using Edges;
using LayerDetection;
using TwoStageHoughTransform;

namespace AutomaticFeatureDetection
{
    public class HoughLayerDetection
    {
        //parameters
        private float cannyLowThreshold = 0.01f;
        private float cannyHighThreshold = 0.016f;
        private int gaussianWidth = 20;
        private int gaussianAngle = 6;

        private float verticalWeight, horizontalWeight;

        private int edgeJoinBonus, edgeLengthBonus, maxLayerAmplitude, peakThreshold;
        
        private int layerSensitivity = 1;
        
        //Data
        private int imageWidth;
        private int imageHeight;
        private int azimuthResolution, depthResolution;
        private int[] imageData;

        private Bitmap originalImage;
        
        private List<Sine> detectedFitSines = new List<Sine>();
        private List<EdgeLine> detectedFitLines = new List<EdgeLine>();
        
        //Detected features
        private List<Layer> detectedLayers = new List<Layer>();

        private int startHeight;

        private bool drawTestImages = false;
        Color testDrawingBackgroundColour, testDrawingEdgeColour, testDrawingOverBackgroundColour;

        private LayerTypeSelector layerTypeSelector = new LayerTypeSelector("Borehole");
        
        private bool disableLayerDetection = false;

        private string imageType = "Borehole";


        private bool drawMultiColouredEdges = false;

        # region properties

        public List<Layer> DetectedLayers
        {
            get
            {
                return detectedLayers;
            }
        }

        # region Edge detection parameters

        public int StartHeight
        {
            get
            {
                return startHeight;
            }
        }

        public float CannyLow
        {
            get
            {
                return cannyLowThreshold;
            }
            set
            {
                cannyLowThreshold = value;
            }
        }

        public float CannyHigh
        {
            get
            {
                return cannyHighThreshold;
            }
            set
            {
                cannyHighThreshold = value;
            }
        }

        public int GaussianWidth
        {
            get
            {
                return gaussianWidth;
            }
            set
            {
                gaussianWidth = value;
            }
        }

        public int GaussianAngle
        {
            get
            {
                return gaussianAngle;
            }
            set
            {
                gaussianAngle = value;
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
            }
        }

        public bool DisableLayerDetection
        {
            get
            {
                return disableLayerDetection;
            }
            set
            {
                disableLayerDetection = value;
            }
        }

        # endregion

        public string ImageType
        {
            get
            {
                return imageType;
            }
            set
            {
                imageType = value;
            }
        }

        public int DepthResolution
        {
            get
            {
                return depthResolution;
            }
            set
            {
                depthResolution = value;
            }
        }

        # region Test properties

        public bool DrawTestImages
        {
            get
            {
                return drawTestImages;
            }
            set
            {
                drawTestImages = value;
            }
        }

        public Color TestDrawingBackgroundColour
        {
            get
            {
                return testDrawingBackgroundColour;
            }
            set
            {
                testDrawingBackgroundColour = value;
            }
        }

        public Color TestDrawingEdgeColour
        {
            get
            {
                return testDrawingEdgeColour;
            }
            set
            {
                testDrawingEdgeColour = value;
            }
        }

        public Color TestDrawingOverBackgroundColour
        {
            get
            {
                return testDrawingOverBackgroundColour;
            }
            set
            {
                testDrawingOverBackgroundColour = value;
            }
        }

        public bool TestDrawMultiColouredEdges
        {
            get
            {
                return drawMultiColouredEdges;
            }
            set
            {
                drawMultiColouredEdges = value;
            }
        }
        
        # endregion

        # endregion

        /// <summary>
        /// Constructor method
        /// </summary>
        public HoughLayerDetection(Bitmap originalImage, int startHeight)
        {            
            this.originalImage = originalImage;
            this.startHeight = startHeight;

            azimuthResolution = originalImage.Width;
            depthResolution = 1;

            SetUpImageData();
        }

        /// <summary>
        /// Sets up the imageData byte array from the given Bitmap
        /// </summary>
        private void SetUpImageData()
        {
            imageData = BitmapConverter.getRGBFromBitmap(originalImage);

            imageHeight = originalImage.Height;
            imageWidth = originalImage.Width;
        }

        /// <summary>
        /// Calls all other methods and runs the feature detection
        /// </summary>
        public void DetectFeatures()
        {
            bool[] cannyData = PerformCannyEdgeDetection();

            if (imageType == "Borehole")
            {
                detectedFitSines = PerformHoughTransform(cannyData);


                detectedFitSines.Sort(new SineDepthComparer());

                if (drawTestImages)
                {
                    DrawSinesImage drawSines = new DrawSinesImage(originalImage, detectedFitSines);
                    drawSines.DrawnImage.Save("06 - Image with fit sines.bmp");
                }

                //Layer detection
                if (disableLayerDetection == false)
                {
                    DetectLayers detectLayers = new DetectLayers(detectedFitSines, originalImage, depthResolution);

                    detectLayers.LayerBrightnessSensitivity = layerSensitivity;

                    detectLayers.Run();

                    detectedLayers = detectLayers.DetectedLayers;

                    //detectedLayers.Sort(new LayerDepthComparer());
                }
                else
                {
                    Layer currentLayerToAdd;
                    Sine currentSineToAdd;

                    for (int sinePlace = 0; sinePlace < detectedFitSines.Count; sinePlace++)
                    {
                        currentSineToAdd = detectedFitSines[sinePlace];

                        currentLayerToAdd = layerTypeSelector.setUpLayer(currentSineToAdd.Depth + startHeight, currentSineToAdd.Amplitude, currentSineToAdd.Azimuth, currentSineToAdd.Depth + startHeight, currentSineToAdd.Amplitude, currentSineToAdd.Azimuth, azimuthResolution, depthResolution);

                        detectedLayers.Add(currentLayerToAdd);
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
            List<Sine> detectedSines = new List<Sine>();

            HoughTransform houghTransform = new HoughTransform(cannyData, imageWidth, imageHeight);

            houghTransform.EdgeJoinBonus = edgeJoinBonus;
            houghTransform.EdgeLengthBonus = edgeLengthBonus;
            houghTransform.MaxSineAmplitude = maxLayerAmplitude;
            houghTransform.PeakThreshold = peakThreshold;

            houghTransform.Testing = drawTestImages;

            if (drawTestImages == true)
                houghTransform.SetOriginalTestImage(originalImage);

            houghTransform.RunHoughTransform();

            detectedSines = houghTransform.Sines;

            return detectedSines;
        }

        private bool[] PerformCannyEdgeDetection()
        {
            CannyDetector detector = new CannyDetector(imageData, imageWidth, imageHeight);

            detector.WrapHorizontally = true;
            detector.WrapVertically = true; 

            detector.LowThreshold = cannyLowThreshold;
            detector.HighThreshold = cannyHighThreshold;
            detector.GaussianWidth = gaussianWidth;
            detector.GaussianSigma = gaussianAngle;

            detector.HorizontalWeight = horizontalWeight;
            detector.VerticalWeight = verticalWeight;

            

            detector.DetectEdges();

            bool[] edgeData = detector.GetEdgeData();

            if (drawTestImages)
            {
                DrawEdgesImageFactory factory = new DrawEdgesImageFactory("Bool");
                DrawEdgesImage drawImage = factory.setUpDrawEdges(edgeData, imageWidth, imageHeight);

                drawImage.setBackgroundColour(testDrawingBackgroundColour);
                drawImage.setEdgeColour(testDrawingEdgeColour);
                drawImage.SetDrawMultiColouredEdges(drawMultiColouredEdges);

                drawImage.drawEdgesImage();
                drawImage.getDrawnEdges().Save("01a - Canny(" + detector.LowThreshold + "," + detector.HighThreshold + "," + detector.GaussianWidth + ", " + (int)detector.GaussianSigma + ").bmp");

                drawImage.setEdgeColour(testDrawingOverBackgroundColour);
                drawImage.drawEdgesOverBackgroundImage(originalImage);
                drawImage.getDrawnEdges().Save("01b - Canny over original(" + detector.LowThreshold + "," + detector.HighThreshold + "," + detector.GaussianWidth + ", " + (int)detector.GaussianSigma + ").bmp");
            }

            return edgeData;
        }
    }
}
