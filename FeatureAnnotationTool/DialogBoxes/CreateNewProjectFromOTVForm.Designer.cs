namespace FeatureAnnotationTool.DialogBoxes
{
    partial class CreateNewProjectFromOTVForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CreateNewProjectFromOTVForm));
            this.finishButton = new System.Windows.Forms.Button();
            this.projectGroupBox = new System.Windows.Forms.GroupBox();
            this.browseLocationButton = new System.Windows.Forms.Button();
            this.projectLocationTextBox = new System.Windows.Forms.TextBox();
            this.projectNameTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.projectGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // finishButton
            // 
            this.finishButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.finishButton.Location = new System.Drawing.Point(541, 143);
            this.finishButton.Name = "finishButton";
            this.finishButton.Size = new System.Drawing.Size(75, 23);
            this.finishButton.TabIndex = 0;
            this.finishButton.Text = "OK";
            this.finishButton.UseVisualStyleBackColor = true;
            this.finishButton.Click += new System.EventHandler(this.finishButton_Click);
            // 
            // projectGroupBox
            // 
            this.projectGroupBox.Controls.Add(this.browseLocationButton);
            this.projectGroupBox.Controls.Add(this.projectLocationTextBox);
            this.projectGroupBox.Controls.Add(this.projectNameTextBox);
            this.projectGroupBox.Controls.Add(this.label2);
            this.projectGroupBox.Controls.Add(this.label1);
            this.projectGroupBox.Location = new System.Drawing.Point(18, 28);
            this.projectGroupBox.Name = "projectGroupBox";
            this.projectGroupBox.Size = new System.Drawing.Size(598, 104);
            this.projectGroupBox.TabIndex = 22;
            this.projectGroupBox.TabStop = false;
            this.projectGroupBox.Text = "Project";
            // 
            // browseLocationButton
            // 
            this.browseLocationButton.Location = new System.Drawing.Point(513, 60);
            this.browseLocationButton.Name = "browseLocationButton";
            this.browseLocationButton.Size = new System.Drawing.Size(75, 23);
            this.browseLocationButton.TabIndex = 11;
            this.browseLocationButton.Text = "Browse...";
            this.browseLocationButton.UseVisualStyleBackColor = true;
            this.browseLocationButton.Click += new System.EventHandler(this.browseLocationButton_Click_1);
            // 
            // projectLocationTextBox
            // 
            this.projectLocationTextBox.Location = new System.Drawing.Point(134, 63);
            this.projectLocationTextBox.Name = "projectLocationTextBox";
            this.projectLocationTextBox.Size = new System.Drawing.Size(370, 20);
            this.projectLocationTextBox.TabIndex = 10;
            this.projectLocationTextBox.Leave += new System.EventHandler(this.projectLocationTextBox_Leave);
            // 
            // projectNameTextBox
            // 
            this.projectNameTextBox.Location = new System.Drawing.Point(134, 37);
            this.projectNameTextBox.Name = "projectNameTextBox";
            this.projectNameTextBox.Size = new System.Drawing.Size(370, 20);
            this.projectNameTextBox.TabIndex = 9;
            this.projectNameTextBox.Leave += new System.EventHandler(this.projectNameTextBox_Leave);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 66);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Project location:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 43);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Project name:";
            // 
            // CreateNewProjectFromOTVForm
            // 
            this.AcceptButton = this.finishButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(628, 178);
            this.Controls.Add(this.projectGroupBox);
            this.Controls.Add(this.finishButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "CreateNewProjectFromOTVForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "New Project";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CreateNewProjectForm_FormClosing);
            this.projectGroupBox.ResumeLayout(false);
            this.projectGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button finishButton;
        private System.Windows.Forms.GroupBox projectGroupBox;
        private System.Windows.Forms.Button browseLocationButton;
        private System.Windows.Forms.TextBox projectLocationTextBox;
        private System.Windows.Forms.TextBox projectNameTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
    }
}