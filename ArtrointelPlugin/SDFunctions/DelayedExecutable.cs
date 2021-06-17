using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArtrointelPlugin.Utils;

namespace ArtrointelPlugin.SDFunctions
{
    public class DelayedExecutable
    {
        protected DelayedTask mDelayedTask;

        public void cancel()
        {
            if(mDelayedTask != null)
            {
                mDelayedTask.cancel();
            }
        }
    }
}
