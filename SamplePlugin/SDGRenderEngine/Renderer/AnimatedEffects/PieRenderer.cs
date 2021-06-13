using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace SDG
{
    public class PieRenderer : SDCanvasRendererBase, IAnimatableRenderer
    {
        const int InitialAlpha = 255 / 2;
        static Color DefaultColor = Color.FromArgb(InitialAlpha, Color.Blue); // make black
        const int PieRadius = (int)((SDCanvas.DEFAULT_IMAGE_SIZE / 2) * 1.42);
        
        private Rectangle mPieRect;
        private Brush mPieBrush;
        private double mEndAngle;

        private ValueAnimator mAngleAnimator;

        private double mInputCoolTime; // in second
        private Color mInputColor;

        private double mAnimatorInterval = ValueAnimator.INTERVAL_60_PER_SEC;

        public PieRenderer(int second)
        {
            mInputCoolTime = second;
            mInputColor = DefaultColor;
            initialize();
        }

        public PieRenderer(int second, Color color)
        {
            mInputCoolTime = second;
            mInputColor = color;
            initialize();
        }

        private void initialize()
        {
            int animationDuration = (int)(mInputCoolTime * 1000.0);
            mAngleAnimator = new ValueAnimator(0, 360, animationDuration, mAnimatorInterval);
            mAngleAnimator.setAnimationListeners((angle, duration) =>
            {
                mEndAngle = angle;
                mNeedToRender = true;
            });

            int center = SDCanvas.DEFAULT_IMAGE_SIZE / 2;
            mPieRect = new Rectangle(center - PieRadius, center - PieRadius, PieRadius * 2, PieRadius * 2);
            mPieBrush = new SolidBrush(mInputColor);
        }
        public override void onRender(Graphics graphics)
        {
            graphics.Clear(Color.Empty);
            graphics.FillPie(mPieBrush, mPieRect, -90, (float)mEndAngle);
            base.onRender(graphics);
        }

        public void animate()
        {
            mAngleAnimator.start();
        }
    }
}
