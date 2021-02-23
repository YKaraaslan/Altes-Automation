using MySql.Data.MySqlClient;
using System;
using System.Globalization;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Automation
{
    /// <summary>
    /// Interaction logic for Mail.xaml
    /// </summary>
    public partial class Mail : UserControl
    {
        Assembly assembly;
        ResourceManager rm;
        CultureInfo cultureInfo;
        string pathToDBFile = Properties.Resources.ConnectionString;

        public Mail()
        {
            InitializeComponent();
            assembly = Assembly.Load("Automation");
            rm = new ResourceManager("Automation.Languages.language", assembly);
            cultureInfo = new CultureInfo(Properties.Settings.Default.language);
            if (Properties.Settings.Default.adminEntered)
                sendButton.IsEnabled = false;
        }

        private void sendMail(object sender, RoutedEventArgs e)
        {
            if (mailSubject.Text.Trim() == "" || mailContent.Text.Trim() == "")
            {
                MessageBox.Show(rm.GetString("blanksCannotBeEmpty", cultureInfo), rm.GetString("system", cultureInfo),
                            MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                string subject = mailSubject.Text.Trim();
                string body = "<html><body><p>Gönderen:" + Properties.Settings.Default.nameSurname + ",</p>"
                         + "<p>" + mailContent.Text.Trim() + "</p>"
                         + "<p>Uygulama: Unity Otomasyon Sistemi</p>"
                         + "<p>Firma: Altes</p>"
                         + "<p>" + Properties.Settings.Default.userMail + " " + Properties.Settings.Default.phoneNumber + "</p></body></html>";

                SmtpClient client = new SmtpClient();
                client.Port = 587;
                client.Host = "smtp.live.com";
                client.EnableSsl = true;
                client.Timeout = 10000;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential("unitysoftware@hotmail.com", "ya++--1122");
                MailMessage mailMessage = new MailMessage("unitysoftware@hotmail.com", "yazilim@unitygrup.com", subject, body)
                {
                    IsBodyHtml = true,
                    BodyEncoding = UTF8Encoding.UTF8,
                    DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure
                };
                client.Send(mailMessage);
            }
            catch (SmtpException)
            {
                MessageBox.Show(rm.GetString("noInternetConnection", cultureInfo), rm.GetString("system", cultureInfo),
                            MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            using (MySqlConnection localDbConnection = new MySqlConnection(pathToDBFile))
            {
                localDbConnection.Open();
                MySqlCommand commandToAddDatabase = new MySqlCommand("INSERT INTO Mails(Sender, SenderMail, Subject, Content, Date)" +
                    " VALUES (@Sender, @SenderMail, @Subject, @Content, @Date)", localDbConnection);

                commandToAddDatabase.Parameters.AddWithValue("@Sender", Properties.Settings.Default.nameSurname);
                commandToAddDatabase.Parameters.AddWithValue("@SenderMail", Properties.Settings.Default.userMail);
                commandToAddDatabase.Parameters.AddWithValue("@Subject", mailSubject.Text.Trim());
                commandToAddDatabase.Parameters.AddWithValue("@Content", mailContent.Text.Trim());
                commandToAddDatabase.Parameters.AddWithValue("@Date", DateTime.Now);
                commandToAddDatabase.ExecuteNonQuery();
                mailSubject.Text = "";
                mailContent.Text = "";
                MessageBox.Show(rm.GetString("mailSentSuccessful", cultureInfo), rm.GetString("system", cultureInfo),
                            MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void listMail(object sender, RoutedEventArgs e)
        {
            MailList mailList = new MailList();
            mailList.ShowDialog();
        }
    }
}
