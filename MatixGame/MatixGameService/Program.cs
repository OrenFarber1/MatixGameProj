using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace MatixWindowsService
{
    static class Program
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            // BasicConfigurator replaced with XmlConfigurator.
            log4net.Config.XmlConfigurator.Configure();

            Log.Info("************************************************************************");

            if (!Environment.UserInteractive)
            {
                Log.Info("Matix Application Server started as a service");

                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                    new MatixWindowsService(Log)
                };
                ServiceBase.Run(ServicesToRun);
            }
            else
            {
                Log.Info("Matix Application Server started as a console application");

                var service = new MatixWindowsService(Log);
                service.StartAsConsole(null);
            }
        }
    }
}
