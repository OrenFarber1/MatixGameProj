using log4net;
using MatixGameClient.MatixGameServiceReference;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;


namespace MatixGameClient
{
    /// <summary>
    /// Interaction logic for Welcom.xaml
    /// </summary>
    public partial class WelcomePage : Page
    {
        /// <summary>
        /// A class logger instance  
        /// </summary>
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Reference to the WCF service 
        /// </summary>
        private MatixServiceClient service = null;

        /// <summary>
        /// The player nickname
        /// </summary>
        private string nickName;

        /// <summary>
        /// The player email address
        /// </summary>
        private string email;

        /// <summary>
        /// A flag indicates whether the player is currently logged in to the server 
        /// </summary>
        private bool loggedin;

        /// <summary>
        /// Constructor while we have no player details 
        /// </summary>
        /// <param name="_service"></param>
        public WelcomePage(MatixGameServiceReference.MatixServiceClient _service)
        {
            InitializeComponent();
            service = _service;
            loginButton.Content = "Login";
            loginName.Content = "Hi user";
            loggedin = false;

            singlePlayerButton.IsEnabled = false;
            multyPlayersButton.IsEnabled = false;
            updateDetailsButton.IsEnabled = false;
            statisticsButton.IsEnabled = false;
        }

        /// <summary>
        /// Construct a page with player details 
        /// </summary>
        /// <param name="_service">WCF service reference</param>
        /// <param name="_nickName">Player nickname </param>
        /// <param name="_email">player email address</param>
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
            updateDetailsButton.IsEnabled = true;
            statisticsButton.IsEnabled = true;
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
                logger.InfoFormat("User clicked on loginButton_Click to logout from server email: {0}", email);

                Properties.Settings.Default.email = "";
                Properties.Settings.Default.password = "";
                Properties.Settings.Default.Save();

                loginButton.Content = "Login";
                loginName.Content = "Hi user";
                loggedin = false;

                singlePlayerButton.IsEnabled = false;
                multyPlayersButton.IsEnabled = false;
                updateDetailsButton.IsEnabled = false;
                statisticsButton.IsEnabled = false;

                // Send logout message to the server
                service.UserLogout(email, "User Logout");
            }
            else
            {
                logger.Info("User clicked on loginButton_Click to login to server");

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
            logger.InfoFormat("User clicked on singlePlayerButton_Click email: {0}", email);

            // Start a new game with the server
            OperationStatus result = service.SelectRobotToPlay(email);

            GamePage page = new GamePage(service, nickName, email);
            NavigationService.Navigate(page);
        }

        /// <summary>
        /// The player select to play with other players so open the waiting players list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void multiPlayersButton_Click(object sender, RoutedEventArgs e)
        {
            logger.InfoFormat("multi players clicked email: {0}", email);
            PlayersListPage page = new PlayersListPage(service, nickName, email);
            NavigationService.Navigate(page);
        }

        /// <summary>
        /// Player select to update its user information so open the update details page 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateDetailsButton_Click(object sender, RoutedEventArgs e)
        {
            logger.InfoFormat("User click to update details email: {0}", email);

            UpdatePlayerDetailsPage page = new UpdatePlayerDetailsPage(service, nickName, email);
            NavigationService.Navigate(page);

        }

        /// <summary>
        /// Player select to see its statistics details so open the player statistics page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StatisticsButton_Click(object sender, RoutedEventArgs e)
        {
            logger.InfoFormat("User click on player statistics email: {0}", email);

            PlayerStatisticsPage page = new PlayerStatisticsPage(service, nickName, email);
            NavigationService.Navigate(page);

        }

        /// <summary>
        /// The player select to quit the application 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void quitButton_Click(object sender, RoutedEventArgs e)
        {
            logger.Info("User click to quit");

            service.Close();

            System.Environment.Exit(0);
        }
    }
}
