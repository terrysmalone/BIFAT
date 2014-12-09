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
    public partial class CreateNewProjectFromOTVForm : Form
    {
        private string projectLocation;
        private string projectName;

        //private bool nonNumberEntered = false;
        private string previousProjectName, previousProjectLocation;


        public CreateNewProjectFromOTVForm()
        {
            InitializeComponent();
        }
        
        private void updateFields()
        {
            projectNameTextBox.Text = projectName;
            projectLocationTextBox.Text = projectLocation;
            
            previousProjectName = projectName;
            previousProjectLocation = projectLocation;
        }

        private void CreateNewProjectForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Visible = false;
        }
        
        private void browseLocationButton_Click_1(object sender, EventArgs e)
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            folderDialog.Description = "Select where you would like to place the annotated project.";
            
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

        private void projectNameTextBox_Leave(object sender, EventArgs e)
        {
            try
            {
                if (projectNameTextBox.Text.Length < 1)
                    throw new Exception();
                else if (ContainsInvalidChar(projectNameTextBox.Text))
                {
                    throw new Exception();
                }

                projectName = projectNameTextBox.Text;
            }
            catch (Exception exception)
            {
                MessageBox.Show("Incorrect value entered.", "Incorrect value");
                projectName = previousProjectName;
            }

            updateFields();
        }

        private bool ContainsInvalidChar(string text)
        {
            if(text.Contains('/') || text.Contains(':') || text.Contains('*') || text.Contains('?') || text.Contains('"') || text.Contains('<') || text.Contains('>') || text.Contains('|'))
                return true;
            else
            return false;
        }

        private void projectLocationTextBox_Leave(object sender, EventArgs e)
        {
            projectLocation = projectLocationTextBox.Text;
        }

        # region set methods

        internal void setProjectLocation(string projectLocation)
        {
            this.projectLocation = projectLocation;
            updateFields();
        }

        public void setBoreholeName(string name)
        {
            projectName = name + "_Annotation (" + DateTime.Now.ToString() + ")";

            string dateString = DateTime.Now.ToString();

            dateString = dateString.Replace("/", "-");
            dateString = dateString.Replace(":", "-");

            projectName = name + "_Annotation (" + dateString + ")";

            updateFields();
        }

        # endregion

        # region get methods

        public string getProjectRoot()
        {
            return projectLocationTextBox.Text + "\\" + projectNameTextBox.Text;
        }

        internal string getProjectName()
        {
            return projectNameTextBox.Text;
        }

        # endregion
    }
}
