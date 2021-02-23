using Modbus.Device;
using System.IO.Ports;

namespace Automation
{
    class VarsDevices
    {
        public static int deviceWorking, deviceProjectReady;
        public static SerialPort port;
        public static IModbusSerialMaster master;

        public string name { get; set; }
        public int amount { get; set; }

        public static int DeviceWorking { get => deviceWorking; set => deviceWorking = value; }
        public static int DeviceProjectReady { get => deviceProjectReady; set => deviceProjectReady = value; }
        public static SerialPort Port { get => port; set => port = value; }
        public static IModbusSerialMaster Master { get => master; set => master = value; }
    }
}
