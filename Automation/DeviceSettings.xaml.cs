using MaterialDesignThemes.Wpf;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Windows;
using System.Windows.Input;

namespace Automation
{
    /// <summary>
    /// Interaction logic for DeviceSettings.xaml
    /// </summary>
    public partial class DeviceSettings : Window
    {
        Assembly assembly;
        ResourceManager rm;
        CultureInfo cultureInfo;
        SnackbarMessageQueue myMessageQueue = new SnackbarMessageQueue(TimeSpan.FromSeconds(2));
        string ID;
        string pathToDBFile = Properties.Resources.ConnectionString;

        public DeviceSettings(string deviceID, string operatorNameReceived)
        {
            InitializeComponent();
            Snackbar.MessageQueue = myMessageQueue;
            assembly = Assembly.Load("Automation");
            rm = new ResourceManager("Automation.Languages.language", assembly);
            cultureInfo = new CultureInfo(Properties.Settings.Default.language);
            ID = deviceID;
            init(operatorNameReceived);
        }

        private void init(string operatorNameReceived)
        {
            using (MySqlConnection localDbConnection = new MySqlConnection(pathToDBFile))
            {
                localDbConnection.Open();
                MySqlCommand commandOperators = new MySqlCommand("SELECT Device FROM Devices WHERE ID = @ID", localDbConnection);
                commandOperators.Parameters.AddWithValue("@ID", ID);
                MySqlDataAdapter dataAdapterOperators = new MySqlDataAdapter(commandOperators);
                DataTable dataTableOperators = new DataTable();
                dataAdapterOperators.Fill(dataTableOperators);
                deviceName.Text = dataTableOperators.Rows[0][0].ToString();
                fillComboBox(operatorNameReceived);
            }        
        }

        private void fillComboBox(string operatorNameReceived)
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
                    operatorName.SelectedItem = operatorNameReceived;
                }
            }
        }

        private void save(object sender, RoutedEventArgs e)
        {
            update();
        }

        private void deviceName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                update();
            }
        }

        private void update()
        {
            if (deviceName.Text.Trim() == "" || operatorName.Text.Trim() == "")
            {
                myMessageQueue.Enqueue(rm.GetString("fillBlanks"), rm.GetString("ok"), () => HandleOKMethod());
                return;
            }

            using (MySqlConnection localDbConnection = new MySqlConnection(pathToDBFile))
            {
                localDbConnection.Open();
                string query = "UPDATE Devices SET Device = @Device, Operator = @Operator WHERE ID = @ID";
                MySqlCommand commandDevices = new MySqlCommand(query, localDbConnection);
                commandDevices.Parameters.AddWithValue("@ID", ID);
                commandDevices.Parameters.AddWithValue("@Device", deviceName.Text.Trim());
                commandDevices.Parameters.AddWithValue("@Operator", operatorName.Text.Trim());

                try
                {
                    commandDevices.ExecuteNonQuery();
                    MessageBox.Show(rm.GetString("deviceInformationUpdated", cultureInfo), rm.GetString("system", cultureInfo),
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

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void HandleOKMethod()
        {

        }

        private void delete(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(rm.GetString("areYouSureToDelete", cultureInfo), rm.GetString("system", cultureInfo),
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                deleteSelectedRow();
            }
        }

        private void deleteSelectedRow()
        {
            DevicesAdminConfirmation adminConfirmation = new DevicesAdminConfirmation(ID);
            adminConfirmation.Closed += ClosedWindow;
            adminConfirmation.ShowDialog();
        }

        private void ClosedWindow(object sender, EventArgs e)
        {
            if (Variables.saved)
            {
                Variables.saved = false;
                this.Close();
            }
        }
    }
}
