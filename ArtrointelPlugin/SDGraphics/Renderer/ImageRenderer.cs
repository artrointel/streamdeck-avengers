using System.Drawing;

namespace ArtrointelPlugin.SDGraphics.Renderer
{
    public class ImageRenderer : CanvasRendererBase
    {
        public Rectangle DEFAULT_IMAGE_CANVAS = new Rectangle(0, 0, 
            SDCanvas.DEFAULT_IMAGE_SIZE, SDCanvas.DEFAULT_IMAGE_SIZE);
        private Image mImage;
        private Rectangle mGeometry;

        public ImageRenderer(Image image)
        {
            mImage = image;
            mGeometry = DEFAULT_IMAGE_CANVAS;
        }

        public ImageRenderer(Image image, Rectangle geometry)
        {
            mGeometry = geometry;
        }

        protected override void onRender(Graphics graphics)
        {
            if(mImage != null)
            {
                graphics.Clear(Color.Empty);
                graphics.DrawImage(mImage, mGeometry);
            }
        }

        public override void onDestroy()
        {
            mImage.Dispose();
            base.onDestroy();
        }
    }
}
