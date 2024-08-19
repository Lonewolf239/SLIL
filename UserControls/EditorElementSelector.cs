using System.Drawing;
using System.Windows.Forms;

namespace SLIL.UserControls
{
    public partial class EditorElementSelector : UserControl
    {
        public EditorElementSelector()
        {
            InitializeComponent();
        }

        public int Index { get; set; }
        public string ElementName { get; set; }
        public Color ElementColor { get; set; }

        private void EditorElementSelector_Load(object sender, System.EventArgs e)
        {
            select_btn.Name = "button_" + Index.ToString();
            element_name.Name = "name_" + Index.ToString();
            element_color.Name = "color_" + Index.ToString();
            element_name.Text = ElementName;
            element_color.BackColor = ElementColor;
        }
    }
}