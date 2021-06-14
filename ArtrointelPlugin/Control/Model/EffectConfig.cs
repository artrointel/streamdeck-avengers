using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ArtrointelPlugin.Control.Model
{
    public class EffectConfig
    {
        public enum ETrigger { OnKeyPressed, OnKeyReleased };
        public enum EType { ImageBlending, Flash, CircleSpread, Pie, BorderWave };

        public String mTrigger;
        public String mType;
        public Color mColor;
        public double mDelay;
        public double mDuration;

        private EffectConfig()
        {

        }

        public static EffectConfig Load(String trigger, String type, Color argb, double delay, double duration)
        {
            EffectConfig cfg = new EffectConfig();
            cfg.mTrigger = trigger;
            cfg.mType = type;
            cfg.mColor = argb;
            cfg.mDelay = delay;
            cfg.mDuration = duration;
            return cfg;
        }

    }
}
