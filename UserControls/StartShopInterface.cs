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
        private CostToolTip CostToolTip;
        private int CurrentMoney, CurrentAmmo = 0;
        private int _DowngradeCost = 0;
        private int DowngradeCost { get { if (Pistol.Level == Levels.LV1) return 0; return _DowngradeCost; } set => _DowngradeCost = value; }
        private Gun Pistol { get => Player.Guns[2]; }
        private FirstAidKit FirstAidKit { get => (FirstAidKit)Player.GUNS[10]; }
        private Helmet Helmet { get => (Helmet)Player.GUNS[14]; }
        private Adrenalin Adrenalin { get => (Adrenalin)Player.GUNS[13]; }
        private MedicalKit MedicalKit { get => (MedicalKit)Player.GUNS[17]; }

        internal StartShopInterface(Player player)
        {
            InitializeComponent();
            if (MainMenu.DownloadedLocalizationList)
            {
                start_shop_title.Text = MainMenu.Localizations.GetLString(MainMenu.Language, "2-24");
                pistol_label.Text = MainMenu.Localizations.GetLString(MainMenu.Language, "3-4");
                level_title.Text = MainMenu.Localizations.GetLString(MainMenu.Language, "2-26");
                items_title.Text = MainMenu.Localizations.GetLString(MainMenu.Language, "2-22");
                stop_start_shop_btn.Text = MainMenu.Localizations.GetLString(MainMenu.Language, "2-27");
            }
            else
            {
                start_shop_title.Text = "Starting purchase";
                pistol_label.Text = "Pistol";
                level_title.Text = "Level";
                items_title.Text = "Items";
                stop_start_shop_btn.Text = "READY";
            }
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
            DowngradeCost = Pistol.UpdateCost;
            Pistol.LevelUpdate();
            UpdateInfo();
        }

        private void Minus_level_btn_Click(object sender, EventArgs e)
        {
            start_shop_panel.Focus();
            if (!Pistol.CanDowngrade()) return;
            Pistol.LevelDowngrade();
            CurrentMoney += Pistol.UpdateCost;
            DowngradeCost = Pistol.UpdateCost;            
            UpdateInfo();
        }

        private void RemoveToolTip(Control toolTip)
        {
            if (start_shop_panel.Controls.Contains(toolTip))
                start_shop_panel.Controls.Remove(toolTip);
            toolTip?.Dispose();
        }

        private void Pistol_icon_MouseEnter(object sender, EventArgs e)
        {
            RemoveToolTip(WeaponToolTip);
            WeaponToolTip = new WeaponToolTip
            {
                Weapon = Pistol,
                Left = 0
            };
            WeaponToolTip.Top = start_shop_panel.Height - WeaponToolTip.Height;
            start_shop_panel.Controls.Add(WeaponToolTip);
            WeaponToolTip.BringToFront();
        }

        private void Pistol_icon_MouseLeave(object sender, EventArgs e) => RemoveToolTip(WeaponToolTip);

        private void Info_icon_MouseEnter(object sender, EventArgs e)
        {
            RemoveToolTip(InfoToolTip);
            string description;
            if (((Control)sender).Name == "medkit_icon")
            {
                if (MainMenu.DownloadedLocalizationList)
                    description = MainMenu.Localizations.GetLString(MainMenu.Language, FirstAidKit.Description[0]);
                else
                    description = FirstAidKit.Description[1];
            }
            else if (((Control)sender).Name == "helmet_icon")
            {
                if (MainMenu.DownloadedLocalizationList)
                    description = MainMenu.Localizations.GetLString(MainMenu.Language, Helmet.Description[0]);
                else
                    description = Helmet.Description[1];
            }
            else if (((Control)sender).Name == "adrenalin_icon")
            {
                if (MainMenu.DownloadedLocalizationList)
                    description = MainMenu.Localizations.GetLString(MainMenu.Language, Adrenalin.Description[0]);
                else
                    description = Adrenalin.Description[1];
            }
            else
            {
                if (MainMenu.DownloadedLocalizationList)
                    description = MainMenu.Localizations.GetLString(MainMenu.Language, MedicalKit.Description[0]);
                else
                    description = MedicalKit.Description[1];
            }
            InfoToolTip = new InfoToolTip(description) { Left = 0 };
            InfoToolTip.Top = start_shop_panel.Height - InfoToolTip.Height;
            start_shop_panel.Controls.Add(InfoToolTip);
            InfoToolTip.BringToFront();
        }

        private void Info_icon_MouseLeave(object sender, EventArgs e) => RemoveToolTip(InfoToolTip);

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
            if (MainMenu.DownloadedLocalizationList) money_count.Text = $"{MainMenu.Localizations.GetLString(MainMenu.Language, "2-28")} {CurrentMoney}$";
            else money_count.Text = $"Money {CurrentMoney}$";
            pistol_ammo_count.Text = $"{Pistol.AmmoInStock + (Pistol.CartridgesClip * CurrentAmmo)}/{Pistol.AmmoCount}";
            pistol_icon.Image = SLIL.IconDict[Pistol.GetType()][Pistol.GetLevel()];
            medkit_count.Text = $"{Player.DisposableItems[0].MaxCount}/{Player.DisposableItems[0].Count}";
            adrenalin_count.Text = $"{Player.DisposableItems[1].MaxCount}/{Player.DisposableItems[1].Count}";
            helmet_count.Text = $"{Player.DisposableItems[2].MaxCount}/{Player.DisposableItems[2].Count}";
            medical_kit_count.Text = $"{Player.DisposableItems[3].MaxCount}/{Player.DisposableItems[3].Count}";
        }

        private void Plus_btn_MouseLeave(object sender, EventArgs e) => RemoveToolTip(CostToolTip);

        private void Plus_btn_MouseEnter(object sender, EventArgs e)
        {
            RemoveToolTip(CostToolTip);
            int cost;
            switch (((Control)sender).Name)
            {
                case "plus_ammo_btn": cost = -Pistol.AmmoCost; break;
                case "minus_ammo_btn": cost = Pistol.AmmoCost; break;
                case "plus_level_btn": cost = -Pistol.UpdateCost; break;
                case "minus_level_btn": cost = DowngradeCost; break;
                case "medkit_plus_btn": cost = -FirstAidKit.GunCost; break;
                case "medkit_minus_btn": cost = FirstAidKit.GunCost; break;
                case "adrenalin_plus_btn": cost = -Adrenalin.GunCost; break;
                case "adrenalin_minus_btn": cost = Adrenalin.GunCost; break;
                case "helmet_plus_btn": cost = -Helmet.GunCost; break;
                case "helmet_minus_btn": cost = Helmet.GunCost; break;
                case "medical_kit_plus_btn": cost = -MedicalKit.GunCost; break;
                default: cost = MedicalKit.GunCost; break;
            }
            CostToolTip = new CostToolTip(cost) { Left = 0 };
            CostToolTip.Top = start_shop_panel.Height - CostToolTip.Height;
            start_shop_panel.Controls.Add(CostToolTip);
            CostToolTip.BringToFront();
        }

        private void Stop_start_shop_btn_Click(object sender, EventArgs e)
        {
            start_shop_panel.Focus();
            Player.Money = CurrentMoney;
            Pistol.AmmoInStock += CurrentAmmo * Pistol.CartridgesClip;
        }
    }
}