using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sam4sTillSystem.Enum;
using Sam4sTillSystem.Model;
using Sam4sTillSystem.State;

namespace Sam4sTillSystem.Data.Model
{
    public class DailyStatsRow
    {
        public List<AddItemPipeline> Items { get; set; }
        public bool IsCashSale { get; set; }
        public OrderType OrderType { get; set; }
        public int OrderDiscountInPercent { get; set; }
    }
}
