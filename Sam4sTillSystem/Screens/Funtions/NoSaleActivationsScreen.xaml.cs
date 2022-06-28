using Newtonsoft.Json;
using Sam4sTillSystem.Data;
using Sam4sTillSystem.Data.EventData;
using Sam4sTillSystem.Data.JsonObject;
using Sam4sTillSystem.Data.JsonObject.SiteSetting;
using Sam4sTillSystem.Enum;
using Sam4sTillSystem.FileFunctions;
using Sam4sTillSystem.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Sam4sTillSystem.Screens.Funtions
{
    /// <summary>
    /// Interaction logic for FunctionsScreen.xaml
    /// </summary>
    public partial class NoSaleActivationsScreen : UserControl
    {
        public NoSaleActivationsScreen()
        {
            InitializeComponent();

            NoSaleActivationsGrid.Visibility = Visibility.Hidden;

            GlobalEvents.ChangeScreenEvent += ChangeScreenEventHandler;
        }

        public void ChangeScreenEventHandler(object sender, ChangeScreenData e)
        {
            if(e.Screen == Screen.NoSaleActivationsScreen)
            {
                NoSaleActivationsGrid.Visibility = Visibility.Visible;
                GlobalEvents.ChangeScreenTitle(null, "No Sale Activations Today");
                LoadData();
            }
            else
            {
                NoSaleActivationsGrid.Visibility = Visibility.Hidden;
            }
        }


        public void LoadData()
        {
            OpeningHoursConfig openingHours = GlobalData.SiteSettings.OpeningHoursConfig;

            List<int> dateTimeValues = Sqlite.SqliteData.GetListOfNoSaleActivations(DateTime.Now);

            foreach (UIElement element in NoSaleActivationsGrid.Children.Cast<UIElement>().ToList())
            {
                if (element.GetType() == typeof(Border))
                {
                    NoSaleActivationsGrid.Children.Remove(element);
                }
            }

            while (NoSaleActivationsGrid.RowDefinitions.Count > 0)
            {
                NoSaleActivationsGrid.RowDefinitions.RemoveAt(NoSaleActivationsGrid.RowDefinitions.Count - 1);
            }

            NoSaleActivationsGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(5, GridUnitType.Pixel) });

            int numSections = dateTimeValues.Count + 3;
            int counter = 0;

            while (numSections > 0)
            {
                NoSaleActivationsGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(80, GridUnitType.Pixel) });
                numSections -= 3;
            }


            int openingTime = DataHelper.GetOpeningTime(DateTime.Now, openingHours);
            int closingTime = DataHelper.GetClosingTime(DateTime.Now, openingHours);

            int timesOpenedBeforeOpeningTime = dateTimeValues.Where(x => x < openingTime).Count();
            int timesOpenedAfterClosingTime = dateTimeValues.Where(x => x > closingTime).Count();

            UIElement beforeOpeningHours = UIHelper.CreateStatsTile("Before Opening Time", timesOpenedBeforeOpeningTime.ToString(), System.Windows.Media.Brushes.ForestGreen);
            NoSaleActivationsGrid.Children.Add(beforeOpeningHours);
            Grid.SetRow(beforeOpeningHours, 1);
            Grid.SetColumn(beforeOpeningHours, 0);

            UIElement duringOpeningHours = UIHelper.CreateStatsTile("During Work Time", (dateTimeValues.Count() - (timesOpenedBeforeOpeningTime + timesOpenedAfterClosingTime)).ToString(), System.Windows.Media.Brushes.ForestGreen);
            NoSaleActivationsGrid.Children.Add(duringOpeningHours);
            Grid.SetRow(duringOpeningHours, 1);
            Grid.SetColumn(duringOpeningHours, 1);

            UIElement afterOpeningHours = UIHelper.CreateStatsTile("After Closing Time", timesOpenedAfterClosingTime.ToString(), System.Windows.Media.Brushes.ForestGreen);
            NoSaleActivationsGrid.Children.Add(afterOpeningHours);
            Grid.SetRow(afterOpeningHours, 1);
            Grid.SetColumn(afterOpeningHours, 2);



            foreach (int dateTimeValue in dateTimeValues)
            {
                UIElement element = UIHelper.CreateStatsTile("Opened at", DataHelper.ConvertIntToTimeString(dateTimeValue), System.Windows.Media.Brushes.DarkCyan);

                NoSaleActivationsGrid.Children.Add(element);
                Grid.SetRow(element, (counter / 3) + 2);
                Grid.SetColumn(element, counter % 3);

                counter++;
            }
        }

    }
}
