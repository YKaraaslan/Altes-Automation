using MaterialDesignThemes.Wpf;
using MySql.Data.MySqlClient;
using System;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Automation
{
    /// <summary>
    /// Interaction logic for PasswordForgotten.xaml
    /// </summary>
    public partial class PasswordForgotten : UserControl
    {
        DockPanel panel;
        string userName, userId, userMail;
        Assembly assembly;
        ResourceManager rm;
        CultureInfo cultureInfo;
        WelcomeScreen welcomeScreen;
        string password;
        SnackbarMessageQueue myMessageQueue = new SnackbarMessageQueue(TimeSpan.FromSeconds(2));
        string pathToDBFile = Properties.Resources.ConnectionString;

        public PasswordForgotten(DockPanel dockPanel, WelcomeScreen welcome)
        {
            InitializeComponent();
            assembly = Assembly.Load("Automation");
            rm = new ResourceManager("Automation.Languages.language", assembly);
            cultureInfo = new CultureInfo(Properties.Settings.Default.language);
            panel = dockPanel;
            welcomeScreen = welcome;

            Snackbar.MessageQueue = myMessageQueue;
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            Login login = new Login(panel, welcomeScreen);
            DockPanel.SetDock(login, Dock.Left);
            panel.Children.Clear();
            panel.Children.Add(login);
        }

        private void HandleUndoMethod()
        {

        }

        private void buttonSend_Click(object sender, RoutedEventArgs e)
        {
            buttonSendEvent();
        }

        private void KeyDownPressed(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                buttonSendEvent();
            }
        }

        private void textMail_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (textMail.Text.Contains("@"))
            {
                textMail.Text = textMail.Text.Replace("@", String.Empty);
                comboBoxMail.Focus();
            }
        }

        private void buttonSendEvent()
        {
            userName = textNameSurname.Text.Trim();
            userId = textUserId.Text.Trim();
            userMail = textMail.Text.Trim() + "@" + comboBoxMail.Text.Trim();

            if (userName.Trim() == "" || userId.Trim() == "" || confirmationNumber.Password.Trim() == "")
            {
                myMessageQueue.Enqueue(rm.GetString("blanksCannotBeEmpty", cultureInfo), "OK", () => HandleUndoMethod());
                return;
            }
            if (comboBoxMail.Text.Trim() == "" || textMail.Text.Trim() == "")
            {
                myMessageQueue.Enqueue(rm.GetString("mailEmpty", cultureInfo), "OK", () => HandleUndoMethod());
                return;
            }

            using (MySqlConnection localDbConnection = new MySqlConnection(pathToDBFile))
            {
                localDbConnection.Open();
                string query = "SELECT * FROM Users WHERE UserName = '" + userId + "' ";
                MySqlCommand command = new MySqlCommand(query, localDbConnection);

                MySqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    if (reader.GetString(1).Trim().ToLower() == userName.ToLower() && reader.GetString(4).Trim().ToLower() == userMail.ToLower())
                    {
                        password = reader.GetString(3).Trim();
                        reader.Close();
                        if (requireNumber(confirmationNumber))
                        {
                            MessageBox.Show(string.Format(rm.GetString("yourPassword"), password), rm.GetString("system"),
                            MessageBoxButton.OK, MessageBoxImage.Information);
                            return;
                        }
                        else
                        {
                            myMessageQueue.Enqueue(rm.GetString("confirmationNumberInvalid", cultureInfo), "OK", () => HandleUndoMethod());
                            return;
                        }
                    }
                }
                myMessageQueue.Enqueue(rm.GetString("userNotFound", cultureInfo), "OK", () => HandleUndoMethod());
            }
        }

        private bool requireNumber(PasswordBox confirmationNumber)
        {
            using (MySqlConnection localDbConnection = new MySqlConnection(pathToDBFile))
            {
                localDbConnection.Open();
                string query = "SELECT Password FROM Confirmation WHERE Password = '" + confirmationNumber.Password.Trim() + "' ";
                MySqlCommand command = new MySqlCommand(query, localDbConnection);
                MySqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    return true;
                }
            }
            return false;
        }
    }
}
