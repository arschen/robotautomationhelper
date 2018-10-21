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

        internal static void AddControl(string type, string name, Point location, Size size, string text, Color color, EventHandler eventHandler, Control owner)
        {
            if (RobotAutomationHelper.Log) Console.WriteLine("AddControl " + " " + type + " " + name + " " + text);
            Control tempControl;

            if (owner.Controls.Find(name, false).Length > 0)
                //if (RobotAutomationHelper.Log) 
                Console.WriteLine(name + " | " + owner.Controls.Find(name,false).Length);

            switch (type.ToLower())
            {
                case "textbox": tempControl = new TextBox(); break;
                case "textwithlist": tempControl = new TextWithList(owner); break;
                case "checkbox": tempControl = new CheckBox(); ((CheckBox)tempControl).Checked = true; break;
                case "button": tempControl = new Button(); tempControl.Click += eventHandler; break;
                case "labelwithtooltip": tempControl = new LabelWithToolTip(); break;
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
                case "textwithlist": owner.Controls.Add((TextWithList)tempControl); break;
                case "checkbox": owner.Controls.Add((CheckBox)tempControl); break;
                case "button": owner.Controls.Add((Button)tempControl); break;
                case "labelwithtooltip": owner.Controls.Add((LabelWithToolTip)tempControl); break;
                default: owner.Controls.Add((Label)tempControl); break;
            }
        }

        internal static void UpdateOutputFileSuggestions(ComboBox comboBox, string type)
        {
            if (RobotAutomationHelper.Log) Console.WriteLine("UpdateOutputFileSuggestions " + comboBox.Name);
            comboBox.Items.Clear();
            comboBox.AutoCompleteCustomSource.Clear();
            comboBox.Items.AddRange(FilesAndFolderStructure.GetSavedFiles(type).ToArray());
            comboBox.AutoCompleteCustomSource.AddRange(FilesAndFolderStructure.GetSavedFiles(type).ToArray());
            comboBox.AutoCompleteSource = AutoCompleteSource.CustomSource;
            if (comboBox.DropDownStyle != ComboBoxStyle.DropDownList)
                comboBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
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
