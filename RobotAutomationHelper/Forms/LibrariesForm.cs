using System;
using System.Drawing;
using System.Windows.Forms;
using RobotAutomationHelper.Scripts.Objects;
using RobotAutomationHelper.Scripts.Static;

namespace RobotAutomationHelper.Forms
{
    internal partial class LibrariesForm : Form
    {
        private const int LabelLength = 100;
        private const int DistanceBetweenLabelAndRadio = 10;
        private const int FieldHeight = 25;
        private const int InitialY = 30;

        internal LibrariesForm()
        {
            InitializeComponent();
        }

        internal void ShowLibrariesContent()
        {
            var stdLibsCounter = 0;
            var extLibsCounter = 0;

            var lockedIncludes = SuggestionsClass.UpdateSuggestionsToIncludes(RobotAutomationHelper.TestCases, RobotAutomationHelper.SuiteSettingsList);

            foreach (var lib in SuggestionsClass.Suggestions)
            {
                if (lib.KeyType.Equals(KeywordType.Standard))
                {
                    ShowLibField(STDLIBlabel.Location.X, InitialY + stdLibsCounter * FieldHeight, lib.Name, lib.ToInclude);
                    if (lockedIncludes.Contains(lib.Name))
                        DisableLibField(lib.Name);
                    stdLibsCounter++;
                }
                else
                {
                    if (lib.KeyType.Equals(KeywordType.ForLoopElements) ||
                        lib.KeyType.Equals(KeywordType.ForLoopInRange) ||
                        lib.KeyType.Equals(KeywordType.Custom)) continue;
                    ShowLibField(EXTLIBlabel.Location.X, InitialY + extLibsCounter * FieldHeight, lib.Name, lib.ToInclude);
                    if (lockedIncludes.Contains(lib.Name))
                        DisableLibField(lib.Name);
                    extLibsCounter++;
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
                Location = new Point(x + LabelLength + DistanceBetweenLabelAndRadio, y - 5)
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
