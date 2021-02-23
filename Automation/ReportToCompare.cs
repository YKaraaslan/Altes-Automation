using Microsoft.Reporting.WinForms;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Windows.Forms;

namespace Automation
{
    public partial class ReportToCompare : Form
    {
        Assembly assembly;
        ResourceManager rm;
        CultureInfo cultureInfo;
        string pathToDBFile = Properties.Resources.ConnectionString;
        DateTime dateBegin, dateEnd;
        List<string> DB;
        ReportParameterCollection collection = new ReportParameterCollection();
        bool checkDate = false;
        int order1, order2;

        public ReportToCompare(List<string> deviceID, List<string> deviceName, List<string> operatorName, List<string> dbName, List<string> dbRunningName, DateTime dateFirst, DateTime dateLast, bool checkReceived, int order1Received, int order2Received)
        {
            InitializeComponent();
            assembly = Assembly.Load("Automation");
            rm = new ResourceManager("Automation.Languages.language", assembly);
            cultureInfo = new CultureInfo(Properties.Settings.Default.language);
            dateBegin = dateFirst;
            dateEnd = dateLast;
            DB = dbName;
            order1 = order1Received;
            order2 = order2Received;
            checkDate = checkReceived;
            collection.Add(new ReportParameter("DeviceNameTitle", "Cihaz Adı:"));
            collection.Add(new ReportParameter("OperatorNameTitle", "Operatör Adı:"));
            collection.Add(new ReportParameter("DeviceInformation", "Cihaz Bilgileri"));

            collection.Add(new ReportParameter("DeviceName1", deviceName[order1]));
            collection.Add(new ReportParameter("OperatorName1", operatorName[order1]));
            collection.Add(new ReportParameter("DeviceName2", deviceName[order2]));
            collection.Add(new ReportParameter("OperatorName2", operatorName[order2]));
        }

        private void ReportToCompare_Load(object sender, EventArgs e)
        {
            if (checkDate == false)
            {
                dbProcess1();
                dbProcess2();
            }
            else
            {
                dbProcessDate1();
                dbProcessDate2();
            }
            init();
            this.reportViewer1.RefreshReport();
        }

        private void init()
        {
            //
            //using (MySqlConnection localDbConnection = new MySqlConnection(pathToDBFile))
            //{
            //    for (int i = 0; i < DB.Length; i++)
            //    {
            //        try
            //        {
            //            localDbConnection.Open();

            //            SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM {0}", localDbConnection);
            //            int result = (int)command.ExecuteScalar();
            //        }
            //        catch {  }
            //    }
            //}
            collection.Add(new ReportParameter("TotalSituation", "Toplam Durum"));
            collection.Add(new ReportParameter("TotalDayTitle", "Toplam Gün Sayısı:"));
            collection.Add(new ReportParameter("TotalWorkHourTitle", "Toplam Çalışma Saati:"));
            collection.Add(new ReportParameter("TotalLengthTitle", "Toplam Metraj:"));
            collection.Add(new ReportParameter("LengthPerHourTitle", "Birim Saatte Metraj:"));
            collection.Add(new ReportParameter("WeekdaySituation", "Haftaiçi Durum"));
            collection.Add(new ReportParameter("WeekendSituation", "Hafta Sonu Durum"));

            this.reportViewer1.LocalReport.SetParameters(collection);
            this.reportViewer1.RefreshReport();
        }

        private void dbProcess1()
        {
            using (MySqlConnection localDbConnection = new MySqlConnection(pathToDBFile))
            {
                localDbConnection.Open();

                MySqlCommand command = new MySqlCommand(string.Format("SELECT COUNT(*) FROM {0}", DB[order1]), localDbConnection);
                collection.Add(new ReportParameter("TotalDay1", command.ExecuteScalar().ToString()));

                MySqlCommand commandForWorkHour = new MySqlCommand(string.Format("SELECT SUM(Worked) FROM {0}", DB[order1]), localDbConnection);
                int workHourLocal = 0;
                try
                {
                    workHourLocal = Convert.ToInt32(commandForWorkHour.ExecuteScalar());
                }
                catch { }
                collection.Add(new ReportParameter("TotalWorkHour1", workHourLocal.ToString()));

                MySqlCommand commandForLength = new MySqlCommand(string.Format("SELECT SUM(Value2) FROM {0}", DB[order1]), localDbConnection);
                int lengthLocal = 0;
                try
                {
                    lengthLocal = Convert.ToInt32(commandForLength.ExecuteScalar());
                }
                catch { }
                collection.Add(new ReportParameter("TotalLength1", lengthLocal.ToString()));

                double result = 0;
                if (workHourLocal != 0)
                    result = lengthLocal / workHourLocal;
                collection.Add(new ReportParameter("LengthPerHour1", result.ToString()));
            }

            using (MySqlConnection localDbConnection = new MySqlConnection(pathToDBFile))
            {
                localDbConnection.Open();

                MySqlCommand command = new MySqlCommand(string.Format("SELECT COUNT(*) FROM {0} WHERE NOT (Day = 6 OR Day = 7)", DB[order1]), localDbConnection);
                collection.Add(new ReportParameter("WeekdayTotalDays1", command.ExecuteScalar().ToString()));

                MySqlCommand commandForWorkHour = new MySqlCommand(string.Format("SELECT SUM(Worked) FROM {0} WHERE NOT (Day = 6 OR Day = 7)", DB[order1]), localDbConnection);
                int workHourLocal = 0;
                try
                {
                    workHourLocal = Convert.ToInt32(commandForWorkHour.ExecuteScalar());
                }
                catch { }
                collection.Add(new ReportParameter("WeekdayTotalWorkHour1", workHourLocal.ToString()));

                MySqlCommand commandForLength = new MySqlCommand(string.Format("SELECT SUM(Value2) FROM {0} WHERE NOT (Day = 6 OR Day = 7)", DB[order1]), localDbConnection);
                int lengthLocal = 0;
                try
                {
                    lengthLocal = Convert.ToInt32(commandForLength.ExecuteScalar());
                }
                catch { }
                collection.Add(new ReportParameter("WeekdayTotalLength1", lengthLocal.ToString()));

                double result = 0;
                if (workHourLocal != 0)
                    result = lengthLocal / workHourLocal;
                collection.Add(new ReportParameter("WeekdayTotalLengthPerHour1", result.ToString()));
            }

            using (MySqlConnection localDbConnection = new MySqlConnection(pathToDBFile))
            {
                localDbConnection.Open();

                MySqlCommand command = new MySqlCommand(string.Format("SELECT COUNT(*) FROM {0} WHERE Day = 6 OR Day = 7", DB[order1]), localDbConnection);
                collection.Add(new ReportParameter("WeekendTotalDays1", command.ExecuteScalar().ToString()));

                MySqlCommand commandForWorkHour = new MySqlCommand(string.Format("SELECT SUM(Worked) FROM {0} WHERE Day = 6 OR Day = 7", DB[order1]), localDbConnection);
                int workHourLocal = 0;
                try
                {
                    workHourLocal = Convert.ToInt32(commandForWorkHour.ExecuteScalar());
                }
                catch { }

                collection.Add(new ReportParameter("WeekendTotalWorkHour1", workHourLocal.ToString()));

                MySqlCommand commandForLength = new MySqlCommand(string.Format("SELECT SUM(Value2) FROM {0} WHERE Day = 6 OR Day = 7", DB[order1]), localDbConnection);
                int lengthLocal = 0;
                try
                {
                    lengthLocal = Convert.ToInt32(commandForLength.ExecuteScalar());
                }
                catch { }
                collection.Add(new ReportParameter("WeekendTotalLength1", lengthLocal.ToString()));

                double result = 0;
                if (workHourLocal != 0)
                    result = lengthLocal / workHourLocal;
                collection.Add(new ReportParameter("WeekendTotalLengthPerHour1", result.ToString()));
            }
        }

        private void dbProcess2()
        {
            using (MySqlConnection localDbConnection = new MySqlConnection(pathToDBFile))
            {
                localDbConnection.Open();

                MySqlCommand command = new MySqlCommand(string.Format("SELECT COUNT(*) FROM {0}", DB[order2]), localDbConnection);
                collection.Add(new ReportParameter("TotalDay2", command.ExecuteScalar().ToString()));

                MySqlCommand commandForWorkHour = new MySqlCommand(string.Format("SELECT SUM(Worked) FROM {0}", DB[order2]), localDbConnection);
                int workHourLocal = 0;
                try
                {
                    workHourLocal = Convert.ToInt32(commandForWorkHour.ExecuteScalar());
                }
                catch { }
                collection.Add(new ReportParameter("TotalWorkHour2", workHourLocal.ToString()));

                MySqlCommand commandForLength = new MySqlCommand(string.Format("SELECT SUM(Value2) FROM {0}", DB[order2]), localDbConnection);
                int lengthLocal = 0;
                try
                {
                    lengthLocal = Convert.ToInt32(commandForLength.ExecuteScalar());
                }
                catch { }
                collection.Add(new ReportParameter("TotalLength2", lengthLocal.ToString()));

                double result = 0;
                if (workHourLocal != 0)
                    result = lengthLocal / workHourLocal;
                collection.Add(new ReportParameter("LengthPerHour2", result.ToString()));
            }

            using (MySqlConnection localDbConnection = new MySqlConnection(pathToDBFile))
            {
                localDbConnection.Open();

                MySqlCommand command = new MySqlCommand(string.Format("SELECT COUNT(*) FROM {0} WHERE NOT (Day = 6 OR Day = 7)", DB[order2]), localDbConnection);
                collection.Add(new ReportParameter("WeekdayTotalDays2", command.ExecuteScalar().ToString()));

                MySqlCommand commandForWorkHour = new MySqlCommand(string.Format("SELECT SUM(Worked) FROM {0} WHERE NOT (Day = 6 OR Day = 7)", DB[order2]), localDbConnection);
                int workHourLocal = 0;
                try
                {
                    workHourLocal = Convert.ToInt32(commandForWorkHour.ExecuteScalar());
                }
                catch { }
                collection.Add(new ReportParameter("WeekdayTotalWorkHour2", workHourLocal.ToString()));

                MySqlCommand commandForLength = new MySqlCommand(string.Format("SELECT SUM(Value2) FROM {0} WHERE NOT (Day = 6 OR Day = 7)", DB[order2]), localDbConnection);
                int lengthLocal = 0;
                try
                {
                    lengthLocal = Convert.ToInt32(commandForLength.ExecuteScalar());
                }
                catch { }
                collection.Add(new ReportParameter("WeekdayTotalLength2", lengthLocal.ToString()));

                double result = 0;
                if (workHourLocal != 0)
                    result = lengthLocal / workHourLocal;
                collection.Add(new ReportParameter("WeekdayTotalLengthPerHour2", result.ToString()));
            }

            using (MySqlConnection localDbConnection = new MySqlConnection(pathToDBFile))
            {
                localDbConnection.Open();

                MySqlCommand command = new MySqlCommand(string.Format("SELECT COUNT(*) FROM {0} WHERE Day = 6 OR Day = 7", DB[order2]), localDbConnection);
                collection.Add(new ReportParameter("WeekendTotalDays2", command.ExecuteScalar().ToString()));

                MySqlCommand commandForWorkHour = new MySqlCommand(string.Format("SELECT SUM(Worked) FROM {0} WHERE Day = 6 OR Day = 7", DB[order2]), localDbConnection);
                int workHourLocal = 0;
                try
                {
                    workHourLocal = Convert.ToInt32(commandForWorkHour.ExecuteScalar());
                }
                catch { }

                collection.Add(new ReportParameter("WeekendTotalWorkHour2", workHourLocal.ToString()));

                MySqlCommand commandForLength = new MySqlCommand(string.Format("SELECT SUM(Value2) FROM {0} WHERE Day = 6 OR Day = 7", DB[order2]), localDbConnection);
                int lengthLocal = 0;
                try
                {
                    lengthLocal = Convert.ToInt32(commandForLength.ExecuteScalar());
                }
                catch { }
                collection.Add(new ReportParameter("WeekendTotalLength2", lengthLocal.ToString()));

                double result = 0;
                if (workHourLocal != 0)
                    result = lengthLocal / workHourLocal;
                collection.Add(new ReportParameter("WeekendTotalLengthPerHour2", result.ToString()));
            }
        }


        // --------------------------------------------------------------------------------------------------------

        private void dbProcessDate1()
        {
            using (MySqlConnection localDbConnection = new MySqlConnection(pathToDBFile))
            {
                localDbConnection.Open();

                MySqlCommand command = new MySqlCommand(string.Format("SELECT COUNT(*) FROM {0} WHERE Date >= @Date1 AND Date <= @Date2", DB[order1]), localDbConnection);
                command.Parameters.AddWithValue("@Date1", dateBegin);
                command.Parameters.AddWithValue("@Date2", dateEnd);
                collection.Add(new ReportParameter("TotalDay1", command.ExecuteScalar().ToString()));

                MySqlCommand commandForWorkHour = new MySqlCommand(string.Format("SELECT SUM(Worked) FROM {0} WHERE Date >= @Date1 AND Date <= @Date2", DB[order1]), localDbConnection);
                commandForWorkHour.Parameters.AddWithValue("@Date1", dateBegin);
                commandForWorkHour.Parameters.AddWithValue("@Date2", dateEnd);
                int workHourLocal = 0;
                try
                {
                    workHourLocal = Convert.ToInt32(commandForWorkHour.ExecuteScalar());
                }
                catch { }

                collection.Add(new ReportParameter("TotalWorkHour1", workHourLocal.ToString()));

                MySqlCommand commandForLength = new MySqlCommand(string.Format("SELECT SUM(Value2) FROM {0} WHERE Date >= @Date1 AND Date <= @Date2", DB[order1]), localDbConnection);
                commandForLength.Parameters.AddWithValue("@Date1", dateBegin);
                commandForLength.Parameters.AddWithValue("@Date2", dateEnd);
                int lengthLocal = 0;
                try
                {
                    lengthLocal = Convert.ToInt32(commandForLength.ExecuteScalar());
                }
                catch { }
                collection.Add(new ReportParameter("TotalLength1", lengthLocal.ToString()));

                double result = 0;
                if (workHourLocal != 0)
                    result = lengthLocal / workHourLocal;
                collection.Add(new ReportParameter("LengthPerHour1", result.ToString()));
            }

            using (MySqlConnection localDbConnection = new MySqlConnection(pathToDBFile))
            {
                localDbConnection.Open();

                MySqlCommand command = new MySqlCommand(string.Format("SELECT COUNT(*) FROM {0} WHERE (Date >= @Date1 AND Date <= @Date2) AND NOT (Day = 6 OR Day = 7)", DB[order1]), localDbConnection);
                command.Parameters.AddWithValue("@Date1", dateBegin);
                command.Parameters.AddWithValue("@Date2", dateEnd);
                collection.Add(new ReportParameter("WeekdayTotalDays1", command.ExecuteScalar().ToString()));

                MySqlCommand commandForWorkHour = new MySqlCommand(string.Format("SELECT SUM(Worked) FROM {0} WHERE (Date >= @Date1 AND Date <= @Date2) AND NOT (Day = 6 OR Day = 7)", DB[order1]), localDbConnection);
                commandForWorkHour.Parameters.AddWithValue("@Date1", dateBegin);
                commandForWorkHour.Parameters.AddWithValue("@Date2", dateEnd);
                int workHourLocal = 0;
                try
                {
                    workHourLocal = Convert.ToInt32(commandForWorkHour.ExecuteScalar());
                }
                catch { }
                collection.Add(new ReportParameter("WeekdayTotalWorkHour1", workHourLocal.ToString()));

                MySqlCommand commandForLength = new MySqlCommand(string.Format("SELECT SUM(Value2) FROM {0} WHERE (Date >= @Date1 AND Date <= @Date2) AND NOT (Day = 6 OR Day = 7)", DB[order1]), localDbConnection);
                commandForLength.Parameters.AddWithValue("@Date1", dateBegin);
                commandForLength.Parameters.AddWithValue("@Date2", dateEnd);
                int lengthLocal = 0;
                try
                {
                    lengthLocal = Convert.ToInt32(commandForLength.ExecuteScalar());
                }
                catch { }
                collection.Add(new ReportParameter("WeekdayTotalLength1", lengthLocal.ToString()));

                double result = 0;
                if (workHourLocal != 0)
                    result = lengthLocal / workHourLocal;
                collection.Add(new ReportParameter("WeekdayTotalLengthPerHour1", result.ToString()));
            }

            using (MySqlConnection localDbConnection = new MySqlConnection(pathToDBFile))
            {
                localDbConnection.Open();

                MySqlCommand command = new MySqlCommand(string.Format("SELECT COUNT(*) FROM {0} WHERE (Day = 6 OR Day = 7) AND (Date >= @Date1 AND Date <= @Date2)", DB[order1]), localDbConnection);
                command.Parameters.AddWithValue("@Date1", dateBegin);
                command.Parameters.AddWithValue("@Date2", dateEnd);
                collection.Add(new ReportParameter("WeekendTotalDays1", command.ExecuteScalar().ToString()));

                MySqlCommand commandForWorkHour = new MySqlCommand(string.Format("SELECT SUM(Worked) FROM {0} WHERE (Day = 6 OR Day = 7) AND (Date >= @Date1 AND Date <= @Date2)", DB[order1]), localDbConnection);
                commandForWorkHour.Parameters.AddWithValue("@Date1", dateBegin);
                commandForWorkHour.Parameters.AddWithValue("@Date2", dateEnd);
                int workHourLocal = 0;
                try
                {
                    workHourLocal = Convert.ToInt32(commandForWorkHour.ExecuteScalar());
                }
                catch { }
                collection.Add(new ReportParameter("WeekendTotalWorkHour1", workHourLocal.ToString()));

                MySqlCommand commandForLength = new MySqlCommand(string.Format("SELECT SUM(Value2) FROM {0} WHERE (Day = 6 OR Day = 7) AND (Date >= @Date1 AND Date <= @Date2)", DB[order1]), localDbConnection);
                commandForLength.Parameters.AddWithValue("@Date1", dateBegin);
                commandForLength.Parameters.AddWithValue("@Date2", dateEnd);
                int lengthLocal = 0;
                try
                {
                    lengthLocal = Convert.ToInt32(commandForLength.ExecuteScalar());
                }
                catch { }
                collection.Add(new ReportParameter("WeekendTotalLength1", lengthLocal.ToString()));

                double result = 0;
                if (workHourLocal != 0)
                    result = lengthLocal / workHourLocal;
                collection.Add(new ReportParameter("WeekendTotalLengthPerHour1", result.ToString()));
            }
        }

        // --------------------------------------------------------------------------------------------------------

        private void dbProcessDate2()
        {
            using (MySqlConnection localDbConnection = new MySqlConnection(pathToDBFile))
            {
                localDbConnection.Open();

                MySqlCommand command = new MySqlCommand(string.Format("SELECT COUNT(*) FROM {0} WHERE Date >= @Date1 AND Date <= @Date2", DB[order2]), localDbConnection);
                command.Parameters.AddWithValue("@Date1", dateBegin);
                command.Parameters.AddWithValue("@Date2", dateEnd);
                collection.Add(new ReportParameter("TotalDay2", command.ExecuteScalar().ToString()));

                MySqlCommand commandForWorkHour = new MySqlCommand(string.Format("SELECT SUM(Worked) FROM {0} WHERE Date >= @Date1 AND Date <= @Date2", DB[order2]), localDbConnection);
                commandForWorkHour.Parameters.AddWithValue("@Date1", dateBegin);
                commandForWorkHour.Parameters.AddWithValue("@Date2", dateEnd);
                int workHourLocal = 0;
                try
                {
                    workHourLocal = Convert.ToInt32(commandForWorkHour.ExecuteScalar());
                }
                catch { }

                collection.Add(new ReportParameter("TotalWorkHour2", workHourLocal.ToString()));

                MySqlCommand commandForLength = new MySqlCommand(string.Format("SELECT SUM(Value2) FROM {0} WHERE Date >= @Date1 AND Date <= @Date2", DB[order2]), localDbConnection);
                commandForLength.Parameters.AddWithValue("@Date1", dateBegin);
                commandForLength.Parameters.AddWithValue("@Date2", dateEnd);
                int lengthLocal = 0;
                try
                {
                    lengthLocal = Convert.ToInt32(commandForLength.ExecuteScalar());
                }
                catch { }
                collection.Add(new ReportParameter("TotalLength2", lengthLocal.ToString()));

                double result = 0;
                if (workHourLocal != 0)
                    result = lengthLocal / workHourLocal;
                collection.Add(new ReportParameter("LengthPerHour2", result.ToString()));
            }

            using (MySqlConnection localDbConnection = new MySqlConnection(pathToDBFile))
            {
                localDbConnection.Open();

                MySqlCommand command = new MySqlCommand(string.Format("SELECT COUNT(*) FROM {0} WHERE (Date >= @Date1 AND Date <= @Date2) AND NOT (Day = 6 OR Day = 7)", DB[order2]), localDbConnection);
                command.Parameters.AddWithValue("@Date1", dateBegin);
                command.Parameters.AddWithValue("@Date2", dateEnd);
                collection.Add(new ReportParameter("WeekdayTotalDays2", command.ExecuteScalar().ToString()));

                MySqlCommand commandForWorkHour = new MySqlCommand(string.Format("SELECT SUM(Worked) FROM {0} WHERE (Date >= @Date1 AND Date <= @Date2) AND NOT (Day = 6 OR Day = 7)", DB[order2]), localDbConnection);
                commandForWorkHour.Parameters.AddWithValue("@Date1", dateBegin);
                commandForWorkHour.Parameters.AddWithValue("@Date2", dateEnd);
                int workHourLocal = 0;
                try
                {
                    workHourLocal = Convert.ToInt32(commandForWorkHour.ExecuteScalar());
                }
                catch { }
                collection.Add(new ReportParameter("WeekdayTotalWorkHour2", workHourLocal.ToString()));

                MySqlCommand commandForLength = new MySqlCommand(string.Format("SELECT SUM(Value2) FROM {0} WHERE (Date >= @Date1 AND Date <= @Date2) AND NOT (Day = 6 OR Day = 7)", DB[order2]), localDbConnection);
                commandForLength.Parameters.AddWithValue("@Date1", dateBegin);
                commandForLength.Parameters.AddWithValue("@Date2", dateEnd);
                int lengthLocal = 0;
                try
                {
                    lengthLocal = Convert.ToInt32(commandForLength.ExecuteScalar());
                }
                catch { }
                collection.Add(new ReportParameter("WeekdayTotalLength2", lengthLocal.ToString()));

                double result = 0;
                if (workHourLocal != 0)
                    result = lengthLocal / workHourLocal;
                collection.Add(new ReportParameter("WeekdayTotalLengthPerHour2", result.ToString()));
            }

            using (MySqlConnection localDbConnection = new MySqlConnection(pathToDBFile))
            {
                localDbConnection.Open();

                MySqlCommand command = new MySqlCommand(string.Format("SELECT COUNT(*) FROM {0} WHERE (Day = 6 OR Day = 7) AND (Date >= @Date1 AND Date <= @Date2)", DB[order2]), localDbConnection);
                command.Parameters.AddWithValue("@Date1", dateBegin);
                command.Parameters.AddWithValue("@Date2", dateEnd);
                collection.Add(new ReportParameter("WeekendTotalDays2", command.ExecuteScalar().ToString()));

                MySqlCommand commandForWorkHour = new MySqlCommand(string.Format("SELECT SUM(Worked) FROM {0} WHERE (Day = 6 OR Day = 7) AND (Date >= @Date1 AND Date <= @Date2)", DB[order2]), localDbConnection);
                commandForWorkHour.Parameters.AddWithValue("@Date1", dateBegin);
                commandForWorkHour.Parameters.AddWithValue("@Date2", dateEnd);
                int workHourLocal = 0;
                try
                {
                    workHourLocal = Convert.ToInt32(commandForWorkHour.ExecuteScalar());
                }
                catch { }
                collection.Add(new ReportParameter("WeekendTotalWorkHour2", workHourLocal.ToString()));

                MySqlCommand commandForLength = new MySqlCommand(string.Format("SELECT SUM(Value2) FROM {0} WHERE (Day = 6 OR Day = 7) AND (Date >= @Date1 AND Date <= @Date2)", DB[order2]), localDbConnection);
                commandForLength.Parameters.AddWithValue("@Date1", dateBegin);
                commandForLength.Parameters.AddWithValue("@Date2", dateEnd);
                int lengthLocal = 0;
                try
                {
                    lengthLocal = Convert.ToInt32(commandForLength.ExecuteScalar());
                }
                catch { }
                collection.Add(new ReportParameter("WeekendTotalLength2", lengthLocal.ToString()));

                double result = 0;
                if (workHourLocal != 0)
                    result = lengthLocal / workHourLocal;
                collection.Add(new ReportParameter("WeekendTotalLengthPerHour2", result.ToString()));
            }
        }
    }
}
