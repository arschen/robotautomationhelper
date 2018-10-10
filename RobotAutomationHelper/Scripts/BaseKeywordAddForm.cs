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
            if ((sender as ComboBox).SelectedIndex != -1)
            {
                int keywordIndex = int.Parse((sender as ComboBox).Name.Replace("Name", "").Replace("DynamicTestStep", ""));
                Keywords[keywordIndex - 1].SetKeywordName((sender as ComboBox).Items[(sender as ComboBox).SelectedIndex].ToString());
                FormControls.GetKeywordType(Keywords[keywordIndex - 1]);
                Console.WriteLine(FormControls.GetKeywordType(Keywords[keywordIndex - 1]));
            }
            else
            {
                ChangeTheKeywordFieldAfterKeyPress(sender, e, Keywords);
            }
        }

        // change the field when the keyword name is changed
        internal static void ChangeTheKeywordFieldAfterKeyPress(object sender, EventArgs e, List<Keyword> Keywords)
        {
            int keywordIndex = int.Parse((sender as ComboBox).Name.Replace("Name", "").Replace("DynamicTestStep", ""));
            Keywords[keywordIndex - 1].SetKeywordName((sender as ComboBox).Text);
            FormControls.GetKeywordType(Keywords[keywordIndex - 1]);
            Console.WriteLine(FormControls.GetKeywordType(Keywords[keywordIndex - 1]));
        }

        // handles key press for keyword name input, the case when return/enter is hit
        internal static void AutoCompleteComboBoxKeyPress(object sender, KeyEventArgs e, List<Keyword> Keywords)
        {
            var comboBox = sender as ComboBox;
            string txt = comboBox.Text;
            FormControls.selectionPointer = comboBox.SelectionStart;
            //Console.WriteLine(e.KeyCode);
            FormControls.keyEvent = e.KeyCode;
            if (FormControls.keyEvent == Keys.Return)
            {
                comboBox.DroppedDown = false;
                comboBox.Text = txt;
                comboBox.SelectionStart = FormControls.selectionPointer;
                ChangeTheKeywordFieldAfterKeyPress(sender, e, Keywords);
            }
        }
    }
}
