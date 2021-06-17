using System;
using System.Diagnostics;
using BarRaider.SdTools;

namespace ArtrointelPlugin.SDFunctions
{
    class OpenWebpage : IExecutable
    {
        private const String HTTP = "http://";
        private const String HTTPS = "https://";
        public void execute(double delayInSecond, double intervalInSecond, double durationInSecond, 
            bool restart, string metadata)
        {
            String addr = metadata;
            if(!metadata.Contains(HTTP) && !metadata.Contains(HTTPS))
            {
                addr = HTTP + addr;
            }

            try
            {
                Uri uri;
                bool result = Uri.TryCreate(addr, UriKind.Absolute, out uri) && (uri.Scheme == Uri.UriSchemeHttp);
                Process.Start(addr);
            }
            catch
            {
                Logger.Instance.LogMessage(TracingLevel.DEBUG, "Couldn't open page : " + metadata);
            }
            
        }
    }
}
