using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDGraphics
{
    interface IAnimatableRenderer
    {
        void animate(bool restart);
        void pause();
    }
}
