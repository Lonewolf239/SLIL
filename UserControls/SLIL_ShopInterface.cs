using System;
using System.Drawing;
using System.Windows.Forms;
using Play_Sound;
using SLIL.Classes;

namespace SLIL.UserControls
{
    internal partial class SLIL_ShopInterface : UserControl
    {
        internal Timer UpdateTimer;
        internal SLIL ParentSLILForm;
        internal Gun weapon;
        internal int index = 0;
        internal static PlaySound buy = new PlaySound(MainMenu.CGFReader.GetFile("buy.wav"), false);
        internal PlaySound cant_pressed = new PlaySound(MainMenu.CGFReader.GetFile("cant_pressed.wav"), false);
        internal Player player;
        private readonly string[,] buy_text = { { "2-8", "2-9" }, { "Buy weapons", "Buy ammo" } };

        internal SLIL_ShopInterface()
        {
            InitializeComponent();
            Cursor = Program.SLILCursorDefault;
            UpdateTimer = new Timer { Interval = 1000 };
            UpdateTimer.Tick += UpdateTimer_Tick;
            UpdateTimer.Enabled = true;
        }

        ~SLIL_ShopInterface() => UpdateTimer.Stop();

        private string GetBuyText()
        {
            if (index == 0)
                return MainMenu.Localizations.GetLString(MainMenu.Language, buy_text[0, weapon.HasIt ? 1 : 0]);
            return buy_text[1, weapon.HasIt ? 1 : 0];
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

        private void Buy_button_Click(object sender, EventArgs e)
        {
            weapon_icon.Focus();
            if (weapon.HasIt)
            {
                if (player.Money >= weapon.AmmoCost && weapon.AmmoInStock + weapon.AmmoCount <= weapon.MaxAmmo)
                {
                    if (MainMenu.sounds) buy.Play(SLIL.EffectsVolume);
                    ParentSLILForm.BuyAmmo(weapon);
                    ammo_count.Text = index == 0 ? $"{MainMenu.Localizations.GetLString(MainMenu.Language, "2-10")} {weapon.AmmoInStock}/{weapon.AmmoCount}" : $"Ammo: {weapon.AmmoInStock}/{weapon.AmmoCount}";
                }
                else if (MainMenu.sounds)
                    cant_pressed?.Play(SLIL.EffectsVolume);
            }
            else
            {
                if (player.Money >= weapon.GunCost)
                {
                    if (MainMenu.sounds) buy.Play(SLIL.EffectsVolume);
                    ParentSLILForm.BuyWeapon(weapon);
                    buy_button.Text = GetBuyText() + $" ${weapon.AmmoCost}";
                    ammo_count.Text = index == 0 ? $"{MainMenu.Localizations.GetLString(MainMenu.Language, "2-10")} {weapon.AmmoInStock}/{weapon.AmmoCount}" : $"Ammo: {weapon.AmmoInStock}/{weapon.AmmoCount}";
                    update_button.Visible = weapon.CanUpdate();
                    if (weapon.CanUpdate())
                    {
                        update_button.Left = buy_button.Right + 6;
                        ammo_panel.Left = update_button.Right + 12;
                    }
                    else ammo_panel.Left = buy_button.Right + 12;
                }
                else if (MainMenu.sounds)
                    cant_pressed?.Play(SLIL.EffectsVolume);
            }
            UpdateInfo();
            ParentSLILForm.UpdateStorage();
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

        private void Update_button_Click(object sender, EventArgs e)
        {
            weapon_icon.Focus();
            if (player.Money >= weapon.UpdateCost)
            {
                if (MainMenu.sounds) buy.Play(SLIL.EffectsVolume);
                ParentSLILForm.UpdateWeapon(weapon);
                weapon_name.Text = GetWeaponName() + $" {weapon.Level}";
                weapon_icon.Image = SLIL.IconDict[weapon.GetType()][weapon.GetLevel()];
                update_button.Text = $"${weapon.UpdateCost}";
                ammo_count.Text = $"{weapon.AmmoInStock}/{weapon.AmmoCount}";
                parametrs_image.Image = DrawWeaponParametrs();
                update_button.Visible = weapon.CanUpdate();
            }
            else if (MainMenu.sounds)
                cant_pressed?.Play(SLIL.EffectsVolume);
            UpdateInfo();
            ParentSLILForm.UpdateStorage();
        }

        private void UpdateInfo()
        {
            int cost = weapon.HasIt ? weapon.AmmoCost : weapon.GunCost;
            string ammo = weapon.HasIt ? $"{weapon.AmmoInStock}/{weapon.AmmoCount}" : "0/0";
            weapon_name.Text = weapon.Upgradeable ? GetWeaponName() + $" {weapon.Level}" : GetWeaponName();
            if (weapon_icon.Image != SLIL.IconDict[weapon.GetType()][weapon.GetLevel()])
                weapon_icon.Image = SLIL.IconDict[weapon.GetType()][weapon.GetLevel()];
            ammo_count.Text = ammo;
            ammo_icon.Image = GetAmmoIcon();
            buy_button.Text = GetBuyText() + $" ${cost}";
            update_button.Text = $"${weapon.UpdateCost}";
            if (weapon.CanUpdate() && weapon.HasIt)
            {
                update_button.Left = buy_button.Right + 6;
                ammo_panel.Left = update_button.Right + 12;
            }
            else ammo_panel.Left = buy_button.Right + 12;
            parametrs_image.Image = DrawWeaponParametrs();
            update_button.Visible = weapon.CanUpdate() && weapon.HasIt;
        }

        private void SLIL_ShopInterface_VisibleChanged(object sender, EventArgs e) => UpdateInfo();
    }
}