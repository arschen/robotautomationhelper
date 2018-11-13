using RobotAutomationHelper.Scripts.CustomControls;

namespace RobotAutomationHelper.Forms
{
    partial class VariablesAddForm
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
            this.AddVariablesLabel = new System.Windows.Forms.Label();
            this.Save = new ButtonWithToolTip();
            this.SaveAndExit = new ButtonWithToolTip();
            this.SuspendLayout();
            // 
            // OutputFile
            // 
            this.OutputFile.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.OutputFile.FormattingEnabled = true;
            this.OutputFile.Location = new System.Drawing.Point(31, 60);
            this.OutputFile.Name = "OutputFile";
            this.OutputFile.Size = new System.Drawing.Size(280, 21);
            this.OutputFile.TabIndex = 28;
            this.OutputFile.SelectedIndexChanged += new System.EventHandler(this.OutputFile_SelectedIndexChanged);
            // 
            // OutputLabel
            // 
            this.OutputLabel.AutoSize = true;
            this.OutputLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OutputLabel.Location = new System.Drawing.Point(11, 45);
            this.OutputLabel.Name = "OutputLabel";
            this.OutputLabel.Size = new System.Drawing.Size(107, 13);
            this.OutputLabel.TabIndex = 27;
            this.OutputLabel.Text = "Variables Output File:";
            // 
            // AddVariablesLabel
            // 
            this.AddVariablesLabel.AutoSize = true;
            this.AddVariablesLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AddVariablesLabel.Location = new System.Drawing.Point(10, 9);
            this.AddVariablesLabel.Margin = new System.Windows.Forms.Padding(1, 0, 3, 0);
            this.AddVariablesLabel.Name = "AddVariablesLabel";
            this.AddVariablesLabel.Size = new System.Drawing.Size(121, 20);
            this.AddVariablesLabel.TabIndex = 26;
            this.AddVariablesLabel.Text = "Edit Variables";
            this.AddVariablesLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Save
            // 
            this.Save.Location = new System.Drawing.Point(317, 58);
            this.Save.Name = "Save";
            this.Save.Size = new System.Drawing.Size(100, 30);
            this.Save.TabIndex = 29;
            this.Save.Text = "Save";
            this.Save.UseVisualStyleBackColor = true;
            this.Save.Click += new System.EventHandler(this.Save_Click);
            // 
            // SaveAndExit
            // 
            this.SaveAndExit.Location = new System.Drawing.Point(423, 58);
            this.SaveAndExit.Name = "SaveAndExit";
            this.SaveAndExit.Size = new System.Drawing.Size(100, 30);
            this.SaveAndExit.TabIndex = 30;
            this.SaveAndExit.Text = "Save and Exit";
            this.SaveAndExit.UseVisualStyleBackColor = true;
            this.SaveAndExit.Click += new System.EventHandler(this.SaveAndExit_Click);
            // 
            // VariablesAddForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 461);
            this.Controls.Add(this.SaveAndExit);
            this.Controls.Add(this.Save);
            this.Controls.Add(this.OutputFile);
            this.Controls.Add(this.OutputLabel);
            this.Controls.Add(this.AddVariablesLabel);
            this.Name = "VariablesAddForm";
            this.Text = "VariablesAddForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox OutputFile;
        private System.Windows.Forms.Label OutputLabel;
        private System.Windows.Forms.Label AddVariablesLabel;
        private Scripts.CustomControls.ButtonWithToolTip Save;
        private Scripts.CustomControls.ButtonWithToolTip SaveAndExit;
    }
}