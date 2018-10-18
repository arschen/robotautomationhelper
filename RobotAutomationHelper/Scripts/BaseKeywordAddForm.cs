using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace RobotAutomationHelper.Scripts
{
    internal static class BaseKeywordAddForm
    {
        // change the field when the keyword name is changed
        internal static void ChangeTheKeywordFieldOnUpdate(object sender, Form form, bool isKeywordForm, List<Keyword> Keywords, string textChangePassed)
        {
            if (RobotAutomationHelper.Log) Console.WriteLine("ChangeTheKeywordFieldAfterSelection " + (sender as TextWithList).Name + " " + form.Name);

            TextWithList textWithList = sender as TextWithList;
            int keywordIndex = int.Parse(textWithList.Name.Replace("Name", "").Replace("DynamicTestStep", ""));

            if (textChangePassed.Equals(""))
                CheckKeywordTypeAndReturnKeyword(Keywords[keywordIndex - 1], textWithList.Text);
            else
                CheckKeywordTypeAndReturnKeyword(Keywords[keywordIndex - 1], textChangePassed);

            if (isKeywordForm)
                UpdateKeywordInThisKeyword(sender, form as KeywordAddForm);
            else
                UpdateKeywordInThisTestCase(sender, form as TestCaseAddForm);
        }

        internal static void UpdateKeywordInThisKeyword(object sender, KeywordAddForm keywordForm)
        {
            if (RobotAutomationHelper.Log) Console.WriteLine("UpdateKeywordInThisKeyword " + ((TextWithList)sender).Name + " " + keywordForm.Name);
            int keywordIndex = int.Parse(((TextWithList)sender).Name.Replace("DynamicTestStep", "").Replace("Name", "")); 

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
                args = StringAndListOperations.ReturnListOfArgs(keywordForm.ThisFormKeywords[keywordIndex - 1].Arguments);

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
                if (!(keywordForm.ThisFormKeywords[keywordIndex - 1].Params == null)
                    && !(keywordForm.ThisFormKeywords[keywordIndex - 1].Params.Count == 0))
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

            if (keywordForm.Controls["DynamicTestStep" + keywordIndex + "Name"] != null)
                keywordForm.Controls["DynamicTestStep" + keywordIndex + "Name"].Text = keywordForm.ThisFormKeywords[keywordIndex - 1].Name.Trim();
        }

        internal static void UpdateKeywordInThisTestCase(object sender, TestCaseAddForm testCaseAddForm)
        {
            if (RobotAutomationHelper.Log) Console.WriteLine("UpdateKeywordInThisTestCase " + ((TextWithList)sender).Name + " " + testCaseAddForm.Name);
            int keywordIndex = int.Parse(((TextWithList)sender).Name.Replace("DynamicTestStep", "").Replace("Name", ""));

            if (testCaseAddForm.Keywords[keywordIndex - 1].Type.Equals(KeywordType.CUSTOM))
            {
                string buttonImplementation = "Add Implementation";
                if (testCaseAddForm.Keywords[keywordIndex - 1].Implemented)
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

            
            if (testCaseAddForm.Keywords[keywordIndex - 1].Type.Equals(KeywordType.CUSTOM))
            {
                List<string> args = new List<string>();
                args = StringAndListOperations.ReturnListOfArgs(testCaseAddForm.Keywords[keywordIndex - 1].Arguments);

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
                if (!(testCaseAddForm.Keywords[keywordIndex - 1].Params == null)
                    && !(testCaseAddForm.Keywords[keywordIndex - 1].Params.Count == 0))
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

            if (testCaseAddForm.Controls["DynamicTestStep" + keywordIndex + "Name"] != null)
                testCaseAddForm.Controls["DynamicTestStep" + keywordIndex + "Name"].Text = testCaseAddForm.Keywords[keywordIndex - 1].Name.Trim();
        }

        internal static void CheckKeywordTypeAndReturnKeyword(Keyword keyword, string name)
        {
            if (RobotAutomationHelper.Log) Console.WriteLine("CheckKeywordTypeAndReturnKeyword " + keyword.Name + " " + name);
            foreach (Keyword seleniumKeyword in FormControls.Suggestions)
                if (seleniumKeyword.Name.Trim().ToLower().Equals(name.ToLower()))
                {
                    keyword.CopyKeyword(seleniumKeyword); //CopyKeyword
                    return;
                }

            if (keyword.Type != KeywordType.CUSTOM)
            {
                keyword.CopyKeyword(new Keyword(name, FilesAndFolderStructure.GetFolder() + "Auto.robot")); //CopyKeyword
                keyword.Type = KeywordType.CUSTOM;
                return;
            }

            foreach (Keyword seleniumKeyword in FormControls.Suggestions)
                if (seleniumKeyword.Name.Trim().ToLower().Equals(keyword.Name.Trim().ToLower()))
                {
                    keyword.CopyKeyword(new Keyword(name, FilesAndFolderStructure.GetFolder() + "Auto.robot")); //CopyKeyword
                    keyword.Type = KeywordType.CUSTOM;
                    return;
                }

            keyword.Name = name;
            keyword.Type = KeywordType.CUSTOM;
        }
    }
}
