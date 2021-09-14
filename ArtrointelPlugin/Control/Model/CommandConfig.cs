namespace ArtrointelPlugin.Control.Model
{
    public class CommandConfig
    {
        public enum ETrigger { OnKeyPressed, OnKeyReleased };
        public enum EType { Text, Keycode, OpenWebpage, OpenFile, ExecuteCommand, PlaySound, VolumeControl };

        public string mTrigger;
        public string mType;

        public double mDelay;
        public double mInterval;
        public double mDuration;
        public string mMetadata;

        private CommandConfig() {}

        public static CommandConfig Load(string trigger, string type, double delay, double interval, double duration, string metadata)
        {
            CommandConfig cfg = new CommandConfig();
            cfg.mTrigger = trigger;
            cfg.mType = type;
            cfg.mDelay = delay;
            cfg.mInterval = interval;
            cfg.mDuration = duration;
            cfg.mMetadata = metadata;
            return cfg;
        }
        public override string ToString()
        {
            return $"Trigger:{mTrigger}, Type:{mType}, Delay:{mDelay}, Alpha:{mInterval}, Duration:{mDuration}, Meta:{mMetadata}";
        }
    }
}
