using System;
using System.Timers;

namespace ArtrointelPlugin.Utils
{
    // TimerControl is Timer wrapper for thread safety
    public class TimerControl : IControllable
    {
        private Timer mTimer;
        private Object mLockObj = new object();

        private readonly double mInterval;
        private readonly ElapsedEventHandler mHnd;

        public TimerControl(double interval, ElapsedEventHandler eHnd)
        {
            mInterval = interval;
            mHnd = eHnd;
        }

        public void start()
        {
            stop();
            lock (mLockObj)
            {
                mTimer = new Timer(mInterval);
                mTimer.Elapsed += mHnd;
                mTimer.AutoReset = true;
                mTimer.Start();
            }
        }

        public void pause()
        {
            lock(mLockObj)
            {
                mTimer?.Stop();
            }
        }

        public void resume()
        {
            lock (mLockObj)
            {
                mTimer?.Start();
            }
        }

        public void stop()
        {
            lock(mLockObj)
            {
                mTimer?.Stop();
                mTimer?.Dispose();
                mTimer = null;
            }
        }
    }
}
