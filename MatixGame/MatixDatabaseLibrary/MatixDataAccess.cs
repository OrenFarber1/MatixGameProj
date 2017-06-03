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

        public bool IsEmailExist(string email)
        {
            logger.Info("IsEnmailExist");

            bool exists = false;

            using (MatixDataDataContext matixData = new MatixDataDataContext())
            {
                exists = matixData.Players.Any(u => u.Email == email);
            }

            return exists;
        }

        public bool AddPlayer(string firstName, string lastName, string nickName, string email, string passwordHash)
        {
            logger.Info("AddPlayer");

            using (MatixDataDataContext matixData = new MatixDataDataContext())
            {
                Player player = new Player
                {
                    CreateTime = DateTime.Now,
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
                    logger.ErrorFormat("Exception on AddPlayer - {0}", ex);
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
                    logger.ErrorFormat("Exception on UpdatePlayerInformation - {0}", ex);
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
