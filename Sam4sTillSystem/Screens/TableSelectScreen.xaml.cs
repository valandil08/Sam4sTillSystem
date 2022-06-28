using Sam4sTillSystem.Controls;
using Sam4sTillSystem.Data.EventData;
using Sam4sTillSystem.Data.JsonObject;
using Sam4sTillSystem.Enum;
using Sam4sTillSystem.FileFunctions;
using System;
using System.Net.Http;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace Sam4sTillSystem.Screens
{
    /// <summary>
    /// Interaction logic for CloudUplinkScreen.xaml
    /// </summary>
    public partial class TableSelectScreen : UserControl
    {
        //TableSelectScreen
        public TableSelectScreen()
        {
            InitializeComponent();

            TableSelectGrid.Visibility = Visibility.Hidden;

            GlobalEvents.ChangeScreenEvent += ChangeScreenEventHandler;
        }

        public void ChangeScreenEventHandler(object sender, ChangeScreenData e)
        {
            if (e.Screen == Screen.TableSelect)
            {
                GlobalEvents.ChangeScreenTitle.Invoke(null, "Table Select");
                SetupTableNumbers();
                TableSelectGrid.Visibility = Visibility.Visible;
            }
            else
            {
                TableSelectGrid.Visibility = Visibility.Hidden;
            }
        }


        private void SetupTableNumbers()
        {
            int numberOfColumns = 1;
            int numberOfTables = GlobalData.SiteSettings.NumberOfTables + 2;

            while ((numberOfColumns * numberOfColumns) < numberOfTables)
            {
                numberOfColumns += 1;
            }

            int currentColumnCount = TableSelectGrid.ColumnDefinitions.Count;

            for (int i = 0; i < (numberOfColumns - currentColumnCount); i++)
            {
                TableSelectGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                TableSelectGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            }

            for (int tableNumber = 0; tableNumber < numberOfTables; tableNumber++)
            {
                Button button = new Button();
                button.Margin = new Thickness(5, 5, 5, 5);
                button.Click += SetTableNumber;
                button.Background = System.Windows.Media.Brushes.LightGray;
                button.FontSize = 14;


                if (tableNumber == 0)
                {
                    button.Name = "SelectTableButton99999";
                    button.Content = "Takeaway";
                }
                else if (tableNumber == 1)
                {
                    button.Name = "SelectTableButton88888";
                    button.Content = "Just Eat";
                }
                else
                {
                    button.Name = "SelectTableButton" + (tableNumber - 1);
                    button.Content = (tableNumber - 1).ToString();
                }

                TableSelectGrid.Children.Add(button);
                Grid.SetRow(button, (tableNumber / numberOfColumns));
                Grid.SetColumn(button, tableNumber % numberOfColumns);

            }
        }


        private void SetTableNumber(object sender, RoutedEventArgs e)
        {
            string name = ((Button)sender).Name;

            int tableNumber = int.Parse(name.ToString().Replace("SelectTableButton", string.Empty));

            foreach (UIElement element in TableSelectGrid.Children)
            {
                if (element.GetType() == typeof(Button))
                {
                    Button button = (Button)element;

                    if (button.Name == "SelectTableButton" + tableNumber)
                    {
                        button.Background = System.Windows.Media.Brushes.Yellow;
                    }
                    else
                    {
                        button.Background = System.Windows.Media.Brushes.LightGray;
                    }
                }
            }
            GlobalData.OrderInfo.DestinationNumber = tableNumber;
            GlobalEvents.ChangeTableNumber.Invoke(null, tableNumber);
            GlobalEvents.ChangeScreenEvent.Invoke(null, new ChangeScreenData() { Screen = Screen.BlankScreen });

        }

    }
}
