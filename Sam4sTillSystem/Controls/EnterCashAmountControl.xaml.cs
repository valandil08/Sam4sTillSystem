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

namespace Sam4sTillSystem.Controls
{
    /// <summary>
    /// Interaction logic for FunctionsScreen.xaml
    /// </summary>
    public partial class EnterCashAmountControl : UserControl
    {

        public static readonly RoutedEvent AmountEnteredEvent = EventManager.RegisterRoutedEvent("AmountEntered", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(EnterCashAmountControl));

        public event RoutedEventHandler AmountEntered
        {
            add { AddHandler(AmountEnteredEvent, value); }
            remove { RemoveHandler(AmountEnteredEvent, value); }
        }

        public string SubmitButtonText { get; set; } = "Submit Amount";

        public float MinAmountAccepted { get; set; } = 0;

        private string amountText = "";

        public EnterCashAmountControl()
        {
            InitializeComponent();

            SubmitButton.Text = SubmitButtonText;
        }
        private void SubmitButtonPressed(object sender, RoutedEventArgs e)
        {
            RoutedEventArgs newEventArgs = new RoutedEventArgs(AmountEnteredEvent);
            RaiseEvent(newEventArgs);
            e.Handled = true;
        }

        private void PaymentNumberPressed(object sender, RoutedEventArgs e)
        {
            string value = ((ClickableTile)sender).Text.ToString();

            switch (value)
            {
                case "£20":
                case "£10":
                case "£5":
                    amountText = value.Substring(1) + ".00";
                    break;

                case "Clear":
                    amountText = "";
                    break;

                case "Del":
                    if (amountText.Length > 0)
                    {
                        amountText = amountText.Substring(0, amountText.Length - 1);
                    }
                    break;

                case "0":
                case "1":
                case "2":
                case "3":
                case "4":
                case "5":
                case "6":
                case "7":
                case "8":
                case "9":
                    if (amountText != "0")
                    {
                        if (amountText.Contains("."))
                        {
                            if (amountText.Split('.')[1].Length < 2)
                            {
                                amountText += value;
                            }
                        }
                        else
                        {
                            amountText += value;
                        }
                    }
                    break;

                case ".":
                    if (amountText.Contains(".") == false && amountText.Length > 0)
                    {
                        amountText += ".";
                    }
                    break;
            }

            if (amountText == "")
            {
                PaymentInput.Text = "";
            } 
            else
            {
                PaymentInput.Text = "£" + amountText;
            }
        }

        private void AddItemPaymentNumberPressed(object sender, RoutedEventArgs e)
        {
            string value = ((Button)sender).Content.ToString();

            switch (value)
            {
                case "£20":
                case "£10":
                case "£5":
                    if (amountText.Contains(".") && amountText.Split('.')[1].Length == 2)
                    {
                        // add to amount
                        string[] parts = amountText.Split('.');

                        int amountOfPounds = int.Parse(parts[0]);
                        amountOfPounds += int.Parse(value.Replace("£", string.Empty));
                        amountText = amountOfPounds + "." + parts[1];
                    }
                    else
                    {
                        amountText = value.Substring(1) + ".00";
                    }
                    break;

                case "Clear":
                    amountText = "";
                    break;

                case "Del":
                    if (amountText.Length > 0)
                    {
                        amountText = amountText.Substring(0, amountText.Length - 1);
                    }
                    break;

                case "0":
                case "1":
                case "2":
                case "3":
                case "4":
                case "5":
                case "6":
                case "7":
                case "8":
                case "9":
                    if (amountText != "0")
                    {
                        if (amountText.Contains("."))
                        {
                            if (amountText.Split('.')[1].Length < 2)
                            {
                                amountText += value;
                            }
                        }
                        else
                        {
                            amountText += value;
                        }
                    }
                    break;

                case ".":
                    if (amountText.Contains(".") == false && amountText.Length > 0)
                    {
                        amountText += ".";
                    }
                    break;
            }

            if (amountText == "")
            {
                PaymentInput.Text = "";
            }
            else
            {
                PaymentInput.Text = "£" + amountText;
            }
        }
    }
}
