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
        /// <param name="delayInSecond"></param>
        /// <param name="intervalInSecond"></param>
        /// <param name="durationInSecond"></param>
        /// <param name="restart"></param>
        /// <param name="metadata"></param>
        void execute(double delayInSecond = 0, double intervalInSecond = 33, double durationInSecond = 0, 
            bool restart = true, string metadata = null);
    }
}
