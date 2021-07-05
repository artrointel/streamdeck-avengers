using System.Drawing;

namespace ArtrointelPlugin.SDGraphics.Renderer
{
    public class ImageRenderer : CanvasRendererBase
    {
        private Image mImage;

        public ImageRenderer(Image image)
        {
            mImage = image;
        }

        public override void onRender(Graphics graphics)
        {
            if(mImage != null)
            {
                graphics.DrawImage(mImage, 0, 0, SDCanvas.DEFAULT_IMAGE_SIZE, SDCanvas.DEFAULT_IMAGE_SIZE);
            }

            base.onRender(graphics);
        }

        public override void onDestroy()
        {
            mImage.Dispose();
            base.onDestroy();
        }
    }
}
