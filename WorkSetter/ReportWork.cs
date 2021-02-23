using Microsoft.Reporting.WinForms;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WorkSetter
{
    public partial class ReportWork : Form
    {
        ReportParameterCollection collection = new ReportParameterCollection();

        public ReportWork(string deviceName, string operatorName, string DB, DateTime dateBegin, DateTime dateEnd)
        {
            InitializeComponent();
            collection.Add(new ReportParameter("DeviceName", deviceName));
            collection.Add(new ReportParameter("OperatorName", operatorName));


            using (MySqlConnection localDbConnection = new MySqlConnection(Path.toDB))
            {
                string query = "SELECT OperatorName, DateWork AS Date, Length, WorkingOn AS Work FROM " + DB + " WHERE DateWork >= @Date1 AND DateWork <= @Date2";
                MySqlCommand command = new MySqlCommand(query, localDbConnection);
                command.Parameters.AddWithValue("@Date1", dateBegin);
                command.Parameters.AddWithValue("@Date2", dateEnd);
                MySqlDataAdapter dataAdapter = new MySqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                DataTable dataTableToSet = new DataTable();
                dataAdapter.Fill(dataTable);

                ReportDataSource reportDataSource = new ReportDataSource("DataSet", dataTable);
                this.reportViewer1.LocalReport.DisplayName = "Rapor" + "_" + DateTime.Now.ToString("dd_MM_yyyy");
                this.reportViewer1.LocalReport.DataSources.Clear();
                this.reportViewer1.LocalReport.DataSources.Add(reportDataSource);
                this.reportViewer1.LocalReport.Refresh();
                this.reportViewer1.RefreshReport();
            }
        }

        private void ReportWork_Load(object sender, EventArgs e)
        {

        }
    }
}
