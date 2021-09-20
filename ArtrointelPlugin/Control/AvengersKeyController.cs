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
    public class AvengersKeyController : IControllable
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

        // State of the key
        private enum KeyState
        {
            READY,
            RUNNING,
            PAUSED
        }
        private KeyState mState;
        private DelayedTask mTaskOnFinished;
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

            // Handles Option payload
            if (PayloadReader.IsOptionPayload(payload))
            {
                int optionCount = PayloadReader.GetArrayCount(payload);
                mSettings.OptionConfigurations = PayloadReader.LoadOptionDataFromPayload(payload, optionCount);
                // TODO reset the key state and update the options
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
        
        #region Internal Functions
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
                var executable = CommandFactory.CreateCommand(cfg);
                mCommandExecutor.addCommand(executable);
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
            mRenderEngine.start();
        }

        private void actionOnDemand(string behavior)
        {
            if (behavior.Equals(OptionConfig.EBehavior.Restart.ToString()))
            {
                stop();
                start();
            }
            else if (behavior.Equals(OptionConfig.EBehavior.PauseResume.ToString()))
            {
                if (mState == KeyState.RUNNING)
                    pause();
                else if (mState == KeyState.PAUSED)
                    resume();
            }
            else if (behavior.Equals(OptionConfig.EBehavior.Stop.ToString()))
            {
                stop();
            }
        }
        #endregion

        #region Implement of Key Events
        public void onKeyAppeared()
        {
            mRenderEngine.start();
        }

        public void onKeyDisappeared()
        {
            mRenderEngine.preserve();
        }

        public void onKeyRemoved()
        {
            // TODO on key removed permanently. dispose all
        }

        public void onKeyPressed()
        {
            if (mState == KeyState.READY)
            {
                stop();
                start();
            }
            else if (mState == KeyState.RUNNING)
            {
                for (int i = 0; i < mSettings.OptionConfigurations.Count; i++)
                {
                    OptionConfig cfg = (OptionConfig)mSettings.OptionConfigurations[i];
                    // Action on key pressed while running
                    if (cfg.mCondition.Equals(OptionConfig.ECondition.
                        OnKeyPressedWhileRunning.ToString()))
                    {
                        actionOnDemand(cfg.mBehavior);
                    }
                }
            }
        }

        public void onKeyLongPressed()
        {
            for (int i = 0; i < mSettings.OptionConfigurations.Count; i++)
            {
                OptionConfig cfg = (OptionConfig)mSettings.OptionConfigurations[i];
                // Action on key pressed while running
                if (cfg.mCondition.Equals(OptionConfig.ECondition.
                    OnKeyLongPressed.ToString()))
                {
                    actionOnDemand(cfg.mBehavior);
                }
            }
        }
        #endregion

        #region Implement of IControllable Interface
        public async void start()
        {
            mTaskOnFinished?.stop();

            double coolTime = 0;
            // Executes commands in separated threads, but keeps current sequence of the commands
            for (int i = 0; i < mSettings.CommandConfigurations.Count; i++)
            {
                CommandConfig cfg = (CommandConfig)mSettings.CommandConfigurations[i];
                if (cfg.mTrigger.Equals(CommandConfig.ETrigger.OnKeyPressed.ToString()))
                {
                    int _i = i;
                    await Task.Run(() => {
                        mCommandExecutor.executeCommandAt(_i);
                    });
                }
                double dur = mCommandExecutor.getTotalDurationAt(i);
                coolTime = (dur > coolTime) ? dur : coolTime;
            }

            // Animates renderers in current thread
            mRenderEngine.start();
            for (int i = 0; i < mSettings.EffectConfigurations.Count; i++)
            {
                EffectConfig cfg = (EffectConfig)mSettings.EffectConfigurations[i];
                if (cfg.mTrigger.Equals(EffectConfig.ETrigger.OnKeyPressed.ToString()))
                {
                    int _i = i;
                    if (mUseBaseImageRenderer) // base image renderer is added at 0
                        _i += 1;
                    
                    double dur = mRenderEngine.getTotalDurationAt(_i);
                    coolTime = (dur > coolTime) ? dur : coolTime;
                    mRenderEngine.animateRendererAt(_i);
                }
            }

            mState = KeyState.RUNNING;
            
            mTaskOnFinished = new DelayedTask((int)(coolTime * 1000), () => {
                mState = KeyState.READY;
            });
            mTaskOnFinished.start();
        }

        public void pause()
        {
            // mCommandExecutor.pause();
            mRenderEngine.pause();
            mTaskOnFinished?.pause();
            mState = KeyState.PAUSED;
        }

        public void resume()
        {
            // mCommandExecutor.resume();
            mRenderEngine.resume();
            mTaskOnFinished?.resume();
            mState = KeyState.RUNNING;
        }

        public void stop()
        {
            // mCommandExecutor.stop();
            mRenderEngine.stop();
            mTaskOnFinished?.stop();
            mState = KeyState.READY;
        }

        public AvengersKeySettings getSettings()
        {
            return mSettings;
        }
        #endregion
    }
}
