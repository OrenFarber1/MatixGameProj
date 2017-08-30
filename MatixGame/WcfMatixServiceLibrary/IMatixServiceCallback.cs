using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace WcfMatixServiceLibrary
{
    /// <summary>
    /// Matix game callback service methods. Defines the  messages the server can sends to the client 
    /// </summary>
    public interface IMatixServiceCallback
    {

        [OperationContract(IsOneWay = true)]
        void Ping(int value);

        [OperationContract(IsOneWay = true)]
        void UpdateWaitingPlayer(WaitingPlayerResult waitingPlayers);

        [OperationContract(IsOneWay = true)]
        void GetMatixBoard(MatixBoard matixBoard, string horizontalNickname, string verticalNickName, GameTurnTypeEnum whoIsStarting);

        [OperationContract(IsOneWay = true)]
        void UpdateGameAction(int row, int col, int score);

        [OperationContract(IsOneWay = true)]
        void UpdateGameEnded(string winnerNickname, int score);

    }
}
