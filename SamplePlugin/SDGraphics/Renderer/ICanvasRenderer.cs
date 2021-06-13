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
        bool needToUpdate();

        void onRender(Graphics graphics);

        void destroy();
    }
}
