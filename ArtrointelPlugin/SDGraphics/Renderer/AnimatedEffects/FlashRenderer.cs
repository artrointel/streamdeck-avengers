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
        private readonly double mDelayInSecond;
        private readonly int mInputDurationInMillisecond;

        // for internal logic
        private ValueAnimator mFlashStartAnimator;
        private ValueAnimator mFlashEndAnimator;
        private Color mAnimFlashColor;
        private DelayedTask mDelayedTask;

        /// <summary>
        /// Flash with the color. alpha value will be used for brightest moment.
        /// </summary>
        /// <param name="color"></param>
        public FlashRenderer(Color color, double delayInSecond, double durationInSecond)
        {
            mInputColor = color;
            mDelayInSecond = delayInSecond;
            mInputDurationInMillisecond = (int)(durationInSecond * 1000);
            initialize();
        }

        private void initialize()
        {
            int flashStartDuration = (int)(mInputDurationInMillisecond * 0.25);
            int flashEndDuration = (int)(mInputDurationInMillisecond * 0.75);
            mFlashStartAnimator = new ValueAnimator(0, 1, flashStartDuration, ValueAnimator.INTERVAL_60_PER_SEC);
            mFlashEndAnimator = new ValueAnimator(1, 0, flashEndDuration, ValueAnimator.INTERVAL_60_PER_SEC);

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

            mDelayedTask = new DelayedTask((int)(mDelayInSecond * 1000), () =>
            {
                mFlashEndAnimator.stop();
                mFlashStartAnimator.start();
            });

            setStartItems(mDelayedTask);
            setControllableItems(mDelayedTask, mFlashStartAnimator, mFlashEndAnimator);
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
            mFlashEndAnimator.destroy();
            mFlashStartAnimator.destroy();
            base.onDestroy();
        }
    }
}
