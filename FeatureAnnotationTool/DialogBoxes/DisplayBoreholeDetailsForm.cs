using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FeatureAnnotationTool.DialogBoxes
{
    public partial class DisplayDetailsForm : Form
    {
        private string boreholeName;
        private int boreholeDepth;
        private int depthResolution;
        private int azimuthResolution;
        private int startDepth;
        private int endDepth;

        public DisplayDetailsForm()
        {
            InitializeComponent();
        }

        # region Set methods

        public void SetBoreholeName(string boreholeName)
        {
            this.boreholeName = boreholeName;
            nameTextBox.Text = boreholeName;
        }

        public void SetBoreholeDepth(int boreholeDepth)
        {
            this.boreholeDepth = boreholeDepth;

            boreholeDepthTextBox.Text = (boreholeDepth * depthResolution).ToString();
        }

        public void SetDepthResolution(int depthResolution)
        {
            this.depthResolution = depthResolution;
            depthResolutionTextBox.Text = depthResolution.ToString();
        }

        public void SetAzimuthResolution(int azimuthResolution)
        {
            this.azimuthResolution = azimuthResolution;
            azimuthResolutionTextBox.Text = azimuthResolution.ToString();
        }

        public void SetStartDepth(int startDepth)
        {
            this.startDepth = startDepth;
            startDepthTextBox.Text = startDepth.ToString();
        }

        public void SetEndDepth(int endDepth)
        {
            this.endDepth = endDepth;
            endDepthTextBox.Text = endDepth.ToString();
        }

        # endregion

        internal void SetImageType(string type)
        {
            if (type == "Borehole")
            {
                this.Text = "Borehole details";
                nameLabel.Text = "Borehole name:";
                depthLabel.Text = "Borehole depth (mm):";
            }
            else if (type == "Core")
            {
                this.Text = "Core details";
                nameLabel.Text = "Core name:";
                depthLabel.Text = "Core depth (mm):";
            }
        }
    }
}
