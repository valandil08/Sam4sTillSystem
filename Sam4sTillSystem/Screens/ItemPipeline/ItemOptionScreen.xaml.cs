using Sam4sTillSystem.Controls;
using Sam4sTillSystem.Data;
using Sam4sTillSystem.Data.EventData;
using Sam4sTillSystem.Data.JsonObject;
using Sam4sTillSystem.Data.JsonObject.MenuSetting;
using Sam4sTillSystem.Data.JsonObject.SiteSetting;
using Sam4sTillSystem.Data.PopupData;
using Sam4sTillSystem.Enum;
using Sam4sTillSystem.FileFunctions;
using Sam4sTillSystem.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace Sam4sTillSystem.Screens.ItemPipeline
{
    /// <summary>
    /// Interaction logic for CloudUplinkScreen.xaml
    /// </summary>
    public partial class ItemOptionsScreen : UserControl
    {
        private int ScreenNumber = 0;

        private OrderItemInfo orderItemInfo = null;

        public ItemOptionsScreen()
        {
            InitializeComponent();

            GlobalEvents.ChangeScreenEvent += ChangeScreenEventHandler;
            GlobalEvents.QuantitySelectGridAmountSelectedEvent += QuanitySelectedViaGrid;
            GlobalEvents.LoadItemSummaryForOrderItem += HandleLoadItemSummaryForOrderItem;
        }

        void QuanitySelectedViaGrid(object sender, GirdAmountSelectedData data)
        {
            if (data.guid == orderItemInfo.Identifier)
            {
                orderItemInfo.Quantity = data.amount;
                LoadItemSummary();
            }
        }

        public void HandleLoadItemSummaryForOrderItem(object sender, Guid identifier)
        {
            this.orderItemInfo = GlobalData.OrderInfo.OrderItems.Where(x => x.Identifier == identifier).First();

            if (this.orderItemInfo.ProductButtonId < 0)
            {
                return;
            }

            GlobalEvents.ChangeScreenEvent.Invoke
            (
                null,
                new ChangeScreenData()
                {
                    Screen = Screen.ItemSummaryScreen,
                    Data = orderItemInfo
                }
            ); ;
        }

        public void ChangeScreenEventHandler(object sender, ChangeScreenData e)
        {
            if(e.Screen == Screen.ItemOptionsScreen || e.Screen == Screen.ItemSummaryScreen)
            {
                orderItemInfo = (OrderItemInfo)e.Data;
                GenericExtrasGrid.Visibility = Visibility.Hidden;
                GeneralExtrasControls.Visibility = Visibility.Hidden;
                if (e.Screen == Screen.ItemOptionsScreen)
                {
                    ScreenNumber = 0;
                    LoadDataForScreen(ScreenNumber);

                    ParentGrid.Visibility = Visibility.Visible;
                    ItemOptionsGrid.Visibility = Visibility.Visible;
                    ItemSummaryGrid.Visibility = Visibility.Hidden;
                }
                else
                {
                    ScreenNumber = orderItemInfo.ProductScreenIds.Count();
                    LoadItemSummary();

                    ParentGrid.Visibility = Visibility.Visible;
                    ItemOptionsGrid.Visibility = Visibility.Hidden;
                    ItemSummaryGrid.Visibility = Visibility.Visible;
                }
            }
            else
            {
                ParentGrid.Visibility = Visibility.Hidden;
                ItemOptionsGrid.Visibility = Visibility.Hidden;
                ItemSummaryGrid.Visibility = Visibility.Hidden;
                GenericExtrasGrid.Visibility = Visibility.Hidden;
            }

        }

        private void LoadDataForScreen(int screenNumber)
        {
            if (GlobalData.SystemSettings.DeveloperSettings.ReloadConfigInbetweenScreens)
            {
                MenuConfigFile.LoadSettings();
            }

            EnsureControlValuesSetup();

            bool isSwapIns = ScreenNumber == orderItemInfo.ProductScreenIds.Length;
            bool isSwapOuts = ScreenNumber == orderItemInfo.ProductScreenIds.Length + 1;

            string productName = GlobalData.MenuSettings.ProductButtons.Where(x => x.ProductButtonId == orderItemInfo.ProductButtonId).Select(x => x.ButtonText).First();


            GeneralExtrasControls.Visibility = (isSwapIns || isSwapOuts) ? Visibility.Visible : Visibility.Hidden;
            ItemOptionControls.Visibility   = GeneralExtrasControls.Visibility == Visibility.Hidden ? Visibility.Visible : Visibility.Hidden;

            if (isSwapIns)
            {
                GlobalEvents.ChangeScreenTitle.Invoke(null, productName + " - Swap Fors");
            }
            else if(isSwapOuts)
            {
                GlobalEvents.ChangeScreenTitle.Invoke(null, productName + " - Default Includes");
            }
            else
            {                
                string screenName = GlobalData.MenuSettings.ProductScreens.Where(x => x.ProductScreenId == orderItemInfo.ProductScreenIds[ScreenNumber]).Select(x => x.ScreenTitle).First();

                GlobalEvents.ChangeScreenTitle.Invoke(null, productName + " - " + screenName);
            }

            for (int i = 0; i < 16; i++)
            {
                ItemQuantitySetter numberControl = (ItemQuantitySetter)ItemOptionsGrid.FindName("NumberControl" + (i + 1));
                OptionElement yesNoControl = (OptionElement)ItemOptionsGrid.FindName("YesNoControl" + (i + 1));
                numberControl.Visibility = Visibility.Hidden;
                yesNoControl.Visibility = Visibility.Hidden;
                yesNoControl.Render();
            }

            int[] controlIds = new int[0];

            if (isSwapIns)
            {
                // load generic extras
                controlIds = GlobalData.MenuSettings.GenericExtraScreenControls;
            }
            else if (isSwapOuts)
            {
                // load default includes
                controlIds = GlobalData.MenuSettings.ProductButtons.Where(x => x.ProductButtonId == orderItemInfo.ProductButtonId).First().AdjustmentControlIds;
            }
            else
            {
                controlIds = GlobalData.MenuSettings.ProductScreens.Where(x => x.ProductScreenId == orderItemInfo.ProductScreenIds[screenNumber]).Select(x => x.ProductScreenControlIds).First();
            }
            int[] controlValues = new int[controlIds.Length];

            bool screenNumberExistInData = orderItemInfo.ScreenValues.ContainsKey(screenNumber);

            if (screenNumberExistInData)
            {
                controlValues = orderItemInfo.ScreenValues[screenNumber];

                if (GlobalData.SystemSettings.DeveloperSettings.ReloadConfigInbetweenScreens)
                {
                    if (controlValues.Length != controlIds.Length)
                    {
                        controlValues = new int[controlIds.Length];
                        orderItemInfo.ScreenValues[ScreenNumber] = controlValues;
                    }
                }

            }

            int numRows = (int)Math.Ceiling(((double)controlIds.Length / 2));

            for (int i = 0; i < controlIds.Length && i < 16; i++)
            {
                ProductScreenControl control = GlobalData.MenuSettings.ProductScreenControls.Where(x => x.ProductScreenControlId == controlIds[i]).First();

                int elementId = i < numRows ? i : ((i - numRows) + 8);

                ItemQuantitySetter numberControl = (ItemQuantitySetter)ItemOptionsGrid.FindName("NumberControl" + (elementId + 1));
                OptionElement yesNoControl = (OptionElement)ItemOptionsGrid.FindName("YesNoControl" + (elementId + 1));


                if (control.IsQuantitySetter == true)
                {
                    numberControl.Visibility = Visibility.Visible;
                    yesNoControl.Visibility = Visibility.Hidden;

                    numberControl.MinQuantity = control.minimumQuantity;
                    numberControl.MaxQuantity = control.maximumQuantity;
                    numberControl.DefaultQuantity = screenNumberExistInData ? controlValues[i] : control.StartingQuantity;
                    numberControl.ItemText = control.ButtonText;

                    if (orderItemInfo.ScreenValues.ContainsKey(ScreenNumber))
                    {
                        numberControl.DefaultQuantity = orderItemInfo.ScreenValues[ScreenNumber][i];
                    }

                    controlValues[i] = control.StartingQuantity;
                    numberControl.Setup(null, null);
                }
                else
                {
                    numberControl.Visibility = Visibility.Hidden;
                    yesNoControl.Visibility = Visibility.Visible;
                    yesNoControl.IsSelected = screenNumberExistInData ? controlValues[i] == 1 : control.StartingQuantity == 1;
                    if (orderItemInfo.ScreenValues.ContainsKey(ScreenNumber))
                    {
                        yesNoControl.IsSelected = orderItemInfo.ScreenValues[ScreenNumber][i] == 1;
                    }

                    yesNoControl.Text = control.ButtonText;
                    yesNoControl.Render();

                    controlValues[i] = yesNoControl.IsSelected == true ? 1 : 0;
                }
            }

            orderItemInfo.ScreenValues[screenNumber] = controlValues;

            UpdateSectionTotal();
            ItemOptionsGrid.Visibility = Visibility.Visible;
            ItemSummaryGrid.Visibility = Visibility.Hidden;
        }

        private void SkipPressed(object sender, RoutedEventArgs e)
        {
            ScreenNumber++;
            while (ScreenNumber < orderItemInfo.ProductScreenIds.Length)
            {
                int screenId = orderItemInfo.ProductScreenIds[ScreenNumber];

                ProductScreen productScreen = GlobalData.MenuSettings.ProductScreens.Where(x => x.ProductScreenId == screenId).First();
                if (productScreen.IsMandatory)
                {
                    if (orderItemInfo.ScreenValues.ContainsKey(ScreenNumber))
                    {

                        int totalAmount = orderItemInfo.ScreenValues[ScreenNumber].Sum();

                        // skip already completed screens
                        if (!(totalAmount >= productScreen.MinItemsRequired && totalAmount <= productScreen.MaxItemsAllowed))
                        {
                            LoadDataForScreen(ScreenNumber);
                            break;
                        }
                        else
                        {
                            ScreenNumber++;
                        }
                    }                
                    else
                    {
                        LoadDataForScreen(ScreenNumber);

                        break;
                    }
                }
                else
                {
                    ScreenNumber++;
                }
            }

            if (ScreenNumber == orderItemInfo.ProductScreenIds.Length)
            {
                LoadItemSummary();
            }
        }

        private void CostBreakdownPressed(object sender, RoutedEventArgs e)
        {
            List<CostBreakdownData> data = new List<CostBreakdownData>();


            int[] controlIds;

            if (ScreenNumber == orderItemInfo.ProductScreenIds.Length)
            {
                controlIds = GlobalData.MenuSettings.GenericExtraScreenControls;
            }
            else if (ScreenNumber == orderItemInfo.ProductScreenIds.Length + 1)
            {
                controlIds = GlobalData.MenuSettings.ProductButtons.Where(x => x.ProductButtonId == orderItemInfo.ProductButtonId).Select(x => x.AdjustmentControlIds).First();
            }
            else
            {
                int screenId = orderItemInfo.ProductScreenIds[ScreenNumber];
                controlIds = GlobalData.MenuSettings.ProductScreens.Where(x => x.ProductScreenId == screenId).Select(x => x.ProductScreenControlIds).First();
            }
            
            int numRows = (int)Math.Ceiling(((double)controlIds.Length / 2));

            for (int i = 0; i< orderItemInfo.ScreenValues[ScreenNumber].Length; i++)
            {
                ProductScreenControl control = GlobalData.MenuSettings.ProductScreenControls.Where(x => x.ProductScreenControlId == controlIds[i]).First();

                CostBreakdownData rowData = new CostBreakdownData();

                rowData.Name = control.ButtonText;
                rowData.CostPerExtraItemInPence = control.CostPerExtraItemInPence;
                rowData.DiscountPerRemovedItemInPence = control.DiscountPerRemovedItemInPence;

                int elementId = i < numRows ? i : ((i - numRows) + 8);
                elementId++;
                if (control.IsQuantitySetter)
                {
                    ItemQuantitySetter numberControl = (ItemQuantitySetter)ItemOptionsGrid.FindName("NumberControl" + elementId);

                    int quantity = numberControl.GetQuantity();

                    rowData.Difference = quantity - control.StartingQuantity;
                }
                else
                {
                    OptionElement yesNoControl = (OptionElement)ItemOptionsGrid.FindName("YesNoControl" + elementId);

                    int quantity = yesNoControl.IsSelected ? 1 : 0;
                    rowData.Difference = quantity - control.StartingQuantity;
                }
                
                
                rowData.CurrentCostInPence = rowData.Difference > 0 ? rowData.CostPerExtraItemInPence * rowData.Difference : -rowData.DiscountPerRemovedItemInPence * rowData.Difference;

                data.Add(rowData);
            }

            GlobalEvents.ShowCostBreakdownGridEvent.Invoke(null, data);
        }

        private void NextScreenPressed(object sender, RoutedEventArgs e)
        {
            ScreenNumber++;

            int genericExtrasScreenNumber = orderItemInfo.ProductScreenIds.Length;
            int adjustmentsScreenNumber = orderItemInfo.ProductScreenIds.Length + 1;

            if (ScreenNumber == genericExtrasScreenNumber || ScreenNumber == adjustmentsScreenNumber)
            {
                ScreenNumber = orderItemInfo.ProductScreenIds.Length + 2;
            }
            
            if (ScreenNumber < orderItemInfo.ProductScreenIds.Length)
            {
                LoadDataForScreen(ScreenNumber);
            }
            else
            {

                LoadItemSummary();
            }
        }

        private void PreviousScreenPressed(object sender, RoutedEventArgs e)
        {
            ScreenNumber--;


            int genericExtrasScreenNumber = orderItemInfo.ProductScreenIds.Length;
            int adjustmentsScreenNumber = orderItemInfo.ProductScreenIds.Length + 1;

            if (ScreenNumber == genericExtrasScreenNumber || ScreenNumber == adjustmentsScreenNumber)
            {
                ScreenNumber = orderItemInfo.ProductScreenIds.Length - 1;
            }

            if (ScreenNumber < 0)
            {
                ReturnToCategoryScreen();
            }
            else
            {
                LoadDataForScreen(ScreenNumber);
            }
        }

        private void ReturnToCategoryScreen()
        {
            int categoryId = GlobalData.MenuSettings.ProductButtons.Where(x => x.ProductButtonId == orderItemInfo.ProductButtonId).Select(x => x.MenuCategoryId).First();
            string categoryName = GlobalData.MenuSettings.Categories.Where(x => x.MenuCategoryId == categoryId).Select(x => x.Text).First();
            List<ProductButton> productButtons = GlobalData.MenuSettings.ProductButtons.Where(x => x.MenuCategoryId == categoryId).ToList();

            GlobalEvents.ChangeScreenEvent.Invoke
            (
                null,
                new ChangeScreenData()
                {
                    Screen = Screen.ItemSelect,
                    ScreenName = categoryName,
                    Data = productButtons
                }
            );
        }

        private void LoadItemSummary()
        {
            EnsureControlValuesSetup();
            if (!GlobalData.OrderInfo.OrderItems.Where(x => x.Identifier == orderItemInfo.Identifier).Any())
            {
                AddToOrderButton.Text = "Add to Order";
            }
            else
            {
                AddToOrderButton.Text = "Update Order";
            }

            AddToOrderButton.Render();

            Quantity.Text = orderItemInfo.Quantity.ToString();

            string productName = GlobalData.MenuSettings.ProductButtons.Where(x => x.ProductButtonId == orderItemInfo.ProductButtonId).Select(x => x.ButtonText).First();
            GlobalEvents.ChangeScreenTitle.Invoke(null, productName + " - Item Summary");

            // ensure back button always returns to screen before item summary
            ScreenNumber = orderItemInfo.ProductScreenIds.Length + 2;


            foreach (UIElement element in ItemSummaryLeftColumn.Children.Cast<UIElement>().ToList())
            {
                ItemSummaryLeftColumn.Children.Remove(element);
            }

            foreach (UIElement element in ItemSummaryCenterColumn.Children.Cast<UIElement>().ToList())
            {
                ItemSummaryCenterColumn.Children.Remove(element);
            }

            foreach (UIElement element in ItemSummaryRightColumn.Children.Cast<UIElement>().ToList())
            {
                ItemSummaryRightColumn.Children.Remove(element);
            }

            int orderTotalInPence = 0;

            List<string> textToRender = new List<string>();
            textToRender.Add("Base Price");
            textToRender.Add(orderItemInfo.BasePriceInPence == 0 ? "£0.00" : UIHelper.FormatPrice(orderItemInfo.BasePriceInPence));
            textToRender.Add("");

            for (int i = 0; i < orderItemInfo.ScreenValues.Count(); i++)
            {
                bool isSwapIns = i == orderItemInfo.ProductScreenIds.Length;
                bool isSwapOuts = i == orderItemInfo.ProductScreenIds.Length + 1;

                // if generic extras screen
                if (isSwapIns)
                {
                    textToRender.Add("Swap Fors");
                }
                else if(isSwapOuts)
                {
                    textToRender.Add("Default Includes");
                }
                else
                {

                    int screenId = orderItemInfo.ProductScreenIds[i];

                    textToRender.Add(GlobalData.MenuSettings.ProductScreens.Where(x => x.ProductScreenId == screenId).First().ScreenTitle);
                }

                int[] controlValues = new int[0];

                if (orderItemInfo.ScreenValues.ContainsKey(i))
                {
                    controlValues = orderItemInfo.ScreenValues[i];
                }

                bool itemAdded = false;

                for (int j = 0; j < controlValues.Length; j++)
                {
                    int controlId = 0;

                    if (isSwapIns)
                    {
                        controlId = GlobalData.MenuSettings.GenericExtraScreenControls[j];
                    }
                    else if(isSwapOuts)
                    {
                        controlId = GlobalData.MenuSettings.ProductButtons.Where(x => x.ProductButtonId == orderItemInfo.ProductButtonId).First().AdjustmentControlIds[j];
                    }
                    else
                    {
                        int screenId = orderItemInfo.ProductScreenIds[i];
                        controlId = GlobalData.MenuSettings.ProductScreens.Where(x => x.ProductScreenId == screenId).First().ProductScreenControlIds[j];
                    }

                    ProductScreenControl control = GlobalData.MenuSettings.ProductScreenControls.Where(x => x.ProductScreenControlId == controlId).First();
                    
                    TextBlock option = new TextBlock();

                    int difference;

                    if (control.IsQuantitySetter)
                    {
                        difference = controlValues[j] - control.StartingQuantity;
                    }
                    else
                    {
                        difference = controlValues[j] - control.StartingQuantity;
                    }

                    if (difference == 0)
                    {
                        continue;
                    }
                    else
                    {
                        itemAdded = true;
                    }


                    int totalInPence = difference > 0 ? control.CostPerExtraItemInPence * difference : -control.DiscountPerRemovedItemInPence * difference;

                    orderTotalInPence += totalInPence;

                    if (isSwapOuts)
                    {
                        if (controlValues[j] == 0)
                        {
                            textToRender.Add("No " + control.ButtonText);
                        }
                        else
                        {
                            textToRender.Add("Only " + controlValues[j] + " " + control.ButtonText);
                        }
                    }
                    else if (isSwapIns)
                    {
                        textToRender.Add("+" + controlValues[j] + " " + control.ButtonText);
                    }
                    else
                    {
                        string priceText = "";

                        if (totalInPence != 0)
                        {
                            priceText = "(" + UIHelper.FormatPrice(totalInPence) + ")"; 
                        }

                        if (control.IsQuantitySetter)
                        {
                            textToRender.Add((difference < 0 ? "" : "+") + difference + " " + control.ButtonText + " " + priceText);
                        }
                        else
                        {
                            textToRender.Add("* " + control.ButtonText + " " + priceText);
                        }

                    }
                }

                if (itemAdded == false)
                {
                    if (isSwapOuts)
                    {
                        textToRender.Add("No Adjustments");
                    }
                    else
                    {
                        textToRender.Add("None");
                    }
                }
                textToRender.Add("");
            }

            int adjustmentFee = orderItemInfo.GetAdjustmentFee();

            if (adjustmentFee > 0)
            {

                textToRender.Add("Adjustment Fee");
                textToRender.Add(UIHelper.FormatPrice(adjustmentFee));
                textToRender.Add("");
            }

            // remove last blank line
            textToRender.RemoveAt(textToRender.Count() - 1);

            int lastIndexRendered = -1;
            int panelNumber = -1;

            for (int i = 0; i < textToRender.Count; i++)
            {
                if (textToRender[i] != "" && i < textToRender.Count() - 1)
                {
                    continue;
                }

                bool firstRender = true;

                StackPanel panel = new StackPanel();


                if (panelNumber != -1)
                {
                    panel.Name = "Screen"+panelNumber.ToString();
                    panel.MouseLeftButtonDown += StackPanelMouseDown;
                    panel.TouchUp += StackPanelTouchUp;
                }
                
                panelNumber++;

                while (lastIndexRendered < i)
                {
                    TextBlock row = new TextBlock();

                    lastIndexRendered++;
                    row.Text = textToRender[lastIndexRendered];

                    if (firstRender)
                    {
                        row.FontWeight = FontWeights.Bold;
                        firstRender = false;
                    }

                    panel.Children.Add(row);
                }

                if (i < 21)
                {
                    ItemSummaryLeftColumn.Children.Add(panel);
                }
                else if (i < 40)
                {
                    ItemSummaryCenterColumn.Children.Add(panel);
                }
                else
                {
                    ItemSummaryRightColumn.Children.Add(panel);
                }
            }
            

            orderTotalInPence += orderItemInfo.BasePriceInPence;

            Each.Text = UIHelper.FormatPrice(orderItemInfo.GetTotalCostEachInPence());

            orderTotalInPence *= orderItemInfo.Quantity;

            Total.Text = UIHelper.FormatPrice(orderItemInfo.GetTotalCostInPence());

            ItemOptionsGrid.Visibility = Visibility.Hidden;
            ItemSummaryGrid.Visibility = Visibility.Visible;
        }

        private void ItemQuantitySetterValueChange(object sender, RoutedEventArgs e)
        {
            UpdateSectionTotal();
        }

        private void OptionElementClick(object sender, RoutedEventArgs e)
        {
            if (MenuHelper.GetProductScreen(orderItemInfo.ProductScreenIds[ScreenNumber]).MaxItemsAllowed == 1)
            {
                string name = ((OptionElement)sender).Name;

                OptionElement yesNoControl = (OptionElement)ItemOptionsGrid.FindName(name);

                if (yesNoControl.IsSelected)
                {
                    for (int i = 0; i < 16; i++)
                    {
                        if (i < orderItemInfo.ScreenValues[ScreenNumber].Length)
                        {
                            orderItemInfo.ScreenValues[ScreenNumber][i] = 0;
                        }

                        if (!("YesNoControl" + (i + 1) == name))
                        {
                            OptionElement yesNoControl2 = (OptionElement)ItemOptionsGrid.FindName("YesNoControl" + (i + 1));
                            yesNoControl2.IsSelected = false;
                            yesNoControl2.Render();
                        }                      
                    }
                }
            }
            UpdateSectionTotal();
        }

        private void UpdateSectionTotal()
        {
            bool isGenericExtras = ScreenNumber == orderItemInfo.ProductScreenIds.Length;
            bool isAdjustments = ScreenNumber == orderItemInfo.ProductScreenIds.Length + 1;

            int totalInPence = 0;

            int[] controlIds = new int[0];

            if (isGenericExtras)
            {
                // load generic extras
                controlIds = GlobalData.MenuSettings.GenericExtraScreenControls;
            }
            else if (isAdjustments)
            {
                controlIds = GlobalData.MenuSettings.ProductButtons.Where(x => x.ProductButtonId == orderItemInfo.ProductButtonId).First().AdjustmentControlIds;
            }
            else
            {
                controlIds = GlobalData.MenuSettings.ProductScreens.Where(x => x.ProductScreenId == orderItemInfo.ProductScreenIds[ScreenNumber]).Select(x => x.ProductScreenControlIds).First();
            }


            int numRows = (int)Math.Ceiling(((double)controlIds.Length / 2));


            int[] controlValues = new int[controlIds.Length];

            for (int i = 0; i < orderItemInfo.ScreenValues[ScreenNumber].Length; i++)
            {
                ProductScreenControl control = GlobalData.MenuSettings.ProductScreenControls.Where(x => x.ProductScreenControlId == controlIds[i]).First();

                CostBreakdownData rowData = new CostBreakdownData();

                rowData.CostPerExtraItemInPence = control.CostPerExtraItemInPence;
                rowData.DiscountPerRemovedItemInPence = control.DiscountPerRemovedItemInPence;

                int elementId = i < numRows ? i : ((i - numRows) + 8);
                elementId++;
                if (control.IsQuantitySetter)
                {
                    ItemQuantitySetter numberControl = (ItemQuantitySetter)ItemOptionsGrid.FindName("NumberControl" + elementId);

                    int quantity = numberControl.GetQuantity();
                    controlValues[i] = quantity;

                    rowData.Difference = quantity - control.StartingQuantity;
                }
                else
                {
                    OptionElement yesNoControl = (OptionElement)ItemOptionsGrid.FindName("YesNoControl" + elementId);

                    int quantity = yesNoControl.IsSelected ? 1 : 0;
                    controlValues[i] = quantity;
                    rowData.Difference = quantity - control.StartingQuantity;
                }


                totalInPence += rowData.Difference > 0 ? rowData.CostPerExtraItemInPence * rowData.Difference : -rowData.DiscountPerRemovedItemInPence * rowData.Difference;
            }

            int totalAmount = controlValues.Sum();


            if (isGenericExtras || isAdjustments)
            {
                ItemOptionsSkip.IsEnabled = true;
                ItemOptionsNextScreen.IsEnabled = true;
                ItemOptionsNextScreen.Text = "Next Screen";
            }
            else
            {
                int screenId = orderItemInfo.ProductScreenIds[ScreenNumber];

                ProductScreen productScreen = GlobalData.MenuSettings.ProductScreens.Where(x => x.ProductScreenId == screenId).First();

                if (productScreen.IsMandatory)
                {
                    ItemOptionsSkip.IsEnabled = false;
                    if (totalAmount > productScreen.MaxItemsAllowed)
                    {
                        ItemOptionsNextScreen.IsEnabled = false;
                        ItemOptionsNextScreen.Text = "Too many selected" + Environment.NewLine + "(" + productScreen.MaxItemsAllowed + " max)";
                    }
                    else if (totalAmount < productScreen.MinItemsRequired)
                    {
                        ItemOptionsNextScreen.IsEnabled = false;
                        ItemOptionsNextScreen.Text = "Too few selected" + Environment.NewLine + "(" + productScreen.MinItemsRequired + " min)";
                    }
                    else
                    {
                        ItemOptionsSkip.IsEnabled = true;
                        ItemOptionsNextScreen.IsEnabled = true;
                        ItemOptionsNextScreen.Text = "Next Screen";
                    }
                }
                else
                {
                    ItemOptionsSkip.IsEnabled = true;
                    ItemOptionsNextScreen.IsEnabled = true;
                    ItemOptionsNextScreen.Text = "Next Screen";
                }
            }
            ItemOptionsSkip.Render();
            ItemOptionsNextScreen.Render();

            ScreenTotal.Text = "Cost Breakdown" + Environment.NewLine + UIHelper.FormatPrice(totalInPence);

            if (orderItemInfo.ScreenValues.ContainsKey(ScreenNumber))
            {
                orderItemInfo.ScreenValues[ScreenNumber] = controlValues;
            }
            else
            {
                orderItemInfo.ScreenValues.Add(ScreenNumber, controlValues);
            }
            ScreenTotalGeneralExtras.Text = ScreenTotal.Text;
            ScreenTotalGeneralExtras.Render();
            ScreenTotal.Render();
        }

        private void OrderQuantityPressed(object sender, RoutedEventArgs e)
        {

            SetQuantityEventData data = new SetQuantityEventData();
            data.MinQuantity = 0;
            data.MaxQuantity = 20;
            data.ControlIdenfier = orderItemInfo.Identifier;
            GlobalEvents.ShowQuantitySelectGridEvent.Invoke(null, data);       
        }

        private void StackPanelMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            string name = ((StackPanel)sender).Name;
            int screenNumber = int.Parse(name.Replace("Screen", string.Empty));
            ScreenNumber = screenNumber;
            LoadDataForScreen(screenNumber);
        }

        private void StackPanelTouchUp(object sender, System.Windows.Input.TouchEventArgs e)
        {
            string name = ((StackPanel)sender).Name;
            int screenNumber = int.Parse(name.Replace("Screen", string.Empty));
            ScreenNumber = screenNumber;
            LoadDataForScreen(screenNumber);
        }


        private void GenericExtrasPressed(object sender, RoutedEventArgs e)
        {
            ScreenNumber = orderItemInfo.ProductScreenIds.Length;

            LoadDataForScreen(ScreenNumber);
        }

        private void RemovalsPressed(object sender, RoutedEventArgs e)
        {
            ScreenNumber = orderItemInfo.ProductScreenIds.Length + 1;

            LoadDataForScreen(ScreenNumber);
        }

        private void AddToOrderPressed(object sender, RoutedEventArgs e)
        {
            GlobalEvents.AddOrUpdateOrderItem.Invoke(null, orderItemInfo);
            GlobalEvents.UpdateOrderTotal.Invoke(null, null);
            GlobalEvents.ChangeScreenEvent.Invoke(null, new ChangeScreenData() { Screen = Screen.BlankScreen });
        }

        private void EnsureControlValuesSetup()
        {
            if (orderItemInfo.ScreenValues.Count() == 0)
            {

                int[] productScreenIds = GlobalData.MenuSettings.ProductButtons.Where(x => x.ProductButtonId == orderItemInfo.ProductButtonId).First().ProductScreenIds;
                for (int i = 0; i < productScreenIds.Length; i++)
                {
                    int[] productScreenControlIds = GlobalData.MenuSettings.ProductScreens.Where(x => x.ProductScreenId == productScreenIds[i]).First().ProductScreenControlIds;

                    int[] controlValues = new int[productScreenControlIds.Length];
                    for (int j = 0; j < productScreenControlIds.Length; j++)
                    {
                        ProductScreenControl control = GlobalData.MenuSettings.ProductScreenControls.Where(x => x.ProductScreenControlId == productScreenControlIds[j]).First();
                        controlValues[j] = control.StartingQuantity;
                    }

                    orderItemInfo.ScreenValues.Add(i, controlValues);
                }

                int[] generalExtraControlValues = new int[GlobalData.MenuSettings.GenericExtraScreenControls.Length];

                // General Extra
                for (int i = 0; i < GlobalData.MenuSettings.GenericExtraScreenControls.Length; i++)
                {
                    ProductScreenControl control = GlobalData.MenuSettings.ProductScreenControls.Where(x => x.ProductScreenControlId == GlobalData.MenuSettings.GenericExtraScreenControls[i]).First();
                    generalExtraControlValues[i] = control.StartingQuantity;
                }

                orderItemInfo.ScreenValues.Add(orderItemInfo.ScreenValues.Count(), generalExtraControlValues);
                
                
                int[] adjustmentControlIds = GlobalData.MenuSettings.ProductButtons.Where(x => x.ProductButtonId == orderItemInfo.ProductButtonId).First().AdjustmentControlIds;

                int[] adjustmentControlValues = new int[adjustmentControlIds.Length];


                for (int i = 0; i < adjustmentControlIds.Length; i++)
                {
                    ProductScreenControl control = GlobalData.MenuSettings.ProductScreenControls.Where(x => x.ProductScreenControlId == adjustmentControlIds[i]).First();
                    adjustmentControlValues[i] = control.StartingQuantity;
                }


                // Simulate Adjustments Screen
                orderItemInfo.ScreenValues.Add(orderItemInfo.ScreenValues.Count(), adjustmentControlValues);

                /*
                int[] removalValues = new int[GlobalData.MenuSettings.GenericExtraScreenControls.Length];

                // Removals
                for (int i = 0; i < orderItemInfo.ScreenValues.Count(); i++)
                {

                }'*/
            }
        }
    }
}
