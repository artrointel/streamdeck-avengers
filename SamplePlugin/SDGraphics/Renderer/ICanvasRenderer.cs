using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace SDGraphics
{
    interface ICanvasRenderer
    {
        /// <summary>
        /// Set 'need to update' flag to be true to render the renderer in the next render loop.
        /// </summary>
        void invalidate();

        bool needToUpdate();

        void onRender(Graphics graphics);

        void onDestroy();
    }
}
