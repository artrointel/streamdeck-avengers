using System.Drawing;
using System.Drawing.Drawing2D;

namespace ArtrointelPlugin.SDGraphics
{
    public class SDCanvas
    {
        public const int DEFAULT_IMAGE_SIZE = 144;

        private Image mImage;
        private Graphics mGraphicsForImage;
        private BufferedGraphicsContext mCtx;
        private BufferedGraphics mBufferedGfx; // TODO on swapbuffer async callback??
        
        private SDCanvas(BufferedGraphicsContext bCtx, BufferedGraphics bGfx, Image image, Graphics graphicsForImage)
        {
            mGraphicsForImage = graphicsForImage;
            mCtx = bCtx;
            mBufferedGfx = bGfx;
            mImage = image;
        }

        ~SDCanvas()
        {
            Dispose();
        }
        
        public void Dispose()
        {
            mBufferedGfx.Dispose();
            mGraphicsForImage.Dispose();
            mImage.Dispose();
            mCtx.Dispose();
        }

        /// <summary>
        /// Returns the image that rendered by it's graphic buffer.
        /// </summary>
        /// <returns>Result of the rendering</returns>
        public Image getImage()
        {
            mBufferedGfx.Render(mGraphicsForImage);
            return mImage;
        }

        public Graphics getGraphics()
        {
            return mBufferedGfx.Graphics;
        }

        public static SDCanvas CreateCanvas(int width = DEFAULT_IMAGE_SIZE, int height = DEFAULT_IMAGE_SIZE)
        {
            Rectangle geometry = new Rectangle(0, 0, width, height);
            Bitmap bitmap = new Bitmap(width, height);
            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.CompositingMode = CompositingMode.SourceOver; //TODO remove unused
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            graphics.Clear(Color.Empty);
            BufferedGraphicsContext bCtx = new BufferedGraphicsContext();
            BufferedGraphics bgfx = bCtx.Allocate(graphics, geometry);
            bgfx.Graphics.CompositingMode = CompositingMode.SourceOver;
            bgfx.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            bgfx.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            bgfx.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            bgfx.Graphics.Clear(Color.Empty);

            return new SDCanvas(bCtx, bgfx, bitmap, graphics);
        }
    }
}
