using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Sam4sTillSystem.Helpers
{
    public static class UIHelper
    {

        public static UIElement CreateStatsTile(string name, string amount, System.Windows.Media.Brush color)
        {
            Border border = new Border();
            border.Margin = new Thickness(5, 0, 5, 5);
            border.Background = color;
            border.CornerRadius = new CornerRadius(5);

            Grid grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(3, GridUnitType.Pixel) });
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(5, GridUnitType.Pixel) });

            TextBlock titleTextBlock = new TextBlock();
            titleTextBlock.Text = name.Replace("\n", " ");
            titleTextBlock.FontSize = 14;
            titleTextBlock.FontWeight = FontWeights.Bold;
            titleTextBlock.VerticalAlignment = VerticalAlignment.Center;
            titleTextBlock.HorizontalAlignment = HorizontalAlignment.Center;
            titleTextBlock.TextAlignment = TextAlignment.Center;

            grid.Children.Add(titleTextBlock);
            Grid.SetColumn(titleTextBlock, 0);
            Grid.SetRow(titleTextBlock, 1);

            TextBlock amountTextBlock = new TextBlock();
            amountTextBlock.Text = amount;
            amountTextBlock.FontSize = 20;
            amountTextBlock.FontWeight = FontWeights.Bold;
            amountTextBlock.VerticalAlignment = VerticalAlignment.Center;
            amountTextBlock.HorizontalAlignment = HorizontalAlignment.Center;
            amountTextBlock.TextAlignment = TextAlignment.Center;
            amountTextBlock.Padding = new Thickness(0, 0, 5, 0);

            grid.Children.Add(amountTextBlock);
            Grid.SetColumn(amountTextBlock, 0);
            Grid.SetRow(amountTextBlock, 2);

            border.Child = grid;

            return border;
        }

        public static string FormatPrice(int priceInPence)
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
            else if (priceInPence < 100)
            {
                return prefix + "£0." + priceInPence;
            }
            else
            {
                string number = priceInPence.ToString();

                number = number.Insert(number.Length - 2, ".");

                return prefix + "£" + number;
            }
        }
    }
}
