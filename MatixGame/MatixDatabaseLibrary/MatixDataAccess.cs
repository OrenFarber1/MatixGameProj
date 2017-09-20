using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using log4net;
using System.Xml.Linq;

namespace MatixDatabaseLibrary
{
    public class MatixDataAccess : IMatixDataAccessInterface
    {
        /// <summary>
        /// A class logger instance  
        /// </summary>
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Check whether an email exists in the database
        /// </summary>
        /// <param name="email">The user email address</param>
        /// <returns>True if the email exists otherwise false</returns>
        public bool IsPlayerEmailExist(string email)
        {
            logger.Info("IsPlayerEmailExist");

            bool exists = false;

            using (MatixDataDataContext matixData = new MatixDataDataContext())
            {
                exists = matixData.Players.Any(u => u.Email == email);
            }

            return exists;
        }

        /// <summary>
        /// Get the user nick name 
        /// </summary>
        /// <param name="email">User email address</param>
        /// <returns>The nick name string</returns>
        public string GetPlayerNickName(string email)
        {
            logger.InfoFormat("GetPlayerNickName Email: {0}", email);

            string nickName = "";
            using (MatixDataDataContext matixData = new MatixDataDataContext())
            {
                var query = from player in matixData.Players
                            where player.Email == email
                            select player;

                foreach (Player p in query)
                {
                    nickName = p.NickName;
                }
            }

            return nickName;
        }



        /// <summary>
        /// Check whether an email and password 
        /// </summary>
        /// <param name="email"></param>
        /// <param name="passwordHash"></param>
        /// <returns></returns>
        public bool CheckEmailAndPasswordHash(string email, string passwordHash)
        {
            logger.InfoFormat("CheckEmailAndPasswordHash Email: {0}, Pass: {1}", email, passwordHash);

            bool exists = false;

            using (MatixDataDataContext matixData = new MatixDataDataContext())
            {
                exists = matixData.Players.Any(u => u.Email == email && u.PasswordHash == passwordHash);
            }

            return exists;
        }

        /// <summary>
        /// Add a new player record to the database 
        /// </summary>
        /// <param name="firstName">User first name</param>
        /// <param name="lastName">User last name</param>
        /// <param name="nickName">The user nick name</param>
        /// <param name="email">The user email address</param>
        /// <param name="passwordHash">A hash string generated from the user password and some salt </param>
        public void AddPlayer(string firstName, string lastName, string nickName, string email, string passwordHash)
        {
            logger.Info("AddPlayer");
            try
            {
                using (MatixDataDataContext matixData = new MatixDataDataContext())
                {
                    Player player = new Player
                    {
                        CreateTime = DateTime.Now,
                        FirstName = firstName,
                        LastName = lastName,
                        NickName = nickName,
                        Email = email,
                        PasswordHash = passwordHash,
                        Type = "Human"
                    };

                    matixData.Players.InsertOnSubmit(player);
                    matixData.SubmitChanges();
                }

            }
            catch (System.Exception ex)
            {
                logger.ErrorFormat("Exception on AddPlayer - {0}", ex);
                throw new Invalid​Operation​Exception("Add Player operation Failed");
            }

        }

        /// <summary>
        /// Update user information in the database 
        /// </summary>
        /// <param name="email">The user email address</param>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="nickName"></param>
        /// <returns></returns>
        public void UpdatePlayerInformation(string email, string firstName, string lastName, string nickName)
        {
            logger.InfoFormat("UpdatePlayerInformation email: {0}", email);

            try
            {
                using (MatixDataDataContext matixData = new MatixDataDataContext())
                {
                    var query = from player in matixData.Players
                                where player.Email == email
                                select player;

                    // Update the available values 
                    foreach (Player p in query)
                    {
                        if (!string.IsNullOrEmpty(firstName))
                            p.FirstName = firstName;

                        if (!string.IsNullOrEmpty(lastName))
                            p.LastName = lastName;

                        if (!string.IsNullOrEmpty(nickName))
                            p.NickName = nickName;
                    }

                    matixData.SubmitChanges();                   

                }
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Exception on UpdatePlayerInformation - {0}", ex);
                throw new Invalid​Operation​Exception("Update Player operation Failed");            
            }

        }

        /// <summary>
        /// Change user password
        /// </summary>
        /// <param name="email">Usr email address</param>
        /// <param name="newPawwsordHash">New generated password hash</param>
        public void ChangePassword(string email, string newPawwsordHash)
        {
            logger.InfoFormat("ChangePassword email: {0} password hash: {1}", email, newPawwsordHash);

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(newPawwsordHash))
            {
                logger.ErrorFormat("ChangePassword Invalid arguments email: {0} password hash: {1}", email, newPawwsordHash);
                throw new ArgumentException("Not email nor password can be empty");
            }

            try
            {
                
                using (MatixDataDataContext matixData = new MatixDataDataContext())
                {
                    var query = from player in matixData.Players
                                where player.Email == email
                                select player;

                    // Update the available values 
                    foreach (Player p in query)
                    {
                        p.PasswordHash = newPawwsordHash;
                    }

                    matixData.SubmitChanges();

                }
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Exception on UpdatePlayerInformation - {0}", ex);
                throw new Invalid​Operation​Exception("Update Player operation Failed");
            }
        }

        /// <summary>
        /// Create a new record that a user logged in to the server
        /// </summary>
        /// <param name="email">User email address</param>
        /// <param name="passwordHash"></param>
        /// <returns></returns>
        public bool PlayerLogin(string email, string passwordHash, string ip, out long lognId)
        {
            logger.InfoFormat("PlayerLogin email: {0}", email);

            try
            {
                lognId = 0;
                using (MatixDataDataContext matixData = new MatixDataDataContext())
                {
                    long playerId = 0;
                    var query = from player in matixData.Players
                                where player.Email == email
                                select player;

                    foreach (Player p in query)
                    {
                        playerId = p.PlayerId;
                    }

                    PlayersLogin login = new PlayersLogin
                    {
                        PlayerId = playerId,
                        LoginTime = DateTime.Now,
                        IPAddress = ip
                    };

                    matixData.PlayersLogins.InsertOnSubmit(login);
                    matixData.SubmitChanges();

                    lognId = login.LoginId;
                }

            }
            catch (System.Exception ex)
            {
                logger.ErrorFormat("Exception on AddPlayer - {0}", ex);
                throw new Invalid​Operation​Exception("Add Player operation Failed");
            }

            return true;
        }

        public bool PlayerLogout(long lognId, string email, string reason)
        {          
            try
            {

                logger.InfoFormat("PlayerLogout email: {0}, reason: {1}, lognId: {2} ", email, reason, lognId);

                using (MatixDataDataContext matixData = new MatixDataDataContext())
                {
                    var query = from record in matixData.PlayersLogins
                                where record.LoginId == lognId
                                select record;

                    foreach (PlayersLogin p in query)
                    {
                        p.LogoutTime = DateTime.Now;
                        p.Reason = reason;                        
                    }

                    matixData.SubmitChanges();
                }
            }
            catch (System.Exception ex)
            {
                logger.ErrorFormat("Exception on PlayerLogout - {0}", ex);
                throw new Invalid​Operation​Exception("Player logout operation Failed");
            }

            return true;
        }

        public PlayerScoreData GetPlayerStatistics(string email)
        {
            logger.InfoFormat("GetPlayerStatistics email: {0}", email);

            try
            {

                PlayerScoreData result = new PlayerScoreData();
                using (MatixDataDataContext matixData = new MatixDataDataContext())
                {
                    var query = from player in matixData.PlayerStatistics                               
                                select player;

                    int rank = 0;
                    foreach (var p in query)
                    {
                        rank++;
                        if (p.Email == email)
                        {
                            result.NickName = p.NickName;
                            result.TotalScore = p.Score.Value;
                            result.TotalNumberOfGames = p.Games.Value;
                            result.NumberOfWinnings = p.Winnings.Value;
                            result.Rank = rank;
                            break;
                        }
                    } 
                }

                return result;
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Exception on GetPlayerStatistics - {0}", ex);
                throw new Invalid​Operation​Exception("Getting Player Statistics Failed");
            }

        }


        /// <summary>
        /// Retrieve player information 
        /// </summary>
        /// <param name="email">Player's email</param>
        /// <returns></returns>
        public PlayerScoreData GetWaitingPlayerData(string email)
        {
            PlayerScoreData playerData = new PlayerScoreData();

            try
            {

                using (MatixDataDataContext matixData = new MatixDataDataContext())
                {
                    var query = from PlayersHistories in matixData.PlayersHistories
                                where
                                  PlayersHistories.Player.Email == email
                                group new { PlayersHistories.Player, PlayersHistories } by new
                                {
                                    PlayersHistories.Player.NickName
                                } into g
                                select new
                                {
                                    g.Key.NickName,
                                    Games = g.Count(p => p.PlayersHistories.GameId != 0),
                                    Winnings = g.Count(p => p.PlayersHistories.Win == true),
                                    TotalScore = (int?)g.Sum(p => p.PlayersHistories.Score)
                                };

                    foreach (var p in query)
                    {
                        playerData.NickName = p.NickName;
                        playerData.NumberOfWinnings = p.Winnings;
                        playerData.TotalNumberOfGames = p.Games;
                        playerData.TotalScore = (int)p.TotalScore;
                    }
                }
            }
            catch (System.Exception ex)
            {
                logger.ErrorFormat("Exception on GetWaitingPlayerData - {0}", ex);
                throw new Invalid​Operation​Exception("Get Waiting Player Data operation Failed");
            }

            return playerData;
        }

        public long CreateNewGame(string horizontalEmail, string verticalEmail, string boardXml)
        {
            try
            {
                using (MatixDataDataContext matixData = new MatixDataDataContext())
                {
                    long horizontalPlayerId = GetPlayerId(matixData, horizontalEmail);
                    long verticalPlayerId = GetPlayerId(matixData, verticalEmail);

                    Game game = new Game
                    {
                        CreateTime = DateTime.Now,
                        HorizontalPlayerId = horizontalPlayerId,
                        VerticalPlayerId = verticalPlayerId,
                        CellsMatrix = XElement.Parse(boardXml),
                    };

                    matixData.Games.InsertOnSubmit(game);
                    matixData.SubmitChanges();

                    return game.GameId;
                }
            }
            catch (System.Exception ex)
            {
                logger.ErrorFormat("Exception on CreateNewGame - {0}", ex);
                throw new Invalid​Operation​Exception("Create New Game operation Failed");
            }
            
        }

        public bool AddGameAction(string email, long gameId, int row, int column, int value)
        {
            try
            {
                using (MatixDataDataContext matixData = new MatixDataDataContext())
                {
                    long playerId = GetPlayerId(matixData, email);

                    GameActivity activity = new GameActivity
                    {
                        GameId = gameId,
                        PlayerId = playerId,
                        ActivityTime = DateTime.Now,
                        CellRow = row,
                        CellColumn = column,
                        CellValue = value
                    };

                    matixData.GameActivities.InsertOnSubmit(activity);
                    matixData.SubmitChanges();
                }
            }
            catch (System.Exception ex)
            {
                logger.ErrorFormat("Exception on CreateNewGame - {0}", ex);
                throw new Invalid​Operation​Exception("Create New Game operation Failed");
            }

            return true;
        }

        public void AddPlayerHistory(string email, long gameId, bool win, int score)
        {
            try
            {
                using (MatixDataDataContext matixData = new MatixDataDataContext())
                {
                    long playerId = GetPlayerId(matixData, email);

                    PlayersHistory history = new PlayersHistory
                    {
                        GameId = gameId,
                        PlayerId = playerId,
                        HistoryTime = DateTime.Now,
                        Win = win,
                        Score = score
                    };

                    matixData.PlayersHistories.InsertOnSubmit(history);
                    matixData.SubmitChanges();
                }
            }
            catch (System.Exception ex)
            {
                logger.ErrorFormat("Exception on CreateNeAddPlayerHistorywGame - {0}", ex);
                throw new Invalid​Operation​Exception("Failed to add player history");
            }
        }

        /// <summary>
        /// Query the Players table and get the record id for the requested email
        /// </summary>
        /// <param name="matixData">Open data context</param>
        /// <param name="email">requested email</param>
        /// <returns></returns>
        private long GetPlayerId(MatixDataDataContext matixData, string email)
        {
            var query = (from player in matixData.Players
                         where player.Email == email
                         select new { player.PlayerId }).Single();

            return query.PlayerId;
        }

   
    }
}
