using Sam4sTillSystem.Data.EventData;
using Sam4sTillSystem.Enum;
using System.Windows;
using System.Windows.Controls;

namespace Sam4sTillSystem.Screens.CloudFunctions
{
    /// <summary>
    /// Interaction logic for FunctionsScreen.xaml
    /// </summary>
    public partial class CloudFunctionsScreen : UserControl
    {
        public CloudFunctionsScreen()
        {
            InitializeComponent();

            CloudFunctionButtonGrid.Visibility = Visibility.Hidden;

            GlobalEvents.ChangeScreenEvent += ChangeScreenEventHandler;
        }

        public void ChangeScreenEventHandler(object sender, ChangeScreenData e)
        {
            if (e.Screen == Screen.CloudFunctionsScreen)
            {
                GlobalEvents.ChangeScreenTitle.Invoke(null, "Cloud Functions");
                ApplyUserPermissions();
                CloudFunctionButtonGrid.Visibility = Visibility.Visible;
            }
            else
            {
                CloudFunctionButtonGrid.Visibility = Visibility.Hidden;
            }
        }

        private void ApplyUserPermissions()
        {
        }

        private void CloudLinkClicked(object sender, RoutedEventArgs e)
        {            
            if(GlobalData.CloudSettings.AuthorisationKey != null)
            {
                GlobalEvents.ChangeScreenEvent.Invoke(null, new ChangeScreenData() { Screen = Screen.CloudLinkStatusScreen });
            }
            else
            {
                GlobalEvents.ChangeScreenEvent.Invoke(null, new ChangeScreenData() { Screen = Screen.CloudUplinkScreen });
            }
        }
    }
}
