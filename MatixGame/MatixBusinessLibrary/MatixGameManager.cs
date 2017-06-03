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
    public class MatixGameManager : IMatixBuisnessInterface
    {
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        MatixDataAccess matixData = null;
        MatixServiceHost matixHost = null;

        public MatixGameManager()
        {
            matixData = new MatixDataAccess();
            matixHost = new MatixServiceHost(this, typeof(MatixWcfService));
            matixHost.Open();
        }

        public bool AddPlayer(string firstName, string lastName, string nickName, string email, string passwordHash)
        {
            logger.Info("AddPlayer");


            return true;
        }


    }
}
