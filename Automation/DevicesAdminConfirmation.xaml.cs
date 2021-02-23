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
    /// Interaction logic for DevicesAdminConfirmation.xaml
    /// </summary>
    public partial class DevicesAdminConfirmation : Window
    {
        Assembly assembly;
        ResourceManager rm;
        CultureInfo cultureInfo;
        SnackbarMessageQueue myMessageQueue = new SnackbarMessageQueue(TimeSpan.FromSeconds(2));
        string ID;
        string pathToDBFile = Properties.Resources.ConnectionString;

        public DevicesAdminConfirmation(string IDReceived)
        {
            InitializeComponent();
            Snackbar.MessageQueue = myMessageQueue;
            assembly = Assembly.Load("Automation");
            rm = new ResourceManager("Automation.Languages.language", assembly);
            cultureInfo = new CultureInfo(Properties.Settings.Default.language);
            ID = IDReceived;
        }

        private void confirm_Click(object sender, RoutedEventArgs e)
        {
            doWork();
        }

        private void confirmation_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                doWork();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        private void doWork()
        {
            if (confirmation.Password == "")
            {
                myMessageQueue.Enqueue(rm.GetString("confirmationIsEmpty"), rm.GetString("ok"), () => HandleOKMethod());
                return;
            }

            if (checkAdmin())
            {
                using (MySqlConnection localDbConnection = new MySqlConnection(pathToDBFile))
                {
                    localDbConnection.Open();

                    string query = "DELETE FROM Devices WHERE ID = @ID";
                    MySqlCommand command = new MySqlCommand(query, localDbConnection);
                    command.Parameters.AddWithValue("@ID", ID);

                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        myMessageQueue.Enqueue(ex.Message, rm.GetString("ok"), () => HandleOKMethod());
                        return;
                    }
                }
            }
            else
            {
                myMessageQueue.Enqueue(rm.GetString("adminConfirmationNotValid", cultureInfo), "OK", () => HandleOKMethod());
                return;
            }
            Variables.saved = true;
            MessageBox.Show(rm.GetString("deviceDeleted"), rm.GetString("system"),
             MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }

        private void HandleOKMethod()
        {

        }

        private bool checkAdmin()
        {
            using (MySqlConnection localDbConnection = new MySqlConnection(pathToDBFile))
            {
                localDbConnection.Open();
                string query = "SELECT Password FROM Confirmation";
                MySqlCommand command = new MySqlCommand(query, localDbConnection);
                MySqlDataAdapter dataAdapterOperators = new MySqlDataAdapter(command);
                DataTable dataTableOperators = new DataTable();
                dataAdapterOperators.Fill(dataTableOperators);
                for(int i = 0; i < dataTableOperators.Rows.Count; i++)
                {
                    if (confirmation.Password == dataTableOperators.Rows[i][0].ToString())
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
