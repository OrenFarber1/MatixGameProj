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
        /// <summary>
        /// A class logger instance  
        /// </summary>
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Reference to the WCF service instance 
        /// </summary>
        private MatixServiceClient service = null;

        /// <summary>
        /// Construct a LoginPage
        /// </summary>
        /// <param name="_service"></param>
        public LoginPage(MatixServiceClient _service)
        {
            InitializeComponent();
            service = _service;
            emailAddrTextBox.Focus();
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

                    LoginResult result = service.UserLogin(loginData);

                    switch (result.Status)
                    {
                        case OperationStatus.Success:
                            {
                                Properties.Settings.Default.email = emailAddrTextBox.Text;
                                Properties.Settings.Default.password = PassTextBox.Password;
                                Properties.Settings.Default.nickname = result.Nickname;

                                Properties.Settings.Default.Save();

                                WelcomePage welcome = new WelcomePage(service, result.Nickname, emailAddrTextBox.Text);
                                NavigationService.Navigate(welcome);
                            }
                            break;
                        case OperationStatus.Failure:
                            errorMessage.Content = "Internal server error";
                            break;
                        case OperationStatus.InvalidEmail:
                            errorMessage.Content = "Email address doesn't exist";
                            break;
                        case OperationStatus.InvalidPassword:
                            errorMessage.Content = "The password is incorrect";
                            break;

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
            logger.Info("Back button clicked");

            WelcomePage welcome = new WelcomePage(service);
            NavigationService.Navigate(welcome);
        }

    }
}
