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
using ArtrointelPlugin.SDFunctions;

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

        // SDFunctions
        FunctionExecutor mFunctionExecutor;

        // Configuration data
        ArrayList mEffectConfigurations = new ArrayList();
        ArrayList mFunctionConfigurations = new ArrayList();

        public AvengersKeyController(Action<SDCanvas> rendererUpdatedListener)
        {
            mRendererUpdatedListener = rendererUpdatedListener;
            mRenderEngine = CreateRenderEngine(mRendererUpdatedListener);
            mFunctionExecutor = CreateFunctionExecutor();
            mRenderEngine.run();
            putBaseImageRenderer();
        }

        private static RenderEngine CreateRenderEngine(Action<SDCanvas> rendererUpdatedListener)
        {
            var renderEngine = new RenderEngine();
            renderEngine.setRenderingUpdatedListener(rendererUpdatedListener);
            return renderEngine;
        }

        private static FunctionExecutor CreateFunctionExecutor()
        {
            return new FunctionExecutor();
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
                updateRenderEngineWithConfig();
                return;
            }

            // Handles Function payload
            int functionCount = PayloadReader.isFunctionPayload(payload);
            if (functionCount > 0)
            {
                mFunctionConfigurations = PayloadReader.LoadFunctionDataFromPayload(payload, functionCount);
                Logger.Instance.LogMessage(TracingLevel.DEBUG, "detected function payload.");
                updateFunctionExecutorWithConfig();
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
                        updateRenderEngineWithConfig();
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
                
        public void updateRenderEngineWithConfig()
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

        public void updateFunctionExecutorWithConfig()
        {
            mFunctionExecutor.destroyAll();
            mFunctionExecutor = new FunctionExecutor();
            foreach (FunctionConfig cfg in mFunctionConfigurations)
            {
                var executable = FunctionFactory.CreateExecutable(cfg);
                mFunctionExecutor.addExecutable(executable);
            }
        }

        public void actionOnKeyPressed()
        {
            // Execute functions
            for (int i = 0; i < mFunctionConfigurations.Count; i++)
            {
                FunctionConfig cfg = (FunctionConfig) mFunctionConfigurations[i];
                if (cfg.mTrigger.Equals(FunctionConfig.ETrigger.OnKeyPressed.ToString()))
                {
                    mFunctionExecutor.executeFunctionAt(i, cfg.mDelay, cfg.mInterval, cfg.mDuration, cfg.mMetadata);
                }
            }

            // Animate renderers
            for (int i = 0; i < mEffectConfigurations.Count; i++)
            {
                EffectConfig cfg = (EffectConfig) mEffectConfigurations[i];
                if (cfg.mTrigger.Equals(EffectConfig.ETrigger.OnKeyPressed.ToString()))
                {
                    // Workaround: baseImage renderer is at 0
                    mRenderEngine.animateRendererAt(i + 1, cfg.mDelay);
                }
            }
        }

        public void actionOnKeyReleased()
        {

        }
    }
}
