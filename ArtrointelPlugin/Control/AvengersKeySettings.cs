using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BarRaider.SdTools;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ArtrointelPlugin.Control.Model;
using System.Collections;

namespace ArtrointelPlugin.Control
{
    public class AvengersKeySettings
    {
        public string Base64ImageString { get; set; }

        public ArrayList FunctionConfigurations { get; set; } // Arraylist of FunctionConfig

        public ArrayList EffectConfigurations { get; set; } // Arraylist of EffectConfig

        private AvengersKeySettings() { }

        public static AvengersKeySettings CreateDefaultSettings()
        {
            AvengersKeySettings instance = new AvengersKeySettings
            {
                Base64ImageString = FileIOManager.LoadFallbackBase64Image(),
                FunctionConfigurations = new ArrayList(),
                EffectConfigurations = new ArrayList()
            };
            return instance;
        }

        /// <summary>
        /// Load settings from settings object.
        /// it falls back to default settings if loading is failed.
        /// </summary>
        /// <param name="jSettings"></param>
        /// <returns></returns>
        public static AvengersKeySettings LoadFrom(JObject jSettings)
        {
            AvengersKeySettings ret = CreateDefaultSettings();
            try
            {
                if(jSettings != null && jSettings.Count != 0)
                {
                    AvengersKeySettings s = jSettings.ToObject<AvengersKeySettings>();

                    ret.Base64ImageString = s.Base64ImageString;

                    if (s.FunctionConfigurations != null)
                    {
                        foreach (JObject cfg in s.FunctionConfigurations)
                        {
                            ret.FunctionConfigurations.Add(cfg.ToObject<FunctionConfig>());
                        }
                    }

                    if (s.EffectConfigurations != null)
                    {
                        foreach (JObject cfg in s.EffectConfigurations)
                        {
                            ret.EffectConfigurations.Add(cfg.ToObject<EffectConfig>());
                        }
                    }
                }
            } catch (Exception e)
            {
                Logger.Instance.LogMessage(TracingLevel.ERROR, "Cannot read settings." + e.Message);
            }
            
            return ret;
        }

    }
}
