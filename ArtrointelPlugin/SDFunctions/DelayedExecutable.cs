using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArtrointelPlugin.Utils;

namespace ArtrointelPlugin.SDFunctions
{
    internal abstract class DelayedExecutable : FunctionBase
    {
        protected DelayedTask mDelayedTask;

        internal DelayedExecutable(string metadata, double delayInSecond, double durationInSecond, double intervalInSecond)
            : base(metadata, delayInSecond, durationInSecond, intervalInSecond) { }

        public void cancel()
        {
            if(mDelayedTask != null)
            {
                mDelayedTask.cancel();
            }
        }
    }
}
