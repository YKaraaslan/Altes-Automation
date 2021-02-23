using System;

namespace Automation
{
    class Device10EventArgs : EventArgs
    {
        public string speed, length;
        public bool running;
        public static event EventHandler<Device10EventArgs> event1;

        public Device10EventArgs() { }
        public Device10EventArgs(bool runningInfo, string speedInfo, string lengthInfo)
        {
            speed = speedInfo;
            length = lengthInfo;
            running = runningInfo;
        }

        protected virtual void onDataReceived(Device10EventArgs e)
        {
            event1?.Invoke(this, e);
        }

        public void sendEventInfo(bool runningInfo, string speedInfo, string lengthInfo)
        {
            onDataReceived(new Device10EventArgs(runningInfo, speedInfo, lengthInfo));
        }
    }
}
