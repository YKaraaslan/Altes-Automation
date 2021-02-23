using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Windows;
using System.Windows.Controls;

namespace Automation
{
    /// <summary>
    /// Interaction logic for ReportCompareShowDate.xaml
    /// </summary>
    public partial class ReportCompareShowDate : Window
    {
        Assembly assembly;
        ResourceManager rm;
        CultureInfo cultureInfo;
        string pathToDBFile = Properties.Resources.ConnectionString;
        List<string> deviceID, deviceName, operatorName, dbName, dbRunningName;
        DateTime dateFirst, dateLast;
        int order1, order2;

        public ReportCompareShowDate(List<string> id, List<string> name, List<string> operators, List<string> db, List<string> dbRunning, int order1Received, int order2Received)
        {
            InitializeComponent();
            assembly = Assembly.Load("Automation");
            rm = new ResourceManager("Automation.Languages.language", assembly);
            cultureInfo = new CultureInfo(Properties.Settings.Default.language);
            deviceID = id; deviceName = name; operatorName = operators; dbName = db; dbRunningName = dbRunning;
            order1 = order1Received; order2 = order2Received;
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
            ReportToCompare report = new ReportToCompare(deviceID, deviceName, operatorName, dbName, dbRunningName, date1, date2, isWanted, order1, order2);
            report.Show();
        }
    }
}
