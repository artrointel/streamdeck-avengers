using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDGraphics;
using System.Drawing;

namespace SDGraphics
{
    /// <summary>
    /// Animates a circle spreading from center of the canvas.
    /// </summary>
    public class CircleSpreadRenderer : CanvasRendererBase, IAnimatableRenderer
    {
        // constants
        private static Color CIRCLE_COLOR = Color.FromArgb(CIRCLE_ALPHA, Color.White);
        private const int CIRCLE_ALPHA = 255 / 2 ;
        private const int CIRCLE_START_RADIUS = SDCanvas.DEFAULT_IMAGE_SIZE / 10;
        private const int ANIM_DURATION = 500;
        
        // input data
        private Color mInputColor;

        // for internal logic
        private ValueAnimator mCircleAnimator;
        private Color mAnimCircleColor;
        private Rectangle mRectCircleGeometry;

        public CircleSpreadRenderer()
        {
            mInputColor = CIRCLE_COLOR;
            initialize();
        }

        public CircleSpreadRenderer(Color color)
        {           
            mInputColor = color;
            initialize();
        }

        private void initialize()
        {
            int center = SDCanvas.DEFAULT_IMAGE_SIZE / 2;
            mRectCircleGeometry = new Rectangle(center, center, 0, 0);

            mCircleAnimator = new ValueAnimator(
                CIRCLE_START_RADIUS, SDCanvas.DEFAULT_IMAGE_SIZE * 1.42f, // maximum circle radius should be (IMAGE_SIZE * sqrt(2)) to fit the rect.
                ANIM_DURATION, ValueAnimator.INTERVAL_60_PER_SEC);

            mCircleAnimator.setAnimationListeners((value, duration) => {
                // circle grows
                mRectCircleGeometry.X = center - (int)value;
                mRectCircleGeometry.Y = center - (int)value;
                mRectCircleGeometry.Width = (int)value * 2;
                mRectCircleGeometry.Height = (int)value * 2;
                // circle disappears
                double progress = duration / ANIM_DURATION;
                mAnimCircleColor = Color.FromArgb(CIRCLE_ALPHA - (int)(progress * CIRCLE_ALPHA), mInputColor);
                invalidate();
            });
        }

        public override void onRender(Graphics graphics)
        {
            graphics.Clear(Color.Empty);
            graphics.FillEllipse(new SolidBrush(mAnimCircleColor), mRectCircleGeometry);
            base.onRender(graphics);
        }

        /// <summary>
        /// Start the animator. animation will be re-played if restart is true, 
        /// else it will keep the state of the animation.
        /// </summary>
        /// <param name="restart"></param>
        public void animate(bool restart = true)
        {
            mCircleAnimator.start(restart);
        }

        /// <summary>
        /// pause the animation.
        /// </summary>
        public void pause()
        {
            mCircleAnimator.pause();
        }

        public override void destroy()
        {
            mCircleAnimator.destroy();
            base.destroy();
        }
    }
}
