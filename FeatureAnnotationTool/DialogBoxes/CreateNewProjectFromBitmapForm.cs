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
    public partial class CreateNewProjectFromBitmapForm : Form
    {
        private string boreholeName = "";
        private int initialHeight;
        private int startDepth, endDepth;
        private int depthResolution;
               
        private string previousBoreholeName;
        private string previousProjectName;
        private string previousProjectLocation;
        private int previousStartDepth;
        private int previousEndDepth;
        private int previousDepthResolution;

        private string projectLocation;
        private string projectName;

        private bool nonNumberEntered = false;

        #region properties

        public string BoreholeName
        {
            get { return boreholeName; }
            set
            {
                boreholeName = value;

                string dateString = DateTime.Now.ToString();

                dateString = dateString.Replace("/", "-");
                dateString = dateString.Replace(":", "-");

                projectName = boreholeName + "_Annotation (" + dateString + ")";
                updateFields();
            }
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

        internal string ProjectName
        {
            get { return projectNameTextBox.Text; }
        }

        public string ProjectRoot
        {
            get { return projectLocationTextBox.Text + "\\" + projectNameTextBox.Text; }
        }

        #endregion properties

        public CreateNewProjectFromBitmapForm()
        {
            InitializeComponent();

            startDepth = 0;

            depthResolution = 1;
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
            nameTextBox.Text = boreholeName;
            projectNameTextBox.Text = projectName;
            projectLocationTextBox.Text = projectLocation;
            startDepthTextBox.Text = System.Convert.ToString(startDepth);
            endDepthTextBox.Text = System.Convert.ToString(endDepth);
            depthResolutionTextBox.Text = System.Convert.ToString(depthResolution);

            previousBoreholeName = boreholeName;
            previousProjectName = projectName;
            previousProjectLocation = projectLocation;
            previousStartDepth = startDepth;
            previousEndDepth = endDepth;
            previousDepthResolution = depthResolution;

        }

        private void CreateNewProjectForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Visible = false;
        }

        # region Leave text boxes methods

        private void startDepthTextBox_Leave(object sender, EventArgs e)
        {
            try
            {
                startDepth = System.Convert.ToInt32(startDepthTextBox.Text);

                if (ContainsInvalidChar(startDepthTextBox.Text))
                {
                    throw new Exception();
                }

                calculateEndDepthInMM();
                updateFields();
            }
            catch (Exception exception)
            {
                MessageBox.Show("Incorrect value entered.", "Incorrect value");
                startDepth = previousStartDepth;
                calculateEndDepthInMM();
                updateFields();
            }
        }

        private void endDepthTextBox_Leave(object sender, EventArgs e)
        {
            try
            {
                if (ContainsInvalidChar(endDepthTextBox.Text))
                {
                    throw new Exception();
                }

                endDepth = System.Convert.ToInt32(endDepthTextBox.Text);
                calculateStartDepthInMM();
                updateFields();
            }
            catch (Exception exception)
            {
                MessageBox.Show("Incorrect value entered.", "Incorrect value");
                endDepth = previousEndDepth;
                calculateStartDepthInMM();
                updateFields();
            }
        }

        private void depthResolutionTextBox_Leave(object sender, EventArgs e)
        {
            try
            {
                depthResolution = System.Convert.ToInt32(depthResolutionTextBox.Text);
                //height = (int)((float)initialHeight / (float)depthResolution);

                if (depthResolution < 1)
                {
                    depthResolution = 1;
                    depthResolutionTextBox.Text = "1";
                }
                else if (ContainsInvalidChar(depthResolutionTextBox.Text))
                {
                    throw new Exception();
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show("Incorrect value entered.", "Incorrect value");
                depthResolution = previousDepthResolution;
                //updateFields();
            }

            calculateEndDepthInMM();
            updateFields();
        }

        private void projectNameTextBox_Leave(object sender, EventArgs e)
            {
            try
            {
                if (projectNameTextBox.Text.Length < 1)
                    throw new Exception();
                else if (ContainsInvalidChar(projectNameTextBox.Text))
                    throw new Exception();

                projectName = projectNameTextBox.Text;
            }
            catch (Exception exception)
            {
                MessageBox.Show("Incorrect value entered.", "Incorrect value");
                projectName = previousProjectName;
            }

            updateFields();
        }

        private void projectLocationTextBox_Leave(object sender, EventArgs e)
        {
            try
            {
                if (projectLocationTextBox.Text.Length < 1)
                    throw new Exception();
                
                projectLocation = projectLocationTextBox.Text;
            }
            catch (Exception exception)
            {
                MessageBox.Show("Incorrect value entered.", "Incorrect value");
                projectLocation = previousProjectLocation;
            }

            updateFields();
        }

        private bool ContainsInvalidChar(string text)
        {
            if (text.Contains('/') || text.Contains(':') || text.Contains('*') || text.Contains('?') || text.Contains('"') || text.Contains('<') || text.Contains('>') || text.Contains('|'))
                return true;
            else
                return false;
        }

        private void nameTextBox_Leave(object sender, EventArgs e)
        {
            try
            {
                if (nameTextBox.Text.Length < 1)
                    throw new Exception();

                boreholeName = nameTextBox.Text;
            }
            catch (Exception exception)
            {
                MessageBox.Show("Incorrect value entered.", "Incorrect value");
                boreholeName = previousBoreholeName;

            }

            updateFields();
        }

        # endregion

        # region Key events

        # region StartDepthTextBox key events

        private void startDepthTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            nonNumberEntered = isKeyNonNumber(e.KeyCode);

            if (e.KeyCode == Keys.Subtract || e.KeyCode == Keys.OemMinus)
            {
                if (startDepthTextBox.Text.Length == 0 || startDepthTextBox.SelectionLength == startDepthTextBox.Text.Length || (startDepthTextBox.SelectionStart == 0 && (startDepthTextBox.Text[0] != '-' || startDepthTextBox.SelectionLength > 0)))
                    nonNumberEntered = false;
            }

            if (e.Shift)
                nonNumberEntered = true;
        }

        private void startDepthTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (nonNumberEntered == true)
            {
                e.Handled = true;
            }
        }       

        # endregion

        # region endDepthTextBox key event

        private void endDepthTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            nonNumberEntered = isKeyNonNumber(e.KeyCode);

            if (e.KeyCode == Keys.Subtract || e.KeyCode == Keys.OemMinus)
            {

                if (endDepthTextBox.Text.Length == 0 || endDepthTextBox.SelectionLength == endDepthTextBox.Text.Length || (endDepthTextBox.SelectionStart == 0 && (endDepthTextBox.Text[0] != '-' || endDepthTextBox.SelectionLength > 0)))
                    nonNumberEntered = false;
            }

            if (e.Shift)
                nonNumberEntered = true;
        }

        private void endDepthTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (nonNumberEntered == true)
            {
                e.Handled = true;
            }
        }

        # endregion

        private bool isKeyNonNumber(Keys key)
        {
            nonNumberEntered = false;

            if (key == Keys.Subtract)
            {

            }
            else if (key < Keys.D0 || key > Keys.D9)
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

        # endregion

        private void browseLocationButton_Click_1(object sender, EventArgs e)
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            folderDialog.Description = "Select where you would like to place the annotated project.";
            //folderDialog.RootFolder = Environment.SpecialFolder.MyComputer;

            //folderDialog.SelectedPath = "C:\\Users\\Terry";
            folderDialog.SelectedPath = projectLocation + "\\";

            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                projectLocationTextBox.Text = folderDialog.SelectedPath;
                projectLocation = projectLocationTextBox.Text;
            }
        }

        private void finishButton_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(projectLocationTextBox.Text + "\\" + projectNameTextBox.Text))
            {
                if (MessageBox.Show("Project already exists. Overwrite?", "Overwrite Existing Project", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    this.Visible = false;
                else
                {
                    projectNameTextBox.SelectAll();
                }
            }
            else
                this.Visible = false;
        }

        # region set methods

        public void setHeight(int boreholeHeight)
        {
            initialHeight = boreholeHeight;

            calculateEndDepthInMM();
            updateFields();
        }

        internal void setProjectLocation(string projectLocation)
        {
            this.projectLocation = projectLocation;
            updateFields();
        }

        public void SetProjectType(string type)
        {
            boreholeDetailsGroupBox.Text = type + " details";
            nameLabel.Text = type + " name";
        }

        # endregion      
    }
}
