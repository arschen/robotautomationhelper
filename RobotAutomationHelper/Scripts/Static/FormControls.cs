using System;
using System.Drawing;
using System.Windows.Forms;
using RobotAutomationHelper.Forms;
using RobotAutomationHelper.Scripts.CustomControls;

namespace RobotAutomationHelper.Scripts.Static
{
    internal static class FormControls
    {
        internal static void AddControl(string type, string name, int indexOf, Point location, Size size, string text, Color color, EventHandler eventHandler, Control owner)
        {
            if (Forms.RobotAutomationHelper.Log) Console.WriteLine(@"AddControl " + @" " + type + @" " + name + @" " + text);
            Control tempControl;

            if (owner.Controls.Find(name, false).Length > 0)
                //if (RobotAutomationHelper.Log) 
                Console.WriteLine(name + @" | " + owner.Controls.Find(name,false).Length);

            switch (type.ToLower())
            {
                case "textbox": tempControl = new TextBox(); break;
                case "textwithlist": tempControl = new TextWithList(owner as BaseKeywordAddForm, indexOf); break;
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

        internal static void RemoveControlByKey(string key, Control.ControlCollection controlCollection)
        {
            if (Forms.RobotAutomationHelper.Log) Console.WriteLine(@"RemoveControlByKey " + key);
            if (Forms.RobotAutomationHelper.Log) Console.WriteLine(key + @" = " + controlCollection.Find(key, false).Length);
            while (controlCollection.Find(key, false).Length != 0)
                controlCollection.RemoveByKey(key);
        }
    }
}
