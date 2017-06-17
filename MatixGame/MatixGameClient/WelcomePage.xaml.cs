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
    /// Interaction logic for Welcom.xaml
    /// </summary>
    public partial class WelcomePage : Page
    {
        private MatixGameServiceReference.MatixServiceClient service = null;
        private string nickName;
        private string email;

        public WelcomePage(MatixGameServiceReference.MatixServiceClient _service)
        {
            InitializeComponent();
            service = _service;
        }

        public WelcomePage(MatixGameServiceReference.MatixServiceClient _service, string _nickName, string _email)
        {
            InitializeComponent();
            service = _service;
            nickName = _nickName;
            email = _email;
            loginName.Content = "Hi " + _nickName;
        }

        /// <summary>
        /// Handle log in button clicked by navigate to login page 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loginButton_Click(object sender, RoutedEventArgs e)
        {
            LoginPage login = new LoginPage(service);
            NavigationService.Navigate(login);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void startGameButton_Click(object sender, RoutedEventArgs e)
        {
            GamePage gamePage = new GamePage();
            NavigationService.Navigate(gamePage);
        }
    }
}
