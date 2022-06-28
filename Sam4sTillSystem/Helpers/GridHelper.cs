using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Sam4sTillSystem.Helpers
{
    public static class GridHelper
    {
        public static void ResetToEmptyGrid(this Grid grid)
        {
            foreach (UIElement element in grid.Children.Cast<UIElement>().ToList())
            {
                RemoveElementNamespaceIfExists(grid, element);

                grid.Children.Remove(element);
            }

            while (grid.RowDefinitions.Count > 0)
            {
                grid.RowDefinitions.RemoveAt(0);
            }

            while (grid.ColumnDefinitions.Count > 0)
            {
                grid.ColumnDefinitions.RemoveAt(0);
            }
        }

        private static void RemoveElementNamespaceIfExists(Grid grid, UIElement element)
        {
            string name = null;

            if ((element as FrameworkElement) != null)
            {
                name = (element as FrameworkElement).Name;
            }

            object propertyValue = element.GetType().GetProperty("Name").GetValue(element, null);

            if (propertyValue != null)
            {
                name = (string)propertyValue;
            }

            if (name != null)
            {
                if (grid.FindName(name) != null)
                {
                    //NameScope.GetNameScope(grid).UnregisterName(name);
                }
            }
        }
    }
}
