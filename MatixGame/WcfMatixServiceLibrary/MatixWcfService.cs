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

            matixBuisnessInterface.AddPlayer(userData.FirstName, userData.LastName, userData.NickName, userData.EmailAddress, userData.Password);

            RegistrationResult result = new RegistrationResult();

            result.Status = OperationStatusnEnum.Success;
            result.Message = "OK";

            return result;
        }

        public LoginResultData UserLogin(LoginData loginData)
        {
            logger.Info("UserLogin");
            
            if(matixBuisnessInterface.UserLogin(loginData.EmailAddress, loginData.Password))
            {
                usersCallbackes[loginData.EmailAddress] = OperationContext.Current.GetCallbackChannel<IMatixServiceCallback>();

            }

            LoginResultData result = new LoginResultData();

            result.Status = OperationStatusnEnum.Success;
            result.NickName = "???";
            result.SessionKey = "654654";

            return result;
        }


        public WaitingPlayerResult GetWaitingPlayr()
        {
         ///   Log.Info("GetWaitingPlayr");


            WaitingPlayerResult result = new WaitingPlayerResult();

            result.Status = OperationStatusnEnum.Success;

            return result;
        }

    }
}
