using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtrointelPlugin.Control.Model
{
    public class FunctionConfig
    {
        public enum ETrigger { OnKeyPressed, OnKeyReleased };
        public enum EType { Text, Keycode, OpenWebpage, OpenFile, ExecuteCommand, PlaySound };

        public String mTrigger;
        public String mType;
        
        public double mDelay;
        public double mInterval;
        public double mDuration;
        public String mMetadata;

        private FunctionConfig()
        {

        }

        public static FunctionConfig Load(String trigger, String type, double delay, double interval, double duration, string metadata)
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
