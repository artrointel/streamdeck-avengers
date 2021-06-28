using System;
using System.Diagnostics;

namespace ArtrointelPlugin.SDFunctions
{
    /// <summary>
    /// Executes a command
    /// </summary>
    class ExecuteCommand : IExecutable
    {
        public void execute(double delayInSecond, double intervalInSecond, double durationInSecond, 
            bool restart, string metadata)
        {
            executeCommand(metadata, true);
        }

        public void executeCommand(string command, bool hide = false)
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
