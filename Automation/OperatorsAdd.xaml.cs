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
    /// Interaction logic for OperatorsAdd.xaml
    /// </summary>
    public partial class OperatorsAdd : Window
    {
        Assembly assembly;
        ResourceManager rm;
        CultureInfo cultureInfo;
        SnackbarMessageQueue myMessageQueue = new SnackbarMessageQueue(TimeSpan.FromSeconds(2));
        string pathToDBFile = Properties.Resources.ConnectionString;

        public OperatorsAdd()
        {
            InitializeComponent();
            Snackbar.MessageQueue = myMessageQueue;
            assembly = Assembly.Load("Automation");
            rm = new ResourceManager("Automation.Languages.language", assembly);
            cultureInfo = new CultureInfo(Properties.Settings.Default.language);
        }

        private void add_Click(object sender, RoutedEventArgs e)
        {
            addOperator();
        }

        private void addOperator()
        {
            if (companyName.Text.Trim() == "")
            {
                myMessageQueue.Enqueue(rm.GetString("fillBlanks", cultureInfo), rm.GetString("ok", cultureInfo), () => HandleOKMethod());
                return;
            }

            using (MySqlConnection localDbConnection = new MySqlConnection(pathToDBFile))
            {
                localDbConnection.Open();

                try
                {
                    MySqlCommand cmd = new MySqlCommand("Select count(*) from Operators where Operator = @Operator", localDbConnection);
                    cmd.Parameters.AddWithValue("@Operator", companyName.Text.Trim());
                    int result = (int)cmd.ExecuteScalar();
                    if (result != 0)
                    {
                        MessageBox.Show(rm.GetString("operatorAlreadyExist"), rm.GetString("system"), MessageBoxButton.OK,
                            MessageBoxImage.Warning);
                        return;
                    }
                }
                catch { }

                try
                {
                    MySqlCommand commandToAddDatabase = new MySqlCommand("INSERT INTO Operators(Operator) VALUES(@Operator)", localDbConnection);
                    commandToAddDatabase.Parameters.AddWithValue("@Operator", companyName.Text.Trim());
                    commandToAddDatabase.ExecuteNonQuery();
                    Variables.saved = true;
                    MessageBox.Show(rm.GetString("operatorSaved"), rm.GetString("system"),
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, rm.GetString("system"), MessageBoxButton.OK,
                            MessageBoxImage.Error);
                }
            }
        }

        private void companyName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                addOperator();
            }
        }

        private void HandleOKMethod()
        {

        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}