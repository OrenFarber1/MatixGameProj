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
        /// logger 
        /// </summary>
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Reference to the business layer interface 
        /// </summary>
        private IMatixBuisnessInterface matixBuisnessInterface = null;

        /// <summary>
        /// A dictionary of uses email address and its callback function
        /// </summary>
        Dictionary<string, IMatixServiceCallback> usersCallbackes = null;

     
        public MatixWcfService(IMatixBuisnessInterface buisnessInterface)
        {
            matixBuisnessInterface = buisnessInterface;
            usersCallbackes = new Dictionary<string, IMatixServiceCallback>();
           
        }
    
        private string GetUserIpAddress()
        {
            OperationContext oOperationContext = OperationContext.Current;
            MessageProperties oMessageProperties = oOperationContext.IncomingMessageProperties;
            RemoteEndpointMessageProperty oRemoteEndpointMessageProperty = (RemoteEndpointMessageProperty)oMessageProperties[RemoteEndpointMessageProperty.Name];

             return oRemoteEndpointMessageProperty.Address;
        }

        public RegistrationResult UserRegistration(UserInformationData userData)
        {
            logger.Info("UserRegistration");

            RegistrationResult result = matixBuisnessInterface.AddPlayer(userData.FirstName, userData.LastName, userData.NickName, userData.EmailAddress, userData.Password);

            return result;
        }

        public LoginResult UserLogin(LoginData loginData)
        {
            logger.Info("UserLogin");

            // Get the IP address the user connect from 
            string ipAddress = GetUserIpAddress();

            LoginResult result = matixBuisnessInterface.UserLogin(loginData.EmailAddress, loginData.Password, ipAddress);

            if ( result.Status == OperationStatusnEnum.Success)
            {
                usersCallbackes[loginData.EmailAddress] = OperationContext.Current.GetCallbackChannel<IMatixServiceCallback>();                
            }
            
            return result;
        }


        public WaitingPlayerResult GetWaitingPlayers(string excludedEmail)
        {
            logger.Info("GetWaitingPlayers");

            WaitingPlayerResult result = matixBuisnessInterface.GetWaitingPlayersList(excludedEmail);

            return result;
        }

        public OperationStatusnEnum SelectPlayerToPlay(string email, string nickName)
        {
            logger.InfoFormat("SelectPlayer email: {0}, nickname: {1} ",email,  nickName);

            OperationStatusnEnum result = matixBuisnessInterface.StartPlayingWithPlayer(email, nickName);

            return result;
        }

        public OperationStatusnEnum SetGameAction(string email, int row, int col)
        {
            logger.InfoFormat("SetGameAction email: {0}, to row: {1}, to col: {2}", email, row, col);

            OperationStatusnEnum result = matixBuisnessInterface.SetGameAction(email, row, col);

            return OperationStatusnEnum.Success;
        }
    }
}
