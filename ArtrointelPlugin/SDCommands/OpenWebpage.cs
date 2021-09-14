using System;
using System.Diagnostics;
using BarRaider.SdTools;

namespace ArtrointelPlugin.SDCommands
{
    internal class OpenWebpage : CommandBase
    {
        private const string HTTP = "http://";
        private const string HTTPS = "https://";

        private string mAddr;
        internal OpenWebpage(string metadata)
            : base(metadata)
        {
            mAddr = mMetadata;
            if (!mMetadata.Contains(HTTP) && !mMetadata.Contains(HTTPS))
            {
                mAddr = HTTP + mAddr;
            }
        }

        public override void execute(bool restart)
        {
            try
            {
                Uri uri;
                bool result = Uri.TryCreate(mAddr, UriKind.Absolute, out uri) && (uri.Scheme == Uri.UriSchemeHttp);
                Process.Start(mAddr);
            }
            catch
            {
                Logger.Instance.LogMessage(TracingLevel.ERROR, "Couldn't open page : " + mMetadata);
            }
            
        }
    }
}
