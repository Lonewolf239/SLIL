using System;
using System.Drawing;
using System.Windows.Forms;

namespace SLIL.SLIL_v0_1
{
    public partial class Map_form : Form
    {
        public string _MAP;
        public double _player_x, _player_y;
        public int _MazeHeight, _MazeWidth;
        public bool show_finish;
        private int MAP_HEIGHT;
        private int MAP_WIDTH;
        private Bitmap screen;

        public Map_form()
        {
            InitializeComponent();
            Cursor = Program.SLILCursorDefault;
        }

        private void Map_form_Load(object sender, EventArgs e)
        {
            MAP_HEIGHT = _MazeHeight * 3 + 1;
            MAP_WIDTH = _MazeWidth * 3 + 1;
            screen = new Bitmap(MAP_WIDTH, MAP_HEIGHT);
            map_refresh.Start();
        }

        private void Map_form_FormClosing(object sender, FormClosingEventArgs e) => map_refresh.Stop();

        private void Map_refresh_Tick(object sender, EventArgs e)
        {
            for (int y = 0; y < MAP_HEIGHT; y++)
            {
                for (int x = 0; x < MAP_WIDTH; x++)
                {
                    char mapChar = _MAP[y * MAP_WIDTH + x];
                    if (mapChar == '#')
                        screen.SetPixel(x, y, Color.Blue);
                    else if (mapChar == '&' && show_finish)
                        screen.SetPixel(x, y, Color.Lime);
                    else
                        screen.SetPixel(x, y, Color.Black);
                }
            }
            screen.SetPixel((int)_player_x, (int)_player_y, Color.Red);
            map_picturebox.Image = screen;
        }
    }
}