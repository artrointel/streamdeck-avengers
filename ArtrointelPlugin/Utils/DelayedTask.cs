using System;
using System.Threading.Tasks;
using System.Threading;

namespace ArtrointelPlugin.Utils
{
    public class DelayedTask : IControllable
    {
        private Task mTask;
        private CancellationTokenSource mCts;

        private int mDelayMs;
        private Action mActionOnTask;

        private int mLastStartedTimeMs;
        private int mRemainedDelayMs;
        
        public DelayedTask(int delayInMillisecond, Action task)
        {
            if (mDelayMs < 0)
                mDelayMs = 0;

            mDelayMs = delayInMillisecond;
            mActionOnTask = task;
        }

        /// <summary>
        /// the delayed task will be canceled and also be disposed
        /// </summary>
        public void cancel()
        {
            if (mTask == null || mTask.IsCompleted || mCts == null) return;

            if (mCts.IsCancellationRequested)
            {
                return;
            }
            mCts.Cancel();
            mCts.Dispose();
            mCts = null;
            mTask = null;
        }

        private void startDelayedTask(int ms)
        {
            mCts = new CancellationTokenSource();
            CancellationToken token = mCts.Token;
            mTask = Task.Delay(ms, token).ContinueWith(t =>
            {
                if (!token.IsCancellationRequested)
                {
                    mActionOnTask?.Invoke();
                }
            });
            
            mLastStartedTimeMs = DateTime.Now.Millisecond;
        }

        public void start()
        {
            cancel();
            mRemainedDelayMs = mDelayMs;
            startDelayedTask(mRemainedDelayMs);
        }

        public void pause()
        {
            // if not started ever, nothing should be done.
            if (mRemainedDelayMs == 0) return;

            mRemainedDelayMs -= DateTime.Now.Millisecond - mLastStartedTimeMs;
            cancel();
        }

        public void resume()
        {
            if(mRemainedDelayMs > 0)
            {
                startDelayedTask(mRemainedDelayMs);
            }
        }

        public void stop()
        {
            cancel();
        }
    }
}
