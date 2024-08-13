using System;
using System.Windows.Forms;
using Play_Sound;
using SLIL.Classes;

namespace SLIL.UserControls
{
    public partial class SLIL_ConsumablesShopInterface : UserControl
    {
        public SLIL_ConsumablesShopInterface()
        {
            InitializeComponent();
        }

        public Item item;
        public int index = 0;
        public int language = 0;
        public int width;
        public PlaySound buy;
        public PlaySound cant_pressed = new PlaySound(MainMenu.CGFReader.GetFile("cant_pressed.wav"), false);
        public Player player;
        public Gun[] GUNS;
        private readonly string[,] buy_text = { { "Купить", "Buy" }, { "Уже есть", "Has already" } };

        private void SLIL_ConsumablesShopInterface_VisibleChanged(object sender, EventArgs e)
        {
            language = index;
            if (item.HasCuteDescription && player.CuteMode)
                language += 2;
            Width = width;
            icon.Image = SLIL.IconDict[item.GetType()][0];
            descryption.Text = item.Description[language];
            name.Text = item.Name[language];
            if (!item.HasIt)
                buy_button.Text = $"{buy_text[0, index]} {item.GunCost}$";
            else
                buy_button.Text = $"{buy_text[1, index]}";
        }

        private void GetFirstAidKit()
        {
            if (player.FirstAidKits.Count == 0)
                player.FirstAidKits.Add((FirstAidKit)GUNS[10]);
            player.FirstAidKits[0].AmmoCount = player.FirstAidKits[0].CartridgesClip;
            player.FirstAidKits[0].MaxAmmoCount = player.FirstAidKits[0].CartridgesClip;
            player.FirstAidKits[0].HasIt = true;
        }

        private void Buy_button_Click(object sender, EventArgs e)
        {
            icon.Focus();
            if (player.Money >= item.GunCost && !item.HasIt)
            {
                if (MainMenu.sounds)
                    buy.Play(SLIL.Volume);
                player.ChangeMoney(-item.GunCost);
                buy_button.Text = $"{buy_text[1, index]}";
                if (item is FirstAidKit)
                    GetFirstAidKit();
            }
            else if (MainMenu.sounds)
                cant_pressed?.Play(SLIL.Volume);
        }
    }
}