using System.Drawing;

namespace ArtrointelPlugin.SDGraphics.Renderer
{
    public class ImageRenderer : CanvasRendererBase
    {
        private Image mImage;

        public ImageRenderer(Image image) : base(BufferedCanvas.DefaultCreateInfo)
        {
            mImage = image;
        }

        protected override void onRender(Graphics graphics)
        {
            if(mImage != null)
            {
                graphics.DrawImage(mImage, 0, 0, BufferedCanvas.DEFAULT_IMAGE_SIZE, BufferedCanvas.DEFAULT_IMAGE_SIZE);
            }
        }

        public override void onDestroy()
        {
            mImage.Dispose();
            base.onDestroy();
        }
    }
}
