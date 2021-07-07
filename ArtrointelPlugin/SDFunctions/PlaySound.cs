using System;
using System.Threading;
using System.Threading.Tasks;
using NAudio.Wave;
using ArtrointelPlugin.Utils;

// https://github.com/naudio/NAudio/blob/master/Docs/PlayAudioFileConsoleApp.md
namespace ArtrointelPlugin.SDFunctions
{
    // TODO In 2.0 Update, playsound will have more detail options with toggle key feature;
    // volume, loop, pause/stop/restart
    internal class PlaySound : FunctionBase
    {
        
        internal PlaySound(string metadata)
            : base(metadata)
        {
            
        }

        public override void execute(bool restart)
        {
            if (restart)
            {
                Task.Run(() =>
                {
                    try
                    {
                        using (var audioFile = new AudioFileReader(mMetadata))
                        using (var outputDevice = new WaveOutEvent())
                        {
                            outputDevice.Init(audioFile);
                            outputDevice.Play();
                            while (outputDevice.PlaybackState == PlaybackState.Playing)
                            {
                                Thread.Sleep(1000);
                            }
                        }
                    } catch (Exception e)
                    {
                        DLogger.LogMessage("Cannot play file :" + mMetadata + ", "+ e.Message);
                    }
                });
            }
            else
            {

            }
        }
    }
}
