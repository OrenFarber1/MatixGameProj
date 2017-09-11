using log4net;
using MatixGameClient.MatixGameServiceReference;
using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;


namespace MatixGameClient
{
    /// <summary>
    /// Interaction logic for GamePage.xaml
    /// </summary>
    public partial class GamePage : Page
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

        /// <summary>
        /// Current player nickname 
        /// </summary>
        private string nickName;

        /// <summary>
        /// Current player email address 
        /// </summary>
        private string email;

        /// <summary>
        /// Use to accumulated the user selected cell values  
        /// </summary>
        private int firstPlayerScoreValue = 0;

        /// <summary>
        /// Contains a generate message content
        /// </summary>
        private string myContentMessage;

        /// <summary>
        /// Contains a generate message content
        /// </summary>
        private string otherContentMessage;

        /// <summary>
        /// Holds the current playing direction 
        /// </summary>
        private PlayingDirectionEnum currentTurn = PlayingDirectionEnum.Vertical;

        /// <summary>
        /// Holds this client player's direction
        /// </summary>
        private PlayingDirectionEnum myDirection = PlayingDirectionEnum.Vertical;

        #endregion

        /// <summary>
        /// Delegate to a Board function 
        /// </summary>
        /// <param name="matixBoard">The generated game board</param>
        /// <param name="direction">Current playing direction</param>
        /// <param name="myDirection">Client player direction</param>
        public delegate void UpdateMatixBoardDelegate(MatixBoard matixBoard, PlayingDirectionEnum direction, PlayingDirectionEnum myDirection);

        /// <summary>
        /// Delegate to user control
        /// </summary>
        public UpdateMatixBoardDelegate updateMatixBoardDelegate = null;

        /// <summary>
        /// Delegate to a Board function
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        public delegate void UpdateBoardTokenDelegate(int row, int column);

        /// <summary>
        /// Delegate to user control
        /// </summary>
        public UpdateBoardTokenDelegate updateBoardTokenDelegate = null;
               
              
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="_service">Reference to the WCF service host instance</param>
        /// <param name="_nickName">Player's nickname</param>
        /// <param name="_email">Player's email address</param>
        public GamePage(MatixServiceClient _service, string _nickName, string _email)
        {
            InitializeComponent();
            nickName = _nickName;
            email = _email;
            service = _service;
            loginName.Content = "Hi " + _nickName;
        }

        /// <summary>
        /// Check if the Board control is loaded 
        /// </summary>
        /// <returns>True whether the board is loaded otherwise False</returns>
        public bool IsBoardLoaded()
        {
            return (updateMatixBoardDelegate != null && updateBoardTokenDelegate != null);
        }

        /// <summary>
        /// Back to the Welcome page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            logger.Info("Back button clicked - Quit the game");

            MessageBoxResult result = MessageBox.Show("Are you sure you want to quit the game?", "Matix Game", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                logger.Info("Back button clicked - Approved - Quit the game");
                service.QuitTheGame(email);
                WelcomePage welcome = new WelcomePage(service, nickName, email);
                NavigationService.Navigate(welcome);
            }
        }

        /// <summary>
        /// Update the page content and set the board
        /// </summary>
        /// <param name="matixBoard">The generated game board</param>
        /// <param name="horizontalNickname"></param>
        /// <param name="verticalNickName"></param>
        /// <param name="whoIsStarting">Who's the player that start the game</param>
        public void UpdatePageAndSetMatixBoard(MatixBoard matixBoard, string horizontalNickname, string verticalNickName, GameTurnTypeEnum whoIsStarting)
        {
            logger.Info("UpdatePageAndSetMatixBoard");

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
            otherContentMessage = otherContent.ToString();

            if (currentTurn == myDirection)
            {
                loginName.Content = myContentMessage;
            }
            else
            {
                loginName.Content = otherContentMessage;
            }

            firstPlayerScore.Content = 0;
            secondPlayerScore.Content = 0;

            // Hide the progress control 
            progress.Visibility = Visibility.Hidden;

            if (updateMatixBoardDelegate != null)
            {
                // Call the delegate to update the user control
                updateMatixBoardDelegate(matixBoard, currentTurn, myDirection);
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

            if (updateBoardTokenDelegate != null)
            {
                updateBoardTokenDelegate(row, column);

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

        /// <summary>
        /// Change current direction
        /// </summary>
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
                loginName.Content = otherContentMessage;
            }

            logger.InfoFormat("ChangeCurrentDirection to: {0}", currentTurn);

        }

    }
}
