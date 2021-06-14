using System;
using System.Collections;
using System.Drawing;
using Newtonsoft.Json.Linq;
using BarRaider.SdTools;
using ArtrointelPlugin.Control.Model;

namespace ArtrointelPlugin.Control.Payload
{
    public class PayloadReader
    {
        // constants from property inspector written in javascript.
        public const String PAYLOAD_EFFECT_KEY = "effectCount";
        public const String KEY_EFFECT_TRIGGER = "sEffectTrigger";
        public const String KEY_EFFECT_TYPE = "sEffectType";
        public const String KEY_EFFECT_RGB = "iEffectRGB";
        public const String KEY_EFFECT_ALPHA = "iEffectAlpha";
        public const String KEY_EFFECT_DELAY = "iEffectDelay";
        public const String KEY_EFFECT_DURATION = "iEffectDuration";

        private PayloadReader()
        {

        }

        public static int isEffectPayload(JObject payload)
        {
            int effectCount = payload.Value<int>(PAYLOAD_EFFECT_KEY);
            return effectCount;
        }

        public static ArrayList LoadEffectDataFromPayload(JObject payload, int count)
        {
            try
            {
                ArrayList newEffectList = new ArrayList();
                for (int i = 1; i <= count; i++)
                {
                    String trigger = payload.Value<String>(KEY_EFFECT_TRIGGER + i);
                    String type = payload.Value<String>(KEY_EFFECT_TYPE + i);
                    String hexrgb = payload.Value<String>(KEY_EFFECT_RGB + i);
                    String alpha = payload.Value<String>(KEY_EFFECT_ALPHA + i);
                    double delay = payload.Value<double>(KEY_EFFECT_DELAY + i);
                    double duration = payload.Value<double>(KEY_EFFECT_DURATION + i);
                    newEffectList.Add(EffectConfig.Load(
                        trigger, type,
                        Color.FromArgb(Int32.Parse(alpha), ColorTranslator.FromHtml(hexrgb)),
                        delay, duration));
                }
                return newEffectList;
            } catch(Exception e)
            {
                Logger.Instance.LogMessage(TracingLevel.ERROR, "input effect data is wrong: " + e.ToString());
            }
            return null;
        }
    }
}
