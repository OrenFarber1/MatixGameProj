using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using WcfMatixServiceLibrary;
using log4net;
using System.Threading;
using MatixBusinessLibrary;

namespace MatixWindowsService
{
    public partial class MatixWindowsService : ServiceBase
    {        
        /// <summary>
        /// A class logger instance  
        /// </summary>
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private ManualResetEvent shutdownEvent = new ManualResetEvent(false);
        private Thread thread;

        private MatixGameManager matixGameManager = null;

        internal static ServiceHost gameServiceHost = null;
        
        public MatixWindowsService(ILog logRef)
        {
            logger.Info("MatixWindowsService Created");
            InitializeComponent();
        }

        public void StartAsConsole(string[] args)
        {
            logger.Info("Started As Console...");

            OnStart(args);
            Console.WriteLine("Press any key to stop the Matix Server");
            Console.Read();
            OnStop();

            logger.Info("Ended As Console...");
        }

        protected override void OnStart(string[] args)
        {
            logger.Info("OnStart - Started");

            thread = new Thread(WorkerThreadFunc);
            thread.Name = "Service Worker Thread";
            thread.IsBackground = true;
            thread.Start();
        }

        protected override void OnStop()
        {
            logger.Info("OnStop - Started");

            shutdownEvent.Set();
            if (!thread.Join(3000))
            { 
                // give the thread 3 seconds to stop
                thread.Abort();
            }
        }

        private void WorkerThreadFunc()
        {
            logger.Info("WorkerThreadFunc - Started");

            if (gameServiceHost != null)
            {
                gameServiceHost.Close();
            }
    
            // Create the business layer instance 
            matixGameManager = new MatixGameManager();

            while (!shutdownEvent.WaitOne(0))
            {
                // Replace the Sleep() call with the work you need to do
                Thread.Sleep(1000);
            }

            logger.Info("WorkerThreadFunc - Thread loop ended");
            
            if (gameServiceHost != null)
            {
                gameServiceHost.Close();
                gameServiceHost = null;
            }

            logger.Info("WorkerThreadFunc - Ended");
        }
    }
}
