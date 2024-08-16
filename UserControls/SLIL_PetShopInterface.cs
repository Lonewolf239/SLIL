using System;
using System.Windows.Forms;
using Play_Sound;
using SLIL.Classes;

namespace SLIL.UserControls
{
    public partial class SLIL_PetShopInterface : UserControl
    {
        public SLIL_PetShopInterface()
        {
            InitializeComponent();
        }

        public Pet pet;
        public int index = 0;
        public int width;
        public static PlaySound buy = new PlaySound(MainMenu.CGFReader.GetFile("buy.wav"), false);
        public PlaySound cant_pressed = new PlaySound(MainMenu.CGFReader.GetFile("cant_pressed.wav"), false);
        public Player player;
        private readonly string[,] buy_text = { { "Купить", "Buy" }, { "Уже есть", "Has already" } };

        private void SLIL_PetShopInterface_VisibleChanged(object sender, EventArgs e)
        {
            Width = width;
            name.Text = pet.Name[index];
            icon.Image = SLIL.ShopImageDict[pet.GetType()];
            descryption.Text = pet.Descryption[index];
            descryption.Width = Width - descryption.Left - 20;
            if (player.PET != pet)
                buy_button.Text = $"{buy_text[0, index]} ${pet.Cost}";
            else
                buy_button.Text = $"{buy_text[1, index]}";
        }

        private void Buy_button_Click(object sender, EventArgs e)
        {
            icon.Focus();
            if (player.PET != pet && player.Money >= pet.Cost)
            {
                if (MainMenu.sounds)
                    buy.Play(SLIL.Volume);
                player.ChangeMoney(-pet.Cost);
                (Parent.FindForm() as SLIL).AddPet(pet.Index);
                buy_button.Text = $"{buy_text[1, index]}";
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