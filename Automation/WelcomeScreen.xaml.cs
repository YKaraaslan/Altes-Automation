using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Automation
{
    /// <summary>
    /// Interaction logic for WelcomeScreen.xaml
    /// </summary>
    public partial class WelcomeScreen : Window
    {
        public WelcomeScreen()
        {
            InitializeComponent();
            var myControl = new Login(panel, this);
            DockPanel.SetDock(myControl, Dock.Left);
            panel.Children.Add(myControl);
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_MouseEnter(object sender, MouseEventArgs e)
        {
            canvas.Visibility = Visibility.Visible;
        }
    }
}
