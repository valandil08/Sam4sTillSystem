using Sam4sTillSystem.Controls;
using Sam4sTillSystem.Data;
using Sam4sTillSystem.Data.EventData;
using Sam4sTillSystem.Data.JsonObject;
using Sam4sTillSystem.Data.JsonObject.MenuSetting;
using Sam4sTillSystem.Data.JsonObject.SiteSetting;
using Sam4sTillSystem.Enum;
using Sam4sTillSystem.FileFunctions;
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
    public partial class ItemSelectScreen : UserControl
    {
        public ItemSelectScreen()
        {
            InitializeComponent();

            GlobalEvents.ChangeScreenEvent += ChangeScreenEventHandler;
        }

        public void ChangeScreenEventHandler(object sender, ChangeScreenData e)
        {
            if (e.Screen == Screen.ItemSelect)
            {
                GlobalEvents.ChangeScreenTitle.Invoke(null, e.ScreenName);
                LoadItemsForNavMenu((List<ProductButton>) e.Data);
                ItemSelectGrid.Visibility = Visibility.Visible;
            }
            else
            {
                ItemSelectGrid.Visibility = Visibility.Hidden;
            }
        }


        private void LoadItemsForNavMenu(List<ProductButton> productButtons)
        {
            int counter = 0;

            foreach (ProductButton productButton in productButtons)
            {
                SellableMenuItem item = (SellableMenuItem)ItemSelectGrid.FindName("SellableItem" + counter);
                item.Padding = new Thickness(5);
                
                Action loadItemsOnClick = () => {
                    OrderItemInfo data = new OrderItemInfo();
                    data.ProductButtonId = productButton.ProductButtonId;
                    data.Quantity = 1;
                    data.BasePriceInPence = productButton.BasePriceInPence;
                    data.ProductScreenIds = productButton.ProductScreenIds;                    

                    if (productButton.ProductScreenIds.Length == 0)
                    {
                        OrderItemInfo itemInfo = GlobalData.OrderInfo.OrderItems.Where(x => x.ProductButtonId == productButton.ProductButtonId).FirstOrDefault();
                        if (itemInfo != null)
                        {
                            itemInfo.Quantity += 1;
                            GlobalEvents.AddOrUpdateOrderItem.Invoke(null, itemInfo);
                            GlobalEvents.ChangeOrderDiscount.Invoke(null, GlobalData.OrderInfo.PercentageBasedOrderDiscount);
                        }
                        else
                        {
                            GlobalEvents.AddOrUpdateOrderItem.Invoke(null, data);
                            GlobalEvents.ChangeOrderDiscount.Invoke(null, GlobalData.OrderInfo.PercentageBasedOrderDiscount);
                        }
                    }
                    else
                    {
                        // load next screen
                        GlobalEvents.ChangeScreenEvent
                        (
                            null,
                            new ChangeScreenData()
                            {
                                Screen = Screen.ItemOptionsScreen,
                                Data = data
                            }
                        );
                    }
                };
                

                item.Text = productButton.ButtonText;
                item.Price = productButton.BasePriceInPence;
                item.OnClick = loadItemsOnClick;
                item.Visibility = Visibility.Visible;
                item.IsVegetarian = false;// sellableItem.IsVegetarian;
                item.IsGlutenFree = false;// sellableItem.IsGlutenFree;


                int row = counter / 4;
                int column = counter % 4;


                item.Margin = new Thickness(
                    column == 0 ? 3 : 2.5,
                    row == 0 ? 3 : 2.5,
                    column == 3 ? 4 : 2.5,
                    row == 3 ? 4 : 2.5
                );

                Grid.SetRow(item, row);
                Grid.SetColumn(item, column);

                item.Render();

                counter++;
            }

            for (int i = counter; i < 25; i++)
            {
                SellableMenuItem item = (SellableMenuItem)ItemSelectGrid.FindName("SellableItem" + i);

                item.Visibility = Visibility.Hidden;
            }
        }

    }
}
