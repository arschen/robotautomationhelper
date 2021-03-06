﻿using System.ComponentModel;
using System.Windows.Forms;
using RobotAutomationHelper.Scripts.CustomControls;

namespace RobotAutomationHelper.Forms
{
    partial class LibrariesForm
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
            this.Save = new ButtonWithToolTip();
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
            this.Save.Click += new System.EventHandler(this.Save_Click);
            // 
            // Skip
            // 
            this.Skip.Location = new System.Drawing.Point(672, 419);
            this.Skip.Name = "Skip";
            this.Skip.Size = new System.Drawing.Size(100, 30);
            this.Skip.TabIndex = 1;
            this.Skip.Text = "Skip";
            this.Skip.UseVisualStyleBackColor = true;
            this.Skip.Click += new System.EventHandler(this.Skip_Click);
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
            // LibrariesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 461);
            this.Controls.Add(this.EXTLIBlabel);
            this.Controls.Add(this.STDLIBlabel);
            this.Controls.Add(this.Skip);
            this.Controls.Add(this.Save);
            this.Name = "LibrariesForm";
            this.Text = "LIbrariesFom";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ButtonWithToolTip Save;
        private Button Skip;
        private Label STDLIBlabel;
        private Label EXTLIBlabel;
    }
}