using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ArtrointelPlugin.SDFunctions
{
    class VolumeControl : IExecutable
    {
        public void execute(double delayInSecond = 0, 
            double intervalInSecond = 33, double durationInSecond = 0, 
            bool restart = true, string metadata = null)
        {
            switch(metadata)
            {
                case "VolumeUp":
                    VolumeUp();
                    break;
                case "VolumeDown":
                    VolumeDown();
                    break;
                case "ToggleMute":
                    ToggleMute();
                    break;
            }
        }

        [DllImport("user32.dll")]
        static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);

        private static void ToggleMute()
        {
            keybd_event((byte)Keys.VolumeMute, 0, 0, 0);
        }

        private static void VolumeDown()
        {
            keybd_event((byte)Keys.VolumeDown, 0, 0, 0);
        }

        private static void VolumeUp()
        {
            keybd_event((byte)Keys.VolumeUp, 0, 0, 0);
        }
    }
}
