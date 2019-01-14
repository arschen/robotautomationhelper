using System;
using System.Drawing;
using System.Windows.Forms;

namespace RobotAutomationHelper.Scripts.CustomControls
{
    public partial class SuggestionsList : ListBox
    {
        private Color HighlightColor { get; }
        private readonly TextWithList _textWithListControl;
        public bool SelectionPerformed { get; set; }

        public SuggestionsList(TextWithList textWithListControl)
        {
            InitializeComponent();
            Name = "SuggestionsList";
            base.DrawMode = DrawMode.OwnerDrawFixed;
            HighlightColor = SystemColors.Highlight;
            _textWithListControl = textWithListControl;
            SelectionPerformed = false;
        }

        private readonly ToolTip _toolTip = new ToolTip();

        public void HideToolTip()
        {
            _toolTip.Hide(this);
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            base.OnDrawItem(e);
            if (e.Index < 0) return;
            e.Graphics.FillRectangle(
                (e.State & DrawItemState.Selected) == DrawItemState.Selected
                    ? new SolidBrush(HighlightColor)
                    : new SolidBrush(BackColor), e.Bounds);

            e.Graphics.DrawString(((SuggestionsListObjects)Items[e.Index]).Text,
                e.Font, new SolidBrush(ForeColor),
                new Point(e.Bounds.X, e.Bounds.Y));
            e.DrawFocusRectangle();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            //Get the item
            var nIdx = IndexFromPoint(e.Location);
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
                _textWithListControl.Focus();
                //TextWithListControl.Text = ((SuggestionsListObjects)Items[SelectedIndex]).ValueMember;
                _textWithListControl.SelectionStart = _textWithListControl.Text.Length;
            }
            else
            {
                if (e.KeyCode == Keys.Escape)
                {
                    _textWithListControl.Focus();
                    _textWithListControl.SelectionStart = _textWithListControl.Text.Length;
                }
            }

        }

        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            base.OnSelectedIndexChanged(e);
            if (SelectedIndex >= 0)
                _toolTip.Show(((SuggestionsListObjects)Items[SelectedIndex]).Documentation, this, Size.Width, ItemHeight * (SelectedIndex - TopIndex + 1) + 3);
            else
                _toolTip.Hide(this);
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            _textWithListControl.EnableKeywordFields();
            Visible = false;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            //Get the item
            var nIdx = IndexFromPoint(e.Location);
            if ((nIdx >= 0) && (nIdx < Items.Count))
            {
                SelectionPerformed = true;
                _textWithListControl.Focus();
                _textWithListControl.SelectionStart = _textWithListControl.Text.Length;
            }
            else
                _toolTip.Hide(this);
        }
    }
}
