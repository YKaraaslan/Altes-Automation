using MySql.Data.MySqlClient;
using System;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Windows;
using System.Windows.Controls;

namespace Automation
{
    /// <summary>
    /// Interaction logic for ReportShowDate.xaml
    /// </summary>
    public partial class ReportShowDate : Window
    {
        Assembly assembly;
        ResourceManager rm;
        CultureInfo cultureInfo;
        string pathToDBFile = Properties.Resources.ConnectionString;
        string deviceID, deviceName, operatorName, dbName, dbRunningName;
        DateTime dateFirst, dateLast;

        public ReportShowDate(string id, string name, string operators, string db, string dbRunning)
        {
            InitializeComponent();
            assembly = Assembly.Load("Automation");
            rm = new ResourceManager("Automation.Languages.language", assembly);
            cultureInfo = new CultureInfo(Properties.Settings.Default.language);
            deviceID = id; deviceName = name; operatorName = operators; dbName = db; dbRunningName = dbRunning;
        }

        private void reportDate_Click(object sender, RoutedEventArgs e)
        {
            if (dateLast > DateTime.Now)
            {
                MessageBox.Show(rm.GetString("dateIsPast", cultureInfo));
                return;
            }
            else if (dateFirst > dateLast)
            {
                MessageBox.Show(rm.GetString("dateCannotBePast", cultureInfo));
                return;
            }
            else if (dateLast == null || dateFirst == null)
            {
                MessageBox.Show(rm.GetString("dateCannotBeEmpty", cultureInfo));
                return;
            }
            else
            {
                openReport(dateFirst, dateLast, true);
            }
        }

        private void reportAll_Click(object sender, RoutedEventArgs e)
        {
            openReport(DateTime.Now, DateTime.Now, false);
        }

        private void dateBeginning_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            dateFirst = Convert.ToDateTime(dateBeginning.SelectedDate);
        }

        private void dateEnd_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            dateLast = Convert.ToDateTime(dateEnd.SelectedDate);
        }

        private void openReport(DateTime date1, DateTime date2, bool isWanted)
        {
            using (MySqlConnection localDbConnection = new MySqlConnection(pathToDBFile))
            {
                try
                {
                    Report report = new Report(deviceID, deviceName, operatorName, dbName, dbRunningName, date1, date2, isWanted);
                    report.Show();
                }
                catch (System.ObjectDisposedException) { }catch (Exception ex) { MessageBox.Show(ex.Message, rm.GetString("system"), MessageBoxButton.OK, MessageBoxImage.Error); }
            }
        }
    }
}
