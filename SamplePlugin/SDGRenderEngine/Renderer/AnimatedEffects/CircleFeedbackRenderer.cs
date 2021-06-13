using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDG;
using System.Drawing;

namespace SDG
{
    public class CircleFeedbackRenderer : SDCanvasRendererBase, IAnimatableRenderer
    {
        static Color DefaultColor = Color.FromArgb(InitialAlpha, Color.White);

        const int InitialAlpha = 255 / 2;
        const int InitialRadius = SDCanvas.DEFAULT_IMAGE_SIZE / 10;
        const int AnimationDuration = 500;

        Color mInputColor;

        ValueAnimator mValueAnimator;
        Color mAnimCircleColor;
        Rectangle mAnimRectForCircle;

        public CircleFeedbackRenderer()
        {
            mInputColor = DefaultColor;
            initialize();
        }

        public CircleFeedbackRenderer(Color color)
        {           
            mInputColor = color;
            initialize();
        }

        private void initialize()
        {
            int center = SDCanvas.DEFAULT_IMAGE_SIZE / 2;
            mAnimRectForCircle = new Rectangle(center, center, 0, 0);

            mValueAnimator = new ValueAnimator(
                InitialRadius, SDCanvas.DEFAULT_IMAGE_SIZE * 1.42f, // maximum circle size should be (radius * sqrt(2)) to fit the rect.
                AnimationDuration, ValueAnimator.INTERVAL_60_PER_SEC);

            mValueAnimator.setAnimationListeners((value, duration) => {
                // circle grows
                mAnimRectForCircle.X = center - (int)value;
                mAnimRectForCircle.Y = center - (int)value;
                mAnimRectForCircle.Width = (int)value * 2;
                mAnimRectForCircle.Height = (int)value * 2;
                // circle disappears
                double progress = duration / AnimationDuration;
                mAnimCircleColor = Color.FromArgb(InitialAlpha - (int)(progress * InitialAlpha), mInputColor);
                mNeedToRender = true;
            },
            null);
        }

        public override void onRender(Graphics graphics)
        {
            graphics.Clear(Color.Empty);
            graphics.FillEllipse(new SolidBrush(mAnimCircleColor), mAnimRectForCircle);
            base.onRender(graphics);
        }

        public void animate()
        {
            mValueAnimator.start();
        }
    }
}
