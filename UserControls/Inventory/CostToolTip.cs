using System.Windows.Forms;

namespace SLIL.UserControls.Inventory
{
    internal partial class CostToolTip : UserControl
    {
        internal CostToolTip(int cost)
        {
            InitializeComponent();
            if (cost > 0) cost_label.Text = $"+{cost}$";
            else cost_label.Text = $"{cost}$";
        }
    }
}