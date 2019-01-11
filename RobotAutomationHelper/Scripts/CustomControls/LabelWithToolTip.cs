using System;
using System.Windows.Forms;

namespace RobotAutomationHelper.Scripts.CustomControls
{
    public class LabelWithToolTip : Label
    {
        private readonly ToolTip _toolTip = new ToolTip();

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            _toolTip.Show(Text, this, e.Location.X + 10, Height);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _toolTip.Hide(this);
        }
    }
}
