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
    public partial class GroupNameForm : Form
    {
        private string name;

        public GroupNameForm(string name)
        {
            InitializeComponent();

            this.name = name;

            nameTextBox.Text = name;
            nameTextBox.Select(0, nameTextBox.Text.Length);
        }

        public string GetName()
        {
            return name;
        }

        private void doneButton_Click(object sender, EventArgs e)
        {
            name = nameTextBox.Text;

            this.Visible = false;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Visible = false;
        }


    }
}
