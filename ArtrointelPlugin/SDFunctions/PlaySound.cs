using System;
using System.Windows.Media;
using ArtrointelPlugin.Utils;

namespace ArtrointelPlugin.SDFunctions
{
    internal class PlaySound : FunctionBase
    {
        private double mVolume = 1.0;
        private string mFilePath;

        internal PlaySound(string metadata)
            : base(metadata)
        {
            
        }

        public override void execute(bool restart)
        {
            if(restart)
            {
                var player = new MediaPlayer();
                player.Open(new Uri(mFilePath));
                player.Volume = mVolume;
                player.Play();
                // TODO call Close(). and use mPlayer.Dispatcher for threading
            }
            else
            {
                
            }
        }
    }
}
