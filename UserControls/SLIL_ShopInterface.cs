using System;
using System.Windows.Forms;
using Play_Sound;
using SLIL.Classes;

namespace SLIL.UserControls
{
    public partial class SLIL_ShopInterface : UserControl
    {
        public Timer UpdateTimer;
        public SLIL ParentSLILForm;
        public Gun weapon;
        public int index = 0;
        public int width;
        public static PlaySound buy = new PlaySound(MainMenu.CGFReader.GetFile("buy.wav"), false);
        public PlaySound cant_pressed = new PlaySound(MainMenu.CGFReader.GetFile("cant_pressed.wav"), false);
        public Player player;
        private readonly string[,] buy_text = { { "Купить оружие", "Купить патроны" }, { "Buy weapons", "Buy ammo" } };

        public SLIL_ShopInterface()
        {
            InitializeComponent();
            UpdateTimer = new Timer
            {
                Interval = 1000
            };
            UpdateTimer.Tick += UpdateTimer_Tick;
            UpdateTimer.Enabled = true;
        }

        ~SLIL_ShopInterface() => UpdateTimer.Stop();

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
                    if (MainMenu.sounds)
                        buy.Play(SLIL.Volume);
                    ParentSLILForm.BuyAmmo(weapon);
                    ammo_count.Text = index == 0 ? $"Патроны: {weapon.AmmoInStock}/{weapon.AmmoCount}" : $"Ammo: {weapon.AmmoInStock}/{weapon.AmmoCount}";
                }
                else if (MainMenu.sounds)
                    cant_pressed?.Play(SLIL.Volume);
            }
            else
            {
                if (player.Money  >= weapon.GunCost)
                {
                    if (MainMenu.sounds)
                        buy.Play(SLIL.Volume);
                    ParentSLILForm.BuyWeapon(weapon);
                    buy_button.Text = buy_text[index, weapon.HasIt ? 1 : 0] + $" ${weapon.AmmoCost}";
                    ammo_count.Text = index == 0 ? $"Патроны: {weapon.AmmoInStock}/{weapon.AmmoCount}" : $"Ammo: {weapon.AmmoInStock}/{weapon.AmmoCount}";
                    update_button.Left = buy_button.Right + 6;
                    update_button.Visible = weapon.CanUpdate();
                }
                else if (MainMenu.sounds)
                    cant_pressed?.Play(SLIL.Volume);
            }
        }

        private void Update_button_Click(object sender, EventArgs e)
        {
            weapon_icon.Focus();
            if (player.Money  >= weapon.UpdateCost)
            {
                if (MainMenu.sounds)
                    buy.Play(SLIL.Volume);
                ParentSLILForm.UpdateWeapon(weapon);
                weapon_name.Text = weapon.Name[index] + $" {weapon.Level}";
                weapon_icon.Image = SLIL.IconDict[weapon.GetType()][weapon.GetLevel()];
                update_button.Text = $"${weapon.UpdateCost}";
                damage_text.Text = index == 0 ? $"Урон: {weapon.MinDamage}-{weapon.MaxDamage}" : $"Damage: {weapon.MinDamage}-{weapon.MaxDamage}";
                ammo_count.Text = index == 0 ? $"Патроны: {weapon.AmmoInStock}/{weapon.AmmoCount}" : $"Ammo: {weapon.AmmoInStock}/{weapon.AmmoCount}";
                ammo_count.Left = damage_text.Right;
                update_button.Visible = weapon.CanUpdate();
            }
            else if (MainMenu.sounds)
                cant_pressed?.Play(SLIL.Volume);
        }

        private void UpdateInfo()
        {
            int cost = weapon.HasIt ? weapon.AmmoCost : weapon.GunCost;
            string ammo = weapon.HasIt ? $"{weapon.AmmoInStock}/{weapon.AmmoCount}" : "0/0";
            weapon_name.Text = weapon.Upgradeable ? weapon.Name[index] + $" {weapon.Level}" : weapon.Name[index];
            if (weapon_icon.Image != SLIL.IconDict[weapon.GetType()][weapon.GetLevel()])
                weapon_icon.Image = SLIL.IconDict[weapon.GetType()][weapon.GetLevel()];
            ammo_count.Text = index == 0 ? $"Патроны: {ammo}" : $"Ammo: {ammo}";
            buy_button.Text = buy_text[index, weapon.HasIt ? 1 : 0] + $" ${cost}";
            update_button.Text = $"${weapon.UpdateCost}";
            damage_text.Text = index == 0 ? $"Урон: {weapon.MinDamage}-{weapon.MaxDamage}" : $"Damage: {weapon.MinDamage}-{weapon.MaxDamage}";
            ammo_count.Left = damage_text.Right;
            update_button.Left = buy_button.Right + 6;
            update_button.Visible = weapon.CanUpdate() && weapon.HasIt;
        }

        private void SLIL_ShopInterface_VisibleChanged(object sender, EventArgs e)
        {
            UpdateInfo();
            Width = width;
        }
    }
}