using Sam4sTillSystem.Enum;
using Sam4sTillSystem.FileFunctions;
using System;
using System.Net.Http;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace Sam4sTillSystem.Screens.CloudFunctions
{
    /// <summary>
    /// Interaction logic for CloudUplinkScreen.xaml
    /// </summary>
    public partial class CloudLinkStatus : UserControl
    {
        public CloudLinkStatus()
        {
            InitializeComponent();

        }

        private void DisconnectLinkPressed(object sender, RoutedEventArgs e)
        {
            GlobalData.CloudSettings.AuthorisationKey = null;
            GlobalData.CloudSettings.Guid = null;

            CloudSettingsFile.SaveSettings();

            GlobalEvents.ChangeScreenEvent(null, new Data.EventData.ChangeScreenData() { Screen = Screen.CloudUplinkScreen });
        }
    }
}
