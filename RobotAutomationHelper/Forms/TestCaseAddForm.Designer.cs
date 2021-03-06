﻿using System.ComponentModel;
using System.Windows.Forms;
using RobotAutomationHelper.Scripts.CustomControls;

namespace RobotAutomationHelper.Forms
{
    partial class TestCaseAddForm
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
            this.TestCaseNameLabel = new System.Windows.Forms.Label();
            this.Skip = new System.Windows.Forms.Button();
            this.Save = new ButtonWithToolTip();
            this.TestCaseName = new System.Windows.Forms.TextBox();
            this.TestCaseAddLabel = new System.Windows.Forms.Label();
            this.TestCaseDocumentation = new System.Windows.Forms.TextBox();
            this.TestStepsLabel = new System.Windows.Forms.Label();
            this.TestDocumentationLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.TestCaseTags = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.OutputFile = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // TestCaseNameLabel
            // 
            this.TestCaseNameLabel.AutoSize = true;
            this.TestCaseNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TestCaseNameLabel.Location = new System.Drawing.Point(10, 45);
            this.TestCaseNameLabel.Name = "TestCaseNameLabel";
            this.TestCaseNameLabel.Size = new System.Drawing.Size(89, 13);
            this.TestCaseNameLabel.TabIndex = 4;
            this.TestCaseNameLabel.Text = "Test Case Name:";
            // 
            // Skip
            // 
            this.Skip.Location = new System.Drawing.Point(626, 92);
            this.Skip.Name = "Skip";
            this.Skip.Size = new System.Drawing.Size(100, 30);
            this.Skip.TabIndex = 3;
            this.Skip.Text = "Skip Test Case";
            this.Skip.UseVisualStyleBackColor = true;
            this.Skip.Click += new System.EventHandler(this.Skip_Click);
            // 
            // Save
            // 
            this.Save.Location = new System.Drawing.Point(626, 54);
            this.Save.Name = "Save";
            this.Save.Size = new System.Drawing.Size(100, 30);
            this.Save.TabIndex = 2;
            this.Save.Text = "Save Test Case";
            this.Save.UseVisualStyleBackColor = true;
            this.Save.Click += new System.EventHandler(this.Save_Click);
            // 
            // TestCaseName
            // 
            this.TestCaseName.Location = new System.Drawing.Point(30, 60);
            this.TestCaseName.Name = "TestCaseName";
            this.TestCaseName.Size = new System.Drawing.Size(280, 20);
            this.TestCaseName.TabIndex = 1;
            this.TestCaseName.TextChanged += new System.EventHandler(this.TestCaseName_TextChanged);
            // 
            // TestCaseAddLabel
            // 
            this.TestCaseAddLabel.AutoSize = true;
            this.TestCaseAddLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TestCaseAddLabel.Location = new System.Drawing.Point(10, 9);
            this.TestCaseAddLabel.Margin = new System.Windows.Forms.Padding(1, 0, 3, 0);
            this.TestCaseAddLabel.Name = "TestCaseAddLabel";
            this.TestCaseAddLabel.Size = new System.Drawing.Size(163, 20);
            this.TestCaseAddLabel.TabIndex = 0;
            this.TestCaseAddLabel.Text = "Test Case Creation";
            this.TestCaseAddLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // TestCaseDocumentation
            // 
            this.TestCaseDocumentation.Location = new System.Drawing.Point(340, 60);
            this.TestCaseDocumentation.Name = "TestCaseDocumentation";
            this.TestCaseDocumentation.Size = new System.Drawing.Size(280, 20);
            this.TestCaseDocumentation.TabIndex = 6;
            // 
            // TestStepsLabel
            // 
            this.TestStepsLabel.AutoSize = true;
            this.TestStepsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TestStepsLabel.Location = new System.Drawing.Point(10, 122);
            this.TestStepsLabel.Name = "TestStepsLabel";
            this.TestStepsLabel.Size = new System.Drawing.Size(61, 13);
            this.TestStepsLabel.TabIndex = 7;
            this.TestStepsLabel.Text = "Test Steps:";
            // 
            // TestDocumentationLabel
            // 
            this.TestDocumentationLabel.AutoSize = true;
            this.TestDocumentationLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TestDocumentationLabel.Location = new System.Drawing.Point(320, 45);
            this.TestDocumentationLabel.Name = "TestDocumentationLabel";
            this.TestDocumentationLabel.Size = new System.Drawing.Size(106, 13);
            this.TestDocumentationLabel.TabIndex = 8;
            this.TestDocumentationLabel.Text = "Test Documentation:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(320, 83);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(85, 13);
            this.label1.TabIndex = 12;
            this.label1.Text = "Test Case Tags:";
            // 
            // TestCaseTags
            // 
            this.TestCaseTags.Location = new System.Drawing.Point(340, 98);
            this.TestCaseTags.Name = "TestCaseTags";
            this.TestCaseTags.Size = new System.Drawing.Size(280, 20);
            this.TestCaseTags.TabIndex = 11;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(10, 83);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(112, 13);
            this.label2.TabIndex = 14;
            this.label2.Text = "Test Case Output File:";
            // 
            // OutputFile
            // 
            this.OutputFile.FormattingEnabled = true;
            this.OutputFile.Location = new System.Drawing.Point(30, 98);
            this.OutputFile.Name = "OutputFile";
            this.OutputFile.Size = new System.Drawing.Size(280, 21);
            this.OutputFile.TabIndex = 15;
            this.OutputFile.TextChanged += new System.EventHandler(this.TestCaseOutputFile_TextChanged);
            // 
            // TestCaseAddForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(784, 461);
            this.Controls.Add(this.OutputFile);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.TestCaseTags);
            this.Controls.Add(this.TestDocumentationLabel);
            this.Controls.Add(this.TestStepsLabel);
            this.Controls.Add(this.TestCaseDocumentation);
            this.Controls.Add(this.Skip);
            this.Controls.Add(this.TestCaseNameLabel);
            this.Controls.Add(this.Save);
            this.Controls.Add(this.TestCaseName);
            this.Controls.Add(this.TestCaseAddLabel);
            this.Name = "TestCaseAddForm";
            this.Text = "Edit Test Case";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private Label TestCaseNameLabel;
        private Button Skip;
        private Button Save;
        private TextBox TestCaseName;
        private Label TestCaseAddLabel;
        private TextBox TestCaseDocumentation;
        private Label TestStepsLabel;
        private Label TestDocumentationLabel;
        private Label label1;
        private TextBox TestCaseTags;
        private Label label2;
        private ComboBox OutputFile;
    }
}