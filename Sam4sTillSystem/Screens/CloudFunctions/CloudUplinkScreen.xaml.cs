using Newtonsoft.Json;
using Sam4sTillSystem.Data.EventData;
using Sam4sTillSystem.Enum;
using Sam4sTillSystem.FileFunctions;
using System;
using System.Net.Http;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Sam4sTillSystem.Screens.CloudFunctions
{
    /// <summary>
    /// Interaction logic for CloudUplinkScreen.xaml
    /// </summary>
    public partial class CloudUplinkScreen : UserControl
    {
        private string Text = "";
        private bool AttemptingLink = false;

        public CloudUplinkScreen()
        {
            InitializeComponent();
        }

        private void Pressed0(object sender, RoutedEventArgs e)
        {
            ButtonPressed(0);
        }

        private void Pressed1(object sender, RoutedEventArgs e)
        {
            ButtonPressed(1);
        }

        private void Pressed2(object sender, RoutedEventArgs e)
        {
            ButtonPressed(2);
        }

        private void Pressed3(object sender, RoutedEventArgs e)
        {
            ButtonPressed(3);
        }

        private void Pressed4(object sender, RoutedEventArgs e)
        {
            ButtonPressed(4);
        }

        private void Pressed5(object sender, RoutedEventArgs e)
        {
            ButtonPressed(5);
        }

        private void Pressed6(object sender, RoutedEventArgs e)
        {
            ButtonPressed(6);
        }

        private void Pressed7(object sender, RoutedEventArgs e)
        {
            ButtonPressed(7);
        }

        private void Pressed8(object sender, RoutedEventArgs e)
        {
            ButtonPressed(8);
        }

        private void Pressed9(object sender, RoutedEventArgs e)
        {
            ButtonPressed(9);
        }

        private class LinkTillData
        {
            public string verificationKey { get; set; }
            public string guid { get; set; }
        }

        private void AttemptLink(object sender, RoutedEventArgs e)
        {
            try
            {
                bool successed = false;

                using (HttpClient client = new HttpClient())
                {
                    LinkTillData requestDto = new LinkTillData();
                    requestDto.guid = Guid.NewGuid().ToString();
                    requestDto.verificationKey = CloudUplinkPinCode.Text;

                    var content = new StringContent(JsonConvert.SerializeObject(requestDto), Encoding.UTF8, "application/json");

                }

                if (successed)
                {
                    // load to uplink details page
                    GlobalEvents.ChangeScreenEvent.Invoke(null, new ChangeScreenData() { Screen = Screen.CloudLinkStatusScreen });
                }
            }
            catch (Exception ex)
            {
                CloudUplinkPinCode.Text = "Error";
            }
        }

        private void Delete(object sender, RoutedEventArgs e)
        {
            if (Text.Length > 0)
            {
                Text = Text.Substring(0, Text.Length - 1);
                CloudUplinkPinCode.Text = Text;
            }
        }

        private void Clear(object sender, RoutedEventArgs e)
        {
            Text = "";
            CloudUplinkPinCode.Text = Text;
        }

        private void ButtonPressed(int number)
        {
            if (Text == "Error" || CloudUplinkPinCode.Text.Length > 6)
            {
                Text = "";
            }

            if (Text.Length < 6)
            {
                Text += number.ToString();
                CloudUplinkPinCode.Text = Text;
            }
        }
    }
}
