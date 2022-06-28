using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sam4sTillSystem.Data
{
    public class SetQuantityEventData
    {
        public Guid ControlIdenfier { get; set; }
        public int MinQuantity { get; set; }
        public int MaxQuantity { get; set; }
    }
}
