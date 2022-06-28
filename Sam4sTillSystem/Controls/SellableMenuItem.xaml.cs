using Sam4sTillSystem.Helpers;
using System;
using System.Collections.Generic;
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

namespace Sam4sTillSystem.Controls
{
    /// <summary>
    /// Interaction logic for SellableMenuItem.xaml
    /// </summary>
    public partial class SellableMenuItem : UserControl
    {
        public string Text  = "";
        public int Price = 0;
        public bool IsVegetarian = false;
        public bool IsGlutenFree = false;
        public Action OnClick;

        public SellableMenuItem()
        {
            InitializeComponent();
        }

        public void Render(object sender = null, RoutedEventArgs e = null)
        {
            SellableItemText.Text = Text.Replace("\n", " ");
            SellableItemPrice.Text = DataHelper.ConvertIntToMoneyString(Price);
            if (IsVegetarian)
            {
                CircleOne.Background = Brushes.Green;
                CircleOne.Visibility = Visibility.Visible;

                if (IsGlutenFree)
                {
                    CircleTwo.Background = Brushes.White;
                    CircleTwo.Visibility = Visibility.Visible;
                }
                else
                {
                    CircleTwo.Visibility = Visibility.Hidden;
                }
            }
            else if (IsGlutenFree)
            {
                CircleOne.Background = Brushes.White;
                CircleOne.Visibility = Visibility.Visible;
                CircleTwo.Visibility = Visibility.Hidden;
            }
            else
            {
                CircleOne.Visibility = Visibility.Hidden;
                CircleTwo.Visibility = Visibility.Hidden;
            }
        }

        private void ControlPressed(object sender, MouseButtonEventArgs e)
        {
            OnClick.Invoke();
        }

        private void ControlPressed(object sender, TouchEventArgs e)
        {
            OnClick.Invoke();
        }
    }
}
