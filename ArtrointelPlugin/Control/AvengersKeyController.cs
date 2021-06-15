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
using ArtrointelPlugin.Control.Payload;

namespace ArtrointelPlugin.Control
{
    /// <summary>
    /// Controller of the avengers key.
    /// It controls UI Animation and functions to be executed.
    /// </summary>
    public class AvengersKeyController
    {
        private Action<SDCanvas> mRendererUpdatedListener;

        // SDGraphics
        RenderEngine mRenderEngine;

        // Configuration data
        ArrayList mEffectConfigurations = new ArrayList();
        ArrayList mFunctionConfigurations = new ArrayList();

        public AvengersKeyController(Action<SDCanvas> rendererUpdatedListener)
        {
            mRendererUpdatedListener = rendererUpdatedListener;
            mRenderEngine = CreateRenderEngine(mRendererUpdatedListener);
            mRenderEngine.run();
        }

        private static RenderEngine CreateRenderEngine(Action<SDCanvas> rendererUpdatedListener)
        {
            var renderEngine = new RenderEngine();
            renderEngine.setRenderingUpdatedListener(rendererUpdatedListener);
            return renderEngine;
        }

        public bool loadPayload(JObject payload)
        {
            
            int effectCount = PayloadReader.isEffectPayload(payload);
            if(effectCount > 0)
            {
                mEffectConfigurations = PayloadReader.LoadEffectDataFromPayload(payload, effectCount);
                Logger.Instance.LogMessage(TracingLevel.DEBUG, "loaded payload");
                return true;
            }
            // TODO add function read
            return false;
        }

        public void updateRenderEngine()
        {
            mRenderEngine.destroyAll();
            mRenderEngine = CreateRenderEngine(mRendererUpdatedListener);
            foreach(EffectConfig effectCfg in mEffectConfigurations)
            {
                var renderer = RendererFactory.CreateRenderer(effectCfg);
                mRenderEngine.addRenderer(renderer);
            }
            mRenderEngine.run();
            Logger.Instance.LogMessage(TracingLevel.DEBUG, "updated render engine.");
        }

        public void actionOnKeyPressed()
        {
            // Execute functions
            
            // Animate renderers
            for (int i = 0; i < mEffectConfigurations.Count; i++)
            {
                EffectConfig cfg = (EffectConfig) mEffectConfigurations[i];
                if (cfg.mTrigger.Equals(EffectConfig.ETrigger.OnKeyPressed.ToString()))
                {
                    ArrayList renderers = mRenderEngine.getRenderers();
                    if (renderers[i] is IAnimatableRenderer)
                    {
                        ((IAnimatableRenderer)renderers[i]).animate(cfg.mDelay);
                    }
                }
                
            }
        }
    }
}
