using System;
using System.Windows.Forms;
using Play_Sound;
using SLIL.Classes;

namespace SLIL.UserControls
{
    internal partial class SLIL_ConsumablesShopInterface : UserControl
    {
        internal DisposableItem item;
        internal int index = 0;
        internal static PlaySound buy = new PlaySound(MainMenu.CGFReader.GetFile("buy.wav"), false);
        internal PlaySound cant_pressed = new PlaySound(MainMenu.CGFReader.GetFile("cant_pressed.wav"), false);
        internal Player player;
        private readonly string[,] buy_text = { { "2-0", "2-15" }, { "Buy", "Maximum" } };

        internal SLIL_ConsumablesShopInterface()
        {
            InitializeComponent();
            Cursor = Program.SLILCursorDefault;
        }

        private string GetBuyText()
        {
            if (index == 0)
                return MainMenu.Localizations.GetLString(MainMenu.Language, buy_text[0, !item.CanBuy() ? 1 : 0]);
            return buy_text[1, !item.CanBuy() ? 1 : 0];
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
            icon.Image = SLIL.IconDict[item.GetType()][player.CuteMode ? 1 : 0];
            descryption.Text = GetItemDescription();
            name.Text = GetItemName();
            if (item.CanBuy())
                buy_button.Text = $"{GetBuyText()} {item.GunCost}$";
            else
                buy_button.Text = $"{GetBuyText()}";
        }

        private void Buy_button_Click(object sender, EventArgs e)
        {
            icon.Focus();
            if (player.Money >= item.GunCost && item.CanBuy())
            {
                if (MainMenu.sounds) buy.Play(SLIL.EffectsVolume);
                (Parent.FindForm() as SLIL).BuyConsumable(item);
                if (item.Count == item.MaxCount)
                    buy_button.Text = $"{GetBuyText()}";
            }
            else if (MainMenu.sounds)
                cant_pressed?.Play(SLIL.EffectsVolume);
        }
    }
}