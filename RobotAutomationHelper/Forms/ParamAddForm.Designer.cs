using RobotAutomationHelper.Scripts.CustomControls;

namespace RobotAutomationHelper.Forms
{
    partial class ParamAddForm
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
            this.Save = new ButtonWithToolTip();
            this.KeywordDocumentationLabel = new System.Windows.Forms.Label();
            this.KeywordDocumentation = new System.Windows.Forms.TextBox();
            this.KeywordNameLabel = new System.Windows.Forms.Label();
            this.KeywordName = new System.Windows.Forms.TextBox();
            this.AddParamLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // Skip
            // 
            this.Skip.Location = new System.Drawing.Point(627, 91);
            this.Skip.Name = "Skip";
            this.Skip.Size = new System.Drawing.Size(100, 30);
            this.Skip.TabIndex = 22;
            this.Skip.Text = "Skip Params";
            this.Skip.UseVisualStyleBackColor = true;
            this.Skip.Click += new System.EventHandler(this.Skip_Click);
            // 
            // Save
            // 
            this.Save.Location = new System.Drawing.Point(627, 55);
            this.Save.Name = "Save";
            this.Save.Size = new System.Drawing.Size(100, 30);
            this.Save.TabIndex = 21;
            this.Save.Text = "Save Params";
            this.Save.UseVisualStyleBackColor = true;
            this.Save.Click += new System.EventHandler(this.Save_Click);
            // 
            // KeywordDocumentationLabel
            // 
            this.KeywordDocumentationLabel.AutoSize = true;
            this.KeywordDocumentationLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.KeywordDocumentationLabel.Location = new System.Drawing.Point(321, 46);
            this.KeywordDocumentationLabel.Name = "KeywordDocumentationLabel";
            this.KeywordDocumentationLabel.Size = new System.Drawing.Size(126, 13);
            this.KeywordDocumentationLabel.TabIndex = 20;
            this.KeywordDocumentationLabel.Text = "Keyword Documentation:";
            // 
            // KeywordDocumentation
            // 
            this.KeywordDocumentation.Location = new System.Drawing.Point(341, 61);
            this.KeywordDocumentation.Name = "KeywordDocumentation";
            this.KeywordDocumentation.ReadOnly = true;
            this.KeywordDocumentation.Size = new System.Drawing.Size(280, 20);
            this.KeywordDocumentation.TabIndex = 19;
            // 
            // KeywordNameLabel
            // 
            this.KeywordNameLabel.AutoSize = true;
            this.KeywordNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.KeywordNameLabel.Location = new System.Drawing.Point(11, 46);
            this.KeywordNameLabel.Name = "KeywordNameLabel";
            this.KeywordNameLabel.Size = new System.Drawing.Size(82, 13);
            this.KeywordNameLabel.TabIndex = 18;
            this.KeywordNameLabel.Text = "Keyword Name:";
            // 
            // KeywordName
            // 
            this.KeywordName.Location = new System.Drawing.Point(31, 61);
            this.KeywordName.Name = "KeywordName";
            this.KeywordName.ReadOnly = true;
            this.KeywordName.Size = new System.Drawing.Size(280, 20);
            this.KeywordName.TabIndex = 17;
            // 
            // AddParamLabel
            // 
            this.AddParamLabel.AutoSize = true;
            this.AddParamLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AddParamLabel.Location = new System.Drawing.Point(11, 10);
            this.AddParamLabel.Margin = new System.Windows.Forms.Padding(1, 0, 3, 0);
            this.AddParamLabel.Name = "AddParamLabel";
            this.AddParamLabel.Size = new System.Drawing.Size(101, 20);
            this.AddParamLabel.TabIndex = 16;
            this.AddParamLabel.Text = "Parameters";
            this.AddParamLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ParamAddForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(784, 461);
            this.Controls.Add(this.Skip);
            this.Controls.Add(this.Save);
            this.Controls.Add(this.KeywordDocumentationLabel);
            this.Controls.Add(this.KeywordDocumentation);
            this.Controls.Add(this.KeywordNameLabel);
            this.Controls.Add(this.KeywordName);
            this.Controls.Add(this.AddParamLabel);
            this.Name = "ParamAddForm";
            this.Text = "ParamAddForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Skip;
        private System.Windows.Forms.Button Save;
        private System.Windows.Forms.Label KeywordDocumentationLabel;
        private System.Windows.Forms.TextBox KeywordDocumentation;
        private System.Windows.Forms.Label KeywordNameLabel;
        private System.Windows.Forms.TextBox KeywordName;
        private System.Windows.Forms.Label AddParamLabel;
    }
}