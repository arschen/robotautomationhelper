namespace RobotAutomationHelper
{
    partial class KeywordAddForm
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
            this.KeywordDocumentationLabel = new System.Windows.Forms.Label();
            this.KeywordDocumentation = new System.Windows.Forms.TextBox();
            this.KeywordNameLabel = new System.Windows.Forms.Label();
            this.KeywordName = new System.Windows.Forms.TextBox();
            this.AddKeywordLabel = new System.Windows.Forms.Label();
            this.Skip = new System.Windows.Forms.Button();
            this.Save = new System.Windows.Forms.Button();
            this.ArgumentType = new System.Windows.Forms.ComboBox();
            this.KeywordOutputFile = new System.Windows.Forms.ComboBox();
            this.KeywordOutputLabel = new System.Windows.Forms.Label();
            this.KeywordArgumentsLabel = new System.Windows.Forms.Label();
            this.KeywordArguments = new System.Windows.Forms.TextBox();
            this.AddArgument = new System.Windows.Forms.Button();
            this.ArgumentName = new System.Windows.Forms.TextBox();
            this.ArgumentTypeLabel = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.RemoveArgument = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // KeywordDocumentationLabel
            // 
            this.KeywordDocumentationLabel.AutoSize = true;
            this.KeywordDocumentationLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.KeywordDocumentationLabel.Location = new System.Drawing.Point(320, 45);
            this.KeywordDocumentationLabel.Name = "KeywordDocumentationLabel";
            this.KeywordDocumentationLabel.Size = new System.Drawing.Size(126, 13);
            this.KeywordDocumentationLabel.TabIndex = 13;
            this.KeywordDocumentationLabel.Text = "Keyword Documentation:";
            // 
            // KeywordDocumentation
            // 
            this.KeywordDocumentation.Location = new System.Drawing.Point(340, 60);
            this.KeywordDocumentation.Name = "KeywordDocumentation";
            this.KeywordDocumentation.Size = new System.Drawing.Size(280, 20);
            this.KeywordDocumentation.TabIndex = 12;
            // 
            // KeywordNameLabel
            // 
            this.KeywordNameLabel.AutoSize = true;
            this.KeywordNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.KeywordNameLabel.Location = new System.Drawing.Point(10, 45);
            this.KeywordNameLabel.Name = "KeywordNameLabel";
            this.KeywordNameLabel.Size = new System.Drawing.Size(82, 13);
            this.KeywordNameLabel.TabIndex = 11;
            this.KeywordNameLabel.Text = "Keyword Name:";
            // 
            // KeywordName
            // 
            this.KeywordName.Location = new System.Drawing.Point(30, 60);
            this.KeywordName.Name = "KeywordName";
            this.KeywordName.Size = new System.Drawing.Size(280, 20);
            this.KeywordName.TabIndex = 10;
            // 
            // AddKeywordLabel
            // 
            this.AddKeywordLabel.AutoSize = true;
            this.AddKeywordLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AddKeywordLabel.Location = new System.Drawing.Point(10, 9);
            this.AddKeywordLabel.Margin = new System.Windows.Forms.Padding(1, 0, 3, 0);
            this.AddKeywordLabel.Name = "AddKeywordLabel";
            this.AddKeywordLabel.Size = new System.Drawing.Size(149, 20);
            this.AddKeywordLabel.TabIndex = 9;
            this.AddKeywordLabel.Text = "Keyword Creation";
            this.AddKeywordLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Skip
            // 
            this.Skip.Location = new System.Drawing.Point(626, 92);
            this.Skip.Name = "Skip";
            this.Skip.Size = new System.Drawing.Size(100, 30);
            this.Skip.TabIndex = 15;
            this.Skip.Text = "Skip Keyword";
            this.Skip.UseVisualStyleBackColor = true;
            this.Skip.Click += new System.EventHandler(this.Skip_Click);
            // 
            // Save
            // 
            this.Save.Location = new System.Drawing.Point(626, 54);
            this.Save.Name = "Save";
            this.Save.Size = new System.Drawing.Size(100, 30);
            this.Save.TabIndex = 14;
            this.Save.Text = "Save Keyword";
            this.Save.UseVisualStyleBackColor = true;
            this.Save.Click += new System.EventHandler(this.Save_Click);
            // 
            // ArgumentType
            // 
            this.ArgumentType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ArgumentType.FormattingEnabled = true;
            this.ArgumentType.Items.AddRange(new object[] {
            "Scalar",
            "Dictionary",
            "List"});
            this.ArgumentType.Location = new System.Drawing.Point(30, 138);
            this.ArgumentType.Name = "ArgumentType";
            this.ArgumentType.Size = new System.Drawing.Size(121, 21);
            this.ArgumentType.TabIndex = 16;
            // 
            // KeywordOutputFile
            // 
            this.KeywordOutputFile.FormattingEnabled = true;
            this.KeywordOutputFile.Location = new System.Drawing.Point(30, 98);
            this.KeywordOutputFile.Name = "KeywordOutputFile";
            this.KeywordOutputFile.Size = new System.Drawing.Size(280, 21);
            this.KeywordOutputFile.TabIndex = 18;
            // 
            // KeywordOutputLabel
            // 
            this.KeywordOutputLabel.AutoSize = true;
            this.KeywordOutputLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.KeywordOutputLabel.Location = new System.Drawing.Point(10, 83);
            this.KeywordOutputLabel.Name = "KeywordOutputLabel";
            this.KeywordOutputLabel.Size = new System.Drawing.Size(105, 13);
            this.KeywordOutputLabel.TabIndex = 17;
            this.KeywordOutputLabel.Text = "Keyword Output File:";
            // 
            // KeywordArgumentsLabel
            // 
            this.KeywordArgumentsLabel.AutoSize = true;
            this.KeywordArgumentsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.KeywordArgumentsLabel.Location = new System.Drawing.Point(320, 83);
            this.KeywordArgumentsLabel.Name = "KeywordArgumentsLabel";
            this.KeywordArgumentsLabel.Size = new System.Drawing.Size(104, 13);
            this.KeywordArgumentsLabel.TabIndex = 20;
            this.KeywordArgumentsLabel.Text = "Keyword Arguments:";
            // 
            // KeywordArguments
            // 
            this.KeywordArguments.Location = new System.Drawing.Point(340, 98);
            this.KeywordArguments.Name = "KeywordArguments";
            this.KeywordArguments.ReadOnly = true;
            this.KeywordArguments.Size = new System.Drawing.Size(280, 20);
            this.KeywordArguments.TabIndex = 19;
            // 
            // AddArgument
            // 
            this.AddArgument.AutoSize = true;
            this.AddArgument.Location = new System.Drawing.Point(263, 137);
            this.AddArgument.Name = "AddArgument";
            this.AddArgument.Size = new System.Drawing.Size(40, 23);
            this.AddArgument.TabIndex = 21;
            this.AddArgument.Text = "Add";
            this.AddArgument.UseVisualStyleBackColor = true;
            this.AddArgument.Click += new System.EventHandler(this.AddArgument_Click);
            // 
            // ArgumentName
            // 
            this.ArgumentName.Location = new System.Drawing.Point(157, 139);
            this.ArgumentName.Name = "ArgumentName";
            this.ArgumentName.Size = new System.Drawing.Size(100, 20);
            this.ArgumentName.TabIndex = 22;
            // 
            // ArgumentTypeLabel
            // 
            this.ArgumentTypeLabel.AutoSize = true;
            this.ArgumentTypeLabel.Location = new System.Drawing.Point(10, 122);
            this.ArgumentTypeLabel.Name = "ArgumentTypeLabel";
            this.ArgumentTypeLabel.Size = new System.Drawing.Size(82, 13);
            this.ArgumentTypeLabel.TabIndex = 23;
            this.ArgumentTypeLabel.Text = "Argument Type:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(154, 122);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(86, 13);
            this.label2.TabIndex = 24;
            this.label2.Text = "Argument Name:";
            // 
            // RemoveArgument
            // 
            this.RemoveArgument.AutoSize = true;
            this.RemoveArgument.Location = new System.Drawing.Point(309, 137);
            this.RemoveArgument.Name = "RemoveArgument";
            this.RemoveArgument.Size = new System.Drawing.Size(57, 23);
            this.RemoveArgument.TabIndex = 25;
            this.RemoveArgument.Text = "Remove";
            this.RemoveArgument.UseVisualStyleBackColor = true;
            this.RemoveArgument.Click += new System.EventHandler(this.RemoveArgument_Click);
            // 
            // KeywordAddForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(784, 461);
            this.Controls.Add(this.RemoveArgument);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.ArgumentTypeLabel);
            this.Controls.Add(this.ArgumentName);
            this.Controls.Add(this.AddArgument);
            this.Controls.Add(this.KeywordArgumentsLabel);
            this.Controls.Add(this.KeywordArguments);
            this.Controls.Add(this.KeywordOutputFile);
            this.Controls.Add(this.KeywordOutputLabel);
            this.Controls.Add(this.ArgumentType);
            this.Controls.Add(this.Skip);
            this.Controls.Add(this.Save);
            this.Controls.Add(this.KeywordDocumentationLabel);
            this.Controls.Add(this.KeywordDocumentation);
            this.Controls.Add(this.KeywordNameLabel);
            this.Controls.Add(this.KeywordName);
            this.Controls.Add(this.AddKeywordLabel);
            this.Name = "KeywordAddForm";
            this.Text = "AddKeywordForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label KeywordDocumentationLabel;
        private System.Windows.Forms.TextBox KeywordDocumentation;
        private System.Windows.Forms.Label KeywordNameLabel;
        private System.Windows.Forms.TextBox KeywordName;
        private System.Windows.Forms.Label AddKeywordLabel;
        private System.Windows.Forms.Button Skip;
        private System.Windows.Forms.Button Save;
        private System.Windows.Forms.ComboBox ArgumentType;
        private System.Windows.Forms.ComboBox KeywordOutputFile;
        private System.Windows.Forms.Label KeywordOutputLabel;
        private System.Windows.Forms.Label KeywordArgumentsLabel;
        private System.Windows.Forms.TextBox KeywordArguments;
        private System.Windows.Forms.Button AddArgument;
        private System.Windows.Forms.TextBox ArgumentName;
        private System.Windows.Forms.Label ArgumentTypeLabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button RemoveArgument;
    }
}