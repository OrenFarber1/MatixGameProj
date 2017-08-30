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

     
        public MatixWcfService(IMatixBuisnessInterface buisnessInterface)
        {
            matixBuisnessInterface = buisnessInterface;
            usersCallbackes = new Dictionary<string, IMatixServiceCallback>();
            matixBuisnessInterface.SetMatixWcfService(this);
        }
        

        public RegistrationResult UserRegistration(UserInformationData userData)
        {
            logger.Info("UserRegistration");

            RegistrationResult result = matixBuisnessInterface.AddPlayer(userData.FirstName, userData.LastName, userData.NickName, userData.EmailAddress, userData.Password);

            return result;
        }

        public OperationStatusEnum UpdateUserDetailes(UserInformationData userData)
        {
            logger.InfoFormat("UpdateUserDetailes email: {0}", userData.EmailAddress);

            OperationStatusEnum result = matixBuisnessInterface.UpdatePlayer(userData.EmailAddress, userData.FirstName, userData.LastName, userData.NickName);

            return OperationStatusEnum.Success;
        }

        public LoginResult UserLogin(LoginData loginData)
        {
            logger.Info("UserLogin");

            // Get the IP address the user connect from 
            string ipAddress = GetUserIpAddress();

            LoginResult result = matixBuisnessInterface.UserLogin(loginData.EmailAddress, loginData.Password, ipAddress);

            if (result.Status == OperationStatusEnum.Success)
            {
                usersCallbackes[loginData.EmailAddress] = OperationContext.Current.GetCallbackChannel<IMatixServiceCallback>();

                logger.InfoFormat("UserLogin - Add callback to usersCallbackes dictionary email: {0}", loginData.EmailAddress);

                OperationContext.Current.Channel.Faulted += ClientDisconnected;
            //    OperationContext.Current.Channel.Closed += ClientDisconnected;
            }
            return result;
        }


        public OperationStatusEnum UserLogout(string email, string reason)
        {
            logger.InfoFormat("UserLogout email: {0}", email);

            OperationStatusEnum result =  matixBuisnessInterface.UserLogout(email, reason);

            usersCallbackes.Remove(email);

            logger.Info("UserLogout - Remove key from usersCallbackes");

            return result;
        }

        private void ClientDisconnected(object sender, EventArgs e)
        {
            logger.Info("ClientDisconnected");

            IMatixServiceCallback callback = sender as IMatixServiceCallback;

            string disconnected = null;
            foreach(var keyValue in usersCallbackes)
            {
                if(keyValue.Value == callback)
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


        public WaitingPlayerResult GetWaitingPlayers(string excludedEmail)
        {
            logger.InfoFormat("GetWaitingPlayers excludedEmail: {0}", excludedEmail);

            WaitingPlayerResult result = matixBuisnessInterface.GetWaitingPlayersList(excludedEmail);

            return result;
        }

        public OperationStatusEnum SelectPlayerToPlay(string email, string nickName)
        {
            logger.InfoFormat("SelectPlayer email: {0}, nickname: {1} ",email,  nickName);

            OperationStatusEnum result = matixBuisnessInterface.StartPlayingWithPlayer(email, nickName);

            return result;
        }

        public OperationStatusEnum SelectRobotToPlay(string email)
        {
            logger.InfoFormat("SelectRobotToPlay email: {0}", email);

            OperationStatusEnum result = matixBuisnessInterface.StartPlayingWithRobot(email);

            return result;
        }

        public OperationStatusEnum SetGameAction(string email, int row, int col)
        {
            logger.InfoFormat("SetGameAction email: {0}, to row: {1}, to col: {2}", email, row, col);

            OperationStatusEnum result = matixBuisnessInterface.SetGameAction(email, row, col);

            return result;
        }


        public void RemoveFromWaitingPlayers(string email)
        {
            matixBuisnessInterface.RemoveFromWaitingPlayers(email);
        }

        public void QuitTheGame(string email)
        {
            matixBuisnessInterface.QuitTheGame(email);
        }


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
        /// <param name="playerEmail"></param>
        /// <param name="horizontalNickname"></param>
        /// <param name="verticalEmail"></param>
        /// <param name="verticalNickname"></param>
        /// <param name="matixBoard"></param>
        /// <param name="whoIsStarting"></param>
        public void NotifyPlayerOfNewGame(string playerEmail, string horizontalNickname, string verticalNickname, MatixBoard matixBoard, GameTurnTypeEnum whoIsStarting)
        {
            logger.InfoFormat("NotifyPlayerOfNewGame - playerEmail: {0},  horizontalNickname:{1},  verticalNickname: {2}", playerEmail,  horizontalNickname, verticalNickname);

            try
            {
                // Get the callback instance
                IMatixServiceCallback callback = usersCallbackes[playerEmail];

                // Send information to the client 
                callback.GetMatixBoard(matixBoard, horizontalNickname, verticalNickname, whoIsStarting);
            }
            catch (System.Exception ex)
            {
                logger.ErrorFormat("Exception on NotifyPlayerOfNewGame: {0}", ex);
            }
        }

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

     
    }
}
