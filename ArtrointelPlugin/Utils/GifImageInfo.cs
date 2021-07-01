using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace ArtrointelPlugin.Utils
{
    // http://web.archive.org/web/20130820015012/http://madskristensen.net/post/Examine-animated-Gife28099s-in-C.aspx
    class GifImageInfo
    {
        public struct ImageInfo
        {
            public int Width;
            public int Height;
            public bool IsAnimated;
            public bool IsLooped;
            public int AnimationLength; // In milliseconds

            public override string ToString()
            {
                return $"W:{Width} H:{Height} IsAnimated:{IsAnimated} IsLooped:{IsLooped} Length:{AnimationLength}";
            }
        }

        public static ImageInfo GetImageInfo(Image image)
        {
            ImageInfo info = new ImageInfo();
            info.Height = image.Height;
            info.Width = image.Width;
            if (image.RawFormat.Equals(ImageFormat.Gif))
            {
                if (ImageAnimator.CanAnimate(image))
                {
                    FrameDimension frameDimension = new FrameDimension(image.FrameDimensionsList[0]);
                    int frameCount = image.GetFrameCount(frameDimension);
                    int delay = 0;
                    int this_delay = 0;
                    int index = 0;
                    for (int f = 0; f < frameCount; f++)
                    {
                        this_delay = BitConverter.ToInt32(image.GetPropertyItem(20736).Value, index) * 10;
                        delay += (this_delay < 100 ? 100 : this_delay);  // Minimum delay is 100 ms
                        index += 4;
                    }

                    info.AnimationLength = delay;
                    info.IsAnimated = true;
                    info.IsLooped = BitConverter.ToInt16(image.GetPropertyItem(20737).Value, 0) != 1;
                }
            }
            
            return info;
        }
    }
}
