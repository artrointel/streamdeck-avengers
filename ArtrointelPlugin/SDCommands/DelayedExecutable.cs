using ArtrointelPlugin.Utils;

namespace ArtrointelPlugin.SDCommands
{
    internal abstract class DelayedCommandBase : CommandBase
    {
        protected DelayedTask mDelayedTask;

        internal DelayedCommandBase(string metadata, double delayInSecond, double durationInSecond, double intervalInSecond)
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
