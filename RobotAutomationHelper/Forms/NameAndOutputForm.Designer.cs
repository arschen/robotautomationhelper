namespace RobotAutomationHelper.Forms
{
    partial class NameAndOutputForm
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
            this.OutputFile = new System.Windows.Forms.ComboBox();
            this.OutputLabel = new System.Windows.Forms.Label();
            this.NameLabel = new System.Windows.Forms.Label();
            this.ContentName = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // OutputFile
            // 
            this.OutputFile.FormattingEnabled = true;
            this.OutputFile.Location = new System.Drawing.Point(32, 62);
            this.OutputFile.Name = "OutputFile";
            this.OutputFile.Size = new System.Drawing.Size(280, 21);
            this.OutputFile.TabIndex = 22;
            // 
            // OutputLabel
            // 
            this.OutputLabel.AutoSize = true;
            this.OutputLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OutputLabel.Location = new System.Drawing.Point(12, 47);
            this.OutputLabel.Name = "OutputLabel";
            this.OutputLabel.Size = new System.Drawing.Size(105, 13);
            this.OutputLabel.TabIndex = 21;
            this.OutputLabel.Text = "Keyword Output File:";
            // 
            // NameLabel
            // 
            this.NameLabel.AutoSize = true;
            this.NameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NameLabel.Location = new System.Drawing.Point(12, 9);
            this.NameLabel.Name = "NameLabel";
            this.NameLabel.Size = new System.Drawing.Size(82, 13);
            this.NameLabel.TabIndex = 20;
            this.NameLabel.Text = "Keyword Name:";
            // 
            // Name
            // 
            this.ContentName.Location = new System.Drawing.Point(32, 24);
            this.ContentName.Name = "Name";
            this.ContentName.Size = new System.Drawing.Size(280, 20);
            this.ContentName.TabIndex = 19;
            // 
            // NameAndOutputForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(334, 111);
            this.Controls.Add(this.OutputFile);
            this.Controls.Add(this.OutputLabel);
            this.Controls.Add(this.NameLabel);
            this.Controls.Add(this.ContentName);
            this.Name = "NameAndOutputForm";
            this.Text = "Enter Name and Output File";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox OutputFile;
        private System.Windows.Forms.Label OutputLabel;
        private System.Windows.Forms.Label NameLabel;
        private System.Windows.Forms.TextBox ContentName;
    }
}