using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Windows;
using System.Windows.Input;

namespace Automation
{
    /// <summary>
    /// Interaction logic for MailList.xaml
    /// </summary>
    public partial class MailList : Window
    {
        Assembly assembly;
        ResourceManager rm;
        CultureInfo cultureInfo;
        List<VarsMail> mails = new List<VarsMail>();
        string pathToDBFile = Properties.Resources.ConnectionString;

        public MailList()
        {
            InitializeComponent();
            assembly = Assembly.Load("Automation");
            rm = new ResourceManager("Automation.Languages.language", assembly);
            cultureInfo = new CultureInfo(Properties.Settings.Default.language);
            init();
        }

        private void init()
        {
            using (MySqlConnection localDbConnection = new MySqlConnection(pathToDBFile))
            {
                localDbConnection.Open();

                MySqlCommand commandMail = new MySqlCommand("SELECT Sender, SenderMail, Subject, Content, Date FROM Mails ORDER BY Date DESC", localDbConnection);

                MySqlDataAdapter dataAdapterProjects = new MySqlDataAdapter(commandMail);
                DataTable dataTableMail = new DataTable();
                dataAdapterProjects.Fill(dataTableMail);

                mails.Clear();

                for (int i = 0; i < dataTableMail.Rows.Count; i++)
                {
                    mails.Add(new VarsMail()
                    {
                        sender = dataTableMail.Rows[i][0].ToString(),
                        senderMail = dataTableMail.Rows[i][1].ToString(),
                        subject = dataTableMail.Rows[i][2].ToString(),
                        content = dataTableMail.Rows[i][3].ToString(),
                        date = dataTableMail.Rows[i][4].ToString()
                    });
                }
                listViewProjects.ItemsSource = mails;
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
