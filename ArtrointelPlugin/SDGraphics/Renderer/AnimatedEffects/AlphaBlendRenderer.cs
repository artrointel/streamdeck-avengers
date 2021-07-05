using System.Drawing;
using System.Drawing.Imaging;
using ArtrointelPlugin.Utils;

namespace ArtrointelPlugin.SDGraphics.Renderer.AnimatedEffects
{
    /// <summary>
    /// Animates alpha blending between input images.
    /// </summary>
    public class AlphaBlendRenderer : CanvasRendererBase, IAnimatableRenderer
    {
        public const int DURATION_ALPHA_BLEND_ANIMATION = 200;

        // input data
        private Image mFirstImage;
        private Image mSecondaryImage;
        private readonly double mDelayInSecond;
        private readonly int mInputDurationInMillisecond;

        // for internal logic
        private ImageAttributes mFirstImageAlpha;
        private ImageAttributes mSecondaryImageAlpha;

        private ValueAnimator mAlphaStartAnimator;
        private ValueAnimator mAlphaEndAnimator;
        private DelayedTask mBlendDurationTask;

        private DelayedTask mDelayedTask;

        public AlphaBlendRenderer(double delayInSecond, double durationInSecond)
        {
            mDelayInSecond = delayInSecond;
            mInputDurationInMillisecond = (int)(durationInSecond * 1000);
        }

        /// <summary>
        /// Set images and initializes the renderer. If secondary image is null, it uses grayscaled image of the first image.
        /// </summary>
        public void setImages(Image firstImage, Image secondaryImage = null)
        {
            mFirstImage = firstImage;
            mSecondaryImage = secondaryImage;
            if(mSecondaryImage == null)
            {
                mSecondaryImage = Utils.ImageProcessor.CreateGrayscaledImage(firstImage);
            }
            initialize();
        }

        private void initialize()
        {
            mFirstImageAlpha = new ImageAttributes();
            mSecondaryImageAlpha = new ImageAttributes();

            mAlphaStartAnimator = new ValueAnimator(0, 1, DURATION_ALPHA_BLEND_ANIMATION);
            mAlphaStartAnimator.setAnimationListeners((value, duration) =>
            {
                mFirstImageAlpha.SetColorMatrix(new ColorMatrix() { Matrix33 = 1.0f - (float)value });
                mSecondaryImageAlpha.SetColorMatrix(new ColorMatrix() { Matrix33 = (float)value });
                invalidate();
            },
            () =>
            {
                if(mBlendDurationTask != null)
                {
                    mBlendDurationTask.cancel();
                }
                mBlendDurationTask = new DelayedTask(mInputDurationInMillisecond - DURATION_ALPHA_BLEND_ANIMATION * 2, () =>
                {
                    mAlphaEndAnimator.start();
                });
            });
            
            mAlphaEndAnimator = new ValueAnimator(0, 1, DURATION_ALPHA_BLEND_ANIMATION);
            mAlphaEndAnimator.setAnimationListeners((value, duration) =>
            {
                mFirstImageAlpha.SetColorMatrix(new ColorMatrix() { Matrix33 = (float)value });
                mSecondaryImageAlpha.SetColorMatrix(new ColorMatrix() { Matrix33 = 1.0f - (float)value });
                invalidate();
            });
        }

        public override void onRender(Graphics graphics)
        {
            graphics.Clear(Color.Empty);
            if (mFirstImage != null && mSecondaryImage != null)
            {
                graphics.DrawImage(mSecondaryImage,
                       new Rectangle(0, 0, SDCanvas.DEFAULT_IMAGE_SIZE, SDCanvas.DEFAULT_IMAGE_SIZE),
                       0, 0, SDCanvas.DEFAULT_IMAGE_SIZE, SDCanvas.DEFAULT_IMAGE_SIZE,
                       GraphicsUnit.Pixel, mSecondaryImageAlpha);

                graphics.DrawImage(mFirstImage,
                       new Rectangle(0, 0, SDCanvas.DEFAULT_IMAGE_SIZE, SDCanvas.DEFAULT_IMAGE_SIZE),
                       0, 0, SDCanvas.DEFAULT_IMAGE_SIZE, SDCanvas.DEFAULT_IMAGE_SIZE,
                       GraphicsUnit.Pixel, mFirstImageAlpha);
            }

            base.onRender(graphics);
        }

        public void animate(bool restart = true)
        {
            if (mDelayInSecond > 0)
            {
                if (mDelayedTask != null)
                {
                    mDelayedTask.cancel();
                }
                mDelayedTask = new DelayedTask((int)(mDelayInSecond * 1000), () =>
                {
                    mAlphaStartAnimator.start(restart);
                });
            }
            else
            {
                mAlphaStartAnimator.start(restart);
            }
        }

        public void pause()
        {
            // TODO no way to stop the delayed task yet.
            mAlphaStartAnimator.stop();
            mAlphaEndAnimator.start();
        }

        public override void onDestroy()
        {
            mFirstImage.Dispose();
            mSecondaryImage.Dispose(); 
            mFirstImageAlpha.Dispose();
            mSecondaryImageAlpha.Dispose();
            if(mDelayedTask != null)
            {
                mDelayedTask.cancel();
            }
            if (mBlendDurationTask != null)
            {
                mBlendDurationTask.cancel();
            }
            mAlphaStartAnimator.destroy();
            mAlphaStartAnimator.destroy();
            
            base.onDestroy();
        }
    }
}
