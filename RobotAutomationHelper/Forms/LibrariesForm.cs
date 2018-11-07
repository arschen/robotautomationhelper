using RobotAutomationHelper.Scripts;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace RobotAutomationHelper.Forms
{
    internal partial class LibrariesForm : Form
    {
        private int LabelLength = 100;
        private int distanceBetweenLabelAndRadio = 10;
        private int FieldHeight = 25;
        private int initialY = 30;

        internal LibrariesForm()
        {
            InitializeComponent();
        }

        internal void ShowLibrariesContent()
        {
            int stdLibsCounter = 0;
            int extLibsCounter = 0;
            List<Keyword> listKeyword = new List<Keyword>();

            foreach (SuiteSettings temp in RobotAutomationHelper.SuiteSettingsList)
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

            List<string> LockedIncludes = SuggestionsClass.UpdateSuggestionsToIncludes(RobotAutomationHelper.TestCases, listKeyword);

            foreach (Lib lib in SuggestionsClass.Suggestions)
            {
                if (lib.keyType.Equals(KeywordType.STANDARD))
                {
                    ShowLibField(STDLIBlabel.Location.X, initialY + stdLibsCounter * FieldHeight, lib.Name, lib.ToInclude);
                    if (LockedIncludes.Contains(lib.Name))
                        DisableLibField(lib.Name);
                    stdLibsCounter++;
                }
                else
                {
                    if (!lib.keyType.Equals(KeywordType.FOR_LOOP_ELEMENTS) && !lib.keyType.Equals(KeywordType.FOR_LOOP_IN_RANGE) && !lib.keyType.Equals(KeywordType.CUSTOM))
                    {
                        ShowLibField(EXTLIBlabel.Location.X, initialY + extLibsCounter * FieldHeight, lib.Name, lib.ToInclude);
                        if (LockedIncludes.Contains(lib.Name))
                            DisableLibField(lib.Name);
                        extLibsCounter++;
                    }
                }
            }

            StartPosition = FormStartPosition.Manual;
            var dialogResult = ShowDialog();
        }

        private void DisableLibField(string Name)
        {
            Controls["checkbox" + Name].Enabled = false;
            Controls[Name].Enabled = false;
        }

        private void ShowLibField(int x, int y, string Name, bool check)
        {
            Label libName = new Label
            {
                Name = Name,
                Text = Name,
                Location = new Point(x, y)
            };
            CheckBox checkBox = new CheckBox
            {
                Name = "checkbox" + Name,
                Checked = check
            };
            checkBox.Location = new Point(x + LabelLength + distanceBetweenLabelAndRadio, y - 5);

            Controls.Add(libName);
            Controls.Add(checkBox);
        }

        private void Skip_Click(object sender, System.EventArgs e)
        {
            Close();
        }

        private void Save_Click(object sender, System.EventArgs e)
        {
            foreach (Lib lib in SuggestionsClass.Suggestions)
            {
                if (lib.keyType.Equals(KeywordType.STANDARD))
                {
                    lib.ToInclude = SuggestionsClass.ContainsName(lib.Name, (Controls["checkbox" + lib.Name] as CheckBox).Checked, false);
                }
                else
                {
                    if (!lib.keyType.Equals(KeywordType.FOR_LOOP_ELEMENTS) && !lib.keyType.Equals(KeywordType.FOR_LOOP_IN_RANGE) && !lib.keyType.Equals(KeywordType.CUSTOM))
                    {
                        lib.ToInclude = SuggestionsClass.ContainsName(lib.Name, (Controls["checkbox" + lib.Name] as CheckBox).Checked, false);
                    }
                }
            }
            Close();
        }
    }
}
