using System;
using System.Runtime.InteropServices;


namespace ArtrointelPlugin.Utils
{
    // TODO: System Timer in C#, System.Timer or System.Threading.Timer,
    // does not guarantees accuracy of the time. Accurate timer might be needed in future.
    class AccurateTimer
    {
        private delegate void TimerEventDel(int id, int msg, IntPtr user, int dw1, int dw2);
        private const int TIME_PERIODIC = 1;
        private const int EVENT_TYPE = TIME_PERIODIC;// + 0x100;  // TIME_KILL_SYNCHRONOUS causes a hang ?!
        [DllImport("winmm.dll")]
        private static extern int timeBeginPeriod(int msec);
        [DllImport("winmm.dll")]
        private static extern int timeEndPeriod(int msec);
        [DllImport("winmm.dll")]
        private static extern int timeSetEvent(int delay, int resolution, TimerEventDel handler, IntPtr user, int eventType);
        [DllImport("winmm.dll")]
        private static extern int timeKillEvent(int id);

        // [DllImport("winmm.dll", SetLastError = true)]
        // static extern UInt32 timeGetDevCaps(ref TimeCaps timeCaps, UInt32 sizeTimeCaps);

        private TimerEventDel mHandler;
        private Action mAction;
        private int mTimerId;

        public AccurateTimer(Action action, int delay)
        {
            mAction = action;
            timeBeginPeriod(1);
            mHandler = new TimerEventDel(TimerCallback);
            mTimerId = timeSetEvent(delay, 0, mHandler, IntPtr.Zero, EVENT_TYPE);
        }

        public void Stop()
        {
            int err = timeKillEvent(mTimerId);
            timeEndPeriod(1);
            System.Threading.Thread.Sleep(100);// Ensure callbacks are drained
        }

        private void TimerCallback(int id, int msg, IntPtr user, int dw1, int dw2)
        {
            //TODO
        }
    }
    
}
