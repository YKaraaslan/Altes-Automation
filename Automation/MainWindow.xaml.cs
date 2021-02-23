using MySql.Data.MySqlClient;
using System;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Automation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Assembly assembly;
        ResourceManager rm;
        CultureInfo cultureInfo;
        string pathToDBFile = Properties.Resources.ConnectionString;
        System.Windows.Forms.NotifyIcon nIcon = new System.Windows.Forms.NotifyIcon();

        public MainWindow()
        {
            InitializeComponent();
            assembly = Assembly.Load("Automation");
            rm = new ResourceManager("Automation.Languages.language", assembly);
            cultureInfo = new CultureInfo(Properties.Settings.Default.language);

            openUserControl(new Devices(), "Devices");

            if (Variables.isAdmin)
            {
                userSettingsButton.IsEnabled = false;
            }

            init();
        }

        private void init()
        {
            if (!Variables.isAdmin)
            {
                expanderUserName.Header = Properties.Settings.Default.nameSurname.Trim();
                expanderUserName.Content = "        " + Properties.Settings.Default.userMail.Trim();
            }
            else
            {
                expanderUserName.Header = "Administrator";
                expanderUserName.Content = "Administrator";
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.DragMove();
            }
            catch { }
        }

        private void ListViewItem_MouseEnter(object sender, MouseEventArgs e)
        {
            if (Tg_Btn.IsChecked == true)
            {
                tt_home.Visibility = Visibility.Collapsed;
                tt_contacts.Visibility = Visibility.Collapsed;
                tt_reports.Visibility = Visibility.Collapsed;
                tt_messages.Visibility = Visibility.Collapsed;
                tt_signout.Visibility = Visibility.Collapsed;
            }
            else
            {
                tt_home.Visibility = Visibility.Visible;
                tt_contacts.Visibility = Visibility.Visible;
                tt_reports.Visibility = Visibility.Visible;
                tt_messages.Visibility = Visibility.Visible;
                tt_signout.Visibility = Visibility.Visible;
            }
        }

        private void Tg_Btn_Unchecked(object sender, RoutedEventArgs e)
        {
            panel.IsEnabled = true;
        }

        private void Tg_Btn_Checked(object sender, RoutedEventArgs e)
        {
            panel.IsEnabled = false;
        }

        private void BG_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Tg_Btn.IsChecked = false;
        }

        private void ListViewItemHome_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (Variables.selectedUserControl != "Devices")
            {
                openUserControl(new Devices(), "Devices");
            }
        }

        private void ListViewItemOperators_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (Variables.selectedUserControl != "Operators")
            {
                openUserControl(new Operators(), "Operators");
            }
        }

        private void ListViewItemReports_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (Variables.selectedUserControl != "Reports")
            {
                openUserControl(new Reports(), "Reports");
            }
        }

        private void ListViewItemMail_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (Variables.selectedUserControl != "Mail")
            {
                openUserControl(new Mail(), "Mail");
            }
        }

        private void ListViewItemSignOut_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (MessageBox.Show(rm.GetString("areYouSureToExit"), rm.GetString("system"), MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                using (MySqlConnection localDbConnection = new MySqlConnection(pathToDBFile))
                {
                    localDbConnection.Open();
                string query = "UPDATE Users SET Active = 0 WHERE ID = @UserID";
                MySqlCommand command = new MySqlCommand(query, localDbConnection);
                command.Parameters.AddWithValue("@UserID", Properties.Settings.Default.userDbID);
                try
                {
                    command.ExecuteNonQuery();
                }
                catch { }
            }
                Properties.Settings.Default.username = string.Empty;
                Properties.Settings.Default.password = string.Empty;
                Properties.Settings.Default.nameSurname = string.Empty;
                Properties.Settings.Default.userMail = string.Empty;
                Properties.Settings.Default.signedIn = false;
                Properties.Settings.Default.userDbID = string.Empty;
                Properties.Settings.Default.phoneNumber = string.Empty;
                Properties.Settings.Default.Save();
                Variables.JustSignedOut = true;
                Variables.IsAdmin = false;
                WelcomeScreen welcomeScreen = new WelcomeScreen();
                welcomeScreen.Show();
                this.Close();
            }
        }

        private void openUserControl(UserControl myControl, string userControlName)
        {
            DockPanel.SetDock(myControl, Dock.Left);
            dockPanel.Children.Clear();
            dockPanel.Children.Add(myControl);

            if (Tg_Btn.IsChecked == true)
            {
                HideStackPanel.Begin();
                Tg_Btn.IsChecked = false;
            }
            Variables.selectedUserControl = userControlName;
        }

        private void settingsClicked(object sender, RoutedEventArgs e)
        {
            Settings settings = new Settings();
            settings.ShowDialog();
        }

        /*private void Search_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                searchEvent();
            }
        }

        private void searchEvent()
        {
            if (search.Text == "")
                return;
        }*/

        private void UserSettings_Click(object sender, RoutedEventArgs e)
        {
            SettingsUser userSettings = new SettingsUser();
            userSettings.ShowDialog();
        }

        private void Button_MouseEnter(object sender, MouseEventArgs e)
        {
            minify.Background = Brushes.OrangeRed;
            minify.Foreground = Brushes.White;
        }

        private void minify_MouseLeave(object sender, MouseEventArgs e)
        {
            minify.Background = Brushes.Transparent;
            minify.Foreground = Brushes.Black;
        }

        private void minify_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Window_StateChanged(object sender, System.EventArgs e)
        {
            if (this.WindowState == WindowState.Minimized)
            {
                nIcon.Icon = new System.Drawing.Icon(@"../../Assets/Logo.ico");
                nIcon.Visible = true;
                //nIcon.ShowBalloonTip(1000, "Title", "Text", System.Windows.Forms.ToolTipIcon.Info);
                nIcon.Click += nIcon_Click;
                this.Hide();
            }
        }

        private void nIcon_Click(object sender, EventArgs e)
        {
            Visibility = Visibility.Visible;
            WindowState = WindowState.Normal;
        }
    }
}
