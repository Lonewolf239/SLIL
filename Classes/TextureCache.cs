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

    internal class TextureCache
    {
        private Dictionary<(int, SpriteStates), Image> textures = new Dictionary<(int, SpriteStates), Image>()
        {
            //Wall
            { (4, SpriteStates.Static), Properties.Resources.wall },
            //Door
            { (5, SpriteStates.Static), Properties.Resources.door },
            //ShopDoor
            { (6, SpriteStates.Static), Properties.Resources.shop_door },
            //Floor
            { (7, SpriteStates.Static), Properties.Resources.floor },
            //Celling
            { (8, SpriteStates.Static), Properties.Resources.ceiling },
            //Zombie
            { (9, SpriteStates.StepForward_0), Properties.Resources.enemy_0 },
            { (9, SpriteStates.StepForward_1), Properties.Resources.enemy_0_1 },
            { (9, SpriteStates.DeadBody), Properties.Resources.enemy_0_Dead },
            //Dog
            { (10, SpriteStates.StepForward_0), Properties.Resources.enemy_1 },
            { (10, SpriteStates.StepForward_1), Properties.Resources.enemy_1_1 },
            { (10, SpriteStates.DeadBody), Properties.Resources.enemy_1_Dead },
            //Ogr
            { (11, SpriteStates.StepForward_0), Properties.Resources.enemy_2 },
            { (11, SpriteStates.StepForward_1), Properties.Resources.enemy_2_1 },
            { (11, SpriteStates.DeadBody), Properties.Resources.enemy_2_Dead },
            //Bat
            { (12, SpriteStates.StepForward_0), Properties.Resources.enemy_3 },
            { (12, SpriteStates.StepForward_1), Properties.Resources.enemy_3_1 },
            { (12, SpriteStates.DeadBody), Properties.Resources.enemy_3_Dead },
            //Teleport
            { (13, SpriteStates.StepForward_0), Properties.Resources.teleport_0 },
            { (13, SpriteStates.StepForward_1), Properties.Resources.teleport_1 },
            //ShopMan
            { (14, SpriteStates.Static), Properties.Resources.shop_man_0 },
            { (14, SpriteStates.FlashlightBlinded), Properties.Resources.shop_man_1 },
            //HittingTheWall
            { (15, SpriteStates.StepForward_0), Properties.Resources.hit_0 },
            { (15, SpriteStates.StepForward_1), Properties.Resources.hit_1 },
            //Box
            { (16, SpriteStates.Static), Properties.Resources.box },
            { (16, SpriteStates.DeadBody), Properties.Resources.box_broken },
            //Barrel
            { (17, SpriteStates.Static), Properties.Resources.barrel },
            { (17, SpriteStates.DeadBody), Properties.Resources.barrel_broken },
            //Vine
            { (18, SpriteStates.Static), Properties.Resources.vine },
            //Lamp
            { (19, SpriteStates.Static), Properties.Resources.lamp },
            //WallWithSing
            { (20, SpriteStates.Static), Properties.Resources.wall_with_sing },
            //Bike
            { (21, SpriteStates.Static), Properties.Resources.bike_forward },
            { (21, SpriteStates.StopForward), Properties.Resources.bike_forward },
            { (21, SpriteStates.StopBack), Properties.Resources.bike_back },
            { (21, SpriteStates.StopLeft), Properties.Resources.bike_left },
            { (21, SpriteStates.StopRight), Properties.Resources.bike_right },
            //SillyCat
            { (22, SpriteStates.StepForward_0), Properties.Resources.pet_cat_0 },
            { (22, SpriteStates.StepForward_1), Properties.Resources.pet_cat_1 },
            { (22, SpriteStates.StopForward), Properties.Resources.pet_cat_2 },
            { (22, SpriteStates.FlashlightBlinded), Properties.Resources.pet_cat_3 },
            //GreenGnome
            { (23, SpriteStates.StepForward_0), Properties.Resources.pet_gnome_0 },
            { (23, SpriteStates.StepForward_1), Properties.Resources.pet_gnome_1 },
            { (23, SpriteStates.FlashlightBlinded), Properties.Resources.pet_gnome_2 },
            //EnergyDrink
            { (24, SpriteStates.Static), Properties.Resources.pet_energy_drink_0 },
            //Pyro
            { (25, SpriteStates.StepForward_0), Properties.Resources.pet_pyro_0 },
            { (25, SpriteStates.StepForward_1), Properties.Resources.pet_pyro_1 },
            { (25, SpriteStates.FlashlightBlinded), Properties.Resources.pet_pyro_3 },
            //RpgRocket
            { (26, SpriteStates.StepForward_0), Properties.Resources.rpg_rocket_0 },
            { (26, SpriteStates.StepForward_1), Properties.Resources.rpg_rocket_1 },
            //RpgExplosion
            { (27, SpriteStates.StepForward_0), Properties.Resources.rpg_explosion_0 },
            { (27, SpriteStates.StepForward_1), Properties.Resources.rpg_explosion_1 },
            //PlayerDeadBody
            { (28, SpriteStates.Static), Properties.Resources.player_Dead },
            { (28, SpriteStates.DeadBody), Properties.Resources.player_Dead },
            //BackWall
            { (29, SpriteStates.Static), Properties.Resources.back_wall },
            //BackCelling
            { (30, SpriteStates.Static), Properties.Resources.back_ceiling },
            //BackFloor
            { (31, SpriteStates.Static), Properties.Resources.back_floor },
            //BackroomsTeleport
            { (32, SpriteStates.StepForward_0), Properties.Resources.fake_teleport_0 },
            { (32, SpriteStates.StepForward_1), Properties.Resources.fake_teleport_1 },
            //Covering
            { (33, SpriteStates.Static), Properties.Resources.fake_teleport_0},
            { (33, SpriteStates.StepForward_0), Properties.Resources.fake_teleport_0 },
            { (33, SpriteStates.StepForward_1), Properties.Resources.fake_teleport_0 },
            { (33, SpriteStates.DeadBody), Properties.Resources.fake_teleport_0 },
            //VoidTeleport
            { (34, SpriteStates.StepForward_0), Properties.Resources.empty_teleport_0 },
            { (34, SpriteStates.StepForward_1), Properties.Resources.empty_teleport_1 },
            //VoidStalker
            { (35, SpriteStates.Static), Properties.Resources.empty_teleport_0 },
            //Stalker
            { (36, SpriteStates.StepForward_0), Properties.Resources.fake_teleport_0 },
            { (36, SpriteStates.StepForward_1), Properties.Resources.fake_teleport_1 },
            //Shooter
            { (37, SpriteStates.StepForward_0), Properties.Resources.player },
            { (37, SpriteStates.StepForward_1), Properties.Resources.player_1 },
            { (37, SpriteStates.DeadBody), Properties.Resources.player_Dead },
            { (37, SpriteStates.Aiming), Properties.Resources.player_aiming},
            { (37, SpriteStates.Shooted), Properties.Resources.player_shooted },
            { (37, SpriteStates.StepEscape_0), Properties.Resources.player_stoped },
            { (37, SpriteStates.StepEscape_1), Properties.Resources.player_stoped },
            //VoidWall
            { (38, SpriteStates.Static), Properties.Resources.void_wall },
            //VoidFloor
            { (39, SpriteStates.Static), Properties.Resources.void_floor },
            //VoidCelling
            { (40, SpriteStates.Static), Properties.Resources.void_ceiling},
            //LostSoul
            { (41, SpriteStates.StepForward_0), Properties.Resources.lost_soul_0},
            { (41, SpriteStates.StepForward_1), Properties.Resources.lost_soul_1},
            { (41, SpriteStates.DeadBody), Properties.Resources.lost_soul_Dead },
            { (41, SpriteStates.Aiming), Properties.Resources.lost_soul_shoot},
            { (41, SpriteStates.Shooted), Properties.Resources.lost_soul_shoot},
            { (41, SpriteStates.StepEscape_0), Properties.Resources.lost_soul_escaping_0 },
            { (41, SpriteStates.StepEscape_1), Properties.Resources.lost_soul_escaping_1 },
            //SoulClot
            { (42, SpriteStates.StepForward_0), Properties.Resources.soul_clot_0},
            { (42, SpriteStates.StepForward_1), Properties.Resources.rpg_rocket_1},
            //ExplodingBarrel
            { (43, SpriteStates.StepForward_0), Properties.Resources.exploding_barrel_0},
            { (43, SpriteStates.StepForward_1), Properties.Resources.exploding_barrel_1},
            //AmmoBox
            { (44, SpriteStates.Static), Properties.Resources.missing},
            //BrokenAmmoBox
            { (45, SpriteStates.Static), Properties.Resources.back_wall},
            //BrokenDoor
            { (46, SpriteStates.Static), Properties.Resources.missing},
            //SoulExplosion
            { (47, SpriteStates.StepForward_0), Properties.Resources.soul_explosion_0},
            { (47, SpriteStates.StepForward_1), Properties.Resources.exploding_barrel_1},
            //MoneyPile
            { (48, SpriteStates.Static), Properties.Resources.money_pile},
            //Dummy
            { (49, SpriteStates.Static), Properties.Resources.dummy},
        };
        private Dictionary<(int , SpriteStates), Image> cute_textures = new Dictionary<(int, SpriteStates), Image>()
        {
            //Wall
            { (4, SpriteStates.Static), Properties.Resources.c_wall },
            //Door
            { (5, SpriteStates.Static), Properties.Resources.c_door },
            //ShopDoor
            { (6, SpriteStates.Static), Properties.Resources.c_shop_door },
            //Floor
            { (7, SpriteStates.Static), Properties.Resources.c_floor },
            //Celling
            { (8, SpriteStates.Static), Properties.Resources.c_ceiling },
            //Zombie
            { (9, SpriteStates.StepForward_0), Properties.Resources.c_enemy_0 },
            { (9, SpriteStates.StepForward_1), Properties.Resources.c_enemy_0_1 },
            { (9, SpriteStates.DeadBody), Properties.Resources.c_enemy_0_Dead },
            //Dog
            { (10, SpriteStates.StepForward_0), Properties.Resources.c_enemy_1 },
            { (10, SpriteStates.StepForward_1), Properties.Resources.c_enemy_1_1 },
            { (10, SpriteStates.DeadBody), Properties.Resources.c_enemy_1_Dead },
            //Ogr
            { (11, SpriteStates.StepForward_0), Properties.Resources.c_enemy_2 },
            { (11, SpriteStates.StepForward_1), Properties.Resources.c_enemy_2_1 },
            { (11, SpriteStates.DeadBody), Properties.Resources.c_enemy_2_Dead },
            //Bat
            { (12, SpriteStates.StepForward_0), Properties.Resources.c_enemy_3 },
            { (12, SpriteStates.StepForward_1), Properties.Resources.c_enemy_3_1 },
            { (12, SpriteStates.DeadBody), Properties.Resources.c_enemy_3_Dead },
            //Teleport
            { (13, SpriteStates.StepForward_0), Properties.Resources.c_teleport_0 },
            { (13, SpriteStates.StepForward_1), Properties.Resources.c_teleport_1 },
            //ShopMan
            { (14, SpriteStates.Static), Properties.Resources.shop_man_0 },
            { (14, SpriteStates.FlashlightBlinded), Properties.Resources.shop_man_1 },
            //HittingTheWall
            { (15, SpriteStates.StepForward_0), Properties.Resources.hit_0 },
            { (15, SpriteStates.StepForward_1), Properties.Resources.hit_1 },
            //Box
            { (16, SpriteStates.Static), Properties.Resources.box },
            { (16, SpriteStates.DeadBody), Properties.Resources.box_broken },
            //Barrel
            { (17, SpriteStates.Static), Properties.Resources.barrel },
            { (17, SpriteStates.DeadBody), Properties.Resources.barrel_broken },
            //Vine
            { (18, SpriteStates.Static), Properties.Resources.c_vine },
            //Lamp
            { (19, SpriteStates.Static), Properties.Resources.lamp },
            //WallWithSing
            { (20, SpriteStates.Static), Properties.Resources.wall_with_sing },
            //Bike
            { (21, SpriteStates.Static), Properties.Resources.bike_forward },
            { (21, SpriteStates.StopForward), Properties.Resources.bike_forward },
            { (21, SpriteStates.StopBack), Properties.Resources.bike_back },
            { (21, SpriteStates.StopLeft), Properties.Resources.bike_left },
            { (21, SpriteStates.StopRight), Properties.Resources.bike_right },
            //SillyCat
            { (22, SpriteStates.StepForward_0), Properties.Resources.pet_cat_0 },
            { (22, SpriteStates.StepForward_1), Properties.Resources.pet_cat_1 },
            { (22, SpriteStates.StopForward), Properties.Resources.pet_cat_2 },
            { (22, SpriteStates.FlashlightBlinded), Properties.Resources.pet_cat_3 },
            //GreenGnome
            { (23, SpriteStates.StepForward_0), Properties.Resources.pet_gnome_0 },
            { (23, SpriteStates.StepForward_1), Properties.Resources.pet_gnome_1 },
            { (23, SpriteStates.FlashlightBlinded), Properties.Resources.pet_gnome_2 },
            //EnergyDrink
            { (24, SpriteStates.Static), Properties.Resources.pet_energy_drink_0 },
            //Pyro
            { (25, SpriteStates.StepForward_0), Properties.Resources.pet_pyro_0 },
            { (25, SpriteStates.StepForward_1), Properties.Resources.pet_pyro_1 },
            { (25, SpriteStates.FlashlightBlinded), Properties.Resources.pet_pyro_3 },
            //RpgRocket
            { (26, SpriteStates.StepForward_0), Properties.Resources.rpg_rocket_0 },
            { (26, SpriteStates.StepForward_1), Properties.Resources.rpg_rocket_1 },
            //RpgExplosion
            { (27, SpriteStates.StepForward_0), Properties.Resources.rpg_explosion_0 },
            { (27, SpriteStates.StepForward_1), Properties.Resources.rpg_explosion_1 },
            //PlayerDeadBody
            { (28, SpriteStates.Static), Properties.Resources.player_Dead },
            { (28, SpriteStates.DeadBody), Properties.Resources.player_Dead },
            //BackWall
            { (29, SpriteStates.Static), Properties.Resources.back_wall },
            //BackCelling
            { (30, SpriteStates.Static), Properties.Resources.back_ceiling },
            //BackFloor
            { (31, SpriteStates.Static), Properties.Resources.back_floor },
            //BackroomsTeleport
            { (32, SpriteStates.StepForward_0), Properties.Resources.fake_teleport_0 },
            { (32, SpriteStates.StepForward_1), Properties.Resources.fake_teleport_1 },
            //Covering
            { (33, SpriteStates.Static), Properties.Resources.fake_teleport_0},
            { (33, SpriteStates.StepForward_0), Properties.Resources.fake_teleport_0 },
            { (33, SpriteStates.StepForward_1), Properties.Resources.fake_teleport_0 },
            { (33, SpriteStates.DeadBody), Properties.Resources.fake_teleport_0 },
            //VoidTeleport
            { (34, SpriteStates.StepForward_0), Properties.Resources.empty_teleport_0 },
            { (34, SpriteStates.StepForward_1), Properties.Resources.empty_teleport_1 },
            //VoidStalker
            { (35, SpriteStates.Static), Properties.Resources.empty_teleport_0 },
            //Stalker
            { (36, SpriteStates.StepForward_0), Properties.Resources.fake_teleport_0 },
            { (36, SpriteStates.StepForward_1), Properties.Resources.fake_teleport_1 },
            //Shooter
            { (37, SpriteStates.StepForward_0), Properties.Resources.player },
            { (37, SpriteStates.StepForward_1), Properties.Resources.player_1 },
            { (37, SpriteStates.DeadBody), Properties.Resources.player_Dead },
            { (37, SpriteStates.Aiming), Properties.Resources.player_aiming},
            { (37, SpriteStates.Shooted), Properties.Resources.player_shooted },
            { (37, SpriteStates.StepEscape_0), Properties.Resources.player_stoped },
            { (37, SpriteStates.StepEscape_1), Properties.Resources.player_stoped },
            //VoidWall
            { (38, SpriteStates.Static), Properties.Resources.void_wall },
            //VoidFloor
            { (39, SpriteStates.Static), Properties.Resources.void_floor },
            //VoidCelling
            { (40, SpriteStates.Static), Properties.Resources.void_ceiling},
            //LostSoul
            { (41, SpriteStates.StepForward_0), Properties.Resources.lost_soul_c_0},
            { (41, SpriteStates.StepForward_1), Properties.Resources.lost_soul_c_1},
            { (41, SpriteStates.DeadBody), Properties.Resources.lost_soul_c_Dead },
            { (41, SpriteStates.Aiming), Properties.Resources.lost_soul_c_shoot},
            { (41, SpriteStates.Shooted), Properties.Resources.lost_soul_c_shoot},
            { (41, SpriteStates.StepEscape_0), Properties.Resources.lost_soul_c_escaping_0 },
            { (41, SpriteStates.StepEscape_1), Properties.Resources.lost_soul_c_escaping_1 },
            //SoulClot
            { (42, SpriteStates.StepForward_0), Properties.Resources.soul_clot_c_0},
            { (42, SpriteStates.StepForward_1), Properties.Resources.rpg_rocket_1},
            //ExplodingBarrel
            { (43, SpriteStates.StepForward_0), Properties.Resources.exploding_barrel_0},
            { (43, SpriteStates.StepForward_1), Properties.Resources.exploding_barrel_1},
            //AmmoBox
            { (44, SpriteStates.Static), Properties.Resources.missing},
            //BrokenAmmoBox
            { (45, SpriteStates.Static), Properties.Resources.back_wall},
            //BrokenDoor
            { (46, SpriteStates.Static), Properties.Resources.missing},
            //SoulExplosion
            { (47, SpriteStates.StepForward_0), Properties.Resources.soul_explosion_c_0},
            { (47, SpriteStates.StepForward_1), Properties.Resources.exploding_barrel_1},
            //MoneyPile
            { (48, SpriteStates.Static), Properties.Resources.money_pile},
            //Dummy
            { (49, SpriteStates.Static), Properties.Resources.dummy},
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
        private Dictionary<(int, SpriteStates), Color[,]> textureColorCache;
        private Dictionary<(int, SpriteStates), Color[,]> textureCuteColorCache;

        internal async Task LoadTextures(IProgress<int> progress)
        {
            textureColorCache = new Dictionary<(int, SpriteStates), Color[,]>();
            textureCuteColorCache = new Dictionary<(int, SpriteStates), Color[,]>();
            int totalTextures = textures.Count + COLORS.Length;
            int processedTextures = 0;
            for (int i = 0; i < COLORS.Length; i++)
            {
                var key = (i, SpriteStates.Static);
                textureColorCache[key] = new Color[1, 1];
                textureColorCache[key][0, 0] = COLORS[i];
                processedTextures++;
                progress?.Report(processedTextures * 100 / totalTextures);
                await Task.Delay(100);
            }
            foreach (var kvp in textures)
            {
                var key = kvp.Key;
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
                var key = (i, SpriteStates.Static);
                textureCuteColorCache[key] = new Color[1, 1];
                textureCuteColorCache[key][0, 0] = CUTE_COLORS[i];
                processedTextures++;
                progress?.Report(processedTextures * 100 / totalTextures);
                await Task.Delay(100);
            }
            foreach (var kvp in cute_textures)
            {
                var key = kvp.Key;
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
                    if (bytesPerPixel == 4) bytes[3] = pixels[offset + 3];
                    if (bytes[3] <= 50) colors[x, y] = Color.Transparent;
                    else colors[x, y] = Color.FromArgb(bytes[3], bytes[2], bytes[1], bytes[0]);
                }
            }
            return colors;
        }

        internal bool IsTransparent(int textureId, SpriteStates spriteState, int x, int y, bool cuteMode)
        {
            var colorCache = cuteMode ? textureCuteColorCache : textureColorCache;
            if (!colorCache.TryGetValue((textureId, spriteState), out var colors))
                return true;
            return colors[x, y].A <= 50;
        }

        internal Color GetTextureColor(int textureId, SpriteStates spriteState, int x, int y, int blackout, bool cuteMode)
        {
            if (blackout == 100) return cuteMode ? CUTE_COLORS[1] : COLORS[1];
            var colorCache = cuteMode ? textureCuteColorCache : textureColorCache;
            if (!colorCache.TryGetValue((textureId, spriteState), out var colors))
                return Color.Transparent;
            return AdjustColor(colors[x, y], blackout, cuteMode);
        }

        private static Color AdjustColor(Color color, int blackout, bool lighten)
        {
            if (color.A <= 50) return Color.Transparent;
            int fogFactor = blackout;
            if (lighten) fogFactor = (fogFactor * 75) / 100;
            int inverseFogFactor = 100 - fogFactor;
            int baseComponent = lighten ? 255 : 0;
            int adjustIntense = baseComponent * fogFactor;
            int r = (color.R * inverseFogFactor + adjustIntense) / 100;
            int g = (color.G * inverseFogFactor + adjustIntense) / 100;
            int b = (color.B * inverseFogFactor + adjustIntense) / 100;
            int a = lighten ? (color.A * inverseFogFactor + adjustIntense) / 100 : color.A;
            r = ML.Clamp(r, 0, 255);
            g = ML.Clamp(g, 0, 255);
            b = ML.Clamp(b, 0, 255);
            a = ML.Clamp(a, 0, 255);
            return Color.FromArgb(a, r, g, b);
        }
    }
}