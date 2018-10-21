using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RobotAutomationHelper.Scripts
{
    internal class LabelWithToolTip : Label
    {
        private ToolTip toolTip = new ToolTip();

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            toolTip.Show(Text, this, e.Location.X + 10, Height);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            toolTip.Hide(this);
        }
    }
}
