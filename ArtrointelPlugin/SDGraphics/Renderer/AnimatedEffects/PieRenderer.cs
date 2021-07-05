using System;
using System.Drawing;
using ArtrointelPlugin.Utils;

namespace ArtrointelPlugin.SDGraphics.Renderer.AnimatedEffects
{
    public class PieRenderer : CanvasRendererBase, IAnimatableRenderer
    {
        // constants
        private const int PIE_RADIUS = (int)((SDCanvas.DEFAULT_IMAGE_SIZE / 2) * 1.42); // pie tightly covers the canvas

        // input data
        private readonly double mDelayInSecond;
        private readonly double mInputDurationInSecond; // animation in second
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

        public PieRenderer(Color color, double delayInSecond, double durationInSecond, bool grow = false, bool clockwise = false)
        {
            mDelayInSecond = delayInSecond;
            mInputDurationInSecond = durationInSecond;
            mInputColor = color;
            mGrow = grow;
            mClockwise = clockwise;
            initialize();
        }

        private void initialize()
        {
            int animationDuration = (int)(mInputDurationInSecond * 1000.0);
            // for better performance, animation interval can be loosen.
            double loosenAnimationInterval = 500.0 * mInputDurationInSecond / 720.0;
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

        public void animate(bool restart)
        {
            if (mDelayInSecond > 0)
            {
                if (mDelayedTask != null)
                {
                    mDelayedTask.cancel();
                }
                mDelayedTask = new DelayedTask((int)(mDelayInSecond * 1000), () =>
                {
                    mAngleAnimator.start(restart);
                });
            }
            else
            {
                mAngleAnimator.start(restart);
            }
        }

        public void pause()
        {
            mAngleAnimator.pause();
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
