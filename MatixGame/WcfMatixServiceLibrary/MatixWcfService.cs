using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;

namespace WcfMatixServiceLibrary
{
    /// <summary>
    /// The class MatixWcfService is an implementation of the IMatixService interface
    /// </summary>
    [ServiceBehavior(
        ConcurrencyMode = ConcurrencyMode.Single,
        InstanceContextMode = InstanceContextMode.PerSession) ]
    public class MatixWcfService : IMatixService
    {
        #region Class Private Members 
        /// <summary>
        /// A class logger instance  
        /// </summary>
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Reference to the business layer interface 
        /// </summary>
        private IMatixBuisnessInterface matixBuisnessInterface = null;

        /// <summary>
        /// A dictionary of uses email address and its callback function
        /// </summary>
        private Dictionary<string, IMatixServiceCallback> usersCallbackes = null;
        
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor 
        /// </summary>
        /// <param name="buisnessInterface"></param>
        public MatixWcfService(IMatixBuisnessInterface buisnessInterface)
        {
            matixBuisnessInterface = buisnessInterface;
            usersCallbackes = new Dictionary<string, IMatixServiceCallback>();
            matixBuisnessInterface.SetMatixWcfService(this);
        }
    
        #endregion

        #region IMatixService Interface Implementation

        /// <summary>
        /// Implementation method for user registration request
        /// </summary>
        /// <param name="userData"></param>
        /// <returns>RegistrationResult</returns>
        public RegistrationResult UserRegistration(UserInformationData userData)
        {
            logger.Info("UserRegistration");

            RegistrationResult result = matixBuisnessInterface.AddPlayer(userData.FirstName, userData.LastName, userData.NickName, userData.EmailAddress, userData.Password);

            return result;
        }

        /// <summary>
        /// Implementation method for updating user details request
        /// </summary>
        /// <param name="userData"></param>
        /// <returns>OperationStatusEnum</returns>
        public OperationStatusEnum UpdateUserDetailes(UserInformationData userData)
        {
            logger.InfoFormat("UpdateUserDetailes email: {0}", userData.EmailAddress);

            OperationStatusEnum result = matixBuisnessInterface.UpdatePlayer(userData.EmailAddress, userData.FirstName, userData.LastName, userData.NickName);

            return result;
        }

        /// <summary>
        /// Change user password
        /// </summary>
        /// <param name="email">User email address</param>
        /// <param name="oldPassword">The current password</param>
        /// <param name="newPawwsord">The new password</param>
        /// <returns></returns>
        public OperationStatusEnum ChangeUserPassword(string email, string oldPassword, string newPawwsord)
        {
            logger.InfoFormat("ChangeUserPassword email: {0}", email);

            OperationStatusEnum result = matixBuisnessInterface.ChangePassword(email, oldPassword, newPawwsord);

            return result;
        }



        /// <summary>
        /// Implementation method for user login request
        /// </summary>
        /// <param name="loginData"></param>
        /// <returns>LoginResult</returns>
        public LoginResult UserLogin(LoginData loginData)
        {
            logger.InfoFormat("UserLogin email: {0}", loginData.EmailAddress);

            // Get the IP address the user connect from 
            string ipAddress = GetUserIpAddress();

            LoginResult result = matixBuisnessInterface.UserLogin(loginData.EmailAddress, loginData.Password, ipAddress);

            if (result.Status == OperationStatusEnum.Success)
            {
                usersCallbackes[loginData.EmailAddress] = OperationContext.Current.GetCallbackChannel<IMatixServiceCallback>();

                logger.InfoFormat("UserLogin - Add callback to usersCallbackes dictionary email: {0}", loginData.EmailAddress);

                OperationContext.Current.Channel.Faulted += ClientDisconnected;
            
            }
            return result;
        }

        /// <summary>
        /// Implementation method for user logout request
        /// </summary>
        /// <param name="email"></param>
        /// <param name="reason"></param>
        /// <returns>OperationStatusEnum</returns>
        public OperationStatusEnum UserLogout(string email, string reason)
        {
            logger.InfoFormat("UserLogout email: {0}", email);

            OperationStatusEnum result =  matixBuisnessInterface.UserLogout(email, reason);

            usersCallbackes.Remove(email);

            logger.Info("UserLogout - Remove key from usersCallbackes");

            return result;
        }

        /// <summary>
        /// Implementation method for getting the waiting players list
        /// </summary>
        /// <param name="excludedEmail">Sender email address</param>
        /// <returns>WaitingPlayerResult</returns>
        public WaitingPlayerResult GetWaitingPlayers(string excludedEmail)
        {
            logger.InfoFormat("GetWaitingPlayers excludedEmail: {0}", excludedEmail);

            WaitingPlayerResult result = matixBuisnessInterface.GetWaitingPlayersList(excludedEmail);

            return result;
        }

        /// <summary>
        /// Implementation method to remove a player from the waiting players list
        /// </summary>
        /// <param name="email">Sender email address</param>
        public void RemoveFromWaitingPlayers(string email)
        {
            logger.InfoFormat("RemoveFromWaitingPlayers email: {0}", email);

            matixBuisnessInterface.RemoveFromWaitingPlayers(email);
        }
        /// <summary>
        /// Implementation method for selecting a player to play 
        /// </summary>
        /// <param name="email">Sender email address</param>
        /// <param name="nickName">The second player nickname</param>
        /// <returns>OperationStatusEnum</returns>
        public OperationStatusEnum SelectPlayerToPlay(string email, string nickName)
        {
            logger.InfoFormat("SelectPlayer email: {0}, nickname: {1} ", email, nickName);

            OperationStatusEnum result = matixBuisnessInterface.StartPlayingWithPlayer(email, nickName);

            return result;
        }

        /// <summary>
        /// Implementation method for selecting to play with the server
        /// </summary>
        /// <param name="email">Sender email address</param>
        /// <returns>OperationStatusEnum</returns>
        public OperationStatusEnum SelectRobotToPlay(string email)
        {
            logger.InfoFormat("SelectRobotToPlay email: {0}", email);

            OperationStatusEnum result = matixBuisnessInterface.StartPlayingWithRobot(email);

            return result;
        }

        /// <summary>
        /// Implementation method for reporting a move of the token on the board 
        /// </summary>
        /// <param name="email">Sender email address</param>
        /// <param name="row">The new token row</param>
        /// <param name="column">The new token column</param>
        /// <returns></returns>
        public OperationStatusEnum SetGameAction(string email, int row, int column)
        {
            logger.InfoFormat("SetGameAction email: {0}, to row: {1}, to column: {2}", email, row, column);

            OperationStatusEnum result = matixBuisnessInterface.SetGameAction(email, row, column);

            return result;
        }


        /// <summary>
        /// Get player's statistics information 
        /// </summary>
        /// <param name="email">The player's email address</param>
        /// <returns></returns>
        public PlayerStatisticsResult GetPlayerStatistics(string email)
        {
            logger.InfoFormat("GetPlayerStatistics - email: {0}", email);

            PlayerStatisticsResult result  = matixBuisnessInterface.GetPlayerStatistics(email);

            return result;
        }


        /// <summary>
        /// Implementation method to quit from a game.
        /// </summary>
        /// <param name="email">Sender email address</param>
        public void QuitTheGame(string email)
        {
            logger.InfoFormat("QuitTheGame email: {0}", email);

            matixBuisnessInterface.QuitTheGame(email);
        }

        #endregion
    
        #region Client Callback Notofocation Methods 
        
        /// <summary>
        /// Notify a player with a waiting players list
        /// </summary>
        /// <param name="email">The email address of the player that should be notifyied</param>
        /// <param name="result">Waiting players information list</param>
        public void NotifyWatingPlars(string email, WaitingPlayerResult result)
        {
            logger.InfoFormat("NotifyWatingPlars - email: {0}", email);

            try
            {
                // Get the callback instance
	            IMatixServiceCallback callback = usersCallbackes[email];
	            callback.UpdateWaitingPlayer(result);
            }
            catch (System.Exception ex)
            {
                logger.ErrorFormat("Exception on NotifyWatingPlars: {0}", ex);
            }
        }


        /// <summary>
        /// Notify the client using a callback to start a new game 
        /// </summary>
        /// <param name="email">The notified player's email</param>
        /// <param name="horizontalNickname">Horizontal player's nickname</param>
        /// <param name="verticalNickname">Vertcal player's nickname</param>
        /// <param name="matixBoard">The generated game board</param>
        /// <param name="whoIsStarting">Who's the player that start the game</param>
        public void NotifyPlayerOfNewGame(string email, string horizontalNickname, string verticalNickname, MatixBoard matixBoard, GameTurnTypeEnum whoIsStarting)
        {
            logger.InfoFormat("NotifyPlayerOfNewGame - playerEmail: {0},  horizontalNickname:{1},  verticalNickname: {2}", email,  horizontalNickname, verticalNickname);

            try
            {
                // Get the callback instance
                IMatixServiceCallback callback = usersCallbackes[email];

                // Send information to the client 
                callback.StartingNewGame(matixBoard, horizontalNickname, verticalNickname, whoIsStarting);
            }
            catch (System.Exception ex)
            {
                logger.ErrorFormat("Exception on NotifyPlayerOfNewGame: {0}", ex);
            }
        }

        /// <summary>
        /// Notify a player withe the game action the other player did
        /// </summary>
        /// <param name="playerEmail">The email of the player to be notifyed</param>
        /// <param name="row">The new token row</param>
        /// <param name="column">The new token column</param>
        /// <param name="value">The new token value</param>
        public void NotifyPlayerOfGameAction(string playerEmail, int row, int column, int value)
        {
            logger.InfoFormat("NotifyPlayerOfGameAction  playerEmail: {0}, row: {1}, column: {2}, value: {3}", playerEmail, row,  column,  value);

            try
            {
                // Get the callback instance
                IMatixServiceCallback callback = usersCallbackes[playerEmail];

                // Send information to the client using the callback 
                callback.UpdateGameAction(row, column, value);
            }
            catch (System.Exception ex)
            {
                logger.ErrorFormat("Exception on NotifyPlayerOfGameAction: {0}", ex);
            }
        }

        /// <summary>
        /// Notify a player that the game is ended and that we have a winner
        /// </summary>
        /// <param name="playerEmail"></param>
        /// <param name="winnerNickname"></param>
        /// <param name="score"></param>
        public void NotifyPlayerOfGameEnded(string playerEmail, string winnerNickname, int score)
        {
            logger.InfoFormat("NotifyPlayerOfGameEnded  playerEmail: {0}, winnerNickname: {1}, score: {2}", playerEmail, winnerNickname, score);

            try
            {
                // Get the callback instance
                IMatixServiceCallback callback = usersCallbackes[playerEmail];

                // Send end of the game to the client using the callback 
                callback.UpdateGameEnded(winnerNickname, score);
            }
            catch (System.Exception ex)
            {
                logger.ErrorFormat("Exception on NotifyPlayerOfGameEnded: {0}", ex);
            }
        }

   
        #endregion

        #region Class Private Methods 

        /// <summary>
        /// Get player IP address 
        /// </summary>
        /// <returns></returns>
        private string GetUserIpAddress()
        {
            OperationContext oOperationContext = OperationContext.Current;
            MessageProperties oMessageProperties = oOperationContext.IncomingMessageProperties;
            RemoteEndpointMessageProperty oRemoteEndpointMessageProperty = (RemoteEndpointMessageProperty)oMessageProperties[RemoteEndpointMessageProperty.Name];

            return oRemoteEndpointMessageProperty.Address;
        }

        /// <summary>
        /// Client disconnected event handler 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClientDisconnected(object sender, EventArgs e)
        {
            logger.Info("ClientDisconnected");

            IMatixServiceCallback callback = sender as IMatixServiceCallback;

            string disconnected = null;
            foreach (var keyValue in usersCallbackes)
            {
                if (keyValue.Value == callback)
                {
                    disconnected = keyValue.Key;
                    break;
                }
            }

            if (string.IsNullOrEmpty(disconnected))
            {
                logger.Error("ClientDisconnected - Can't find the disconnected client");
            }
            else
            {
                logger.InfoFormat("ClientDisconnected - Client {0} disconnected!", disconnected);
                matixBuisnessInterface.ClientDisconnected(disconnected);
            }
        }

        #endregion

    }
}
