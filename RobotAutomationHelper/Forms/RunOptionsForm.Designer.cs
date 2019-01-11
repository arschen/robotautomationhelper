namespace RobotAutomationHelper.Forms
{
    partial class RunOptionsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RunOptionsForm));
            this.RunOptionAgendaLabel = new System.Windows.Forms.Label();
            this.RunOptionsLabel = new System.Windows.Forms.Label();
            this.RunOptionsText = new System.Windows.Forms.TextBox();
            this.Save = new System.Windows.Forms.Button();
            this.CloseButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // RunOptionAgendaLabel
            // 
            this.RunOptionAgendaLabel.AutoSize = true;
            this.RunOptionAgendaLabel.Location = new System.Drawing.Point(12, 53);
            this.RunOptionAgendaLabel.Name = "RunOptionAgendaLabel";
            this.RunOptionAgendaLabel.Size = new System.Drawing.Size(250, 351);
            this.RunOptionAgendaLabel.TabIndex = 0;
            this.RunOptionAgendaLabel.Text = resources.GetString("RunOptionAgendaLabel.Text");
            // 
            // RunOptionsLabel
            // 
            this.RunOptionsLabel.AutoSize = true;
            this.RunOptionsLabel.Location = new System.Drawing.Point(12, 9);
            this.RunOptionsLabel.Name = "RunOptionsLabel";
            this.RunOptionsLabel.Size = new System.Drawing.Size(67, 13);
            this.RunOptionsLabel.TabIndex = 1;
            this.RunOptionsLabel.Text = "Run options:";
            // 
            // RunOptionsText
            // 
            this.RunOptionsText.Location = new System.Drawing.Point(12, 30);
            this.RunOptionsText.Name = "RunOptionsText";
            this.RunOptionsText.Size = new System.Drawing.Size(500, 20);
            this.RunOptionsText.TabIndex = 2;
            // 
            // Save
            // 
            this.Save.Location = new System.Drawing.Point(518, 24);
            this.Save.Name = "Save";
            this.Save.Size = new System.Drawing.Size(100, 30);
            this.Save.TabIndex = 3;
            this.Save.Text = "Save";
            this.Save.UseVisualStyleBackColor = true;
            this.Save.Click += new System.EventHandler(this.Save_Click);
            // 
            // Close
            // 
            this.CloseButton.Location = new System.Drawing.Point(624, 24);
            this.CloseButton.Name = "Close";
            this.CloseButton.Size = new System.Drawing.Size(100, 30);
            this.CloseButton.TabIndex = 4;
            this.CloseButton.Text = "Close";
            this.CloseButton.UseVisualStyleBackColor = true;
            this.CloseButton.Click += new System.EventHandler(this.Close_Click);
            // 
            // RunOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 461);
            this.Controls.Add(this.CloseButton);
            this.Controls.Add(this.Save);
            this.Controls.Add(this.RunOptionsText);
            this.Controls.Add(this.RunOptionsLabel);
            this.Controls.Add(this.RunOptionAgendaLabel);
            this.Name = "RunOptions";
            this.Text = "RunOptions";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label RunOptionAgendaLabel;
        private System.Windows.Forms.Label RunOptionsLabel;
        private System.Windows.Forms.TextBox RunOptionsText;
        private System.Windows.Forms.Button Save;
        private System.Windows.Forms.Button CloseButton;
    }
}