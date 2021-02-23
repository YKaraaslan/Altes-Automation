using MaterialDesignThemes.Wpf;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace WorkSetter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Card[] cards;
        string pathToDBFile = Path.toDB;
        List<string> deviceID = new List<string>();
        TextBlock[] machineNames;
        TextBlock[] operatorNames;
        TextBlock[] machineWorks;
        TextBlock[] timePasseds;
        Rectangle[] connections;
        BackgroundWorker worker = new BackgroundWorker();
        List<string> dbNamesWorkingOn = new List<string>();
        List<string> dbNames = new List<string>();

        const int SECOND = 1;
        const int MINUTE = 60 * SECOND;
        const int HOUR = 60 * MINUTE;
        const int DAY = 24 * HOUR;
        const int MONTH = 30 * DAY;

        public MainWindow()
        {
            InitializeComponent();
            cards = new Card[] { card1, card2, card3, card4, card5,
                card6, card7, card8, card9, card10 };

            machineNames = new TextBlock[] { machine1, machine2, machine3, machine4, machine5,
                machine6, machine7, machine8, machine9, machine10 };

            operatorNames = new TextBlock[] { operator1, operator2, operator3, operator4, operator5,
                operator6, operator7, operator8, operator9, operator10 };

            machineWorks = new TextBlock[] { work1, work2, work3, work4, work5,
                work6, work7, work8, work9, work10 };

            timePasseds = new TextBlock[] { timePassed1, timePassed2, timePassed3, timePassed4, timePassed5,
                timePassed6, timePassed7, timePassed8, timePassed9, timePassed10 };

            connections = new Rectangle[] { connection1, connection2, connection3, connection4, connection5,
                connection6, connection7, connection8, connection9, connection10 };

            worker.DoWork += Worker_DoWork;
            worker.RunWorkerAsync();
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    try
                    {
                        init();
                    }
                    catch { }
                }));
                Thread.Sleep(1000);
            }
        }

        private void init()
        {
            using (MySqlConnection localDbConnection = new MySqlConnection(pathToDBFile))
            {
                try
                {
                    localDbConnection.Open();
                    connection.Fill = Brushes.Green;
                    connectionLabel.Content = "Bağlandı!";

                    MySqlCommand command = new MySqlCommand("SELECT ID, Device, Operator, WorkingOn, WorkingOnDb, DbName, WorkingOnTime, Status FROM Devices ORDER BY ID ASC", localDbConnection);
                    MySqlDataAdapter dataAdapter = new MySqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    dataAdapter.Fill(dataTable);

                    deviceID.Clear();
                    dbNamesWorkingOn.Clear();

                    for (int i = 0; i < dataTable.Rows.Count; i++)
                    {
                        deviceID.Add(dataTable.Rows[i][0].ToString());
                        machineNames[i].Text = dataTable.Rows[i][1].ToString();
                        operatorNames[i].Text = dataTable.Rows[i][2].ToString();
                        machineWorks[i].Text = dataTable.Rows[i][3].ToString();
                        dbNamesWorkingOn.Add(dataTable.Rows[i][4].ToString());
                        dbNames.Add(dataTable.Rows[i][5].ToString());
                        timePasseds[i].Text = CalculateTime(Convert.ToDateTime(dataTable.Rows[i][6]));
                        if (dataTable.Rows[i][7].ToString() == "1")
                        {
                            connections[i].Fill = Brushes.Green;
                        }
                        else
                        {
                            connections[i].Fill = Brushes.Red;
                        }
                        cards[i].Visibility = Visibility.Visible;
                    }
                }
                catch (Exception ex)
                {
                    connection.Fill = Brushes.Red;
                    connectionLabel.Content = "Bağlantı sağlanamadı...";
                }
            }
        }

        private void Work_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = sender as Button;

            try
            {
                switch (clickedButton.Name)
                {
                    case "machineWork1":
                        workHistory(dbNamesWorkingOn[0], machineNames[0].Text, operatorNames[0].Text);
                        break;
                    case "machineWork2":
                        workHistory(dbNamesWorkingOn[1], machineNames[1].Text, operatorNames[1].Text);
                        break;
                    case "machineWork3":
                        workHistory(dbNamesWorkingOn[2], machineNames[2].Text, operatorNames[2].Text);
                        break;
                    case "machineWork4":
                        workHistory(dbNamesWorkingOn[3], machineNames[3].Text, operatorNames[3].Text);
                        break;
                    case "machineWork5":
                        workHistory(dbNamesWorkingOn[4], machineNames[4].Text, operatorNames[4].Text);
                        break;
                    case "machineWork6":
                        workHistory(dbNamesWorkingOn[5], machineNames[5].Text, operatorNames[5].Text);
                        break;
                    case "machineWork7":
                        workHistory(dbNamesWorkingOn[6], machineNames[6].Text, operatorNames[6].Text);
                        break;
                    case "machineWork8":
                        workHistory(dbNamesWorkingOn[7], machineNames[7].Text, operatorNames[7].Text);
                        break;
                    case "machineWork9":
                        workHistory(dbNamesWorkingOn[8], machineNames[8].Text, operatorNames[8].Text);
                        break;
                    case "machineWork10":
                        workHistory(dbNamesWorkingOn[9], machineNames[9].Text, operatorNames[9].Text);
                        break;
                }
            }
            catch { }
        }

        private void workHistory(string dbNamesWorkingOn, string machineName, string operatorName)
        {
            WorkHistory workHistory = new WorkHistory(dbNamesWorkingOn, machineName, operatorName);
            workHistory.Show();
        }

        private void Report_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = sender as Button;

            try
            {
                switch (clickedButton.Name)
                {
                    case "machineReport1":
                        Report(machineNames[0].Text, operatorNames[0].Text, dbNames[0]);
                        break;
                    case "machineReport2":
                        Report(machineNames[1].Text, operatorNames[1].Text, dbNames[1]);
                        break;
                    case "machineReport3":
                        Report(machineNames[2].Text, operatorNames[2].Text, dbNames[2]);
                        break;
                    case "machineReport4":
                        Report(machineNames[3].Text, operatorNames[3].Text, dbNames[3]);
                        break;
                    case "machineReport5":
                        Report(machineNames[4].Text, operatorNames[4].Text, dbNames[4]);
                        break;
                    case "machineReport6":
                        Report(machineNames[5].Text, operatorNames[5].Text, dbNames[5]);
                        break;
                    case "machineReport7":
                        Report(machineNames[6].Text, operatorNames[6].Text, dbNames[6]);
                        break;
                    case "machineReport8":
                        Report(machineNames[7].Text, operatorNames[7].Text, dbNames[7]);
                        break;
                    case "machineReport9":
                        Report(machineNames[8].Text, operatorNames[8].Text, dbNames[8]);
                        break;
                    case "machineReport10":
                        Report(machineNames[9].Text, operatorNames[9].Text, dbNames[9]);
                        break;
                }
            }
            catch { }
        }

        private void Report(string name, string operatorName, string db)
        {
            ReportChooser reportChooser = new ReportChooser(name, operatorName, db);
            reportChooser.ShowDialog();
        }

        private string CalculateTime(DateTime time)
        {
            var ts = new TimeSpan(DateTime.Now.Ticks - time.Ticks);
            double delta = Math.Abs(ts.TotalSeconds);

            if (delta < 1 * MINUTE)
                return ts.Seconds + " saniye";

            if (delta < 60 * MINUTE)
                return ts.Minutes + " dakika";

            if (delta < 24 * HOUR)
                return ts.Hours + " saat";

            if (delta < 30 * DAY)
                return ts.Days + " gün";

            if (delta < 12 * MONTH)
            {
                int months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                return months + " ay";
            }
            else
            {
                int years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
                return years + " yıl";
            }
        }
    }
}
