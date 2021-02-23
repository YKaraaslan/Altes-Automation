using MaterialDesignThemes.Wpf;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace Automation
{
    /// <summary>
    /// Interaction logic for DevicesAdd.xaml
    /// </summary>
    public partial class DevicesAdd : Window
    {
        Assembly assembly;
        ResourceManager rm;
        CultureInfo cultureInfo;
        SnackbarMessageQueue myMessageQueue = new SnackbarMessageQueue(TimeSpan.FromSeconds(2));
        string pathToDBFile = Properties.Resources.ConnectionString;
        string dbNameForDevice = null, dbNameRunningForDevice = null, dbNameWorkingOnForDevice = null;

        public DevicesAdd()
        {
            InitializeComponent();
            Snackbar.MessageQueue = myMessageQueue;
            assembly = Assembly.Load("Automation");
            rm = new ResourceManager("Automation.Languages.language", assembly);
            cultureInfo = new CultureInfo(Properties.Settings.Default.language);
            fillComboBoxes();
        }

        private void fillComboBoxes()
        {
            using (MySqlConnection localDbConnection = new MySqlConnection(pathToDBFile))
            {
                localDbConnection.Open();

                string query = "SELECT Operator FROM Operators";
                MySqlCommand command = new MySqlCommand(query, localDbConnection);
                MySqlDataAdapter dataAdapter = new MySqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                dataAdapter.Fill(dataTable);

                for (int i =0; i < dataTable.Rows.Count; i++)
                {
                    operatorName.Items.Add(dataTable.Rows[i][0]);
                }

                if (operatorName.Items.Count > 0)
                {
                    operatorName.SelectedIndex = 0;
                }
            }
        }

        private void add_Click(object sender, RoutedEventArgs e)
        {
            addDevice();
        }

        private void addDevice()
        {
            if (slaveId.Text.Trim() == "" || deviceName.Text.Trim() == "" || readingAdress1.Text.Trim() == "" || readingAdress2.Text.Trim() == ""
                || readingAdress3.Text.Trim() == "")
            {
                myMessageQueue.Enqueue(rm.GetString("fillBlanks", cultureInfo), rm.GetString("ok", cultureInfo), () => HandleOKMethod());
                return;
            }

            if(operatorName.Text.Trim() == "")
            {
                MessageBox.Show(rm.GetString("operatorEmpty", cultureInfo), rm.GetString("system", cultureInfo),
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            if (!checkedDeviceAmount())
            {
                MessageBox.Show("Maksimum cihaz sayısına ulaşılmıştır.", rm.GetString("system"),
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            using (MySqlConnection localDbConnection = new MySqlConnection(pathToDBFile))
            {
                localDbConnection.Open();

                if (!createDbForDevice())
                {
                    return;
                }

                try
                {
                    MySqlCommand commandToAddDatabase = new MySqlCommand("INSERT INTO Devices(Device, Operator, DbName, DbNameRunning, SlaveID, Address1, Address2, Address3, Address4, Address5, WorkingOn, WorkingOnDb) " +
                        "VALUES(@Device, @Operator, @DbName, @DbNameRunning, @SlaveID, @Address1, @Address2, @Address3, @Address4, @Address5, @WorkingOn, @WorkingOnDb)", localDbConnection);
                    commandToAddDatabase.Parameters.AddWithValue("@Device", deviceName.Text.Trim());
                    commandToAddDatabase.Parameters.AddWithValue("@Operator", operatorName.Text.Trim());
                    commandToAddDatabase.Parameters.AddWithValue("@DbName", dbNameForDevice);
                    commandToAddDatabase.Parameters.AddWithValue("@DbNameRunning", dbNameRunningForDevice);
                    commandToAddDatabase.Parameters.AddWithValue("@SlaveID", slaveId.Text.Trim());
                    commandToAddDatabase.Parameters.AddWithValue("@Address1", readingAdress1.Text.Trim());
                    commandToAddDatabase.Parameters.AddWithValue("@Address2", readingAdress2.Text.Trim());
                    commandToAddDatabase.Parameters.AddWithValue("@Address3", readingAdress3.Text.Trim());
                    commandToAddDatabase.Parameters.AddWithValue("@Address4", readingAdress4.Text.Trim());
                    commandToAddDatabase.Parameters.AddWithValue("@Address5", readingAdress5.Text.Trim());
                    commandToAddDatabase.Parameters.AddWithValue("@WorkingOn", "----------");
                    commandToAddDatabase.Parameters.AddWithValue("@WorkingOnDb", dbNameWorkingOnForDevice);
                    commandToAddDatabase.ExecuteNonQuery();
                    MessageBox.Show(rm.GetString("deviceSaved"), rm.GetString("system"),
                    MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, rm.GetString("system"),
                    MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private bool checkedDeviceAmount()
        {
            using (MySqlConnection localDbConnection = new MySqlConnection(pathToDBFile))
            {
                localDbConnection.Open();

                string query = "SELECT Count(*) FROM Devices";
                MySqlCommand command = new MySqlCommand(query, localDbConnection);
                int number = int.Parse(command.ExecuteScalar().ToString());

                if (number > 9)
                {
                    return false;
                }
                return true;
            }
        }

        private bool createDbForDevice()
        {
            dbNameForDevice = generateDatabaseName() + "d";
            dbNameRunningForDevice = generateDatabaseName() + "r";
            dbNameWorkingOnForDevice = generateDatabaseName() + "w";
            using (MySqlConnection localDbConnection = new MySqlConnection(pathToDBFile))
            {
                localDbConnection.Open();
                string createTableApplication = string.Format("CREATE TABLE `{0}`(ID INT NOT NULL AUTO_INCREMENT," +
                   "Value1 INT, Value2 INT, Value3 INT, Value4 INT, Value5 INT, Date DateTime, Day VARCHAR(15), Worked INT, PRIMARY KEY (ID))", dbNameForDevice);
                MySqlCommand CommandCreateTableApplication = new MySqlCommand(createTableApplication, localDbConnection);

                string createTable = string.Format("CREATE TABLE `{0}`(ID INT NOT NULL AUTO_INCREMENT," +
                   "ChangeTime Time, Situation Tinyint, Dates DateTime, Day VARCHAR(15), PRIMARY KEY (ID))", dbNameRunningForDevice);
                MySqlCommand CommandCreateTable = new MySqlCommand(createTable, localDbConnection);

                string query = string.Format("CREATE TABLE `{0}`(ID INT NOT NULL AUTO_INCREMENT," +
                   "OperatorName VARCHAR(50), WorkingOn VARCHAR(500), Length INT, " +
                   "DateWork DateTime, PRIMARY KEY (ID))", dbNameWorkingOnForDevice);
                MySqlCommand command = new MySqlCommand(query, localDbConnection);

                try
                {
                    CommandCreateTableApplication.ExecuteNonQuery();
                    CommandCreateTable.ExecuteNonQuery();
                    command.ExecuteNonQuery();
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, rm.GetString("system"),
                    MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }
        }

        private void HandleOKMethod()
        {

        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private new void KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                addDevice();
            }
        }

        private string generateDatabaseName()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            var stringChars = new char[10];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            return new String(stringChars);
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}