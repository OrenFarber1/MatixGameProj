using log4net;
using MatixGameClient.MatixGameServiceReference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Reference to the WCF service instance 
        /// </summary>
        private MatixGameServiceReference.MatixServiceClient service = null;

        /// <summary>
        /// Construct a LoginPage
        /// </summary>
        /// <param name="_service"></param>
        public LoginPage(MatixGameServiceReference.MatixServiceClient _service)
        {
            InitializeComponent();
            service = _service;
        }

        /// <summary>
        /// Handle login button event. Send information to server
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loginButton_Click(object sender, RoutedEventArgs e)
        {
            logger.Info("loginButton_Click");
            
            if (!Regex.IsMatch(emailAddrTextBox.Text, @"^[a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]$"))
            {
                errorMessage.Content = "Enter a valid email.";
                emailAddrTextBox.Select(0, emailAddrTextBox.Text.Length);
                emailAddrTextBox.Focus();
            }

            else if (PassTextBox.Password.Length == 0)
            {
                errorMessage.Content = "Enter password.";
                PassTextBox.Focus();
            }
            else
            {
                using (new CursorWait())
                {
                    LoginData loginData = new LoginData();

                    loginData.EmailAddress = emailAddrTextBox.Text;
                    loginData.Password = PassTextBox.Password;

                    LoginResultData result = service.UserLogin(loginData);

                    if (result.Status == OperationStatus.Failure)
                    {

                    }
                    else
                    {

                    }
                }
            }
        }

        /// <summary>
        /// Open a Registration page 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void registerButton_Click(object sender, RoutedEventArgs e)
        {
            RegistrationPage register = new RegistrationPage(service);
            NavigationService.Navigate(register);
        }

        /// <summary>
        /// Back to the Welcome page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            WelcomePage welcome = new WelcomePage(service);
            NavigationService.Navigate(welcome);
        }
        
    }
}
