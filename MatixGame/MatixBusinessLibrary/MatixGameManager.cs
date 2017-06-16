using log4net;
using MatixDatabaseLibrary;
using WcfMatixServiceLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace MatixBusinessLibrary
{
    public class MatixGameManager : IMatixBuisnessInterface
    {
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
        /// <summary>
        /// Use to add salt while creating a password hash
        /// </summary>
        static string salt = "Should I use another hashing algorithm";

        /// <summary>
        /// Data Access Layer 
        /// </summary>
        MatixDataAccess matixData = null;

        /// <summary>
        /// Wcf Server host 
        /// </summary>
        MatixServiceHost matixHost = null;

        public MatixGameManager()
        {
            matixData = new MatixDataAccess();
            matixHost = new MatixServiceHost(this, typeof(MatixWcfService));
            matixHost.Open();
        }

        /// <summary>
        /// Add a new player to the database 
        /// </summary>
        /// <param name="firstName">User first name</param>
        /// <param name="lastName">User last name</param>
        /// <param name="nickName">User nick name</param>
        /// <param name="email">User email address</param>
        /// <param name="password">User password</param>
        /// <returns></returns>
        public bool AddPlayer(string firstName, string lastName, string nickName, string email, string password)
        {
            logger.Info("AddPlayer");

            if(matixData.IsEmailExist(email))
            {
                // Error 
                return false;
            }

            // Generate password hash  based on the user password and some salt.           
            string passwordHash = GetHashString( password + salt);

            // Add user credentials to the database
            bool added = matixData.AddPlayer(firstName, lastName, nickName, email, passwordHash);            
            return added;
        }

        /// <summary>
        /// Validate the user credentials and save a login record
        /// </summary>
        /// <param name="email">User email address</param>
        /// <param name="password">User password</param>
        /// <returns></returns>
        public bool UserLogin(string email, string password)
        {
            logger.InfoFormat("UserLogin Email: {0}, Pass: {1}", email, password);
            
            // Generate password hash  based on the user password and some salt.           
            string passwordHash = GetHashString(password + salt);

            // checked the database that user email and password exists 
            if (matixData.CheckEmailAndPasswordHash(email, passwordHash))
            {
                // Add a login record
                if (matixData.PlayerLogin(email, passwordHash))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Generate hash array based on the input string using SHA256
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        private byte[] GetHash(string inputString)
        {
            HashAlgorithm algorithm = SHA256.Create();  
            return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }

        /// <summary>
        /// Generate 64 characters hash string generate from the input string
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        private string GetHashString(string inputString)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in GetHash(inputString))
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }

    }
}
