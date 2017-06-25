using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// Interaction logic for GamePage.xaml
    /// </summary>
    public partial class GamePage : Page
    {
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private MatixGameServiceReference.MatixServiceClient service = null;
        private string nickName;
        private string email;


        public GamePage(MatixGameServiceReference.MatixServiceClient _service, string _nickName, string _email)
        {
            InitializeComponent();
            nickName = _nickName;
            email = _email;
            service = _service;
            loginName.Content = "Hi " + _nickName;
        }

        /// <summary>
        /// Back to the Welcome page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            WelcomePage welcome = new WelcomePage(service, nickName, email);
            NavigationService.Navigate(welcome);
        }
    }
}
