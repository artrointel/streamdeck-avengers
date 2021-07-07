using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace ArtrointelPlugin.SDFunctions
{
    public class FunctionExecutor
    {
        ArrayList mExecutables = new ArrayList();

        public FunctionExecutor()
        {

        }

        public void addExecutable(IExecutable executable)
        {
            mExecutables.Add(executable);
        }

        public bool executeFunctionAt(int index, double delayInSecond, double interval, double duration, String metadata)
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
