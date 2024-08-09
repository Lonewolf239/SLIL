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
        };
        private readonly Image[] cute_textures =
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
            Properties.Resources.c_enemy_3, //28
            Properties.Resources.c_enemy_3_1,
            Properties.Resources.c_enemy_3_DEAD,
            Properties.Resources.pet_pyro_0, //31
            Properties.Resources.pet_pyro_1,
            Properties.Resources.pet_pyro_3,
            Properties.Resources.teleport_0, //34
            Properties.Resources.teleport_1,
            Properties.Resources.hit_0, //36
            Properties.Resources.hit_1,
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
            Color.FromArgb(90, 80, 90),
            //dark
            Color.FromArgb(150, 140, 150)
        };
        private readonly Color[,][,] textureColorCache;
        private readonly Color[,][,] textureCuteColorCache;

        public TextureCache()
        {
            int textureCount = textures.Length + COLORS.Length;
            textureColorCache = new Color[textureCount, 101][,];
            textureCuteColorCache = new Color[textureCount, 101][,];
            for (int id = 0; id < COLORS.Length; id++)
            {
                Color color = COLORS[id];
                for (int blackout = 0; blackout <= 100; blackout += 10)
                {
                    if (blackout == 100)
                        textureColorCache[id, blackout] = GenerateBlackTexture(1, 1, COLORS[1]);
                    else
                        textureColorCache[id, blackout] = CacheColor(color, blackout);
                }
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
                for (int blackout = 0; blackout <= 100; blackout += 10)
                {
                    float darkenAmount = 1 - (blackout / 100f);
                    if (blackout == 100)
                        textureColorCache[id, blackout] = GenerateBlackTexture(textureBitmap.Width, textureBitmap.Height, COLORS[1]);
                    else
                        textureColorCache[id, blackout] = CacheTextureColors(pixels, bitmapData.Stride, textureBitmap.Width, textureBitmap.Height, bytesPerPixel, darkenAmount);
                }
            }
            for (int id = 0; id < CUTE_COLORS.Length; id++)
            {
                Color color = CUTE_COLORS[id];
                for (int blackout = 0; blackout <= 100; blackout += 10)
                {
                    if (blackout == 100)
                        textureCuteColorCache[id, blackout] = GenerateBlackTexture(1, 1, CUTE_COLORS[1]);
                    else
                        textureCuteColorCache[id, blackout] = CacheColor(color, blackout);
                }
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
                for (int blackout = 0; blackout <= 100; blackout += 10)
                {
                    float darkenAmount = 1 - (blackout / 100f);
                    if (blackout == 100)
                        textureCuteColorCache[id, blackout] = GenerateBlackTexture(textureBitmap.Width, textureBitmap.Height, CUTE_COLORS[1]);
                    else
                        textureCuteColorCache[id, blackout] = CacheTextureColors(pixels, bitmapData.Stride, textureBitmap.Width, textureBitmap.Height, bytesPerPixel, darkenAmount);
                }
            }
        }

        public Color GetTextureColor(int textureId, int x, int y, int blackout, bool cute)
        {
            if (cute)
                return textureCuteColorCache[textureId, blackout][x, y];
            return textureColorCache[textureId, blackout][x, y];
        }

        private Color[,] GenerateBlackTexture(int width, int height, Color color)
        {
            Color[,] blackTexture = new Color[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                    blackTexture[x, y] = color;
            }
            return blackTexture;
        }

        private Color[,] CacheTextureColors(byte[] pixels, int stride, int width, int height, int bytesPerPixel, float darkenAmount)
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
                    if (bytes[3] == 0)
                        colors[x, y] = Color.Transparent;
                    else
                    {
                        Color originalColor = Color.FromArgb(bytes[3], bytes[2], bytes[1], bytes[0]);
                        colors[x, y] = DarkenColor(originalColor, darkenAmount);
                    }
                }
            }
            return colors;
        }

        private Color[,] CacheColor(Color color, int blackout)
        {
            float darkenAmount = 1 - (blackout / 100f);
            return new Color[,] { { DarkenColor(color, darkenAmount) } };
        }

        private Color DarkenColor(Color color, float darkenAmount) => Color.FromArgb(color.A, (int)(color.R * darkenAmount), (int)(color.G * darkenAmount), (int)(color.B * darkenAmount));
    }
}