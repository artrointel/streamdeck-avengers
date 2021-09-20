using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtrointelPlugin.Utils
{
    /// <summary>
    /// Delayable process model.
    /// Every process that can be delayed and also can be done in a duration with an interval.
    /// </summary>
    public class DelayedProcessModel
    {
        public const double INTERVAL_30FPS = 0.033;
        protected readonly double mDelayInSecond;
        protected readonly double mDurationInSecond;
        protected readonly double mIntervalInSecond;

        public DelayedProcessModel(
            double delayInSecond = 0, 
            double durationInSecond = 0, 
            double intervalInSecond = INTERVAL_30FPS)
        {
            mDelayInSecond = delayInSecond;
            mDurationInSecond = durationInSecond;
            mIntervalInSecond = intervalInSecond;
        }

        /// <summary>
        /// Get total duration(delayed time + duration time) in second.
        /// </summary>
        public double getTotalDuration()
        {
            return mDelayInSecond + mDurationInSecond;
        }

        public int toMillisecond(double second)
        {
            return (int)(second * 1000);
        }
    }
}
