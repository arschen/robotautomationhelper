using System;
using System.Collections;
using System.Windows.Forms;

namespace RobotAutomationHelper.Scripts.CustomControls
{
    public class SuggestionsListObjects : ListControl
    {
        public string Documentation { get; set; }

        public override int SelectedIndex { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        protected override void RefreshItem(int index)
        {
            throw new NotImplementedException();
        }

        protected override void SetItemsCore(IList items)
        {
            throw new NotImplementedException();
        }
    }
}