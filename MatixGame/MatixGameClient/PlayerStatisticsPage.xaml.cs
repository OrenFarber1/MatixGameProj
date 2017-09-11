using log4net;
using MatixGameClient.MatixGameServiceReference;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace MatixGameClient
{
    /// <summary>
    /// Interaction logic for PlayerStatistics.xaml
    /// </summary>
    public partial class PlayerStatisticsPage : Page
    {
        #region Class Private Members 

        /// <summary>
        /// A class logger instance  
        /// </summary>
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Matix game WCF service reference 
        /// </summary>
        private MatixServiceClient service = null;

        /// <summary>
        /// The current players nickname 
        /// </summary>
        private string nickName;

        /// <summary>
        /// Player email address 
        /// </summary>
        private string email;
        
        #endregion

        #region Class Constructors

        /// <summary>
        /// Construct a page 
        /// </summary>
        /// <param name="_service">Reference to the WCF service host instance</param>
        /// <param name="_nickName">Player's nickname</param>
        /// <param name="_email">Player's email address</param>
        public PlayerStatisticsPage(MatixGameServiceReference.MatixServiceClient _service, string _nickName, string _email)
        {
            InitializeComponent();
            nickName = _nickName;
            email = _email;
            service = _service;
       
            loginName.Content = "Hi " + _nickName;

            message.Content = "Your current statistics are";
            
            // Get waiting players from server 
            PlayerStatisticsResult results = service.GetPlayerStatistics(email);
                                    
            rankValue.Content = results.Rank;
            numberofGamesValue.Content = results.NumberOfGames;
            winningsValue.Content = results.Winnings;
            averageScoreValue.Content = results.AverageScore;

        }

        /// <summary>
        /// Construct a page with winner information 
        /// </summary>
        /// <param name="_service">Reference to the WCF service host instance</param>
        /// <param name="_nickName">Player's nickname</param>
        /// <param name="_email">Player's email address</param>
        /// <param name="_winnerNickname">The winner nickname</param>
        /// <param name="_winnerScore">the winner score</param>
        public PlayerStatisticsPage(MatixGameServiceReference.MatixServiceClient _service, string _nickName, string _email, string _winnerNickname, int _winnerScore)
        {
            InitializeComponent();
            nickName = _nickName;
            email = _email;
            service = _service;

            loginName.Content = "Hi " + _nickName;

            if(_winnerNickname == nickName)
               message.Content = "Well done, you won the game\nwith " + _winnerScore + " points."; 
            else
               message.Content = "You lost the game\n" + _winnerNickname + " won with "  + _winnerScore  + " points.";

            // Get waiting players from server 
            PlayerStatisticsResult results = service.GetPlayerStatistics(email);

            rankValue.Content = results.Rank;
            numberofGamesValue.Content = results.NumberOfGames;
            winningsValue.Content = results.Winnings;
            averageScoreValue.Content = results.AverageScore;
        }

        #endregion

        #region Class Private Methods 

        /// <summary>
        /// Back button click event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            logger.Info("Back button clicked");
            WelcomePage welcome = new WelcomePage(service, nickName, email);
            NavigationService.Navigate(welcome);
        }

        #endregion
    }
}
