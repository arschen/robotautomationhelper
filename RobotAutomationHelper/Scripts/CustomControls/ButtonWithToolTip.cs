using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace RobotAutomationHelper.Scripts.CustomControls
{
    internal partial class ButtonWithToolTip : Button
    {
        private ToolTip toolTip = new ToolTip();
        private string toolTipText = "";
        private bool ContainsTwoEmptySpaces;
        private bool EmptyName;
        private bool OutpuFileEndsWithRobot;

        internal ButtonWithToolTip()
        {
            InitializeComponent();
        }

        internal void SetupToolTipText()
        {
            toolTipText = "";
            if (ContainsTwoEmptySpaces)
                toolTipText += "Cannot have names with 2 empty spaces.\n";
            if (EmptyName)
                toolTipText += "Cannot have empty names.\n";
            if (OutpuFileEndsWithRobot)
                toolTipText += "Output file name should end with '.robot'";
        }

        internal void UpdateState(List<string> Names, string OutputFile)
        {
            bool name = NameCheck(Names);
            bool file = OutputFileCheck(OutputFile);
            if (name && file)
            {
                Enabled = true;
                toolTip.Hide(this);
            }
            else
            {
                Enabled = false;
                SetupToolTipText();
                toolTip.Show(toolTipText, this, Width, 0);
            }
        }

        private bool OutputFileCheck(string OutputFileText)
        {
            if (!OutputFileText.ToLower().EndsWith(".robot"))
            {
                OutpuFileEndsWithRobot = true;
                return false;
            }
            else
            {
                OutpuFileEndsWithRobot = false;
                return true;
            }
        }

        private bool NameCheck(List<string> Names)
        {
            foreach (string Name in Names)
            {
                if (Name.Trim().Length == 0)
                {
                    EmptyName = true;
                    ContainsTwoEmptySpaces = false;
                    return false;
                }
                else
                {
                    string[] checkForVariables = Name.Trim().Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    if (checkForVariables != null && checkForVariables.Length != 0)
                        if (StringAndListOperations.StartsWithVariable(checkForVariables[0]))
                                return true;

                    for (int i = 0; i < Name.Trim().Length - 1; i++)
                        if (Name.Trim()[i].Equals(' '))
                            if (Name.Trim()[i + 1].Equals(' '))
                            {
                                EmptyName = false;
                                ContainsTwoEmptySpaces = true;
                                return false;
                            }
                }
            }
            EmptyName = false;
            ContainsTwoEmptySpaces = false;
            return true;
        }
    }
}
