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
        private String mFilePath;

        public ImageRenderer(String filePath)
        {
            setImageFile(filePath);
        }

        public void setImageFile(String filePath)
        {
            mFilePath = filePath;
            invalidate();
        }

        public override void onRender(Graphics graphics)
        {
            // TODO any exception? ->  fallback image
            Image img = Image.FromFile(mFilePath);
            graphics.DrawImage(img, 0, 0, SDCanvas.DEFAULT_IMAGE_SIZE, SDCanvas.DEFAULT_IMAGE_SIZE);
            img.Dispose();

            base.onRender(graphics);
        }
    }
}
