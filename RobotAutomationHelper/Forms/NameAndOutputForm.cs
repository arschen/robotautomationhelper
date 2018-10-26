using RobotAutomationHelper.Scripts;

namespace RobotAutomationHelper.Forms
{
    internal partial class NameAndOutputForm : BaseKeywordAddForm
    {
        internal NameAndOutputForm(FormType type)
        {
            InitializeComponent();
            FormControls.UpdateOutputFileSuggestions(OutputFile, type);
        }

        private void Save_Click(object sender, System.EventArgs e)
        {
            Close();
        }

        private void Cancel_Click(object sender, System.EventArgs e)
        {
            Close();
        }

        internal void ShowContent()
        {
            var dialogResult = ShowDialog();
        }
    }
}
