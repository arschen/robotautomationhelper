using RobotAutomationHelper.Forms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace RobotAutomationHelper.Scripts
{
    internal class TextWithList : TextBox
    {

        private SuggestionsList SuggestionsList;
        private Control ParentControl;
        private bool ForcedFocusToList { get; set; }
        private int IndexOf;

        internal TextWithList(Control Parent, int IndexOf)
        {
            ParentControl = Parent;
            SuggestionsList = new SuggestionsList(this);
            this.IndexOf = IndexOf;
        }

        protected override void OnTextChanged(EventArgs e)
        {
            if (Focused || SuggestionsList.SelectionPerformed)
            {
                base.OnTextChanged(e);
                string txt = Text;

                // if an item is selected from the list, then TriggerUpdate
                if (SuggestionsList.SelectionPerformed)
                {
                    string realName = ((SuggestionsListObjects)SuggestionsList.Items[SuggestionsList.SelectedIndex]).ValueMember;
                    SuggestionsList.SelectionPerformed = false;
                    HideSuggestionsList();
                    TriggerUpdate(realName);
                    EnableKeywordFields();
                }
                else
                {
                    DisableKeywordFields();
                    // find all the items in suggestion that match the current text
                    List<SuggestionsListObjects> foundItems = ReturnSuggestionsMatches(txt);

                    if (foundItems.Count > 0)
                    {
                        //show suggestions list
                        SuggestionsList.Items.Clear();
                        SuggestionsList.Items.AddRange(foundItems.ToArray());
                        SuggestionsList.Visible = true;
                        SuggestionsList.Location = new Point(Location.X, Location.Y + 20);
                        SuggestionsList.Size = new Size(Size.Width, foundItems.Count * SuggestionsList.ItemHeight > 200 ? 200 : (foundItems.Count + 1) * SuggestionsList.ItemHeight);
                        SuggestionsList.IntegralHeight = true;
                        FormControls.RemoveControlByKey(SuggestionsList.Name, ParentControl.Controls);
                        ParentControl.Controls.Add(SuggestionsList);
                        SuggestionsList.BringToFront();
                    }
                    else
                    {
                        //hide suggestions list
                        HideSuggestionsList();
                    }
                }
            }
        }

        //on Enter triggers update and hides suggestions
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.KeyCode == Keys.Enter && e.KeyCode == Keys.Return)
            {
                HideSuggestionsList();
                TriggerUpdate("");
                EnableKeywordFields();
            }
            else
            {
                if (e.KeyCode == Keys.Down && Parent.Controls.Find("SuggestionsList", false).Length > 0)
                {
                    ForcedFocusToList = true;
                    SuggestionsList.SelectedIndex = 0;
                    Parent.Controls["SuggestionsList"].Focus();
                }
                else
                {
                    if (e.KeyCode == Keys.Up && Parent.Controls.Find("SuggestionsList", false).Length > 0)
                    {
                        ForcedFocusToList = true;
                        SuggestionsList.SelectedIndex = SuggestionsList.Items.Count - 1;
                        Parent.Controls["SuggestionsList"].Focus();
                    }
                }
            }
        }

        protected override void OnLeave(EventArgs e)
        {
            base.OnLeave(e);
            if (!ForcedFocusToList)
            {
                HideSuggestionsList();
                EnableKeywordFields();
            }
            ForcedFocusToList = false;
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            OnTextChanged(e);
        }

        private List<SuggestionsListObjects> ReturnSuggestionsMatches(string txt)
        {
            List<SuggestionsListObjects> foundItems = new List<SuggestionsListObjects>();
            foreach (Keyword keyword in FormControls.Suggestions)
            //if (!keyword.Name.ToLower().Trim().Equals(txt.ToLower().Trim()))
            {
                bool containsAll = true;
                foreach (string temp in txt.ToLower().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (RobotAutomationHelper.Log) Console.WriteLine(keyword.ToString());
                    if (!keyword.Name.ToLower().Contains(temp))
                    {
                        containsAll = false;
                        break;
                    }
                }
                if (containsAll)
                {
                    if (keyword.Type != KeywordType.CUSTOM)
                        foundItems.Add(new SuggestionsListObjects { Text = keyword.ToString(), ValueMember = keyword.Name, Documentation = keyword.Documentation });
                    else
                        foundItems.Add(new SuggestionsListObjects { Text = keyword.ToString().Trim(), ValueMember = keyword.Name.Trim(), Documentation = keyword.OutputFilePath + "\n" + keyword.Documentation.Trim() });
                }
            }
            return foundItems;
        }

        private void TriggerUpdate(string textChangedPassed)
        {
            bool isKeywordAddForm = (ParentControl as Form).Name.Contains("Keyword");
            if (ParentControl.Name.Contains("Keyword"))
                (ParentControl as KeywordAddForm).UpdateTheKeywordOnNameChange(this, textChangedPassed);
            else
            {
                if (ParentControl.Name.Contains("Settings"))
                {
                    (ParentControl as SettingsAddForm).UpdateTheKeywordOnNameChange(this, textChangedPassed);
                }
                else
                    if (ParentControl.Name.Contains("TestCase"))
                        (ParentControl as TestCaseAddForm).UpdateTheKeywordOnNameChange(this, textChangedPassed);
            }
        }

        internal void HideSuggestionsList()
        {
            SuggestionsList.Visible = false;
            SuggestionsList.HideToolTip();
            ParentControl.Controls.Remove(SuggestionsList);
        }

        internal void DisableKeywordFields()
        {
            if (ParentControl.Name.Contains("Keyword"))
                (ParentControl as KeywordAddForm).DisableKeywordFields(IndexOf);
            else
            {
                if (ParentControl.Name.Contains("Settings"))
                {
                    (ParentControl as SettingsAddForm).DisableKeywordFields(IndexOf);
                }
                else
                    if (ParentControl.Name.Contains("TestCase"))
                    (ParentControl as TestCaseAddForm).DisableKeywordFields(IndexOf);
            }
        }

        internal void EnableKeywordFields()
        {
            if (ParentControl.Name.Contains("Keyword"))
                (ParentControl as KeywordAddForm).EnableKeywordFields(IndexOf);
            else
            {
                if (ParentControl.Name.Contains("Settings"))
                {
                    (ParentControl as SettingsAddForm).EnableKeywordFields(IndexOf);
                }
                else
                    if (ParentControl.Name.Contains("TestCase"))
                    (ParentControl as TestCaseAddForm).EnableKeywordFields(IndexOf);
            }
        }
    }
}
