using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Reflection;
using System.Resources;
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
    /// Interaction logic for WorkHistory.xaml
    /// </summary>
    public partial class WorkHistory : Window
    {
        string pathToDBFile = Path.toDB;
        string dbWorkingOn, machine, operators;

        public WorkHistory(string dbWorkingOnReceived, string machineName, string operatorName)
        {
            InitializeComponent();
            dbWorkingOn = dbWorkingOnReceived;
            machine = machineName;
            operators = operatorName;

            using (MySqlConnection connection = new MySqlConnection(pathToDBFile + ";convert zero datetime=True"))
            {
                connection.Open();
                MySqlCommand command = new MySqlCommand(string.Format("SELECT * FROM {0}", dbWorkingOnReceived), connection);
                MySqlDataAdapter dataAdapter = new MySqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                dataAdapter.Fill(dataTable);

                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    workHistoryDataGrid.Items.Add(new
                    {
                        Operator = dataTable.Rows[i][1].ToString(),
                        Work = dataTable.Rows[i][2].ToString(),
                        Length = dataTable.Rows[i][3].ToString(),
                        Date = Convert.ToDateTime(dataTable.Rows[i][4]).ToString("dd/MM/yyyy HH:mm")
                    });
                }
            }
        }

        private void Report_Click(object sender, RoutedEventArgs e)
        {
            ReportWorkChooser reportChooser = new ReportWorkChooser(machine, operators, dbWorkingOn);
            reportChooser.ShowDialog();
        }
    }
}
