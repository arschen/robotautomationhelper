﻿namespace RobotAutomationHelper
{
    partial class RobotAutomationHelper
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
            this.MainMenu = new System.Windows.Forms.MenuStrip();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToRobotToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.IndexLabel = new System.Windows.Forms.Label();
            this.TestCaseNameLabel = new System.Windows.Forms.Label();
            this.AddLabel = new System.Windows.Forms.Label();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.MainMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainMenu
            // 
            this.MainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1});
            this.MainMenu.Location = new System.Drawing.Point(0, 0);
            this.MainMenu.Name = "MainMenu";
            this.MainMenu.Size = new System.Drawing.Size(784, 24);
            this.MainMenu.TabIndex = 0;
            this.MainMenu.Text = "menuStrip1";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem2,
            this.saveToRobotToolStripMenuItem});
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(37, 20);
            this.toolStripMenuItem1.Text = "File";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(147, 22);
            this.toolStripMenuItem2.Text = "Open";
            this.toolStripMenuItem2.Click += new System.EventHandler(this.ToolStripMenuOpen_Click);
            // 
            // saveToRobotToolStripMenuItem
            // 
            this.saveToRobotToolStripMenuItem.Name = "saveToRobotToolStripMenuItem";
            this.saveToRobotToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.saveToRobotToolStripMenuItem.Text = "Save to Robot";
            this.saveToRobotToolStripMenuItem.Click += new System.EventHandler(this.SaveToRobotToolStripMenuItem_Click);
            // 
            // openFileDialog
            // 
            this.openFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.OpenFileDialog1_FileOk);
            // 
            // IndexLabel
            // 
            this.IndexLabel.AutoSize = true;
            this.IndexLabel.Location = new System.Drawing.Point(15, 30);
            this.IndexLabel.Name = "IndexLabel";
            this.IndexLabel.Size = new System.Drawing.Size(14, 13);
            this.IndexLabel.TabIndex = 4;
            this.IndexLabel.Text = "#";
            this.IndexLabel.Visible = false;
            // 
            // TestCaseNameLabel
            // 
            this.TestCaseNameLabel.AutoSize = true;
            this.TestCaseNameLabel.Location = new System.Drawing.Point(30, 30);
            this.TestCaseNameLabel.Name = "TestCaseNameLabel";
            this.TestCaseNameLabel.Size = new System.Drawing.Size(86, 13);
            this.TestCaseNameLabel.TabIndex = 5;
            this.TestCaseNameLabel.Text = "Test Case Name";
            this.TestCaseNameLabel.Visible = false;
            // 
            // AddLabel
            // 
            this.AddLabel.AutoSize = true;
            this.AddLabel.Location = new System.Drawing.Point(325, 30);
            this.AddLabel.Name = "AddLabel";
            this.AddLabel.Size = new System.Drawing.Size(29, 13);
            this.AddLabel.TabIndex = 6;
            this.AddLabel.Text = "Add:";
            this.AddLabel.Visible = false;
            // 
            // folderBrowserDialog1
            // 
            this.folderBrowserDialog1.Description = "Choose Robot Output Folder";
            // 
            // ApplicationMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(784, 461);
            this.Controls.Add(this.AddLabel);
            this.Controls.Add(this.TestCaseNameLabel);
            this.Controls.Add(this.IndexLabel);
            this.Controls.Add(this.MainMenu);
            this.MainMenuStrip = this.MainMenu;
            this.Name = "ApplicationMain";
            this.Text = "Robot Automation Helper";
            this.Load += new System.EventHandler(this.ApplicationMain_Load);
            this.MainMenu.ResumeLayout(false);
            this.MainMenu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip MainMenu;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.Label IndexLabel;
        private System.Windows.Forms.Label TestCaseNameLabel;
        private System.Windows.Forms.Label AddLabel;
        private System.Windows.Forms.ToolStripMenuItem saveToRobotToolStripMenuItem;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
    }
}

