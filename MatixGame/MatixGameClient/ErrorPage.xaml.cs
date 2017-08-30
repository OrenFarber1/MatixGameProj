using System.Windows;
using System.Windows.Controls;


namespace MatixGameClient
{
    /// <summary>
    /// Interaction logic for ErrorPage.xaml
    /// </summary>
    public partial class ErrorPage : Page
    {
        public ErrorPage(string message)
        {
            InitializeComponent();
            errorMessage.Content = message;
        }

        private void ok_Click(object sender, RoutedEventArgs e)
        {
            System.Environment.Exit(1);
        }
    }
}
