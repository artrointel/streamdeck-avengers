﻿using System;
using System.Windows.Forms;
using ArtrointelPlugin.Utils;

namespace ArtrointelPlugin.SDFunctions
{
    public class SendKeyEvent : DelayedExecutable, IExecutable
    {
        Action<string> mSendKeyAction;
        ValueAnimator mKeyEventAnimator;

        public SendKeyEvent(bool asciiCode = false)
        {
            if (asciiCode)
            {
                mSendKeyAction = (ascNumbers) =>
                {
                    string[] ascs = ascNumbers.Split(' ');
                    short[] ascii = new short[ascs.Length];

                    for (int i = 0; i < ascii.Length; i++)
                    {
                        ascii[i] = short.Parse(ascs[i]);
                    }
                    
                    Keyboard.Send(ascii);
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

            int delms = (int)(delayInSecond * 1000);
            double ims = (int)(intervalInSecond * 1000);
            int durms = (int)(durationInSecond * 1000);
            if (delms == 0)
            {
                sendKeyEvent(ims, durms, metadata);
                return;
            }

            mDelayedTask = new DelayedTask(delms, () =>
            {
                sendKeyEvent(ims, durms, metadata);
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
