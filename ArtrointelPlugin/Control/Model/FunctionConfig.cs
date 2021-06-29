namespace ArtrointelPlugin.Control.Model
{
    public class FunctionConfig
    {
        public enum ETrigger { OnKeyPressed, OnKeyReleased };
        public enum EType { Text, Keycode, OpenWebpage, OpenFile, ExecuteCommand, PlaySound };

        public string mTrigger;
        public string mType;
        
        public double mDelay;
        public double mInterval;
        public double mDuration;
        public string mMetadata;

        private FunctionConfig() {}

        public static FunctionConfig Load(string trigger, string type, double delay, double interval, double duration, string metadata)
        {
            FunctionConfig cfg = new FunctionConfig();
            cfg.mTrigger = trigger;
            cfg.mType = type;
            cfg.mDelay = delay;
            cfg.mInterval = interval;
            cfg.mDuration = duration;
            cfg.mMetadata = metadata;
            return cfg;
        }

    }
}
