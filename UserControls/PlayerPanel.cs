using System.Windows.Forms;

namespace SLIL.UserControls
{
    public partial class PlayerPanel : UserControl
    {
        public PlayerPanel()
        {
            InitializeComponent();
            Cursor = Program.SLILCursorDefault;
        }
    }
}