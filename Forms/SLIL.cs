﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MazeGenerator;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Convert_Bitmap;
using System.IO;
using System.Drawing.Drawing2D;
using System.Threading;
using SLIL.Classes;
using SLIL.UserControls;
using Play_Sound;

namespace SLIL
{
    public delegate void StartGameDelegate();
    public delegate void InitPlayerDelegate();
    public partial class SLIL : Form
    {
        private GameController Controller;
        private bool isCursorVisible = true;
        public int CustomMazeHeight, CustomMazeWidth;
        public bool CUSTOM = false, ShowFPS = true, ShowMiniMap = true, EnableAnimation = true;
        public static int difficulty = 1, old_difficulty;
        private int inDebug = 0;
        public static double LOOK_SPEED = 2.5;
        public StringBuilder CUSTOM_MAP = new StringBuilder();
        public int CUSTOM_X, CUSTOM_Y;
        private static readonly Maze MazeGenerator = new Maze();
        private readonly Random rand;
        private const int texWidth = 128;
        private readonly int[] SCREEN_HEIGHT = { 128, 128 * 2 }, SCREEN_WIDTH = { 228, 228 * 2 };
        public static int resolution = 0;
        public static bool hight_fps = true;
        private static int MazeHeight;
        private static int MazeWidth;
        private int MAP_WIDTH, MAP_HEIGHT;
        private const int START_EASY = 5, START_NORMAL = 10, START_HARD = 15, START_VERY_HARD = 20;
        private const double FOV = Math.PI / 3;
        private double elapsed_time = 0;
        private static double enemy_count;
        private static StringBuilder MAP = new StringBuilder();
        private static readonly StringBuilder DISPLAYED_MAP = new StringBuilder();
        private Bitmap SCREEN, WEAPON, BUFFER;
        private readonly Font[] consolasFont = { new Font("Consolas", 9.75F), new Font("Consolas", 16F), new Font("Consolas", 22F) };
        private readonly SolidBrush whiteBrush = new SolidBrush(Color.White);
        private readonly StringFormat rightToLeft = new StringFormat() { FormatFlags = StringFormatFlags.DirectionRightToLeft };
        private Graphics graphicsWeapon;
        private int seconds, minutes, fps;
        private enum Direction { STOP, FORWARD, BACK, LEFT, RIGHT, WALK, RUN };
        private Direction playerDirection = Direction.STOP, strafeDirection = Direction.STOP, playerMoveStyle = Direction.WALK;
        private DateTime total_time = DateTime.Now;
        private List<int> soundIndices = new List<int> { 0, 1, 2, 3, 4 };
        private int currentIndex = 0;
        private bool map_presed = false, active = true;
        private bool Paused = false;
        public static readonly Dictionary<Type, Image[]> IconDict = new Dictionary<Type, Image[]>
        {
            { typeof(Flashlight), new[] { Properties.Resources.missing } },
            { typeof(Knife), new[] { Properties.Resources.missing } },
            { typeof(Candy), new[] { Properties.Resources.missing } },
            { typeof(Rainblower), new[] { Properties.Resources.missing } },
            { typeof(Pistol), new[]
            {
                    /*LV1:*/ Properties.Resources.gun_1_0_icon,
                    /*LV2:*/ Properties.Resources.gun_1_1_icon,
                    /*LV3:*/ Properties.Resources.gun_1_2_icon,
                    /*LV4:*/ Properties.Resources.gun_1_3_icon
            } },
            { typeof(Shotgun), new[]
            {
                    /*LV1:*/ Properties.Resources.gun_0_0_icon,
                    /*LV2:*/ Properties.Resources.gun_0_1_icon,
                    /*LV3:*/ Properties.Resources.gun_0_2_icon,
            } },
            { typeof(SubmachineGun), new[]
            {
                    /*LV1:*/ Properties.Resources.gun_3_1_icon,
                    /*LV2:*/ Properties.Resources.gun_3_2_icon,
                    /*LV3:*/ Properties.Resources.gun_3_3_icon,
            } },
            { typeof(AssaultRifle), new[]
            {
                    /*LV1:*/ Properties.Resources.gun_4_1_icon,
                    /*LV2:*/ Properties.Resources.gun_4_2_icon,
                    /*LV3:*/ Properties.Resources.gun_4_3_icon,
            } },
            { typeof(SniperRifle), new[]
            {
                    Properties.Resources.gun_2_icon
            } },
            { typeof(Fingershot), new[] { Properties.Resources.missing } },
            { typeof(TSPitW), new[] { Properties.Resources.missing } },
            { typeof(Gnome), new[] { Properties.Resources.missing } },
            { typeof(FirstAidKit), new[]
            {
                Properties.Resources.medkit_icon,
                Properties.Resources.medkit_icon
            } },
            { typeof(Adrenalin), new[]
            {
                Properties.Resources.adrenalin_icon,
                Properties.Resources.adrenalin_icon
            } },
        };
        public static readonly Dictionary<Type, Image[,]> ImagesDict = new Dictionary<Type, Image[,]>
        {
            { typeof(Flashlight), new[,] { { Properties.Resources.flashlight, Properties.Resources.flashlight_run } } },
            { typeof(Knife), new[,] { { Properties.Resources.knife, Properties.Resources.knife_hit, Properties.Resources.knife_run } } },
            { typeof(Candy), new[,] { { Properties.Resources.gun_candy, Properties.Resources.gun_candy_shooted, Properties.Resources.gun_candy_run } } },
            { typeof(Rainblower), new[,]
            {
                   { Properties.Resources.gun_rainblower, Properties.Resources.gun_rainblower_shooted, Properties.Resources.gun_rainblower_run },
            } },
            { typeof(Pistol), new[,]
            {                    
                   /*LV1:*/ { Properties.Resources.gun_1_0, Properties.Resources.gun_1_0_shooted, Properties.Resources.gun_1_0_reload, Properties.Resources.gun_1_0_reload, Properties.Resources.gun_1_0, Properties.Resources.gun_1_0_run, Properties.Resources.gun_1_0_run },
                   /*LV2:*/ { Properties.Resources.gun_1_1, Properties.Resources.gun_1_1_shooted, Properties.Resources.gun_1_1_reload, Properties.Resources.gun_1_1_reload_empty, Properties.Resources.gun_1_1_empty, Properties.Resources.gun_1_1_run, Properties.Resources.gun_1_1_run_empty },
                   /*LV3:*/ { Properties.Resources.gun_1_2, Properties.Resources.gun_1_2_shooted, Properties.Resources.gun_1_2_reload, Properties.Resources.gun_1_2_reload_empty, Properties.Resources.gun_1_2_empty, Properties.Resources.gun_1_2_run, Properties.Resources.gun_1_2_run_empty },
                   /*LV4:*/ { Properties.Resources.gun_1_3, Properties.Resources.gun_1_3_shooted, Properties.Resources.gun_1_3_1_reload, Properties.Resources.gun_1_3_2_reload, Properties.Resources.gun_1_3, Properties.Resources.gun_1_3_run, Properties.Resources.gun_1_3_run },
            } },
            { typeof(Shotgun), new[,]
            {
                    /*LV1:*/ { Properties.Resources.gun_0_0, Properties.Resources.gun_0_0_shooted, Properties.Resources.gun_0_0_1_reload, Properties.Resources.gun_0_0_2_reload, Properties.Resources.gun_0_0_2_reload, Properties.Resources.gun_0_0_run },
                    /*LV2:*/ { Properties.Resources.gun_0_1, Properties.Resources.gun_0_1_shooted, Properties.Resources.gun_0_1_pump, Properties.Resources.gun_0_1_1_reload, Properties.Resources.gun_0_1_2_reload, Properties.Resources.gun_0_1_run },
                    /*LV3:*/ { Properties.Resources.gun_0_2, Properties.Resources.gun_0_2_shooted, Properties.Resources.gun_0_2_pump, Properties.Resources.gun_0_2_1_reload, Properties.Resources.gun_0_2_2_reload, Properties.Resources.gun_0_2_run },
            } },
            { typeof(SubmachineGun), new[,]
            {
                    /*LV1:*/ { Properties.Resources.gun_3_1, Properties.Resources.gun_3_1_shooted, Properties.Resources.gun_3_1_1_reload, Properties.Resources.gun_3_1_2_reload, Properties.Resources.gun_3_1_run},
                    /*LV2:*/ { Properties.Resources.gun_3_2, Properties.Resources.gun_3_2_shooted, Properties.Resources.gun_3_2_1_reload, Properties.Resources.gun_3_2_2_reload, Properties.Resources.gun_3_2_run },
                    /*LV3:*/ { Properties.Resources.gun_3_3, Properties.Resources.gun_3_3_shooted, Properties.Resources.gun_3_3_1_reload, Properties.Resources.gun_3_3_2_reload, Properties.Resources.gun_3_3_run }
            } },
            { typeof(AssaultRifle), new[,]
            {
                    /*LV1:*/ { Properties.Resources.gun_4_1, Properties.Resources.gun_4_1_shooted, Properties.Resources.gun_4_1_1_reload, Properties.Resources.gun_4_1_2_reload, Properties.Resources.gun_4_1_run },
                    /*LV2:*/ { Properties.Resources.gun_4_2, Properties.Resources.gun_4_2_shooted, Properties.Resources.gun_4_2_1_reload, Properties.Resources.gun_4_2_2_reload, Properties.Resources.gun_4_2_run },
                    /*LV3:*/ { Properties.Resources.gun_4_3, Properties.Resources.gun_4_3_shooted, Properties.Resources.gun_4_3_1_reload, Properties.Resources.gun_4_3_2_reload, Properties.Resources.gun_4_3_run }
            } },
            { typeof(SniperRifle), new[,]
            {
                    { Properties.Resources.gun_2, Properties.Resources.gun_2_shooted, Properties.Resources.gun_2_reload, Properties.Resources.gun_2_aiming, Properties.Resources.gun_2_run }
            } },
            { typeof(Fingershot), new[,]
            {
                   { Properties.Resources.gun_5, Properties.Resources.gun_5_shooted, Properties.Resources.gun_5_1_reload, Properties.Resources.gun_5_2_reload, Properties.Resources.gun_5_run, Properties.Resources.gun_5_run }
            } },
            { typeof(TSPitW), new[,]
            {
                   { Properties.Resources.gun_6, Properties.Resources.gun_6_shooted, Properties.Resources.gun_6_1_reload, Properties.Resources.gun_6_2_reload, Properties.Resources.gun_6_3_reload, Properties.Resources.gun_6 }
            } },
            { typeof(Gnome), new[,]
            {
                   { Properties.Resources.gun_gnome, Properties.Resources.gun_gnome_shooted,
                    Properties.Resources.gun_gnome_reloading_0, Properties.Resources.gun_gnome_reloading_1,
                    Properties.Resources.gun_gnome_reloading_2, Properties.Resources.gun_gnome_reloading_3,
                    Properties.Resources.gun_gnome_run }
            } },
            { typeof(FirstAidKit), new[,]
            {
                   { Properties.Resources.medkit, Properties.Resources.medkit, Properties.Resources.medkit_using_0, Properties.Resources.medkit_using_1, Properties.Resources.medkit_using_2, Properties.Resources.medkit_run },
                   { Properties.Resources.syringe, Properties.Resources.syringe, Properties.Resources.syringe_using_0, Properties.Resources.syringe_using_1, Properties.Resources.syringe_using_2, Properties.Resources.medkit_run },
                   { Properties.Resources.hand, Properties.Resources.hand, Properties.Resources.hand_using_0, Properties.Resources.hand_using_1, Properties.Resources.hand_using_2, Properties.Resources.medkit_run },
                   { Properties.Resources.food, Properties.Resources.food, Properties.Resources.food_using_0, Properties.Resources.food_using_1, Properties.Resources.food_using_2, Properties.Resources.medkit_run },
            } },
            { typeof(Adrenalin), new[,]
            {
                   { Properties.Resources.adrenalin, Properties.Resources.adrenalin, Properties.Resources.adrenalin_using_0, Properties.Resources.adrenalin_using_1, Properties.Resources.adrenalin_using_2, Properties.Resources.medkit_run },
            } },
        };
        public static readonly Dictionary<Type, PlaySound[,]> SoundsDict = new Dictionary<Type, PlaySound[,]>
        {
            { typeof(Flashlight), new[,] { { new PlaySound(null, false), } } },
            { typeof(Knife), new[,] { { new PlaySound(MainMenu.CGFReader.GetFile("knife.wav"), false) } } },
            { typeof(Candy), new[,] { { new PlaySound(MainMenu.CGFReader.GetFile("knife.wav"), false) } } },
            { typeof(Rainblower),  new[,]
            {
                   { new PlaySound(MainMenu.CGFReader.GetFile("gun_0_1.wav"), false), new PlaySound(null, false), new PlaySound(null, false) },
            } },
            { typeof(Pistol), new[,]
            {
                   /*LV1:*/ { new PlaySound(MainMenu.CGFReader.GetFile("gun_0_1.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("gun_0_2_reloading.wav"), false), new PlaySound(null, false) },
                   /*LV2:*/ { new PlaySound(MainMenu.CGFReader.GetFile("gun_0_2.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("gun_0_2_reloading.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("gun_0_empty.wav"), false) },
                   /*LV3:*/ { new PlaySound(MainMenu.CGFReader.GetFile("gun_0_4.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("gun_0_2_reloading.wav"), false), new PlaySound(null, false) },
                   /*LV4:*/ { new PlaySound(MainMenu.CGFReader.GetFile("gun_0_3.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("gun_0_3_reloading.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("gun_0_3_empty.wav"), false) },
            } },
            { typeof(Shotgun), new[,]
            {
                    /*LV1:*/ { new PlaySound(MainMenu.CGFReader.GetFile("gun_1.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("gun_1_0_reloading.wav"), false), new PlaySound(null, false), new PlaySound(null, false) },
                    /*LV2:*/ { new PlaySound(MainMenu.CGFReader.GetFile("gun_1.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("gun_1_1_reloading.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("gun_1_empty.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("gun_1_shell.wav"), false) },
                    /*LV3:*/ { new PlaySound(MainMenu.CGFReader.GetFile("gun_1.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("gun_1_2_reloading.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("gun_1_empty.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("gun_1_shell.wav"), false) },
            } },
            { typeof(SubmachineGun), new[,]
            {
                    /*LV1:*/ { new PlaySound(MainMenu.CGFReader.GetFile("gun_3_1.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("gun_3_1_reloading.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("gun_3_empty.wav"), false) },
                    /*LV2:*/ { new PlaySound(MainMenu.CGFReader.GetFile("gun_3_2.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("gun_3_2_reloading.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("gun_3_empty.wav"), false) },
                    /*LV3:*/ { new PlaySound(MainMenu.CGFReader.GetFile("gun_3_3.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("gun_3_3_reloading.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("gun_3_empty.wav"), false) }
            } },
            { typeof(AssaultRifle), new[,]
            {
                    /*LV1:*/ { new PlaySound(MainMenu.CGFReader.GetFile("gun_4.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("gun_4_1_reloading.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("gun_4_empty.wav"), false) },
                    /*LV2:*/ { new PlaySound(MainMenu.CGFReader.GetFile("gun_4.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("gun_4_1_reloading.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("gun_4_empty.wav"), false) },
                    /*LV3:*/ { new PlaySound(MainMenu.CGFReader.GetFile("gun_4_3.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("gun_4_3_reloading.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("gun_4_empty.wav"), false) }
            } },
            { typeof(SniperRifle), new[,]
            {
                    { new PlaySound(MainMenu.CGFReader.GetFile("gun_2.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("gun_2_reloading.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("gun_2_empty.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("gun_2_aiming.wav"), false) }
            } },
            { typeof(Fingershot), new[,]
            {
                   { new PlaySound(MainMenu.CGFReader.GetFile("gun_5.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("gun_5_reloading.wav"), false), new PlaySound(null, false) }
            } },
            { typeof(TSPitW), new[,]
            {
                   { new PlaySound(MainMenu.CGFReader.GetFile("gun_6.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("gun_6_reloading.wav"), false), new PlaySound(null, false) }
            } },
            { typeof(Gnome), new[,]
            {
                   { new PlaySound(MainMenu.CGFReader.GetFile("fireball.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("gnome_reloading.wav"), false), new PlaySound(null, false) }
            } },
            { typeof(FirstAidKit), new[,]
            {
                   { new PlaySound(null, false), new PlaySound(MainMenu.CGFReader.GetFile("medkit_using.wav"), false), new PlaySound(null, false) },
                   { new PlaySound(null, false), new PlaySound(MainMenu.CGFReader.GetFile("syringe_using.wav"), false), new PlaySound(null, false) },
                   { new PlaySound(null, false), new PlaySound(MainMenu.CGFReader.GetFile("hand_using.wav"), false), new PlaySound(null, false) },
                   { new PlaySound(null, false), new PlaySound(MainMenu.CGFReader.GetFile("food_using.wav"), false), new PlaySound(null, false) }
            } },
            { typeof(Adrenalin), new[,]
            {
                   { new PlaySound(null, false), new PlaySound(MainMenu.CGFReader.GetFile("adrenalin_using.wav"), false), new PlaySound(null, false) }
            } },
        };
        public static readonly Dictionary<Type, Image> ItemIconDict = new Dictionary<Type, Image>
        {
            { typeof(FirstAidKit), Properties.Resources.first_aid },
            { typeof(Adrenalin), Properties.Resources.adrenalin_count_icon },
        };
        public static readonly Dictionary<Type, Image> CuteItemIconDict = new Dictionary<Type, Image>
        {
            { typeof(FirstAidKit), Properties.Resources.food_count },
            { typeof(Adrenalin), Properties.Resources.adrenalin_count_icon },
        };
        private readonly PlaybackState playbackState = new PlaybackState();
        private readonly BindControls Bind;
        private readonly TextureCache textureCache;
        public PlaySound[,] step;
        public PlaySound[] ost;
        public PlaySound[,] DeathSounds;
        public PlaySound[,] CuteDeathSounds;
        public PlaySound game_over, draw, buy, hit, hungry, wall, tp, screenshot;
        public PlaySound[] door;
        private const string bossMap = "#########################...............##F###.................####..##...........##..###...=...........=...###...=.....E.....=...###...................###...................###.........#.........###...##.........##...###....#.........#....###...................###..#...##.#.##...#..####.....#.....#.....######...............##############d####################...#################E=...=E#################...#################$D.P.D$#################...################################",
            debugMap = @"####################.................##..=======........##..=.....=.....#..##..=....E=........##..=..====........##..=..=........d..##..=.E=...........##..====...........##........P.....=..##.................##.................##..............F..##.................##..WWW....#D#..#..##..WEW====#$#.#d=.##..WWW....###..=..##.................####################";
        public static float Volume = 0.4f;
        private static int MAX_SHOP_COUNT = 1;
        private int burst_shots = 0, reload_frames = 0;
        public static int ost_index = 0;
        public static int prev_ost;
        private Image scope_hit = null;
        private readonly Image[] scope =
        { 
            Properties.Resources.scope,
            Properties.Resources.scope_cross,
            Properties.Resources.scope_line,
            Properties.Resources.scope_dot,
            Properties.Resources.scope_null 
        };
        private readonly Image[] scope_shotgun =
        { 
            Properties.Resources.scope_shotgun,
            Properties.Resources.scope_cross,
            Properties.Resources.scope_line,
            Properties.Resources.scope_dot,
            Properties.Resources.scope_null 
        };
        public static int scope_color = 0, scope_type = 0;
        public static bool ShowMap = false;
        private bool open_shop = false, pressed_r = false, pressed_h = false;
        private Display display;
        private Bitmap map;
        private ConsolePanel console_panel;
        private readonly char[] impassibleCells  = { '#', 'D', '=', 'd' };
        private const double playerWidth = 0.4;
        private bool GameStarted = false, CorrectExit = false;

        StartGameDelegate StartGameHandle;
        InitPlayerDelegate InitPlayerHandle;
        public void StartGameInvokerSinglePlayer()
        {
            if (this.InvokeRequired && this.IsHandleCreated)
            {
                this.BeginInvoke((MethodInvoker)delegate
                {
                        this.StartGame();
                });
            }
            else
            {
                this.StartGame();
            }
        }
        public void StartGameInvokerMultiPlayer()
        {
            while (!this.IsHandleCreated)
            {

            }
            this.BeginInvoke((MethodInvoker)delegate
                {
                        this.StartGame();
                });
        }
        public void InitPlayerInvoker()
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    //player = Controller.GetPlayer();
                    //player.IsPetting = false;
                });
            }
            else
            {
                //player = Controller.GetPlayer();
                //player.IsPetting = false;
            }
        }

        public SLIL(TextureCache textures)
        {
            InitializeComponent();
            StartGameHandle = StartGameInvokerSinglePlayer;
            InitPlayerHandle = InitPlayerInvoker;
            Controller = new GameController(StartGameHandle, InitPlayerHandle);
            rand = new Random();
            Bind = new BindControls(MainMenu.BindControls);
            difficulty = MainMenu.difficulty;
            resolution = MainMenu.resolution;
            scope_type = MainMenu.scope_type;
            scope_color = MainMenu.scope_color;
            hight_fps = MainMenu.hight_fps;
            ShowFPS = MainMenu.ShowFPS;
            ShowMiniMap = MainMenu.ShowMiniMap;
            LOOK_SPEED = MainMenu.LOOK_SPEED;
            Volume = MainMenu.Volume;
            textureCache = textures; 
            Controller.AddPlayer();
            //player = Controller.GetPlayer();
            //player.IsPetting = false;
            ost = new PlaySound[]
            {
                new PlaySound(MainMenu.CGFReader.GetFile("slil_ost_0.wav"), true),
                new PlaySound(MainMenu.CGFReader.GetFile("slil_ost_1.wav"), true),
                new PlaySound(MainMenu.CGFReader.GetFile("slil_ost_2.wav"), true),
                new PlaySound(MainMenu.CGFReader.GetFile("slil_ost_3.wav"), true),
                new PlaySound(MainMenu.CGFReader.GetFile("slil_ost_4.wav"), true),
                new PlaySound(MainMenu.CGFReader.GetFile("soul_forge.wav"), true),
                new PlaySound(MainMenu.CGFReader.GetFile("gnome.wav"), true)
            };
            Controller.StartGame();
        }
        public SLIL(TextureCache textures, string adress, int port)
        {
            InitializeComponent();
            StartGameHandle = StartGameInvokerMultiPlayer;
            InitPlayerHandle = InitPlayerInvoker;
            Controller = new GameController(adress, port, StartGameHandle, InitPlayerHandle);
            rand = new Random();
            Bind = new BindControls(MainMenu.BindControls);
            difficulty = MainMenu.difficulty;
            resolution = MainMenu.resolution;
            scope_type = MainMenu.scope_type;
            scope_color = MainMenu.scope_color;
            hight_fps = MainMenu.hight_fps;
            ShowFPS = MainMenu.ShowFPS;
            ShowMiniMap = MainMenu.ShowMiniMap;
            LOOK_SPEED = MainMenu.LOOK_SPEED;
            Volume = MainMenu.Volume;
            textureCache = textures;
            ost = new PlaySound[]
            {
                new PlaySound(MainMenu.CGFReader.GetFile("slil_ost_0.wav"), true),
                new PlaySound(MainMenu.CGFReader.GetFile("slil_ost_1.wav"), true),
                new PlaySound(MainMenu.CGFReader.GetFile("slil_ost_2.wav"), true),
                new PlaySound(MainMenu.CGFReader.GetFile("slil_ost_3.wav"), true),
                new PlaySound(MainMenu.CGFReader.GetFile("slil_ost_4.wav"), true),
                new PlaySound(MainMenu.CGFReader.GetFile("soul_forge.wav"), true),
                new PlaySound(MainMenu.CGFReader.GetFile("gnome.wav"), true)
            };
        }

        public void AddPet(int index)
        {
            foreach (SLIL_PetShopInterface control in pet_shop_page.Controls.Find("SLIL_PetShopInterface", true))
                control.buy_button.Text = MainMenu.Language ? $"Купить ${control.pet.Cost}" : $"Buy ${control.pet.Cost}";
            Controller.AddPet(index);
            //for (int i = 0; i < Entities.Count; i++)
            //{
            //    if (Entities[i] is Pet)
            //    {
            //        if ((Entities[i] as Pet).IsInstantAbility != 0)
            //        {
            //            switch ((Entities[i] as Pet).GetPetAbility())
            //            {
            //                case 1: //GreenGnome
            //                    player.MAX_HP -= 25;
            //                    player.HealHP(125);
            //                    break;
            //                case 2: //Energy Drink
            //                    player.MAX_STAMINE -= 150;
            //                    player.MOVE_SPEED -= 0.15;
            //                    player.RUN_SPEED -= 0.15;
            //                    break;
            //                case 3: //Pyro
            //                    player.CuteMode = false;
            //                    CuteMode();
            //                    break;
            //            }
            //        }
            //        Entities.RemoveAt(i);
            //    }
            //}
            //if (pet.IsInstantAbility != 0)
            //{
            //    switch (pet.GetPetAbility())
            //    {
            //        case 1: //GreenGnome
            //            player.MAX_HP += 25;
            //            player.HealHP(125);
            //            break;
            //        case 2: //Energy Drink
            //            player.MAX_STAMINE += 150;
            //            player.MOVE_SPEED += 0.15;
            //            player.RUN_SPEED += 0.15;
            //            break;
            //        case 3: //Pyro
            //            player.CuteMode = true;
            //            CuteMode();
            //            break;

            //    }
            //}
            //player.PET = pet;
            //UpdatePet();
        }

        private void CuteMode()
        {
            Player player = Controller.GetPlayer();
            player.Guns.Clear();
            shop_tab_control.Controls.Clear();
            if (player.CuteMode)
            {
                prev_ost = ost_index;
                ChangeOst(this, 7);
                player.DEPTH = 10;
                player.GUNS[11].HasIt = true;
                player.GUNS[12].HasIt = true;
                player.Guns.Add(player.GUNS[11]);
                player.Guns.Add(player.GUNS[12]);
            }
            else
            {
                prev_ost = rand.Next(ost.Length - 3);
                ChangeOst(this, prev_ost);
                player.DEPTH = 8;
                shop_tab_control.Controls.Add(weapon_shop_page);
                player.GUNS[11].HasIt = false;
                player.GUNS[12].HasIt = false;
                for(int i = 0; i < 10; i++)
                {
                    if (player.GUNS[i].HasIt)
                        player.Guns.Add(player.GUNS[i]);
                }
            }
            shop_tab_control.Controls.Add(pet_shop_page);
            shop_tab_control.Controls.Add(consumables_shop_page);
            TakeFlashlight(false);
            ChangeWeapon(1);
        }

        private void Chill_timer_Tick(object sender, EventArgs e) => chill_timer.Stop();

        private void Shop_panel_VisibleChanged(object sender, EventArgs e)
        {
            Player player = Controller.GetPlayer();
            if (player != null)
                player.Look = 0;
            shop_panel.BringToFront();
        }

        private void Stage_timer_Tick(object sender, EventArgs e) => stage_timer.Stop();

        public static void SetVolume(SLIL slil) => slil.ost[ost_index].SetVolume(Volume);

        public void ShowShop()
        {
            open_shop = true;
            ShopInterface_panel.VerticalScroll.Value = 0;
            mouse_timer.Stop();
            time_remein.Stop();
            shop_panel.BringToFront();
            shop_panel.Visible = true;
        }

        private void SLIL_Activated(object sender, EventArgs e) => active = true;

        private void Pause_btn_Click(object sender, EventArgs e)
        {
            pause_panel.Focus();
            Pause();
        }

        private void Exit_btn_Click(object sender, EventArgs e)
        {
            pause_panel.Focus();
            GameOver(-1);
            CorrectExit = true;
            Close();
        }

        public static void GoDebug(SLIL slil, int debug)
        {
            slil.GameOver(-1);
            slil.inDebug = debug;
            old_difficulty = difficulty;
            difficulty = 5;
            slil.StartGame();
        }

        public static void ChangeOst(SLIL slil, int index)
        {
            if (!MainMenu.sounds)
                return;
            slil.ost[ost_index]?.Stop();
            ost_index = index;
            slil.ost[ost_index].LoopPlay(Volume);
        }

        private async void Step_sound_timer_Tick(object sender, EventArgs e)
        {
            Player player = Controller.GetPlayer();
            if (player == null) return;
            if ((playerDirection != Direction.STOP || strafeDirection != Direction.STOP) && !player.Aiming && !playbackState.IsPlaying)
            {
                if (currentIndex >= soundIndices.Count)
                {
                    soundIndices = soundIndices.OrderBy(x => rand.Next()).ToList();
                    currentIndex = 0;
                }
                int i = playerMoveStyle == Direction.RUN || player.Fast ? 1 : 0;
                if (player.CuteMode)
                    i += 2;
                int j = soundIndices[currentIndex];
                bool completed = await step[i, j].PlayWithWait(Volume, playbackState);
                if (completed)
                    currentIndex++;
            }
        }

        private void Time_remein_Tick(object sender, EventArgs e)
        {
            Player player = Controller.GetPlayer();
            if (player == null) return;
            seconds--;
            if (player.Invulnerable)
                player.InvulnerableEnd();
            player.UpdateEffectsTime();
            Pet playerPet = player.PET;
            if (playerPet != null && playerPet.IsInstantAbility != 1)
            {
                if (playerPet.PetAbilityReloading)
                {
                    if (playerPet.AbilityTimer >= playerPet.AbilityReloadTime)
                        playerPet.PetAbilityReloading = false;
                    else
                        playerPet.AbilityTimer++;
                }
                if (!playerPet.PetAbilityReloading)
                {
                    switch (playerPet.GetPetAbility())
                    {
                        case 0: //Silly Cat
                            player.HealHP(2);
                            playerPet.AbilityTimer = 0;
                            playerPet.PetAbilityReloading = true;
                            break;
                        case 3: //Pyro
                            if (player.GUNS[12].AmmoCount + 10 <= player.GUNS[12].MaxAmmo)
                                player.GUNS[12].AmmoCount += 10;
                            else
                                player.GUNS[12].AmmoCount = player.GUNS[12].MaxAmmo;
                            playerPet.AbilityTimer = 0;
                            playerPet.PetAbilityReloading = true;
                            break;
                    }
                }
            }
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
        }

        private void Status_refresh_Tick(object sender, EventArgs e)
        {
            if (display == null) return;
            if (!raycast.Enabled && display.SCREEN != null)
                display.SCREEN = null;
            bool shouldShowCursor = !GameStarted || open_shop || console_panel.Visible || (active && !GameStarted) || Paused;
            if (shouldShowCursor != isCursorVisible)
            {
                if (shouldShowCursor)
                {
                    Cursor.Show();
                    isCursorVisible = true;
                }
                else
                {
                    Cursor.Hide();
                    isCursorVisible = false;
                }
            }
            if (!GameStarted)
            {
                Paused = false;
                pause_panel.Visible = false;
            }
            if (Paused)
            {
                pause_btn.Left = (Width - pause_btn.Width) / 2;
                exit_btn.Left = (Width - exit_btn.Width) / 2;
            }
            if (game_over_panel.Visible)
            {
                restart_btn.Left = (Width - restart_btn.Width) / 2;
                exit_restart_btn.Left = (Width - exit_restart_btn.Width) / 2;
            }
            Player player = Controller.GetPlayer();
            if (player == null) return;
            shop_money.Text = $"$: {player.Money}";
            if (player.HP <= 0 && GameStarted)
                GameOver(0);
            try
            {
                if (!shot_timer.Enabled && !reload_timer.Enabled && !pressed_h)
                {
                    if (player.DisposableItems.Count > 0 && player.Guns.Contains(player.DisposableItems[player.SelectedItem]))
                    {
                        ChangeWeapon(player.PreviousGun);
                        player.PreviousGun = player.CurrentGun;
                        player.Guns.Remove(player.DisposableItems[player.SelectedItem]);
                        if (player.DisposableItems[player.SelectedItem].AmmoCount <= 0 && player.DisposableItems[player.SelectedItem].MaxAmmoCount <= 0)
                            player.DisposableItems[player.SelectedItem].HasIt = false;
                    }
                }
                if (player.GetCurrentGun() is Flashlight)
                    shot_timer.Enabled = reload_timer.Enabled = false;
                if (player.GetCurrentGun() is TSPitW)
                {
                    if (playerMoveStyle != Direction.WALK)
                        playerMoveStyle = Direction.WALK;
                    if (player.STAMINE > 15)
                        player.STAMINE -= 15;
                }
                if (playerMoveStyle == Direction.RUN)
                {
                    if (player.GetCurrentGun() is Pistol || player.GetCurrentGun() is Shotgun || player.GetCurrentGun() is Fingershot)
                    {
                        if (player.GetCurrentGun() is Pistol && player.GetCurrentGun().AmmoCount <= 0)
                            player.MoveStyle = 6;
                        else
                            player.MoveStyle = 5;
                    }
                    else if (player.GetCurrentGun() is Flashlight)
                        player.MoveStyle = 1;
                    else if (player.GetCurrentGun() is Knife || player.GetCurrentGun() is Rainblower)
                        player.MoveStyle = 2;
                    else if (player.GetCurrentGun() is Gnome)
                        player.MoveStyle = 6;
                    else
                        player.MoveStyle = 4;
                }
                else
                {
                    if (player.GetCurrentGun() is Pistol && player.GetCurrentGun().AmmoCount <= 0 && player.GetCurrentGun().Level != Levels.LV4 && !shot_timer.Enabled && !reload_timer.Enabled)
                        player.MoveStyle = 4;
                    else
                        player.MoveStyle = 0;
                }
                if (!shot_timer.Enabled && !reload_timer.Enabled && player.GunState != player.MoveStyle && !player.Aiming)
                    player.GunState = player.MoveStyle;
                if (player.LevelUpdated && !open_shop)
                {
                    ChangeWeapon(player.CurrentGun);
                    player.LevelUpdated = false;
                }
            }
            catch { }
        }

        private void SLIL_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                if (open_shop)
                {
                    open_shop = false;
                    time_remein.Start();
                    mouse_timer.Start();
                    int x = display.PointToScreen(Point.Empty).X + (display.Width / 2);
                    int y = display.PointToScreen(Point.Empty).Y + (display.Height / 2);
                    Cursor.Position = new Point(x, y);
                    shop_panel.Visible = false;
                }
                else if (console_panel.Visible)
                {
                    scope[scope_type] = GetScope(scope[scope_type]);
                    scope_shotgun[scope_type] = GetScope(scope_shotgun[scope_type]);
                    console_panel.Visible = false;
                    time_remein.Start();
                    mouse_timer.Start();
                    console_panel.command_input.Text = null;
                    display.Focus();
                    int x = display.PointToScreen(Point.Empty).X + (display.Width / 2);
                    int y = display.PointToScreen(Point.Empty).Y + (display.Height / 2);
                    Cursor.Position = new Point(x, y);
                }
                else if (!GameStarted)
                    Close();
                else
                    Pause();
                return;
            }
            if (GameStarted && !Paused)
            {
                if (!console_panel.Visible)
                {
                    if (!open_shop)
                    {
                        Keys runKey;
                        switch (Bind.Run)
                        {
                            case Keys.ShiftKey:
                                runKey = Keys.Shift;
                                break;
                            case Keys.ControlKey:
                                runKey = Keys.Control;
                                break;
                            case Keys.Alt:
                                runKey = Keys.Alt;
                                break;
                            default:
                                runKey = Keys.Shift;
                                break;
                        }
                        Player player = Controller.GetPlayer();
                        if (player != null && ModifierKeys.HasFlag(runKey) && playerDirection == Direction.FORWARD && !player.Fast && player.STAMINE >= player.MAX_STAMINE / 1.75 && !player.Aiming && !reload_timer.Enabled && !chill_timer.Enabled)
                            playerMoveStyle = Direction.RUN;
                        if (e.KeyCode == Bind.Forward)
                            playerDirection = Direction.FORWARD;
                        if (e.KeyCode == Bind.Back)
                            playerDirection = Direction.BACK;
                        if (e.KeyCode == Bind.Left)
                            strafeDirection = Direction.LEFT;
                        if (e.KeyCode == Bind.Right)
                            strafeDirection = Direction.RIGHT;
                        if (!shot_timer.Enabled && !reload_timer.Enabled && player!= null)
                        {
                            int count = player.Guns.Count;
                            if (player.Guns.Contains(player.GUNS[0]))
                                count--;
                            if (e.KeyCode == Bind.Reloading)
                            {
                                if (player.GetCurrentGun().AmmoCount != player.GetCurrentGun().CartridgesClip && player.GetCurrentGun().MaxAmmoCount > 0)
                                {
                                    pressed_r = true;
                                    player.CanShoot = false;
                                    player.Aiming = false;
                                    int sound = 1;
                                    if (!(player.GetCurrentGun() is Shotgun))
                                    {
                                        player.GunState = 2;
                                        if (player.GetCurrentGun() is Pistol && player.GetCurrentGun().Level != Levels.LV4 && player.GetCurrentGun().AmmoCount == 0)
                                            player.GunState = 3;
                                    }
                                    else
                                    {
                                        player.GunState = 3;
                                        if (player.GetCurrentGun().Level != Levels.LV1)
                                            sound = 3;
                                    }
                                    if (MainMenu.sounds)
                                        SoundsDict[player.GetCurrentGun().GetType()][player.GetCurrentGun().GetLevel(), sound].Play(Volume);
                                    reload_timer.Start();
                                }
                            }
                            if (e.KeyCode == Bind.Item)
                            {
                                if (player.DisposableItems.Count > 0 && player.DisposableItems[player.SelectedItem].HasIt)
                                {
                                    if (player.SelectedItem == 0 && (player.EffectCheck(0) || player.HP == player.MAX_HP)) return;
                                    if (player.SelectedItem == 1 && player.EffectCheck(1)) return;
                                    TakeFlashlight(false);
                                    pressed_h = true;
                                    if (!player.Guns.Contains(player.DisposableItems[player.SelectedItem]))
                                        player.Guns.Add(player.DisposableItems[player.SelectedItem]);
                                    player.PreviousGun = player.CurrentGun;
                                    if (player.DisposableItems[player.SelectedItem].HasLVMechanics)
                                    {
                                        if (player.CuteMode)
                                            player.DisposableItems[player.SelectedItem].Level = Levels.LV4;
                                        else
                                        {
                                            if (rand.NextDouble() <= player.CurseCureChance)
                                            {
                                                if (rand.NextDouble() <= 0.5)
                                                    player.DisposableItems[player.SelectedItem].Level = Levels.LV2;
                                                else
                                                    player.DisposableItems[player.SelectedItem].Level = Levels.LV3;
                                            }
                                            else
                                                player.DisposableItems[player.SelectedItem].Level = Levels.LV1;
                                        }
                                    }
                                    else
                                        player.DisposableItems[player.SelectedItem].Level = Levels.LV1;
                                    ChangeWeapon(player.Guns.IndexOf(player.DisposableItems[player.SelectedItem]));
                                    player.GunState = 1;
                                    player.Aiming = false;
                                    player.CanShoot = false;
                                    player.UseItem = true;
                                    burst_shots = 0;
                                    shot_timer.Start();
                                    pressed_h = false;
                                }
                            }
                            if (e.KeyCode == Bind.SelectItem)
                            {
                                int selected_item = player.SelectedItem;
                                selected_item++;
                                if (selected_item >= player.DisposableItems.Count) selected_item = 0;
                                player.SelectedItem = selected_item;
                            }
                            if (e.KeyCode == Keys.D1)
                            {
                                TakeFlashlight(false);
                                ChangeWeapon(0);
                            }
                            if (e.KeyCode == Keys.D2 && count > 1)
                            {
                                TakeFlashlight(false);
                                ChangeWeapon(1);
                            }
                            if (e.KeyCode == Keys.D3 && count > 2)
                            {
                                TakeFlashlight(false);
                                ChangeWeapon(2);
                            }
                            if (e.KeyCode == Keys.D4 && count > 3)
                            {
                                TakeFlashlight(false);
                                ChangeWeapon(3);
                            }
                            if (e.KeyCode == Keys.D5 && count > 4)
                            {
                                TakeFlashlight(false);
                                ChangeWeapon(4);
                            }
                            if (e.KeyCode == Keys.D6 && count > 5)
                            {
                                TakeFlashlight(false);
                                ChangeWeapon(5);
                            }
                            if (e.KeyCode == Keys.D7 && count > 6)
                            {
                                TakeFlashlight(false);
                                ChangeWeapon(6);
                            }
                            if (e.KeyCode == Keys.D8 && count > 7)
                            {
                                TakeFlashlight(false);
                                ChangeWeapon(7);
                            }
                            if (e.KeyCode == Keys.D9 && count > 8)
                            {
                                TakeFlashlight(false);
                                ChangeWeapon(8);
                            }
                            if (e.KeyCode == Keys.D0 && count > 9)
                            {
                                TakeFlashlight(false);
                                ChangeWeapon(9);
                            }
                        }
                    }
                }
                if (e.KeyCode == Keys.Oemtilde && !open_shop && MainMenu.ConsoleEnabled)
                {
                    console_panel.Visible = !console_panel.Visible;
                    ShowMap = false;
                    if (console_panel.Visible)
                    {
                        mouse_timer.Stop();
                        time_remein.Stop();
                        console_panel.command_input.Text = null;
                        console_panel.command_input.Focus();
                        console_panel.BringToFront();
                    }
                    else
                    {
                        time_remein.Start();
                        mouse_timer.Start();
                        console_panel.command_input.Text = null;
                        display.Focus();
                        int x = display.PointToScreen(Point.Empty).X + (display.Width / 2);
                        int y = display.PointToScreen(Point.Empty).Y + (display.Height / 2);
                        Cursor.Position = new Point(x, y);
                    }
                    scope[scope_type] = GetScope(scope[scope_type]);
                    scope_shotgun[scope_type] = GetScope(scope_shotgun[scope_type]);
                }
            }
        }

        private void SLIL_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Bind.Run)
            {
                playerMoveStyle = Direction.WALK;
                if (!chill_timer.Enabled)
                    chill_timer.Start();
            }
            if (e.KeyCode == Bind.Forward || e.KeyCode == Bind.Back)
                playerDirection = Direction.STOP;
            if (e.KeyCode == Bind.Left || e.KeyCode == Bind.Right)
                strafeDirection = Direction.STOP;
            if (GameStarted && !Paused && !console_panel.Visible && !open_shop)
            {
                if (e.KeyCode == Bind.Screenshot)
                    DoScreenshot();
                if (e.KeyCode == Bind.Show_map_0 || e.KeyCode == Bind.Show_map_1)
                {
                    map_presed = true;
                    ShowMap = !ShowMap;
                    Activate();
                }
                Player player = Controller.GetPlayer();
                if (!shot_timer.Enabled && !reload_timer.Enabled && !player.IsPetting)
                {
                    if (e.KeyCode == Bind.Flashlight)
                        TakeFlashlight(true);
                }
                if (e.KeyCode == Bind.Interaction_0 || e.KeyCode == Bind.Interaction_1)
                {
                    double rayA = player.A + FOV / 2 - (SCREEN_WIDTH[resolution] / 2) * FOV / SCREEN_WIDTH[resolution];
                    double ray_x = Math.Sin(rayA);
                    double ray_y = Math.Cos(rayA);
                    double distance = 0;
                    bool hit = false;
                    while (raycast.Enabled && !hit && distance <= 1)
                    {
                        distance += 0.1d;
                        int x = (int)(player.X + ray_x * distance);
                        int y = (int)(player.Y + ray_y * distance);
                        char test_wall = Controller.GetMap()[y * Controller.GetMapWidth() + x];
                        switch (test_wall)
                        {
                            case '#':
                            case '=':
                            case 'F':
                                hit = true;
                                if (MainMenu.sounds)
                                    wall.Play(Volume);
                                break;
                            case 'D':
                                hit = true;
                                ShowShop();
                                break;
                            case 'd':
                                hit = true;
                                if (MainMenu.sounds)
                                    door[0].Play(Volume);
                                Controller.GetMap()[y * Controller.GetMapWidth() + x] = 'o';
                                break;
                            case 'o':
                                hit = true;
                                if (distance < playerWidth || ((int)player.X == x && (int)player.Y == y))
                                    break;
                                if (MainMenu.sounds)
                                    door[1].Play(Volume);
                                Controller.GetMap()[y * Controller.GetMapWidth() + x] = 'd';
                                break;
                        }
                    }
                    if (hit)
                        return;
                    DateTime time = DateTime.Now;
                    elapsed_time = (time - total_time).TotalSeconds;
                    total_time = time;
                    PlayerMove();
                    ClearDisplayedMap();
                    int factor = player.Aiming ? 12 : 0;
                    if (player.GetCurrentGun() is Flashlight)
                        factor = 8;
                    double[] ZBuffer = new double[SCREEN_WIDTH[resolution]];
                    double[] ZBufferWindow = new double[SCREEN_WIDTH[resolution]];
                    Pixel[][] rays = CastRaysParallel(ZBuffer, ZBufferWindow);
                    List<Entity> Entities = Controller.GetEntities();
                    int[] spriteOrder = new int[Entities.Count];
                    double[] spriteDistance = new double[Entities.Count];
                    int[] textures = new int[Entities.Count];
                    for (int i = 0; i < Entities.Count; i++)
                    {
                        spriteOrder[i] = i;
                        spriteDistance[i] = (player.X - Entities[i].X) * (player.X - Entities[i].X) + (player.Y - Entities[i].Y) * (player.Y - Entities[i].Y);
                        textures[i] = Entities[i].Texture;
                    }
                    SortSpritesNotReversed(ref spriteOrder, ref spriteDistance, ref textures, Entities.Count);
                    for (int i = 0; i < Entities.Count; i++)
                    {
                        Entity entity = Entities[spriteOrder[i]];
                        if (!(entity is Enemy))
                        {
                            double spriteX = entity.X - player.X;
                            double spriteY = entity.Y - player.Y;
                            double dirX = Math.Sin(player.A);
                            double dirY = Math.Cos(player.A);
                            double planeX = Math.Sin(player.A - Math.PI / 2) * Math.Tan(FOV / 2);
                            double planeY = Math.Cos(player.A - Math.PI / 2) * Math.Tan(FOV / 2);
                            double invDet = 1.0 / (planeX * dirY - dirX * planeY);
                            double transformX = invDet * (dirY * spriteX - dirX * spriteY);
                            double transformY = invDet * (-planeY * spriteX + planeX * spriteY);
                            int spriteScreenX = (int)((SCREEN_WIDTH[resolution] / 2) * (1 + transformX / transformY));
                            double Distance = Math.Sqrt((player.X - entity.X) * (player.X - entity.X) + (player.Y - entity.Y) * (player.Y - entity.Y));
                            if (Distance == 0) Distance = 0.01;
                            double spriteTop = (SCREEN_HEIGHT[resolution] - player.Look) / 2 - (SCREEN_HEIGHT[resolution] * FOV) / Distance;
                            double spriteBottom = SCREEN_HEIGHT[resolution] - (spriteTop + player.Look);
                            int spriteCenterY = (int)((spriteTop + spriteBottom) / 2);
                            int drawStartY = (int)spriteTop;
                            int drawEndY = (int)spriteBottom;
                            int spriteHeight = Math.Abs((int)(SCREEN_HEIGHT[resolution] / Distance));
                            int spriteWidth = Math.Abs((int)(SCREEN_WIDTH[resolution] / Distance));
                            int drawStartX = -spriteWidth / 2 + spriteScreenX;
                            if (drawStartX < 0) drawStartX = 0;
                            int drawEndX = spriteWidth / 2 + spriteScreenX;
                            if (drawEndX >= SCREEN_WIDTH[resolution]) drawEndX = SCREEN_WIDTH[resolution];
                            var timeNow = (long)((DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds * 2);
                            for (int stripe = drawStartX; stripe < drawEndX; stripe++)
                            {
                                int texWidth = 128;
                                double texX = (double)((256 * (stripe - (-spriteWidth / 2 + spriteScreenX)) * texWidth / spriteWidth) / 256) / texWidth;
                                if (transformY > 0 && stripe > 0 && stripe < SCREEN_WIDTH[resolution] && transformY < ZBuffer[stripe])
                                {
                                    for (int y = drawStartY; y < drawEndY && y < SCREEN_HEIGHT[resolution]; y++)
                                    {
                                        if (y < 0 || (transformY > ZBufferWindow[stripe] && y > spriteCenterY))
                                            continue;
                                        double d = y - (SCREEN_HEIGHT[resolution] - (int)player.Look) / 2 + (drawEndY - drawStartY) / 2;
                                        double texY = d / (drawEndY - drawStartY);
                                        if (y == drawStartY) texY = 0;
                                        if (rays[stripe].Length > y && y >= 0)
                                        {
                                            if (EnableAnimation)
                                            {
                                                if (player.GetCurrentGun() is Flashlight && entity.RespondsToFlashlight)
                                                    rays[stripe][y].TextureId = textures[i] + 2;
                                                else
                                                    rays[stripe][y].TextureId = entity.Animations[0][timeNow % entity.Frames];
                                            }
                                            else
                                                rays[stripe][y].TextureId = textures[i];
                                            rays[stripe][y].Blackout = 0;
                                            rays[stripe][y].TextureX = texX;
                                            rays[stripe][y].TextureY = texY;
                                            Color color = GetColorForPixel(rays[stripe][y]);
                                            if (color != Color.Transparent && stripe == SCREEN_WIDTH[resolution] / 2 && y == SCREEN_HEIGHT[resolution] / 2 && player.GetCurrentGun().FiringRange >= Distance)
                                            {
                                                if (Distance <= 2)
                                                {
                                                    switch (entity.Interaction())
                                                    {
                                                        case 1: //SillyCat
                                                            if (player.IsPetting) break;
                                                            player.IsPetting = true;
                                                            new Thread(() =>
                                                            {
                                                                Thread.Sleep(2000);
                                                                player.IsPetting = false;
                                                            }).Start();
                                                            break;
                                                        case 2: //GreenGnome
                                                            if (player.IsPetting) break;
                                                            player.IsPetting = true;
                                                            new Thread(() =>
                                                            {
                                                                Thread.Sleep(2000);
                                                                player.IsPetting = false;
                                                            }).Start();
                                                            break;
                                                        case 3: //EnergyDrink
                                                            if (player.IsPetting) break;
                                                            player.IsPetting = true;
                                                            new Thread(() =>
                                                            {
                                                                Thread.Sleep(2000);
                                                                player.IsPetting = false;
                                                            }).Start();
                                                            break;
                                                    }
                                                }
                                                return;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void DoScreenshot()
        {
            string path = GetPath();
            console_panel.Log($"Screenshot successfully created and saved to path:\n<{path}<", true, true, Color.Lime);
            using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
                BUFFER?.Save(fileStream, ImageFormat.Png);
            if (MainMenu.sounds)
                screenshot.Play(Volume);
        }

        private string GetPath()
        {
            DateTime dateTime = DateTime.Now;
            string path = Path.Combine("screenshots", $"screenshot_{dateTime:yyyy_MM_dd}__{dateTime:HH_mm_ss}.png");
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            return path;
        }

        private void Pause()
        {
            Paused = !Paused;
            pause_panel.Visible = Paused;
            if (Paused)
            {
                time_remein.Stop();
                stamina_timer.Stop();
                mouse_timer.Stop();
                enemy_timer.Stop();
                pause_panel.BringToFront();
                ShowMap = false;
                shop_panel.Visible = false;
            }
            else
            {
                time_remein.Start();
                stamina_timer.Start();
                mouse_timer.Start();
                enemy_timer.Start();
            }
        }

        private void TakeFlashlight(bool change)
        {
            Player player = Controller.GetPlayer();
            if (player.CuteMode) return;
            if (player.Guns.Contains((Flashlight)player.GUNS[0]))
            {
                player.Guns.Remove((Flashlight)player.GUNS[0]);
                ChangeWeapon(player.PreviousGun);
            }
            else if (change)
            {
                player.Guns.Add((Flashlight)player.GUNS[0]);
                player.PreviousGun = player.CurrentGun;
                ChangeWeapon(player.Guns.IndexOf((Flashlight)player.GUNS[0]));
            }
        }

        private void Display_Scroll(object sender, MouseEventArgs e)
        {
            Player player = Controller.GetPlayer();
            if (GameStarted && !shot_timer.Enabled && !reload_timer.Enabled && !player.IsPetting)
            {
                int new_gun = player.CurrentGun;
                if (e.Delta > 0)
                    new_gun--;
                else
                    new_gun++;
                if (new_gun < 0)
                    new_gun = player.Guns.Count - 1;
                else if (new_gun > player.Guns.Count - 1)
                    new_gun = 0;
                TakeFlashlight(false);
                ChangeWeapon(new_gun);
            }
        }

        private void SLIL_Deactivate(object sender, EventArgs e)
        {
            if (!map_presed)
            {
                strafeDirection = Direction.STOP;
                playerDirection = Direction.STOP;
                playerMoveStyle = Direction.WALK;
                active = false;
            }
            map_presed = false;
        }

        private void ChangeWeapon(int new_gun)
        {
            Player player = Controller.GetPlayer();
            if ((new_gun != player.CurrentGun || player.LevelUpdated) && player.Guns[new_gun].HasIt)
            {
                if (MainMenu.sounds)
                    draw.Play(Volume);
                player.CurrentGun = new_gun;
                player.GunState = 0;
                player.Aiming = false;
                reload_timer.Interval = player.GetCurrentGun().RechargeTime;
                shot_timer.Interval = player.GetCurrentGun().FiringRate;
                if (player.GetCurrentGun() is Gnome)
                {
                    prev_ost = ost_index;
                    ChangeOst(this, 6);
                }
                else if (prev_ost != ost_index)
                {
                    if (player.CuteMode)
                    {
                        if (ost_index != 7)
                            ChangeOst(this, 7);
                    }
                    else ChangeOst(this, prev_ost);
                }
            }
        }

        private void Display_MouseDown(object sender, MouseEventArgs e)
        {
            Player player = Controller.GetPlayer();
            if (GameStarted && player.CanShoot && !reload_timer.Enabled && !shot_timer.Enabled)
            {
                if (player.GetCurrentGun().CanShoot && !player.IsPetting)
                {
                    if (e.Button == MouseButtons.Left)
                    {
                        if (player.GetCurrentGun().MaxAmmoCount >= 0 && player.GetCurrentGun().AmmoCount > 0)
                        {
                            if (player.GetCurrentGun() is SniperRifle && !player.Aiming)
                                return;
                            if (MainMenu.sounds)
                                SoundsDict[player.GetCurrentGun().GetType()][player.GetCurrentGun().GetLevel(), 0].Play(Volume);
                            player.GunState = 1;
                            player.Aiming = false;
                            player.CanShoot = false;
                            burst_shots = 0;
                            if (player.GetCurrentGun().FireType == FireTypes.Single)
                            {
                                BulletRayCasting();
                                if (player.Look - player.GetCurrentGun().Recoil > -360)
                                    player.Look -= player.GetCurrentGun().Recoil;
                                else
                                    player.Look = -360;
                            }
                            shot_timer.Start();
                        }
                        else if (player.GetCurrentGun().MaxAmmoCount > 0 && player.GetCurrentGun().AmmoCount == 0)
                        {
                            player.GunState = 2;
                            if (player.GetCurrentGun() is Pistol || player.GetCurrentGun() is Shotgun)
                                player.GunState = 3;
                            player.Aiming = false;
                            reload_timer.Start();
                            if (player.GetCurrentGun() is Shotgun && player.GetCurrentGun().Level != Levels.LV1)
                                return;
                            if (MainMenu.sounds)
                                SoundsDict[player.GetCurrentGun().GetType()][player.GetCurrentGun().GetLevel(), 1].Play(Volume);
                        }
                        else if (!(player.GetCurrentGun() is Pistol && player.GetCurrentGun().Level == Levels.LV1) &&
                            !(player.GetCurrentGun() is Shotgun && player.GetCurrentGun().Level == Levels.LV1) && MainMenu.sounds)
                            SoundsDict[player.GetCurrentGun().GetType()][player.GetCurrentGun().GetLevel(), 2].Play(Volume);
                    }
                    else if (e.Button == MouseButtons.Right)
                    {
                        if (player.GetCurrentGun().CanAiming)
                        {
                            if (MainMenu.sounds)
                                SoundsDict[player.GetCurrentGun().GetType()][player.GetCurrentGun().GetLevel(), 3].Play(Volume);
                            player.Aiming = !player.Aiming;
                            player.GunState = player.Aiming ? 3 : 0;
                        }
                    }
                }
            }
        }

        private int GetAccurateSide(double distance, double rayX, double rayY)
        {
            Player player = Controller.GetPlayer();
            double x1_1 = player.X, y1_1 = player.Y;
            double x2_1 = player.X + distance * rayX, y2_1 = player.Y + distance * rayY;
            int cellY = (int)y2_1;
            int cellX = (int)x2_1;
            double x1_2, y1_2, x2_2, y2_2;
            if (-rayY < 0)
            {
                x1_2 = cellX + 1; y1_2 = cellY; x2_2 = cellX; y2_2 = cellY;
            }
            else if (rayY < 0)
            {
                x1_2 = cellX; y1_2 = cellY + 1; x2_2 = cellX + 1; y2_2 = cellY + 1;
            }
            else if (-rayX < 0)
            {
                x1_2 = cellX; y1_2 = cellY; x2_2 = cellX; y2_2 = cellY + 1;
            }
            else
            {
                x1_2 = cellX + 1; y1_2 = cellY + 1; x2_2 = cellX + 1; y2_2 = cellY;
            }
            var intersectionPoint = SolveNotCanonical(x1_1, x2_1, y1_1, y2_1, x1_2, x2_2, y1_2, y2_2);
            if (intersectionPoint == null)
                return -1;
            double x = intersectionPoint.Value.x;
            double y = intersectionPoint.Value.y;
            if (-rayY < 0 && x >= x2_2 && x <= x1_2)
                return 0;
            else if (rayY < 0 && x >= x1_2 && x <= x2_2)
                return 1;
            else if (-rayX < 0 && y >= y1_2 && y <= y2_2)
                return 2;
            else if (y >= y2_2 && y <= y1_2)
                return 3;
            return -1;
        }

        private void BulletRayCasting()
        {
            DateTime time = DateTime.Now;
            elapsed_time = (time - total_time).TotalSeconds;
            total_time = time;
            PlayerMove();
            ClearDisplayedMap();
            Player player = Controller.GetPlayer();
            int factor = player.Aiming ? 12 : 0;
            if (player.GetCurrentGun() is Flashlight)
                factor = 8;
            scope_hit = null;
            double dirX = Math.Sin(player.A);
            double dirY = Math.Cos(player.A);
            double planeX = Math.Sin(player.A - Math.PI / 2) * Math.Tan(FOV / 2);
            double planeY = Math.Cos(player.A - Math.PI / 2) * Math.Tan(FOV / 2);
            double invDet = 1.0 / (planeX * dirY - dirX * planeY);
            double[] ZBuffer = new double[SCREEN_WIDTH[resolution]];
            double[] ZBufferWindow = new double[SCREEN_WIDTH[resolution]];
            Pixel[][] rays = CastRaysParallel(ZBuffer, ZBufferWindow);
            List<Entity> Entities = Controller.GetEntities();
            int[] spriteOrder = new int[Entities.Count];
            double[] spriteDistance = new double[Entities.Count];
            int[] textures = new int[Entities.Count];
            for (int i = 0; i < Entities.Count; i++)
            {
                spriteOrder[i] = i;
                spriteDistance[i] = (player.X - Entities[i].X) * (player.X - Entities[i].X) + (player.Y - Entities[i].Y) * (player.Y - Entities[i].Y);
                textures[i] = Entities[i].Texture;
            }
            SortSpritesNotReversed(ref spriteOrder, ref spriteDistance, ref textures, Entities.Count);
            for (int i = 0; i < Entities.Count; i++)
            {
                if (Entities[i] is Player)
                {
                    if (Controller.GetPlayer().ID == (Entities[i] as Player).ID) continue;
                }
                Entity entity = Entities[spriteOrder[i]];
                Creature creature = null;
                if (entity is Creature)
                {
                    creature = Entities[spriteOrder[i]] as Creature;
                    if (!creature.CanHit)
                        creature = null;
                }
                double spriteX = entity.X - player.X;
                double spriteY = entity.Y - player.Y;
                double transformX = invDet * (dirY * spriteX - dirX * spriteY);
                double transformY = invDet * (-planeY * spriteX + planeX * spriteY);
                int spriteScreenX = (int)((SCREEN_WIDTH[resolution] / 2) * (1 + transformX / transformY));
                double Distance = Math.Sqrt((player.X - entity.X) * (player.X - entity.X) + (player.Y - entity.Y) * (player.Y - entity.Y));
                double spriteTop = (SCREEN_HEIGHT[resolution] - player.Look) / 2 - (SCREEN_HEIGHT[resolution] * FOV) / Distance;
                double spriteBottom = SCREEN_HEIGHT[resolution] - (spriteTop + player.Look);
                int spriteCenterY = (int)((spriteTop + spriteBottom) / 2);
                int drawStartY = (int)spriteTop;
                int drawEndY = (int)spriteBottom;
                int spriteHeight = Math.Abs((int)(SCREEN_HEIGHT[resolution] / Distance));
                int spriteWidth = Math.Abs((int)(SCREEN_WIDTH[resolution] / Distance));
                int drawStartX = -spriteWidth / 2 + spriteScreenX;
                if (drawStartX < 0) drawStartX = 0;
                int drawEndX = spriteWidth / 2 + spriteScreenX;
                if (drawEndX >= SCREEN_WIDTH[resolution]) drawEndX = SCREEN_WIDTH[resolution];
                var timeNow = (long)((DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds * 2);
                for (int stripe = drawStartX; stripe < drawEndX; stripe++)
                {
                    int texWidth = 128;
                    double texX = (double)((256 * (stripe - (-spriteWidth / 2 + spriteScreenX)) * texWidth / spriteWidth) / 256) / texWidth;
                    if (transformY > 0 && stripe > 0 && stripe < SCREEN_WIDTH[resolution] && transformY < ZBuffer[stripe])
                    {
                        for (int y = drawStartY; y < drawEndY && y < SCREEN_HEIGHT[resolution]; y++)
                        {
                            if (y < 0 || (transformY > ZBufferWindow[stripe] && y > spriteCenterY))
                                continue;
                            double d = y - (SCREEN_HEIGHT[resolution] - (int)player.Look) / 2 + (drawEndY - drawStartY) / 2;
                            double texY = d / (drawEndY - drawStartY);
                            if (y == drawStartY) texY = 0;
                            if (rays[stripe].Length > y && y >= 0)
                            {
                                if (EnableAnimation)
                                {
                                    if (player.GetCurrentGun() is Flashlight && entity.RespondsToFlashlight)
                                        rays[stripe][y].TextureId = textures[i] + 2;
                                    else
                                        rays[stripe][y].TextureId = entity.Animations[0][timeNow % entity.Frames];
                                }
                                else
                                    rays[stripe][y].TextureId = textures[i];
                                rays[stripe][y].Blackout = 0;
                                rays[stripe][y].TextureX = texX;
                                rays[stripe][y].TextureY = texY;
                                Color color = GetColorForPixel(rays[stripe][y]);
                                if (color != Color.Transparent && stripe == SCREEN_WIDTH[resolution] / 2 && y == SCREEN_HEIGHT[resolution] / 2 && player.GetCurrentGun().FiringRange >= Distance)
                                {
                                    if (creature != null)
                                    {
                                        if (creature.DEAD)
                                            continue;
                                        double damage = (double)rand.Next((int)(player.GetCurrentGun().MinDamage * 100), (int)(player.GetCurrentGun().MaxDamage * 100)) / 100;
                                        if (player.GetCurrentGun() is Shotgun)
                                            damage *= player.GetCurrentGun().FiringRange - Distance;
                                        if (Controller.DealDamage(creature, damage))
                                        {
                                            if (MainMenu.sounds)
                                            {
                                                if (player.CuteMode)
                                                    CuteDeathSounds[creature.DeathSound, rand.Next(0, DeathSounds.GetLength(1))].Play(Volume);
                                                else
                                                    DeathSounds[creature.DeathSound, rand.Next(0, DeathSounds.GetLength(1))].Play(Volume);
                                            }
                                        }
                                        else if (difficulty == 0 && player.GetCurrentGun().FireType == FireTypes.Single && !(player.GetCurrentGun() is Knife))
                                            creature.UpdateCoordinates(MAP.ToString(), player.X, player.Y);
                                        if (!player.CuteMode) scope_hit = Properties.Resources.scope_hit;
                                        else scope_hit = Properties.Resources.scope_c_hit;
                                        return;
                                    }
                                    else 
                                        return;
                                }
                            }
                        }
                    }
                }
            }
            if (player.CuteMode) return;
            double rayA = player.A + FOV / 2 - (SCREEN_WIDTH[resolution] / 2) * FOV / SCREEN_WIDTH[resolution];
            double ray_x = Math.Sin(rayA);
            double ray_y = Math.Cos(rayA);
            double distance = 0;
            bool hit = false;
            scope_hit = null;
            while (raycast.Enabled && !hit && distance < player.GetCurrentGun().FiringRange)
            {
                distance += 0.01d;
                int test_x = (int)(player.X + ray_x * distance);
                int test_y = (int)(player.Y + ray_y * distance);
                if (test_x < 0 || test_x >= (player.DEPTH + factor) + player.X || test_y < 0 || test_y >= (player.DEPTH + factor) + player.Y)
                    hit = true;
                else
                {
                    char test_wall = Controller.GetMap()[test_y * Controller.GetMapWidth() + test_x];
                    double celling = (SCREEN_HEIGHT[resolution] - player.Look) / 2.25d - (SCREEN_HEIGHT[resolution] * FOV) / distance;
                    double floor = SCREEN_HEIGHT[resolution] - (celling + player.Look);
                    double mid = (celling + floor) / 2;
                    if (test_wall == '#' || test_wall == 'd' || test_wall == 'D' || (test_wall == '=' && SCREEN_HEIGHT[resolution] / 2 >= mid))
                    {
                        hit = true;
                        int side = GetAccurateSide(distance, ray_x, ray_y);
                        switch (side)
                        {
                            case 0:
                                Controller.AddHittingTheWall(player.X + ray_x * distance - 0.5, player.Y + ray_y * distance - 0.2 - 0.5);
                                break;
                            case 1:
                                Controller.AddHittingTheWall(player.X + ray_x * distance - 0.5, player.Y + ray_y * distance + 0.2 - 0.5);
                                break;
                            case 2:
                                Controller.AddHittingTheWall(player.X + ray_x * distance - 0.2 - 0.5, player.Y + ray_y * distance - 0.5);
                                break;
                            case 3:
                                Controller.AddHittingTheWall(player.X + ray_x * distance + 0.2 - 0.5, player.Y + ray_y * distance - 0.5);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }

        private void Reload_gun_Tick(object sender, EventArgs e)
        {
            try
            {
                scope_hit = null;
                if (GameStarted)
                {
                    int index = 1;
                    Player player = Controller.GetPlayer();
                    if (player.GetCurrentGun() is Shotgun && (player.GetCurrentGun().MaxAmmoCount == 0 || pressed_r))
                    {
                        if (player.GetCurrentGun().Level == Levels.LV1)
                            index = 2;
                        else
                        {
                            index = 3;
                            if (pressed_r)
                                index--;
                        }
                    }
                    if (!pressed_r && player.GetCurrentGun().AmmoCount > 0 && player.GetCurrentGun() is Shotgun)
                    {
                        player.GunState = player.MoveStyle;
                        player.CanShoot = true;
                        reload_timer.Stop();
                        reload_frames = 0;
                        return;
                    }
                    if (reload_frames >= player.GetCurrentGun().ReloadFrames - index)
                    {
                        player.GunState = player.MoveStyle;
                        pressed_r = false;
                        player.CanShoot = true;
                        player.GetCurrentGun().ReloadClip();
                        if (player.UseItem)
                            player.SetEffect();
                        reload_timer.Stop();
                        reload_frames = 0;
                        return;
                    }
                    else if (player.GetCurrentGun().ReloadFrames > 1)
                        player.GunState++;
                    reload_frames++;
                    if (player.GetCurrentGun() is Shotgun && MainMenu.sounds)
                        SoundsDict[player.GetCurrentGun().GetType()][player.GetCurrentGun().GetLevel(), 3].Play(Volume);
                }
                else
                {
                    reload_timer.Stop();
                    reload_frames = 0;
                }
            }
            catch { }
        }

        private void Shot_timer_Tick(object sender, EventArgs e)
        {
            try
            {
                Player player = Controller.GetPlayer();
                if (burst_shots >= player.GetCurrentGun().BurstShots)
                    shot_timer.Stop();
                else
                {
                    if (player.GetCurrentGun().FireType != FireTypes.Single)
                        player.GunState = player.GunState == 1 ? 0 : 1;
                    else
                        player.GunState = player.Aiming ? 3 : 0;
                    if (!(player.GetCurrentGun() is Knife))
                        player.GetCurrentGun().AmmoCount--;
                    if (player.GetCurrentGun().FireType != FireTypes.Single)
                    {
                        BulletRayCasting();
                        if (player.Look - player.GetCurrentGun().Recoil > -360)
                            player.Look -= player.GetCurrentGun().Recoil;
                        else
                            player.Look = -360;
                    }
                    if ((player.GetCurrentGun().AmmoCount <= 0 && player.GetCurrentGun().MaxAmmoCount > 0) || player.GetCurrentGun() is DisposableItem)
                    {
                        player.GunState = 2;
                        if (player.GetCurrentGun() is Pistol && player.GetCurrentGun().Level != Levels.LV4)
                            player.GunState = 3;
                        player.Aiming = false;
                        if (MainMenu.sounds)
                            SoundsDict[player.GetCurrentGun().GetType()][player.GetCurrentGun().GetLevel(), 1].Play(Volume);
                        reload_timer.Start();
                    }
                    else if (player.GetCurrentGun().AmmoCount <= 0)
                    {
                        player.Aiming = false;
                        player.CanShoot = true;
                        player.GunState = player.MoveStyle;
                        if (player.GetCurrentGun() is Pistol && player.GetCurrentGun().Level != Levels.LV4)
                            player.GunState = 4;
                        else if (player.GetCurrentGun() is Shotgun)
                        {
                            if (player.GetCurrentGun().Level == Levels.LV1 || player.GetCurrentGun().MaxAmmoCount == 0)
                                player.GunState = 2;
                            else
                                player.GunState = 3;
                            if (MainMenu.sounds)
                                SoundsDict[player.GetCurrentGun().GetType()][player.GetCurrentGun().GetLevel(), 1].Play(Volume);
                            reload_timer.Start();
                        }
                    }
                    else
                    {
                        if (player.GetCurrentGun().FireType == FireTypes.Single)
                            player.GunState = player.Aiming ? 3 : 0;
                        if (player.GetCurrentGun() is Shotgun && player.GetCurrentGun().Level != Levels.LV1)
                        {
                            player.GunState = 2;
                            if (MainMenu.sounds)
                                SoundsDict[player.GetCurrentGun().GetType()][player.GetCurrentGun().GetLevel(), 1].Play(Volume);
                            reload_timer.Start();
                        }
                        player.CanShoot = true;
                    }
                }
                burst_shots++;
                if (!shot_timer.Enabled || player.GetCurrentGun().FireType == FireTypes.Single)
                    scope_hit = null;
            }
            catch { }
        }

        private void Enemy_timer_Tick(object sender, EventArgs e)
        {
            
        }

        private void SLIL_LocationChanged(object sender, EventArgs e)
        {
            strafeDirection = Direction.STOP;
            playerDirection = Direction.STOP;
            playerMoveStyle = Direction.WALK;
        }

        private void Mouse_timer_Tick(object sender, EventArgs e)
        {
            Rectangle displayRectangle = new Rectangle
            {
                Location = display.PointToScreen(Point.Empty),
                Width = display.Width,
                Height = display.Height
            };
            Point cursorPosition = Cursor.Position;
            if (!displayRectangle.Contains(cursorPosition) && active)
                Cursor.Position = display.PointToScreen(new Point(display.Width / 2, display.Height / 2));
        }

        private void Stamina_timer_Tick(object sender, EventArgs e)
        {
            Player player = Controller.GetPlayer();
            //TODO:
            if (player == null) return;
            if (playerMoveStyle == Direction.RUN && playerDirection == Direction.FORWARD && !player.Aiming && !reload_timer.Enabled)
            {
                if (player.STAMINE <= 0)
                    playerMoveStyle = Direction.WALK;
                else
                {
                    if (player.GetCurrentGun() is Pistol || player.GetCurrentGun() is Flashlight || player.GetCurrentGun() is Knife)
                        player.STAMINE -= 2;
                    else
                        player.STAMINE -= 3;
                }
            }
            else
            {
                playerMoveStyle = Direction.WALK;
                if (player.STAMINE < player.MAX_STAMINE)
                    player.STAMINE += 2;
            }
        }

        private void Display_MouseMove(object sender, MouseEventArgs e)
        {
            if (GameStarted && active && !console_panel.Visible && !shop_panel.Visible)
            {
                double scale = 2.5;
                double x = display.Width / 2, y = display.Height / 2;
                double X = e.X - x, Y = e.Y - y;
                double size = 1;
                Controller.ChangePlayerA(-(((X / x) / 10) * (LOOK_SPEED * size)) * scale);
                //player.A -= (((X / x) / 10) * (LOOK_SPEED * size)) * scale;
                double LookDif = (((Y / y) * 30) * (LOOK_SPEED * size)) * scale;
                Controller.ChangePlayerLook(LookDif);
                Cursor.Position = display.PointToScreen(new Point((int)x, (int)y));
            }
        }

        private void PlayerMove()
        {
            Player player = Controller.GetPlayer();
            if (player == null) return;
            if (player.Aiming) return;
            if (Controller.GetMap()[(int)player.Y * Controller.GetMapWidth() + (int)player.X] == 'P')
            {
                Controller.GetMap()[(int)player.Y * Controller.GetMapWidth() + (int)player.X] = '.';
                DISPLAYED_MAP[(int)player.Y * Controller.GetMapWidth() + (int)player.X] = '.';
            }
            double run = 1;
            if (playerMoveStyle == Direction.RUN && playerDirection == Direction.FORWARD)
                run = player.RUN_SPEED;
            double move = player.MOVE_SPEED * run * elapsed_time;
            double moveSin = Math.Sin(player.A) * move;
            double moveCos = Math.Cos(player.A) * move;
            double strafeSin = moveSin / 1.4;
            double strafeCos = moveCos / 1.4;
            double newX = player.X;
            double newY = player.Y;
            double tempX = player.X;
            double tempY = player.Y;
            switch (strafeDirection)
            {
                case Direction.LEFT:
                    newX += strafeCos;
                    newY -= strafeSin;
                    break;
                case Direction.RIGHT:
                    newX -= strafeCos;
                    newY += strafeSin;
                    break;
            }
            switch (playerDirection)
            {
                case Direction.FORWARD:
                    newX += moveSin;
                    newY += moveCos;
                    break;
                case Direction.BACK:
                    newX -= moveSin * 0.675;
                    newY -= moveCos * 0.675;
                    break;
            }
            if (!(impassibleCells.Contains(Controller.GetMap()[(int)newY * Controller.GetMapWidth() + (int)(newX + playerWidth / 2)])
                || impassibleCells.Contains(Controller.GetMap()[(int)newY * Controller.GetMapWidth() + (int)(newX - playerWidth / 2)])))
                tempX = newX;
            if (!(impassibleCells.Contains(Controller.GetMap()[(int)(newY + playerWidth / 2) * Controller.GetMapWidth() + (int)newX])
                || impassibleCells.Contains(Controller.GetMap()[(int)(newY - playerWidth / 2) * Controller.GetMapWidth() + (int)newX])))
                tempY = newY;
            if (impassibleCells.Contains(Controller.GetMap()[(int)tempY * Controller.GetMapWidth() + (int)(tempX + playerWidth / 2)]))
                tempX -= playerWidth / 2 - (1 - tempX % 1);
            if (impassibleCells.Contains(Controller.GetMap()[(int)tempY * Controller.GetMapWidth() + (int)(tempX - playerWidth / 2)]))
                tempX += playerWidth / 2 - (tempX % 1);
            if (impassibleCells.Contains(Controller.GetMap()[(int)(tempY + playerWidth / 2) * Controller.GetMapWidth() + (int)tempX]))
                tempY -= playerWidth / 2 - (1 - tempY % 1);
            if (impassibleCells.Contains(Controller.GetMap()[(int)(tempY - playerWidth / 2) * Controller.GetMapWidth() + (int)tempX]))
                tempY += playerWidth / 2 - (tempY % 1);
            if (tempX - player.X != 0 || tempY - player.Y != 0)
                Controller.MovePlayer(tempX - player.X, tempY - player.Y);
            player.X = tempX;
            player.Y = tempY;
            if (Controller.GetMap()[(int)player.Y * Controller.GetMapWidth() + (int)player.X] == 'F')
            {
                GameOver(1);
                return;
            }
            if (Controller.GetMap()[(int)player.Y * Controller.GetMapWidth() + (int)player.X] == '.')
            {
                //MAP[(int)player.Y * Controller.GetMapWidth() + (int)player.X] = 'P';
                DISPLAYED_MAP.Replace('P', '.');
                DISPLAYED_MAP[(int)player.Y * Controller.GetMapWidth() + (int)player.X] = 'P';
            }
        }

        private void UpdateBitmap()
        {
            SCREEN?.Dispose();
            WEAPON?.Dispose();
            BUFFER?.Dispose();
            graphicsWeapon?.Dispose();
            SCREEN = new Bitmap(SCREEN_WIDTH[resolution], SCREEN_HEIGHT[resolution]);
            WEAPON = new Bitmap(SCREEN_WIDTH[resolution], SCREEN_HEIGHT[resolution]);
            BUFFER = new Bitmap(SCREEN_WIDTH[resolution], SCREEN_HEIGHT[resolution]);
            graphicsWeapon = Graphics.FromImage(WEAPON);
            graphicsWeapon.SmoothingMode = SmoothingMode.AntiAlias;
            display.ResizeImage(SCREEN_WIDTH[resolution], SCREEN_HEIGHT[resolution]);
            raycast.Interval = hight_fps ? 15 : 30;
        }

        private void SLIL_Load(object sender, EventArgs e)
        {

        }

        private void SLIL_FormClosing(object sender, FormClosingEventArgs e)
        {
            Player player = Controller.GetPlayer();
            Controller.CloseConnection();
            if (!CorrectExit)
            {
                e.Cancel = true;
                if (!Paused)
                    Pause();
                return;
            }
            raycast.Stop();
            time_remein.Stop();
            step_sound_timer.Stop();
            stamina_timer.Stop();
            mouse_timer.Stop();
            shot_timer.Stop();
            reload_timer.Stop();
            enemy_timer.Stop();
            status_refresh.Stop();
            if (!isCursorVisible)
                Cursor.Show();
            foreach (Control control in ShopInterface_panel.Controls)
            {
                if (control is SLIL_ShopInterface)
                {
                    SLIL_ShopInterface ShopInterface = control as SLIL_ShopInterface;
                    ShopInterface.cant_pressed?.Dispose();
                    ShopInterface.cant_pressed = null;
                }
            }
            player.Money = 15;
            ShowMap = false;
        }

        private void Restart_btn_Click(object sender, EventArgs e) => StartGame();

        //private void SLIL_Shown(object sender, EventArgs e) => StartGame();

        private void Respawn_timer_Tick(object sender, EventArgs e)
        {

        }

        private void Raycast_Tick(object sender, EventArgs e)
        {
            DateTime time = DateTime.Now;
            elapsed_time = (time - total_time).TotalSeconds;
            total_time = time;
            PlayerMove();
            ClearDisplayedMap();
            double[] ZBuffer = new double[SCREEN_WIDTH[resolution]];
            double[] ZBufferWindow = new double[SCREEN_WIDTH[resolution]];
            Pixel[][] rays = CastRaysParallel(ZBuffer, ZBufferWindow);
            List<int> enemiesCoords;
            DrawSprites(ref rays, ref ZBuffer, ref ZBufferWindow, out enemiesCoords);
            foreach(int i in enemiesCoords)
            {
                DISPLAYED_MAP[i] = 'E';
            }
            DrawRaysOnScreen(rays);
            DrawWeaponGraphics();
            UpdateDisplay();
            fps = CalculateFPS(elapsed_time);
        }

        private void DrawSprites(ref Pixel[][] rays, ref double[] ZBuffer, ref double[] ZBufferWindow, out List<int> enemiesCoords)
        {
            Player player = Controller.GetPlayer();
            enemiesCoords = new List<int>();
            if (player == null) return;
            int factor = player.Aiming ? 12 : 0;
            if (player.GetCurrentGun() is Flashlight)
                factor = 8;
            double dirX = Math.Sin(player.A);
            double dirY = Math.Cos(player.A);
            double planeX = Math.Sin(player.A - Math.PI / 2) * Math.Tan(FOV / 2);
            double planeY = Math.Cos(player.A - Math.PI / 2) * Math.Tan(FOV / 2);
            double invDet = 1.0 / (planeX * dirY - dirX * planeY);
            Entity[] Entities = new Entity[Controller.GetEntities().Count];
            Controller.GetEntities().CopyTo(Entities);
            int entityCount = Entities.Length;
            var spriteInfo = new (int Order, double Distance, int Texture)[entityCount];
            for (int i = 0; i < entityCount; i++)
            {
                double dx = player.X - Entities[i].X;
                double dy = player.Y - Entities[i].Y;
                spriteInfo[i] = (i, dx * dx + dy * dy, Entities[i].Texture);
            }
            Array.Sort(spriteInfo, (a, b) => b.Distance.CompareTo(a.Distance));
            for (int i = 0; i < Entities.Length; i++)
            {
                if (Entities[i] is Player) {
                    if (Controller.GetPlayer().ID == (Entities[i] as Player).ID) continue;
                }
                double Distance = Math.Sqrt((player.X - Entities[spriteInfo[i].Order].X) * (player.X - Entities[spriteInfo[i].Order].X) + (player.Y - Entities[spriteInfo[i].Order].Y) * (player.Y - Entities[spriteInfo[i].Order].Y));
                if (Distance > 22)
                    continue;
                if (Distance == 0) Distance = 0.01;
                double spriteX = Entities[spriteInfo[i].Order].X - player.X;
                double spriteY = Entities[spriteInfo[i].Order].Y - player.Y;
                double transformX = invDet * (dirY * spriteX - dirX * spriteY);
                double transformY = invDet * (-planeY * spriteX + planeX * spriteY);
                int spriteScreenX = (int)((SCREEN_WIDTH[resolution] / 2) * (1 + transformX / transformY));
                double spriteTop = (SCREEN_HEIGHT[resolution] - player.Look) / 2 - (SCREEN_HEIGHT[resolution] * FOV) / Distance;
                double spriteBottom = SCREEN_HEIGHT[resolution] - (spriteTop + player.Look);
                int spriteCenterY = (int)((spriteTop + spriteBottom) / 2);
                int drawStartY = (int)spriteTop;
                int drawEndY = (int)spriteBottom;
                int spriteHeight = Math.Abs((int)(SCREEN_HEIGHT[resolution] / Distance));
                int spriteWidth = Math.Abs((int)(SCREEN_WIDTH[resolution] / Distance));
                int drawStartX = -spriteWidth / 2 + spriteScreenX;
                if (drawStartX < 0) drawStartX = 0;
                int drawEndX = spriteWidth / 2 + spriteScreenX;
                if (drawEndX >= SCREEN_WIDTH[resolution]) drawEndX = SCREEN_WIDTH[resolution];
                var timeNow = (long)((DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds * 2);
                for (int stripe = drawStartX; stripe < drawEndX; stripe++)
                {
                    double texX = (double)((256 * (stripe - (-spriteWidth / 2 + spriteScreenX)) * texWidth / spriteWidth) / 256) / texWidth;
                    if (transformY > 0 && stripe >= 0 && stripe <= SCREEN_WIDTH[resolution] && transformY < ZBuffer[stripe])
                    {
                        for (int y = drawStartY; y < drawEndY && y < SCREEN_HEIGHT[resolution]; y++)
                        {
                            if (y < 0 || (transformY > ZBufferWindow[stripe] && y > spriteCenterY))
                                continue;
                            double d = y - (SCREEN_HEIGHT[resolution] - (int)player.Look) / 2 + (drawEndY - drawStartY) / 2;
                            double texY = d / (drawEndY - drawStartY);
                            if (y == drawStartY) texY = 0;
                            if (rays[stripe].Length > y && y >= 0)
                            {
                                int tempTextureId = rays[stripe][y].TextureId;
                                int tempBlackout = rays[stripe][y].Blackout;
                                double tempTextureX = rays[stripe][y].TextureX;
                                double tempTextureY = rays[stripe][y].TextureY;
                                if (Entities[spriteInfo[i].Order] is Creature)
                                {
                                    Creature creature = Entities[spriteInfo[i].Order] as Creature;
                                    if (!creature.DEAD)
                                    {
                                        if (EnableAnimation)
                                        {
                                            if (!(player.GetCurrentGun() is Flashlight && creature.RespondsToFlashlight) && creature is Pet && (creature as Pet).Stoped && (creature as Pet).HasStopAnimation)
                                                rays[stripe][y].TextureId = spriteInfo[i].Texture + 3;
                                            else
                                            {
                                                if (player.GetCurrentGun() is Flashlight && creature.RespondsToFlashlight)
                                                    rays[stripe][y].TextureId = spriteInfo[i].Texture + 2;
                                                else
                                                    rays[stripe][y].TextureId = creature.Animations[0][timeNow % creature.Frames];
                                            }
                                        }
                                        else
                                            rays[stripe][y].TextureId = spriteInfo[i].Texture;
                                        if (creature is Enemy)
                                            enemiesCoords.Add(Entities[spriteInfo[i].Order].IntY * Controller.GetMapWidth() + Entities[spriteInfo[i].Order].IntX);
                                    }
                                    else
                                    {
                                        if (creature.RespondsToFlashlight)
                                            rays[stripe][y].TextureId = spriteInfo[i].Texture + 3;
                                        else
                                            rays[stripe][y].TextureId = spriteInfo[i].Texture + 2;
                                    }
                                }
                                else if (Entities[spriteInfo[i].Order] is Player)
                                {
                                    Player playerTar = Entities[spriteInfo[i].Order] as Player;
                                    if (!playerTar.Dead)
                                    {
                                        if (EnableAnimation)
                                        {
                                            {
                                                    rays[stripe][y].TextureId = playerTar.Animations[0][timeNow % playerTar.Frames];
                                            }
                                        }
                                        else
                                            rays[stripe][y].TextureId = spriteInfo[i].Texture;
                                    }
                                    else
                                    {
                                        rays[stripe][y].TextureId = spriteInfo[i].Texture + 2;
                                    }
                                }
                                else
                                {
                                    if (Entities[spriteInfo[i].Order] is GameObject)
                                    {
                                        GameObject gameObject = Entities[spriteInfo[i].Order] as GameObject;
                                        if (gameObject.Animated && gameObject.Temporarily)
                                            rays[stripe][y].TextureId = gameObject.Animations[0][gameObject.CurrentFrame];
                                        else if (gameObject.Animated)
                                            rays[stripe][y].TextureId = gameObject.Animations[0][timeNow % gameObject.Frames];
                                        else
                                            rays[stripe][y].TextureId = spriteInfo[i].Texture;
                                    }
                                    else
                                        rays[stripe][y].TextureId = spriteInfo[i].Texture;
                                }
                                rays[stripe][y].Blackout = (int)(Math.Min(Math.Max(0, Math.Floor((Distance / (player.DEPTH + factor)) * 100)), 100));
                                rays[stripe][y].TextureX = texX;
                                rays[stripe][y].TextureY = texY;
                                Color color = GetColorForPixel(rays[stripe][y]);
                                if (color == Color.Transparent)
                                {
                                    rays[stripe][y].TextureId = tempTextureId;
                                    rays[stripe][y].Blackout = tempBlackout;
                                    rays[stripe][y].TextureX = tempTextureX;
                                    rays[stripe][y].TextureY = tempTextureY;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void SortSpritesNotReversed(ref int[] order, ref double[] dist, ref int[] text, int amount)
        {
            Tuple<double, int, int>[] spritess = new Tuple<double, int, int>[amount];
            for (int i = 0; i < amount; i++) spritess[i] = Tuple.Create(dist[i], order[i], text[i]);
            spritess = spritess.OrderBy(item => item.Item1).ToArray();
        }

        private void UpdateDisplay()
        {
            using (Graphics g = Graphics.FromImage(SCREEN))
                g.DrawImage(WEAPON, 0, 0, SCREEN.Width, SCREEN.Height);
            using (Graphics g = Graphics.FromImage(BUFFER))
            {
                g.Clear(Color.Black);
                g.DrawImage(SCREEN, 0, 0, SCREEN.Width, SCREEN.Height);
            }
            SharpDX.Direct2D1.Bitmap dxBitmap = ConvertBitmap.ToDX(SCREEN, display.renderTarget);
            display.SCREEN = dxBitmap;
            display.DrawImage();
            dxBitmap?.Dispose();
        }

        private Pixel[][] CastRaysParallel(double[] ZBuffer, double[] ZBufferWindow)
        {
            Pixel[][] rays = new Pixel[SCREEN_WIDTH[resolution]][];
            Player player = Controller.GetPlayer();
            if (player == null)
            {
                for(int i = 0; i< SCREEN_WIDTH[resolution]; i++)
                {
                    rays[i] = new Pixel[SCREEN_HEIGHT[resolution]];
                    for(int j = 0; j < SCREEN_HEIGHT[resolution]; j++)
                    {
                        rays[i][j] = new Pixel(i, j, 100, 1, 1, 0);
                    }
                }
                return rays;
            }
            int factor = player.Aiming ? 12 : 0;
            if (player.GetCurrentGun() is Flashlight)
                factor = 8;
            double playerLook = player.Look;
            double dirX = Math.Sin(player.A);
            double dirY = Math.Cos(player.A);
            double planeX = Math.Sin(player.A - Math.PI / 2) * Math.Tan(FOV / 2);
            double planeY = Math.Cos(player.A - Math.PI / 2) * Math.Tan(FOV / 2);
            int mapX = (int)(player.X);
            int mapY = (int)(player.Y);
            Parallel.For(0, SCREEN_WIDTH[resolution], x => rays[x] = CastRay(x, ZBuffer, ZBufferWindow, factor, playerLook, dirX, dirY, planeX, planeY, mapX, mapY));
            return rays;
        }

        private void DrawRaysOnScreen(Pixel[][] rays)
        {
            BitmapData data = SCREEN.LockBits(new Rectangle(0, 0, SCREEN.Width, SCREEN.Height), ImageLockMode.WriteOnly, SCREEN.PixelFormat);
            int bytesPerPixel = Bitmap.GetPixelFormatSize(SCREEN.PixelFormat) / 8;
            byte[] pixels = new byte[data.Height * data.Stride];
            Parallel.ForEach(rays, (list) =>
            {
                foreach (Pixel pixel in list)
                {
                    int i = (pixel.Y * data.Stride) + (pixel.X * bytesPerPixel);
                    Color color = GetColorForPixel(pixel);
                    pixels[i] = color.B;
                    pixels[i + 1] = color.G;
                    pixels[i + 2] = color.R;
                    if (bytesPerPixel == 4)
                        pixels[i + 3] = color.A;
                }
            });
            rays = null;
            Marshal.Copy(pixels, 0, data.Scan0, pixels.Length);
            SCREEN.UnlockBits(data);
        }

        private Color GetColorForPixel(Pixel pixel)
        {
            Player player = Controller.GetPlayer();
            bool cute = false;
            if (player != null) cute = player.CuteMode;
            int textureSize = 128;
            int x = 0, y = 0;
            if (pixel.TextureId >= 2)
            {
                x = (int)WrapTexture((int)(pixel.TextureX * textureSize), textureSize);
                y = (int)WrapTexture((int)(pixel.TextureY * textureSize), textureSize);
            }
            Color color = textureCache.GetTextureColor(pixel.TextureId, x, y, pixel.Blackout, cute);
            return color;
        }

        private double WrapTexture(double value, int textureSize)
        {
            value %= textureSize;
            if (value < 0)
                value += textureSize;
            return value;
        }

        private void ClearDisplayedMap()
        {
            Player player = Controller.GetPlayer();
            if (player == null)
            {
                for (int i = 0; i < DISPLAYED_MAP.Length; i++) DISPLAYED_MAP[i] = '.';
                return;
            }
            int radius = 30;
            for (int y = Math.Max(0, (int)player.Y - radius); y < Math.Min(Controller.GetMapHeight(), (int)player.Y + radius + 1); y++)
            {
                for (int x = Math.Max(0, (int)player.X - radius); x < Math.Min(Controller.GetMapWidth(), (int)player.X + radius + 1); x++)
                {
                    int index = y * Controller.GetMapWidth() + x;
                    if (DISPLAYED_MAP[index] == '*' || DISPLAYED_MAP[index] == 'E')
                        DISPLAYED_MAP[index] = '.';
                }
            }
        }

        private Bitmap DrawMiniMap()
        {
            Player player = Controller.GetPlayer();
            int FACTOR = resolution == 1 ? 2 : 1;
            const int MINI_MAP_SIZE = 25;
            const int BORDER_SIZE = 1;
            const int MINI_MAP_DRAW_SIZE = 37;
            const int PIXEL_SIZE = 2;
            int totalSize = (MINI_MAP_DRAW_SIZE + 2 * BORDER_SIZE) * FACTOR;
            Bitmap miniMap = new Bitmap(totalSize, totalSize);
            Color[,] miniMapArray = new Color[MINI_MAP_SIZE, MINI_MAP_SIZE];
            for (int y = 0; y < MINI_MAP_SIZE; y++)
            {
                for (int x = 0; x < MINI_MAP_SIZE; x++)
                {
                    int mapX = x - MINI_MAP_SIZE / 2 + (int)player.X + 3;
                    int mapY = y - MINI_MAP_SIZE / 2 + (int)player.Y + 3;
                    Color pixelColor;
                    if (mapX >= 0 && mapX < Controller.GetMapWidth() && mapY >= 0 && mapY < Controller.GetMapHeight())
                    {
                        char mapChar = DISPLAYED_MAP[mapY * Controller.GetMapWidth() + mapX];
                        pixelColor = GetColorForMapChar(mapChar);
                    }
                    else
                        pixelColor = Color.Black;
                    miniMapArray[x, y] = pixelColor;
                }
            }
            using (Graphics g = Graphics.FromImage(miniMap))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                using (SolidBrush borderBrush = new SolidBrush(Color.Green))
                    g.FillEllipse(borderBrush, 0, 0, totalSize, totalSize);
                using (GraphicsPath path = new GraphicsPath())
                {
                    path.AddEllipse(BORDER_SIZE * FACTOR, BORDER_SIZE * FACTOR, MINI_MAP_DRAW_SIZE * FACTOR, MINI_MAP_DRAW_SIZE * FACTOR);
                    g.SetClip(path);
                    for (int y = 0; y < MINI_MAP_SIZE; y++)
                    {
                        for (int x = 0; x < MINI_MAP_SIZE; x++)
                        {
                            using (SolidBrush pixelBrush = new SolidBrush(miniMapArray[x, y]))
                                g.FillRectangle(pixelBrush, (x * PIXEL_SIZE + BORDER_SIZE) * FACTOR, (y * PIXEL_SIZE + BORDER_SIZE) * FACTOR, PIXEL_SIZE * FACTOR, PIXEL_SIZE * FACTOR);
                        }
                    }
                    g.ResetClip();
                }
            }
            return miniMap;
        }

        private Color GetColorForMapChar(char mapChar)
        {
            switch (mapChar)
            {
                case '#': return Color.Blue;
                case '=': return Color.YellowGreen;
                case 'P': return Color.Red;
                case 'd':
                case 'o':
                case 'D':
                case 'O': return Color.FromArgb(255, 165, 0);
                case '$': return Color.Pink;
                case 'F': return Color.MediumVioletRed;
                case '*': return Color.FromArgb(255, 128, 128);
                case 'E': return Color.Cyan;
                default: return Color.Black;
            }
        }

        private Bitmap DrawMap()
        {
            BitmapData data = map.LockBits(new Rectangle(0, 0, map.Width, map.Height), ImageLockMode.WriteOnly, map.PixelFormat);
            int bytesPerPixel = Bitmap.GetPixelFormatSize(map.PixelFormat) / 8;
            byte[] pixels = new byte[data.Height * data.Stride];
            Color color;
            for (int y = 0; y < Controller.GetMapHeight(); y++)
            {
                for (int x = 0; x < Controller.GetMapWidth(); x++)
                {
                    int i = (y * data.Stride) + (x * bytesPerPixel);
                    char mapChar = DISPLAYED_MAP[y * Controller.GetMapWidth() + x];
                    if (mapChar == '#')
                        color = Color.Blue;
                    else if (mapChar == '=')
                        color = Color.YellowGreen;
                    else if (mapChar == 'D' || mapChar == 'O')
                        color = Color.FromArgb(255, 165, 0);
                    else if (mapChar == '$')
                        color = Color.Pink;
                    else if (mapChar == 'F')
                        color = Color.MediumVioletRed;
                    else if (mapChar == 'P')
                        color = Color.Red;
                    else if (mapChar == '*')
                        color = Color.FromArgb(255, 128, 128);
                    else if (mapChar == 'E')
                        color = Color.Cyan;
                    else
                        color = Color.Black;
                    pixels[i] = color.B;
                    pixels[i + 1] = color.G;
                    pixels[i + 2] = color.R;
                    if (bytesPerPixel == 4)
                        pixels[i + 3] = color.A;
                }
            }
            Marshal.Copy(pixels, 0, data.Scan0, pixels.Length);
            map.UnlockBits(data);
            return map;
        }

        private void DrawCircularProgressBar(Image effect_image, int icon_size, int index)
        {
            Player player = Controller.GetPlayer();
            if (player == null) return;
            int diameter = icon_size;
            int x = WEAPON.Width - icon_size - 4 - ((icon_size + 4) * index);
            int y = WEAPON.Height - icon_size - 4;
            RectangleF circleRect = new RectangleF(x, y, diameter, diameter);
            using (Pen pen = new Pen(Color.FromArgb(104, 213, 248), 3))
                graphicsWeapon.DrawEllipse(pen, circleRect);
            float sweepAngle = (float)player.Effects[index].EffectTimeRemaining / player.Effects[index].EffectTotalTime * 360;
            using (Pen pen = new Pen(Color.FromArgb(90, 131, 182), 3))
            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddArc(circleRect, -90, sweepAngle);
                graphicsWeapon.DrawPath(pen, path);
            }
            graphicsWeapon.DrawImage(effect_image, x, y, icon_size, icon_size);
        }

        private void DrawWeaponGraphics()
        {
            Player player = Controller.GetPlayer();
            if (player == null) return;
            if (ShowMap)
            {
                graphicsWeapon.Clear(Color.Black);
                graphicsWeapon.DrawImage(DrawMap(), 0, 0, SCREEN_WIDTH[resolution], SCREEN_HEIGHT[resolution]);
                return;
            }
            string space_0 = "0", space_1 = "0";
            if (seconds > 9)
                space_1 = "";
            if (minutes > 9)
                space_0 = "";
            int item_count = 0;
            if (player.DisposableItems.Count > 0)
                item_count = player.DisposableItems[player.SelectedItem].AmmoCount + player.DisposableItems[player.SelectedItem].MaxAmmoCount;
            int icon_size = resolution == 0 ? 14 : 28;
            graphicsWeapon.Clear(Color.Transparent);
            try
            {
                if (player.IsPetting) graphicsWeapon.DrawImage(Properties.Resources.pet_animation, 0, 0, WEAPON.Width, WEAPON.Height);
                else graphicsWeapon.DrawImage(ImagesDict[player.GetCurrentGun().GetType()][player.GetCurrentGun().GetLevel(), player.GunState], 0, 0, WEAPON.Width, WEAPON.Height);
            }
            catch
            {
                try
                {
                    if (player.IsPetting) graphicsWeapon.DrawImage(Properties.Resources.pet_animation, 0, 0, WEAPON.Width, WEAPON.Height);
                    else graphicsWeapon.DrawImage(ImagesDict[player.GetCurrentGun().GetType()][player.GetCurrentGun().GetLevel(), 0], 0, 0, WEAPON.Width, WEAPON.Height);
                }
                catch { }
            }
            if (ShowFPS)
                graphicsWeapon.DrawString($"FPS: {fps}", consolasFont[resolution], whiteBrush, SCREEN_WIDTH[resolution], 0, rightToLeft);
            graphicsWeapon.DrawString($"TIME LEFT: {space_0}{minutes}:{space_1}{seconds}", consolasFont[resolution], whiteBrush, 0, 0);
            if (!player.CuteMode)
            {
                graphicsWeapon.DrawImage(Properties.Resources.hp, 2, 108 + (110 * resolution), icon_size, icon_size);
                graphicsWeapon.DrawImage(ItemIconDict[player.DisposableItems[player.SelectedItem].GetType()], 2, 94 + (96 * resolution), icon_size, icon_size);
            }
            else
            {
                graphicsWeapon.DrawImage(Properties.Resources.food_hp, 2, 108 + (110 * resolution), icon_size, icon_size);
                graphicsWeapon.DrawImage(CuteItemIconDict[player.DisposableItems[player.SelectedItem].GetType()], 2, 94 + (96 * resolution), icon_size, icon_size);
            }
            graphicsWeapon.DrawImage(Properties.Resources.money, 2, 14 + (14 * resolution), icon_size, icon_size);
            graphicsWeapon.DrawString(player.Money.ToString(), consolasFont[resolution], whiteBrush, icon_size + 2, 14 + (14 * resolution));
            graphicsWeapon.DrawString(player.HP.ToString("0"), consolasFont[resolution], whiteBrush, icon_size + 2, 108 + (110 * resolution));
            graphicsWeapon.DrawString(item_count.ToString(), consolasFont[resolution], whiteBrush, icon_size + 2, 94 + (98 * resolution));
            if (!player.IsPetting && player.Guns.Count > 0 && player.GetCurrentGun().ShowAmmo)
            {
                if (player.GetCurrentGun().IsMagic || player.GetCurrentGun() is Rainblower)
                    graphicsWeapon.DrawString($"{player.GetCurrentGun().MaxAmmoCount + player.GetCurrentGun().AmmoCount}", consolasFont[resolution], whiteBrush, 52 + (42 * resolution), 108 + (110 * resolution));
                else
                    graphicsWeapon.DrawString($"{player.GetCurrentGun().MaxAmmoCount}/{player.GetCurrentGun().AmmoCount}", consolasFont[resolution], whiteBrush, 52 + (42 * resolution), 108 + (110 * resolution));
                if (player.GetCurrentGun().IsMagic)
                    graphicsWeapon.DrawImage(Properties.Resources.magic, 40 + (30 * resolution), 108 + (110 * resolution), icon_size, icon_size);
                else if(player.GetCurrentGun() is Rainblower)
                    graphicsWeapon.DrawImage(Properties.Resources.bubbles, 40 + (30 * resolution), 108 + (110 * resolution), icon_size, icon_size);
                else
                {
                    if (player.GetCurrentGun() is SniperRifle || player.GetCurrentGun() is AssaultRifle)
                        graphicsWeapon.DrawImage(Properties.Resources.rifle_bullet, 40 + (30 * resolution), 108 + (110 * resolution), icon_size, icon_size);
                    else if (player.GetCurrentGun() is Shotgun)
                        graphicsWeapon.DrawImage(Properties.Resources.shell, 40 + (30 * resolution), 108 + (110 * resolution), icon_size, icon_size);
                    else
                        graphicsWeapon.DrawImage(Properties.Resources.bullet, 40 + (30 * resolution), 108 + (110 * resolution), icon_size, icon_size);
                }
            }
            if (player.GetCurrentGun().ShowScope)
            {
                if (player.GetCurrentGun() is Shotgun)
                    graphicsWeapon.DrawImage(scope_shotgun[scope_type], WEAPON.Width / 4, WEAPON.Height / 4, WEAPON.Width / 2, WEAPON.Height / 2);
                else
                    graphicsWeapon.DrawImage(scope[scope_type], WEAPON.Width / 4, WEAPON.Height / 4, WEAPON.Width / 2, WEAPON.Height / 2);
            }
            if (scope_hit != null)
                graphicsWeapon.DrawImage(scope_hit, WEAPON.Width / 4, WEAPON.Height / 4, WEAPON.Width / 2, WEAPON.Height / 2);
            if (ShowMiniMap)
            {
                Bitmap mini_map = DrawMiniMap();
                int mini_map_top = ShowFPS ? 14 : 0;
                graphicsWeapon.DrawImage(mini_map, SCREEN_WIDTH[resolution] - mini_map.Width - 5, mini_map_top + (mini_map_top * resolution));
                mini_map.Dispose();
            }
            if (stage_timer.Enabled)
            {
                string text = $"STAGE: {player.Stage + 1}";
                if (inDebug == 1)
                    text = "STAGE: Debug";
                else if (inDebug == 2)
                    text = "STAGE: Debug Boss";
                else if (difficulty == 4)
                    text = "STAGE: Custom";
                SizeF textSize = graphicsWeapon.MeasureString(text, consolasFont[resolution + 1]);
                graphicsWeapon.DrawString(text, consolasFont[resolution + 1], whiteBrush, (WEAPON.Width - textSize.Width) / 2, 30 + (30 * resolution));
            }
            if (player.STAMINE < player.MAX_STAMINE)
                graphicsWeapon.DrawLine(new Pen(Color.Lime, 2), 0, SCREEN_HEIGHT[resolution], (int)(player.STAMINE / player.MAX_STAMINE * SCREEN_WIDTH[resolution]), SCREEN_HEIGHT[resolution]);
            if (player.Effects.Count > 0)
            {
                for (int i = 0; i < player.Effects.Count; i++)
                    DrawCircularProgressBar(player.Effects[i].Icon, icon_size, i);
            }
        }

        private Image GetScope(Image scope)
        {
            Bitmap bmp = new Bitmap(scope);
            Color color = GetScopeColor(scope_color);
            for (int x = 0; x < bmp.Width; x++)
            {
                for (int y = 0; y < bmp.Height; y++)
                {
                    Color current = bmp.GetPixel(x, y);
                    if (current.A != 0)
                        bmp.SetPixel(x, y, color);
                }
            }
            return bmp;
        }

        private Color GetScopeColor(int scope_color)
        {
            if (scope_color == 8)
            {
                int r = rand.Next(125, 200);
                int g = rand.Next(125, 200);
                int b = rand.Next(125, 200);
                return Color.FromArgb(r, g, b);
            }
            else
            {
                string[] col = { "Lime", "Red", "Yellow", "Blue", "Magenta", "Cyan", "Orange", "White" };
                return Color.FromName(col[scope_color]);
            }
        }

        private int CalculateFPS(double elapsedTime)
        {
            int fps = (int)(1.0 / elapsedTime);
            return fps < 0 ? 0 : fps;
        }

        private Pixel[] CastRay(int x, double[] ZBuffer, double[] ZBufferWindow, int factor, double playerLook, double dirX, double dirY, double planeX, double planeY, int mapX, int mapY)
        {
            Player player = Controller.GetPlayer();
            Pixel[] result = new Pixel[SCREEN_HEIGHT[resolution]];
            double cameraX = 2 * x / (double)SCREEN_WIDTH[resolution] - 1;
            double rayDirX = dirX + planeX * cameraX;
            double rayDirY = dirY + planeY * cameraX;
            double sideDistX;
            double sideDistY;
            double deltaDistX = (rayDirX == 0) ? 1e30 : Math.Abs(1 / rayDirX);
            double deltaDistY = (rayDirY == 0) ? 1e30 : Math.Abs(1 / rayDirY);
            int stepX;
            int stepY;
            int wallSide = -1;
            if (rayDirX < 0)
            {
                stepX = -1;
                sideDistX = (player.X - mapX) * deltaDistX;
            }
            else
            {
                stepX = 1;
                sideDistX = (mapX + 1.0 - player.X) * deltaDistX;
            }
            if (rayDirY < 0)
            {
                stepY = -1;
                sideDistY = (player.Y - mapY) * deltaDistY;
            }
            else
            {
                stepY = 1;
                sideDistY = (mapY + 1.0 - player.Y) * deltaDistY;
            }
            double distance = 0;
            double window_distance = 0;
            bool hit_wall = false;
            bool hit_window = false;
            bool hit_door = false;
            bool is_bound = false;
            bool is_window_bound = false;
            int windowSide = 0;
            while (raycast.Enabled && !hit_wall && !hit_door)
            {
                if (sideDistX < sideDistY)
                {
                    sideDistX += deltaDistX;
                    mapX += stepX;
                    wallSide = 0;
                    if (!hit_window)
                        windowSide = 0;
                }
                else
                {
                    sideDistY += deltaDistY;
                    mapY += stepY;
                    wallSide = 1;
                    if (!hit_window)
                        windowSide = 1;
                }
                if (wallSide == 0) distance = (sideDistX - deltaDistX);
                else distance = (sideDistY - deltaDistY);
                if (!hit_window)
                {
                    if (wallSide == 0) window_distance = (sideDistX - deltaDistX);
                    else window_distance = (sideDistY - deltaDistY);
                }
                if (mapX < 0 || mapX >= (player.DEPTH + factor) + player.X || mapY < 0 || mapY >= (player.DEPTH + factor) + player.Y || distance >= (player.DEPTH + factor))
                {
                    hit_wall = true;
                    distance = (player.DEPTH + factor);
                    continue;
                }
                char test_wall = Controller.GetMap()[mapY * Controller.GetMapWidth() + mapX];
                switch (test_wall)
                {
                    case '#':
                        hit_wall = true;
                        DISPLAYED_MAP[mapY * Controller.GetMapWidth() + mapX] = '#';
                        break;
                    case '=':
                        if (!hit_window) {
                            hit_window = true;
                            DISPLAYED_MAP[mapY * Controller.GetMapWidth() + mapX] = '=';
                        }
                        break;
                    case 'd':
                        hit_door = true;
                        DISPLAYED_MAP[mapY * Controller.GetMapWidth() + mapX] = 'D';
                        break;
                    case 'D':
                        DISPLAYED_MAP[mapY * Controller.GetMapWidth() + mapX] = 'D';
                        break;
                    case '$':
                        DISPLAYED_MAP[mapY * Controller.GetMapWidth() + mapX] = '$';
                        break;
                    case 'F':
                        DISPLAYED_MAP[mapY * Controller.GetMapWidth() + mapX] = 'F';
                        break;
                    case 'E':
                        DISPLAYED_MAP[mapY * Controller.GetMapWidth() + mapX] = 'E';
                        break;
                    case '.':
                    case '*':
                        DISPLAYED_MAP[mapY * Controller.GetMapWidth() + mapX] = '*';
                        break;
                }
            }
            //double perpWallDist = distance * cosDeltaA;
            double ceiling = (SCREEN_HEIGHT[resolution] - playerLook) / 2 - (SCREEN_HEIGHT[resolution] * FOV) / distance;
            double floor = SCREEN_HEIGHT[resolution] - (ceiling + playerLook);
            double mid = (ceiling + floor) / 2;
            bool get_texture = false, get_texture_window = false;
            int side = 0;
            double wallX = 0;
            if (wallSide == 1)
                wallX = player.X + distance * rayDirX;
            else if (wallSide == 0)
                wallX = player.Y + distance * rayDirY;
            wallX -= Math.Floor(wallX);
            double windowX = 0;
            if (windowSide == 1)
                windowX = player.X + window_distance * rayDirX;
            else if (windowSide == 0)
                windowX = player.Y + window_distance * rayDirY;
            windowX -= Math.Floor(windowX);
            if (wallX > 0.97 || wallX < 0.03) is_bound = true;
            if (windowX > 0.97 || windowX < 0.03) is_window_bound = true;
            for (int y = 0; y < SCREEN_HEIGHT[resolution]; y++)
            {
                if (!GameStarted)
                    break;
                int blackout = 0, textureId = 1;
                if (hit_window && y > mid)
                {
                    ceiling = (SCREEN_HEIGHT[resolution] - playerLook) / 2 - (SCREEN_HEIGHT[resolution] * FOV) / window_distance;
                    floor = SCREEN_HEIGHT[resolution] - (ceiling + playerLook);
                }
                else
                {
                    ceiling = (SCREEN_HEIGHT[resolution] - playerLook) / 2 - (SCREEN_HEIGHT[resolution] * FOV) / distance;
                    floor = SCREEN_HEIGHT[resolution] - (ceiling + playerLook);
                }
                if (y <= ceiling)
                {
                    textureId = 7;
                    double d = (y + playerLook / 2) / (SCREEN_HEIGHT[resolution] / 2);
                    blackout = (int)(Math.Min(Math.Max(0, d * 100), 100));
                }
                else if (y >= mid && y <= floor && hit_window)
                {
                    textureId = 2;
                    if (Math.Abs(y - mid) <= 6 / window_distance || is_window_bound)
                        textureId = 0;
                    blackout = (int)(Math.Min(Math.Max(0, Math.Floor((window_distance / (player.DEPTH + factor)) * 100)), 100));
                }
                else if ((y < mid || !hit_window) && y > ceiling && y < floor)
                {
                    textureId = 2;
                    if (hit_door)
                        textureId = 3;
                    if (is_bound)
                        textureId = 0;
                    blackout = (int)(Math.Min(Math.Max(0, Math.Floor((distance / (player.DEPTH + factor)) * 100)), 100));
                }
                else if (y >= floor)
                {
                    textureId = 6;
                    double d = 1 - (y - (SCREEN_HEIGHT[resolution] - playerLook) / 2) / (SCREEN_HEIGHT[resolution] / 2);
                    blackout = (int)(Math.Min(Math.Max(0, d * 100), 100));
                }
                result[y] = new Pixel(x, y, blackout, distance, ceiling - floor, textureId);
                if (y < ceiling)
                {
                    int p = y - (int)(SCREEN_HEIGHT[resolution] - playerLook) / 2;
                    double rowDistance = (double)SCREEN_HEIGHT[resolution] / p;
                    double floorX = player.X - rowDistance * rayDirX;
                    double floorY = player.Y - rowDistance * rayDirY;
                    if (floorX < 0) floorX = 0;
                    if (floorY < 0) floorY = 0;
                    result[y].TextureX = floorX % 1;
                    result[y].TextureY = floorY % 1;
                    result[y].Side = 0;
                }
                else if (y >= floor)
                {
                    int p = y - (int)(SCREEN_HEIGHT[resolution] - playerLook) / 2;
                    double rowDistance = (double)SCREEN_HEIGHT[resolution] / p;
                    double floorX = player.X + rowDistance * rayDirX;
                    double floorY = player.Y + rowDistance * rayDirY;
                    if (floorX < 0) floorX = 0;
                    if (floorY < 0) floorY = 0;
                    result[y].TextureX = floorX % 1;
                    result[y].TextureY = floorY % 1;
                    result[y].Side = 0;
                }
                else
                {
                    if (y >= mid && hit_window)
                    {
                        if (!get_texture_window)
                        {
                            get_texture_window = true;
                            side = windowSide;
                            wallX = windowX;
                            if (side == -1)
                                result[y].TextureId = 0;
                        }
                    }
                    else if (hit_door || hit_wall)
                    {
                        if (!get_texture)
                        {
                            get_texture = true;
                            side = wallSide;
                            if (side == -1)
                                result[y].TextureId = 0;
                        }
                    }
                    else
                    {
                        result[y].TextureId = 1;
                        result[y].TextureY = 0;
                        result[y].TextureX = 0;
                        result[y].Side = 0;
                    }
                    if (get_texture || get_texture_window)
                    {
                        result[y].TextureY = (double)(y - ceiling) / (double)(floor - ceiling);
                        result[y].TextureX = wallX;
                        result[y].Side = side;
                    }
                }
            }
            ZBuffer[x] = distance;
            ZBufferWindow[x] = window_distance;
            return result;
        }

        private (double x, double y)? SolveSLU(double a1, double b1, double c1, double a2, double b2, double c2)
        {
            if (a1 == 0 && a2 == 0)
                return null;
            if (a1 == 0)
                return ((c2 - b2 * (c1 / b1)) / a2, c1 / b1);
            double det = a1 * b2 - a2 * b1;
            if (Math.Abs(det) < 1e-10)
                return null;
            double y = (c2 * a1 - a2 * c1) / det;
            double x = (c1 - b1 * y) / a1;
            return (x, y);
        }

        private (double x, double y)? SolveNotCanonical(double x1, double x2, double y1, double y2, double x1_2, double x2_2, double y1_2, double y2_2)
        {
            double a1 = y1 - y2;
            double b1 = x2 - x1;
            double c1 = x1 * (y1 - y2) + y1 * (x2 - x1);
            double a2 = y1_2 - y2_2;
            double b2 = x2_2 - x1_2;
            double c2 = x1_2 * (y1_2 - y2_2) + y1_2 * (x2_2 - x1_2);
            return SolveSLU(a1, b1, c1, a2, b2, c2);
        }

        private int GetSide(double distance, double rayX, double rayY)
        {
            Player player = Controller.GetPlayer();
            double x1_1 = player.X, y1_1 = player.Y;
            double x2_1 = player.X + distance * rayX, y2_1 = player.Y + distance * rayY;
            int cellY = (int)y2_1;
            int cellX = (int)x2_1;
            double x1_2, y1_2, x2_2, y2_2;
            if (-rayY < 0)
            {
                x1_2 = cellX + 1; y1_2 = cellY; x2_2 = cellX; y2_2 = cellY;
            }
            else if (rayY < 0)
            {
                x1_2 = cellX; y1_2 = cellY + 1; x2_2 = cellX + 1; y2_2 = cellY + 1;
            }
            else if (-rayX < 0)
            {
                x1_2 = cellX; y1_2 = cellY; x2_2 = cellX; y2_2 = cellY + 1;
            }
            else
            {
                x1_2 = cellX + 1; y1_2 = cellY + 1; x2_2 = cellX + 1; y2_2 = cellY;
            }
            var intersectionPoint = SolveNotCanonical(x1_1, x2_1, y1_1, y2_1, x1_2, x2_2, y1_2, y2_2);
            if (intersectionPoint == null)
                return -1;
            double x = intersectionPoint.Value.x;
            double y = intersectionPoint.Value.y;
            if (-rayY < 0 && x >= x2_2 && x <= x1_2)
                return 0;
            else if (rayY < 0 && x >= x1_2 && x <= x2_2)
                return 0;
            else if (-rayX < 0 && y >= y1_2 && y <= y2_2)
                return 1;
            else if (y >= y2_2 && y <= y1_2)
                return 1;
            return -1;
        }

        private void InitMap()
        {
            Controller.InitMap();
            DISPLAYED_MAP.Clear(); 
            DISPLAYED_MAP.Append(Controller.GetMap());
        }

        private void ResetDefault(Player player)
        {
            map = null;
            display.SCREEN = null;
            scope[scope_type] = GetScope(scope[scope_type]);
            scope_shotgun[scope_type] = GetScope(scope_shotgun[scope_type]);
            display.Refresh();
            int x = display.PointToScreen(Point.Empty).X + (display.Width / 2);
            int y = display.PointToScreen(Point.Empty).Y + (display.Height / 2);
            Cursor.Position = new Point(x, y);
            seconds = 0;
            //if (!CUSTOM)
            //    player.X = player.Y = 1.5d;
            //else
            //{
            //    player.X = CUSTOM_X;
            //    player.Y = CUSTOM_Y;
            //}
            if (player.Guns.Count == 0)
            {
                player.Guns.Add(player.GUNS[1]);
                player.Guns.Add(player.GUNS[2]);
            }
            player.SetDefault();
            player.LevelUpdated = false;
            open_shop = false;
            strafeDirection = playerDirection = Direction.STOP;
            playerMoveStyle = Direction.WALK;
            if (difficulty == 0)
                enemy_count = 0.07;
            else if (difficulty == 1)
                enemy_count = 0.065;
            else if (difficulty == 2)
            {
                enemy_count = 0.055;
                if (player.Guns[1].Level == Levels.LV1)
                    player.Guns[1].LevelUpdate();
            }
            else if (difficulty == 3)
            {
                enemy_count = 0.045;
                if (player.Guns[1].Level == Levels.LV1)
                    player.Guns[1].LevelUpdate();
            }
            else if (difficulty == 4)
            {
                minutes = 9999;
                MazeHeight = CustomMazeHeight;
                MazeWidth = CustomMazeWidth;
                enemy_count = 0.06;
                MAX_SHOP_COUNT = 5;
            }
            else
            {
                if (inDebug == 1)
                {
                    player.X = 9;
                    player.Y = 9;
                    MazeHeight = 6;
                    MazeWidth = 6;
                }
                else if (inDebug == 2)
                {
                    player.X = 10.5;
                    player.Y = 19.5;
                    MazeHeight = 7;
                    MazeWidth = 7;
                }
                minutes = 9999;
            }
            if (difficulty != 4 && difficulty != 5)
            {
                if (player.Stage == 0)
                {
                    minutes = START_EASY;
                    MazeHeight = MazeWidth = 10;
                    MAX_SHOP_COUNT = 2;
                }
                else if (player.Stage == 1)
                {
                    minutes = START_NORMAL;
                    MazeHeight = MazeWidth = 15;
                    MAX_SHOP_COUNT = 4;
                }
                else if (player.Stage == 2)
                {
                    minutes = START_HARD;
                    MazeHeight = MazeWidth = 20;
                    MAX_SHOP_COUNT = 6;
                }
                else if (player.Stage == 3)
                {
                    minutes = START_VERY_HARD;
                    MazeHeight = MazeWidth = 25;
                    MAX_SHOP_COUNT = 8;
                }
                else
                {
                    minutes = START_VERY_HARD;
                    MazeHeight = MazeWidth = 25;
                    MAX_SHOP_COUNT = 8;
                }
            }
            MAP_WIDTH = MazeWidth * 3 + 1;
            MAP_HEIGHT = MazeHeight * 3 + 1;
            //map = new Bitmap(Controller.GetMapWidth(), Controller.GetMapHeight());
            map = new Bitmap(MAP_WIDTH, MAP_HEIGHT);
            if (MainMenu.sounds)
            {
                if (!player.CuteMode)
                {
                    prev_ost = rand.Next(ost.Length - 3);
                    ChangeOst(this, prev_ost);
                }
                else ChangeOst(this, 7);
            }
        }

        private void GetFirstAidKit() {
            Player player = Controller.GetPlayer();
            if (player == null) return;
            player.DisposableItems[0].AddItem();
        }

        private void StartGame()
        {
            Player player = Controller.GetPlayer();
            if (!MainMenu.Language)
            {
                shop_title.Text = "SHOP";
                weapon_shop_page.Text = "Weapons";
                pet_shop_page.Text = "Pets";
                consumables_shop_page.Text = "Other";
                pause_text.Text = "PAUSE";
                pause_btn.Text = "CONTINUE";
                exit_btn.Text = "EXIT";
            }
            for (int i = player.GUNS.Length - 1; i >= 0; i--)
            {
                if (player.GUNS[i].AddToShop)
                {
                    SLIL_ShopInterface ShopInterface = new SLIL_ShopInterface()
                    {
                        index = MainMenu.Language ? 0 : 1,
                        weapon = player.GUNS[i],
                        buy = buy,
                        player = player,
                        BackColor = shop_panel.BackColor,
                        Dock = DockStyle.Top
                    };
                    weapon_shop_page.Controls.Add(ShopInterface);
                }
            }
            for (int i = Controller.GetPets().Length - 1; i >= 0; i--)
            {
                SLIL_PetShopInterface ShopInterface = new SLIL_PetShopInterface()
                {
                    index = MainMenu.Language ? 0 : 1,
                    pet = Controller.GetPets()[i],
                    buy = buy,
                    player = player,
                    BackColor = shop_panel.BackColor,
                    Dock = DockStyle.Top
                };
                pet_shop_page.Controls.Add(ShopInterface);
            }
            for (int i = player.GUNS.Length - 1; i >= 0; i--)
            {
                if (player.GUNS[i] is Item && !(player.GUNS[i] is Flashlight))
                {
                    SLIL_ConsumablesShopInterface ShopInterface = new SLIL_ConsumablesShopInterface()
                    {
                        index = MainMenu.Language ? 0 : 1,
                        item = player.GUNS[i] as DisposableItem,
                        buy = buy,
                        player = player,
                        //GUNS = player.GUNS,
                        BackColor = shop_panel.BackColor,
                        Dock = DockStyle.Top
                    };
                    consumables_shop_page.Controls.Add(ShopInterface);
                }
            }
            console_panel = new ConsolePanel()
            {
                Dock = DockStyle.Fill,
                Visible = false,
                player = player,
                //GUNS = player.GUNS,
                Entities = Controller.GetEntities()
            };
            console_panel.Log("SLIL console *v1.2*\nType \"-help-\" for a list of commands...", false, false, Color.Lime);
            Controls.Add(console_panel);
            display = new Display() { Size = Size, Dock = DockStyle.Fill, TabStop = false };
            display.MouseDown += new MouseEventHandler(Display_MouseDown);
            display.MouseMove += new MouseEventHandler(Display_MouseMove);
            display.MouseWheel += new MouseEventHandler(Display_Scroll);
            Controls.Add(display);
            UpdateBitmap();
            Activate();
            ResetDefault(player);
            InitMap();
            try
            {
                if (Controller.GetMap()[(int)(player.Y + 2) * Controller.GetMapWidth() + (int)player.X] == '.')
                    player.A = 0;
                else if (Controller.GetMap()[(int)(player.Y - 2) * Controller.GetMapWidth() + (int)player.X] == '.')
                    player.A = 3;
                else if (Controller.GetMap()[(int)player.Y * Controller.GetMapWidth() + (int)(player.X + 2)] == '.')
                    player.A = 1;
                else if (Controller.GetMap()[(int)player.Y * Controller.GetMapWidth() + (int)(player.X - 2)] == '.')
                    player.A = 4;
            }
            catch
            {
                player.A = 0;
            }
            stage_timer.Stop();
            stage_timer.Start();
            raycast.Start();
            time_remein.Start();
            stamina_timer.Start();
            mouse_timer.Start();
            enemy_timer.Start();
            respawn_timer.Start();
            if (MainMenu.sounds)
                step_sound_timer.Start();
            GameStarted = true;
            game_over_panel.Visible = false;
        }

        private void ToDefault()
        {
            Player player = Controller.GetPlayer();
            player.Dead = true;
            player.SetDefault();
            shop_tab_control.Controls.Clear();
            shop_tab_control.Controls.Add(weapon_shop_page);
            shop_tab_control.Controls.Add(pet_shop_page);
            shop_tab_control.Controls.Add(consumables_shop_page);
            if (inDebug == 1)
            {
                inDebug = 0;
                difficulty = old_difficulty;
            }
            for (int i = 0; i < player.GUNS.Length; i++)
                player.GUNS[i].SetDefault();
        }

        private void GameOver(int win)
        {
            Player player = Controller.GetPlayer();
            ost[ost_index]?.Stop();
            raycast.Stop();
            shot_timer.Stop();
            reload_timer.Stop();
            time_remein.Stop();
            step_sound_timer.Stop();
            stamina_timer.Stop();
            mouse_timer.Stop();
            enemy_timer.Stop();
            respawn_timer.Stop();
            ShowMap = false;
            shop_panel.Visible = false;
            console_panel.Visible = false;
            display.SCREEN = null;
            display.Refresh();
            GameStarted = false;
            if (win == 1)
            {
                if (MainMenu.sounds)
                    tp.Play(Volume);
                if (difficulty != 4)
                    player.Stage++;
                if (!player.CuteMode)
                {
                    for (int i = 0; i < player.Guns.Count; i++)
                    {
                        if (player.Guns[i].MaxAmmoCount == 0)
                            player.Guns[i].MaxAmmoCount = player.Guns[i].CartridgesClip;
                    }
                }
                player.ChangeMoney(50 + (5 * player.EnemiesKilled));
                GetFirstAidKit();
                StartGame();
                //UpdatePet();
            }
            else if (win == 0)
            {
                ToDefault();
                game_over_panel.Visible = true;
                game_over_panel.BringToFront();
                if (MainMenu.sounds)
                    game_over.Play(Volume);
            }
            else
                ToDefault();
        }
    }
}