using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ArtrointelPlugin.Utils;

namespace ArtrointelPlugin.SDFunctions
{
    public class SendKeyEvent : DelayedExecutable, IExecutable
    {
        ValueAnimator mKeyEventAnimator;
        public void execute(double delayInSecond, double intervalInSecond, double durationInSecond,
            bool restart, string metadata)
        {
            if(restart)
            {
                if (mDelayedTask != null)
                {
                    mDelayedTask.cancel();
                }
                if(mKeyEventAnimator != null)
                {
                    mKeyEventAnimator.stop();
                    mKeyEventAnimator.destroy();
                }
            }

            int delay = (int)(delayInSecond * 1000);
            double ims = (int)(intervalInSecond * 1000);
            int dms = (int)(durationInSecond * 1000);
            if (delay == 0)
            {
                sendKeyEvent(ims, dms, metadata);
                return;
            }

            mDelayedTask = new DelayedTask(delay, () =>
            {
                sendKeyEvent(ims, dms, metadata);
            });
        }

        private void sendKeyEvent(double intervalInMillisecond, int durationInMillisecond, string metadata)
        {
            if (durationInMillisecond == 0)
            {
                SendKeys.SendWait(metadata);
                return;
            }
            else
            {
                // actually user needs keyboard macro in this case
                buildKeyEventAnimator(intervalInMillisecond, durationInMillisecond, metadata);
                mKeyEventAnimator.start();
                return;
            }
        }
        private void buildKeyEventAnimator(double intervalInMillisecond, int durationInMillisecond, string keyString)
        {
            // ValueAnimator was not intended to use like this, but it makes quite simple
            mKeyEventAnimator = new ValueAnimator(0, 1, durationInMillisecond, intervalInMillisecond);
            mKeyEventAnimator.setAnimationListeners((v, duration) =>
            {
                SendKeys.SendWait(keyString);
            });
        }
    }
}
