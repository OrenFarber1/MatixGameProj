using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace WcfMatixServiceLibrary
{
    /// <summary>
    /// The Matix service interface. Defines all the messages that can be used between the client and the server.
    /// </summary>
    [ServiceContract(
    SessionMode = SessionMode.Required,
    CallbackContract = typeof(IMatixServiceCallback))]
    public interface IMatixService
    {
        [OperationContract]
        RegistrationResult UserRegistration(UserInformationData userData);

        [OperationContract]
        OperationStatusEnum UpdateUserDetailes(UserInformationData userData);

        [OperationContract]
        OperationStatusEnum ChangeUserPassword(string email, string oldPassword, string newPawwsord);

        [OperationContract]
        LoginResult UserLogin(LoginData loginData);

        [OperationContract]
        OperationStatusEnum UserLogout(string email, string reason);

        [OperationContract]
        WaitingPlayerResult GetWaitingPlayers(string excludedEmail);

        [OperationContract]
        OperationStatusEnum SelectPlayerToPlay(string email, string nickName);

        [OperationContract]
        OperationStatusEnum SelectRobotToPlay(string email);

        [OperationContract]
        OperationStatusEnum SetGameAction(string email, int row, int col);

        [OperationContract]
        void RemoveFromWaitingPlayers(string email);

        [OperationContract]
        PlayerStatisticsResult GetPlayerStatistics(string email);

        [OperationContract]
        void QuitTheGame(string email);
    }

    /// <summary>
    /// Operation stratus enumeration
    /// </summary>
    [DataContract(Name = "OperationStatus")]
    public enum OperationStatusEnum
    {
        [EnumMember]
        Success,
        [EnumMember]
        Failure,
        [EnumMember]
        InvalidEmail,
        [EnumMember]
        InvalidPassword,
        [EnumMember]
        OperationtimeOut,
        [EnumMember]
        Rejected,
        [EnumMember]
        InvalidAction
    }

    /// <summary>
    /// Enumeration the player's direction 
    /// </summary>
    [DataContract]
    public enum GameTurnTypeEnum
    {
        [EnumMember]
        HorizontalPlayer,
        [EnumMember]
        VerticalPlayer
    }

    /// <summary>
    /// Registration operation result
    /// </summary>
    [DataContract]
    public class RegistrationResult
    {
        OperationStatusEnum status;
        string message;

        /// <summary>
        /// Operation status enumeration
        /// </summary>
        [DataMember]
        public OperationStatusEnum Status
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

    /// <summary>
    /// The class contains the player information for login message 
    /// </summary>
    [DataContract]
    public class LoginData
    {
        string emailAddress;
        string password;

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
        public string Password
        {
            get { return password; }
            set { password = value; }
        }
    }

    /// <summary>
    /// Log in result information 
    /// </summary>
    [DataContract]
    public class LoginResult
    {
        string nickName;
        OperationStatusEnum status;

        /// <summary>
        /// The user nick name
        /// </summary>
        [DataMember]
        public string Nickname
        {
            get { return nickName; }
            set { nickName = value; }
        }

        /// <summary>
        /// The operation result status
        /// </summary>
        [DataMember]
        public OperationStatusEnum Status
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
        string password;

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
        public string Password
        {
            get { return password; }
            set { password = value; }
        }

    }

    /// <summary>
    /// Waiting player statistics information 
    /// </summary>
    [DataContract]
    public class WaitingPlayer
    {
        string nickName;
        int totalGames;
        int numberOfWinnings;
        int totalScore;

        /// <summary>
        /// A player's nickname
        /// </summary>
        [DataMember]
        public string NickName
        {
            get { return nickName; }
            set { nickName = value; }
        }

        /// <summary>
        /// A player's total number of games
        /// </summary>
        [DataMember]
        public int TotalGames
        {
            get { return totalGames; }
            set { totalGames = value; }
        }

        /// <summary>
        /// A player's total number of winnings 
        /// </summary>
        [DataMember]
        public int NumberOfWinnings
        {
            get { return numberOfWinnings; }
            set { numberOfWinnings = value; }
        }

        /// <summary>
        /// A player's total score (value can be negative)
        /// </summary>
        [DataMember]
        public int TotalScore
        {
            get { return totalScore; }
            set { totalScore = value; }
        }
    }

    /// <summary>
    /// The current waiting players  
    /// </summary>
    [DataContract]
    public class WaitingPlayerResult
    {
        List<WaitingPlayer> waitingPlayerslist;
        OperationStatusEnum status;

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
        public OperationStatusEnum Status
        {
            get { return status; }
            set { status = value; }
        }
    }

    /// <summary>
    /// The class encapsulates a Matix cell information
    /// </summary>
    [DataContract]
    public class MatixCell
    {
        int row;
        int column;
        int value;
        bool token;
        bool used;

        /// <summary>
        /// Constructor
        /// </summary>
        public MatixCell()
        {
            row = -1;
            column = -1;
            value = 0;
            token = false;
            used = false;
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="other"></param>
        public MatixCell(MatixCell other)
        {
            row = other.row;
            column = other.column;
            value = other.value;
            token = other.token;
            used = other.used;
        }

        /// <summary>
        /// Cell row index
        /// </summary>
        [DataMember]
        public int Row
        {
            get { return row; }
            set { row = value; }
        }

        /// <summary>
        /// Cell column index
        /// </summary>
        [DataMember]
        public int Column
        {
            get { return column; }
            set { column = value; }
        }

        /// <summary>
        /// Cell value 
        /// </summary>
        [DataMember]
        public int Value
        {
            get { return value; }
            set { this.value = value; }
        }

        /// <summary>
        /// A flag indicate whether the cell is a token or not
        /// </summary>
        [DataMember]
        public bool Token
        {
            get { return token; }
            set { token = value; }
        }

        /// <summary>
        /// A flag indicate whether the cell is already used
        /// </summary>
        [DataMember]
        public bool Used
        {
            get { return used; }
            set { used = value; }
        }
    }

    /// <summary>
    /// The class encapsulates a Matix board 
    /// </summary>
    [DataContract]
    public class MatixBoard
    {
        /// <summary>
        /// List of MatrixCells lists
        /// </summary>
        List<List<MatixCell>> matixCells;

        /// <summary>
        /// List of MatrixCells lists
        /// </summary>
        [DataMember]
        public List<List<MatixCell>> MatixCells
        {
            get { return matixCells; }
            set { matixCells = value; }
        }

    }

    /// <summary>
    /// The class encapsulates statistic information of a player
    /// </summary>
    [DataContract]
    public class PlayerStatisticsResult
    {
        private int rank;
        private int numberOfGames;
        private int winnings;
        private double averageScore;

        [DataMember]
        public int Rank
        {
            get { return rank; }
            set { rank = value; }
        }


        [DataMember]
        public int NumberOfGames
        {
            get { return numberOfGames; }
            set { numberOfGames = value; }
        }


        [DataMember]
        public int Winnings
        {
            get { return winnings; }
            set { winnings = value; }
        }

        [DataMember]
        public double AverageScore
        {
            get { return averageScore; }
            set { averageScore = value; }
        }
    }
}
