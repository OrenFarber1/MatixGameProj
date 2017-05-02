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

namespace MatixWindowsService
{
    public partial class MatixWindowsService : ServiceBase
    {
        private ILog Log = null;
        internal static ServiceHost gameServiceHost = null;
        
        public MatixWindowsService(ILog logRef)
        {
            Log = logRef;
            InitializeComponent();
        }

        public void StartAsConsole(string[] args)
        {
            OnStart(args);
            Console.WriteLine("Press any key to stop the Matix Server");
            Console.Read();
            OnStop();
        }

        protected override void OnStart(string[] args)
        {
            if (gameServiceHost != null)
            {
                gameServiceHost.Close();
            }

            // Note that when an object is provided to this overload, 
            // some features related to the Windows Communication Foundation (WCF) instancing
            // behavior work differently. For example, calling InstanceContext.ReleaseServiceInstance
            // have no effect when a well-known object instance is provided using this constructor 
            // overload. Similarly, any other instance release mechanism is ignored. The ServiceHost 
            // always behaves as if the OperationBehaviorAttribute.ReleaseInstanceMode property is 
            // set to ReleaseInstanceMode.None for all operations.

            //MatixWcfService sevice = new MatixWcfService(Log);
            gameServiceHost = new ServiceHost(typeof(MatixWcfService));
            gameServiceHost.Open();
        }

        protected override void OnStop()
        {
            if (gameServiceHost != null)
            {
                gameServiceHost.Close();
                gameServiceHost = null;
            }
        }
    }
}
