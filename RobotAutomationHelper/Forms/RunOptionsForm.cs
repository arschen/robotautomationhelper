using System;
using System.Windows.Forms;

namespace RobotAutomationHelper.Forms
{
    public partial class RunOptionsForm : Form
    {
        public static string RunOptionsString = "";

        public RunOptionsForm()
        {
            InitializeComponent();
            RunOptionsText.Text = RunOptionsString;
        }

        public void ShowRunOptionsContent()
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
