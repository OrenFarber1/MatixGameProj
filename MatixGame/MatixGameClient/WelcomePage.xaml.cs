using log4net;
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
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private MatixGameServiceReference.MatixServiceClient service = null;
        private string nickName;
        private string email;
        private bool loggedin;

        public WelcomePage(MatixGameServiceReference.MatixServiceClient _service)
        {
            InitializeComponent();
            service = _service;
            loginButton.Content = "Login";
            loginName.Content = "Hi user";
            loggedin = false;

            singlePlayerButton.IsEnabled = false;
            multyPlayersButton.IsEnabled = false;
        }

        public WelcomePage(MatixGameServiceReference.MatixServiceClient _service, string _nickName, string _email)
        {
            InitializeComponent();
            service = _service;
            nickName = _nickName;
            email = _email;
            loginName.Content = "Hi " + _nickName;
            loginButton.Content = "Logout";
            loggedin = true;

            singlePlayerButton.IsEnabled = true;
            multyPlayersButton.IsEnabled = true;
        }

        /// <summary>
        /// Handle log in button clicked by navigate to login page 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loginButton_Click(object sender, RoutedEventArgs e)
        {
            if (loggedin)
            {
                Properties.Settings.Default.email = "" ;
                Properties.Settings.Default.password = "";
                Properties.Settings.Default.Save();

                loginButton.Content = "Login";
                loginName.Content = "Hi user";
                loggedin = false;

                singlePlayerButton.IsEnabled = false;
                multyPlayersButton.IsEnabled = false;
            }
            else
            {
                LoginPage login = new LoginPage(service);
                NavigationService.Navigate(login);
            }
        }

        /// <summary>
        /// Event handler for single player button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void singlePlayerButton_Click(object sender, RoutedEventArgs e)
        {
            logger.Info("User clicked game");
            GamePage gamePage = new GamePage(service, nickName, email);
            NavigationService.Navigate(gamePage);
        }

             
        private void multiPlayersButton_Click(object sender, RoutedEventArgs e)
        {
            logger.Info("multi players clicked");
            PlayersListPage playersListPage = new PlayersListPage(service, nickName, email);
            NavigationService.Navigate(playersListPage);
        }


        private void quitButton_Click(object sender, RoutedEventArgs e)
        {
            logger.Info("User click to quit");
            System.Environment.Exit(0);            
        }
    }
}
