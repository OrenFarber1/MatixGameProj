using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatixBusinessLibrary
{

    enum PlayerType
    {
        Human,
        Robot
    }

    class Player
    {
        /// <summary>
        /// Class internal logger 
        /// </summary>
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// The player email address 
        /// </summary>
        string email;

        /// <summary>
        /// Player nickname 
        /// </summary>
        string nickname; 

        /// <summary>
        /// The player current score 
        /// </summary>
        int score;

        /// <summary>
        /// The player type human or robot
        /// </summary>
        PlayerType playerType;


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

        public void UpdateScore(int value)
        {
            score += value;
        }

        public int GetScore()
        {
            return score;
        }

        public PlayerType GetPlayerType()
        {
            return playerType;
        }
                
    }
}
