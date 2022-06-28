using Sam4sTillSystem.Data;
using Sam4sTillSystem.Data.EventData;
using Sam4sTillSystem.Enum;
using Sam4sTillSystem.Helpers;
using Sam4sTillSystem.Model.FileModel;
using Sam4sTillSystem.Screens.DailyItemSectionBreakdown;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Sam4sTillSystem.Screens.DailyStats
{
    /// <summary>
    /// Interaction logic for DailyStatsControl.xaml
    /// </summary>
    public partial class DailyStatsScreen : UserControl
    {
        private DateTime StatsDay = DateTime.Now;
        private Sqlite.Model.DailyStats dailyStats = null;

        public DailyStatsScreen()
        {
            InitializeComponent();

            GlobalEvents.ChangeScreenEvent += ChangeScreenEventHandler;
        }

        public void ChangeScreenEventHandler(object sender, ChangeScreenData e)
        {
            if (e.Screen == Screen.DailyStatsScreen)
            {
                StatsDay = DateTime.Now.Date;
                GlobalEvents.ChangeScreenTitle.Invoke(null, "Daily Stats - " + StatsDay.Day.ToString("00") + "/" + StatsDay.Month.ToString("00") + "/" + StatsDay.Year.ToString("0000"));
                DailyStatsGrid.Visibility = Visibility.Visible;
                LoadData();
            }
            else
            {
                DailyStatsGrid.Visibility = Visibility.Hidden;
            }
        }


        private void LoadData()
        {
            if (DateTime.Now.Date == StatsDay.Date)
            {
                NextDayButton.IsEnabled = false;
            }
            else
            {
                NextDayButton.IsEnabled = true;
            }

            dailyStats = Sqlite.SqliteData.GetDailyStats(StatsDay);

            foreach (UIElement element in DailyStatsGrid.Children.Cast<UIElement>().ToList())
            {
                if (element.GetType() == typeof(Border))
                {
                    DailyStatsGrid.Children.Remove(element);
                }
            }

            while (DailyStatsGrid.RowDefinitions.Count > 0)
            {
                DailyStatsGrid.RowDefinitions.RemoveAt(DailyStatsGrid.RowDefinitions.Count - 1);
            }

            DailyStatsGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(5, GridUnitType.Pixel) });

            int counter = 0;

            for (int i = 0; i < 8; i++)
            {
                DailyStatsGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            }

            DailyStatsGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(135, GridUnitType.Pixel) });


            UIElement totalCardSales = UIHelper.CreateStatsTile("Total Card Sales", DataHelper.ConvertIntToMoneyString(dailyStats.TotalCardSales), System.Windows.Media.Brushes.ForestGreen);
            DailyStatsGrid.Children.Add(totalCardSales);
            Grid.SetRow(totalCardSales, (counter / 3) + 1);
            Grid.SetColumn(totalCardSales, counter % 3);
            counter++;

            UIElement totalCashSales = UIHelper.CreateStatsTile("Total Cash Sales", DataHelper.ConvertIntToMoneyString(dailyStats.TotalCashSales), System.Windows.Media.Brushes.ForestGreen);
            DailyStatsGrid.Children.Add(totalCashSales);
            Grid.SetRow(totalCashSales, (counter / 3) + 1);
            Grid.SetColumn(totalCashSales, counter % 3);
            counter++;

            UIElement totalSales = UIHelper.CreateStatsTile("Total Sales", DataHelper.ConvertIntToMoneyString(dailyStats.TotalSales), System.Windows.Media.Brushes.ForestGreen);
            DailyStatsGrid.Children.Add(totalSales);
            Grid.SetRow(totalSales, (counter / 3) + 1);
            Grid.SetColumn(totalSales, counter % 3);
            counter++;

            UIElement onSiteSales = UIHelper.CreateStatsTile("On Site Sales", DataHelper.ConvertIntToMoneyString(dailyStats.OnSiteSales), System.Windows.Media.Brushes.Coral);
            DailyStatsGrid.Children.Add(onSiteSales);
            Grid.SetRow(onSiteSales, (counter / 3) + 1);
            Grid.SetColumn(onSiteSales, counter % 3);
            counter++;

            UIElement takeawaySales = UIHelper.CreateStatsTile("Takeaway Sales", DataHelper.ConvertIntToMoneyString(dailyStats.TakeAwaySales), System.Windows.Media.Brushes.Coral);
            DailyStatsGrid.Children.Add(takeawaySales);
            Grid.SetRow(takeawaySales, (counter / 3) + 1);
            Grid.SetColumn(takeawaySales, counter % 3);
            counter++;

            UIElement justEatSales = UIHelper.CreateStatsTile("Just Eat Sales", DataHelper.ConvertIntToMoneyString(dailyStats.JustEatSales), System.Windows.Media.Brushes.Coral);
            DailyStatsGrid.Children.Add(justEatSales);
            Grid.SetRow(justEatSales, (counter / 3) + 1);
            Grid.SetColumn(justEatSales, counter % 3);
            counter++;

            UIElement totalDiscount = UIHelper.CreateStatsTile("Total Discount", DataHelper.ConvertIntToMoneyString(dailyStats.TotalDiscount), System.Windows.Media.Brushes.Orange);
            DailyStatsGrid.Children.Add(totalDiscount);
            Grid.SetRow(totalDiscount, (counter / 3) + 1);
            Grid.SetColumn(totalDiscount, counter % 3);
            counter++;

            UIElement totalManualEntry = UIHelper.CreateStatsTile("Total Manual Entry", DataHelper.ConvertIntToMoneyString(dailyStats.TotalManualEntry), System.Windows.Media.Brushes.Orange);
            DailyStatsGrid.Children.Add(totalManualEntry);
            Grid.SetRow(totalManualEntry, (counter / 3) + 1);
            Grid.SetColumn(totalManualEntry, counter % 3);
            counter++;

            UIElement totalRefund = UIHelper.CreateStatsTile("Total Refund", DataHelper.ConvertIntToMoneyString(dailyStats.TotalRefund), System.Windows.Media.Brushes.Orange);
            DailyStatsGrid.Children.Add(totalRefund);
            Grid.SetRow(totalRefund, (counter / 3) + 1);
            Grid.SetColumn(totalRefund, counter % 3);
            counter++;


            List<int> dateTimeValues = Sqlite.SqliteData.GetListOfNoSaleActivations(StatsDay.Date);

            int openingTime = 9;// DataHelper.GetOpeningTime(DateTime.Now, Props.OpeningHours);
            int closingTime = 17;// DataHelper.GetClosingTime(DateTime.Now, Props.OpeningHours);

            int timesOpenedBeforeOpeningTime = dateTimeValues.Where(x => x < openingTime).Count();
            int timesOpenedAfterClosingTime = dateTimeValues.Where(x => x > closingTime).Count();

            UIElement beforeOpeningHours = UIHelper.CreateStatsTile("No Sales Before Work Time", timesOpenedBeforeOpeningTime.ToString(), System.Windows.Media.Brushes.LimeGreen);
            DailyStatsGrid.Children.Add(beforeOpeningHours);
            Grid.SetRow(beforeOpeningHours, (counter / 3) + 1);
            Grid.SetColumn(beforeOpeningHours, counter % 3);
            counter++;

            UIElement duringOpeningHours = UIHelper.CreateStatsTile("No Sales During Work Time", (dateTimeValues.Count() - (timesOpenedBeforeOpeningTime + timesOpenedAfterClosingTime)).ToString(), System.Windows.Media.Brushes.LimeGreen);
            DailyStatsGrid.Children.Add(duringOpeningHours);
            Grid.SetRow(duringOpeningHours, (counter / 3) + 1);
            Grid.SetColumn(duringOpeningHours, counter % 3);
            counter++;

            UIElement afterOpeningHours = UIHelper.CreateStatsTile("No Sales After Work Time", timesOpenedAfterClosingTime.ToString(), System.Windows.Media.Brushes.LimeGreen); ;
            DailyStatsGrid.Children.Add(afterOpeningHours);
            Grid.SetRow(afterOpeningHours, (counter / 3) + 1);
            Grid.SetColumn(afterOpeningHours, counter % 3);
            counter++;

            Action loadDailyStats = () =>
            {
                GlobalEvents.ChangeScreenEvent.Invoke(null, new ChangeScreenData() { Screen = Screen.DailyStatsScreen });
            };
            
            foreach (string sectionName in dailyStats.SectionSales.Keys)
            {
                string cashAmount = DataHelper.ConvertIntToMoneyString(dailyStats.SectionSales[sectionName]);

                UIElement element = UIHelper.CreateStatsTile(sectionName, cashAmount, System.Windows.Media.Brushes.DarkCyan);
                /*
                element.MouseLeftButtonDown += (object sender, MouseButtonEventArgs e) => {
                    //Props.mainWindow.ScreenTitle.Text = section.SectionName.Replace("\n", " ") + " - Daily Stats";
                    
                    Props.mainWindow.DailyItemSectionBreakdownControl.LoadData
                    (
                        new DailyItemSectionBreakdownProps()
                        {
                            SectionName = section.SectionName,
                            Date = DateTime.Now
                        },
                        loadDailyStats
                    );

                    GlobalEvents.ChangeScreenEvent.Invoke(null, new ChangeScreenData() { Screen = Screen.DailyItemSectionBreakdownScreen });
                    
                };
                */
                DailyStatsGrid.Children.Add(element);
                Grid.SetRow(element, (counter / 3) + 1);
                Grid.SetColumn(element, counter % 3);

                counter++;
            }
            
            /*
            for (int i = 0; i < (12 - Props.OrderSections.Count); i++)
            {
                UIElement element = UIHelper.CreateStatsTile("", "", System.Windows.Media.Brushes.DarkCyan);
                DailyStatsGrid.Children.Add(element);
                Grid.SetRow(element, (counter / 3) + 1);
                Grid.SetColumn(element, counter % 3);
                counter++;
            }
            */
        }

        private void PrintTodaysStats(object sender, RoutedEventArgs e)
        {
            ReceiptPrinter.DailyStatsQueue.QueueDocument(StatsDay);
        }

        private void PreviousDayPressed(object sender, RoutedEventArgs e)
        {
            StatsDay = StatsDay.AddDays(-1);
            GlobalEvents.ChangeScreenTitle.Invoke(null, "Daily Stats - " + StatsDay.Day.ToString("00") + "/" + StatsDay.Month.ToString("00") + "/" + StatsDay.Year.ToString("0000"));
            LoadData();
        }

        private void  NextDayPressed(object sender, RoutedEventArgs e)
        {
            StatsDay = StatsDay.AddDays(+1); 
            GlobalEvents.ChangeScreenTitle.Invoke(null, "Daily Stats - " + StatsDay.Day.ToString("00") + "/" + StatsDay.Month.ToString("00") + "/" + StatsDay.Year.ToString("0000"));
            LoadData();
        }
    }
}
