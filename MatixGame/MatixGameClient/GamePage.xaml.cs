using log4net;
using MatixGameClient.MatixGameServiceReference;
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
    /// Interaction logic for GamePage.xaml
    /// </summary>
    public partial class GamePage : Page
    {
        /// <summary>
        /// A class logger instance  
        /// </summary>
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private MatixGameServiceReference.MatixServiceClient service = null;

        private string nickName;

        private string email;

        /// <summary>
        /// Use to accumulated the user selected cell values  
        /// </summary>
        private int firstPlayerScoreValue = 0;


        public delegate void SetBoardDelegate(MatixBoard matixBoard, PlayingDirectionEnum direction, PlayingDirectionEnum myDirection);

        /// <summary>
        /// Delegate to user control
        /// </summary>
        public SetBoardDelegate setBoard = null;

        public delegate void SetTokenDelegate(int row, int column);

        /// <summary>
        /// Delegate to user control
        /// </summary>
        public SetTokenDelegate setToken = null;

        string myContentMessage;
        string othercontentMessage;

        PlayingDirectionEnum currentTurn = PlayingDirectionEnum.Vertical;
        PlayingDirectionEnum myDirection = PlayingDirectionEnum.Vertical;


        public GamePage(MatixGameServiceReference.MatixServiceClient _service, string _nickName, string _email)
        {
            InitializeComponent();
            nickName = _nickName;
            email = _email;
            service = _service;
            loginName.Content = "Hi " + _nickName;
        }

        /// <summary>
        /// Back to the Welcome page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            WelcomePage welcome = new WelcomePage(service, nickName, email);
            NavigationService.Navigate(welcome);
        }

        public void SetMatixBoard(MatixBoard matixBoard, string horizontalNickname, string verticalNickName, GameTurnTypeEnum whoIsStarting)
        {
            logger.Info("SetMatixBoard");

            StringBuilder myContent = new StringBuilder();
            StringBuilder otherContent = new StringBuilder();

            myContent.Append("Hi ");
            myContent.Append(nickName);
            myContent.Append(", It's your turn to play ");

            otherContent.Append("Hi ");
            otherContent.Append(nickName);

            // Nickname is the name of the current client player
            if (nickName == horizontalNickname)
            {
                firstPlayer.Content = horizontalNickname;
                secondPlayer.Content = verticalNickName;

                myDirection = PlayingDirectionEnum.Horizontal;
                myContent.Append("horizontally");

                otherContent.Append(": Its ");
                otherContent.Append(verticalNickName);
                otherContent.Append(" turn to play vertically");

                if (whoIsStarting == GameTurnTypeEnum.HorizontalPlayer)
                {
                    currentTurn = PlayingDirectionEnum.Horizontal;
                }
            }
            else
            {
                firstPlayer.Content = verticalNickName;
                secondPlayer.Content = horizontalNickname;
                myContent.Append("vertically");

                otherContent.Append(", It's ");
                otherContent.Append(horizontalNickname);
                otherContent.Append(" turn to play horizontally");

                if (whoIsStarting == GameTurnTypeEnum.HorizontalPlayer)
                {
                    currentTurn = PlayingDirectionEnum.Horizontal;
                }
            }

            // Save the messages for later use 
            myContentMessage = myContent.ToString();
            othercontentMessage = otherContent.ToString();

            if (currentTurn == myDirection)
            {
                loginName.Content = myContentMessage;
            }
            else
            {
                loginName.Content = othercontentMessage;
            }

            firstPlayerScore.Content = 0;
            secondPlayerScore.Content = 0;

            // Hide the progress control 
            progress.Visibility = Visibility.Hidden;

            if (setBoard != null)
            {
                // Call the delegate to update the user control
                setBoard(matixBoard, currentTurn, myDirection);
            }
        }

        /// <summary>
        /// Update the page and the board with changes received from the server 
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <param name="score"></param>
        public void UpdateMatixBoard(int row, int column, int score)
        {
            logger.InfoFormat("UpdateMatixBoard row: {0},  column: {1}, score: {2} ", row, column, score);

            if (setToken != null)
            {
                setToken(row, column);

                secondPlayerScore.Content = score;
            }
            else
            {
                logger.Error("UpdateMatixBoard setToken delegate is null");
            }
                    
            ChangeCurrentDirection();       

        }

        /// <summary>
        /// Update the server with the player selection
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        public OperationStatus UpdateMatixServer(int row, int column, int value)
        {
            logger.InfoFormat("UpdateMatixServer row: {0},  column: {1}, value: {2}", row, column, value);

            OperationStatus status = OperationStatus.Success;
            try
            {
                status = service.SetGameAction(email, row, column);
            }
            catch (System.Exception ex)
            {
                logger.ErrorFormat("UpdateMatixServe Error: {0}", ex);
                status = OperationStatus.Failure;

                service.Abort();
                service.Open();
            }

            if (status == OperationStatus.Success)
            { 
                // Accumulate the values
                firstPlayerScoreValue += value;

                // Update the label
                firstPlayerScore.Content = firstPlayerScoreValue;

                // change the current turn
                ChangeCurrentDirection();
            }

            logger.InfoFormat("UpdateMatixServe return status: {0}", status);

            return status;
        }

        private void ChangeCurrentDirection()
        {
            if (currentTurn == PlayingDirectionEnum.Vertical)
            {
                currentTurn = PlayingDirectionEnum.Horizontal;
            }
            else
            {
                currentTurn = PlayingDirectionEnum.Vertical;
            }

            // Change instruction message 
            if (currentTurn == myDirection)
            {
                loginName.Content = myContentMessage;
            }
            else
            {
                loginName.Content = othercontentMessage;
            }
        }

    }
}
