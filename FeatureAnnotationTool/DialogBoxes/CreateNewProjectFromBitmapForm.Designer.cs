namespace FeatureAnnotationTool.DialogBoxes
{
    partial class CreateNewProjectFromBitmapForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CreateNewProjectFromBitmapForm));
            this.finishButton = new System.Windows.Forms.Button();
            this.boreholeDetailsGroupBox = new System.Windows.Forms.GroupBox();
            this.depthResolutionTextBox = new System.Windows.Forms.MaskedTextBox();
            this.endDepthTextBox = new System.Windows.Forms.MaskedTextBox();
            this.startDepthTextBox = new System.Windows.Forms.MaskedTextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.nameTextBox = new System.Windows.Forms.TextBox();
            this.nameLabel = new System.Windows.Forms.Label();
            this.projectGroupBox = new System.Windows.Forms.GroupBox();
            this.browseLocationButton = new System.Windows.Forms.Button();
            this.projectLocationTextBox = new System.Windows.Forms.TextBox();
            this.projectNameTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.boreholeDetailsGroupBox.SuspendLayout();
            this.projectGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // finishButton
            // 
            this.finishButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.finishButton.Location = new System.Drawing.Point(541, 309);
            this.finishButton.Name = "finishButton";
            this.finishButton.Size = new System.Drawing.Size(75, 23);
            this.finishButton.TabIndex = 0;
            this.finishButton.Text = "OK";
            this.finishButton.UseVisualStyleBackColor = true;
            this.finishButton.Click += new System.EventHandler(this.finishButton_Click);
            // 
            // boreholeDetailsGroupBox
            // 
            this.boreholeDetailsGroupBox.Controls.Add(this.depthResolutionTextBox);
            this.boreholeDetailsGroupBox.Controls.Add(this.endDepthTextBox);
            this.boreholeDetailsGroupBox.Controls.Add(this.startDepthTextBox);
            this.boreholeDetailsGroupBox.Controls.Add(this.label5);
            this.boreholeDetailsGroupBox.Controls.Add(this.label4);
            this.boreholeDetailsGroupBox.Controls.Add(this.label3);
            this.boreholeDetailsGroupBox.Controls.Add(this.nameTextBox);
            this.boreholeDetailsGroupBox.Controls.Add(this.nameLabel);
            this.boreholeDetailsGroupBox.Location = new System.Drawing.Point(18, 21);
            this.boreholeDetailsGroupBox.Name = "boreholeDetailsGroupBox";
            this.boreholeDetailsGroupBox.Size = new System.Drawing.Size(598, 152);
            this.boreholeDetailsGroupBox.TabIndex = 21;
            this.boreholeDetailsGroupBox.TabStop = false;
            this.boreholeDetailsGroupBox.Text = "Borehole Details";
            // 
            // depthResolutionTextBox
            // 
            this.depthResolutionTextBox.Location = new System.Drawing.Point(134, 110);
            this.depthResolutionTextBox.Mask = "0";
            this.depthResolutionTextBox.Name = "depthResolutionTextBox";
            this.depthResolutionTextBox.PromptChar = ' ';
            this.depthResolutionTextBox.Size = new System.Drawing.Size(454, 20);
            this.depthResolutionTextBox.TabIndex = 28;
            this.depthResolutionTextBox.Leave += new System.EventHandler(this.depthResolutionTextBox_Leave);
            // 
            // endDepthTextBox
            // 
            this.endDepthTextBox.Location = new System.Drawing.Point(134, 84);
            this.endDepthTextBox.Name = "endDepthTextBox";
            this.endDepthTextBox.PromptChar = ' ';
            this.endDepthTextBox.Size = new System.Drawing.Size(454, 20);
            this.endDepthTextBox.TabIndex = 27;
            this.endDepthTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.endDepthTextBox_KeyDown);
            this.endDepthTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.endDepthTextBox_KeyPress);
            this.endDepthTextBox.Leave += new System.EventHandler(this.endDepthTextBox_Leave);
            // 
            // startDepthTextBox
            // 
            this.startDepthTextBox.BeepOnError = true;
            this.startDepthTextBox.Location = new System.Drawing.Point(134, 58);
            this.startDepthTextBox.Name = "startDepthTextBox";
            this.startDepthTextBox.PromptChar = ' ';
            this.startDepthTextBox.Size = new System.Drawing.Size(454, 20);
            this.startDepthTextBox.TabIndex = 26;
            this.startDepthTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.startDepthTextBox_KeyDown);
            this.startDepthTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.startDepthTextBox_KeyPress);
            this.startDepthTextBox.Leave += new System.EventHandler(this.startDepthTextBox_Leave);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(19, 113);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(89, 13);
            this.label5.TabIndex = 25;
            this.label5.Text = "Depth Resolution";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(19, 87);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(83, 13);
            this.label4.TabIndex = 24;
            this.label4.Text = "End Depth (mm)";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(19, 61);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(86, 13);
            this.label3.TabIndex = 23;
            this.label3.Text = "Start Depth (mm)";
            // 
            // nameTextBox
            // 
            this.nameTextBox.Location = new System.Drawing.Point(134, 32);
            this.nameTextBox.Name = "nameTextBox";
            this.nameTextBox.Size = new System.Drawing.Size(454, 20);
            this.nameTextBox.TabIndex = 22;
            this.nameTextBox.Leave += new System.EventHandler(this.nameTextBox_Leave);
            // 
            // nameLabel
            // 
            this.nameLabel.AutoSize = true;
            this.nameLabel.Location = new System.Drawing.Point(19, 32);
            this.nameLabel.Name = "nameLabel";
            this.nameLabel.Size = new System.Drawing.Size(80, 13);
            this.nameLabel.TabIndex = 21;
            this.nameLabel.Text = "Borehole Name";
            // 
            // projectGroupBox
            // 
            this.projectGroupBox.Controls.Add(this.browseLocationButton);
            this.projectGroupBox.Controls.Add(this.projectLocationTextBox);
            this.projectGroupBox.Controls.Add(this.projectNameTextBox);
            this.projectGroupBox.Controls.Add(this.label2);
            this.projectGroupBox.Controls.Add(this.label1);
            this.projectGroupBox.Location = new System.Drawing.Point(18, 190);
            this.projectGroupBox.Name = "projectGroupBox";
            this.projectGroupBox.Size = new System.Drawing.Size(598, 104);
            this.projectGroupBox.TabIndex = 22;
            this.projectGroupBox.TabStop = false;
            this.projectGroupBox.Text = "Project";
            // 
            // browseLocationButton
            // 
            this.browseLocationButton.Location = new System.Drawing.Point(513, 63);
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
            // CreateNewProjectFromBitmapForm
            // 
            this.AcceptButton = this.finishButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(628, 344);
            this.Controls.Add(this.projectGroupBox);
            this.Controls.Add(this.boreholeDetailsGroupBox);
            this.Controls.Add(this.finishButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "CreateNewProjectFromBitmapForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "New Project";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CreateNewProjectForm_FormClosing);
            this.boreholeDetailsGroupBox.ResumeLayout(false);
            this.boreholeDetailsGroupBox.PerformLayout();
            this.projectGroupBox.ResumeLayout(false);
            this.projectGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button finishButton;
        private System.Windows.Forms.GroupBox boreholeDetailsGroupBox;
        private System.Windows.Forms.MaskedTextBox depthResolutionTextBox;
        private System.Windows.Forms.MaskedTextBox endDepthTextBox;
        private System.Windows.Forms.MaskedTextBox startDepthTextBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox nameTextBox;
        private System.Windows.Forms.Label nameLabel;
        private System.Windows.Forms.GroupBox projectGroupBox;
        private System.Windows.Forms.Button browseLocationButton;
        private System.Windows.Forms.TextBox projectLocationTextBox;
        private System.Windows.Forms.TextBox projectNameTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
    }
}