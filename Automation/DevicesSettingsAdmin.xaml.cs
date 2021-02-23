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
    /// Interaction logic for DevicesSettingsAdmin.xaml
    /// </summary>
    public partial class DevicesSettingsAdmin : Window
    {
        Assembly assembly;
        ResourceManager rm;
        CultureInfo cultureInfo;
        SnackbarMessageQueue myMessageQueue = new SnackbarMessageQueue(TimeSpan.FromSeconds(2));
        string pathToDBFile = Properties.Resources.ConnectionString;
        string ID;

        public DevicesSettingsAdmin(string idReceived, string operatorNameReceived)
        {
            InitializeComponent();
            Snackbar.MessageQueue = myMessageQueue;
            assembly = Assembly.Load("Automation");
            rm = new ResourceManager("Automation.Languages.language", assembly);
            cultureInfo = new CultureInfo(Properties.Settings.Default.language);
            ID = idReceived;
            init(operatorNameReceived);
        }

        private void init(string operatorNameReceived)
        {
            using (MySqlConnection localDbConnection = new MySqlConnection(pathToDBFile))
            {
                localDbConnection.Open();

                try
                {
                    MySqlCommand command = new MySqlCommand("SELECT * FROM Devices WHERE ID = @ID", localDbConnection);
                    command.Parameters.AddWithValue("@ID", ID);
                    MySqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        deviceName.Text = reader.GetString(1);
                        slaveId.Text = reader.GetInt16(5).ToString();
                        readingAdress1.Text = reader.GetInt32(6).ToString();
                        readingAdress2.Text = reader.GetInt32(7).ToString();
                        readingAdress3.Text = reader.GetInt32(8).ToString();
                        readingAdress4.Text = reader.GetInt32(9).ToString();
                        readingAdress5.Text = reader.GetInt32(10).ToString();
                        fillComboBoxes(operatorNameReceived);
                    }
                    else
                    {
                        MessageBox.Show("Cihaz Bilgilerine Erişilemedi!");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void fillComboBoxes(string operatorNameReceived)
        {
            using (MySqlConnection localDbConnection = new MySqlConnection(pathToDBFile))
            {
                localDbConnection.Open();

                string query = "SELECT Operator FROM Operators";
                MySqlCommand command = new MySqlCommand(query, localDbConnection);
                MySqlDataAdapter dataAdapter = new MySqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                dataAdapter.Fill(dataTable);

                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    operatorName.Items.Add(dataTable.Rows[i][0]);
                }

                if (operatorName.Items.Count > 0)
                {
                    operatorName.SelectedIndex = 0;
                }
            }
        }

        private void update_Click(object sender, RoutedEventArgs e)
        {
            updateDevice();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private new void KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                updateDevice();
            }
        }

        private void updateDevice()
        {
            if (slaveId.Text.Trim() == "" || deviceName.Text.Trim() == "" || readingAdress1.Text.Trim() == "" || readingAdress2.Text.Trim() == ""
                || readingAdress3.Text.Trim() == "")
            {
                myMessageQueue.Enqueue(rm.GetString("fillBlanks", cultureInfo), rm.GetString("ok", cultureInfo), () => HandleOKMethod());
                return;
            }

            using (MySqlConnection localDbConnection = new MySqlConnection(pathToDBFile))
            {
                localDbConnection.Open();

                try
                {
                    MySqlCommand command = new MySqlCommand("UPDATE Devices SET Device = @Device, SlaveID = @SlaveID, Address1 = @Address1," +
                        "Address2 = @Address2, Address3 = @Address3, Address4 = @Address4, Address5 = @Address5 " +
                        "WHERE ID = @ID", localDbConnection);
                    command.Parameters.AddWithValue("@ID", ID);
                    command.Parameters.AddWithValue("@Device", deviceName.Text.Trim());
                    command.Parameters.AddWithValue("@SlaveID", slaveId.Text.Trim());
                    command.Parameters.AddWithValue("@Address1", readingAdress1.Text.Trim());
                    command.Parameters.AddWithValue("@Address2", readingAdress2.Text.Trim());
                    command.Parameters.AddWithValue("@Address3", readingAdress3.Text.Trim());
                    command.Parameters.AddWithValue("@Address4", readingAdress4.Text.Trim());
                    command.Parameters.AddWithValue("@Address5", readingAdress5.Text.Trim());
                    command.ExecuteNonQuery();
                    MessageBox.Show(rm.GetString("deviceInformationUpdated"), rm.GetString("system"),
                    MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, rm.GetString("system"),
                   MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void HandleOKMethod()
        {

        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
