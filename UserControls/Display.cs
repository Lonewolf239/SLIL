using System;
using System.Windows.Forms;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;

namespace SLIL.UserControls
{
    public partial class Display : UserControl
    {
        public WindowRenderTarget RenderTarget;
        private readonly SharpDX.Direct2D1.Factory RenderFactory;
        private static RawColor4 ClearColor = new RawColor4(12, 12, 50, 0);
        private static RawRectangleF DestinationRect;
        private int ScreenHeight = 456, ScreenWidth = 256;
        public Bitmap Screen;
        private Bitmap Buffer;

        public Display()
        {
            InitializeComponent();
            Cursor = Program.SLILCursorDefault;
            RenderFactory = new SharpDX.Direct2D1.Factory();
            DestinationRect = new RawRectangleF(0, 0, ScreenWidth, ScreenHeight);
            RenderTarget?.Dispose();
            RenderTargetProperties renderProps = new RenderTargetProperties(RenderTargetType.Default,
                new PixelFormat(Format.R8G8B8A8_UNorm, SharpDX.Direct2D1.AlphaMode.Premultiplied), 0, 0,
                RenderTargetUsage.None, FeatureLevel.Level_10);
            HwndRenderTargetProperties hwndRenderProps = new HwndRenderTargetProperties()
            {
                Hwnd = Handle,
                PixelSize = new Size2(ScreenWidth, ScreenHeight),
                PresentOptions = PresentOptions.None
            };
            RenderTarget = new WindowRenderTarget(RenderFactory, renderProps, hwndRenderProps);
        }

        public void DrawImage()
        {
            try
            {
                RenderTarget.BeginDraw();
                RenderTarget.Clear(ClearColor);
                if (Screen != null)
                {
                    if (Buffer == null || !ReferenceEquals(Screen, Buffer))
                        Buffer = Screen;
                    RenderTarget.DrawBitmap(Buffer, DestinationRect, 1.0f, BitmapInterpolationMode.Linear);
                }
                RenderTarget.EndDraw();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Rendering error: {ex.Message}", $"Error {ex.HResult}", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        }

        public void ResizeImage(int width, int height)
        {
            ScreenWidth = width;
            ScreenHeight = height;
            RenderTarget?.Resize(new Size2(width, height));
            DestinationRect = new RawRectangleF(0, 0, ScreenWidth, ScreenHeight);
        }
    }
}