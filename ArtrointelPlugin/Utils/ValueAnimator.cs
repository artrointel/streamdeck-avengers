using System;
using System.Timers;

namespace ArtrointelPlugin.Utils
{
    public class ValueAnimator : IControllable
    {
        // constants
        public const double INTERVAL_30_PER_SEC = 1000 / 30.0;
        public const double INTERVAL_60_PER_SEC = 1000 / 60.0;

        private TimerControl mTimer;
        private readonly double mInterval;
        private readonly double mDuration;
        private double mCurrentDuration;
        private DateTime mPrevDateTime;
        
        private readonly float mFromValue;
        private readonly float mToValue;

        // on animation update listeners
        private Action<double, double> mOnValueUpdatedAction;
        private Action mOnFinishedAction;

        // IControllable listeners
        private Action mOnStarted;

        public ValueAnimator(float from, float to, int durationInMillisecond, double animationInterval = INTERVAL_30_PER_SEC)
        {
            mFromValue = from;
            mToValue = to;

            mDuration = durationInMillisecond;
            mInterval = animationInterval;

            mTimer = new TimerControl(mInterval, onTimedEvent);
        }

        private void onTimedEvent(object sender, ElapsedEventArgs e)
        {
            // linear interpolation
            var currentValue = mFromValue + (mToValue - mFromValue) * (mCurrentDuration / mDuration);
           
            mOnValueUpdatedAction?.Invoke(currentValue, mCurrentDuration);

            if(mCurrentDuration == mDuration)
            {
                mOnFinishedAction?.Invoke();
                mCurrentDuration = 0;
                mTimer.stop();
            }

            // updates current duration
            double diffDuration = (e.SignalTime - mPrevDateTime).TotalMilliseconds;
            mPrevDateTime = e.SignalTime;
            mCurrentDuration += diffDuration;
            if (mCurrentDuration > mDuration)
            {   // note that the last value should be mToValue.
                mCurrentDuration = mDuration;
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
                
        public void setStartListener(Action onStarted)
        {
            mOnStarted = onStarted;
        }

        public void start()
        {
            mCurrentDuration = 0;
            mPrevDateTime = DateTime.Now;
            mTimer.start();
            mOnStarted?.Invoke();
        }

        public void resume()
        {
            mTimer.resume();
        }

        public void pause()
        {
            mTimer.pause();
        }

        public void stop()
        {
            mCurrentDuration = 0;
            mTimer.stop();
        }

        public void destroy()
        {
            mTimer.stop();
        }
    }
}
