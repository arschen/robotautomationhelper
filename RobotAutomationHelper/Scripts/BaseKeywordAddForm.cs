using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace RobotAutomationHelper.Scripts
{
    internal static class BaseKeywordAddForm
    {

        // change the field when the keyword name is changed
        internal static void ChangeTheKeywordFieldAfterSelection(object sender, EventArgs e, List<Keyword> Keywords)
        {
            if ((sender as ComboTheme).SelectedIndex != -1)
            {
                ComboTheme combo = sender as ComboTheme;
                int keywordIndex = int.Parse(combo.Name.Replace("Name", "").Replace("DynamicTestStep", ""));
                Console.WriteLine(combo.SelectedValue + " sv");
                Keywords[keywordIndex - 1].SetKeywordName(((ListControl)combo.Items[combo.SelectedIndex]).ValueMember);
                FormControls.GetKeywordType(Keywords[keywordIndex - 1]);
                Console.WriteLine(FormControls.GetKeywordType(Keywords[keywordIndex - 1]));
                combo.HideToolTip();
            }
            else
            {
                ChangeTheKeywordFieldAfterKeyPress(sender, e, Keywords);
            }
        }

        // change the field when the keyword name is changed
        internal static void ChangeTheKeywordFieldAfterKeyPress(object sender, EventArgs e, List<Keyword> Keywords)
        {
            ComboTheme combo = sender as ComboTheme;
            int keywordIndex = int.Parse(combo.Name.Replace("Name", "").Replace("DynamicTestStep", ""));
            Keywords[keywordIndex - 1].SetKeywordName(combo.Text);
            FormControls.GetKeywordType(Keywords[keywordIndex - 1]);
            Console.WriteLine(FormControls.GetKeywordType(Keywords[keywordIndex - 1]));
            combo.HideToolTip();
        }

        // handles key press for keyword name input, the case when return/enter is hit
        internal static void AutoCompleteComboBoxKeyPress(object sender, KeyEventArgs e, List<Keyword> Keywords)
        {
            var comboTheme = sender as ComboTheme;
            string txt = comboTheme.Text;
            FormControls.selectionPointer = comboTheme.SelectionStart;
            //Console.WriteLine(e.KeyCode);
            FormControls.keyEvent = e.KeyCode;
            if (FormControls.keyEvent == Keys.Return)
            {
                comboTheme.DroppedDown = false;
                comboTheme.Text = txt;
                comboTheme.SelectionStart = FormControls.selectionPointer;
                ChangeTheKeywordFieldAfterKeyPress(sender, e, Keywords);
            }
        }
    }
}
