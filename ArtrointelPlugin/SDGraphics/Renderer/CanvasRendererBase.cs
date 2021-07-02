using System.Drawing;
using System.Threading.Tasks;

namespace ArtrointelPlugin.SDGraphics.Renderer
{
    /// <summary>
    /// Base class of the renderer. Override <see cref="onRender(Graphics)"/>
    /// </summary>
    public abstract class CanvasRendererBase
    {
        protected SDCanvas mOffscreenCanvas;
        private bool mNeedToRender = false;
        private bool mVisible = true;

        public CanvasRendererBase(
            int canvasWidth = SDCanvas.DEFAULT_IMAGE_SIZE,
            int canvasHeight = SDCanvas.DEFAULT_IMAGE_SIZE)
        {
            mOffscreenCanvas = SDCanvas.CreateCanvas(canvasWidth, canvasHeight);
        }

        /// <summary>
        /// Set this renderer need to be rendered. <see cref="renderAsync()"/>
        /// </summary>
        public void invalidate()
        {
            mNeedToRender = true;
        }

        public bool needToUpdate()
        {
            return mNeedToRender;
        }

        /// <summary>
        /// Renders on internal offscreen canvas. <see cref="mNeedToRender"/> flag is set false after this rendering call.
        /// </summary>
        public async void renderAsync()
        {
            await Task.Run(() =>
            {
                onRender(mOffscreenCanvas.getGraphics());
                // TODO lock and called on updated callback?
            });
            mNeedToRender = false;
        }

        /// <summary>
        /// Override this method for graphic rendering.
        /// </summary>
        /// <param name="graphics"></param>
        protected abstract void onRender(Graphics graphics);

        /// <summary>
        /// Gets the rendering result as Image from offscreen canvas. <see cref="SDCanvas.getImage()"/>
        /// </summary>
        /// <returns></returns>
        public Image getImage()
        {
            return mOffscreenCanvas.getImage();
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
