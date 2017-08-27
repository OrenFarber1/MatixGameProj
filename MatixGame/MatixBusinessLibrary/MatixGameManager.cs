﻿using log4net;
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
        HashSet<string> waitingPlayersList = null;

        /// <summary>
        /// A list of active games found by users email
        /// </summary>
        Dictionary<string, Game> userEmailToGamel = null;


        public MatixGameManager()
        {
            userEmailToNickname = new Dictionary<string, string>();
            usernNiknameToEmail = new Dictionary<string, string>();
            waitingPlayersList = new HashSet<string>();
            matixData = new MatixDataAccess();
            userEmailToGamel = new Dictionary<string, Game>();

            matixHost = new MatixServiceHost(this, typeof(MatixWcfService));
            matixHost.Open();
        }

        public void ClientDisconnected(string email)
        {
            logger.InfoFormat("ClientDisconnected - {0}", email);

            if (userEmailToNickname.Remove(email))
            {
                logger.InfoFormat("ClientDisconnected - {0} removed from userEmailToNickname", email);
            }

            if (waitingPlayersList.Remove(email))
            {
                logger.InfoFormat("ClientDisconnected - {0} removed from waitingPlayers", email);
            }

            if (waitingPlayersList.Remove(email))
            {
                logger.InfoFormat("ClientDisconnected - {0} removed from userEmailToGamel", email);
            }

            // Handle closing a running game !!!
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
                    result.Status = OperationStatusEnum.InvalidEmail;
                }

                // Add user credentials to the database
                matixData.AddPlayer(firstName, lastName, nickName, email, passwordHash);

                result.Status = OperationStatusEnum.Success;

            }
            catch (System.Invalid​Operation​Exception ex)
            {
                logger.ErrorFormat("AddPlayer - Exception: {0}", ex);
                result.Status = OperationStatusEnum.Failure;
            }

            return result;
        }

        public OperationStatusEnum UpdatePlayer(string email, string firstName, string lastName, string nickName)
        {
            logger.InfoFormat("UpdatePlayer email: {0}", email);

            try
            {
                matixData.UpdatePlayerInformation(email, firstName, lastName, nickName);
            }
            catch (System.Invalid​Operation​Exception ex)
            {
                logger.ErrorFormat("UpdatePlayer - Exception: {0}", ex);
                return OperationStatusEnum.Failure;
            }

            return OperationStatusEnum.Success;
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
                    result.Status = OperationStatusEnum.Success;

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

                }
                else
                {
                    result.Status = OperationStatusEnum.Failure;
                }
            }
            else
            {
                if (matixData.IsPlayerEmailExist(email))
                {
                    result.Status = OperationStatusEnum.InvalidPassword;
                }
                else
                {
                    result.Status = OperationStatusEnum.InvalidEmail;
                }
            }

            return result;
        }

        public WaitingPlayerResult GetWaitingPlayersList(string excludedEmail)
        {
            logger.Info("GetWaitingPlayr ");

            WaitingPlayerResult result = new WaitingPlayerResult();

            // Add the player to the waiting list
            if (waitingPlayersList.Add(excludedEmail))
            {
                logger.InfoFormat("GetWaitingPlayersList Email: {0} Added to the waiting list.", excludedEmail);
            }
            else
            {
                logger.ErrorFormat("GetWaitingPlayersList Email: {0} Faile to add to the waiting list or email already there.", excludedEmail);
            }


            foreach (string email in waitingPlayersList)
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

            result.Status = OperationStatusEnum.Success;

            Task.Run(() => NotifyPlayersWithWaitingPlayersTask(excludedEmail));
            
            return result;
        }

        private void NotifyPlayersWithWaitingPlayersTask(string excudedEmail)
        {

            foreach (string notifyPlayer in waitingPlayersList)
            {
                // Skip, just notify
                if (notifyPlayer == excudedEmail)
                    continue;

                WaitingPlayerResult result = new WaitingPlayerResult();

                foreach (string email in waitingPlayersList)
                {
                    if (email == notifyPlayer)
                        continue;

                    WaitingPlayer waitingPlayer = new WaitingPlayer();
                    PlayerScoreData playerData = matixData.GetWaitingPlayerData(email);

                    waitingPlayer.NickName = userEmailToNickname[email];
                    waitingPlayer.TotalGames = playerData.TotalNumberOfGames;
                    waitingPlayer.TotalScore = playerData.TotalScore;
                    waitingPlayer.NumberOfWinnings = playerData.NumberOfWinnings;

                    result.WaitingPlayerslist.Add(waitingPlayer);
                }

                matixWcfService.NotifyWatingPlars(notifyPlayer, result);

            }
        }

        public OperationStatusEnum StartPlayingWithPlayer(string firstEmail, string secondNickname)
        {
            logger.InfoFormat("StartPlayingWithPlayer - firstEmail: {0}, secondNickname: {1}", firstEmail, secondNickname);

            //  Get second player email         
            string secondEmail;
            if (!usernNiknameToEmail.TryGetValue(secondNickname, out secondEmail))
            {
                return OperationStatusEnum.Failure;
            }

            string firstNickname;
            if (!userEmailToNickname.TryGetValue(firstEmail, out firstNickname))
            {
                return OperationStatusEnum.Failure;
            }

            // Remove players from waiting list 
            waitingPlayersList.Remove(firstEmail);
            waitingPlayersList.Remove(secondEmail);

            Task.Run(() => CreateNewGameTask(firstEmail, firstNickname, secondEmail, secondNickname, PlayerType.Human));

            return OperationStatusEnum.Success;
        }

        /// <summary>
        /// Start a new game with the server 
        /// </summary>
        /// <param name="firstEmail"></param>
        /// <returns></returns>
        public OperationStatusEnum StartPlayingWithRobot(string firstEmail)
        {
            string firstNickname;
            if (!userEmailToNickname.TryGetValue(firstEmail, out firstNickname))
            {
                return OperationStatusEnum.Failure;
            }

            // Remove players from waiting list 
            waitingPlayersList.Remove(firstEmail);

            string botEmail;
            string botNickname;
            GetBotDetails(out botEmail, out botNickname);

            Task.Run(() => CreateNewGameTask(firstEmail, firstNickname, botEmail, botNickname, PlayerType.Robot));

            return OperationStatusEnum.Success;
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
        public OperationStatusEnum SetGameAction(string email, int row, int column)
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
                    matixData.AddGameAction(email, game.GameId, row, column, score);

                    // After update change the turn to the other player
                    GameTurnType turn = game.ChangeCurrentTurn();

                    Task.Run(() => SetGameActionTask(email, game, turn, row, column, score));

                    return OperationStatusEnum.Success;
                }
                catch (System.Exception ex)
                {
                    logger.ErrorFormat("SetGameAction - Error: {0}", ex);
                    return OperationStatusEnum.InvalidAction;
                }
            }

            return OperationStatusEnum.InvalidEmail;
        }

        private OperationStatusEnum SetGameAction(string email, int row, int column, string firstEmail, Game game)
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

                // Check if the game is ended !!
                List<MatixCell> list;
                if (turn == GameTurnType.HorizontalPlayer)
                {
                    list = game.GetRowOfCells(row);
                }
                else
                {
                    list = game.GetColumnOfCells(column);
                }

                // Notify the other player 
                matixWcfService.NotifyPlayerOfGameAction(firstEmail, row, column, score);

                return OperationStatusEnum.Success;
            }
            catch (System.Exception ex)
            {
                logger.ErrorFormat("SetGameAction - Error: {0}", ex);
            }

            return OperationStatusEnum.InvalidAction;
        }

        /// <summary>
        /// Set WCF service instance 
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

                int horScore = game.GetHorizontalScore();
                int vertScore = game.GetVerticalScore();

                int winnerScore;
                string winnerNickname;
                if (horScore > vertScore)
                {
                    winnerNickname = game.GetHorizontalNickname();
                    winnerScore = horScore;
                }
                else
                {
                    winnerNickname = game.GetVerticalPlayerNickname();
                    winnerScore = vertScore;
                }


                if (game.GetHorizontalPlayerType() == PlayerType.Human)
                {
                    string horEmail = game.GetHorizontalPlayerEmail();

                    userEmailToGamel.Remove(horEmail);

                    // The game is ended and we should notify the players  
                    matixWcfService.NotifyPlayerOfGameEnded(horEmail, winnerNickname, winnerScore);
                }

                if (game.GetVerticalPlayerType() == PlayerType.Human)
                {
                    string vertEmail = game.GetVerticalPlayerEmail();
                    userEmailToGamel.Remove(vertEmail);

                    // The game is ended and we should notify the players  
                    matixWcfService.NotifyPlayerOfGameEnded(vertEmail, winnerNickname, winnerScore);
                }

                // Update database 

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

        public void RemoveFromWaitingPlayers(string email)
        {            
            if (waitingPlayersList.Remove(email))
            {
                logger.InfoFormat("RemoveFromWaitingPlayers - {0} removed from waitingPlayers", email);
            }
            
            Task.Run(() => NotifyPlayersWithWaitingPlayersTask(email));
        }

        public void QuitTheGame(string email)
        {
            logger.InfoFormat("QuitTheGame email: {0}", email);

            Game game;
            if (userEmailToGamel.TryGetValue(email, out game))
            {
                PlayerType playerType;
                string otherEmail;
                string othernickname;
                int playerScore;

                otherEmail = game.GetHorizontalPlayerEmail();
                if (email != otherEmail)
                {
                    playerType = game.GetHorizontalPlayerType();
                    playerScore = game.GetHorizontalScore();
                    othernickname = game.GetHorizontalNickname();
                }
                else
                {
                    otherEmail = game.GetVerticalPlayerEmail();
                    playerType = game.GetVerticalPlayerType();
                    playerScore = game.GetVerticalScore();
                    othernickname = game.GetVerticalPlayerNickname();
                }

                userEmailToGamel.Remove(email);
                userEmailToGamel.Remove(otherEmail);

                // Update database 

                if (playerType == PlayerType.Human)
                {
                    // Notify the other player that he wins the game   
                    matixWcfService.NotifyPlayerOfGameEnded(otherEmail, othernickname, playerScore);
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
