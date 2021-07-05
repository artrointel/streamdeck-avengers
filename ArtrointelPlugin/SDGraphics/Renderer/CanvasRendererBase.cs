using System.Drawing;

namespace ArtrointelPlugin.SDGraphics.Renderer
{
    /// <summary>
    /// Base class of the renderer. Override <see cref="onRender(Graphics)"/>
    /// </summary>
    public abstract class CanvasRendererBase : ICanvasRenderer
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
