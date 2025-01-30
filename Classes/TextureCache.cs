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
            { 3, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.Static, Properties.Resources.wall }
                }
            },
            { 4, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.Static, Properties.Resources.door }
                }
            },
            { 5, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.Static, Properties.Resources.shop_door }
                }
            },
            { 6, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.Static, Properties.Resources.floor }
                }
            },
            { 7, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.Static, Properties.Resources.ceiling }
                }
            },
            { 8, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.StepForward_0, Properties.Resources.enemy_0 },
                    { SpriteStates.StepForward_1, Properties.Resources.enemy_0_1 },
                    { SpriteStates.DeadBody, Properties.Resources.enemy_0_DEAD },
                }
            },
            { 9, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.StepForward_0, Properties.Resources.enemy_1 },
                    { SpriteStates.StepForward_1, Properties.Resources.enemy_1_1 },
                    { SpriteStates.DeadBody, Properties.Resources.enemy_1_DEAD },
                }
            },
            { 10, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.StepForward_0, Properties.Resources.enemy_2 },
                    { SpriteStates.StepForward_1, Properties.Resources.enemy_2_1 },
                    { SpriteStates.DeadBody, Properties.Resources.enemy_2_DEAD },
                }
            },
            { 11, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.StepForward_0, Properties.Resources.enemy_3 },
                    { SpriteStates.StepForward_1, Properties.Resources.enemy_3_1 },
                    { SpriteStates.DeadBody, Properties.Resources.enemy_3_DEAD },
                }
            },
            { 12, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.StepForward_0, Properties.Resources.teleport_0 },
                    { SpriteStates.StepForward_1, Properties.Resources.teleport_1 }
                }
            },
            { 13, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.Static, Properties.Resources.shop_man_0 },
                    { SpriteStates.FlashlightBlinded, Properties.Resources.shop_man_1 }
                }
            },
            { 14, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.StepForward_0, Properties.Resources.hit_0 },
                    { SpriteStates.StepForward_1, Properties.Resources.hit_1 }
                }
            },
            { 15, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.Static, Properties.Resources.box },
                    { SpriteStates.DeadBody, Properties.Resources.box_broken }
                }
            },
            { 16, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.Static, Properties.Resources.barrel },
                    { SpriteStates.DeadBody, Properties.Resources.barrel_broken }
                }
            },
            { 17, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.Static, Properties.Resources.vine },
                }
            },
            { 18, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.Static, Properties.Resources.lamp },
                }
            },
            { 19, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.Static, Properties.Resources.wall_with_sing },
                }
            },
            { 20, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.Static, Properties.Resources.bike_forward },
                    { SpriteStates.StopForward, Properties.Resources.bike_forward },
                    { SpriteStates.StopBack, Properties.Resources.bike_back },
                    { SpriteStates.StopLeft, Properties.Resources.bike_left },
                    { SpriteStates.StopRight, Properties.Resources.bike_right },
                }
            },
            { 21, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.StepForward_0, Properties.Resources.pet_cat_0 },
                    { SpriteStates.StepForward_1, Properties.Resources.pet_cat_1 },
                    { SpriteStates.StopForward, Properties.Resources.pet_cat_2 },
                    { SpriteStates.FlashlightBlinded, Properties.Resources.pet_cat_3 },
                }
            },
            { 22, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.StepForward_0, Properties.Resources.pet_gnome_0 },
                    { SpriteStates.StepForward_1, Properties.Resources.pet_gnome_1 },
                    { SpriteStates.FlashlightBlinded, Properties.Resources.pet_gnome_2 },
                }
            },
            { 23, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.Static, Properties.Resources.pet_energy_drink_0 },
                }
            },
            { 24, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.StepForward_0, Properties.Resources.pet_pyro_0 },
                    { SpriteStates.StepForward_1, Properties.Resources.pet_pyro_1 },
                    { SpriteStates.FlashlightBlinded, Properties.Resources.pet_pyro_3 },
                }
            },
            { 25, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.StepForward_0, Properties.Resources.rpg_rocket_0 },
                    { SpriteStates.StepForward_1, Properties.Resources.rpg_rocket_1 },
                }
            },
            { 26, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.StepForward_0, Properties.Resources.rpg_explosion_0 },
                    { SpriteStates.StepForward_1, Properties.Resources.rpg_explosion_1 },
                }
            },
            { 27, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.DeadBody, Properties.Resources.player_DEAD },
                    { SpriteStates.Static, Properties.Resources.player_DEAD },
                }
            },
            { 28, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.Static, Properties.Resources.back_wall }
                }
            },
            { 29, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.Static, Properties.Resources.back_ceiling }
                }
            },
            { 30, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.Static, Properties.Resources.back_floor }
                }
            },
            { 31, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.StepForward_0, Properties.Resources.fake_teleport_0 },
                    { SpriteStates.StepForward_1, Properties.Resources.fake_teleport_1 }
                }
            },
            { 32, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.Static, Properties.Resources.fake_teleport_0 },
                    { SpriteStates.StepForward_0, Properties.Resources.fake_teleport_0 },
                    { SpriteStates.StepForward_1, Properties.Resources.fake_teleport_0 },
                    { SpriteStates.DeadBody, Properties.Resources.fake_teleport_0 }
                }
            },
            { 33, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.StepForward_0, Properties.Resources.empty_teleport_0 },
                    { SpriteStates.StepForward_1, Properties.Resources.empty_teleport_1 }
                }
            },
            { 34, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.Static, Properties.Resources.empty_teleport_0 }
                }
            },
            { 35, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.StepForward_0, Properties.Resources.fake_teleport_0 },
                    { SpriteStates.StepForward_1, Properties.Resources.fake_teleport_1 }
                }
            },
            { 36, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.StepForward_0, Properties.Resources.player },
                    { SpriteStates.StepForward_1, Properties.Resources.player_1 },
                    { SpriteStates.DeadBody, Properties.Resources.player_DEAD },
                    { SpriteStates.Aiming, Properties.Resources.player_aiming },
                    { SpriteStates.Shooted, Properties.Resources.player_shooted },
                    { SpriteStates.StepEscape_0, Properties.Resources.player_stoped },
                    { SpriteStates.StepEscape_1, Properties.Resources.player_stoped },
                }
            },
        };
        private Dictionary<int, Dictionary<SpriteStates, Image>> cute_textures = new Dictionary<int, Dictionary<SpriteStates, Image>>()
        {
            { 3, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.Static, Properties.Resources.c_wall }
                }
            },
            { 4, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.Static, Properties.Resources.c_door }
                }
            },
            { 5, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.Static, Properties.Resources.c_shop_door }
                }
            },
            { 6, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.Static, Properties.Resources.c_floor }
                }
            },
            { 7, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.Static, Properties.Resources.c_ceiling }
                }
            },
            { 8, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.StepForward_0, Properties.Resources.c_enemy_0 },
                    { SpriteStates.StepForward_1, Properties.Resources.c_enemy_0_1 },
                    { SpriteStates.DeadBody, Properties.Resources.c_enemy_0_DEAD },
                }
            },
            { 9, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.StepForward_0, Properties.Resources.c_enemy_1 },
                    { SpriteStates.StepForward_1, Properties.Resources.c_enemy_1_1 },
                    { SpriteStates.DeadBody, Properties.Resources.c_enemy_1_DEAD },
                }
            },
            { 10, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.StepForward_0, Properties.Resources.c_enemy_2 },
                    { SpriteStates.StepForward_1, Properties.Resources.c_enemy_2_1 },
                    { SpriteStates.DeadBody, Properties.Resources.c_enemy_2_DEAD },
                }
            },
            { 11, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.StepForward_0, Properties.Resources.c_enemy_3 },
                    { SpriteStates.StepForward_1, Properties.Resources.c_enemy_3_1 },
                    { SpriteStates.DeadBody, Properties.Resources.c_enemy_3_DEAD },
                }
            },
            { 12, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.StepForward_0, Properties.Resources.c_teleport_0 },
                    { SpriteStates.StepForward_1, Properties.Resources.c_teleport_1 }
                }
            },
            { 13, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.Static, Properties.Resources.shop_man_0 },
                    { SpriteStates.FlashlightBlinded, Properties.Resources.shop_man_1 }
                }
            },
            { 14, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.StepForward_0, Properties.Resources.hit_0 },
                    { SpriteStates.StepForward_1, Properties.Resources.hit_1 }
                }
            },
            { 15, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.Static, Properties.Resources.box },
                    { SpriteStates.DeadBody, Properties.Resources.box_broken }
                }
            },
            { 16, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.Static, Properties.Resources.barrel },
                    { SpriteStates.DeadBody, Properties.Resources.barrel_broken }
                }
            },
            { 17, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.Static, Properties.Resources.c_vine },
                }
            },
            { 18, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.Static, Properties.Resources.lamp },
                }
            },
            { 19, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.Static, Properties.Resources.wall_with_sing },
                }
            },
            { 20, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.Static, Properties.Resources.bike_forward },
                    { SpriteStates.StopForward, Properties.Resources.bike_forward },
                    { SpriteStates.StopBack, Properties.Resources.bike_back },
                    { SpriteStates.StopLeft, Properties.Resources.bike_left },
                    { SpriteStates.StopRight, Properties.Resources.bike_right },
                }
            },
            { 21, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.StepForward_0, Properties.Resources.pet_cat_0 },
                    { SpriteStates.StepForward_1, Properties.Resources.pet_cat_1 },
                    { SpriteStates.StopForward, Properties.Resources.pet_cat_2 },
                    { SpriteStates.FlashlightBlinded, Properties.Resources.pet_cat_3 },
                }
            },
            { 22, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.StepForward_0, Properties.Resources.pet_gnome_0 },
                    { SpriteStates.StepForward_1, Properties.Resources.pet_gnome_1 },
                    { SpriteStates.FlashlightBlinded, Properties.Resources.pet_gnome_2 },
                }
            },
            { 23, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.Static, Properties.Resources.pet_energy_drink_0 },
                }
            },
            { 24, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.StepForward_0, Properties.Resources.pet_pyro_0 },
                    { SpriteStates.StepForward_1, Properties.Resources.pet_pyro_1 },
                    { SpriteStates.FlashlightBlinded, Properties.Resources.pet_pyro_3 },
                }
            },
            { 25, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.StepForward_0, Properties.Resources.rpg_rocket_0 },
                    { SpriteStates.StepForward_1, Properties.Resources.rpg_rocket_1 },
                }
            },
            { 26, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.StepForward_0, Properties.Resources.rpg_explosion_0 },
                    { SpriteStates.StepForward_1, Properties.Resources.rpg_explosion_1 },
                }
            },
            { 27, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.DeadBody, Properties.Resources.player_DEAD },
                    { SpriteStates.Static, Properties.Resources.player_DEAD },
                }
            },
            { 28, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.Static, Properties.Resources.back_wall }
                }
            },
            { 29, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.Static, Properties.Resources.back_ceiling }
                }
            },
            { 30, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.Static, Properties.Resources.back_floor }
                }
            },
            { 31, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.StepForward_0, Properties.Resources.fake_teleport_0 },
                    { SpriteStates.StepForward_1, Properties.Resources.fake_teleport_1 }
                }
            },
            { 32, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.Static, Properties.Resources.fake_teleport_0 },
                    { SpriteStates.StepForward_0, Properties.Resources.fake_teleport_0 },
                    { SpriteStates.StepForward_1, Properties.Resources.fake_teleport_0 },
                    { SpriteStates.DeadBody, Properties.Resources.fake_teleport_0 }
                }
            },
            { 33, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.StepForward_0, Properties.Resources.empty_teleport_0 },
                    { SpriteStates.StepForward_1, Properties.Resources.empty_teleport_1 }
                }
            },
            { 34, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.Static, Properties.Resources.empty_teleport_0 }
                }
            },
            { 35, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.StepForward_0, Properties.Resources.fake_teleport_0 },
                    { SpriteStates.StepForward_1, Properties.Resources.fake_teleport_1 }
                }
            },
            { 36, new Dictionary<SpriteStates, Image>()
                {
                    { SpriteStates.StepForward_0, Properties.Resources.player },
                    { SpriteStates.StepForward_1, Properties.Resources.player_1 },
                    { SpriteStates.DeadBody, Properties.Resources.player_DEAD },
                    { SpriteStates.Aiming, Properties.Resources.player_aiming },
                    { SpriteStates.Shooted, Properties.Resources.player_shooted },
                    { SpriteStates.StepEscape_0, Properties.Resources.player_stoped },
                    { SpriteStates.StepEscape_1, Properties.Resources.player_stoped },
                }
            },
        };
        private readonly Color[] COLORS =
        {
            //bound
            Color.FromArgb(90, 80, 90),
            //dark
            Color.Black,
            //back bound
            Color.FromArgb(139, 126, 89)
        };
        private readonly Color[] CUTE_COLORS =
        {
            //bound
            Color.FromArgb(98, 138, 82),
            //dark
            Color.White,
            //back bound
            Color.FromArgb(139, 126, 89)
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
                if (blackout >= 96)
                    return CUTE_COLORS[1];
                return LightenColor(textureCuteColorCache[textureId][spriteState][x, y], blackout);
            }
            if (blackout >= 96)
                return COLORS[1];
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
            blackout = 0.65f / (0.96f - (blackout / 100));
            int r = (int)(color.R * blackout);
            int g = (int)(color.G * blackout);
            int b = (int)(color.B * blackout);
            return Color.FromArgb(color.A, Math.Min(r, 255), Math.Min(g, 255), Math.Min(b, 255));
        }

        private Color DarkenColor(Color color, float blackout)
        {
            if (color.A <= 50) return Color.Transparent;
            blackout = 0.96f - (blackout / 100);
            int r = (int)(color.R * blackout);
            int g = (int)(color.G * blackout);
            int b = (int)(color.B * blackout);
            return Color.FromArgb(color.A, Math.Min(r, 255), Math.Min(g, 255), Math.Min(b, 255));
        }
    }
}