using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using RobotAutomationHelper.Forms;
using RobotAutomationHelper.Scripts.Objects;
using RobotAutomationHelper.Scripts.Static;

namespace RobotAutomationHelper.Scripts.CustomControls
{
    internal class TextWithList : TextBox
    {

        // Fields and Properties ===================================================
        private readonly SuggestionsList _suggestionsList;
        private readonly BaseKeywordAddForm _parentForm;
        private readonly int _indexOf;
        private bool _justGotFocused;
        private bool _changedImmediatelyAfterSelection;
        internal int MaxItemsInSuggestionsList { get; set; }
        private bool _updateNeeded;
        internal string UpdateValue = "";

        // Methods =================================================================
        internal TextWithList(BaseKeywordAddForm parent, int indexOf)
        {
            _parentForm = parent;
            _suggestionsList = new SuggestionsList(this);
            _indexOf = indexOf;
        }

        protected override void OnTextChanged(EventArgs e)
        {
            UpdateListNamesAndUpdateStateOfSave();
            _updateNeeded = true;
            if (!Focused && !_suggestionsList.SelectionPerformed) return;
            base.OnTextChanged(e);
            var txt = Text;

            // if an item is selected from the list, then TriggerUpdate
            if (_suggestionsList.SelectionPerformed)
            {
                var realName = ((SuggestionsListObjects)_suggestionsList.Items[_suggestionsList.SelectedIndex]).ValueMember;
                _suggestionsList.SelectionPerformed = false;
                _changedImmediatelyAfterSelection = true;
                _updateNeeded = false;
                //Console.WriteLine("OnTextChanged.SelectionPerformed Trigger Update: " + ((SuggestionsListObjects)SuggestionsList.Items[SuggestionsList.SelectedIndex]).Text);
                UpdateValue = ((SuggestionsListObjects)_suggestionsList.Items[_suggestionsList.SelectedIndex]).Text;
                TriggerUpdate(realName, ((SuggestionsListObjects)_suggestionsList.Items[_suggestionsList.SelectedIndex]).Text);
                HideSuggestionsList();
                EnableKeywordFields();
            }
            else
            {
                if (!_justGotFocused)
                    DisableKeywordFields();
                else
                    _justGotFocused = false;

                if (_changedImmediatelyAfterSelection)
                {
                    _changedImmediatelyAfterSelection = false;
                }
                else
                {
                    ShowSuggestions(txt);
                }
            }
        }

        //on Enter triggers update and hides suggestions
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.KeyCode != Keys.Enter || e.KeyCode != Keys.Return)
            {
                if (e.KeyCode != Keys.Down)
                {
                    if (e.KeyCode != Keys.Up) return;
                    ShowSuggestions(Text);
                    _suggestionsList.SelectedIndex = _suggestionsList.Items.Count - 1;
                    Parent.Controls["SuggestionsList"].Focus();
                }
                else
                {
                    ShowSuggestions(Text);
                    _suggestionsList.SelectedIndex = 0;
                    Parent.Controls["SuggestionsList"].Focus();
                }
            }
            else
            {
                HideSuggestionsList();
                //Console.WriteLine("OnKeyDown Trigger Update " + updateNeeded.ToString() + " / " + updateValue);
                TriggerUpdate("", _updateNeeded ? "" : UpdateValue);
                EnableKeywordFields();
                SelectionStart = Text.Length;
            }
        }
        protected override void OnLeave(EventArgs e)
        {
            base.OnLeave(e);
            if (Parent.Controls.Find("SuggestionsList", false).Length > 0)
            {
                if (Parent.Controls["SuggestionsList"].Focused) return;
                HideSuggestionsList();
                //Console.WriteLine("OnLeave Trigger Update " + updateNeeded.ToString() + " / " + updateValue);
                TriggerUpdate("", _updateNeeded ? "" : UpdateValue);
                EnableKeywordFields();
            }
            else
            {
                HideSuggestionsList();
                //Console.WriteLine("OnLeave Trigger Update " + updateNeeded.ToString() + " / " + updateValue);
                TriggerUpdate("", _updateNeeded ? "" : UpdateValue);
                EnableKeywordFields();
            }
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            _justGotFocused = true;
            OnTextChanged(e);
            _updateNeeded = false;
        }

        internal void TriggerUpdate(string textChangedPassed, string keywordType)
        {
            switch (_parentForm.FormType)
            {
                case FormType.Keyword:
                    ((KeywordAddForm) _parentForm).UpdateTheKeywordOnNameChange(this, textChangedPassed, keywordType);
                    break;
                case FormType.Settings:
                    ((SettingsAddForm) _parentForm).UpdateTheKeywordOnNameChange(this, textChangedPassed, keywordType);
                    break;
                case FormType.Params:
                    ((ParamAddForm) _parentForm).UpdateTheKeywordOnNameChange(this, textChangedPassed, keywordType);
                    break;
                case FormType.Test:
                    ((TestCaseAddForm) _parentForm).UpdateTheKeywordOnNameChange(this, textChangedPassed, keywordType);
                    break;
                case FormType.NameAndOutput:
                    ((NameAndOutputForm) _parentForm).UpdateTheKeywordOnNameChange(this, textChangedPassed, keywordType);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void UpdateListNamesAndUpdateStateOfSave()
        {
            switch (_parentForm.FormType)
            {
                case FormType.Keyword:
                    ((KeywordAddForm) _parentForm).UpdateNamesListAndUpdateStateOfSave();
                    break;
                case FormType.Settings:
                    ((SettingsAddForm) _parentForm).UpdateNamesListAndUpdateStateOfSave();
                    break;
                case FormType.Params:
                    ((ParamAddForm) _parentForm).UpdateListNamesAndUpdateStateOfSave();
                    break;
                case FormType.Test:
                    ((TestCaseAddForm) _parentForm).UpdateListNamesAndUpdateStateOfSave();
                    break;
                case FormType.NameAndOutput:
                    //(ParentForm as NameAndOutputForm).UpdateTheKeywordOnNameChange(this, textChangedPassed);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private List<SuggestionsListObjects> ReturnSuggestionsMatches(string txt)
        {
            var foundItems = new List<SuggestionsListObjects>();
            foreach (var lib in SuggestionsClass.Suggestions)
                if (lib.ToInclude)
                    foreach (var keyword in lib.LibKeywords)
                    //if (!keyword.Name.ToLower().Trim().Equals(txt.ToLower().Trim()))
                    {
                        var containsAll = true;
                        foreach (var temp in txt.ToLower().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            if (Forms.RobotAutomationHelper.Log) Console.WriteLine(keyword.ToString());
                            if (!keyword.Name.ToLower().Contains(temp))
                            {
                                containsAll = false;
                                break;
                            }
                        }

                        if (!containsAll) continue;
                        foundItems.Add(keyword.Type != KeywordType.Custom
                            ? new SuggestionsListObjects
                            {
                                Text = keyword.ToString(), ValueMember = keyword.Name,
                                Documentation = keyword.Documentation ?? ""
                            }
                            : new SuggestionsListObjects
                            {
                                Text = keyword.ToString().Trim(), ValueMember = keyword.Name.Trim(),
                                Documentation = keyword.OutputFilePath + "\n" + keyword.Documentation ?? ""
                            });
                    }
                    return foundItems;
        }

        private void ShowSuggestions(string textInTheField)
        {
            // find all the items in suggestion that match the current text
            var foundItems = ReturnSuggestionsMatches(textInTheField);

            if (foundItems.Count > 0)
            {
                //show suggestions list
                _suggestionsList.Items.Clear();
                // ReSharper disable once CoVariantArrayConversion
                _suggestionsList.Items.AddRange(foundItems.ToArray());
                _suggestionsList.Visible = true;
                _suggestionsList.Location = new Point(Location.X, Location.Y + Height);
                _suggestionsList.Size = new Size(Size.Width, foundItems.Count >= MaxItemsInSuggestionsList ? (MaxItemsInSuggestionsList + 1) * _suggestionsList.ItemHeight : (foundItems.Count + 1) * _suggestionsList.ItemHeight);
                _suggestionsList.IntegralHeight = true;
                FormControls.RemoveControlByKey(_suggestionsList.Name, _parentForm.Controls);
                _parentForm.Controls.Add(_suggestionsList);
                _suggestionsList.BringToFront();
            }
            else
            {
                //hide suggestions list
                HideSuggestionsList();
            }
        }
        internal void HideSuggestionsList()
        {
            _suggestionsList.Visible = false;
            _suggestionsList.HideToolTip();
            _parentForm.Controls.Remove(_suggestionsList);
        }

        internal void DisableKeywordFields()
        {
            switch (_parentForm.FormType)
            {
                case FormType.Keyword:
                    ((KeywordAddForm) _parentForm).DisableKeywordFields(_indexOf);
                    break;
                case FormType.Params:
                    ((ParamAddForm) _parentForm).DisableKeywordFields(_indexOf);
                    break;
                case FormType.Settings:
                    ((SettingsAddForm) _parentForm).DisableKeywordFields(_indexOf);
                    break;
                case FormType.Test:
                    ((TestCaseAddForm) _parentForm).DisableKeywordFields(_indexOf);
                    break;
                case FormType.NameAndOutput:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        internal void EnableKeywordFields()
        {
            switch (_parentForm.FormType)
            {
                case FormType.Keyword:
                    ((KeywordAddForm) _parentForm).EnableKeywordFields(_indexOf);
                    break;
                case FormType.Params:
                    ((ParamAddForm) _parentForm).EnableKeywordFields(_indexOf);
                    break;
                case FormType.Settings:
                    ((SettingsAddForm) _parentForm).EnableKeywordFields(_indexOf);
                    break;
                case FormType.Test:
                    ((TestCaseAddForm) _parentForm).EnableKeywordFields(_indexOf);
                    break;
                case FormType.NameAndOutput:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
