using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace SDG
{
    public class ValueAnimator
    {
        public const int TIMER_TICK_30TPS = 1000 / 30;
        public const int INTERVAL_60_PER_SEC = 1000 / 60;
        public Timer mTimer;
        public double mTick;
        public double mTotalDuration;
        public double mCurrentDuration;

        public float mFromValue = 0.0f;
        public float mToValue = 0.0f;

        Action<double, double> mOnValueUpdatedAction;
        Action mOnFinishedAction;

        public ValueAnimator(float from, float to, int durationInMillisecond, double timerTick = TIMER_TICK_30TPS)
        {
            mFromValue = from;
            mToValue = to;

            mTotalDuration = durationInMillisecond;
            mTick = timerTick;

            mTimer = new Timer(mTick);
            mTimer.Elapsed += onTimedEvent;
            mTimer.AutoReset = true;
        }
        protected void onTimedEvent(object sender, ElapsedEventArgs e)
        {
            // linear interpolation
            var currentValue = mFromValue + (mToValue - mFromValue) * (mCurrentDuration / mTotalDuration);
           
            if (mOnValueUpdatedAction != null)
            {
                mOnValueUpdatedAction(currentValue, mCurrentDuration);
            }
            if(mCurrentDuration == mTotalDuration)
            {
                if (mOnFinishedAction != null)
                {
                    mOnFinishedAction();
                }
                this.stop();
            }
            
            // updates current duration
            mCurrentDuration += mTick;
            if(mCurrentDuration > mTotalDuration)
            {   // note that the last value should be mToValue.
                mCurrentDuration = mTotalDuration;
            }
        }

        /// <summary>
        /// Set animation listener onUpdated and onFinished.
        /// This onUpdated action will be called on every animation event with (animated value, current duration).
        /// </summary>
        public void setAnimationListeners(Action<double, double> onUpdated, Action onFinished = null)
        {
            mOnValueUpdatedAction = onUpdated;
            mOnFinishedAction = onFinished;
        }

        public void start()
        {
            mCurrentDuration = 0;
            mTimer.Start();
        }

        public void stop()
        {
            mCurrentDuration = 0;
            mTimer.Stop();
        }

        public bool isRunning()
        {
            return mTimer.Enabled;
        }
    }
}
