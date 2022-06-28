using Sam4sTillSystem.Data.JsonObject;
using Sam4sTillSystem.Enum;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sam4sTillSystem.Data
{
    public class OrderInfo
    {
        public Guid Identifier { get; set; } = Guid.NewGuid();

        public string MenuIdentifier { get; set; } = null;

        public PaymentMethod paymentMethod { get; set; } = PaymentMethod.None;
        
        public List<OrderItemInfo> OrderItems { get; set; } = new List<OrderItemInfo>();

        public int DestinationNumber { get; set; } = 0;
        public string VatNumber { get; set; } = null;

        public int PercentageBasedOrderDiscount { get; set; } = 0;

        public double Vat { get; set; } = 12.5;

        public int ManualEntryInPence { get; set; } = 0;
        public int FixedDiscountInPence { get; set; } = 0;
        public int RefundInPence { get; set; } = 0;

        public int GetTotalCostInPence(MenuSettings menuSettings = null)
        {
            if (OrderItems == null)
            {
                return 0;
            }
            int totalInPence = OrderItems.Select(x => x.GetTotalCostInPence(menuSettings)).Sum();

            if (PercentageBasedOrderDiscount != 0)
            {
                totalInPence = (int)Math.Floor((((double)totalInPence) / 100) * (100 - PercentageBasedOrderDiscount));
            }
            return totalInPence;
        }


        public int GetOriginalTotalCostInPence(MenuSettings menuSettings = null)
        {
            if (OrderItems == null)
            {
                return 0;
            }

            return OrderItems.Select(x => x.GetTotalCostInPence(menuSettings)).Sum();
        }


        public int GetTotalRefundInPence(MenuSettings menuSettings = null)
        {
            if (OrderItems == null)
            {
                return 0;
            }

            return OrderItems.Where(x => x.ProductButtonId == -3).Select(x => x.GetTotalCostInPence(menuSettings)).Sum() * -1;
        }


        public int GetTotalManualEntryInPence(MenuSettings menuSettings = null)
        {
            if (OrderItems == null)
            {
                return 0;
            }

            return OrderItems.Where(x => x.ProductButtonId == -1).Select(x => x.GetTotalCostInPence(menuSettings)).Sum();
        }


        public int GetTotalDiscountInPence(MenuSettings menuSettings = null)
        {
            if (OrderItems == null)
            {
                return 0;
            }
            int totalNonFixedDiscount = OrderItems.Where(x => x.ProductButtonId != -2).Select(x => x.GetTotalCostInPence(menuSettings)).Sum();
            int totalFixedDiscount = OrderItems.Where(x => x.ProductButtonId == -2).Select(x => x.GetTotalCostInPence(menuSettings)).Sum();

            totalNonFixedDiscount = (int)Math.Floor(((double)totalNonFixedDiscount / 100) * PercentageBasedOrderDiscount);
            totalFixedDiscount = (int)Math.Floor((((double)totalFixedDiscount / 100) * (100 - PercentageBasedOrderDiscount)));
            

            return (-1 * totalNonFixedDiscount) + totalFixedDiscount;
        }

        public int GetTotalVatInPence(MenuSettings menuSettings = null)
        {
            if (OrderItems == null)
            {
                return 0;
            }

            int totalInPence = OrderItems.Select(x => x.GetTotalCostInPence(menuSettings)).Sum();

            if (PercentageBasedOrderDiscount != 0)
            {
                totalInPence = (int)Math.Floor(((double)totalInPence) / 100 * (100 - PercentageBasedOrderDiscount));
            }

            totalInPence = (int)Math.Ceiling((((double)totalInPence) * Vat) / 100);



            return totalInPence;
        }
    }
}
