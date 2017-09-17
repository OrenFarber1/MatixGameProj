using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WcfMatixServiceLibrary
{
    /// <summary>
    /// The Matix business layer interface  
    /// </summary>
    public interface IMatixBusinessInterface
    {
        void SetMatixWcfService(MatixWcfService matixService);

        void ClientDisconnected(string email);

        RegistrationResult AddPlayer(string firstName, string lastName, string nickName, string email, string password);

        OperationStatusEnum UpdatePlayer(string email, string firstName, string lastName, string nickName);

        OperationStatusEnum ChangePassword(string email, string oldPassword, string newPawwsord);

        LoginResult UserLogin(string email, string password, string ipAddress);

        OperationStatusEnum UserLogout(string email, string reason);

        WaitingPlayerResult GetWaitingPlayersList(string excludedEmail);

        OperationStatusEnum StartPlayingWithPlayer(string firstEmail, string nickName);

        OperationStatusEnum StartPlayingWithRobot(string firstEmail);

        OperationStatusEnum SetGameAction(string email, int row, int col);

        PlayerStatisticsResult GetPlayerStatistics(string email);

        void RemoveFromWaitingPlayers(string email);
        
        void QuitTheGame(string email);

    }
}
