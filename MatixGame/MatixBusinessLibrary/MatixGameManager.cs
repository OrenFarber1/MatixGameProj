using log4net;
using MatixDatabaseLibrary;
using WcfMatixServiceLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Xml.Serialization;
using System.IO;

namespace MatixBusinessLibrary
{
    public class MatixGameManager : IMatixBuisnessInterface
    {
        /// <summary>
        /// Class internal logger 
        /// </summary>
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
        /// Reference to the WCF service. Needed to send messages to the players.
        /// </summary>
        MatixWcfService matixWcfService = null;
        
        /// <summary>
        /// A list of active users with their emails and nick names 
        /// </summary>
        Dictionary<string, string> userEmailToNickname = null;

        /// <summary>
        /// A Dictionary to translate nickname to email
        /// </summary>
        Dictionary<string, string> usernNiknameToEmail = null;

        /// <summary>
        /// A list of waiting players
        /// </summary>
        HashSet<string> waitingPlayers = null;

        /// <summary>
        /// A list of active games found by users email
        /// </summary>
        Dictionary<string, Game> userEmailToGamel = null;


        public MatixGameManager()
        {
            userEmailToNickname = new Dictionary<string, string>();
            usernNiknameToEmail = new Dictionary<string, string>();
            waitingPlayers = new HashSet<string>();
            matixData = new MatixDataAccess();
            userEmailToGamel = new Dictionary<string, Game>();

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
            string passwordHash = GetHashString(email + password + salt);

            RegistrationResult result = new RegistrationResult();

            try
            {

                if (matixData.IsPlayerEmailExist(email))
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
        public LoginResult UserLogin(string email, string password, string ipAddress)
        {
            logger.InfoFormat("UserLogin Email: {0}, Pass: {1}, Ip: {2}", email, password, ipAddress);

            LoginResult result = new LoginResult();

            // Generate password hash based on the user password and some salt.           
            string passwordHash = GetHashString(email + password + salt);

            // checked the database that user email and password exists 
            if (matixData.CheckEmailAndPasswordHash(email, passwordHash))
            {
                // Add a login record
                if (matixData.PlayerLogin(email, passwordHash, ipAddress))
                {
                    result.Status = OperationStatusnEnum.Success;

                    string nickName;
                    if (!userEmailToNickname.TryGetValue(email, out nickName))
                    {
                        // Get it from the database
                        nickName = matixData.GetPlayerNickName(email);
                        userEmailToNickname[email] = nickName;
                    }

                    string checkEmail;
                    if (!usernNiknameToEmail.TryGetValue(nickName, out checkEmail))
                    {
                        usernNiknameToEmail[nickName] = email;
                    }

                    result.NickName = nickName;

                    // Add the player to the waiting list
                    if (waitingPlayers.Add(email))
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
                if (matixData.IsPlayerEmailExist(email))
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

        public WaitingPlayerResult GetWaitingPlayersList(string excludedEmail)
        {
            logger.Info("GetWaitingPlayr ");

            WaitingPlayerResult result = new WaitingPlayerResult();

            foreach (string email in waitingPlayers)
            {
                if (email == excludedEmail)
                    continue;

                WaitingPlayer waitingPlayer = new WaitingPlayer();
                PlayerScoreData playerData = matixData.GetWaitingPlayerData(email);

                waitingPlayer.NickName = userEmailToNickname[email];
                waitingPlayer.TotalGames = playerData.TotalNumberOfGames;
                waitingPlayer.TotalScore = playerData.TotalScore;
                waitingPlayer.NumberOfWinnings = playerData.NumberOfWinnings;

                result.WaitingPlayerslist.Add(waitingPlayer);
            }

            WaitingPlayer waitingPlayer1 = new WaitingPlayer();
            waitingPlayer1.NickName = "Player1";
            waitingPlayer1.TotalGames = 15;
            waitingPlayer1.TotalScore = 85;
            waitingPlayer1.NumberOfWinnings = 7;
            result.WaitingPlayerslist.Add(waitingPlayer1);

            waitingPlayer1 = new WaitingPlayer();
            waitingPlayer1.NickName = "Player2";
            waitingPlayer1.TotalGames = 150;
            waitingPlayer1.TotalScore = -6585;
            waitingPlayer1.NumberOfWinnings = 87;
            result.WaitingPlayerslist.Add(waitingPlayer1);

            waitingPlayer1 = new WaitingPlayer();
            waitingPlayer1.NickName = "Player__3";
            waitingPlayer1.TotalGames = 25;
            waitingPlayer1.TotalScore = 45;
            waitingPlayer1.NumberOfWinnings = 17;
            result.WaitingPlayerslist.Add(waitingPlayer1);

            result.Status = OperationStatusnEnum.Success;

            return result;
        }

        public OperationStatusnEnum StartPlayingWithPlayer(string firstEmail, string secondNickname)
        {
            //  Get second player email         
            string secondEmail;
            if (!usernNiknameToEmail.TryGetValue(secondNickname, out secondEmail))
            {
                return OperationStatusnEnum.Failure;
            }

            string firstNickname;
            if (!userEmailToNickname.TryGetValue(firstEmail, out firstNickname))
            {
                return OperationStatusnEnum.Failure;
            }

            // Remove players from waiting list 
            waitingPlayers.Remove(firstEmail);
            waitingPlayers.Remove(secondEmail);

            Task.Run(() => CreateNewGameTask(firstEmail, firstNickname, secondEmail, secondNickname, PlayerType.Human));

            return OperationStatusnEnum.Success;
        }

        /// <summary>
        /// Start a new game with the server 
        /// </summary>
        /// <param name="firstEmail"></param>
        /// <returns></returns>
        public OperationStatusnEnum StartPlayingWithRobot(string firstEmail)
        {
            string firstNickname;
            if (!userEmailToNickname.TryGetValue(firstEmail, out firstNickname))
            {
                return OperationStatusnEnum.Failure;
            }

            // Remove players from waiting list 
            waitingPlayers.Remove(firstEmail);

            string botEmail;
            string botNickname;
            GetBotDetails(out botEmail, out botNickname);

            Task.Run(() => CreateNewGameTask(firstEmail, firstNickname, botEmail, botNickname, PlayerType.Robot));

            return OperationStatusnEnum.Success;
        }

        /// <summary>
        /// Create a new game as a separate task
        /// </summary>
        /// <param name="firstEmail"></param>
        /// <param name="firstNickname"></param>
        /// <param name="secondEmail"></param>
        /// <param name="secondNickname"></param>
        /// <param name="secondType"></param>
        private void CreateNewGameTask(string firstEmail, string firstNickname, string secondEmail, string secondNickname, PlayerType secondType)
        {
            logger.InfoFormat("CreateNewGameTask firstEmail: {0}, firstNickname: {1}, secondEmail: {2}, secondNickname: {3}", firstEmail, firstNickname, secondEmail, secondNickname);

            // Currently the first player is always human 
            Game game = new Game(firstEmail, firstNickname, PlayerType.Human, secondEmail, secondNickname, secondType);

            // Add both email addresses as keys for the same game 
            userEmailToGamel[firstEmail] = game;

            // Add second player only if he is a human 
            if (secondType == PlayerType.Human)
                userEmailToGamel[secondEmail] = game;

            string xmlBoard = game.GetBoardXml();
            string horizontalEmail = game.GetHorizontalPlayerEmail();
            string verticalEmail = game.GetVerticalPlayerEmail();

            // Save the new created game in the database
            game.GameId = matixData.CreateNewGame(horizontalEmail, verticalEmail, xmlBoard);

            string verticalNickname = game.GetVerticalPlayerNickname();
            string horizontalNickname = game.GetHorizontalNickname();

            // if players are human they should be notify 
            if (game.GetHorizontalPlayerType() == PlayerType.Human)
            {
                matixWcfService.NotifyPlayerOfNewGame(horizontalEmail, horizontalNickname,
                                                verticalNickname, game.GetMatixBoard(),
                                                (WcfMatixServiceLibrary.GameTurnTypeEnum)game.GetWhoseTurnIsIt());
            }

            if (game.GetVerticalPlayerType() == PlayerType.Human)
            {
                matixWcfService.NotifyPlayerOfNewGame(verticalEmail, horizontalNickname,
                                               verticalNickname, game.GetMatixBoard(),
                                               (WcfMatixServiceLibrary.GameTurnTypeEnum)game.GetWhoseTurnIsIt());
            }

            logger.InfoFormat("CreateNewGameTask - Task ended token set at row: {0}, column: {1}", game.GetCurrentToken().Row, game.GetCurrentToken().Column);

        }

        /// <summary>
        /// Update the server with the change a user did
        /// </summary>
        /// <param name="email">the player email</param>
        /// <param name="row">The new selected row</param>
        /// <param name="column">The new selected column</param>
        /// <returns></returns>
        public OperationStatusnEnum SetGameAction(string email, int row, int column)
        {
            logger.InfoFormat("SetGameAction email: {0}, row: {1}, column: {2}", email, row, column);

            Game game;
            if (userEmailToGamel.TryGetValue(email, out game))
            {
                try
                {
                    // Update the game instance 
                    int score = game.SetGameAction(email, row, column);
                    
                    // Update the database 
                    matixData.AddGameAction( email, game.GameId, row,  column, score);

                    // After update change the turn to the other player
                    GameTurnType turn = game.ChangeCurrentTurn();
                    
                    Task.Run(() => SetGameActionTask(email, game, turn, row, column, score));                    

                    return OperationStatusnEnum.Success;
                }
                catch (System.Exception ex)
                {
                    logger.ErrorFormat("SetGameAction - Error: {0}", ex);
                    return OperationStatusnEnum.InvalidAction;
                }
            }

            return OperationStatusnEnum.InvalidEmail;
        }

        private OperationStatusnEnum SetGameAction(string email, int row, int column, string firstEmail, Game game)
        {
            logger.InfoFormat("SetGameAction email: {0}, row: {1}, column: {2} firstEmail: {3}, GameId: {4}", email, row, column, firstEmail, game.GameId);
            try
            {
                // Update the game instance 
                int score = game.SetGameAction(email, row, column);

                // Update the database 
                matixData.AddGameAction(email, game.GameId, row, column, score);

                // After update change the turn to the other player
                GameTurnType turn = game.ChangeCurrentTurn();

             //   Task.Run(() => SetGameActionTask(email, game, turn, row, column, score));

                // Notify the other player 
                matixWcfService.NotifyPlayerOfGameAction(firstEmail, row, column, score);

                return OperationStatusnEnum.Success;
            }
            catch (System.Exception ex)
            {
                logger.ErrorFormat("SetGameAction - Error: {0}", ex);               
            }

            return OperationStatusnEnum.InvalidAction;
        }

        /// <summary>
        /// Set wcf service instance 
        /// </summary>
        /// <param name="matixService"></param>
        public void SetMatixWcfService(MatixWcfService matixService)
        {
            matixWcfService = matixService;
        }


        private void SetGameActionTask(string firstEmail, Game game, GameTurnType turn, int row, int col, int score)
        {
            logger.InfoFormat("SetGameActionTask email: {0}, row: {1}, col: {2} score: {3}", firstEmail, row, col, score);

            PlayerType playerType;
            string otherEmail;
            if (turn == GameTurnType.HorizontalPlayer)
            {
                otherEmail = game.GetHorizontalPlayerEmail();
                playerType = game.GetHorizontalPlayerType();
            }
            else
            {
                otherEmail = game.GetVerticalPlayerEmail();
                playerType = game.GetVerticalPlayerType();
            }

            List<MatixCell> list;
            if (turn == GameTurnType.HorizontalPlayer)
            {
                list = game.GetRowOfCells(row);
            }
            else
            {
                list = game.GetColumnOfCells(col);
            }

            // Check if we can continue the game 

            if (list.Count == 0)
            {
                // The is ended and we should notify the winner 


            }
            else
            {

                if (playerType == PlayerType.Human)
                {
                    // Notify the second player 
                    matixWcfService.NotifyPlayerOfGameAction(otherEmail, row, col, score);
                }
                else
                {
                    // Generate game action !!!
                    
                    MatixCell maxCell = null;
                    // for the first phase get the high value 
                    foreach (var c in list)
                    {
                        if (c.Used == false)
                        {
                            if (maxCell == null)
                            {
                                maxCell = c;
                            }
                            else
                            {
                                if (c.Value > maxCell.Value)
                                {
                                    maxCell = c;
                                }
                            }
                        }
                    }

                    // use the max cell as the selected cell
                    SetGameAction(otherEmail, maxCell.Row, maxCell.Column, firstEmail, game);

                }
            }
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

        private void GetBotDetails(out string botEmail, out string botNickname)
        {
            botEmail = "m1@matix.com";
            botNickname = "Matix - 1";
        }

    }
}
