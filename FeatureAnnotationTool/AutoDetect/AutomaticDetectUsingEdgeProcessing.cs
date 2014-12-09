using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using AutomaticFeatureDetection;
using ImageTiler;
using FeatureAnnotationTool.Interfaces;
using BoreholeFeatures;

namespace FeatureAnnotationTool.AutoDetect
{
    /// <summary>
    /// Deals with calling the auotmatic detection class and sending the results back to the view
    /// 
    /// Author Terry Malone (trm8@aber.ac.uk)
    /// Version 1.0
    /// </summary>
    public class AutomaticDetectUsingEdgeProcessing
    {
        private bool drawTestImages = false;

        private EdgeProcessingLayerDetection detection;
        private IView _view;
        private FileTiler tiler;

        private float cannyLow, cannyHigh;
        private int gaussianWidth, gaussianAngle;

        private float verticalWeight, horizontalWeight;
        private int edgeLinkingThreshold, edgeLinkingDistance, edgeRemovalThreshold, edgeJoiningThreshold;
        private int layerSensitivity;

        private int start, end;
        private int startDepth, endDepth;

        private bool disableLayerDetection = false;
        
        private string imageType;

        private int overlapSize = 100;
        private List<Layer> savedFromPreviousSection;

        # region properties

        public int StartAnalysisDepth
        {
            get
            {
                return startDepth;
            }
            set
            {
                startDepth = value;
            }
        }

        public int EndAnalysisDepth
        {
            get
            {
                return endDepth;
            }
            set
            {
                endDepth = value;
            }
        }

        public float CannyLow
        {
            get
            {
                return cannyLow;
            }
            set
            {
                cannyLow = value;
            }
        }

        public float CannyHigh
        {
            get
            {
                return cannyHigh;
            }
            set
            {
                cannyHigh = value;
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
                return edgeLinkingDistance;
            }
            set
            {
                edgeLinkingDistance = value;
            }
        }

        public int EdgeRemovalThreshold
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

        public int EdgeJoiningThreshold
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

        #endregion

        # region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tiler">The boreholes image tiler</param>
        /// <param name="view">The connection to the view</param>
        public AutomaticDetectUsingEdgeProcessing(FileTiler tiler, IView view, string imageType)
        {
            this.imageType = imageType;
            this._view = view;
            this.tiler = tiler;

            startDepth = 0;
            endDepth = tiler.BoreholeHeight;

            savedFromPreviousSection = new List<Layer>();
        }

        # endregion

        /// <summary>
        /// Runs the automatic detection
        /// </summary>
        /// <param name="worker"></param>
        /// <param name="e"></param>
        //public void detect(System.ComponentModel.BackgroundWorker worker, System.ComponentModel.DoWorkEventArgs e)
        public void Detect()
        {
            start = startDepth;
            end = startDepth + overlapSize;

            bool endReached = false;

            while (end < endDepth && endReached == false)
            {
                start = end - overlapSize;
                end += 1000;

                if (end >= endDepth)
                {
                    end = endDepth - 1;
                    endReached = true;
                }

                _view.UpdateProgress(start, endDepth);
                tiler.LoadSpecificSection(start, end);

                DetectFeaturesInSection(tiler.GetCurrentSectionAsBitmap());
            }

            //Add anything left in savedFromPreviousSection
            for (int i = 0; i < savedFromPreviousSection.Count; i++)
            {
                _view.AddCompleteLayer(savedFromPreviousSection[i]);
            } 

            _view.StopEdgeDetection(true);
        }

        /// <summary>
        /// NEEDS REFACTORING - BADLY!
        /// </summary>
        /// <param name="tile"></param>
        private void DetectFeaturesInSection(Bitmap tile)
        {
            SetUpAutomaticLayerDetection(tile);
            
            detection.DetectFeatures();

            List<Layer> detectedLayers = detection.DetectedLayers;

            detection.DrawTestImages = drawTestImages;

            List<Layer> savedFromThisSection = new List<Layer>();
 
            //If features are in saved overlap area
                //If new feature is in area
                    //If yes choose which to delete & add other to found features/remove from saved
 
            for (int layerListPos = 0; layerListPos < detectedLayers.Count; layerListPos++)
            {
                detectedLayers[layerListPos].MoveEdge(3, 0, start);

                if (detectedLayers[layerListPos].StartY <= start + overlapSize)    //Layer is in start overlap section
                {
                    if (savedFromPreviousSection.Count > 0)                             //There are previously saved layers from end of last overlap section
                    {
                        for (int i = 0; i < savedFromPreviousSection.Count; i++)
                        {
                            if (detectedLayers[layerListPos].StartY < savedFromPreviousSection[i].EndY)
                            {
                                int firstHeight = savedFromPreviousSection[i].EndY - savedFromPreviousSection[i].StartY;
                                int secondHeight = detectedLayers[layerListPos].EndY - detectedLayers[layerListPos].StartY;

                                if(firstHeight > secondHeight)
                                {
                                    //Do nothing. This layer will not be added
                                }
                                else
                                {
                                    savedFromPreviousSection.Remove(savedFromPreviousSection[i]);
                                    _view.AddCompleteLayer(detectedLayers[layerListPos]);
                                }
                            }
                            else
                                _view.AddCompleteLayer(detectedLayers[layerListPos]);
                        }
                    }
                    else
                        _view.AddCompleteLayer(detectedLayers[layerListPos]);  
                }
                else if (detectedLayers[layerListPos].EndY > end-overlapSize)           //Layer is in end overlap section
                    savedFromThisSection.Add(detectedLayers[layerListPos]);
                else
                    _view.AddCompleteLayer(detectedLayers[layerListPos]);

            }

            //Add anything left in savedFromPreviousSection
            for (int i = 0; i < savedFromPreviousSection.Count; i++)
            {
                _view.AddCompleteLayer(savedFromPreviousSection[i]);
            } 
           
            savedFromPreviousSection.Clear();

            //Copy all from savedFromThisSection to savedFromPreviousSection
            for (int i = 0; i < savedFromThisSection.Count; i++)
            {
                savedFromPreviousSection.Add(savedFromThisSection[i]);
            }

            savedFromThisSection.Clear();
        }

        /// <summary>
        /// Initialises the instance of AutomaticLayerDetection
        /// </summary>
        /// <param name="tile"></param>
        private void SetUpAutomaticLayerDetection(Bitmap tile)
        {
            int currentHeight = 0;
            detection = new EdgeProcessingLayerDetection(tile, currentHeight);
            detection.ImageType = imageType;

            detection.CannyLow = cannyLow;
            detection.CannyHigh = cannyHigh;
            detection.GaussianWidth = gaussianWidth;
            detection.GaussianAngle = gaussianAngle;

            detection.VerticalWeight = verticalWeight;
            detection.HorizontalWeight = horizontalWeight;

            detection.EdgeLinkingThreshold = edgeLinkingThreshold;
            detection.EdgeLinkingDistance = edgeLinkingDistance;
            detection.RemoveEdgeThreshold = edgeRemovalThreshold;
            detection.JoinEdgeThreshold = edgeJoiningThreshold;

            detection.LayerSensitivity = layerSensitivity;
            
            detection.DisableLayerDetection = disableLayerDetection;

            if (drawTestImages)
            {
                detection.TestDrawingBackgroundColour = Color.Black;
                detection.TestDrawingEdgeColour = Color.White;
                detection.TestDrawingOverBackgroundColour = Color.Red;

                detection.TestDrawMultiColouredEdges = true;

                detection.DrawTestImages = true;
            }
            else
                detection.DrawTestImages = false;
        }
    }
}
