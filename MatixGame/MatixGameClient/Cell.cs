using System.ComponentModel;

namespace MatixGameClient
{
    /// <summary>
    /// The class represents a cell in a Matix board 
    /// </summary>
    public class Cell : INotifyPropertyChanged
    {
        #region Class Private Members 
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

        /// <summary>
        /// This parameter indicates who use that cell 
        /// </summary>
        private PlayingDirectionEnum usedBy = PlayingDirectionEnum.None;

        /// <summary>
        /// The current color value
        /// </summary>
        private int colorValue = 0;

        #endregion

        /// <summary>
        /// The current color value
        /// </summary>
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

        /// <summary>
        /// The value property of the cell 
        /// </summary>
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

        /// <summary>
        /// A flag indicates whether the cell is a token or not 
        /// </summary>
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
               
        /// <summary>
        /// A flag indicates whether the call is already used by a player or not 
        /// </summary>
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

        /// <summary>
        /// The property indicates witch player use this cell 
        /// </summary>
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

        /// <summary>
        /// Construct a call with its value and if it is a token or not 
        /// </summary>
        /// <param name="value">The cell value (can be negative)</param>
        /// <param name="token">A falg indicats whether the cell is a token or not</param>
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
