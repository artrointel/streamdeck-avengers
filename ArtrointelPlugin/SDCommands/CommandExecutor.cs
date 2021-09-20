using System.Collections.Generic;

namespace ArtrointelPlugin.SDCommands
{
    public class CommandExecutor
    {
        private List<CommandBase> mExecutables = new List<CommandBase>();

        public CommandExecutor()
        {

        }

        public void addCommand(CommandBase executable)
        {
            mExecutables.Add(executable);
        }

        public bool executeCommandAt(int index)
        {
            if(mExecutables == null || mExecutables[index] == null)
            {
                return false;
            }
            ((IExecutable)mExecutables[index]).execute(true); // TODO
            return true;
        }

        public double getTotalDurationAt(int index)
        {
            return mExecutables[index].getTotalDuration();
        }
        
        public void destroyAll()
        {
            foreach(IExecutable e in mExecutables)
            {
                if(e is DelayedCommandBase)
                {
                    ((DelayedCommandBase) e).cancel();
                }
            }
        }
    }
}
