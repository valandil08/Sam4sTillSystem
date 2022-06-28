using Sam4sTillSystem.Controls;
using Sam4sTillSystem.Data.JsonObject;
using Sam4sTillSystem.Data.JsonObject.SiteSetting;
using Sam4sTillSystem.Enum;
using Sam4sTillSystem.FileFunctions;
using System;
using System.Net.Http;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace Sam4sTillSystem.Screens
{
    /// <summary>
    /// Interaction logic for CloudUplinkScreen.xaml
    /// </summary>
    public partial class LoginScreen : UserControl
    {
        private static string Pin = "";

        public LoginScreen()
        {
            InitializeComponent();

            UserLoginsFile.LoadSettings();

            LoadLogins();
        }

        private void LoadLogins(int page = 0)
        {

            LoginButton0.Visibility = Visibility.Hidden;
            LoginButton1.Visibility = Visibility.Hidden;
            LoginButton2.Visibility = Visibility.Hidden;
            LoginButton3.Visibility = Visibility.Hidden;
            LoginButton4.Visibility = Visibility.Hidden;
            LoginButton5.Visibility = Visibility.Hidden;
            LoginButton6.Visibility = Visibility.Hidden;
            LoginButton7.Visibility = Visibility.Hidden;
            LoginButton8.Visibility = Visibility.Hidden;
            LoginButton9.Visibility = Visibility.Hidden;
            LoginButton10.Visibility = Visibility.Hidden;
            LoginButton11.Visibility = Visibility.Hidden;
            LoginButton12.Visibility = Visibility.Hidden;
            LoginButton13.Visibility = Visibility.Hidden;
            LoginButton14.Visibility = Visibility.Hidden;
            LoginButton15.Visibility = Visibility.Hidden;

            LoginButton0.Background = null;
            LoginButton1.Background = null;
            LoginButton0.Background = null;
            LoginButton3.Background = null;
            LoginButton4.Background = null;
            LoginButton5.Background = null;
            LoginButton6.Background = null;
            LoginButton7.Background = null;
            LoginButton8.Background = null;
            LoginButton9.Background = null;
            LoginButton10.Background = null;
            LoginButton11.Background = null;
            LoginButton0.Background = null;
            LoginButton13.Background = null;
            LoginButton14.Background = null;
            LoginButton15.Background = null;

            for (int i = 16 * page; i < GlobalData.SiteSettings.UserLogins.Count && i < (16 * (page + 1)); i++)
            {
                UserLogin login = GlobalData.SiteSettings.UserLogins[i];

                int buttonNumber = i % 16;

                switch (buttonNumber)
                {
                    case 0:
                        LoginButton0.Text = login.Name;
                        LoginButton0.Visibility = Visibility.Visible;
                        break;

                    case 1:
                        LoginButton1.Text = login.Name;
                        LoginButton1.Visibility = Visibility.Visible;
                        break;

                    case 2:
                        LoginButton2.Text = login.Name;
                        LoginButton2.Visibility = Visibility.Visible;
                        break;

                    case 3:
                        LoginButton3.Text = login.Name;
                        LoginButton3.Visibility = Visibility.Visible;
                        break;

                    case 4:
                        LoginButton4.Text = login.Name;
                        LoginButton4.Visibility = Visibility.Visible;
                        break;

                    case 5:
                        LoginButton5.Text = login.Name;
                        LoginButton5.Visibility = Visibility.Visible;
                        break;

                    case 6:
                        LoginButton6.Text = login.Name;
                        LoginButton6.Visibility = Visibility.Visible;
                        break;

                    case 7:
                        LoginButton7.Text = login.Name;
                        LoginButton7.Visibility = Visibility.Visible;
                        break;

                    case 8:
                        LoginButton8.Text = login.Name;
                        LoginButton8.Visibility = Visibility.Visible;
                        break;

                    case 9:
                        LoginButton9.Text = login.Name;
                        LoginButton9.Visibility = Visibility.Visible;
                        break;

                    case 10:
                        LoginButton10.Text = login.Name;
                        LoginButton10.Visibility = Visibility.Visible;
                        break;

                    case 11:
                        LoginButton11.Text = login.Name;
                        LoginButton11.Visibility = Visibility.Visible;
                        break;

                    case 12:
                        LoginButton12.Text = login.Name;
                        LoginButton12.Visibility = Visibility.Visible;
                        break;

                    case 13:
                        LoginButton13.Text = login.Name;
                        LoginButton13.Visibility = Visibility.Visible;
                        break;

                    case 14:
                        LoginButton14.Text = login.Name;
                        LoginButton14.Visibility = Visibility.Visible;
                        break;

                    case 15:
                        LoginButton15.Text = login.Name;
                        LoginButton15.Visibility = Visibility.Visible;
                        break;

                }

            }
        }

        private void UsernamePressed(object sender, RoutedEventArgs e)
        {
            LoginButton0.Background = null;
            LoginButton1.Background = null;
            LoginButton2.Background = null;
            LoginButton3.Background = null;
            LoginButton4.Background = null;
            LoginButton5.Background = null;
            LoginButton6.Background = null;
            LoginButton7.Background = null;
            LoginButton8.Background = null;
            LoginButton9.Background = null;
            LoginButton10.Background = null;
            LoginButton11.Background = null;
            LoginButton12.Background = null;
            LoginButton13.Background = null;
            LoginButton15.Background = null;

            ClickableTile tile = (ClickableTile)sender;
            string name = tile.Text;

            foreach (UserLogin login in GlobalData.SiteSettings.UserLogins)
            {
                if (login.Name == name)
                {
                    GlobalData.UserLogin = login;
                    tile.Background = System.Windows.Media.Brushes.Yellow;

                    if (PinDisplay.Text == "Select User")
                    {
                        Pin = "";
                        PinDisplay.Text = "";
                    }
                    break;
                }
            }
        }

        private void PinButtonPressed(object sender, RoutedEventArgs e)
        {
            string text = ((ClickableTile)sender).Text;

            if (Pin.Length < 20)
            {
                switch (text)
                {
                    case "0":
                    case "1":
                    case "2":
                    case "3":
                    case "4":
                    case "5":
                    case "6":
                    case "7":
                    case "8":
                    case "9":
                        Pin += text;
                        break;

                }
            }
            if (Pin.Length > 0)
            {
                switch (text)
                {
                    case "Delete":
                        Pin = Pin.Substring(0, Pin.Length - 1);
                        break;

                    case "Clear":
                        Pin = "";
                        break;

                }
            }

            string displayedPin = "";

            for (int i = 0; i < Pin.Length; i++)
            {
                displayedPin += ".";
            }
            PinDisplay.Padding = new Thickness(0);
            PinDisplay.Text = displayedPin;
        }

        private void LoginButtonPressed(object sender, RoutedEventArgs e)
        {
            if (GlobalData.UserLogin != null)
            {
                if (GlobalData.UserLogin.Pin == Pin)
                {
                    GlobalEvents.SetLoggedInStatusEvent.Invoke(null, true);
                    GlobalEvents.ChangeScreenEvent.Invoke(null, new Data.EventData.ChangeScreenData() { Screen = Screen.TableSelect });
                    Pin = "";

                    LoginButton0.Background = null;
                    LoginButton1.Background = null;
                    LoginButton2.Background = null;
                    LoginButton3.Background = null;
                    LoginButton4.Background = null;
                    LoginButton5.Background = null;
                    LoginButton6.Background = null;
                    LoginButton7.Background = null;
                    LoginButton8.Background = null;
                    LoginButton9.Background = null;
                    LoginButton10.Background = null;
                    LoginButton11.Background = null;
                    LoginButton12.Background = null;
                    LoginButton13.Background = null;
                    LoginButton15.Background = null;

                    PinDisplay.Text = "";
                }
                else
                {
                    Pin = "";
                    PinDisplay.Padding = new Thickness(0,10,0,0);
                    PinDisplay.Text = "Invalid";
                }
            }
            else
            {
                Pin = "";
                PinDisplay.Padding = new Thickness(0, 10, 0, 0);
                PinDisplay.Text = "Select User";
            }
        }
    }
}
