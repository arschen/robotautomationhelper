using System;
using System.Collections.Generic;
using System.Windows.Forms;
using RobotAutomationHelper.Scripts.Static;

namespace RobotAutomationHelper.Scripts.CustomControls
{
    internal partial class ButtonWithToolTip : Button
    {
        private readonly ToolTip _toolTip = new ToolTip();
        private string _toolTipText = "";
        private bool _containsTwoEmptySpaces;
        private bool _emptyName;
        private bool _outputFileEndsWithRobot;

        internal ButtonWithToolTip()
        {
            InitializeComponent();
        }

        internal void SetupToolTipText()
        {
            _toolTipText = "";
            if (_containsTwoEmptySpaces)
                _toolTipText += "Cannot have names with 2 empty spaces.\n";
            if (_emptyName)
                _toolTipText += "Cannot have empty names.\n";
            if (_outputFileEndsWithRobot)
                _toolTipText += "Output file name should end with '.robot'";
        }

        internal void UpdateState(List<string> names, string outputFile)
        {
            var name = NameCheck(names);
            var file = OutputFileCheck(outputFile);
            if (name && file)
            {
                Enabled = true;
                _toolTip.Hide(this);
            }
            else
            {
                Enabled = false;
                SetupToolTipText();
                _toolTip.Show(_toolTipText, this, Width, 0);
            }
        }

        private bool OutputFileCheck(string outputFileText)
        {
            if (!outputFileText.ToLower().EndsWith(".robot"))
            {
                _outputFileEndsWithRobot = true;
                return false;
            }

            _outputFileEndsWithRobot = false;
            return true;
        }

        private bool NameCheck(List<string> names)
        {
            foreach (var name in names)
            {
                if (name.Trim().Length == 0)
                {
                    _emptyName = true;
                    _containsTwoEmptySpaces = false;
                    return false;
                }

                var checkForVariables = name.Trim().Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                if (checkForVariables.Length != 0)
                    if (StringAndListOperations.StartsWithVariable(checkForVariables[0]))
                        return true;

                for (var i = 0; i < name.Trim().Length - 1; i++)
                    if (name.Trim()[i].Equals(' '))
                        if (name.Trim()[i + 1].Equals(' '))
                        {
                            _emptyName = false;
                            _containsTwoEmptySpaces = true;
                            return false;
                        }
            }
            _emptyName = false;
            _containsTwoEmptySpaces = false;
            return true;
        }
    }
}
