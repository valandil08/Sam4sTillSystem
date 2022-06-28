using System.Collections.Generic;

namespace Sam4sTillSystem.Data.Model
{
    public class DailyItemSectionBreakdownStats
    {
        public int TotalCardSales { get; set; } = 0;
        public int TotalCashSales { get; set; } = 0;
        public int TotalSales { get; set; } = 0;
        public int TotalDiscount { get; set; } = 0;
        public int TotalManualEntry { get; set; } = 0;
        public int TotalRefund { get; set; } = 0;
        public int OnSiteSales { get; set; } = 0;
        public int TakeawaySales { get; set; } = 0;
        public int JustEatSales { get; set; } = 0;

        public int TotalCardSalesDiscount { get; set; } = 0;
        public int TotalCashSalesDiscount { get; set; } = 0;
        public int TotalSalesDiscount { get; set; } = 0;
        public int TotalDiscountDiscount { get; set; } = 0;
        public int TotalManualEntryDiscount { get; set; } = 0;
        public int TotalRefundDiscount { get; set; } = 0;
        public int OnSiteSalesDiscount { get; set; } = 0;
        public int TakeawaySalesDiscount { get; set; } = 0;
        public int JustEatSalesDiscount { get; set; } = 0;
        public Dictionary<string, int> ItemBreakdown { get; set; } = new Dictionary<string, int>();
    }
}
