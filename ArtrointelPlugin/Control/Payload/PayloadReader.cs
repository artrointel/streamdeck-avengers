using System;
using System.Collections;
using Newtonsoft.Json.Linq;
using BarRaider.SdTools;
using ArtrointelPlugin.Control.Model;

namespace ArtrointelPlugin.Control.Payload
{
    public class PayloadReader
    {
        // constants from property inspector written in javascript.
        public const string PAYLOAD_IMAGE_UPDATE_KEY = "payload_updateImage";
        public const string PAYLOAD_IMAGE_UPDATE_FROM_FILE_KEY = "payload_updateImageFromFile";
        public const string PAYLOAD_EFFECT_KEY = "payload_updateEffects";
        public const string PAYLOAD_COMMAND_KEY = "payload_updateCommands";
        public const string PAYLOAD_OPTION_KEY = "payload_updateOptions";

        public const string META_DATA_COUNT = "meta_arrayCount";
        public const string META_FILE_PATH = "meta_filePath";

        public const string KEY_EFFECT_TRIGGER = "sEffectTrigger";
        public const string KEY_EFFECT_TYPE = "sEffectType";
        public const string KEY_EFFECT_RGB = "iEffectRGB";
        public const string KEY_EFFECT_ALPHA = "iEffectAlpha";
        public const string KEY_EFFECT_DELAY = "iEffectDelay";
        public const string KEY_EFFECT_DURATION = "iEffectDuration";
        public const string KEY_EFFECT_METADATA = "iEffectMetadata";

        public const string KEY_COMMAND_TRIGGER = "sCommandTrigger";
        public const string KEY_COMMAND_TYPE = "sCommandType";
        public const string KEY_COMMAND_DELAY = "iCommandDelay";
        public const string KEY_COMMAND_INTERVAL = "iCommandInterval"; // in millisecond
        public const string KEY_COMMAND_DURATION = "iCommandDuration";
        public const string KEY_COMMAND_METADATA = "iCommandMetadata"; // handled by the type

        public const string KEY_OPTION_CONDITION = "sCondition";
        public const string KEY_OPTION_BEHAVIOR = "sBehavior";
        public const string KEY_OPTION_METADATA = "iOptionMetadata"; // not used yet

        private PayloadReader()
        {

        }

        public static bool IsImageUpdatePayload(JObject payload)
        {
            return IdentifyPayload(payload, PAYLOAD_IMAGE_UPDATE_KEY);
        }

        public static bool IsImageUpdateFromFilePayload(JObject payload)
        {
            return IdentifyPayload(payload, PAYLOAD_IMAGE_UPDATE_FROM_FILE_KEY);
        }

        public static bool IsEffectPayload(JObject payload)
        {
            return IdentifyPayload(payload, PAYLOAD_EFFECT_KEY);
        }

        public static bool IsCommandPayload(JObject payload)
        {
            return IdentifyPayload(payload, PAYLOAD_COMMAND_KEY);
        }

        public static bool IsOptionPayload(JObject payload)
        {
            return IdentifyPayload(payload, PAYLOAD_OPTION_KEY);
        }

        private static bool IdentifyPayload(JObject payload, string key)
        {
            bool ret = false;
            try
            {
                ret = bool.Parse(payload.Value<string>(key));
            }
            catch { }
            return ret;
        }

        public static int GetArrayCount(JObject payload)
        {
            return payload.Value<int>(META_DATA_COUNT);
        }

        public static string GetFilePath(JObject payload)
        {
            string ret = null;
            try
            {
                ret = payload.Value<string>(META_FILE_PATH);
            }
            catch { }
            
            return ret;
        }

        public static ArrayList LoadEffectDataFromPayload(JObject payload, int count)
        {
            try
            {
                ArrayList newEffectList = new ArrayList();
                for (int i = 1; i <= count; i++)
                {
                    string trigger = payload.Value<string>(KEY_EFFECT_TRIGGER + i);
                    string type = payload.Value<string>(KEY_EFFECT_TYPE + i);
                    string hexrgb = payload.Value<string>(KEY_EFFECT_RGB + i);
                    string alpha = payload.Value<string>(KEY_EFFECT_ALPHA + i);
                    double delay = payload.Value<double>(KEY_EFFECT_DELAY + i);
                    double duration = payload.Value<double>(KEY_EFFECT_DURATION + i);
                    string metadata = payload.Value<string>(KEY_EFFECT_METADATA + i);
                    newEffectList.Add(EffectConfig.Create(
                        trigger, type, hexrgb, alpha, delay, duration, metadata));
                }
                return newEffectList;
            } 
            catch(Exception e)
            {
                Logger.Instance.LogMessage(TracingLevel.ERROR, "Input effect data is wrong: " + e.ToString());
            }
            return null;
        }

        public static ArrayList LoadCommandDataFromPayload(JObject payload, int count)
        {
            try
            {
                ArrayList newCommandList = new ArrayList();
                for (int i = 1; i <= count; i++)
                {
                    string trigger = payload.Value<string>(KEY_COMMAND_TRIGGER + i);
                    string type = payload.Value<string>(KEY_COMMAND_TYPE + i);
                    double delay = payload.Value<double>(KEY_COMMAND_DELAY + i);
                    double interval = payload.Value<double>(KEY_COMMAND_INTERVAL + i);
                    double duration = payload.Value<double>(KEY_COMMAND_DURATION + i);
                    string metadata = payload.Value<string>(KEY_COMMAND_METADATA + i);
                    newCommandList.Add(CommandConfig.Load(
                        trigger, type, delay, interval, duration, metadata));
                }
                return newCommandList;
            }
            catch (Exception e)
            {
                Logger.Instance.LogMessage(TracingLevel.ERROR, "Input command data is wrong: " + e.ToString());
            }
            return null;
        }

        public static ArrayList LoadOptionDataFromPayload(JObject payload, int count)
        {
            try
            {
                ArrayList newOptionList = new ArrayList();
                for (int i = 1; i <= count; i++)
                {
                    string condition = payload.Value<string>(KEY_OPTION_CONDITION + i);
                    string behavior = payload.Value<string>(KEY_OPTION_BEHAVIOR + i);
                    newOptionList.Add(OptionConfig.Create(
                        condition, behavior, ""));
                }
                return newOptionList;
            }
            catch (Exception e)
            {
                Logger.Instance.LogMessage(TracingLevel.ERROR, "Input option data is wrong: " + e.ToString());
            }
            return null;
        }
    }
}
