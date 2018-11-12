using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using RobotAutomationHelper.Scripts.Objects;
using RobotAutomationHelper.Scripts.Static;

namespace RobotAutomationHelper.Forms
{
    internal partial class LibrariesForm : Form
    {
        private readonly int _labelLength = 100;
        private readonly int _distanceBetweenLabelAndRadio = 10;
        private readonly int _fieldHeight = 25;
        private readonly int _initialY = 30;

        internal LibrariesForm()
        {
            InitializeComponent();
        }

        internal void ShowLibrariesContent()
        {
            var stdLibsCounter = 0;
            var extLibsCounter = 0;
            var listKeyword = new List<Keyword>();

            foreach (var temp in RobotAutomationHelper.SuiteSettingsList)
                if (temp != null)
                {
                    if (temp.TestSetup != null)
                        listKeyword.Add(temp.TestSetup);
                    if (temp.SuiteSetup != null)
                        listKeyword.Add(temp.SuiteSetup);
                    if (temp.TestTeardown != null)
                        listKeyword.Add(temp.TestTeardown);
                    if (temp.SuiteTeardown != null)
                        listKeyword.Add(temp.SuiteTeardown);
                }

            var lockedIncludes = SuggestionsClass.UpdateSuggestionsToIncludes(RobotAutomationHelper.TestCases, listKeyword);

            foreach (var lib in SuggestionsClass.Suggestions)
            {
                if (lib.KeyType.Equals(KeywordType.Standard))
                {
                    ShowLibField(STDLIBlabel.Location.X, _initialY + stdLibsCounter * _fieldHeight, lib.Name, lib.ToInclude);
                    if (lockedIncludes.Contains(lib.Name))
                        DisableLibField(lib.Name);
                    stdLibsCounter++;
                }
                else
                {
                    if (!lib.KeyType.Equals(KeywordType.ForLoopElements) && !lib.KeyType.Equals(KeywordType.ForLoopInRange) && !lib.KeyType.Equals(KeywordType.Custom))
                    {
                        ShowLibField(EXTLIBlabel.Location.X, _initialY + extLibsCounter * _fieldHeight, lib.Name, lib.ToInclude);
                        if (lockedIncludes.Contains(lib.Name))
                            DisableLibField(lib.Name);
                        extLibsCounter++;
                    }
                }
            }

            StartPosition = FormStartPosition.Manual;
            ShowDialog();
        }

        private void DisableLibField(string name)
        {
            Controls["checkbox" + name].Enabled = false;
            Controls[name].Enabled = false;
        }

        private void ShowLibField(int x, int y, string name, bool check)
        {
            var libName = new Label
            {
                Name = name,
                Text = name,
                Location = new Point(x, y)
            };
            var checkBox = new CheckBox
            {
                Name = "checkbox" + name,
                Checked = check,
                Location = new Point(x + _labelLength + _distanceBetweenLabelAndRadio, y - 5)
            };

            Controls.Add(libName);
            Controls.Add(checkBox);
        }

        private void Skip_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Save_Click(object sender, EventArgs e)
        {
            foreach (var lib in SuggestionsClass.Suggestions)
            {
                if (lib.KeyType.Equals(KeywordType.Standard))
                {
                    lib.ToInclude = SuggestionsClass.ContainsName(lib.Name, ((CheckBox) Controls["checkbox" + lib.Name]).Checked, false);
                }
                else
                {
                    if (!lib.KeyType.Equals(KeywordType.ForLoopElements) && !lib.KeyType.Equals(KeywordType.ForLoopInRange) && !lib.KeyType.Equals(KeywordType.Custom))
                    {
                        lib.ToInclude = SuggestionsClass.ContainsName(lib.Name, ((CheckBox) Controls["checkbox" + lib.Name]).Checked, false);
                    }
                }
            }
            Close();
        }
    }
}
