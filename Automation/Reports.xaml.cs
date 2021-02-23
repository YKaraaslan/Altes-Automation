using MaterialDesignThemes.Wpf;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
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
    /// Interaction logic for Reports.xaml
    /// </summary>
    public partial class Reports : UserControl
    {
        Assembly assembly;
        ResourceManager rm;
        CultureInfo cultureInfo;
        SnackbarMessageQueue myMessageQueue = new SnackbarMessageQueue(TimeSpan.FromSeconds(2));
        List<string> deviceID = new List<string>();
        List<string> dbNames = new List<string>();
        List<string> dbNamesRunning = new List<string>(); 
        List<string> operatorsName = new List<string>(); 
        List<string> deviceName = new List<string>();
        string pathToDBFile = Properties.Resources.ConnectionString;
        bool[] cardsBool = new bool[] { false, false, false, false, false, false, false, false, false, false };
        Int16 deviceCounter;
        Card[] cards;
        DateTime dateMonthAgo, dateYearAgo;
        TextBlock[] deviceNames;
        TextBlock[] operatorNames;

        public Reports()
        {
            InitializeComponent();
            assembly = Assembly.Load("Automation");
            rm = new ResourceManager("Automation.Languages.language", assembly);
            cultureInfo = new CultureInfo(Properties.Settings.Default.language);
            cards = new Card[] { device1Card, device2Card, device3Card, device4Card, device5Card,
                device6Card, device7Card, device8Card, device9Card, device10Card };
            dateMonthAgo = DateTime.Now.AddMonths(-1);
            dateYearAgo = DateTime.Now.AddYears(-1);
            deviceNames = new TextBlock[] { deviceName1, deviceName2, deviceName3, deviceName4, deviceName5,
                deviceName6, deviceName7, deviceName8, deviceName9, deviceName10 };
            operatorNames = new TextBlock[] { operatorName1, operatorName2, operatorName3, operatorName4, operatorName5,
                operatorName6, operatorName7, operatorName8, operatorName9, operatorName10 };
            init();
        }

        private void init()
        {
            disableVisibility();
            using (MySqlConnection localDbConnection = new MySqlConnection(pathToDBFile))
            {
                localDbConnection.Open();

                MySqlCommand command = new MySqlCommand("SELECT ID, Device, Operator, DbName, DbNameRunning FROM Devices ORDER BY ID ASC", localDbConnection);
                MySqlDataAdapter dataAdapter = new MySqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                dataAdapter.Fill(dataTable);

                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    deviceCounter++;
                    deviceNames[i].Text = dataTable.Rows[i][1].ToString();
                    deviceName.Add(dataTable.Rows[i][1].ToString());
                    operatorNames[i].Text = dataTable.Rows[i][2].ToString();
                    operatorsName.Add(dataTable.Rows[i][2].ToString());
                    deviceID.Add(dataTable.Rows[i][0].ToString());
                    dbNames.Add(dataTable.Rows[i][3].ToString());
                    dbNamesRunning.Add(dataTable.Rows[i][4].ToString());
                    cards[i].Visibility = Visibility.Visible;
                }
            }
        }

        private void disableVisibility()
        {
            for (int i = 0; i < cards.Length; i++)
            {
                cards[i].Visibility = Visibility.Collapsed;
            }
        }

        private void Card1Clicked(object sender, MouseButtonEventArgs e)
        {
            cardsBool[0] = !cardsBool[0];
            if (countSelected() > 2)
            {
                cardsBool[0] = !cardsBool[0];
                countSelected();
                return;
            }
            else
            {
                if (cardsBool[0])
                {
                    device1Card.Background = Brushes.Aqua;
                }
                else
                {
                    device1Card.Background = Brushes.WhiteSmoke;
                }
            }
        }

        private void Card2Clicked(object sender, MouseButtonEventArgs e)
        {
            cardsBool[1] = !cardsBool[1];
            if (countSelected() > 2)
            {
                cardsBool[1] = !cardsBool[1];
                countSelected();
                return;
            }
            else
            {
                if (cardsBool[1])
                {
                    device2Card.Background = Brushes.Aqua;
                }
                else
                {
                    device2Card.Background = Brushes.WhiteSmoke;
                }
            }
        }

        private void Card3Clicked(object sender, MouseButtonEventArgs e)
        {
            cardsBool[2] = !cardsBool[2];
            if (countSelected() > 2)
            {
                cardsBool[2] = !cardsBool[2];
                countSelected();
                return;
            }
            else
            {
                if (cardsBool[2])
                {
                    device3Card.Background = Brushes.Aqua;
                }
                else
                {
                    device3Card.Background = Brushes.WhiteSmoke;
                }
            }
        }

        private void Card4Clicked(object sender, MouseButtonEventArgs e)
        {
            cardsBool[3] = !cardsBool[3];
            if (countSelected() > 2)
            {
                cardsBool[3] = !cardsBool[3];
                countSelected();
                return;
            }
            else
            {
                if (cardsBool[3])
                {
                    device4Card.Background = Brushes.Aqua;
                }
                else
                {
                    device4Card.Background = Brushes.WhiteSmoke;
                }
            }
        }

        private void Card5Clicked(object sender, MouseButtonEventArgs e)
        {
            cardsBool[4] = !cardsBool[4];
            if (countSelected() > 2)
            {
                cardsBool[4] = !cardsBool[4];
                countSelected();
                return;
            }
            else
            {
                if (cardsBool[4])
                {
                    device5Card.Background = Brushes.Aqua;
                }
                else
                {
                    device5Card.Background = Brushes.WhiteSmoke;
                }
            }
        }

        private void Card6Clicked(object sender, MouseButtonEventArgs e)
        {
            cardsBool[5] = !cardsBool[5];
            if (countSelected() > 2)
            {
                cardsBool[5] = !cardsBool[5];
                countSelected();
                return;
            }
            else
            {
                if (cardsBool[5])
                {
                    device6Card.Background = Brushes.Aqua;
                }
                else
                {
                    device6Card.Background = Brushes.WhiteSmoke;
                }
            }
        }

        private void Card7Clicked(object sender, MouseButtonEventArgs e)
        {
            cardsBool[6] = !cardsBool[6];
            if (countSelected() > 2)
            {
                cardsBool[6] = !cardsBool[6];
                countSelected();
                return;
            }
            else
            {
                if (cardsBool[6])
                {
                    device7Card.Background = Brushes.Aqua;
                }
                else
                {
                    device7Card.Background = Brushes.WhiteSmoke;
                }
            }
        }

        private void Card8Clicked(object sender, MouseButtonEventArgs e)
        {
            cardsBool[7] = !cardsBool[7];
            if (countSelected() > 2)
            {
                cardsBool[7] = !cardsBool[7];
                countSelected();
                return;
            }
            else
            {
                if (cardsBool[7])
                {
                    device8Card.Background = Brushes.Aqua;
                }
                else
                {
                    device8Card.Background = Brushes.WhiteSmoke;
                }
            }
        }

        private void Card9Clicked(object sender, MouseButtonEventArgs e)
        {
            cardsBool[8] = !cardsBool[8];
            if (countSelected() > 2)
            {
                cardsBool[8] = !cardsBool[8];
                countSelected();
                return;
            }
            else
            {
                if (cardsBool[8])
                {
                    device9Card.Background = Brushes.Aqua;
                }
                else
                {
                    device9Card.Background = Brushes.WhiteSmoke;
                }
            }
        }

        private void Card10Clicked(object sender, MouseButtonEventArgs e)
        {
            cardsBool[9] = !cardsBool[9];
            if (countSelected() > 2)
            {
                cardsBool[9] = !cardsBool[9];
                countSelected();
                return;
            }
            else
            {
                if (cardsBool[9])
                {
                    device10Card.Background = Brushes.Aqua;
                }
                else
                {
                    device10Card.Background = Brushes.WhiteSmoke;
                }
            }
        }

        //------------------------------------------------------------------------------

        private void report1_Click(object sender, RoutedEventArgs e)
        {
            ReportShowDate reportShowDate = new ReportShowDate(deviceID[0], deviceNames[0].Text, operatorNames[0].Text, dbNames[0], dbNamesRunning[0]);
            reportShowDate.ShowDialog();
        }

        private void report2_Click(object sender, RoutedEventArgs e)
        {
            ReportShowDate reportShowDate = new ReportShowDate(deviceID[1], deviceNames[1].Text, operatorNames[1].Text, dbNames[1], dbNamesRunning[1]);
            reportShowDate.ShowDialog();
        }

        private void report3_Click(object sender, RoutedEventArgs e)
        {
            ReportShowDate reportShowDate = new ReportShowDate(deviceID[2], deviceNames[2].Text, operatorNames[2].Text, dbNames[2], dbNamesRunning[2]);
            reportShowDate.ShowDialog();
        }

        private void report4_Click(object sender, RoutedEventArgs e)
        {
            ReportShowDate reportShowDate = new ReportShowDate(deviceID[3], deviceNames[3].Text, operatorNames[3].Text, dbNames[3], dbNamesRunning[3]);
            reportShowDate.ShowDialog();
        }

        private void report5_Click(object sender, RoutedEventArgs e)
        {
            ReportShowDate reportShowDate = new ReportShowDate(deviceID[4], deviceNames[4].Text, operatorNames[4].Text, dbNames[4], dbNamesRunning[4]);
            reportShowDate.ShowDialog();
        }

        private void report6_Click(object sender, RoutedEventArgs e)
        {
            ReportShowDate reportShowDate = new ReportShowDate(deviceID[5], deviceNames[5].Text, operatorNames[5].Text, dbNames[5], dbNamesRunning[5]);
            reportShowDate.ShowDialog();
        }

        private void report7_Click(object sender, RoutedEventArgs e)
        {
            ReportShowDate reportShowDate = new ReportShowDate(deviceID[6], deviceNames[6].Text, operatorNames[6].Text, dbNames[6], dbNamesRunning[6]);
            reportShowDate.ShowDialog();
        }

        private void report8_Click(object sender, RoutedEventArgs e)
        {
            ReportShowDate reportShowDate = new ReportShowDate(deviceID[7], deviceNames[7].Text, operatorNames[7].Text, dbNames[7], dbNamesRunning[7]);
            reportShowDate.ShowDialog();
        }

        private void report9_Click(object sender, RoutedEventArgs e)
        {
            ReportShowDate reportShowDate = new ReportShowDate(deviceID[8], deviceNames[8].Text, operatorNames[8].Text, dbNames[8], dbNamesRunning[8]);
            reportShowDate.ShowDialog();
        }

        private void report10_Click(object sender, RoutedEventArgs e)
        {
            ReportShowDate reportShowDate = new ReportShowDate(deviceID[9], deviceNames[9].Text, operatorNames[9].Text, dbNames[9], dbNamesRunning[9]);
            reportShowDate.ShowDialog();
        }

        //------------------------------------------------------------------------------

        private void Report1_monthlyReport_Click(object sender, RoutedEventArgs e)
        {
            createReport(deviceID[0], deviceNames[0].Text, operatorNames[0].Text, dbNames[0], dbNamesRunning[0], DateTime.Now.AddMonths(-1), DateTime.Now, true);
        }

        private void Report2_monthlyReport_Click(object sender, RoutedEventArgs e)
        {
            createReport(deviceID[1], deviceNames[1].Text, operatorNames[1].Text, dbNames[1], dbNamesRunning[1], DateTime.Now.AddMonths(-1), DateTime.Now, true);
        }

        private void Report3_monthlyReport_Click(object sender, RoutedEventArgs e)
        {
            createReport(deviceID[2], deviceNames[2].Text, operatorNames[2].Text, dbNames[2], dbNamesRunning[2], DateTime.Now.AddMonths(-1), DateTime.Now, true);
        }

        private void Report4_monthlyReport_Click(object sender, RoutedEventArgs e)
        {
            createReport(deviceID[3], deviceNames[3].Text, operatorNames[3].Text, dbNames[3], dbNamesRunning[3], DateTime.Now.AddMonths(-1), DateTime.Now, true);
        }

        private void Report5_monthlyReport_Click(object sender, RoutedEventArgs e)
        {
            createReport(deviceID[4], deviceNames[4].Text, operatorNames[4].Text, dbNames[4], dbNamesRunning[4], DateTime.Now.AddMonths(-1), DateTime.Now, true);
        }

        private void Report6_monthlyReport_Click(object sender, RoutedEventArgs e)
        {
            createReport(deviceID[5], deviceNames[5].Text, operatorNames[5].Text, dbNames[5], dbNamesRunning[5], DateTime.Now.AddMonths(-1), DateTime.Now, true);
        }

        private void Report7_monthlyReport_Click(object sender, RoutedEventArgs e)
        {
            createReport(deviceID[6], deviceNames[6].Text, operatorNames[6].Text, dbNames[6], dbNamesRunning[6], DateTime.Now.AddMonths(-1), DateTime.Now, true);
        }

        private void Report8_monthlyReport_Click(object sender, RoutedEventArgs e)
        {
            createReport(deviceID[7], deviceNames[7].Text, operatorNames[7].Text, dbNames[7], dbNamesRunning[7], DateTime.Now.AddMonths(-1), DateTime.Now, true);

        }

        private void Report9_monthlyReport_Click(object sender, RoutedEventArgs e)
        {
            createReport(deviceID[8], deviceNames[8].Text, operatorNames[8].Text, dbNames[8], dbNamesRunning[8], DateTime.Now.AddMonths(-1), DateTime.Now, true);

        }

        private void Report10_monthlyReport_Click(object sender, RoutedEventArgs e)
        {
            createReport(deviceID[9], deviceNames[9].Text, operatorNames[9].Text, dbNames[9], dbNamesRunning[9], DateTime.Now.AddMonths(-1), DateTime.Now, true);
        }

        //---------------------------------------------------------------------------------------

        private void Report1_yearlyReport_Click(object sender, RoutedEventArgs e)
        {
            createReport(deviceID[0], deviceNames[0].Text, operatorNames[0].Text, dbNames[0], dbNamesRunning[0], DateTime.Now.AddYears(-1), DateTime.Now, true);
        }

        private void Report2_yearlyReport_Click(object sender, RoutedEventArgs e)
        {
            createReport(deviceID[1], deviceNames[1].Text, operatorNames[1].Text, dbNames[1], dbNamesRunning[1], DateTime.Now.AddMonths(-1), DateTime.Now, true);
        }

        private void Report3_yearlyReport_Click(object sender, RoutedEventArgs e)
        {
            createReport(deviceID[2], deviceNames[2].Text, operatorNames[2].Text, dbNames[2], dbNamesRunning[2], DateTime.Now.AddYears(-1), DateTime.Now, true);
        }

        private void Report4_yearlyReport_Click(object sender, RoutedEventArgs e)
        {
            createReport(deviceID[3], deviceNames[3].Text, operatorNames[3].Text, dbNames[3], dbNamesRunning[3], DateTime.Now.AddYears(-1), DateTime.Now, true);
        }

        private void Report5_yearlyReport_Click(object sender, RoutedEventArgs e)
        {
            createReport(deviceID[4], deviceNames[4].Text, operatorNames[4].Text, dbNames[4], dbNamesRunning[4], DateTime.Now.AddYears(-1), DateTime.Now, true);
        }

        private void Report6_yearlyReport_Click(object sender, RoutedEventArgs e)
        {
            createReport(deviceID[5], deviceNames[5].Text, operatorNames[5].Text, dbNames[5], dbNamesRunning[5], DateTime.Now.AddYears(-1), DateTime.Now, true);
        }

        private void Report7_yearlyReport_Click(object sender, RoutedEventArgs e)
        {
            createReport(deviceID[6], deviceNames[6].Text, operatorNames[6].Text, dbNames[6], dbNamesRunning[6], DateTime.Now.AddYears(-1), DateTime.Now, true);
        }

        private void Report8_yearlyReport_Click(object sender, RoutedEventArgs e)
        {
            createReport(deviceID[7], deviceNames[7].Text, operatorNames[7].Text, dbNames[7], dbNamesRunning[7], DateTime.Now.AddYears(-1), DateTime.Now, true);
        }

        private void Report9_yearlyReport_Click(object sender, RoutedEventArgs e)
        {
            createReport(deviceID[8], deviceNames[8].Text, operatorNames[8].Text, dbNames[8], dbNamesRunning[8], DateTime.Now.AddYears(-1), DateTime.Now, true);
        }

        private void Report10_yearlyReport_Click(object sender, RoutedEventArgs e)
        {
            createReport(deviceID[9], deviceNames[9].Text, operatorNames[9].Text, dbNames[9], dbNamesRunning[9], DateTime.Now.AddYears(-1), DateTime.Now, true);
        }

        private void createReport(string deviceID, string deviceName, string operatorName, string dbName, string dbRunningName, DateTime date1, DateTime date2, bool isWAnted)
        {
            try
            {
                Report report = new Report(deviceID, deviceName, operatorName, dbName, dbRunningName, date1, date2, isWAnted);
                report.Show();
            }
            catch (ObjectDisposedException) { }
        }

        private void Compare(object sender, RoutedEventArgs e)
        {
            if(countSelected() != 2)
            {
                MessageBox.Show("2 adet cihazın seçili olması gerekmektedir", "system");
                return;
            }

            List<int> orders = new List<int>();

            for (int i = 0; i < cards.Length; i++)
            {
                if (cards[i].Background == Brushes.Aqua)
                    orders.Add(i);
            }

            ReportCompareShowDate reportCompare = new ReportCompareShowDate(deviceID, deviceName, operatorsName, dbNames, dbNamesRunning, orders[0], orders[1]);
            reportCompare.ShowDialog();
        }

        //------------------------------------------------------------------------------

        private Int16 countSelected()
        {
            Int16 selectedCounter = 0;
            for(int i = 0; i < cards.Length; i++)
            {
                if (cardsBool[i])
                    selectedCounter++;
            }

            if(selectedCounter > 0)
            {
                borderSelectedReport.Visibility = Visibility.Visible;
                selectedReportAmount.Content = string.Format(rm.GetString("selectedReport"), selectedCounter);
            }
            else
            {
                borderSelectedReport.Visibility = Visibility.Collapsed;
                buttonCompare.Visibility = Visibility.Collapsed;
            }
            
            if (selectedCounter > 1)
            {
                buttonCompare.Visibility = Visibility.Visible;
            }
            else
            {
                buttonCompare.Visibility = Visibility.Collapsed;
            }

            return selectedCounter;
        }
    }
}
