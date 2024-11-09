using System;
using System.Drawing;
using System.Windows.Forms;
using Play_Sound;
using SLIL.Classes;

namespace SLIL.UserControls
{
    public partial class SLIL_TransportStoreInterface : UserControl
    {
        public Transport transport;
        public int index = 0;
        public int width;
        public static PlaySound buy = new PlaySound(MainMenu.CGFReader.GetFile("buy.wav"), false);
        public PlaySound cant_pressed = new PlaySound(MainMenu.CGFReader.GetFile("cant_pressed.wav"), false);
        public Player player;
        private readonly string[] buy_text = { "2-0", "Buy" };

        public SLIL_TransportStoreInterface()
        {
            InitializeComponent();
            Cursor = Program.SLILCursor;
        }

        private string GetBuyText()
        {
            if (index == 0)
                return MainMenu.Localizations.GetLString(MainMenu.Language, buy_text[0]);
            return buy_text[1];
        }

        private string GetTransportName()
        {
            if (index == 0)
                return MainMenu.Localizations.GetLString(MainMenu.Language, transport.Name[0]);
            return transport.Name[1];
        }

        private int GetParametr(int type)
        {
            if (type == 0)
                return (int)(transport.Speed / 7.5 * 336);
            else if (type == 1)
                return (175 - transport.Controllability) * 336 / (175 - 90);
            return (int)(transport.TransportHP / 500 * 336);
        }

        private Image DrawWeaponParametrs()
        {
            Bitmap result = new Bitmap(parametrs_image.Width, parametrs_image.Height);
            using (Graphics g = Graphics.FromImage(result))
            {
                Brush progressbar_background = new SolidBrush(Color.FromArgb(77, 79, 86));
                Brush progressbar = new SolidBrush(Color.FromArgb(195, 195, 195));
                g.DrawImage(Properties.Resources.speed, 0, 0, 30, 30);
                g.DrawImage(Properties.Resources.сontrollability, 0, 38, 30, 30);
                g.DrawImage(Properties.Resources.durability, 0, 76, 30, 30);
                g.FillRectangle(progressbar_background, 35, 0, 340, 30);
                g.FillRectangle(progressbar_background, 35, 38, 340, 30);
                g.FillRectangle(progressbar_background, 35, 76, 340, 30);
                g.FillRectangle(progressbar, 37, 2, GetParametr(0), 26);
                g.FillRectangle(progressbar, 37, 40, GetParametr(1), 26);
                g.FillRectangle(progressbar, 37, 78, GetParametr(2), 26);
            }
            return result;
        }

        private void SLIL_TransportStoreInterface_VisibleChanged(object sender, EventArgs e)
        {
            Width = width;
            name.Text = GetTransportName();
            icon.Image = SLIL.ShopImageDict[transport.GetType()];
            parametrs_image.Image = DrawWeaponParametrs();
            buy_button.Text = $"{GetBuyText()} {transport.Cost}$";
        }

        private void Buy_button_Click(object sender, EventArgs e)
        {
            icon.Focus();
            if (player.Money >= transport.Cost)
            {
                if (MainMenu.sounds)
                    buy.Play(SLIL.EffectsVolume);
                (Parent.FindForm() as SLIL).AddTransport(transport.Index);
            }
            else
            {
                if (MainMenu.sounds)
                    cant_pressed?.Play(SLIL.EffectsVolume);
            }

        }
    }
}