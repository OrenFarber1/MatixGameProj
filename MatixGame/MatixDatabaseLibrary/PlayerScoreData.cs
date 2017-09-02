using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatixDatabaseLibrary
{
    /// <summary>
    /// The class contains Player's statistics information 
    /// </summary>
    public class PlayerScoreData
    {
        public string NickName { get; set; }
        public int TotalNumberOfGames { get; set; }
        public int NumberOfWinnings { get; set; }
        public int TotalScore { get; set; }        
        
    }
}
