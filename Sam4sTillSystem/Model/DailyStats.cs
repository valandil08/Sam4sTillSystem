using System;
using System.Collections.Generic;

namespace Sam4sTillSystem.Model
{
    public class DailyStats
    {
        public Dictionary<string, int> SectionSales { get; set; } = new Dictionary<string, int>();
        public int TotalCashSales { get; set; } = 0;
        public int TotalCardSales { get; set; } = 0;
        public int TotalSales { get; set; } = 0;
        public int TotalRefund { get; set; } = 0;
        public int TotalVat { get; set; } = 0;
        public int TotalDiscount { get; set; } = 0;
        public int TotalManualEntry{ get; set; } = 0;
        public int OnSiteSales { get; set; } = 0;
        public int JustEatSales { get; set; } = 0;
        public int TakeAwaySales { get; set; } = 0;

        public int TotalCashSalesDiscount { get; set; } = 0;
        public int TotalCardSalesDiscount { get; set; } = 0;
        public int TotalSalesDiscount { get; set; } = 0;
        public int OnSiteSalesDiscount { get; set; } = 0;
        public int JustEatSalesDiscount { get; set; } = 0;
        public int TakeAwaySalesDiscount { get; set; } = 0;
    }
}
