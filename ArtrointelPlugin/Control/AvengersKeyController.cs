using System;
using System.IO;
using System.Collections;
using System.Drawing;
using Newtonsoft.Json.Linq;
using BarRaider.SdTools;
using SDGraphics;
using ArtrointelPlugin.Control.Model;
using ArtrointelPlugin.Control.Payload;
using ArtrointelPlugin.Utils;

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
            putBaseImageRenderer();
        }

        private static RenderEngine CreateRenderEngine(Action<SDCanvas> rendererUpdatedListener)
        {
            var renderEngine = new RenderEngine();
            renderEngine.setRenderingUpdatedListener(rendererUpdatedListener);
            return renderEngine;
        }

        private void putBaseImageRenderer()
        {
            try
            {
                var imageRenderer = new ImageRenderer(FileIOManager.loadBaseImage());
                imageRenderer.invalidate();
                mRenderEngine.addRenderer(imageRenderer);
            }
            catch (Exception e)
            {
                Logger.Instance.LogMessage(TracingLevel.WARN, "Couldn't load baseImage:" + e.Message);
            }
        }

        public void handlePayload(JObject payload)
        {
            // Handles Effect payload
            int effectCount = PayloadReader.isEffectPayload(payload);
            if(effectCount > 0)
            {
                // TODO base image config
                mEffectConfigurations = PayloadReader.LoadEffectDataFromPayload(payload, effectCount);
                Logger.Instance.LogMessage(TracingLevel.DEBUG, "detected effect payload.");
                updateRenderEngine();
                return;
            }

            // Handles Image update payload
            // TODO handle input file extension
            String base64Image = PayloadReader.isImageUpdatePayload(payload);
            if(base64Image != null)
            {
                Logger.Instance.LogMessage(TracingLevel.DEBUG, "detected update image payload.");
                Image img = Tools.Base64StringToImage(base64Image);
                if (img != null)
                {
                    if(FileIOManager.saveAsBaseImage(img))
                    {
                        updateRenderEngine();
                        return;
                    }
                }
                else
                {
                    Logger.Instance.LogMessage(TracingLevel.DEBUG, "could'nt read input image");
                }

            }
            // TODO add function read
            Logger.Instance.LogMessage(TracingLevel.WARN, "wrong payload is sent.");
        }

        
        public void updateRenderEngine()
        {
            mRenderEngine.destroyAll();
            mRenderEngine = CreateRenderEngine(mRendererUpdatedListener);
            putBaseImageRenderer();

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

                    // Workaround: baseImage renderer is at 0
                    if (renderers[i+1] is IAnimatableRenderer)
                    {
                        ((IAnimatableRenderer)renderers[i+1]).animate(cfg.mDelay);
                    }
                }
                
            }
        }
    }
}
