namespace ArtrointelPlugin.SDCommands
{
    public interface IExecutable
    {
        /// <summary>
        /// Execute commands that can be done in separated threads.
        /// </summary>
        void execute(bool restart);
    }
}
