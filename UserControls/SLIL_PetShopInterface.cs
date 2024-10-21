using System;
using System.Windows.Forms;
using Play_Sound;
using SLIL.Classes;

namespace SLIL.UserControls
{
    public partial class SLIL_PetShopInterface : UserControl
    {
        public SLIL_PetShopInterface() => InitializeComponent();

        public Pet pet;
        public int index = 0;
        public int width;
        public static PlaySound buy = new PlaySound(MainMenu.CGFReader.GetFile("buy.wav"), false);
        public PlaySound cant_pressed = new PlaySound(MainMenu.CGFReader.GetFile("cant_pressed.wav"), false);
        public Player player;
        private readonly string[,] buy_text = { { "2-0", "2-12" }, { "Buy", "Has already" } };

        private string GetBuyText()
        {
            if (index == 0)
                return MainMenu.Localizations.GetLString(MainMenu.Language, buy_text[0, player.PET == pet ? 1 : 0]);
            return buy_text[1, player.PET == pet ? 1 : 0];
        }

        private string GetPetName()
        {
            if (index == 0)
                return MainMenu.Localizations.GetLString(MainMenu.Language, pet.Name[0]);
            return pet.Name[1];
        }

        private string GetPetDescryption()
        {
            if (index == 0)
                return MainMenu.Localizations.GetLString(MainMenu.Language, pet.Description[0]);
            return pet.Description[1];
        }

        private void SLIL_PetShopInterface_VisibleChanged(object sender, EventArgs e)
        {
            Width = width;
            name.Text = GetPetName();
            icon.Image = SLIL.ShopImageDict[pet.GetType()];
            descryption.Text = GetPetDescryption();
            descryption.Width = Width - descryption.Left - 20;
            if (player.PET != pet)
                buy_button.Text = $"{GetBuyText()} ${pet.Cost}";
            else
                buy_button.Text = $"{GetBuyText()}";
        }

        private void Buy_button_Click(object sender, EventArgs e)
        {
            icon.Focus();
            if (player.PET != pet && player.Money >= pet.Cost)
            {
                if (MainMenu.sounds)
                    buy.Play(SLIL.Volume);
                (Parent.FindForm() as SLIL).AddPet(pet.Index);
                buy_button.Text = $"{GetBuyText()}";
            }
            else
            {
                if (MainMenu.sounds)
                    cant_pressed?.Play(SLIL.Volume);
            }
        }

        private void SLIL_PetShopInterface_SizeChanged(object sender, EventArgs e) => descryption.Width = Width - descryption.Left - 20;
    }
}