using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MatixGameClient
{
    /// <summary>
    /// Interaction logic for Registration.xaml
    /// </summary>
    public partial class RegistrationPage : Page
    {

        /// <summary>
        /// Reference to the WCF service instance 
        /// </summary>
        private MatixGameServiceReference.MatixServiceClient service = null;

        public RegistrationPage(MatixGameServiceReference.MatixServiceClient _service)
        {
            InitializeComponent();
            service = _service;
        }

        private void BackClicked(object sender, RoutedEventArgs e)
        {
            LoginPage login = new LoginPage(service);
            NavigationService.Navigate(login);
        }

        private void RegisterClicked(object sender, RoutedEventArgs e)
        {

        }
    }
}
