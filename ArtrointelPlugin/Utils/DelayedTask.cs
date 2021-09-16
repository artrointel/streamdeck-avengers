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
        public async void cancel()
        {
            if (mTask == null || mTask.IsCompleted) return;

            lock (mCts)
            {
                if (mCts.IsCancellationRequested)
                {
                    return;
                }
                mCts.Cancel();
            }

            try
            {
                await mTask.ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                // this exception is raised by design by the cancellation
            }
            catch (Exception)
            {
                // an error has occurred in the asynchronous work before cancellation was requested
            }

            mCts.Dispose();
        }

        private void startDelayedTask(int ms)
        {
            mCts = new CancellationTokenSource();
            mTask = Task.Delay(ms, mCts.Token).ContinueWith(t =>
            {
                if (mActionOnTask != null && !mCts.Token.IsCancellationRequested)
                {
                    mActionOnTask();
                }
            });
            mLastStartedTimeMs = DateTime.Now.Millisecond;
        }

        public void start()
        {
            mRemainedDelayMs = mDelayMs;
            cancel();
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
