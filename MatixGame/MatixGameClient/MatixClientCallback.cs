using log4net;
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
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // Saves the UI synchronization context
        private SynchronizationContext uiSyncContext = null;
        private MainWindow ownerWindow = null;


        public MatixClientCallback(SynchronizationContext syncContext)             
        {
            uiSyncContext = syncContext;
          //  ownerWindow = owner;
        }

        public void GetMatixBoard(MatixBoard matixBoard, string horizontalNickname, string verticalNickName, GameTurnTypeEnum whoIsStarting)
        {
            logger.InfoFormat("GetMatixBoard horizontalNickname: {0}, verticalNickName: [1}", horizontalNickname, verticalNickName);
            
            SendOrPostCallback callback = (x => ownerWindow.SetMatixBoard(matixBoard, horizontalNickname, verticalNickName, whoIsStarting));

            uiSyncContext.Post(callback, null);
            
        }

        public void Ping(int value)
        {
            MessageBox.Show("Ping: " + value);
        }

        public void UpdateGameAction(int row, int col, int score)
        {
            logger.InfoFormat("UpdateGameAction - row: {0}, col: {1}, score: {2}", row, col, score);

            SendOrPostCallback callback = (x => ownerWindow.UpdateGameAction(row, col, score));

            uiSyncContext.Post(callback, null);            
        }

        public void UpdateWaitingPlayer(WaitingPlayerResult waitingPlayers)
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
