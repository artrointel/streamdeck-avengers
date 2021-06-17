using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Media;
using ArtrointelPlugin.Utils;

namespace ArtrointelPlugin.SDFunctions
{
    public class PlaySound : DelayedExecutable, IExecutable
    {
        public void execute(double delayInSecond, double intervalInSecond, double durationInSecond, 
            bool restart = true, string metadata = null)
        {
            if(restart)
            {
                if(mDelayedTask != null)
                {
                    mDelayedTask.cancel();
                }
            }

            int dms = (int)(delayInSecond * 1000);
            if(dms == 0)
            {
                playSound(metadata);
            }
            else
            {
                mDelayedTask = new DelayedTask(dms, () =>
                {
                    playSound(metadata);
                });
            }
        }

        private void playSound(string mediaFilePath) 
        {
            SoundPlayer player = new SoundPlayer(mediaFilePath);
            player.Play();
        }
    }
}
