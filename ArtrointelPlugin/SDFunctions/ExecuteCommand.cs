using System;
using System.Diagnostics;

namespace ArtrointelPlugin.SDFunctions
{
    /// <summary>
    /// Executes a command
    /// </summary>
    internal class ExecuteCommand : FunctionBase
    {
        internal ExecuteCommand(string metadata)
            : base(metadata)
        {
            
        }

        public override void execute(bool restart)
        {
            executeCommand(mMetadata, true);
        }

        private void executeCommand(string command, bool hide = false)
        {
            var p = new Process(); 
            var procStartInfo = new ProcessStartInfo("cmd", "/c " + command);

            if (hide)
            {
                procStartInfo.CreateNoWindow = true;
                procStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            }
            p.StartInfo = procStartInfo;
            p.Start();
        }
    }
}
