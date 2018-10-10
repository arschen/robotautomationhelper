using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace RobotAutomationHelper.Scripts
{
    internal static class BaseKeywordAddForm
    {

        // change the field when the keyword name is changed
        internal static void ChangeTheKeywordFieldAfterSelection(object sender, EventArgs e, Form form, bool isKeywordForm, List<Keyword> Keywords)
        {
            FormControls.keyEvent = Keys.None;
            if ((sender as ComboTheme).SelectedIndex != -1)
            {
                ComboTheme combo = sender as ComboTheme;
                int keywordIndex = int.Parse(combo.Name.Replace("Name", "").Replace("DynamicTestStep", ""));
                Keywords[keywordIndex - 1].SetKeywordName(combo.Text);
                FormControls.CheckKeywordTypeAndReturnKeyword(Keywords[keywordIndex - 1], combo.Text);
                combo.HideToolTip();
                if (isKeywordForm)
                    UpdateKeywordInThisKeyword(sender, e, form as KeywordAddForm);
            }
            else
                ChangeTheKeywordFieldAfterKeyPress(sender, e, form, isKeywordForm, Keywords, (sender as ComboTheme).Text);
        }

        // change the field when the keyword name is changed
        internal static void ChangeTheKeywordFieldAfterKeyPress(object sender, EventArgs e, Form form, bool isKeywordForm, List<Keyword> Keywords, string text)
        {
            ComboTheme combo = sender as ComboTheme;
            int keywordIndex = int.Parse(combo.Name.Replace("Name", "").Replace("DynamicTestStep", ""));
            Keywords[keywordIndex - 1].SetKeywordName(text);
            FormControls.CheckKeywordTypeAndReturnKeyword(Keywords[keywordIndex - 1], text);
            combo.HideToolTip();
            if (isKeywordForm)
                UpdateKeywordInThisKeyword(sender, e, form as KeywordAddForm);
        }

        // handles key press for keyword name input, the case when return/enter is hit
        internal static void AutoCompleteComboBoxKeyPress(object sender, KeyEventArgs e, Form form, bool isKeywordForm, List<Keyword> Keywords)
        {
            var comboTheme = sender as ComboTheme;
            if (FormControls.keyEvent != Keys.Return)
                FormControls.textBeforeDroppedDown = comboTheme.Text;
            FormControls.keyEvent = e.KeyCode;
            Console.WriteLine("key down " + e.KeyCode + "\\" + FormControls.textBeforeDroppedDown);
            FormControls.selectionPointer = comboTheme.SelectionStart;
            //Console.WriteLine(e.KeyCode);

            if (FormControls.keyEvent == Keys.Return && comboTheme.SelectedIndex == -1)
            {
                comboTheme.DroppedDown = false;
                comboTheme.Text = FormControls.textBeforeDroppedDown;
                comboTheme.SelectionStart = FormControls.selectionPointer;
                ChangeTheKeywordFieldAfterKeyPress(sender, e, form, isKeywordForm, Keywords, FormControls.textBeforeDroppedDown);
            }
        }

        internal static void UpdateKeywordInThisKeyword(object sender, EventArgs e, KeywordAddForm keywordForm)
        {
            int keywordIndex = int.Parse(((ComboTheme)sender).Name.Replace("DynamicTestStep", "").Replace("Name", ""));
            if (!keywordForm.ThisFormKeywords[keywordIndex - 1].Type.Equals(KeywordType.CUSTOM))
            {
                if (keywordForm.Controls.Find("DynamicTestStep" + keywordIndex + "AddImplementation", false).Length != 0)
                    keywordForm.Controls.RemoveByKey("DynamicTestStep" + keywordIndex + "AddImplementation");
            }
            else
            {
                string buttonImplementation = "Add Implementation";
                FormControls.AddControl("Button", "DynamicTestStep" + keywordIndex + "AddImplementation",
                new Point(320 - keywordForm.HorizontalScroll.Value, keywordForm.initialYValue + (keywordIndex - 1) * 30 - keywordForm.VerticalScroll.Value),
                new Size(120, 20),
                buttonImplementation,
                Color.Black,
                new EventHandler(keywordForm.InstantiateKeywordAddForm),
                keywordForm);
            }

            List<string> args = new List<string>();
            args = StringAndListOperations.ReturnListOfArgs(keywordForm.ThisFormKeywords[keywordIndex - 1].GetKeywordArguments());
            if (keywordForm.Controls.Find("DynamicTestStep" + keywordIndex + "Params", false).Length != 0
                && (args == null || args.Count == 0))
                    keywordForm.Controls.RemoveByKey("DynamicTestStep" + keywordIndex + "Params");
            else
                if (args != null && args.Count != 0)
                    FormControls.AddControl("Button", "DynamicTestStep" + keywordIndex + "Params",
                        new Point(500 - keywordForm.HorizontalScroll.Value, keywordForm.initialYValue + (keywordIndex - 1) * 30 - keywordForm.VerticalScroll.Value),
                        new Size(75, 20),
                        "Params",
                        Color.Black,
                        new EventHandler(keywordForm.InstantiateParamsAddForm),
                        keywordForm);

            keywordForm.Controls["DynamicTestStep" + keywordIndex + "Name"].Text = keywordForm.ThisFormKeywords[keywordIndex - 1].GetKeywordName().Trim();
        }

        internal static void UpdateKeywordInThisKeyword(object sender, EventArgs e, TestCaseAddForm testCaseAddForm)
        {
            int keywordIndex = int.Parse(((ComboTheme)sender).Name.Replace("DynamicTestStep", "").Replace("RemoveKeyword", ""));
            if (!TestCaseAddForm.Keywords[keywordIndex - 1].Type.Equals(KeywordType.CUSTOM))
            {
                if (testCaseAddForm.Controls.Find("DynamicTestStep" + keywordIndex + "AddImplementation", false).Length != 0)
                    testCaseAddForm.Controls.RemoveByKey("DynamicTestStep" + keywordIndex + "AddImplementation");
            }
            else
            {
                string buttonImplementation = "Add Implementation";
                FormControls.AddControl("Button", "DynamicTestStep" + keywordIndex + "AddImplementation",
                new Point(320 - testCaseAddForm.HorizontalScroll.Value, testCaseAddForm.initialYValue + (keywordIndex - 1) * 30 - testCaseAddForm.VerticalScroll.Value),
                new Size(120, 20),
                buttonImplementation,
                Color.Black,
                new EventHandler(testCaseAddForm.InstantiateKeywordAddForm),
                testCaseAddForm);
            }

            List<string> args = new List<string>();
            args = StringAndListOperations.ReturnListOfArgs(TestCaseAddForm.Keywords[keywordIndex - 1].GetKeywordArguments());
            if (testCaseAddForm.Controls.Find("DynamicTestStep" + keywordIndex + "Params", false).Length != 0
                && (args == null || args.Count == 0))
                testCaseAddForm.Controls.RemoveByKey("DynamicTestStep" + keywordIndex + "Params");
            else
                if (args != null && args.Count != 0)
                FormControls.AddControl("Button", "DynamicTestStep" + keywordIndex + "Params",
                    new Point(500 - testCaseAddForm.HorizontalScroll.Value, testCaseAddForm.initialYValue + (keywordIndex - 1) * 30 - testCaseAddForm.VerticalScroll.Value),
                    new Size(75, 20),
                    "Params",
                    Color.Black,
                    new EventHandler(testCaseAddForm.InstantiateParamsAddForm),
                    testCaseAddForm);

            testCaseAddForm.Controls["DynamicTestStep" + keywordIndex + "Name"].Text = TestCaseAddForm.Keywords[keywordIndex - 1].GetKeywordName().Trim();
        }
    }
}
