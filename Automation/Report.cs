using Microsoft.Reporting.WinForms;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Windows.Forms;

namespace Automation
{
    public partial class Report : Form
    {
        Assembly assembly;
        ResourceManager rm;
        CultureInfo cultureInfo;
        string pathToDBFile = Properties.Resources.ConnectionString;
        private DataTable dataTable;
        DateTime dateBegin, dateEnd;
        string DB;
        ReportParameterCollection collection = new ReportParameterCollection();
        bool checkDate = false;
        double average;

        public Report(string deviceID, string deviceName, string operatorName, string dbName, string dbRunningName, DateTime dateFirst, DateTime dateLast, bool checkReceived)
        {
            InitializeComponent();
            assembly = Assembly.Load("Automation");
            rm = new ResourceManager("Automation.Languages.language", assembly);
            cultureInfo = new CultureInfo(Properties.Settings.Default.language);
            dateBegin = dateFirst;
            dateEnd = dateLast;
            DB = dbName;
            setAverages();
            checkDate = checkReceived;
            collection.Add(new ReportParameter("DeviceNameTitle", "Cihaz Adı:"));
            collection.Add(new ReportParameter("DeviceName", deviceName));
            collection.Add(new ReportParameter("OperatorNameTitle", "Operatör Adı:"));
            collection.Add(new ReportParameter("OperatorName", operatorName));
            collection.Add(new ReportParameter("DeviceInformation", "Cihaz Bilgileri"));

            if (!checkReceived)
            {
                using (MySqlConnection localDbConnection = new MySqlConnection(pathToDBFile))
                {
                    MySqlDataAdapter dataAdapter = new MySqlDataAdapter("SELECT * FROM " + DB, localDbConnection);
                    dataTable = new DataTable();
                    dataAdapter.Fill(dataTable);
                    ReportDataSource reportDataSource = new ReportDataSource("DataSet1", dataTable);
                    this.reportViewer1.LocalReport.DisplayName = rm.GetString("report", cultureInfo) + "_" + DateTime.Now.ToString("dd_MM_yyyy");
                    this.reportViewer1.LocalReport.DataSources.Clear();
                    this.reportViewer1.LocalReport.DataSources.Add(reportDataSource);
                    this.reportViewer1.LocalReport.Refresh();
                    this.reportViewer1.RefreshReport();
                }
            }
            else
            {
                using (MySqlConnection localDbConnection = new MySqlConnection(pathToDBFile))
                {
                    string query = "SELECT * FROM " + DB + " WHERE Date >= @Date1 AND Date <= @Date2";
                    MySqlCommand command = new MySqlCommand(query, localDbConnection);
                    command.Parameters.AddWithValue("@Date1", dateBegin);
                    command.Parameters.AddWithValue("@Date2", dateEnd);
                    MySqlDataAdapter dataAdapter = new MySqlDataAdapter(command);
                    dataTable = new DataTable();
                    dataAdapter.Fill(dataTable);
                    ReportDataSource reportDataSource = new ReportDataSource("DataSet1", dataTable);
                    this.reportViewer1.LocalReport.DisplayName = rm.GetString("report", cultureInfo) + "_" + DateTime.Now.ToString("dd_MM_yyyy");
                    this.reportViewer1.LocalReport.DataSources.Clear();
                    this.reportViewer1.LocalReport.DataSources.Add(reportDataSource);
                    this.reportViewer1.LocalReport.Refresh();
                    this.reportViewer1.RefreshReport();
                }
            }
        }

        private void setAverages()
        {
            using (MySqlConnection localDbConnection = new MySqlConnection(pathToDBFile))
            {
                localDbConnection.Open();

                MySqlCommand commandForWorkHour = new MySqlCommand("SELECT SUM(Worked) FROM " + DB, localDbConnection);

                try
                {
                    int workHourLocal = Convert.ToInt32(commandForWorkHour.ExecuteScalar());

                    MySqlCommand commandForLength = new MySqlCommand("SELECT SUM(Value2) FROM " + DB, localDbConnection);
                    int lengthLocal = Convert.ToInt32(commandForLength.ExecuteScalar());

                    average = (lengthLocal / workHourLocal);

                    double averagePlus = average + (average * 20 / 100);
                    double averageMinus = average - (average * 20 / 100);
                    collection.Add(new ReportParameter("AveragePlus", averagePlus.ToString()));
                    collection.Add(new ReportParameter("AverageMinus", averageMinus.ToString()));
                }
                catch (Exception ex) {
                    this.Close();
                    MessageBox.Show(rm.GetString("nullDb"), rm.GetString("system"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void Report_Load(object sender, EventArgs e)
        {
            if(checkDate == false)
                dbProcess();
            else
            {
                dbProcessDate();
            }
            init();
        }

        private void dbProcess()
        {
            using (MySqlConnection localDbConnection = new MySqlConnection(pathToDBFile))
            {
                localDbConnection.Open();

                MySqlCommand command = new MySqlCommand(string.Format("SELECT COUNT(*) FROM {0}", DB), localDbConnection);
                collection.Add(new ReportParameter("TotalDay", command.ExecuteScalar().ToString()));

                MySqlCommand commandForWorkHour = new MySqlCommand(string.Format("SELECT SUM(Worked) FROM {0}", DB), localDbConnection);
                int workHourLocal = 0;
                try
                {
                    workHourLocal = Convert.ToInt32(commandForWorkHour.ExecuteScalar());
                }
                catch { }
                collection.Add(new ReportParameter("TotalWorkHour", workHourLocal.ToString()));

                MySqlCommand commandForLength = new MySqlCommand(string.Format("SELECT SUM(Value2) FROM {0}", DB), localDbConnection);
                int lengthLocal = 0;
                try
                {
                    lengthLocal = Convert.ToInt32(commandForLength.ExecuteScalar());
                }
                catch { }
                collection.Add(new ReportParameter("TotalLength", lengthLocal.ToString()));

                double result = 0;
                if (workHourLocal != 0)
                    result = lengthLocal / workHourLocal;
                collection.Add(new ReportParameter("LengthPerHour", result.ToString()));
            }

            using (MySqlConnection localDbConnection = new MySqlConnection(pathToDBFile))
            {
                localDbConnection.Open();

                MySqlCommand command = new MySqlCommand(string.Format("SELECT COUNT(*) FROM {0} WHERE NOT (Day = 6 OR Day = 7)", DB), localDbConnection);
                collection.Add(new ReportParameter("WeekdayTotalDays", command.ExecuteScalar().ToString()));

                MySqlCommand commandForWorkHour = new MySqlCommand(string.Format("SELECT SUM(Worked) FROM {0} WHERE NOT (Day = 6 OR Day = 7)", DB), localDbConnection);
                int workHourLocal = 0;
                try
                {
                    workHourLocal = Convert.ToInt32(commandForWorkHour.ExecuteScalar());
                }
                catch { }
                collection.Add(new ReportParameter("WeekdayTotalWorkHours", workHourLocal.ToString()));

                MySqlCommand commandForLength = new MySqlCommand(string.Format("SELECT SUM(Value2) FROM {0} WHERE NOT (Day = 6 OR Day = 7)", DB), localDbConnection);
                int lengthLocal = 0;
                try
                {
                    lengthLocal = Convert.ToInt32(commandForLength.ExecuteScalar());
                }
                catch { }
                collection.Add(new ReportParameter("WeekdayTotalLength", lengthLocal.ToString()));

                double result = 0;
                if (workHourLocal != 0)
                    result = lengthLocal / workHourLocal;
                collection.Add(new ReportParameter("WeekdayTotalLengthPerHours", result.ToString()));
            }

            using (MySqlConnection localDbConnection = new MySqlConnection(pathToDBFile))
            {
                localDbConnection.Open();

                MySqlCommand command = new MySqlCommand(string.Format("SELECT COUNT(*) FROM {0} WHERE Day = 6 OR Day = 7", DB), localDbConnection);
                collection.Add(new ReportParameter("WeekendTotalDays", command.ExecuteScalar().ToString()));

                MySqlCommand commandForWorkHour = new MySqlCommand(string.Format("SELECT SUM(Worked) FROM {0} WHERE Day = 6 OR Day = 7", DB), localDbConnection);
                int workHourLocal = 0;
                try
                {
                    workHourLocal = Convert.ToInt32(commandForWorkHour.ExecuteScalar());
                } catch { }

                collection.Add(new ReportParameter("WeekendTotalWorkHours", workHourLocal.ToString()));

                MySqlCommand commandForLength = new MySqlCommand(string.Format("SELECT SUM(Value2) FROM {0} WHERE Day = 6 OR Day = 7", DB), localDbConnection);
                int lengthLocal = 0;
                try
                {
                    lengthLocal = Convert.ToInt32(commandForLength.ExecuteScalar());
                }
                catch { }
                collection.Add(new ReportParameter("WeekendTotalLength", lengthLocal.ToString()));

                double result = 0;
                if(workHourLocal != 0)
                    result = lengthLocal / workHourLocal;
                collection.Add(new ReportParameter("WeekendTotalLengthPerHour", result.ToString()));
            }
        }

        private void dbProcessDate()
        {
            using (MySqlConnection localDbConnection = new MySqlConnection(pathToDBFile))
            {
                localDbConnection.Open();

                MySqlCommand command = new MySqlCommand(string.Format("SELECT COUNT(*) FROM {0} WHERE Date >= @Date1 AND Date <= @Date2", DB), localDbConnection);
                command.Parameters.AddWithValue("@Date1", dateBegin);
                command.Parameters.AddWithValue("@Date2", dateEnd);
                collection.Add(new ReportParameter("TotalDay", command.ExecuteScalar().ToString()));

                MySqlCommand commandForWorkHour = new MySqlCommand(string.Format("SELECT SUM(Worked) FROM {0} WHERE Date >= @Date1 AND Date <= @Date2", DB), localDbConnection);
                commandForWorkHour.Parameters.AddWithValue("@Date1", dateBegin);
                commandForWorkHour.Parameters.AddWithValue("@Date2", dateEnd);
                int workHourLocal = 0;
                try
                {
                    workHourLocal = Convert.ToInt32(commandForWorkHour.ExecuteScalar());
                }
                catch { }

                collection.Add(new ReportParameter("TotalWorkHour", workHourLocal.ToString()));

                MySqlCommand commandForLength = new MySqlCommand(string.Format("SELECT SUM(Value2) FROM {0} WHERE Date >= @Date1 AND Date <= @Date2", DB), localDbConnection);
                commandForLength.Parameters.AddWithValue("@Date1", dateBegin);
                commandForLength.Parameters.AddWithValue("@Date2", dateEnd);
                int lengthLocal = 0;
                try
                {
                    lengthLocal = Convert.ToInt32(commandForLength.ExecuteScalar());
                }
                catch { }
                collection.Add(new ReportParameter("TotalLength", lengthLocal.ToString()));

                double result = 0;
                if (workHourLocal != 0)
                    result = lengthLocal / workHourLocal;
                collection.Add(new ReportParameter("LengthPerHour", result.ToString()));
            }

            using (MySqlConnection localDbConnection = new MySqlConnection(pathToDBFile))
            {
                localDbConnection.Open();

                MySqlCommand command = new MySqlCommand(string.Format("SELECT COUNT(*) FROM {0} WHERE (Date >= @Date1 AND Date <= @Date2) AND NOT (Day = 6 OR Day = 7)", DB), localDbConnection);
                command.Parameters.AddWithValue("@Date1", dateBegin);
                command.Parameters.AddWithValue("@Date2", dateEnd);
                collection.Add(new ReportParameter("WeekdayTotalDays", command.ExecuteScalar().ToString()));

                MySqlCommand commandForWorkHour = new MySqlCommand(string.Format("SELECT SUM(Worked) FROM {0} WHERE (Date >= @Date1 AND Date <= @Date2) AND NOT (Day = 6 OR Day = 7)", DB), localDbConnection);
                commandForWorkHour.Parameters.AddWithValue("@Date1", dateBegin);
                commandForWorkHour.Parameters.AddWithValue("@Date2", dateEnd);
                int workHourLocal = 0;
                try
                {
                    workHourLocal = Convert.ToInt32(commandForWorkHour.ExecuteScalar());
                }
                catch { }
                collection.Add(new ReportParameter("WeekdayTotalWorkHours", workHourLocal.ToString()));

                MySqlCommand commandForLength = new MySqlCommand(string.Format("SELECT SUM(Value2) FROM {0} WHERE (Date >= @Date1 AND Date <= @Date2) AND NOT (Day = 6 OR Day = 7)", DB), localDbConnection);
                commandForLength.Parameters.AddWithValue("@Date1", dateBegin);
                commandForLength.Parameters.AddWithValue("@Date2", dateEnd);
                int lengthLocal = 0;
                try
                {
                    lengthLocal = Convert.ToInt32(commandForLength.ExecuteScalar());
                }
                catch { }
                collection.Add(new ReportParameter("WeekdayTotalLength", lengthLocal.ToString()));

                double result = 0;
                if (workHourLocal != 0)
                    result = lengthLocal / workHourLocal;
                collection.Add(new ReportParameter("WeekdayTotalLengthPerHours", result.ToString()));
            }

            using (MySqlConnection localDbConnection = new MySqlConnection(pathToDBFile))
            {
                localDbConnection.Open();

                MySqlCommand command = new MySqlCommand(string.Format("SELECT COUNT(*) FROM {0} WHERE (Day = 6 OR Day = 7) AND (Date >= @Date1 AND Date <= @Date2)", DB), localDbConnection);
                command.Parameters.AddWithValue("@Date1", dateBegin);
                command.Parameters.AddWithValue("@Date2", dateEnd);
                collection.Add(new ReportParameter("WeekendTotalDays", command.ExecuteScalar().ToString()));

                MySqlCommand commandForWorkHour = new MySqlCommand(string.Format("SELECT SUM(Worked) FROM {0} WHERE (Day = 6 OR Day = 7) AND (Date >= @Date1 AND Date <= @Date2)", DB), localDbConnection);
                commandForWorkHour.Parameters.AddWithValue("@Date1", dateBegin);
                commandForWorkHour.Parameters.AddWithValue("@Date2", dateEnd);
                int workHourLocal = 0;
                try
                {
                    workHourLocal = Convert.ToInt32(commandForWorkHour.ExecuteScalar());
                }
                catch { }
                collection.Add(new ReportParameter("WeekendTotalWorkHours", workHourLocal.ToString()));

                MySqlCommand commandForLength = new MySqlCommand(string.Format("SELECT SUM(Value2) FROM {0} WHERE (Day = 6 OR Day = 7) AND (Date >= @Date1 AND Date <= @Date2)", DB), localDbConnection);
                commandForLength.Parameters.AddWithValue("@Date1", dateBegin);
                commandForLength.Parameters.AddWithValue("@Date2", dateEnd);
                int lengthLocal = 0;
                try
                {
                    lengthLocal = Convert.ToInt32(commandForLength.ExecuteScalar());
                }
                catch { }
                collection.Add(new ReportParameter("WeekendTotalLength", lengthLocal.ToString()));

                double result = 0;
                if (workHourLocal != 0)
                    result = lengthLocal / workHourLocal;
                collection.Add(new ReportParameter("WeekendTotalLengthPerHour", result.ToString()));
            }
        }

        private void init()
        {
            using (MySqlConnection localDbConnection = new MySqlConnection(pathToDBFile))
            {
                try
                {
                    localDbConnection.Open();

                    MySqlCommand command = new MySqlCommand("SELECT COUNT(*) FROM " + DB, localDbConnection);
                    int result = Convert.ToInt32(command.ExecuteScalar());
                    //MessageBox.Show(result.ToString());
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            }

            collection.Add(new ReportParameter("Weekend", "Hafta Sonu"));
            collection.Add(new ReportParameter("UpTolerance", "Toleransın Üzerinde"));
            collection.Add(new ReportParameter("DownTolerance", "Toleransın Altında"));
            collection.Add(new ReportParameter("Monday", "Pazartesi"));
            collection.Add(new ReportParameter("Tuesday", "Salı"));
            collection.Add(new ReportParameter("Wednesday", "Çarşamba"));
            collection.Add(new ReportParameter("Thursday", "Perşembe"));
            collection.Add(new ReportParameter("Friday", "Cuma"));
            collection.Add(new ReportParameter("Saturday", "Cumartesi"));
            collection.Add(new ReportParameter("Sunday", "Pazar"));
            collection.Add(new ReportParameter("Day", "Gün"));
            collection.Add(new ReportParameter("Date", "Tarih"));
            collection.Add(new ReportParameter("Length", "Metraj"));
            collection.Add(new ReportParameter("Work", "Çalışma Saati"));
            collection.Add(new ReportParameter("WorkTable", "Makine Çalışma Verileri"));
            collection.Add(new ReportParameter("TotalDayTitle", "Toplam Gün Sayısı:"));
            collection.Add(new ReportParameter("TotalWorkHourTitle", "Toplam Çalışma Saati:"));
            collection.Add(new ReportParameter("TotalSituation", "Toplam Durum"));
            collection.Add(new ReportParameter("TotalLengthTitle", "Toplam Metraj:"));
            collection.Add(new ReportParameter("LengthPerHourTitle", "Birim Saatte Metraj:"));
            collection.Add(new ReportParameter("WeekdaySituation", "Haftaiçi Durum"));
            collection.Add(new ReportParameter("WeekdayTotalDaysTitle", "Toplam Gün Sayısı:"));
            collection.Add(new ReportParameter("WeekdayTotalWorkHoursTitle", "Toplam Çalışma Saati:"));
            collection.Add(new ReportParameter("WeekdayTotalLengthTitle", "Toplam Metraj:"));
            collection.Add(new ReportParameter("WeekdayTotalLengthPerHoursTitle", "Birim Saatte Metraj:"));
            collection.Add(new ReportParameter("WeekendSituation", "HaftaSonu Durum"));
            collection.Add(new ReportParameter("WeekendTotalDaysTitle", "Toplam Gün Sayısı:"));
            collection.Add(new ReportParameter("WeekendTotalWorkHoursTitle", "Toplam Çalışma Saati:"));
            collection.Add(new ReportParameter("WeekendTotalLengthTitle", "Toplam Metraj:"));

            //collection.Add(new ReportParameter("WeekendTotalLength", "3128 m"));
            collection.Add(new ReportParameter("WeekendTotalLengthPerHourTitle", "Birim Saatte Metraj:"));

            this.reportViewer1.LocalReport.SetParameters(collection);
            this.reportViewer1.RefreshReport();
        }
    }
}
