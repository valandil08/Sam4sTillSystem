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
    /// Interaction logic for MandatoryOptionElement.xaml
    /// </summary>
    public partial class OptionElement : UserControl
    {
        public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent("Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(MandatoryOptionElement));

        public event RoutedEventHandler Click
        {
            add { AddHandler(ClickEvent, value); }
            remove { RemoveHandler(ClickEvent, value); }
        }


        public bool IsSelected { get; set; } = false;

        public string Text { get; set; } = "";

        public OptionElement()
        {
            InitializeComponent();
            this.Loaded += Render;
        }

        public void Render(object sender = null, EventArgs e = null)
        {
            if (IsSelected)
            {
                MandatoryOptionElementBorder.Background = Brushes.Green;
            }
            else
            {
                MandatoryOptionElementBorder.Background = new SolidColorBrush(Color.FromRgb(221, 221, 221));
            }

            MandatoryOptionElementText.Text = Text;
        }

        private void ControlPressed(object sender, MouseButtonEventArgs e)
        {
            IsSelected = !IsSelected;
            Render();

            RoutedEventArgs newEventArgs = new RoutedEventArgs(ClickEvent);
            RaiseEvent(newEventArgs);
        }

        private void ControlPressed(object sender, TouchEventArgs e)
        {
            IsSelected = !IsSelected;
            Render();

            RoutedEventArgs newEventArgs = new RoutedEventArgs(ClickEvent);
            RaiseEvent(newEventArgs);
        }
    }
}
