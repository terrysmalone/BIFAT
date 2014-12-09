using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FeatureAnnotationTool
{
    /// <summary>
    /// Displays the borehole betails and allows the user to alter them
    /// 
    /// Author - Terry Malone (trm8@aber.ac.uk)
    /// Version 1.0
    /// </summary>
    public partial class BoreholeDetailsForm : Form
    {
        private string name = "";
        private int startDepth, endDepth;
        private int depthResolution;
        private int initialHeight;

        private bool nonNumberEntered = false;

        #region properties

        public string BoreholeName
        {
            get { return name; }
        }

        public int DepthResolution
        {
            get { return depthResolution; }
        }

        public int StartDepth
        {
            get { return startDepth; }
        }

        public int EndDepth
        {
            get { return endDepth; }
        }

        #endregion properties

        public BoreholeDetailsForm(int initialHeight)
        {
            InitializeComponent();

            this.initialHeight = initialHeight;
            startDepth = 0;
            //firstDigit = true;
            
            //height = initialHeight;
            depthResolution = 1;
            calculateEndDepthInMM();
            updateFields();
        }

        private void calculateEndDepthInMM()
        {
            endDepth = startDepth + (initialHeight * depthResolution);
            
        }

        private void calculateStartDepthInMM()
        {
            startDepth = endDepth - (initialHeight * depthResolution);
        }

        private void updateFields()
        {
            nameTextBox.Text = name;
            startDepthTextBox.Text = System.Convert.ToString(startDepth);
            endDepthTextBox.Text = System.Convert.ToString(endDepth);
            depthResolutionTextBox.Text = System.Convert.ToString(depthResolution);
        }

        private void doneButton_Click(object sender, EventArgs e)
        {
            this.Visible = false;
        }
        
        private void BoreholeDetailsForm_FormClosing(object sender, FormClosingEventArgs e)
        {            
            e.Cancel = true;
            this.Visible = false;
        }

        private void nameTextBox_TextChanged(object sender, EventArgs e)
        {
            name = nameTextBox.Text;
        }
        
        private void startDepthTextBox_Leave(object sender, EventArgs e)
        {
            startDepth = System.Convert.ToInt32(startDepthTextBox.Text);
            calculateEndDepthInMM();
            updateFields();
        }

        private void endDepthTextBox_Leave(object sender, EventArgs e)
        {
            endDepth = System.Convert.ToInt32(endDepthTextBox.Text);
            calculateStartDepthInMM();
            updateFields();
        }

        private void depthResolutionTextBox_Leave(object sender, EventArgs e)
        {
            depthResolution = System.Convert.ToInt32(depthResolutionTextBox.Text);
            //height = (int)((float)initialHeight / (float)depthResolution);

            if (depthResolution < 1)
            {
                depthResolution = 1;
                depthResolutionTextBox.Text = "1";
            }

            calculateEndDepthInMM();
            updateFields();
        }

        public void setDefaultName(string name)
        {
            this.name = name;
            updateFields();
        }

        private void startDepthTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (nonNumberEntered == true)
            {
                //MessageBox.Show("Invalid input");
                e.Handled = true;
            }
        }

        private void startDepthTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            nonNumberEntered = isKeyNonNumber(e.KeyCode);
        }

        private bool isKeyNonNumber(Keys key)
        {
            nonNumberEntered = false;

            if (key < Keys.D0 || key > Keys.D9)
            {
                // Determine whether the keystroke is a number from the keypad.
                if (key < Keys.NumPad0 || key > Keys.NumPad9)
                {
                    // Determine whether the keystroke is a backspace.
                    if (key != Keys.Back)
                    {
                        // A non-numerical keystroke was pressed.
                        // Set the flag to true and evaluate in KeyPress event.
                        nonNumberEntered = true;
                    }
                }
            }

            return nonNumberEntered;
        }
    }
}
