using Sam4sTillSystem.Data;
using Sam4sTillSystem.Helpers;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Sam4sTillSystem.Screens.DailyItemSectionBreakdown
{

    /// <summary>
    /// Interaction logic for DailyItemSectionBreakdownScreen.xaml
    /// </summary>
    public partial class DailyItemSectionBreakdownScreen : UserControl
    {

        private DailyItemSectionBreakdownProps Props = new DailyItemSectionBreakdownProps();
        private DailyItemSectionBreakdownState State = new DailyItemSectionBreakdownState();
        private Action HandleBackPressed;

        public DailyItemSectionBreakdownScreen()
        {
            InitializeComponent();
        }

        public void LoadData(DailyItemSectionBreakdownProps props, Action handleBackPressed)
        {
            HandleBackPressed = handleBackPressed;
            Props = props;
            State.Stats = SqliteData.GetDailyItemSectionBreakdown(props.Date, props.SectionName);
            Render();
        }

        private void Render()
        {
            DailyItemSectionBreakdownGrid.ResetToEmptyGrid();

            DailyItemSectionBreakdownGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(5, GridUnitType.Pixel) });

            int rows = 8;

            for (int i = 0; i < rows; i++)
            {
                DailyItemSectionBreakdownGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            }

            // display results as 3 column grid
            DailyItemSectionBreakdownGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            DailyItemSectionBreakdownGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            DailyItemSectionBreakdownGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

            int counter = 0;
            
            UIElement totalCardSalesTile = UIHelper.CreateStatsTile("Total Card Sales", DataHelper.ConvertIntToMoneyString(State.Stats.TotalCardSales), System.Windows.Media.Brushes.ForestGreen);
            DailyItemSectionBreakdownGrid.Children.Add(totalCardSalesTile);
            Grid.SetRow(totalCardSalesTile, (counter / 3) + 1);
            Grid.SetColumn(totalCardSalesTile, counter % 3);
            counter++;

            UIElement totalCashSalesTile = UIHelper.CreateStatsTile("Total Cash Sales", DataHelper.ConvertIntToMoneyString(State.Stats.TotalCashSales), System.Windows.Media.Brushes.ForestGreen);
            DailyItemSectionBreakdownGrid.Children.Add(totalCashSalesTile);
            Grid.SetRow(totalCashSalesTile, (counter / 3) + 1);
            Grid.SetColumn(totalCashSalesTile, counter % 3);
            counter++;

            UIElement totalSalesTile = UIHelper.CreateStatsTile("Total Sales", DataHelper.ConvertIntToMoneyString(State.Stats.TotalSales), System.Windows.Media.Brushes.ForestGreen);
            DailyItemSectionBreakdownGrid.Children.Add(totalSalesTile);
            Grid.SetRow(totalSalesTile, (counter / 3) + 1);
            Grid.SetColumn(totalSalesTile, counter % 3);
            counter++;

            UIElement totalDiscountTile = UIHelper.CreateStatsTile("Total Discount", DataHelper.ConvertIntToMoneyString(State.Stats.TotalDiscount), System.Windows.Media.Brushes.Orange);
            DailyItemSectionBreakdownGrid.Children.Add(totalDiscountTile);
            Grid.SetRow(totalDiscountTile, (counter / 3) + 1);
            Grid.SetColumn(totalDiscountTile, counter % 3);
            counter++;
            /*
            UIElement totalManualEntryTile = UIHelper.CreateStatsTile("Total Manual Entry", DataHelper.ConvertIntToMoneyString(totalManualEntry), System.Windows.Media.Brushes.Orange);
            DailyItemSectionBreakdownGrid.Children.Add(totalManualEntryTile);
            Grid.SetRow(totalManualEntryTile, (counter / 3) + 1);
            Grid.SetColumn(totalManualEntryTile, counter % 3);
            */
            counter++;
            /*
            UIElement totalRefundTile = UIHelper.CreateStatsTile("Total Refund", DataHelper.ConvertIntToMoneyString(totalRefund), System.Windows.Media.Brushes.Orange);
            DailyItemSectionBreakdownGrid.Children.Add(totalRefundTile);
            Grid.SetRow(totalRefundTile, (counter / 3) + 1);
            Grid.SetColumn(totalRefundTile, counter % 3);
            */
            counter++;

            UIElement onSiteSalesTile = UIHelper.CreateStatsTile("On Site Sales", DataHelper.ConvertIntToMoneyString(State.Stats.OnSiteSales), System.Windows.Media.Brushes.Coral);
            DailyItemSectionBreakdownGrid.Children.Add(onSiteSalesTile);
            Grid.SetRow(onSiteSalesTile, (counter / 3) + 1);
            Grid.SetColumn(onSiteSalesTile, counter % 3);
            counter++;

            UIElement takeawaySalesTile = UIHelper.CreateStatsTile("Takeaway Sales", DataHelper.ConvertIntToMoneyString(State.Stats.TakeawaySales), System.Windows.Media.Brushes.Coral);
            DailyItemSectionBreakdownGrid.Children.Add(takeawaySalesTile);
            Grid.SetRow(takeawaySalesTile, (counter / 3) + 1);
            Grid.SetColumn(takeawaySalesTile, counter % 3);
            counter++;

            UIElement justEatSalesTile = UIHelper.CreateStatsTile("Just Eat Sales", DataHelper.ConvertIntToMoneyString(State.Stats.JustEatSales), System.Windows.Media.Brushes.Coral);
            DailyItemSectionBreakdownGrid.Children.Add(justEatSalesTile);
            Grid.SetRow(justEatSalesTile, (counter / 3) + 1);
            Grid.SetColumn(justEatSalesTile, counter % 3);
            counter++;

            foreach (string key in State.Stats.ItemBreakdown.Keys)
            {
                UIElement element = UIHelper.CreateStatsTile(key, DataHelper.ConvertIntToMoneyString(State.Stats.ItemBreakdown[key]), System.Windows.Media.Brushes.DarkCyan);
                DailyItemSectionBreakdownGrid.Children.Add(element);
                Grid.SetRow(element, (counter / 3) + 1);
                Grid.SetColumn(element, counter % 3);
                counter++;
            }
        }

        #region Events

        private void BackPressed(object sender, RoutedEventArgs e)
        {
            HandleBackPressed.Invoke();
        }

        private void PrintStatsPressed(object sender, RoutedEventArgs e)
        {
            //ReceiptPrinter.PrintItemSectionBreakdownItems(Props.SectionName, State.Stats);
            //ReceiptPrinter.PrintItemSectionBreakdownSummary(Props.SectionName, State.Stats);
        }

        #endregion
    }
}
