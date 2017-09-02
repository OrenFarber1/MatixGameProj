using log4net;
using MatixGameClient.MatixGameServiceReference;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace MatixGameClient
{
    /// <summary>
    /// Interaction logic for Registration.xaml
    /// </summary>
    public partial class RegistrationPage : Page
    {
        #region Class Private Members 

        /// <summary>
        /// A class logger instance  
        /// </summary> 
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Reference to the WCF service instance 
        /// </summary>
        private MatixServiceClient service = null;

        #endregion

        public RegistrationPage(MatixGameServiceReference.MatixServiceClient _service)
        {
            InitializeComponent();
            service = _service;
        }

        private void BackClicked(object sender, RoutedEventArgs e)
        {
            logger.Info("Back button clicked");
            BackToLogin();
        }

        private void BackToLogin()
        {
            LoginPage login = new LoginPage(service);
            NavigationService.Navigate(login);
        }

        private void RegisterClicked(object sender, RoutedEventArgs e)
        {
            logger.Info("RegisterClicked");

           if (firstNameTextBox.Text.Length == 0)
            {
                errorMessage.Content = "Enter first name.";
                firstNameTextBox.Focus();
            }
            else if (lastNameTextBox.Text.Length == 0)
            {
                errorMessage.Content = "Enter last name.";
                lastNameTextBox.Focus();
            }
            else if (nickNameTextBox.Text.Length == 0)
            {
                errorMessage.Content = "Enter nick name.";
                nickNameTextBox.Focus();
            }
            else if (emailAddrTextBox.Text.Length == 0)
            {
                errorMessage.Content = "Enter email.";
                emailAddrTextBox.Focus();
            }
            else if (!Regex.IsMatch(emailAddrTextBox.Text, @"^[a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]$"))
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
            else if (confirmPassTextBox.Password.Length == 0)
            {
                errorMessage.Content = "Enter Confirm password.";
                confirmPassLabel.Focus();
            }
            else if (PassTextBox.Password != confirmPassTextBox.Password)
            {
                errorMessage.Content = "Confirm password must be same as password.";
                confirmPassTextBox.Focus();
            }
            else
            {
                UserInformationData userData = new UserInformationData();
                userData.FirstName = firstNameTextBox.Text;
                userData.LastName = lastNameTextBox.Text;
                userData.NickName = nickNameTextBox.Text;
                userData.EmailAddress = emailAddrTextBox.Text;
                userData.Password = PassTextBox.Password;

                RegistrationResult result = service.UserRegistration(userData);

                if(result.Status == OperationStatus.Success)
                {
                    BackToLogin();
                }
                else
                {
                    MessageBox.Show("Registration Failed: " + result.Message);
                }
                
            }
        }
    }
}
