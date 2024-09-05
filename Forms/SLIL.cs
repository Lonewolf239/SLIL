using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
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
using System.Reflection;

namespace SLIL
{
    public delegate void StartGameDelegate();
    public delegate void StopGameDelegate(int win);
    public delegate void InitPlayerDelegate();
    public delegate void PlaySoundDelegate(PlaySound sound);
    public delegate void CloseFormDelegate();

    public partial class SLIL : Form
    {
        private readonly GameController Controller;
        private bool InSelectingMode = false, BlockCamera = false, CanUnblockCamera = false;
        private bool isCursorVisible = true;
        public int CustomMazeHeight, CustomMazeWidth;
        public bool CUSTOM = false, ShowFPS = true, ShowMiniMap = true;
        public bool inv_y = false, inv_x = false;
        public static int difficulty = 1;
        private int inDebug = 0;
        public static double LOOK_SPEED = 2.5;
        public StringBuilder CUSTOM_MAP = new StringBuilder();
        public int CUSTOM_X, CUSTOM_Y;
        private readonly Random rand;
        private const int texWidth = 128;
        private readonly int[] SCREEN_HEIGHT = { 128, 128 * 2 }, SCREEN_WIDTH = { 228, 228 * 2 };
        private int display_size = 0, center_x = 0, center_y = 0, cursor_x = 0, cursor_y = 0;
        private readonly int[,] DISPLAY_SIZE =
        {
            { 256, 128 },
            { 512, 256 },
            { 640, 480 },
            { 800, 600 },
            { 1024, 768 },
            { 1280, 720 },
        };
        public static int resolution = 0, smoothing = 1;
        private readonly SmoothingMode[] smoothingModes =
        {
            SmoothingMode.None,
            SmoothingMode.AntiAlias,
            SmoothingMode.HighQuality,
            SmoothingMode.HighSpeed
        };
        public static bool hight_fps = true;
        private const double FOV = Math.PI / 3;
        private double elapsed_time = 0;
        private static readonly StringBuilder DISPLAYED_MAP = new StringBuilder();
        private Bitmap SCREEN, WEAPON, BUFFER;
        private readonly Font[] consolasFont = { new Font("Consolas", 8F), new Font("Consolas", 16F), new Font("Consolas", 22F) };
        private readonly SolidBrush whiteBrush = new SolidBrush(Color.White);
        private readonly StringFormat rightToLeft = new StringFormat() { FormatFlags = StringFormatFlags.DirectionRightToLeft };
        private Graphics graphicsWeapon;
        private int fps;
        private double planeX, planeY, dirX, dirY, invDet;
        private enum Direction { STOP, FORWARD, BACK, LEFT, RIGHT, WALK, RUN };
        private Direction playerDirection = Direction.STOP, strafeDirection = Direction.STOP, playerMoveStyle = Direction.WALK;
        private DateTime total_time = DateTime.Now;
        private List<int> soundIndices = new List<int> { 0, 1, 2, 3, 4 };
        private int currentIndex = 0;
        private bool active = true;
        private bool Paused = false, RunKeyPressed = false;
        public static readonly Dictionary<Type, Image[]> IconDict = new Dictionary<Type, Image[]>
        {
            { typeof(Flashlight), new[] { Properties.Resources.missing } },
            { typeof(Knife), new[] { Properties.Resources.missing } },
            { typeof(Candy), new[] { Properties.Resources.missing } },
            { typeof(Rainblower), new[] { Properties.Resources.missing } },
            { typeof(Pistol), new[]
            {
                    /*LV1:*/ Properties.Resources.pistol_lv1_icon,
                    /*LV2:*/ Properties.Resources.pistol_lv2_icon,
                    /*LV3:*/ Properties.Resources.pistol_lv3_icon,
                    /*LV4:*/ Properties.Resources.pistol_lv4_icon
            } },
            { typeof(Shotgun), new[]
            {
                    /*LV1:*/ Properties.Resources.shotgun_lv1_icon,
                    /*LV2:*/ Properties.Resources.shotgun_lv2_icon,
                    /*LV3:*/ Properties.Resources.shotgun_lv3_icon,
            } },
            { typeof(SubmachineGun), new[]
            {
                    /*LV1:*/ Properties.Resources.smg_lv1_icon,
                    /*LV2:*/ Properties.Resources.smg_lv2_icon,
                    /*LV3:*/ Properties.Resources.smg_lv3_icon,
            } },
            { typeof(AssaultRifle), new[]
            {
                    /*LV1:*/ Properties.Resources.rifle_lv1_icon,
                    /*LV2:*/ Properties.Resources.rifle_lv2_icon,
                    /*LV3:*/ Properties.Resources.rifle_lv3_icon,
            } },
            { typeof(SniperRifle), new[]
            {
                    /*LV1:*/ Properties.Resources.sniper_lv1_icon,
                    /*LV2:*/ Properties.Resources.sniper_lv2_icon,
                    /*LV3:*/ Properties.Resources.sniper_lv3_icon,
            } },
            { typeof(Fingershot), new[] { Properties.Resources.missing } },
            { typeof(TSPitW), new[] { Properties.Resources.missing } },
            { typeof(Gnome), new[] { Properties.Resources.missing } },
            { typeof(FirstAidKit), new[]
            {
                Properties.Resources.medkit_icon,
                Properties.Resources.food_icon
            } },
            { typeof(Adrenalin), new[]
            {
                Properties.Resources.adrenalin_icon,
                Properties.Resources.adrenalin_icon
            } },
            { typeof(Helmet), new[]
            {
                Properties.Resources.helmet_icon,
                Properties.Resources.helmet_icon
            } },
            { typeof(RPG), new[] { Properties.Resources.rpg_icon } },
        };
        public static readonly Dictionary<Type, Image[,]> ImagesDict = new Dictionary<Type, Image[,]>
        {
            { typeof(Flashlight), new[,] { { Properties.Resources.flashlight, Properties.Resources.flashlight_run } } },
            { typeof(Knife), new[,] { { Properties.Resources.knife, Properties.Resources.knife_hit, Properties.Resources.knife_run } } },
            { typeof(Candy), new[,] { { Properties.Resources.candy, Properties.Resources.candy_shoot, Properties.Resources.candy_run } } },
            { typeof(Rainblower), new[,]
            {
                   { Properties.Resources.rainblower, Properties.Resources.rainblower_shoot, Properties.Resources.rainblower_run },
            } },
            { typeof(Pistol), new[,]
            {                    
                   /*LV1:*/ { Properties.Resources.pistol_lv1, Properties.Resources.pistol_lv1_shoot, Properties.Resources.pistol_lv1_reload, Properties.Resources.pistol_lv1_reload, Properties.Resources.pistol_lv1, Properties.Resources.pistol_lv1_run, Properties.Resources.pistol_lv1_run },
                   /*LV2:*/ { Properties.Resources.pistol_lv2, Properties.Resources.pistol_lv2_shoot, Properties.Resources.pistol_lv2_reload, Properties.Resources.pistol_lv2_reload_empty, Properties.Resources.pistol_lv2_empty, Properties.Resources.pistol_lv2_run, Properties.Resources.pistol_lv2_run_empty },
                   /*LV3:*/ { Properties.Resources.pistol_lv3, Properties.Resources.pistol_lv3_shoot, Properties.Resources.pistol_lv3_reload, Properties.Resources.pistol_lv3_reload_empty, Properties.Resources.pistol_lv3_empty, Properties.Resources.pistol_lv3_run, Properties.Resources.pistol_lv3_run_empty },
                   /*LV4:*/ { Properties.Resources.pistol_lv4, Properties.Resources.pistol_lv4_shoot, Properties.Resources.pistol_lv4_reload_0, Properties.Resources.pistol_lv4_reload_1, Properties.Resources.pistol_lv4, Properties.Resources.pistol_lv4_run, Properties.Resources.pistol_lv4_run },
            } },
            { typeof(Shotgun), new[,]
            {
                    /*LV1:*/ { Properties.Resources.shotgun_lv1, Properties.Resources.shotgun_lv1_shoot, Properties.Resources.shotgun_lv1_reload_0, Properties.Resources.shotgun_lv1_reload_1, Properties.Resources.shotgun_lv1_reload_1, Properties.Resources.shotgun_lv1_run },
                    /*LV2:*/ { Properties.Resources.shotgun_lv2, Properties.Resources.shotgun_lv2_shoot, Properties.Resources.shotgun_lv2_pump, Properties.Resources.shotgun_lv2_reload_0, Properties.Resources.shotgun_lv2_reload_1, Properties.Resources.shotgun_lv2_run },
                    /*LV3:*/ { Properties.Resources.shotgun_lv3, Properties.Resources.shotgun_lv3_shoot, Properties.Resources.shotgun_lv3_pump, Properties.Resources.shotgun_lv3_reload_0, Properties.Resources.shotgun_lv3_reload_1, Properties.Resources.shotgun_lv3_run },
            } },
            { typeof(SubmachineGun), new[,]
            {
                    /*LV1:*/ { Properties.Resources.smg_lv1, Properties.Resources.smg_lv1_shoot, Properties.Resources.smg_lv1_reload_0, Properties.Resources.smg_lv1_reload_1, Properties.Resources.smg_lv1_run },
                    /*LV2:*/ { Properties.Resources.smg_lv2, Properties.Resources.smg_lv2_shoot, Properties.Resources.smg_lv2_reload_0, Properties.Resources.smg_lv2_reload_1, Properties.Resources.smg_lv2_run },
                    /*LV3:*/ { Properties.Resources.smg_lv3, Properties.Resources.smg_lv3_shoot, Properties.Resources.smg_lv3_reload_0, Properties.Resources.smg_lv3_reload_1, Properties.Resources.smg_lv3_run }
            } },
            { typeof(AssaultRifle), new[,]
            {
                    /*LV1:*/ { Properties.Resources.rifle_lv1, Properties.Resources.rifle_lv1_shoot, Properties.Resources.rifle_lv1_reload_0, Properties.Resources.rifle_lv1_reload_1, Properties.Resources.rifle_lv1_run },
                    /*LV2:*/ { Properties.Resources.rifle_lv2, Properties.Resources.rifle_lv2_shoot, Properties.Resources.rifle_lv2_reload_0, Properties.Resources.rifle_lv2_reload_1, Properties.Resources.rifle_lv2_run },
                    /*LV3:*/ { Properties.Resources.rifle_lv3, Properties.Resources.rifle_lv3_shoot, Properties.Resources.rifle_lv3_reload_0, Properties.Resources.rifle_lv3_reload_1, Properties.Resources.rifle_lv3_run }
            } },
            { typeof(SniperRifle), new[,]
            {
                    /*LV1:*/ { Properties.Resources.sniper_lv1, Properties.Resources.sniper_lv1_shoot, Properties.Resources.sniper_lv1_reload_0, Properties.Resources.sniper_lv1_reload_1, Properties.Resources.sniper_lv1_reload_2, Properties.Resources.sniper_lv1_aim, Properties.Resources.sniper_lv1_run },
                    /*LV2:*/ { Properties.Resources.sniper_lv2, Properties.Resources.sniper_lv2_shoot, Properties.Resources.sniper_lv2_reload_0, Properties.Resources.sniper_lv2_reload_1, Properties.Resources.sniper_lv2_reload_2, Properties.Resources.sniper_lv2_aim, Properties.Resources.sniper_lv2_run },
                    /*LV3:*/ { Properties.Resources.sniper_lv3, Properties.Resources.sniper_lv3_shoot, Properties.Resources.sniper_lv3_reload_0, Properties.Resources.sniper_lv3_reload_1, Properties.Resources.missing, Properties.Resources.sniper_lv3_aim, Properties.Resources.missing }
            } },
            { typeof(Fingershot), new[,]
            {
                   { Properties.Resources.fingershot, Properties.Resources.fingershot_shoot, Properties.Resources.fingershot_reload_0, Properties.Resources.fingershot_reload_1, Properties.Resources.fingershot_run, Properties.Resources.fingershot_run }
            } },
            { typeof(TSPitW), new[,]
            {
                   { Properties.Resources.TSPitW, Properties.Resources.TSPitW_shoot, Properties.Resources.TSPitW_reload_0, Properties.Resources.TSPitW_reload_1, Properties.Resources.TSPitW_reload_2, Properties.Resources.TSPitW }
            } },
            { typeof(Gnome), new[,]
            {
                   { Properties.Resources.gnome, Properties.Resources.gnome_shoot,
                    Properties.Resources.gnome_reload_0, Properties.Resources.gnome_reload_1,
                    Properties.Resources.gnome_reload_2, Properties.Resources.gnome_reload_3,
                    Properties.Resources.gnome_run }
            } },
            { typeof(FirstAidKit), new[,]
            {
                   { Properties.Resources.medkit, Properties.Resources.medkit, Properties.Resources.medkit_using_0, Properties.Resources.medkit_using_1, Properties.Resources.medkit_using_2 },
                   { Properties.Resources.syringe, Properties.Resources.syringe, Properties.Resources.syringe_using_0, Properties.Resources.syringe_using_1, Properties.Resources.syringe_using_2 },
                   { Properties.Resources.hand, Properties.Resources.hand, Properties.Resources.hand_using_0, Properties.Resources.hand_using_1, Properties.Resources.hand_using_2 },
                   { Properties.Resources.food, Properties.Resources.food, Properties.Resources.food_using_0, Properties.Resources.food_using_1, Properties.Resources.food_using_2 },
            } },
            { typeof(Adrenalin), new[,]
            {
                   { Properties.Resources.adrenalin, Properties.Resources.adrenalin, Properties.Resources.adrenalin_using_0, Properties.Resources.adrenalin_using_1, Properties.Resources.adrenalin_using_2 },
            } },
            { typeof(Helmet), new[,]
            {
                   { Properties.Resources.helmet, Properties.Resources.helmet, Properties.Resources.helmet_using_0, Properties.Resources.helmet_using_1, Properties.Resources.helmet_using_2, Properties.Resources.helmet_using_3 }
            } },
            { typeof(RPG), new[,]
            {
                   { Properties.Resources.rpg, Properties.Resources.rpg_shooted, Properties.Resources.rpg_reload_0, Properties.Resources.rpg_reload_1, Properties.Resources.rpg_reload_2, Properties.Resources.rpg_empty }
            } },
        };
        public static readonly Dictionary<Type, PlaySound[,]> SoundsDict = new Dictionary<Type, PlaySound[,]>
        {
            { typeof(Flashlight), new[,] { { new PlaySound(null, false), } } },
            { typeof(Knife), new[,] { { new PlaySound(MainMenu.CGFReader.GetFile("knife.wav"), false) } } },
            { typeof(Candy), new[,] { { new PlaySound(MainMenu.CGFReader.GetFile("candy.wav"), false) } } },
            { typeof(Rainblower),  new[,]
            {
                   { new PlaySound(MainMenu.CGFReader.GetFile("rainblower.wav"), false), new PlaySound(null, false), new PlaySound(null, false) },
            } },
            { typeof(Pistol), new[,]
            {
                   /*LV1:*/ { new PlaySound(MainMenu.CGFReader.GetFile("pistol_lv1.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("pistol_reload.wav"), false), new PlaySound(null, false) },
                   /*LV2:*/ { new PlaySound(MainMenu.CGFReader.GetFile("pistol_lv2.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("pistol_reload.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("pistol_lv2_empty.wav"), false) },
                   /*LV3:*/ { new PlaySound(MainMenu.CGFReader.GetFile("pistol_lv3.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("pistol_reload.wav"), false), new PlaySound(null, false) },
                   /*LV4:*/ { new PlaySound(MainMenu.CGFReader.GetFile("pistol_lv4.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("pistol_lv4_reload.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("pistol_lv4_empty.wav"), false) },
            } },
            { typeof(Shotgun), new[,]
            {
                    /*LV1:*/ { new PlaySound(MainMenu.CGFReader.GetFile("shotgun.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("shotgun_lv1_reload.wav"), false), new PlaySound(null, false), new PlaySound(null, false) },
                    /*LV2:*/ { new PlaySound(MainMenu.CGFReader.GetFile("shotgun.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("shotgun_lv2_reload.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("shotgun_empty.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("shotgun_shell.wav"), false) },
                    /*LV3:*/ { new PlaySound(MainMenu.CGFReader.GetFile("shotgun.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("shotgun_lv3_reload.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("shotgun_empty.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("shotgun_shell.wav"), false) },
            } },
            { typeof(SubmachineGun), new[,]
            {
                    /*LV1:*/ { new PlaySound(MainMenu.CGFReader.GetFile("smg_lv1.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("smg_lv1_reload.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("smg_empty.wav"), false) },
                    /*LV2:*/ { new PlaySound(MainMenu.CGFReader.GetFile("smg_lv2.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("smg_lv2_reload.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("smg_empty.wav"), false) },
                    /*LV3:*/ { new PlaySound(MainMenu.CGFReader.GetFile("smg_lv3.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("smg_lv3_reload.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("smg_empty.wav"), false) }
            } },
            { typeof(AssaultRifle), new[,]
            {
                    /*LV1:*/ { new PlaySound(MainMenu.CGFReader.GetFile("rifle.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("rifle_reload.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("rifle_empty.wav"), false) },
                    /*LV2:*/ { new PlaySound(MainMenu.CGFReader.GetFile("rifle.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("rifle_reload.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("rifle_empty.wav"), false) },
                    /*LV3:*/ { new PlaySound(MainMenu.CGFReader.GetFile("rifle_lv3.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("rifle_lv3_reload.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("rifle_empty.wav"), false) }
            } },
            { typeof(SniperRifle), new[,]
            {
                    /*LV1:*/ { new PlaySound(MainMenu.CGFReader.GetFile("sniper_lv1.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("sniper_lv1_reload.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("sniper_lv1_empty.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("sniper_aiming.wav"), false) },
                    /*LV2:*/ { new PlaySound(MainMenu.CGFReader.GetFile("sniper_lv2.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("sniper_lv2_reload.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("sniper_lv2_empty.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("sniper_aiming.wav"), false) },
                    /*LV3:*/ { new PlaySound(MainMenu.CGFReader.GetFile("sniper_lv3.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("sniper_lv3_reload.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("sniper_lv3_empty.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("sniper_aiming.wav"), false) },
            } },
            { typeof(Fingershot), new[,]
            {
                   { new PlaySound(MainMenu.CGFReader.GetFile("fingershot.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("fingershot_reload.wav"), false), new PlaySound(null, false) }
            } },
            { typeof(TSPitW), new[,]
            {
                   { new PlaySound(MainMenu.CGFReader.GetFile("TSPitW.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("TSPitW_reload.wav"), false), new PlaySound(null, false) }
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
            { typeof(Helmet), new[,]
            {
                   { new PlaySound(null, false), new PlaySound(MainMenu.CGFReader.GetFile("helmet_using.wav"), false), new PlaySound(null, false) }
            } },
            { typeof(RPG), new[,]
            {
                   { new PlaySound(MainMenu.CGFReader.GetFile("rpg.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("rpg_reloading.wav"), false), new PlaySound(null, false) }
            } },
        };
        public static readonly Dictionary<Type, Image> ItemIconDict = new Dictionary<Type, Image>
        {
            { typeof(FirstAidKit), Properties.Resources.first_aid },
            { typeof(Adrenalin), Properties.Resources.adrenalin_count_icon },
            { typeof(Helmet), Properties.Resources.helmet_count_icon },
        };
        public static readonly Dictionary<Type, Image> CuteItemIconDict = new Dictionary<Type, Image>
        {
            { typeof(FirstAidKit), Properties.Resources.food_count },
            { typeof(Adrenalin), Properties.Resources.adrenalin_count_icon },
            { typeof(Helmet), Properties.Resources.helmet_count_icon },
        };
        public static readonly Dictionary<Type, Image> ShopImageDict = new Dictionary<Type, Image>
        {
            { typeof(SillyCat), Properties.Resources.pet_cat_icon },
            { typeof(GreenGnome), Properties.Resources.pet_gnome_icon },
            { typeof(EnergyDrink), Properties.Resources.pet_energy_drink_icon },
            { typeof(Pyro), Properties.Resources.pet_pyro_icon },
        };
        private readonly BindControls Bind;
        private readonly TextureCache textureCache;
        public static PlaySound hit = new PlaySound(MainMenu.CGFReader.GetFile("hit_player.wav"), false);
        public static PlaySound hungry = new PlaySound(MainMenu.CGFReader.GetFile("hungry_player.wav"), false);
        private PlaySound step;
        public static PlaySound[,] steps = new PlaySound[,]
            {
                {
                    new PlaySound(MainMenu.CGFReader.GetFile("step_0.wav"), false),
                    new PlaySound(MainMenu.CGFReader.GetFile("step_1.wav"), false),
                    new PlaySound(MainMenu.CGFReader.GetFile("step_2.wav"), false),
                    new PlaySound(MainMenu.CGFReader.GetFile("step_3.wav"), false),
                    new PlaySound(MainMenu.CGFReader.GetFile("step_4.wav"), false)
                },
                {
                    new PlaySound(MainMenu.CGFReader.GetFile("step_run_0.wav"), false),
                    new PlaySound(MainMenu.CGFReader.GetFile("step_run_1.wav"), false),
                    new PlaySound(MainMenu.CGFReader.GetFile("step_run_2.wav"), false),
                    new PlaySound(MainMenu.CGFReader.GetFile("step_run_3.wav"), false),
                    new PlaySound(MainMenu.CGFReader.GetFile("step_run_4.wav"), false)
                },
                {
                    new PlaySound(MainMenu.CGFReader.GetFile("step_c_0.wav"), false),
                    new PlaySound(MainMenu.CGFReader.GetFile("step_c_1.wav"), false),
                    new PlaySound(MainMenu.CGFReader.GetFile("step_c_2.wav"), false),
                    new PlaySound(MainMenu.CGFReader.GetFile("step_c_3.wav"), false),
                    new PlaySound(MainMenu.CGFReader.GetFile("step_c_4.wav"), false)
                },
                {
                    new PlaySound(MainMenu.CGFReader.GetFile("step_run_c_0.wav"), false),
                    new PlaySound(MainMenu.CGFReader.GetFile("step_run_c_1.wav"), false),
                    new PlaySound(MainMenu.CGFReader.GetFile("step_run_c_2.wav"), false),
                    new PlaySound(MainMenu.CGFReader.GetFile("step_run_c_3.wav"), false),
                    new PlaySound(MainMenu.CGFReader.GetFile("step_run_c_4.wav"), false)
                }
            };
        public static PlaySound[] ost = new PlaySound[]
        {
            new PlaySound(MainMenu.CGFReader.GetFile("slil_ost_0.wav"), true),
            new PlaySound(MainMenu.CGFReader.GetFile("slil_ost_1.wav"), true),
            new PlaySound(MainMenu.CGFReader.GetFile("slil_ost_2.wav"), true),
            new PlaySound(MainMenu.CGFReader.GetFile("slil_ost_3.wav"), true),
            new PlaySound(MainMenu.CGFReader.GetFile("slil_ost_4.wav"), true),
            new PlaySound(null, false),
            new PlaySound(MainMenu.CGFReader.GetFile("gnome.wav"), true),
            new PlaySound(MainMenu.CGFReader.GetFile("cmode_ost.wav"), true)
        };
        public PlaySound[,] DeathSounds;
        public PlaySound[,] CuteDeathSounds;
        public PlaySound game_over, draw, buy, wall, tp, screenshot;
        public static PlaySound[] door = { new PlaySound(MainMenu.CGFReader.GetFile("door_opened.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("door_closed.wav"), false) };
        private const string bossMap = @"#########################...............##F###.................####..##...........##..###...=...........=...###...=.....E.....=...###...................###...................###.........#.........###...##.........##...###....#.........#....###...................###..#...##.#.##...#..####.....#.....#.....######...............##############d####################...#################E=...=E#################...#################$D.P.D$#################...################################",
            debugMap = @"####################.................##..WWWW...........##..W..W..B..b..#..##..WE.W...........##..W..W...........##..W..W........d..##..W.EW...........##..WWWW...........##........P.....=..##..#b.............##..###............##..#B..........F..##.................##..WWW.B=.#D#..#..##..WEW====#$#.#d=.##..WWW.=b.###..=..##.................####################";
        public static float Volume = 0.4f;
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
        private bool open_shop = false, pressed_r = false, cancelReload = false, pressed_h = false;
        private Display display;
        private Bitmap map;
        private ConsolePanel console_panel;
        private readonly char[] impassibleCells  = { '#', 'D', '=', 'd' };
        private const double playerWidth = 0.4;
        private bool GameStarted = false, CorrectExit = false;
        private readonly Image[] ConnectionIcons =
        {
            Properties.Resources.excellent_connection,
            Properties.Resources.good_connection,
            Properties.Resources.bad_connection,
            Properties.Resources.no_connection
        };
        private readonly StartGameDelegate StartGameHandle;
        private readonly StopGameDelegate StopGameHandle;
        private readonly InitPlayerDelegate InitPlayerHandle;
        private readonly PlaySoundDelegate PlaySoundHandle;
        private readonly CloseFormDelegate CloseFormHandle;

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
        public void StopGameInvoker(int win)
        {
            if (this.InvokeRequired && this.IsHandleCreated)
            {
                this.BeginInvoke((MethodInvoker)delegate
                {
                        this.GameOver(win);
                });
            }
            else
            {
                this.GameOver(win);
            }
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

        public void PlaySoundInvoker(PlaySound sound)
        {
            if (this.InvokeRequired && this.IsHandleCreated)
            {
                this.BeginInvoke((MethodInvoker)delegate
                {
                    if (MainMenu.sounds)
                    {
                        sound.Play(Volume);
                    }
                });
            }
            else
            {
                if (MainMenu.sounds)
                {
                    sound.Play(Volume);
                }
            }
        }

        public void CloseFormInvoker()
        {
            if (this.InvokeRequired && this.IsHandleCreated)
            {
                this.BeginInvoke((MethodInvoker)delegate
                {
                    this.CorrectExit = true;
                    this.Close();
                });
            }
            else
            {
                this.CorrectExit = true;
                this.Close();
            }
        }

        public SLIL(TextureCache textures)
        {
            InitializeComponent();
            SetLocalization();
            StartGameHandle = StartGameInvokerSinglePlayer;
            StopGameHandle = StopGameInvoker;
            InitPlayerHandle = InitPlayerInvoker;
            PlaySoundHandle = PlaySoundInvoker;
            Controller = new GameController(StartGameHandle, InitPlayerHandle, StopGameHandle, PlaySoundHandle);
            Controller.SetCustom(CUSTOM, CustomMazeWidth, CustomMazeHeight, CUSTOM_MAP.ToString(), CUSTOM_X, CUSTOM_Y);
            rand = new Random();
            Bind = new BindControls(MainMenu.BindControls);
            SetParameters();
            textureCache = textures;
            Controller.StartGame();
        }
        public SLIL(TextureCache textures, bool custom, StringBuilder customMap, int mazeWidth, int mazeHeight, int customX, int customY)
        {
            InitializeComponent();
            SetLocalization();
            StartGameHandle = StartGameInvokerSinglePlayer;
            StopGameHandle = StopGameInvoker;
            InitPlayerHandle = InitPlayerInvoker;
            PlaySoundHandle = PlaySoundInvoker;
            Controller = new GameController(StartGameHandle, InitPlayerHandle, StopGameHandle, PlaySoundHandle);
            rand = new Random();
            Bind = new BindControls(MainMenu.BindControls);
            SetParameters();
            textureCache = textures;
            CUSTOM = custom;
            CUSTOM_MAP = customMap;
            CustomMazeWidth = mazeWidth;
            CustomMazeHeight = mazeHeight;
            CUSTOM_X = customX;
            CUSTOM_Y = customY;
            Controller.SetCustom(CUSTOM, CustomMazeWidth, CustomMazeHeight, CUSTOM_MAP.ToString(), CUSTOM_X, CUSTOM_Y);
            Controller.StartGame();
        }
        public SLIL(TextureCache textures, string adress, int port)
        {
            InitializeComponent();
            SetLocalization();
            StartGameHandle = StartGameInvokerMultiPlayer;
            StopGameHandle = StopGameInvoker;
            InitPlayerHandle = InitPlayerInvoker;
            PlaySoundHandle = PlaySoundInvoker;
            CloseFormHandle = CloseFormInvoker;
            Controller = new GameController(adress, port, StartGameHandle, InitPlayerHandle, StopGameHandle, PlaySoundHandle, CloseFormHandle);
            rand = new Random();
            Bind = new BindControls(MainMenu.BindControls);
            SetParameters();
            textureCache = textures;
        }

        private void SetLocalization()
        {
            if (!MainMenu.DownloadedLocalizationList)
            {
                shop_title.Text = "SHOP";
                weapon_shop_page.Text = "Weapons";
                pet_shop_page.Text = "Pets";
                consumables_shop_page.Text = "Other";
                pause_text.Text = "PAUSE";
                pause_btn.Text = "CONTINUE";
                exit_btn.Text = "EXIT";
            }
            else
            {
                shop_title.Text = MainMenu.Localizations.GetLString(MainMenu.Language, "2-1");
                weapon_shop_page.Text = MainMenu.Localizations.GetLString(MainMenu.Language, "2-2");
                pet_shop_page.Text = MainMenu.Localizations.GetLString(MainMenu.Language, "2-3");
                consumables_shop_page.Text = MainMenu.Localizations.GetLString(MainMenu.Language, "2-4");
                pause_text.Text = MainMenu.Localizations.GetLString(MainMenu.Language, "2-5");
                pause_btn.Text = MainMenu.Localizations.GetLString(MainMenu.Language, "2-6");
                exit_btn.Text = MainMenu.Localizations.GetLString(MainMenu.Language, "2-7");
            }
        }

        private void SetParameters()
        {
            difficulty = MainMenu.difficulty;
            resolution = MainMenu.resolution;
            display_size = MainMenu.display_size;
            smoothing = MainMenu.smoothing;
            scope_type = MainMenu.scope_type;
            scope_color = MainMenu.scope_color;
            hight_fps = MainMenu.hight_fps;
            ShowFPS = MainMenu.ShowFPS;
            ShowMiniMap = MainMenu.ShowMiniMap;
            LOOK_SPEED = MainMenu.LOOK_SPEED;
            inv_x = MainMenu.inv_x;
            inv_y = MainMenu.inv_y;
            Volume = MainMenu.Volume;
        }

        public void AddPet(int index)
        {
            foreach (SLIL_PetShopInterface control in pet_shop_page.Controls.Find("SLIL_PetShopInterface", true).Cast<SLIL_PetShopInterface>())
                control.buy_button.Text = MainMenu.DownloadedLocalizationList
                    ? $"{MainMenu.Localizations.GetLString(MainMenu.Language, "2-0")} ${control.pet.Cost}"
                    : $"Buy ${control.pet.Cost}";
            Controller.AddPet(index);
            CuteMode();
            Player player = Controller.GetPlayer();
            HideShop();
            if (player.CuteMode && shop_tab_control.Controls.ContainsKey("weapon_shop_page"))
            {
                shop_tab_control.Controls.Clear();
                shop_tab_control.Controls.Add(pet_shop_page);
                shop_tab_control.Controls.Add(consumables_shop_page);
            }
            else if (!shop_tab_control.Controls.ContainsKey("weapon_shop_page"))
            {
                shop_tab_control.Controls.Clear();
                shop_tab_control.Controls.Add(weapon_shop_page);
                shop_tab_control.Controls.Add(pet_shop_page);
                shop_tab_control.Controls.Add(consumables_shop_page);
            }
        }

        public int GetPetCost(int index)
        {
            int cost = 0;
            switch (index)
            {
                case 0:
                    SillyCat cat = new SillyCat(0, 0, 0, 0);
                    cost = cat.Cost;
                    break;
                case 1:
                    GreenGnome gnome = new GreenGnome(0, 0, 0, 0);
                    cost = gnome.Cost;
                    break;
                case 2:
                    EnergyDrink drink = new EnergyDrink(0, 0, 0, 0);
                    cost = drink.Cost;
                    break;
                case 3:
                    Pyro pyro = new Pyro(0, 0, 0, 0);
                    cost = pyro.Cost;
                    break;
            }
            return cost;
        }

        private void CuteMode()
        {
            Player player = Controller.GetPlayer();
            if (player.CuteMode && ost_index != 7)
            {
                prev_ost = ost_index;
                ChangeOst(7);
            }
            else if (ost_index == 7)
            {
                prev_ost = rand.Next(ost.Length - 3);
                ChangeOst(prev_ost);
            }
        }

        private void Chill_timer_Tick(object sender, EventArgs e) => chill_timer.Stop();

        private void Shop_panel_VisibleChanged(object sender, EventArgs e)
        {
            Player player = Controller.GetPlayer();
            if (player != null) player.Look = 0;
            shop_panel.BringToFront();
        }

        private void Stage_timer_Tick(object sender, EventArgs e) => stage_timer.Stop();

        public static void SetVolume() => ost[ost_index].SetVolume(Volume);

        public void ShowShop()
        {
            open_shop = true;
            ShopInterface_panel.VerticalScroll.Value = 0;
            mouse_timer.Stop();
            weapon_shop_page.Controls.Clear();
            pet_shop_page.Controls.Clear();
            consumables_shop_page.Controls.Clear();
            Player player = Controller.GetPlayer();
            if (!player.CuteMode)
            {
                for (int i = player.GUNS.Length - 1; i >= 0; i--)
                {
                    if (Controller.IsMultiplayer() && !player.GUNS[i].InMultiplayer)
                        continue;
                    if (player.GUNS[i].AddToShop)
                    {
                        SLIL_ShopInterface ShopInterface = new SLIL_ShopInterface()
                        {
                            ParentSLILForm = this,
                            index = MainMenu.DownloadedLocalizationList ? 0 : 1,
                            weapon = player.GUNS[i],
                            player = player,
                            BackColor = shop_panel.BackColor,
                            Dock = DockStyle.Top
                        };
                        weapon_shop_page.Controls.Add(ShopInterface);
                    }
                }
            }
            for (int i = Controller.GetPets().Length - 1; i >= 0; i--)
            {
                SLIL_PetShopInterface ShopInterface = new SLIL_PetShopInterface()
                {
                    index = MainMenu.DownloadedLocalizationList ? 0 : 1,
                    pet = Controller.GetPets()[i],
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
                        index = MainMenu.DownloadedLocalizationList ? 0 : 1,
                        item = player.GUNS[i] as DisposableItem,
                        player = player,
                        BackColor = shop_panel.BackColor,
                        Dock = DockStyle.Top
                    };
                    consumables_shop_page.Controls.Add(ShopInterface);
                }
            }
            shop_panel.BringToFront();
            shop_panel.Visible = true;
        }

        private void HideShop()
        {
            open_shop = false;
            mouse_timer.Start();
            int x = display.PointToScreen(Point.Empty).X + (display.Width / 2);
            int y = display.PointToScreen(Point.Empty).Y + (display.Height / 2);
            Cursor.Position = new Point(x, y);
            shop_panel.Visible = false;
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
            Controller.StopGame(-1);
            CorrectExit = true;
            Close();
        }

        public static void GoDebug(SLIL slil, int debug)
        {
            slil.Controller.StopGame(-1);
            slil.inDebug = debug;
            difficulty = 5;
            slil.Controller.GoDebug(debug);
            slil.StartGame();
        }

        public static void ChangeOst(int index)
        {
            if (!MainMenu.sounds)
                return;
            ost[ost_index]?.Stop();
            ost_index = index;
            ost[ost_index].LoopPlay(Volume);
        }

        private void Step_sound_timer_Tick(object sender, EventArgs e)
        {
            Player player = Controller.GetPlayer();
            if (player == null) return;
            if ((playerDirection != Direction.STOP || strafeDirection != Direction.STOP) && !player.Aiming && (step == null || !step.IsPlaying))
            {
                if (currentIndex >= soundIndices.Count)
                {
                    soundIndices = soundIndices.OrderBy(x => rand.Next()).ToList();
                    currentIndex = 0;
                }
                int i = playerMoveStyle == Direction.RUN || player.Fast ? 1 : 0;
                if (player.CuteMode) i += 2;
                int j = soundIndices[currentIndex];
                step = steps[i, j];
                step.PlayWithWait(Volume);
                currentIndex++;
            }
        }

        private void Status_refresh_Tick(object sender, EventArgs e)
        {
            if (display == null) return;
            if (!raycast.Enabled && display.SCREEN != null) display.SCREEN = null;
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
            //if (player.HP <= 0 && GameStarted) GameOver(0);
            try
            {
                if (!shot_timer.Enabled && !reload_timer.Enabled && !shotgun_pull_timer.Enabled && !pressed_h)
                {
                    if (player.DisposableItems.Count > 0 && player.Guns.Contains(player.DisposableItems[player.SelectedItem]))
                    {
                        ChangeWeapon(player.PreviousGun);
                        player.PreviousGun = player.CurrentGun;
                        player.Guns.Remove(player.DisposableItems[player.SelectedItem]);
                        if (player.DisposableItems[player.SelectedItem].AmmoCount <= 0 && player.DisposableItems[player.SelectedItem].AmmoInStock <= 0)
                            player.DisposableItems[player.SelectedItem].HasIt = false;
                    }
                }
                if (player.GetCurrentGun() is Flashlight)
                    shot_timer.Enabled = reload_timer.Enabled = shotgun_pull_timer.Enabled = false;
                if (!player.GetCurrentGun().CanRun)
                    playerMoveStyle = Direction.WALK;
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
                if (open_shop) HideShop();
                else if (console_panel.Visible)
                {
                    scope[scope_type] = GetScope(scope[scope_type]);
                    scope_shotgun[scope_type] = GetScope(scope_shotgun[scope_type]);
                    console_panel.Visible = false;
                    mouse_timer.Start();
                    console_panel.command_input.Text = null;
                    display.Focus();
                    int x = display.PointToScreen(Point.Empty).X + (display.Width / 2);
                    int y = display.PointToScreen(Point.Empty).Y + (display.Height / 2);
                    Cursor.Position = new Point(x, y);
                }
                else if (!GameStarted) Close();
                else Pause();
                return;
            }
            if (GameStarted && !Paused)
            {
                if (!console_panel.Visible && !open_shop)
                {
                    Player player = Controller.GetPlayer();
                    if (e.KeyCode == Bind.Run) RunKeyPressed = true;
                    if (e.KeyCode == Bind.Forward)
                        playerDirection = Direction.FORWARD;
                    if (e.KeyCode == Bind.Back)
                        playerDirection = Direction.BACK;
                    if (e.KeyCode == Bind.Left)
                        strafeDirection = Direction.LEFT;
                    if (e.KeyCode == Bind.Right)
                        strafeDirection = Direction.RIGHT;
                    if (!shot_timer.Enabled && !reload_timer.Enabled && !shotgun_pull_timer.Enabled && player != null)
                    {
                        int count = player.Guns.Count;
                        if (player.Guns.Contains(player.GUNS[0])) count--;
                        if (e.KeyCode == Bind.Reloading)
                        {
                            if (player.GetCurrentGun().AmmoCount != player.GetCurrentGun().CartridgesClip && player.GetCurrentGun().AmmoInStock > 0)
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
                                if (player.EffectCheck(player.GetEffectID())) return;
                                if (player.SelectedItem == 0 && player.HP == player.MAX_HP) return;
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
                            InSelectingMode = true;
                            int x = center_x - 14;
                            int y = center_y - 14;
                            if (player.SelectedItem == 0) x -= 100;
                            else if (player.SelectedItem == 1) y -= 28;
                            else if (player.SelectedItem == 2) x += 28;
                            else if (player.SelectedItem == 3) y += 28;
                            Cursor.Position = new Point(x, y);
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
                if (!Controller.IsMultiplayer() && e.KeyCode == Keys.Oemtilde && !open_shop && MainMenu.ConsoleEnabled)
                {
                    console_panel.Visible = !console_panel.Visible;
                    ShowMap = false;
                    if (console_panel.Visible)
                    {
                        mouse_timer.Stop();
                        console_panel.player = Controller.GetPlayer();
                        console_panel.command_input.Text = null;
                        console_panel.command_input.Focus();
                        console_panel.BringToFront();
                    }
                    else
                    {
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
                chill_timer.Start();
            }
            if (e.KeyCode == Bind.Run) RunKeyPressed = false;
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
                    ShowMap = !ShowMap;
                    Activate();
                }
                Player player = Controller.GetPlayer();
                if (player == null) return;
                if (!shot_timer.Enabled && !reload_timer.Enabled && !shotgun_pull_timer.Enabled && !player.IsPetting)
                {
                    if (e.KeyCode == Bind.Flashlight)
                        TakeFlashlight(true);
                    if (e.KeyCode == Bind.SelectItem)
                    {
                        BlockCamera = CanUnblockCamera = true;
                        InSelectingMode = false;
                        Cursor.Position = display.PointToScreen(new Point(center_x, center_y));
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
                                    Controller.InteractingWithDoors(y * Controller.GetMapWidth() + x);
                                    break;
                                case 'o':
                                    hit = true;
                                    if (distance < playerWidth || ((int)player.X == x && (int)player.Y == y)) break;
                                    Controller.InteractingWithDoors(y * Controller.GetMapWidth() + x);
                                    break;
                            }
                        }
                        if (hit) return;
                        DateTime time = DateTime.Now;
                        elapsed_time = (time - total_time).TotalSeconds;
                        total_time = time;
                        PlayerMove();
                        ClearDisplayedMap();
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
                                                if (player.GetCurrentGun() is Flashlight && entity.RespondsToFlashlight)
                                                    rays[stripe][y].TextureId = textures[i] + 2;
                                                else
                                                    rays[stripe][y].TextureId = entity.Animations[0][timeNow % entity.Frames];
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
                stamina_timer.Stop();
                mouse_timer.Stop();
                pause_panel.BringToFront();
                ShowMap = false;
                shop_panel.Visible = false;
            }
            else
            {
                stamina_timer.Start();
                mouse_timer.Start();
            }
            Controller.Pause(Paused);
        }

        private void TakeFlashlight(bool change)
        {
            if (Controller.IsMultiplayer()) return;
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
            if (GameStarted && !shot_timer.Enabled && !reload_timer.Enabled && !shotgun_pull_timer.Enabled && !player.IsPetting)
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
            strafeDirection = Direction.STOP;
            playerDirection = Direction.STOP;
            playerMoveStyle = Direction.WALK;
            RunKeyPressed = false;
            active = false;
        }

        private void ChangeWeapon(int new_gun)
        {
            Player player = Controller.GetPlayer();
            if (player == null) return;
            if ((new_gun != player.CurrentGun || player.LevelUpdated) && !InSelectingMode && player.Guns[new_gun].HasIt)
            {
                if (MainMenu.sounds) draw.Play(Volume);
                Controller.ChangeWeapon(new_gun);
                player.GunState = 0;
                player.Aiming = false;
                reload_timer.Interval = player.GetCurrentGun().RechargeTime;
                shot_timer.Interval = player.GetCurrentGun().FiringRate;
                if (player.GetCurrentGun() is Gnome)
                {
                    prev_ost = ost_index;
                    ChangeOst(6);
                }
                else if (prev_ost != ost_index)
                {
                    if (player.CuteMode)
                    {
                        if (ost_index != 7)
                            ChangeOst(7);
                    }
                    else ChangeOst(prev_ost);
                }
            }
        }

        private bool Shoot(Player player)
        {
            if (player.GetCurrentGun() is DisposableItem) return false;
                reload_timer.Interval = player.GetCurrentGun().RechargeTime;
            shot_timer.Interval = player.GetCurrentGun().FiringRate;
            mouse_hold_timer.Interval = player.GetCurrentGun().PauseBetweenShooting;
            if (player.GetCurrentGun() is Shotgun shotgun)
                shotgun_pull_timer.Interval = shotgun.PullTime;
            if (player.GetCurrentGun().AmmoInStock >= 0 && player.GetCurrentGun().AmmoCount > 0)
            {
                if (player.GetCurrentGun() is SniperRifle && !player.Aiming) return false;
                if (MainMenu.sounds)
                    SoundsDict[player.GetCurrentGun().GetType()][player.GetCurrentGun().GetLevel(), 0].Play(Volume);
                player.GunState = 1;
                player.Aiming = false;
                player.CanShoot = false;
                burst_shots = 0;
                if (player.GetCurrentGun().FireType == FireTypes.Single)
                {
                    BulletRayCasting();
                    if (player.Look - player.GetCurrentGun().RecoilY > -360)
                        player.Look -= player.GetCurrentGun().RecoilY;
                    else player.Look = -360;
                    Controller.ChangePlayerA(player.GetCurrentGun().GetRecoilX(rand.NextDouble()));
                }
                shot_timer.Start();
                return true;
            }
            else if (player.GetCurrentGun().AmmoInStock > 0 && player.GetCurrentGun().AmmoCount == 0)
            {
                player.GunState = 2;
                if (player.GetCurrentGun() is Pistol || player.GetCurrentGun() is Shotgun)
                    player.GunState = 3;
                player.Aiming = false;
                reload_timer.Start();
                if (player.GetCurrentGun() is Shotgun && player.GetCurrentGun().Level != Levels.LV1)
                    return false;
                if (MainMenu.sounds)
                    SoundsDict[player.GetCurrentGun().GetType()][player.GetCurrentGun().GetLevel(), 1].Play(Volume);
                return false;
            }
            else if (!(player.GetCurrentGun() is Pistol && player.GetCurrentGun().Level == Levels.LV1) &&
                !(player.GetCurrentGun() is Shotgun && player.GetCurrentGun().Level == Levels.LV1) && MainMenu.sounds)
            {
                SoundsDict[player.GetCurrentGun().GetType()][player.GetCurrentGun().GetLevel(), 2].Play(Volume);
                return false;
            }
            return false;
        }

        private void Display_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                mouse_hold_timer.Stop();
        }

        private void Display_MouseDown(object sender, MouseEventArgs e)
        {
            Player player = Controller.GetPlayer();
            if (GameStarted && !InSelectingMode && !player.IsPetting && !shotgun_pull_timer.Enabled && !shot_timer.Enabled)
            {
                if (e.Button == MouseButtons.Left)
                {
                    if (player.GetCurrentGun() is Shotgun && player.GetCurrentGun().Level != Levels.LV1 && reload_timer.Enabled)
                        cancelReload = true;
                    else if (!reload_timer.Enabled && !mouse_hold_timer.Enabled && player.CanShoot && player.GetCurrentGun().CanShoot)
                    {
                        if (Shoot(player))
                            mouse_hold_timer.Start();
                    }
                }
                else if (e.Button == MouseButtons.Right && !reload_timer.Enabled)
                {
                    if (player.GetCurrentGun().CanAiming && player.CanShoot && player.GetCurrentGun().CanShoot)
                    {
                        if (MainMenu.sounds)
                            SoundsDict[player.GetCurrentGun().GetType()][player.GetCurrentGun().GetLevel(), 3].Play(Volume);
                        player.Aiming = !player.Aiming;
                        player.GunState = player.Aiming ? player.GetCurrentGun().AimingState : 0;
                    }
                }
            }
        }

        private void BulletRayCasting()
        {
            scope_hit = null;
            Player player = Controller.GetPlayer();
            if (player.GetCurrentGun() is RPG) Controller.SpawnRockets(player.X, player.Y, 0, player.A);
            else
            {
                int[,] bullet = new int[player.GetCurrentGun().BulletCount, 2];
                int maxXOffset = (int)(34 * (1 - player.GetCurrentGun().Accuracy));
                int maxYOffset = (int)(15 * (1 - player.GetCurrentGun().Accuracy));
                if (player.GetCurrentGun().BulletCount == 1)
                    bullet = new int[,] { { center_x + rand.Next(-maxXOffset, maxXOffset), center_y + rand.Next(-maxYOffset, maxYOffset) } };
                else
                {
                    for (int i = 0; i < player.GetCurrentGun().BulletCount; i++)
                    {
                        bullet[i, 0] = center_x + rand.Next(-maxXOffset, maxXOffset);
                        bullet[i, 1] = center_y + rand.Next(-maxYOffset, maxYOffset);
                    }
                    if (player.GetCurrentGun() is SubmachineGun && player.GetCurrentGun().Level == Levels.LV3)
                    {
                        bullet[0, 0] -= 5;
                        bullet[1, 0] += 5;
                    }
                }
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
                    if (Entities[spriteOrder[i]] is Player)
                    {
                        if (Controller.GetPlayer().ID == (Entities[spriteOrder[i]] as Player).ID) continue;
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
                    double vMove = entity.VMove;
                    int vMoveScreen = (int)(vMove / transformY);
                    int spriteWidth = Math.Abs((int)(SCREEN_WIDTH[resolution] / Distance));
                    int drawStartX = -spriteWidth / 2 + spriteScreenX + vMoveScreen;
                    if (drawStartX < 0) drawStartX = 0;
                    int drawEndX = spriteWidth / 2 + spriteScreenX + vMoveScreen;
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
                                double d = (y - vMoveScreen) - (SCREEN_HEIGHT[resolution] - (int)player.Look) / 2 + (drawEndY - drawStartY) / 2;
                                double texY = d / (drawEndY - drawStartY);
                                if (y == drawStartY) texY = 0;
                                if (rays[stripe].Length > y && y >= 0)
                                {
                                    if (player.GetCurrentGun() is Flashlight && entity.RespondsToFlashlight)
                                        rays[stripe][y].TextureId = textures[i] + 2;
                                    else
                                        rays[stripe][y].TextureId = entity.Animations[0][timeNow % entity.Frames];
                                    rays[stripe][y].Blackout = 0;
                                    rays[stripe][y].TextureX = texX;
                                    rays[stripe][y].TextureY = texY;
                                    Color color = GetColorForPixel(rays[stripe][y]);
                                    for (int k = 0; k < bullet.GetLength(0); k++)
                                    {
                                        if (color != Color.Transparent && stripe == bullet[k, 0] && y == bullet[k, 1] && player.GetCurrentGun().FiringRange >= Distance)
                                        {
                                            if (creature != null)
                                            {
                                                if (creature.DEAD) continue;
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
                                                    creature.UpdateCoordinates(Controller.GetMap().ToString(), player.X, player.Y);
                                                if (!player.CuteMode) scope_hit = Properties.Resources.scope_hit;
                                                else scope_hit = Properties.Resources.scope_c_hit;
                                                bullet[k, 0] = -1;
                                                bullet[k, 1] = -1;
                                            }
                                            else if (entity is Player targetPlayer && entity.ID != player.ID)
                                            {
                                                if (targetPlayer.Dead) continue;
                                                double damage = (double)rand.Next((int)(player.GetCurrentGun().MinDamage * 100), (int)(player.GetCurrentGun().MaxDamage * 100)) / 100;
                                                if (player.GetCurrentGun() is Shotgun)
                                                    damage *= player.GetCurrentGun().FiringRange - Distance;
                                                if (Controller.DealDamage(targetPlayer, damage * 5))
                                                {
                                                    if (MainMenu.sounds)
                                                    {
                                                        if (player.CuteMode)
                                                            CuteDeathSounds[targetPlayer.DeathSound, rand.Next(0, DeathSounds.GetLength(1))].Play(Volume);
                                                        else
                                                            DeathSounds[targetPlayer.DeathSound, rand.Next(0, DeathSounds.GetLength(1))].Play(Volume);
                                                    }
                                                }
                                                if (!player.CuteMode) scope_hit = Properties.Resources.scope_hit;
                                                else scope_hit = Properties.Resources.scope_c_hit;
                                                bullet[k, 0] = -1;
                                                bullet[k, 1] = -1;
                                            }
                                            else
                                            {
                                                bullet[k, 0] = -1;
                                                bullet[k, 1] = -1;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                if (player.CuteMode) return;
                for (int k = 0; k < bullet.GetLength(0); k++)
                {
                    if (bullet[k, 0] == -1) continue;
                    double deltaA = FOV / 2 - bullet[k, 0] * FOV / SCREEN_WIDTH[resolution];
                    double rayA = player.A + deltaA;
                    double ray_x = Math.Sin(rayA);
                    double ray_y = Math.Cos(rayA);
                    double distance = 0;
                    bool hit = false;
                    while (raycast.Enabled && !hit && distance < player.GetCurrentGun().FiringRange)
                    {
                        distance += 0.01d;
                        int test_x = (int)(player.X + ray_x * distance);
                        int test_y = (int)(player.Y + ray_y * distance);
                        if (test_x < 0 || test_x >= (player.GetDrawDistance()) + player.X || test_y < 0 || test_y >= (player.GetDrawDistance()) + player.Y) hit = true;
                        else
                        {
                            char test_wall = Controller.GetMap()[test_y * Controller.GetMapWidth() + test_x];
                            double celling = (SCREEN_HEIGHT[resolution] - player.Look) / 2.25d - (SCREEN_HEIGHT[resolution] * FOV) / distance;
                            double floor = SCREEN_HEIGHT[resolution] - (celling + player.Look);
                            double mid = (celling + floor) / 2;
                            if (test_wall == '#' || test_wall == 'd' || test_wall == 'D' || (test_wall == '=' && SCREEN_HEIGHT[resolution] / 2 >= mid))
                            {
                                hit = true;
                                distance -= 0.2;
                                if (bullet[k, 1] > floor || bullet[k, 1] < celling) continue;
                                double vMove = player.Look / 2.25d + player.Look * FOV / (distance + 0.2) + SCREEN_HEIGHT[resolution] / 2 - bullet[k, 1];
                                Controller.AddHittingTheWall(player.X + ray_x * distance, player.Y + ray_y * distance, vMove);
                            }
                        }
                    }
                }
            }
        }

        private void Shotgun_pull_timer_Tick(object sender, EventArgs e)
        {
            Player player = Controller.GetPlayer();
            player.GunState = player.MoveStyle;
            player.CanShoot = true;
            shotgun_pull_timer.Stop();
            reload_frames = 0;
            if (player.GetCurrentGun().AmmoCount == 0)
                reload_timer.Start();
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
                    if (player.GetCurrentGun().AmmoCount == 0 && player.GetCurrentGun().AmmoInStock == 0 && !(player.GetCurrentGun() is DisposableItem)) reload_timer.Stop();
                    if (player.GetCurrentGun() is Shotgun && (player.GetCurrentGun().AmmoInStock == 0 || pressed_r))
                    {
                        if (player.GetCurrentGun().Level == Levels.LV1) index = 2;
                        else
                        {
                            index = 3;
                            if (pressed_r) index--;
                        }
                    }
                    if (reload_frames >= player.GetCurrentGun().ReloadFrames - index)
                    {
                        if (player.GetCurrentGun() is Shotgun && player.GetCurrentGun().Level != Levels.LV1)
                        {
                            if (player.GetCurrentGun().AmmoInStock > 0)
                            {
                                player.GunState = 3;
                                reload_frames = pressed_r ? -1 : 0;
                                Controller.ReloadClip();
                            }
                            if (cancelReload || player.GetCurrentGun().AmmoInStock == 0 || player.GetCurrentGun().AmmoCount == player.GetCurrentGun().CartridgesClip)
                            {
                                cancelReload = false;
                                pressed_r = false;
                                player.CanShoot = true;
                                reload_timer.Stop();
                                reload_frames = 0;
                                return;
                            }
                        }
                        else
                        {
                            player.GunState = player.MoveStyle;
                            pressed_r = false;
                            player.CanShoot = true;
                            Controller.ReloadClip();
                            if (player.UseItem) player.SetEffect();
                            reload_timer.Stop();
                            reload_frames = 0;
                            return;
                        }
                    }
                    else if (player.GetCurrentGun().ReloadFrames > 1) player.GunState++;
                    reload_frames++;
                    if (player.GetCurrentGun() is Shotgun && MainMenu.sounds)
                        SoundsDict[player.GetCurrentGun().GetType()][player.GetCurrentGun().GetLevel(), 3].Play(Volume);
                }
                else
                {
                    cancelReload = false;
                    pressed_r = false;
                    reload_frames = 0;
                    reload_timer.Stop();
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
                        Controller.AmmoCountDecrease();
                    if (player.GetCurrentGun().CanShoot && player.GetCurrentGun().FireType != FireTypes.Single)
                    {
                        BulletRayCasting();
                        if (player.Look - player.GetCurrentGun().RecoilY > -360)
                            player.Look -= player.GetCurrentGun().RecoilY;
                        else player.Look = -360;
                        player.A += player.GetCurrentGun().GetRecoilX(rand.NextDouble());
                    }
                    if (player.GetCurrentGun() is DisposableItem || (player.GetCurrentGun().AmmoCount <= 0 && player.GetCurrentGun().AmmoInStock > 0))
                    {
                        player.GunState = 2;
                        if (player.GetCurrentGun() is Pistol && player.GetCurrentGun().Level != Levels.LV4)
                            player.GunState = 3;
                        player.Aiming = false;
                        if (MainMenu.sounds)
                            SoundsDict[player.GetCurrentGun().GetType()][player.GetCurrentGun().GetLevel(), 1].Play(Volume);
                        shot_timer.Stop();
                        reload_timer.Start();
                    }
                    else if (player.GetCurrentGun().AmmoCount <= 0)
                    {
                        player.Aiming = false;
                        player.CanShoot = true;
                        player.GunState = player.MoveStyle;
                        if (player.GetCurrentGun() is Pistol && player.GetCurrentGun().Level != Levels.LV4)
                            player.GunState = 4;
                        if (player.GetCurrentGun() is RPG)
                            player.GunState = 5;
                        else if (player.GetCurrentGun() is Shotgun)
                        {
                            if (player.GetCurrentGun().Level == Levels.LV1 || player.GetCurrentGun().AmmoInStock == 0)
                                player.GunState = 2;
                            else
                                player.GunState = 3;
                            if (MainMenu.sounds)
                                SoundsDict[player.GetCurrentGun().GetType()][player.GetCurrentGun().GetLevel(), 1].Play(Volume);
                            shot_timer.Stop();
                            if (player.GetCurrentGun().Level != Levels.LV1)
                                shotgun_pull_timer.Start();
                            else
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
                            shot_timer.Stop();
                            shotgun_pull_timer.Start();
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

        private void SLIL_LocationChanged(object sender, EventArgs e)
        {
            strafeDirection = Direction.STOP;
            playerDirection = Direction.STOP;
            playerMoveStyle = Direction.WALK;
        }

        private void Mouse_timer_Tick(object sender, EventArgs e)
        {
            if (display == null) return;
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
            if (player == null) return;
            if (player.GetCurrentGun().CanRun && RunKeyPressed &&
                playerDirection == Direction.FORWARD && !player.Fast &&
                player.STAMINE >= player.MAX_STAMINE / 2.5 && !player.Aiming &&
                !shot_timer.Enabled && !reload_timer.Enabled && !shotgun_pull_timer.Enabled && !chill_timer.Enabled)
                playerMoveStyle = Direction.RUN;
            if (playerMoveStyle == Direction.RUN && playerDirection == Direction.FORWARD && !player.Aiming && !reload_timer.Enabled && !shotgun_pull_timer.Enabled)
            {
                if (player.STAMINE <= 0 || chill_timer.Enabled)
                {
                    playerMoveStyle = Direction.WALK;
                    chill_timer.Start();
                }
                else player.STAMINE -= player.GetCurrentGun().LowWeight ? 2 : 3;
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
                double x = display.Width / 2, y = display.Height / 2;
                double X = e.X - x, Y = e.Y - y;
                cursor_x = (int)X; cursor_y = (int)Y;
                if (!InSelectingMode)
                {
                    if (!BlockCamera)
                    {
                        int invY = inv_y ? -1 : 1;
                        int invX = inv_x ? -1 : 1;
                        double A = -(((X / x) / 10) * LOOK_SPEED) * 2.5;
                        double Look = (((Y / y) * 20) * LOOK_SPEED) * 2.5;
                        Controller.ChangePlayerA(A * invX);
                        Controller.ChangePlayerLook(Look * invY);
                    }
                    else if (CanUnblockCamera) BlockCamera = CanUnblockCamera = false;
                    Cursor.Position = display.PointToScreen(new Point((int)x, (int)y));
                }
                else
                {
                    Player player = Controller.GetPlayer();
                    if (player != null)
                    {
                        if (cursor_x < 0 && player.DisposableItems.Count >= 1)
                            player.SelectedItem = 0;
                        else if (cursor_y < 0 && player.DisposableItems.Count >= 2)
                            player.SelectedItem = 1;
                        else if (cursor_x > 0 && player.DisposableItems.Count >= 3)
                            player.SelectedItem = 2;
                        else if (cursor_y > 0 && player.DisposableItems.Count >= 4)
                            player.SelectedItem = 3;
                    }
                }
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
            DISPLAYED_MAP.Replace('P', '.');
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
                    newX -= moveSin * 0.65;
                    newY -= moveCos * 0.65;
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
            if (Controller.GetMap()[(int)player.Y * Controller.GetMapWidth() + (int)player.X] == '.')
                DISPLAYED_MAP[(int)player.Y * Controller.GetMapWidth() + (int)player.X] = 'P';
        }

        private void UpdateBitmap()
        {
            SCREEN?.Dispose();
            WEAPON?.Dispose();
            BUFFER?.Dispose();
            graphicsWeapon?.Dispose();
            center_x = SCREEN_WIDTH[resolution] / 2;
            center_y = SCREEN_HEIGHT[resolution] / 2;
            SCREEN = new Bitmap(SCREEN_WIDTH[resolution], SCREEN_HEIGHT[resolution]);
            WEAPON = new Bitmap(SCREEN_WIDTH[resolution], SCREEN_HEIGHT[resolution]);
            BUFFER = new Bitmap(SCREEN_WIDTH[resolution], SCREEN_HEIGHT[resolution]);
            graphicsWeapon = Graphics.FromImage(WEAPON);
            graphicsWeapon.SmoothingMode = smoothingModes[smoothing];
            display.ResizeImage(DISPLAY_SIZE[display_size, 0], DISPLAY_SIZE[display_size, 1]);
            raycast.Interval = hight_fps ? 15 : 30;
        }

        private void SLIL_FormClosing(object sender, FormClosingEventArgs e)
        {
            Controller.CloseConnection();
            if (!CorrectExit)
            {
                e.Cancel = true;
                if (!Paused)
                    Pause();
                return;
            }
            raycast.Stop();
            step_sound_timer.Stop();
            stamina_timer.Stop();
            mouse_timer.Stop();
            shot_timer.Stop();
            reload_timer.Stop();
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
            ShowMap = false;
        }

        private void Restart_btn_Click(object sender, EventArgs e) => StartGame();

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
            DrawSprites(ref rays, ref ZBuffer, ref ZBufferWindow, out List<int> enemiesCoords);
            foreach (int i in enemiesCoords)
                DISPLAYED_MAP[i] = 'E';
            DrawRaysOnScreen(rays);
            if(!Controller.IsInSpectatorMode())
                DrawWeaponGraphics();
            UpdateDisplay();
            fps = CalculateFPS(elapsed_time);
        }

        private void Mouse_hold_timer_Tick(object sender, EventArgs e)
        {
            if (!GameStarted)
            {
                mouse_hold_timer.Stop();
                return;
            }
            Player player = Controller.GetPlayer();
            if (player.GetCurrentGun() is DisposableItem)
            {
                mouse_hold_timer.Stop();
                return;
            }
            if (player.CanShoot && !reload_timer.Enabled && !shotgun_pull_timer.Enabled && !shot_timer.Enabled)
            {
                if (player.GetCurrentGun().CanShoot && !player.IsPetting)
                {
                    if (!Shoot(player))
                        mouse_hold_timer.Stop();
                }
            }
        }

        private void DrawSprites(ref Pixel[][] rays, ref double[] ZBuffer, ref double[] ZBufferWindow, out List<int> enemiesCoords)
        {
            Player player = Controller.GetPlayer();
            enemiesCoords = new List<int>();
            if (player == null) return;
            invDet = 1.0 / (planeX * dirY - dirX * planeY);
            Entity[] Entities = new Entity[Controller.GetEntities().Count];
            Controller.GetEntities().CopyTo(Entities);
            int mapWidth = Controller.GetMapWidth();
            int entityCount = Entities.Length;
            var spriteInfo = new (int Order, double Distance, int Texture)[entityCount];
            for (int i = 0; i < entityCount; i++)
            {
                double dx = player.X - Entities[i].X;
                double dy = player.Y - Entities[i].Y;
                spriteInfo[i] = (i, dx * dx + dy * dy, Entities[i].Texture);
            }
            Array.Sort(spriteInfo, (a, b) => b.Distance.CompareTo(a.Distance));
            for (int i = 0; i < spriteInfo.Length; i++)
            {
                if (Entities[spriteInfo[i].Order] is Player pl && player.ID == pl.ID) continue;
                double Distance = Math.Sqrt((player.X - Entities[spriteInfo[i].Order].X) * (player.X - Entities[spriteInfo[i].Order].X) + (player.Y - Entities[spriteInfo[i].Order].Y) * (player.Y - Entities[spriteInfo[i].Order].Y));
                if (Distance > player.GetDrawDistance() || Distance == 0) continue;
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
                double vMove = Entities[spriteInfo[i].Order].VMove;
                int vMoveScreen = (int)(vMove / transformY);
                int drawStartX = -spriteWidth / 2 + spriteScreenX + vMoveScreen;
                if (drawStartX < 0) drawStartX = 0;
                int drawEndX = spriteWidth / 2 + spriteScreenX + vMoveScreen;
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
                            double d = (y-vMoveScreen) - (SCREEN_HEIGHT[resolution] - (int)player.Look) / 2 + (drawEndY - drawStartY) / 2;
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
                                        if (!(player.GetCurrentGun() is Flashlight && creature.RespondsToFlashlight) && creature is Pet && (creature as Pet).Stoped && (creature as Pet).HasStopAnimation)
                                            rays[stripe][y].TextureId = spriteInfo[i].Texture + 3;
                                        else
                                        {
                                            if (player.GetCurrentGun() is Flashlight && creature.RespondsToFlashlight)
                                                rays[stripe][y].TextureId = spriteInfo[i].Texture + 2;
                                            else
                                                rays[stripe][y].TextureId = creature.Animations[0][timeNow % creature.Frames];
                                        }
                                        if (creature is Enemy)
                                            enemiesCoords.Add(Entities[spriteInfo[i].Order].IntY * mapWidth + Entities[spriteInfo[i].Order].IntX);
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
                                        rays[stripe][y].TextureId = playerTar.Animations[0][timeNow % playerTar.Frames];
                                    else
                                        rays[stripe][y].TextureId = spriteInfo[i].Texture + 2;
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
                                rays[stripe][y].Blackout = (int)(Math.Min(Math.Max(0, Math.Floor((Distance / player.GetDrawDistance()) * 100)), 100));
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
            using (Graphics g = Graphics.FromImage(BUFFER))
            {
                g.Clear(Color.Black);
                g.DrawImage(SCREEN, 0, 0, BUFFER.Width, BUFFER.Height);
                g.DrawImage(WEAPON, 0, 0, BUFFER.Width, BUFFER.Height);
            }
            SharpDX.Direct2D1.Bitmap dxBitmap = ConvertBitmap.ToDX(BUFFER, display.renderTarget);
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
            dirX = Math.Sin(player.A);
            dirY = Math.Cos(player.A);
            planeX = Math.Sin(player.A - Math.PI / 2) * Math.Tan(FOV / 2);
            planeY = Math.Cos(player.A - Math.PI / 2) * Math.Tan(FOV / 2);
            int mapX = (int)(player.X);
            int mapY = (int)(player.Y);
            Parallel.For(0, SCREEN_WIDTH[resolution], x => rays[x] = CastRay(x, ZBuffer, ZBufferWindow, mapX, mapY));
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
                case 'W': return Color.White;
                case '#': return Color.Blue;
                case '=': return Color.YellowGreen;
                case 'P': return Color.Red;
                case 'B':
                case 'b': return Color.Brown;
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
                    color = GetColorForMapChar(mapChar);
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

        private void DrawDurationEffect(Image effect_image, int icon_size, int index)
        {
            Player player = Controller.GetPlayer();
            if (player == null) return;
            int diameter = icon_size;
            int x = WEAPON.Width - icon_size - 4 - ((icon_size + 4) * index);
            int y = WEAPON.Height - icon_size - 4;
            RectangleF circleRect = new RectangleF(x, y, diameter, diameter);
            using (Pen pen = new Pen(Color.FromArgb(90, 131, 182), 1.75f))
                graphicsWeapon.DrawEllipse(pen, circleRect);
            float sweepAngle = (float)player.Effects[index].EffectTimeRemaining / player.Effects[index].EffectTotalTime * 360;
            using (Pen pen = new Pen(Color.FromArgb(104, 213, 248), 3))
            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddArc(circleRect, -90, sweepAngle);
                graphicsWeapon.DrawPath(pen, path);
            }
            graphicsWeapon.DrawImage(effect_image, x, y, icon_size, icon_size);
        }

        private void DrawItemSelecter(Image item_image, int icon_size, int index, bool selected)
        {
            Player player = Controller.GetPlayer();
            if (player == null) return;
            int x = center_x - (icon_size / 2);
            int y = center_y - (icon_size / 2);
            if (index == 0) x -= icon_size * 2;
            else if (index == 1) y -= icon_size * 2;
            else if (index == 2) x += icon_size * 2;
            else if (index == 3) y += icon_size * 2;
            RectangleF circleRect = new RectangleF(x, y, icon_size, icon_size);
            using (Pen pen = !selected ? new Pen(Color.FromArgb(100, 150, 200), 3.75f) : new Pen(Color.FromArgb(125, 175, 225), 5f))
                graphicsWeapon.DrawEllipse(pen, circleRect);
            if (selected) DrawArrow(cursor_x, cursor_y);
            graphicsWeapon.DrawImage(item_image, x, y, icon_size, icon_size);
        }

        private void DrawArrow(int targetX, int targetY)
        {
            int arrowLength = resolution == 0 ? 12 : 24;
            float angle = (float)Math.Atan2(targetY - (center_y - 1), targetX - (center_x - 1));
            PointF arrowTip = new PointF(
                (center_x - 1) + (float)(arrowLength * Math.Cos(angle)),
                (center_y - 1) + (float)(arrowLength * Math.Sin(angle))
            );
            PointF arrowBase1 = new PointF(
                (center_x - 1) + (float)(arrowLength * 0.7 * Math.Cos(angle + Math.PI / 6)),
                (center_y - 1) + (float)(arrowLength * 0.7 * Math.Sin(angle + Math.PI / 6))
            );
            PointF arrowBase2 = new PointF(
                (center_x - 1) + (float)(arrowLength * 0.7 * Math.Cos(angle - Math.PI / 6)),
                (center_y - 1) + (float)(arrowLength * 0.7 * Math.Sin(angle - Math.PI / 6))
            );
            using (Pen arrowPen = new Pen(Color.FromArgb(104, 213, 248), 2f))
            {
                graphicsWeapon.DrawLine(arrowPen, center_x - 1, center_y - 1, arrowTip.X, arrowTip.Y);
                graphicsWeapon.DrawLine(arrowPen, arrowTip.X, arrowTip.Y, arrowBase1.X, arrowBase1.Y);
                graphicsWeapon.DrawLine(arrowPen, arrowTip.X, arrowTip.Y, arrowBase2.X, arrowBase2.Y);
            }
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
            int item_count = 0;
            if (player.DisposableItems.Count > 0)
                item_count = player.DisposableItems[player.SelectedItem].AmmoCount + player.DisposableItems[player.SelectedItem].AmmoInStock;
            int icon_size = resolution == 0 ? 14 : 28;
            int size = resolution == 0 ? 1 : 2;
            SizeF hpSize = graphicsWeapon.MeasureString(player.HP.ToString("0"), consolasFont[resolution]);
            SizeF moneySize = graphicsWeapon.MeasureString(player.Money.ToString(), consolasFont[resolution]);
            int ammo_icon_x = (icon_size + 2) + (int)hpSize.Width + 2;
            int ammo_x = ammo_icon_x + icon_size;
            graphicsWeapon.Clear(Color.Transparent);
            try
            {
                UpdateMoveStyle(player);
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
            if (player.EffectCheck(2))
                graphicsWeapon.DrawImage(Properties.Resources.helmet_on_head, 0, 0, WEAPON.Width, WEAPON.Height);
            if (ShowFPS)
                graphicsWeapon.DrawString($"FPS: {fps}", consolasFont[resolution], whiteBrush, 0, 0);
            if (!player.CuteMode)
            {
                graphicsWeapon.DrawImage(Properties.Resources.hp, 2, 110 * size, icon_size, icon_size);
                graphicsWeapon.DrawImage(ItemIconDict[player.DisposableItems[player.SelectedItem].GetType()], 2, 92 * size, icon_size, icon_size);
            }
            else
            {
                graphicsWeapon.DrawImage(Properties.Resources.food_hp, 2, 110 * size, icon_size, icon_size);
                graphicsWeapon.DrawImage(CuteItemIconDict[player.DisposableItems[player.SelectedItem].GetType()], 2, 92 * size, icon_size, icon_size);
            }
            if (Controller.IsMultiplayer())
            {
                SizeF fpsSize = graphicsWeapon.MeasureString($"FPS: {fps}", consolasFont[resolution]);
                int ping = Controller.GetPing();
                int connection_status;
                if (ping < 100) connection_status = 0;
                else if (ping < 150) connection_status = 1;
                else if (ping < 300) connection_status = 2;
                else connection_status = 3;
                graphicsWeapon.DrawImage(ConnectionIcons[connection_status], 2, ShowFPS ? fpsSize.Height : 0, icon_size, icon_size);
                graphicsWeapon.DrawString($"{ping}ms", consolasFont[resolution], whiteBrush, icon_size + 2, ShowFPS ? fpsSize.Height : 0);
            }
            graphicsWeapon.DrawString(player.HP.ToString("0"), consolasFont[resolution], whiteBrush, icon_size + 2, 110 * size);
            graphicsWeapon.DrawString(item_count.ToString(), consolasFont[resolution], whiteBrush, icon_size + 2, 92 * size);
            if (!player.IsPetting && player.Guns.Count > 0 && player.GetCurrentGun().ShowAmmo)
            {
                if (player.GetCurrentGun().ShowAmmoAsNumber)
                    graphicsWeapon.DrawString($"{player.GetCurrentGun().AmmoInStock + player.GetCurrentGun().AmmoCount}", consolasFont[resolution], whiteBrush, ammo_x, 110 * size);
                else
                    graphicsWeapon.DrawString($"{player.GetCurrentGun().AmmoInStock}/{player.GetCurrentGun().AmmoCount}", consolasFont[resolution], whiteBrush, ammo_x, 110 * size);
                switch (player.GetCurrentGun().AmmoType)
                {
                    case AmmoTypes.Magic:
                        graphicsWeapon.DrawImage(Properties.Resources.magic, ammo_icon_x, 110 * size, icon_size, icon_size);
                        break;
                    case AmmoTypes.Bubbles:
                        graphicsWeapon.DrawImage(Properties.Resources.bubbles, ammo_icon_x, 110 * size, icon_size, icon_size);
                        break;
                    case AmmoTypes.Bullet:
                        graphicsWeapon.DrawImage(Properties.Resources.bullet, ammo_icon_x, 110 * size, icon_size, icon_size);
                        break;
                    case AmmoTypes.Shell:
                        graphicsWeapon.DrawImage(Properties.Resources.shell, ammo_icon_x, 110 * size, icon_size, icon_size);
                        break;
                    case AmmoTypes.Rifle:
                        graphicsWeapon.DrawImage(Properties.Resources.rifle_bullet, ammo_icon_x, 110 * size, icon_size, icon_size);
                        break;
                    case AmmoTypes.Rocket:
                        graphicsWeapon.DrawImage(Properties.Resources.rocket, ammo_icon_x, 110 * size, icon_size, icon_size);
                        break;
                    case AmmoTypes.C4:
                        graphicsWeapon.DrawImage(Properties.Resources.c4, ammo_icon_x, 110 * size, icon_size, icon_size);
                        break;
                }
            }
            if (player.GetCurrentGun().ShowScope && !InSelectingMode)
            {
                if (player.GetCurrentGun() is Shotgun)
                    graphicsWeapon.DrawImage(scope_shotgun[scope_type], WEAPON.Width / 4, WEAPON.Height / 4, WEAPON.Width / 2, WEAPON.Height / 2);
                else
                    graphicsWeapon.DrawImage(scope[scope_type], WEAPON.Width / 4, WEAPON.Height / 4, WEAPON.Width / 2, WEAPON.Height / 2);
            }
            if (scope_hit != null)
                graphicsWeapon.DrawImage(scope_hit, WEAPON.Width / 4, WEAPON.Height / 4, WEAPON.Width / 2, WEAPON.Height / 2);
            int money_y = 2;
            if (ShowMiniMap)
            {
                Bitmap mini_map = DrawMiniMap();
                money_y = mini_map.Height + 3;
                graphicsWeapon.DrawImage(mini_map, SCREEN_WIDTH[resolution] - mini_map.Width - 1, size);
                mini_map.Dispose();
            }
            graphicsWeapon.DrawString(player.Money.ToString(), consolasFont[resolution], whiteBrush, SCREEN_WIDTH[resolution], money_y, rightToLeft);
            graphicsWeapon.DrawImage(Properties.Resources.money, SCREEN_WIDTH[resolution] - moneySize.Width - icon_size, money_y, icon_size, icon_size);
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
                graphicsWeapon.DrawString(text, consolasFont[resolution + 1], whiteBrush, (WEAPON.Width - textSize.Width) / 2, 30 * size);
            }
            if (player.STAMINE < player.MAX_STAMINE)
                graphicsWeapon.DrawLine(new Pen(Color.Lime, 2 * size), 0, SCREEN_HEIGHT[resolution], (int)(player.STAMINE / player.MAX_STAMINE * SCREEN_WIDTH[resolution]), SCREEN_HEIGHT[resolution]);
            if (player.Effects.Count > 0)
            {
                for (int i = 0; i < player.Effects.Count; i++)
                    DrawDurationEffect(player.Effects[i].Icon, icon_size, i);
            }
            if (InSelectingMode)
            {
                for (int i = 0; i < player.DisposableItems.Count; i++)
                {
                    Image icon = ItemIconDict[player.DisposableItems[i].GetType()];
                    bool selected = false;
                    if (player.CuteMode)
                        icon = CuteItemIconDict[player.DisposableItems[i].GetType()];
                    if (player.SelectedItem == i) selected = true;
                    DrawItemSelecter(icon, icon_size, i, selected);
                }
            }
            if (resolution == 1)
            {
                graphicsWeapon.DrawLine(new Pen(Color.Black, 1), 0, WEAPON.Height - 1, WEAPON.Width, WEAPON.Height - 1);
                graphicsWeapon.DrawLine(new Pen(Color.Black, 1), WEAPON.Width - 1, 0, WEAPON.Width - 1, WEAPON.Height - 1);
            }
        }

        private void UpdateMoveStyle(Player player)
        {
            if (!shot_timer.Enabled && !reload_timer.Enabled && !shotgun_pull_timer.Enabled)
            {
                if (playerMoveStyle == Direction.RUN)
                {
                    if (player.GetCurrentGun() is Pistol || player.GetCurrentGun() is Shotgun || player.GetCurrentGun() is SniperRifle || player.GetCurrentGun() is Fingershot)
                    {
                        if (player.GetCurrentGun() is SniperRifle || (player.GetCurrentGun() is Pistol && player.GetCurrentGun().AmmoCount <= 0))
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
                    if (player.GetCurrentGun() is Pistol && player.GetCurrentGun().AmmoCount <= 0 && player.GetCurrentGun().Level != Levels.LV4)
                        player.MoveStyle = 4;
                    else if (player.GetCurrentGun() is RPG && player.GetCurrentGun().AmmoCount <= 0)
                        player.MoveStyle = 5;
                    else
                        player.MoveStyle = 0;
                }
                if (!player.Aiming) player.GunState = player.MoveStyle;
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

        private Pixel[] CastRay(int x, double[] ZBuffer, double[] ZBufferWindow, int mapX, int mapY)
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
                if (mapX < 0 || mapX >= (player.GetDrawDistance()) + player.X || mapY < 0 || mapY >= (player.GetDrawDistance()) + player.Y || distance >= player.GetDrawDistance())
                {
                    hit_wall = true;
                    distance = player.GetDrawDistance();
                    continue;
                }
                char test_wall = Controller.GetMap()[mapY * Controller.GetMapWidth() + mapX];
                switch (test_wall)
                {
                    case 'W':
                        DISPLAYED_MAP[mapY * Controller.GetMapWidth() + mapX] = 'W';
                        break;
                    case '#':
                        hit_wall = true;
                        DISPLAYED_MAP[mapY * Controller.GetMapWidth() + mapX] = '#';
                        break;
                    case '=':
                        hit_window = true;
                        DISPLAYED_MAP[mapY * Controller.GetMapWidth() + mapX] = '=';
                        break;
                    case 'B':
                    case 'b':
                        DISPLAYED_MAP[mapY * Controller.GetMapWidth() + mapX] = 'b';
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
            double ceiling = (SCREEN_HEIGHT[resolution] - player.Look) / 2 - (SCREEN_HEIGHT[resolution] * FOV) / distance;
            double floor = SCREEN_HEIGHT[resolution] - (ceiling + player.Look);
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
                if (!GameStarted) break;
                int blackout = 0, textureId = 1;
                if (hit_window && y > mid)
                {
                    ceiling = (SCREEN_HEIGHT[resolution] - player.Look) / 2 - (SCREEN_HEIGHT[resolution] * FOV) / window_distance;
                    floor = SCREEN_HEIGHT[resolution] - (ceiling + player.Look);
                }
                else
                {
                    ceiling = (SCREEN_HEIGHT[resolution] - player.Look) / 2 - (SCREEN_HEIGHT[resolution] * FOV) / distance;
                    floor = SCREEN_HEIGHT[resolution] - (ceiling + player.Look);
                }
                if (y <= ceiling)
                {
                    textureId = 7;
                    double d = (y + player.Look / 2) / (SCREEN_HEIGHT[resolution] / 2);
                    blackout = (int)(Math.Min(Math.Max(0, d * 100), 100));
                }
                else if (y >= mid && y <= floor && hit_window)
                {
                    textureId = 2;
                    if (Math.Abs(y - mid) <= 6 / window_distance || is_window_bound)
                        textureId = 0;
                    blackout = (int)(Math.Min(Math.Max(0, Math.Floor((window_distance / player.GetDrawDistance()) * 100)), 100));
                }
                else if ((y < mid || !hit_window) && y > ceiling && y < floor)
                {
                    textureId = 2;
                    if (hit_door)
                        textureId = 3;
                    if (is_bound)
                        textureId = 0;
                    blackout = (int)(Math.Min(Math.Max(0, Math.Floor((distance / player.GetDrawDistance()) * 100)), 100));
                }
                else if (y >= floor)
                {
                    textureId = 6;
                    double d = 1 - (y - (SCREEN_HEIGHT[resolution] - player.Look) / 2) / (SCREEN_HEIGHT[resolution] / 2);
                    blackout = (int)(Math.Min(Math.Max(0, d * 100), 100));
                }
                result[y] = new Pixel(x, y, blackout, distance, ceiling - floor, textureId);
                if (y < ceiling)
                {
                    int p = y - (int)(SCREEN_HEIGHT[resolution] - player.Look) / 2;
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
                    int p = y - (int)(SCREEN_HEIGHT[resolution] - player.Look) / 2;
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

        private void InitMap()
        {
            DISPLAYED_MAP.Clear();
            string DMAP = "";
            for (int i = 0; i < Controller.GetMap().Length; i++)
                DMAP += '.';
            if (difficulty < 5)
                DISPLAYED_MAP.Append(DMAP);
            else
            {
                if (inDebug == 1)
                    DISPLAYED_MAP.Append(debugMap);
                else if (inDebug == 2)
                    DISPLAYED_MAP.Append(bossMap);
            }
        }

        private void ResetDefault(Player player)
        {
            map = null;
            display.SCREEN = null;
            scope[scope_type] = GetScope(scope[scope_type]);
            scope_shotgun[scope_type] = GetScope(scope_shotgun[scope_type]);
            display.Refresh();
            BlockCamera = CanUnblockCamera = true;
            int x = display.PointToScreen(Point.Empty).X + (display.Width / 2);
            int y = display.PointToScreen(Point.Empty).Y + (display.Height / 2);
            Cursor.Position = new Point(x, y);
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
            map = new Bitmap(Controller.GetMapWidth(), Controller.GetMapHeight());
            if (MainMenu.sounds)
            {
                if (!player.CuteMode)
                {
                    prev_ost = rand.Next(ost.Length - 3);
                    ChangeOst(prev_ost);
                }
                else ChangeOst(7);
            }
        }

        public void KillFromConsole()
        {
            if (Controller.IsMultiplayer())
                Controller.DealDamage(Controller.GetPlayer(), 9999);
            else Controller.StopGame(0);
        }

        private void StartGame()
        {
            Controller.RestartGame();
            Player player = Controller.GetPlayer();
            if (console_panel == null)
            {
                console_panel = new ConsolePanel()
                {
                    Dock = DockStyle.Fill,
                    Visible = false,
                    player = player,
                    Entities = Controller.GetEntities()
                };
                console_panel.Log("SLIL console *v1.3*\nType \"-help-\" for a list of commands...", false, false, Color.Lime);
                Controls.Add(console_panel);
                display = new Display() { Size = Size, Dock = DockStyle.Fill, TabStop = false };
                display.MouseDown += new MouseEventHandler(Display_MouseDown);
                display.MouseUp += new MouseEventHandler(Display_MouseUp);
                display.MouseMove += new MouseEventHandler(Display_MouseMove);
                display.MouseWheel += new MouseEventHandler(Display_Scroll);
                Controls.Add(display);
            }
            BlockCamera = CanUnblockCamera = true;
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
            stamina_timer.Start();
            mouse_timer.Start();
            if (MainMenu.sounds) step_sound_timer.Start();
            GameStarted = true;
            game_over_panel.Visible = false;
            display.BringToFront();
            display.Focus();
        }

        private void ToDefault()
        {
            shop_tab_control.Controls.Clear();
            shop_tab_control.Controls.Add(weapon_shop_page);
            shop_tab_control.Controls.Add(pet_shop_page);
            shop_tab_control.Controls.Add(consumables_shop_page);
        }

        private void GameOver(int win)
        {
            foreach (PlaySound ostTrack in ost)
                ostTrack?.Stop();
            raycast.Stop();
            shot_timer.Stop();
            reload_timer.Stop();
            step_sound_timer.Stop();
            stamina_timer.Stop();
            mouse_timer.Stop();
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
                StartGame();
            }
            else if (win == 0)
            {
                ToDefault();
                game_over_panel.Visible = true;
                game_over_panel.BringToFront();
                if (MainMenu.sounds)
                    game_over.Play(Volume);
            }
            else ToDefault();
        }

        internal void BuyAmmo(Gun weapon) => Controller.BuyAmmo(weapon);

        internal void BuyWeapon(Gun weapon) => Controller.BuyWeapon(weapon);

        internal void UpdateWeapon(Gun weapon) => Controller.UpdateWeapon(weapon);

        internal void BuyConsumable(DisposableItem item) => Controller.BuyConsumable(item);

        internal Player GetPlayer() => Controller.GetPlayer();
    }
}