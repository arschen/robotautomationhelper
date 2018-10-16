using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace RobotAutomationHelper.Scripts
{
    internal class ComboTheme : ComboBox
    {
        new internal DrawMode DrawMode { get; set; }
        internal Color HighlightColor { get; set; }
        private ToolTip toolTip = new ToolTip();
        internal string textBeforeDroppedDown { get; set; }
        internal int selectionPointer { get; set; }
        // checks if text update/change is triggered inside UpdateAutoCompleteComboBox recursively
        private static bool checkDouble = false;

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

        protected override void OnDropDown(EventArgs e)
        {
            //if (RobotAutomationHelper.Log)
                Console.WriteLine("OnDropDown " +
                 this.SelectedIndex + " " + this.Text);
            this.SelectedItem = null;
        }

        protected override void OnDropDownClosed(EventArgs e)
        {
            //if (RobotAutomationHelper.Log)
            Console.WriteLine("OnDropDownClosed " +
             this.SelectedIndex + " " + this.Text);
            this.SelectedItem = null;
        }

        protected override void OnTextUpdate(EventArgs e)
        {
            if (RobotAutomationHelper.Log) Console.WriteLine("UpdateAutoCompleteComboBox");

            selectionPointer = this.SelectionStart;
            if (RobotAutomationHelper.Log) Console.WriteLine(checkDouble);
            if (!checkDouble)
            {
                if (BaseKeywordAddForm.keyEvent != Keys.Down && BaseKeywordAddForm.keyEvent != Keys.Up)
                {
                    string txt = this.Text;

                    List<Object> foundItems = new List<Object>();
                    foreach (Keyword keyword in FormControls.Suggestions)
                        if (!string.IsNullOrEmpty(txt))
                        {
                            bool containsAll = true;
                            foreach (string temp in txt.ToLower().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                            {
                                if (RobotAutomationHelper.Log) Console.WriteLine(keyword.ToString());
                                if (!keyword.GetKeywordName().ToLower().Contains(temp))
                                {
                                    containsAll = false;
                                    break;
                                }
                            }
                            if (containsAll)
                            {
                                if (RobotAutomationHelper.Log) Console.WriteLine(keyword.GetKeywordName().ToLower() + " | ");
                                //foreach (string temp in txt.ToLower().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                                //    if (RobotAutomationHelper.Log) Console.Write(temp + " + ");
                                if (keyword.Type != KeywordType.CUSTOM)
                                    foundItems.Add(new ComboBoxObject { Text = keyword.ToString(), ValueMember = keyword.GetKeywordName(), Documentation = keyword.GetKeywordDocumentation() });
                                else
                                    foundItems.Add(new ComboBoxObject { Text = keyword.ToString().Trim(), ValueMember = keyword.GetKeywordName().Trim(), Documentation = keyword.GetOutputFilePath() + "\n" + keyword.GetKeywordDocumentation().Trim() });
                                if (RobotAutomationHelper.Log) Console.WriteLine("Success: " + keyword.ToString());
                            }
                        }

                    if (foundItems.Count > 0)
                    {
                        checkDouble = true;
                        this.Items.Clear();
                        this.Items.AddRange(foundItems.ToArray());
                        this.DroppedDown = true;
                        if (RobotAutomationHelper.Log) Console.WriteLine(this.SelectedIndex + " after /tb: " + textBeforeDroppedDown + " /txt: " + txt + " /cb: " + this.Text);
                        Cursor.Current = Cursors.Default;
                        if (RobotAutomationHelper.Log) Console.WriteLine(this.Text + " | " + txt + " suggestions");
                        if (!this.Text.Equals(txt))
                        {
                            if (RobotAutomationHelper.Log) Console.WriteLine("done " + BaseKeywordAddForm.prevEnterKey);
                            if (!BaseKeywordAddForm.prevEnterKey)
                                this.Text = txt;
                            else
                                BaseKeywordAddForm.prevEnterKey = true;
                        }
                        if (RobotAutomationHelper.Log) Console.WriteLine("Second pass - /tb: " + textBeforeDroppedDown + " /txt: " + txt + " /cb: " + this.Text);
                        this.SelectionStart = selectionPointer;
                        checkDouble = false;
                        return;
                    }
                    else
                    {
                        this.DroppedDown = false;
                        this.HideToolTip();
                        if (RobotAutomationHelper.Log) Console.WriteLine(txt + " | " + this.SelectionStart + " no suggestions");
                    }

                }
            }
            checkDouble = false;
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

                if ((e.State & DrawItemState.Selected) == DrawItemState.Selected && box.DroppedDown)
                    this.toolTip.Show(((ComboBoxObject)box.Items[e.Index]).Documentation, box, e.Bounds.Right, e.Bounds.Bottom);
                else
                    this.toolTip.Hide(box);
                e.DrawFocusRectangle();
            }
        }
    }
}
