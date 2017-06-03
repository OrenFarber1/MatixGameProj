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
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private IMatixBuisnessInterface matixBuisnessInterface = null;

        public MatixWcfService(IMatixBuisnessInterface buisnessInterface)
        {
            matixBuisnessInterface = buisnessInterface;
        }



        int x = 1;
        public string GetData(int value)
        {
             return string.Format("You entered: {0}", value* x++);
        }

        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            logger.Info("GetDataUsingDataContract"); 

            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return composite;
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
