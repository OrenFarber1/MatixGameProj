using log4net;
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
using System.Windows.Threading;

namespace MatixGameClient
{
    
    /// <summary>
    /// Interaction logic for Board.xaml
    /// </summary>
    public partial class Board : UserControl
    {
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        CellCollection BoardCells;

        int currentChipRow = -1;
        int currentChipCol = -1;

        public Board()
        {
            InitializeComponent();

            BoardCells = new CellCollection(8);
            this.BoardControl.DataContext = BoardCells;
            currentChipRow = 3;
            currentChipCol = 3;
        }

        //public Board()
        //{
        //    InitializeComponent();

        //}


        private void BoardMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            logger.Info("BoardMouseLeftButtonDown");

            if (e.ClickCount == 2)
            {
                logger.Info("BoardMouseLeftButtonDown ClickCount == 2");

                Point p = e.GetPosition(BoardControl);

                double rowH = ActualHeight / 8;
                double colW = ActualWidth / 8;

                int col = (int)(p.X / colW);
                int row = (int)(p.Y / rowH);

                logger.InfoFormat("BoardMouseLeftButtonDown on col: {0}, row: {1]", col, row);

                Cell cCurrent = BoardCells.GetCell(currentChipRow, currentChipCol);
                cCurrent.Used = true;
                cCurrent.Token = false;

                Cell c = BoardCells.GetCell(row, col);
                c.Token = true;
                
                currentChipRow = row;
                currentChipCol = col;

                //if (currentChipRow != -1 && currentChipCol != -1)
                //{
                //    logger.Info("BoardMouseLeftButtonDown Update cell");


                //    Cell pc = BoardCells.GetCell(currentChipRow, currentChipCol);
                //    pc.Token = false;
                //    pc.Used = true;

                //    currentChipRow = row;
                //    currentChipCol = col;
                //}



            }
        }
    }
}