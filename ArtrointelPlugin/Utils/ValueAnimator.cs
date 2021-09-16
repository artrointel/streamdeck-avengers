using System;
using System.Timers;

namespace ArtrointelPlugin.Utils
{
    public class ValueAnimator : IControllable
    {
        public const double INTERVAL_30_PER_SEC = 1000 / 30.0;
        public const double INTERVAL_60_PER_SEC = 1000 / 60.0;
        public Timer mTimer;
        public double mInterval;
        public double mTotalDuration;
        public double mCurrentDuration;
        private DateTime mPrevDateTime;
        
        public float mFromValue;
        public float mToValue;

        Action<double, double> mOnValueUpdatedAction;
        Action mOnFinishedAction;

        public ValueAnimator(float from, float to, int durationInMillisecond, double animationInterval = INTERVAL_30_PER_SEC)
        {
            mFromValue = from;
            mToValue = to;

            mTotalDuration = durationInMillisecond;
            mInterval = animationInterval;

            mTimer = new Timer(mInterval);
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
            double diffDuration = (e.SignalTime - mPrevDateTime).TotalMilliseconds;
            mPrevDateTime = e.SignalTime;
            mCurrentDuration += diffDuration;
            if (mCurrentDuration > mTotalDuration)
            {   // note that the last value should be mToValue.
                mCurrentDuration = mTotalDuration;
            }
        }

        /// <summary>
        /// Set animation listener onUpdated and onFinished.
        /// This onUpdated action will be called on every animation event with (animated value, current duration).
        /// </summary>
        /// <param name="onUpdated"></param>
        /// <param name="onFinished">called when the animation finished on last animation update time</param>
        public void setAnimationListeners(Action<double, double> onUpdated, Action onFinished = null)
        {
            mOnValueUpdatedAction = onUpdated;
            mOnFinishedAction = onFinished;
        }

        /// <summary>
        /// Start the animator.
        /// </summary>
        public void start()
        {
            mCurrentDuration = 0;
            mPrevDateTime = DateTime.Now;
            mTimer.Start();
        }

        public void resume()
        {
            mTimer.Start();
        }

        public void pause()
        {
            mTimer.Stop();
        }

        public void stop()
        {
            mCurrentDuration = 0;
            mTimer.Stop();
        }

        public void destroy()
        {
            mTimer.Stop();
            mTimer.Dispose();
        }
    }
}
