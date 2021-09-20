using System.Drawing;
using ArtrointelPlugin.Utils;

namespace ArtrointelPlugin.SDGraphics.Renderer
{
    /// <summary>
    /// Base class of the renderer. Override <see cref="onRender(Graphics)"/>
    /// </summary>
    public abstract class CanvasRendererBase : DelayedProcessModel, ICanvasRenderer
    {
        public SDCanvas mOffscreenCanvas { get; }
        private bool mNeedToRender = false;
        private bool mVisible = true;

        public CanvasRendererBase(
            int canvasWidth = SDCanvas.DEFAULT_IMAGE_SIZE,
            int canvasHeight = SDCanvas.DEFAULT_IMAGE_SIZE)
            : this(SDCanvas.CreateInfo.DEFAULT, canvasWidth, canvasHeight)
        {}

        public CanvasRendererBase(SDCanvas.CreateInfo info,
            int canvasWidth = SDCanvas.DEFAULT_IMAGE_SIZE,
            int canvasHeight = SDCanvas.DEFAULT_IMAGE_SIZE)
            : base()
        {
            mOffscreenCanvas = SDCanvas.CreateCanvas(info, canvasWidth, canvasHeight);
        }

        public CanvasRendererBase(
            double delayInSecond, double durationInSecond, double intervalInSecond = INTERVAL_30FPS,
            int canvasWidth = SDCanvas.DEFAULT_IMAGE_SIZE,
            int canvasHeight = SDCanvas.DEFAULT_IMAGE_SIZE)
            : this(SDCanvas.CreateInfo.DEFAULT,
                delayInSecond, durationInSecond, intervalInSecond,
                canvasWidth, canvasHeight)
        {}

        public CanvasRendererBase(
            SDCanvas.CreateInfo info,
            double delayInSecond, double durationInSecond, double intervalInSecond = INTERVAL_30FPS,
            int canvasWidth = SDCanvas.DEFAULT_IMAGE_SIZE,
            int canvasHeight = SDCanvas.DEFAULT_IMAGE_SIZE)
            : base(delayInSecond, durationInSecond, intervalInSecond)
        {
            mOffscreenCanvas = SDCanvas.CreateCanvas(info, canvasWidth, canvasHeight);
        }

        public void invalidate()
        {
            mNeedToRender = true;
        }

        public bool needToUpdate()
        {
            return mNeedToRender;
        }

        public virtual void onRender(Graphics graphics)
        {
            mNeedToRender = false;
        }

        public bool isVisible()
        {
            return mVisible;
        }

        public void setVisible(bool visible)
        {
            mVisible = visible;
        }

        public virtual void onDestroy()
        {
            mOffscreenCanvas.Dispose();
        }
    }
}
