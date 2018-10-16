using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace RobotAutomationHelper.Scripts
{
    internal class TextWithList : TextBox
    {

        private SuggestionsList SuggestionsList = new SuggestionsList();
        private Control ParentControl;

        internal TextWithList(Control Parent)
        {
            this.ParentControl = Parent;
        }

        protected override void OnTextChanged(EventArgs e)
        {
            string txt = Text;

            List<SuggestionsListObjects> foundItems = new List<SuggestionsListObjects>();
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
                        if (keyword.Type != KeywordType.CUSTOM)
                            foundItems.Add(new SuggestionsListObjects { Text = keyword.ToString(), ValueMember = keyword.GetKeywordName(), Documentation = keyword.GetKeywordDocumentation() });
                        else
                            foundItems.Add(new SuggestionsListObjects { Text = keyword.ToString().Trim(), ValueMember = keyword.GetKeywordName().Trim(), Documentation = keyword.GetOutputFilePath() + "\n" + keyword.GetKeywordDocumentation().Trim() });
                    }
                }

            if (foundItems.Count > 0)
            {
                SuggestionsList.Items.Clear();
                SuggestionsList.Items.AddRange(foundItems.ToArray());
                SuggestionsList.Visible = true;
                SuggestionsList.Location = new Point(this.Location.X, this.Location.Y + 20);
                SuggestionsList.Size = new Size(this.Size.Width, 200);
                SuggestionsList.DisplayMember = "ValueMember";
                SuggestionsList.IntegralHeight = true;
                ParentControl.Controls.Add(SuggestionsList);
                return;
            }
            else
            {
                SuggestionsList.Visible = false;
                SuggestionsList.HideToolTip();
                ParentControl.Controls.Remove(SuggestionsList);
            }
        }

        protected override void OnLeave(EventArgs e)
        {
            SuggestionsList.Visible = false;
            SuggestionsList.HideToolTip();
            ParentControl.Controls.Remove(SuggestionsList);
        }
    }
}
