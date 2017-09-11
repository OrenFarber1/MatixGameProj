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
        /// <summary>
        /// Allow the client to be notified about changes in the waiting players 
        /// </summary>
        /// <param name="waitingPlayers"></param>
        [OperationContract(IsOneWay = true)]
        void UpdateWaitingPlayer(WaitingPlayerResult waitingPlayers);

        /// <summary>
        /// Allow the client to be notified about starting of a new game
        /// </summary>
        /// <param name="matixBoard">The generated game board</param>
        /// <param name="horizontalNickname">Horizontal player's nickname</param>
        /// <param name="verticalNickName">Vertical player's nickname</param>
        /// <param name="whoIsStarting"></param>
        [OperationContract(IsOneWay = true)]
        void StartingNewGame(MatixBoard matixBoard, string horizontalNickname, string verticalNickName, GameTurnTypeEnum whoIsStarting);

        /// <summary>
        /// Allow the client to be notified with the second player move 
        /// </summary>
        /// <param name="row">The new token row</param>
        /// <param name="column">The new token column</param>
        /// <param name="score">The new token score</param>
        [OperationContract(IsOneWay = true)]
        void UpdateGameAction(int row, int column, int score);

        /// <summary>
        /// Allow the client to be notified when a game ends
        /// </summary>
        /// <param name="winnerNickname">The winner nickname</param>
        /// <param name="score">The winner score</param>
        [OperationContract(IsOneWay = true)]
        void UpdateGameEnded(string winnerNickname, int score);

    }
}
