using System;

namespace Automation
{
    class Device2EventArgs : EventArgs
    {
        public string speed, length;
        public bool running;
        public static event EventHandler<Device2EventArgs> event1;

        public Device2EventArgs() { }
        public Device2EventArgs(bool runningInfo, string speedInfo, string lengthInfo)
        {
            speed = speedInfo;
            length = lengthInfo;
            running = runningInfo;
        }

        protected virtual void onDataReceived(Device2EventArgs e)
        {
            event1?.Invoke(this, e);
        }

        public void sendEventInfo(bool runningInfo, string speedInfo, string lengthInfo)
        {
            onDataReceived(new Device2EventArgs(runningInfo, speedInfo, lengthInfo));
        }
    }
}
