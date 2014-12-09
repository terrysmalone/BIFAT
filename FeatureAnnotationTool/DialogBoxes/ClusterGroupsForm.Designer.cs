namespace FeatureAnnotationTool.DialogBoxes
{
    partial class ClusterGroupsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ClusterGroupsForm));
            this.groupsListView = new System.Windows.Forms.ListView();
            this.groupName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.layersInGroup = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.addGroupButton = new System.Windows.Forms.Button();
            this.deleteGroupButton = new System.Windows.Forms.Button();
            this.renameGroupButton = new System.Windows.Forms.Button();
            this.changeColourButton = new System.Windows.Forms.Button();
            this.doneButton = new System.Windows.Forms.Button();
            this.colourPicker = new System.Windows.Forms.ColorDialog();
            this.SuspendLayout();
            // 
            // groupsListView
            // 
            this.groupsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.groupName,
            this.layersInGroup});
            this.groupsListView.Location = new System.Drawing.Point(12, 12);
            this.groupsListView.MultiSelect = false;
            this.groupsListView.Name = "groupsListView";
            this.groupsListView.Size = new System.Drawing.Size(403, 176);
            this.groupsListView.TabIndex = 1;
            this.groupsListView.UseCompatibleStateImageBehavior = false;
            this.groupsListView.SelectedIndexChanged += new System.EventHandler(this.groupsListView_SelectedIndexChanged);
            // 
            // groupName
            // 
            this.groupName.Text = "Name";
            this.groupName.Width = 150;
            // 
            // layersInGroup
            // 
            this.layersInGroup.Text = "Number of Clusters in Group";
            this.layersInGroup.Width = 200;
            // 
            // addGroupButton
            // 
            this.addGroupButton.Location = new System.Drawing.Point(12, 194);
            this.addGroupButton.Name = "addGroupButton";
            this.addGroupButton.Size = new System.Drawing.Size(90, 23);
            this.addGroupButton.TabIndex = 2;
            this.addGroupButton.Text = "Add group";
            this.addGroupButton.UseVisualStyleBackColor = true;
            this.addGroupButton.Click += new System.EventHandler(this.addGroupButton_Click);
            // 
            // deleteGroupButton
            // 
            this.deleteGroupButton.Enabled = false;
            this.deleteGroupButton.Location = new System.Drawing.Point(108, 194);
            this.deleteGroupButton.Name = "deleteGroupButton";
            this.deleteGroupButton.Size = new System.Drawing.Size(90, 23);
            this.deleteGroupButton.TabIndex = 3;
            this.deleteGroupButton.Text = "Delete group";
            this.deleteGroupButton.UseVisualStyleBackColor = true;
            this.deleteGroupButton.Click += new System.EventHandler(this.deleteGroupButton_Click);
            // 
            // renameGroupButton
            // 
            this.renameGroupButton.Enabled = false;
            this.renameGroupButton.Location = new System.Drawing.Point(204, 194);
            this.renameGroupButton.Name = "renameGroupButton";
            this.renameGroupButton.Size = new System.Drawing.Size(90, 23);
            this.renameGroupButton.TabIndex = 4;
            this.renameGroupButton.Text = "Rename group";
            this.renameGroupButton.UseVisualStyleBackColor = true;
            this.renameGroupButton.Click += new System.EventHandler(this.renameGroupButton_Click);
            // 
            // changeColourButton
            // 
            this.changeColourButton.Enabled = false;
            this.changeColourButton.Location = new System.Drawing.Point(300, 194);
            this.changeColourButton.Name = "changeColourButton";
            this.changeColourButton.Size = new System.Drawing.Size(90, 23);
            this.changeColourButton.TabIndex = 5;
            this.changeColourButton.Text = "Change colour";
            this.changeColourButton.UseVisualStyleBackColor = true;
            this.changeColourButton.Click += new System.EventHandler(this.changeColourButton_Click);
            // 
            // doneButton
            // 
            this.doneButton.Location = new System.Drawing.Point(340, 238);
            this.doneButton.Name = "doneButton";
            this.doneButton.Size = new System.Drawing.Size(75, 23);
            this.doneButton.TabIndex = 6;
            this.doneButton.Text = "Done";
            this.doneButton.UseVisualStyleBackColor = true;
            this.doneButton.Click += new System.EventHandler(this.doneButton_Click);
            // 
            // ClusterGroupsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(425, 273);
            this.Controls.Add(this.doneButton);
            this.Controls.Add(this.changeColourButton);
            this.Controls.Add(this.renameGroupButton);
            this.Controls.Add(this.deleteGroupButton);
            this.Controls.Add(this.addGroupButton);
            this.Controls.Add(this.groupsListView);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "ClusterGroupsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Cluster Groups";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView groupsListView;
        private System.Windows.Forms.ColumnHeader groupName;
        private System.Windows.Forms.ColumnHeader layersInGroup;
        private System.Windows.Forms.Button addGroupButton;
        private System.Windows.Forms.Button deleteGroupButton;
        private System.Windows.Forms.Button renameGroupButton;
        private System.Windows.Forms.Button changeColourButton;
        private System.Windows.Forms.Button doneButton;
        private System.Windows.Forms.ColorDialog colourPicker;


    }
}