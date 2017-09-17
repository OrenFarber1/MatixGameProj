using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatixDatabaseLibrary
{
    public interface IMatixDataAccessInterface
    {
        bool IsPlayerEmailExist(string email);

        string GetPlayerNickName(string email);

        bool CheckEmailAndPasswordHash(string email, string passwordHash);

        void AddPlayer(string firstName, string lastName, string nickName, string email, string passwordHash);

        void UpdatePlayerInformation(string email, string firstName, string lastName, string nickName);

        void ChangePassword(string email, string newPawwsordHash);

        bool PlayerLogin(string email, string passwordHash, string ip, out long lognId);

        bool PlayerLogout(long lognId, string email, string reason);
        
        PlayerScoreData GetPlayerStatistics(string email);

        PlayerScoreData GetWaitingPlayerData(string email);

        long CreateNewGame(string horizontalEmail, string verticalEmail, string boardXml);
        
        bool AddGameAction(string email, long gameId, int row, int column, int value);

        void AddPlayerHistory(string email, long gameId, bool win, int score);
        
    }
}
