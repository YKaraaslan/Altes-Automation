using System;

namespace Automation
{
    public class Device1EventArgs : EventArgs
    {
        public string speed, length;
        public bool running;
        public static event EventHandler<Device1EventArgs> event1;

        public Device1EventArgs() { }
        public Device1EventArgs(bool runningInfo, string speedInfo, string lengthInfo) 
        {
            speed = speedInfo; 
            length = lengthInfo;
            running = runningInfo;
        }

        protected virtual void onDataReceived(Device1EventArgs e)
        {
            event1?.Invoke(this, e);
        }

        public void sendEventInfo(bool runningInfo, string speedInfo, string lengthInfo)
        {
            onDataReceived(new Device1EventArgs(runningInfo, speedInfo, lengthInfo));
        }
    }
}
