using System;
using System.Drawing;

namespace ArtrointelPlugin.Control.Model
{
    public class OptionConfig
    {
        public enum ECondition { OnKeyPressedWhileRunning, OnKeyLongPressed, OnPaused, OnResumed };
        public enum EBehavior { Restart, PauseResume, Stop };

        public string mCondition;
        public string mBehavior;
        public string mMetadata;

        private OptionConfig() {}
        
        public static OptionConfig Create(string condition, string behavior, string metadata)
        {
            OptionConfig cfg = new OptionConfig();
            cfg.mCondition = condition;
            cfg.mBehavior = behavior;
            cfg.mMetadata = metadata;
            return cfg;
        }

        public override string ToString() {
            return $"Condition:{mCondition}, Behavior:{mBehavior}, Metadata:{mMetadata}";
        }
    }
}
