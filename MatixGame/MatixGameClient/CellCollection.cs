using MatixGameClient.MatixGameServiceReference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatixGameClient
{
    /// <summary>
    /// The class encapsulates the Cells collection 
    /// </summary>
    public class CellCollection
    {
        public List<List<Cell>> RowCollection { get; set; }

        public Cell GetCell(int row, int col)
        {
            return RowCollection[row][col];
        }

        /// <summary>
        /// Construct a CellCollection from a MatixBoard
        /// </summary>
        /// <param name="size">The size of the board</param>
        /// <param name="matixBoard">The MatixBoard received from the server </param>
        public CellCollection(int size, MatixBoard matixBoard)
        {
            this.RowCollection = new List<List<Cell>>(size);
            foreach (var matixList in matixBoard.MatixCells)
            {
                int row = 0;
                List<Cell> columns = new List<Cell>(size);
                foreach (var matixCell in matixList)
                {
                    row = matixCell.Row;
                    
                    Cell cell = new Cell(matixCell.Value, matixCell.Token);
                    
                    columns.Insert(matixCell.Column, cell);
                }
                this.RowCollection.Insert(row,columns);
            }
        }

        /// <summary>
        /// Create a CellCollection instance 
        /// </summary>
        /// <param name="size">Define the size of the board</param>
        public CellCollection(int size)
        {
            this.RowCollection = new List<List<Cell>>();
            for (int i = 0; i < size; i++)
            {
                List<Cell> columns = new List<Cell>();
                for (int j = 0; j < size; j++)
                {
                    Cell c;

                    if (i == 3 && j == 3)
                        c = new Cell(i * size + j, true);
                    else
                        c = new Cell(i * size + j, false);

                    columns.Add(c);
                }
                this.RowCollection.Add(columns);
            }
        }
    }
}
