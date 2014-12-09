using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FeatureAnnotationTool.Interfaces;

namespace FeatureAnnotationTool.DialogBoxes
{
    public partial class InclusionGroupsForm : Form
    {
        private IModel _model;
        private List<Tuple<string, Color>> inclusionGroups;

        private Tuple<string, Color> selectedGroup;

        public InclusionGroupsForm(IModel _model)
        {
            this._model = _model;
            InitializeComponent();

            inclusionGroups = _model.GetInclusionGroups();

            groupsListView.View = View.Details;
            //groupsListView.GridLines = true;
            groupsListView.FullRowSelect = true;

            DisplayGroups();
        }

        private void DisplayGroups()
        {
            groupsListView.Items.Clear();

            for (int i = 0; i < inclusionGroups.Count; i++)
            {
                ListViewItem row = new ListViewItem(inclusionGroups[i].Item1);

                row.SubItems.Add(_model.GetInclusionGroupCount(inclusionGroups[i].Item1).ToString());
                groupsListView.Items.Add(row);
                groupsListView.Items[i].BackColor = inclusionGroups[i].Item2;
            }
        }

        # region Button clicks



        private void doneButton_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        # endregion

        private void groupsListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            string rowName = "";

            if (groupsListView.SelectedItems.Count > 0)
            {
                rowName = groupsListView.SelectedItems[0].Text;

                //string rowName = groupsListView.

                if (rowName == "Unspecified")
                {
                    changeColourButton.Enabled = true;
                    renameGroupButton.Enabled = false;
                    deleteGroupButton.Enabled = false;
                }
                else
                {
                    ActiveGroupButtons();
                }

                selectedGroup = inclusionGroups[groupsListView.SelectedIndices[0]];
            }
            else
            {
                NoActiveGroupButtons();
            }
        }

        private void changeColourButton_Click(object sender, EventArgs e)
        {
            colourPicker.Color = selectedGroup.Item2;
            Color newColour = selectedGroup.Item2;

            if (colourPicker.ShowDialog() == DialogResult.OK)
            {
                newColour = colourPicker.Color;
                _model.ChangeInclusionGroupColour(selectedGroup.Item1, newColour);

                groupsListView.SelectedItems[0].BackColor = newColour;
            }
        }

        private void addGroupButton_Click(object sender, EventArgs e)
        {
            GroupNameForm nameForm = new GroupNameForm("New Group");

            nameForm.ShowDialog();

            _model.AddInclusionGroup(nameForm.GetName());
            _model.SaveGroups();

            DisplayGroups();

            selectedGroup = inclusionGroups[groupsListView.Items.Count - 1];
            groupsListView.Items[groupsListView.Items.Count - 1].Selected = true;

            nameForm.Dispose();
        }

        private void renameGroupButton_Click(object sender, EventArgs e)
        {
            int selectedIndex = groupsListView.SelectedIndices[0];

            GroupNameForm nameForm = new GroupNameForm(selectedGroup.Item1);

            nameForm.ShowDialog();

            _model.RenameInclusionGroup(selectedGroup.Item1, nameForm.GetName());

            DisplayGroups();

            selectedGroup = inclusionGroups[selectedIndex];
            groupsListView.Items[selectedIndex].Selected = true;

            nameForm.Dispose();
        }

        private void deleteGroupButton_Click(object sender, EventArgs e)
        {
            _model.DeleteInclusionGroup(selectedGroup.Item1);

            NoActiveGroupButtons();

            DisplayGroups();
        }

        private void ActiveGroupButtons()
        {
            changeColourButton.Enabled = true;
            renameGroupButton.Enabled = true;
            deleteGroupButton.Enabled = true;
        }

        private void NoActiveGroupButtons()
        {
            changeColourButton.Enabled = false;
            renameGroupButton.Enabled = false;
            deleteGroupButton.Enabled = false;
        }
    }
}
