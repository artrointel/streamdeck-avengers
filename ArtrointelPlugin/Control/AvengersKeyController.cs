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
    /// It controls icon rendering and functions to be executed.
    /// </summary>
    public class AvengersKeyController
    {
        #region Internal Members
        private Action<SDCanvas> mRendererUpdatedListener;

        // Render engine to render user-customized effects
        private RenderEngine mRenderEngine;

        // Executor of user-customized functions
        private FunctionExecutor mFunctionExecutor;

        // Configuration data created by user (i.e settings).
        AvengersKeySettings mSettings;
        #endregion

        public AvengersKeyController(JObject settings, Action<SDCanvas> rendererUpdatedListener)
        {
            mSettings = AvengersKeySettings.LoadFrom(settings);
            mRendererUpdatedListener = rendererUpdatedListener;

            initializeRenderEngine();
            initializeFunctionExecutor();

            mRenderEngine.run();            
        }

        #region Internal
        private void refineEffectConfigurations()
        {
            var effectCfgs = mSettings.EffectConfigurations;
            for (int i = effectCfgs.Count - 1; i != 0; i--)
            {
                if (!RendererFactory.IsSupported((EffectConfig)effectCfgs[i]))
                {
                    effectCfgs.RemoveAt(i);
                }
            }
        }

        private void refineFunctionConfigurations()
        {
            var functionCfgs = mSettings.FunctionConfigurations;
            for (int i = functionCfgs.Count - 1; i != 0; i--)
            {
                if (!FunctionFactory.IsSupported((FunctionConfig)functionCfgs[i]))
                {
                    functionCfgs.RemoveAt(i);
                }
            }
        }

        private void initializeFunctionExecutor()
        {
            if (mFunctionExecutor != null)
            {
                mFunctionExecutor.destroyAll();
            }
            mFunctionExecutor = new FunctionExecutor();
            foreach (FunctionConfig cfg in mSettings.FunctionConfigurations)
            {
                var executable = FunctionFactory.CreateExecutable(cfg);
                mFunctionExecutor.addExecutable(executable);
            }
        }

        private void initializeRenderEngine()
        {
            if (mRenderEngine != null)
            {
                mRenderEngine.destroyAll();
            }
            mRenderEngine = new RenderEngine();
            mRenderEngine.setRenderingUpdatedListener(mRendererUpdatedListener);
            Image baseImage = FileIOManager.LoadBase64(mSettings.Base64ImageString);
            if (baseImage == null)
            {
                baseImage = FileIOManager.LoadFallbackImage();
            }
            var imageRenderer = new ImageRenderer(baseImage);
            imageRenderer.invalidate();
            mRenderEngine.addRenderer(imageRenderer);

            foreach (EffectConfig effectCfg in mSettings.EffectConfigurations)
            {
                var renderer = RendererFactory.CreateRenderer(effectCfg);
                mRenderEngine.addRenderer(renderer);
            }
            mRenderEngine.run();
            Logger.Instance.LogMessage(TracingLevel.DEBUG, "updated render engine.");
        }
        #endregion

        /// <summary>
        /// Handles payload.
        /// </summary>
        /// <param name="payload"></param>
        /// <returns>true if handled the payload, else false</returns>
        public bool handlePayload(JObject payload)
        {
            // Handles Effect payload
            int effectCount = PayloadReader.isEffectPayload(payload);
            if(effectCount > 0)
            {
                mSettings.EffectConfigurations = PayloadReader.LoadEffectDataFromPayload(payload, effectCount);
                Logger.Instance.LogMessage(TracingLevel.DEBUG, "detected effect payload.");
                refineEffectConfigurations();
                initializeRenderEngine();
                return true;
            }

            // Handles Function payload
            int functionCount = PayloadReader.isFunctionPayload(payload);
            if (functionCount > 0)
            {
                mSettings.FunctionConfigurations = PayloadReader.LoadFunctionDataFromPayload(payload, functionCount);
                Logger.Instance.LogMessage(TracingLevel.DEBUG, "detected function payload.");
                refineFunctionConfigurations();
                initializeFunctionExecutor();
                return true;
            }

            // Handles Image update payload
            String imgPath = PayloadReader.isImageUpdatePayload(payload);
            if (imgPath != null && File.Exists(imgPath))
            {
                Logger.Instance.LogMessage(TracingLevel.DEBUG, "detected update image payload, " + imgPath);
                mSettings.Base64ImageString = FileIOManager.ProcessImageToBase64(imgPath);
                initializeRenderEngine();
                return true;
            }

            return false;
        }
        
        public void actionOnKeyPressed()
        {
            // Execute functions
            for (int i = 0; i < mSettings.FunctionConfigurations.Count; i++)
            {
                FunctionConfig cfg = (FunctionConfig)mSettings.FunctionConfigurations[i];
                if (cfg.mTrigger.Equals(FunctionConfig.ETrigger.OnKeyPressed.ToString()))
                {
                    mFunctionExecutor.executeFunctionAt(i, cfg.mDelay, cfg.mInterval, cfg.mDuration, cfg.mMetadata);
                }
            }

            // Animate renderers
            for (int i = 0; i < mSettings.EffectConfigurations.Count; i++)
            {
                EffectConfig cfg = (EffectConfig)mSettings.EffectConfigurations[i];
                if (cfg.mTrigger.Equals(EffectConfig.ETrigger.OnKeyPressed.ToString()))
                {
                    // image renderer is always at 0
                    mRenderEngine.animateRendererAt(i + 1, cfg.mDelay);
                }
            }
        }

        public void actionOnKeyReleased()
        {

        }

        public AvengersKeySettings getSettings()
        {
            return mSettings;
        }
    }
}
