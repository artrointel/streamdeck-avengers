using System;
using System.Drawing;
using ArtrointelPlugin.Utils;

namespace ArtrointelPlugin.SDGraphics.Renderer.AnimatedEffects
{
    public class PieRenderer : CanvasRendererAnimatable
    {
        // constants
        private const int PIE_RADIUS = (int)((SDCanvas.DEFAULT_IMAGE_SIZE / 2) * 1.42); // pie tightly covers the canvas

        // input data
        private readonly Color mInputColor; // color of the pie
        private readonly bool mGrow; // growing or eating the pie
        private readonly bool mClockwise; // animation direction

        // for internal logic
        private ValueAnimator mAngleAnimator;
        private double mAnimatorInterval = ValueAnimator.INTERVAL_60_PER_SEC;
        private double mAnimSweepAngle;
        private Brush mPieBrush;
        private Rectangle mRectPieGeometry;
        Action<Graphics> mRenderPieMethod;
        private DelayedTask mDelayedTask;

        public PieRenderer(double delayInSecond, double durationInSecond, Color color, bool grow = false, bool clockwise = false)
            : base(delayInSecond, durationInSecond)
        {
            mInputColor = color;
            mGrow = grow;
            mClockwise = clockwise;
            initialize();
        }

        private void initialize()
        {
            int animationDuration = toMillisecond(mDurationInSecond);
            // for better performance, animation interval can be loosen.
            double loosenAnimationInterval = 500.0 * mDurationInSecond / 720.0;
            mAnimatorInterval = 
                mAnimatorInterval < loosenAnimationInterval ? 
                loosenAnimationInterval : mAnimatorInterval;

            int center = SDCanvas.DEFAULT_IMAGE_SIZE / 2;
            mPieBrush = new SolidBrush(mInputColor);
            mRectPieGeometry = new Rectangle(center - PIE_RADIUS, center - PIE_RADIUS, PIE_RADIUS * 2, PIE_RADIUS * 2);

            mAngleAnimator = CreateValueAnimator(0, 360, animationDuration, mAnimatorInterval);
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

            mDelayedTask = new DelayedTask((int)(mDelayInSecond * 1000), () =>
            {
                mAngleAnimator.start();
            });

            setStartItems(mDelayedTask);
            setControllableItems(mDelayedTask, mAngleAnimator);
        }

        protected override void onRenderImpl(Graphics graphics)
        {
            mRenderPieMethod(graphics);
        }

        public override void onDestroy()
        {
            if (mDelayedTask != null)
            {
                mDelayedTask.cancel();
            }
            mAngleAnimator.destroy();
            base.onDestroy();
        }
    }
}
