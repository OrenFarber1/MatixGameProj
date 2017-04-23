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
        private static MatixClientCallback callback = new MatixClientCallback(SynchronizationContext.Current);
        private MatixGameServiceReference.MatixServiceClient service = new MatixGameServiceReference.MatixServiceClient(new InstanceContext(callback), "NetTcpBinding_IMatixService");

        public MainWindow()
        { 
            InitializeComponent();

       ///     MessageBox.Show("Before!");
            try
            {
                service.Open();

                ((ICommunicationObject)service).Faulted += new EventHandler(delegate
                {
                    MessageBox.Show("Service faulted!");
                });

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);

            }
            
           /// MessageBox.Show("After!");
            ////        string s = service.GetData(12);

            WelcomePage welcome = new WelcomePage(service);
           mainFrame.NavigationService.Navigate(welcome);
        }
    }
}
