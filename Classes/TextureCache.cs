using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SLIL.Classes
{
    internal enum SpriteStates
    {
        Static,
        StepForward_0,
        StepForward_1,
        StopForward,
        StepBack_0,
        StepBack_1,
        StopBack,
        StepLeft_0,
        StepLeft_1,
        StopLeft,
        StepRight_0,
        StepRight_1,
        StopRight,
        DeadBody,
        DeadBodyBlinded,
        FlashlightBlinded,
        Aiming,
        Shooted,
        StepEscape_0,
        StepEscape_1,
    }

    internal readonly struct TextureKey
    {
        internal int Id { get; }
        internal SpriteStates State { get; }

        internal TextureKey(int id, SpriteStates state)
        {
            Id = id;
            State = state;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is TextureKey)) return false;
            TextureKey other = (TextureKey)obj;
            return Id == other.Id && State == other.State;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + Id.GetHashCode();
                hash = hash * 23 + State.GetHashCode();
                return hash;
            }
        }
    }

    internal class TextureCache
    {
        private Dictionary<TextureKey, Image> textures = new Dictionary<TextureKey, Image>()
        {
            //Wall
            { new TextureKey(4, SpriteStates.Static), Properties.Resources.wall },
            //Door
            { new TextureKey(5, SpriteStates.Static), Properties.Resources.door },
            //ShopDoor
            { new TextureKey(6, SpriteStates.Static), Properties.Resources.shop_door },
            //Floor
            { new TextureKey(7, SpriteStates.Static), Properties.Resources.floor },
            //Celling
            { new TextureKey(8, SpriteStates.Static), Properties.Resources.ceiling },
            //Zombie
            { new TextureKey(9, SpriteStates.StepForward_0), Properties.Resources.enemy_0 },
            { new TextureKey(9, SpriteStates.StepForward_1), Properties.Resources.enemy_0_1 },
            { new TextureKey(9, SpriteStates.DeadBody), Properties.Resources.enemy_0_Dead },
            //Dog
            { new TextureKey(10, SpriteStates.StepForward_0), Properties.Resources.enemy_1 },
            { new TextureKey(10, SpriteStates.StepForward_1), Properties.Resources.enemy_1_1 },
            { new TextureKey(10, SpriteStates.DeadBody), Properties.Resources.enemy_1_Dead },
            //Ogr
            { new TextureKey(11, SpriteStates.StepForward_0), Properties.Resources.enemy_2 },
            { new TextureKey(11, SpriteStates.StepForward_1), Properties.Resources.enemy_2_1 },
            { new TextureKey(11, SpriteStates.DeadBody), Properties.Resources.enemy_2_Dead },
            //Bat
            { new TextureKey(12, SpriteStates.StepForward_0), Properties.Resources.enemy_3 },
            { new TextureKey(12, SpriteStates.StepForward_1), Properties.Resources.enemy_3_1 },
            { new TextureKey(12, SpriteStates.DeadBody), Properties.Resources.enemy_3_Dead },
            //Teleport
            { new TextureKey(13, SpriteStates.StepForward_0), Properties.Resources.teleport_0 },
            { new TextureKey(13, SpriteStates.StepForward_1), Properties.Resources.teleport_1 },
            //ShopMan
            { new TextureKey(14, SpriteStates.Static), Properties.Resources.shop_man_0 },
            { new TextureKey(14, SpriteStates.FlashlightBlinded), Properties.Resources.shop_man_1 },
            //HittingTheWall
            { new TextureKey(15, SpriteStates.StepForward_0), Properties.Resources.hit_0 },
            { new TextureKey(15, SpriteStates.StepForward_1), Properties.Resources.hit_1 },
            //Box
            { new TextureKey(16, SpriteStates.Static), Properties.Resources.box },
            { new TextureKey(16, SpriteStates.DeadBody), Properties.Resources.box_broken },
            //Barrel
            { new TextureKey(17, SpriteStates.Static), Properties.Resources.barrel },
            { new TextureKey(17, SpriteStates.DeadBody), Properties.Resources.barrel_broken },
            //Vine
            { new TextureKey(18, SpriteStates.Static), Properties.Resources.vine },
            //Lamp
            { new TextureKey(19, SpriteStates.Static), Properties.Resources.lamp },
            //WallWithSing
            { new TextureKey(20, SpriteStates.Static), Properties.Resources.wall_with_sing },
            //Bike
            { new TextureKey(21, SpriteStates.Static), Properties.Resources.bike_forward },
            { new TextureKey(21, SpriteStates.StopForward), Properties.Resources.bike_forward },
            { new TextureKey(21, SpriteStates.StopBack), Properties.Resources.bike_back },
            { new TextureKey(21, SpriteStates.StopLeft), Properties.Resources.bike_left },
            { new TextureKey(21, SpriteStates.StopRight), Properties.Resources.bike_right },
            //SillyCat
            { new TextureKey(22, SpriteStates.StepForward_0), Properties.Resources.pet_cat_0 },
            { new TextureKey(22, SpriteStates.StepForward_1), Properties.Resources.pet_cat_1 },
            { new TextureKey(22, SpriteStates.StopForward), Properties.Resources.pet_cat_2 },
            { new TextureKey(22, SpriteStates.FlashlightBlinded), Properties.Resources.pet_cat_3 },
            //GreenGnome
            { new TextureKey(23, SpriteStates.StepForward_0), Properties.Resources.pet_gnome_0 },
            { new TextureKey(23, SpriteStates.StepForward_1), Properties.Resources.pet_gnome_1 },
            { new TextureKey(23, SpriteStates.FlashlightBlinded), Properties.Resources.pet_gnome_2 },
            //EnergyDrink
            { new TextureKey(24, SpriteStates.Static), Properties.Resources.pet_energy_drink_0 },
            //Pyro
            { new TextureKey(25, SpriteStates.StepForward_0), Properties.Resources.pet_pyro_0 },
            { new TextureKey(25, SpriteStates.StepForward_1), Properties.Resources.pet_pyro_1 },
            { new TextureKey(25, SpriteStates.FlashlightBlinded), Properties.Resources.pet_pyro_3 },
            //RpgRocket
            { new TextureKey(26, SpriteStates.StepForward_0), Properties.Resources.rpg_rocket_0 },
            { new TextureKey(26, SpriteStates.StepForward_1), Properties.Resources.rpg_rocket_1 },
            //RpgExplosion
            { new TextureKey(27, SpriteStates.StepForward_0), Properties.Resources.rpg_explosion_0 },
            { new TextureKey(27, SpriteStates.StepForward_1), Properties.Resources.rpg_explosion_1 },
            //PlayerDeadBody
            { new TextureKey(28, SpriteStates.Static), Properties.Resources.player_Dead },
            { new TextureKey(28, SpriteStates.DeadBody), Properties.Resources.player_Dead },
            //BackWall
            { new TextureKey(29, SpriteStates.Static), Properties.Resources.back_wall },
            //BackCelling
            { new TextureKey(30, SpriteStates.Static), Properties.Resources.back_ceiling },
            //BackFloor
            { new TextureKey(31, SpriteStates.Static), Properties.Resources.back_floor },
            //BackroomsTeleport
            { new TextureKey(32, SpriteStates.StepForward_0), Properties.Resources.fake_teleport_0 },
            { new TextureKey(32, SpriteStates.StepForward_1), Properties.Resources.fake_teleport_1 },
            //Covering
            { new TextureKey(33, SpriteStates.Static), Properties.Resources. fake_teleport_0},
            { new TextureKey(33, SpriteStates.StepForward_0), Properties.Resources.fake_teleport_0 },
            { new TextureKey(33, SpriteStates.StepForward_1), Properties.Resources.fake_teleport_0 },
            { new TextureKey(33, SpriteStates.DeadBody), Properties.Resources.fake_teleport_0 },
            //VoidTeleport
            { new TextureKey(34, SpriteStates.StepForward_0), Properties.Resources.empty_teleport_0 },
            { new TextureKey(34, SpriteStates.StepForward_1), Properties.Resources.empty_teleport_1 },
            //VoidStalker
            { new TextureKey(35, SpriteStates.Static), Properties.Resources.empty_teleport_0 },
            //Stalker
            { new TextureKey(36, SpriteStates.StepForward_0), Properties.Resources.fake_teleport_0 },
            { new TextureKey(36, SpriteStates.StepForward_1), Properties.Resources.fake_teleport_1 },
            //Shooter
            { new TextureKey(37, SpriteStates.StepForward_0), Properties.Resources.player },
            { new TextureKey(37, SpriteStates.StepForward_1), Properties.Resources.player_1 },
            { new TextureKey(37, SpriteStates.DeadBody), Properties.Resources.player_Dead },
            { new TextureKey(37, SpriteStates.Aiming), Properties.Resources. player_aiming},
            { new TextureKey(37, SpriteStates.Shooted), Properties.Resources.player_shooted },
            { new TextureKey(37, SpriteStates.StepEscape_0), Properties.Resources.player_stoped },
            { new TextureKey(37, SpriteStates.StepEscape_1), Properties.Resources.player_stoped },
            //VoidWall
            { new TextureKey(38, SpriteStates.Static), Properties.Resources.void_wall },
            //VoidFloor
            { new TextureKey(39, SpriteStates.Static), Properties.Resources.void_floor },
            //VoidCelling
            { new TextureKey(40, SpriteStates.Static), Properties.Resources. void_ceiling},
            //LostSoul
            { new TextureKey(41, SpriteStates.StepForward_0), Properties.Resources. lost_soul_0},
            { new TextureKey(41, SpriteStates.StepForward_1), Properties.Resources. lost_soul_1},
            { new TextureKey(41, SpriteStates.DeadBody), Properties.Resources.missing },
            { new TextureKey(41, SpriteStates.Aiming), Properties.Resources. lost_soul_shoot},
            { new TextureKey(41, SpriteStates.Shooted), Properties.Resources. lost_soul_shoot},
            { new TextureKey(41, SpriteStates.StepEscape_0), Properties.Resources.lost_soul_escaping_0 },
            { new TextureKey(41, SpriteStates.StepEscape_1), Properties.Resources.lost_soul_escaping_1 },
            //SoulClot
            { new TextureKey(42, SpriteStates.StepForward_0), Properties.Resources. rpg_rocket_0},
            { new TextureKey(42, SpriteStates.StepForward_1), Properties.Resources. rpg_rocket_1},
        };
        private Dictionary<TextureKey, Image> cute_textures = new Dictionary<TextureKey, Image>()
        {
            //Wall
            { new TextureKey(4, SpriteStates.Static), Properties.Resources.c_wall },
            //Door
            { new TextureKey(5, SpriteStates.Static), Properties.Resources.c_door },
            //ShopDoor
            { new TextureKey(6, SpriteStates.Static), Properties.Resources.c_shop_door },
            //Floor
            { new TextureKey(7, SpriteStates.Static), Properties.Resources.c_floor },
            //Celling
            { new TextureKey(8, SpriteStates.Static), Properties.Resources.c_ceiling },
            //Zombie
            { new TextureKey(9, SpriteStates.StepForward_0), Properties.Resources.c_enemy_0 },
            { new TextureKey(9, SpriteStates.StepForward_1), Properties.Resources.c_enemy_0_1 },
            { new TextureKey(9, SpriteStates.DeadBody), Properties.Resources.c_enemy_0_Dead },
            //Dog
            { new TextureKey(10, SpriteStates.StepForward_0), Properties.Resources.c_enemy_1 },
            { new TextureKey(10, SpriteStates.StepForward_1), Properties.Resources.c_enemy_1_1 },
            { new TextureKey(10, SpriteStates.DeadBody), Properties.Resources.c_enemy_1_Dead },
            //Ogr
            { new TextureKey(11, SpriteStates.StepForward_0), Properties.Resources.c_enemy_2 },
            { new TextureKey(11, SpriteStates.StepForward_1), Properties.Resources.c_enemy_2_1 },
            { new TextureKey(11, SpriteStates.DeadBody), Properties.Resources.c_enemy_2_Dead },
            //Bat
            { new TextureKey(12, SpriteStates.StepForward_0), Properties.Resources.c_enemy_3 },
            { new TextureKey(12, SpriteStates.StepForward_1), Properties.Resources.c_enemy_3_1 },
            { new TextureKey(12, SpriteStates.DeadBody), Properties.Resources.c_enemy_3_Dead },
            //Teleport
            { new TextureKey(13, SpriteStates.StepForward_0), Properties.Resources.c_teleport_0 },
            { new TextureKey(13, SpriteStates.StepForward_1), Properties.Resources.c_teleport_1 },
            //ShopMan
            { new TextureKey(14, SpriteStates.Static), Properties.Resources.shop_man_0 },
            { new TextureKey(14, SpriteStates.FlashlightBlinded), Properties.Resources.shop_man_1 },
            //HittingTheWall
            { new TextureKey(15, SpriteStates.StepForward_0), Properties.Resources.hit_0 },
            { new TextureKey(15, SpriteStates.StepForward_1), Properties.Resources.hit_1 },
            //Box
            { new TextureKey(16, SpriteStates.Static), Properties.Resources.box },
            { new TextureKey(16, SpriteStates.DeadBody), Properties.Resources.box_broken },
            //Barrel
            { new TextureKey(17, SpriteStates.Static), Properties.Resources.barrel },
            { new TextureKey(17, SpriteStates.DeadBody), Properties.Resources.barrel_broken },
            //Vine
            { new TextureKey(18, SpriteStates.Static), Properties.Resources.c_vine },
            //Lamp
            { new TextureKey(19, SpriteStates.Static), Properties.Resources.lamp },
            //WallWithSing
            { new TextureKey(20, SpriteStates.Static), Properties.Resources.wall_with_sing },
            //Bike
            { new TextureKey(21, SpriteStates.Static), Properties.Resources.bike_forward },
            { new TextureKey(21, SpriteStates.StopForward), Properties.Resources.bike_forward },
            { new TextureKey(21, SpriteStates.StopBack), Properties.Resources.bike_back },
            { new TextureKey(21, SpriteStates.StopLeft), Properties.Resources.bike_left },
            { new TextureKey(21, SpriteStates.StopRight), Properties.Resources.bike_right },
            //SillyCat
            { new TextureKey(22, SpriteStates.StepForward_0), Properties.Resources.pet_cat_0 },
            { new TextureKey(22, SpriteStates.StepForward_1), Properties.Resources.pet_cat_1 },
            { new TextureKey(22, SpriteStates.StopForward), Properties.Resources.pet_cat_2 },
            { new TextureKey(22, SpriteStates.FlashlightBlinded), Properties.Resources.pet_cat_3 },
            //GreenGnome
            { new TextureKey(23, SpriteStates.StepForward_0), Properties.Resources.pet_gnome_0 },
            { new TextureKey(23, SpriteStates.StepForward_1), Properties.Resources.pet_gnome_1 },
            { new TextureKey(23, SpriteStates.FlashlightBlinded), Properties.Resources.pet_gnome_2 },
            //EnergyDrink
            { new TextureKey(24, SpriteStates.Static), Properties.Resources.pet_energy_drink_0 },
            //Pyro
            { new TextureKey(25, SpriteStates.StepForward_0), Properties.Resources.pet_pyro_0 },
            { new TextureKey(25, SpriteStates.StepForward_1), Properties.Resources.pet_pyro_1 },
            { new TextureKey(25, SpriteStates.FlashlightBlinded), Properties.Resources.pet_pyro_3 },
            //RpgRocket
            { new TextureKey(26, SpriteStates.StepForward_0), Properties.Resources.rpg_rocket_0 },
            { new TextureKey(26, SpriteStates.StepForward_1), Properties.Resources.rpg_rocket_1 },
            //RpgExplosion
            { new TextureKey(27, SpriteStates.StepForward_0), Properties.Resources.rpg_explosion_0 },
            { new TextureKey(27, SpriteStates.StepForward_1), Properties.Resources.rpg_explosion_1 },
            //PlayerDeadBody
            { new TextureKey(28, SpriteStates.Static), Properties.Resources.player_Dead },
            { new TextureKey(28, SpriteStates.DeadBody), Properties.Resources.player_Dead },
            //BackWall
            { new TextureKey(29, SpriteStates.Static), Properties.Resources.back_wall },
            //BackCelling
            { new TextureKey(30, SpriteStates.Static), Properties.Resources.back_ceiling },
            //BackFloor
            { new TextureKey(31, SpriteStates.Static), Properties.Resources.back_floor },
            //BackroomsTeleport
            { new TextureKey(32, SpriteStates.StepForward_0), Properties.Resources.fake_teleport_0 },
            { new TextureKey(32, SpriteStates.StepForward_1), Properties.Resources.fake_teleport_1 },
            //Covering
            { new TextureKey(33, SpriteStates.Static), Properties.Resources. fake_teleport_0},
            { new TextureKey(33, SpriteStates.StepForward_0), Properties.Resources.fake_teleport_0 },
            { new TextureKey(33, SpriteStates.StepForward_1), Properties.Resources.fake_teleport_0 },
            { new TextureKey(33, SpriteStates.DeadBody), Properties.Resources.fake_teleport_0 },
            //VoidTeleport
            { new TextureKey(34, SpriteStates.StepForward_0), Properties.Resources.empty_teleport_0 },
            { new TextureKey(34, SpriteStates.StepForward_1), Properties.Resources.empty_teleport_1 },
            //VoidStalker
            { new TextureKey(35, SpriteStates.Static), Properties.Resources.empty_teleport_0 },
            //Stalker
            { new TextureKey(36, SpriteStates.StepForward_0), Properties.Resources.fake_teleport_0 },
            { new TextureKey(36, SpriteStates.StepForward_1), Properties.Resources.fake_teleport_1 },
            //Shooter
            { new TextureKey(37, SpriteStates.StepForward_0), Properties.Resources.player },
            { new TextureKey(37, SpriteStates.StepForward_1), Properties.Resources.player_1 },
            { new TextureKey(37, SpriteStates.DeadBody), Properties.Resources.player_Dead },
            { new TextureKey(37, SpriteStates.Aiming), Properties.Resources. player_aiming},
            { new TextureKey(37, SpriteStates.Shooted), Properties.Resources.player_shooted },
            { new TextureKey(37, SpriteStates.StepEscape_0), Properties.Resources.player_stoped },
            { new TextureKey(37, SpriteStates.StepEscape_1), Properties.Resources.player_stoped },
            //VoidWall
            { new TextureKey(38, SpriteStates.Static), Properties.Resources.void_wall },
            //VoidFloor
            { new TextureKey(39, SpriteStates.Static), Properties.Resources.void_floor },
            //VoidCelling
            { new TextureKey(40, SpriteStates.Static), Properties.Resources. void_ceiling},
            //LostSoul
            { new TextureKey(41, SpriteStates.StepForward_0), Properties.Resources. lost_soul_c_0},
            { new TextureKey(41, SpriteStates.StepForward_1), Properties.Resources. lost_soul_c_1},
            { new TextureKey(41, SpriteStates.DeadBody), Properties.Resources.missing },
            { new TextureKey(41, SpriteStates.Aiming), Properties.Resources. lost_soul_c_shoot},
            { new TextureKey(41, SpriteStates.Shooted), Properties.Resources. lost_soul_c_shoot},
            { new TextureKey(41, SpriteStates.StepEscape_0), Properties.Resources.lost_soul_c_escaping_0 },
            { new TextureKey(41, SpriteStates.StepEscape_1), Properties.Resources.lost_soul_c_escaping_1 },
            //SoulClot
            { new TextureKey(42, SpriteStates.StepForward_0), Properties.Resources. rpg_rocket_0},
            { new TextureKey(42, SpriteStates.StepForward_1), Properties.Resources. rpg_rocket_1},
        };
        private readonly Color[] COLORS =
        {
            //bound
            Color.FromArgb(90, 80, 90),
            //dark
            Color.Black,
            //backrooms
            Color.FromArgb(139, 126, 89),
            //void
            Color.FromArgb(0, 0, 0)
        };
        private readonly Color[] CUTE_COLORS =
        {
            //bound
            Color.FromArgb(98, 138, 82),
            //dark
            Color.White,
            //backrooms
            Color.FromArgb(139, 126, 89),
            //void
            Color.FromArgb(0, 0, 0)
        };
        private Dictionary<TextureKey, Color[,]> textureColorCache;
        private Dictionary<TextureKey, Color[,]> textureCuteColorCache;

        internal async Task LoadTextures(IProgress<int> progress)
        {
            textureColorCache = new Dictionary<TextureKey, Color[,]>();
            textureCuteColorCache = new Dictionary<TextureKey, Color[,]>();
            int totalTextures = textures.Count + COLORS.Length;
            int processedTextures = 0;
            for (int i = 0; i < COLORS.Length; i++)
            {
                TextureKey key = new TextureKey(i, SpriteStates.Static);
                textureColorCache[key] = new Color[1, 1];
                textureColorCache[key][0, 0] = COLORS[i];
                processedTextures++;
                progress?.Report(processedTextures * 100 / totalTextures);
                await Task.Delay(100);
            }
            foreach (var kvp in textures)
            {
                TextureKey key = kvp.Key;
                Image image = kvp.Value;
                textureColorCache[key] = ProcessImage(image);
                processedTextures++;
                progress?.Report(processedTextures * 100 / totalTextures);
                await Task.Delay(100);
            }
            textures.Clear();
            textures = null;
            totalTextures = cute_textures.Count + CUTE_COLORS.Length;
            processedTextures = 0;
            for (int i = 0; i < CUTE_COLORS.Length; i++)
            {
                TextureKey key = new TextureKey(i, SpriteStates.Static);
                textureCuteColorCache[key] = new Color[1, 1];
                textureCuteColorCache[key][0, 0] = CUTE_COLORS[i];
                processedTextures++;
                progress?.Report(processedTextures * 100 / totalTextures);
                await Task.Delay(100);
            }
            foreach (var kvp in cute_textures)
            {
                TextureKey key = kvp.Key;
                Image image = kvp.Value;
                textureCuteColorCache[key] = ProcessImage(image);
                processedTextures++;
                progress?.Report(processedTextures * 100 / totalTextures);
                await Task.Delay(100);
            }
            cute_textures.Clear();
            cute_textures = null;
        }

        private static Color[,] ProcessImage(Image image)
        {
            Bitmap textureBitmap = new Bitmap(image);
            BitmapData bitmapData = textureBitmap.LockBits(new Rectangle(0, 0, textureBitmap.Width, textureBitmap.Height), ImageLockMode.ReadOnly, textureBitmap.PixelFormat);
            int bytesPerPixel = Bitmap.GetPixelFormatSize(textureBitmap.PixelFormat) / 8;
            int byteCount = bitmapData.Stride * textureBitmap.Height;
            byte[] pixels = new byte[byteCount];
            System.Runtime.InteropServices.Marshal.Copy(bitmapData.Scan0, pixels, 0, byteCount);
            textureBitmap.UnlockBits(bitmapData);
            return CacheTextureColors(pixels, bitmapData.Stride, textureBitmap.Width, textureBitmap.Height, bytesPerPixel);
        }

        private static Color[,] CacheTextureColors(byte[] pixels, int stride, int width, int height, int bytesPerPixel)
        {
            Color[,] colors = new Color[width, height];
            for (int y = 0; y < height; y++)
            {
                int rowOffset = y * stride;
                for (int x = 0; x < width; x++)
                {
                    int offset = rowOffset + x * bytesPerPixel;
                    byte[] bytes = new byte[4];
                    bytes[0] = pixels[offset];
                    bytes[1] = pixels[offset + 1];
                    bytes[2] = pixels[offset + 2];
                    if (bytesPerPixel == 4)
                        bytes[3] = pixels[offset + 3];
                    if (bytes[3] <= 50) colors[x, y] = Color.Transparent;
                    else colors[x, y] = Color.FromArgb(bytes[3], bytes[2], bytes[1], bytes[0]);
                }
            }
            return colors;
        }

        internal bool IsTransparent(int textureId, SpriteStates spriteState, int x, int y, bool cuteMode)
        {
            var colorCache = cuteMode ? textureCuteColorCache : textureColorCache;
            if (!colorCache.TryGetValue(new TextureKey(textureId, spriteState), out var colors))
                return true;
            Color color = colors[x, y];
            return color.A <= 50;
        }

        internal Color GetTextureColor(int textureId, SpriteStates spriteState, int x, int y, int blackout, bool cuteMode)
        {
            if (blackout == 100) return cuteMode ? CUTE_COLORS[1] : COLORS[1];
            var colorCache = cuteMode ? textureCuteColorCache : textureColorCache;
            if (!colorCache.TryGetValue(new TextureKey(textureId, spriteState), out var colors))
                return Color.Transparent;
            Color color = colors[x, y];
            return AdjustColor(color, blackout, cuteMode);
        }

        private static Color AdjustColor(Color color, int blackout, bool lighten)
        {
            if (color.A <= 50) return Color.Transparent;
            int fogFactor = blackout;
            if (lighten) fogFactor = (fogFactor * 75) / 100;
            int inverseFogFactor = 100 - fogFactor;
            int baseComponent = lighten ? 255 : 0;
            int r = (color.R * inverseFogFactor + baseComponent * fogFactor) / 100;
            int g = (color.G * inverseFogFactor + baseComponent * fogFactor) / 100;
            int b = (color.B * inverseFogFactor + baseComponent * fogFactor) / 100;
            int a = lighten ? (color.A * inverseFogFactor + 255 * fogFactor) / 100 : color.A;
            r = ML.Clamp(r, 0, 255);
            g = ML.Clamp(g, 0, 255);
            b = ML.Clamp(b, 0, 255);
            a = ML.Clamp(a, 0, 255);
            return Color.FromArgb(a, r, g, b);
        }
    }
}