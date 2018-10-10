using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace RobotAutomationHelper.Scripts
{
    internal static class FormControls
    {
        internal static List<Keyword> Suggestions = new List<Keyword>();
        internal static Keys keyEvent;
        internal static int selectionPointer;

        // checks if text update/change is triggered inside UpdateAutoCompleteComboBox recursively
        private static bool checkDouble = false;

        internal static void AddControl(string type, string name, Point location, Size size, string text, Color color, EventHandler eventHandler, Control owner)
        {
            Control tempControl;
            switch (type.ToLower())
            {
                case "textbox": tempControl = new TextBox(); break;
                case "combobox": tempControl = new ComboBox(); break;
                case "checkbox": tempControl = new CheckBox(); ((CheckBox)tempControl).Checked = true; break;
                case "button": tempControl = new Button(); tempControl.Click += eventHandler; break;
                default: tempControl = new Label(); break;
            }

            tempControl.Name = name;
            tempControl.Location = location;
            tempControl.Size = size;
            tempControl.Text = text;
            tempControl.ForeColor = color;
            tempControl.Anchor = (AnchorStyles.Top | AnchorStyles.Left);
            tempControl.Visible = true;

            switch (type.ToLower())
            {
                case "textbox": owner.Controls.Add((TextBox)tempControl); break;
                case "combobox": owner.Controls.Add((ComboBox)tempControl); break;
                case "checkbox": owner.Controls.Add((CheckBox)tempControl); break;
                case "button": owner.Controls.Add((Button)tempControl); break;
                default: owner.Controls.Add((Label)tempControl); break;
            }
        }

        internal static void UpdateOutputFileSuggestions(ComboBox comboBox)
        {
            comboBox.Items.Clear();
            comboBox.AutoCompleteCustomSource.Clear();
            comboBox.Items.AddRange(FilesAndFolderStructure.GetFilesList().ToArray());
            comboBox.AutoCompleteCustomSource.AddRange(FilesAndFolderStructure.GetFilesList().ToArray());
            comboBox.AutoCompleteSource = AutoCompleteSource.CustomSource;
            comboBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
        }

        internal static void UpdateAutoCompleteComboBox(object sender, EventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox == null)
                return;

            selectionPointer = comboBox.SelectionStart;
            //Console.WriteLine(checkDouble);
            if (!checkDouble)
            {
                if (keyEvent != Keys.Down && keyEvent != Keys.Up)
                {
                    string txt = comboBox.Text;

                    List<string> foundItems = new List<string>();
                    foreach (Keyword keyword in Suggestions)
                        if (!string.IsNullOrEmpty(txt))
                        {
                            bool containsAll = true;
                            foreach (string temp in txt.ToLower().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                            {
                                if (!keyword.GetKeywordName().ToLower().Contains(temp))
                                {
                                    containsAll = false;
                                    break;
                                }
                            }
                            if (containsAll)
                            {
                                //Console.WriteLine(keyword.GetKeywordName().ToLower() + " | ");
                                //foreach (string temp in txt.ToLower().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                                //    Console.Write(temp + " + ");
                                foundItems.Add(keyword.GetKeywordName());
                            }
                        }

                    if (foundItems.Count > 0 && !foundItems.ToArray().Equals(comboBox.Items))
                    {
                        checkDouble = true;
                        comboBox.Items.Clear();
                        comboBox.Items.AddRange(foundItems.ToArray());
                        comboBox.DropDownStyle = ComboBoxStyle.DropDown;
                        comboBox.DroppedDown = true;
                        Cursor.Current = Cursors.Default;
                        //Console.WriteLine(comboBox.Text + " | " + txt + " suggestions");
                        if (!comboBox.Text.Equals(txt))
                            comboBox.Text = txt;
                        comboBox.SelectionStart = selectionPointer;
                        checkDouble = false;
                        return;
                    }
                    else
                    {
                        comboBox.DroppedDown = false;
                        //Console.WriteLine(txt + " | " + comboBox.SelectionStart + " no suggestions");
                    }

                }
            }
            checkDouble = false;
        }

        internal static void AddSuggestionsToComboBox(ComboBox comboBox)
        {
            foreach (Keyword key in Suggestions)
                comboBox.Items.Add(key.GetKeywordName());
        }

        internal static void ComboBoxMouseClick(object sender, MouseEventArgs e)
        {
            var combo = sender as ComboBox;
            if (combo.Items.Count != Suggestions.Count)
            {
                combo.Items.Clear();
                foreach (Keyword key in Suggestions)
                    combo.Items.Add(key.GetKeywordName());
            }
        }

        internal static KeywordType GetKeywordType(Keyword keyword)
        {
            foreach (Keyword seleniumKeywords in HtmlLibsGetter.Selenium)
            {
                if (seleniumKeywords.GetKeywordName().ToLower().Equals(keyword.GetKeywordName().ToLower()))
                {
                    return KeywordType.SELENIUM;
                }
            }
            return KeywordType.CUSTOM;
        }
    }
}
