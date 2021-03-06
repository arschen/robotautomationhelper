﻿using System.ComponentModel;
using System.Windows.Forms;
using RobotAutomationHelper.Scripts.CustomControls;

namespace RobotAutomationHelper.Forms
{
    partial class SettingsAddForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

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
            this.SuiteDocumentationLabel = new System.Windows.Forms.Label();
            this.SuiteDocumentation = new System.Windows.Forms.TextBox();
            this.AddSettingsLabel = new System.Windows.Forms.Label();
            this.SaveAndExit = new ButtonWithToolTip();
            this.Save = new ButtonWithToolTip();
            this.SuspendLayout();
            // 
            // OutputFile
            // 
            this.OutputFile.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.OutputFile.FormattingEnabled = true;
            this.OutputFile.Location = new System.Drawing.Point(31, 60);
            this.OutputFile.Name = "OutputFile";
            this.OutputFile.Size = new System.Drawing.Size(280, 21);
            this.OutputFile.TabIndex = 25;
            this.OutputFile.SelectedIndexChanged += new System.EventHandler(this.OutputFile_SelectedIndexChanged);
            // 
            // OutputLabel
            // 
            this.OutputLabel.AutoSize = true;
            this.OutputLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OutputLabel.Location = new System.Drawing.Point(11, 45);
            this.OutputLabel.Name = "OutputLabel";
            this.OutputLabel.Size = new System.Drawing.Size(102, 13);
            this.OutputLabel.TabIndex = 24;
            this.OutputLabel.Text = "Settings Output File:";
            // 
            // Save
            // 
            this.Save.Location = new System.Drawing.Point(626, 54);
            this.Save.Name = "Save";
            this.Save.Size = new System.Drawing.Size(100, 30);
            this.Save.TabIndex = 22;
            this.Save.Text = "Save Settings";
            this.Save.UseVisualStyleBackColor = true;
            this.Save.Click += new System.EventHandler(this.Save_Click);
            // 
            // SuiteDocumentationLabel
            // 
            this.SuiteDocumentationLabel.AutoSize = true;
            this.SuiteDocumentationLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SuiteDocumentationLabel.Location = new System.Drawing.Point(320, 45);
            this.SuiteDocumentationLabel.Name = "SuiteDocumentationLabel";
            this.SuiteDocumentationLabel.Size = new System.Drawing.Size(109, 13);
            this.SuiteDocumentationLabel.TabIndex = 21;
            this.SuiteDocumentationLabel.Text = "Suite Documentation:";
            // 
            // SuiteDocumentation
            // 
            this.SuiteDocumentation.Location = new System.Drawing.Point(340, 60);
            this.SuiteDocumentation.Name = "SuiteDocumentation";
            this.SuiteDocumentation.Size = new System.Drawing.Size(280, 20);
            this.SuiteDocumentation.TabIndex = 20;
            // 
            // AddSettingsLabel
            // 
            this.AddSettingsLabel.AutoSize = true;
            this.AddSettingsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AddSettingsLabel.Location = new System.Drawing.Point(10, 9);
            this.AddSettingsLabel.Margin = new System.Windows.Forms.Padding(1, 0, 3, 0);
            this.AddSettingsLabel.Name = "AddSettingsLabel";
            this.AddSettingsLabel.Size = new System.Drawing.Size(113, 20);
            this.AddSettingsLabel.TabIndex = 19;
            this.AddSettingsLabel.Text = "Edit Settings";
            this.AddSettingsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // SaveAndExit
            // 
            this.SaveAndExit.Location = new System.Drawing.Point(626, 90);
            this.SaveAndExit.Name = "SaveAndExit";
            this.SaveAndExit.Size = new System.Drawing.Size(100, 30);
            this.SaveAndExit.TabIndex = 26;
            this.SaveAndExit.Text = "Save and Exit";
            this.SaveAndExit.UseVisualStyleBackColor = true;
            this.SaveAndExit.Click += new System.EventHandler(this.SaveAndExit_Click);
            // 
            // SettingsAddForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.SaveAndExit);
            this.Controls.Add(this.OutputFile);
            this.Controls.Add(this.OutputLabel);
            this.Controls.Add(this.Save);
            this.Controls.Add(this.SuiteDocumentationLabel);
            this.Controls.Add(this.SuiteDocumentation);
            this.Controls.Add(this.AddSettingsLabel);
            this.Name = "SettingsAddForm";
            this.Text = "SettingsAddForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ComboBox OutputFile;
        private Label OutputLabel;
        private Label SuiteDocumentationLabel;
        private TextBox SuiteDocumentation;
        private Label AddSettingsLabel;
        private ButtonWithToolTip SaveAndExit;
        private ButtonWithToolTip Save;
    }
}