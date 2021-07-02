using System.Drawing;
using System;
using System.Threading.Tasks;

namespace ArtrointelPlugin.SDGraphics.Renderer
{
    /// <summary>
    /// Base class of the renderer. Override <see cref="onRender(Graphics)"/>
    /// </summary>
    public abstract class CanvasRendererBase
    {
        protected BufferedCanvas mOffscreenCanvas { get; }
        private bool mNeedToRender = false;
        private bool mVisible = true;

        private object o = new object();

        public CanvasRendererBase(
            BufferedCanvas.CreateInfo info,
            int canvasWidth = BufferedCanvas.DEFAULT_IMAGE_SIZE,
            int canvasHeight = BufferedCanvas.DEFAULT_IMAGE_SIZE)
        {
            mOffscreenCanvas = new BufferedCanvas(info, canvasWidth, canvasHeight);
        }

        public void invalidate()
        {
            lock (o)
            {
                mNeedToRender = true;
            }
        }

        public bool needToUpdate()
        {
            lock (o)
            {
                return mNeedToRender;
            }
        }

        internal async void renderAsync(Action asyncCallback)
        {
            await Task.Run(() => {
                BufferedCanvas.Canvas c = mOffscreenCanvas.acquire();
                if(c != null)
                {
                    var gfx = c.lockCanvas();
                    onRender(gfx);
                    c.unlockCanvas();
                    lock(o)
                    {
                        mNeedToRender = false;
                    }

                    if (asyncCallback != null)
                        asyncCallback();
                }
            });
        }

        protected abstract void onRender(Graphics graphics);

        public bool isVisible()
        {
            return mVisible;
        }

        public void setVisible(bool visible)
        {
            mVisible = visible;
        }

        internal BufferedCanvas getBufferedCanvas()
        {
            return mOffscreenCanvas;
        }

        public virtual void onDestroy()
        {
            mOffscreenCanvas.dispose();
        }
    }
}
