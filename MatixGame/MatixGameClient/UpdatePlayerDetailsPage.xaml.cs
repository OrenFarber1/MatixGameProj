﻿using log4net;
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
    /// Interaction logic for UpdatePlayerDetailsPage.xaml
    /// </summary>
    public partial class UpdatePlayerDetailsPage : Page
    {
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Reference to the WCF service instance 
        /// </summary>
        private MatixGameServiceReference.MatixServiceClient service = null;
        private string nickName;
      
        public UpdatePlayerDetailsPage(MatixGameServiceReference.MatixServiceClient _service, string _nickName, string _email)
        {
            InitializeComponent();
            service = _service;
            nickName = _nickName;
            emailAddr.Content = _email;
            loginName.Content = "Hi " + _nickName;            
        }

        private void BackClicked(object sender, RoutedEventArgs e)
        {
            WelcomePage page = new WelcomePage(service, nickName, emailAddr.Content.ToString());
            NavigationService.Navigate(page);
        }

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
                // Send new information to server 
            }

        }

        private void ChangePassClicked(object sender, RoutedEventArgs e)
        {
            logger.Info("User click to change password");

            ChangePasswordPage page = new ChangePasswordPage(service, nickName, emailAddr.Content.ToString());
            NavigationService.Navigate(page);
        }
    }
}