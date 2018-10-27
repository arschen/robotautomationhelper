using System.Collections.Generic;
using System.Drawing;
using RobotAutomationHelper.Scripts;

namespace RobotAutomationHelper.Forms
{
    internal partial class NameAndOutputForm : BaseKeywordAddForm
    {
        internal NameAndOutputForm(FormType type)
        {
            InitializeComponent();
            FormControls.UpdateOutputFileSuggestions(OutputFile, type);
            FormType = FormType.NameAndOutput;
            ThisFormKeywords = new List<Keyword>
                {
                    new Keyword("", "")
                };
        }

        private void Save_Click(object sender, System.EventArgs e)
        {
            Close();
        }

        private void Cancel_Click(object sender, System.EventArgs e)
        {
            Close();
        }

        internal void ShowKeywordContent()
        {
            FormControls.RemoveControlByKey(ContentName.Name, Controls);
            FormControls.AddControl("TextWithList", "DynamicStep" + 1 + "Name",
                1,
                new Point(32 - HorizontalScroll.Value, 24 - VerticalScroll.Value),
                new Size(280, 20),
                "",
                Color.Black,
                null,
                this);
            var dialogResult = ShowDialog();
        }

        internal void ShowTestCaseContent()
        {
            OutputLabel.Text = OutputLabel.Text.Replace("Keyword", "Test Case");
            NameLabel.Text = NameLabel.Text.Replace("Keyword", "Test Case");
            var dialogResult = ShowDialog();
        }
    }
}
