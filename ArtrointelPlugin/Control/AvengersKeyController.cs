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
using ArtrointelPlugin.SDCommands;

namespace ArtrointelPlugin.Control
{
    /// <summary>
    /// Controller of the avengers key.
    /// It controls icon rendering and commands to be executed.
    /// </summary>
    public class AvengersKeyController
    {
        #region Internal Members
        // Render engine to render user-customized effects
        private RenderEngine mRenderEngine;

        // Callback action after the Render engine updated the canvas image.
        private Action<Image> mRendererUpdatedListener;

        // Executor of user-customized commands
        private CommandExecutor mCommandExecutor;

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
            initializeCommandExecutor();  
        }

        public void startRenderEngine()
        {
            mRenderEngine.run();
        }

        public void pauseRenderEngine()
        {
            mRenderEngine.pause();
        }

        public bool handlePayload(JObject payload)
        {
            // Handles Effect payload
            if (PayloadReader.IsEffectPayload(payload))
            {
                int effectCount = PayloadReader.GetArrayCount(payload);
                mSettings.EffectConfigurations = PayloadReader.LoadEffectDataFromPayload(payload, effectCount);
                refineEffectConfigurations();
                initializeRenderEngine();
                return true;
            }

            // Handles Command payload
            if (PayloadReader.IsCommandPayload(payload))
            {
                int commandCount = PayloadReader.GetArrayCount(payload);
                mSettings.CommandConfigurations = PayloadReader.LoadCommandDataFromPayload(payload, commandCount);
                refineCommandConfigurations();
                initializeCommandExecutor();
                return true;
            }

            // Handles Image update payload
            if (PayloadReader.IsImageUpdatePayload(payload))
            {
                string imgPath = PayloadReader.GetFilePath(payload);
                if (imgPath != null && File.Exists(imgPath))
                {
                    mSettings.Base64ImageString = FileIOManager.ProcessImageFileToSDBase64(imgPath);
                    initializeRenderEngine();
                }
                return true;
            }

            // Handles special things for the Avengers Key below.
            if (PayloadReader.IsImageUpdateFromFilePayload(payload))
            {
                return updateImageFromFileIcon(payload);
            }

            return false;
        }

        private bool updateImageFromFileIcon(JObject payload)
        {
            // Command to use file icon as a base image
            string data = PayloadReader.GetFilePath(payload);
            if (data != null && File.Exists(data))
            {
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
        private void refineCommandConfigurations()
        {
            var commandCfgs = mSettings.CommandConfigurations;
            for (int i = commandCfgs.Count - 1; i != -1; i--)
            {
                if (!CommandFactory.IsSupported((CommandConfig)commandCfgs[i]))
                {
                    commandCfgs.RemoveAt(i);
                }
            }
        }

        private void initializeCommandExecutor()
        {
            if (mCommandExecutor != null)
            {
                mCommandExecutor.destroyAll();
            }
            mCommandExecutor = new CommandExecutor();
            foreach (CommandConfig cfg in mSettings.CommandConfigurations)
            {
                var executable = CommandFactory.CreateExecutable(cfg);
                mCommandExecutor.addExecutable(executable);
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
        
        public async void actionOnKeyPressed()
        {
            // Executes commands in separated threads
            for (int i = 0; i < mSettings.CommandConfigurations.Count; i++)
            {
                CommandConfig cfg = (CommandConfig)mSettings.CommandConfigurations[i];
                if (cfg.mTrigger.Equals(CommandConfig.ETrigger.OnKeyPressed.ToString()))
                {
                    await Task.Run(() => {
                        mCommandExecutor.executeCommandAt(
                            i, cfg.mDelay, cfg.mInterval, cfg.mDuration, cfg.mMetadata);
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
                        mRenderEngine.animateRendererAt(i + 1); // base image renderer is added at 0
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
