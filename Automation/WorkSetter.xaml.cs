using MaterialDesignThemes.Wpf;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Automation
{
    /// <summary>
    /// Interaction logic for WorkSetter.xaml
    /// </summary>
    public partial class WorkSetter : Window
    {
        Assembly assembly;
        ResourceManager rm;
        CultureInfo cultureInfo;
        SnackbarMessageQueue myMessageQueue = new SnackbarMessageQueue(TimeSpan.FromSeconds(2));
        string pathToDBFile = Properties.Resources.ConnectionString, dbWorkingOn;
        string id, machineName, operatorName;
        int length;

        public WorkSetter(string deviceID, int lengthReceived, string dbWorkingOnReceived)
        {
            InitializeComponent();
            Snackbar.MessageQueue = myMessageQueue;
            assembly = Assembly.Load("Automation");
            rm = new ResourceManager("Automation.Languages.language", assembly);
            cultureInfo = new CultureInfo(Properties.Settings.Default.language);

            id = deviceID;
            length = lengthReceived;
            dbWorkingOn = dbWorkingOnReceived;
        }


        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void workName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Save();
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Save();
        }

        private void Save()
        {
            if (workName.Text.Trim() == "")
            {
                myMessageQueue.Enqueue(rm.GetString("fillBlanks", cultureInfo), rm.GetString("ok", cultureInfo), () => HandleOKMethod());
                return;
            }

            using (MySqlConnection localDbConnection = new MySqlConnection(pathToDBFile))
            {
                localDbConnection.Open();

                try
                {
                    getNames();

                    string queryToRead = string.Format("SELECT Length FROM `{0}` ORDER BY ID DESC LIMIT 1", dbWorkingOn);
                    MySqlCommand commandToRead = new MySqlCommand(queryToRead, localDbConnection);
                    MySqlDataAdapter dataAdapter = new MySqlDataAdapter(commandToRead);
                    DataTable dataTable = new DataTable();
                    dataAdapter.Fill(dataTable);
                    try
                    {

                        length = length - Convert.ToInt32(dataTable.Rows[0][0]);

                        if (length < 0)
                        {
                            length = 0;
                        }
                    }
                    catch { }

                    string query = string.Format("INSERT INTO `{0}`(OperatorName, WorkingOn, Length, DateWork) " +
                        "VALUES(@OperatorName, @WorkingOn, @Length, @Date)", dbWorkingOn);
                    MySqlCommand commandToAddDatabase = new MySqlCommand(query, localDbConnection);
                    commandToAddDatabase.Parameters.AddWithValue("@OperatorName", operatorName);
                    commandToAddDatabase.Parameters.AddWithValue("@WorkingOn", workName.Text.Trim());
                    commandToAddDatabase.Parameters.AddWithValue("@Length", length);
                    commandToAddDatabase.Parameters.AddWithValue("@Date", Convert.ToDateTime(DateTime.Now.ToString("dd/MM/yyyy HH:mm")));

                    string updateQuery = "UPDATE `Devices` SET WorkingOn = @WorkingOn, WorkingOnTime = @WorkingOnTime WHERE ID = @ID";
                    MySqlCommand commandUpdate = new MySqlCommand(updateQuery, localDbConnection);
                    commandUpdate.Parameters.AddWithValue("@WorkingOn", workName.Text.Trim());
                    commandUpdate.Parameters.AddWithValue("@WorkingOnTime", DateTime.Now);
                    commandUpdate.Parameters.AddWithValue("@ID", id);

                    commandToAddDatabase.ExecuteNonQuery();
                    commandUpdate.ExecuteNonQuery();
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, rm.GetString("system"), MessageBoxButton.OK,
                            MessageBoxImage.Error);
                }
            }
        }

        private void getNames()
        {
            using (MySqlConnection connection = new MySqlConnection(pathToDBFile))
            {
                MySqlCommand command = new MySqlCommand("SELECT Device, Operator FROM Devices WHERE ID = @ID ", connection);
                command.Parameters.AddWithValue("@ID", id);
                MySqlDataAdapter dataAdapter = new MySqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                dataAdapter.Fill(dataTable);

                machineName = dataTable.Rows[0][0].ToString();
                operatorName = dataTable.Rows[0][1].ToString();
            }
        }

        private void HandleOKMethod()
        {

        }
    }
}
