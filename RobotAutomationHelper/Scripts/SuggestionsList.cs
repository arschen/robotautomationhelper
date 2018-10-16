
using System;
using System.Drawing;
using System.Windows.Forms;

namespace RobotAutomationHelper.Scripts
{
    internal partial class SuggestionsList : ListBox
    {
        private Color HighlightColor { get; set; }
        private TextWithList TextWithListControl;

        internal SuggestionsList(TextWithList TextWithListControl)
        {
            InitializeComponent();
            this.Name = "SuggesionsList";
            base.DrawMode = DrawMode.OwnerDrawFixed;
            HighlightColor = SystemColors.Highlight;
            this.TextWithListControl = TextWithListControl;
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
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            //Get the item
            int nIdx = IndexFromPoint(e.Location);
            if ((nIdx >= 0) && (nIdx < Items.Count))
                toolTip.Show(((SuggestionsListObjects)Items[nIdx]).Documentation, this);
            else
                toolTip.Hide(this);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            int nIdx = IndexFromPoint(e.Location);
            if ((nIdx >= 0) && (nIdx < Items.Count))
                TextWithListControl.Text = (((SuggestionsListObjects)Items[nIdx]).Text);
        }
    }
}
