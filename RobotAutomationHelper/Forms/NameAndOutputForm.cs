using RobotAutomationHelper.Scripts;
using System.Windows.Forms;

namespace RobotAutomationHelper.Forms
{
    internal partial class NameAndOutputForm : BaseKeywordAddForm
    {
        internal NameAndOutputForm(BaseKeywordAddForm Parent)
        {
            InitializeComponent();
            FormControls.UpdateOutputFileSuggestions(OutputFile, Parent.FormType);
        }

        private void Save_Click(object sender, System.EventArgs e)
        {

        }

        private void Cancel_Click(object sender, System.EventArgs e)
        {

        }
    }
}
