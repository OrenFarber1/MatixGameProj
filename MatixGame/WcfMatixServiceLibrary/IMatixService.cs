using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace WcfMatixServiceLibrary
{
    [ServiceContract(
    SessionMode = SessionMode.Required,
    CallbackContract = typeof(IMatixServiceCallback))]
    public interface IMatixService
    {
        [OperationContract]
        string GetData(int value);

        [OperationContract]
        CompositeType GetDataUsingDataContract(CompositeType composite);

        // TODO: Add your service operations here


        [OperationContract]
        RegistrationResult UserRegistration(UserInformationData userData);

        [OperationContract]
        LoginResultData UserLogin(LoginData loginData);
        
        [OperationContract]
        WaitingPlayerResult GetWaitingPlayr();


        
    }

    /// <summary>
    /// Operation stratus enumeration
    /// </summary>
    [DataContract(Name = "OperationStatus")]
    public enum OperationStatusnEnum
    {
        [EnumMember]
        Success,
        [EnumMember]
        Failure
    }

    /// <summary>
    /// Registration operation result
    /// </summary>
    [DataContract]
    public class RegistrationResult
    {
        OperationStatusnEnum status;
        string message;

        /// <summary>
        /// Operation status enumeration
        /// </summary>
        [DataMember]
        public OperationStatusnEnum Status
        {
            get { return status; }
            set { status = value; }
        }

        /// <summary>
        /// Failure explanation message
        /// </summary>
        [DataMember]
        public string Message
        {
            get { return message; }
            set { message = value; }
        }
    }

    [DataContract]
    public class LoginData
    {
        string emailAddress;
        string passwordHash;

        /// <summary>
        /// The user email address 
        /// </summary>
        [DataMember]
        public string EmailAddress
        {
            get { return emailAddress; }
            set { emailAddress = value; }
        }

        /// <summary>
        /// A hash on the password the user uses
        /// </summary>
        [DataMember]
        public string PasswordHash
        {
            get { return passwordHash; }
            set { passwordHash = value; }
        }
    }

    [DataContract]
    public class LoginResultData
    {
        string nickName;
        string sessionKey;
        OperationStatusnEnum status;

        /// <summary>
        /// The user nick name
        /// </summary>
        [DataMember]
        public string NickName
        {
            get { return nickName; }
            set { nickName = value; }
        }

        /// <summary>
        /// The current session key
        /// </summary>
        [DataMember]
        public string SessionKey
        {
            get { return sessionKey; }
            set { sessionKey = value; }
        }

        /// <summary>
        /// The operation result status
        /// </summary>
        [DataMember]
        public OperationStatusnEnum Status
        {
            get { return status; }
            set { status = value; }
        }


    }

    /// <summary>
    /// User details needed for registration
    /// </summary>
    [DataContract]
    public class UserInformationData
    {
        string firstName;
        string lastName;
        string emailAddress;
        string nickName;
        string passwordHash;

        /// <summary>
        /// The user first name 
        /// </summary>
        [DataMember]
        public string FirstName
        {
            get { return firstName; }
            set { firstName = value; }
        }

        /// <summary>
        /// The user last name 
        /// </summary>
        [DataMember]
        public string LastName
        {
            get { return lastName; }
            set { lastName = value; }
        }

        /// <summary>
        /// The user email address 
        /// </summary>
        [DataMember]
        public string EmailAddress
        {
            get { return emailAddress; }
            set { emailAddress = value; }
        }

        /// <summary>
        /// The user nick name
        /// </summary>
        [DataMember]
        public string NickName
        {
            get { return nickName; }
            set { nickName = value; }
        }

        /// <summary>
        /// A hash on the password the user uses
        /// </summary>
        [DataMember]
        public string PasswordHash
        {
            get { return passwordHash; }
            set { passwordHash = value; }
        }

    }


    [DataContract]
    public class WaitingPlayer
    {
        string nickName;

        /// <summary>
        /// The user nick name
        /// </summary>
        [DataMember]
        public string NickName
        {
            get { return nickName; }
            set { nickName = value; }
        }

    }

    /// <summary>
    /// The current waiting players  
    /// </summary>
    [DataContract]
    public class WaitingPlayerResult
    {
        List<WaitingPlayer> waitingPlayerslist;
        OperationStatusnEnum status;

        public WaitingPlayerResult()
        {
            waitingPlayerslist = new List<WaitingPlayer>();
        }

        /// <summary>
        /// The list of waiting players 
        /// </summary>
        [DataMember]
        public List<WaitingPlayer> WaitingPlayerslist
        {
            get { return waitingPlayerslist; }            
        }

        /// <summary>
        /// The operation result status
        /// </summary>
        [DataMember]
        public OperationStatusnEnum Status
        {
            get { return status; }
            set { status = value; }
        }
    }


    // Use a data contract as illustrated in the sample below to add composite types to service operations.
    // You can add XSD files into the project. After building the project, you can directly use the data types defined there, with the namespace "WcfMatixServiceLibrary.ContractType".
    [DataContract]
    public class CompositeType
    {
        bool boolValue = true;
        string stringValue = "Hello ";

        [DataMember]
        public bool BoolValue
        {
            get { return boolValue; }
            set { boolValue = value; }
        }

        [DataMember]
        public string StringValue
        {
            get { return stringValue; }
            set { stringValue = value; }
        }
    }
}
