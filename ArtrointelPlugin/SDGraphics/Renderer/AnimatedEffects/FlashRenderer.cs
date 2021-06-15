using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Drawing;
using ArtrointelPlugin.Utils;

namespace SDGraphics
{
    /// <summary>
    /// Flashes with input color with animated alpha value.
    /// </summary>
    class FlashRenderer : CanvasRendererBase, IAnimatableRenderer
    {
        // input data
        private Color mInputColor;
        private int mInputDurationInMliisecond;

        // for internal logic
        private ValueAnimator mFlashStartAnimator;
        private ValueAnimator mFlashEndAnimator;
        private Color mAnimFlashColor;
        private DelayedTask mDelayedTask;

        /// <summary>
        /// Flash with the color. alpha value will be used for brightest moment.
        /// </summary>
        /// <param name="color"></param>
        public FlashRenderer(Color color, double durationInSecond)
        {
            mInputColor = color;
            mInputDurationInMliisecond = (int)(durationInSecond * 1000);
            initialize();
        }

        private void initialize()
        {
            int flashStartDuration = (int)(mInputDurationInMliisecond * 0.25);
            int flashEndDuration = (int)(mInputDurationInMliisecond * 0.75);
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
        }

        public override void onRender(Graphics graphics)
        {
            graphics.Clear(mAnimFlashColor);
            base.onRender(graphics);
        }

        public void animate(double delayInSecond, bool restart)
        {
            if (delayInSecond > 0)
            {
                if (mDelayedTask != null)
                {
                    mDelayedTask.cancel();
                }
                mDelayedTask = new DelayedTask((int)(delayInSecond * 1000), () =>
                {
                    mFlashEndAnimator.stop();
                    mFlashStartAnimator.start(restart);
                });
            }
            else
            {
                mFlashEndAnimator.stop();
                mFlashStartAnimator.start(restart);
            }
        }

        public void pause()
        {
            // do nothing. it is more natural that not pausing the flash animation.
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
