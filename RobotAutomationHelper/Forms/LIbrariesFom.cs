using System.Drawing;
using System.Windows.Forms;

namespace RobotAutomationHelper.Forms
{
    internal partial class LIbrariesFom : Form
    {
        private int LabelLength = 100;
        private int distanceBetweenLabelAndRadio = 10;
        private int FieldHeight = 20;
        private int verticalDistanceBetweenField = 10;

        internal LIbrariesFom()
        {
            InitializeComponent();
        }

        internal void ShowLibrariesContent()
        {

        }

        internal void ShowLibField(int x, int y, string Name, bool check)
        {
            Label libName = new Label
            {
                Name = Name,
                Text = Name,
                Location = new Point(x, y)
            };
            RadioButton radioBut = new RadioButton
            {
                Name = "radio" + Name,
                Location = new Point(x + LabelLength + distanceBetweenLabelAndRadio, y)
            };

        }
    }
}
