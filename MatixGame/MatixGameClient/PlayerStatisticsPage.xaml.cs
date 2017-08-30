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

        /// <summary>
        /// The nickname of the winner 
        /// </summary>
        private string winnerNickname;

        /// <summary>
        /// The score of the winner 
        /// </summary>
        private int winnerScore;
        
        /// <summary>
        /// Construct a page 
        /// </summary>
        /// <param name="_service"></param>
        /// <param name="_nickName"></param>
        /// <param name="_email"></param>
        public PlayerStatisticsPage(MatixGameServiceReference.MatixServiceClient _service, string _nickName, string _email)
        {
            InitializeComponent();
            nickName = _nickName;
            email = _email;
            service = _service;
            winnerNickname = "";
            winnerScore = 0;

            loginName.Content = "Hi " + _nickName;
        }

        /// <summary>
        /// Construct a page with winner information 
        /// </summary>
        /// <param name="_service"></param>
        /// <param name="_nickName"></param>
        /// <param name="_email"></param>
        /// <param name="_winnerNickname"></param>
        /// <param name="_winnerScore"></param>
        public PlayerStatisticsPage(MatixGameServiceReference.MatixServiceClient _service, string _nickName, string _email, string _winnerNickname, int _winnerScore)
        {
            InitializeComponent();
            nickName = _nickName;
            email = _email;
            service = _service;

            winnerNickname = _winnerNickname;
            winnerScore = _winnerScore;

            loginName.Content = "Hi " + _nickName;
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            logger.Info("Back button clicked");
            WelcomePage welcome = new WelcomePage(service, nickName, email);
            NavigationService.Navigate(welcome);
        }
    }
}
