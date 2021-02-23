using LiveCharts;
using LiveCharts.Wpf;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Data.SqlTypes;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Windows;
using System.Windows.Media;

namespace Automation
{
    /// <summary>
    /// Interaction logic for DevicesShow.xaml
    /// </summary>
    public partial class DevicesShow : Window
    {
        Assembly assembly;
        ResourceManager rm;
        CultureInfo cultureInfo;
        string pathToDBFile = Properties.Resources.ConnectionString;
        public SeriesCollection SeriesCollection { get; set; }
        string DB, DBRunning;

        public DevicesShow(int number, string id, string dbName, string dbNameRunning, string slave, string reading1, string reading2, string reading3, string reading4, string reading5)
        {
            InitializeComponent();
            assembly = Assembly.Load("Automation");
            rm = new ResourceManager("Automation.Languages.language", assembly);
            cultureInfo = new CultureInfo(Properties.Settings.Default.language);
            setLength(dbName);
            DB = dbName;
            DBRunning = dbNameRunning;
            setGraph();

            switch (number)
            {
                case 0:
                    Device1EventArgs.event1 -= Device1EventArgs_event1;
                    Device1EventArgs.event1 += Device1EventArgs_event1;
                    break;
                case 1:
                    Device2EventArgs.event1 -= Device2EventArgs_event1;
                    Device2EventArgs.event1 += Device2EventArgs_event1;
                    break;
                case 2:
                    Device3EventArgs.event1 -= Device3EventArgs_event1;
                    Device3EventArgs.event1 += Device3EventArgs_event1;
                    break;
                case 3:
                    Device4EventArgs.event1 -= Device4EventArgs_event1;
                    Device4EventArgs.event1 += Device4EventArgs_event1;
                    break;

                case 4:
                    Device5EventArgs.event1 -= Device5EventArgs_event1;
                    Device5EventArgs.event1 += Device5EventArgs_event1;
                    break;
                case 5:
                    Device6EventArgs.event1 -= Device6EventArgs_event1;
                    Device6EventArgs.event1 += Device6EventArgs_event1;
                    break;
                case 6:
                    Device7EventArgs.event1 -= Device7EventArgs_event1;
                    Device7EventArgs.event1 += Device7EventArgs_event1;
                    break;
                case 7:
                    Device8EventArgs.event1 -= Device8EventArgs_event1;
                    Device8EventArgs.event1 += Device8EventArgs_event1;
                    break;
                case 8:
                    Device9EventArgs.event1 -= Device9EventArgs_event1;
                    Device9EventArgs.event1 += Device9EventArgs_event1;
                    break;
                case 9:
                    Device10EventArgs.event1 -= Device10EventArgs_event1;
                    Device10EventArgs.event1 += Device10EventArgs_event1;
                    break;
            }
        }

        private void setLength(string dbName)
        {
            using (MySqlConnection localDbConnection = new MySqlConnection(pathToDBFile))
            {
                localDbConnection.Open();

                try
                {
                    MySqlCommand command = new MySqlCommand(string.Format("SELECT SUM(Value2), AVG(Value2) FROM {0}", dbName), localDbConnection);
                    MySqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        length_average_daily.Text = reader.GetInt32(1).ToString();
                        length_total.Text = reader.GetInt32(0).ToString();
                    }
                }
                catch (SqlNullValueException) { length_average_daily.Text = "0"; length_total.Text = "0"; }
            }
        }

        private void Device10EventArgs_event1(object sender, Device10EventArgs e)
        {
            speed.Text = e.speed.ToString() + " m/dak";
            length.Text = e.length.ToString() + " m";
            if (e.running == true)
            {
                status.Background = Brushes.Green;
            }
            else
            {
                status.Background = Brushes.Red;
            }
        }

        private void Device9EventArgs_event1(object sender, Device9EventArgs e)
        {
            speed.Text = e.speed.ToString() + " m/dak";
            length.Text = e.length.ToString() + " m";
            if (e.running == true)
            {
                status.Background = Brushes.Green;
            }
            else
            {
                status.Background = Brushes.Red;
            }
        }

        private void Device8EventArgs_event1(object sender, Device8EventArgs e)
        {
            speed.Text = e.speed.ToString() + " m/dak";
            length.Text = e.length.ToString() + " m";
            if (e.running == true)
            {
                status.Background = Brushes.Green;
            }
            else
            {
                status.Background = Brushes.Red;
            }
        }

        private void Device7EventArgs_event1(object sender, Device7EventArgs e)
        {
            speed.Text = e.speed.ToString() + " m/dak";
            length.Text = e.length.ToString() + " m";
            if (e.running == true)
            {
                status.Background = Brushes.Green;
            }
            else
            {
                status.Background = Brushes.Red;
            }
        }

        private void Device6EventArgs_event1(object sender, Device6EventArgs e)
        {
            speed.Text = e.speed.ToString() + " m/dak";
            length.Text = e.length.ToString() + " m";
            if (e.running == true)
            {
                status.Background = Brushes.Green;
            }
            else
            {
                status.Background = Brushes.Red;
            }
        }

        private void Device5EventArgs_event1(object sender, Device5EventArgs e)
        {
            speed.Text = e.speed.ToString() + " m/dak";
            length.Text = e.length.ToString() + " m";
            if (e.running == true)
            {
                status.Background = Brushes.Green;
            }
            else
            {
                status.Background = Brushes.Red;
            }
        }

        private void Device4EventArgs_event1(object sender, Device4EventArgs e)
        {
            speed.Text = e.speed.ToString() + " m/dak";
            length.Text = e.length.ToString() + " m";
            if (e.running == true)
            {
                status.Background = Brushes.Green;
            }
            else
            {
                status.Background = Brushes.Red;
            }
        }

        private void Device3EventArgs_event1(object sender, Device3EventArgs e)
        {
            speed.Text = e.speed.ToString() + " m/dak";
            length.Text = e.length.ToString() + " m";
            if (e.running == true)
            {
                status.Background = Brushes.Green;
            }
            else
            {
                status.Background = Brushes.Red;
            }
        }

        private void Device2EventArgs_event1(object sender, Device2EventArgs e)
        {
            speed.Text = e.speed.ToString() + " m/dak";
            length.Text = e.length.ToString() + " m";
            if (e.running == true)
            {
                status.Background = Brushes.Green;
            }
            else
            {
                status.Background = Brushes.Red;
            }
        }

        private void Device1EventArgs_event1(object sender, Device1EventArgs e)
        {
            speed.Text = e.speed.ToString() + " m/dak";
            length.Text = e.length.ToString() + " m";
            if(e.running == true)
            {
                status.Background = Brushes.Green;
            }
            else
            {
                status.Background = Brushes.Red;
            }
        }

        private void setGraph()
        {
            using (MySqlConnection localDbConnection = new MySqlConnection(pathToDBFile))
            {
                localDbConnection.Open();

                MySqlCommand command = new MySqlCommand(string.Format("SELECT Date, Day, Value2 FROM {0}", DB), localDbConnection);
                MySqlDataAdapter dataAdapter = new MySqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                dataAdapter.Fill(dataTable);
                SeriesCollection = new SeriesCollection();
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    if(i < 30)
                    {
                        SeriesCollection.Add(new ColumnSeries
                        {
                            Title = dataTable.Rows[i][0].ToString().Split(' ')[0],
                            Values = new ChartValues<double> { Convert.ToDouble(dataTable.Rows[i][2].ToString())
                        }
                        });
                    }

                    machineInformation.Items.Add(new MachineInfoItems
                    {
                        Date = dataTable.Rows[i][0].ToString(),
                        Day = dataTable.Rows[i][1].ToString(),
                        Length = Convert.ToDouble(dataTable.Rows[i][2])
                    }); 
                    DataContext = this;
                }
            }
        }

        //private void counterClick(object sender, RoutedEventArgs e)
        //{
        //    string[] times = new string[] { "08:00", "08:25", "09:00", "09:45", "10:00", "12:00", "12:45", "17:43", "18:30", "20:15" };
        //    using (MySqlConnection localDbConnection = new MySqlConnection(pathToDBFile)){
        //        localDbConnection.Open();

        //        SqlCommand commandToAddDatabase = new SqlCommand(string.Format("INSERT INTO {0}(ChangeTime, Situation, Dates, Day) " +
        //                   "VALUES(@ChangeTime, @Situation, @Dates, @Day)", DBRunning), localDbConnection);
                
        //        for (int i = 0; i < 10; i++)
        //        {
        //            commandToAddDatabase.Parameters.AddWithValue("@ChangeTime", Convert.ToDateTime(times[i]));
        //            commandToAddDatabase.Parameters.AddWithValue("@Situation", i % 2);
        //            commandToAddDatabase.Parameters.AddWithValue("@Dates", DateTime.Now);
        //            commandToAddDatabase.Parameters.AddWithValue("@Day", DateTime.Now.DayOfWeek);
        //            commandToAddDatabase.ExecuteNonQuery();
        //            commandToAddDatabase.Parameters.Clear();
        //        }
        //    }
        //    MessageBox.Show("kaydedildi");
        //}
    }

    public class MachineInfoItems
    {
        public string Date { get; set; }
        public string Day { get; set; }
        public double Length { get; set; }
    }
}
