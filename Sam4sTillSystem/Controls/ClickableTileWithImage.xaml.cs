using Microsoft.PointOfService;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Sam4sTillSystem.Controls
{
    /// <summary>
    /// Interaction logic for ClickableTile.xaml
    /// </summary>
    public partial class ClickableTileWithImage : UserControl
    {
        public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent("Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(TabItem));

        public event RoutedEventHandler Click
        {
            add { AddHandler(ClickEvent, value); }
            remove { RemoveHandler(ClickEvent, value); }
        }

        public string Text { get; set; }

        public bool Borderless { get; set; } = false;

        public static new readonly DependencyProperty IsEnabledProperty = DependencyProperty.Register("IsEnabled", typeof(bool), typeof(ClickableTileWithImage), new PropertyMetadata(true));

        public new bool IsEnabled
        {
            get { return (bool)base.GetValue(IsEnabledProperty); }
            set
            {
                base.SetValue(IsEnabledProperty, value);
                Render();
            }
        }

        public string ImagePath { get; set; }

        public ClickableTileWithImage()
        {
            InitializeComponent();
            this.Loaded += Render;
        }

        public void Render(object sender = null, EventArgs e = null)
        {
            if (IsEnabled)
            {
                ClickableTileBorder.Background = new SolidColorBrush(Color.FromRgb(221, 221, 221));
            }
            else
            {
                ClickableTileBorder.Background = Brushes.Gray;
            }

            if (Borderless)
            {
                ClickableTileBorder.BorderThickness = new Thickness(0);
            }
            else
            {
                ClickableTileBorder.BorderThickness = new Thickness(0.6);
            }

            ClickableTileText.FontSize = 14;

            ClickableTileText.Text = Text;

            if (ImagePath != null)
            {
                ClickableTileImage.Source = new BitmapImage(new Uri(ImagePath, UriKind.Relative));
            }
        }

        private void ControlPressed(object sender, MouseButtonEventArgs e)
        {
            if (IsEnabled)
            {
                RoutedEventArgs newEventArgs = new RoutedEventArgs(ClickEvent);
                RaiseEvent(newEventArgs);
            }
            e.Handled = true;
        }

        private void ControlPressed(object sender, TouchEventArgs e)
        {
            if (IsEnabled)
            {
                RoutedEventArgs newEventArgs = new RoutedEventArgs(ClickEvent);
                RaiseEvent(newEventArgs);
            }
            e.Handled = true;
        }
    }
}
