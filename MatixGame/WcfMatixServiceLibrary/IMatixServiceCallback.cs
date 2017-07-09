using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace WcfMatixServiceLibrary
{
    /// <summary>
    /// Matix game callback service operations
    /// </summary>
    public interface IMatixServiceCallback
    {

        [OperationContract(IsOneWay = true)]
        void Ping(int value);

        [OperationContract(IsOneWay = true)]
        void UpdateWaitingPlayr(WaitingPlayerResult waitingPlayers);

        [OperationContract(IsOneWay = true)]
        void GetMatixBoard(MatixBoard matixBoard);

        [OperationContract(IsOneWay = true)]
        void UpdateGameAction(int row, int col);
    }
}
