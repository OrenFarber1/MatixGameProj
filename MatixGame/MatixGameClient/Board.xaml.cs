using log4net;
using MatixGameClient.MatixGameServiceReference;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Threading;

namespace MatixGameClient
{
    /// <summary>
    /// Enumerate the playing direction 
    /// </summary>
    public enum PlayingDirectionEnum
    {
        Horizontal,
        Vertical
    }

    /// <summary>
    /// Interaction logic for Board.xaml
    /// </summary>
    [ComplexBindingProperties("DataSource")]
    public partial class Board : UserControl
    {
        /// <summary>
        /// Class logger instance 
        /// </summary>
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Board size definition 
        /// </summary>
       private static readonly int BOARD_SIZE = 8;

        /// <summary>
        /// UI board data context 
        /// </summary>
        private CellCollection boardCells;

        /// <summary>
        /// The current row index of the token
        /// </summary>
        int currentTokenRow = -1;

        /// <summary>
        /// The current column index of the token
        /// </summary>
        int currentTokenCol = -1;

        /// <summary>
        /// Contains the current playing direction
        /// </summary>
        PlayingDirectionEnum currentPlayingDirection;

        /// <summary>
        /// Save the 
        /// </summary>
        PlayingDirectionEnum myPlayingDirection;
        
        /// <summary>
        /// Reference to the parent page use to update changes the player made on the board 
        /// </summary>
        GamePage parentPage = null; 


        /// <summary>
        /// Create a default Board
        /// </summary>
        public Board()
        {
            InitializeComponent();
            
            boardCells = new CellCollection(BOARD_SIZE);
            this.DataContext = boardCells;
            currentTokenRow = 3;
            currentTokenCol = 3;
            
            this.Loaded += new RoutedEventHandler(Page_Loaded);
        }

        /// <summary>
        /// Event handler method while the page is loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Window parentWindow = Window.GetWindow(this);

            var parent = VisualTreeHelper.GetParent(this);
            while (!(parent is Page))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }

            parentPage = (GamePage)parent;

            // Set function handlers to the Game Page 
            parentPage.setBoard = UpdateMatixBoard;
            parentPage.setToken = UpdateBoardToken;

        }

        public void UpdateMatixBoard(MatixBoard matixBoard, PlayingDirectionEnum direction, PlayingDirectionEnum myDirectionn)
        {
            // Set current playing direction and current player direction
            currentPlayingDirection = direction;
            myPlayingDirection = myDirectionn;
           
            for (int i = 0; i < BOARD_SIZE; i++)
            {
                var row = matixBoard.MatixCells[i];
                for (int j = 0; j < BOARD_SIZE; j++)
                {
                    MatixCell matixCell = row[j];

                    Cell cCurrent = boardCells.GetCell(i, j);
                    cCurrent.Token = matixCell.Token;
                    cCurrent.Value = matixCell.Value;

                    if (cCurrent.Token)
                    {
                        currentTokenRow = i;
                        currentTokenCol = j;
                    }
                }
            }    
        }

        public void UpdateBoardToken(int row, int column)
        {
            // Receive notification from the server so change to my turn 
            currentPlayingDirection = myPlayingDirection;

            // Set the previous token cell as not token and used 
            Cell cCurrent = boardCells.GetCell(currentTokenRow, currentTokenCol);
            cCurrent.Token = false;
            cCurrent.Used = true;

            // Set the current token
            cCurrent = boardCells.GetCell(row, column);
            cCurrent.Token = true;
        }

        /// <summary>
        /// Handler method for clicking the board the method allow selecting a new token 
        /// and update the client and the server with the selection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BoardMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            logger.InfoFormat("BoardMouseLeftButtonDown ClickCount: {0}", e.ClickCount);
            
            // Check if the user double clicked and its turn  
            if (e.ClickCount == 2 && currentPlayingDirection == myPlayingDirection)
            {
                logger.Info("BoardMouseLeftButtonDown DoubleClickCount");

                Point p = e.GetPosition(BoardControl);

                double rowH = ActualHeight / 8;
                double colW = ActualWidth / 8;

                int col = (int)(p.X / colW);
                int row = (int)(p.Y / rowH);

                logger.InfoFormat("BoardMouseLeftButtonDown on col: {0}, row: {1}", col, row);

                Cell cCurrent = boardCells.GetCell(currentTokenRow, currentTokenCol);
                cCurrent.Used = true;
                cCurrent.Token = false;

                Cell c = boardCells.GetCell(row, col);
                c.Token = true;

                currentTokenRow = row;
                currentTokenCol = col;

                // Change the direction so we block the user from clicking
                CahngeCurrentTurn();

                // Update the server of the change 
                parentPage.UpdateMatixServer(row, col, c.Value);
            }
        }

        /// <summary>
        /// Change the current 
        /// </summary>
        private void CahngeCurrentTurn()
        {
            if (currentPlayingDirection == PlayingDirectionEnum.Horizontal)
            {
                currentPlayingDirection = PlayingDirectionEnum.Vertical;
            }
            else
            {
                currentPlayingDirection = PlayingDirectionEnum.Horizontal;
            }
        }

        #region INotifyPropertyChanged Members & Handlers
        public event PropertyChangedEventHandler PropertyChanged;
      
        protected void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(info));
        }
        #endregion
    }
}