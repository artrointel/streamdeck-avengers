using System;
using ArtrointelPlugin.Control.Model;
using ArtrointelPlugin.SDFunctions;

namespace ArtrointelPlugin.Control
{
    class FunctionFactory
    {
        public static bool IsSupported(FunctionConfig cfg)
        {
            if(cfg.mType == null)
                return false;

            foreach (FunctionConfig.EType t in Enum.GetValues(typeof(FunctionConfig.EType)))
            {
                if (cfg.mType.Equals(t.ToString())) return true;
            }
            return false;
        }

        internal static IExecutable CreateExecutable(FunctionConfig cfg)
        {
            if (cfg.mType == null)
                return null;

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
                e = new SendKeyEvent(true);
            }
            return e;
        }
    }
}
