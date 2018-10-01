using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RobotAutomationHelper.Scripts
{
    public static class FormControls
    {
        public static void AddControl(string type, string name, System.Drawing.Point location, System.Drawing.Size size, string text, System.Drawing.Color color, EventHandler eventHandler, Control owner)
        {
            Control tempControl;
            switch (type.ToLower())
            {
                case "textbox": tempControl = new TextBox(); break;
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
                case "checkbox": owner.Controls.Add((CheckBox)tempControl); break;
                case "button": owner.Controls.Add((Button)tempControl); break;
                default: owner.Controls.Add((Label)tempControl); break;
            }
        }
    }
}
