using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Timers;

namespace SDG
{
    public abstract class SDCanvasRendererBase : ICanvasRenderer
    {
        public SDCanvas mDefaultCanvas { get; }
        protected bool mNeedToRender = true;

        public SDCanvasRendererBase(
            int canvasWidth = SDCanvas.DEFAULT_IMAGE_SIZE,
            int canvasHeight = SDCanvas.DEFAULT_IMAGE_SIZE)
        {
            mDefaultCanvas = SDCanvas.CreateCanvas(canvasWidth, canvasHeight);
        }

        public bool needToUpdate()
        {
            return mNeedToRender;
        }

        public virtual void onRender(Graphics graphics)
        {
            mNeedToRender = false;
        }
    }
}
