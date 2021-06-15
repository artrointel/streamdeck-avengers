using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;


namespace SDGraphics
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
            // TODO any exception? ->  fallback image
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
