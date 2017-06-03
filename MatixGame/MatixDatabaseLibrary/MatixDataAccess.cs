using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using log4net;

namespace MatixDatabaseLibrary
{
    public class MatixDataAccess
    {
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        public bool AddPlayer(string firstName, string lastName, string nickName, string email, string passwordHash)
        {
            
            using (MatixDataDataContext matixData = new MatixDataDataContext())
            {
                Player player = new Player 
                {
                    FirstName = firstName,
                    LastName = lastName,
                    NickName = nickName,
                    Email = email,
                    PasswordHash = passwordHash
                };

                matixData.Players.InsertOnSubmit(player);

                try
                {
                	matixData.SubmitChanges();
                }
                catch (System.Exception ex)
                {
                	
                }
            }
            
            return true;
        }

        public bool UpdatePlayerInformation(string firstName, string lastName, string nickName)
        {
            using (MatixDataDataContext matixData = new MatixDataDataContext())
            {
                var query =
                from player in matixData.Players
                where player.PlayerId == 11000
                select player;

                foreach(Player p in query)
                {

                }

                try
                {
                	matixData.SubmitChanges();
                }
                catch (System.Exception ex)
                {
                	
                }

            }

            return true;
        }
           
        public bool PlayerLogin(string email, string passwordHash)
        {

            return true;
        }
    }
}
