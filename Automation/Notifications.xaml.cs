using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Windows;
using System.Windows.Media;

namespace Automation
{
    /// <summary>
    /// Interaction logic for Notifications.xaml
    /// </summary>
    public partial class Notifications : Window
    {
        Assembly assembly;
        ResourceManager rm;
        CultureInfo cultureInfo;
        List<NotificationsItems> items = new List<NotificationsItems>();
        string pathToDBFile = Properties.Resources.ConnectionString;

        public Notifications()
        {
            InitializeComponent();
            assembly = Assembly.Load("Automation");
            rm = new ResourceManager("Automation.Languages.language", assembly);
            cultureInfo = new CultureInfo(Properties.Settings.Default.language);
            init();
        }

        private void init()
        {
            items.Clear();
            icTodoList.ItemsSource = items;
            icTodoList.Items.Refresh();

            using (MySqlConnection localDbConnection = new MySqlConnection(pathToDBFile))
            {
                localDbConnection.Open();

                MySqlCommand command = new MySqlCommand("SELECT DeviceName, Project, DateDeadline FROM projectsToDo WHERE DateDeadline <= @date ORDER BY ID DESC", localDbConnection);
                command.Parameters.AddWithValue("@date", Convert.ToDateTime(DateTime.Now.ToString("dd/MM/yyy HH:mm:ss")));
                MySqlDataAdapter dataAdapter = new MySqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                dataAdapter.Fill(dataTable);

                localDbConnection.Close();

                if (Properties.Settings.Default.language == "tr-TR")
                {
                    for (int i = 0; i < dataTable.Rows.Count; i++)
                    {
                        items.Add(new NotificationsItems()
                        {
                            Content = dataTable.Rows[i][0].ToString() + " cihazında yapılacak " + dataTable.Rows[i][1].ToString() + " projesinin süresi doldu.",
                            Time = dataTable.Rows[i][2].ToString(),
                            Color = Brushes.IndianRed
                        });
                    }
                }
                else
                {
                    for (int i = 0; i < dataTable.Rows.Count; i++)
                    {
                        items.Add(new NotificationsItems()
                        {
                            Content = "Time is up for project " + dataTable.Rows[i][1].ToString() + " on device " + dataTable.Rows[i][0].ToString(),
                            Time = dataTable.Rows[i][2].ToString(),
                            Color = Brushes.IndianRed
                        });
                    }
                }
            }

            using (MySqlConnection localDbConnection = new MySqlConnection(pathToDBFile))
            {
                localDbConnection.Open();

                MySqlCommand command = new MySqlCommand("SELECT DeviceName, Project, DateDeadline FROM projectsToDo WHERE DateDeadline <= @date AND DateDeadline >= @date2 ORDER BY ID DESC", localDbConnection);
                command.Parameters.AddWithValue("@date", Convert.ToDateTime(DateTime.Now.AddDays(1).ToString("dd/MM/yyy HH:mm:ss")));
                command.Parameters.AddWithValue("@date2", Convert.ToDateTime(DateTime.Now.ToString("dd/MM/yyy HH:mm:ss")));
                MySqlDataAdapter dataAdapter = new MySqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                dataAdapter.Fill(dataTable);

                localDbConnection.Close();

                if (Properties.Settings.Default.language == "tr-TR")
                {
                    for (int i = 0; i < dataTable.Rows.Count; i++)
                    {
                        items.Add(new NotificationsItems()
                        {
                            Content = dataTable.Rows[i][0].ToString() + " cihazında yapılacak " + dataTable.Rows[i][1].ToString() + " projesinin son günü.",
                            Time = dataTable.Rows[i][2].ToString(),
                            Color = Brushes.Black
                        });
                    }
                }
                else
                {
                    for (int i = 0; i < dataTable.Rows.Count; i++)
                    {
                        items.Add(new NotificationsItems()
                        {
                            Content = "Last day for project " + dataTable.Rows[i][1].ToString() + " on device " + dataTable.Rows[i][0].ToString(),
                            Time = dataTable.Rows[i][2].ToString(),
                            Color = Brushes.Black
                        });
                    }
                }
            }

            icTodoList.ItemsSource = items;
            icTodoList.Items.Refresh();
        }

        public class NotificationsItems
        {
            public string Content { get; set; }
            public string Time { get; set; }
            public SolidColorBrush Color { get; set; }
        }
    }
}