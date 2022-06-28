using Sam4sTillSystem.Data.EventData;
using Sam4sTillSystem.Enum;
using Sam4sTillSystem.Helpers;
using System.Windows;
using System.Windows.Controls;

namespace Sam4sTillSystem.Screens
{

    public partial class OrderDiscountScreen : UserControl
    {
        //TableSelectScreen
        public OrderDiscountScreen()
        {
            InitializeComponent();

            OrderDiscountScreenGrid.Visibility = Visibility.Hidden;

            GlobalEvents.ChangeScreenEvent += ChangeScreenEventHandler;
        }

        public void ChangeScreenEventHandler(object sender, ChangeScreenData e)
        {
            if (e.Screen == Screen.EnterOrderDiscountScreen)
            {
                GlobalEvents.ChangeScreenTitle.Invoke(null, "Order Discount");
                OrderDiscountScreenGrid.Visibility = Visibility.Visible;
            }
            else
            {
                OrderDiscountScreenGrid.Visibility = Visibility.Hidden;
            }
        }


        private void SetOrderDiscount(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            string text = ((TextBlock)button.Content).Text;

            if (text == "No Discount")
            {
                GlobalEvents.ChangeOrderDiscount(null, 0);
            }
            else
            {
                GlobalEvents.ChangeOrderDiscount(null, int.Parse(text.Replace("%", string.Empty)));
            }
        }

    }
}
