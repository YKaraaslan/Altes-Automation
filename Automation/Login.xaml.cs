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
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : UserControl
    {
        DockPanel panel;
        string userId, userPassword;
        Assembly assembly;
        ResourceManager rm;
        CultureInfo cultureInfo;
        WelcomeScreen welcomeScreen;
        SnackbarMessageQueue myMessageQueue = new SnackbarMessageQueue(TimeSpan.FromSeconds(2));
        
        string pathToDBFile = Properties.Resources.ConnectionString;

        public Login(DockPanel dockPanel, WelcomeScreen welcome)
        {
            InitializeComponent();
            assembly = Assembly.Load("Automation");
            rm = new ResourceManager("Automation.Languages.language", assembly);
            cultureInfo = new CultureInfo(Properties.Settings.Default.language);
            panel = dockPanel;
            welcomeScreen = welcome;
            Snackbar.MessageQueue = myMessageQueue; 
            Variables.isAdmin = false;
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                buttonLoginEvent();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void HandleOKMethod()
        {

        }

        private void buttonLoginEvent()
        {
            userId = id.Text.Trim();
            userPassword = password.Password.Trim();

            if (userId.Trim() == "" || userPassword.Trim() == "")
            {
                myMessageQueue.Enqueue(rm.GetString("blanksCannotBeEmpty", cultureInfo), "OK", () => HandleOKMethod());
                return;
            }

            using (MySqlConnection localDbConnection = new MySqlConnection(pathToDBFile))
            {
                localDbConnection.Open();
                string query = "SELECT * FROM Admins WHERE UserName = '" + userId + "' AND Password = '" + userPassword + "' ";
                MySqlCommand command = new MySqlCommand(query, localDbConnection);

                MySqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    Variables.isAdmin = true;

                    reader.Close();

                    myMessageQueue.Enqueue(rm.GetString("welcome", cultureInfo), "OK", () => HandleOKMethod());
                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();
                    welcomeScreen.Close();
                    return;
                }
            }

            using (MySqlConnection localDbConnection = new MySqlConnection(pathToDBFile))
            {
                localDbConnection.Open();
                string query = "SELECT * FROM Users WHERE UserName = '" + userId + "' AND Password = '" + userPassword + "' ";
                MySqlCommand command = new MySqlCommand(query, localDbConnection);

                MySqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    Variables.UserName = reader.GetString(1);
                    //string status = Convert.ToBoolean(checkBoxRemember.IsChecked) ? "+" : "-";

                    if ((bool)checkBoxRemember.IsChecked)
                    {
                        Properties.Settings.Default.username = userId;
                        Properties.Settings.Default.password = userPassword;
                        Properties.Settings.Default.nameSurname = Variables.UserName;
                        Properties.Settings.Default.userMail = reader.GetString(4);
                        Properties.Settings.Default.signedIn = true;
                        Properties.Settings.Default.phoneNumber = reader.GetString(5);
                        Properties.Settings.Default.userDbID = reader.GetInt32(0).ToString();
                        Properties.Settings.Default.Save();
                    }

                    reader.Close();

                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();
                    welcomeScreen.Close();
                    return;
                }
                else
                    myMessageQueue.Enqueue(rm.GetString("incorrectIdOrPassword", cultureInfo), "OK", () => HandleOKMethod());
            }
        }

        private void passwordForgotten_Click(object sender, RoutedEventArgs e)
        {
            PasswordForgotten passwordForgotten = new PasswordForgotten(panel, welcomeScreen);
            DockPanel.SetDock(passwordForgotten, Dock.Left);
            panel.Children.Clear();
            panel.Children.Add(passwordForgotten);
        }

        private void ToggleButtonFacebook_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(@"https://www.facebook.com/unityotomasyon.com.tr/");
            System.Diagnostics.Process.Start(@"https://www.facebook.com/unityotomasyonofficial");
        }

        private void ToggleButtonInstagram_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(@"https://instagram.com/unity.otomasyon?igshid=14ai0n44fasvd");
        }

        private void ToggleButtonTwitter_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(@"https://twitter.com/unityotomasyon");
        }

        private void ToggleButtonUnity_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(@"https://www.unityotomasyon.com.tr");
        }

        private void ToggleButtonYoutube_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(@"https://www.youtube.com/channel/UCkGAeeXojw8ExlUiMwWMFYg");
        }

        private void ToggleButtonLinkedIn_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(@"https://tr.linkedin.com/company/unity-otomasyon");
        }

        private void KeyDownPressed(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                try
                {
                    buttonLoginEvent();
                }
                catch { }
            }
        }

        private void buttonSignUp_Click(object sender, RoutedEventArgs e)
        {
            SignUp signUp = new SignUp(panel, welcomeScreen);
            DockPanel.SetDock(signUp, Dock.Left);
            panel.Children.Clear();
            panel.Children.Add(signUp);
        }
    }
}
