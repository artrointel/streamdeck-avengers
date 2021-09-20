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
        public ColorOverlayRenderer(double delayInSecond, double durationInSecond, Color color)
            : base(delayInSecond, durationInSecond)
        {
            mInputColor = color;
            initialize();
        }

        private void initialize()
        {
            mAlphaBlendStartAnimator = CreateValueAnimator(0, 1, ALPHA_DURATION);
            mAlphaBlendEndAnimator = CreateValueAnimator(1, 0, ALPHA_DURATION);

            mAlphaBlendStartAnimator.setAnimationListeners((value, duration) => {
                mAnimFlashColor = Color.FromArgb((int)(mInputColor.A * value), mInputColor);
                invalidate();
            });

            mAlphaBlendEndAnimator.setAnimationListeners((value, duration) =>
            {
                mAnimFlashColor = Color.FromArgb((int)(mInputColor.A * value), mInputColor);
                invalidate();
            });

            mOverlayTask = new DelayedTask(toMillisecond(mDurationInSecond), () => {
                mAlphaBlendEndAnimator.start();
            });

            mDelayedTask = new DelayedTask(toMillisecond(mDelayInSecond), () =>
            {
                mAlphaBlendStartAnimator.start();
            });

            setStartItems(mDelayedTask, mOverlayTask);
            setControllableItems(mDelayedTask, mAlphaBlendStartAnimator, mAlphaBlendEndAnimator, mOverlayTask);
        }

        protected override void onRenderImpl(Graphics graphics)
        {
            graphics.Clear(mAnimFlashColor);
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
