using Sam4sTillSystem.Data;
using Sam4sTillSystem.Data.EventData;
using Sam4sTillSystem.Enum;
using Sam4sTillSystem.HardwareFunctions;
using Sam4sTillSystem.Helpers;
using Sam4sTillSystem.State;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Sam4sTillSystem.Screens.Funtions
{
    /// <summary>
    /// Interaction logic for FunctionsScreen.xaml
    /// </summary>
    public partial class FunctionsScreen : UserControl
    {
        public YesNoPopupModel YesNoPopupModel = new YesNoPopupModel();

        public FunctionsScreen()
        {
            InitializeComponent();

            FunctionButtonGrid.Visibility = Visibility.Hidden;

            GlobalEvents.ChangeScreenEvent += ChangeScreenEventHandler;
        }

        public void ChangeScreenEventHandler(object sender, ChangeScreenData e)
        {
            if (e.Screen == Screen.FunctionScreen)
            {
                GlobalEvents.ChangeScreenTitle.Invoke(null, "Till Functions");
                ApplyUserPermissions();                
                FunctionButtonGrid.Visibility = Visibility.Visible;
            }
            else
            {
                FunctionButtonGrid.Visibility = Visibility.Hidden;
            }
        }

        private void ApplyUserPermissions()
        {
            FunctionsAddManualEntryButton.IsEnabled = GlobalData.UserLogin.ManualEntryAllowed;
            FunctionsAddDiscountButton.IsEnabled = GlobalData.UserLogin.FixedDiscountAllowed;
            FunctionsRefundButton.IsEnabled = GlobalData.UserLogin.RefundAllowed;
            FunctionsNoSaleButton.IsEnabled = GlobalData.UserLogin.NoSaleAllowed;

            FunctionsNoSaleActivationsButton.IsEnabled = GlobalData.UserLogin.NoSaleActivationsAllowed;
            FunctionsShowTodaysStatsButton.IsEnabled = GlobalData.UserLogin.TodaysStatsAllowed;

            FunctionsHardwareSettingsButton.IsEnabled = GlobalData.UserLogin.HardwareSettingsAllowed;

            FunctionsCloseApplicationButton.IsEnabled = GlobalData.UserLogin.CloseAppAllowed;
            //FunctionsReloadConfigButton.IsEnabled = GlobalData.UserLogin.ReloadConfigAllowed;
            //FunctionsRestoreLastWorkingConfigButton.IsEnabled = GlobalData.UserLogin.RestoreLastWorkingConfigAllowed;
            TestModeButton.IsEnabled = GlobalData.UserLogin.EnableTestModeAllowed;

        }

        private void ReloadConfig(object sender, RoutedEventArgs e)
        {
            GlobalEvents.ReloadConfigEvent.Invoke(null, null);
        }

        private void ShowNoSaleActivations(object sender, RoutedEventArgs e)
        {
            GlobalEvents.ChangeScreenEvent(null, new ChangeScreenData() { Screen = Screen.NoSaleActivationsScreen });
        }

        private void CloseApplication(object sender, RoutedEventArgs e)
        {
            ShowYesNoPopup(
                "Close Application",
                "Are you sure you want to close the application?",
                CloseYesNoPopup,
                Application.Current.Shutdown
            );
        }

        private void ShowTodaysStats(object sender, RoutedEventArgs e)
        {
            GlobalEvents.ChangeScreenEvent(null, new ChangeScreenData() { Screen = Screen.DailyStatsScreen });
        }

        private void NoSalePressed(object sender, RoutedEventArgs e)
        {
            Sqlite.SqliteData.RecordNoSale();
            CashDrawer.Open();
        }


        private void EnableTestModePressed(object sender, RoutedEventArgs e)
        {
            GlobalData.TestModeEnabled = !GlobalData.TestModeEnabled;

            GlobalEvents.TestModeChangedEvent.Invoke(null, GlobalData.TestModeEnabled);

            if (GlobalData.TestModeEnabled)
            {
                TestModeButton.Text = "Disable Test Mode";
            }
            else
            {
                TestModeButton.Text = "Enable Test Mode";
            }
            TestModeButton.Render(null, null);
        }


        private void RestoreLastWorkingConfigPressed(object sender, RoutedEventArgs e)
        {
            ShowYesNoPopup(
                "Restore Last Valid Config",
                "This will override the previous config with the last valid" + Environment.NewLine + "config used, if the current config is valid it will" + Environment.NewLine + "reset it to the same config",
                CloseYesNoPopup,
                RestorePreviousConfig
            );
        }

        private void CloseYesNoPopup()
        {
            YesNoPopup.IsOpen = false;
        }




        private void YesNoPopup_NoPressed(object sender, RoutedEventArgs e)
        {
            YesNoPopupModel.NoPressedAction.Invoke();
            YesNoPopup.IsOpen = false;
        }

        private void YesNoPopup_YesPressed(object sender, RoutedEventArgs e)
        {
            YesNoPopupModel.YesPressedAction.Invoke();
            YesNoPopup.IsOpen = false;
        }

        private void RestorePreviousConfig()
        {

        }

        private void ShowYesNoPopup(string title, string text, Action noPressed, Action yesPressed)
        {
            YesNoPopupTitle.Text = title;
            YesNoPopupText.Text = text;
            YesNoPopupModel.NoPressedAction = noPressed;
            YesNoPopupModel.YesPressedAction = yesPressed;
            YesNoPopup.IsOpen = true;
        }

        private void RefundButtonPressed(object sender, RoutedEventArgs e)
        {
            GlobalEvents.ChangeScreenEvent(null, new Data.EventData.ChangeScreenData() { Screen = Screen.RefundScreen });
        }

        private void AddDiscountButtonPressed(object sender, RoutedEventArgs e)
        {
            GlobalEvents.ChangeScreenEvent(null, new Data.EventData.ChangeScreenData() { Screen = Screen.DiscountScreen });
        }

        private void AddManualEntryButtonPressed(object sender, RoutedEventArgs e)
        {
            GlobalEvents.ChangeScreenEvent(null, new Data.EventData.ChangeScreenData() { Screen = Screen.ManualEntryAmountScreen });
        }

        private void PrinterSettingsPressed(object sender, RoutedEventArgs e)
        {
            GlobalEvents.ChangeScreenEvent(null, new Data.EventData.ChangeScreenData() { Screen = Screen.PrinterSettingsScreen });
        }
    }
}
