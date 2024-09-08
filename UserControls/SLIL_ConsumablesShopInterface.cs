using System;
using System.Windows.Forms;
using Play_Sound;
using SLIL.Classes;

namespace SLIL.UserControls
{
    public partial class SLIL_ConsumablesShopInterface : UserControl
    {
        public DisposableItem item;
        public int index = 0;
        public int width;
        public static PlaySound buy = new PlaySound(MainMenu.CGFReader.GetFile("buy.wav"), false);
        public PlaySound cant_pressed = new PlaySound(MainMenu.CGFReader.GetFile("cant_pressed.wav"), false);
        public Player player;
        private readonly string[,] buy_text = { { "2-0", "2-12" }, { "Buy", "Has already" } };

        public SLIL_ConsumablesShopInterface() => InitializeComponent();

        private string GetBuyText()
        {
            if (index == 0)
                return MainMenu.Localizations.GetLString(MainMenu.Language, buy_text[0, item.HasIt ? 1 : 0]);
            return buy_text[1, item.HasIt ? 1 : 0];
        }

        private string GetItemName()
        {
            int i = item.HasCuteDescription && player.CuteMode ? index + 2 : index;
            if (index == 0)
                return MainMenu.Localizations.GetLString(MainMenu.Language, item.Name[i]);
            return item.Name[i];
        }

        private string GetItemDescription()
        {
            int i = item.HasCuteDescription && player.CuteMode ? index + 2 : index;
            if (index == 0)
                return MainMenu.Localizations.GetLString(MainMenu.Language, item.Description[i]);
            return item.Description[i];
        }

        private void SLIL_ConsumablesShopInterface_VisibleChanged(object sender, EventArgs e)
        {
            Width = width;
            icon.Image = SLIL.IconDict[item.GetType()][player.CuteMode ? 1 : 0];
            descryption.Text = GetItemDescription();
            name.Text = GetItemName();
            if (!item.HasIt)
                buy_button.Text = $"{GetBuyText()} {item.GunCost}$";
            else
                buy_button.Text = $"{GetBuyText()}";
        }

        private void Buy_button_Click(object sender, EventArgs e)
        {
            icon.Focus();
            if (player.Money >= item.GunCost && !item.HasIt)
            {
                if (MainMenu.sounds)
                    buy.Play(SLIL.Volume);
                (Parent.FindForm() as SLIL).BuyConsumable(item);
                buy_button.Text = $"{GetBuyText()}";
            }
            else if (MainMenu.sounds)
                cant_pressed?.Play(SLIL.Volume);
        }
    }
}