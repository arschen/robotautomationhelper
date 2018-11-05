
using System;
using System.Drawing;
using System.Windows.Forms;

namespace RobotAutomationHelper.Scripts
{
    internal partial class SuggestionsList : ListBox
    {
        private Color HighlightColor { get; set; }
        private TextWithList TextWithListControl;
        internal bool SelectionPerformed { get; set; }

        internal SuggestionsList(TextWithList TextWithListControl)
        {
            InitializeComponent();
            Name = "SuggestionsList";
            base.DrawMode = DrawMode.OwnerDrawFixed;
            HighlightColor = SystemColors.Highlight;
            this.TextWithListControl = TextWithListControl;
            SelectionPerformed = false;
        }

        private ToolTip toolTip = new ToolTip();

        internal void HideToolTip()
        {
            toolTip.Hide(this);
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            base.OnDrawItem(e);
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
            base.OnMouseMove(e);
            //Get the item
            int nIdx = IndexFromPoint(e.Location);
            if ((nIdx >= 0) && (nIdx < Items.Count))
                SelectedIndex = nIdx;
        }

        //on Enter triggers update and hides suggestions
        protected override void OnKeyDown(KeyEventArgs e)
        {
            //base.OnKeyDown(e);
            if (e.KeyCode == Keys.Enter && e.KeyCode == Keys.Return)
            {
                SelectionPerformed = true;
                TextWithListControl.Focus();
                //TextWithListControl.Text = ((SuggestionsListObjects)Items[SelectedIndex]).ValueMember;
                TextWithListControl.SelectionStart = TextWithListControl.Text.Length;
            }
            else
            {
                if (e.KeyCode == Keys.Escape)
                {
                    TextWithListControl.Focus();
                    TextWithListControl.SelectionStart = TextWithListControl.Text.Length;
                }
            }

        }

        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            base.OnSelectedIndexChanged(e);
            if (SelectedIndex >= 0)
                toolTip.Show(((SuggestionsListObjects)Items[SelectedIndex]).Documentation, this, Size.Width, ItemHeight * (SelectedIndex - TopIndex) + 3);
            else
                toolTip.Hide(this);
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            TextWithListControl.EnableKeywordFields();
            Visible = false;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            //Get the item
            int nIdx = IndexFromPoint(e.Location);
            if ((nIdx >= 0) && (nIdx < Items.Count))
            {
                SelectionPerformed = true;
                TextWithListControl.Focus();
                TextWithListControl.SelectionStart = TextWithListControl.Text.Length;
            }
            else
                toolTip.Hide(this);
        }
    }
}
