using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sam4sTillSystem.Data.PopupData
{
    public class CostBreakdownData
    {
        public string Name { get; set; }
        public int CostPerExtraItemInPence { get; set; }
        public int DiscountPerRemovedItemInPence { get; set; }
        public int Difference { get; set; }
        public int CurrentCostInPence { get; set; }
    }
}
