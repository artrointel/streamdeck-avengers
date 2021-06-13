using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace SDGraphics
{
    /// <summary>
    /// Flashes with input color with animated alpha value.
    /// </summary>
    class FlashRenderer : CanvasRendererBase, IAnimatableRenderer
    {
        // constants
        private static Color DEFAULT_COLOR = Color.FromArgb(200, Color.White);
        private const int FLASH_START_DURATION = 50;
        private const int FLASH_END_DURATION = FLASH_START_DURATION * 4;

        // input data
        private Color mInputColor;

        // for internal logic
        private ValueAnimator mFlashStartAnimator;
        private ValueAnimator mFlashEndAnimator;
        private Color mAnimFlashColor;

        public FlashRenderer()
        {
            mInputColor = DEFAULT_COLOR;
            initialize();
        }

        /// <summary>
        /// Flash with the color. alpha value will be used for brightest moment.
        /// </summary>
        /// <param name="color"></param>
        public FlashRenderer(Color color)
        {
            mInputColor = color;
            initialize();
        }

        private void initialize()
        {
            mFlashStartAnimator = new ValueAnimator(0, 1, FLASH_START_DURATION, ValueAnimator.INTERVAL_60_PER_SEC);
            mFlashEndAnimator = new ValueAnimator(1, 0, FLASH_END_DURATION, ValueAnimator.INTERVAL_60_PER_SEC);

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

        public void animate(bool restart = true)
        {
            mFlashEndAnimator.stop();
            mFlashStartAnimator.start(restart);
        }

        public void pause()
        {
            // do nothing. it is more natural that not pausing the flash animation.
        }

        public override void onDestroy()
        {
            mFlashEndAnimator.destroy();
            mFlashStartAnimator.destroy();
            base.onDestroy();
        }
    }
}
