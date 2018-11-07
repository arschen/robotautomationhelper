namespace RobotAutomationHelper.Forms
{
    partial class LIbrariesFom
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
            this.Skip = new System.Windows.Forms.Button();
            this.STDLIBlabel = new System.Windows.Forms.Label();
            this.EXTLIBlabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // Save
            // 
            this.Save.Location = new System.Drawing.Point(566, 419);
            this.Save.Name = "Save";
            this.Save.Size = new System.Drawing.Size(100, 30);
            this.Save.TabIndex = 0;
            this.Save.Text = "Save";
            this.Save.UseVisualStyleBackColor = true;
            // 
            // Skip
            // 
            this.Skip.Location = new System.Drawing.Point(672, 419);
            this.Skip.Name = "Skip";
            this.Skip.Size = new System.Drawing.Size(100, 30);
            this.Skip.TabIndex = 1;
            this.Skip.Text = "Skip";
            this.Skip.UseVisualStyleBackColor = true;
            // 
            // STDLIBlabel
            // 
            this.STDLIBlabel.AutoSize = true;
            this.STDLIBlabel.Location = new System.Drawing.Point(12, 9);
            this.STDLIBlabel.Name = "STDLIBlabel";
            this.STDLIBlabel.Size = new System.Drawing.Size(88, 13);
            this.STDLIBlabel.TabIndex = 2;
            this.STDLIBlabel.Text = "Standard libraries";
            // 
            // EXTLIBlabel
            // 
            this.EXTLIBlabel.AutoSize = true;
            this.EXTLIBlabel.Location = new System.Drawing.Point(250, 9);
            this.EXTLIBlabel.Name = "EXTLIBlabel";
            this.EXTLIBlabel.Size = new System.Drawing.Size(83, 13);
            this.EXTLIBlabel.TabIndex = 3;
            this.EXTLIBlabel.Text = "External libraries";
            // 
            // LIbrariesFom
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 461);
            this.Controls.Add(this.EXTLIBlabel);
            this.Controls.Add(this.STDLIBlabel);
            this.Controls.Add(this.Skip);
            this.Controls.Add(this.Save);
            this.Name = "LIbrariesFom";
            this.Text = "LIbrariesFom";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Scripts.CustomControls.ButtonWithToolTip Save;
        private System.Windows.Forms.Button Skip;
        private System.Windows.Forms.Label STDLIBlabel;
        private System.Windows.Forms.Label EXTLIBlabel;
    }
}