using System;

namespace Automation
{
    class Device4EventArgs : EventArgs
    {
        public string speed, length;
        public bool running;
        public static event EventHandler<Device4EventArgs> event1;

        public Device4EventArgs() { }
        public Device4EventArgs(bool runningInfo, string speedInfo, string lengthInfo)
        {
            speed = speedInfo;
            length = lengthInfo;
            running = runningInfo;
        }

        protected virtual void onDataReceived(Device4EventArgs e)
        {
            event1?.Invoke(this, e);
        }

        public void sendEventInfo(bool runningInfo, string speedInfo, string lengthInfo)
        {
            onDataReceived(new Device4EventArgs(runningInfo, speedInfo, lengthInfo));
        }
    }
}
