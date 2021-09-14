using System;
using System.Collections;

namespace ArtrointelPlugin.SDCommands
{
    public class CommandExecutor
    {
        ArrayList mExecutables = new ArrayList();

        public CommandExecutor()
        {

        }

        public void addExecutable(IExecutable executable)
        {
            mExecutables.Add(executable);
        }

        public bool executeCommandAt(int index, double delayInSecond, double interval, double duration, String metadata)
        {
            if(mExecutables == null || mExecutables[index] == null)
            {
                return false;
            }
            ((IExecutable)mExecutables[index]).execute(true);
            return true;
        }
        
        public void destroyAll()
        {
            foreach(IExecutable e in mExecutables)
            {
                if(e is DelayedExecutable)
                {
                    ((DelayedExecutable) e).cancel();
                }
            }
        }
    }
}
