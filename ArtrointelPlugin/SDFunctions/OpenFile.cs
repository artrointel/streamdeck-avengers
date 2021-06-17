using System.Diagnostics;
using BarRaider.SdTools;

namespace ArtrointelPlugin.SDFunctions
{
    class OpenFile : DelayedExecutable, IExecutable
    {
        public void execute(double delayInSecond, double intervalInSecond, double durationInSecond, 
            bool restart, string metadata)
        {
            try
            {
                if (System.IO.Directory.Exists(metadata) || System.IO.File.Exists(metadata))
                {
                    Process.Start(metadata);
                }
                else
                {
                    Logger.Instance.LogMessage(TracingLevel.WARN, metadata + " does not exists.");
                }
            } 
            catch
            {
                Logger.Instance.LogMessage(TracingLevel.DEBUG, "Couldn't open :" + metadata);
            }
        }
    }
}
