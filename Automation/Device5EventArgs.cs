using System;

namespace Automation
{
    class Device5EventArgs : EventArgs
    {
        public string speed, length;
        public bool running;
        public static event EventHandler<Device5EventArgs> event1;

        public Device5EventArgs() { }
        public Device5EventArgs(bool runningInfo, string speedInfo, string lengthInfo)
        {
            speed = speedInfo;
            length = lengthInfo;
            running = runningInfo;
        }

        protected virtual void onDataReceived(Device5EventArgs e)
        {
            event1?.Invoke(this, e);
        }

        public void sendEventInfo(bool runningInfo, string speedInfo, string lengthInfo)
        {
            onDataReceived(new Device5EventArgs(runningInfo, speedInfo, lengthInfo));
        }
    }
}
