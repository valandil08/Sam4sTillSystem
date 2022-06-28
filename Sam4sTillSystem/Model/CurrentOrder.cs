using Sam4sTillSystem.State;
using System.Collections.Generic;

namespace Sam4sTillSystem.Model
{
    public class CurrentOrder
    {
        public List<AddItemPipeline> Items { get; set; } = new List<AddItemPipeline>();
        public int OrderDiscountInPercent { get; set; } = 0;
    }
}
