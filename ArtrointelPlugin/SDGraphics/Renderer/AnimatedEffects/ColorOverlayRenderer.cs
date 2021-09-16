using System.Drawing;
using ArtrointelPlugin.Utils;

namespace ArtrointelPlugin.SDGraphics.Renderer.AnimatedEffects
{
    /// <summary>
    /// Flashes with input color with animated alpha value.
    /// </summary>
    class ColorOverlayRenderer : CanvasRendererAnimatable
    {
        // input data
        private readonly Color mInputColor;
        private readonly double mDelayInSecond;
        private readonly int mInputDurationInMillisecond;

        // for internal logic
        private const int ALPHA_DURATION = 100;
        private ValueAnimator mAlphaBlendStartAnimator;
        private ValueAnimator mAlphaBlendEndAnimator;
        private Color mAnimFlashColor;
        private DelayedTask mOverlayTask;

        private DelayedTask mDelayedTask;

        /// <summary>
        /// Flash with the color. alpha value will be used for brightest moment.
        /// </summary>
        /// <param name="color"></param>
        public ColorOverlayRenderer(Color color, double delayInSecond, double durationInSecond)
        {
            mInputColor = color;
            mDelayInSecond = delayInSecond;
            mInputDurationInMillisecond = (int)(durationInSecond * 1000);
            initialize();
        }

        private void initialize()
        {
            mAlphaBlendStartAnimator = new ValueAnimator(0, 1, ALPHA_DURATION, ValueAnimator.INTERVAL_60_PER_SEC);
            mAlphaBlendEndAnimator = new ValueAnimator(1, 0, ALPHA_DURATION, ValueAnimator.INTERVAL_60_PER_SEC);

            mAlphaBlendStartAnimator.setAnimationListeners((value, duration) => {
                mAnimFlashColor = Color.FromArgb((int)(mInputColor.A * value), mInputColor);
                invalidate();
            });

            mAlphaBlendEndAnimator.setAnimationListeners((value, duration) =>
            {
                mAnimFlashColor = Color.FromArgb((int)(mInputColor.A * value), mInputColor);
                invalidate();
            });

            mOverlayTask = new DelayedTask(mInputDurationInMillisecond, () => {
                mAlphaBlendEndAnimator.start();
            });

            mDelayedTask = new DelayedTask((int)(mDelayInSecond * 1000), () =>
            {
                mAlphaBlendStartAnimator.start();
            });

            setStartItems(mDelayedTask, mOverlayTask);
            setControllableItems(mDelayedTask, mAlphaBlendStartAnimator, mAlphaBlendEndAnimator, mOverlayTask);
        }

        public override void onRender(Graphics graphics)
        {
            graphics.Clear(mAnimFlashColor);
            base.onRender(graphics);
        }

        public override void onDestroy()
        {
            if (mDelayedTask != null)
            {
                mDelayedTask.cancel();
            }
            if(mOverlayTask != null)
            {
                mOverlayTask.cancel();
            }
            mAlphaBlendEndAnimator.destroy();
            mAlphaBlendStartAnimator.destroy();
            base.onDestroy();
        }
    }
}
