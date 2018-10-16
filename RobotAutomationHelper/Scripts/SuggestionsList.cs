
using System.Drawing;
using System.Windows.Forms;

namespace RobotAutomationHelper.Scripts
{
    internal partial class SuggestionsList : ListBox
    {
        private Color HighlightColor { get; set; }

        internal SuggestionsList()
        {
            InitializeComponent();
            this.Name = "SuggesionsList";
            base.DrawMode = DrawMode.OwnerDrawFixed;
            HighlightColor = SystemColors.Highlight;
        }

        private ToolTip toolTip = new ToolTip();

        internal void HideToolTip()
        {
            toolTip.Hide(this);
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            if (e.Index >= 0)
            {
                if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                {
                    e.Graphics.FillRectangle(new SolidBrush(HighlightColor), e.Bounds);
                }
                else { e.Graphics.FillRectangle(new SolidBrush(BackColor), e.Bounds); }

                e.Graphics.DrawString(((SuggestionsListObjects)Items[e.Index]).Text,
                        e.Font, new SolidBrush(ForeColor),
                        new Point(e.Bounds.X, e.Bounds.Y));
                e.DrawFocusRectangle();

                if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                    this.toolTip.Show(((SuggestionsListObjects)Items[e.Index]).Documentation, this, e.Bounds.Right, e.Bounds.Bottom);
                else
                    this.toolTip.Hide(this);
                e.DrawFocusRectangle();
            }
        }
    }
}
