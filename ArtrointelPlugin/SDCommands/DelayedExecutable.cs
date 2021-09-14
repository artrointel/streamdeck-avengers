using ArtrointelPlugin.Utils;

namespace ArtrointelPlugin.SDCommands
{
    internal abstract class DelayedExecutable : CommandBase
    {
        protected DelayedTask mDelayedTask;

        internal DelayedExecutable(string metadata, double delayInSecond, double durationInSecond, double intervalInSecond)
            : base(metadata, delayInSecond, durationInSecond, intervalInSecond) { }

        public void cancel()
        {
            if(mDelayedTask != null)
            {
                mDelayedTask.cancel();
            }
        }
    }
}
