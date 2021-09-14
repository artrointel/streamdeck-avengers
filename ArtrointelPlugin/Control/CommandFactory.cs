using System;
using ArtrointelPlugin.Control.Model;
using ArtrointelPlugin.SDCommands;

namespace ArtrointelPlugin.Control
{
    class CommandFactory
    {
        public static bool IsSupported(CommandConfig cfg)
        {
            if(cfg.mType == null)
                return false;

            foreach (CommandConfig.EType t in Enum.GetValues(typeof(CommandConfig.EType)))
            {
                if (cfg.mType.Equals(t.ToString())) return true;
            }
            return false;
        }

        internal static IExecutable CreateExecutable(CommandConfig cfg)
        {
            if (cfg.mType == null)
                return null;

            IExecutable e = null;

            if (cfg.mType.Equals(CommandConfig.EType.ExecuteCommand.ToString()))
            {
                e = new ExecuteCommandPrompt(cfg.mMetadata);
            }
            else if(cfg.mType.Equals(CommandConfig.EType.OpenFile.ToString()))
            {
                e = new OpenFile(cfg.mMetadata);
            }
            else if (cfg.mType.Equals(CommandConfig.EType.OpenWebpage.ToString()))
            {
                e = new OpenWebpage(cfg.mMetadata);
            }
            else if (cfg.mType.Equals(CommandConfig.EType.PlaySound.ToString()))
            {
                e = new PlaySound(cfg.mMetadata);
            }
            else if (cfg.mType.Equals(CommandConfig.EType.Text.ToString()))
            {
                e = new SendKeyEvent(false, cfg.mMetadata, cfg.mDelay, cfg.mDuration, cfg.mInterval);
            }
            else if (cfg.mType.Equals(CommandConfig.EType.Keycode.ToString()))
            {
                e = new SendKeyEvent(true, cfg.mMetadata, cfg.mDelay, cfg.mDuration, cfg.mInterval);
            }
            else if (cfg.mType.Equals(CommandConfig.EType.VolumeControl.ToString()))
            {
                e = new VolumeControl(cfg.mMetadata);
            }
            return e;
        }
    }
}
