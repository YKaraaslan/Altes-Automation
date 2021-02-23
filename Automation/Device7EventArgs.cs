using System;

namespace Automation
{
    class Device7EventArgs : EventArgs
    {
        public string speed, length;
        public bool running;
        public static event EventHandler<Device7EventArgs> event1;

        public Device7EventArgs() { }
        public Device7EventArgs(bool runningInfo, string speedInfo, string lengthInfo)
        {
            speed = speedInfo;
            length = lengthInfo;
            running = runningInfo;
        }

        protected virtual void onDataReceived(Device7EventArgs e)
        {
            event1?.Invoke(this, e);
        }

        public void sendEventInfo(bool runningInfo, string speedInfo, string lengthInfo)
        {
            onDataReceived(new Device7EventArgs(runningInfo, speedInfo, lengthInfo));
        }
    }
}
