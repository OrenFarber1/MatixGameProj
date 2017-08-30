using log4net;
using MatixGameClient.MatixGameServiceReference;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace MatixGameClient
{
    /// <summary>
    /// Interaction logic for UpdatePlayerDetailsPage.xaml
    /// </summary>
    public partial class UpdatePlayerDetailsPage : Page
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
        /// Current players nickname 
        /// </summary>
        private string nickName;
  
        #endregion
              
        public UpdatePlayerDetailsPage(MatixServiceClient _service, string _nickName, string _email)
        {
            InitializeComponent();
            service = _service;
            nickName = _nickName;
            emailAddr.Content = _email;
            loginName.Content = "Hi " + _nickName;            
        }

        /// <summary>
        /// User select to back to the welcome page 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BackClicked(object sender, RoutedEventArgs e)
        {
            logger.Info("Back button clicked");

            WelcomePage page = new WelcomePage(service, nickName, emailAddr.Content.ToString());
            NavigationService.Navigate(page);
        }

        /// <summary>
        /// User clicked to update its details. The method validates the parameters and notify if something is missing 
        /// On success the method navigate to the welcome page 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateClicked(object sender, RoutedEventArgs e)
        {
            logger.Info("User click to update details");

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
            else
            {
                UserInformationData userData = new UserInformationData();
                userData.EmailAddress = (string)emailAddr.Content;
                userData.FirstName = firstNameTextBox.Text;
                userData.LastName = lastNameTextBox.Text;
                userData.NickName = nickNameTextBox.Text;

                if(service.UpdateUserDetailes(userData) == OperationStatus.Success)
                {
                    MessageBox.Show("User details updated successfully");
                    WelcomePage page = new WelcomePage(service, userData.NickName, emailAddr.Content.ToString());
                    NavigationService.Navigate(page);
                }
                else
                {
                    MessageBox.Show("Failed to update User details!");
                }
            }

        }

        /// <summary>
        /// User click to change password so open the change password page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChangePassClicked(object sender, RoutedEventArgs e)
        {
            logger.Info("User click to change password");

            ChangePasswordPage page = new ChangePasswordPage(service, nickName, emailAddr.Content.ToString());
            NavigationService.Navigate(page);
        }
    }
}
