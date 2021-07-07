using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtrointelPlugin.SDFunctions
{
    public interface IExecutable
    {
        /// <summary>
        /// execute function can be called by a separated thread.
        /// </summary>
        void execute(bool restart);
    }
}
