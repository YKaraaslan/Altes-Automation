using System;

namespace Automation
{
    class Device8EventArgs : EventArgs
    {
        public string speed, length;
        public bool running;
        public static event EventHandler<Device8EventArgs> event1;

        public Device8EventArgs() { }
        public Device8EventArgs(bool runningInfo, string speedInfo, string lengthInfo)
        {
            speed = speedInfo;
            length = lengthInfo;
            running = runningInfo;
        }

        protected virtual void onDataReceived(Device8EventArgs e)
        {
            event1?.Invoke(this, e);
        }

        public void sendEventInfo(bool runningInfo, string speedInfo, string lengthInfo)
        {
            onDataReceived(new Device8EventArgs(runningInfo, speedInfo, lengthInfo));
        }
    }
}
