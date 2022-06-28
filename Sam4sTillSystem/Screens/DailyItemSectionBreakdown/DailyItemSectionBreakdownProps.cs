using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sam4sTillSystem.Screens.DailyItemSectionBreakdown
{
    public class DailyItemSectionBreakdownProps 
    {
        public string SectionName { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
    }
}
