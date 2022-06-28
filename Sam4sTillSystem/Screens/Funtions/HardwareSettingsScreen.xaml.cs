using Sam4sTillSystem.Data.EventData;
using Sam4sTillSystem.Data.JsonObject;
using Sam4sTillSystem.Enum;
using Sam4sTillSystem.FileFunctions;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Sam4sTillSystem.Screens.Funtions
{
    /// <summary>
    /// Interaction logic for FunctionsScreen.xaml
    /// </summary>
    public partial class HardwareSettingsScreen : UserControl
    {
        public HardwareSettingsScreen()
        {
            InitializeComponent();

            PrinterSettingsScreenGrid.Visibility = Visibility.Hidden;

            GlobalEvents.ChangeScreenEvent += ChangeScreenEventHandler;
        }

        public void ChangeScreenEventHandler(object sender, ChangeScreenData e)
        {
            if(e.Screen == Screen.PrinterSettingsScreen)
            {
                PrinterSettingsScreenGrid.Visibility = Visibility.Visible;
                LoadData();
            }
            else
            {
                PrinterSettingsScreenGrid.Visibility = Visibility.Hidden;
            }
        }

        public void LoadData()
        {
            Render();
        }

        private void AddNoneOptionToDropDown(ref ComboBox comboBox, string currentlySelectedTextValue)
        {
            ComboBoxItem item = new ComboBoxItem();
            item.VerticalContentAlignment = VerticalAlignment.Center;
            item.HorizontalContentAlignment = HorizontalAlignment.Center;
            item.Content = "None";
            item.Height = 50;
            item.IsSelected = ("None" == currentlySelectedTextValue);

            comboBox.Items.Add(item);
        }

        private void AddOptionToDropDown(ref ComboBox comboBox, string text, string currentlySelectedTextValue)
        {
            ComboBoxItem item = new ComboBoxItem();
            item.VerticalContentAlignment = VerticalAlignment.Center;
            item.HorizontalContentAlignment = HorizontalAlignment.Center;
            item.Content = text;
            item.Height = 50;
            item.IsSelected = (text == currentlySelectedTextValue);

            comboBox.Items.Add(item);
        }

        public void Render()
        {
            ReceiptPrinterList.Items.Clear();
            ReceiptPrinterBackupOneList.Items.Clear();
            ReceiptPrinterBackupTwoList.Items.Clear();

            ChefsPrinterMainList.Items.Clear();
            ChefsPrinterBackupOneList.Items.Clear();
            ChefsPrinterBackupTwoList.Items.Clear();


            AddNoneOptionToDropDown(ref ReceiptPrinterList, GlobalData.HardwareSettings.ReceiptPrinterName);
            AddNoneOptionToDropDown(ref ReceiptPrinterBackupOneList, GlobalData.HardwareSettings.BackupReceiptPrinterOneName);
            AddNoneOptionToDropDown(ref ReceiptPrinterBackupTwoList, GlobalData.HardwareSettings.BackupReceiptPrinterTwoName);

            AddNoneOptionToDropDown(ref ChefsPrinterMainList, GlobalData.HardwareSettings.ChefsPrinterMainName);
            AddNoneOptionToDropDown(ref ChefsPrinterBackupOneList, GlobalData.HardwareSettings.BackupChefsPrinterOneName);
            AddNoneOptionToDropDown(ref ChefsPrinterBackupTwoList, GlobalData.HardwareSettings.BackupChefsPrinterTwoName);


            string[] printersToIgnore = { "microsoft xps document writer", "microsoft print to pdf", "fax" };
            foreach (string printer in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
            {
                if (printersToIgnore.Contains(printer.ToLower()))
                {
                    continue;
                }

                AddOptionToDropDown(ref ReceiptPrinterList, printer, GlobalData.HardwareSettings.ReceiptPrinterName);
                AddOptionToDropDown(ref ReceiptPrinterBackupOneList, printer, GlobalData.HardwareSettings.BackupReceiptPrinterOneName);
                AddOptionToDropDown(ref ReceiptPrinterBackupTwoList, printer, GlobalData.HardwareSettings.BackupReceiptPrinterTwoName);

                AddOptionToDropDown(ref ChefsPrinterMainList, printer, GlobalData.HardwareSettings.ChefsPrinterMainName);
                AddOptionToDropDown(ref ChefsPrinterBackupOneList, printer, GlobalData.HardwareSettings.BackupChefsPrinterOneName);
                AddOptionToDropDown(ref ChefsPrinterBackupTwoList, printer, GlobalData.HardwareSettings.BackupChefsPrinterTwoName);

            }
        }

        private void SaveHardwareSettings_Click(object sender, RoutedEventArgs e)
        {

            HardwareSettingsChangesSavedReadout.Visibility = Visibility.Hidden;

            HardwareSettings newSettings = new HardwareSettings();
            newSettings.ReceiptPrinterName = (string)((ComboBoxItem)ReceiptPrinterList.SelectedItem).Content;
            newSettings.BackupReceiptPrinterOneName = (string)((ComboBoxItem)ReceiptPrinterBackupOneList.SelectedItem).Content;
            newSettings.BackupReceiptPrinterTwoName = (string)((ComboBoxItem)ReceiptPrinterBackupTwoList.SelectedItem).Content;

            newSettings.ChefsPrinterMainName = (string)((ComboBoxItem)ChefsPrinterMainList.SelectedItem).Content;
            newSettings.BackupChefsPrinterOneName = (string)((ComboBoxItem)ChefsPrinterBackupOneList.SelectedItem).Content;
            newSettings.BackupChefsPrinterTwoName = (string)((ComboBoxItem)ChefsPrinterBackupTwoList.SelectedItem).Content;


            GlobalData.HardwareSettings = newSettings;

            HardwareSettingsFile.SaveSettings();

            HardwareSettingsChangesSavedReadout.Visibility = Visibility.Visible;
        }

        private void UndoHardwareSettings_Click(object sender, RoutedEventArgs e)
        {
            if (HardwareSettingsChangesSavedReadout != null)
            {
                HardwareSettingsChangesSavedReadout.Visibility = Visibility.Hidden;
            }

            LoadData();
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (HardwareSettingsChangesSavedReadout != null)
            {
                HardwareSettingsChangesSavedReadout.Visibility = Visibility.Hidden;
            }
        }
    }
}
