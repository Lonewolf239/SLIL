using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SLIL_v0_1.MazeGenerator;
using System.Diagnostics;
using System.Globalization;
using Play_Sound;

namespace SLIL.SLIL_v0_1
{
    public partial class SLILv0_1 : Form
    {
        private static readonly Maze MazeGenerator = new Maze();
        public static bool Language = true;
        private readonly Random rand = new Random();
        private const int SCREEN_HEIGHT = 228, SCREEN_WIDTH = 344;
        private static int MazeHeight;
        private static int MazeWidth;
        private int MAP_WIDTH;
        private const int START_EASY = 5, START_NORMAL = 10, START_HARD = 15;
        private const double DEPTH = 8;
        private const double FOV = Math.PI / 3;
        private double player_x = 1.5d, player_y = 1.5d, player_a = 0;
        public static double LOOK_SPEED = 1.75f;
        private const double MOVE_SPEED = 1.75d;
        private static string MAP = "";
        private static readonly Bitmap SCREEN = new Bitmap(SCREEN_WIDTH, SCREEN_HEIGHT);
        private int seconds, minutes, fps;
        public static int difficulty = 2, old_difficulty;
        private enum Direction { STOP, FORWARD, BACK, LEFT, RIGHT };
        private Direction playerDirection = Direction.STOP;
        private Direction lookDirection = Direction.STOP;
        private DateTime total_time = DateTime.Now;
        private List<int> soundIndices = new List<int> { 0, 1, 2, 3, 4 };
        private int currentIndex = 0;
        private bool show_finish = true, map_presed = false;
        private Map_form form;
        private PlaySound step;
        private static readonly PlaySound[] steps =
        {
                new PlaySound(MainMenu.CGFReader.GetFile("step_0.wav"), false),
                new PlaySound(MainMenu.CGFReader.GetFile("step_1.wav"), false),
                new PlaySound(MainMenu.CGFReader.GetFile("step_2.wav"), false),
                new PlaySound(MainMenu.CGFReader.GetFile("step_3.wav"), false),
                new PlaySound(MainMenu.CGFReader.GetFile("step_4.wav"), false)
        };
        public PlaySound game_over;

        public SLILv0_1()
        {
            InitializeComponent();
            Cursor = Program.SLILCursor;
            Language = Check_Language();
        }

        private bool Check_Language()
        {
            CultureInfo ci = CultureInfo.InstalledUICulture;
            string language = ci.Name.ToLower();
            string[] supportedLanguages = { "ru", "uk", "be", "kk", "ky" };
            return Array.Exists(supportedLanguages, lang => lang.Equals(language) || lang.Equals(language.Substring(0, 2)));
        }

        private void Developer_name_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                Process.Start(new ProcessStartInfo("https://github.com/Lonewolf239") { UseShellExecute = true });
        }

        private void Show_settings_MouseEnter(object sender, EventArgs e)
        {
            if (start_btn.Enabled)
                show_settings.Image = Properties.Resources.setting_pressed_btn;
        }

        private void Show_settings_MouseLeave(object sender, EventArgs e)
        {
            if (start_btn.Enabled)
                show_settings.Image = Properties.Resources.setting_btn;
        }

        private void Show_settings_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && start_btn.Enabled)
            {
                SLIL_Settings form = new SLIL_Settings
                {
                    Owner = this
                };
                form.ShowDialog();
            }
        }

        private void Question_Click(object sender, EventArgs e)
        {
            top_panel.Focus();
            SLIL_about form = new SLIL_about();
            form.ShowDialog();
        }

        private void Step_sound_timer_Tick(object sender, EventArgs e)
        {
            if ((playerDirection == Direction.FORWARD || playerDirection == Direction.BACK) && (step == null || !step.IsPlaying))
            {
                if (currentIndex >= soundIndices.Count)
                {
                    soundIndices = soundIndices.OrderBy(x => rand.Next()).ToList();
                    currentIndex = 0;
                }
                step = steps[soundIndices[currentIndex]];
                step.PlayWithWait(MainMenu.Volume);
                currentIndex++;
            }
        }

        private void Time_remein_Tick(object sender, EventArgs e)
        {
            string space_0 = "0", space_1 = "0";
            seconds--;
            if (seconds < 0)
            {
                if (minutes > 0)
                {
                    minutes--;
                    seconds = 59;
                }
                else
                {
                    seconds = 0;
                    GameOver(0);
                }
            }
            if (seconds > 9) space_1 = "";
            if (minutes > 9) space_0 = "";
            status_text.Text = $"FPS: {fps} | TIME LEFT: {space_0}{minutes}:{space_1}{seconds}";
        }

        private void SLIL_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W || e.KeyCode == Keys.Up)
                playerDirection = Direction.FORWARD;
            if (e.KeyCode == Keys.S || e.KeyCode == Keys.Down)
                playerDirection = Direction.BACK;
            if (e.KeyCode == Keys.A || e.KeyCode == Keys.Left)
                lookDirection = Direction.LEFT;
            if (e.KeyCode == Keys.D || e.KeyCode == Keys.Right)
                lookDirection = Direction.RIGHT;
            if(e.KeyCode == Keys.M || e.KeyCode == Keys.Tab || e.KeyCode == Keys.Space)
            {
                if (!start_btn.Enabled)
                {
                    map_presed = true;
                    if (form != null)
                    {
                        map_timer.Stop();
                        form.Close();
                        form = null;
                        return;
                    }
                    form = new Map_form
                    {
                        Left = Right,
                        Top = Top,
                        _MAP = MAP,
                        _MazeHeight = MazeHeight,
                        _MazeWidth = MazeWidth,
                        _player_x = player_x,
                        _player_y = player_y
                    };
                    map_timer.Start();
                    form.Show();
                    Activate();
                }
            }
            if (e.KeyCode == Keys.Space && start_btn.Enabled)
                StartGame();
            if (e.KeyCode == Keys.Escape)
            {
                if (start_btn.Enabled) Close();
                else GameOver(-1);
            }
        }

        private void SLIL_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W || e.KeyCode == Keys.Up || e.KeyCode == Keys.S || e.KeyCode == Keys.Down)
                playerDirection = Direction.STOP;
            if (e.KeyCode == Keys.A || e.KeyCode == Keys.Left ||  e.KeyCode == Keys.D || e.KeyCode == Keys.Right)
                lookDirection = Direction.STOP;
        }

        private void Map_timer_Tick(object sender, EventArgs e)
        {
            form.Left = Right;
            form.Top = Top;
            form._player_x = player_x;
            form._player_y = player_y;
            form.show_finish = show_finish;
        }

        private void SLIL_LocationChanged(object sender, EventArgs e) => lookDirection = playerDirection = Direction.STOP;

        private void SLIL_Deactivate(object sender, EventArgs e)
        {
            if (!map_presed) lookDirection = playerDirection = Direction.STOP;
            map_presed = false;
        }

        private void PlayerMove(double elapsed_time)
        {
            switch (lookDirection)
            {
                case Direction.LEFT:
                    player_a += elapsed_time * LOOK_SPEED;
                    break;
                case Direction.RIGHT:
                    player_a -= elapsed_time * LOOK_SPEED;
                    break;
            }
            switch (playerDirection)
            {
                case Direction.FORWARD:
                    player_x += Math.Sin(player_a) * MOVE_SPEED * elapsed_time;
                    player_y += Math.Cos(player_a) * MOVE_SPEED * elapsed_time;
                    if (MAP[(int)player_y * MAP_WIDTH + (int)player_x] == '#')
                    {
                        player_x -= Math.Sin(player_a) * MOVE_SPEED * elapsed_time;
                        player_y -= Math.Cos(player_a) * MOVE_SPEED * elapsed_time;
                    }
                    else if (MAP[(int)player_y * MAP_WIDTH + (int)player_x] == '&')
                    {
                        GameOver(1);
                        return;
                    }
                    break;
                case Direction.BACK:
                    player_x -= Math.Sin(player_a) * MOVE_SPEED * elapsed_time;
                    player_y -= Math.Cos(player_a) * MOVE_SPEED * elapsed_time;
                    if (MAP[(int)player_y * MAP_WIDTH + (int)player_x] == '#')
                    {
                        player_x += Math.Sin(player_a) * MOVE_SPEED * elapsed_time;
                        player_y += Math.Cos(player_a) * MOVE_SPEED * elapsed_time;
                    }
                    else if (MAP[(int)player_y * MAP_WIDTH + (int)player_x] == '&')
                    {
                        GameOver(1);
                        return;
                    }
                    break;
            }
        }

        private void SLIL_Load(object sender, EventArgs e)
        {
            if (!Language) start_btn.Text = "START";
            LOOK_SPEED = MainMenu.SLIL_v0_1_LOOK_SPEED;
            difficulty = MainMenu.SLIL_v0_1_difficulty;
            if (LOOK_SPEED < 1 || LOOK_SPEED > 4.5) LOOK_SPEED = 1.75;
            if (difficulty < 0 || difficulty > 2) difficulty = 1;
            old_difficulty = difficulty;
            Activate();
        }

        static void InitMap()
        {
            StringBuilder sb = new StringBuilder();
            char[,] map = MazeGenerator.GenerateCharMap(MazeWidth, MazeHeight, '#', '.', '&');
            for (int y = 0; y < map.GetLength(1); y++)
            {
                for (int x = 0; x < map.GetLength(0); x++)
                {
                    try
                    {
                        if (map[x, y] == '.' && map[x + 1, y] == '#' && map[x, y + 1] == '#' && (map[x + 2, y] == '#' || map[x, y + 2] == '#'))
                            map[x, y] = '#';
                    }
                    catch { }
                    sb.Append(map[x, y]);
                }
            }
            MAP = sb.ToString();
        }

        private void SLIL_FormClosing(object sender, FormClosingEventArgs e)
        {
            raycast.Stop();
            time_remein.Stop();
            step_sound_timer.Stop();
            map_timer.Stop();
            game_over?.Dispose();
            step?.Dispose();
            for (int i = 0; i < steps.Length; i++)
                steps[i]?.Dispose();
            step = null;
            if (form != null)
            {
                form.Close();
                form = null;
            }
            MainMenu.SLIL_v0_1_LOOK_SPEED = LOOK_SPEED;
            MainMenu.SLIL_v0_1_difficulty = difficulty;
        }

        private void Raycast_Tick(object sender, EventArgs e)
        {
            DateTime time = DateTime.Now;
            double elapsed_time = (time - total_time).TotalSeconds;
            total_time = DateTime.Now;
            PlayerMove(elapsed_time);
            Color color;
            //Ray Casting
            for (int x = 0; x < SCREEN_WIDTH; x++)
            {
                double rayA = player_a + FOV / 2 - x * FOV / SCREEN_WIDTH;
                double ray_x = Math.Sin(rayA);
                double ray_y = Math.Cos(rayA);
                double distance = 0;
                bool hit_wall = false;
                bool is_bound = false;
                bool hit_finish = false;
                while (!hit_wall && !hit_finish && distance < DEPTH)
                {
                    distance += 0.1d;
                    int test_x = (int)(player_x + ray_x * distance);
                    int test_y = (int)(player_y + ray_y * distance);
                    if (test_x < 0 || test_x >= DEPTH + player_x || test_y < 0 || test_y >= DEPTH + player_y)
                    {
                        hit_wall = true;
                        distance = DEPTH;
                    }
                    else
                    {
                        char test_wall = MAP[test_y * MAP_WIDTH + test_x];
                        if (test_wall == '#')
                        {
                            hit_wall = true;
                            List<(double module, double cos)> bounds = new List<(double module, double cos)>();
                            for (int tx = 0; tx < 2; tx++)
                            {
                                for (int ty = 0; ty < 2; ty++)
                                {
                                    double vx = test_x + tx - player_x;
                                    double vy = test_y + ty - player_y;
                                    double module_vector = Math.Sqrt(vx * vx + vy * vy);
                                    double cos_a = ray_x * vx / module_vector + ray_y * vy / module_vector;
                                    bounds.Add((module_vector, cos_a));
                                }
                            }
                            bounds = bounds.OrderBy(v => v.module).ToList();
                            double bound_a = 0.03 / distance;
                            if (Math.Acos(bounds[0].cos) < bound_a || Math.Acos(bounds[1].cos) < bound_a)
                                is_bound = true;
                        }
                        else if (test_wall == '&')
                            hit_finish = true;
                    }
                }
                if (is_bound)
                    color = Color.White;
                else if (distance < DEPTH / 4.25)
                    color = Color.Silver;
                else if (distance < DEPTH / 3.25)
                    color = Color.DarkGray;
                else if (distance < DEPTH / 2.25)
                    color = Color.Gray;
                else if (distance < DEPTH)
                    color = Color.DimGray;
                else
                    color = Color.Black;
                int celling = (int)(SCREEN_HEIGHT / 2d - SCREEN_HEIGHT / FOV / distance);
                int floor = SCREEN_HEIGHT - celling;
                for (int y = 0; y < SCREEN_HEIGHT; y++)
                {
                    if (y <= celling)
                        SCREEN.SetPixel(x, y, Color.Blue);
                    else if (y > celling && y <= floor)
                    {
                        if (!hit_finish)
                            SCREEN.SetPixel(x, y, color);
                        else
                            SCREEN.SetPixel(x, y, Color.Lime);
                    }
                    else
                    {
                        double d = 1 - (y - SCREEN_HEIGHT / 2d) / (SCREEN_HEIGHT / 2d);
                        if (d < 0.25d)
                            SCREEN.SetPixel(x, y, Color.FromArgb(12, 12, 100));
                        else if (d < 0.5d)
                            SCREEN.SetPixel(x, y, Color.FromArgb(10, 10, 75));
                        else if (d < 0.75d)
                            SCREEN.SetPixel(x, y, Color.FromArgb(6, 6, 45));
                        else
                            SCREEN.SetPixel(x, y, Color.Black);
                    }
                }
                fps = (int)(1 / elapsed_time);
            }
            display.Image = SCREEN;
        }

        private void Start_btn_Click(object sender, EventArgs e)
        {
            top_panel.Focus();
            StartGame();
        }

        private void StartGame()
        {
            game_over_text.Visible = question.Enabled = start_btn.Enabled = false;
            seconds = 0;
            player_x = player_y = 1.5d;
            player_a = 0;
            lookDirection = playerDirection = Direction.STOP;
            if (difficulty == 0)
            {
                minutes = START_HARD;
                MazeHeight = MazeWidth = 20;
                show_finish = false;
            }
            else if (difficulty == 1)
            {
                minutes = START_NORMAL;
                MazeHeight = MazeWidth = 15;
                show_finish = true;
            }
            else
            {
                minutes = START_EASY;
                MazeHeight = MazeWidth = 10;
                show_finish = true;
            }
            MAP_WIDTH = MazeWidth * 3 + 1;
            InitMap();
            string space_0 = "0", space_1 = "0";
            if (seconds > 9)
                space_1 = "";
            if (minutes > 9)
                space_0 = "";
            status_text.Text = $"FPS: {fps} | TIME LEFT: {space_0}{minutes}:{space_1}{seconds}";
            raycast.Start();
            time_remein.Start();
            if (MainMenu.sounds)
                step_sound_timer.Start();
        }

        private void GameOver(int win)
        {
            raycast.Stop();
            time_remein.Stop();
            step_sound_timer.Stop();
            map_timer.Stop();
            if (form != null)
            {
                form.Close();
                form = null;
            }
            question.Enabled = start_btn.Enabled = true;
            status_text.Text = "";
            if (win == 1)
            {
                if (difficulty > 0)
                    difficulty--;
                StartGame();
            }
            else if(win == 0)
            {
                game_over_text.Visible = true;
                if (MainMenu.sounds)
                    game_over.Play(SLIL.Volume);
                difficulty = old_difficulty;
            }
            display.Image = null;
        }
    }
}