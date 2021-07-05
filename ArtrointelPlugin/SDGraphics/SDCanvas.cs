using System.Drawing;
using System.Drawing.Drawing2D;

namespace ArtrointelPlugin.SDGraphics
{
    public class SDCanvas
    {
        public const int DEFAULT_IMAGE_SIZE = 144;

        private Image mImage;
        private Graphics mGraphics;

        public class CreateInfo
        {
            internal static readonly CreateInfo DEFAULT = new CreateInfo
            {
                CompositingMode = CompositingMode.SourceOver,
                SmoothingMode = SmoothingMode.HighSpeed,
                InterpolationMode = InterpolationMode.Default,
                PixelOffsetMode = PixelOffsetMode.HighSpeed,
                ClearColor = Color.Empty
            };

            public CompositingMode CompositingMode;
            public SmoothingMode SmoothingMode;
            public InterpolationMode InterpolationMode;
            public PixelOffsetMode PixelOffsetMode;
            public Color ClearColor;
            
            public CreateInfo()
            {
                CompositingMode = CompositingMode.SourceOver;
                SmoothingMode = SmoothingMode.HighSpeed;
                InterpolationMode = InterpolationMode.Default;
                PixelOffsetMode = PixelOffsetMode.HighSpeed;
                ClearColor = Color.Empty;
            }
        }

        public SDCanvas(Graphics graphics, Image image)
        {
            mGraphics = graphics;
            mImage = image;
        }

        public Image getImage()
        {
            return mImage;
        }

        public Graphics getGraphics()
        {
            return mGraphics;
        }

        public static SDCanvas CreateCanvas(CreateInfo info, int width = DEFAULT_IMAGE_SIZE, int height = DEFAULT_IMAGE_SIZE)
        {
            Bitmap bitmap = new Bitmap(width, height);
            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.CompositingMode = info.CompositingMode;
            graphics.SmoothingMode = info.SmoothingMode;
            graphics.InterpolationMode = info.InterpolationMode;
            graphics.PixelOffsetMode = info.PixelOffsetMode;
            graphics.Clear(info.ClearColor);
            return new SDCanvas(graphics, bitmap);
        }

        public static SDCanvas CreateCanvas(int width = DEFAULT_IMAGE_SIZE, int height = DEFAULT_IMAGE_SIZE)
        {
            return CreateCanvas(CreateInfo.DEFAULT, width, height);
        }

        public void Dispose()
        {
            mImage.Dispose();
            mGraphics.Dispose();
        }
    }
}
