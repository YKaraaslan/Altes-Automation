using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Windows;
using System.Windows.Controls;

namespace Automation
{
    /// <summary>
    /// Interaction logic for Operators.xaml
    /// </summary>
    public partial class Operators : UserControl
    {
        List<OperatorItems> items = new List<OperatorItems>();
        Assembly assembly;
        ResourceManager rm;
        CultureInfo cultureInfo;
        string pathToDBFile = Properties.Resources.ConnectionString;

        public Operators()
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

            using (MySqlConnection localDbConnection = new MySqlConnection(pathToDBFile))
            {
                localDbConnection.Open();

                MySqlCommand command = new MySqlCommand("SELECT * FROM Operators ORDER BY ID ASC", localDbConnection);
                MySqlDataAdapter dataAdapter = new MySqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                dataAdapter.Fill(dataTable);

                localDbConnection.Close();
                int counter = 0;

                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    items.Add(new OperatorItems
                    {
                        OperatorName = dataTable.Rows[i][1].ToString(),
                        OperatorID = Convert.ToInt32(dataTable.Rows[i][0])
                    });
                    counter++;
                }

                operatorNumber.Content = counter.ToString();
            }

            icTodoList.ItemsSource = items;
            icTodoList.Items.Refresh();
        }

        private void settings(object sender, RoutedEventArgs e)
        {
            OperatorItems person = ((Button)sender).Tag as OperatorItems;
            OperatorsSettings operatorsSettings = new OperatorsSettings(person.OperatorName);
            operatorsSettings.Closed += Closed;
            operatorsSettings.ShowDialog();
        }

        private void add_Click(object sender, RoutedEventArgs e)
        {
            OperatorsAdd operatorsAdd = new OperatorsAdd();
            operatorsAdd.Closed += Closed;
            operatorsAdd.ShowDialog();
        }

        private void Closed(object sender, EventArgs e)
        {
            init();
        }
    }

    public class OperatorItems
    {
        public string OperatorName { get; set; }
        public int OperatorID { get; set; }
    }
}
