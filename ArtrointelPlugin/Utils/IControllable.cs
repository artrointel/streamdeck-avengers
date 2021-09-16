using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtrointelPlugin.Utils
{
    public interface IControllable
    {
        void start();
        void pause();
        void resume();
        void stop();
    }
}
