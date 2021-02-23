using System;

namespace Automation
{
    class Device3EventArgs : EventArgs
    {
        public string speed, length;
        public bool running;
        public static event EventHandler<Device3EventArgs> event1;

        public Device3EventArgs() { }
        public Device3EventArgs(bool runningInfo, string speedInfo, string lengthInfo)
        {
            speed = speedInfo;
            length = lengthInfo;
            running = runningInfo;
        }

        protected virtual void onDataReceived(Device3EventArgs e)
        {
            event1?.Invoke(this, e);
        }

        public void sendEventInfo(bool runningInfo, string speedInfo, string lengthInfo)
        {
            onDataReceived(new Device3EventArgs(runningInfo, speedInfo, lengthInfo));
        }
    }
}
