﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Skylabs.Lobby;

namespace Octgn.Launcher
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Page
    {
        private readonly DispatcherTimer animationTimer;
        private bool bSpin = false;

        public Login()
        {
            InitializeComponent();
            SpinnerRotate.CenterX = image2.Width / 2;
            SpinnerRotate.CenterY = image2.Height / 2;
            animationTimer = new DispatcherTimer(DispatcherPriority.ContextIdle, Dispatcher);
            animationTimer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            versionText.Text = string.Format("Version {0}", OctgnApp.OctgnVersion.ToString(4));
#if(DEBUG)
            //TODO Remove this at some point
            MenuItem m = new MenuItem();
            m.Name = "menuOldMenu";
            m.Header = "Old Menu";
            m.Click += new RoutedEventHandler(menuOldMenu_Click);
            menuOctgn.Items.Add(m);
#endif
        }

        private void Start_Spinning()
        {
            if(!bSpin && animationTimer.IsEnabled == false)
            {
                bSpin = true;
                animationTimer.Tick += HandleAnimationTick;
                animationTimer.Start();
            }
        }

        private void Stop_Spinning()
        {
            bSpin = false;
        }

        private void HandleAnimationTick(object sender, EventArgs e)
        {
            SpinnerRotate.Angle = (SpinnerRotate.Angle + 10) % 360;
            if(SpinnerRotate.Angle == 0 && bSpin == false)
                animationTimer.Stop();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Start_Spinning();
            bError.Visibility = System.Windows.Visibility.Hidden;
            Program.lobbyClient = new Skylabs.Lobby.LobbyClient();
            bool c = Program.lobbyClient.Connected;
            if(!c)
                c = Program.lobbyClient.Connect(Program.LobbySettings.Server, Program.LobbySettings.ServerPort);
            if(c)
            {
                Program.lobbyClient.Login(LoginFinished, textBox1.Text, passwordBox1.Password);
            }
            else
                DoErrorMessage("Could not connect to the server.");
        }

        private void LoginFinished(LoginResult success, DateTime BanEnd)
        {
            Dispatcher.Invoke((Action)(() =>
            {
                Stop_Spinning();
                if(success == LoginResult.Success)
                {
                }
                else if(success == LoginResult.Banned)
                {
                    DoErrorMessage("You have been banned until " + BanEnd.ToShortTimeString() + " on " + BanEnd.ToShortDateString());
                }
                else if(success == LoginResult.Failure)
                {
                    DoErrorMessage("Login Failed");
                }
            }), new object[0] { });
        }

        private void menuExit_Click(object sender, RoutedEventArgs e)
        {
            Program.Exit();
        }

        private void menuOctgn_Click(object sender, RoutedEventArgs e)
        {
        }

        private void textBox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            bError.Visibility = System.Windows.Visibility.Hidden;
        }

        private void DoErrorMessage(string message)
        {
            Dispatcher.Invoke((Action)(() =>
            {
                lError.Text = message;
                bError.Visibility = System.Windows.Visibility.Visible;
            }), new object[0] { });
        }

        private void passwordBox1_PasswordChanged(object sender, RoutedEventArgs e)
        {
            bError.Visibility = System.Windows.Visibility.Hidden;
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            Register r = new Register();
            try
            {
                r.ShowDialog();
            }
            catch(Exception ex)
            {
            }
        }

        private void menuOldMenu_Click(object sender, RoutedEventArgs e)
        {
            //TODO This event and the menu need to be removed before release. This is only here for debugging purposes
            this.NavigationService.Navigate(new MainMenu());
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Uri.ToString());
        }
    }
}