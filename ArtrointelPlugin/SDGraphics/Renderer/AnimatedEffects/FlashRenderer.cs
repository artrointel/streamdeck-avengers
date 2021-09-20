using System.Drawing;
using ArtrointelPlugin.Utils;

namespace ArtrointelPlugin.SDGraphics.Renderer.AnimatedEffects
{
    /// <summary>
    /// Flashes with input color with animated alpha value.
    /// </summary>
    class FlashRenderer : CanvasRendererAnimatable
    {
        // input data
        private readonly Color mInputColor;

        // for internal logic
        private ValueAnimator mFlashStartAnimator;
        private ValueAnimator mFlashEndAnimator;
        private Color mAnimFlashColor;
        private DelayedTask mDelayedTask;

        /// <summary>
        /// Flash with the color. alpha value will be used for brightest moment.
        /// </summary>
        /// <param name="color"></param>
        public FlashRenderer(double delayInSecond, double durationInSecond, Color color)
            : base(delayInSecond, durationInSecond)
        {
            mInputColor = color;
            initialize();
        }

        private void initialize()
        {
            int flashStartDuration = (int)(toMillisecond(mDurationInSecond) * 0.25);
            int flashEndDuration = (int)(toMillisecond(mDurationInSecond) * 0.75);
            mFlashStartAnimator = CreateValueAnimator(0, 1, flashStartDuration);
            mFlashEndAnimator = CreateValueAnimator(1, 0, flashEndDuration);

            mFlashStartAnimator.setAnimationListeners((value, duration) => {
                mAnimFlashColor = Color.FromArgb((int)(mInputColor.A * value), mInputColor);
                invalidate();
            },
            () =>
            {
                mFlashEndAnimator.start();
            });

            mFlashEndAnimator.setAnimationListeners((value, duration) =>
            {
                mAnimFlashColor = Color.FromArgb((int)(mInputColor.A * value), mInputColor);
                invalidate();
            });

            mDelayedTask = new DelayedTask(toMillisecond(mDelayInSecond), () =>
            {
                mFlashEndAnimator.stop();
                mFlashStartAnimator.start();
            });

            setStartItems(mDelayedTask);
            setControllableItems(mDelayedTask, mFlashStartAnimator, mFlashEndAnimator);
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
            mFlashEndAnimator.destroy();
            mFlashStartAnimator.destroy();
            base.onDestroy();
        }
    }
}
