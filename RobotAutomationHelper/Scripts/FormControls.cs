using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using static System.Windows.Forms.Form;

namespace RobotAutomationHelper.Scripts
{
    internal static class FormControls
    {
        internal static List<Keyword> Suggestions = new List<Keyword>();
        
        internal static int selectionPointer;

        // checks if text update/change is triggered inside UpdateAutoCompleteComboBox recursively
        private static bool checkDouble = false;

        internal static void AddControl(string type, string name, Point location, Size size, string text, Color color, EventHandler eventHandler, Control owner)
        {
            //Console.WriteLine("AddControl " + " " + type + " " + name + " " + text);
            Control tempControl;

            if (owner.Controls.Find(name, false).Length > 1)
                Console.WriteLine(name + " | " + owner.Controls.Find(name,false).Length);

            switch (type.ToLower())
            {
                case "textbox": tempControl = new TextBox(); break;
                case "combobox": tempControl = new ComboTheme(); break;
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
                case "combobox": owner.Controls.Add((ComboTheme)tempControl); break;
                case "checkbox": owner.Controls.Add((CheckBox)tempControl); break;
                case "button": owner.Controls.Add((Button)tempControl); break;
                default: owner.Controls.Add((Label)tempControl); break;
            }
        }

        internal static void UpdateOutputFileSuggestions(ComboBox comboBox)
        {
            Console.WriteLine("UpdateOutputFileSuggestions " + comboBox.Name);
            comboBox.Items.Clear();
            comboBox.AutoCompleteCustomSource.Clear();
            comboBox.Items.AddRange(FilesAndFolderStructure.GetFilesList().ToArray());
            comboBox.AutoCompleteCustomSource.AddRange(FilesAndFolderStructure.GetFilesList().ToArray());
            comboBox.AutoCompleteSource = AutoCompleteSource.CustomSource;
            comboBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
        }

        internal static string textBeforeDroppedDown = "";

        internal static void UpdateAutoCompleteComboBox(object sender, EventArgs e)
        {
            Console.WriteLine("UpdateAutoCompleteComboBox");
            var comboTheme = sender as ComboTheme;
            if (comboTheme == null)
                return;

            selectionPointer = comboTheme.SelectionStart;
            //Console.WriteLine(checkDouble);
            if (!checkDouble)
            {
                if (BaseKeywordAddForm.keyEvent != Keys.Down && BaseKeywordAddForm.keyEvent != Keys.Up)
                {
                    string txt = comboTheme.Text;

                    List<Object> foundItems = new List<Object>();
                    foreach (Keyword keyword in Suggestions)
                        if (!string.IsNullOrEmpty(txt))
                        {
                            bool containsAll = true;
                            foreach (string temp in txt.ToLower().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                            {
                                //Console.WriteLine(keyword.ToString());
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
                               if (keyword.Type != KeywordType.CUSTOM)
                                    foundItems.Add(new ComboBoxObject{Text = keyword.ToString(), ValueMember = keyword.GetKeywordName(), Documentation = keyword.GetKeywordDocumentation()});
                                else
                                    foundItems.Add(new ComboBoxObject { Text = keyword.ToString().Trim(), ValueMember = keyword.GetKeywordName().Trim(), Documentation = keyword.GetOutputFilePath() + "\n" + keyword.GetKeywordDocumentation().Trim() });
                                //Console.WriteLine("Success: " + keyword.ToString());
                            }
                        }

                    if (foundItems.Count > 0)
                    {
                        checkDouble = true;
                        comboTheme.Items.Clear();
                        comboTheme.Items.AddRange(foundItems.ToArray());
                        comboTheme.DroppedDown = true;
                        //Console.WriteLine(comboTheme.SelectedIndex + " after /tb: " + textBeforeDroppedDown + " /txt: " + txt + " /cb: " + comboTheme.Text);
                        Cursor.Current = Cursors.Default;
                        //Console.WriteLine(comboBox.Text + " | " + txt + " suggestions");
                        if (!comboTheme.Text.Equals(txt))
                        {
                            //Console.WriteLine("done " + BaseKeywordAddForm.prevEnterKey);
                            if (!BaseKeywordAddForm.prevEnterKey)
                                comboTheme.Text = txt;
                            else
                                BaseKeywordAddForm.prevEnterKey = true;
                        }
                        //Console.WriteLine("Second pass - /tb: " + textBeforeDroppedDown + " /txt: " + txt + " /cb: " + comboTheme.Text);
                        comboTheme.SelectionStart = selectionPointer;
                        checkDouble = false;
                        return;
                    }
                    else
                    {
                        comboTheme.DroppedDown = false;
                        comboTheme.HideToolTip();
                        //Console.WriteLine(txt + " | " + comboBox.SelectionStart + " no suggestions");
                    }

                }
            }
            checkDouble = false;
        }

        internal static void DataChanged(object sender, EventArgs e, string text)
        {
            Console.WriteLine("DataChanged " + (sender as ComboTheme).Name + " " + text);
            (sender as ComboTheme).Text = text;
        }

        internal static void AddSuggestionsToComboBox(ComboTheme comboBox)
        {
            Console.WriteLine("AddSuggestionsToComboBox " + comboBox.Name);
            string current = comboBox.Text;
            foreach (Keyword keyword in Suggestions)
                if (keyword.Type != KeywordType.CUSTOM)
                    comboBox.Items.Add(new ComboBoxObject { Text = keyword.ToString(), ValueMember = keyword.GetKeywordName(), Documentation = keyword.GetKeywordDocumentation() });
                else
                    comboBox.Items.Add(new ComboBoxObject { Text = keyword.ToString().Trim(), ValueMember = keyword.GetKeywordName().Trim(), Documentation = keyword.GetOutputFilePath() + "\n" + keyword.GetKeywordDocumentation().Trim() });
        }

        internal static void ComboBoxMouseClick(object sender, MouseEventArgs e)
        {
            var combo = sender as ComboTheme;
            Console.WriteLine("ComboBoxMouseClick " + combo.Name);
            if (combo.Items.Count != Suggestions.Count)
            {
                combo.Items.Clear();
                AddSuggestionsToComboBox(combo);
            }
        }

        internal static void CheckKeywordTypeAndReturnKeyword(Keyword keyword, string name)
        {
            Console.WriteLine("CheckKeywordTypeAndReturnKeyword " + keyword.GetKeywordName() + " " + name);
            foreach (Keyword seleniumKeyword in Suggestions)
                if (seleniumKeyword.GetKeywordName().Trim().ToLower().Equals(name.ToLower()))
                {
                    keyword.CopyKeyword(seleniumKeyword);
                    return;
                }

            if (keyword.Type != KeywordType.CUSTOM)
            {
                keyword.CopyKeyword(new Keyword(name, FilesAndFolderStructure.GetFolder() + "Auto.robot"));
                keyword.Type = KeywordType.CUSTOM;
                return;
            }

            foreach (Keyword seleniumKeyword in Suggestions)
                if (seleniumKeyword.GetKeywordName().Trim().ToLower().Equals(keyword.GetKeywordName().Trim().ToLower()))
                {
                    keyword.CopyKeyword(new Keyword(name, FilesAndFolderStructure.GetFolder() + "Auto.robot"));
                    keyword.Type = KeywordType.CUSTOM;
                    return;
                }

            keyword.Type = KeywordType.CUSTOM;
        }

        internal static void RemoveControlByKey(string key, ControlCollection controlCollection)
        {
            Console.WriteLine("RemoveControlByKey " + key);
            //Console.WriteLine(key + " = " + controlCollection.Find(key, false).Length);
            while (controlCollection.Find(key, false).Length != 0)
                controlCollection.RemoveByKey(key);
        }

        internal static void RemoveControlByKey(string key, Control.ControlCollection controlCollection)
        {
            Console.WriteLine("RemoveControlByKey " + key);
            //Console.WriteLine(key + " = " + controlCollection.Find(key, false).Length);
            while (controlCollection.Find(key, false).Length != 0)
                controlCollection.RemoveByKey(key);
        }
    }
}
