using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using BarRaider.SdTools;

namespace ArtrointelPlugin.Utils
{
    internal class FileIOManager
    {
        // For resource directories
        public const String RES_DIR = "Res";
        public const String IMAGE_DIR = "Images";
        public const String BASE_IMAGE_NAME = "avengersIcon.png";

        public static String getFallbackImagePath()
        {
            var Sep = System.IO.Path.DirectorySeparatorChar;
            return RES_DIR + Sep + IMAGE_DIR + Sep + BASE_IMAGE_NAME;
        }

        public static string getResourceImagePath(string image)
        {
            var Sep = System.IO.Path.DirectorySeparatorChar;
            return RES_DIR + Sep + IMAGE_DIR + Sep + image;
        }

        /// <summary>
        /// Returns new image to fit the stream deck icon, 144x144
        /// </summary>
        /// <param name="imgToResize"></param>
        /// <returns></returns>
        // https://www.c-sharpcorner.com/UploadFile/ishbandhu2009/resize-an-image-in-C-Sharp/
        public static Image ResizeImage(Image imgToResize, int newWidth = 144, int newHeight = 144)
        {
            Size size = new Size(newWidth, newHeight);

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
            return bmp;
        }

        /// <summary>
        /// load and resizes the input image to 144x144.
        /// if any fails, falls back to the default image.
        /// </summary>
        /// <param name="path"></param>
        /// <returns>base64image</returns>
        public static string ProcessImageToBase64(String path)
        {
            string base64 = null;
            try
            {
                Image img = Image.FromFile(path);
                base64 = Tools.ImageToBase64(ResizeImage(img), false);
                img.Dispose();
            }
            catch (Exception e)
            {
                Logger.Instance.LogMessage(TracingLevel.ERROR, "Cannot read the image:" + path + ", " + e.Message);
            }
            return base64;
        }

        public static Image LoadBase64(string base64ImageString)
        {
            return Tools.Base64StringToImage(base64ImageString);
        }

        public static Image LoadFallbackImage()
        {
            return Image.FromFile(getFallbackImagePath());
        }

        public static string LoadFallbackBase64Image()
        {
            return Tools.ImageToBase64(LoadFallbackImage(), false);
        }

        public static Image LoadSpinner()
        {
            return Image.FromFile(getResourceImagePath("spinner.png"));
        }
    }
}
