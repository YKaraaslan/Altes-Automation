using MaterialDesignThemes.Wpf;
using System;
using System.Data.SqlClient;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Automation
{
    /// <summary>
    /// Interaction logic for Confirmation.xaml
    /// </summary>
    public partial class Confirmation : UserControl
    {
        DockPanel panel;
        WelcomeScreen welcomeScreen;
        Assembly assembly;
        ResourceManager rm;
        CultureInfo cultureInfo;
        SnackbarMessageQueue myMessageQueue = new SnackbarMessageQueue(TimeSpan.FromSeconds(2));
        bool confirmationSituation = false;
        string pathToDBFile = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Unity Otomasyon\Unity.mdf";

        string name, phone, mail, id, pass;
        DateTime dateTime; 

        public Confirmation(DockPanel dockPanel, WelcomeScreen welcome, string nameString, 
            string phoneString, string mailString, DateTime dateTimeString, string idString, string passString)
        {
            InitializeComponent();
            assembly = Assembly.Load("Automation");
            rm = new ResourceManager("Automation.Languages.language", assembly);
            cultureInfo = new CultureInfo(Properties.Settings.Default.language);
            panel = dockPanel;
            welcomeScreen = welcome;
            Snackbar.MessageQueue = myMessageQueue;
            name = nameString; phone = phoneString;
            mail = mailString; id = idString; pass = passString; dateTime = dateTimeString;
        }

        private void KeyDownPressed(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                buttonConfirmEvent();
            }
        }

        private void buttonConfirmEvent()
        {
            string textInput = confirmation.Text.Trim();

            if (textInput == "")
            {
                myMessageQueue.Enqueue(rm.GetString("blanksCannotBeEmpty", cultureInfo), "OK", () => HandleUndoMethod());
                return;
            }

            using (SqlConnection localDbConnection = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=" + pathToDBFile + ";Integrated Security=True"))
            {
                localDbConnection.Open();
                string query = "SELECT Password FROM Confirmation";
                SqlCommand command = new SqlCommand(query, localDbConnection);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    if (confirmation.Text == reader.GetString(0))
                    {
                        confirmationSituation = true;
                    }
                }

                if (confirmationSituation)
                {
                    InsertIntoDatabase();
                }
                else
                {
                    myMessageQueue.Enqueue(rm.GetString("confirmationNumberInvalid", cultureInfo), "OK", () => HandleUndoMethod());
                    return;
                }
            }

        }

        private void InsertIntoDatabase()
        {
            using (SqlConnection localDbConnection = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=" + pathToDBFile + ";Integrated Security=True"))
            {
                localDbConnection.Open();

                try
                {
                    SqlCommand commandToAddDatabase = new SqlCommand("INSERT INTO Users(NameSurname, UserName, Password, Mail, Phone)" +
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

        private void buttonConfirm_Click(object sender, RoutedEventArgs e)
        {
            buttonConfirmEvent();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Login login = new Login(panel, welcomeScreen);
            DockPanel.SetDock(login, Dock.Left);
            panel.Children.Clear();
            panel.Children.Add(login);
        }
    }
}
