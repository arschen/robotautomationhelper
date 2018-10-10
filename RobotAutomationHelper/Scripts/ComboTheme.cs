using System.Drawing;
using System.Windows.Forms;

namespace RobotAutomationHelper.Scripts
{
    internal class ComboTheme : ComboBox
    {
        new internal DrawMode DrawMode { get; set; }
        internal Color HighlightColor { get; set; }
        private ToolTip toolTip = new ToolTip();

        internal ComboTheme()
        {
            base.DrawMode = DrawMode.OwnerDrawFixed;
            HighlightColor = SystemColors.Highlight;
            DrawItem += new DrawItemEventHandler(ComboTheme_DrawItem);
        }

        internal void HideToolTip()
        {
            toolTip.Hide(this);
        }

        internal void ComboTheme_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index >= 0)
            {
                ComboTheme box = ((ComboTheme)sender);
                if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                {
                    e.Graphics.FillRectangle(new SolidBrush(HighlightColor), e.Bounds);
                }
                else { e.Graphics.FillRectangle(new SolidBrush(box.BackColor), e.Bounds); }

                e.Graphics.DrawString(((ComboBoxObject)box.Items[e.Index]).Text,
                     e.Font, new SolidBrush(box.ForeColor),
                     new Point(e.Bounds.X, e.Bounds.Y));
                e.DrawFocusRectangle();

                if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                    this.toolTip.Show(((ComboBoxObject)box.Items[e.Index]).Documentation, box, e.Bounds.Right, e.Bounds.Bottom);
                else
                    this.toolTip.Hide(box);
                e.DrawFocusRectangle();
            }
        }
    }
}
