using System.Diagnostics;
using BarRaider.SdTools;

namespace ArtrointelPlugin.SDCommands
{
    internal class OpenFile : CommandBase
    {
        internal OpenFile(string metadata)
            : base(metadata)
        {

        }
        public override void execute(bool restart)
        {
            try
            {
                if (System.IO.Directory.Exists(mMetadata) || System.IO.File.Exists(mMetadata))
                {
                    Process.Start(mMetadata);
                }
                else
                {
                    Logger.Instance.LogMessage(TracingLevel.WARN, "OpenFile: " + mMetadata + " does not exists.");
                }
            } 
            catch
            {
                Logger.Instance.LogMessage(TracingLevel.ERROR, "Couldn't open :" + mMetadata);
            }
        }
    }
}
