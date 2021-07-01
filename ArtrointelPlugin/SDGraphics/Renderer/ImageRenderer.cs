using System.Drawing;
using System.Drawing.Imaging;

namespace ArtrointelPlugin.SDGraphics.Renderer
{
    public class ImageRenderer : CanvasRendererBase
    {
        private Image mImage;
        private bool mIsAnimatable;

        public ImageRenderer(Image image)
        {
            mImage = image;
            mIsAnimatable = ImageAnimator.CanAnimate(mImage);
            if (mIsAnimatable)
            {
                var dim = new FrameDimension(image.FrameDimensionsList[0]);
                Utils.DLogger.LogMessage("animatableImage, count:" +
                    mImage.GetFrameCount(dim) + Utils.GifImageInfo.GetImageInfo(image));
                ImageAnimator.Animate(mImage, (s, e) =>{
                    invalidate(); });
            }
        }

        public override void onRender(Graphics graphics)
        {
            graphics.Clear(Color.Empty);
            if(mIsAnimatable)
            {
                ImageAnimator.UpdateFrames(mImage);
                Utils.DLogger.LogMessage("UpdateFrames");
            }
            
            if (mImage != null)
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
