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

        CellCollection BoardCells;

        int currentChipRow = -1;
        int currentChipCol = -1;

        public Board()
        {
            InitializeComponent();

            BoardCells = new CellCollection(8);
            this.BoardControl.DataContext = BoardCells;
        }

      
        private void BoardMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                Point p = e.GetPosition(BoardControl);

                double rowH = ActualHeight / 8;
                double colW = ActualWidth / 8;

                int col = (int)(p.X / colW);
                int row = (int)(p.Y / rowH);

                Cell c = BoardCells.GetCell(row, col);

                c.Token = true;

                if (currentChipRow != -1 && currentChipCol != -1)
                {
                    Cell pc = BoardCells.GetCell(currentChipRow, currentChipCol);
                    pc.Token = false;
                    pc.Used = true;
                }

                currentChipRow = row;
                currentChipCol = col;
                
            }
        }
    }
}