using System;
using System.IO;
using System.Drawing;
using Newtonsoft.Json.Linq;
using BarRaider.SdTools;
using ArtrointelPlugin.SDGraphics;
using ArtrointelPlugin.SDGraphics.Renderer;
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
            for (int i = effectCfgs.Count - 1; i != -1; i--)
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
            for (int i = functionCfgs.Count - 1; i != -1; i--)
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
                baseImage = FileIOManager.GetFallbackImage();
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
            if(PayloadReader.isEffectPayload(payload))
            {
                int effectCount = PayloadReader.getArrayCount(payload);
                mSettings.EffectConfigurations = PayloadReader.LoadEffectDataFromPayload(payload, effectCount);
                refineEffectConfigurations();
                initializeRenderEngine();
                return true;
            }

            // Handles Function payload
            if(PayloadReader.isFunctionPayload(payload))
            {
                int functionCount = PayloadReader.getArrayCount(payload);
                mSettings.FunctionConfigurations = PayloadReader.LoadFunctionDataFromPayload(payload, functionCount);
                refineFunctionConfigurations();
                initializeFunctionExecutor();
                return true;
            }
            
            // Handles Image update payload
            if (PayloadReader.isImageUpdatePayload(payload))
            {
                string imgPath = PayloadReader.getFilePath(payload);
                if(imgPath != null && File.Exists(imgPath))
                {
                    mSettings.Base64ImageString = FileIOManager.ProcessImageFileToSDBase64(imgPath);
                    initializeRenderEngine();
                }
                return true;
            }

            // Handles Additional special commands for the Avengers Key.
            if(PayloadReader.isCommandPayload(payload))
            {
                return handleCommands(payload);
            }

            return false;
        }

        private bool handleCommands(JObject payload)
        {
            // Command to use file icon as a base image
            string data = PayloadReader.getFilePath(payload);
            if (data != null && File.Exists(data)) {
                try
                {
                    LoadIconHelper icon = new LoadIconHelper(data);
                    Image resizedIcon = FileIOManager.ResizeImage(icon.getJumboIcon());
                    string base64Image = FileIOManager.ImageToBase64(resizedIcon);
                    resizedIcon.Dispose();
                    mSettings.Base64ImageString = base64Image;
                    initializeRenderEngine();
                }
                catch (Exception e)
                {
                    DLogger.LogMessage(TracingLevel.DEBUG, "Could not load icon from " + data + ": " + e.Message);
                }
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
            // TODO not used at least now. for long-pressed event
        }

        public AvengersKeySettings getSettings()
        {
            return mSettings;
        }
    }
}
