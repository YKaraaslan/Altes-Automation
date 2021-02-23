using MaterialDesignThemes.Wpf;
using Modbus.Device;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;


namespace Automation
{
    /// <summary>
    /// Interaction logic for Devices.xaml
    /// </summary>
    public partial class Devices : UserControl
    {
        Assembly assembly;
        ResourceManager rm;
        CultureInfo cultureInfo;
        string pathToDBFile = Properties.Resources.ConnectionString;
        SerialPort connection;
        IModbusSerialMaster modbus;
        BackgroundWorker backgroundWorker = new BackgroundWorker();
        BackgroundWorker backgroundWorker2 = new BackgroundWorker();
        bool connected = false;
        List<string> deviceID = new List<string>();
        List<string> dbNames = new List<string>();
        List<string> dbNamesRunning = new List<string>();
        List<string> dbNamesWorkingOn = new List<string>();
        List<string> slaveAddress = new List<string>();
        List<string> readingAddresses = new List<string>();
        List<string> readingAddresses1 = new List<string>();
        List<string> readingAddresses2 = new List<string>();
        List<string> readingAddresses3 = new List<string>();
        List<string> readingAddresses4 = new List<string>();
        List<string> readingAddresses5 = new List<string>();
        Card[] cards;
        TextBlock[] deviceNames;
        TextBlock[] operatorNames;
        private Device1EventArgs eventArgs1 = new Device1EventArgs();
        private Device2EventArgs eventArgs2 = new Device2EventArgs();
        private Device3EventArgs eventArgs3 = new Device3EventArgs();
        private Device4EventArgs eventArgs4 = new Device4EventArgs();
        private Device5EventArgs eventArgs5 = new Device5EventArgs();
        private Device6EventArgs eventArgs6 = new Device6EventArgs();
        private Device7EventArgs eventArgs7 = new Device7EventArgs();
        private Device8EventArgs eventArgs8 = new Device8EventArgs();
        private Device9EventArgs eventArgs9 = new Device9EventArgs();
        private Device10EventArgs eventArgs10 = new Device10EventArgs();
        int deviceCounter = 0;

        bool online1, online2, online3, online4, online5;
        bool online6, online7, online8, online9, online10;

        bool lastSituation1, lastSituation2, lastSituation3, lastSituation4, lastSituation5;
        bool lastSituation6, lastSituation7, lastSituation8, lastSituation9, lastSituation10;

        int machine1Length, machine2Length, machine3Length, machine4Length, machine5Length;
        int machine6Length, machine7Length, machine8Length, machine9Length, machine10Length;

        List<DateTime> zeros = new List<DateTime>();
        List<DateTime> ones = new List<DateTime>();

        public Devices()
        {
            InitializeComponent();
            assembly = Assembly.Load("Automation");
            rm = new ResourceManager("Automation.Languages.language", assembly);
            cultureInfo = new CultureInfo(Properties.Settings.Default.language);
            cards = new Card[] { device1Card, device2Card, device3Card, device4Card, device5Card, 
                device6Card, device7Card, device8Card, device9Card, device10Card };

            deviceNames = new TextBlock[] { deviceName1, deviceName2, deviceName3, deviceName4, deviceName5,
                deviceName6, deviceName7, deviceName8, deviceName9, deviceName10 };
            operatorNames = new TextBlock[] { operatorName1, operatorName2, operatorName3, operatorName4, operatorName5,
                operatorName6, operatorName7, operatorName8, operatorName9, operatorName10 };

            backgroundWorker.WorkerSupportsCancellation = false;
            backgroundWorker2.WorkerSupportsCancellation = true;
            backgroundWorker.DoWork += workCheck;
            backgroundWorker2.DoWork += workDevice;
            backgroundWorker.RunWorkerAsync();

            init();
        }

        private void workCheck(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                Thread.Sleep(500);
                if (!connected)
                {
                    string[] ports = SerialPort.GetPortNames();
                    if(ports.Length < 1)
                    {
                        this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                        {
                            connectionStatus.Background = Brushes.Red;
                        }));
                        continue;
                    }
                    try
                    {
                        if (VarsDevices.port != null && VarsDevices.Master != null)
                        {
                            connection = VarsDevices.Port;
                            modbus = VarsDevices.Master;
                            connected = true;
                            if (!connection.IsOpen)
                            {
                                connection.Open();
                            }
                            this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                            connectionStatus.Background = Brushes.Green
                            ));
                            if (!backgroundWorker2.IsBusy)
                                backgroundWorker2.RunWorkerAsync();
                        }
                        else
                        {
                            connection = new SerialPort();
                            connection.PortName = "COM3";
                            connection.BaudRate = 9600;
                            connection.Parity = Parity.Odd;
                            connection.DataBits = 8;
                            connection.StopBits = StopBits.One;
                            connection.ReadTimeout = 500;
                            connection.Open();
                            modbus = ModbusSerialMaster.CreateRtu(connection);
                            connected = true;
                            VarsDevices.Master = modbus;
                            VarsDevices.Port = connection;
                            this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                            connectionStatus.Background = Brushes.Green
                            ));
                            if (!backgroundWorker2.IsBusy)
                                backgroundWorker2.RunWorkerAsync();
                        }
                    }
                    catch
                    {
                        connected = false;
                        backgroundWorker2.CancelAsync();
                    }
                }

                if (!connection.IsOpen)
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                    {
                        connected = false;
                        connectionStatus.Background = Brushes.Red;
                    }));

                TimeSpan start = new TimeSpan(08, 30, 0);
                TimeSpan end = new TimeSpan(23, 59, 0);
                TimeSpan now = DateTime.Now.TimeOfDay;

                if ((now > start) && (now < end) && (machine1Length != 0 || machine2Length != 0 || machine3Length != 0
                    || machine4Length != 0 || machine5Length != 0 || machine6Length != 0 || machine7Length != 0 ||
                    machine8Length != 0 || machine9Length != 0 || machine10Length != 0))
                {
                    if (!File.Exists(Properties.Resources.LastSavedFilePath))
                    {
                        using (StreamWriter sw = File.CreateText(Properties.Resources.LastSavedFilePath))
                        {
                            sw.WriteLine(DateTime.Now.ToString());
                            Write.ToFile("LastSaved.txt is created");
                        }
                    }

                    var LastLine = Convert.ToDateTime(File.ReadLines(Properties.Resources.LastSavedFilePath).Last().Trim());
                    var lastSavedTime = DateTime.Now.Subtract(LastLine).TotalHours;

                    if (lastSavedTime > 12)
                    {
                        try
                        {
                            Write.ToFile("Beginning to save to the database. Last time: " + String.Format("{0:0.00}", lastSavedTime)  + " hours.");
                            SaveToDatabase();
                        }
                        catch (Exception ex)
                        {
                            Write.ToFile("SaveToDb Failed - Line 192 - Exception: " + ex.Message);
                        }
                    }
                }
            }
        }

        private void init()
        {
            disableVisibility();
            using (MySqlConnection localDbConnection = new MySqlConnection(pathToDBFile))
            {
                localDbConnection.Open();

                MySqlCommand command = new MySqlCommand("SELECT * FROM Devices ORDER BY ID ASC", localDbConnection);
                MySqlDataAdapter dataAdapter = new MySqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                dataAdapter.Fill(dataTable);

                deviceCounter = 0;
                deviceID.Clear();
                dbNames.Clear();
                slaveAddress.Clear();
                readingAddresses1.Clear();
                readingAddresses2.Clear();
                readingAddresses3.Clear();
                readingAddresses4.Clear();
                readingAddresses5.Clear();

                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    deviceCounter++;
                    deviceNames[i].Text = dataTable.Rows[i][1].ToString();
                    operatorNames[i].Text = dataTable.Rows[i][3].ToString();
                    deviceID.Add(dataTable.Rows[i][0].ToString());
                    dbNames.Add(dataTable.Rows[i][4].ToString());
                    dbNamesRunning.Add(dataTable.Rows[i][11].ToString());
                    dbNamesWorkingOn.Add(dataTable.Rows[i][13].ToString());
                    slaveAddress.Add(dataTable.Rows[i][5].ToString());
                    readingAddresses1.Add(dataTable.Rows[i][6].ToString());
                    readingAddresses2.Add(dataTable.Rows[i][7].ToString());
                    readingAddresses3.Add(dataTable.Rows[i][8].ToString());
                    readingAddresses4.Add(dataTable.Rows[i][9].ToString());
                    readingAddresses5.Add(dataTable.Rows[i][10].ToString());
                    cards[i].Visibility = Visibility.Visible;
                }
                deviceAmount.Content = deviceCounter.ToString();
            }
        }

        private void disableVisibility()
        {
            for(int i = 0; i < cards.Length; i++)
            {
                cards[i].Visibility = Visibility.Collapsed;
            }
        }

        private void Closed(object sender, EventArgs e)
        {
            init();
        }

        private void add_Click(object sender, RoutedEventArgs e)
        {
            DevicesAdd devicesAdd = new DevicesAdd();
            devicesAdd.Closed += Closed;
            devicesAdd.ShowDialog();
        }

        private void workDevice(object sender, DoWorkEventArgs e)
        {
            while (!backgroundWorker2.CancellationPending)
            {
                try
                {
                    _ = readDevice1();
                }
                catch
                {
                    Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {
                        length1.Content = "----------";
                        speed1.Content = "----------";
                        work1.Text = "----------";
                        status1.Background = Brushes.OrangeRed;
                        sendData(0, false, "----------", "----------");
                    }));
                }


                try
                {
                    _ = readDevice2();
                }
                catch
                {
                    Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {
                        length2.Content = "----------";
                        speed2.Content = "----------";
                        work2.Text = "----------";
                        status2.Background = Brushes.OrangeRed;
                        sendData(1, false, "----------", "----------");
                    }));
                }


                try
                {
                    _ = readDevice3();
                }
                catch
                {
                    Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {
                        length3.Content = "----------";
                        speed3.Content = "----------";
                        work3.Text = "----------";
                        status3.Background = Brushes.OrangeRed;
                        sendData(2, false, "----------", "----------");
                    }));
                }


                try
                {
                    _ = readDevice4();
                }
                catch
                {
                    Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {
                        length4.Content = "----------";
                        speed4.Content = "----------";
                        work4.Text = "----------";
                        status4.Background = Brushes.OrangeRed;
                        sendData(3, false, "----------", "----------");
                    }));
                }


                try
                {
                    _ = readDevice5();
                }
                catch
                {
                    Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {
                        length5.Content = "----------";
                        speed5.Content = "----------";
                        work5.Text = "----------";
                        status5.Background = Brushes.OrangeRed;
                        sendData(4, false, "----------", "----------");
                    }));
                }


                try
                {
                    _ = readDevice6();
                }
                catch
                {
                    Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {
                        length6.Content = "----------";
                        speed6.Content = "----------";
                        work6.Text = "----------";
                        status6.Background = Brushes.OrangeRed;
                        sendData(5, false, "----------", "----------");
                    }));
                }


                try
                {
                    _ = readDevice7();
                }
                catch
                {
                    Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {
                        length7.Content = "----------";
                        speed7.Content = "----------";
                        work7.Text = "----------";
                        status7.Background = Brushes.OrangeRed;
                        sendData(6, false, "----------", "----------");
                    }));
                }


                try
                {
                    _ = readDevice8();
                }
                catch
                {
                    Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {
                        length8.Content = "----------";
                        speed8.Content = "----------";
                        work8.Text = "----------";
                        status8.Background = Brushes.OrangeRed;
                        sendData(7, false, "----------", "----------");
                    }));
                }


                try
                {
                    _ = readDevice9();
                }
                catch
                {
                    Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {
                        length9.Content = "----------";
                        speed9.Content = "----------";
                        work9.Text = "----------";
                        status9.Background = Brushes.OrangeRed;
                        sendData(8, false, "----------", "----------");
                    }));
                }

                try
                {
                    _ = readDevice10();
                }
                catch
                {
                    Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {
                        length10.Content = "----------";
                        speed10.Content = "----------";
                        work10.Text = "----------";
                        status10.Background = Brushes.OrangeRed;
                        sendData(9, false, "----------", "----------");
                    }));
                }
            }
        }

        private async Task readDevice10()
        {
            //registeradress3 = metraj, registerAdress2 = hız
            ushort[] registers = await modbus.ReadHoldingRegistersAsync(byte.Parse(slaveAddress[9]), ushort.Parse(readingAddresses1[9]), 1);
            ushort[] registers2 = await modbus.ReadHoldingRegistersAsync(byte.Parse(slaveAddress[9]), ushort.Parse(readingAddresses2[9]), 1);
            ushort[] registers3 = await modbus.ReadHoldingRegistersAsync(byte.Parse(slaveAddress[9]), ushort.Parse(readingAddresses3[9]), 1);

            machine10Length = Convert.ToInt32(registers3[0])*10;

            using (MySqlConnection localDbConnection = new MySqlConnection(pathToDBFile))
            {
                await localDbConnection.OpenAsync();
                MySqlCommand command = new MySqlCommand("SELECT WorkingOn FROM Devices WHERE ID = @ID ", localDbConnection);
                command.Parameters.AddWithValue("@ID", deviceID[9]);
                MySqlDataAdapter dataAdapter = new MySqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                await dataAdapter.FillAsync(dataTable);
                await Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {
                work10.Text = dataTable.Rows[0][0].ToString();
                }));
            }

            await Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {


                length10.Content = machine10Length.ToString() + " m";
                speed10.Content = registers2[0].ToString() + " m/dak";
                if (registers[0].ToString() == "1")
                {
                    online10 = true;
                    status10.Background = Brushes.Green;
                    UpdateDeviceStatus(true, deviceID[9]);
                }
                else
                {
                    online10 = false;
                    status10.Background = Brushes.Red;
                    UpdateDeviceStatus(false, deviceID[9]);
                }
                sendData(9, online10, registers2[0].ToString(), machine10Length.ToString());
            }));
            if (online10 != lastSituation10)
            {
                await saveRunningSituationIntoDatabase(online10, dbNamesRunning[9]);
            }
            lastSituation10 = online10;
        }

        private async Task readDevice9()
        {
            ushort[] registers = await modbus.ReadHoldingRegistersAsync(byte.Parse(slaveAddress[8]), ushort.Parse(readingAddresses1[8]), 1);
            ushort[] registers2 = await modbus.ReadHoldingRegistersAsync(byte.Parse(slaveAddress[8]), ushort.Parse(readingAddresses2[8]), 1);
            ushort[] registers3 = await modbus.ReadHoldingRegistersAsync(byte.Parse(slaveAddress[8]), ushort.Parse(readingAddresses3[8]), 1);

            machine9Length = Convert.ToInt32(registers3[0]) * 10;

            using (MySqlConnection localDbConnection = new MySqlConnection(pathToDBFile))
            {
                await localDbConnection.OpenAsync();
                MySqlCommand command = new MySqlCommand("SELECT WorkingOn FROM Devices WHERE ID = @ID ", localDbConnection);
                command.Parameters.AddWithValue("@ID", deviceID[8]);
                MySqlDataAdapter dataAdapter = new MySqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                await dataAdapter.FillAsync(dataTable);
                await Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {
                work9.Text = dataTable.Rows[0][0].ToString();
                }));
            }

            await Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {
                length9.Content = machine9Length.ToString() + " m"; ;
                speed9.Content = registers2[0].ToString() + " m/dak";
                if (registers[0].ToString() == "1")
                {
                    online9 = true;
                    status9.Background = Brushes.Green;
                    UpdateDeviceStatus(true, deviceID[8]);
                }
                else
                {
                    online9 = false;
                    status9.Background = Brushes.Red;
                    UpdateDeviceStatus(false, deviceID[8]);
                }
                sendData(8, online9, registers2[0].ToString(), machine9Length.ToString());
            }));
            if (online9 != lastSituation9)
            {
                await saveRunningSituationIntoDatabase(online9, dbNamesRunning[8]);
            }
            lastSituation9 = online9;
        }

        private async Task readDevice8()
        {
            ushort[] registers = await modbus.ReadHoldingRegistersAsync(byte.Parse(slaveAddress[7]), ushort.Parse(readingAddresses1[7]), 1);
            ushort[] registers2 = await modbus.ReadHoldingRegistersAsync(byte.Parse(slaveAddress[7]), ushort.Parse(readingAddresses2[7]), 1);
            ushort[] registers3 = await modbus.ReadHoldingRegistersAsync(byte.Parse(slaveAddress[7]), ushort.Parse(readingAddresses3[7]), 1);

            machine8Length = Convert.ToInt32(registers3[0]) * 10;

            using (MySqlConnection localDbConnection = new MySqlConnection(pathToDBFile))
            {
                await localDbConnection.OpenAsync();
                MySqlCommand command = new MySqlCommand("SELECT WorkingOn FROM Devices WHERE ID = @ID ", localDbConnection);
                command.Parameters.AddWithValue("@ID", deviceID[7]);
                MySqlDataAdapter dataAdapter = new MySqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                await dataAdapter.FillAsync(dataTable);
                await Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {
                work8.Text = dataTable.Rows[0][0].ToString();
                }));
            }

            await Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {
                length8.Content = machine8Length.ToString() + " m"; ;
                speed8.Content = registers2[0].ToString() + " m/dak";
                if (registers[0].ToString() == "1")
                {
                    online8 = true;
                    status8.Background = Brushes.Green;
                    UpdateDeviceStatus(true, deviceID[7]);
                }
                else
                {
                    online8 = false;
                    status8.Background = Brushes.Red;
                    UpdateDeviceStatus(false, deviceID[7]);
                }
                sendData(7, online8, registers2[0].ToString(), machine8Length.ToString());
            }));
            if (online8 != lastSituation8)
            {
                await saveRunningSituationIntoDatabase(online8, dbNamesRunning[7]);
            }
            lastSituation8 = online8;
        }

        private async Task readDevice7()
        {
            ushort[] registers = await modbus.ReadHoldingRegistersAsync(byte.Parse(slaveAddress[6]), ushort.Parse(readingAddresses1[6]), 1);
            ushort[] registers2 = await modbus.ReadHoldingRegistersAsync(byte.Parse(slaveAddress[6]), ushort.Parse(readingAddresses2[6]), 1);
            ushort[] registers3 = await modbus.ReadHoldingRegistersAsync(byte.Parse(slaveAddress[6]), ushort.Parse(readingAddresses3[6]), 1);

            machine7Length = Convert.ToInt32(registers3[0]) * 10;

            using (MySqlConnection localDbConnection = new MySqlConnection(pathToDBFile))
            {
                await localDbConnection.OpenAsync();
                MySqlCommand command = new MySqlCommand("SELECT WorkingOn FROM Devices WHERE ID = @ID ", localDbConnection);
                command.Parameters.AddWithValue("@ID", deviceID[6]);
                MySqlDataAdapter dataAdapter = new MySqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                await dataAdapter.FillAsync(dataTable);
                await Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {
                work7.Text = dataTable.Rows[0][0].ToString();
                }));
            }

            await Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {
                length7.Content = machine7Length.ToString() + " m"; ;
                speed7.Content = registers2[0].ToString() + " m/dak";
                if (registers[0].ToString() == "1")
                {
                    online7 = true;
                    status7.Background = Brushes.Green;
                    UpdateDeviceStatus(true, deviceID[6]);
                }
                else
                {
                    online7 = false;
                    status7.Background = Brushes.Red;
                    UpdateDeviceStatus(false, deviceID[6]);
                }
                sendData(6, online7, registers2[0].ToString(), machine7Length.ToString());
            }));
            if (online7 != lastSituation7)
            {
                await saveRunningSituationIntoDatabase(online7, dbNamesRunning[6]);
            }
            lastSituation7 = online7;
        }

        private async Task readDevice6()
        {
            ushort[] registers = await modbus.ReadHoldingRegistersAsync(byte.Parse(slaveAddress[5]), ushort.Parse(readingAddresses1[5]), 1);
            ushort[] registers2 = await modbus.ReadHoldingRegistersAsync(byte.Parse(slaveAddress[5]), ushort.Parse(readingAddresses2[5]), 1);
            ushort[] registers3 = await modbus.ReadHoldingRegistersAsync(byte.Parse(slaveAddress[5]), ushort.Parse(readingAddresses3[5]), 1);

            machine6Length = Convert.ToInt32(registers3[0]) * 10;

            using (MySqlConnection localDbConnection = new MySqlConnection(pathToDBFile))
            {
                await localDbConnection.OpenAsync();
                MySqlCommand command = new MySqlCommand("SELECT WorkingOn FROM Devices WHERE ID = @ID ", localDbConnection);
                command.Parameters.AddWithValue("@ID", deviceID[5]);
                MySqlDataAdapter dataAdapter = new MySqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                await dataAdapter.FillAsync(dataTable);
                await Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {
                work6.Text = dataTable.Rows[0][0].ToString();
                }));
            }

            await Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {
                length6.Content = machine6Length.ToString() + " m"; ;
                speed6.Content = registers2[0].ToString() + " m/dak";
                if (registers[0].ToString() == "1")
                {
                    online6 = true;
                    status6.Background = Brushes.Green;
                    UpdateDeviceStatus(true, deviceID[5]);
                }
                else
                {
                    online6 = false;
                    status6.Background = Brushes.Red;
                    UpdateDeviceStatus(false, deviceID[5]);
                }
                sendData(5, online6, registers2[0].ToString(), machine6Length.ToString());
            }));
            if (online6 != lastSituation6)
            {
                await saveRunningSituationIntoDatabase(online6, dbNamesRunning[5]);
            }
            lastSituation6 = online6;
        }

        private async Task readDevice5()
        {
            ushort[] registers = await modbus.ReadHoldingRegistersAsync(byte.Parse(slaveAddress[4]), ushort.Parse(readingAddresses1[4]), 1);
            ushort[] registers2 = await modbus.ReadHoldingRegistersAsync(byte.Parse(slaveAddress[4]), ushort.Parse(readingAddresses2[4]), 1);
            ushort[] registers3 = await modbus.ReadHoldingRegistersAsync(byte.Parse(slaveAddress[4]), ushort.Parse(readingAddresses3[4]), 1);

            machine5Length = Convert.ToInt32(registers3[0]) * 10;

            using (MySqlConnection localDbConnection = new MySqlConnection(pathToDBFile))
            {
                await localDbConnection.OpenAsync();
                MySqlCommand command = new MySqlCommand("SELECT WorkingOn FROM Devices WHERE ID = @ID ", localDbConnection);
                command.Parameters.AddWithValue("@ID", deviceID[4]);
                MySqlDataAdapter dataAdapter = new MySqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                await dataAdapter.FillAsync(dataTable);
                await Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {
                work5.Text = dataTable.Rows[0][0].ToString();
                }));
            }

            await Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {
                length5.Content = machine5Length.ToString() + " m"; ;
                speed5.Content = registers2[0].ToString() + " m/dak";
                if (registers[0].ToString() == "1")
                {
                    online5 = true;
                    status5.Background = Brushes.Green;
                    UpdateDeviceStatus(true, deviceID[4]);
                }
                else
                {
                    online5 = false;
                    status5.Background = Brushes.Red;
                    UpdateDeviceStatus(false, deviceID[4]);
                }
                sendData(4, online5, registers2[0].ToString(), machine5Length.ToString());
            }));
            if (online5 != lastSituation5)
            {
                await saveRunningSituationIntoDatabase(online5, dbNamesRunning[4]);
            }
            lastSituation5 = online5;
        }

        private async Task readDevice4()
        {
            ushort[] registers = await modbus.ReadHoldingRegistersAsync(byte.Parse(slaveAddress[3]), ushort.Parse(readingAddresses1[3]), 1);
            ushort[] registers2 = await modbus.ReadHoldingRegistersAsync(byte.Parse(slaveAddress[3]), ushort.Parse(readingAddresses2[3]), 1);
            ushort[] registers3 = await modbus.ReadHoldingRegistersAsync(byte.Parse(slaveAddress[3]), ushort.Parse(readingAddresses3[3]), 1);

            machine4Length = Convert.ToInt32(registers3[0]) * 10;

            using (MySqlConnection localDbConnection = new MySqlConnection(pathToDBFile))
            {
                await localDbConnection.OpenAsync();
                MySqlCommand command = new MySqlCommand("SELECT WorkingOn FROM Devices WHERE ID = @ID ", localDbConnection);
                command.Parameters.AddWithValue("@ID", deviceID[3]);
                MySqlDataAdapter dataAdapter = new MySqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                await dataAdapter.FillAsync(dataTable);
                await Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {
                work4.Text = dataTable.Rows[0][0].ToString();
                }));
            }

            await Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {
                length4.Content = machine4Length.ToString() + " m"; ;
                speed4.Content = registers2[0].ToString() + " m/dak";
                if (registers[0].ToString() == "1")
                {
                    online4 = true;
                    status4.Background = Brushes.Green;
                    UpdateDeviceStatus(true, deviceID[3]);
                }
                else
                {
                    online4 = false;
                    status4.Background = Brushes.Red;
                    UpdateDeviceStatus(false, deviceID[3]);
                }
                sendData(3, online4, registers2[0].ToString(), machine4Length.ToString());
            }));
            if (online4 != lastSituation4)
            {
                await saveRunningSituationIntoDatabase(online4, dbNamesRunning[3]);
            }
            lastSituation4 = online4;
        }

        private async Task readDevice3()
        {
            ushort[] registers = await modbus.ReadHoldingRegistersAsync(byte.Parse(slaveAddress[2]), ushort.Parse(readingAddresses1[2]), 1);
            ushort[] registers2 = await modbus.ReadHoldingRegistersAsync(byte.Parse(slaveAddress[2]), ushort.Parse(readingAddresses2[2]), 1);
            ushort[] registers3 = await modbus.ReadHoldingRegistersAsync(byte.Parse(slaveAddress[2]), ushort.Parse(readingAddresses3[2]), 1);

            machine3Length = Convert.ToInt32(registers3[0]) * 10;

            using (MySqlConnection localDbConnection = new MySqlConnection(pathToDBFile))
            {
                await localDbConnection.OpenAsync();
                MySqlCommand command = new MySqlCommand("SELECT WorkingOn FROM Devices WHERE ID = @ID ", localDbConnection);
                command.Parameters.AddWithValue("@ID", deviceID[2]);
                MySqlDataAdapter dataAdapter = new MySqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                await dataAdapter.FillAsync(dataTable);
                await Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {
                work3.Text = dataTable.Rows[0][0].ToString();
                }));
            }

            await Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {
                length3.Content = machine3Length.ToString() + " m"; ;
                speed3.Content = registers2[0].ToString() + " m/dak";
                if (registers[0].ToString() == "1")
                {
                    online3 = true;
                    status3.Background = Brushes.Green;
                    UpdateDeviceStatus(true, deviceID[2]);
                }
                else
                {
                    online3 = false;
                    status3.Background = Brushes.Red;
                    UpdateDeviceStatus(false, deviceID[2]);
                }
                sendData(2, online3, registers2[0].ToString(), machine3Length.ToString());
            }));
            if (online3 != lastSituation3)
            {
                await saveRunningSituationIntoDatabase(online3, dbNamesRunning[2]);
            }
            lastSituation3 = online3;
        }

        private async Task readDevice2()
        {
            ushort[] registers = await modbus.ReadHoldingRegistersAsync(byte.Parse(slaveAddress[1]), ushort.Parse(readingAddresses1[1]), 1);
            ushort[] registers2 = await modbus.ReadHoldingRegistersAsync(byte.Parse(slaveAddress[1]), ushort.Parse(readingAddresses2[1]), 1);
            ushort[] registers3 = await modbus.ReadHoldingRegistersAsync(byte.Parse(slaveAddress[1]), ushort.Parse(readingAddresses3[1]), 1);

            machine2Length = Convert.ToInt32(registers3[0]) * 10;

            using (MySqlConnection localDbConnection = new MySqlConnection(pathToDBFile))
            {
                await localDbConnection.OpenAsync();
                MySqlCommand command = new MySqlCommand("SELECT WorkingOn FROM Devices WHERE ID = @ID ", localDbConnection);
                command.Parameters.AddWithValue("@ID", deviceID[1]);
                MySqlDataAdapter dataAdapter = new MySqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                await dataAdapter.FillAsync(dataTable);
                await Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {
                work2.Text = dataTable.Rows[0][0].ToString();
                }));
            }

            await Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {
                length2.Content = machine2Length.ToString() + " m"; ;
                speed2.Content = registers2[0].ToString() + " m/dak";
                if (registers[0].ToString().Trim() == "1")
                {
                    online2 = true;
                    status2.Background = Brushes.Green;
                    UpdateDeviceStatus(true, deviceID[1]);
                }
                else
                {
                    online2 = false;
                    status2.Background = Brushes.Red;
                    UpdateDeviceStatus(false, deviceID[1]);
                }
                sendData(1, online2, registers2[0].ToString(), machine2Length.ToString());
            }));
            if (online2 != lastSituation2)
            {
                await saveRunningSituationIntoDatabase(online2, dbNamesRunning[1]);
            }
            lastSituation2 = online2;
        }

        private async Task readDevice1()
        {
            ushort[] registers = await modbus.ReadHoldingRegistersAsync(byte.Parse(slaveAddress[0]), ushort.Parse(readingAddresses1[0]), 1);
            ushort[] registers2 = await modbus.ReadHoldingRegistersAsync(byte.Parse(slaveAddress[0]), ushort.Parse(readingAddresses2[0]), 1);
            ushort[] registers3 = await modbus.ReadHoldingRegistersAsync(byte.Parse(slaveAddress[0]), ushort.Parse(readingAddresses3[0]), 1);

            machine1Length = Convert.ToInt32(registers3[0]) * 10;

            using (MySqlConnection localDbConnection = new MySqlConnection(pathToDBFile))
            {
                try
                {
                    await localDbConnection.OpenAsync();
                    MySqlCommand command = new MySqlCommand("SELECT WorkingOn FROM Devices WHERE ID = @ID", localDbConnection);
                    command.Parameters.AddWithValue("@ID", deviceID[0]);
                    MySqlDataAdapter dataAdapter = new MySqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    await dataAdapter.FillAsync(dataTable);
                    await Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {
                        work1.Text = dataTable.Rows[0][0].ToString();
                    }));
                }
                catch (Exception ex)
                {
                    var a = ex.Message;
                }
            }

            await Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {
                length1.Content = machine1Length.ToString() + " m";
                speed1.Content = registers2[0].ToString() + " m/dak";
                if (registers[0].ToString() == "1")
                {
                    online1 = true;
                    status1.Background = Brushes.Green;
                    UpdateDeviceStatus(true, deviceID[0]);
                }
                else
                {
                    online1 = false;
                    status1.Background = Brushes.Red;
                    UpdateDeviceStatus(false, deviceID[0]);
                }
                sendData(0, online1, registers2[0].ToString(), machine1Length.ToString());
            }));
            if (online1 != lastSituation1)
            {
                await saveRunningSituationIntoDatabase(online1, dbNamesRunning[0]);
            }
            lastSituation1 = online1;
        }

        private void UpdateDeviceStatus(bool status, string id)
        {
            using (MySqlConnection connection = new MySqlConnection(pathToDBFile))
            {
                string query = "UPDATE Devices SET Status = @Status WHERE ID = @ID";
                MySqlCommand commandToUpdateStatus = new MySqlCommand(query, connection);
                commandToUpdateStatus.Parameters.AddWithValue("@Status", status);
                commandToUpdateStatus.Parameters.AddWithValue("@ID", id);
                commandToUpdateStatus.ExecuteNonQuery();
            }
        }

        private async Task saveRunningSituationIntoDatabase(bool situation, string dbName)
        {
            using (MySqlConnection localDbConnection = new MySqlConnection(pathToDBFile))
            {
                await localDbConnection.OpenAsync();

                MySqlCommand commandDevice = new MySqlCommand(string.Format("INSERT INTO {0}(ChangeTime, Situation, Dates, Day) " +
                            "VALUES(@ChangeTime, @Situation, @Dates, @Day)", dbName), localDbConnection);

                commandDevice.Parameters.AddWithValue("@ChangeTime", DateTime.Now.TimeOfDay);
                commandDevice.Parameters.AddWithValue("@Situation", situation);
                commandDevice.Parameters.AddWithValue("@Dates", DateTime.Now.Date);
                commandDevice.Parameters.AddWithValue("@Day", DateTime.Now.DayOfWeek);
                await commandDevice.ExecuteNonQueryAsync();
                commandDevice.Parameters.Clear();
            }
        }

        private void sendData(int number, bool status, string speed, string length)
        {
            switch (number)
            {
                case 0:
                    eventArgs1.sendEventInfo(status, speed, length);
                    break;
                case 1:
                    eventArgs2.sendEventInfo(status, speed, length);
                    break;
                case 2:
                    eventArgs3.sendEventInfo(status, speed, length);
                    break;
                case 3:
                    eventArgs4.sendEventInfo(status, speed, length);
                    break;
                case 4:
                    eventArgs5.sendEventInfo(status, speed, length);
                    break;
                case 5:
                    eventArgs6.sendEventInfo(status, speed, length);
                    break;
                case 6:
                    eventArgs7.sendEventInfo(status, speed, length);
                    break;
                case 7:
                    eventArgs8.sendEventInfo(status, speed, length);
                    break;
                case 8:
                    eventArgs9.sendEventInfo(status, speed, length);
                    break;
                case 9:
                    eventArgs10.sendEventInfo(status, speed, length);
                    break;
            }
        }

        private void device1_Click(object sender, MouseButtonEventArgs e)
        {
            openDevicesShow(0, deviceID[0], dbNames[0], dbNamesRunning[0], slaveAddress[0], readingAddresses1[0], readingAddresses2[0], readingAddresses3[0], readingAddresses4[0], readingAddresses5[0]);
        }

        private void device2_Click(object sender, MouseButtonEventArgs e)
        {
            openDevicesShow(1, deviceID[1], dbNames[1], dbNamesRunning[1], slaveAddress[1], readingAddresses1[1], readingAddresses2[1], readingAddresses3[1], readingAddresses4[1], readingAddresses5[1]);
        }

        private void device3_Click(object sender, MouseButtonEventArgs e)
        {
            openDevicesShow(2, deviceID[2], dbNames[2], dbNamesRunning[2], slaveAddress[2], readingAddresses1[2], readingAddresses2[2], readingAddresses3[2], readingAddresses4[2], readingAddresses5[2]);
        }

        private void device4_Click(object sender, MouseButtonEventArgs e)
        {
            openDevicesShow(3, deviceID[3], dbNames[3], dbNamesRunning[3], slaveAddress[3], readingAddresses1[3], readingAddresses2[3], readingAddresses3[3], readingAddresses4[3], readingAddresses5[3]);
        }

        private void device5_Click(object sender, MouseButtonEventArgs e)
        {
            openDevicesShow(4, deviceID[4], dbNames[4], dbNamesRunning[4], slaveAddress[4], readingAddresses1[4], readingAddresses2[4], readingAddresses3[4], readingAddresses4[4], readingAddresses5[4]);
        }

        private void device6_Click(object sender, MouseButtonEventArgs e)
        {
            openDevicesShow(5, deviceID[5], dbNames[5], dbNamesRunning[5], slaveAddress[5], readingAddresses1[5], readingAddresses2[5], readingAddresses3[5], readingAddresses4[5], readingAddresses5[5]);
        }

        private void device7_Click(object sender, MouseButtonEventArgs e)
        {
            openDevicesShow(6, deviceID[6], dbNames[6], dbNamesRunning[6], slaveAddress[6], readingAddresses1[6], readingAddresses2[6], readingAddresses3[6], readingAddresses4[6], readingAddresses5[6]);
        }

        private void device8_Click(object sender, MouseButtonEventArgs e)
        {
            openDevicesShow(7, deviceID[7], dbNames[7], dbNamesRunning[7], slaveAddress[7], readingAddresses1[7], readingAddresses2[7], readingAddresses3[7], readingAddresses4[7], readingAddresses5[7]);
        }

        private void device9_Click(object sender, MouseButtonEventArgs e)
        {
            openDevicesShow(8, deviceID[8], dbNames[8], dbNamesRunning[8], slaveAddress[8], readingAddresses1[8], readingAddresses2[8], readingAddresses3[8], readingAddresses4[8], readingAddresses5[8]);
        }

        private void device10_Click(object sender, MouseButtonEventArgs e)
        {
            openDevicesShow(9, deviceID[9], dbNames[9], dbNamesRunning[9], slaveAddress[9], readingAddresses1[9], readingAddresses2[9], readingAddresses3[9], readingAddresses4[9], readingAddresses5[9]);
        }

        private void work1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            openWorkSetter(deviceID[0], machine1Length, dbNamesWorkingOn[0]);
        }

        private void work2_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            openWorkSetter(deviceID[1], machine2Length, dbNamesWorkingOn[1]);
        }

        private void work3_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            openWorkSetter(deviceID[2], machine3Length, dbNamesWorkingOn[2]);
        }

        private void work4_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            openWorkSetter(deviceID[3], machine4Length, dbNamesWorkingOn[3]);
        }

        private void work5_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            openWorkSetter(deviceID[4], machine5Length, dbNamesWorkingOn[4]);
        }

        private void work6_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            openWorkSetter(deviceID[5], machine6Length, dbNamesWorkingOn[5]);
        }

        private void work7_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            openWorkSetter(deviceID[6], machine7Length, dbNamesWorkingOn[6]);
        }

        private void work8_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            openWorkSetter(deviceID[7], machine8Length, dbNamesWorkingOn[7]);
        }

        private void work9_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            openWorkSetter(deviceID[8], machine9Length, dbNamesWorkingOn[8]);
        }

        private void work10_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            openWorkSetter(deviceID[9], machine10Length, dbNamesWorkingOn[9]);
        }

        private void openWorkSetter(string id, int machineLength, string dbWorkingOn)
        {
            WorkSetter workSetter = new WorkSetter(id, machineLength, dbWorkingOn);
            workSetter.ShowDialog();
        }

        private void DevicesSettings1(object sender, RoutedEventArgs e)
        {
            openSettings(deviceID[0], operatorNames[0].Text);
        }

        private void DevicesSettings2(object sender, RoutedEventArgs e)
        {
            openSettings(deviceID[1], operatorNames[1].Text);
        }

        private void DevicesSettings3(object sender, RoutedEventArgs e)
        {
            openSettings(deviceID[2], operatorNames[2].Text);
        }

        private void DevicesSettings4(object sender, RoutedEventArgs e)
        {
            openSettings(deviceID[3], operatorNames[3].Text);
        }

        private void DevicesSettings5(object sender, RoutedEventArgs e)
        {
            openSettings(deviceID[4], operatorNames[4].Text);
        }

        private void DevicesSettings6(object sender, RoutedEventArgs e)
        {
            openSettings(deviceID[5], operatorNames[5].Text);
        }

        private void DevicesSettings7(object sender, RoutedEventArgs e)
        {
            openSettings(deviceID[6], operatorNames[6].Text);
        }

        private void DevicesSettings8(object sender, RoutedEventArgs e)
        {
            openSettings(deviceID[7], operatorNames[7].Text);
        }

        private void DevicesSettings9(object sender, RoutedEventArgs e)
        {
            openSettings(deviceID[8], operatorNames[8].Text);
        }

        private void DevicesSettings10(object sender, RoutedEventArgs e)
        {
            openSettings(deviceID[9], operatorNames[9].Text);
        }

        private void openSettings(string id, string operatorNameReceived)
        {
            if (Variables.isAdmin)
            {
                DevicesSettingsAdmin deviceSettings = new DevicesSettingsAdmin(id, operatorNameReceived);
                deviceSettings.Closed += Closed;
                deviceSettings.Show();
            }
            else
            {
                DeviceSettings deviceSettings = new DeviceSettings(id, operatorNameReceived);
                deviceSettings.Closed += Closed;
                deviceSettings.Show();
            }
        }

        private void openDevicesShow(int number, string id, string dbName, string dbNameRunning, string slave, string reading1, string reading2, string reading3, string reading4, string reading5)
        {
            DevicesShow devicesShow = new DevicesShow(number, id, dbName, dbNameRunning, slave, reading1, reading2, reading3, reading4, reading5);
            devicesShow.Show();
        }

        private void SaveToDatabase()
        {
            Write.ToFile("Retrieving required information from database.");
            getDatabaseRunningName();
            Write.ToFile("All required information is retrieved");
            Write.ToFile("Saving device length data");
            FinishSaving();
            Write.ToFile("All device length data is saved");
            Write.ToFile("PLC reset began.");
            ResetPLC();
            Write.ToFile("PLC is reset.");
            Write.ToFile("Attempting to change the saved time.");
            File.WriteAllText(Properties.Resources.LastSavedFilePath, DateTime.Now.ToString());
            Write.ToFile("Changed the last saved time.");
            Write.ToFile("All Data is successfully saved.");
        }

        private void getDatabaseRunningName()
        {
            using(MySqlConnection connection = new MySqlConnection(Properties.Resources.ConnectionString))
            {
                try
                {
                    connection.Open();
                    MySqlCommand command = new MySqlCommand("SELECT * FROM Devices ORDER BY ID ASC", connection);
                    MySqlDataAdapter dataAdapter = new MySqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    dataAdapter.Fill(dataTable);

                    dbNames.Clear();
                    dbNamesRunning.Clear();
                    slaveAddress.Clear();
                    readingAddresses.Clear();

                    for (int i = 0; i < dataTable.Rows.Count; i++)
                    {
                        dbNames.Add(dataTable.Rows[i][4].ToString());
                        dbNamesRunning.Add(dataTable.Rows[i][11].ToString());
                        readingAddresses.Add(dataTable.Rows[i][8].ToString());
                        slaveAddress.Add(dataTable.Rows[i][5].ToString());
                    }
                }
                catch (Exception ex)
                {
                    Write.ToFile(ex.Message);
                }
            }
        }


        private void FinishSaving()
        {
            int[] lengths =
            {
                machine1Length, machine2Length, machine3Length, machine4Length, machine5Length, machine6Length, machine7Length,
                machine8Length, machine9Length, machine10Length
            };

            for (int i = 0; i < dbNames.Count; i++)
            {
                using (MySqlConnection connection = new MySqlConnection(Properties.Resources.ConnectionString))
                {
                    connection.Open();

                    if (dbNames[i] != null)
                    {
                        MySqlCommand commandDevice = new MySqlCommand(String.Format("INSERT INTO {0}(Value1, Value2, Value3, Value4, Value5, Date, Day, Worked) " +
                               "VALUES(@Value1, @Value2, @Value3, @Value4, @Value5, @Date, @Day, @Worked)", dbNames[i]), connection);

                        commandDevice.Parameters.AddWithValue("@Value1", lengths[i]);
                        commandDevice.Parameters.AddWithValue("@Value2", lengths[i]);
                        commandDevice.Parameters.AddWithValue("@Value3", lengths[i]);
                        commandDevice.Parameters.AddWithValue("@Value4", lengths[i]);
                        commandDevice.Parameters.AddWithValue("@Value5", lengths[i]);
                        commandDevice.Parameters.AddWithValue("@Day", DateTime.Now.DayOfWeek);
                        commandDevice.Parameters.AddWithValue("@Date", DateTime.Now);
                        commandDevice.Parameters.AddWithValue("@Worked", GetStartedTime(dbNamesRunning[i]));

                        commandDevice.ExecuteNonQuery();
                    }
                }
            }
        }

        private object GetStartedTime(string dbName)
        {
            using (MySqlConnection localDbConnection = new MySqlConnection(Properties.Resources.ConnectionString))
            {
                localDbConnection.Open();
                string query = string.Format("SELECT ChangeTime FROM {0} WHERE (Dates >= @Dates) AND (Situation = 1) ORDER BY ID ASC", dbName);
                MySqlCommand command = new MySqlCommand(query, localDbConnection);
                command.Parameters.AddWithValue("@Dates", DateTime.Now.AddDays(-1));
                MySqlDataAdapter dataAdapter = new MySqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                dataAdapter.Fill(dataTable);

                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    ones.Add(Convert.ToDateTime(dataTable.Rows[i][0].ToString()));
                    //timeFirst = Convert.ToDateTime(dataTable.Rows[i][0].ToString());
                }

                string query2 = string.Format("SELECT ChangeTime FROM {0} WHERE (Dates >= @Dates) AND (Situation = 0) ORDER BY ID ASC", dbName);
                MySqlCommand command2 = new MySqlCommand(query2, localDbConnection);
                command2.Parameters.AddWithValue("@Dates", DateTime.Now.AddDays(-1));

                MySqlDataAdapter dataAdapter2 = new MySqlDataAdapter(command2);
                DataTable dataTable2 = new DataTable();
                dataAdapter2.Fill(dataTable2);


                for (int i = 0; i < dataTable2.Rows.Count; i++)
                {
                    zeros.Add(Convert.ToDateTime(dataTable2.Rows[i][0].ToString()));
                    //timeLast = Convert.ToDateTime(dataTable2.Rows[i][0].ToString());
                }

                int counter = ones.Count;

                if (ones.Count > zeros.Count)
                {
                    counter = zeros.Count;
                }

                TimeSpan difference = TimeSpan.Zero;

                for (int i = 0; i < counter; i++)
                {
                    if (zeros[0] > ones[0])
                    {
                        difference += (zeros[i] - ones[i]);
                    }
                    else
                    {
                        try
                        {
                            difference += (zeros[i + 1] - ones[i]);
                        }
                        catch { }
                    }
                }

                return difference.TotalHours;
                //return Math.Abs(Convert.ToInt32(difference.TotalHours));
            }
        }

        private void ResetPLC()
        {
            using (MySqlConnection localDbConnection = new MySqlConnection(Properties.Resources.ConnectionString))
            {
                MySqlCommand command = new MySqlCommand("SELECT SlaveID, Address4 FROM Devices", localDbConnection);

                MySqlDataAdapter dataAdapter = new MySqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                dataAdapter.Fill(dataTable);

                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    modbus.WriteSingleRegister(byte.Parse(dataTable.Rows[i][0].ToString()), ushort.Parse(dataTable.Rows[i][1].ToString()), 1);
                    Thread.Sleep(TimeSpan.FromMilliseconds(1000));
                    modbus.WriteSingleRegister(byte.Parse(dataTable.Rows[i][0].ToString()), ushort.Parse(dataTable.Rows[i][1].ToString()), 0);
                    Thread.Sleep(TimeSpan.FromMilliseconds(1000));
                }
            }
        }
    }
}
