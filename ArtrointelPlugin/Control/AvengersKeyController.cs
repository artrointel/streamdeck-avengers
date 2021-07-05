using System;
using System.IO;
using System.Threading.Tasks;
using System.Drawing;
using Newtonsoft.Json.Linq;
using ArtrointelPlugin.SDGraphics;
using ArtrointelPlugin.SDGraphics.Renderer;
using ArtrointelPlugin.SDGraphics.Renderer.AnimatedEffects;
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
        // Render engine to render user-customized effects
        private RenderEngine mRenderEngine;

        // Callback action after the Render engine updated the canvas image.
        private Action<Image> mRendererUpdatedListener;

        // Executor of user-customized functions
        private FunctionExecutor mFunctionExecutor;

        // Configuration data created by user
        private AvengersKeySettings mSettings;

        private bool mUseBaseImageRenderer;
        #endregion

        /// <summary>
        /// Creates Avengers Key controller.
        /// </summary>
        /// <param name="jSettings">json settings object for the key configurations</param>
        /// <param name="rendererUpdatedListener">called whenever the effect is updated</param>
        public AvengersKeyController(JObject jSettings, Action<Image> rendererUpdatedListener)
        {
            mSettings = AvengersKeySettings.LoadFrom(jSettings);
            mRendererUpdatedListener = rendererUpdatedListener;

            initializeRenderEngine();
            initializeFunctionExecutor();  
        }

        #region Internal
        // Refines incoming data from PI to make controller safer.
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

        // Refines incoming data from PI to make controller safer.
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

            mUseBaseImageRenderer = true;
            foreach (EffectConfig effectCfg in mSettings.EffectConfigurations)
            {
                var renderer = RendererFactory.CreateRenderer(effectCfg);
                if(renderer is AlphaBlendRenderer)
                {
                    AlphaBlendRenderer baseImageRenderer = (AlphaBlendRenderer) renderer;
                    baseImageRenderer.setImages(baseImage);
                    baseImageRenderer.invalidate();
                    mUseBaseImageRenderer = false;
                }
                mRenderEngine.addRenderer(renderer);
            }

            if(mUseBaseImageRenderer)
            {
                ImageRenderer imageRenderer = new ImageRenderer(baseImage);
                imageRenderer.invalidate();
                mRenderEngine.addRendererAt(0, imageRenderer);
            }
            mRenderEngine.run();
        }
        #endregion

        /// <summary>
        /// Handles incoming payload regarding avengers key. <see cref="PayloadReader"/>
        /// </summary>
        /// <param name="payload"></param>
        /// <returns>true if handled the payload, else false</returns>
        public bool handlePayload(JObject payload)
        {
            // Handles Effect payload
            if(PayloadReader.IsEffectPayload(payload))
            {
                int effectCount = PayloadReader.GetArrayCount(payload);
                mSettings.EffectConfigurations = PayloadReader.LoadEffectDataFromPayload(payload, effectCount);
                refineEffectConfigurations();
                initializeRenderEngine();
                return true;
            }

            // Handles Function payload
            if(PayloadReader.IsFunctionPayload(payload))
            {
                int functionCount = PayloadReader.GetArrayCount(payload);
                mSettings.FunctionConfigurations = PayloadReader.LoadFunctionDataFromPayload(payload, functionCount);
                refineFunctionConfigurations();
                initializeFunctionExecutor();
                return true;
            }
            
            // Handles Image update payload
            if (PayloadReader.IsImageUpdatePayload(payload))
            {
                string imgPath = PayloadReader.GetFilePath(payload);
                if(imgPath != null && File.Exists(imgPath))
                {
                    mSettings.Base64ImageString = FileIOManager.ProcessImageFileToSDBase64(imgPath);
                    initializeRenderEngine();
                }
                return true;
            }

            // Handles the other special commands for the Avengers Key.
            if(PayloadReader.IsCommandPayload(payload))
            {
                return handleCommands(payload);
            }

            return false;
        }

        private bool handleCommands(JObject payload)
        {
            // Command to use file icon as a base image
            string data = PayloadReader.GetFilePath(payload);
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
                    DLogger.LogMessage("Could not load icon from " + data + ": " + e.Message);
                }
                return true;
            }
            return false;
        }
        
        public async void actionOnKeyPressed()
        {
            // Executes functions in separated thread
            for (int i = 0; i < mSettings.FunctionConfigurations.Count; i++)
            {
                FunctionConfig cfg = (FunctionConfig)mSettings.FunctionConfigurations[i];
                if (cfg.mTrigger.Equals(FunctionConfig.ETrigger.OnKeyPressed.ToString()))
                {
                    await Task.Run(() => {
                        mFunctionExecutor.executeFunctionAt(i, cfg.mDelay, cfg.mInterval, cfg.mDuration, cfg.mMetadata);
                    });
                }
            }

            // Animates renderers in current thread
            for (int i = 0; i < mSettings.EffectConfigurations.Count; i++)
            {
                EffectConfig cfg = (EffectConfig)mSettings.EffectConfigurations[i];
                if (cfg.mTrigger.Equals(EffectConfig.ETrigger.OnKeyPressed.ToString()))
                {
                    if (mUseBaseImageRenderer)
                        mRenderEngine.animateRendererAt(i+1); // base image renderer is added at 0
                    else
                        mRenderEngine.animateRendererAt(i);
                }
            }
        }

        /* TODO not used at least now. can be used with long-pressed event in future
        public void actionOnKeyReleased()
        {
            
        }
        */

        public AvengersKeySettings getSettings()
        {
            return mSettings;
        }
    }
}
