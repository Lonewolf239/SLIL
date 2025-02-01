using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Play_Sound;
using SLIL.Classes;

namespace SLIL.UserControls
{
    public partial class SLIL_StorageInterface : UserControl
    {
        public Timer UpdateTimer;
        public SLIL ParentSLILForm;
        public Gun weapon;
        public int index = 0;
        public int width;
        public PlaySound cant_pressed = new PlaySound(MainMenu.CGFReader.GetFile("cant_pressed.wav"), false);
        public Player player;
        private readonly string[,] slot_text = { { "2-17", "2-18", "2-19" }, { "Slot ", "Instead ", "Equipped" }  };

        public SLIL_StorageInterface()
        {
            InitializeComponent();
            Cursor = Program.SLILCursor;
            UpdateTimer = new Timer { Interval = 1000 };
            UpdateTimer.Tick += UpdateTimer_Tick;
            UpdateTimer.Enabled = true;
        }

        ~SLIL_StorageInterface() => UpdateTimer.Stop();

        private string GetButtonText(int i)
        {
            try
            {
                if (i == 1 && player.WeaponSlot_0 != -1)
                {
                    if (player.WeaponSlot_0 == player.Guns.IndexOf(weapon))
                    {
                        if (index == 0)
                            return MainMenu.Localizations.GetLString(MainMenu.Language, slot_text[0, 2]);
                        return slot_text[1, 2];
                    }
                    if (index == 0)
                        return MainMenu.Localizations.GetLString(MainMenu.Language, slot_text[0, 1]) +
                            MainMenu.Localizations.GetLString(MainMenu.Language, player.Guns[player.WeaponSlot_0].Name[0]);
                    return slot_text[1, 1] + player.Guns[player.WeaponSlot_0].Name[1];
                }
                if (i == 2 && player.WeaponSlot_1 != -1)
                {
                    if (player.WeaponSlot_1 == player.Guns.IndexOf(weapon))
                    {
                        if (index == 0)
                            return MainMenu.Localizations.GetLString(MainMenu.Language, slot_text[0, 2]);
                        return slot_text[1, 2];
                    }
                    if (index == 0)
                        return MainMenu.Localizations.GetLString(MainMenu.Language, slot_text[0, 1]) +
                            MainMenu.Localizations.GetLString(MainMenu.Language, player.Guns[player.WeaponSlot_1].Name[0]);
                    return slot_text[1, 1] + player.Guns[player.WeaponSlot_1].Name[1];
                }
                if (index == 0)
                    return MainMenu.Localizations.GetLString(MainMenu.Language, slot_text[0, 0]) + i.ToString();
                return slot_text[1, 0] + i.ToString();
            }
            catch { return "None"; }
        }

        private string GetWeaponName()
        {
            if (index == 0)
                return MainMenu.Localizations.GetLString(MainMenu.Language, weapon.Name[0]);
            return weapon.Name[1];
        }

        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            player = ParentSLILForm.GetPlayer();
            if (player == null)
            {
                this.UpdateTimer.Stop();
                return;
            }
            foreach (Gun gun in player.Guns)
            {
                if (gun.GetType() == weapon.GetType())
                {
                    weapon = gun;
                    break;
                }
            }
            UpdateInfo();
        }

        private Image GetAmmoIcon()
        {
            switch (weapon.AmmoType)
            {
                case AmmoTypes.Magic: return Properties.Resources.magic;
                case AmmoTypes.Bubbles: return Properties.Resources.bubbles;
                case AmmoTypes.Bullet: return Properties.Resources.bullet;
                case AmmoTypes.Shell: return Properties.Resources.shell;
                case AmmoTypes.Rifle: return Properties.Resources.rifle_bullet;
                case AmmoTypes.Rocket: return Properties.Resources.rocket;
                case AmmoTypes.C4: return Properties.Resources.c4;
                default: return Properties.Resources.bullet;
            }
        }

        private int GetDamage()
        {
            double maxDamage = 40.0;
            double totalMinDamage = weapon.MinDamage * weapon.BulletCount;
            double damagePercentage = totalMinDamage / maxDamage;
            return (int)(damagePercentage * 336);
        }

        private Image DrawWeaponParametrs()
        {
            Bitmap result = new Bitmap(parametrs_image.Width, parametrs_image.Height);
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
                g.FillRectangle(progressbar, 37, 40, (int)(weapon.FiringRange / 30 * 336), 26);
                g.FillRectangle(progressbar, 37, 78, (int)((weapon.Accuracy * 100) / 100 * 336), 26);
            }
            return result;
        }

        private void UpdateInfo()
        {
            string ammo = weapon.InfinityAmmo ? "∞" : $"{weapon.AmmoInStock}/{weapon.AmmoCount}";
            weapon_name.Text = weapon.Upgradeable ? GetWeaponName() + $" {weapon.Level}" : GetWeaponName();
            if (weapon_icon.Image != SLIL.IconDict[weapon.GetType()][weapon.GetLevel()])
                weapon_icon.Image = SLIL.IconDict[weapon.GetType()][weapon.GetLevel()];
            ammo_count.Text = ammo;
            ammo_icon.Image = GetAmmoIcon();
            slot_0_btn.Text = GetButtonText(1);
            slot_1_btn.Text = GetButtonText(2);
            slot_1_btn.Left = slot_0_btn.Right + 6;
            ammo_panel.Left = slot_1_btn.Right + 12;
            parametrs_image.Image = DrawWeaponParametrs();
        }

        private void SLIL_ShopInterface_VisibleChanged(object sender, EventArgs e)
        {
            UpdateInfo();
            Width = width;
        }

        private int GetGunIndex()
        {
            for (int i = 0; i < player.Guns.Count; i++)
            {
                if (player.Guns[i].GetType() == weapon.GetType())
                    return i;
            }
            return -1;
        }

        private void Slot_0_btn_Click(object sender, EventArgs e) => ParentSLILForm.SetWeaponSlot(0, GetGunIndex());

        private void Slot_1_btn_Click(object sender, EventArgs e) => ParentSLILForm.SetWeaponSlot(1, GetGunIndex());
    }
}