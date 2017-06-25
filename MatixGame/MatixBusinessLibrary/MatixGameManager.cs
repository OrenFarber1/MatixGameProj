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
        /// WCF Server host 
        /// </summary>
        MatixServiceHost matixHost = null;

        /// <summary>
        /// A list of active users with their emails and nick names 
        /// </summary>
        Dictionary<string, string> userNickName = null;

        /// <summary>
        /// A list of waiting players
        /// </summary>
        HashSet<string> waitingPlayers = null;


        public MatixGameManager()
        {
            userNickName = new Dictionary<string, string>();
            waitingPlayers = new HashSet<string>();
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
        public RegistrationResult AddPlayer(string firstName, string lastName, string nickName, string email, string password)
        {
            logger.Info("AddPlayer");
            
            // Generate password hash  based on the user password and some salt.           
            string passwordHash = GetHashString(password + salt);

            RegistrationResult result = new RegistrationResult();

            try
            {

                if (matixData.IsEmailExist(email))
                {
                    // Error 
                    result.Status = OperationStatusnEnum.InvalidEmail;
                }

                // Add user credentials to the database
                matixData.AddPlayer(firstName, lastName, nickName, email, passwordHash);

                result.Status = OperationStatusnEnum.Success;

            }
            catch (System.Invalid​Operation​Exception ex)
            {
                logger.ErrorFormat("AddPlayer - Exception: {0}", ex);
                result.Status = OperationStatusnEnum.Failure;
            }

            return result; 
        }

        /// <summary>
        /// Validate the user credentials and save a login record
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public LoginResult UserLogin(string email, string password)
        {
            logger.InfoFormat("UserLogin Email: {0}, Pass: {1}", email, password);

            LoginResult result = new LoginResult();

            string ip = "";

            // Generate password hash  based on the user password and some salt.           
            string passwordHash = GetHashString(password + salt);
                       
            // checked the database that user email and password exists 
            if (matixData.CheckEmailAndPasswordHash(email, passwordHash))
            {
                // Add a login record
                if (matixData.PlayerLogin(email, passwordHash, ip))
                {
                    result.Status = OperationStatusnEnum.Success;

                    string nickName;
                    if (!userNickName.TryGetValue(email, out nickName))
                    {
                        // Get it from the database
                        nickName = matixData.GetUserNickName(email);
                        userNickName[email] = nickName;
                    }
                    result.NickName = nickName;

                    // Add the player to the waiting list
                    if(waitingPlayers.Add(email))
                    {
                        logger.InfoFormat("UserLogin Email: {0} Added to the waiting list.", email);
                    }
                    else
                    {
                        logger.ErrorFormat("UserLogin Email: {0} Faile to add to the waiting list or email already there.", email);
                    }

                }
                else
                {
                    result.Status = OperationStatusnEnum.Failure;
                }
            }
            else
            {
                if(matixData.IsEmailExist(email))
                {
                    result.Status = OperationStatusnEnum.InvalidPassword;                    
                }
                else
                {
                    result.Status = OperationStatusnEnum.InvalidEmail;
                }
            }
                    
            return result;
        }

        public WaitingPlayerResult GetWaitingPlayrslist()
        {
            logger.Info("GetWaitingPlayr");

            WaitingPlayerResult result = new WaitingPlayerResult();

            foreach(string email in waitingPlayers)
            {
                WaitingPlayer waitingPlayer = new WaitingPlayer();
                waitingPlayer.NickName = userNickName[email];
                result.WaitingPlayerslist.Add(waitingPlayer);
            }
            
            result.Status = OperationStatusnEnum.Success;

            return result;
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
