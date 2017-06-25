using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace WcfMatixServiceLibrary
{
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

        public RegistrationResult UserRegistration(UserInformationData userData)
        {
            logger.Info("UserRegistration");

            RegistrationResult result = matixBuisnessInterface.AddPlayer(userData.FirstName, userData.LastName, userData.NickName, userData.EmailAddress, userData.Password);
                    
            return result;
        }

        public LoginResult UserLogin(LoginData loginData)
        {
            logger.Info("UserLogin");

            LoginResult result = matixBuisnessInterface.UserLogin(loginData.EmailAddress, loginData.Password);

            if ( result.Status == OperationStatusnEnum.Success)
            {
                usersCallbackes[loginData.EmailAddress] = OperationContext.Current.GetCallbackChannel<IMatixServiceCallback>();                
            }
            
            return result;
        }


        public WaitingPlayerResult GetWaitingPlayr()
        {
            logger.Info("GetWaitingPlayr");

            WaitingPlayerResult result = matixBuisnessInterface.GetWaitingPlayrslist();

            return result;
        }

    }
}
