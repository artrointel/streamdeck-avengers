using ArtrointelPlugin.Control.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArtrointelPlugin.SDFunctions;

namespace ArtrointelPlugin.Control
{
    class FunctionFactory
    {
        internal static IExecutable CreateExecutable(FunctionConfig cfg)
        {
            IExecutable e = null;
            if (cfg.mType.Equals(FunctionConfig.EType.ExecuteCommand.ToString()))
            {
                e = new ExecuteCommand();
            }
            else if(cfg.mType.Equals(FunctionConfig.EType.OpenFile.ToString()))
            {
                e = new OpenFile();
            }
            else if (cfg.mType.Equals(FunctionConfig.EType.OpenWebpage.ToString()))
            {
                e = new OpenWebpage();
            }
            else if (cfg.mType.Equals(FunctionConfig.EType.PlaySound.ToString()))
            {
                e = new PlaySound();
            }
            else if (cfg.mType.Equals(FunctionConfig.EType.Text.ToString()))
            {
                e = new SendKeyEvent();
            }
            else if (cfg.mType.Equals(FunctionConfig.EType.Keycode.ToString()))
            {
                e = new SendKeyEvent(); // todo
            }
            return e;
        }
    }
}
