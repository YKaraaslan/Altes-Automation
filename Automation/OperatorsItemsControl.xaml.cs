using System.Windows;
using System.Windows.Controls;

namespace Automation
{
    /// <summary>
    /// Interaction logic for OperatorsItemsControl.xaml
    /// </summary>
    public partial class OperatorsItemsControl : UserControl
    {

        public OperatorsItemsControl()
        {
            InitializeComponent();
        }

        private void settings(object sender, RoutedEventArgs e)
        {
            OperatorItems person = ((Button)sender).Tag as OperatorItems;
            MessageBox.Show(person.OperatorName);
        }

        private void operatorInformation(object sender, RoutedEventArgs e)
        {

        }
    }
}
