using Newtonsoft.Json;
using Sam4sTillSystem.Enum;
using Sam4sTillSystem.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Sam4sTillSystem.Data;
using Sam4sTillSystem.Helpers;
using Sam4sTillSystem.Controls;
using Sam4sTillSystem.Screens.Funtions;
using System.Reflection;
using Sam4sTillSystem.Screens.CloudFunctions;
using Sam4sTillSystem.FileFunctions;
using Sam4sTillSystem.Data.EventData;
using Sam4sTillSystem.Data.PopupData;

namespace Sam4sTillSystem
{
    public partial class MainWindow : Window
    {
        private static OrderInfo ReceiptData = null;

        public PageState State = new PageState();

        public PageState ChefsNoteState = new PageState();

        private Guid quantitySetterGridGuid;

        public void ShowQuantitySelectGridEventHandler(object sender, SetQuantityEventData data)
        {
            quantitySetterGridGuid = data.ControlIdenfier;
            QuantitySelectorGrid.Visibility = Visibility.Visible;

            for (int i = 0; i <= 20; i++)
            {
                ClickableTile tile = (ClickableTile)FindName("QuanitySetterGridControl" + i);
                tile.Visibility = Visibility.Hidden;
            }

            int range = (data.MaxQuantity - data.MinQuantity) + 1;

            for (int i = 0; i < range && i <= 20; i++)
            {
                ClickableTile tile = (ClickableTile)FindName("QuanitySetterGridControl" + i);
                tile.Visibility = Visibility.Visible;
                tile.Text = (data.MinQuantity + i).ToString();
            }
        }

        public void HandleUpdateOrderTotal(object sender, EventArgs e)
        {
            int totalInPence = GlobalData.OrderInfo.GetTotalCostInPence();

            CurrentOrderTotalValueTextBlock.Text = UIHelper.FormatPrice(totalInPence);

            CurrentOrderPayByCashButton.IsEnabled = GlobalData.OrderInfo.OrderItems.Count > 0;
            CurrentOrderPayByCardButton.IsEnabled = GlobalData.OrderInfo.OrderItems.Count > 0;
        }

        public void HandleAddOrUpdateOrderItem(object sender, OrderItemInfo orderItemInfo)
        {
            if (!GlobalData.OrderInfo.OrderItems.Where(x => x.Identifier == orderItemInfo.Identifier).Any())
            {
                OrderDisplayRow orderDisplayRow = new OrderDisplayRow();
                orderDisplayRow.Identifier = orderItemInfo.Identifier;
                orderDisplayRow.Update(orderItemInfo);                

                CurrentOrderDisplay.Children.Add(orderDisplayRow);

                GlobalData.OrderInfo.OrderItems.Add(orderItemInfo);
            }

            if (orderItemInfo.Quantity == 0)
            {

                foreach (OrderDisplayRow element in CurrentOrderDisplay.Children.Cast<OrderDisplayRow>().ToList())
                {
                    if (element.Identifier == orderItemInfo.Identifier)
                    {
                        CurrentOrderDisplay.Children.Remove(element);
                        break;
                    }
                }
            }

            GlobalEvents.ChangeOrderDiscount.Invoke(null, GlobalData.OrderInfo.PercentageBasedOrderDiscount);
        }

        public void ShowCostBreakdownGridEventHandler(object sender, List<CostBreakdownData> data)
        {
            data = data.OrderBy(x => x.Name).ToList();

            for (int i = 0; i < 16; i++)
            {
                TextBlock name = (TextBlock)FindName("CostBreakdownName" + i);
                TextBlock add = (TextBlock)FindName("CostBreakdownAddAmount" + i);
                TextBlock remove = (TextBlock)FindName("CostBreakdownRemoveAmount" + i);
                TextBlock diff = (TextBlock)FindName("CostBreakdownDiff" + i);
                TextBlock cost = (TextBlock)FindName("CostBreakdownCost" + i);


                name.Visibility = Visibility.Hidden;
                add.Visibility = Visibility.Hidden;
                remove.Visibility = Visibility.Hidden;
                diff.Visibility = Visibility.Hidden;
                cost.Visibility = Visibility.Hidden;
            }

            int total = 0;

            for (int i = 0; i < data.Count; i ++)
            {
                TextBlock name = (TextBlock)FindName("CostBreakdownName" + i);
                TextBlock add = (TextBlock)FindName("CostBreakdownAddAmount" + i);
                TextBlock remove = (TextBlock)FindName("CostBreakdownRemoveAmount" + i);
                TextBlock diff = (TextBlock)FindName("CostBreakdownDiff" + i);
                TextBlock cost = (TextBlock)FindName("CostBreakdownCost" + i);


                name.Text = data[i].Name;

                add.Text = UIHelper.FormatPrice(data[i].CostPerExtraItemInPence);

                remove.Text = UIHelper.FormatPrice(data[i].DiscountPerRemovedItemInPence);


                int dif = data[i].Difference;
                diff.Text = dif.ToString();

                cost.Text = UIHelper.FormatPrice(data[i].CurrentCostInPence);

                name.Visibility = Visibility.Visible;
                add.Visibility = Visibility.Visible;
                remove.Visibility = Visibility.Visible;
                diff.Visibility = Visibility.Visible;
                cost.Visibility = Visibility.Visible;

                total += data[i].CurrentCostInPence;
            }

            CostBreakdownTotal.Text = UIHelper.FormatPrice(total);

            CostBreakdownGrid.Visibility = Visibility.Visible;
        }

        public void ChangeScreenEventHandler(object sender, ChangeScreenData e)
        {
            if (e.ScreenName != null)
            {
                GlobalEvents.ChangeScreenTitle.Invoke(null, e.ScreenName);

                // hide popups
                QuantitySelectorGrid.Visibility = Visibility.Hidden;
                CostBreakdownGrid.Visibility = Visibility.Hidden;
            }
            else if (e.Screen == Screen.BlankScreen)
            {
                GlobalEvents.ChangeScreenTitle.Invoke(null, "");
            }

            State.PreviousScreen = State.CurrentScreen;

            CancelOrderGrid.Visibility = Visibility.Hidden;
            ChangeDueScreen.Visibility = Visibility.Hidden;
            CloudFunctionsScreen.Visibility = Visibility.Hidden;
            DailyItemBreakdownGrid.Visibility = Visibility.Hidden;
            OrderSummaryScreen.Visibility = Visibility.Hidden;
            PaymentScreen.Visibility = Visibility.Hidden;
            DailyItemSectionBreakdownControl.Visibility = Visibility.Hidden;
            CloudUplinkScreen.Visibility = Visibility.Hidden;
            CloudLinkStatusScreen.Visibility = Visibility.Hidden;

            switch (e.Screen) 
            {
                case Screen.CancelOrderScreen:
                    ScreenTitle.Text = "Cancel Order";
                    CancelOrderGrid.Visibility = Visibility.Visible;
                    break;

                case Screen.ChangeDueScreen:
                    ScreenTitle.Text = "Change Due";
                    if (State.PaymentMethod == PaymentMethod.Card)
                    {
                        ChangeDueChangeDueTextBox.Text = "Paid By Card";
                    }
                    ChangeDueScreen.Visibility = Visibility.Visible;
                    break;

                case Screen.CloudFunctionsScreen:
                    ScreenTitle.Text = "Cloud Functions";
                    CloudFunctionsScreen.Visibility = Visibility.Visible;
                    break;

                case Screen.CloudUplinkScreen:
                    ScreenTitle.Text = "Cloud Link";
                    CloudUplinkScreen.Visibility = Visibility.Visible;
                    break;

                case Screen.CloudLinkStatusScreen:
                    ScreenTitle.Text = "Cloud Link Status";
                    CloudLinkStatusScreen.Visibility = Visibility.Visible;
                    break;

                case Screen.DailyItemSectionBreakdownScreen:
                    DailyItemSectionBreakdownControl.Visibility = Visibility.Visible;
                    break;

                case Screen.DiscountScreen:
                    ScreenTitle.Text = "Fixed Discount";

                    OrderSummaryButton.IsEnabled = false;
                    AddManualEntryButton.IsEnabled = false;
                    AddDiscountButton.IsEnabled = false;
                    AddRefundButton.IsEnabled = false;

                    // reset text when loading page to remove previous order amount
                    CommonHelper.ClearCashAmount(this, State);

                    AddManualEntryButton.Visibility = Visibility.Hidden;
                    AddDiscountButton.Visibility = Visibility.Visible;
                    OrderSummaryButton.Visibility = Visibility.Hidden;
                    AddRefundButton.Visibility = Visibility.Hidden;

                    PaymentScreen.Visibility = Visibility.Visible;
                    break;

                case Screen.ManualEntryAmountScreen:
                    ScreenTitle.Text = "Manual Entry";

                    OrderSummaryButton.IsEnabled = false;
                    AddManualEntryButton.IsEnabled = false;
                    AddDiscountButton.IsEnabled = false;
                    AddRefundButton.IsEnabled = false;

                    // reset text when loading page to remove previous order amount
                    CommonHelper.ClearCashAmount(this, State);

                    AddManualEntryButton.Visibility = Visibility.Visible;
                    AddDiscountButton.Visibility = Visibility.Hidden;
                    OrderSummaryButton.Visibility = Visibility.Hidden;
                    AddRefundButton.Visibility = Visibility.Hidden;

                    PaymentScreen.Visibility = Visibility.Visible;
                    break;

                case Screen.OrderSummaryScreen:
                    ScreenTitle.Text = "Order Summary";
                    OrderSummaryScreen.Visibility = Visibility.Visible;
                    Sam4sTillSystem.HardwareFunctions.CashDrawer.Open();
                    break;

                case Screen.PaymentScreen:
                    ScreenTitle.Text = "Payment";

                    OrderSummaryButton.IsEnabled = false;
                    AddManualEntryButton.IsEnabled = false;
                    AddDiscountButton.IsEnabled = false;
                    AddRefundButton.IsEnabled = false;

                    // reset text when loading page to remove previous order amount
                    CommonHelper.ClearCashAmount(this, State);

                    AddManualEntryButton.Visibility = Visibility.Hidden;
                    AddDiscountButton.Visibility = Visibility.Hidden;
                    AddRefundButton.Visibility = Visibility.Hidden;
                    OrderSummaryButton.Visibility = Visibility.Visible;

                    PaymentScreen.Visibility = Visibility.Visible;
                    break;

                case Screen.PrinterSettingsScreen:
                    ScreenTitle.Text = "Hardware Settings";
                    break;

                case Screen.RefundScreen:
                    ScreenTitle.Text = "Refund";

                    OrderSummaryButton.IsEnabled = false;
                    AddManualEntryButton.IsEnabled = false;
                    AddDiscountButton.IsEnabled = false;
                    AddRefundButton.IsEnabled = false;

                    // reset text when loading page to remove previous order amount
                    CommonHelper.ClearCashAmount(this, State);

                    AddManualEntryButton.Visibility = Visibility.Hidden;
                    AddDiscountButton.Visibility = Visibility.Hidden;
                    OrderSummaryButton.Visibility = Visibility.Hidden;
                    AddRefundButton.Visibility = Visibility.Visible;

                    PaymentScreen.Visibility = Visibility.Visible;
                    break;

                case Screen.SavedOrdersAddNewScreen:
                    ScreenTitle.Text = "Add New Regular Order";
                    break;

                case Screen.PayByCardScreen:
                    State.PaymentMethod = PaymentMethod.Card;

                    GlobalEvents.ChangeScreenTitle.Invoke(null, "Paying By Card");

                    ScreenTitle.Text = "Fixed Discount";

                    OrderSummaryButton.IsEnabled = false;
                    AddManualEntryButton.IsEnabled = false;
                    AddDiscountButton.IsEnabled = false;
                    AddRefundButton.IsEnabled = false;

                    // reset text when loading page to remove previous order amount
                    CommonHelper.ClearCashAmount(this, this.State);

                    AddManualEntryButton.Visibility = Visibility.Hidden;
                    AddDiscountButton.Visibility = Visibility.Visible;
                    OrderSummaryButton.Visibility = Visibility.Hidden;
                    AddRefundButton.Visibility = Visibility.Hidden;

                    PaymentScreen.Visibility = Visibility.Visible;

                    break;
            }

            if (e.ScreenName != null)
            {
                ScreenTitle.Text = e.ScreenName;
            }

            State.CurrentScreen = e.Screen;


            ScreenLoader.LoadScreenData(this, e.Screen);
        }

        public void ChangeScreenTitleEventHandler(object sender, string e)
        {
            ScreenTitle.Text = e;
        }

        public void ChangeTableNumberEventHandler(object sender, int tableNumber)
        {
            bool tableNumberIsNotZero = (tableNumber != 0);

            for (int i = 0; i < 12; i++)
            {
                ClickableTile button = (ClickableTile)FindName("NavMenu" + (i + 1));

                button.IsEnabled = tableNumberIsNotZero;
                button.Render();
            }


            State.SelectedTableNumber = tableNumber;
            if (tableNumber == 99999)
            {
                CurrentOrderTableNumberValueTextBlock.Text = "Takeaway";
            }
            else if (tableNumber == 88888)
            {
                CurrentOrderTableNumberValueTextBlock.Text = "Just Eat";
            }
            else if (tableNumber == 0)
            {
                CurrentOrderTableNumberValueTextBlock.Text = "None";
            }
            else
            {
                CurrentOrderTableNumberValueTextBlock.Text = tableNumber.ToString();
            }

            GlobalData.OrderInfo.DestinationNumber = tableNumber;
        }

        public void TestModeChangedEventHandler(object sender, bool e)
        {
            if (e)
            {
                MainWindowGrid.Background = System.Windows.Media.Brushes.Yellow;
            }
            else
            {
                MainWindowGrid.Background = System.Windows.Media.Brushes.White;
            }
        }

        public void SaveNewOrderEventHandler(object sender, string e)
        {
            try
            {
                string json = JsonConvert.SerializeObject(GlobalData.OrderInfo);
                File.WriteAllText(@"./SavedOrders/" + GlobalData.OrderInfo.MenuIdentifier + "/" + e + ".json", json);
            }
            catch
            {

            }
        }

        public void SetLoggedInStatusEventHandler(object sender, bool e)
        {
            if (e)
            {
                OrderTakerText.Text = GlobalData.UserLogin.Name;
                State.UserLogin = GlobalData.UserLogin;

                LoginScreenControl.Visibility = Visibility.Hidden;

                LayoutNavigationGrid.Visibility = Visibility.Visible;
                ScreenTitle.Visibility = Visibility.Visible;
                ScreenTitleUpperLine.Visibility = Visibility.Visible;
                ScreenTitleLowerLine.Visibility = Visibility.Visible;
                LayoutScreenGrid.Visibility = Visibility.Visible;
                LayoutCurrentOrderGrid.Visibility = Visibility.Visible;
            }
            else
            {
                LoginScreenControl.Visibility = Visibility.Visible;

                LayoutNavigationGrid.Visibility = Visibility.Hidden;
                ScreenTitle.Visibility = Visibility.Hidden;
                ScreenTitleUpperLine.Visibility = Visibility.Hidden;
                ScreenTitleLowerLine.Visibility = Visibility.Hidden;
                LayoutScreenGrid.Visibility = Visibility.Hidden;
                LayoutCurrentOrderGrid.Visibility = Visibility.Hidden;
            }
        }

        public void ReloadConfigEventHandler(object sender, EventArgs e)
        {
            LoadAndSetupConfig();

            GlobalEvents.ChangeScreenEvent.Invoke(null, new ChangeScreenData() { Screen = Screen.TableSelect });
        }

        public void ChangeOrderDiscountEventHandler(object sender, int discount)
        {
            GlobalData.OrderInfo.PercentageBasedOrderDiscount = discount;
            DataHelper.UpdateOrderDiscount(this, State);
            HandleUpdateOrderTotal(null,null);
        }

        public MainWindow()
        {
            InitializeComponent();


            DisableWPFTabletSupport();

            // Setup event handlers
            GlobalEvents.ChangeScreenEvent += ChangeScreenEventHandler;
            GlobalEvents.TestModeChangedEvent += TestModeChangedEventHandler;
            GlobalEvents.SaveNewOrderSelectedEvent += SaveNewOrderEventHandler;
            GlobalEvents.SetLoggedInStatusEvent += SetLoggedInStatusEventHandler;
            GlobalEvents.ChangeScreenTitle += ChangeScreenTitleEventHandler;
            GlobalEvents.ChangeTableNumber += ChangeTableNumberEventHandler;
            GlobalEvents.ChangeOrderDiscount += ChangeOrderDiscountEventHandler;
            GlobalEvents.ReloadConfigEvent += ReloadConfigEventHandler;
            GlobalEvents.ShowQuantitySelectGridEvent += ShowQuantitySelectGridEventHandler;
            GlobalEvents.ShowCostBreakdownGridEvent += ShowCostBreakdownGridEventHandler;
            GlobalEvents.AddOrUpdateOrderItem += HandleAddOrUpdateOrderItem;
            GlobalEvents.UpdateOrderTotal += HandleUpdateOrderTotal;
            GlobalEvents.SavedOrderSelectedEvent += HandleSavedOrderSelectedEvent;

            // load file data
            HardwareSettingsFile.LoadSettings();
            CloudSettingsFile.LoadSettings();
            UserLoginsFile.LoadSettings();
            OpenningHoursFile.LoadSettings();
            MenuConfigFile.LoadSettings();
            DeveloperSettingsFile.LoadSettings();



            if (!GlobalData.SystemSettings.DeveloperSettings.DebugMode)
            {
                Mouse.OverrideCursor = Cursors.None;

                WindowState = WindowState.Maximized;
                WindowStyle = WindowStyle.None;

                this.Topmost = true;
            }

            Sqlite.SqliteData.CreateDatabaseIfNotExists();

            GlobalData.OrderInfo = new OrderInfo();
            GlobalData.OrderInfo.MenuIdentifier = Sqlite.SqliteData.GetIdentiferForMenuConfig();
            GlobalData.OrderInfo.VatNumber = GlobalData.MenuSettings.VatNumber;

            HandleUpdateOrderTotal(null ,null);

            LoadAndSetupConfig();


            GlobalEvents.ChangeTableNumber.Invoke(null, 0);

            // Encase multiple screen have been set to visible by default at the same time
            GlobalEvents.ChangeScreenEvent(null, new ChangeScreenData() { Screen = Screen.TableSelect });
        }

        public static void DisableWPFTabletSupport()
        {
            // Get a collection of the tablet devices for this  
            TabletDeviceCollection devices = System.Windows.Input.Tablet.TabletDevices;

            if (devices.Count > 0)
            {
                // Get the Type of InputManager.
                Type inputManagerType = typeof(System.Windows.Input.InputManager);
                
                // Call the StylusLogic method on the InputManager.Current instance.
                object stylusLogic = inputManagerType.InvokeMember("StylusLogic",BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.NonPublic,null, InputManager.Current, null);

                if (stylusLogic != null)
                {
                    //  Get the type of the stylusLogic returned from the call to StylusLogic.
                    Type stylusLogicType = stylusLogic.GetType();

                    // Loop until there are no more devices to remove.
                    while (devices.Count > 0)
                    {
                        // Remove the first tablet device in the devices collection.
                        stylusLogicType.InvokeMember("OnTabletRemoved",BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.NonPublic, null, stylusLogic, new object[] { (uint)0 });
                    }

                }

            }

        }

        private void HandleSavedOrderSelectedEvent(object sender, OrderInfo e)
        {
            foreach (OrderItemInfo orderItemInfo in GlobalData.OrderInfo.OrderItems)
            {
                orderItemInfo.Quantity = 0;
                GlobalEvents.AddOrUpdateOrderItem.Invoke(null, orderItemInfo);
            }

            GlobalData.OrderInfo.OrderItems.Clear();

            foreach (OrderItemInfo orderItemInfo in e.OrderItems)
            {
                GlobalEvents.AddOrUpdateOrderItem.Invoke(null, orderItemInfo);
            }

            int x = GlobalData.OrderInfo.DestinationNumber;

            GlobalEvents.ChangeOrderDiscount.Invoke(null, e.PercentageBasedOrderDiscount);
        }

        private void QuanitySetterGridAmountSelected(object sender, RoutedEventArgs e)
        {
            int amount = int.Parse(((ClickableTile)sender).Text);
            GlobalEvents.QuantitySelectGridAmountSelectedEvent.Invoke(null, new GirdAmountSelectedData(){ amount = amount, guid = quantitySetterGridGuid });
            QuantitySelectorGrid.Visibility = Visibility.Hidden;
        }

        #region Configuration

        public void LoadAndSetupConfig()
        {
            HandleUpdateOrderTotal(null, null);

            SetupLayout();
            SetupNavMenu();
            SetupCurrentOrder();
            SetupOrderSummaryScreen();
            CreateBackupOfConfig();


            GlobalData.OrderInfo.PercentageBasedOrderDiscount = 0;
            DataHelper.UpdateOrderDiscount(this, State);
            HandleUpdateOrderTotal(null, null);
        }

        private void SetupNavMenu()
        {
            int navMenuCounter = 1;

            foreach (var category in GlobalData.MenuSettings.Categories)
            {
                ClickableTile button = (ClickableTile)FindName("NavMenu" + navMenuCounter);
                button.Text = category.Text;

                Grid.SetRow(button, category.RowNum);
                Grid.SetRowSpan(button, category.RowSize);
                Grid.SetColumn(button, category.ColNum);
                Grid.SetColumnSpan(button, category.ColSize);

                navMenuCounter++;
            }

            while(navMenuCounter <= 12)
            {
                ClickableTile button = (ClickableTile)FindName("NavMenu" + navMenuCounter);
                button.Visibility = Visibility.Hidden;
                navMenuCounter++;
            }

            int numColumns = GlobalData.MenuSettings.Categories.Select(x => x.ColNum + x.ColSize).Max() + 1;

            if (numColumns < 7)
            {
                Grid.SetColumn(NavMenuFunctionsButton, numColumns);
            }

            // Remove unnecessary columns
            while (LayoutNavigationGrid.ColumnDefinitions.Count > numColumns)
            {
                LayoutNavigationGrid.ColumnDefinitions.RemoveAt(LayoutNavigationGrid.ColumnDefinitions.Count - 1);
            }
        }

        private void SetupOrderSummaryScreen()
        {
            // set row definition height
            OrderSummaryPrintReceiptRowDefinition.Height = new GridLength(160);
            OrderSummaryProcessOrderRowDefinition.Height = new GridLength(160);

            // set text font size
            OrderSummaryPrintReceiptButton.FontSize = 18;
            OrderSummaryProcessOrderButton.FontSize = 18;

            OrderSummaryChangeDueTextBox.FontSize = 40;
        }

        private void SetupLayout()
        {
            Grid.SetColumn(LayoutScreenGrid, 0);
            Grid.SetColumn(LayoutNavigationGrid, 0);
            Grid.SetColumn(LayoutCurrentOrderGrid, 1);

            LayoutLeftSectionColumnDefinition.Width = new GridLength(2, GridUnitType.Star);
            LayoutRightSectionColumnDefinition.Width = new GridLength(1, GridUnitType.Star);

            LayoutNavMenuHeight.Height = new GridLength(120);


        }

        private void SetupCurrentOrder()
        {
            // set if bold or not
            CurrentOrderTableNumberTextBlock.FontWeight = FontWeights.Bold;
            CurrentOrderTableNumberValueTextBlock.FontWeight = FontWeights.Bold;

            CurrentOrderTotalTextBlock.FontWeight = FontWeights.Bold;
            CurrentOrderTotalValueTextBlock.FontWeight = FontWeights.Bold;

            CurrentOrderCancelButton.FontWeight = FontWeights.Bold;
            CurrentOrderPayByCardButton.FontWeight = FontWeights.Bold;
            CurrentOrderPayByCashButton.FontWeight = FontWeights.Bold;
        }


        private void CreateBackupOfConfig()
        {

        }

        #endregion

        #region Events

        #region OrderSection

        private void CloseCostBreakdownPopup(object sender, RoutedEventArgs e)
        {
            CostBreakdownGrid.Visibility = Visibility.Hidden;
        }

        private void CancelOrderPressed(object sender, RoutedEventArgs e)
        {
            GlobalEvents.ChangeScreenEvent.Invoke(null, new ChangeScreenData() { Screen = Screen.CancelOrderScreen });
        }

        private void CancelOrder(object sender = null, RoutedEventArgs e = null)
        {
            ClearOrder();

            GlobalEvents.ChangeScreenEvent(null, new ChangeScreenData() { Screen = Screen.TableSelect });
            GlobalEvents.ChangeTableNumber.Invoke(null, 0);
        }

        private void ClearOrder()
        {
            ReceiptData = GlobalData.OrderInfo;
            GlobalData.OrderInfo = new OrderInfo();
            GlobalData.OrderInfo.MenuIdentifier = Sqlite.SqliteData.GetIdentiferForMenuConfig();
            GlobalData.OrderInfo.VatNumber = GlobalData.MenuSettings.VatNumber;
            foreach (OrderDisplayRow element in CurrentOrderDisplay.Children.Cast<OrderDisplayRow>().ToList())
            {
                CurrentOrderDisplay.Children.Remove(element);
            }

            HandleUpdateOrderTotal(null, null);
            GlobalEvents.ChangeTableNumber.Invoke(null, 0);
        }

        private DateTime LastPayByClick = DateTime.Now;

        private void PayWithCard(object sender, RoutedEventArgs e)
        {
            if (LastPayByClick.AddSeconds(1) < DateTime.Now)
            {
                GlobalData.OrderInfo.paymentMethod = PaymentMethod.Card;
                GlobalData.OrderInfo.Vat = GlobalData.MenuSettings.VatInPercent; 
                GlobalData.OrderInfo.MenuIdentifier = Sqlite.SqliteData.GetIdentiferForMenuConfig();
                GlobalData.OrderInfo.VatNumber = GlobalData.MenuSettings.VatNumber;
                ReceiptPrinter.ChefsNoteQueue.QueueDocument(GlobalData.OrderInfo);
                Sqlite.SqliteData.SaveOrder(GlobalData.OrderInfo, GlobalData.OrderInfo.Vat, false);

                OrderSummaryChangeDueTextBox.Text = "Paying By Card";
                GlobalEvents.ChangeScreenTitle.Invoke(null, "Order Summary");
                GlobalEvents.ChangeScreenEvent.Invoke(null, new ChangeScreenData() { Screen = Screen.OrderSummaryScreen });
                LastPayByClick = DateTime.Now;

                ClearOrder();
            }
        }

        private void PayWithCash(object sender, RoutedEventArgs e)
        {
            if (LastPayByClick.AddSeconds(1) < DateTime.Now)
            {
                GlobalEvents.ChangeScreenTitle.Invoke(null, "Enter Cash Amount");
                GlobalEvents.ChangeScreenEvent.Invoke(null, new ChangeScreenData() { Screen = Screen.PaymentScreen });
                LastPayByClick = DateTime.Now;
            }
        }

        private void ShowTableSelectScreen(object sender, MouseEventArgs e)
        {
            GlobalEvents.ChangeScreenEvent.Invoke(null, new ChangeScreenData() { Screen = Screen.TableSelect });
        }

        #endregion

        #region NavMenu

        private void FunctionButtonPressed(object sender, RoutedEventArgs e)
        {            
            GlobalEvents.ChangeScreenEvent.Invoke(null, new ChangeScreenData() { Screen = Screen.FunctionScreen });
        }


        private void CloudFunctionsButtonPressed(object sender, RoutedEventArgs e)
        {
            GlobalEvents.ChangeScreenEvent.Invoke(null, new ChangeScreenData() { Screen = Screen.CloudFunctionsScreen });
        }

        private void NavMenuPressed(object sender, RoutedEventArgs e)
        {
            ClickableTile button = sender as ClickableTile;

            int categoryPosition = int.Parse(button.Name.Replace("NavMenu", "")) - 1;
            var category = GlobalData.MenuSettings.Categories[categoryPosition];
            var productButtons = GlobalData.MenuSettings.ProductButtons.Where(x => x.MenuCategoryId == category.MenuCategoryId).ToList();

            GlobalEvents.ChangeScreenEvent.Invoke
            (
                null,
                new ChangeScreenData()
                {
                    Screen = Screen.ItemSelect,
                    ScreenName = category.Text,
                    Data = productButtons
                }
            );
        }

        #endregion

        private void AddItemPaymentNumberPressed(object sender, RoutedEventArgs e)
        {
            string value = ((Button)sender).Content.ToString();

            switch (value)
            {
                case "£20":
                case "£10":
                case "£5":
                    if (State.PaymentAmountText.Contains(".") && State.PaymentAmountText.Split('.')[1].Length == 2)
                    {
                        // add to amount
                        string[] parts = State.PaymentAmountText.Split('.');

                        int amountOfPounds = int.Parse(parts[0]);
                        amountOfPounds += int.Parse(value.Replace("£", string.Empty));
                        State.PaymentAmountText = amountOfPounds + "." + parts[1];
                    }
                    else
                    {
                        State.PaymentAmountText = value.Substring(1) + ".00";
                    }
                    break;

                case "Clear":
                    State.PaymentAmountText = "";
                    break;

                case "Del":
                    if (State.PaymentAmountText.Length > 0)
                    {
                        State.PaymentAmountText = State.PaymentAmountText.Substring(0, State.PaymentAmountText.Length - 1);
                    }
                    break;

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
                    if (State.PaymentAmountText != "0")
                    {
                        if (State.PaymentAmountText.Contains("."))
                        {
                            if (State.PaymentAmountText.Split('.')[1].Length < 2)
                            {
                                State.PaymentAmountText += value;
                            }
                        }
                        else
                        {
                            State.PaymentAmountText += value;
                        }
                    }
                    break;

                case ".":
                    if (State.PaymentAmountText.Contains(".") == false && State.PaymentAmountText.Length > 0)
                    {
                        State.PaymentAmountText += ".";
                    }
                    break;
            }

            if (State.PaymentAmountText == "")
            {
                PaymentInput.Text = "";
            }
            else
            {
                PaymentInput.Text = "£" + State.PaymentAmountText;
            }




            if (State.PaymentAmountText.Contains(".") && State.PaymentAmountText.Split('.')[1].Length == 2)
            {
                AddManualEntryButton.Content = "Add Manual Entry";
                AddManualEntryButton.IsEnabled = true;

                AddDiscountButton.Content = "Add Discount";
                AddDiscountButton.IsEnabled = true;

                AddRefundButton.Content = "Add Refund";
                AddRefundButton.IsEnabled = true;

                int changeDue = int.Parse(State.PaymentAmountText.Replace("£", string.Empty).Replace(".", string.Empty)) - GlobalData.OrderInfo.GetTotalCostInPence();

                if (changeDue >= 0)
                {
                    OrderSummaryButton.Content = "Process Order";
                    OrderSummaryButton.IsEnabled = true;
                    State.ChangeDue = changeDue;
                }
                else
                {
                    OrderSummaryButton.Content = "Amount entered not enough";
                    OrderSummaryButton.IsEnabled = false;
                    State.ChangeDue = 0;
                }
            }
            else
            {
                OrderSummaryButton.Content = "Enter cash amount";
                OrderSummaryButton.IsEnabled = false;

                AddManualEntryButton.Content = "Enter cash amount";
                AddManualEntryButton.IsEnabled = false;

                AddDiscountButton.Content = "Enter cash amount";
                AddDiscountButton.IsEnabled = false;

                State.ChangeDue = 0;
            }
        }

        private void PaymentNumberPressed(object sender, RoutedEventArgs e)
        {
            if (State.CurrentScreen != Screen.PaymentScreen)
            {
                AddItemPaymentNumberPressed(sender, e);
                return;
            }

            bool instantProcessIfEnough = false;

            string value = ((Button)sender).Content.ToString();

            switch (value)
            {
                case "£20":
                case "£10":
                case "£5":
                    State.PaymentAmountText = value.Substring(1) + ".00";
                    instantProcessIfEnough = true;
                    break;

                case "Clear":
                    State.PaymentAmountText = "";
                    break;

                case "Del":
                    if (State.PaymentAmountText.Length > 0)
                    {
                        State.PaymentAmountText = State.PaymentAmountText.Substring(0, State.PaymentAmountText.Length - 1);
                    }
                    break;

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
                    if (State.PaymentAmountText != "0")
                    {
                        if (State.PaymentAmountText.Contains("."))
                        {
                            if (State.PaymentAmountText.Split('.')[1].Length < 2)
                            {
                                State.PaymentAmountText += value;
                            }
                        }
                        else
                        {
                            State.PaymentAmountText += value;
                        }
                    }
                    break;

                case ".":
                    if (State.PaymentAmountText.Contains(".") == false && State.PaymentAmountText.Length > 0)
                    {
                        State.PaymentAmountText += ".";
                    }
                    break;
            }

            if (State.PaymentAmountText == "")
            {
                PaymentInput.Text = "";
            }
            else
            {
                PaymentInput.Text = "£" + State.PaymentAmountText;
            }




            if (State.PaymentAmountText.Contains(".") && State.PaymentAmountText.Split('.')[1].Length == 2)
            {
                AddManualEntryButton.Content = "Add Manual Entry";
                AddManualEntryButton.IsEnabled = true;

                AddDiscountButton.Content = "Add Discount";
                AddDiscountButton.IsEnabled = true;

                int changeDue = int.Parse(State.PaymentAmountText.Replace("£", string.Empty).Replace(".", string.Empty));
                int totalCostInPence = GlobalData.OrderInfo.GetTotalCostInPence();

                changeDue -= totalCostInPence;

                if (changeDue >= 0)
                {
                    State.ChangeDue = changeDue;
                    if (instantProcessIfEnough)
                    {
                        LoadOrderSummaryScreen(null, null);
                    }
                    else
                    {
                        OrderSummaryButton.Content = "Process Order";
                        OrderSummaryButton.IsEnabled = true;
                    }
                }
                else
                {
                    OrderSummaryButton.Content = "Amount entered not enough";
                    OrderSummaryButton.IsEnabled = false;
                    State.ChangeDue = 0;
                }
            }
            else
            {
                OrderSummaryButton.Content = "Enter cash amount";
                OrderSummaryButton.IsEnabled = false;

                AddManualEntryButton.Content = "Enter cash amount";
                AddManualEntryButton.IsEnabled = false;

                AddDiscountButton.Content = "Enter cash amount";
                AddDiscountButton.IsEnabled = false;

                State.ChangeDue = 0;
            }
        }

        private string ConvertIntToMoneyString(int priceInPence)
        {
            string prefix = "";

            if (priceInPence < 0)
            {
                prefix = "-";
                priceInPence *= -1;
            }
            if (priceInPence < 10)
            {
                return prefix + "£0.0" + priceInPence;
            }

            if (priceInPence < 100)
            {
                return prefix + "£0." + priceInPence;
            }
            string priceInPenceString = priceInPence.ToString();


            return prefix + "£" + priceInPenceString.Insert(priceInPenceString.Length - 2, ".");
        }

        private void LoadOrderSummaryScreen(object sender, RoutedEventArgs e)
        {
            OrderSummaryChangeDueTextBox.Text = "Change due:" + Environment.NewLine + ConvertIntToMoneyString(State.ChangeDue);

            GlobalData.OrderInfo.paymentMethod = PaymentMethod.Cash;
            GlobalData.OrderInfo.Vat = GlobalData.MenuSettings.VatInPercent;
            GlobalData.OrderInfo.MenuIdentifier = Sqlite.SqliteData.GetIdentiferForMenuConfig();
            GlobalData.OrderInfo.VatNumber = GlobalData.MenuSettings.VatNumber;
            Sqlite.SqliteData.SaveOrder(GlobalData.OrderInfo, GlobalData.OrderInfo.Vat, false);
            ReceiptPrinter.ChefsNoteQueue.QueueDocument(GlobalData.OrderInfo);
            ClearOrder();

            

            GlobalEvents.ChangeScreenEvent.Invoke(null, new ChangeScreenData() { Screen = Screen.OrderSummaryScreen });
        }

        private void AddManualEntryToOrder(object sender, RoutedEventArgs e)
        {
            int priceInPence = int.Parse(State.PaymentAmountText.Replace("£", string.Empty).Replace(".", string.Empty));

            OrderItemInfo orderItemInfo = new OrderItemInfo();
            orderItemInfo.ProductButtonId = -1;
            orderItemInfo.Quantity = 1;
            orderItemInfo.BasePriceInPence = priceInPence;

            GlobalEvents.AddOrUpdateOrderItem.Invoke(null, orderItemInfo);

            GlobalEvents.ChangeScreenEvent.Invoke(null, new ChangeScreenData() { Screen = Screen.BlankScreen });
        }

        private void AddDiscountToOrder(object sender, RoutedEventArgs e)
        {
            int priceInPence = int.Parse(State.PaymentAmountText.Replace("£", string.Empty).Replace(".", string.Empty)) * -1;

            OrderItemInfo orderItemInfo = new OrderItemInfo();
            orderItemInfo.ProductButtonId = -2;
            orderItemInfo.Quantity = 1;
            orderItemInfo.BasePriceInPence = priceInPence;

            GlobalEvents.AddOrUpdateOrderItem.Invoke(null, orderItemInfo);

            GlobalEvents.ChangeScreenEvent.Invoke(null, new ChangeScreenData() { Screen = Screen.BlankScreen });
        }

        private void AddRefundToOrder(object sender, RoutedEventArgs e)
        {
            int priceInPence = int.Parse(State.PaymentAmountText.Replace("£", string.Empty).Replace(".", string.Empty)) * -1;

            OrderItemInfo orderItemInfo = new OrderItemInfo();
            orderItemInfo.ProductButtonId = -3;
            orderItemInfo.Quantity = 1;
            orderItemInfo.BasePriceInPence = priceInPence;

            GlobalEvents.AddOrUpdateOrderItem.Invoke(null, orderItemInfo);

            GlobalEvents.ChangeScreenEvent.Invoke(null, new ChangeScreenData() { Screen = Screen.FunctionScreen });
        }

        private void ChangeDueScreenNewOrderPressed(object sender, RoutedEventArgs e)
        {
            CancelOrder();
            GlobalEvents.ChangeScreenEvent.Invoke(null, new ChangeScreenData() { Screen = Screen.TableSelect });
        }

        private void PrintReceipt(object sender, RoutedEventArgs e)
        {
            GlobalData.OrderInfo.MenuIdentifier = Sqlite.SqliteData.GetIdentiferForMenuConfig();
            GlobalData.OrderInfo.VatNumber = GlobalData.MenuSettings.VatNumber;
            ReceiptPrinter.CustomerReceiptQueue.QueueDocument(ReceiptData);
        }

        private void ShowOrderDiscountScreen(object sender, MouseButtonEventArgs e)
        {
            GlobalEvents.ChangeScreenEvent(null, new ChangeScreenData() { Screen = Screen.EnterOrderDiscountScreen });
        }

        private void OrderTakerClicked(object sender, RoutedEventArgs e)
        {
            GlobalEvents.SetLoggedInStatusEvent.Invoke(null, false);
        }

        #endregion

        public List<UIElement> ChildrenInRow(Grid grid, int row)
        {
            return grid.Children.Cast<UIElement>().Where(c => Grid.GetRow(c) == row).ToList();
        }

        private void GoBackCancelOrderPressed(object sender, RoutedEventArgs e)
        {
            GlobalEvents.ChangeScreenEvent.Invoke(null, new ChangeScreenData() { Screen = Screen.BlankScreen });
        }
    }
}