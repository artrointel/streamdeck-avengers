using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Timers;

namespace SDGraphics
{
    public abstract class CanvasRendererBase : ICanvasRenderer
    {
        public SDCanvas mDefaultCanvas { get; }
        private bool mNeedToRender = true;

        public CanvasRendererBase(
            int canvasWidth = SDCanvas.DEFAULT_IMAGE_SIZE,
            int canvasHeight = SDCanvas.DEFAULT_IMAGE_SIZE)
        {
            mDefaultCanvas = SDCanvas.CreateCanvas(canvasWidth, canvasHeight);
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

        public virtual void destroy()
        {
            mDefaultCanvas.mImage.Dispose();
        }
    }
}
