using System;
using SLIL.Classes;
using System.Drawing;
using System.Windows.Forms;
using SLIL.UserControls.Inventory;

namespace SLIL.UserControls
{
    internal partial class StartShopInterface : UserControl
    {
        internal Player Player;
        private WeaponToolTip WeaponToolTip;
        private InfoToolTip InfoToolTip;
        private int CurrentMoney, CurrentAmmo = 0;
        private Gun Pistol { get => Player.Guns[2]; }

        internal StartShopInterface(Player player)
        {
            InitializeComponent();
            Player = player;
            CurrentMoney = player.Money;
        }

        private void StartShopInterface_Load(object sender, EventArgs e)
        {
            start_shop_panel.Location = new Point((Width - start_shop_panel.Width) / 2, (Height - start_shop_panel.Height) / 2);
            UpdateInfo();
        }

        private void Plus_ammo_btn_Click(object sender, EventArgs e)
        {
            start_shop_panel.Focus();
            if (CurrentMoney - Pistol.AmmoCost < 0) return;
            if (Pistol.AmmoInStock + (Pistol.CartridgesClip * CurrentAmmo) < Pistol.MaxAmmo)
            {
                CurrentAmmo++;
                CurrentMoney -= Pistol.AmmoCost;
            }
            UpdateInfo();
        }

        private void Minus_ammo_btn_Click(object sender, EventArgs e)
        {
            start_shop_panel.Focus();
            if (CurrentAmmo <= 0) return;
            CurrentAmmo--;
            CurrentMoney += Pistol.AmmoCost;
            UpdateInfo();
        }

        private void Plus_level_btn_Click(object sender, EventArgs e)
        {
            start_shop_panel.Focus();
            if (CurrentMoney - Pistol.UpdateCost < 0) return;
            if (!Pistol.CanUpdate()) return;
            CurrentMoney -= Pistol.UpdateCost;
            Pistol.LevelUpdate();
            UpdateInfo();
        }

        private void Minus_level_btn_Click(object sender, EventArgs e)
        {
            start_shop_panel.Focus();
            if (!Pistol.CanDowngrade()) return;
            Pistol.LevelDowngrade();
            CurrentMoney += Pistol.UpdateCost;
            UpdateInfo();
        }

        private void Pistol_icon_MouseEnter(object sender, EventArgs e)
        {
            if (start_shop_panel.Controls.Contains(WeaponToolTip))
                start_shop_panel.Controls.Remove(WeaponToolTip);
            WeaponToolTip?.Dispose();
            WeaponToolTip = new WeaponToolTip
            {
                Weapon = Pistol,
                Left = 0
            };
            WeaponToolTip.Top = start_shop_panel.Height - WeaponToolTip.Height;
            start_shop_panel.Controls.Add(WeaponToolTip);
            WeaponToolTip.BringToFront();
        }

        private void Pistol_icon_MouseLeave(object sender, EventArgs e)
        {
            if (start_shop_panel.Controls.Contains(WeaponToolTip))
                start_shop_panel.Controls.Remove(WeaponToolTip);
            WeaponToolTip?.Dispose();
        }

        private void Info_icon_MouseEnter(object sender, EventArgs e)
        {
            if (start_shop_panel.Controls.Contains(InfoToolTip))
                start_shop_panel.Controls.Remove(InfoToolTip);
            InfoToolTip?.Dispose();
            string description;
            if (((Control)sender).Name == "medkit_icon")
            {
                if (MainMenu.DownloadedLocalizationList)
                    description = MainMenu.Localizations.GetLString(MainMenu.Language, ((FirstAidKit)Player.GUNS[10]).Description[0]);
                else
                    description = ((FirstAidKit)Player.GUNS[10]).Description[1];
            }
            else if (((Control)sender).Name == "helmet_icon")
            {
                if (MainMenu.DownloadedLocalizationList)
                    description = MainMenu.Localizations.GetLString(MainMenu.Language, ((Helmet)Player.GUNS[14]).Description[0]);
                else
                    description = ((Helmet)Player.GUNS[14]).Description[1];
            }
            else if (((Control)sender).Name == "adrenalin_icon")
            {
                if (MainMenu.DownloadedLocalizationList)
                    description = MainMenu.Localizations.GetLString(MainMenu.Language, ((Adrenalin)Player.GUNS[13]).Description[0]);
                else
                    description = ((Adrenalin)Player.GUNS[13]).Description[1];
            }
            else
            {
                if (MainMenu.DownloadedLocalizationList)
                    description = MainMenu.Localizations.GetLString(MainMenu.Language, ((MedicalKit)Player.GUNS[17]).Description[0]);
                else
                    description = ((MedicalKit)Player.GUNS[17]).Description[1];
            }
            InfoToolTip = new InfoToolTip(description) { Left = 0 };
            InfoToolTip.Top = start_shop_panel.Height - InfoToolTip.Height;
            start_shop_panel.Controls.Add(InfoToolTip);
            InfoToolTip.BringToFront();
        }

        private void Info_icon_MouseLeave(object sender, EventArgs e)
        {
            if (start_shop_panel.Controls.Contains(InfoToolTip))
                start_shop_panel.Controls.Remove(InfoToolTip);
            InfoToolTip?.Dispose();
        }

        private void Medkit_plus_btn_Click(object sender, EventArgs e) => BuyItem(0);

        private void Medkit_minus_btn_Click(object sender, EventArgs e) => SellItem(0);

        private void Adrenalin_plus_btn_Click(object sender, EventArgs e) => BuyItem(1);

        private void Adrenalin_minus_btn_Click(object sender, EventArgs e) => SellItem(1);

        private void Helmet_plus_btn_Click(object sender, EventArgs e) => BuyItem(2);

        private void Helmet_minus_btn_Click(object sender, EventArgs e) => SellItem(2);

        private void Medical_kit_plus_btn_Click(object sender, EventArgs e) => BuyItem(3);

        private void Medical_kit_minus_btn_Click(object sender, EventArgs e) => SellItem(3);

        private void BuyItem(int index)
        {
            start_shop_panel.Focus();
            if (CurrentMoney - Player.DisposableItems[index].GunCost < 0) return;
            if (!Player.DisposableItems[index].CanBuy()) return;
            CurrentMoney -= Player.DisposableItems[index].GunCost;
            Player.DisposableItems[index].AddItem();
            UpdateInfo();
        }

        private void SellItem(int index)
        {
            start_shop_panel.Focus();
            if (Player.DisposableItems[index].Count == 0) return;
            Player.DisposableItems[index].RemoveItem();
            CurrentMoney += Player.DisposableItems[index].GunCost;
            UpdateInfo();
        }

        private void UpdateInfo()
        {
            while (Pistol.AmmoInStock + (Pistol.CartridgesClip * CurrentAmmo) > Pistol.MaxAmmo)
            {
                CurrentMoney += Pistol.AmmoCost;
                CurrentAmmo--;
            }
            money_count.Text = $"Деньги {CurrentMoney}$";
            pistol_ammo_count.Text = $"{Pistol.AmmoInStock + (Pistol.CartridgesClip * CurrentAmmo)}/{Pistol.AmmoCount}";
            pistol_icon.Image = SLIL.IconDict[Pistol.GetType()][Pistol.GetLevel()];
            medkit_count.Text = $"{Player.DisposableItems[0].MaxCount}/{Player.DisposableItems[0].Count}";
            adrenalin_count.Text = $"{Player.DisposableItems[1].MaxCount}/{Player.DisposableItems[1].Count}";
            helmet_count.Text = $"{Player.DisposableItems[2].MaxCount}/{Player.DisposableItems[2].Count}";
            medical_kit_count.Text = $"{Player.DisposableItems[3].MaxCount}/{Player.DisposableItems[3].Count}";
        }

        private void Stop_start_shop_btn_Click(object sender, EventArgs e)
        {
            start_shop_panel.Focus();
            Player.Money = CurrentMoney;
            Pistol.AmmoInStock += CurrentAmmo * Pistol.CartridgesClip;
        }
    }
}