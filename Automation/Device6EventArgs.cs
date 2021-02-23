using System;

namespace Automation
{
    class Device6EventArgs : EventArgs
    {
        public string speed, length;
        public bool running;
        public static event EventHandler<Device6EventArgs> event1;

        public Device6EventArgs() { }
        public Device6EventArgs(bool runningInfo, string speedInfo, string lengthInfo)
        {
            speed = speedInfo;
            length = lengthInfo;
            running = runningInfo;
        }

        protected virtual void onDataReceived(Device6EventArgs e)
        {
            event1?.Invoke(this, e);
        }

        public void sendEventInfo(bool runningInfo, string speedInfo, string lengthInfo)
        {
            onDataReceived(new Device6EventArgs(runningInfo, speedInfo, lengthInfo));
        }
    }
}
