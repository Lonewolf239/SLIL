using System;
using System.IO;
using Play_Sound;
using System.Data;
using System.Linq;
using System.Text;
using SLIL.Classes;
using System.Drawing;
using Convert_Bitmap;
using System.Threading;
using SLIL.UserControls;
using Accord.Video.FFMPEG;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using SLIL.UserControls.Inventory;
using System.Runtime.InteropServices;

namespace SLIL
{
    internal delegate void StartGameDelegate();
    internal delegate void StopGameDelegate(int win);
    internal delegate void InitPlayerDelegate();
    internal delegate void PlaySoundDelegate(PlaySound sound, double X, double Y, bool setVolume);
    internal delegate void CloseFormDelegate();

    internal partial class SLIL : Form
    {
        private readonly GameController Controller;
        internal string PlayerName;
        private bool isCursorVisible = true;
        internal int CustomMazeHeight, CustomMazeWidth;
        internal bool CUSTOM = false, ShowFPS = true, ShowMiniMap = true;
        internal bool ShowDebugSpeed = false, ShowPositongDebug = false, ShowGameDebug = false;
        internal bool InvY = false, InvX = false;
        internal static int difficulty = 1;
        private int inDebug = 0;
        internal static double LookSpeed = 2.5;
        internal StringBuilder CUSTOM_MAP = new StringBuilder();
        internal double CUSTOM_X, CUSTOM_Y;
        private readonly Random Rand;
        private const int TexWidth = 128;
        private readonly int SCREEN_HEIGHT = 128, SCREEN_WIDTH = 228;
        private int CenterX = 0, CenterY = 0, CursorX = 0, CursorY = 0;
        private readonly int[] DISPLAY_SIZE = { 228, 128 };
        internal static int Resolution = 0, Smoothing = 1, InterfaceSize = 2;
        private readonly SmoothingMode[] SmoothingModes =
        {
            SmoothingMode.None,
            SmoothingMode.AntiAlias,
            SmoothingMode.HighQuality,
            SmoothingMode.HighSpeed
        };
        private float StageOpacity = 1;
        internal static bool HightFps = true;
        private const double FOV = Math.PI / 3;
        private static readonly StringBuilder DISPLAYED_MAP = new StringBuilder();
        private Bitmap SCREEN, WEAPON, BUFFER;
        private ImageAttributes imageAttributes, imageCuteAttributes;
        private readonly Font[,] ConsolasFont =
        {
            { new Font("Consolas", 8F), new Font("Consolas", 14F), new Font("Consolas", 20F) },
            { new Font("Consolas", 8F), new Font("Consolas", 16F), new Font("Consolas", 20F) },
            { new Font("Consolas", 10F), new Font("Consolas", 18F), new Font("Consolas", 20F) },
            { new Font("Consolas", 12F), new Font("Consolas", 18F), new Font("Consolas", 20F) },
        };
        private readonly SolidBrush WhiteBrush = new SolidBrush(Color.White), blackBrush = new SolidBrush(Color.Black);
        private readonly StringFormat rightToLeft = new StringFormat() { FormatFlags = StringFormatFlags.DirectionRightToLeft };
        private Graphics graphicsWeapon;
        private int fps;
        private double planeX, planeY, dirX, dirY, invDet;
        private double ElapsedTime = 0, PreviousTime, DeltaTime;
        private DateTime TotalTime = DateTime.Now;
        private List<int> soundIndices = new List<int> { 0, 1, 2, 3, 4 };
        private int CurrentIndex = 0;
        private bool active = true;
        private bool Paused = false, RunKeyPressed = false;
        internal static readonly Dictionary<Type, Image> ScreenEffectsIcons = new Dictionary<Type, Image>()
        {
            { typeof(BloodEffect1), Properties.Resources.blood_effect_0 },
            { typeof(BloodEffect2), Properties.Resources.blood_effect_1 },
            { typeof(BloodEffect3), Properties.Resources.blood_effect_2 },
            { typeof(BloodEffect4), Properties.Resources.blood_effect_3 },
            { typeof(CuteBloodEffect1), Properties.Resources.blood_effect_0 },
            { typeof(CuteBloodEffect2), Properties.Resources.blood_effect_1 },
            { typeof(CuteBloodEffect3), Properties.Resources.blood_effect_2 },
            { typeof(CuteBloodEffect4), Properties.Resources.blood_effect_3 },
        };
        internal static readonly Dictionary<Type, Image[]> IconDict = new Dictionary<Type, Image[]>
        {
            { typeof(Flashlight), new[] { Properties.Resources.missing } },
            { typeof(Knife), new[] { Properties.Resources.missing } },
            { typeof(Candy), new[] { Properties.Resources.missing } },
            { typeof(Petition), new[] { Properties.Resources.petition_icon } },
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
            { typeof(Fingershot), new[] { Properties.Resources.fingershot_icon } },
            { typeof(TSPitW), new[] { Properties.Resources.tspitw_icon } },
            { typeof(Gnome), new[] { Properties.Resources.gnome_icon } },
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
            { typeof(MedicalKit), new[]
            {
                Properties.Resources.medical_kit_icon,
                Properties.Resources.medical_kit_icon
            } },
            { typeof(Minigun), new[] { Properties.Resources.minigun_icon } },
        };
        internal static readonly Dictionary<Type, Image[,]> GunsImagesDict = new Dictionary<Type, Image[,]>
        {
            { typeof(Flashlight), new[,] { { Properties.Resources.flashlight, Properties.Resources.flashlight_run } } },
            { typeof(Knife), new[,] { { Properties.Resources.knife, Properties.Resources.knife_hit, Properties.Resources.knife_run } } },
            { typeof(Candy), new[,] { { Properties.Resources.candy, Properties.Resources.candy_shoot, Properties.Resources.candy_run } } },
            { typeof(Petition), new[,] { { Properties.Resources.petition, Properties.Resources.petition_shoot, Properties.Resources.petition_run } } },
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
                   { Properties.Resources.medkit, Properties.Resources.medkit_using_0, Properties.Resources.medkit_using_1, Properties.Resources.medkit_using_2 },
                   { Properties.Resources.syringe, Properties.Resources.syringe_using_0, Properties.Resources.syringe_using_1, Properties.Resources.syringe_using_2 },
                   { Properties.Resources.hand, Properties.Resources.hand_using_0, Properties.Resources.hand_using_1, Properties.Resources.hand_using_2 },
                   { Properties.Resources.food, Properties.Resources.food_using_0, Properties.Resources.food_using_1, Properties.Resources.food_using_2 },
            } },
            { typeof(Adrenalin), new[,]
            {
                   { Properties.Resources.adrenalin, Properties.Resources.adrenalin_using_0, Properties.Resources.adrenalin_using_1, Properties.Resources.adrenalin_using_2 },
            } },
            { typeof(Helmet), new[,]
            {
                   { Properties.Resources.helmet, Properties.Resources.helmet_using_0, Properties.Resources.helmet_using_1, Properties.Resources.helmet_using_2, Properties.Resources.helmet_using_3 }
            } },
            { typeof(RPG), new[,]
            {
                   { Properties.Resources.rpg, Properties.Resources.rpg_shooted, Properties.Resources.rpg_reload_0, Properties.Resources.rpg_reload_1, Properties.Resources.rpg_reload_2, Properties.Resources.rpg_empty }
            } },
            { typeof(MedicalKit), new[,]
            {
                   { Properties.Resources.full_medical_kit, Properties.Resources.full_medical_kit_0, Properties.Resources.full_medical_kit_1, Properties.Resources.full_medical_kit_2 }
            } },
            { typeof(Minigun), new[,]
            {
                   { Properties.Resources.minigun, Properties.Resources.minigun_shooted, Properties.Resources.minigun, Properties.Resources.minigun, Properties.Resources.minigun, Properties.Resources.minigun }
            } },
        };
        internal static readonly Dictionary<Type, PlaySound[,]> GunsSoundsDict = new Dictionary<Type, PlaySound[,]>
        {
            { typeof(Flashlight), new[,] { { new PlaySound(null, false), } } },
            { typeof(Knife), new[,] { { new PlaySound(MainMenu.CGFReader.GetFile("knife.wav"), false) } } },
            { typeof(Candy), new[,] { { new PlaySound(MainMenu.CGFReader.GetFile("candy.wav"), false) } } },
            { typeof(Petition), new[,] { { new PlaySound(MainMenu.CGFReader.GetFile("petition.wav"), false) } } },
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
                   { new PlaySound(MainMenu.CGFReader.GetFile("medkit_using.wav"), false) },
                   { new PlaySound(MainMenu.CGFReader.GetFile("syringe_using.wav"), false) },
                   { new PlaySound(MainMenu.CGFReader.GetFile("hand_using.wav"), false) },
                   { new PlaySound(MainMenu.CGFReader.GetFile("food_using.wav"), false) }
            } },
            { typeof(Adrenalin), new[,]
            {
                   { new PlaySound(MainMenu.CGFReader.GetFile("adrenalin_using.wav"), false) }
            } },
            { typeof(Helmet), new[,]
            {
                   { new PlaySound(MainMenu.CGFReader.GetFile("helmet_using.wav"), false) }
            } },
            { typeof(RPG), new[,]
            {
                   { new PlaySound(MainMenu.CGFReader.GetFile("rpg.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("rpg_reloading.wav"), false), new PlaySound(null, false) }
            } },
            { typeof(MedicalKit), new[,]
            {
                   { new PlaySound(MainMenu.CGFReader.GetFile("medical_kit_using.wav"), false) }
            } },
            { typeof(Minigun), new[,]
            {
                   { new PlaySound(MainMenu.CGFReader.GetFile("minigun.wav"), false), new PlaySound(null, false), new PlaySound(null, false) }
            } },
        };
        internal static readonly Dictionary<Type, PlaySound[]> TransportsSoundsDict = new Dictionary<Type, PlaySound[]>
        {
            {typeof(Bike), new[] {
                    new PlaySound(MainMenu.CGFReader.GetFile("bike_idle.wav"), false),
                    new PlaySound(MainMenu.CGFReader.GetFile("bike_stopping.wav"), false),
                    new PlaySound(MainMenu.CGFReader.GetFile("bike_full_speed.wav"), false)
            } },
        };
        internal static readonly Dictionary<Type, Image[]> TransportImages = new Dictionary<Type, Image[]>
        {
            { typeof(Bike), new[]
            {
                Properties.Resources.im_biker,
                Properties.Resources.on_bike,
                Properties.Resources.using_bike_left,
                Properties.Resources.using_bike_right,
                Properties.Resources.bike_jump,
            } },
        };
        internal static readonly Dictionary<Type, Image> EffectIcon = new Dictionary<Type, Image>
        {
            { typeof(Regeneration), Properties.Resources.regeneration_effect },
            { typeof(Adrenaline), Properties.Resources.adrenalin_effect },
            { typeof(Protection), Properties.Resources.protection_effect },
            { typeof(Fatigue), Properties.Resources.fatigue_effect },
            { typeof(Rider), Properties.Resources.driver },
            { typeof(Bleeding), Properties.Resources.bleeding },
            { typeof(Blindness), Properties.Resources.blindness },
            { typeof(Stunned), Properties.Resources.stunned },
            { typeof(VoidE), Properties.Resources.missing },
            { typeof(God), Properties.Resources.missing },
        };
        internal static readonly Dictionary<Type, Image> ItemIconDict = new Dictionary<Type, Image>
        {
            { typeof(FirstAidKit), Properties.Resources.first_aid },
            { typeof(Adrenalin), Properties.Resources.adrenalin_count_icon },
            { typeof(Helmet), Properties.Resources.helmet_count_icon },
            { typeof(MedicalKit), Properties.Resources.super_medical_kit_icon },
        };
        internal static readonly Dictionary<Type, Image> CuteItemIconDict = new Dictionary<Type, Image>
        {
            { typeof(FirstAidKit), Properties.Resources.food_count },
            { typeof(Adrenalin), Properties.Resources.adrenalin_count_icon },
            { typeof(Helmet), Properties.Resources.helmet_count_icon },
            { typeof(MedicalKit), Properties.Resources.super_medical_kit_icon },
        };
        internal static readonly Dictionary<Type, Image> ShopImageDict = new Dictionary<Type, Image>
        {
            { typeof(SillyCat), Properties.Resources.pet_cat_icon },
            { typeof(GreenGnome), Properties.Resources.pet_gnome_icon },
            { typeof(EnergyDrink), Properties.Resources.pet_energy_drink_icon },
            { typeof(Pyro), Properties.Resources.pet_pyro_icon },
            { typeof(Bike), Properties.Resources.bike_icon}
        };
        private readonly BindControls Bind;
        private readonly TextureCache Textures;
        private readonly List<ScreenEffects> ScreenEffects;
        internal static PlaySound[] Hit =
        {
            new PlaySound(MainMenu.CGFReader.GetFile("hit_player_0.wav"), false),
            new PlaySound(MainMenu.CGFReader.GetFile("hit_player_1.wav"), false),
            new PlaySound(MainMenu.CGFReader.GetFile("hit_player_2.wav"), false)
        };
        internal static PlaySound HitTransport = new PlaySound(MainMenu.CGFReader.GetFile("hungry_player.wav"), false);
        internal static PlaySound Hungry = new PlaySound(MainMenu.CGFReader.GetFile("hungry_player.wav"), false);
        private PlaySound Step, TransportStep;
        internal static PlaySound[] ScarySounds = new PlaySound[]
        {
            new PlaySound(MainMenu.CGFReader.GetFile("scary_sound_0.wav"), false),
            new PlaySound(MainMenu.CGFReader.GetFile("scary_sound_1.wav"), false),
            new PlaySound(MainMenu.CGFReader.GetFile("scary_sound_2.wav"), false),
            new PlaySound(MainMenu.CGFReader.GetFile("scary_sound_3.wav"), false),
            new PlaySound(MainMenu.CGFReader.GetFile("scary_sound_4.wav"), false),
            new PlaySound(MainMenu.CGFReader.GetFile("scary_sound_5.wav"), false),
            new PlaySound(MainMenu.CGFReader.GetFile("scary_sound_6.wav"), false),
            new PlaySound(MainMenu.CGFReader.GetFile("scary_sound_7.wav"), false),
            new PlaySound(MainMenu.CGFReader.GetFile("scary_sound_8.wav"), false),
            new PlaySound(MainMenu.CGFReader.GetFile("scary_sound_9.wav"), false)
        };
        internal static PlaySound[,] Steps = new PlaySound[,]
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
            },
            {
                new PlaySound(MainMenu.CGFReader.GetFile("step_back_0.wav"), false),
                new PlaySound(MainMenu.CGFReader.GetFile("step_back_1.wav"), false),
                new PlaySound(MainMenu.CGFReader.GetFile("step_back_2.wav"), false),
                new PlaySound(MainMenu.CGFReader.GetFile("step_back_3.wav"), false),
                new PlaySound(MainMenu.CGFReader.GetFile("step_back_4.wav"), false)
            },
            {
                new PlaySound(MainMenu.CGFReader.GetFile("step_run_back_0.wav"), false),
                new PlaySound(MainMenu.CGFReader.GetFile("step_run_back_1.wav"), false),
                new PlaySound(MainMenu.CGFReader.GetFile("step_run_back_2.wav"), false),
                new PlaySound(MainMenu.CGFReader.GetFile("step_run_back_3.wav"), false),
                new PlaySound(MainMenu.CGFReader.GetFile("step_run_back_4.wav"), false)
            },
            {
                new PlaySound(MainMenu.CGFReader.GetFile("step_void_0.wav"), false),
                new PlaySound(MainMenu.CGFReader.GetFile("step_void_1.wav"), false),
                new PlaySound(MainMenu.CGFReader.GetFile("step_void_2.wav"), false),
                new PlaySound(MainMenu.CGFReader.GetFile("step_void_3.wav"), false),
                new PlaySound(MainMenu.CGFReader.GetFile("step_void_4.wav"), false)
            },
            {
                new PlaySound(MainMenu.CGFReader.GetFile("step_run_void_0.wav"), false),
                new PlaySound(MainMenu.CGFReader.GetFile("step_run_void_1.wav"), false),
                new PlaySound(MainMenu.CGFReader.GetFile("step_run_void_2.wav"), false),
                new PlaySound(MainMenu.CGFReader.GetFile("step_run_void_3.wav"), false),
                new PlaySound(MainMenu.CGFReader.GetFile("step_run_void_4.wav"), false)
            },
        };
        internal static PlaySound[] Ost = new PlaySound[]
        {
            new PlaySound(MainMenu.CGFReader.GetFile("slil_ost_0.wav"), true),
            new PlaySound(MainMenu.CGFReader.GetFile("slil_ost_1.wav"), true),
            new PlaySound(MainMenu.CGFReader.GetFile("slil_ost_2.wav"), true),
            new PlaySound(MainMenu.CGFReader.GetFile("slil_ost_3.wav"), true),
            new PlaySound(MainMenu.CGFReader.GetFile("slil_ost_4.wav"), true),
            new PlaySound(null, false),
            new PlaySound(MainMenu.CGFReader.GetFile("gnome.wav"), true),
            new PlaySound(MainMenu.CGFReader.GetFile("cmode_ost.wav"), true),
            new PlaySound(MainMenu.CGFReader.GetFile("backrooms_ost.wav"), true),
            new PlaySound(MainMenu.CGFReader.GetFile("empty_ost.wav"), true)
        };
        internal static PlaySound[,] DeathSounds = new PlaySound[,]
        {
            //Zombie
            {
                new PlaySound(MainMenu.CGFReader.GetFile("zombie_die_0.wav"), false),
                new PlaySound(MainMenu.CGFReader.GetFile("zombie_die_1.wav"), false),
                new PlaySound(MainMenu.CGFReader.GetFile("zombie_die_2.wav"), false)
            },
            //Dog
            {
                new PlaySound(MainMenu.CGFReader.GetFile("dog_die_0.wav"), false),
                new PlaySound(MainMenu.CGFReader.GetFile("dog_die_1.wav"), false),
                new PlaySound(MainMenu.CGFReader.GetFile("dog_die_2.wav"), false)
            },
            //Ogr
            {
                new PlaySound(MainMenu.CGFReader.GetFile("ogr_die_0.wav"), false),
                new PlaySound(MainMenu.CGFReader.GetFile("ogr_die_1.wav"), false),
                new PlaySound(MainMenu.CGFReader.GetFile("ogr_die_2.wav"), false)
            },
            //Bat
            {
                new PlaySound(MainMenu.CGFReader.GetFile("bat_die_0.wav"), false),
                new PlaySound(MainMenu.CGFReader.GetFile("bat_die_1.wav"), false),
                new PlaySound(MainMenu.CGFReader.GetFile("bat_die_2.wav"), false)
            },
            //Box
            {
                new PlaySound(MainMenu.CGFReader.GetFile("break_box_0.wav"), false),
                new PlaySound(MainMenu.CGFReader.GetFile("break_box_1.wav"), false),
                new PlaySound(MainMenu.CGFReader.GetFile("break_box_2.wav"), false)
            },
            //Shooter
            {
                new PlaySound(MainMenu.CGFReader.GetFile("shooter_die_0.wav"), false),
                new PlaySound(MainMenu.CGFReader.GetFile("shooter_die_1.wav"), false),
                new PlaySound(MainMenu.CGFReader.GetFile("shooter_die_2.wav"), false)
            },
            //LostSoul
            {
                new PlaySound(MainMenu.CGFReader.GetFile("lost_soul_die_0.wav"), false),
                new PlaySound(MainMenu.CGFReader.GetFile("lost_soul_die_1.wav"), false),
                new PlaySound(MainMenu.CGFReader.GetFile("lost_soul_die_2.wav"), false)
            }
        };
        internal static PlaySound[,] CuteDeathSounds = new PlaySound[,]
        {
            //Zombie
            {
                new PlaySound(MainMenu.CGFReader.GetFile("c_zombie_die_0.wav"), false),
                new PlaySound(MainMenu.CGFReader.GetFile("c_zombie_die_1.wav"), false),
                new PlaySound(MainMenu.CGFReader.GetFile("c_zombie_die_2.wav"), false)
            },
            //Dog
            {
                new PlaySound(MainMenu.CGFReader.GetFile("c_dog_die_0.wav"), false),
                new PlaySound(MainMenu.CGFReader.GetFile("c_dog_die_1.wav"), false),
                new PlaySound(MainMenu.CGFReader.GetFile("c_dog_die_2.wav"), false)
            },
            //Ogr
            {
                new PlaySound(MainMenu.CGFReader.GetFile("c_ogr_die_0.wav"), false),
                new PlaySound(MainMenu.CGFReader.GetFile("c_ogr_die_1.wav"), false),
                new PlaySound(MainMenu.CGFReader.GetFile("c_ogr_die_2.wav"), false)
            },
            //Bat
            {
                new PlaySound(MainMenu.CGFReader.GetFile("c_bat_die_0.wav"), false),
                new PlaySound(MainMenu.CGFReader.GetFile("c_bat_die_1.wav"), false),
                new PlaySound(MainMenu.CGFReader.GetFile("c_bat_die_2.wav"), false)
            },
            //Box
            {
                new PlaySound(MainMenu.CGFReader.GetFile("break_box_0.wav"), false),
                new PlaySound(MainMenu.CGFReader.GetFile("break_box_1.wav"), false),
                new PlaySound(MainMenu.CGFReader.GetFile("break_box_2.wav"), false)
            },
            //Shooter
            {
                new PlaySound(MainMenu.CGFReader.GetFile("c_shooter_die_0.wav"), false),
                new PlaySound(MainMenu.CGFReader.GetFile("c_shooter_die_1.wav"), false),
                new PlaySound(MainMenu.CGFReader.GetFile("c_shooter_die_2.wav"), false)
            },
            //LostSoul
            {
                new PlaySound(MainMenu.CGFReader.GetFile("c_lost_soul_die_0.wav"), false),
                new PlaySound(MainMenu.CGFReader.GetFile("c_lost_soul_die_1.wav"), false),
                new PlaySound(MainMenu.CGFReader.GetFile("c_lost_soul_die_2.wav"), false)
            }
        };
        internal static PlaySound[,] SoundsofShotsEnemies = new PlaySound[,]
        {
            //Shooter
            {
                new PlaySound(MainMenu.CGFReader.GetFile("shooter_shot.wav"), false),
                new PlaySound(MainMenu.CGFReader.GetFile("shooter_c_shot.wav"), false)
            },
            //LostSoul
            {
                new PlaySound(MainMenu.CGFReader.GetFile("lost_soul_shot.wav"), false),
                new PlaySound(MainMenu.CGFReader.GetFile("lost_soul_c_shot.wav"), false)
            }
        };
        internal static PlaySound Draw = new PlaySound(MainMenu.CGFReader.GetFile("draw.wav"), false),
            Buy = new PlaySound(MainMenu.CGFReader.GetFile("buy.wav"), false),
            Wall = new PlaySound(MainMenu.CGFReader.GetFile("wall_interaction.wav"), false),
            Wp = new PlaySound(MainMenu.CGFReader.GetFile("tp.wav"), false),
            Screenshot = new PlaySound(MainMenu.CGFReader.GetFile("screenshot.wav"), false),
            LowStamine = new PlaySound(MainMenu.CGFReader.GetFile("low_stamine.wav"), false),
            Starter = new PlaySound(MainMenu.CGFReader.GetFile("starter.wav"), false),
            RPGExplosion = new PlaySound(MainMenu.CGFReader.GetFile("explosion.wav"), false),
            BarrelExplosion = new PlaySound(MainMenu.CGFReader.GetFile("barrel_explosion.wav"), false),
            BreakdownDoor = new PlaySound(MainMenu.CGFReader.GetFile("breakdown_door.wav"), false),
            LiftingAmmoBox = new PlaySound(MainMenu.CGFReader.GetFile("lifting_ammo_box.wav"), false),
            LiftingMoneyPile = new PlaySound(MainMenu.CGFReader.GetFile("lifting_money_pile.wav"), false),
            VoidStalkerScreamer = new PlaySound(MainMenu.CGFReader.GetFile("void_stalker_screamer.wav"), false),
            Kick = new PlaySound(MainMenu.CGFReader.GetFile("kick.wav"), false),
            DamnKick = new PlaySound(MainMenu.CGFReader.GetFile("damn_kick.wav"), false),
            PlayerDeathSound = new PlaySound(MainMenu.CGFReader.GetFile("player_death_sound.wav"), false);
        internal static PlaySound[] Climb = new PlaySound[] { new PlaySound(MainMenu.CGFReader.GetFile("climb.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("climb_bike.wav"), false) };
        internal static PlaySound[] Door = { new PlaySound(MainMenu.CGFReader.GetFile("door_opened.wav"), false), new PlaySound(MainMenu.CGFReader.GetFile("door_closed.wav"), false) };
        private const string bossMap = @"#########################...............##F###.................####..##...........##..###...=...........=...###...=.....E.....=...###...................###...................###.........#.........###...##.........##...###....#.........#....###...................###..#...##.#.##...#..####.....#.....#.....######...............##############d####################...#################E=...=E#################...#################$D.P.D$#################...################################",
            debugMap = @"##########################.......................##.......................##..WWWW..7.6.4.3.2.1.#..##..WE.W.................##..W.EW.................##..WWWW...........b..d..##.......................##.......................##.................B..=..##.......................##..#....................##..#b.......P.....L..F..##..####X................##..#B...................##..#..............l..b..##..####.................##..X....................##....................B..##.......................##......b=5#D#...##d#=#####...======#$#L###X.#....##.....=B..###.#B#....#..##...............L..l##.f##########################",
            bikeMap = @"############################......######.......#####........####.........###.......................##.......................##....####......####....=##...######....######...=##...######====#dd###...=##...##$###....#dd###...=##...##D###....######...=##...##.b##.....####....=##WWW##..##..............##EEE#F...d..............##WWW##..##..............##...##.B##.....####.....##...##D###....######....##...##$###....###dd#====##...######....###dd#....##...######....######....##....####......####.....##.......................##.......................###........####.......P.#####......######.......############################";
        internal static float Volume = 0.4f, EffectsVolume = 0.4f, MusicVolume = 0;
        private int BurstShots = 0, ReloadFrames = 0;
        internal static int OstIndex = 0;
        internal static int PrevOst;
        private Image ScopeHit = null;
        private readonly Image[] Scope =
        {
            Properties.Resources.scope,
            Properties.Resources.scope_cross,
            Properties.Resources.scope_line,
            Properties.Resources.scope_dot,
            Properties.Resources.scope_null
        };
        private readonly Image[] ScopeShotgun =
        {
            Properties.Resources.scope_shotgun,
            Properties.Resources.scope_cross,
            Properties.Resources.scope_line,
            Properties.Resources.scope_dot,
            Properties.Resources.scope_null
        };
        private readonly Image[] HScope =
        {
            Properties.Resources.h_scope,
            Properties.Resources.h_scope_cross,
            Properties.Resources.h_scope_line,
            Properties.Resources.h_scope_dot,
            Properties.Resources.scope_null
        };
        private readonly Image[] HScopeShotgun =
        {
            Properties.Resources.h_scope_shotgun,
            Properties.Resources.h_scope_cross,
            Properties.Resources.h_scope_line,
            Properties.Resources.h_scope_dot,
            Properties.Resources.scope_null
        };
        private bool IsTutorial = false;
        internal static int ScopeColor = 0, ScopeType = 0;
        internal static bool ShowMap = false;
        private bool ShowSing = false, ShowInventory = false;
        private int SingID, scrollPosition = 0;
        private const int ScrollBarWidth = 4, ScrollPadding = 5;
        private bool ShopOpen = false, StartShopOpen = false, PressedR = false, CancelReload = false;
        private float xOffset = 0, yOffset = 0, xOffsetDirection = 0.25f, yOffsetDirection = 0.25f;
        private double RecoilY = 0, RecoilLX = 0, RecoilRX = 0;
        private double RecoilOY = 0, RecoilLOX = 0, RecoilROX = 0;
        private const float RecoilRecoverySpeed = 9.5f;
        private Display SLILDisplay;
        private Bitmap map;
        private ConsolePanel ConsolePanel;
        private const double playerWidth = 0.4;
        private bool ScreenRecording = false;
        private VideoFileWriter VideoWriter;
        private const int SavesWidth = 912;
        private const int SavesHeight = 512;
        private bool GameStarted = false, CorrectExit = false;
        private bool HoldLMB = false;
        private WeaponToolTip InventoryWeaponToolTip;
        private InfoToolTip InventoryInfoToolTip;
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
        //private readonly CloseFormDelegate CloseFormHandle;

        internal SLIL(TextureCache textures)
        {
            InitializeComponent();
            SetLocalization();
            StartGameHandle = StartGameInvokerSinglePlayer;
            StopGameHandle = StopGameInvoker;
            InitPlayerHandle = InitPlayerInvoker;
            PlaySoundHandle = PlaySoundInvoker;
            Controller = new GameController(StartGameHandle, InitPlayerHandle, StopGameHandle, PlaySoundHandle);
            Controller.SetCustom(CUSTOM, CustomMazeWidth, CustomMazeHeight, CUSTOM_MAP.ToString(), CUSTOM_X, CUSTOM_Y);
            Rand = new Random(Guid.NewGuid().GetHashCode());
            Bind = new BindControls(MainMenu.BindControls);
            ScreenEffects = new List<ScreenEffects>();
            SetParameters();
            Textures = textures;
            Controller.StartGame();
        }
        internal SLIL(TextureCache textures, bool custom, StringBuilder customMap, int mazeWidth, int mazeHeight, double customX, double customY)
        {
            InitializeComponent();
            SetLocalization();
            StartGameHandle = StartGameInvokerSinglePlayer;
            StopGameHandle = StopGameInvoker;
            InitPlayerHandle = InitPlayerInvoker;
            PlaySoundHandle = PlaySoundInvoker;
            Controller = new GameController(StartGameHandle, InitPlayerHandle, StopGameHandle, PlaySoundHandle);
            Rand = new Random(Guid.NewGuid().GetHashCode());
            Bind = new BindControls(MainMenu.BindControls);
            ScreenEffects = new List<ScreenEffects>();
            SetParameters();
            Textures = textures;
            CUSTOM = custom;
            CUSTOM_MAP = customMap;
            CustomMazeWidth = mazeWidth;
            CustomMazeHeight = mazeHeight;
            CUSTOM_X = customX;
            CUSTOM_Y = customY;
            Controller.SetCustom(CUSTOM, CustomMazeWidth, CustomMazeHeight, CUSTOM_MAP.ToString(), CUSTOM_X, CUSTOM_Y);
            if (IsTutorial) Controller.ToTutorial();
            Controller.StartGame();
        }
        internal SLIL(TextureCache textures, string adress, int port, string password, string PlayerName)
        {
            InitializeComponent();
            SetLocalization();
            this.PlayerName = PlayerName;
            StartGameHandle = StartGameInvokerMultiPlayer;
            StopGameHandle = StopGameInvoker;
            InitPlayerHandle = InitPlayerInvoker;
            PlaySoundHandle = PlaySoundInvoker;
            //CloseFormHandle = CloseFormInvoker;
            //Controller = new GameController(adress, port, password, StartGameHandle, InitPlayerHandle, StopGameHandle, PlaySoundHandle, CloseFormHandle, PlayerName);
            Rand = new Random(Guid.NewGuid().GetHashCode());
            Bind = new BindControls(MainMenu.BindControls);
            ScreenEffects = new List<ScreenEffects>();
            SetParameters();
            Textures = textures;
        }
        internal SLIL() { }

        private void SetLocalization()
        {
            if (!MainMenu.DownloadedLocalizationList)
            {
                shop_title.Text = "SHOP";
                weapon_shop_page.Text = "Weapons";
                pet_shop_page.Text = "Pets";
                consumables_shop_page.Text = "Other";
                transport_shop_page.Text = "Transport";
                storage_shop_page.Text = "Storage";
                inventory_label.Text = "Inventory";
                weapon_title.Text = "Weapons";
                items_title.Text = "Items";
                pet_title.Text = "Pet";
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
                transport_shop_page.Text = MainMenu.Localizations.GetLString(MainMenu.Language, "2-14");
                storage_shop_page.Text = MainMenu.Localizations.GetLString(MainMenu.Language, "2-16");
                inventory_label.Text = MainMenu.Localizations.GetLString(MainMenu.Language, "2-20");
                weapon_title.Text = MainMenu.Localizations.GetLString(MainMenu.Language, "2-21");
                items_title.Text = MainMenu.Localizations.GetLString(MainMenu.Language, "2-22");
                pet_title.Text = MainMenu.Localizations.GetLString(MainMenu.Language, "2-23");
                pause_text.Text = MainMenu.Localizations.GetLString(MainMenu.Language, "2-5");
                pause_btn.Text = MainMenu.Localizations.GetLString(MainMenu.Language, "2-6");
                exit_btn.Text = MainMenu.Localizations.GetLString(MainMenu.Language, "2-7");
            }
        }

        private void SetParameters()
        {
            Cursor = Program.SLILCursorDefault;
            IsTutorial = MainMenu.IsTutorial;
            difficulty = MainMenu.difficulty;
            switch (difficulty)
            {
                case 0:
                    Controller.SetEnemyDamageOffset(1.75);
                    break;
                case 1:
                    Controller.SetEnemyDamageOffset(1.25);
                    break;
                case 2:
                    Controller.SetEnemyDamageOffset(1);
                    break;
                case 3:
                    Controller.SetEnemyDamageOffset(0.75);
                    break;
                default:
                    Controller.SetEnemyDamageOffset(1);
                    break;
            }
            InterfaceSize = MainMenu.interface_size;
            Smoothing = MainMenu.smoothing;
            ScopeType = MainMenu.scope_type;
            ScopeColor = MainMenu.scope_color;
            HightFps = MainMenu.hight_fps;
            ShowFPS = MainMenu.ShowFPS;
            ShowMiniMap = MainMenu.ShowMiniMap;
            LookSpeed = MainMenu.LOOK_SPEED;
            InvX = MainMenu.inv_x;
            InvY = MainMenu.inv_y;
            Volume = MainMenu.Volume;
            EffectsVolume = MainMenu.EffectsVolume;
        }

        private void UpdateBitmap()
        {
            SCREEN?.Dispose();
            WEAPON?.Dispose();
            BUFFER?.Dispose();
            graphicsWeapon?.Dispose();
            CenterX = SCREEN_WIDTH / 2;
            CenterY = SCREEN_HEIGHT / 2;
            SCREEN = new Bitmap(SCREEN_WIDTH, SCREEN_HEIGHT);
            WEAPON = new Bitmap(SCREEN_WIDTH, SCREEN_HEIGHT);
            BUFFER = new Bitmap(SCREEN_WIDTH, SCREEN_HEIGHT);
            imageAttributes = new ImageAttributes();
            imageCuteAttributes = new ImageAttributes();
            float[][] colorMatrixElements =
            {
                new float[] { (float)MainMenu.Gamma / 100, 0.0f, 0.0f, 0.0f, 0.0f},
                new float[] {0.0f, (float)MainMenu.Gamma / 100, 0.0f, 0.0f, 0.0f},
                new float[] {0.0f, 0.0f, (float)MainMenu.Gamma / 100, 0.0f, 0.0f},
                new float[] {0.0f, 0.0f, 0.0f, 1.0f, 0.0f},
                new float[] {0.0f, 0.0f, 0.0f, 0.0f, 1.0f}
            };
            float[][] colorCuteMatrixElements =
            {
                new float[] { (float)MainMenu.Gamma / 125, 0.0f, 0.0f, 0.0f, 0.0f},
                new float[] {0.0f, (float)MainMenu.Gamma / 125, 0.0f, 0.0f, 0.0f},
                new float[] {0.0f, 0.0f, (float)MainMenu.Gamma / 125, 0.0f, 0.0f},
                new float[] {0.0f, 0.0f, 0.0f, 1.0f, 0.0f},
                new float[] {0.0f, 0.0f, 0.0f, 0.0f, 1.0f}
            };
            ColorMatrix colorMatrix = new ColorMatrix(colorMatrixElements);
            ColorMatrix colorCuteMatrix = new ColorMatrix(colorCuteMatrixElements);
            imageAttributes.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
            imageCuteAttributes.SetColorMatrix(colorCuteMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
            graphicsWeapon = Graphics.FromImage(WEAPON);
            graphicsWeapon.InterpolationMode = InterpolationMode.NearestNeighbor;
            graphicsWeapon.SmoothingMode = SmoothingModes[Smoothing];
            SLILDisplay.ResizeImage(DISPLAY_SIZE[0], DISPLAY_SIZE[1]);
            raycast.Interval = HightFps ? 15 : 30;
        }

        //  #====      Invokers     ====#

        internal void StartGameInvokerSinglePlayer()
        {
            if (this.InvokeRequired && this.IsHandleCreated)
            {
                this.BeginInvoke((MethodInvoker)delegate
                {
                    this.StartGame();
                });
            }
            else this.StartGame();
        }

        internal void StartGameInvokerMultiPlayer()
        {
            while (!this.IsHandleCreated)
            {

            }
            this.BeginInvoke((MethodInvoker)delegate
            {
                this.StartGame();
            });
        }

        internal void StopGameInvoker(int win)
        {
            if (this.InvokeRequired && this.IsHandleCreated)
            {
                this.BeginInvoke((MethodInvoker)delegate
                {
                    this.GameOver(win);
                });
            }
            else this.GameOver(win);
        }

        internal void InitPlayerInvoker()
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    //player = GetPlayer();
                    //player.IsPetting = false;
                });
            }
            else
            {
                //player = GetPlayer();
                //player.IsPetting = false;
            }
        }

        internal void PlaySoundInvoker(PlaySound sound, double X, double Y, bool setVolume)
        {
            Player player = GetPlayer();
            if (player == null) return;
            float vol = Volume;
            if (setVolume)
            {
                float distance = (float)Math.Sqrt((player.X - X) * (player.X - X) + (player.Y - Y) * (player.Y - Y));
                if (distance > 14) return;
                if (distance > 1) vol /= (float)Math.Pow(distance, 0.87);
            }
            if (this.InvokeRequired && this.IsHandleCreated)
            {
                this.BeginInvoke((MethodInvoker)delegate
                {
                    sound.Play(vol);
                });
            }
            else sound.Play(vol);
        }

        internal void CloseFormInvoker()
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

        //  #====  Console methods  ====#

        internal static void SetVolume() => Ost[OstIndex].SetVolume(MusicVolume);

        internal bool OnOffNoClip() => Controller.OnOffNoClip();

        internal static void GoDebug(SLIL slil, int debug)
        {
            slil.IsTutorial = false;
            slil.Controller.StopGame(-1);
            slil.inDebug = debug;
            difficulty = 5;
            slil.Controller.GoDebug(debug);
            slil.StartGame();
        }

        internal static void ChangeOst(int index)
        {
            if (!MainMenu.sounds) return;
            Ost[OstIndex]?.Stop();
            OstIndex = index;
            Ost[OstIndex].LoopPlay(MusicVolume);
        }

        internal void KillFromConsole()
        {
            if (Controller.IsMultiplayer()) Controller.DealDamage(GetPlayer(), 9999);
            else Controller.StopGame(0);
        }

        internal bool SpawnEntity(int id, bool hasAI)
        {
            Player player = GetPlayer();
            Entity entity = null;
            double moveSin = Math.Sin(player.A) * 3;
            double moveCos = Math.Cos(player.A) * 3;
            double x = player.X + moveSin;
            double y = player.Y + moveCos;
            if (Controller.GetMap()[GetCoordinate(x, y)] != '.') return false;
            switch (id)
            {
                case 0: // player dead
                    entity = new PlayerDeadBody(x, y, Controller.GetMapWidth(), Controller.GetMaxEntityID());
                    break;
                case 1: // zombie
                    entity = new Zombie(x, y, Controller.GetMapWidth(), Controller.GetMaxEntityID());
                    break;
                case 2: // dog
                    entity = new Dog(x, y, Controller.GetMapWidth(), Controller.GetMaxEntityID());
                    break;
                case 3: // abomination
                    entity = new Ogr(x, y, Controller.GetMapWidth(), Controller.GetMaxEntityID());
                    break;
                case 4: // bat
                    entity = new Bat(x, y, Controller.GetMapWidth(), Controller.GetMaxEntityID());
                    break;
                case 5: // box
                    entity = new Box(x, y, Controller.GetMapWidth(), Controller.GetMaxEntityID());
                    break;
                case 6: // barrel
                    entity = new Barrel(x, y, Controller.GetMapWidth(), Controller.GetMaxEntityID());
                    break;
                case 7: // shop door
                    entity = new ShopDoor(x, y, Controller.GetMapWidth(), Controller.GetMaxEntityID());
                    break;
                case 8: // shop man
                    entity = new ShopMan(x, y, Controller.GetMapWidth(), Controller.GetMaxEntityID());
                    break;
                case 9: // teleport
                    entity = new Teleport(x, y, Controller.GetMapWidth(), Controller.GetMaxEntityID());
                    break;
                case 10: // hitting the wall
                    entity = new HittingTheWall(x, y, Controller.GetMapWidth(), Controller.GetMaxEntityID());
                    break;
                case 11: // rpg rocket
                    entity = new RpgRocket(x, y, Controller.GetMapWidth(), Controller.GetMaxEntityID());
                    break;
                case 12: // rpg explosion
                    entity = new RpgExplosion(x, y, Controller.GetMapWidth(), Controller.GetMaxEntityID());
                    break;
                case 13: // silly cat
                    entity = new SillyCat(x, y, Controller.GetMapWidth(), Controller.GetMaxEntityID());
                    break;
                case 14: // green gnome
                    entity = new GreenGnome(x, y, Controller.GetMapWidth(), Controller.GetMaxEntityID());
                    break;
                case 15: // energy drink
                    entity = new EnergyDrink(x, y, Controller.GetMapWidth(), Controller.GetMaxEntityID());
                    break;
                case 16: // pyro
                    entity = new Pyro(x, y, Controller.GetMapWidth(), Controller.GetMaxEntityID());
                    break;
                case 17: // bike
                    entity = new Bike(x, y, Controller.GetMapWidth(), Controller.GetMaxEntityID());
                    break;
                case 18: // vine
                    entity = new Vine(x, y, Controller.GetMapWidth(), Controller.GetMaxEntityID());
                    break;
                case 19: // lamp
                    entity = new Lamp(x, y, Controller.GetMapWidth(), Controller.GetMaxEntityID());
                    break;
                case 20: // backrooms teleport
                    entity = new BackroomsTeleport(x, y, Controller.GetMapWidth(), Controller.GetMaxEntityID());
                    break;
                case 21: // void teleport
                    entity = new VoidTeleport(x, y, Controller.GetMapWidth(), Controller.GetMaxEntityID());
                    break;
                case 22: // void stalker
                    entity = new VoidStalker(x, y, Controller.GetMapWidth(), Controller.GetMaxEntityID());
                    break;
                case 23: // stalker
                    entity = new Stalker(x, y, Controller.GetMapWidth(), Controller.GetMaxEntityID());
                    break;
                case 24: // shooter
                    entity = new Shooter(x, y, Controller.GetMapWidth(), Controller.GetMaxEntityID());
                    break;
                case 25: // lost soul
                    entity = new LostSoul(x, y, Controller.GetMapWidth(), Controller.GetMaxEntityID());
                    break;
                case 26: // soul explosion
                    entity = new SoulExplosion(x, y, Controller.GetMapWidth(), Controller.GetMaxEntityID());
                    break;
                case 27:
                    //exploding barrel
                    entity = new ExplodingBarrel(x, y, Controller.GetMapWidth(), Controller.GetMaxEntityID());
                    break;
                case 28:
                    //barrel explosion
                    entity = new BarrelExplosion(x, y, Controller.GetMapWidth(), Controller.GetMaxEntityID());
                    break;
                case 29:
                    //dummy
                    entity = new Dummy(x, y, Controller.GetMapWidth(), Controller.GetMaxEntityID());
                    break;
            }
            if (entity == null) return false;
            entity.HasAI = hasAI;
            Controller.AddEntity(entity);
            return true;
        }

        //  #====    SLIL methods   ====#

        private void SLIL_Activated(object sender, EventArgs e)
        {
            TopMost = true;
            active = true;
        }

        private void SLIL_Deactivate(object sender, EventArgs e)
        {
            Player player = GetPlayer();
            if (player != null)
            {
                player.StrafeDirection = Directions.STOP;
                player.PlayerDirection = Directions.STOP;
                player.PlayerMoveStyle = Directions.WALK;
            }
            RunKeyPressed = false;
            if (!Paused) Pause();
            TopMost = false;
            active = false;
        }

        private void SLIL_LocationChanged(object sender, EventArgs e)
        {
            Player player = GetPlayer();
            if (player != null)
            {
                player.StrafeDirection = Directions.STOP;
                player.PlayerDirection = Directions.STOP;
                player.PlayerMoveStyle = Directions.WALK;
            }
        }

        private void SLIL_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Controller.CloseConnection();
            if (!CorrectExit)
            {
                e.Cancel = true;
                if (!Paused) Pause();
                return;
            }
            raycast.Stop();
            step_sound_timer.Stop();
            stamina_timer.Stop();
            mouse_timer.Stop();
            shot_timer.Stop();
            reload_timer.Stop();
            status_refresh.Stop();
            chill_timer.Stop();
            stage_timer.Stop();
            shotgun_pull_timer.Stop();
            mouse_hold_timer.Stop();
            fps_timer.Stop();
            if (!isCursorVisible) Cursor.Show();
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

        private void Restart_btn_Click(object sender, EventArgs e) => StartGame();

        //  #====       Timer       ====#

        private void Raycast_Tick(object sender, EventArgs e)
        {
            try
            {
                DateTime time = DateTime.Now;
                ElapsedTime = (time - TotalTime).TotalSeconds;
                TotalTime = time;
                DeltaTime = time.TimeOfDay.TotalSeconds - PreviousTime;
                PreviousTime = time.TimeOfDay.TotalSeconds;
                PlayerMove();
                ClearDisplayedMap();
                double[] ZBuffer = new double[SCREEN_WIDTH];
                double[] ZBufferWindow = new double[SCREEN_WIDTH];
                Pixel[][] rays = CastRaysParallel(ZBuffer, ZBufferWindow);
                DrawSprites(ref rays, ref ZBuffer, ref ZBufferWindow, out List<int> enemiesCoords);
                foreach (int i in enemiesCoords) DISPLAYED_MAP[i] = 'E';
                DrawRaysOnScreen(rays);
                if (!Controller.InBackrooms())
                {
                    if (!Controller.IsInSpectatorMode()) DrawGameInterface();
                    //else DrawSpectatorInterface();
                }
                else DrawCamera();
                UpdateDisplay();
                DoingScreenRecording();
            }
            catch { }
        }

        private void Step_sound_timer_Tick(object sender, EventArgs e)
        {
            Player player = GetPlayer();
            if (player == null) return;
            if (!player.InTransport || player.TRANSPORT == null)
                TransportStep?.Stop();
            if ((player.MoveSpeed != 0 || (player.StrafeSpeed != 0 && !player.InTransport)) && !player.InParkour && !player.Aiming && (Step == null || !Step.IsPlaying))
            {
                if (CurrentIndex >= soundIndices.Count)
                {
                    soundIndices = soundIndices.OrderBy(x => Rand.Next()).ToList();
                    CurrentIndex = 0;
                }
                int i = player.PlayerMoveStyle == Directions.RUN || player.Fast ? 1 : 0;
                if (player.InTransport && player.TRANSPORT != null)
                {
                    if (player.PlayerDirection == Directions.STOP || player.MoveSpeed < 0 ||
                        (player.PlayerDirection == Directions.BACK && player.MoveSpeed > 0)) //stopping
                        Step = TransportsSoundsDict[player.TRANSPORT.GetType()][1];
                    else //full speed
                        Step = TransportsSoundsDict[player.TRANSPORT.GetType()][2];
                    if (TransportStep == null || TransportStep != Step)
                    {
                        TransportStep?.Stop();
                        TransportStep = Step;
                        Controller.PlayGameSound(TransportStep);
                    }
                    else
                    {
                        if (TransportStep.GetRemainTime() <= 0)
                        {
                            TransportStep?.Stop();
                            TransportStep = Step;
                            Controller.PlayGameSound(TransportStep);
                        }
                    }
                }
                else
                {
                    int index = i;
                    if (Controller.InBackrooms())
                    {
                        if (Controller.GetBackroomsStage() == 0)
                            index = i + 4;
                        else index = i + 6;
                    }
                    else if (player.CuteMode) index = i + 2;
                    Step = Steps[index, soundIndices[CurrentIndex]];
                    Step.PlayWithWait(Volume);
                }
                CurrentIndex++;
            }
            else if (player.InTransport && player.TRANSPORT != null && player.MoveSpeed == 0)
            {
                //IDLE
                Step = TransportsSoundsDict[player.TRANSPORT.GetType()][0];
                if (TransportStep == null || TransportStep != Step)
                {
                    TransportStep?.Stop();
                    TransportStep = Step;
                    Controller.PlayGameSound(TransportStep);
                }
                else
                {
                    if (TransportStep.GetRemainTime() <= 0)
                    {
                        TransportStep?.Stop();
                        TransportStep = Step;
                        Controller.PlayGameSound(TransportStep);
                    }
                }
                CurrentIndex++;
            }
        }

        private void Stamina_timer_Tick(object sender, EventArgs e)
        {
            Player player = GetPlayer();
            if (player == null || Controller.IsInSpectatorMode()) return;
            if (RunKeyPressed && PlayerCanRun() && player.PlayerDirection == Directions.FORWARD)
            {
                if (player.Stamine >= player.MaxStamine / 2.5)
                    player.PlayerMoveStyle = Directions.RUN;
            }
            else player.PlayerMoveStyle = Directions.WALK;
            if (player.PlayerMoveStyle == Directions.RUN && player.PlayerDirection == Directions.FORWARD && !player.Aiming && !reload_timer.Enabled && !shotgun_pull_timer.Enabled)
            {
                if (player.Stamine <= 0)
                {
                    player.Stamine = 0;
                    player.PlayerMoveStyle = Directions.WALK;
                    chill_timer.Start();
                    Controller.PlayGameSound(LowStamine);
                }
                else player.ReducesStamine();
            }
            else
            {
                player.PlayerMoveStyle = Directions.WALK;
                if (player.Stamine < player.MaxStamine)
                    player.RestoreStamine();
            }
        }

        private void Mouse_timer_Tick(object sender, EventArgs e)
        {
            if (SLILDisplay == null) return;
            Rectangle displayRectangle = new Rectangle
            {
                Location = SLILDisplay.PointToScreen(Point.Empty),
                Width = SLILDisplay.Width,
                Height = SLILDisplay.Height
            };
            Point cursorPosition = Cursor.Position;
            if (!displayRectangle.Contains(cursorPosition) && active)
                Cursor.Position = SLILDisplay.PointToScreen(new Point(SLILDisplay.Width / 2, SLILDisplay.Height / 2));
        }

        private void Shot_timer_Tick(object sender, EventArgs e)
        {
            try
            {
                Player player = GetPlayer();
                if (BurstShots < player.GetCurrentGun().BurstShots)
                {
                    if (player.GetCurrentGun().FireType != FireTypes.Single)
                        player.GunState = player.GunState == 1 ? 0 : 1;
                    else
                        player.GunState = player.Aiming ? 3 : 0;
                    if (!player.GetCurrentGun().InfinityAmmo)
                        Controller.AmmoCountDecrease();
                    if (player.GetCurrentGun().CanShoot && player.GetCurrentGun().FireType != FireTypes.Single)
                    {
                        BulletRayCasting();
                        SetRecoil(player);
                    }
                    if (player.GetCurrentGun().AmmoCount <= 0 && player.GetCurrentGun().AmmoInStock > 0)
                    {
                        player.GunState = 2;
                        if (player.GetCurrentGun() is Pistol && player.GetCurrentGun().Level != Levels.LV4)
                            player.GunState = 3;
                        player.Aiming = false;
                        Controller.PlayGameSound(GunsSoundsDict[player.GetCurrentGun().GetType()][player.GetCurrentGun().GetLevel(), 1]);
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
                            Controller.PlayGameSound(GunsSoundsDict[player.GetCurrentGun().GetType()][player.GetCurrentGun().GetLevel(), 1]);
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
                            Controller.PlayGameSound(GunsSoundsDict[player.GetCurrentGun().GetType()][player.GetCurrentGun().GetLevel(), 1]);
                            shot_timer.Stop();
                            shotgun_pull_timer.Start();
                        }
                        player.CanShoot = true;
                    }
                }
                BurstShots++;
                if (BurstShots >= player.GetCurrentGun().BurstShots)
                    shot_timer.Stop();
                if (!shot_timer.Enabled || player.GetCurrentGun().FireType == FireTypes.Single)
                    ScopeHit = null;
            }
            catch { }
        }

        private void Reload_gun_Tick(object sender, EventArgs e)
        {
            try
            {
                ScopeHit = null;
                if (GameStarted)
                {
                    int index = 1;
                    Player player = GetPlayer();
                    if (player.GetCurrentGun().AmmoCount == 0 && player.GetCurrentGun().AmmoInStock == 0) reload_timer.Stop();
                    if (player.GetCurrentGun() is Shotgun && (player.GetCurrentGun().AmmoInStock == 0 || PressedR))
                    {
                        if (player.GetCurrentGun().Level == Levels.LV1) index = 2;
                        else
                        {
                            index = 3;
                            if (PressedR) index--;
                        }
                    }
                    if (ReloadFrames >= player.GetCurrentGun().ReloadFrames - index)
                    {
                        if (player.GetCurrentGun() is Shotgun && player.GetCurrentGun().Level != Levels.LV1)
                        {
                            if (player.GetCurrentGun().AmmoInStock > 0)
                            {
                                player.GunState = 3;
                                ReloadFrames = PressedR ? -1 : 0;
                                Controller.ReloadClip();
                            }
                            if (CancelReload || player.GetCurrentGun().AmmoInStock == 0 || player.GetCurrentGun().AmmoCount == player.GetCurrentGun().CartridgesClip)
                            {
                                CancelReload = false;
                                PressedR = false;
                                player.CanShoot = true;
                                reload_timer.Stop();
                                ReloadFrames = 0;
                                return;
                            }
                        }
                        else
                        {
                            player.GunState = player.MoveStyle;
                            PressedR = false;
                            player.CanShoot = true;
                            Controller.ReloadClip();
                            reload_timer.Stop();
                            ReloadFrames = 0;
                            return;
                        }
                    }
                    else if (player.GetCurrentGun().ReloadFrames > 1) player.GunState++;
                    ReloadFrames++;
                    if (player.GetCurrentGun() is Shotgun)
                        Controller.PlayGameSound(GunsSoundsDict[player.GetCurrentGun().GetType()][player.GetCurrentGun().GetLevel(), 3]);
                }
                else
                {
                    CancelReload = false;
                    PressedR = false;
                    ReloadFrames = 0;
                    reload_timer.Stop();
                }
            }
            catch { }
        }

        private void Status_refresh_Tick(object sender, EventArgs e)
        {
            if (SLILDisplay == null) return;
            if (!raycast.Enabled && SLILDisplay.Screen != null) SLILDisplay.Screen = null;
            bool shouldShowCursor = StartShopOpen || !GameStarted || ShopOpen || ShowInventory || ConsolePanel.Visible || (active && !GameStarted) || Paused;
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
                game_over_interface.Left = (Width - game_over_interface.Width) / 2;
                game_over_interface.Top = (Height - game_over_interface.Height) / 2;
                restart_btn.Left = (game_over_interface.Width - restart_btn.Width) / 2;
                exit_restart_btn.Left = (game_over_interface.Width - exit_restart_btn.Width) / 2;
                total_killed_label.Text = Controller.GetTotalKilled().ToString();
                last_stage_label.Text = (Controller.GetStage() + 1).ToString();
                cause_of_death_icon.Image = GetDeathCause(Controller.GetDeathCause());
                total_time_label.Text = Controller.GetTotalTime();
            }
            Player player = GetPlayer();
            if (player == null) return;
            if (player.Hit)
            {
                player.Hit = false;
                int dice = Rand.Next(100);
                if (player.CuteMode)
                {
                    if (dice < 25) ScreenEffects.Add(new CuteBloodEffect1());
                    else if (dice < 50) ScreenEffects.Add(new CuteBloodEffect2());
                    else if (dice < 75) ScreenEffects.Add(new CuteBloodEffect3());
                    else ScreenEffects.Add(new CuteBloodEffect4());
                }
                else
                {
                    if (dice < 25) ScreenEffects.Add(new BloodEffect1());
                    else if (dice < 50) ScreenEffects.Add(new BloodEffect2());
                    else if (dice < 75) ScreenEffects.Add(new BloodEffect3());
                    else ScreenEffects.Add(new BloodEffect4());
                }
            }
            shop_money.Text = $"$: {player.Money}";
            try
            {
                if (player.GetCurrentGun() is Flashlight)
                    shot_timer.Enabled = reload_timer.Enabled = shotgun_pull_timer.Enabled = false;
                if (!player.GetCurrentGun().CanRun)
                    player.PlayerMoveStyle = Directions.WALK;
                if (player.LevelUpdated && !ShopOpen)
                {
                    ChangeWeapon(player.CurrentGun);
                    player.LevelUpdated = false;
                }
            }
            catch { }
        }

        private void Chill_timer_Tick(object sender, EventArgs e) => chill_timer.Stop();

        private void Stage_timer_Tick(object sender, EventArgs e)
        {
            StageOpacity -= 0.1f;
            if (StageOpacity <= 0) stage_timer.Stop();
        }

        private void Shotgun_pull_timer_Tick(object sender, EventArgs e)
        {
            if (!GameStarted)
            {
                shotgun_pull_timer.Stop();
                return;
            }
            Player player = GetPlayer();
            player.GunState = player.MoveStyle;
            player.CanShoot = true;
            shotgun_pull_timer.Stop();
            ReloadFrames = 0;
            if (player.GetCurrentGun().AmmoCount == 0)
                reload_timer.Start();
        }

        private void Mouse_hold_timer_Tick(object sender, EventArgs e)
        {
            if (!GameStarted)
            {
                mouse_hold_timer.Stop();
                return;
            }
            Player player = GetPlayer();
            if (player.GetCurrentGun() is DisposableItem || !HoldLMB)
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

        private void Camera_shaking_timer_Tick(object sender, EventArgs e)
        {
            Player player = GetPlayer();
            if (player == null) return;
            if (!player.InTransport && player.MoveSpeed == 0 && player.StrafeSpeed == 0 ||
                player.InTransport && player.MoveSpeed == 0)
            {
                xOffset = 0;
                yOffset = 0;
            }
            else
            {
                double move = Math.Max(0.005, Math.Abs(player.GetMoveSpeed(ElapsedTime) * player.GetMoveSpeed(ElapsedTime)));
                double strafe = Math.Max(0.0025, Math.Abs(player.GetStrafeSpeed(ElapsedTime) * player.GetStrafeSpeed(ElapsedTime)));
                float offSet = (float)Math.Sqrt(move + strafe) * (25 * (player.PlayerMoveStyle == Directions.RUN ? 2 : 1));
                if (xOffset > 3)
                    xOffsetDirection = -0.2f * offSet;
                if (xOffset < 0)
                    xOffsetDirection = 0.2f * offSet;
                xOffset += xOffsetDirection;
                if (yOffset > 3)
                    yOffsetDirection = -0.15f * offSet;
                if (yOffset < 0)
                    yOffsetDirection = 0.15f * offSet;
                yOffset += yOffsetDirection;
            }
        }

        private void Recoil_timer_Tick(object sender, EventArgs e)
        {
            Player player = GetPlayer();
            if (player == null) return;
            if (RecoilY > 0)
            {
                double change = Math.Min(RecoilRecoverySpeed + RecoilOY, RecoilY);
                RecoilOY++;
                RecoilY -= change;
                Controller.ChangePlayerLook(-change);
            }
            else RecoilOY = 0;
            if (RecoilLX > 0)
            {
                double change = Math.Min(RecoilRecoverySpeed + RecoilLOX, RecoilLX);
                RecoilLOX++;
                RecoilLX -= change;
                Controller.ChangePlayerA(-change);
            }
            else RecoilLOX = 0;
            if (RecoilRX > 0)
            {
                double change = Math.Min(RecoilRecoverySpeed + RecoilROX, RecoilRX);
                RecoilROX++;
                RecoilRX -= change;
                Controller.ChangePlayerA(change);
            }
            else RecoilROX = 0;
        }

        private void Fps_timer_Tick(object sender, EventArgs e) => fps = CalculateFPS();

        private void Screen_effects_timer_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < ScreenEffects.Count; i++)
                ScreenEffects[i].ReducingTimeRemaining();
        }

        private void Fade_timer_Tick(object sender, EventArgs e)
        {
            if (!MainMenu.sounds || MainMenu.MusicVolume == 0) fade_timer.Stop();
            else
            {
                MusicVolume += 0.01f;
                if (Math.Abs(MusicVolume - MainMenu.MusicVolume) <= 0.01)
                {
                    MusicVolume = MainMenu.MusicVolume;
                    fade_timer.Stop();
                }
                Ost[OstIndex].SetVolume(MusicVolume);
            }
        }

        //  #====       Input       ====#

        private void SLIL_KeyDown(object sender, KeyEventArgs e)
        {
            Player player = GetPlayer();
            if (e.KeyCode == Keys.Escape)
            {
                if (ShopOpen) HideShop();
                else if (ConsolePanel.Visible)
                {
                    Scope[ScopeType] = GetScope(Scope[ScopeType]);
                    HScope[ScopeType] = GetScope(HScope[ScopeType]);
                    ScopeShotgun[ScopeType] = GetScope(ScopeShotgun[ScopeType]);
                    HScopeShotgun[ScopeType] = GetScope(HScopeShotgun[ScopeType]);
                    ConsolePanel.Visible = false;
                    mouse_timer.Start();
                    ConsolePanel.ClearCommand();
                    SLILDisplay.Focus();
                    int x = SLILDisplay.PointToScreen(Point.Empty).X + (SLILDisplay.Width / 2);
                    int y = SLILDisplay.PointToScreen(Point.Empty).Y + (SLILDisplay.Height / 2);
                    Cursor.Position = new Point(x, y);
                }
                else if (!GameStarted) Close();
                else if (ShowSing) ShowSing = player.BlockInput = player.BlockCamera = false;
                else if (ShowInventory) CloseInventory();
                else Pause();
                return;
            }
            if (GameStarted && !Paused)
            {
                if (!ConsolePanel.Visible && !ShopOpen && !player.BlockInput)
                {
                    if (e.KeyCode == Bind.Run) RunKeyPressed = true;
                    if (e.KeyCode == Bind.Forward)
                        player.PlayerDirection = Directions.FORWARD;
                    if (e.KeyCode == Bind.Back)
                        player.PlayerDirection = Directions.BACK;
                    if (e.KeyCode == Bind.Left)
                        player.StrafeDirection = Directions.LEFT;
                    if (e.KeyCode == Bind.Right)
                        player.StrafeDirection = Directions.RIGHT;
                    if (player != null && !shot_timer.Enabled && !reload_timer.Enabled &&
                        !shotgun_pull_timer.Enabled && !player.InTransport &&
                        !Controller.InBackrooms())
                    {
                        int count = player.Guns.Count;
                        if (e.KeyCode == Bind.Reloading)
                        {
                            if (player.UseItem) return;
                            if (player.GetCurrentGun().AmmoCount != player.GetCurrentGun().CartridgesClip && player.GetCurrentGun().AmmoInStock > 0)
                            {
                                PressedR = true;
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
                                Controller.PlayGameSound(GunsSoundsDict[player.GetCurrentGun().GetType()][player.GetCurrentGun().GetLevel(), sound]);
                                reload_timer.Start();
                            }
                        }
                        if (e.KeyCode == Bind.Item)
                        {
                            if (player.DisposableItem == null)
                                Controller.ChangeItem(player.SelectedItem);
                            if (player.DisposableItems.Count > 0 && player.DisposableItem.HasIt)
                            {
                                if (player.UseItem) return;
                                if (player.EffectCheck(player.GetEffectID())) return;
                                if ((player.SelectedItem == 0 || player.SelectedItem == 3) && player.HP == player.MaxHP) return;
                                TakeFlashlight(false);
                                Controller.DrawItem();
                                new Thread(() =>
                                {
                                    Thread.Sleep(150);
                                    player.ItemFrame = 1;
                                }).Start();
                                Controller.PlayGameSound(GunsSoundsDict[player.DisposableItem.GetType()][player.DisposableItem.GetLevel(), 0]);
                                Controller.UseItem();
                            }
                        }
                        if (e.KeyCode == Bind.Select_item)
                        {
                            if (Controller.IsMultiplayer()) return;
                            if (!player.InSelectingMode || player.UseItem)
                            {
                                int x = Width / 2, y = Height / 2;
                                if (player.SelectedItem == 0) x = 0;
                                else if (player.SelectedItem == 1) y = 0;
                                else if (player.SelectedItem == 2) x = Width;
                                else if (player.SelectedItem == 3) y = Height;
                                player.BlockCamera = true;
                                player.InSelectingMode = true;
                                Cursor.Position = SLILDisplay.PointToScreen(new Point(x, y));
                            }
                        }
                        if (e.KeyCode == Keys.D1)
                        {
                            TakeFlashlight(false);
                            ChangeWeapon(1);
                        }
                        if (e.KeyCode == Keys.D2)
                        {
                            TakeFlashlight(false);
                            ChangeWeapon(2);
                        }
                        if (e.KeyCode == Keys.D3 && player.WeaponSlot_0 != -1)
                        {
                            TakeFlashlight(false);
                            ChangeWeapon(player.WeaponSlot_0);
                        }
                        if (e.KeyCode == Keys.D4 && player.WeaponSlot_1 != -1)
                        {
                            TakeFlashlight(false);
                            ChangeWeapon(player.WeaponSlot_1);
                        }
                    }
                }
                if (!Controller.IsMultiplayer() && !Controller.InBackrooms() && e.KeyCode == Keys.Oemtilde && !ShopOpen && MainMenu.ConsoleEnabled)
                {
                    ConsolePanel.Visible = !ConsolePanel.Visible;
                    ShowMap = false;
                    if (ConsolePanel.Visible)
                    {
                        mouse_timer.Stop();
                        ConsolePanel.player = GetPlayer();
                        ConsolePanel.ClearCommand();
                        ConsolePanel.console.Focus();
                        ConsolePanel.BringToFront();
                    }
                    else
                    {
                        mouse_timer.Start();
                        ConsolePanel.ClearCommand();
                        SLILDisplay.Focus();
                        int x = SLILDisplay.PointToScreen(Point.Empty).X + (SLILDisplay.Width / 2);
                        int y = SLILDisplay.PointToScreen(Point.Empty).Y + (SLILDisplay.Height / 2);
                        Cursor.Position = new Point(x, y);
                    }
                    Scope[ScopeType] = GetScope(Scope[ScopeType]);
                    HScope[ScopeType] = GetScope(HScope[ScopeType]);
                    ScopeShotgun[ScopeType] = GetScope(ScopeShotgun[ScopeType]);
                    HScopeShotgun[ScopeType] = GetScope(HScopeShotgun[ScopeType]);
                }
            }
        }

        private void SLIL_KeyUp(object sender, KeyEventArgs e)
        {
            Player player = GetPlayer();
            if (e.KeyCode == Bind.Run)
            {
                RunKeyPressed = false;
                if (player != null)
                {
                    if (player.InTransport)
                    {
                        if (player.TRANSPORT is Bike)
                            DISPLAYED_MAP[GetCoordinate(player.X, player.Y)] = '5';
                        Controller.GettingOffTheTransport();
                    }
                    else
                    {
                        player.PlayerMoveStyle = Directions.WALK;
                        chill_timer.Start();
                    }
                }
            }
            if (e.KeyCode == Bind.Forward)
            {
                if (player == null || player.PlayerDirection != Directions.FORWARD) return;
                player.PlayerDirection = Directions.STOP;
            }
            if (e.KeyCode == Bind.Back)
            {
                if (player == null || player.PlayerDirection != Directions.BACK) return;
                player.PlayerDirection = Directions.STOP;
            }
            if (e.KeyCode == Bind.Left)
            {
                if (player == null || player.StrafeDirection != Directions.LEFT) return;
                player.StrafeDirection = Directions.STOP;
            }
            if (e.KeyCode == Bind.Right)
            {
                if (player == null || player.StrafeDirection != Directions.RIGHT) return;
                player.StrafeDirection = Directions.STOP;
            }
            if ((e.KeyCode == Bind.Interaction_0 || e.KeyCode == Bind.Interaction_1) && ShowSing)
            {
                ShowSing = player.BlockInput = player.BlockCamera = false;
                return;
            }
            if (e.KeyCode == Bind.Inventory && ShowInventory)
            {
                CloseInventory();
                return;
            }
            if (GameStarted && !Paused && !ConsolePanel.Visible && !ShopOpen)
            {
                if (e.KeyCode == Bind.Screenshot) DoScreenshot();
                if (e.KeyCode == Bind.ScreenRecording) StartStopScreenRecording();
                if ((e.KeyCode == Bind.Show_map_0 || e.KeyCode == Bind.Show_map_1) && !player.BlockInput)
                {
                    if (!ShowSing)
                    {
                        ShowMap = !ShowMap;
                        Activate();
                    }
                }
                if (player == null) return;
                if (!shot_timer.Enabled && !reload_timer.Enabled && !shotgun_pull_timer.Enabled && !player.BlockInput && !player.IsPetting)
                {
                    if (player.InTransport || player.UseItem || Controller.InBackrooms()) return;
                    if (e.KeyCode == Bind.Kick) Controller.DidKick();
                    if (e.KeyCode == Bind.Flashlight)
                    {
                        if (player.GetCurrentGun() is Flashlight) TakeFlashlight(false);
                        else TakeFlashlight(true);
                    }
                    if (e.KeyCode == Bind.Climb)
                    {
                        double rayA = player.A + FOV / 2 - (SCREEN_WIDTH / 2) * FOV / SCREEN_WIDTH;
                        double ray_x = Math.Sin(rayA);
                        double ray_y = Math.Cos(rayA);
                        double distance = 0;
                        bool hit = false;
                        while (raycast.Enabled && !hit && distance <= 1)
                        {
                            distance += 0.1d;
                            int x = (int)(player.X + ray_x * distance);
                            int y = (int)(player.Y + ray_y * distance);
                            char test_wall = Controller.GetMap()[GetCoordinate(x, y)];
                            switch (test_wall)
                            {
                                case '=':
                                case 'R':
                                    while (raycast.Enabled && !hit && distance <= 2)
                                    {
                                        distance += 0.1d;
                                        int x1 = (int)(player.X + ray_x * distance);
                                        int y1 = (int)(player.Y + ray_y * distance);
                                        if (!HasImpassibleCells(GetCoordinate(x1, y1)))
                                        {
                                            Controller.DoParkour(y, x);
                                            break;
                                        }
                                    }
                                    hit = true;
                                    break;
                                case '#':
                                case 'F':
                                case 'D':
                                case 'd':
                                case 'o':
                                case 'S':
                                    hit = true;
                                    break;
                            }
                        }
                        if (player.GetMoveSpeed(ElapsedTime) < 0 || player.GetStrafeSpeed(ElapsedTime) != 0)
                        {
                            if (!PlayerCanDodge()) return;
                            double dodgeAngle = player.GetMoveSpeed(ElapsedTime) < 0 ? ML.NormalizeAngle(player.A + Math.PI) : player.A;
                            double strafeSpeed = player.GetStrafeSpeed(ElapsedTime);
                            if (strafeSpeed != 0)
                            {
                                if (player.GetMoveSpeed(ElapsedTime) < 0) dodgeAngle += (strafeSpeed > 0 ? -1 : 1) * (Math.PI / 2);
                                else dodgeAngle += (strafeSpeed > 0 ? 1 : -1) * (Math.PI / 2);
                            }
                            Controller.DoDodge(Math.Sin(dodgeAngle), Math.Cos(dodgeAngle));
                            return;
                        }
                    }
                    if (e.KeyCode == Bind.Select_item)
                    {
                        player.InSelectingMode = false;
                        Cursor.Position = SLILDisplay.PointToScreen(new Point(SLILDisplay.Width / 2, SLILDisplay.Height / 2));
                        player.CanUnblockCamera = true;
                    }
                    if (e.KeyCode == Bind.Inventory) OpenInventory();
                    if (e.KeyCode == Bind.Interaction_0 || e.KeyCode == Bind.Interaction_1)
                    {
                        double[] ZBuffer = new double[SCREEN_WIDTH];
                        double[] ZBufferWindow = new double[SCREEN_WIDTH];
                        Pixel[][] rays = CastRaysParallel(ZBuffer, ZBufferWindow);
                        List<Entity> Entities = Controller.GetEntities();
                        int entityCount = Entities.Count;
                        var spriteInfo = new (int Order, double Distance)[entityCount];
                        for (int i = 0; i < entityCount; i++)
                        {
                            double dx = player.X - Entities[i].X;
                            double dy = player.Y - Entities[i].Y;
                            spriteInfo[i] = (i, dx * dx + dy * dy);
                        }
                        Array.Sort(spriteInfo, (b, a) => b.Distance.CompareTo(a.Distance));
                        for (int i = 0; i < Entities.Count; i++)
                        {
                            Entity entity = Entities[spriteInfo[i].Order];
                            if (!entity.HasAI) continue;
                            if (entity is Creature creature && creature.Dead) continue;
                            if (!(entity is Enemy))
                            {
                                double spriteX = entity.X - player.X;
                                double spriteY = entity.Y - player.Y;
                                double transformX = invDet * (dirY * spriteX - dirX * spriteY);
                                double transformY = invDet * (-planeY * spriteX + planeX * spriteY);
                                int spriteScreenX = (int)((SCREEN_WIDTH / 2) * (1 + transformX / transformY));
                                double Distance = Math.Sqrt((player.X - entity.X) * (player.X - entity.X) + (player.Y - entity.Y) * (player.Y - entity.Y));
                                if (Distance == 0) Distance = 0.01;
                                double spriteTop = (SCREEN_HEIGHT - player.Look) / 2 - (SCREEN_HEIGHT * FOV) / Distance;
                                double spriteBottom = SCREEN_HEIGHT - (spriteTop + player.Look);
                                int spriteCenterY = (int)((spriteTop + spriteBottom) / 2);
                                int drawStartY = (int)spriteTop;
                                int drawEndY = (int)spriteBottom;
                                int spriteHeight = Math.Abs((int)(SCREEN_HEIGHT / Distance));
                                int spriteWidth = Math.Abs((int)(SCREEN_WIDTH / Distance));
                                int drawStartX = -spriteWidth / 2 + spriteScreenX;
                                if (drawStartX < 0) drawStartX = 0;
                                int drawEndX = spriteWidth / 2 + spriteScreenX;
                                if (drawEndX >= SCREEN_WIDTH) drawEndX = SCREEN_WIDTH;
                                var timeNow = (long)((DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds * 2);
                                for (int stripe = drawStartX; stripe < drawEndX; stripe++)
                                {
                                    int texWidth = 128;
                                    double texX = (double)((256 * (stripe - (-spriteWidth / 2 + spriteScreenX)) * texWidth / spriteWidth) / 256) / texWidth;
                                    if (transformY > 0 && stripe > 0 && stripe < SCREEN_WIDTH && transformY < ZBuffer[stripe])
                                    {
                                        for (int y = drawStartY; y < drawEndY && y < SCREEN_HEIGHT; y++)
                                        {
                                            if (y < 0 || (transformY > ZBufferWindow[stripe] && y > spriteCenterY))
                                                continue;
                                            double d = y - (SCREEN_HEIGHT - (int)player.Look) / 2 + (drawEndY - drawStartY) / 2;
                                            double texY = d / (drawEndY - drawStartY);
                                            if (y == drawStartY) texY = 0;
                                            if (rays[stripe].Length > y && y >= 0)
                                            {
                                                if (player.GetCurrentGun() is Flashlight && entity.RespondsToFlashlight)
                                                    rays[stripe][y].SpriteState = SpriteStates.FlashlightBlinded;
                                                else
                                                    rays[stripe][y].SpriteState = GetSpriteRotation(entity, timeNow);
                                                rays[stripe][y].TextureId = entity.Texture;
                                                rays[stripe][y].Blackout = 0;
                                                rays[stripe][y].TextureX = texX;
                                                rays[stripe][y].TextureY = texY;
                                                Color color = GetColorForPixel(rays[stripe][y]);
                                                if (color != Color.Transparent && stripe == SCREEN_WIDTH / 2 && y == SCREEN_HEIGHT / 2 && Distance <= 2)
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
                                                            return;
                                                        case 2: //GreenGnome
                                                            if (player.IsPetting) break;
                                                            player.IsPetting = true;
                                                            new Thread(() =>
                                                            {
                                                                Thread.Sleep(2000);
                                                                player.IsPetting = false;
                                                            }).Start();
                                                            return;
                                                        case 3: //EnergyDrink
                                                            if (player.IsPetting) break;
                                                            player.IsPetting = true;
                                                            new Thread(() =>
                                                            {
                                                                Thread.Sleep(2000);
                                                                player.IsPetting = false;
                                                            }).Start();
                                                            return;
                                                        case 4: //Transport
                                                            Controller.PlayGameSound(Starter);
                                                            Controller.GetOnATransport(entity.ID);
                                                            return;
                                                        case 5:
                                                            ShowShop();
                                                            return;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        double rayA = player.A + FOV / 2 - (SCREEN_WIDTH / 2) * FOV / SCREEN_WIDTH;
                        double ray_x = Math.Sin(rayA);
                        double ray_y = Math.Cos(rayA);
                        double distance = 0;
                        bool hit = false;
                        while (raycast.Enabled && !hit && distance <= 2)
                        {
                            distance += 0.1d;
                            int x = (int)(player.X + ray_x * distance);
                            int y = (int)(player.Y + ray_y * distance);
                            char test_wall = Controller.GetMap()[GetCoordinate(x, y)];
                            switch (test_wall)
                            {
                                case '#':
                                case '=':
                                case 'F':
                                    hit = true;
                                    Controller.PlayGameSound(Wall);
                                    break;
                                case 'D':
                                    hit = true;
                                    ShowShop();
                                    break;
                                case 'd':
                                    hit = true;
                                    Controller.InteractingWithDoors(GetCoordinate(x, y));
                                    break;
                                case 'o':
                                    hit = true;
                                    if (distance < playerWidth || ((int)player.X == x && (int)player.Y == y)) break;
                                    Controller.InteractingWithDoors(GetCoordinate(x, y));
                                    break;
                                case 'S':
                                    hit = true;
                                    SingID = GetCoordinate(x, y);
                                    scrollPosition = 0;
                                    ShowSing = player.BlockInput = player.BlockCamera = true;
                                    break;
                            }
                        }
                    }
                }
            }
        }

        private void Display_MouseMove(object sender, MouseEventArgs e)
        {
            if (GameStarted && active && !ConsolePanel.Visible && !shop_panel.Visible)
            {
                Player player = GetPlayer();
                if (player == null || player.BlockMouse) return;
                double x = SLILDisplay.Width / 2, y = SLILDisplay.Height / 2;
                double X = e.X - x, Y = e.Y - y;
                CursorX = (int)X; CursorY = (int)Y;
                if (!player.InSelectingMode)
                {
                    int invY = InvY ? -1 : 1;
                    int invX = InvX ? -1 : 1;
                    double A = -(((X / x) / 10) * LookSpeed) * 2.5;
                    double Look = (((Y / y) * 20) * LookSpeed) * 2.5;
                    Controller.ChangePlayerA(A * invX);
                    Controller.ChangePlayerLook(Look * invY);
                    Cursor.Position = SLILDisplay.PointToScreen(new Point((int)x, (int)y));
                }
                else
                {
                    if (player.DisposableItems.Count >= 1)
                    {
                        if (X < 0 && Math.Abs(X) > Math.Abs(Y))
                            Controller.ChangeItem(0);
                        else if (Y < 0 && Math.Abs(Y) > Math.Abs(X) && player.DisposableItems.Count >= 2)
                            Controller.ChangeItem(1);
                        else if (X > 0 && Math.Abs(X) > Math.Abs(Y) && player.DisposableItems.Count >= 3)
                            Controller.ChangeItem(2);
                        else if (Y > 0 && Math.Abs(Y) > Math.Abs(X) && player.DisposableItems.Count >= 4)
                            Controller.ChangeItem(3);
                    }
                }
            }
        }

        private void Display_Scroll(object sender, MouseEventArgs e)
        {
            Player player = GetPlayer();
            double delta = e.Delta / 10;
            if (ShowSing) UpdateScrollPosition(-delta);
            if (GameStarted && !Paused && !player.BlockInput && player.CanShoot && !shot_timer.Enabled && !reload_timer.Enabled && !shotgun_pull_timer.Enabled && !player.IsPetting)
            {
                List<int> availableWeapons = new List<int>();
                if (player.WeaponSlot_0 != -1 && !player.CuteMode) availableWeapons.Add(player.WeaponSlot_0);
                if (player.WeaponSlot_1 != -1 && !player.CuteMode) availableWeapons.Add(player.WeaponSlot_1);
                availableWeapons.Add(2);
                availableWeapons.Add(1);
                int currentIndex = availableWeapons.IndexOf(player.CurrentGun);
                if (currentIndex == -1) currentIndex = 2;
                int newIndex;
                if (delta > 0)
                    newIndex = (currentIndex + 1) % availableWeapons.Count;
                else
                    newIndex = (currentIndex - 1 + availableWeapons.Count) % availableWeapons.Count;
                int new_gun = availableWeapons[newIndex];
                TakeFlashlight(false);
                if (new_gun == 1 || new_gun == 2)
                    ChangeWeapon(new_gun);
                else if (new_gun == player.WeaponSlot_0)
                    ChangeWeapon(player.WeaponSlot_0);
                else if (new_gun == player.WeaponSlot_1)
                    ChangeWeapon(player.WeaponSlot_1);
            }
        }

        private void Display_MouseDown(object sender, MouseEventArgs e)
        {
            if (!Controller.IsInSpectatorMode())
            {
                Player player = GetPlayer();
                if (GameStarted && !Paused && !player.BlockInput && !player.InSelectingMode && !player.IsPetting &&
                    !shotgun_pull_timer.Enabled && !shot_timer.Enabled && !Controller.IsInSpectatorMode() &&
                    !Controller.InBackrooms())
                {
                    if (e.Button == MouseButtons.Left)
                    {
                        HoldLMB = true;
                        if (player.GetCurrentGun() is Shotgun && player.GetCurrentGun().Level != Levels.LV1 && reload_timer.Enabled)
                            CancelReload = true;
                        else if (!reload_timer.Enabled && !mouse_hold_timer.Enabled && player.CanShoot && player.GetCurrentGun().CanShoot)
                        {
                            if (Shoot(player)) mouse_hold_timer.Start();
                        }
                    }
                    else if (e.Button == MouseButtons.Right && !reload_timer.Enabled)
                    {
                        if (player.GetCurrentGun().CanAiming && player.CanShoot && player.GetCurrentGun().CanShoot)
                        {
                            Controller.PlayGameSound(GunsSoundsDict[player.GetCurrentGun().GetType()][player.GetCurrentGun().GetLevel(), 3]);
                            player.Aiming = !player.Aiming;
                            player.GunState = player.Aiming ? player.GetCurrentGun().AimingState : 0;
                        }
                    }
                }
                if (ShowSing) ShowSing = player.BlockInput = player.BlockCamera = false;
            }
            else
            {
                if (e.Button == MouseButtons.Left)
                    Controller.ChangeSpectatedPlayer(1);
                else if (e.Button == MouseButtons.Right)
                    Controller.ChangeSpectatedPlayer(2);
            }
        }

        private void Display_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                HoldLMB = false;
        }

        //  #====    Move Player    ====#

        private bool HasImpassibleCells(int index)
        {
            char[] impassibleCells = { '#', 'D', '=', 'd', 'S', '$', 'T', 't', 'R' };
            if (Controller.HasNoClip() || GetPlayer().InParkour) return false;
            if (index < 0 || index > Controller.GetMap().Length) return true;
            return impassibleCells.Contains(Controller.GetMap()[index]);
        }

        private void PlayerMove()
        {
            Player player = GetPlayer();
            if (player == null || player.Aiming) return;
            if (Controller.GetMap()[GetCoordinate(player.X, player.Y)] == 'P')
            {
                Controller.GetMap()[GetCoordinate(player.X, player.Y)] = '.';
                DISPLAYED_MAP[GetCoordinate(player.X, player.Y)] = '.';
            }
            if (player.X < 0) player.X = 1.5;
            if (player.X >= Controller.GetMapWidth() - 0.5) player.X = Controller.GetMapWidth() - 1.5;
            if (player.Y < 0) player.Y = 1.5;
            if (player.Y >= Controller.GetMapHeight() - 0.5) player.Y = Controller.GetMapHeight() - 1.5;
            if (HasImpassibleCells(GetCoordinate(player.X, player.Y)))
            {
                double x = player.X;
                double y = player.Y;
                GetCoordinateWithoutWall(ref x, ref y);
                if (Math.Abs(x - player.X) > 0.001 || Math.Abs(y - player.Y) > 0.001)
                {
                    if (!player.BlockInput)
                        Controller.MovePlayer(x - player.X, y - player.Y);
                }
            }
            DISPLAYED_MAP.Replace('P', '.');
            player.ChangeSpeed();
            double move = player.GetMoveSpeed(ElapsedTime);
            double strafe = player.GetStrafeSpeed(ElapsedTime);
            double moveSin = Math.Sin(player.A) * move;
            double moveCos = Math.Cos(player.A) * move;
            double strafeSin = Math.Sin(player.A) * strafe;
            double strafeCos = Math.Cos(player.A) * strafe;
            double newX = player.X;
            double newY = player.Y;
            double tempX = player.X;
            double tempY = player.Y;
            double factor = 1;
            if (player.MoveSpeed < 0)
            {
                if (player.InTransport) factor = 0.25;
                else factor = 0.65;
            }
            newX += moveSin * factor;
            newY += moveCos * factor;
            if (!player.InTransport)
            {
                newX += strafeCos;
                newY -= strafeSin;
            }
            else
            {
                double turnAmount = 0;
                if (player.MoveSpeed < 0)
                    turnAmount = (-player.StrafeSpeed * 0.5) / (player.TRANSPORT.Controllability + 25);
                else if (player.MoveSpeed > 0)
                    turnAmount = (player.StrafeSpeed) / player.TRANSPORT.Controllability;
                Controller.ChangePlayerA(turnAmount * DeltaTime * 60);
            }
            if (!(HasImpassibleCells(GetCoordinate(newX + playerWidth / 2, newY))
                || HasImpassibleCells(GetCoordinate(newX - playerWidth / 2, newY))))
                tempX = newX;
            if (!(HasImpassibleCells(GetCoordinate(newX, newY + playerWidth / 2))
                || HasImpassibleCells(GetCoordinate(newX, newY - playerWidth / 2))))
                tempY = newY;
            if (HasImpassibleCells(GetCoordinate(tempX + playerWidth / 2, tempY)))
                tempX -= playerWidth / 2 - (1 - tempX % 1);
            if (HasImpassibleCells(GetCoordinate(tempX - playerWidth / 2, tempY)))
                tempX += playerWidth / 2 - (tempX % 1);
            if (HasImpassibleCells(GetCoordinate(tempX, tempY + playerWidth / 2)))
                tempY -= playerWidth / 2 - (1 - tempY % 1);
            if (HasImpassibleCells(GetCoordinate(tempX, tempY - playerWidth / 2)))
                tempY += playerWidth / 2 - (tempY % 1);
            if (Math.Abs(tempX - player.X) > 0.001 || Math.Abs(tempY - player.Y) > 0.001)
            {
                if (!player.BlockInput)
                    Controller.MovePlayer(tempX - player.X, tempY - player.Y);
            }
            if (Controller.GetMap()[GetCoordinate(player.X, player.Y)] == '.')
                DISPLAYED_MAP[GetCoordinate(player.X, player.Y)] = 'P';
        }

        private void GetCoordinateWithoutWall(ref double x, ref double y)
        {
            for (double offsetX = -3.5; offsetX <= 3.5; offsetX += 0.5)
            {
                for (double offsetY = -3.5; offsetY <= 3.5; offsetY += 0.5)
                {
                    if (!HasImpassibleCells(GetCoordinate(x + offsetX, y + offsetY)))
                    {
                        x += offsetX;
                        y += offsetY;
                        return;
                    }
                }
            }
        }

        //  #====     RayCasting    ====#

        private void ClearDisplayedMap()
        {
            Player player = GetPlayer();
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
                    int index = GetCoordinate(x, y);
                    if (DISPLAYED_MAP[index] == '*' || DISPLAYED_MAP[index] == 'E')
                        DISPLAYED_MAP[index] = '.';
                }
            }
        }

        private Pixel[][] CastRaysParallel(double[] ZBuffer, double[] ZBufferWindow)
        {
            Pixel[][] rays = new Pixel[SCREEN_WIDTH][];
            Player player = GetPlayer();
            if (player == null || !GameStarted)
            {
                for (int i = 0; i < SCREEN_WIDTH; i++)
                {
                    rays[i] = new Pixel[SCREEN_HEIGHT];
                    for (int j = 0; j < SCREEN_HEIGHT; j++)
                        rays[i][j] = new Pixel(i, j, 100, 1, 1, 0, SpriteStates.Static);
                }
                return rays;
            }
            dirX = Math.Sin(player.A);
            dirY = Math.Cos(player.A);
            planeX = Math.Sin(player.A - Math.PI / 2) * Math.Tan(FOV / 2);
            planeY = Math.Cos(player.A - Math.PI / 2) * Math.Tan(FOV / 2);
            int mapX = (int)(player.X);
            int mapY = (int)(player.Y);
            StringBuilder MAP = Controller.GetMap();
            int MAP_WIDTH = Controller.GetMapWidth();
            Parallel.For(0, SCREEN_WIDTH, x => rays[x] = CastRay(x, ZBuffer, ZBufferWindow, mapX, mapY, ref player, ref MAP, MAP_WIDTH));
            return rays;
        }

        private Pixel[] CastRay(int x, double[] ZBuffer, double[] ZBufferWindow, int mapX, int mapY, ref Player player, ref StringBuilder MAP, int MAP_WIDTH)
        {
            Pixel[] result = new Pixel[SCREEN_HEIGHT];
            double cameraX = 2 * x / (double)SCREEN_WIDTH - 1;
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
            int hit_wall = -1;
            bool hit_window = false;
            bool hit_door = false;
            bool is_bound = false;
            bool is_window_bound = false;
            int windowSide = 0;
            while (raycast.Enabled && hit_wall == -1 && !hit_door)
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
                if (mapX < 0 || mapX >= player.GetDrawDistance() + player.X || mapY < 0 || mapY >= player.GetDrawDistance() + player.Y || distance >= player.GetDrawDistance())
                {
                    hit_wall = 0;
                    distance = player.GetDrawDistance();
                    continue;
                }
                char test_wall = MAP[mapY * MAP_WIDTH + mapX];
                switch (test_wall)
                {
                    case '#':
                        hit_wall = 0;
                        DISPLAYED_MAP[mapY * MAP_WIDTH + mapX] = '#';
                        break;
                    case '=':
                        hit_window = true;
                        DISPLAYED_MAP[mapY * MAP_WIDTH + mapX] = '=';
                        break;
                    case 'S':
                        hit_wall = 1;
                        DISPLAYED_MAP[mapY * MAP_WIDTH + mapX] = 'S';
                        break;
                    case 'd':
                        hit_door = true;
                        DISPLAYED_MAP[mapY * MAP_WIDTH + mapX] = 'D';
                        break;
                    case '@':
                        hit_wall = 2;
                        DISPLAYED_MAP[mapY * MAP_WIDTH + mapX] = 'T';
                        break;
                    default:
                        if (test_wall == '.') test_wall = '*';
                        DISPLAYED_MAP[mapY * MAP_WIDTH + mapX] = test_wall;
                        break;
                }
            }
            double ceiling = (SCREEN_HEIGHT - player.Look) / 2 - (SCREEN_HEIGHT * FOV) / distance;
            double floor = SCREEN_HEIGHT - (ceiling + player.Look);
            double mid = (ceiling + floor) / 2;
            bool get_texture = false, get_texture_window = false;
            int side = 0;
            double wallX = 0;
            if (wallSide == 1) wallX = player.X + distance * rayDirX;
            else if (wallSide == 0) wallX = player.Y + distance * rayDirY;
            wallX -= Math.Floor(wallX);
            double windowX = 0;
            if (windowSide == 1) windowX = player.X + window_distance * rayDirX;
            else if (windowSide == 0) windowX = player.Y + window_distance * rayDirY;
            windowX -= Math.Floor(windowX);
            if (ShowBounds())
            {
                if (wallX > 0.97 || wallX < 0.03) is_bound = true;
                if (windowX > 0.97 || windowX < 0.03) is_window_bound = true;
            }
            for (int y = 0; y < SCREEN_HEIGHT; y++)
            {
                if (!GameStarted) break;
                int blackout = 0, textureId = 1;
                if (hit_window && y > mid)
                {
                    ceiling = (SCREEN_HEIGHT - player.Look) / 2 - (SCREEN_HEIGHT * FOV) / window_distance;
                    floor = SCREEN_HEIGHT - (ceiling + player.Look);
                }
                else
                {
                    ceiling = (SCREEN_HEIGHT - player.Look) / 2 - (SCREEN_HEIGHT * FOV) / distance;
                    floor = SCREEN_HEIGHT - (ceiling + player.Look);
                }
                if (y >= mid && y <= floor && hit_window)
                {
                    textureId = 4;
                    if (Controller.InBackrooms())
                    {
                        if (Controller.GetBackroomsStage() == 0) textureId = 29;
                        if (Controller.GetBackroomsStage() == 1) textureId = 38;
                    }
                    if (Math.Abs(y - mid) <= 6 / window_distance || is_window_bound)
                    {
                        textureId = 0;
                        if (Controller.InBackrooms())
                        {
                            if (Controller.GetBackroomsStage() == 0) textureId = 2;
                            if (Controller.GetBackroomsStage() == 1) textureId = 3;
                        }
                    }
                    blackout = (int)Math.Floor((window_distance / player.GetDrawDistance()) * 100);
                }
                else if ((y < mid || !hit_window) && y > ceiling && y < floor)
                {
                    textureId = 4;
                    if (Controller.InBackrooms())
                    {
                        if (Controller.GetBackroomsStage() == 0) textureId = 29;
                        if (Controller.GetBackroomsStage() == 1) textureId = 38;
                    }
                    if (hit_wall == 1) textureId = 20;
                    if (hit_wall == 2) textureId = 1;
                    if (hit_door) textureId = 5;
                    if (is_bound)
                    {
                        textureId = 0;
                        if (Controller.InBackrooms())
                        {
                            if (Controller.GetBackroomsStage() == 0) textureId = 2;
                            if (Controller.GetBackroomsStage() == 1) textureId = 3;
                        }
                    }
                    blackout = (int)Math.Floor((distance / player.GetDrawDistance()) * 100);
                }
                result[y] = new Pixel(x, y, blackout, distance, ceiling - floor, textureId, SpriteStates.Static);
                if (y <= ceiling)
                {
                    int p = y - (int)(SCREEN_HEIGHT - player.Look) / 2;
                    double rowDistance = (double)SCREEN_HEIGHT / p;
                    double floorX = player.X - rowDistance * rayDirX;
                    double floorY = player.Y - rowDistance * rayDirY;
                    if (floorX < 0) floorX = 0;
                    if (floorY < 0) floorY = 0;
                    result[y].TextureId = 8;
                    if (Controller.InBackrooms())
                    {
                        if (Controller.GetBackroomsStage() == 0) result[y].TextureId = 30;
                        if (Controller.GetBackroomsStage() == 1) result[y].TextureId = 40;
                    }
                    result[y].Blackout = (int)Math.Floor((-rowDistance / player.GetDrawDistance()) * 100);
                    result[y].TextureX = floorX % 1;
                    result[y].TextureY = floorY % 1;
                    result[y].Side = 0;
                }
                else if (y >= floor)
                {
                    int p = y - (int)(SCREEN_HEIGHT - player.Look) / 2;
                    double rowDistance = (double)SCREEN_HEIGHT / p;
                    double floorX = player.X + rowDistance * rayDirX;
                    double floorY = player.Y + rowDistance * rayDirY;
                    if (floorX < 0) floorX = 0;
                    if (floorY < 0) floorY = 0;
                    result[y].TextureId = 7;
                    if (Controller.InBackrooms())
                    {
                        if (Controller.GetBackroomsStage() == 0) result[y].TextureId = 31;
                        if (Controller.GetBackroomsStage() == 1) result[y].TextureId = 39;
                    }
                    result[y].Blackout = (int)Math.Floor((rowDistance / player.GetDrawDistance()) * 100);
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
                            if (side == -1) result[y].TextureId = 0;
                        }
                    }
                    else if (hit_door || (hit_wall != -1 && hit_wall != 2))
                    {
                        if (!get_texture)
                        {
                            get_texture = true;
                            side = wallSide;
                            if (side == -1) result[y].TextureId = 0;
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

        private bool ShowBounds()
        {
            if (Controller.InBackrooms() && Controller.GetBackroomsStage() == 1) return false;
            return true;
        }

        private void DrawSprites(ref Pixel[][] rays, ref double[] ZBuffer, ref double[] ZBufferWindow, out List<int> enemiesCoords)
        {
            Player player = GetPlayer();
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
            var timeNow = (long)((DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds * 2);
            for (int i = 0; i < spriteInfo.Length; i++)
            {
                try
                {
                    Entity entity = Entities[spriteInfo[i].Order];
                    if (entity is Player pl && player.ID == pl.ID) continue;
                    double Distance = ML.GetDistance(new TPoint(entity.X, entity.Y), new TPoint(player.X, player.Y));
                    if (Distance >= player.GetDrawDistance() + 1 || Distance == 0) continue;
                    double spriteX = entity.X - player.X;
                    double spriteY = entity.Y - player.Y;
                    double transformX = invDet * (dirY * spriteX - dirX * spriteY);
                    double transformY = invDet * (-planeY * spriteX + planeX * spriteY);
                    int spriteScreenX = (int)((SCREEN_WIDTH / 2) * (1 + transformX / transformY));
                    double spriteTop = (SCREEN_HEIGHT - player.Look) / 2 - (SCREEN_HEIGHT * FOV) / Distance;
                    double spriteBottom = SCREEN_HEIGHT - (spriteTop + player.Look);
                    int spriteCenterY = (int)((spriteTop + spriteBottom) / 2);
                    int drawStartY = (int)spriteTop;
                    int drawEndY = (int)spriteBottom;
                    int spriteHeight = Math.Abs((int)(SCREEN_HEIGHT / Distance));
                    int spriteWidth = Math.Abs((int)(SCREEN_WIDTH / Distance));
                    double vMove = entity.VMove;
                    int vMoveScreen = (int)(vMove / transformY);
                    int drawStartX = -spriteWidth / 2 + spriteScreenX + vMoveScreen;
                    if (drawStartX < 0) drawStartX = 0;
                    int drawEndX = spriteWidth / 2 + spriteScreenX + vMoveScreen;
                    if (drawEndX >= SCREEN_WIDTH) drawEndX = SCREEN_WIDTH;
                    for (int stripe = drawStartX; stripe < drawEndX; stripe++)
                    {
                        double texX = (double)((256 * (stripe - (-spriteWidth / 2 + spriteScreenX)) * TexWidth / spriteWidth) / 256) / TexWidth;
                        if (!(transformY > 0 && stripe >= 0 && stripe <= SCREEN_WIDTH && transformY < ZBuffer[stripe])) continue;
                        for (int y = drawStartY; y < drawEndY && y < SCREEN_HEIGHT; y++)
                        {
                            if (y < 0 || (transformY > ZBufferWindow[stripe] && y > spriteCenterY)) continue;
                            double d = (y - vMoveScreen) - (SCREEN_HEIGHT - (int)player.Look) / 2 + (drawEndY - drawStartY) / 2;
                            double texY = d / (drawEndY - drawStartY);
                            if (y == drawStartY) texY = 0;
                            if (!(rays[stripe].Length > y && y >= 0)) continue;
                            int tempTextureId = rays[stripe][y].TextureId;
                            int tempBlackout = rays[stripe][y].Blackout;
                            double tempTextureX = rays[stripe][y].TextureX;
                            double tempTextureY = rays[stripe][y].TextureY;
                            SpriteStates tempSpriteState = rays[stripe][y].SpriteState;
                            if (entity is Creature creature)
                            {
                                if (!creature.Dead)
                                {
                                    if (!(player.GetCurrentGun() is Flashlight && creature.RespondsToFlashlight) && entity is Pet pet && pet.Stoped && pet.HasStopAnimation)
                                        rays[stripe][y].SpriteState = GetSpriteRotation(creature, 0, false, true);
                                    else
                                    {
                                        if (player.GetCurrentGun() is Flashlight && creature.RespondsToFlashlight)
                                            rays[stripe][y].SpriteState = SpriteStates.FlashlightBlinded;
                                        else
                                            rays[stripe][y].SpriteState = GetSpriteRotation(creature, timeNow);
                                    }
                                    if (creature is Enemy)
                                    {
                                        if (creature is VoidStalker stalker)
                                        {
                                            if (stalker.ISeeU()) Controller.PlayGameSound(VoidStalkerScreamer);
                                        }
                                        int coords = (int)entity.Y * mapWidth + (int)entity.X;
                                        if (!enemiesCoords.Contains(coords))
                                            enemiesCoords.Add(coords);
                                    }
                                }
                                else
                                {
                                    if (creature.RespondsToFlashlight)
                                        rays[stripe][y].SpriteState = SpriteStates.DeadBodyBlinded;
                                    else
                                        rays[stripe][y].SpriteState = SpriteStates.DeadBody;
                                }
                            }
                            else if (entity is Player playerTar)
                            {
                                if (!playerTar.Dead)
                                    rays[stripe][y].SpriteState = GetSpriteRotation(playerTar, timeNow);
                                else
                                    rays[stripe][y].SpriteState = SpriteStates.DeadBody;
                            }
                            else
                            {
                                if (entity is GameObject gameObject)
                                {
                                    if (gameObject.Animated && gameObject.Temporarily)
                                        rays[stripe][y].SpriteState = GetSpriteRotation(gameObject, 0, false);
                                    else
                                        rays[stripe][y].SpriteState = GetSpriteRotation(gameObject, timeNow);
                                }
                                else
                                    rays[stripe][y].SpriteState = SpriteStates.Static;
                            }
                            rays[stripe][y].TextureId = entity.Texture;
                            rays[stripe][y].Blackout = (int)Math.Floor((Distance / player.GetDrawDistance()) * 100);
                            rays[stripe][y].TextureX = texX;
                            rays[stripe][y].TextureY = texY;
                            if (IsTransparent(rays[stripe][y]))
                            {
                                rays[stripe][y].TextureId = tempTextureId;
                                rays[stripe][y].Blackout = tempBlackout;
                                rays[stripe][y].TextureX = tempTextureX;
                                rays[stripe][y].TextureY = tempTextureY;
                                rays[stripe][y].SpriteState = tempSpriteState;
                            }
                        }
                    }
                }
                catch { }
            }
        }

        private bool IsTransparent(Pixel pixel)
        {
            Player player = GetPlayer();
            if (player == null) return true;
            const int textureSize = 128;
            int x = 0, y = 0;
            if (pixel.TextureId >= 4)
            {
                x = ((int)(pixel.TextureX * textureSize)) % textureSize;
                if (x < 0) x += textureSize;
                y = ((int)(pixel.TextureY * textureSize)) % textureSize;
                if (y < 0) y += textureSize;
            }
            bool cuteMode = !Controller.InBackrooms() && player.CuteMode;
            return Textures.IsTransparent(pixel.TextureId, pixel.SpriteState, x, y, cuteMode);
        }

        private SpriteStates GetSpriteRotation(Entity entity, long timeNow, bool useTimeNow = true, bool returnStopState = false)
        {
            int state = 0;
            if (!entity.HasStaticAnimation)
            {
                if (useTimeNow) state = entity.Animations[timeNow % entity.Frames];
                if (entity is GameObject @object && @object.Animated && @object.Temporarily)
                    state = entity.Animations[@object.CurrentFrame];
            }
            if (entity.HasSpriteRotation)
            {
                Player player = GetPlayer();
                double entityRotationAngle = Math.Atan2(player.Y - entity.Y, player.X - entity.X);
                double normalizedEntityAngle = entity.A;
                entityRotationAngle -= normalizedEntityAngle;
                entityRotationAngle = ML.NormalizeAngle(entityRotationAngle);
                if (returnStopState || entity.HasStaticAnimation)
                {
                    if (entityRotationAngle < Math.PI / 4 && entityRotationAngle > -Math.PI / 4)
                        return SpriteStates.StopBack;
                    else if (entityRotationAngle >= Math.PI / 4 && entityRotationAngle < 3 * Math.PI / 4)
                        return SpriteStates.StopLeft;
                    else if (entityRotationAngle >= 3 * Math.PI / 4 || entityRotationAngle < -3 * Math.PI / 4)
                        return SpriteStates.StopForward;
                    else if (entityRotationAngle >= -3 * Math.PI / 4 && entityRotationAngle <= -Math.PI / 4)
                        return SpriteStates.StopRight;
                    return SpriteStates.StopForward;
                }
                else
                {
                    if (entityRotationAngle < Math.PI / 4 && entityRotationAngle > -Math.PI / 4)
                        return state == 0 ? SpriteStates.StepBack_0 : SpriteStates.StepBack_1;
                    else if (entityRotationAngle >= Math.PI / 4 && entityRotationAngle < 3 * Math.PI / 4)
                        return state == 0 ? SpriteStates.StepLeft_0 : SpriteStates.StepLeft_1;
                    else if (entityRotationAngle >= 3 * Math.PI / 4 || entityRotationAngle < -3 * Math.PI / 4)
                        return state == 0 ? SpriteStates.StepForward_0 : SpriteStates.StepForward_1;
                    else if (entityRotationAngle >= -3 * Math.PI / 4 && entityRotationAngle <= -Math.PI / 4)
                        return state == 0 ? SpriteStates.StepRight_0 : SpriteStates.StepRight_1;
                    return state == 0 ? SpriteStates.StepForward_0 : SpriteStates.StepForward_1;
                }
            }
            else
            {
                if (entity.HasStaticAnimation)
                {
                    if (entity is Covering covering)
                    {
                        if (covering.HP > 75) return SpriteStates.Static;
                        else if (covering.HP > 45) return SpriteStates.StepForward_0;
                        else if (covering.HP > 0) return SpriteStates.StepForward_1;
                        else return SpriteStates.DeadBody;
                    }
                    return SpriteStates.Static;
                }
                if (returnStopState) return SpriteStates.StopForward;
                if (entity is RangeEnemy range)
                {
                    if (range.TimeAfterShot > 0) return SpriteStates.Shooted;
                    if (range.ReadyToShot) return SpriteStates.Aiming;
                }
                if (entity is Enemy enemy && enemy.Stage == Enemy.Stages.Escaping)
                {
                    if (state == 0) return SpriteStates.StepEscape_0;
                    return SpriteStates.StepEscape_1;
                }
                if (state == 0) return SpriteStates.StepForward_0;
                return SpriteStates.StepForward_1;
            }
        }

        private Color GetColorForPixel(Pixel pixel)
        {
            Player player = GetPlayer();
            if (player == null) return Color.White;
            const int textureSize = 128;
            int x = 0, y = 0;
            if (pixel.TextureId >= 4)
            {
                x = ((int)(pixel.TextureX * textureSize)) % textureSize;
                if (x < 0) x += textureSize;
                y = ((int)(pixel.TextureY * textureSize)) % textureSize;
                if (y < 0) y += textureSize;
            }
            bool cuteMode = !Controller.InBackrooms() && player.CuteMode;
            return Textures.GetTextureColor(pixel.TextureId, pixel.SpriteState, x, y, pixel.Blackout, cuteMode);
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

        private void UpdateDisplay()
        {
            bool cute = GetPlayer() != null && GetPlayer().CuteMode;
            using (Graphics g = Graphics.FromImage(BUFFER))
            {
                g.Clear(Color.Black);
                g.DrawImage(SCREEN, new Rectangle(0, 0, BUFFER.Width, BUFFER.Height), 0, 0, SCREEN.Width, SCREEN.Height, GraphicsUnit.Pixel, cute ? imageCuteAttributes : imageAttributes);
                g.DrawImage(WEAPON, new Rectangle(0, 0, BUFFER.Width, BUFFER.Height), 0, 0, WEAPON.Width, WEAPON.Height, GraphicsUnit.Pixel, cute ? imageCuteAttributes : imageAttributes);
            }
            SharpDX.Direct2D1.Bitmap dxBitmap = ConvertBitmap.ToDX(BUFFER, SLILDisplay.RenderTarget);
            SLILDisplay.Screen = dxBitmap;
            SLILDisplay.DrawImage();
            dxBitmap?.Dispose();
        }

        private int CalculateFPS()
        {
            int fps = (int)(1.0 / ElapsedTime);
            return fps < 0 ? 0 : fps;
        }

        //  #====       Sings       ====#

        private void DrawTextOnSing(string text)
        {
            RectangleF textRectangle = new RectangleF(ScrollPadding, ScrollPadding, SCREEN_WIDTH - 2 * ScrollPadding - ScrollBarWidth, SCREEN_HEIGHT - 2 * ScrollPadding);
            SizeF textSize = graphicsWeapon.MeasureString(text, ConsolasFont[2, Resolution], SCREEN_WIDTH - 40 - ScrollBarWidth);
            graphicsWeapon.SetClip(textRectangle);
            graphicsWeapon.DrawString(text, ConsolasFont[2, Resolution], blackBrush, new RectangleF(textRectangle.X, textRectangle.Y - scrollPosition, textRectangle.Width, textSize.Height));
            graphicsWeapon.ResetClip();
            DrawScrollBar(textSize.Height, textRectangle.Height);
        }

        private void DrawScrollBar(float contentHeight, float viewportHeight)
        {
            if (contentHeight <= viewportHeight) return;
            float scrollBarHeight = (viewportHeight / contentHeight) * viewportHeight;
            float scrollBarPosition = (scrollPosition / (contentHeight - viewportHeight)) * (viewportHeight - scrollBarHeight);
            RectangleF scrollBarRect = new RectangleF(
                SCREEN_WIDTH - ScrollBarWidth - ScrollPadding / 2,
                ScrollPadding + scrollBarPosition,
                ScrollBarWidth,
                scrollBarHeight);
            graphicsWeapon.FillRectangle(Brushes.Gray, scrollBarRect);
        }

        internal void UpdateScrollPosition(double delta)
        {
            string text = GetTextOnSing();
            RectangleF textRectangle = new RectangleF(ScrollPadding, ScrollPadding, SCREEN_WIDTH - 2 * ScrollPadding - ScrollBarWidth, SCREEN_HEIGHT - 2 * ScrollPadding);
            SizeF textSize = graphicsWeapon.MeasureString(text, ConsolasFont[2, Resolution], SCREEN_WIDTH - 40 - ScrollBarWidth);
            float maxScroll = Math.Max(0, textSize.Height - textRectangle.Height);
            scrollPosition = (int)Math.Max(0, Math.Min(scrollPosition + delta, (int)maxScroll));
        }

        private string DetectingHotkeysInString(string text)
        {
            if (!text.Contains("*")) return text;
            string result = "";
            string[] parts = text.Split('*');
            foreach (string part in parts)
            {
                if (!Bind.ExistKey(part))
                {
                    result += part;
                    continue;
                }
                result += Bind.GetKey(part).ToString().Replace("Key", null).Replace("Return", "Enter");
            }
            return result;
        }

        private string GetTextOnSing()
        {
            if (IsTutorial)
            {
                switch (SingID)
                {
                    case 253:
                        if (MainMenu.DownloadedLocalizationList)
                            return DetectingHotkeysInString(MainMenu.Localizations.GetLString(MainMenu.Language, "6-0"));
                        return DetectingHotkeysInString("About the game:\n" +
                            "SLIL is a game where you'll explore procedurally generated labyrinths, battle dangerous enemies, and find a portal to advance to the next level.\n" +
                            "Arm yourself, be cautious, and don't let the creatures stop your progress towards the goal.");
                    case 255:
                        if (MainMenu.DownloadedLocalizationList)
                            return DetectingHotkeysInString(MainMenu.Localizations.GetLString(MainMenu.Language, "6-1"));
                        return DetectingHotkeysInString("How to play:\n" +
                            "The controls are done using the *forward*, *back*, *left*, and *right* buttons, as well as the mouse.\n" +
                            "To shoot, use the left mouse button, and to aim (for sniper rifles only), use the right mouse button.\n" +
                            "To interact, use the *interaction_0* or *interaction_1* buttons.\n" +
                            "To open/close the map, press *show_map_0* or *show_map_1*.");
                    case 257:
                        if (MainMenu.DownloadedLocalizationList)
                            return DetectingHotkeysInString(MainMenu.Localizations.GetLString(MainMenu.Language, "6-2"));
                        return DetectingHotkeysInString("Weaponry:\n" +
                            "The game has a large arsenal of weapons, pets, and consumables.\n" +
                            "All of these can be purchased at stores that appear randomly on the map and are displayed in pink.\n" +
                            "Pets are companions that follow the player and provide certain bonuses.\n" +
                            "Consumables are one-time-use items that temporarily affect the player.\n" +
                            "To select an item, press *select_item* and hover the mouse cursor over the desired item, and to use it, press *item*.");
                    case 259:
                        if (MainMenu.DownloadedLocalizationList)
                            return DetectingHotkeysInString(MainMenu.Localizations.GetLString(MainMenu.Language, "6-3"));
                        return DetectingHotkeysInString("Stores:\n" +
                            "In the stores, you'll find everything you need for a successful labyrinth run.\n" +
                            "Go inside and stock up on weapons, pets, and consumables.\n" +
                            "For training, you're provided with $500 so you can equip yourself well.\n" +
                            "The store is located across from this sign.");
                    case 166:
                        if (MainMenu.DownloadedLocalizationList)
                            return DetectingHotkeysInString(MainMenu.Localizations.GetLString(MainMenu.Language, "6-4"));
                        return DetectingHotkeysInString("Supplies:\n" +
                            "In the game, there are crates and barrels. By destroying them, you can obtain ammunition or money.\n" +
                            "Crates have a 75% chance to drop ammunition and a 25% chance to drop money.\n" +
                            "Barrels have a 25% chance to drop ammunition and a 75% chance to drop money.");
                    case 170:
                        if (MainMenu.DownloadedLocalizationList)
                            return DetectingHotkeysInString(MainMenu.Localizations.GetLString(MainMenu.Language, "6-5"));
                        return DetectingHotkeysInString("Portal:\n" +
                            "Go through it to advance to the next level.\n" +
                            "Upon transitioning to the next level, you'll receive a cash reward.");
                    case 369:
                        if (MainMenu.DownloadedLocalizationList)
                            return DetectingHotkeysInString(MainMenu.Localizations.GetLString(MainMenu.Language, "6-6"));
                        return DetectingHotkeysInString("Darkness:\n" +
                            "The catacombs are quite a dark place, but you have a flashlight.\n" +
                            "Using the flashlight, you'll see a smaller area but at greater distances.\n" +
                            "To use the flashlight, press *flashlight*.");
                    case 391:
                        if (MainMenu.DownloadedLocalizationList)
                            return DetectingHotkeysInString(MainMenu.Localizations.GetLString(MainMenu.Language, "6-7"));
                        return DetectingHotkeysInString("Monsters:\n" +
                            "On your path, you'll encounter creatures of evil and chaos.\n" +
                            "Enemy types:\n" +
                            "Zombies - flesh possessed by dark forces (slow and weak foe).\n" +
                            "Rabid dogs - feral beings that have lost their minds (fast but not durable).\n" +
                            "Orcs - a warlike race thirsting for blood (sluggish but durable foe).\n" +
                            "Bats - nocturnal creatures of darkness (fast and stealthy threat).");
                    case 359:
                        if (MainMenu.DownloadedLocalizationList)
                            return DetectingHotkeysInString(MainMenu.Localizations.GetLString(MainMenu.Language, "6-8"));
                        return DetectingHotkeysInString("Battle:\n" +
                            "Behind this door awaits a small arena with a couple of opponents.\n" +
                            "Test your might in real combat.\n" +
                            "If you didn't buy any weapons at the store, go back and do so.\n" +
                            "Going into battle with just a Makarov pistol is a bad idea... ");
                    default: return $"Error getting text from the table\nSingID = {SingID}";
                }
            }
            if (inDebug == 1) return $"Hello, World!\nSingID = {SingID}";
            return $"Error getting text from the table\nSingID = {SingID}";
        }

        //  #====   GameInterface   ====#

        private void DrawCamera()
        {
            Player player = GetPlayer();
            if (player == null) return;
            graphicsWeapon.Clear(Color.Transparent);
            DrawWeapon(player, 0);
            ShowDebugs(player);
        }

        private void DrawGameInterface()
        {
            Player player = GetPlayer();
            if (player == null) return;
            if (ShowSing)
            {
                SaveGraphicsWeaponSmoothing(out var save1);
                graphicsWeapon.Clear(Color.Black);
                graphicsWeapon.DrawImage(Properties.Resources.sing, 0, 0, SCREEN_WIDTH, SCREEN_HEIGHT);
                DrawTextOnSing(GetTextOnSing());
                graphicsWeapon.SmoothingMode = save1;
                return;
            }
            if (ShowMap)
            {
                SaveGraphicsWeaponSmoothing(out var save1);
                graphicsWeapon.Clear(Color.Black);
                graphicsWeapon.DrawImage(DrawMap(), 0, 0, SCREEN_WIDTH, SCREEN_HEIGHT);
                graphicsWeapon.SmoothingMode = save1;
                return;
            }
            int iconSize = 12 + (2 * InterfaceSize);
            if (Resolution == 1) iconSize *= 2;
            int size = Resolution == 0 ? 1 : 2;
            int add = Resolution == 0 ? 2 : 4;
            SizeF moneySize = graphicsWeapon.MeasureString(player.Money.ToString(), ConsolasFont[InterfaceSize, Resolution]);
            graphicsWeapon.Clear(Color.Transparent);
            try
            {
                UpdateMoveStyle(player);
                DrawWeapon(player, player.GunState);
            }
            catch
            {
                try { DrawWeapon(player, 0); }
                catch { }
            }
            DrawScreenEffects(player);
            if (ShowFPS) graphicsWeapon.DrawString($"FPS: {fps}", ConsolasFont[InterfaceSize, Resolution], WhiteBrush, 0, 0);
            DrawHPIcons(player, iconSize, add);
            DrawPing(iconSize);
            DrawHPAndItemCount(player, iconSize, add);
            SaveGraphicsWeaponSmoothing(out var save);
            DrawScope(player);
            DisplayStamine(player, iconSize, size);
            int moneyY = 2;
            if (ShowMiniMap)
            {
                using (var miniMap = DrawMiniMap())
                {
                    moneyY = miniMap.Height + 3;
                    graphicsWeapon.DrawImage(miniMap, SCREEN_WIDTH - miniMap.Width - 1, size);
                }
            }
            graphicsWeapon.SmoothingMode = save;
            graphicsWeapon.DrawString(player.Money.ToString(), ConsolasFont[InterfaceSize, Resolution], WhiteBrush, SCREEN_WIDTH, moneyY, rightToLeft);
            graphicsWeapon.DrawImage(Properties.Resources.money, SCREEN_WIDTH - moneySize.Width - iconSize, moneyY, iconSize, iconSize);
            DrawStageTitle(size);
            for (int i = 0; i < player.Effects.Count; i++) DrawDurationEffect(EffectIcon[player.Effects[i].GetType()], iconSize, i, player.Effects[i] is Debaf);
            if (player.InSelectingMode)
            {
                for (int i = 0; i < player.DisposableItems.Count; i++)
                {
                    Image icon = ItemIconDict[player.DisposableItems[i].GetType()];
                    bool selected = false;
                    if (player.CuteMode) icon = CuteItemIconDict[player.DisposableItems[i].GetType()];
                    if (player.SelectedItem == i) selected = true;
                    DrawItemSelecter(icon, iconSize, i, selected);
                }
            }
            ShowDebugs(player);
            if (ScreenRecording) graphicsWeapon.DrawImage(Properties.Resources.record, (WEAPON.Width - 40) / 2, 0, 40, 15);
            if (Resolution == 1)
            {
                graphicsWeapon.DrawLine(new Pen(Color.Black, 1), 0, WEAPON.Height - 1, WEAPON.Width, WEAPON.Height - 1);
                graphicsWeapon.DrawLine(new Pen(Color.Black, 1), WEAPON.Width - 1, 0, WEAPON.Width - 1, WEAPON.Height - 1);
            }
        }

        /*
        private void DrawSpectatorInterface()
        {
            Player player = GetPlayer();
            if (player == null) return;
            int item_count = 0, item_max_count = 0;
            if (player.DisposableItem != null)
            {
                item_count = player.DisposableItem.Count;
                item_max_count = player.DisposableItem.MaxCount;
            }
            int icon_size = 12 + (2 * interface_size);
            if (resolution == 1) icon_size *= 2;
            int size = resolution == 0 ? 1 : 2;
            int add = resolution == 0 ? 2 : 4;
            double hp = player.InTransport ? player.TransportHP : player.HP;
            SizeF hpSize = graphicsWeapon.MeasureString(hp.ToString("0"), consolasFont[interface_size, resolution]);
            SizeF moneySize = graphicsWeapon.MeasureString(player.Money.ToString(), consolasFont[interface_size, resolution]);
            int ammo_icon_x = (icon_size + 2) + (int)hpSize.Width + 2;
            int ammo_x = ammo_icon_x + icon_size;
            graphicsWeapon.Clear(Color.Transparent);
            try
            {
                UpdateMoveStyle(player);
                DrawWeapon(player, player.GunState);
            }
            catch
            {
                try { DrawWeapon(player, 0); }
                catch { }
            }
            if (player.EffectCheck(2))
                graphicsWeapon.DrawImage(Properties.Resources.helmet_on_head, 0, 0, WEAPON.Width, WEAPON.Height);
            if (ShowFPS)
                graphicsWeapon.DrawString($"FPS: {fps}", consolasFont[interface_size, resolution], whiteBrush, 0, 0);
            if (player.InTransport)
                graphicsWeapon.DrawImage(TransportImages[player.TRANSPORT.GetType()][0], 2, SCREEN_HEIGHT - icon_size - add, icon_size, icon_size);
            else if (!player.CuteMode)
            {
                graphicsWeapon.DrawImage(Properties.Resources.hp, 2, SCREEN_HEIGHT - icon_size - add, icon_size, icon_size);
                if (player.DisposableItem != null && !Controller.IsMultiplayer())
                    graphicsWeapon.DrawImage(ItemIconDict[player.DisposableItem.GetType()], 2, SCREEN_HEIGHT - (icon_size * 2) - add, icon_size, icon_size);
            }
            else
            {
                graphicsWeapon.DrawImage(Properties.Resources.food_hp, 2, SCREEN_HEIGHT - icon_size - add, icon_size, icon_size);
                if (player.DisposableItem != null && !Controller.IsMultiplayer())
                    graphicsWeapon.DrawImage(CuteItemIconDict[player.DisposableItem.GetType()], 2, SCREEN_HEIGHT - (icon_size * 2) - add, icon_size, icon_size);
            }
            if (!player.InTransport && !Controller.IsMultiplayer())
                graphicsWeapon.DrawString($"{item_max_count}/{item_count}", consolasFont[interface_size, resolution], whiteBrush, icon_size + 2, SCREEN_HEIGHT - (icon_size * 2) - add);
            SizeF fpsSize = graphicsWeapon.MeasureString($"FPS: {fps}", consolasFont[interface_size, resolution]);
            DrawPing(fpsSize, icon_size);
            string playerName = player.Name.Length == 0 ? "NoName" : player.Name;
            SizeF textSize = graphicsWeapon.MeasureString(playerName, consolasFont[0, 0]);
            graphicsWeapon.DrawString(playerName, consolasFont[0, 0], whiteBrush, (WEAPON.Width - textSize.Width) / 2, 2);
            graphicsWeapon.DrawString(hp.ToString("0"), consolasFont[interface_size, resolution], whiteBrush, icon_size + 2, SCREEN_HEIGHT - icon_size - add);
            if (!player.IsPetting && !player.InParkour && !player.InTransport && player.Guns.Count > 0 && player.GetCurrentGun().ShowAmmo)
            {
                if (player.GetCurrentGun().ShowAmmoAsNumber)
                    graphicsWeapon.DrawString($"{player.GetCurrentGun().AmmoInStock + player.GetCurrentGun().AmmoCount}", consolasFont[interface_size, resolution], whiteBrush, ammo_x, SCREEN_HEIGHT - icon_size - add);
                else
                    graphicsWeapon.DrawString($"{player.GetCurrentGun().AmmoInStock}/{player.GetCurrentGun().AmmoCount}", consolasFont[interface_size, resolution], whiteBrush, ammo_x, SCREEN_HEIGHT - icon_size - add);
                graphicsWeapon.DrawImage(GetAmmoIcon(player.GetCurrentGun().AmmoType), ammo_icon_x, SCREEN_HEIGHT - icon_size - add, icon_size, icon_size);
            }
            SmoothingMode save = graphicsWeapon.SmoothingMode;
            graphicsWeapon.SmoothingMode = SmoothingMode.None;
            DisplayStamine(player, icon_size, size);
            int money_y = 2;
            if (ShowMiniMap)
            {
                Bitmap mini_map = DrawMiniMap();
                money_y = mini_map.Height + 3;
                graphicsWeapon.DrawImage(mini_map, SCREEN_WIDTH - mini_map.Width - 1, size);
                mini_map.Dispose();
            }
            graphicsWeapon.SmoothingMode = save;
            graphicsWeapon.DrawString(player.Money.ToString(), consolasFont[interface_size, resolution], whiteBrush, SCREEN_WIDTH, money_y, rightToLeft);
            graphicsWeapon.DrawImage(Properties.Resources.money, SCREEN_WIDTH - moneySize.Width - icon_size, money_y, icon_size, icon_size);
            if (player.Effects.Count > 0)
            {
                for (int i = 0; i < player.Effects.Count; i++)
                    DrawDurationEffect(EffectIcon[player.Effects[i].GetType()], icon_size, i, player.Effects[i] is Debaf);
            }
            ShowDebugs(player);
            if (resolution == 1)
            {
                graphicsWeapon.DrawLine(new Pen(Color.Black, 1), 0, WEAPON.Height - 1, WEAPON.Width, WEAPON.Height - 1);
                graphicsWeapon.DrawLine(new Pen(Color.Black, 1), WEAPON.Width - 1, 0, WEAPON.Width - 1, WEAPON.Height - 1);
            }
        }
        */

        private void DrawStageTitle(int size)
        {
            if (stage_timer.Enabled && StageOpacity > 0)
            {
                string text = "STAGE: ";
                if (Controller.InBackrooms()) text += "???";
                else if (IsTutorial) text += "Tutorial";
                else if (inDebug == 1) text += "Debug";
                else if (inDebug == 2) text += "Debug Boss";
                else if (inDebug == 3) text += "Debug Bike";
                else if (difficulty == 4) text += "Custom";
                else text += (Controller.GetStage() + 1).ToString(); SizeF textSize = graphicsWeapon.MeasureString(text, ConsolasFont[InterfaceSize, Resolution + 1]);
                SolidBrush brush = (SolidBrush)WhiteBrush.Clone();
                brush.Color = Color.FromArgb((int)(255 * StageOpacity), brush.Color.R, brush.Color.G, brush.Color.B);
                graphicsWeapon.DrawString(text, ConsolasFont[InterfaceSize, Resolution + 1], brush, (WEAPON.Width - textSize.Width) / 2, 30 * size);
            }
        }

        private void DrawScope(Player player)
        {
            if (player.GetCurrentGun().ShowScope && !player.DoesKick && !player.IsPetting && !player.InParkour && !player.InTransport && !player.InSelectingMode)
            {
                if (Resolution == 0)
                {
                    if (player.GetCurrentGun() is Shotgun)
                        graphicsWeapon.DrawImage(ScopeShotgun[ScopeType], 0, 0, WEAPON.Width, WEAPON.Height);
                    else
                        graphicsWeapon.DrawImage(Scope[ScopeType], 0, 0, WEAPON.Width, WEAPON.Height);
                }
                else
                {
                    if (player.GetCurrentGun() is Shotgun)
                        graphicsWeapon.DrawImage(HScopeShotgun[ScopeType], 0, 0, WEAPON.Width, WEAPON.Height);
                    else
                        graphicsWeapon.DrawImage(HScope[ScopeType], 0, 0, WEAPON.Width, WEAPON.Height);
                }
            }
            if (player.GetCurrentGun().ShowHitScope && ScopeHit != null)
                graphicsWeapon.DrawImage(ScopeHit, 0, 0, WEAPON.Width, WEAPON.Height);
        }

        private void SaveGraphicsWeaponSmoothing(out SmoothingMode save)
        {
            save = graphicsWeapon.SmoothingMode;
            graphicsWeapon.SmoothingMode = SmoothingMode.None;
        }

        private void DrawHPIcons(Player player, int iconSize, int add)
        {
            if (player.InTransport) graphicsWeapon.DrawImage(TransportImages[player.TRANSPORT.GetType()][0], 2, SCREEN_HEIGHT - iconSize - add, iconSize, iconSize);
            else if (!player.CuteMode)
            {
                graphicsWeapon.DrawImage(Properties.Resources.hp, 2, SCREEN_HEIGHT - iconSize - add, iconSize, iconSize);
                if (player.DisposableItem != null && !Controller.IsMultiplayer())
                    graphicsWeapon.DrawImage(ItemIconDict[player.DisposableItem.GetType()], 2, SCREEN_HEIGHT - (iconSize * 2) - add, iconSize, iconSize);
            }
            else
            {
                graphicsWeapon.DrawImage(Properties.Resources.food_hp, 2, SCREEN_HEIGHT - iconSize - add, iconSize, iconSize);
                if (player.DisposableItem != null && !Controller.IsMultiplayer())
                    graphicsWeapon.DrawImage(CuteItemIconDict[player.DisposableItem.GetType()], 2, SCREEN_HEIGHT - (iconSize * 2) - add, iconSize, iconSize);
            }
        }

        private void DrawHPAndItemCount(Player player, int iconSize, int add)
        {
            double hp = player.InTransport ? player.TransportHP : player.HP;
            SizeF hpSize = graphicsWeapon.MeasureString(hp.ToString("0"), ConsolasFont[InterfaceSize, Resolution]);
            int ammoIconX = (iconSize + 2) + (int)hpSize.Width + 2;
            int ammoX = ammoIconX + iconSize;
            int itemCount = 0, itemMaxCount = 0;
            if (player.DisposableItem != null)
            {
                itemCount = player.DisposableItem.Count;
                itemMaxCount = player.DisposableItem.MaxCount;
            }
            if (!player.InTransport && !Controller.IsMultiplayer())
                graphicsWeapon.DrawString($"{itemMaxCount}/{itemCount}", ConsolasFont[InterfaceSize, Resolution], WhiteBrush, iconSize + 2, SCREEN_HEIGHT - (iconSize * 2) - add);
            graphicsWeapon.DrawString(hp.ToString("0"), ConsolasFont[InterfaceSize, Resolution], WhiteBrush, iconSize + 2, SCREEN_HEIGHT - iconSize - add);
            if (!player.IsPetting && !player.InParkour && !player.InTransport && player.Guns.Count > 0 && player.GetCurrentGun().ShowAmmo)
            {
                if (player.GetCurrentGun().ShowAmmoAsNumber)
                    graphicsWeapon.DrawString($"{player.GetCurrentGun().AmmoInStock + player.GetCurrentGun().AmmoCount}", ConsolasFont[InterfaceSize, Resolution], WhiteBrush, ammoX, SCREEN_HEIGHT - iconSize - add);
                else
                    graphicsWeapon.DrawString($"{player.GetCurrentGun().AmmoInStock}/{player.GetCurrentGun().AmmoCount}", ConsolasFont[InterfaceSize, Resolution], WhiteBrush, ammoX, SCREEN_HEIGHT - iconSize - add);
                graphicsWeapon.DrawImage(GetAmmoIcon(player.GetCurrentGun().AmmoType), ammoIconX, SCREEN_HEIGHT - iconSize - add, iconSize, iconSize);
            }            
        }

        private void DrawPing(int iconSize)
        {
            SizeF fpsSize = graphicsWeapon.MeasureString($"FPS: {fps}", ConsolasFont[InterfaceSize, Resolution]);
            if (Controller.IsMultiplayer()) DrawPing(fpsSize, iconSize);
        }

        private void DrawScreenEffects(Player player)
        {
            if (player.EffectCheck(2))
                graphicsWeapon.DrawImage(Properties.Resources.helmet_on_head, 0, 0, WEAPON.Width, WEAPON.Height);
            for (int i = 0; i < ScreenEffects.Count; i++)
            {
                var screenEffect = ScreenEffects[i];
                float timeRatio = ML.Clamp((float)screenEffect.TimeRemaining / screenEffect.TotalTime, 0, 1);
                using (var attributes = new ImageAttributes())
                {
                    var matrix = new ColorMatrix { Matrix33 = timeRatio };
                    attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                    Image effectImage;
                    if (screenEffect is ScreenShot ss)
                    {
                        effectImage = ss.SSImage;
                        var (screenWidth, screenHeight) = (WEAPON.Width, WEAPON.Height);
                        var imageSize = new Size(screenWidth * 2 / 3, screenHeight * 2 / 3);
                        var offset = new Point((screenWidth - imageSize.Width) / 2, (screenHeight - imageSize.Height) / 2);
                        graphicsWeapon.DrawImage(effectImage, new Rectangle(offset, imageSize),
                            0, 0, effectImage.Width, effectImage.Height, GraphicsUnit.Pixel, attributes);
                        using (var framePen = new Pen(Color.FromArgb((int)(255 * timeRatio), Color.White), 2))
                            graphicsWeapon.DrawRectangle(framePen, new Rectangle(offset, imageSize));
                    }
                    else
                    {
                        effectImage = ScreenEffectsIcons[screenEffect.GetType()];
                        graphicsWeapon.DrawImage(effectImage, new Rectangle(0, 0, WEAPON.Width, WEAPON.Height), 0, 0, effectImage.Width, effectImage.Height, GraphicsUnit.Pixel, attributes);
                    }
                }
                if (screenEffect.TimeRemaining < 0)
                {
                    if (screenEffect is ScreenShot screenShot) screenShot.SSImage?.Dispose();
                    ScreenEffects.RemoveAt(i);
                    i--;
                }
            }
            if (player.EffectCheck(6) || Controller.InBackrooms())
            {
                if (player.CuteMode && !Controller.InBackrooms())
                    graphicsWeapon.DrawImage(Properties.Resources.blindness_display_cute_effect, 0, 0, WEAPON.Width, WEAPON.Height);
                else
                    graphicsWeapon.DrawImage(Properties.Resources.blindness_display_effect, 0, 0, WEAPON.Width, WEAPON.Height);
            }
            if (player.EffectCheck(9))
                graphicsWeapon.DrawImage(Properties.Resources.god_display_effect, 0, 0, WEAPON.Width, WEAPON.Height);
        }

        private void DrawPing(SizeF fpsSize, int iconSize)
        {
            int ping = Controller.GetPing();
            int connection_status;
            if (ping < 100) connection_status = 0;
            else if (ping < 150) connection_status = 1;
            else if (ping < 300) connection_status = 2;
            else connection_status = 3;
            graphicsWeapon.DrawImage(ConnectionIcons[connection_status], 2, ShowFPS ? fpsSize.Height : 0, iconSize, iconSize);
            graphicsWeapon.DrawString($"{ping}ms", ConsolasFont[InterfaceSize, Resolution], WhiteBrush, iconSize + 2, ShowFPS ? fpsSize.Height : 0);
        }

        private void ShowDebugs(Player player)
        {
            SaveGraphicsWeaponSmoothing(out var save);
            string debugInfo = null;
            if (ShowDebugSpeed)
                debugInfo += string.Format(
                    "MMS: {0,5:0.##}  MSS: {1,5:0.##}\n" +
                    "MRS: {2,5:0.##}  RS:  {3,5:0.##}\n" +
                    "MS:  {4,5:0.##}  CMS: {5,5:0.##}\n" +
                    "SS:  {6,5:0.##}  CSS: {7,5:0.##}\n" +
                    "MST: {8,5:0.##}  ST:  {9,5:0.##}\n" +
                    "CW:  {10,5:0.##}\n",
                    player.MaxMoveSpeed,
                    player.MaxStrafeSpeed,
                    player.MaxRunSpeed,
                    player.RunSpeed,
                    player.MoveSpeed,
                    player.GetMoveSpeed(ElapsedTime),
                    player.StrafeSpeed,
                    player.GetStrafeSpeed(ElapsedTime),
                    player.MaxStamine,
                    player.Stamine,
                    player.GetWeight()
                );
            if (ShowPositongDebug)
                debugInfo += string.Format(
                    "PX:  {0,5:0.##}  PY:  {1,7:0.##}\n" +
                    "PIX: {2,5:0.##}  PIY: {3,7:0.##}\n" +
                    "PA:  {4,5:0.##}  PL:  {5,7:0.##}\n",
                    player.X,
                    player.Y,
                    (int)player.X,
                    (int)player.Y,
                    player.A,
                    player.Look
                );
            if (ShowGameDebug)
                debugInfo += string.Format(
                    "Stage:  {0,3}\n" +
                    "BStage: {1,3}\n" +
                    "MaxEID: {2,3}\n" +
                    "Difclt: {3,3}",
                    Controller.GetStage(),
                    Controller.GetBackroomsStage(),
                    Controller.GetMaxEntityID(),
                    difficulty
                );
            graphicsWeapon.DrawString(debugInfo, ConsolasFont[0, 0], WhiteBrush, 0, 16);
            graphicsWeapon.SmoothingMode = save;
        }

        private Image GetAmmoIcon(AmmoTypes ammoType)
        {
            switch (ammoType)
            {
                case AmmoTypes.Magic:
                    return Properties.Resources.magic;
                case AmmoTypes.Bubbles:
                    return Properties.Resources.bubbles;
                case AmmoTypes.Bullet:
                    return Properties.Resources.bullet;
                case AmmoTypes.Shell:
                    return Properties.Resources.shell;
                case AmmoTypes.Rifle:
                    return Properties.Resources.rifle_bullet;
                case AmmoTypes.Rocket:
                    return Properties.Resources.rocket;
                case AmmoTypes.C4:
                    return Properties.Resources.c4;
                default:
                    return Properties.Resources.missing;
            }
        }

        private void DrawWeapon(Player player, int index)
        {
            Image imageToDraw;
            if (player.IsPetting)
                imageToDraw = Properties.Resources.pet_animation;
            else if (Controller.InBackrooms())
                imageToDraw = Properties.Resources.camera;
            else if (player.InTransport)
            {
                if (player.InParkour)
                    imageToDraw = TransportImages[player.TRANSPORT.GetType()][4];
                else if (player.StrafeDirection == Directions.LEFT)
                    imageToDraw = TransportImages[player.TRANSPORT.GetType()][2];
                else if (player.StrafeDirection == Directions.RIGHT)
                    imageToDraw = TransportImages[player.TRANSPORT.GetType()][3];
                else
                    imageToDraw = TransportImages[player.TRANSPORT.GetType()][1];
            }
            else if (player.InParkour)
                imageToDraw = Properties.Resources.player_parkour;
            //else if (player.DoesKick)
            //{
            //    if (player.KickState == 0) imageToDraw = Properties.Resources.kick;
            //    else imageToDraw = Properties.Resources.damn_kick;
            //}
            else if (player.UseItem)
                imageToDraw = GunsImagesDict[player.DisposableItem.GetType()][player.DisposableItem.GetLevel(), player.ItemFrame];
            else
                imageToDraw = GunsImagesDict[player.GetCurrentGun().GetType()][player.GetCurrentGun().GetLevel(), index];
            int safeXOffset = Math.Max(0, Math.Min((int)xOffset, imageToDraw.Width - WEAPON.Width));
            int safeYOffset = Math.Max(0, Math.Min((int)yOffset, imageToDraw.Height - WEAPON.Height));
            Rectangle sourceRect = new Rectangle(safeXOffset, safeYOffset, WEAPON.Width, WEAPON.Height);
            if (player.DoesKick || player.InParkour || player.Aiming)
                sourceRect.X = sourceRect.Y = 0;
            Rectangle destRect = new Rectangle(0, 0, WEAPON.Width, WEAPON.Height);
            if (sourceRect.Right > imageToDraw.Width) sourceRect.Width = imageToDraw.Width - sourceRect.X;
            if (sourceRect.Bottom > imageToDraw.Height) sourceRect.Height = imageToDraw.Height - sourceRect.Y;
            graphicsWeapon.DrawImage(imageToDraw, destRect, sourceRect, GraphicsUnit.Pixel);
        }

        private Bitmap DrawMiniMap()
        {
            Player player = GetPlayer();
            int FACTOR = Resolution == 1 ? 2 : 1;
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
                        char mapChar = DISPLAYED_MAP[GetCoordinate(mapX, mapY)];
                        pixelColor = GetColorForMapChar(mapChar);
                    }
                    else pixelColor = Color.Black;
                    if (pixelColor == Color.Black) pixelColor = Color.FromArgb(200, Color.Black);
                    miniMapArray[x, y] = pixelColor;
                }
            }
            using (Graphics g = Graphics.FromImage(miniMap))
            {
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                using (SolidBrush borderBrush = new SolidBrush(Color.Green))
                    g.FillEllipse(borderBrush, 0, 0, totalSize, totalSize);
                g.SmoothingMode = SmoothingMode.None;
                using (GraphicsPath path = new GraphicsPath())
                {
                    path.AddEllipse(BORDER_SIZE * FACTOR, BORDER_SIZE * FACTOR, MINI_MAP_DRAW_SIZE * FACTOR, MINI_MAP_DRAW_SIZE * FACTOR);
                    g.SetClip(path);
                    Pen gridPen = new Pen(Color.FromArgb(50, 255, 255, 255), 1);
                    for (int gridLine = 0; gridLine <= MINI_MAP_SIZE; gridLine++)
                    {
                        int linePosition = (gridLine * PIXEL_SIZE + BORDER_SIZE) * FACTOR;
                        g.DrawLine(gridPen, linePosition, BORDER_SIZE * FACTOR, linePosition, (MINI_MAP_SIZE * PIXEL_SIZE + BORDER_SIZE) * FACTOR);
                        g.DrawLine(gridPen, BORDER_SIZE * FACTOR, linePosition, (MINI_MAP_SIZE * PIXEL_SIZE + BORDER_SIZE) * FACTOR, linePosition);
                    }
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
                case 'S': return Color.DimGray;
                case 'W': return Color.White;
                case '#': return Color.Blue;
                case '=': return Color.YellowGreen;
                case 'P': return Color.Red;
                case 'B':
                case 'X':
                case 'b': return Color.Brown;
                case 'R':
                case 'd':
                case 'o':
                case 'D':
                case 'O': return Color.FromArgb(255, 165, 0);
                case '5': return Color.Yellow;
                case 'L': return Color.OliveDrab;
                case 'l': return Color.DarkGoldenrod;
                case '$': return Color.Pink;
                case 'T':
                case 't': return Color.Turquoise;
                case 'V': return Color.GhostWhite;
                case 'F':
                case 'f': return Color.MediumVioletRed;
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
                    char mapChar = DISPLAYED_MAP[GetCoordinate(x, y)];
                    color = GetColorForMapChar(mapChar);
                    pixels[i] = color.B;
                    pixels[i + 1] = color.G;
                    pixels[i + 2] = color.R;
                    if (bytesPerPixel == 4) pixels[i + 3] = color.A;
                }
            }
            Marshal.Copy(pixels, 0, data.Scan0, pixels.Length);
            map.UnlockBits(data);
            return map;
        }

        private void DrawDurationEffect(Image effectImage, int iconSize, int index, bool isDebuff)
        {
            Player player = GetPlayer();
            if (player == null) return;
            int x = WEAPON.Width - iconSize - 4 - ((iconSize + 4) * index);
            int y = WEAPON.Height - iconSize - 4;
            RectangleF circleRect = new RectangleF(x, y, iconSize, iconSize);
            RectangleF shadowRect = new RectangleF(x + 2, y + 2, iconSize, iconSize);
            using (Brush shadowBrush = new SolidBrush(Color.FromArgb(50, 0, 0, 0)))
                graphicsWeapon.FillEllipse(shadowBrush, shadowRect);
            using (LinearGradientBrush gradientBrush = new LinearGradientBrush(circleRect,
                isDebuff ? Color.FromArgb(130, 110, 140) : Color.FromArgb(110, 150, 200),
                isDebuff ? Color.FromArgb(90, 70, 120) : Color.FromArgb(70, 110, 170), LinearGradientMode.ForwardDiagonal))
                graphicsWeapon.FillEllipse(gradientBrush, circleRect);
            using (Pen outerPen = new Pen(isDebuff ? Color.FromArgb(70, 60, 80) : Color.FromArgb(50, 80, 120), 1.75f))
                graphicsWeapon.DrawEllipse(outerPen, circleRect);
            float sweepAngle = (float)player.Effects[index].TimeRemaining / player.Effects[index].TotalTime * 360;
            using (Pen progressPen = new Pen(isDebuff ? Color.FromArgb(220, 80, 80) : Color.FromArgb(80, 200, 250), 1.75f))
            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddArc(circleRect, -90, sweepAngle);
                graphicsWeapon.DrawPath(progressPen, path);
            }
            graphicsWeapon.DrawImage(effectImage, x + 0.5f, y + 0.5f, iconSize, iconSize);
        }

        private void DrawItemSelecter(Image itemImage, int iconSize, int index, bool isHighlighted)
        {
            Player player = GetPlayer();
            if (player == null) return;
            int x = CenterX - (iconSize / 2);
            int y = CenterY - (iconSize / 2);
            switch (index)
            {
                case 0: x -= iconSize * 2; break;
                case 1: y -= iconSize * 2; break;
                case 2: x += iconSize * 2; break;
                case 3: y += iconSize * 2; break;
            }
            RectangleF circleRect = new RectangleF(x, y, iconSize, iconSize);
            using (LinearGradientBrush gradientBrush = new LinearGradientBrush(circleRect, Color.FromArgb(150, 180, 210), Color.FromArgb(100, 130, 165), LinearGradientMode.ForwardDiagonal))
                graphicsWeapon.FillEllipse(gradientBrush, circleRect);
            RectangleF shadowRect = new RectangleF(x + 3, y + 3, iconSize, iconSize);
            using (Brush shadowBrush = new SolidBrush(Color.FromArgb(50, 0, 0, 0)))
                graphicsWeapon.FillEllipse(shadowBrush, shadowRect);
            using (Pen pen = new Pen(isHighlighted ? Color.FromArgb(128, 230, 255) : Color.FromArgb(100, 180, 230), isHighlighted ? 4.5f : 3f))
                graphicsWeapon.DrawEllipse(pen, circleRect);
            if (isHighlighted) DrawArrow(CursorX, CursorY);
            graphicsWeapon.DrawImage(itemImage, x, y, iconSize, iconSize);
        }

        private void DrawArrow(int targetX, int targetY)
        {
            int arrowLength = 8 + (2 * InterfaceSize);
            if (Resolution == 1) arrowLength *= 2;
            float angle = (float)Math.Atan2(targetY - (CenterY - 1), targetX - (CenterX - 1));
            PointF arrowTip = new PointF(
                (CenterX - 1) + (float)(arrowLength * Math.Cos(angle)),
                (CenterY - 1) + (float)(arrowLength * Math.Sin(angle))
            );
            PointF arrowBase1 = new PointF(
                (CenterX - 1) + (float)(arrowLength * 0.7 * Math.Cos(angle + Math.PI / 6)),
                (CenterY - 1) + (float)(arrowLength * 0.7 * Math.Sin(angle + Math.PI / 6))
            );
            PointF arrowBase2 = new PointF(
                (CenterX - 1) + (float)(arrowLength * 0.7 * Math.Cos(angle - Math.PI / 6)),
                (CenterY - 1) + (float)(arrowLength * 0.7 * Math.Sin(angle - Math.PI / 6))
            );
            using (Pen arrowPen = new Pen(Color.FromArgb(104, 233, 248), 2.5f))
            {
                graphicsWeapon.DrawLine(arrowPen, CenterX - 1, CenterY - 1, arrowTip.X, arrowTip.Y);
                graphicsWeapon.DrawLine(arrowPen, arrowTip.X, arrowTip.Y, arrowBase1.X, arrowBase1.Y);
                graphicsWeapon.DrawLine(arrowPen, arrowTip.X, arrowTip.Y, arrowBase2.X, arrowBase2.Y);
            }
        }

        private void DrawRunIcon(int stamineLeft, int stamineTop, int iconSize)
        {
            Image icon = PlayerCanRun() ?
                Properties.Resources.stamine_icon : Properties.Resources.stamine_cant_run_icon;
            graphicsWeapon.DrawImage(icon, stamineLeft, stamineTop, iconSize, iconSize);
        }

        private void DisplayStamine(Player player, int iconSize, int size)
        {
            int stamine_width = 40 + (10 * InterfaceSize);
            if (Resolution == 1) stamine_width *= 2;
            int progress_width = (int)(player.Stamine / player.MaxStamine * (stamine_width - 2));
            int stamine_top = SCREEN_HEIGHT - (iconSize * 2);
            int stamine_left = (SCREEN_WIDTH - (stamine_width + iconSize + 2)) / 2;
            if (RunKeyPressed || player.Stamine < player.MaxStamine)
                DrawRunIcon(stamine_left, stamine_top, iconSize);
            if (player.Stamine >= player.MaxStamine) return;
            int stamine_progressbar_left = stamine_left + iconSize + 2;
            int stamine_progressbar_top = stamine_top + ((iconSize - 3) / 2);
            graphicsWeapon.FillRectangle(new SolidBrush(Color.Gray), stamine_progressbar_left, stamine_progressbar_top, stamine_width, 2.25f * size);
            Rectangle progressBackgroundRect = new Rectangle(stamine_progressbar_left + 1, stamine_progressbar_top + 1, stamine_width - 2, size);
            using (LinearGradientBrush progressBrush = new LinearGradientBrush(progressBackgroundRect, Color.Red, Color.White, LinearGradientMode.Horizontal))
                graphicsWeapon.FillRectangle(progressBrush, progressBackgroundRect);
            Rectangle progressRect = new Rectangle(stamine_progressbar_left + progress_width + 1, stamine_progressbar_top + 1, stamine_width - progress_width - 2, size);
            graphicsWeapon.FillRectangle(new SolidBrush(Color.Gray), progressRect);
        }

        private void UpdateMoveStyle(Player player)
        {
            if (!shot_timer.Enabled && !reload_timer.Enabled && !shotgun_pull_timer.Enabled)
            {
                if (player.PlayerMoveStyle == Directions.RUN)
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

        //  #====      Shooting     ====#

        private void SetRecoil(Player player)
        {
            RecoilY += player.GetCurrentGun().RecoilY;
            double recoilX = player.GetCurrentGun().GetRecoilX(Rand.NextDouble());
            if (recoilX < 0) RecoilLX += (float)(-recoilX);
            else RecoilRX += (float)recoilX;
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
                Controller.PlayGameSound(GunsSoundsDict[player.GetCurrentGun().GetType()][player.GetCurrentGun().GetLevel(), 0]);
                player.GunState = 1;
                player.Aiming = false;
                player.CanShoot = false;
                BurstShots = 0;
                if (player.GetCurrentGun().FireType == FireTypes.Single)
                {
                    BulletRayCasting();
                    SetRecoil(player);
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
                PressedR = true;
                reload_timer.Start();
                if (player.GetCurrentGun() is Shotgun && player.GetCurrentGun().Level != Levels.LV1)
                    return false;
                Controller.PlayGameSound(GunsSoundsDict[player.GetCurrentGun().GetType()][player.GetCurrentGun().GetLevel(), 1]);
                return false;
            }
            else if (!(player.GetCurrentGun() is Pistol && player.GetCurrentGun().Level == Levels.LV1) &&
                !(player.GetCurrentGun() is Shotgun && player.GetCurrentGun().Level == Levels.LV1))
            {
                Controller.PlayGameSound(GunsSoundsDict[player.GetCurrentGun().GetType()][player.GetCurrentGun().GetLevel(), 2]);
                return false;
            }
            return false;
        }

        private void BulletRayCasting()
        {
            ScopeHit = null;
            Player player = GetPlayer();
            if (player == null || !GameStarted) return;
            List<Entity> Entities = Controller.GetEntities();
            if (player.GetCurrentGun() is RPG) Controller.SpawnRockets(player.X, player.Y, 0, player.A);
            else
            {
                double shotDistance = 0;
                double step = 0.01;
                double rayAngleX = Math.Sin(player.A);
                double rayAngleY = Math.Cos(player.A);
                char[] impassibleCells = { '#', '=', 'd', 'D', 'S', 'R' };
                while (shotDistance <= player.GetCurrentGun().FiringRange)
                {
                    int test_x = (int)(player.X + rayAngleX * shotDistance);
                    int test_y = (int)(player.Y + rayAngleY * shotDistance);
                    if (impassibleCells.Contains(Controller.GetMap()[GetCoordinate(test_x, test_y)]))
                        break;
                    foreach (Entity ent in Entities)
                    {
                        if ((ent as Player) == player) continue;
                        if ((ent.X - player.X) * (ent.X - player.X) + (ent.Y - player.Y) * (ent.Y - player.Y) <= 1)
                            break;
                    }
                    shotDistance += step;
                }
                int[,] bullet = new int[player.GetCurrentGun().BulletCount, 2];
                int maxOffset = (int)(shotDistance * 5 * (1 - player.GetCurrentGun().Accuracy));
                if (player.GetCurrentGun().BulletCount == 1)
                    bullet = new int[,] { { CenterX + Rand.Next(-maxOffset, maxOffset), CenterY + Rand.Next(-maxOffset, maxOffset) } };
                else
                {
                    for (int i = 0; i < player.GetCurrentGun().BulletCount; i++)
                    {
                        bullet[i, 0] = CenterX + Rand.Next(-maxOffset, maxOffset);
                        bullet[i, 1] = CenterY + Rand.Next(-maxOffset, maxOffset);
                    }
                    if (player.GetCurrentGun() is SubmachineGun && player.GetCurrentGun().Level == Levels.LV3)
                    {
                        bullet[0, 0] -= 5;
                        bullet[1, 0] += 5;
                    }
                }
                double[] ZBuffer = new double[SCREEN_WIDTH];
                double[] ZBufferWindow = new double[SCREEN_WIDTH];
                Pixel[][] rays = CastRaysParallel(ZBuffer, ZBufferWindow);
                int entityCount = Entities.Count;
                var spriteInfo = new (int Order, double Distance)[entityCount];
                for (int i = 0; i < entityCount; i++)
                {
                    double dx = player.X - Entities[i].X;
                    double dy = player.Y - Entities[i].Y;
                    spriteInfo[i] = (i, dx * dx + dy * dy);
                }
                Array.Sort(spriteInfo, (b, a) => b.Distance.CompareTo(a.Distance));
                for (int i = 0; i < Entities.Count; i++)
                {
                    try
                    {
                        if (Entities[spriteInfo[i].Order] is HittingTheWall) continue;
                        if (Entities[spriteInfo[i].Order] is Player p && GetPlayer().ID == p.ID) continue;
                        Entity entity = Entities[spriteInfo[i].Order];
                        if (!entity.HasAI) continue;
                        Creature creature = null;
                        if (entity is Creature c)
                        {
                            if (c.Dead || !c.CanHit) continue;
                            creature = c;
                        }
                        double spriteX = entity.X - player.X;
                        double spriteY = entity.Y - player.Y;
                        double transformX = invDet * (dirY * spriteX - dirX * spriteY);
                        double transformY = invDet * (-planeY * spriteX + planeX * spriteY);
                        int spriteScreenX = (int)((SCREEN_WIDTH / 2) * (1 + transformX / transformY));
                        double Distance = ML.GetDistance(new TPoint(entity.X, entity.Y), new TPoint(player.X, player.Y));
                        if (Distance <= 0.1) Distance = 0.1;
                        double spriteTop = (SCREEN_HEIGHT - player.Look) / 2 - (SCREEN_HEIGHT * FOV) / Distance;
                        double spriteBottom = SCREEN_HEIGHT - (spriteTop + player.Look);
                        int spriteCenterY = (int)((spriteTop + spriteBottom) / 2);
                        int drawStartY = (int)spriteTop;
                        int drawEndY = (int)spriteBottom;
                        double vMove = entity.VMove;
                        int vMoveScreen = (int)(vMove / transformY);
                        int spriteWidth = Math.Abs((int)(SCREEN_WIDTH / Distance));
                        int drawStartX = -spriteWidth / 2 + spriteScreenX + vMoveScreen;
                        if (drawStartX < 0) drawStartX = 0;
                        int drawEndX = spriteWidth / 2 + spriteScreenX + vMoveScreen;
                        if (drawEndX >= SCREEN_WIDTH) drawEndX = SCREEN_WIDTH;
                        var timeNow = (long)((DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds * 2);
                        for (int stripe = drawStartX; stripe < drawEndX; stripe++)
                        {
                            int texWidth = 128;
                            double texX = (double)((256 * (stripe - (-spriteWidth / 2 + spriteScreenX)) * texWidth / spriteWidth) / 256) / texWidth;
                            if (transformY > 0 && stripe > 0 && stripe < SCREEN_WIDTH && transformY < ZBuffer[stripe])
                            {
                                for (int y = drawStartY; y < drawEndY && y < SCREEN_HEIGHT; y++)
                                {
                                    if (y < 0 || (transformY > ZBufferWindow[stripe] && y > spriteCenterY))
                                        continue;
                                    double d = (y - vMoveScreen) - (SCREEN_HEIGHT - (int)player.Look) / 2 + (drawEndY - drawStartY) / 2;
                                    double texY = d / (drawEndY - drawStartY);
                                    if (y == drawStartY) texY = 0;
                                    if (rays[stripe].Length > y && y >= 0)
                                    {
                                        rays[stripe][y].SpriteState = GetSpriteRotation(entity, timeNow);
                                        rays[stripe][y].TextureId = entity.Texture;
                                        rays[stripe][y].Blackout = 0;
                                        rays[stripe][y].TextureX = texX;
                                        rays[stripe][y].TextureY = texY;
                                        Color color = GetColorForPixel(rays[stripe][y]);
                                        for (int k = 0; k < bullet.GetLength(0); k++)
                                        {
                                            if (!(color != Color.Transparent && stripe == bullet[k, 0] && y == bullet[k, 1] && player.GetCurrentGun().FiringRange >= Distance))
                                                continue;
                                            if (creature != null)
                                            {
                                                if (creature.Dead || !creature.CanHit) continue;
                                                double damage = (double)Rand.Next((int)(player.GetCurrentGun().MinDamage * 100), (int)(player.GetCurrentGun().MaxDamage * 100)) / 100;
                                                if (Controller.DealDamage(creature, damage))
                                                {
                                                    if (creature.DeathSound != -1)
                                                    {
                                                        if (player.CuteMode)
                                                            Controller.PlayGameSound(CuteDeathSounds[creature.DeathSound, Rand.Next(0, DeathSounds.GetLength(1))], creature.X, creature.Y);
                                                        else
                                                            Controller.PlayGameSound(DeathSounds[creature.DeathSound, Rand.Next(0, DeathSounds.GetLength(1))], creature.X, creature.Y);
                                                    }
                                                }
                                                if (!player.CuteMode)
                                                {
                                                    if (Resolution == 0)
                                                        ScopeHit = Properties.Resources.scope_hit;
                                                    else
                                                        ScopeHit = Properties.Resources.h_scope_hit;
                                                }
                                                else
                                                {
                                                    if (Resolution == 0)
                                                        ScopeHit = Properties.Resources.scope_c_hit;
                                                    else
                                                        ScopeHit = Properties.Resources.h_scope_c_hit;
                                                }
                                                bullet[k, 0] = -1;
                                                bullet[k, 1] = -1;
                                            }
                                            else if (entity is Player targetPlayer && entity.ID != player.ID)
                                            {
                                                if (targetPlayer.Dead) continue;
                                                double damage = (double)Rand.Next((int)(player.GetCurrentGun().MinDamage * 100), (int)(player.GetCurrentGun().MaxDamage * 100)) / 100;
                                                if (Controller.DealDamage(targetPlayer, damage * 5))
                                                {
                                                    if (player.CuteMode)
                                                        Controller.PlayGameSound(CuteDeathSounds[targetPlayer.DeathSound, Rand.Next(0, DeathSounds.GetLength(1))], targetPlayer.X, targetPlayer.Y);
                                                    else
                                                        Controller.PlayGameSound(DeathSounds[targetPlayer.DeathSound, Rand.Next(0, DeathSounds.GetLength(1))], targetPlayer.X, targetPlayer.Y);
                                                }
                                                if (!player.CuteMode)
                                                {
                                                    if (Resolution == 0)
                                                        ScopeHit = Properties.Resources.scope_hit;
                                                    else
                                                        ScopeHit = Properties.Resources.h_scope_hit;
                                                }
                                                else
                                                {
                                                    if (Resolution == 0)
                                                        ScopeHit = Properties.Resources.scope_c_hit;
                                                    else
                                                        ScopeHit = Properties.Resources.h_scope_c_hit;
                                                }
                                                bullet[k, 0] = -1;
                                                bullet[k, 1] = -1;
                                            }
                                            else if (entity is Transport transport)
                                            {
                                                if (transport.TransportHP <= 0) continue;
                                                double damage = (double)Rand.Next((int)(player.GetCurrentGun().MinDamage * 100), (int)(player.GetCurrentGun().MaxDamage * 100)) / 100;
                                                Controller.DealDamage(transport, damage * 1.5);
                                                if (!player.CuteMode)
                                                {
                                                    if (Resolution == 0)
                                                        ScopeHit = Properties.Resources.scope_hit;
                                                    else
                                                        ScopeHit = Properties.Resources.h_scope_hit;
                                                }
                                                else
                                                {
                                                    if (Resolution == 0)
                                                        ScopeHit = Properties.Resources.scope_c_hit;
                                                    else
                                                        ScopeHit = Properties.Resources.h_scope_c_hit;
                                                }
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
                    catch { }
                }
                if (player.CuteMode) return;
                for (int k = 0; k < bullet.GetLength(0); k++)
                {
                    if (bullet[k, 0] == -1) continue;
                    double deltaA = FOV / 2 - bullet[k, 0] * FOV / SCREEN_WIDTH;
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
                        if (test_x < 0 || test_x >= player.GetCurrentGun().FiringRange + player.X || test_y < 0 || test_y >= player.GetCurrentGun().FiringRange + player.Y) hit = true;
                        else
                        {
                            char test_wall = Controller.GetMap()[GetCoordinate(test_x, test_y)];
                            double celling = (SCREEN_HEIGHT - player.Look) / 2.25d - (SCREEN_HEIGHT * FOV) / distance;
                            double floor = SCREEN_HEIGHT - (celling + player.Look);
                            double mid = (celling + floor) / 2;
                            if (test_wall == '#' || test_wall == 'S' || test_wall == 'd' || test_wall == 'D' || (test_wall == '=' && SCREEN_HEIGHT / 2 >= mid))
                            {
                                hit = true;
                                distance -= 0.2;
                                if (bullet[k, 1] > floor || bullet[k, 1] < celling) continue;
                                double vMove = player.Look / 2.25d + player.Look * FOV / (distance + 0.2) + SCREEN_HEIGHT / 2 - bullet[k, 1];
                                Controller.AddHittingTheWall(player.X + ray_x * distance, player.Y + ray_y * distance, vMove);
                            }
                        }
                    }
                }
            }
        }

        //  #====    ChangeWeapon   ====#

        private void ChangeWeapon(int new_gun)
        {
            Player player = GetPlayer();
            if (player == null || player.UseItem || Controller.InBackrooms()) return;
            if ((new_gun != player.CurrentGun || player.LevelUpdated) && !player.InSelectingMode && player.Guns[new_gun].HasIt)
            {
                Controller.PlayGameSound(Draw);
                Controller.ChangeWeapon(new_gun);
                player.GunState = 0;
                player.Aiming = false;
                reload_timer.Interval = player.GetCurrentGun().RechargeTime;
                shot_timer.Interval = player.GetCurrentGun().FiringRate;
                mouse_hold_timer.Interval = player.GetCurrentGun().PauseBetweenShooting;
                if (player.GetCurrentGun() is Shotgun shotgun)
                    shotgun_pull_timer.Interval = shotgun.PullTime;
                if (player.GetCurrentGun() is Gnome)
                {
                    PrevOst = OstIndex;
                    ChangeOst(6);
                }
                else if (PrevOst != OstIndex)
                {
                    if (player.CuteMode)
                    {
                        if (OstIndex != 7)
                            ChangeOst(7);
                    }
                    else if (Controller.InBackrooms())
                    {
                        if (OstIndex != 8)
                            ChangeOst(8);
                    }
                    else ChangeOst(PrevOst);
                }
            }
        }

        private void TakeFlashlight(bool take)
        {
            Player player = GetPlayer();
            if (player.CuteMode || player.UseItem) return;
            if (take)
            {
                player.PreviousGun = player.CurrentGun;
                ChangeWeapon(0);
            }
            else
                ChangeWeapon(player.PreviousGun);
        }

        //  #====     Screenshot    ====#

        private void DoScreenshot()
        {
            string path = GetScreenshotPath();
            if (BUFFER != null)
            {
                using (Bitmap resizedImage = new Bitmap(SavesWidth, SavesHeight))
                {
                    using (Graphics g = Graphics.FromImage(resizedImage))
                    {
                        g.InterpolationMode = InterpolationMode.NearestNeighbor;
                        g.Clear(Color.Black);
                        g.DrawImage(BUFFER, 0, 0, SavesWidth, SavesHeight);
                    }
                    using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
                        resizedImage.Save(fileStream, ImageFormat.Png);
                }
                ScreenEffects.Add(new ScreenShot(BUFFER.Clone(new Rectangle(0, 0, BUFFER.Width, BUFFER.Height), BUFFER.PixelFormat)));
                ConsolePanel.Log($"Screenshot successfully created and saved to path:\n<{path}<", true, true, Color.Lime);
            }
            else ConsolePanel.Log("Error: BUFFER is null. Cannot take screenshot.", true, true, Color.Red);
            Controller.PlayGameSound(Screenshot);
        }

        private string GetScreenshotPath()
        {
            var dateTime = DateTime.Now;
            var path = Path.Combine("saves", $"screenshot_{dateTime:yyyy_MM_dd}__{dateTime:HH_mm_ss}.png");
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            return path;
        }

        //  #====  ScreenRecording  ====#

        private void StartStopScreenRecording()
        {
            ScreenRecording = !ScreenRecording;
            if (ScreenRecording)
            {
                DisposeVideoWriter();
                try
                {
                    VideoWriter = new VideoFileWriter();
                    VideoWriter.Open(GetScreenRecordingPath(), SavesWidth, SavesHeight, 30, VideoCodec.MPEG4, 8400000);
                }
                catch (Exception ex)
                {
                    ScreenRecording = false;
                    ConsolePanel.Log($"An error occurred while opening the video: {ex.Message}", true, true, Color.Red);
                    DisposeVideoWriter();
                }
            }
            else DisposeVideoWriter();
        }

        private void DisposeVideoWriter()
        {
            if (VideoWriter != null)
            {
                try
                {
                    VideoWriter.Close();
                    VideoWriter.Dispose();
                }
                catch (Exception ex) { ConsolePanel.Log($"An error occurred while disposing the video writer: {ex.Message}", true, true, Color.Red); }
                finally { VideoWriter = null; }
            }
        }

        private string GetScreenRecordingPath()
        {
            var dateTime = DateTime.Now;
            var path = Path.Combine("saves", $"screen_recording_{dateTime:yyyy_MM_dd}__{dateTime:HH_mm_ss}.mp4");
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            return path;
        }

        private void DoingScreenRecording()
        {
            if (!ScreenRecording) return;
            using (var frame = new Bitmap(SavesWidth, SavesHeight))
            {
                using (var g = Graphics.FromImage(frame))
                {
                    g.InterpolationMode = InterpolationMode.NearestNeighbor;
                    g.Clear(Color.Black);
                    if (BUFFER != null) g.DrawImage(BUFFER, 0, 0, SavesWidth, SavesHeight);
                    const string recordingTitle = "Recorded in SLIL (Game by. Lonewolf239)";
                    var titleSize = graphicsWeapon.MeasureString(recordingTitle, ConsolasFont[0, 2]);
                    g.DrawString(recordingTitle, ConsolasFont[0, 2], WhiteBrush, (SavesWidth - titleSize.Width) / 2, 60);
                }
                try { VideoWriter.WriteVideoFrame(frame); }
                catch { }
            }
        }

        //  #====       Scope       ====#

        private Image GetScope(Image scope)
        {
            Bitmap bmp = new Bitmap(scope);
            Color color = GetScopeColor(ScopeColor);
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
                int r = Rand.Next(125, 200);
                int g = Rand.Next(125, 200);
                int b = Rand.Next(125, 200);
                return Color.FromArgb(r, g, b);
            }
            else
            {
                string[] col = { "Lime", "Red", "Yellow", "Blue", "Magenta", "Cyan", "Orange", "White" };
                return Color.FromName(col[scope_color]);
            }
        }

        //  #====   Game methods    ====#

        private int GetCoordinate(double x, double y) => Controller.GetCoordinate(x, y);

        private bool PlayerCanRun()
        {
            Player player = GetPlayer();
            return player.GetCurrentGun().CanRun && !player.InParkour &&
                !player.Fast && !player.IsPetting && !player.Aiming &&
                !shot_timer.Enabled && !reload_timer.Enabled &&
                !shotgun_pull_timer.Enabled && !chill_timer.Enabled &&
                !mouse_hold_timer.Enabled && !player.UseItem;
        }

        private bool PlayerCanDodge()
        {
            Player player = GetPlayer();
            return player.GetCurrentGun().CanRun && !player.InParkour &&
                !player.IsPetting && !player.Aiming &&
                !shot_timer.Enabled && !reload_timer.Enabled &&
                !shotgun_pull_timer.Enabled && !chill_timer.Enabled &&
                !mouse_hold_timer.Enabled && !player.UseItem;
        }

        private void StartGame()
        {
            Controller.RestartGame();
            Player player = GetPlayer();
            if (ConsolePanel == null)
            {
                ConsolePanel = new ConsolePanel()
                {
                    Dock = DockStyle.Fill,
                    Visible = false,
                    player = player,
                    Entities = Controller.GetEntities()
                };
                ConsolePanel.Log("SLIL console *v1.6*\nType \"-HELP-\" for a list of commands...", false, false, Color.Lime);
                ConsolePanel.Log("\n\nEnter the command: ", false, false, Color.Lime);
                Controls.Add(ConsolePanel);
                SLILDisplay = new Display() { Size = Size, Dock = DockStyle.Fill, TabStop = false };
                SLILDisplay.MouseDown += new MouseEventHandler(Display_MouseDown);
                SLILDisplay.MouseUp += new MouseEventHandler(Display_MouseUp);
                SLILDisplay.MouseMove += new MouseEventHandler(Display_MouseMove);
                SLILDisplay.MouseWheel += new MouseEventHandler(Display_Scroll);
                Controls.Add(SLILDisplay);
            }
            UpdateBitmap();
            Activate();
            ResetDefault(player);
            ShopToDefault();
            InitMap();
            try
            {
                if (Controller.GetMap()[GetCoordinate(player.X, player.Y + 2)] == '.')
                    player.A = 0;
                else if (Controller.GetMap()[GetCoordinate(player.X, player.Y - 2)] == '.')
                    player.A = 3;
                else if (Controller.GetMap()[GetCoordinate(player.X + 2, player.Y)] == '.')
                    player.A = 1;
                else if (Controller.GetMap()[GetCoordinate(player.X - 2, player.Y)] == '.')
                    player.A = 4;
            }
            catch { player.A = 0; }
            StageOpacity = 1;
            if (MainMenu.sounds) step_sound_timer.Start();
            StartShopOpen = false;
            player.CanUnblockCamera = player.BlockCamera = true;
            GameStarted = true;
            game_over_panel.Visible = false;
            SLILDisplay.BringToFront();
            SLILDisplay.Focus();
            StartShop(player);
        }

        private void GameOver(int win)
        {
            StopAllSounds();
            raycast.Stop();
            shot_timer.Stop();
            reload_timer.Stop();
            step_sound_timer.Stop();
            stamina_timer.Stop();
            mouse_timer.Stop();
            camera_shaking_timer.Stop();
            recoil_timer.Stop();
            fps_timer.Stop();
            ShowMap = false;
            shop_panel.Visible = false;
            ConsolePanel.Visible = false;
            SLILDisplay.Screen = null;
            SLILDisplay.Refresh();
            GameStarted = false;
            if (win == 1)
            {
                Controller.PlayGameSound(Wp);
                StartGame();
            }
            else if (win == 0)
            {
                ShopToDefault();
                game_over_panel.Visible = true;
                game_over_panel.BringToFront();
            }
            else ShopToDefault();
        }

        private void ResetDefault(Player player)
        {
            map = null;
            SLILDisplay.Screen = null;
            PreviousTime = DateTime.Now.TimeOfDay.TotalSeconds;
            Scope[ScopeType] = GetScope(Scope[ScopeType]);
            HScope[ScopeType] = GetScope(HScope[ScopeType]);
            ScopeShotgun[ScopeType] = GetScope(ScopeShotgun[ScopeType]);
            HScopeShotgun[ScopeType] = GetScope(HScopeShotgun[ScopeType]);
            SLILDisplay.Refresh();
            int x = SLILDisplay.PointToScreen(Point.Empty).X + (SLILDisplay.Width / 2);
            int y = SLILDisplay.PointToScreen(Point.Empty).Y + (SLILDisplay.Height / 2);
            Cursor.Position = new Point(x, y);
            player.SetDefault();
            ShopOpen = false;
            map = new Bitmap(Controller.GetMapWidth(), Controller.GetMapHeight());
            if (!Controller.IsMultiplayer() && Controller.InBackrooms() && Controller.GetBackroomsStage() == 1)
                player.GiveEffect(8, true);
            if (MainMenu.sounds)
            {
                if (Controller.InBackrooms())
                {
                    if (Controller.GetBackroomsStage() == 0)
                        ChangeOst(8);
                    else
                        ChangeOst(9);
                }
                else if (!player.CuteMode)
                {
                    PrevOst = Rand.Next(Ost.Length - 5);
                    ChangeOst(PrevOst);
                }
                else ChangeOst(7);
            }
        }

        private void StopAllSounds()
        {
            foreach (var ostTrack in Ost) ostTrack?.Stop();
            foreach (var hitSound in Hit) hitSound?.Stop();
            foreach (var climbSound in Climb) climbSound?.Stop();
            foreach (var doorSound in Door) doorSound?.Stop();
            StopTwoDimensionalSoundsArray(Steps);
            StopTwoDimensionalSoundsArray(DeathSounds);
            StopTwoDimensionalSoundsArray(CuteDeathSounds);
            StopTwoDimensionalSoundsArray(SoundsofShotsEnemies);
            foreach (var key in GunsSoundsDict.Keys)
            {
                var soundDict = GunsSoundsDict[key];
                for (int i = 0; i < soundDict.GetLength(0); i++)
                {
                    for (int j = 0; j < soundDict.GetLength(1); j++)
                        soundDict[i, j]?.Stop();
                }
            }
            foreach (var key in TransportsSoundsDict.Keys)
            {
                var transportSoundDict = TransportsSoundsDict[key];
                foreach (var transportSound in transportSoundDict)
                    transportSound?.Stop();
            }
            foreach (var scary_sound in ScarySounds) scary_sound?.Stop();
            HitTransport?.Stop();
            Hungry?.Stop();
            Step?.Stop();
            TransportStep?.Stop();
            Draw?.Stop();
            Buy?.Stop();
            Wall?.Stop();
            Wp?.Stop();
            Screenshot?.Stop();
            LowStamine?.Stop();
            Starter?.Stop();
            RPGExplosion?.Stop();
            BarrelExplosion?.Stop();
            BreakdownDoor?.Stop();
            LiftingAmmoBox?.Stop();
            LiftingMoneyPile?.Stop();
            VoidStalkerScreamer?.Stop();
            Kick?.Stop();
            DamnKick?.Stop();
            PlayerDeathSound?.Stop();
        }

        private static void StopTwoDimensionalSoundsArray(PlaySound[,] array)
        {
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                    array[i, j]?.Stop();
            }
        }

        private void ShopToDefault()
        {
            shop_tab_control.Controls.Clear();
            Player player = GetPlayer();
            if (player == null || !player.CuteMode) shop_tab_control.Controls.Add(weapon_shop_page);
            if (player == null || !player.CuteMode) shop_tab_control.Controls.Add(storage_shop_page);
            //TEMP
            if (!Controller.IsMultiplayer())
            {
                shop_tab_control.Controls.Add(pet_shop_page);
                shop_tab_control.Controls.Add(consumables_shop_page);
                shop_tab_control.Controls.Add(transport_shop_page);
            }
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

        private void InitMap()
        {
            DISPLAYED_MAP.Clear();
            string DMAP = "";
            for (int i = 0; i < Controller.GetMap().Length; i++) DMAP += '.';
            if (difficulty < 5 || Controller.InBackrooms())
                DISPLAYED_MAP.Append(DMAP);
            else
            {
                if (inDebug == 1)
                    DISPLAYED_MAP.Append(debugMap);
                else if (inDebug == 2)
                    DISPLAYED_MAP.Append(bossMap);
                else if (inDebug == 3)
                    DISPLAYED_MAP.Append(bikeMap);
            }
        }

        private void DeleteInterface(Control control) => control.Dispose();

        //  #====     StartShop     ====#

        private void StartShop(Player player)
        {
            foreach (Control control in Controls)
            {
                if (control is StartShopInterface)
                    DeleteInterface(control);
            }
            if (Controller.GetStage() != 0 || difficulty <= 1 || inDebug != 0)
            {
                StartTimers();
                return;
            }
            Controller.Pause(true);
            player.CanUnblockCamera = false;
            player.BlockCamera = player.BlockInput = true;
            StartShopOpen = true;
            StartShopInterface startShopInterface = new StartShopInterface(player) { Dock = DockStyle.Fill };
            startShopInterface.stop_start_shop_btn.Click += (Sender, E) =>
            {
                Controller.Pause(false);
                player.CanUnblockCamera = true;
                player.BlockCamera = player.BlockInput = false;
                StartShopOpen = false;
                StartTimers();
                DeleteInterface(startShopInterface);
            };
            Controls.Add(startShopInterface);
            startShopInterface.BringToFront();
            startShopInterface.Focus();
        }

        private void StartTimers()
        {
            raycast.Start();
            stamina_timer.Start();
            camera_shaking_timer.Start();
            recoil_timer.Start();
            fps_timer.Start();
            stage_timer.Start();
            mouse_timer.Start();
        }

        //  #====     Inventory     ====#

        private Image GetDeathCause(int deathCause)
        {
            switch (deathCause)
            {
                case 0: return Properties.Resources.enemy_0_DC;
                case 1: return Properties.Resources.enemy_1_DC;
                case 2: return Properties.Resources.enemy_2_DC;
                case 3: return Properties.Resources.enemy_3_DC;
                case 4: return Properties.Resources.rpg_explosion_0;
                default: return Properties.Resources.missing;
            }
        }

        private void OpenInventory()
        {
            ShowInventory = true;
            mouse_timer.Stop();
            Player player = GetPlayer();
            player.BlockInput = player.BlockCamera = true;
            player.PlayerDirection = Directions.STOP;
            player.StrafeDirection = Directions.STOP;
            player.PlayerMoveStyle = Directions.WALK;
            UpdateInventory();
            inventory_panel.BringToFront();
            inventory_panel.Visible = true;
        }

        private void CloseInventory()
        {
            ShowInventory = false;
            mouse_timer.Start();
            int x = SLILDisplay.PointToScreen(Point.Empty).X + (SLILDisplay.Width / 2);
            int y = SLILDisplay.PointToScreen(Point.Empty).Y + (SLILDisplay.Height / 2);
            Cursor.Position = new Point(x, y);
            inventory_panel.Visible = false;
            Player player = GetPlayer();
            player.BlockInput = player.BlockCamera = false;
        }

        private void UpdateInventory()
        {
            Player player = GetPlayer();
            if (player == null) return;
            inventory_content_panel.Location = new Point((Width - inventory_content_panel.Width) / 2, (Height - inventory_content_panel.Height) / 2);
            medkit_count.Text = $"{player.DisposableItems[0].MaxCount}/{player.DisposableItems[0].Count}";
            adrenalin_count.Text = $"{player.DisposableItems[1].MaxCount}/{player.DisposableItems[1].Count}";
            helmet_count.Text = $"{player.DisposableItems[2].MaxCount}/{player.DisposableItems[2].Count}";
            medical_kit_count.Text = $"{player.DisposableItems[3].MaxCount}/{player.DisposableItems[3].Count}";
            if (player.PET != null) pet_icon.Image = ShopImageDict[player.PET.GetType()];
            else pet_icon.Image = Properties.Resources.empty;
            if (MainMenu.DownloadedLocalizationList)
            {
                if (player.PET != null)
                    pet_label.Text = MainMenu.Localizations.GetLString(MainMenu.Language, player.PET.Name[0]);
                else
                    pet_label.Text = MainMenu.Localizations.GetLString(MainMenu.Language, "9-0");
            }
            else
            {
                if (player.PET != null)
                    pet_label.Text = player.PET.Name[1];
                else
                    pet_label.Text = "Absent";
            }
            if (player.CuteMode)
            {
                hide_weapon_picture.Visible = true;
                return;
            }
            else hide_weapon_picture.Visible = false;
            if (MainMenu.DownloadedLocalizationList)
            {
                pistol_label.Text = MainMenu.Localizations.GetLString(MainMenu.Language, player.GUNS[2].Name[0]);
                if (player.WeaponSlot_0 != -1)
                    weapon_0_label.Text = MainMenu.Localizations.GetLString(MainMenu.Language, player.Guns[player.WeaponSlot_0].Name[0]);
                else
                    weapon_0_label.Text = MainMenu.Localizations.GetLString(MainMenu.Language, "9-0");
                if (player.WeaponSlot_1 != -1)
                    weapon_1_label.Text = MainMenu.Localizations.GetLString(MainMenu.Language, player.Guns[player.WeaponSlot_1].Name[0]);
                else
                    weapon_1_label.Text = MainMenu.Localizations.GetLString(MainMenu.Language, "9-0");
            }
            else
            {
                pistol_label.Text = "Pistol";
                if (player.WeaponSlot_0 != -1)
                    weapon_0_label.Text = player.Guns[player.WeaponSlot_0].Name[1];
                else
                    weapon_0_label.Text = "Absent";
                if (player.WeaponSlot_1 != -1)
                    weapon_1_label.Text = player.Guns[player.WeaponSlot_1].Name[1];
                else
                    weapon_1_label.Text = "Absent";
            }
            pistol_icon.Image = IconDict[player.GUNS[2].GetType()][player.GUNS[2].GetLevel()];
            pistol_ammo_count.Text = $"{player.GUNS[2].AmmoInStock}/{player.GUNS[2].AmmoCount}";
            if (player.WeaponSlot_0 != -1)
            {
                weapon_0_icon.Image = IconDict[player.Guns[player.WeaponSlot_0].GetType()][player.Guns[player.WeaponSlot_0].GetLevel()];
                weapon_0_ammo_icon.Image = GetAmmoIcon(player.Guns[player.WeaponSlot_0].AmmoType);
                if (player.Guns[player.WeaponSlot_0].InfinityAmmo)
                    weapon_0_ammo_count.Text = "∞";
                else
                    weapon_0_ammo_count.Text = $"{player.Guns[player.WeaponSlot_0].AmmoInStock}/{player.Guns[player.WeaponSlot_0].AmmoCount}";
                weapon_0_ammo_icon.Visible = weapon_0_ammo_count.Visible = true;
            }
            else
            {
                weapon_0_icon.Image = Properties.Resources.empty;
                weapon_0_ammo_icon.Visible = weapon_0_ammo_count.Visible = false;
            }
            if (player.WeaponSlot_1 != -1)
            {
                weapon_1_icon.Image = IconDict[player.Guns[player.WeaponSlot_1].GetType()][player.Guns[player.WeaponSlot_1].GetLevel()];
                weapon_1_ammo_icon.Image = GetAmmoIcon(player.Guns[player.WeaponSlot_1].AmmoType);
                if (player.Guns[player.WeaponSlot_1].InfinityAmmo)
                    weapon_1_ammo_count.Text = "∞";
                else
                    weapon_1_ammo_count.Text = $"{player.Guns[player.WeaponSlot_1].AmmoInStock}/{player.Guns[player.WeaponSlot_1].AmmoCount}";
                weapon_1_ammo_icon.Visible = weapon_1_ammo_count.Visible = true;
            }
            else
            {
                weapon_1_icon.Image = Properties.Resources.empty;
                weapon_1_ammo_icon.Visible = weapon_1_ammo_count.Visible = false;
            }
        }

        private void Weapon_icon_MouseEnter(object sender, EventArgs e)
        {
            if (inventory_content_panel.Controls.Contains(InventoryWeaponToolTip))
                inventory_content_panel.Controls.Remove(InventoryWeaponToolTip);
            InventoryWeaponToolTip?.Dispose();
            Player player = GetPlayer();
            if (player == null) return;
            Gun weapon;
            if (((Control)sender).Name == "pistol_icon")
                weapon = player.GUNS[2];
            else if (((Control)sender).Name == "weapon_0_icon" && player.WeaponSlot_0 != -1)
                weapon = player.Guns[player.WeaponSlot_0];
            else if (((Control)sender).Name == "weapon_1_icon" && player.WeaponSlot_1 != -1)
                weapon = player.Guns[player.WeaponSlot_1];
            else return;
            InventoryWeaponToolTip = new WeaponToolTip
            {
                Weapon = weapon,
                Left = 0
            };
            InventoryWeaponToolTip.Top = inventory_content_panel.Height - InventoryWeaponToolTip.Height;
            inventory_content_panel.Controls.Add(InventoryWeaponToolTip);
            InventoryWeaponToolTip.BringToFront();
        }

        private void Weapon_icon_MouseLeave(object sender, EventArgs e)
        {
            if (inventory_content_panel.Controls.Contains(InventoryWeaponToolTip))
                inventory_content_panel.Controls.Remove(InventoryWeaponToolTip);
            InventoryWeaponToolTip?.Dispose();
        }

        private void Info_icon_MouseEnter(object sender, EventArgs e)
        {
            Player player = GetPlayer();
            if (player == null) return;
            if (inventory_content_panel.Controls.Contains(InventoryInfoToolTip))
                inventory_content_panel.Controls.Remove(InventoryInfoToolTip);
            InventoryInfoToolTip?.Dispose();
            string description;
            if (((Control)sender).Name == "medkit_icon")
            {
                if (MainMenu.DownloadedLocalizationList)
                    description = MainMenu.Localizations.GetLString(MainMenu.Language, ((FirstAidKit)player.GUNS[10]).Description[player.CuteMode ? 2 : 0]);
                else
                    description = ((FirstAidKit)player.GUNS[10]).Description[player.CuteMode ? 3 : 1];
            }
            else if (((Control)sender).Name == "helmet_icon")
            {
                if (MainMenu.DownloadedLocalizationList)
                    description = MainMenu.Localizations.GetLString(MainMenu.Language, ((Helmet)player.GUNS[14]).Description[0]);
                else
                    description = ((Helmet)player.GUNS[14]).Description[1];
            }
            else if (((Control)sender).Name == "adrenalin_icon")
            {
                if (MainMenu.DownloadedLocalizationList)
                    description = MainMenu.Localizations.GetLString(MainMenu.Language, ((Adrenalin)player.GUNS[13]).Description[0]);
                else
                    description = ((Adrenalin)player.GUNS[13]).Description[1];
            }
            else if (((Control)sender).Name == "medical_kit_icon")
            {
                if (MainMenu.DownloadedLocalizationList)
                    description = MainMenu.Localizations.GetLString(MainMenu.Language, ((MedicalKit)player.GUNS[17]).Description[0]);
                else
                    description = ((MedicalKit)player.GUNS[17]).Description[1];
            }
            else if (((Control)sender).Name == "pet_icon" && player.PET != null)
            {
                if (MainMenu.DownloadedLocalizationList)
                    description = MainMenu.Localizations.GetLString(MainMenu.Language, player.PET.Description[0]);
                else
                    description = player.PET.Description[1];
            }
            else return;
            InventoryInfoToolTip = new InfoToolTip(description) { Left = 0 };
            InventoryInfoToolTip.Top = inventory_content_panel.Height - InventoryInfoToolTip.Height;
            inventory_content_panel.Controls.Add(InventoryInfoToolTip);
            InventoryInfoToolTip.BringToFront();
        }

        private void Info_icon_MouseLeave(object sender, EventArgs e)
        {
            if (inventory_content_panel.Controls.Contains(InventoryInfoToolTip))
                inventory_content_panel.Controls.Remove(InventoryInfoToolTip);
            InventoryInfoToolTip?.Dispose();
        }

        //  #====       Shop        ====#

        private void Shop_panel_VisibleChanged(object sender, EventArgs e)
        {
            Player player = GetPlayer();
            if (player != null) player.Look = 0;
            shop_panel.BringToFront();
        }

        internal void UpdateStorage()
        {
            Player player = GetPlayer();
            if (player == null) return;
            storage_shop_page.Controls.Clear();
            if (!player.CuteMode)
            {
                for (int i = player.Guns.Count - 1; i >= 0; i--)
                {
                    if (Controller.IsMultiplayer() && !player.Guns[i].InMultiplayer)
                        continue;
                    if (!player.Guns[i].AddToStorage)
                        continue;
                    SLIL_StorageInterface ShopInterface = new SLIL_StorageInterface()
                    {
                        ParentSLILForm = this,
                        index = MainMenu.DownloadedLocalizationList ? 0 : 1,
                        weapon = player.Guns[i],
                        player = player,
                        BackColor = shop_panel.BackColor,
                        Dock = DockStyle.Top
                    };
                    storage_shop_page.Controls.Add(ShopInterface);
                }
            }
        }

        internal void ShowShop()
        {
            Player player = GetPlayer();
            if (player == null) return;
            ShopOpen = true;
            ShopInterface_panel.VerticalScroll.Value = 0;
            mouse_timer.Stop();
            weapon_shop_page.Controls.Clear();
            pet_shop_page.Controls.Clear();
            consumables_shop_page.Controls.Clear();
            transport_shop_page.Controls.Clear();
            player.BlockInput = player.BlockCamera = true;
            player.PlayerDirection = Directions.STOP;
            player.StrafeDirection = Directions.STOP;
            player.PlayerMoveStyle = Directions.WALK;
            if (!player.CuteMode)
            {
                for (int i = player.GUNS.Length - 1; i >= 0; i--)
                {
                    if (Controller.IsMultiplayer() && !player.GUNS[i].InMultiplayer)
                        continue;
                    if (!player.GUNS[i].AddToShop) continue;
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
                UpdateStorage();
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
            for (int i = Controller.GetTransports().Length - 1; i >= 0; i--)
            {
                if (!Controller.GetTransports()[i].AddToShop) continue;
                SLIL_TransportStoreInterface ShopInterface = new SLIL_TransportStoreInterface()
                {
                    index = MainMenu.DownloadedLocalizationList ? 0 : 1,
                    transport = Controller.GetTransports()[i],
                    player = player,
                    BackColor = shop_panel.BackColor,
                    Dock = DockStyle.Top
                };
                transport_shop_page.Controls.Add(ShopInterface);
            }
            shop_panel.BringToFront();
            shop_panel.Visible = true;
        }

        private void HideShop()
        {
            ShopOpen = false;
            mouse_timer.Start();
            int x = SLILDisplay.PointToScreen(Point.Empty).X + (SLILDisplay.Width / 2);
            int y = SLILDisplay.PointToScreen(Point.Empty).Y + (SLILDisplay.Height / 2);
            Cursor.Position = new Point(x, y);
            shop_panel.Visible = false;
            Player player = GetPlayer();
            player.BlockInput = player.BlockCamera = false;
        }

        private void CuteMode()
        {
            Player player = GetPlayer();
            if (player.CuteMode && OstIndex != 7)
            {
                PrevOst = OstIndex;
                ChangeOst(7);
            }
            else if (OstIndex == 7)
            {
                PrevOst = Rand.Next(Ost.Length - 5);
                ChangeOst(PrevOst);
            }
        }

        internal void AddTransport(int index)
        {
            Controller.AddTransport(index);
            HideShop();
        }

        internal void AddPet(int index)
        {
            foreach (SLIL_PetShopInterface control in pet_shop_page.Controls.Find("SLIL_PetShopInterface", true).Cast<SLIL_PetShopInterface>())
                control.buy_button.Text = MainMenu.DownloadedLocalizationList
                    ? $"{MainMenu.Localizations.GetLString(MainMenu.Language, "2-0")} ${control.pet.Cost}"
                    : $"Buy ${control.pet.Cost}";
            Controller.AddPet(index);
            CuteMode();
            Player player = GetPlayer();
            HideShop();
            if (player.CuteMode && shop_tab_control.Controls.ContainsKey("weapon_shop_page"))
            {
                shop_tab_control.Controls.Clear();
                shop_tab_control.Controls.Add(pet_shop_page);
                shop_tab_control.Controls.Add(consumables_shop_page);
                shop_tab_control.Controls.Add(transport_shop_page);
            }
            else if (!shop_tab_control.Controls.ContainsKey("weapon_shop_page"))
            {
                shop_tab_control.Controls.Clear();
                shop_tab_control.Controls.Add(weapon_shop_page);
                shop_tab_control.Controls.Add(storage_shop_page);
                //TEMP
                if (!Controller.IsMultiplayer())
                {
                    shop_tab_control.Controls.Add(pet_shop_page);
                    shop_tab_control.Controls.Add(consumables_shop_page);
                    shop_tab_control.Controls.Add(transport_shop_page);
                }
            }
        }

        internal int GetPetCost(int index)
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

        internal void SetWeaponSlot(int slot, int index) => Controller.SetWeaponSlot(slot, index);

        internal void BuyAmmo(Gun weapon) => Controller.BuyAmmo(weapon);

        internal void BuyWeapon(Gun weapon) => Controller.BuyWeapon(weapon);

        internal void UpdateWeapon(Gun weapon) => Controller.UpdateWeapon(weapon);

        internal void BuyConsumable(DisposableItem item) => Controller.BuyConsumable(item);

        internal Player GetPlayer() => Controller.GetPlayer();
    }
}