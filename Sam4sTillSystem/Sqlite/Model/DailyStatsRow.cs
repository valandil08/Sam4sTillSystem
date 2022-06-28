using System.Collections.Generic;
using Sam4sTillSystem.Enum;
using Sam4sTillSystem.State;

namespace Sam4sTillSystem.Sqlite.Model
{
    public class DailyStatsRow
    {
        public List<AddItemPipeline> Items { get; set; }
        public bool IsCashSale { get; set; }
        public OrderType OrderType { get; set; }
        public int OrderDiscountInPercent { get; set; }
    }
}
