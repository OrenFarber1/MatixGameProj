using log4net;
using MatixGameClient.MatixGameServiceReference;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading;
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
        /// <summary>
        /// A class logger instance  
        /// </summary>
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
                      
        private MatixServiceClient service = null;


        public MainWindow() 
         { 
            InitializeComponent();
            
            try
            {
                OpenServiceConnection();
                ShowWelcomePage();
            }
            catch (Exception e)
            {
               logger.ErrorFormat("Main Window - Exception: {0}", e.Message);
              
                string message = "Failed to connect to server!" + Environment.NewLine + "Try to connect later.";
                ErrorPage errorPage = new ErrorPage(message);
                mainFrame.NavigationService.Navigate(errorPage);
            }
        }
        
        /// <summary>
        /// Open a connection to the server 
        /// </summary>
        private void OpenServiceConnection()
        {
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
        }

        /// <summary>
        /// Show the welcome page 
        /// </summary>
        private void ShowWelcomePage()
        {
            string email = Properties.Settings.Default.email;
            string pass = Properties.Settings.Default.password;

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
                }
                else
                {
                    welcome = new WelcomePage(service);
                }
            }

            mainFrame.NavigationService.Navigate(welcome);
        }

        public void SetMatixBoard(MatixBoard matixBoard, string horizontalNickname, string verticalNickName, GameTurnTypeEnum whoIsStarting)
        {
            logger.InfoFormat("SetMatixBoard horizontalNickname: {0}, verticalNickName: {1}, whoIsStarting: {2} ", horizontalNickname, verticalNickName, whoIsStarting);

            string name = mainFrame.NavigationService.Content.GetType().Name;
            if (name == "GamePage")
            {
                ((GamePage)mainFrame.NavigationService.Content).SetMatixBoard(matixBoard, horizontalNickname, verticalNickName, whoIsStarting);
            }
            else 
            {
                StartNewGame(matixBoard, horizontalNickname, verticalNickName, whoIsStarting);
            }
        }

        /// <summary>
        /// Change the current page to GamePage and initialize it with the parameters received from the server 
        /// </summary>
        /// <param name="matixBoard">Generated board</param>
        /// <param name="horizontalNickname">Horizontal player nickname</param>
        /// <param name="verticalNickName">Vertican player nickname</param>
        /// <param name="whoIsStarting"></param>
        public void StartNewGame(MatixBoard matixBoard, string horizontalNickname, string verticalNickName, GameTurnTypeEnum whoIsStarting)
        {
            string email = Properties.Settings.Default.email;
            string nickname = Properties.Settings.Default.nickname;

            GamePage page = new GamePage(service, nickname, email);
            page.SetMatixBoard(matixBoard, horizontalNickname, verticalNickName, whoIsStarting);
            mainFrame.NavigationService.Navigate(page);            
        }

        public void Ping(int value)
        {
            MessageBox.Show("Ping: " + value);
        }
        
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

        public void GetMatixBoard(MatixBoard matixBoard, string horizontalNickname, string verticalNickName, GameTurnTypeEnum whoIsStarting)
        {
            logger.InfoFormat("GetMatixBoard horizontalNickname: {0}, verticalNickName: {1}", horizontalNickname, verticalNickName);

            SendOrPostCallback callback = (x => SetMatixBoard(matixBoard, horizontalNickname, verticalNickName, whoIsStarting));

            SynchronizationContext uiSyncContext = SynchronizationContext.Current;

            uiSyncContext.Post(callback, null);

        }

        public void UpdateGameAction(int row, int col, int score)
        {
            logger.InfoFormat("UpdateGameAction - row: {0}, col: {1}, score: {2}", row, col, score);

            string name = mainFrame.NavigationService.Content.GetType().Name;
            if (name == "GamePage")
            {
                ((GamePage)mainFrame.NavigationService.Content).UpdateMatixBoard(row, col, score);
            }            
        }

        public void UpdateGameEnded(string winnerNickname, int score)
        {
            logger.InfoFormat("UpdateGameEnded - winnerNickname: {0}, score: {1}", winnerNickname, score);

            string email = Properties.Settings.Default.email;
            string nickname = Properties.Settings.Default.nickname;

            PlayerStatisticsPage page = new PlayerStatisticsPage(service, nickname, email, winnerNickname, score);
            mainFrame.NavigationService.Navigate(page);

        }   
    }  
}
