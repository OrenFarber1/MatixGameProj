using log4net;
using MatixGameClient.MatixGameServiceReference;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;


namespace MatixGameClient
{
    
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [CallbackBehavior(
      ConcurrencyMode = ConcurrencyMode.Single,
      UseSynchronizationContext = true)]
    public partial class MainWindow : Window, IMatixServiceCallback
    {
        #region Class Private Members 
        /// <summary>
        /// A class logger instance  
        /// </summary>
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
        /// <summary>
        /// Reference to the WCF service instance 
        /// </summary>
        private MatixServiceClient service = null;

        #endregion
        
        /// <summary>
        /// Default constructor
        /// </summary>
        public MainWindow() 
         {
            InitializeComponent();

            this.Closing += Window_Closing;

            try
            {
                OpenServiceConnection();
                ShowWelcomePage();
            }
            catch (Exception e)
            {
               logger.ErrorFormat("Main Window - Exception: {0}", e.Message);

                StringBuilder message = new StringBuilder("Failed to connect to server!");
                message.Append(Environment.NewLine);
                message.Append("Try to connect later.");
                ErrorPage errorPage = new ErrorPage(message.ToString());
                mainFrame.NavigationService.Navigate(errorPage);
            }
        }

        #region Class Private methods 
    
        /// <summary>
        /// Application closing event handler 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            string name = mainFrame.NavigationService.Content.GetType().Name;
            if (name == "GamePage")
            {
                MessageBoxResult result = MessageBox.Show("Are you sure you want to quit the game?", "Matix Game", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.No)
                {
                    e.Cancel = true;
                }
            }            
        }

        /// <summary>
        /// Open a connection to the server 
        /// </summary>
        private void OpenServiceConnection()
        {
            logger.Info("OpenServiceConnection");
            
            service = new MatixServiceClient(new InstanceContext(this), "NetTcpBinding_IMatixService");

            //Open connection to the server 
            service.Open();

            ((ICommunicationObject)service).Faulted += new EventHandler(delegate
            {
                logger.Error("Main Window - Service faulted!");
                service.Abort();
                service = null;

                // Reopen connection 
                OpenServiceConnection();
            });

            logger.Info("OpenServiceConnection - connection opened successfully");
        }

        /// <summary>
        /// Show the welcome page 
        /// </summary>
        private void ShowWelcomePage()
        {
            string email = Properties.Settings.Default.email;
            string pass = Properties.Settings.Default.password;

            logger.InfoFormat("ShowWelcomePage saved email: {0}", email);

            WelcomePage welcome;
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(pass))
            {
                welcome = new WelcomePage(service);
            }
            else
            {
                LoginData loginData = new LoginData();

                loginData.EmailAddress = email;
                loginData.Password = pass;

                LoginResult result = service.UserLogin(loginData);

                if (result.Status == OperationStatus.Success)
                {
                    welcome = new WelcomePage(service, result.Nickname, email);

                    Properties.Settings.Default.email = email;
                    Properties.Settings.Default.password = pass;
                    Properties.Settings.Default.nickname = result.Nickname;

                    Properties.Settings.Default.Save();

                }
                else
                {
                    welcome = new WelcomePage(service);
                }
            }

            mainFrame.NavigationService.Navigate(welcome);
        }

        /// <summary>
        /// Internal method to create a Player Statistics Page
        /// </summary>
        /// <param name="winnerNickname">The winner's nickname</param>
        /// <param name="score">The score of the winner</param>
        private void UpdateGameEndedTask(string winnerNickname, int score)
        {
            logger.InfoFormat("UpdateGameEndedTask - winnerNickname: {0}, score: {1}", winnerNickname, score);

            string email = Properties.Settings.Default.email;
            string nickname = Properties.Settings.Default.nickname;

            PlayerStatisticsPage page = new PlayerStatisticsPage(service, nickname, email, winnerNickname, score);
            mainFrame.NavigationService.Navigate(page);
        }
     #endregion

        /// <summary>
        /// A callback function to set a game board 
        /// </summary>
        /// <param name="matixBoard">The generated game board</param>
        /// <param name="horizontalNickname">Horizontal player's nickname</param>
        /// <param name="verticalNickName">Vertical player's nickname</param>
        /// <param name="whoIsStarting">Who's the player that start the game</param>
        public void SetMatixBoard(MatixBoard matixBoard, string horizontalNickname, string verticalNickName, GameTurnTypeEnum whoIsStarting)
        {
            logger.InfoFormat("SetMatixBoard horizontalNickname: {0}, verticalNickName: {1}, whoIsStarting: {2} ", horizontalNickname, verticalNickName, whoIsStarting);

            string name = mainFrame.NavigationService.Content.GetType().Name;
            if (name == "GamePage")
            {
                ((GamePage)mainFrame.NavigationService.Content).UpdatePageAndSetMatixBoard(matixBoard, horizontalNickname, verticalNickName, whoIsStarting);
            }
            else 
            {
                StartNewGame(matixBoard, horizontalNickname, verticalNickName, whoIsStarting);
            }
        }

        /// <summary>
        /// Change the current page to GamePage and initialize it with the parameters received from the server 
        /// </summary>
        /// <param name="matixBoard">The generated game board</param>
        /// <param name="horizontalNickname">Horizontal player nickname</param>
        /// <param name="verticalNickName">Vertican player nickname</param>
        /// <param name="whoIsStarting">Who's the player that start the game</param>
        public void StartNewGame(MatixBoard matixBoard, string horizontalNickname, string verticalNickName, GameTurnTypeEnum whoIsStarting)
        {
            string email = Properties.Settings.Default.email;
            string nickname = Properties.Settings.Default.nickname;

            logger.InfoFormat("StartNewGame email: {0}, nickname: {1}", email, nickname);

            GamePage page = new GamePage(service, nickname, email);
            mainFrame.NavigationService.Navigate(page);

            SynchronizationContext uiSyncContext = SynchronizationContext.Current;

            // We should update the board after it is loaded so create a task that wait till the 
            // page and the board is loaded and than update it with the received data 

            Task.Run(() =>
            {
                logger.Info("StartNewGame Running task.");

                while (!page.IsBoardLoaded())
                    Thread.Sleep(300);

                logger.Info("StartNewGame Running task Board is loaded");

                SendOrPostCallback callback = (x => SetMatixBoard(matixBoard, horizontalNickname, verticalNickName, whoIsStarting));
                uiSyncContext.Post(callback, null);
                
            });
         
        }


        #region IMatixServiceCallback Interface Implementation

        /// <summary>
        /// A callback function that allow the server to update the current waiting list.
        /// </summary>
        /// <param name="waitingPlayers"></param>
        public void UpdateWaitingPlayer(WaitingPlayerResult waitingPlayers)
        {
            logger.InfoFormat("WaitingPlayerResult Status: {0}", waitingPlayers.Status);

            string name = mainFrame.NavigationService.Content.GetType().Name;
            if (name == "PlayersListPage")
            {
                List<WaitingPlayer> list = new List<WaitingPlayer>();
                list.AddRange(waitingPlayers.WaitingPlayerslist);                
                ((PlayersListPage)mainFrame.NavigationService.Content).UpdateWaitingPlayerslistView(list);
            }
        }

        /// <summary>
        /// A callback function sent from the server to start a new game.
        /// </summary>
        /// <param name="matixBoard">The generated game board</param>
        /// <param name="horizontalNickname">Horizontal player nickname</param>
        /// <param name="verticalNickName">Vertican player nickname</param>
        /// <param name="whoIsStarting">Who's the player that start the game</param>
        public void StartingNewGame(MatixBoard matixBoard, string horizontalNickname, string verticalNickName, GameTurnTypeEnum whoIsStarting)
        {
            logger.InfoFormat("StartingNewGame horizontalNickname: {0}, verticalNickName: {1}", horizontalNickname, verticalNickName);

            SendOrPostCallback callback = (x => SetMatixBoard(matixBoard, horizontalNickname, verticalNickName, whoIsStarting));

            SynchronizationContext uiSyncContext = SynchronizationContext.Current;

            uiSyncContext.Post(callback, null);

        }

        /// <summary>
        /// A callback function to allow the server to notify the client about move the other player did
        /// </summary>
        /// <param name="row">The new token row</param>
        /// <param name="column">The new token column</param>
        /// <param name="score">The score of the selected token</param>
        public void UpdateGameAction(int row, int column, int score)
        {
            logger.InfoFormat("UpdateGameAction - row: {0}, column: {1}, score: {2}", row, column, score);

            string name = mainFrame.NavigationService.Content.GetType().Name;
            if (name == "GamePage")
            {
                ((GamePage)mainFrame.NavigationService.Content).UpdateMatixBoard(row, column, score);
            }            
        }

        /// <summary>
        /// A callback function to update the client that the game is ended and that we have a winner
        /// </summary>
        /// <param name="winnerNickname">The winner's nickname</param>
        /// <param name="score">The score of the winner</param>
        public void UpdateGameEnded(string winnerNickname, int score)
        {
            logger.InfoFormat("UpdateGameEnded - winnerNickname: {0}, score: {1}", winnerNickname, score);

            SynchronizationContext uiSyncContext = SynchronizationContext.Current;
            
            Task.Run(() =>
            {
                logger.Info("UpdateGameEnded Running task.");

                SendOrPostCallback callback = (x => UpdateGameEndedTask(winnerNickname, score));
                uiSyncContext.Post(callback, null);
            });

        }

        #endregion

    }
}
