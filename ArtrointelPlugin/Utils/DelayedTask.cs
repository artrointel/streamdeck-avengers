using System;
using System.Threading.Tasks;
using System.Threading;

namespace ArtrointelPlugin.Utils
{
    public class DelayedTask
    {
        CancellationTokenSource mCts;
        Task mTask;

        public DelayedTask(int delayInMillisecond, Action task)
        {
            mCts = new CancellationTokenSource();
            mTask = Task.Delay(delayInMillisecond, mCts.Token).ContinueWith(t =>
            {
                if(task != null && !mCts.Token.IsCancellationRequested)
                {
                    task();
                }
            });
        }

        /// <summary>
        /// the delayed task will be canceled and also be disposed
        /// </summary>
        public async void cancel()
        {
            lock(mCts)
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
    }
}
