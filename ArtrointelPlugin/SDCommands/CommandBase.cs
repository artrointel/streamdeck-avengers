namespace ArtrointelPlugin.SDCommands
{
    internal abstract class CommandBase : IExecutable
    {
        protected readonly double mDelayInSecond;
        protected readonly double mDurationInSecond;
        protected readonly double mIntervalInSecond;
        protected readonly string mMetadata;

        internal CommandBase(string metadata = null, 
            double delayInSecond = 0, double durationInSecond = 0, double intervalInSecond = 33)
        {
            mDelayInSecond = delayInSecond;
            mDurationInSecond = durationInSecond;
            mIntervalInSecond = intervalInSecond;
            mMetadata = metadata;
        }

        public abstract void execute(bool restart);
    }
}
