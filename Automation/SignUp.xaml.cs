using MaterialDesignThemes.Wpf;
using MySql.Data.MySqlClient;
using System;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Automation
{
    /// <summary>
    /// Interaction logic for SignUp.xaml
    /// </summary>
    public partial class SignUp : UserControl
    {
        DockPanel panel;
        WelcomeScreen welcomeScreen;
        string name, id, pass, passRepeat, mail, phone;
        protected string adminPass;
        Assembly assembly;
        ResourceManager rm;
        CultureInfo cultureInfo;
        SnackbarMessageQueue myMessageQueue = new SnackbarMessageQueue(TimeSpan.FromSeconds(2));
        string pathToDBFile = Properties.Resources.ConnectionString;

        public SignUp(DockPanel dockPanel, WelcomeScreen welcome)
        {
            InitializeComponent();
            assembly = Assembly.Load("Automation");
            rm = new ResourceManager("Automation.Languages.language", assembly);
            cultureInfo = new CultureInfo(Properties.Settings.Default.language);
            panel = dockPanel;
            welcomeScreen = welcome;
            Snackbar.MessageQueue = myMessageQueue;
        }
        private void KeyDownPressed(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                buttonSignUpEvent();
            }
        }

        private new void PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void textMail_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (textMail.Text.Contains("@"))
            {
                textMail.Text = textMail.Text.Replace("@", String.Empty);
                comboBoxMail.Focus();
            }
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            Login login = new Login(panel, welcomeScreen);
            DockPanel.SetDock(login, Dock.Left);
            panel.Children.Clear();
            panel.Children.Add(login);
        }
        private void buttonSignUp_Click(object sender, RoutedEventArgs e)
        {
            buttonSignUpEvent();
        }

        private void buttonSignUpEvent()
        {
            name = textNameSurname.Text.Trim();
            id = textUserId.Text.Trim();
            pass = textPassword.Password.Trim();
            passRepeat = textPasswordRepeat.Password.Trim();
            mail = textMail.Text.Trim() + "@" + comboBoxMail.Text.Trim();
            phone = phoneNumber.Text.Trim();

            if (name == "" || id == "" || pass == "" || passRepeat == "" || phone == "")
            {
                myMessageQueue.Enqueue(rm.GetString("blanksCannotBeEmpty", cultureInfo), "OK", () => HandleUndoMethod());
                return;
            }
            if (pass.Length < 5 || pass.Length > 16)
            {
                myMessageQueue.Enqueue(rm.GetString("passwordMinAndMax", cultureInfo), "OK", () => HandleUndoMethod());
                return;
            }
            if (pass != passRepeat)
            {
                myMessageQueue.Enqueue(rm.GetString("passwordDoesntMatch", cultureInfo), "OK", () => HandleUndoMethod());
                return;
            }
            if (comboBoxMail.Text.Trim() == "" || textMail.Text.Trim() == "")
            {
                myMessageQueue.Enqueue(rm.GetString("mailEmpty", cultureInfo), "OK", () => HandleUndoMethod());
                return;
            }
            /*Confirmation confirmation = new Confirmation(panel, welcomeScreen,
                name, phone, mail, Convert.ToDateTime(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")), id, pass);
            DockPanel.SetDock(confirmation, Dock.Left);
            panel.Children.Clear();
            panel.Children.Add(confirmation);*/
            InsertIntoDatabase();
            return;
        }
        private void InsertIntoDatabase()
        {
            using (MySqlConnection localDbConnection = new MySqlConnection(pathToDBFile))
            {
                localDbConnection.Open();

                try
                {
                    MySqlCommand commandToAddDatabase = new MySqlCommand("INSERT INTO Users(NameSurname, UserName, Password, Mail, Phone)" +
                        " VALUES(@NameSurname, @UserName, @Password, @Mail, @Phone)", localDbConnection);
                    commandToAddDatabase.Parameters.AddWithValue("@NameSurname", name);
                    commandToAddDatabase.Parameters.AddWithValue("@UserName", id);
                    commandToAddDatabase.Parameters.AddWithValue("@Password", pass);
                    commandToAddDatabase.Parameters.AddWithValue("@Mail", mail);
                    commandToAddDatabase.Parameters.AddWithValue("@Phone", phone);
                    commandToAddDatabase.ExecuteNonQuery();
                    MessageBox.Show(rm.GetString("signUpSucceeded"), rm.GetString("system"),
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    Login login = new Login(panel, welcomeScreen);
                    DockPanel.SetDock(login, Dock.Left);
                    panel.Children.Clear();
                    panel.Children.Add(login);
                }
                catch (Exception ex)
                {
                    myMessageQueue.Enqueue(ex.Message, rm.GetString("ok"), () => HandleUndoMethod());
                }
            }
        }

        private void HandleUndoMethod()
        {

        }
    }
}
