using System;
using System.Collections.Generic;
using System.Linq;
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

namespace WorkSetter
{
    /// <summary>
    /// Interaction logic for ReportWorkChooser.xaml
    /// </summary>
    public partial class ReportWorkChooser : Window
    {
        string pathToDBFile = Path.toDB;
        string deviceName, operatorName, dbName;
        DateTime dateFirst, dateLast;

        public ReportWorkChooser(string name, string operators, string db)
        {
            InitializeComponent();
            deviceName = name; operatorName = operators; dbName = db;
        }

        private void Monthly_Click(object sender, RoutedEventArgs e)
        {
            openReport(DateTime.Now.AddMonths(-1), DateTime.Now);
        }

        private void Weekly_Click(object sender, RoutedEventArgs e)
        {
            openReport(DateTime.Now.AddDays(-7), DateTime.Now);
        }

        private void dateBeginning_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            dateFirst = Convert.ToDateTime(dateBeginning.SelectedDate);
        }

        private void dateEnd_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            dateLast = Convert.ToDateTime(dateEnd.SelectedDate);
        }

        private void ReportDate_Click(object sender, RoutedEventArgs e)
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
                openReport(dateFirst, dateLast);
            }
        }

        private void openReport(DateTime dateFirst, DateTime dateLast)
        {
            try
            {
                ReportWork report = new ReportWork(deviceName, operatorName, dbName, dateFirst, dateLast);
                report.Show();
            }
            catch (System.ObjectDisposedException) { }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Sistem", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
