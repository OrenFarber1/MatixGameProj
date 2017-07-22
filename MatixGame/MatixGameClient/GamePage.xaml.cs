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

            PlayingDirectionEnum direction = PlayingDirectionEnum.Vertical;
            PlayingDirectionEnum myDirection = PlayingDirectionEnum.Vertical;
            
            // Nickname is the name of the current client player
            if (nickName == horizontalNickname)
            {
                firstPlayer.Content = horizontalNickname;
                secondPlayer.Content = verticalNickName;

                myDirection = PlayingDirectionEnum.Horizontal;

                if (whoIsStarting == GameTurnTypeEnum.HorizontalPlayer)
                {
                    direction = PlayingDirectionEnum.Horizontal;                    
                }
            }
            else
            {
                firstPlayer.Content = verticalNickName;
                secondPlayer.Content = horizontalNickname;
                
                if (whoIsStarting == GameTurnTypeEnum.HorizontalPlayer)
                {
                    direction = PlayingDirectionEnum.Horizontal;                  
                }
            }

            firstPlayerScore.Content = 0;
            secondPlayerScore.Content = 0;
                        
            if (setBoard != null)
            {   
                // Call the delegate to update the user control
                setBoard(matixBoard, direction, myDirection);
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


        }

        /// <summary>
        /// Update the server with the player selection
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        public void UpdateMatixServer(int row, int column, int value)
        {
            logger.InfoFormat("UpdateMatixServer row: {0},  column: {1}, value: {2}", row, column, value);

            // Accumulate the values
            firstPlayerScoreValue += value;

            // Update the label
            firstPlayerScore.Content = firstPlayerScoreValue;

            service.SetGameAction(email, row, column);
        }
    }
}
