using System.Drawing;
using ArtrointelPlugin.Utils;

namespace ArtrointelPlugin.SDGraphics.Renderer.AnimatedEffects
{
    /// <summary>
    /// Animates a circle spreading from center of the canvas.
    /// </summary>
    public class CircleSpreadRenderer : CanvasRendererBase, IAnimatableRenderer
    {
        // constants
        private const int CIRCLE_START_RADIUS = SDCanvas.DEFAULT_IMAGE_SIZE / 10;
        
        // input data
        private Color mInputColor;
        private int mInputDurationInMillisecond;

        // for internal logic
        private ValueAnimator mCircleAnimator;
        private Color mAnimCircleColor;
        private Rectangle mRectCircleGeometry;
        private DelayedTask mDelayedTask;

        public CircleSpreadRenderer(Color color, double durationInSecond)
        {           
            mInputColor = color;
            mInputDurationInMillisecond = (int)(durationInSecond * 1000);
            initialize();
        }

        private void initialize()
        {
            int center = SDCanvas.DEFAULT_IMAGE_SIZE / 2;
            mRectCircleGeometry = new Rectangle(center, center, 0, 0);

            mCircleAnimator = new ValueAnimator(
                CIRCLE_START_RADIUS, SDCanvas.DEFAULT_IMAGE_SIZE * 1.42f, // maximum circle radius should be (IMAGE_SIZE * sqrt(2)) to fit the rect.
                mInputDurationInMillisecond, ValueAnimator.INTERVAL_60_PER_SEC);

            mCircleAnimator.setAnimationListeners((value, duration) => {
                // circle grows
                mRectCircleGeometry.X = center - (int)value;
                mRectCircleGeometry.Y = center - (int)value;
                mRectCircleGeometry.Width = (int)value * 2;
                mRectCircleGeometry.Height = (int)value * 2;
                // circle disappears
                double progress = duration / mInputDurationInMillisecond;
                mAnimCircleColor = Color.FromArgb(mInputColor.A - (int)(progress * mInputColor.A), mInputColor);
                invalidate();
            });
        }

        public override void onRender(Graphics graphics)
        {
            graphics.Clear(Color.Empty);
            graphics.FillEllipse(new SolidBrush(mAnimCircleColor), mRectCircleGeometry);
            base.onRender(graphics);
        }

        public void animate(double delayInSecond, bool restart)
        {
            if (delayInSecond > 0)
            {
                if(mDelayedTask != null)
                {
                    mDelayedTask.cancel();
                }
                mDelayedTask = new DelayedTask((int)(delayInSecond * 1000), () =>
                {
                    mCircleAnimator.start(restart);
                });
            }
            else
            {
                mCircleAnimator.start(restart);
            }
        }


        public void pause()
        {
            mCircleAnimator.pause();
        }

        public override void onDestroy()
        {
            if(mDelayedTask != null)
            {
                mDelayedTask.cancel();
            }
            mCircleAnimator.destroy();
            base.onDestroy();
        }
    }
}
