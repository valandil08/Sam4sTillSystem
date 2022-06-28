using Sam4sTillSystem.Data;
using Sam4sTillSystem.Data.JsonObject.MenuSetting;
using Sam4sTillSystem.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Sam4sTillSystem.Controls
{
    /// <summary>
    /// Interaction logic for ClickableTile.xaml
    /// </summary>
    public partial class OrderDisplayRow : UserControl
    {
        public Guid Identifier;

        public OrderDisplayRow()
        {
            InitializeComponent();
            GlobalEvents.AddOrUpdateOrderItem += HandleAddOrUpdateOrderItem;
        }

        public void Update(OrderItemInfo orderItemInfo)
        {
            switch (orderItemInfo.ProductButtonId)
            {
                case -1:
                    ProductName.Text = "Manual Entry";
                    break;

                case -2:
                    ProductName.Text = "Fixed Discount";
                    break;

                case -3:
                    ProductName.Text = "Refund";
                    break;

                default:
                    ProductName.Text = GlobalData.MenuSettings.ProductButtons.Where(x => x.ProductButtonId == orderItemInfo.ProductButtonId).First().ButtonText;
                    break;
            }
            Quantity.Text = orderItemInfo.Quantity.ToString();

            foreach (UIElement element in LeftOptionsDisplay.Children.Cast<UIElement>().ToList())
            {
                LeftOptionsDisplay.Children.Remove(element);
            }

            foreach (UIElement element in RightOptionsDisplay.Children.Cast<UIElement>().ToList())
            {
                RightOptionsDisplay.Children.Remove(element);
            }
            


            List<TextBlock> textBlocks = new List<TextBlock>();

            for (int i = 0; i < orderItemInfo.ScreenValues.Count(); i++)
            {
                bool isGenericExtras = i == orderItemInfo.ProductScreenIds.Length;
                bool isAdjustments = i == orderItemInfo.ProductScreenIds.Length + 1;

                int[] controlValues = new int[0];

                if (orderItemInfo.ScreenValues.ContainsKey(i))
                {
                    controlValues = orderItemInfo.ScreenValues[i];
                }

                for (int j = 0; j < controlValues.Length; j++)
                {
                    TextBlock textBlock = new TextBlock();

                    int controlId = 0;

                    if (isGenericExtras)
                    {
                        controlId = GlobalData.MenuSettings.GenericExtraScreenControls[j];
                    }
                    else if (isAdjustments)
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

                    if (difference != 0)
                    {
                        if (isAdjustments)
                        {
                            textBlock.Text = (controlValues[j] == 0 ? "No" : "Only " + controlValues[j]) + " " + control.ButtonText;
                        }
                        else if (control.IsQuantitySetter == false)
                        {
                            textBlock.Text = "* " + control.ButtonText;
                        }
                        else
                        {
                            int totalInPence = difference > 0 ? control.CostPerExtraItemInPence * difference : -control.DiscountPerRemovedItemInPence * difference;

                            textBlock.Text = (difference < 0 ? difference.ToString() : "+" + difference) + " " + control.ButtonText;

                        }
                        textBlocks.Add(textBlock);
                    }
                }
            }

            CostEach.Text = UIHelper.FormatPrice(orderItemInfo.GetTotalCostEachInPence());
            CostTotal.Text = UIHelper.FormatPrice(orderItemInfo.GetTotalCostInPence());

            // Sort by item name and ignore quantity (+1 Beans)
            textBlocks = textBlocks.OrderBy(x => x.Text.StartsWith("No") ? "z" + x.Text.Substring(3) : x.Text.StartsWith("Only") ? "y" + x.Text.Substring(5) : x.Text.StartsWith("*") ? "a" + x.Text.Substring(2) : "x" + x.Text.Substring(3)).ToList();


            for (int i = 0; i < textBlocks.Count(); i++)
            {
                if (i > (textBlocks.Count() / 2))
                {
                    RightOptionsDisplay.Children.Add(textBlocks[i]);
                }
                else
                {
                    LeftOptionsDisplay.Children.Add(textBlocks[i]);
                }
            }
        }

        public void HandleAddOrUpdateOrderItem(object sender, OrderItemInfo orderItemInfo)
        {
            if (Identifier == orderItemInfo.Identifier)
            {
                Update(orderItemInfo);
            }
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            GlobalEvents.LoadItemSummaryForOrderItem.Invoke(null, Identifier);
        }

        private void Grid_TouchDown(object sender, TouchEventArgs e)
        {
            GlobalEvents.LoadItemSummaryForOrderItem.Invoke(null, Identifier);
        }
    }    
}
