using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WorkSetter
{
    /// <summary>
    /// Interaction logic for ReportChooser.xaml
    /// </summary>
    public partial class ReportChooser : Window
    {
        string pathToDBFile = Path.toDB;
        string deviceName, operatorName, dbName;
        DateTime dateFirst, dateLast;

        public ReportChooser(string name, string operators, string db)
        {
            InitializeComponent();
            deviceName = name; operatorName = operators; dbName = db;
        }

        private void dateBeginning_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            dateFirst = Convert.ToDateTime(dateBeginning.SelectedDate);
        }

        private void dateEnd_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            dateLast = Convert.ToDateTime(dateEnd.SelectedDate);
        }

        private void reportAll_Click(object sender, RoutedEventArgs e)
        {
            openReport(DateTime.Now, DateTime.Now, false);
        }

        private void reportDate_Click(object sender, RoutedEventArgs e)
        {
            if (dateLast > DateTime.Now)
            {
                MessageBox.Show("Gelecek tarihli rapor oluşturulamaz", "Sistem", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            else if (dateFirst > dateLast)
            {
                MessageBox.Show("Başlangıç tarihi bitiş tarihinden sonra olamaz.", "Sistem", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            else if (dateLast == null || dateFirst == null)
            {
                MessageBox.Show("Tarihleri kontrol ediniz.", "Sistem", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            else
            {
                openReport(dateFirst, dateLast, true);
            }
        }

        private void Weekly_Click(object sender, RoutedEventArgs e)
        {
            openReport(DateTime.Now.AddDays(-7), DateTime.Now, true);
        }

        private void Monthly_Click(object sender, RoutedEventArgs e)
        {
            openReport(DateTime.Now.AddMonths(-1), DateTime.Now, true);
        }

        private void Yearly_Click(object sender, RoutedEventArgs e)
        {
            openReport(DateTime.Now.AddYears(-1), DateTime.Now, true);
        }

        private void openReport(DateTime now1, DateTime now2, bool v)
        {
            using (MySqlConnection localDbConnection = new MySqlConnection(pathToDBFile))
            {
                try
                {
                    Report report = new Report(deviceName, operatorName, dbName, now1, now2, v);
                    report.Show();
                }
                catch (System.ObjectDisposedException) { }
                catch (Exception ex) { MessageBox.Show(ex.Message, "Sistem", MessageBoxButton.OK, MessageBoxImage.Error); }
            }
        }
    }
}
