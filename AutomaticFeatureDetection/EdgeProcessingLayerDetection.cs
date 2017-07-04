using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Edges;
using DrawEdges;
using BoreholeFeatures;
using LayerDetection;
using DrawEdges.DrawEdgesFactory;
using PerformEdgeOperations;
using EdgeFitting;
using EdgeDetector;

namespace AutomaticFeatureDetection
{
    /// <summary>
    /// Sets up all automatic layer detection classes
    /// </summary>
    public class EdgeProcessingLayerDetection
    {
        //parameters
        private float cannyLowThreshold = 0.01f;
        private float cannyHighThreshold = 0.016f;
        private int gaussianWidth = 20;
        private int gaussianAngle = 6;

        private float verticalWeight, horizontalWeight;

        private int edgeLinkingThreshold = 320;
        private int edgeLinkingMaxNeighbourDistance = 5;
        private int edgeRemovalThreshold = 320;
        private int edgeJoiningThreshold = 20;

        private int layerSensitivity = 1;

        private int maxSineAmplitude = 100;

        //Data
        private int imageWidth;
        private int imageHeight;
        private int azimuthResolution, depthResolution;
        private int[] imageData;

        private Bitmap originalImage;
        private List<Edge> foundEdges = new List<Edge>();
        private List<Edge> edgesBeforeOperations = new List<Edge>();
        private List<Edge> edgesAfterSineFitting = new List<Edge>();

        private List<Sine> detectedFitSines = new List<Sine>();
        private List<EdgeLine> detectedFitLines = new List<EdgeLine>();
        
        //Detected features
        private List<Layer> detectedLayers = new List<Layer>();

        private int startHeight;

        private bool drawTestImages = false;
        Color testDrawingBackgroundColour, testDrawingEdgeColour, testDrawingOverBackgroundColour;

        DateTime initializeTime, setUpTime;

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

        public int EdgeLinkingThreshold
        {
            get
            {
                return edgeLinkingThreshold;
            }
            set
            {
                edgeLinkingThreshold = value;
            }
        }

        public int EdgeLinkingDistance
        {
            get
            {
                return edgeLinkingMaxNeighbourDistance;
            }
            set
            {
                edgeLinkingMaxNeighbourDistance = value;
            }
        }

        public int RemoveEdgeThreshold
        {
            get
            {
                return edgeRemovalThreshold;
            }
            set
            {
                edgeRemovalThreshold = value;
            }
        }

        public int JoinEdgeThreshold
        {
            get
            {
                return edgeJoiningThreshold;
            }
            set
            {
                edgeJoiningThreshold = value;
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

        public int MaxSineAmplitude
        {
            get
            {
                return maxSineAmplitude;
            }
            set
            {
                maxSineAmplitude = value;
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
        public EdgeProcessingLayerDetection(Bitmap originalImage, int startHeight)
        {
            initializeTime = DateTime.Now;
            Console.WriteLine("Initialising at: " + initializeTime);

            this.originalImage = originalImage;
            this.startHeight = startHeight;

            azimuthResolution = originalImage.Width;
            depthResolution = 1;

            SetUpImageData();

            setUpTime = DateTime.Now;
            Console.WriteLine("Setup Done: " + setUpTime);
        }

        /// <summary>
        /// Sets up the imageData byte array from the given Bitmap
        /// </summary>
        private void SetUpImageData()
        {
            imageData = BitmapConverter.GetRgbFromBitmap(originalImage);

            imageHeight = originalImage.Height;
            imageWidth = originalImage.Width;
        }

        /// <summary>
        /// Calls all other methods and runs the feature detection
        /// </summary>
        public void DetectFeatures()
        {
            bool[] cannyData = PerformCannyEdgeDetection();

            foundEdges = PerformEdgeOperations(cannyData);

            if (imageType == "Borehole")
            {
                detectedFitSines = PerformSineFitting();

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
                detectedFitLines = PerformLineFitting();

                if (drawTestImages)
                {
                    DrawSinesImage drawSines = new DrawSinesImage(originalImage, detectedFitSines);
                    drawSines.DrawnImage.Save("06 - Image with fit sines.bmp");
                }

                //Layer detection
                if (disableLayerDetection == false)
                {
                    DetectLayers detectLayers = new DetectLayers(detectedFitLines, originalImage, depthResolution);

                    detectLayers.LayerBrightnessSensitivity = layerSensitivity;

                    detectLayers.Run();

                    detectedLayers = detectLayers.DetectedLayers;
                }
                else
                {
                    Layer currentLayerToAdd;
                    EdgeLine currentLineToAdd;

                    for (int linePlace = 0; linePlace < detectedFitLines.Count; linePlace++)
                    {
                        currentLineToAdd = detectedFitLines[linePlace];

                        currentLayerToAdd = layerTypeSelector.setUpLayer(currentLineToAdd.Slope, currentLineToAdd.Intercept + startHeight, currentLineToAdd.Slope, currentLineToAdd.Intercept + startHeight, azimuthResolution, depthResolution);

                        detectedLayers.Add(currentLayerToAdd);
                    }
                }
            }
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

        private List<Edge> PerformEdgeOperations(bool[] cannyData)
        {
            List<Edge> foundEdges = new List<Edge>();

            EdgeOperations performEdgeOperations = new EdgeOperations(cannyData, imageWidth, imageHeight);
            performEdgeOperations.DrawTestImages = drawTestImages;

            if (drawTestImages)
                performEdgeOperations.SetTestImage(originalImage);
            
            performEdgeOperations.EdgeLinkingThreshold = edgeLinkingThreshold;
            performEdgeOperations.EdgeLinkingDistance = edgeLinkingMaxNeighbourDistance;

            performEdgeOperations.EdgeRemovalThreshold = edgeRemovalThreshold;
            performEdgeOperations.EdgeJoiningThreshold = edgeJoiningThreshold;

            performEdgeOperations.TestDrawingBackgroundColour = testDrawingBackgroundColour;
            performEdgeOperations.TestDrawingEdgeColour = testDrawingEdgeColour;
            performEdgeOperations.TestDrawingOverBackgroundColour = testDrawingOverBackgroundColour;
            performEdgeOperations.TestDrawMultiColouredEdges = drawMultiColouredEdges;

            performEdgeOperations.Run();

            foundEdges = performEdgeOperations.FoundEdges;
            edgesBeforeOperations = performEdgeOperations.EdgesBeforeOperations;

            return foundEdges;
        }

        private List<Sine> PerformSineFitting()
        {
            EdgeFit edgeFitting = new EdgeFit(foundEdges, imageWidth, imageHeight);
            edgeFitting.ImageType = "Borehole";

            //edgeFitting.MaxAmplitude = maxSineAmplitude;
            edgeFitting.FitEdges();

            List<Sine> sines = edgeFitting.FitSines;

            sines.Sort(new SineDepthComparer());

            return sines;
        }

        private List<EdgeLine> PerformLineFitting()
        {
            EdgeFit edgeFitting = new EdgeFit(foundEdges, imageWidth, imageHeight);
            edgeFitting.ImageType = "Core";

            edgeFitting.MaxAmplitude = maxSineAmplitude;
            edgeFitting.FitEdges();

            List<EdgeLine> lines = edgeFitting.FitLines;

            lines.Sort(new LineDepthComparer());

            return lines;
        }

        private void deleteSinesFromEdgeData()
        {
            if (drawTestImages)
            {
                DrawEdgesImageFactory factory = new DrawEdgesImageFactory("Edge");
                DrawEdgesImage drawImage = factory.setUpDrawEdges(edgesBeforeOperations, imageWidth, imageHeight);

                drawImage.setBackgroundColour(testDrawingBackgroundColour);
                drawImage.setEdgeColour(testDrawingEdgeColour);
                drawImage.SetDrawMultiColouredEdges(drawMultiColouredEdges);

                drawImage.drawEdgesImage();
                drawImage.getDrawnEdges().Save("07a - All edges before operations.bmp");

                drawImage.setEdgeColour(testDrawingOverBackgroundColour);
                drawImage.drawEdgesOverBackgroundImage(originalImage);
                drawImage.getDrawnEdges().Save("07b - All edges before operations.bmp");
            }

            for (int i = 0; i < edgesBeforeOperations.Count; i++)
            {
                removeIfAlreadyUsed(edgesBeforeOperations[i]);
            }

            if (drawTestImages)
            {
                DrawEdgesImageFactory factory = new DrawEdgesImageFactory("Edge");
                DrawEdgesImage drawImage = factory.setUpDrawEdges(edgesAfterSineFitting, imageWidth, imageHeight);

                drawImage.setBackgroundColour(testDrawingBackgroundColour);
                drawImage.setEdgeColour(testDrawingEdgeColour);
                drawImage.drawEdgesImage();
                drawImage.getDrawnEdges().Save("08a - Remaining edges after sine removal.bmp");

                drawImage.setEdgeColour(testDrawingOverBackgroundColour);
                drawImage.drawEdgesOverBackgroundImage(originalImage);
                drawImage.getDrawnEdges().Save("08b - Remaining edges after sine removal.bmp");
            }
        }

        /// <summary>
        /// Checks if this edge is part of any of the Edges used for sinefitting.  If not it keeps the edge
        /// </summary>
        /// <param name="point"></param>
        private void removeIfAlreadyUsed(Edge edge)
        {
            Point point = edge.EdgeEnd1;
            bool edgeUsed = false;

            for (int i = 0; i < foundEdges.Count; i++)
            {
                if (foundEdges[i].Points.Contains(point))
                    edgeUsed = true;
            }

            if (edgeUsed == false)
                edgesAfterSineFitting.Add(edge);
        }
    }
}
