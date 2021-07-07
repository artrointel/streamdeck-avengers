using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtrointelPlugin.SDFunctions
{
    internal abstract class FunctionBase : IExecutable
    {
        protected readonly double mDelayInSecond;
        protected readonly double mDurationInSecond;
        protected readonly double mIntervalInSecond;
        protected readonly string mMetadata;

        internal FunctionBase(string metadata = null, 
            double delayInSecond = 0, double durationInSecond = 0, double intervalInSecond = 33)
        {
            mDelayInSecond = delayInSecond;
            mDurationInSecond = durationInSecond;
            mIntervalInSecond = intervalInSecond;
            mMetadata = metadata;
        }

        public abstract void execute(bool restart);
    }
}
