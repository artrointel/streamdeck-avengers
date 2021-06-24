using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ArtrointelPlugin.Utils;
using BarRaider.SdTools;

namespace ArtrointelPlugin.SDFunctions
{
    public class SendKeyEvent : DelayedExecutable, IExecutable
    {
        Action<String> mSendKeyAction;
        ValueAnimator mKeyEventAnimator;
        Keyboard mKeyboard;

        public SendKeyEvent(bool asciiCode = false)
        {
            if (asciiCode)
            {
                mKeyboard = new Keyboard();
                mSendKeyAction = (ascNumbers) =>
                {
                    string[] ascs = ascNumbers.Split(' ');
                    short[] ascii = new short[ascs.Length];

                    for (int i = 0; i < ascii.Length; i++)
                    {
                        ascii[i] = short.Parse(ascs[i]);
                    }
                    
                    mKeyboard.Send(ascii);
                };
            }
            else
            {
                mSendKeyAction = (text) =>
                {
                    SendKeys.SendWait("(" + text + ")"); // send text directly
                };
            }
        }

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
                mSendKeyAction(metadata);
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
                mSendKeyAction(keyString);
            });
        }

    }
}
