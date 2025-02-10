using System.Windows.Forms;

namespace SLIL.UserControls.Inventory
{
    public partial class InfoToolTip : UserControl
    {
        public InfoToolTip(string description)
        {
            InitializeComponent();
            Cursor = Program.SLILCursorDefault;
            description_label.Text = description;
        }
    }
}