using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BarRaider.SdTools;

namespace ArtrointelPlugin.Utils
{
    class DLogger
    {
        public static void LogMessage(TracingLevel level, string msg)
        {
#if DEBUG
            Logger.Instance.LogMessage(level, msg);
#endif
        }
    }
}
