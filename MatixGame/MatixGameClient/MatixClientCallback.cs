using MatixGameClient.MatixGameServiceReference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace MatixGameClient
{
    [CallbackBehavior(
       ConcurrencyMode = ConcurrencyMode.Single,
       UseSynchronizationContext = false)]
    public  class MatixClientCallback : IMatixServiceCallback
    {
        // Saves the UI synchronization context
        private SynchronizationContext uiSyncContext = null;


        public MatixClientCallback(SynchronizationContext syncContext)             
        {
            uiSyncContext = syncContext;
        }

        public void GetMatixBoard(MatixBoard matixBoard, string horizontalNickname, string verticalNickName, GameTurnTypeEnum whoIsStarting)
        {
            SendOrPostCallback callback = state => SetMatixBoard(matixBoard, horizontalNickname, verticalNickName, whoIsStarting);


            SetMatixBoard
          //  matixBoard.MatixCells
        }

        public void Ping(int value)
        {
            MessageBox.Show("Ping: " + value);
        }

        public void UpdateGameAction(int row, int col)
        {
            throw new NotImplementedException();
        }

        public void UpdateWaitingPlayr(WaitingPlayerResult waitingPlayers)
        {
            // The UI thread won't be handling the callback, but it is the only one allowed to update the controls.  
            // So, we will dispatch the UI update back to the UI sync context.
            //SendOrPostCallback callback =
            //    delegate (object state)
            //    { this.WritePartyLogMessage(String.Format("{0} has joined the party.", state.ToString())); };

            //uiSyncContext.Post(callback, guestName);
        }
    }
}
