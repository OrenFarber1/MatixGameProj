using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WcfMatixServiceLibrary
{
    public interface IMatixBuisnessInterface
    {
        void SetMatixWcfService(MatixWcfService matixService);

        RegistrationResult AddPlayer(string firstName, string lastName, string nickName, string email, string password);

        LoginResult UserLogin(string email, string password, string ipAddress);

        WaitingPlayerResult GetWaitingPlayersList(string excludedEmail);

        OperationStatusnEnum StartPlayingWithPlayer(string firstEmail, string nickName);

        OperationStatusnEnum SetGameAction(string email, int row, int col);
    }
}
