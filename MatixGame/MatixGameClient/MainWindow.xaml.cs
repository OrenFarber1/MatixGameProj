using log4net;
using MatixGameClient.MatixGameServiceReference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [CallbackBehavior(
      ConcurrencyMode = ConcurrencyMode.Single,
      UseSynchronizationContext = true)]
    public partial class MainWindow : Window, IMatixServiceCallback
    {
        /// <summary>
        /// logger instance of the class 
        /// </summary>
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Callback instance 
        /// </summary>
        //   private static MatixClientCallback callback = new MatixClientCallback(SynchronizationContext.Current);
        // private MatixGameServiceReference.MatixServiceClient service = new MatixGameServiceReference.MatixServiceClient(new InstanceContext(this), "NetTcpBinding_IMatixService");

                  
        private MatixGameServiceReference.MatixServiceClient service = null;


        public MainWindow() 
         { 
            InitializeComponent();

            service = new MatixGameServiceReference.MatixServiceClient(new InstanceContext(this), "NetTcpBinding_IMatixService");

            bool showWelcome = false;

            try
            {
                //Open connection to the server 
                service.Open();

                ((ICommunicationObject)service).Faulted += new EventHandler(delegate
                {
                    MessageBox.Show("Service faulted!");
                });

                showWelcome = true;
            }
            catch (Exception e)
            {
               logger.ErrorFormat("Main Window - Exception: {0}", e.Message);
               showWelcome = false;
            }

            if (showWelcome)
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
                        welcome = new WelcomePage(service, result.NickName, email);
                    }
                    else
                    {
                        welcome = new WelcomePage(service);
                    }
                }

                mainFrame.NavigationService.Navigate(welcome);
            }
            else
            {
                string message = "Failed to connect to server!" + Environment.NewLine + "Try to connect later.";
                ErrorPage errorPage = new ErrorPage(message);
                mainFrame.NavigationService.Navigate(errorPage);                
            }
        }

        public void SetMatixBoard(MatixBoard matixBoard, string horizontalNickname, string verticalNickName, GameTurnTypeEnum whoIsStarting)
        {
            logger.InfoFormat("SetMatixBoard horizontalNickname: {0}, verticalNickName: {1}, whoIsStarting: {2} ", horizontalNickname, verticalNickName, whoIsStarting);

            string name = mainFrame.NavigationService.Content.GetType().Name;
            if (name == "GamePage")
            {
                ((GamePage)mainFrame.NavigationService.Content).SetMatixBoard(matixBoard, horizontalNickname, verticalNickName, whoIsStarting);
            }
        }

        public void Ping(int value)
        {
            MessageBox.Show("Ping: " + value);
        }

        public void UpdateWaitingPlayer(WaitingPlayerResult waitingPlayers)
        {
          //  ((IMatixServiceCallback)callback).UpdateWaitingPlayr(waitingPlayers);
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
    }  
}
