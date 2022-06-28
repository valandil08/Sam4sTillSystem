using Sam4sTillSystem.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
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
    /// Interaction logic for ItemQuantitySetter.xaml
    /// </summary>
    public partial class ItemQuantitySetter : UserControl
    {
        /*
            use an event combined with a guid generated in the class to show the quantity setter popup and upon a qty being pressed going straight to the specific control
         */

        private Guid ControlIdentifier = Guid.NewGuid();

        private int Quantity { get; set; }

        #region Xaml Properties

        public int MinQuantity
        {
            get { return (int)GetValue(MinQuantityProperty); }
            set { SetValue(MinQuantityProperty, value);  }
        }

        public static readonly DependencyProperty MinQuantityProperty = DependencyProperty.Register("MinQuantity", typeof(int), typeof(ItemQuantitySetter), new PropertyMetadata(0));
        
        public int MaxQuantity
        {
            get { return (int)GetValue(MaxQuantityProperty); }
            set { SetValue(MaxQuantityProperty, value); }
        }

        public static readonly DependencyProperty MaxQuantityProperty = DependencyProperty.Register("MaxQuantity", typeof(int), typeof(ItemQuantitySetter), new PropertyMetadata(0));

        public int DefaultQuantity
        {
            get { return (int)GetValue(DefaultQuantityProperty); }
            set { SetValue(DefaultQuantityProperty, value); }
        }

        public static readonly DependencyProperty DefaultQuantityProperty = DependencyProperty.Register("DefaultQuantity", typeof(int), typeof(ItemQuantitySetter), new PropertyMetadata(0));

        

        public string ItemText
        {
            get { return (string)GetValue(ItemTextProperty); }
            set { SetValue(ItemTextProperty, value); ItemNameTextBlock.Text = value; }
        }

        // Using a DependencyProperty as the backing store for ItemText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemTextProperty =
            DependencyProperty.Register("ItemText", typeof(string), typeof(ItemQuantitySetter), new PropertyMetadata(string.Empty));


        public static readonly RoutedEvent QuantityChangeEvent = EventManager.RegisterRoutedEvent("QuantityChange", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ItemQuantitySetter));

        public event RoutedEventHandler QuantityChange
        {
            add { AddHandler(QuantityChangeEvent, value); }
            remove { RemoveHandler(QuantityChangeEvent, value); }
        }
        #endregion

        public ItemQuantitySetter()
        {
            InitializeComponent();
            GlobalEvents.QuantitySelectGridAmountSelectedEvent += QuanitySelectedViaGrid;

            this.Loaded += Setup;
        }

        void QuanitySelectedViaGrid(object sender, GirdAmountSelectedData data)
        {
            if (data.guid == ControlIdentifier)
            {
                Quantity = data.amount;
                UpdateDropdownValue();
            }
        }

        public void Setup(object sender, RoutedEventArgs e)
        {
            ItemNameTextBlock.Text = ItemText;

            ItemQtyTextBox.Text = DefaultQuantity.ToString();

            if (DefaultQuantity >= MinQuantity && DefaultQuantity <= MaxQuantity)
            {
                Quantity = DefaultQuantity;
            }
            else
            {
                Quantity = MinQuantity;
            }
        }


        public int GetQuantity()
        {
            return Quantity;
        }

        private void GridButtonPressed(object sender, RoutedEventArgs e)
        {
            SetQuantityEventData data = new SetQuantityEventData();
            data.MinQuantity = MinQuantity;
            data.MaxQuantity = MaxQuantity;
            data.ControlIdenfier = ControlIdentifier;
            GlobalEvents.ShowQuantitySelectGridEvent.Invoke(null, data);
        }

        private void IncrementQuantityByOne(object sender, RoutedEventArgs e)
        {
            Quantity = GetNewQuantity(Quantity + 1);

            UpdateDropdownValue();
        }

        private void DecrementQuantityByOne(object sender, RoutedEventArgs e)
        {
            Quantity = GetNewQuantity(Quantity - 1);

            UpdateDropdownValue();
        }

        private void DropdownValueChanged(object sender, RoutedEventArgs e)
        {
            object value = ((ComboBox)sender).SelectedValue;

            if (value != null)
            {
                int selectedValue = int.Parse(value.ToString());
                Quantity = GetNewQuantity(selectedValue);

                RoutedEventArgs newEventArgs = new RoutedEventArgs(QuantityChangeEvent);
                newEventArgs.Source = this;

                RaiseEvent(newEventArgs);
            }
        }

        private int GetNewQuantity(int value)
        {
            if (value > MaxQuantity)
            {
                return MaxQuantity;
            }
            else
            if (value < MinQuantity)
            {
                return MinQuantity;
            }
            else
            {
                return value;
            }
        }

        private void UpdateDropdownValue()
        {
            ItemQtyTextBox.Text = Quantity.ToString();

            RoutedEventArgs newEventArgs = new RoutedEventArgs(QuantityChangeEvent);
            newEventArgs.Source = this;

            RaiseEvent(newEventArgs);
        }
    }
}
