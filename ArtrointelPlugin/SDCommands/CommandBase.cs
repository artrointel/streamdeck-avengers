using ArtrointelPlugin.Utils;

namespace ArtrointelPlugin.SDCommands
{
    public abstract class CommandBase : DelayedProcessModel, IExecutable
    {
        protected readonly string mMetadata;

        public CommandBase(string metadata = null, 
            double delayInSecond = 0, double durationInSecond = 0, double intervalInSecond = INTERVAL_30FPS)
            : base(delayInSecond, durationInSecond, intervalInSecond)
        {
            mMetadata = metadata;
        }

        public abstract void execute(bool restart);
        
    }
}
