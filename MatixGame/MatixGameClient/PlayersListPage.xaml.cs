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
    /// Interaction logic for PlayersListPage.xaml
    /// </summary>
    public partial class PlayersListPage : Page
    {
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private MatixGameServiceReference.MatixServiceClient service = null;
        private string nickName;
        private string email;

        public PlayersListPage(MatixGameServiceReference.MatixServiceClient _service, string _nickName, string _email)
        {
            InitializeComponent();
            service = _service;
            nickName = _nickName;
            email = _email;
            loginName.Content = "Hi " + _nickName;

            // Get waiting players from server 
            WaitingPlayerResult results = service.GetWaitingPlayers(email);

            lvWaitingPlayers.ItemsSource = results.WaitingPlayerslist;
            
        }

        /// <summary>
        /// Handle double click event on the waiting players.  
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ListViewDoubleClick(object sender, RoutedEventArgs e)
        {
            //grab the original element that was double clicked on and search from child to parent until
            //you find either a ListViewItem or the top of the tree
            DependencyObject originalSource = (DependencyObject)e.OriginalSource;
            while ((originalSource != null) && !(originalSource is ListViewItem))
            {
                originalSource = VisualTreeHelper.GetParent(originalSource);
            }
         
            // if it didn't find a ListViewItem anywhere in the hierarchy, it's because the user
            // didn't click on one. Therefore, if the variable isn't null, run the code
            if (originalSource != null)
            {
                WaitingPlayer selectedPlayer = ((ListViewItem)originalSource).Content as WaitingPlayer;
                string nnn = selectedPlayer.NickName;
            }
        }

        /// <summary>
        /// Back to the Welcome page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BackClicked(object sender, RoutedEventArgs e)
        {
            WelcomePage welcome = new WelcomePage(service, nickName, email);
            NavigationService.Navigate(welcome);
        }
    }
}