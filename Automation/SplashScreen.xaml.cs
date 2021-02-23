using System;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace Automation
{
    /// <summary>
    /// Interaction logic for SplashScreen.xaml
    /// </summary>
    public partial class SplashScreen : Window
    {
        DispatcherTimer dispatcherTimer = new DispatcherTimer();

        public SplashScreen()
        {
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(Properties.Settings.Default.language);
            InitializeComponent();
            dispatcherTimer.Tick += timer1_Tick;
            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(2300);
            dispatcherTimer.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.adminEntered)
            {
                Properties.Settings.Default.username = string.Empty;
                Properties.Settings.Default.password = string.Empty;
                Properties.Settings.Default.nameSurname = string.Empty;
                Properties.Settings.Default.userMail = string.Empty;
                Properties.Settings.Default.signedIn = false;
                Properties.Settings.Default.userDbID = string.Empty;
                Properties.Settings.Default.phoneNumber = string.Empty;
                Properties.Settings.Default.database = string.Empty;
                Properties.Settings.Default.adminEntered = false;
                Properties.Settings.Default.Save();
            }

            try
            {
                if (Properties.Settings.Default.signedIn)
                {
                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();
                }
                else
                {
                    WelcomeScreen welcomeScreen = new WelcomeScreen();
                    welcomeScreen.Show();
                }
            }
            catch { }
            dispatcherTimer.Stop();
            this.Close();
        }
    }
}
