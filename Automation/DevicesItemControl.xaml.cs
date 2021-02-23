using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Automation
{
    /// <summary>
    /// Interaction logic for DevicesItemControl.xaml
    /// </summary>
    public partial class DevicesItemControl : UserControl
    {
        public DevicesItemControl()
        {
            InitializeComponent();
        }

        #region deviceRegion

        private string name;
        private string dbName;
        private string operators;
        private int id;

        public string DeviceNameText
        {
            get { return name; }
            set { name = value; }
        }

        public string OperatorText
        {
            get { return operators; }
            set { operators = value; }
        }

        public string DbNameText
        {
            get { return dbName; }
            set { dbName = value; }
        }

        public int IDText
        {
            get { return id; }
            set { id = value; }
        }

        #endregion

        private void devicesSettings(object sender, RoutedEventArgs e)
        {

        }

        private void device_Click(object sender, MouseButtonEventArgs e)
        {

        }

        private void projectOpen(object sender, RoutedEventArgs e)
        {

        }
    }

    public class DeviceID
    {

    }
}
