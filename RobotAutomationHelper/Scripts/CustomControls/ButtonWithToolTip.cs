using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RobotAutomationHelper.Scripts.CustomControls
{
    internal partial class ButtonWithToolTip : Button
    {
        private ToolTip toolTip = new ToolTip();
        private string toolTipText = "";

        internal ButtonWithToolTip()
        {
            InitializeComponent();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (!Enabled)
                toolTip.Show(Text, this, e.Location.X + 10, Height);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            toolTip.Hide(this);
        }

        internal void SetupToolTipText(bool ContainsTwoEmptySpaces, bool EmptyName)
        {
            toolTipText = "";
            if (ContainsTwoEmptySpaces)
                toolTipText += "Cannot have names with 2 empty spaces.";
            if (EmptyName)
                toolTipText += "Cannot have empty names.";
        }
    }
}
