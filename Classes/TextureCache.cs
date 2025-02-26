using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SLIL.Classes
{
    public enum SpriteStates
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

    public class TextureCache
    {
        private Dictionary<int, Dictionary<SpriteStates, Image>> textures = new Dictionary<int, Dictionary<SpriteStates, Image>>()
        {
            { 4, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.Static, Properties.Resources.wall }
                }
            },
            { 5, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.Static, Properties.Resources.door }
                }
            },
            { 6, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.Static, Properties.Resources.shop_door }
                }
            },
            { 7, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.Static, Properties.Resources.floor }
                }
            },
            { 8, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.Static, Properties.Resources.ceiling }
                }
            },
            { 9, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.StepForward_0, Properties.Resources.enemy_0 },
                    { SpriteStates.StepForward_1, Properties.Resources.enemy_0_1 },
                    { SpriteStates.DeadBody, Properties.Resources.enemy_0_Dead },
                }
            },
            { 10, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.StepForward_0, Properties.Resources.enemy_1 },
                    { SpriteStates.StepForward_1, Properties.Resources.enemy_1_1 },
                    { SpriteStates.DeadBody, Properties.Resources.enemy_1_Dead },
                }
            },
            { 11, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.StepForward_0, Properties.Resources.enemy_2 },
                    { SpriteStates.StepForward_1, Properties.Resources.enemy_2_1 },
                    { SpriteStates.DeadBody, Properties.Resources.enemy_2_Dead },
                }
            },
            { 12, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.StepForward_0, Properties.Resources.enemy_3 },
                    { SpriteStates.StepForward_1, Properties.Resources.enemy_3_1 },
                    { SpriteStates.DeadBody, Properties.Resources.enemy_3_Dead },
                }
            },
            { 13, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.StepForward_0, Properties.Resources.teleport_0 },
                    { SpriteStates.StepForward_1, Properties.Resources.teleport_1 }
                }
            },
            { 14, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.Static, Properties.Resources.shop_man_0 },
                    { SpriteStates.FlashlightBlinded, Properties.Resources.shop_man_1 }
                }
            },
            { 15, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.StepForward_0, Properties.Resources.hit_0 },
                    { SpriteStates.StepForward_1, Properties.Resources.hit_1 }
                }
            },
            { 16, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.Static, Properties.Resources.box },
                    { SpriteStates.DeadBody, Properties.Resources.box_broken }
                }
            },
            { 17, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.Static, Properties.Resources.barrel },
                    { SpriteStates.DeadBody, Properties.Resources.barrel_broken }
                }
            },
            { 18, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.Static, Properties.Resources.vine },
                }
            },
            { 19, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.Static, Properties.Resources.lamp },
                }
            },
            { 20, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.Static, Properties.Resources.wall_with_sing },
                }
            },
            { 21, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.Static, Properties.Resources.bike_forward },
                    { SpriteStates.StopForward, Properties.Resources.bike_forward },
                    { SpriteStates.StopBack, Properties.Resources.bike_back },
                    { SpriteStates.StopLeft, Properties.Resources.bike_left },
                    { SpriteStates.StopRight, Properties.Resources.bike_right },
                }
            },
            { 22, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.StepForward_0, Properties.Resources.pet_cat_0 },
                    { SpriteStates.StepForward_1, Properties.Resources.pet_cat_1 },
                    { SpriteStates.StopForward, Properties.Resources.pet_cat_2 },
                    { SpriteStates.FlashlightBlinded, Properties.Resources.pet_cat_3 },
                }
            },
            { 23, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.StepForward_0, Properties.Resources.pet_gnome_0 },
                    { SpriteStates.StepForward_1, Properties.Resources.pet_gnome_1 },
                    { SpriteStates.FlashlightBlinded, Properties.Resources.pet_gnome_2 },
                }
            },
            { 24, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.Static, Properties.Resources.pet_energy_drink_0 },
                }
            },
            { 25, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.StepForward_0, Properties.Resources.pet_pyro_0 },
                    { SpriteStates.StepForward_1, Properties.Resources.pet_pyro_1 },
                    { SpriteStates.FlashlightBlinded, Properties.Resources.pet_pyro_3 },
                }
            },
            { 26, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.StepForward_0, Properties.Resources.rpg_rocket_0 },
                    { SpriteStates.StepForward_1, Properties.Resources.rpg_rocket_1 },
                }
            },
            { 27, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.StepForward_0, Properties.Resources.rpg_explosion_0 },
                    { SpriteStates.StepForward_1, Properties.Resources.rpg_explosion_1 },
                }
            },
            { 28, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.DeadBody, Properties.Resources.player_Dead },
                    { SpriteStates.Static, Properties.Resources.player_Dead },
                }
            },
            { 29, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.Static, Properties.Resources.back_wall }
                }
            },
            { 30, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.Static, Properties.Resources.back_ceiling }
                }
            },
            { 31, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.Static, Properties.Resources.back_floor }
                }
            },
            { 32, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.StepForward_0, Properties.Resources.fake_teleport_0 },
                    { SpriteStates.StepForward_1, Properties.Resources.fake_teleport_1 }
                }
            },
            { 33, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.Static, Properties.Resources.fake_teleport_0 },
                    { SpriteStates.StepForward_0, Properties.Resources.fake_teleport_0 },
                    { SpriteStates.StepForward_1, Properties.Resources.fake_teleport_0 },
                    { SpriteStates.DeadBody, Properties.Resources.fake_teleport_0 }
                }
            },
            { 34, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.StepForward_0, Properties.Resources.empty_teleport_0 },
                    { SpriteStates.StepForward_1, Properties.Resources.empty_teleport_1 }
                }
            },
            { 35, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.Static, Properties.Resources.empty_teleport_0 }
                }
            },
            { 36, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.StepForward_0, Properties.Resources.fake_teleport_0 },
                    { SpriteStates.StepForward_1, Properties.Resources.fake_teleport_1 }
                }
            },
            { 37, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.StepForward_0, Properties.Resources.player },
                    { SpriteStates.StepForward_1, Properties.Resources.player_1 },
                    { SpriteStates.DeadBody, Properties.Resources.player_Dead },
                    { SpriteStates.Aiming, Properties.Resources.player_aiming },
                    { SpriteStates.Shooted, Properties.Resources.player_shooted },
                    { SpriteStates.StepEscape_0, Properties.Resources.player_stoped },
                    { SpriteStates.StepEscape_1, Properties.Resources.player_stoped },
                }
            },
            { 38, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.Static, Properties.Resources.void_wall }
                }
            },
            { 39, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.Static, Properties.Resources.void_floor }
                }
            },
            { 40, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.Static, Properties.Resources.void_ceiling }
                }
            },
            { 41, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.StepForward_0, Properties.Resources.lost_soul_0 },
                    { SpriteStates.StepForward_1, Properties.Resources.lost_soul_1 },
                    { SpriteStates.DeadBody, Properties.Resources.missing },
                    { SpriteStates.Aiming, Properties.Resources.lost_soul_aiming },
                    { SpriteStates.Shooted, Properties.Resources.lost_soul_shoot },
                    { SpriteStates.StepEscape_0, Properties.Resources.lost_soul_escaping_0 },
                    { SpriteStates.StepEscape_1, Properties.Resources.lost_soul_escaping_1 }
                }
            },
            { 42, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.StepForward_0, Properties.Resources.rpg_rocket_0 },
                    { SpriteStates.StepForward_1, Properties.Resources.rpg_rocket_1 },
                }
            },
        };
        private Dictionary<int, Dictionary<SpriteStates, Image>> cute_textures = new Dictionary<int, Dictionary<SpriteStates, Image>>()
        {
            { 4, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.Static, Properties.Resources.c_wall }
                }
            },
            { 5, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.Static, Properties.Resources.c_door }
                }
            },
            { 6, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.Static, Properties.Resources.c_shop_door }
                }
            },
            { 7, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.Static, Properties.Resources.c_floor }
                }
            },
            { 8, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.Static, Properties.Resources.c_ceiling }
                }
            },
            { 9, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.StepForward_0, Properties.Resources.c_enemy_0 },
                    { SpriteStates.StepForward_1, Properties.Resources.c_enemy_0_1 },
                    { SpriteStates.DeadBody, Properties.Resources.c_enemy_0_Dead },
                }
            },
            { 10, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.StepForward_0, Properties.Resources.c_enemy_1 },
                    { SpriteStates.StepForward_1, Properties.Resources.c_enemy_1_1 },
                    { SpriteStates.DeadBody, Properties.Resources.c_enemy_1_Dead },
                }
            },
            { 11, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.StepForward_0, Properties.Resources.c_enemy_2 },
                    { SpriteStates.StepForward_1, Properties.Resources.c_enemy_2_1 },
                    { SpriteStates.DeadBody, Properties.Resources.c_enemy_2_Dead },
                }
            },
            { 12, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.StepForward_0, Properties.Resources.c_enemy_3 },
                    { SpriteStates.StepForward_1, Properties.Resources.c_enemy_3_1 },
                    { SpriteStates.DeadBody, Properties.Resources.c_enemy_3_Dead },
                }
            },
            { 13, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.StepForward_0, Properties.Resources.c_teleport_0 },
                    { SpriteStates.StepForward_1, Properties.Resources.c_teleport_1 }
                }
            },
            { 14, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.Static, Properties.Resources.shop_man_0 },
                    { SpriteStates.FlashlightBlinded, Properties.Resources.shop_man_1 }
                }
            },
            { 15, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.StepForward_0, Properties.Resources.hit_0 },
                    { SpriteStates.StepForward_1, Properties.Resources.hit_1 }
                }
            },
            { 16, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.Static, Properties.Resources.box },
                    { SpriteStates.DeadBody, Properties.Resources.box_broken }
                }
            },
            { 17, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.Static, Properties.Resources.barrel },
                    { SpriteStates.DeadBody, Properties.Resources.barrel_broken }
                }
            },
            { 18, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.Static, Properties.Resources.c_vine },
                }
            },
            { 19, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.Static, Properties.Resources.lamp },
                }
            },
            { 20, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.Static, Properties.Resources.wall_with_sing },
                }
            },
            { 21, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.Static, Properties.Resources.bike_forward },
                    { SpriteStates.StopForward, Properties.Resources.bike_forward },
                    { SpriteStates.StopBack, Properties.Resources.bike_back },
                    { SpriteStates.StopLeft, Properties.Resources.bike_left },
                    { SpriteStates.StopRight, Properties.Resources.bike_right },
                }
            },
            { 22, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.StepForward_0, Properties.Resources.pet_cat_0 },
                    { SpriteStates.StepForward_1, Properties.Resources.pet_cat_1 },
                    { SpriteStates.StopForward, Properties.Resources.pet_cat_2 },
                    { SpriteStates.FlashlightBlinded, Properties.Resources.pet_cat_3 },
                }
            },
            { 23, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.StepForward_0, Properties.Resources.pet_gnome_0 },
                    { SpriteStates.StepForward_1, Properties.Resources.pet_gnome_1 },
                    { SpriteStates.FlashlightBlinded, Properties.Resources.pet_gnome_2 },
                }
            },
            { 24, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.Static, Properties.Resources.pet_energy_drink_0 },
                }
            },
            { 25, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.StepForward_0, Properties.Resources.pet_pyro_0 },
                    { SpriteStates.StepForward_1, Properties.Resources.pet_pyro_1 },
                    { SpriteStates.FlashlightBlinded, Properties.Resources.pet_pyro_3 },
                }
            },
            { 26, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.StepForward_0, Properties.Resources.rpg_rocket_0 },
                    { SpriteStates.StepForward_1, Properties.Resources.rpg_rocket_1 },
                }
            },
            { 27, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.StepForward_0, Properties.Resources.rpg_explosion_0 },
                    { SpriteStates.StepForward_1, Properties.Resources.rpg_explosion_1 },
                }
            },
            { 28, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.DeadBody, Properties.Resources.player_Dead },
                    { SpriteStates.Static, Properties.Resources.player_Dead },
                }
            },
            { 29, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.Static, Properties.Resources.back_wall }
                }
            },
            { 30, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.Static, Properties.Resources.back_ceiling }
                }
            },
            { 31, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.Static, Properties.Resources.back_floor }
                }
            },
            { 32, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.StepForward_0, Properties.Resources.fake_teleport_0 },
                    { SpriteStates.StepForward_1, Properties.Resources.fake_teleport_1 }
                }
            },
            { 33, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.Static, Properties.Resources.fake_teleport_0 },
                    { SpriteStates.StepForward_0, Properties.Resources.fake_teleport_0 },
                    { SpriteStates.StepForward_1, Properties.Resources.fake_teleport_0 },
                    { SpriteStates.DeadBody, Properties.Resources.fake_teleport_0 }
                }
            },
            { 34, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.StepForward_0, Properties.Resources.empty_teleport_0 },
                    { SpriteStates.StepForward_1, Properties.Resources.empty_teleport_1 }
                }
            },
            { 35, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.Static, Properties.Resources.empty_teleport_0 }
                }
            },
            { 36, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.StepForward_0, Properties.Resources.fake_teleport_0 },
                    { SpriteStates.StepForward_1, Properties.Resources.fake_teleport_1 }
                }
            },
            { 37, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.StepForward_0, Properties.Resources.player },
                    { SpriteStates.StepForward_1, Properties.Resources.player_1 },
                    { SpriteStates.DeadBody, Properties.Resources.player_Dead },
                    { SpriteStates.Aiming, Properties.Resources.player_aiming },
                    { SpriteStates.Shooted, Properties.Resources.player_shooted },
                    { SpriteStates.StepEscape_0, Properties.Resources.player_stoped },
                    { SpriteStates.StepEscape_1, Properties.Resources.player_stoped },
                }
            },
            { 38, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.Static, Properties.Resources.void_wall }
                }
            },
            { 39, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.Static, Properties.Resources.void_floor }
                }
            },
            { 40, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.Static, Properties.Resources.void_ceiling }
                }
            },
            { 41, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.StepForward_0, Properties.Resources.lost_soul_c_0 },
                    { SpriteStates.StepForward_1, Properties.Resources.lost_soul_c_1 },
                    { SpriteStates.DeadBody, Properties.Resources.missing },
                    { SpriteStates.Aiming, Properties.Resources.lost_soul_aiming },
                    { SpriteStates.Shooted, Properties.Resources.lost_soul_c_shoot },
                    { SpriteStates.StepEscape_0, Properties.Resources.lost_soul_c_escaping_0 },
                    { SpriteStates.StepEscape_1, Properties.Resources.lost_soul_c_escaping_1 }
                }
            },
            { 42, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.StepForward_0, Properties.Resources.rpg_rocket_0 },
                    { SpriteStates.StepForward_1, Properties.Resources.rpg_rocket_1 },
                }
            },
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
        private Dictionary<int, Dictionary<SpriteStates, Color[,]>> textureColorCache;
        private Dictionary<int, Dictionary<SpriteStates, Color[,]>> textureCuteColorCache;

        public async Task LoadTextures(IProgress<int> progress)
        {
            int textureCount = textures.Count + COLORS.Length;
            textureColorCache = new Dictionary<int, Dictionary<SpriteStates, Color[,]>>();
            textureCuteColorCache = new Dictionary<int, Dictionary<SpriteStates, Color[,]>>();
            int stageCount = COLORS.Length;
            for (int i = 0; i < stageCount; i++)
            {
                textureColorCache.Add(i, new Dictionary<SpriteStates, Color[,]>() { { SpriteStates.Static, new Color[1, 1] } });
                textureColorCache[i][SpriteStates.Static][0, 0] = COLORS[i];
                progress?.Report(i * 100 / stageCount);
                await Task.Delay(100);
            }
            stageCount = textureCount - COLORS.Length;
            int currentStage;
            int processedTextures = 0;
            for (int i = COLORS.Length; i < textureCount; i++)
            {
                if (!textures.ContainsKey(i)) continue;
                var innerDict = textures[i];
                textureColorCache[i] = new Dictionary<SpriteStates, Color[,]>();
                foreach (var innerKvp in innerDict)
                    textureColorCache[i][innerKvp.Key] = ProcessImage(innerKvp.Value);
                processedTextures++;
                currentStage = COLORS.Length + processedTextures;
                progress.Report(currentStage * 100 / stageCount);
                await Task.Delay(100);
            }
            textures.Clear();
            textures = null;
            stageCount = CUTE_COLORS.Length;
            for (int i = 0; i < stageCount; i++)
            {
                textureCuteColorCache.Add(i, new Dictionary<SpriteStates, Color[,]>() { { SpriteStates.Static, new Color[1, 1] } });
                textureCuteColorCache[i][SpriteStates.Static][0, 0] = CUTE_COLORS[i];
                progress?.Report(i * 100 / stageCount);
                await Task.Delay(100);
            }
            stageCount = textureCount - CUTE_COLORS.Length;
            processedTextures = 0;
            for (int i = CUTE_COLORS.Length; i < textureCount; i++)
            {
                if (!cute_textures.ContainsKey(i)) continue;
                var innerDict = cute_textures[i];
                textureCuteColorCache[i] = new Dictionary<SpriteStates, Color[,]>();
                foreach (var innerKvp in innerDict)
                    textureCuteColorCache[i][innerKvp.Key] = ProcessImage(innerKvp.Value);
                processedTextures++;
                currentStage = CUTE_COLORS.Length + processedTextures;
                progress.Report(currentStage * 100 / stageCount);
                await Task.Delay(100);
            }
            cute_textures.Clear();
            cute_textures = null;
        }

        private Color[,] ProcessImage(Image image)
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

        public Color GetTextureColor(int textureId, SpriteStates spriteState, int x, int y, int blackout, bool cute)
        {
            if (cute)
            {
                if (blackout >= 96) return CUTE_COLORS[1];
                return LightenColor(textureCuteColorCache[textureId][spriteState][x, y], blackout);
            }
            if (blackout >= 96) return COLORS[1];
            return DarkenColor(textureColorCache[textureId][spriteState][x, y], blackout);
        }

        private Color[,] CacheTextureColors(byte[] pixels, int stride, int width, int height, int bytesPerPixel)
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
                    if (bytes[3] <= 50)
                        colors[x, y] = Color.Transparent;
                    else
                        colors[x, y] = Color.FromArgb(bytes[3], bytes[2], bytes[1], bytes[0]);
                }
            }
            return colors;
        }

        private Color LightenColor(Color color, float blackout)
        {
            if (color.A <= 50) return Color.Transparent;
            float fogFactor = (blackout / 100.0f) * 0.75f;
            int r = (int)(color.R * (1 - fogFactor) + 255 * fogFactor);
            int g = (int)(color.G * (1 - fogFactor) + 255 * fogFactor);
            int b = (int)(color.B * (1 - fogFactor) + 255 * fogFactor);
            int a = (int)(color.A * (1 - fogFactor) + 255 * fogFactor);
            r = ML.Clamp(r, 0, 255);
            g = ML.Clamp(g, 0, 255);
            b = ML.Clamp(b, 0, 255);
            a = ML.Clamp(a, 0, 255);
            return Color.FromArgb(a, r, g, b);
        }

        private Color DarkenColor(Color color, float blackout)
        {
            if (color.A <= 50) return Color.Transparent;
            float fogFactor = blackout / 100.0f;
            int r = (int)(color.R * (1 - fogFactor));
            int g = (int)(color.G * (1 - fogFactor));
            int b = (int)(color.B * (1 - fogFactor));
            r = ML.Clamp(r, 0, 255);
            g = ML.Clamp(g, 0, 255);
            b = ML.Clamp(b, 0, 255);
            return Color.FromArgb(color.A, r, g, b);
        }
    }
}