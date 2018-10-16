using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace RobotAutomationHelper.Scripts
{
    internal static class BaseKeywordAddForm
    {

        internal static Keys keyEvent;
        internal static bool prevEnterKey = false;

        // change the field when the keyword name is changed
        internal static void ChangeTheKeywordFieldAfterSelection(object sender, EventArgs e, Form form, bool isKeywordForm, List<Keyword> Keywords)
        {
            if (RobotAutomationHelper.Log) Console.WriteLine("ChangeTheKeywordFieldAfterSelection " + (sender as ComboTheme).Name + " " + form.Name);
            keyEvent = Keys.None;
            if ((sender as ComboTheme).SelectedIndex != -1)
            {
                ComboTheme combo = sender as ComboTheme;
                int keywordIndex = int.Parse(combo.Name.Replace("Name", "").Replace("DynamicTestStep", ""));

                // fix bug where if you edit library keyword name to CUSTOM one and then lose focus on textbox, the combo.Text stores the library keyword name
                if ((sender as ComboBox).Focused)
                {
                    FormControls.CheckKeywordTypeAndReturnKeyword(Keywords[keywordIndex - 1], combo.Text);
                    Keywords[keywordIndex - 1].SetKeywordName(combo.Text);
                }
                else
                    FormControls.CheckKeywordTypeAndReturnKeyword(Keywords[keywordIndex - 1], Keywords[keywordIndex - 1].GetKeywordName());

                combo.HideToolTip();
                if (isKeywordForm)
                    UpdateKeywordInThisKeyword(sender, e, form as KeywordAddForm);
                else
                    UpdateKeywordInThisTestCase(sender, e, form as TestCaseAddForm);
            }
            else
                ChangeTheKeywordFieldAfterKeyPress(sender, e, form, isKeywordForm, Keywords, (sender as ComboTheme).Text);
        }

        // change the field when the keyword name is changed
        internal static void ChangeTheKeywordFieldAfterKeyPress(object sender, EventArgs e, Form form, bool isKeywordForm, List<Keyword> Keywords, string text)
        {
            if (RobotAutomationHelper.Log) Console.WriteLine("ChangeTheKeywordFieldAfterKeyPress " + form.Name + " " + text);
            ComboTheme combo = sender as ComboTheme;
            int keywordIndex = int.Parse(combo.Name.Replace("Name", "").Replace("DynamicTestStep", ""));
            FormControls.CheckKeywordTypeAndReturnKeyword(Keywords[keywordIndex - 1], text);
            Keywords[keywordIndex - 1].SetKeywordName(text);
            combo.HideToolTip();
            if (isKeywordForm)
                UpdateKeywordInThisKeyword(sender, e, form as KeywordAddForm);
            else
                UpdateKeywordInThisTestCase(sender, e, form as TestCaseAddForm);
        }

        // handles key press for keyword name input, the case when return/enter is hit
        internal static void ComboBoxKeyPress(object sender, KeyEventArgs e, Form form, bool isKeywordForm, List<Keyword> Keywords)
        {
            var comboTheme = sender as ComboTheme;
            if (RobotAutomationHelper.Log) Console.WriteLine("ComboBoxKeyPress " + comboTheme.Name);
            if (keyEvent != Keys.Return)
            {
                comboTheme.textBeforeDroppedDown = comboTheme.Text;
                prevEnterKey = false;
            }
            else
                if (e.KeyCode != Keys.Return)
                prevEnterKey = true;
            keyEvent = e.KeyCode;
            if (RobotAutomationHelper.Log) Console.WriteLine("key down " + e.KeyCode + "\\" + comboTheme.textBeforeDroppedDown);
            comboTheme.selectionPointer = comboTheme.SelectionStart;
            if (RobotAutomationHelper.Log) Console.WriteLine(e.KeyCode);

            if (keyEvent == Keys.Return && comboTheme.SelectedIndex == -1)
            {
                comboTheme.DroppedDown = false;
                comboTheme.Text = comboTheme.textBeforeDroppedDown;
                comboTheme.SelectionStart = comboTheme.selectionPointer;
                ChangeTheKeywordFieldAfterKeyPress(sender, e, form, isKeywordForm, Keywords, comboTheme.textBeforeDroppedDown);
            }
        }

        internal static void UpdateKeywordInThisKeyword(object sender, EventArgs e, KeywordAddForm keywordForm)
        {
            if (RobotAutomationHelper.Log) Console.WriteLine("UpdateKeywordInThisKeyword " + ((ComboTheme)sender).Name + " " + keywordForm.Name);
            int keywordIndex = int.Parse(((ComboTheme)sender).Name.Replace("DynamicTestStep", "").Replace("Name", "")); 

            if (keywordForm.ThisFormKeywords[keywordIndex - 1].Type.Equals(KeywordType.CUSTOM))
            {
                string buttonImplementation = "Add Implementation";
                if (keywordForm.ThisFormKeywords[keywordIndex - 1].Implemented)
                    buttonImplementation = "Edit Implementation";

                if (RobotAutomationHelper.Log) Console.WriteLine("length: " + keywordForm.Controls.Find("DynamicTestStep" + keywordIndex + "AddImplementation", false).Length);
                if (keywordForm.Controls.Find("DynamicTestStep" + keywordIndex + "AddImplementation", false).Length == 0)
                    FormControls.AddControl("Button", "DynamicTestStep" + keywordIndex + "AddImplementation",
                    new Point(320 - keywordForm.HorizontalScroll.Value, keywordForm.initialYValue + (keywordIndex - 1) * 30 - keywordForm.VerticalScroll.Value),
                    new Size(120, 20),
                    buttonImplementation,
                    Color.Black,
                    new EventHandler(keywordForm.InstantiateKeywordAddForm),
                    keywordForm);
                else
                    (keywordForm.Controls.Find("DynamicTestStep" + keywordIndex + "AddImplementation", false)[0] as Button).Text = buttonImplementation;
            }
            else
                FormControls.RemoveControlByKey("DynamicTestStep" + keywordIndex + "AddImplementation", keywordForm.Controls);

            if (keywordForm.ThisFormKeywords[keywordIndex - 1].Type.Equals(KeywordType.CUSTOM))
            {
                List<string> args = new List<string>();
                args = StringAndListOperations.ReturnListOfArgs(keywordForm.ThisFormKeywords[keywordIndex - 1].GetKeywordArguments());

                if (args != null && args.Count != 0)
                {
                    if (keywordForm.Controls.Find("DynamicTestStep" + keywordIndex + "Params", false).Length == 0)
                        FormControls.AddControl("Button", "DynamicTestStep" + keywordIndex + "Params",
                        new Point(500 - keywordForm.HorizontalScroll.Value, keywordForm.initialYValue + (keywordIndex - 1) * 30 - keywordForm.VerticalScroll.Value),
                        new Size(75, 20),
                        "Params",
                        Color.Black,
                        new EventHandler(keywordForm.InstantiateParamsAddForm),
                        keywordForm);
                }else
                    FormControls.RemoveControlByKey("DynamicTestStep" + keywordIndex + "Params", keywordForm.Controls);
            }
            else
            {
                if (!(keywordForm.ThisFormKeywords[keywordIndex - 1].GetKeywordParams() == null)
                    && !(keywordForm.ThisFormKeywords[keywordIndex - 1].GetKeywordParams().Count == 0))
                {
                    if (keywordForm.Controls.Find("DynamicTestStep" + keywordIndex + "Params", false).Length == 0)
                        FormControls.AddControl("Button", "DynamicTestStep" + keywordIndex + "Params",
                            new Point(500 - keywordForm.HorizontalScroll.Value, keywordForm.initialYValue + (keywordIndex - 1) * 30 - keywordForm.VerticalScroll.Value),
                            new Size(75, 20),
                            "Params",
                            Color.Black,
                            new EventHandler(keywordForm.InstantiateParamsAddForm),
                            keywordForm);
                }else
                    FormControls.RemoveControlByKey("DynamicTestStep" + keywordIndex + "Params", keywordForm.Controls);
            }


            keywordForm.Controls["DynamicTestStep" + keywordIndex + "Name"].Text = keywordForm.ThisFormKeywords[keywordIndex - 1].GetKeywordName().Trim();
        }

        internal static void UpdateKeywordInThisTestCase(object sender, EventArgs e, TestCaseAddForm testCaseAddForm)
        {
            if (RobotAutomationHelper.Log) Console.WriteLine("UpdateKeywordInThisTestCase " + ((ComboTheme)sender).Name + " " + testCaseAddForm.Name);
            int keywordIndex = int.Parse(((ComboTheme)sender).Name.Replace("DynamicTestStep", "").Replace("Name", ""));

            if (TestCaseAddForm.Keywords[keywordIndex - 1].Type.Equals(KeywordType.CUSTOM))
            {
                string buttonImplementation = "Add Implementation";
                if (TestCaseAddForm.Keywords[keywordIndex - 1].Implemented)
                    buttonImplementation = "Edit Implementation";

                if (testCaseAddForm.Controls.Find("DynamicTestStep" + keywordIndex + "AddImplementation", false).Length == 0)
                    FormControls.AddControl("Button", "DynamicTestStep" + keywordIndex + "AddImplementation",
                    new Point(320 - testCaseAddForm.HorizontalScroll.Value, testCaseAddForm.initialYValue + (keywordIndex - 1) * 30 - testCaseAddForm.VerticalScroll.Value),
                    new Size(120, 20),
                    buttonImplementation,
                    Color.Black,
                    new EventHandler(testCaseAddForm.InstantiateKeywordAddForm),
                    testCaseAddForm);
                else
                    (testCaseAddForm.Controls.Find("DynamicTestStep" + keywordIndex + "AddImplementation", false)[0] as Button).Text = buttonImplementation;
            }
            else
                FormControls.RemoveControlByKey("DynamicTestStep" + keywordIndex + "AddImplementation", testCaseAddForm.Controls);

            
            if (TestCaseAddForm.Keywords[keywordIndex - 1].Type.Equals(KeywordType.CUSTOM))
            {
                List<string> args = new List<string>();
                args = StringAndListOperations.ReturnListOfArgs(TestCaseAddForm.Keywords[keywordIndex - 1].GetKeywordArguments());

                if (args != null && args.Count != 0)
                {
                    if (testCaseAddForm.Controls.Find("DynamicTestStep" + keywordIndex + "Params", false).Length == 0)
                        FormControls.AddControl("Button", "DynamicTestStep" + keywordIndex + "Params",
                            new Point(450 - testCaseAddForm.HorizontalScroll.Value, testCaseAddForm.initialYValue + (keywordIndex - 1) * 30 - testCaseAddForm.VerticalScroll.Value),
                            new Size(75, 20),
                            "Params",
                            Color.Black,
                            new EventHandler(testCaseAddForm.InstantiateParamsAddForm),
                            testCaseAddForm);
                }else
                    FormControls.RemoveControlByKey("DynamicTestStep" + keywordIndex + "Params", testCaseAddForm.Controls);
            }
            else
            {
                if (!(TestCaseAddForm.Keywords[keywordIndex - 1].GetKeywordParams() == null)
                    && !(TestCaseAddForm.Keywords[keywordIndex - 1].GetKeywordParams().Count == 0))
                {
                    if (testCaseAddForm.Controls.Find("DynamicTestStep" + keywordIndex + "Params", false).Length == 0)
                        FormControls.AddControl("Button", "DynamicTestStep" + keywordIndex + "Params",
                            new Point(450 - testCaseAddForm.HorizontalScroll.Value, testCaseAddForm.initialYValue + (keywordIndex - 1) * 30 - testCaseAddForm.VerticalScroll.Value),
                            new Size(75, 20),
                            "Params",
                            Color.Black,
                            new EventHandler(testCaseAddForm.InstantiateParamsAddForm),
                            testCaseAddForm);
                }
                else
                    FormControls.RemoveControlByKey("DynamicTestStep" + keywordIndex + "Params", testCaseAddForm.Controls);
            }

            testCaseAddForm.Controls["DynamicTestStep" + keywordIndex + "Name"].Text = TestCaseAddForm.Keywords[keywordIndex - 1].GetKeywordName().Trim();
        }
    }
}
