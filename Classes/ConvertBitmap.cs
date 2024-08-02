using SharpDX;
using SharpDX.DXGI;
using SharpDX.Direct2D1;
using System.Drawing;
using System.Drawing.Imaging;

namespace Convert_Bitmap
{
    public class ConvertBitmap
    {

        private static SharpDX.Direct2D1.PixelFormat pixelFormat = new SharpDX.Direct2D1.PixelFormat(Format.B8G8R8A8_UNorm, SharpDX.Direct2D1.AlphaMode.Premultiplied);
        private static BitmapProperties bitmapProperties = new BitmapProperties(pixelFormat);

        public static SharpDX.Direct2D1.Bitmap ToDX(System.Drawing.Bitmap sourceBitmap, RenderTarget renderTarget)
        {
            Rectangle bounds = new Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height);
            BitmapData bitmapData = sourceBitmap.LockBits(bounds, ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            DataPointer dataPointer = new DataPointer(bitmapData.Scan0, bitmapData.Stride * bitmapData.Height);
            SharpDX.Direct2D1.Bitmap dxBitmap = new SharpDX.Direct2D1.Bitmap(renderTarget, new Size2(sourceBitmap.Width, sourceBitmap.Height), dataPointer, bitmapData.Stride, bitmapProperties);
            sourceBitmap.UnlockBits(bitmapData);
            return dxBitmap;
        }
    }
}