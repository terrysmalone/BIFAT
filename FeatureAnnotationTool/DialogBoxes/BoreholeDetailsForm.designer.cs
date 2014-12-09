namespace FeatureAnnotationTool
{
    partial class BoreholeDetailsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BoreholeDetailsForm));
            this.doneButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.nameTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.startDepthTextBox = new System.Windows.Forms.MaskedTextBox();
            this.endDepthTextBox = new System.Windows.Forms.MaskedTextBox();
            this.depthResolutionTextBox = new System.Windows.Forms.MaskedTextBox();
            this.SuspendLayout();
            // 
            // doneButton
            // 
            this.doneButton.Location = new System.Drawing.Point(303, 133);
            this.doneButton.Name = "doneButton";
            this.doneButton.Size = new System.Drawing.Size(75, 23);
            this.doneButton.TabIndex = 1;
            this.doneButton.Text = "Done";
            this.doneButton.UseVisualStyleBackColor = true;
            this.doneButton.Click += new System.EventHandler(this.doneButton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(80, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Borehole Name";
            // 
            // nameTextBox
            // 
            this.nameTextBox.Location = new System.Drawing.Point(127, 20);
            this.nameTextBox.Name = "nameTextBox";
            this.nameTextBox.Size = new System.Drawing.Size(251, 20);
            this.nameTextBox.TabIndex = 3;
            this.nameTextBox.TextChanged += new System.EventHandler(this.nameTextBox_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 49);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(86, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Start Depth (mm)";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 75);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(83, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "End Depth (mm)";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 101);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(89, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Depth Resolution";
            // 
            // startDepthTextBox
            // 
            this.startDepthTextBox.BeepOnError = true;
            this.startDepthTextBox.Location = new System.Drawing.Point(127, 46);
            this.startDepthTextBox.Name = "startDepthTextBox";
            this.startDepthTextBox.PromptChar = ' ';
            this.startDepthTextBox.Size = new System.Drawing.Size(251, 20);
            this.startDepthTextBox.TabIndex = 10;
            this.startDepthTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.startDepthTextBox_KeyDown);
            this.startDepthTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.startDepthTextBox_KeyPress);
            this.startDepthTextBox.Leave += new System.EventHandler(this.startDepthTextBox_Leave);
            // 
            // endDepthTextBox
            // 
            this.endDepthTextBox.Location = new System.Drawing.Point(127, 72);
            this.endDepthTextBox.Name = "endDepthTextBox";
            this.endDepthTextBox.PromptChar = ' ';
            this.endDepthTextBox.Size = new System.Drawing.Size(251, 20);
            this.endDepthTextBox.TabIndex = 11;
            this.endDepthTextBox.Leave += new System.EventHandler(this.endDepthTextBox_Leave);
            // 
            // depthResolutionTextBox
            // 
            this.depthResolutionTextBox.Location = new System.Drawing.Point(127, 98);
            this.depthResolutionTextBox.Mask = "0";
            this.depthResolutionTextBox.Name = "depthResolutionTextBox";
            this.depthResolutionTextBox.PromptChar = ' ';
            this.depthResolutionTextBox.Size = new System.Drawing.Size(251, 20);
            this.depthResolutionTextBox.TabIndex = 12;
            this.depthResolutionTextBox.Leave += new System.EventHandler(this.depthResolutionTextBox_Leave);
            // 
            // BoreholeDetailsForm
            // 
            this.AcceptButton = this.doneButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(391, 171);
            this.Controls.Add(this.depthResolutionTextBox);
            this.Controls.Add(this.endDepthTextBox);
            this.Controls.Add(this.startDepthTextBox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.nameTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.doneButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "BoreholeDetailsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Borehole Details";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.BoreholeDetailsForm_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button doneButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox nameTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.MaskedTextBox startDepthTextBox;
        private System.Windows.Forms.MaskedTextBox endDepthTextBox;
        private System.Windows.Forms.MaskedTextBox depthResolutionTextBox;
    }
}