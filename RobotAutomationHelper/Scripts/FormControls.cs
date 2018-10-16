﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using static System.Windows.Forms.Form;

namespace RobotAutomationHelper.Scripts
{
    internal static class FormControls
    {
        internal static List<Keyword> Suggestions = new List<Keyword>();

        internal static void AddControl(string type, string name, Point location, Size size, string text, Color color, EventHandler eventHandler, Control owner)
        {
            if (RobotAutomationHelper.Log) Console.WriteLine("AddControl " + " " + type + " " + name + " " + text);
            Control tempControl;

            if (owner.Controls.Find(name, false).Length > 1)
                if (RobotAutomationHelper.Log) Console.WriteLine(name + " | " + owner.Controls.Find(name,false).Length);

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
            if (RobotAutomationHelper.Log) Console.WriteLine("UpdateOutputFileSuggestions " + comboBox.Name);
            comboBox.Items.Clear();
            comboBox.AutoCompleteCustomSource.Clear();
            comboBox.Items.AddRange(FilesAndFolderStructure.GetFilesList().ToArray());
            comboBox.AutoCompleteCustomSource.AddRange(FilesAndFolderStructure.GetFilesList().ToArray());
            comboBox.AutoCompleteSource = AutoCompleteSource.CustomSource;
            comboBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
        }

        internal static void DataChanged(object sender, EventArgs e, string text)
        {
            if (RobotAutomationHelper.Log) Console.WriteLine("DataChanged " + (sender as ComboTheme).Name + " " + text);
            (sender as ComboTheme).Text = text;
        }

        internal static void AddSuggestionsToComboBox(ComboTheme comboBox)
        {
            if (RobotAutomationHelper.Log) Console.WriteLine("AddSuggestionsToComboBox " + comboBox.Name);
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
            if (RobotAutomationHelper.Log) Console.WriteLine("ComboBoxMouseClick " + combo.Name);
            if (combo.Items.Count != Suggestions.Count)
            {
                combo.Items.Clear();
                AddSuggestionsToComboBox(combo);
            }
        }

        internal static void CheckKeywordTypeAndReturnKeyword(Keyword keyword, string name)
        {
            if (RobotAutomationHelper.Log) Console.WriteLine("CheckKeywordTypeAndReturnKeyword " + keyword.GetKeywordName() + " " + name);
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
            if (RobotAutomationHelper.Log) Console.WriteLine("RemoveControlByKey " + key);
            if (RobotAutomationHelper.Log) Console.WriteLine(key + " = " + controlCollection.Find(key, false).Length);
            while (controlCollection.Find(key, false).Length != 0)
                controlCollection.RemoveByKey(key);
        }

        internal static void RemoveControlByKey(string key, Control.ControlCollection controlCollection)
        {
            if (RobotAutomationHelper.Log) Console.WriteLine("RemoveControlByKey " + key);
            if (RobotAutomationHelper.Log) Console.WriteLine(key + " = " + controlCollection.Find(key, false).Length);
            while (controlCollection.Find(key, false).Length != 0)
                controlCollection.RemoveByKey(key);
        }
    }
}
