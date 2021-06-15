using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;


namespace ArtrointelPlugin.Control
{
    internal class FileIOManager
    {
        // For resource directories
        public const String RES_DIR = "Res";
        public const String IMAGE_DIR = "Images";
        public const String BASE_IMAGE_NAME = "baseImage.png";

        public static String getBaseImagePath()
        {
            var Sep = System.IO.Path.DirectorySeparatorChar;
            return RES_DIR + Sep + IMAGE_DIR + Sep + BASE_IMAGE_NAME;
        }

        /// <summary>
        /// Save as base image to png file. it will be resized.
        /// </summary>
        /// <param name="imgToResize"></param>
        /// <returns></returns>
        // https://www.c-sharpcorner.com/UploadFile/ishbandhu2009/resize-an-image-in-C-Sharp/
        public static bool saveAsBaseImage(Image imgToResize)
        {
            Size size = new Size(144, 144);

            // Get the image current width  
            int sourceWidth = imgToResize.Width;
            // Get the image current height  
            int sourceHeight = imgToResize.Height;
            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;
            // Calulate  width with new desired size  
            nPercentW = ((float)size.Width / (float)sourceWidth);
            // Calculate height with new desired size  
            nPercentH = ((float)size.Height / (float)sourceHeight);
            if (nPercentH < nPercentW)
                nPercent = nPercentH;
            else
                nPercent = nPercentW;
            // New Width
            int destWidth = (int)(sourceWidth * nPercent);

            // New Height
            int destHeight = (int)(sourceHeight * nPercent);
            Bitmap bmp = new Bitmap(destWidth, destHeight);
            Graphics graphics = Graphics.FromImage(bmp);
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            // Draw image with new width and height  
            graphics.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
            graphics.Dispose();

            bmp.Save(getBaseImagePath(), ImageFormat.Png);
            return true;
        }

        public static Image loadBaseImage()
        {
            return Image.FromFile(getBaseImagePath());
        }
        
    }
}
