using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatixGameClient
{
    class Cell : INotifyPropertyChanged
    {
        
        private int cellValue;
        private bool token;
        private bool used;

        public int Value
        {
            get
            {
                return cellValue;
            }
            set
            {
                cellValue = value;
            }
        }

        public bool Token
        {
            get
            {
                return token;
            }
            set
            {
                token = value;
                if (PropertyChanged != null) PropertyChanged(this,
                  new PropertyChangedEventArgs("Chip"));
            }
        }
               
        public bool Used
        {
            get
            {
                return used;
            }

            set
            {
                used = value;
                if (PropertyChanged != null) PropertyChanged(this,
                   new PropertyChangedEventArgs("Used"));
            }
        }

        public Cell(int value)
        {
            this.Value = value;
            this.Token = false;
            this.Used = false;
        }

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }
}
