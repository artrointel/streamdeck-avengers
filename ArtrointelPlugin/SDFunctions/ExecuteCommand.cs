using System;
using System.Diagnostics;

namespace ArtrointelPlugin.SDFunctions
{
    /// <summary>
    /// Execute command as background task
    /// </summary>
    class ExecuteCommand : IExecutable
    {
        public void execute(double delayInSecond, double intervalInSecond, double durationInSecond, 
            bool restart, string metadata)
        {
            executeCommand(metadata, true);
        }

        public void executeCommand(String command, bool hide = false)
        {
            var p = new Process();
            if (hide)
            {
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            }
            p.StartInfo.FileName = "CMD.exe";
            p.StartInfo.Arguments = command;
            p.Start();
        }
    }
}
