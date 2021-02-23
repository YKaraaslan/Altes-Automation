using System.Windows.Controls;
using System.Windows.Media;

namespace Automation
{
    /// <summary>
    /// Interaction logic for NotificationItems.xaml
    /// </summary>
    public partial class NotificationItems : UserControl
    {

        public NotificationItems()
        {
            InitializeComponent();
        }


        #region notificationRegion
        private string _notificationContent;
        private string _time;
        private Brush _color;

        public string NotificationText
        {
            get { return _notificationContent; }
            set { _notificationContent = value; notificationContent.Content = value; }
        }

        public string TimeText
        {
            get { return _time; }
            set { _time = value; time.Content = value; }
        }

        public Brush TextColor
        {
            get { return _color; }
            set { _color = value; notificationContent.Foreground = value; time.Foreground = value; pack.Foreground = value; }
        }

        #endregion
    }
}
