using log4net;
using MatixDatabaseLibrary;
using WcfMatixServiceLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatixBusinessLibrary
{
    public class MatixGameManager : MatixBuisnessInterface
    {
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        MatixDataAccess matixData = null;
        ServiceHost matixHost = null;

        public MatixGameManager()
        {
            matixData = new MatixDataAccess();
        }

        public bool AddPlayer(string firstName, string lastName, string nickName, string email, string passwordHash)
        {
            logger.Info("AddPlayer");


            return true;
        }


    }
}
