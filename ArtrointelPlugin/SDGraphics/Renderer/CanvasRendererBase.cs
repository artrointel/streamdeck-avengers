using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Timers;

namespace SDGraphics
{
    /// <summary>
    /// Base class of the renderer. Override <see cref="onRender(Graphics)"/>
    /// </summary>
    public abstract class CanvasRendererBase : ICanvasRenderer
    {
        public SDCanvas mOffscreenCanvas { get; }
        private bool mNeedToRender = false;

        public CanvasRendererBase(
            int canvasWidth = SDCanvas.DEFAULT_IMAGE_SIZE,
            int canvasHeight = SDCanvas.DEFAULT_IMAGE_SIZE)
        {
            mOffscreenCanvas = SDCanvas.CreateCanvas(canvasWidth, canvasHeight);
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

        public virtual void onDestroy()
        {
            mOffscreenCanvas.mImage.Dispose();
        }
    }
}
