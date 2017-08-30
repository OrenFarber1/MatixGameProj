using log4net;
using MatixGameClient.MatixGameServiceReference;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;

namespace MatixGameClient
{
    
    /// <summary>
    /// Interaction logic for PlayersListPage.xaml
    /// </summary>
    public partial class PlayersListPage : Page
    {
        /// <summary>
        /// A class logger instance  
        /// </summary>
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Reference to the WCF service instance 
        /// </summary>
        private MatixServiceClient service = null;

        private string nickName;
        /// <summary>
        /// Current player email address 
        /// </summary>
        private string email;

        public PlayersListPage(MatixServiceClient _service, string _nickName, string _email)
        {
            InitializeComponent();
            service = _service;
            nickName = _nickName;
            email = _email;
            loginName.Content = "Hi " + _nickName;

            // Get waiting players from server 
            WaitingPlayerResult results = service.GetWaitingPlayers(email);

          //  UpdateWaitingPlayerslistView(results.WaitingPlayerslist);
            
        }

        public void UpdateWaitingPlayerslistView(List<WaitingPlayer> list)
        {
            waitingPlayerslistView.ItemsSource = list;
        }

        /// <summary>
        /// Handle double click event on the waiting players.  
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ListViewDoubleClick(object sender, RoutedEventArgs e)
        {
            //grab the original element that was double clicked on and search from child to parent until
            //you find either a ListViewItem or the top of the tree
            DependencyObject originalSource = (DependencyObject)e.OriginalSource;
            while ((originalSource != null) && !(originalSource is ListViewItem))
            {
                originalSource = VisualTreeHelper.GetParent(originalSource);
            }
         
            // if it didn't find a ListViewItem anywhere in the hierarchy, it's because the user
            // didn't click on one. Therefore, if the variable isn't null, run the code
            if (originalSource != null)
            {
                WaitingPlayer selectedPlayer = ((ListViewItem)originalSource).Content as WaitingPlayer;
                                
                if(service.SelectPlayerToPlay(email, selectedPlayer.NickName) == OperationStatus.Success)
                {

                }
            }
        }
             

        /// <summary>
        /// Back to the Welcome page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BackClicked(object sender, RoutedEventArgs e)
        {
            logger.Info("Back button clicked");

            // Remove the player from waiting list 
            service.RemoveFromWaitingPlayers(email);

            WelcomePage welcome = new WelcomePage(service, nickName, email);
            NavigationService.Navigate(welcome);
        }
    }
}
