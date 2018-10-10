using System.Collections;
using System.Windows.Forms;

namespace RobotAutomationHelper.Scripts
{
    internal class ComboBoxObject : ListControl
    {
        internal string Documentation { get; set; }

        public override int SelectedIndex { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        protected override void RefreshItem(int index)
        {
            throw new System.NotImplementedException();
        }

        protected override void SetItemsCore(IList items)
        {
            throw new System.NotImplementedException();
        }
    }
}