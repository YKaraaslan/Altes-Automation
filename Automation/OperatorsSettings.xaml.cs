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
    /// Interaction logic for OperatorsSettings.xaml
    /// </summary>
    public partial class OperatorsSettings : Window
    {
        Assembly assembly;
        ResourceManager rm;
        CultureInfo cultureInfo;
        string oldName;
        SnackbarMessageQueue myMessageQueue = new SnackbarMessageQueue(TimeSpan.FromSeconds(2));
        string pathToDBFile = Properties.Resources.ConnectionString;

        public OperatorsSettings(string userNamed)
        {
            InitializeComponent();
            Snackbar.MessageQueue = myMessageQueue;
            assembly = Assembly.Load("Automation");
            rm = new ResourceManager("Automation.Languages.language", assembly);
            cultureInfo = new CultureInfo(Properties.Settings.Default.language);
            oldName = userNamed;
            operatorName.Text = userNamed;
        }

        private void operatorName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                update();
            }
        }

        private void save(object sender, RoutedEventArgs e)
        {
            update();
        }

        private void update()
        {
            if (operatorName.Text.Trim() == "")
            {
                myMessageQueue.Enqueue(rm.GetString("fillBlanks"), rm.GetString("ok"), () => HandleOKMethod());
                return;
            }

            using (MySqlConnection localDbConnection = new MySqlConnection(pathToDBFile))
            {
                localDbConnection.Open();

                string query = "UPDATE Operators SET Operator = @newName WHERE Operator = @OldName";
                string queryDevice = "UPDATE Devices SET Operator = @newName WHERE Operator = @OldName";

                MySqlCommand commandToAddDatabase = new MySqlCommand(query, localDbConnection);
                commandToAddDatabase.Parameters.AddWithValue("@newName", operatorName.Text.Trim());
                commandToAddDatabase.Parameters.AddWithValue("@oldName", oldName);

                MySqlCommand commandUpdateDevices = new MySqlCommand(queryDevice, localDbConnection);
                commandUpdateDevices.Parameters.AddWithValue("@newName", operatorName.Text.Trim());
                commandUpdateDevices.Parameters.AddWithValue("@oldName", oldName);

                try
                {
                    commandToAddDatabase.ExecuteNonQuery();
                    commandUpdateDevices.ExecuteNonQuery();
                    MessageBox.Show(rm.GetString("operatorInformationUpdated"), rm.GetString("system"),
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }
                catch (Exception ex) { MessageBox.Show(ex.Message, rm.GetString("system", cultureInfo), MessageBoxButton.OK, MessageBoxImage.Error); }
            }
        }

        private void HandleOKMethod()
        {

        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
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
            OperatorsAdminConfirmation adminConfirmation = new OperatorsAdminConfirmation(oldName);
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
