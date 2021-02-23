using MaterialDesignThemes.Wpf;
using MySql.Data.MySqlClient;
using System;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Windows;
using System.Windows.Input;

namespace Automation
{
    /// <summary>
    /// Interaction logic for OperatorsAdminConfirmation.xaml
    /// </summary>
    public partial class OperatorsAdminConfirmation : Window
    {
        Assembly assembly;
        ResourceManager rm;
        CultureInfo cultureInfo;
        SnackbarMessageQueue myMessageQueue = new SnackbarMessageQueue(TimeSpan.FromSeconds(2));
        string nameReceived;
        string pathToDBFile = Properties.Resources.ConnectionString;

        public OperatorsAdminConfirmation(string name)
        {
            InitializeComponent();
            Snackbar.MessageQueue = myMessageQueue;
            assembly = Assembly.Load("Automation");
            rm = new ResourceManager("Automation.Languages.language", assembly);
            cultureInfo = new CultureInfo(Properties.Settings.Default.language);
            nameReceived = name;
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

                    string query = "DELETE FROM Operators WHERE Operator = @Name";
                    MySqlCommand command = new MySqlCommand(query, localDbConnection);
                    command.Parameters.AddWithValue("@Name", nameReceived);

                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, rm.GetString("system"), MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }
                }
            }
            else
            {
                MessageBox.Show(rm.GetString("adminConfirmationNotValid", cultureInfo), rm.GetString("system"),
                 MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            Variables.saved = true;
            MessageBox.Show(rm.GetString("operatorDeleted"), rm.GetString("system"),
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
                string query = "SELECT Password FROM Confirmation WHERE Password = @Password";
                MySqlCommand command = new MySqlCommand(query, localDbConnection);
                command.Parameters.AddWithValue("@Password", confirmation.Password);
                MySqlDataReader reader = command.ExecuteReader();
                if (reader.Read() && confirmation.Password == reader.GetString(0))
                {
                    return true;
                }
            }
            return false;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
