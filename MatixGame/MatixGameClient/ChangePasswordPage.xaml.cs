using log4net;
using MatixGameClient.MatixGameServiceReference;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace MatixGameClient
{

    /// <summary>
    /// Interaction logic for ChangePasswordPage.xaml
    /// </summary>
    public partial class ChangePasswordPage : Page
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

        /// <summary>
        /// User email address
        /// </summary>
        private string email;

        /// <summary>
        /// User nickname
        /// </summary>
        private string nickName;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="_service">Reference to the WCF service host instance</param>
        /// <param name="_nickName">Player's nickname</param>
        /// <param name="_email">Player's email address</param>
        public ChangePasswordPage(MatixServiceClient _service, string _nickName, string _email)
        {
            InitializeComponent();
            service = _service;
            nickName = _nickName;
            email = _email;
            emailAddr.Content = email;
            loginName.Content = "Hi " + _nickName;
        }


        #region Class Private Methods

        /// <summary>
        /// Back button event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BackClicked(object sender, RoutedEventArgs e)
        {
            logger.Info("Back button clicked");

            UpdatePlayerDetailsPage updatePage = new UpdatePlayerDetailsPage(service, nickName, email);
            NavigationService.Navigate(updatePage);
        }

        /// <summary>
        /// Change password button event handler 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChangePassClicked(object sender, RoutedEventArgs e)
        {
            logger.Info("Change password button clicked");

            errorMessage.Content = "";

            if (oldPassTextBox.Password.Length == 0)
            {
                errorMessage.Content = "Enter an old password.";
                oldPassTextBox.Focus();
            }
            else if (newPassTextBox.Password.Length == 0)
            {
                errorMessage.Content = "Enter a new password.";
                newPassTextBox.Focus();
            }
            else if (confirmPassTextBox.Password.Length == 0)
            {
                errorMessage.Content = "Enter a Confirm password.";
                confirmPassLabel.Focus();
            }
            else if (newPassTextBox.Password != confirmPassTextBox.Password)
            {
                errorMessage.Content = "Confirm password must be the same\nas new the password.";
                confirmPassTextBox.Focus();
            }
            else
            {
                // Send information to server 
               OperationStatus result = service.ChangeUserPassword(email, oldPassTextBox.Password, newPassTextBox.Password);
                if(result == OperationStatus.Success)
                {
                    logger.Info("Password has been changed successfully");
                    MessageBox.Show("Password has been changed successfully.");

                    Properties.Settings.Default.password = newPassTextBox.Password;
                    Properties.Settings.Default.Save();

                    WelcomePage page = new WelcomePage(service, nickName, email);
                    NavigationService.Navigate(page);
                }
                else
                {
                    logger.WarnFormat("Failed to update password - email: {0}, Old Password: {1},  New Password: {2}", email, oldPassTextBox.Password, newPassTextBox.Password);

                    errorMessage.Content = "Failed to update password.";
                    oldPassTextBox.Focus();
                }
            }

        }
        #endregion
    }
}
