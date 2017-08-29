using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatixBusinessLibrary
{
    /// <summary>
    /// Enumerate the player types
    /// </summary>
    public enum PlayerType
    {
        /// <summary>
        /// A human player type 
        /// </summary>
        Human,  
        /// <summary>
        /// A robot player type
        /// </summary>
        Robot
    }

    /// <summary>
    /// The class encapsulates player details 
    /// </summary>
    public class Player
    {
        #region Class Private Members
        /// <summary>
        /// Class internal logger 
        /// </summary>
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// The player email address 
        /// </summary>
        private string email;

        /// <summary>
        /// Player nickname 
        /// </summary>
        private string nickname; 

        /// <summary>
        /// The player current score 
        /// </summary>
        private int score;

        /// <summary>
        /// The player type human or robot
        /// </summary>
        private PlayerType playerType;

        #endregion

        public Player(string _email, string _nickname, PlayerType _type)
        {
            email = _email;
            nickname = _nickname;
            score = 0;
            playerType = _type;
        }

        /// <summary>
        /// Get the player's email
        /// </summary>
        /// <returns></returns>
        public string GetEmail()
        {
            return email;
        }

        /// <summary>
        /// Get the player's nickname 
        /// </summary>
        /// <returns></returns>
        public string GetNickname()
        {
            return nickname;
        }

        /// <summary>
        /// Update the player score 
        /// </summary>
        /// <param name="value">The value to add (can be negativ)</param>
        /// <returns>The new player score </returns>
        public int UpdateScore(int value)
        {
            score += value;
            return score;
        }

        /// <summary>
        /// Get player score 
        /// </summary>
        /// <returns></returns>
        public int GetScore()
        {
            return score;
        }

        /// <summary>
        /// Get players type Human or Robot
        /// </summary>
        /// <returns></returns>
        public PlayerType GetPlayerType()
        {
            return playerType;
        }
                
    }
}
