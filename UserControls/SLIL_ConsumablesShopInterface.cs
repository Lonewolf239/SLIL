using System;
using System.Linq;
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

        public DisposableItem item;
        public int index = 0;
        public int language = 0;
        public int width;
        public static PlaySound buy = new PlaySound(MainMenu.CGFReader.GetFile("buy.wav"), false);
        public PlaySound cant_pressed = new PlaySound(MainMenu.CGFReader.GetFile("cant_pressed.wav"), false);
        public Player player;
        private readonly string[,] buy_text = { { "Купить", "Buy" }, { "Уже есть", "Has already" } };

        private void SLIL_ConsumablesShopInterface_VisibleChanged(object sender, EventArgs e)
        {
            language = index;
            if (item.HasCuteDescription && player.CuteMode)
                language += 2;
            Width = width;
            icon.Image = SLIL.IconDict[item.GetType()][player.CuteMode ? 1 : 0];
            descryption.Text = item.Description[language];
            name.Text = item.Name[language];
            if (!item.HasIt)
                buy_button.Text = $"{buy_text[0, index]} {item.GunCost}$";
            else
                buy_button.Text = $"{buy_text[1, index]}";
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
                item.AddItem();
            }
            else if (MainMenu.sounds)
                cant_pressed?.Play(SLIL.Volume);
        }
    }
}