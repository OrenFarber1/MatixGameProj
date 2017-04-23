using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatixGameClient
{
    class CellCollection
    {
        public List<List<Cell>> RowCollection { get; set; }

        public Cell GetCell(int row, int col)
        {
            return RowCollection[row][col];
        }


        public CellCollection(int size)
        {
            this.RowCollection = new List<List<Cell>>();
            for (int i = 0; i < size; i++)
            {
                List<Cell> columns = new List<Cell>();
                for (int j = 0; j < size; j++)
                {
                    Cell c = new Cell(i * size + j);

                    if (i == 3 && j == 3)
                        c.Token = true;

                    columns.Add(c);
                }
                this.RowCollection.Add(columns);
            }
        }
    }
}
