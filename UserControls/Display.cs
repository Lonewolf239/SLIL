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
        public WindowRenderTarget renderTarget;
        private readonly SharpDX.Direct2D1.Factory factory;
        private RawColor4 ClearColor = new RawColor4(12, 12, 50, 0);
        private int SCREEN_HEIGHT = 456, SCREEN_WIDTH = 256;
        public Bitmap SCREEN;
        private Bitmap buffer;

        public Display()
        {
            InitializeComponent();
            factory = new SharpDX.Direct2D1.Factory();
            InitializeRenderTarget();
        }

        private void InitializeRenderTarget()
        {
            renderTarget?.Dispose();
            RenderTargetProperties renderProps = new RenderTargetProperties(RenderTargetType.Default,
                new PixelFormat(Format.R8G8B8A8_UNorm, SharpDX.Direct2D1.AlphaMode.Premultiplied), 0, 0,
                RenderTargetUsage.None, FeatureLevel.Level_10);
            HwndRenderTargetProperties hwndRenderProps = new HwndRenderTargetProperties()
            {
                Hwnd = Handle,
                PixelSize = new Size2(SCREEN_WIDTH, SCREEN_HEIGHT),
                PresentOptions = PresentOptions.None
            };
            renderTarget = new WindowRenderTarget(factory, renderProps, hwndRenderProps);
        }

        public void DrawImage()
        {
            try
            {
                renderTarget.BeginDraw();
                renderTarget.Clear(ClearColor);
                if (SCREEN != null)
                {
                    if (buffer == null || !ReferenceEquals(SCREEN, buffer))
                        buffer = SCREEN;
                    RawRectangleF destinationRect = new RawRectangleF(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT);
                    renderTarget.DrawBitmap(buffer, destinationRect, 1.0f, BitmapInterpolationMode.Linear);
                }
                renderTarget.EndDraw();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Rendering error: {ex.Message}", $"Error {ex.HResult}", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        }

        public void ResizeImage(int width, int height)
        {
            SCREEN_WIDTH = width;
            SCREEN_HEIGHT = height;
            renderTarget?.Resize(new Size2(width, height));
        }
    }
}