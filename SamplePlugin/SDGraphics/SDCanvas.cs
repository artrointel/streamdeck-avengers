using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace SDGraphics
{
    public class SDCanvas
    {
        public const int DEFAULT_IMAGE_SIZE = 144;

        public Graphics mGraphics;
        public Image mImage;
        public SDCanvas(Graphics graphics, Image image)
        {
            mGraphics = graphics;
            mImage = image;
        }

        public static SDCanvas CreateCanvas(int width = DEFAULT_IMAGE_SIZE, int height = DEFAULT_IMAGE_SIZE)
        {
            Bitmap bitmap = new Bitmap(width, height);
            var brush = new SolidBrush(Color.Black);
            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            graphics.FillRectangle(brush, 0, 0, width, height);

            return new SDCanvas(graphics, bitmap);
        }
    }
}
