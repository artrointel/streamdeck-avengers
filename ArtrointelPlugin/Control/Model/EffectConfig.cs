using System;
using System.Drawing;

namespace ArtrointelPlugin.Control.Model
{
    public class EffectConfig
    {
        public enum ETrigger { OnKeyPressed, OnKeyReleased };
        public enum EType { Flash, CircleSpread, Pie, BorderWave, ColorOverlay, BlendGrayscaleFiltering };

        public string mTrigger;
        public string mType;
        public string mHexRgb;
        public string mAlpha;
        public double mDelay;
        public double mDuration;
        public string mMetadata;

        private EffectConfig() {}

        public Color getColor()
        {
            return Color.FromArgb(Int32.Parse(mAlpha), ColorTranslator.FromHtml(mHexRgb));
        }

        public static EffectConfig Create(string trigger, string type, string hexRgb, string alpha, double delay, double duration, string metadata)
        {
            EffectConfig cfg = new EffectConfig();
            cfg.mTrigger = trigger;
            cfg.mType = type;
            cfg.mHexRgb = hexRgb;
            cfg.mAlpha = alpha;
            cfg.mDelay = delay;
            cfg.mDuration = duration;
            cfg.mMetadata = metadata;
            return cfg;
        }

        public override string ToString() {
            return $"Trigger:{mTrigger}, Type:{mType}, HexRgb:{mHexRgb}, Alpha:{mAlpha}, Delay:{mDelay}, Duration:{mDuration}, Meta:{mMetadata}";
        }
    }
}
