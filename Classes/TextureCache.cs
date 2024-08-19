﻿using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace SLIL.Classes
{
    public class TextureCache
    {
        private readonly Image[] textures =
        { 
            Properties.Resources.wall,
            Properties.Resources.door,
            Properties.Resources.shop_door,
            Properties.Resources.teleport,
            Properties.Resources.floor,
            Properties.Resources.ceiling,
            Properties.Resources.enemy_0, //8
            Properties.Resources.enemy_0_1,
            Properties.Resources.enemy_0_DEAD,
            Properties.Resources.enemy_1, //11
            Properties.Resources.enemy_1_1,
            Properties.Resources.enemy_1_DEAD,
            Properties.Resources.enemy_2, //14
            Properties.Resources.enemy_2_1,
            Properties.Resources.enemy_2_DEAD,
            Properties.Resources.pet_cat_0, //17
            Properties.Resources.pet_cat_1,
            Properties.Resources.pet_cat_3,
            Properties.Resources.pet_cat_2,
            Properties.Resources.shop_man_0, //21
            Properties.Resources.shop_man_0,
            Properties.Resources.shop_man_1,
            Properties.Resources.pet_gnome_0, //24
            Properties.Resources.pet_gnome_1,
            Properties.Resources.pet_gnome_2,
            Properties.Resources.pet_energy_drink_0, //27
            Properties.Resources.enemy_3, //28
            Properties.Resources.enemy_3_1,
            Properties.Resources.enemy_3_DEAD,
            Properties.Resources.pet_pyro_0, //31
            Properties.Resources.pet_pyro_1,
            Properties.Resources.pet_pyro_3,
            Properties.Resources.teleport_0, //34
            Properties.Resources.teleport_1,
            Properties.Resources.hit_0, //36
            Properties.Resources.hit_1,
            Properties.Resources.player, //38
            Properties.Resources.player_1,
            Properties.Resources.player_stoped,
            Properties.Resources.player_aiming,
            Properties.Resources.player_shooted,
            Properties.Resources.player_stoped, //43
            Properties.Resources.box, //44
            Properties.Resources.box_broken,
            Properties.Resources.barrel, //46
            Properties.Resources.barrel_broken,
        };
        private readonly Image[] cute_textures =
        {
            Properties.Resources.c_wall,
            Properties.Resources.c_door,
            Properties.Resources.c_shop_door,
            Properties.Resources.teleport,
            Properties.Resources.c_floor,
            Properties.Resources.c_ceiling,
            Properties.Resources.c_enemy_0, //8
            Properties.Resources.c_enemy_0_1,
            Properties.Resources.c_enemy_0_DEAD,
            Properties.Resources.c_enemy_1, //11
            Properties.Resources.c_enemy_1_1,
            Properties.Resources.c_enemy_1_DEAD,
            Properties.Resources.c_enemy_2, //14
            Properties.Resources.c_enemy_2_1,
            Properties.Resources.c_enemy_2_DEAD,
            Properties.Resources.pet_cat_0, //17
            Properties.Resources.pet_cat_1,
            Properties.Resources.pet_cat_3,
            Properties.Resources.pet_cat_2,
            Properties.Resources.shop_man_0, //21
            Properties.Resources.shop_man_0,
            Properties.Resources.shop_man_1,
            Properties.Resources.pet_gnome_0, //24
            Properties.Resources.pet_gnome_1,
            Properties.Resources.pet_gnome_2,
            Properties.Resources.pet_energy_drink_0, //27
            Properties.Resources.c_enemy_3, //28
            Properties.Resources.c_enemy_3_1,
            Properties.Resources.c_enemy_3_DEAD,
            Properties.Resources.pet_pyro_0, //31
            Properties.Resources.pet_pyro_1,
            Properties.Resources.pet_pyro_3,
            Properties.Resources.c_teleport_0, //34
            Properties.Resources.c_teleport_1,
            Properties.Resources.hit_0, //36
            Properties.Resources.hit_1,
            Properties.Resources.player, //38
            Properties.Resources.player_1,
            Properties.Resources.player_stoped,
            Properties.Resources.player_aiming,
            Properties.Resources.player_shooted,
            Properties.Resources.player_stoped, //43
            Properties.Resources.box, //44
            Properties.Resources.box_broken,
            Properties.Resources.barrel, //46
            Properties.Resources.barrel_broken,
        };
        private readonly Color[] COLORS =
        {
            //bound
            Color.FromArgb(90, 80, 90),
            //dark
            Color.Black
        };
        private readonly Color[] CUTE_COLORS =
        {
            //bound
            Color.FromArgb(98, 138, 82),
            //dark
            Color.White
        };
        private readonly Color[][,] textureColorCache;
        private readonly Color[][,] textureCuteColorCache;

        public TextureCache()
        {
            int textureCount = textures.Length + COLORS.Length;
            textureColorCache = new Color[textureCount][,];
            textureCuteColorCache = new Color[textureCount][,];
            for (int id = 0; id < COLORS.Length; id++)
            {
                textureColorCache[id] = new Color[1, 1];
                textureColorCache[id][0, 0] = COLORS[id];
            }
            for (int id = COLORS.Length; id < textureCount; id++)
            {
                Bitmap textureBitmap = new Bitmap(textures[id - COLORS.Length]);
                BitmapData bitmapData = textureBitmap.LockBits(new Rectangle(0, 0, textureBitmap.Width, textureBitmap.Height), ImageLockMode.ReadOnly, textureBitmap.PixelFormat);
                int bytesPerPixel = Bitmap.GetPixelFormatSize(textureBitmap.PixelFormat) / 8;
                int byteCount = bitmapData.Stride * textureBitmap.Height;
                byte[] pixels = new byte[byteCount];
                System.Runtime.InteropServices.Marshal.Copy(bitmapData.Scan0, pixels, 0, byteCount);
                textureBitmap.UnlockBits(bitmapData);
                textureColorCache[id] = new Color[textureBitmap.Width, textureBitmap.Height];
                textureColorCache[id] = CacheTextureColors(pixels, bitmapData.Stride, textureBitmap.Width, textureBitmap.Height, bytesPerPixel);
            }
            for (int id = 0; id < CUTE_COLORS.Length; id++)
            {
                textureCuteColorCache[id] = new Color[1, 1];
                textureCuteColorCache[id][0, 0] = CUTE_COLORS[id];
            }
            for (int id = CUTE_COLORS.Length; id < textureCount; id++)
            {
                Bitmap textureBitmap = new Bitmap(cute_textures[id - CUTE_COLORS.Length]);
                BitmapData bitmapData = textureBitmap.LockBits(new Rectangle(0, 0, textureBitmap.Width, textureBitmap.Height), ImageLockMode.ReadOnly, textureBitmap.PixelFormat);
                int bytesPerPixel = Bitmap.GetPixelFormatSize(textureBitmap.PixelFormat) / 8;
                int byteCount = bitmapData.Stride * textureBitmap.Height;
                byte[] pixels = new byte[byteCount];
                System.Runtime.InteropServices.Marshal.Copy(bitmapData.Scan0, pixels, 0, byteCount);
                textureBitmap.UnlockBits(bitmapData);
                textureCuteColorCache[id] = new Color[textureBitmap.Width, textureBitmap.Height];
                textureCuteColorCache[id] = CacheTextureColors(pixels, bitmapData.Stride, textureBitmap.Width, textureBitmap.Height, bytesPerPixel);
            }
        }

        public Color GetTextureColor(int textureId, int x, int y, int blackout, bool cute)
        {
            if (cute)
            {
                if (blackout >= 96)
                    return CUTE_COLORS[1];
                return LightenColor(textureCuteColorCache[textureId][x, y], blackout);
            }
            if (blackout >= 96)
                return COLORS[1];
            return DarkenColor(textureColorCache[textureId][x, y], blackout);
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
            return Color.FromArgb(color.A, Math.Max(r, 0), Math.Max(g, 0), Math.Max(b, 0));
        }
    }
}