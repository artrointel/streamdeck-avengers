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
                    Logger.Instance.LogMessage(TracingLevel.WARN, "OpenFile: " + metadata + " does not exists.");
                }
            } 
            catch
            {
                Logger.Instance.LogMessage(TracingLevel.ERROR, "Couldn't open :" + metadata);
            }
        }
    }
}
