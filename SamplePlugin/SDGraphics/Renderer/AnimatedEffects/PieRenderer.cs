using System;
using System.Drawing;

namespace SDGraphics
{
    public class PieRenderer : CanvasRendererBase, IAnimatableRenderer
    {
        // constants
        private const int INITIAL_ALPHA = 255 / 2;
        private static Color DEFAULT_COLOR = Color.FromArgb(INITIAL_ALPHA, Color.White); // TODO make black
        private const int PIE_RADIUS = (int)((SDCanvas.DEFAULT_IMAGE_SIZE / 2) * 1.42); // pie tightly covers the canvas

        // input data
        private double mInputAnimationDuration; // animation in second
        private Color mInputColor; // color of the pie
        private bool mGrow; // growing or eating the pie
        private bool mClockwise; // animation direction

        // for internal logic
        private ValueAnimator mAngleAnimator;
        private double mAnimatorInterval = ValueAnimator.INTERVAL_60_PER_SEC;
        private double mAnimSweepAngle;
        private Brush mPieBrush;
        private Rectangle mRectPieGeometry;
        Action<Graphics> mRenderPieMethod;

        public PieRenderer(double second, bool grow = true, bool clockwise = true)
        {
            mInputAnimationDuration = second;
            mInputColor = DEFAULT_COLOR;
            mGrow = grow;
            mClockwise = clockwise;
            initialize();
        }

        public PieRenderer(double second, Color color, bool grow = true, bool clockwise = true)
        {
            mInputAnimationDuration = second;
            mInputColor = color;
            mGrow = grow;
            mClockwise = clockwise;
            initialize();
        }

        private void initialize()
        {
            int animationDuration = (int)(mInputAnimationDuration * 1000.0);
            // for better performance, animation interval can be loosen.
            double loosenAnimationInterval = 500.0 * mInputAnimationDuration / 720.0;
            mAnimatorInterval = 
                mAnimatorInterval < loosenAnimationInterval ? 
                loosenAnimationInterval : mAnimatorInterval;

            int center = SDCanvas.DEFAULT_IMAGE_SIZE / 2;
            mPieBrush = new SolidBrush(mInputColor);
            mRectPieGeometry = new Rectangle(center - PIE_RADIUS, center - PIE_RADIUS, PIE_RADIUS * 2, PIE_RADIUS * 2);

            mAngleAnimator = new ValueAnimator(0, 360, animationDuration, mAnimatorInterval);
            mAngleAnimator.setAnimationListeners((angle, duration) =>
            {
                mAnimSweepAngle = angle;
                invalidate();
            });

            if (mGrow)
            {
                if(mClockwise)
                {
                    mRenderPieMethod = (graphics) =>
                    {
                        graphics.Clear(Color.Empty);
                        graphics.FillPie(mPieBrush, mRectPieGeometry, -90, (float)mAnimSweepAngle);
                    };
                } else
                {
                    mRenderPieMethod = (graphics) =>
                    {
                        graphics.Clear(Color.Empty);
                        graphics.FillPie(mPieBrush, mRectPieGeometry, -90, -(float)mAnimSweepAngle);
                    };
                }
            } else
            {
                if(mClockwise)
                {
                    mRenderPieMethod = (graphics) =>
                    {
                        graphics.Clear(Color.Empty);
                        graphics.FillPie(mPieBrush, mRectPieGeometry, -90, (float)mAnimSweepAngle - 360.0f);
                    };
                } else
                {
                    mRenderPieMethod = (graphics) =>
                    {
                        graphics.Clear(Color.Empty);
                        graphics.FillPie(mPieBrush, mRectPieGeometry, -90, 360.0f - (float)mAnimSweepAngle);
                    };
                }
                
            }
        }

        public override void onRender(Graphics graphics)
        {
            mRenderPieMethod(graphics);
            base.onRender(graphics);
        }

        public void animate(bool restart = true)
        {
            mAngleAnimator.start(restart);
        }

        public void pause()
        {
            mAngleAnimator.pause();
        }

        public override void onDestroy()
        {
            mAngleAnimator.destroy();
            base.onDestroy();
        }
    }
}
