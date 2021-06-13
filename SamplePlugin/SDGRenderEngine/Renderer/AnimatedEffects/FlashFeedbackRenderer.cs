using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace SDG
{
    class FlashFeedbackRenderer : SDCanvasRendererBase, IAnimatableRenderer
    {
        static Color DefaultColor = Color.White;

        const int FlashAlpha = 200;

        private const int FlashStartDuration = 50;
        private const int FlashEndDuration = 200;

        private Color mInputColor;

        private ValueAnimator mFlashStartAnimator;
        private ValueAnimator mFlashEndAnimator;

        private Color mAnimColor;

        public FlashFeedbackRenderer()
        {
            mInputColor = DefaultColor;
            initialize();
        }

        public FlashFeedbackRenderer(Color color)
        {
            mInputColor = color;
            initialize();
        }

        private void initialize()
        {
            mFlashStartAnimator = new ValueAnimator(0, 1, FlashStartDuration, ValueAnimator.INTERVAL_60_PER_SEC);
            mFlashEndAnimator = new ValueAnimator(1, 0, FlashEndDuration, ValueAnimator.INTERVAL_60_PER_SEC);

            mFlashStartAnimator.setAnimationListeners((value, duration) => {
                mAnimColor = Color.FromArgb((int)(FlashAlpha * value), mInputColor);
                mNeedToRender = true;
            },
            () =>
            {
                mFlashEndAnimator.start();
            });

            mFlashEndAnimator.setAnimationListeners((value, duration) =>
            {
                mAnimColor = Color.FromArgb((int)(FlashAlpha * value), mInputColor);
                mNeedToRender = true;
            }, null);
        }

        public override void onRender(Graphics graphics)
        {
            graphics.Clear(mAnimColor);
            base.onRender(graphics);
        }

        public void animate()
        {
            mFlashEndAnimator.stop();
            mFlashStartAnimator.start();
        }
    }
}
