﻿using log4net;
using MatixGameClient.MatixGameServiceReference;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MatixGameClient
{
    /// <summary>
    /// Enumerate the playing direction 
    /// </summary>
    public enum PlayingDirectionEnum
    {
        /// <summary>
        /// Not set yet 
        /// </summary>
        None,
        /// <summary>
        /// The first token 
        /// </summary>
        InitializeToken,
        /// <summary>
        /// Horizontal Playing Direction 
        /// </summary>
        Horizontal,
        /// <summary>
        /// Vertical Playing Direction 
        /// </summary>
        Vertical
    }

    /// <summary>
    /// Interaction logic for Board.xaml
    /// </summary>
    [ComplexBindingProperties("DataSource")]
    public partial class Board : UserControl
    {

        #region Class Private Members 
        /// <summary>
        /// A class logger instance  
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
        private int currentTokenRow = -1;

        /// <summary>
        /// The current column index of the token
        /// </summary>
        private int currentTokenCol = -1;

        /// <summary>
        /// Contains the current playing direction
        /// </summary>
        private PlayingDirectionEnum currentPlayingDirection;

        /// <summary>
        /// Save the 
        /// </summary>
        private PlayingDirectionEnum myPlayingDirection;

        /// <summary>
        /// Reference to the parent page use to update changes the player made on the board 
        /// </summary>
        private GamePage parentPage = null;

        #endregion


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
        /// Get token row index
        /// </summary>
        /// <returns></returns>
        public int GetTokenRow()
        {
            return currentTokenRow;
        }

        /// <summary>
        /// Get token column index
        /// </summary>
        /// <returns></returns>
        public int GetTokenColumn()
        {
            return currentTokenCol;
        }

        /// <summary>
        /// Update a game with a new board
        /// </summary>
        /// <param name="matixBoard">The generated game board</param>
        /// <param name="direction">Current playing direction</param>
        /// <param name="myDirection">Client player direction</param>
        public void UpdateMatixBoard(MatixBoard matixBoard, PlayingDirectionEnum direction, PlayingDirectionEnum myDirectionn)
        {
            logger.InfoFormat("UpdateMatixBoard");

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
                        cCurrent.UsedBy = PlayingDirectionEnum.InitializeToken;
                        UpdateColorValue(cCurrent, i, j);

                        logger.InfoFormat("UpdateMatixBoard set Token to Row: {0}, Column:{1}", currentTokenRow, currentTokenCol);
                    }
                }
            }
        }


        /// <summary>
        /// Update the game that a new token selected by the other player
        /// </summary>
        /// <param name="row">The new token row</param>
        /// <param name="column">The new token column</param>
        public void UpdateBoardToken(int row, int column)
        {
            logger.InfoFormat("UpdateBoardToken row: {0}, column: {1} currentPlayingDirection: {2}", row, column, currentPlayingDirection);

            // Set the previous token cell as not token and used 
            Cell cCurrent = boardCells.GetCell(currentTokenRow, currentTokenCol);
            cCurrent.Used = true;
            cCurrent.Token = false;
            UpdateColorValue(cCurrent, currentTokenRow, currentTokenCol);

            // Set the current token
            cCurrent = boardCells.GetCell(row, column);
            cCurrent.Token = true;
            cCurrent.UsedBy = currentPlayingDirection;
            UpdateColorValue(cCurrent, row, column);

            currentTokenRow = row;
            currentTokenCol = column;

            // Receive notification from the server so change to my turn 
            currentPlayingDirection = myPlayingDirection;
        }

        #region Class Private Mothods 

        /// <summary>
        /// Event handler method while the page is loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Window parentWindow = Window.GetWindow(this);

            var parent = VisualTreeHelper.GetParent(this);
            while (!(parent is Page))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }

            parentPage = (GamePage)parent;

            // Set function handlers to the Game Page 
            parentPage.updateMatixBoardDelegate = UpdateMatixBoard;
            parentPage.updateBoardTokenDelegate = UpdateBoardToken;

        }
        /// <summary>
        /// Handler method for clicking the board the method allow selecting a new token 
        /// and update the client and the server with the selection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BoardMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Check if the user double clicked and its turn  
            if (e.ClickCount == 2)
            {
                if (currentPlayingDirection == myPlayingDirection)
                {
                    Point p = e.GetPosition(BoardControl);

                    double rowH = ActualHeight / 8;
                    double colW = ActualWidth / 8;

                    int column = (int)(p.X / colW);
                    int row = (int)(p.Y / rowH);

                    logger.InfoFormat("BoardMouseLeftButtonDown DoubleClick on row: {0}, col: {1}, PlayingDirection: {2}", row, column, currentPlayingDirection);

                    // validate client action 
                    bool valid = false;
                    if (currentPlayingDirection == PlayingDirectionEnum.Horizontal)
                    {
                        valid = currentTokenRow == row;
                    }
                    else
                    {
                        valid = currentTokenCol == column;
                    }

                    Cell c = boardCells.GetCell(row, column);
                    if (!c.Used && !c.Token && valid)
                    {

                        // Update the server of the change 
                        OperationStatus status = parentPage.UpdateMatixServer(row, column, c.Value);

                        if (status == OperationStatus.Success)
                        {
                            Cell cCurrent = boardCells.GetCell(currentTokenRow, currentTokenCol);
                            cCurrent.Used = true;
                            cCurrent.Token = false;
                            UpdateColorValue(cCurrent, currentTokenRow, currentTokenCol);

                            // Set the new token
                            c.Token = true;
                            c.UsedBy = currentPlayingDirection;
                            UpdateColorValue(c, row, column);

                            currentTokenRow = row;
                            currentTokenCol = column;

                            // Change the direction so we block the user from clicking
                            CahngeCurrentTurn();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Set the color value of the cell based on its properties
        /// </summary>
        /// <param name="c"></param>
        /// <param name="row"></param>
        /// <param name="column"></param>
        private void UpdateColorValue(Cell c, int row, int column)
        {
            if (c.Token)
            {
                if (c.UsedBy == PlayingDirectionEnum.InitializeToken)
                    c.ColorValue = 1;
                else
                    c.ColorValue = 5;
            }
            else
            {
                switch (c.UsedBy)
                {
                    case PlayingDirectionEnum.Horizontal:
                        c.ColorValue = 2;
                        break;
                    case PlayingDirectionEnum.Vertical:
                        c.ColorValue = 3;
                        break;
                    case PlayingDirectionEnum.InitializeToken:
                        c.ColorValue = 4;
                        break;
                }
            }

            logger.InfoFormat("UpdateColorValue row: {0}, column: {1}, token: {2}, used by:{3},  ColorVale: {4}", row, column, c.Token, c.UsedBy, c.ColorValue);
        }

        /// <summary>
        /// Change the current 
        /// </summary>
        private void CahngeCurrentTurn()
        {
            logger.InfoFormat("CahngeCurrentTurn: {0}", currentPlayingDirection);

            if (currentPlayingDirection == PlayingDirectionEnum.Horizontal)
            {
                currentPlayingDirection = PlayingDirectionEnum.Vertical;
            }
            else
            {
                currentPlayingDirection = PlayingDirectionEnum.Horizontal;
            }
        }
        #endregion

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