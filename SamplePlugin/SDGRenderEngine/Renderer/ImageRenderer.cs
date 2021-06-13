using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;


namespace SDG
{
    public class ImageRenderer : SDCanvasRendererBase
    {
        private String mFilePath;

        public ImageRenderer(String filePath)
        {
            setImageFile(filePath);
        }

        public void setImageFile(String filePath)
        {
            mNeedToRender = true;
            mFilePath = filePath;
        }

        public override void onRender(Graphics graphics)
        {
            // TODO any exception?
            Image img = Image.FromFile(mFilePath);
            graphics.DrawImage(img, 0, 0, SDCanvas.DEFAULT_IMAGE_SIZE, SDCanvas.DEFAULT_IMAGE_SIZE);
            img.Dispose();

            base.onRender(graphics);
        }
    }
}
