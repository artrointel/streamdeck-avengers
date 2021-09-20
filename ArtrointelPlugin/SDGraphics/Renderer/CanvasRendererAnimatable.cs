using ArtrointelPlugin.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace ArtrointelPlugin.SDGraphics.Renderer
{
    public abstract class CanvasRendererAnimatable : CanvasRendererBase, IControllable
    {
        #region Initializers
        public CanvasRendererAnimatable(
               int canvasWidth = SDCanvas.DEFAULT_IMAGE_SIZE,
               int canvasHeight = SDCanvas.DEFAULT_IMAGE_SIZE)
               : this(SDCanvas.CreateInfo.DEFAULT, canvasWidth, canvasHeight)
        { }

        public CanvasRendererAnimatable(SDCanvas.CreateInfo info,
            int canvasWidth = SDCanvas.DEFAULT_IMAGE_SIZE,
            int canvasHeight = SDCanvas.DEFAULT_IMAGE_SIZE)
            : base(info, canvasWidth, canvasHeight)
        { }

        public CanvasRendererAnimatable(
            double delayInSecond, double durationInSecond,
            int canvasWidth = SDCanvas.DEFAULT_IMAGE_SIZE,
            int canvasHeight = SDCanvas.DEFAULT_IMAGE_SIZE)
            : this(SDCanvas.CreateInfo.DEFAULT, delayInSecond, durationInSecond, canvasWidth, canvasHeight)
        { }

        public CanvasRendererAnimatable(
            SDCanvas.CreateInfo info,
            double delayInSecond, double durationInSecond,
            int canvasWidth = SDCanvas.DEFAULT_IMAGE_SIZE,
            int canvasHeight = SDCanvas.DEFAULT_IMAGE_SIZE)
            : base(info, delayInSecond, durationInSecond, canvasWidth, canvasHeight)
        { }
        #endregion

        private List<IControllable> mStartItems = new List<IControllable>();
        private List<IControllable> mControllableItems = new List<IControllable>();
        private bool mForceToClear = false;

        /// <summary>
        /// Set Controllable items for animation start.
        /// </summary>
        /// <param name="controllables"></param>
        internal void setStartItems(params IControllable[] controllables)
        {
            mStartItems.AddRange(controllables);
        }

        /// <summary>
        /// Set Controllable items so that it can be managed internally.
        /// </summary>
        /// <param name="controllables"></param>
        internal void setControllableItems(params IControllable[] controllables)
        {
            mControllableItems.AddRange(controllables);
        }

        public sealed override void onRender(Graphics graphics)
        {
            if (mForceToClear)
            {
                graphics.Clear(Color.Empty);
            }
            else
            {
                onRenderImpl(graphics);
            }
            
            base.onRender(graphics);
        }

        /// <summary>
        /// Implement animated rendering logics.
        /// </summary>
        /// <param name="graphics"></param>
        protected abstract void onRenderImpl(Graphics graphics);

        /// <summary>
        /// Called when the animation is stopped. 
        /// Initialize variables to make canvas clean.
        /// </summary>
        protected virtual void onAnimationStopped()
        {
            mForceToClear = true;
            invalidate();
        }

        /// <summary>
        /// Create and return a ValueAnimator for the rendering.
        /// returned animator should be only used for rendering.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="durationInMillisecond"></param>
        /// <returns><see cref="ValueAnimator"/></returns>
        protected ValueAnimator CreateValueAnimator(float from, float to, 
            int durationInMillisecond, double interval = ValueAnimator.INTERVAL_60_PER_SEC)
        {
            ValueAnimator ret = new ValueAnimator(from, to, durationInMillisecond, interval);
            ret.setStartListener(()=> { mForceToClear = false; });
            return ret;
        }
        
        #region Implements IControllable Interface
        public void start()
        {
            stop();
            
            foreach (IControllable item in mStartItems)
            {
                item?.start();
            }
        }

        public void pause()
        {
            foreach (IControllable item in mControllableItems)
            {
                item?.pause();
            }
        }

        public void resume()
        {
            foreach (IControllable item in mControllableItems)
            {
                item?.resume();
            }
        }
        
        public void stop()
        {
            foreach (IControllable item in mControllableItems)
            {
                item?.stop();
            }
            onAnimationStopped();
        }
        #endregion
    }
}
