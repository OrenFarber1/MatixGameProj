using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MatixGameClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// logger
        /// </summary>
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Callback instance 
        /// </summary>
        private static MatixClientCallback callback = new MatixClientCallback(SynchronizationContext.Current);
        private MatixGameServiceReference.MatixServiceClient service = new MatixGameServiceReference.MatixServiceClient(new InstanceContext(callback), "NetTcpBinding_IMatixService");

        public MainWindow()
        { 
            InitializeComponent();

            bool showWelcom = false;

            try
            {
                //Open connection to the server 
                service.Open();

                ((ICommunicationObject)service).Faulted += new EventHandler(delegate
                {
                    MessageBox.Show("Service faulted!");
                });

                showWelcom = true;
            }
            catch (Exception e)
            {
               logger.ErrorFormat("Main Window - Exception: {0}", e.Message);
               showWelcom = false;
            }

            if (showWelcom)
            {
                WelcomePage welcome = new WelcomePage(service);
                mainFrame.NavigationService.Navigate(welcome);
            }
            else
            {
                string message = "Fail to connect to server " + Environment.NewLine + "Try to connect later!";
                ErrorPage errorPage = new ErrorPage(message);
                mainFrame.NavigationService.Navigate(errorPage);
            }
        }
    }  
}
