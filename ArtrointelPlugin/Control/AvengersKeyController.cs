using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using Newtonsoft.Json.Linq;
using BarRaider.SdTools;
using SDGraphics;
using ArtrointelPlugin.Control.Model;

namespace ArtrointelPlugin.Control
{
    /// <summary>
    /// Controller of the avengers key.
    /// It controls UI Animation and functions to be executed.
    /// </summary>
    public class AvengersKeyController
    {
        // SDGraphics
        RenderEngine mRenderEngine;

        // Configuration data
        ArrayList mEffectConfigurations = new ArrayList();
        ArrayList mFunctionConfigurations = new ArrayList();

        public AvengersKeyController()
        {

        }

        public void initializeRenderEngine(Action<SDCanvas> listener)
        {
            mRenderEngine = new RenderEngine();
            mRenderEngine.setRenderingUpdatedListener(listener);
            mRenderEngine.run();
        }

        public bool loadPayload(JObject payload)
        {
            int effectCount = Payload.PayloadReader.isEffectPayload(payload);
            if(effectCount > 0)
            {
                mEffectConfigurations = Payload.PayloadReader.LoadEffectDataFromPayload(payload, effectCount);
                return true;
            }
            return false;
        }

        public void updateRenderEngine()
        {
            
        }
    }
}
