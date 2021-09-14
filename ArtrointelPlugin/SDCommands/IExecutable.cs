namespace ArtrointelPlugin.SDCommands
{
    public interface IExecutable
    {
        /// <summary>
        /// execute command can be called by a separated thread.
        /// </summary>
        void execute(bool restart);
    }
}
