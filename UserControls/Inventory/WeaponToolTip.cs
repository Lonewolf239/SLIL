using SLIL.Classes;
using System.Drawing;
using System.Windows.Forms;

namespace SLIL.UserControls.Inventory
{
    public partial class WeaponToolTip : UserControl
    {
        public Gun Weapon;

        public WeaponToolTip() => InitializeComponent();

        private int GetDamage()
        {
            double maxDamage = 40.0;
            double totalMinDamage = Weapon.MinDamage * Weapon.BulletCount;
            double damagePercentage = totalMinDamage / maxDamage;
            return (int)(damagePercentage * 336);
        }

        private void DrawWeaponParametrs()
        {
            Bitmap result = new Bitmap(parametrs_image.Width - 10, parametrs_image.Height - 10);
            using (Graphics g = Graphics.FromImage(result))
            {
                Brush progressbar_background = new SolidBrush(Color.FromArgb(77, 79, 86));
                Brush progressbar = new SolidBrush(Color.FromArgb(195, 195, 195));
                g.DrawImage(Properties.Resources.damage, 0, 0, 30, 30);
                g.DrawImage(Properties.Resources.firing_range, 0, 38, 30, 30);
                g.DrawImage(Properties.Resources.accuracy, 0, 76, 30, 30);
                g.FillRectangle(progressbar_background, 35, 0, 340, 30);
                g.FillRectangle(progressbar_background, 35, 38, 340, 30);
                g.FillRectangle(progressbar_background, 35, 76, 340, 30);
                g.FillRectangle(progressbar, 37, 2, GetDamage(), 26);
                g.FillRectangle(progressbar, 37, 40, (int)(Weapon.FiringRange / 30 * 336), 26);
                g.FillRectangle(progressbar, 37, 78, (int)((Weapon.Accuracy * 100) / 100 * 336), 26);
            }
            parametrs_image.Image = result;
        }

        private void WeaponToolTip_Load(object sender, System.EventArgs e) => DrawWeaponParametrs();
    }
}