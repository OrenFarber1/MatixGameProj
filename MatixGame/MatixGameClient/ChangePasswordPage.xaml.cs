using log4net;
using MatixGameClient.MatixGameServiceReference;
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
    /// Interaction logic for ChangePasswordPage.xaml
    /// </summary>
    public partial class ChangePasswordPage : Page
    {
        /// <summary>
        /// A class logger instance  
        /// </summary>
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Reference to the WCF service instance 
        /// </summary>
        private MatixGameServiceReference.MatixServiceClient service = null;
        private string nickName;
        

        public ChangePasswordPage(MatixGameServiceReference.MatixServiceClient _service, string _nickName, string _email)
        {
            InitializeComponent();
            service = _service;
            nickName = _nickName;
            emailAddr.Content = _email;
            loginName.Content = "Hi " + _nickName;
        }

        private void BackClicked(object sender, RoutedEventArgs e)
        {
            UpdatePlayerDetailsPage updatePage = new UpdatePlayerDetailsPage(service, nickName, emailAddr.Content.ToString());
            NavigationService.Navigate(updatePage);
        }

        private void ChangePassClicked(object sender, RoutedEventArgs e)
        {
            if(oldPassTextBox.Password.Length == 0)
            {
                errorMessage.Content = "Enter old password.";
                oldPassTextBox.Focus();
            }
            else if (newPassTextBox.Password.Length == 0)
            {
                errorMessage.Content = "Enter new password.";
                newPassTextBox.Focus();
            }
            else if (confirmPassTextBox.Password.Length == 0)
            {
                errorMessage.Content = "Enter Confirm password.";
                confirmPassLabel.Focus();
            }
            else if (newPassTextBox.Password != confirmPassTextBox.Password)
            {
                errorMessage.Content = "Confirm password must be same as new password.";
                confirmPassTextBox.Focus();
            }
            else
            {
                // Send information to server 

            }

        }
    }
}
