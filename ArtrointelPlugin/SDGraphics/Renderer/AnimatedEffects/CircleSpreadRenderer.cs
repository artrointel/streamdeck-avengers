using System.Drawing;
using ArtrointelPlugin.Utils;

namespace ArtrointelPlugin.SDGraphics.Renderer.AnimatedEffects
{
    /// <summary>
    /// Animates a circle spreading from center of the canvas.
    /// </summary>
    public class CircleSpreadRenderer : CanvasRendererAnimatable
    {
        // constants
        private const int CIRCLE_START_RADIUS = SDCanvas.DEFAULT_IMAGE_SIZE / 10;
        
        // input data
        private readonly Color mInputColor;

        // for internal logic
        private ValueAnimator mCircleAnimator;
        private Color mAnimCircleColor;
        private Rectangle mRectCircleGeometry;

        private DelayedTask mDelayedTask;

        public CircleSpreadRenderer(double delayInSecond, double durationInSecond, Color color)
            : base(delayInSecond, durationInSecond)
        {           
            mInputColor = color;
            initialize();
        }

        private void initialize()
        {
            int center = SDCanvas.DEFAULT_IMAGE_SIZE / 2;
            mRectCircleGeometry = new Rectangle(center, center, 0, 0);

            mCircleAnimator = CreateValueAnimator(
                CIRCLE_START_RADIUS, 
                SDCanvas.DEFAULT_IMAGE_SIZE * 1.42f, // maximum circle radius should be (IMAGE_SIZE * sqrt(2)) to fit the rect.
                toMillisecond(mDurationInSecond));

            mCircleAnimator.setAnimationListeners((value, duration) => {
                // circle grows
                mRectCircleGeometry.X = center - (int)value;
                mRectCircleGeometry.Y = center - (int)value;
                mRectCircleGeometry.Width = (int)value * 2;
                mRectCircleGeometry.Height = (int)value * 2;
                // circle disappears
                double progress = duration / toMillisecond(mDurationInSecond);
                mAnimCircleColor = Color.FromArgb(mInputColor.A - (int)(progress * mInputColor.A), mInputColor);
                invalidate();
            });

            mDelayedTask = new DelayedTask((int)(mDelayInSecond * 1000), () =>
            {
                mCircleAnimator.start();
            });

            setStartItems(mDelayedTask);
            setControllableItems(mDelayedTask, mCircleAnimator);
        }

        protected override void onRenderImpl(Graphics graphics)
        {
            graphics.Clear(Color.Empty);
            graphics.FillEllipse(new SolidBrush(mAnimCircleColor), mRectCircleGeometry);
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
