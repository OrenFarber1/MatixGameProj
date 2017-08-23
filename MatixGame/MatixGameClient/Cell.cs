using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatixGameClient
{
    public class Cell : INotifyPropertyChanged
    {
        /// <summary>
        /// The current cell value 
        /// </summary>
        private int cellValue;

        /// <summary>
        /// Is this cell is currently a token 
        /// </summary>
        private bool token;

        /// <summary>
        /// Is this cell already used by a player
        /// </summary>
        private bool used;


        private PlayingDirectionEnum usedBy = PlayingDirectionEnum.None;


        private int colorValue = 0;


        public int ColorValue
        {
            get
            {                
                return colorValue;
            }
            set
            {
                colorValue = value;
                NotifyPropertyChanged("ColorValue");
            }
        }

        public int Value
        {
            get
            {
                return cellValue;
            }
            set
            {
                cellValue = value;
                NotifyPropertyChanged("Value");
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
                NotifyPropertyChanged("Token");
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
                NotifyPropertyChanged("Used");
            }
        }

        public PlayingDirectionEnum UsedBy
        {
            get
            {
                return usedBy;
            }

            set
            {               
                usedBy = value;
                NotifyPropertyChanged("Used");               
            }
        }

        public Cell(int value, bool token)
        {
            this.Value = value;
            this.Token = token;
            this.Used = false;
        }

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        protected void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(info));
        }

    }
}
