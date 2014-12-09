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
    public partial class SectionSizeDialogBox : Form
    {
        private int maximum = 0;
        private int value = 10000;

        private bool okClicked = false;

        public SectionSizeDialogBox()
        {
            InitializeComponent();
            okClicked = false;
        }

        private void SetInitialValues()
        {
            okClicked = false;

            sectionSizeTrackbar.Minimum = 0;
            sectionSizeTrackbar.Maximum = Convert.ToInt32(Math.Ceiling((double)maximum / 10000)) - 1;
            sectionSizeTrackbar.Value = 0;

            SetTextBoxValues();
        }

        private void SetTextBoxValues()
        {
            value = (sectionSizeTrackbar.Value + 1) * 10000;
            if(value>maximum)
                value = maximum;

            sectionSizeTextBox.Text = value.ToString();

            if (value == maximum)
                numberOfBitmapsTextBox.Text = "1";
            else
            {
                int numberOfSections = Convert.ToInt32(Math.Ceiling(((double)maximum / (double)value)));
                numberOfBitmapsTextBox.Text = numberOfSections.ToString();
            }
        }

        public int GetSectionHeight()
        {
            return value;
        }

        public void SetMaximum(int maximum)
        {
            this.maximum = maximum;

            SetInitialValues();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            this.Visible = false;
            okClicked = true;
        }

        private void sectionSizeTrackbar_Scroll(object sender, EventArgs e)
        {
            SetTextBoxValues();
        }

        public bool OkClicked()
        {
            return okClicked;
        }
    }
}
