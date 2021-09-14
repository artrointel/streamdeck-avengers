using System.Diagnostics;

namespace ArtrointelPlugin.SDCommands
{
    /// <summary>
    /// Executes a command
    /// </summary>
    internal class ExecuteCommandPrompt : CommandBase
    {
        internal ExecuteCommandPrompt(string metadata)
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
