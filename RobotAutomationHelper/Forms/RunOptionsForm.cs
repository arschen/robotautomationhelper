using System;
using System.Windows.Forms;

namespace RobotAutomationHelper.Forms
{
    internal partial class RunOptionsForm : Form
    {
        internal static string RunOptionsString = "";

        internal RunOptionsForm()
        {
            InitializeComponent();
            RunOptionsText.Text = RunOptionsString;
        }

        internal void ShowRunOptionsContent()
        {
            StartPosition = FormStartPosition.Manual;
            ShowDialog();
        }

        private void Close_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Save_Click(object sender, EventArgs e)
        {
            RunOptionsString = RunOptionsText.Text;
            Close();
        }
    }
}
