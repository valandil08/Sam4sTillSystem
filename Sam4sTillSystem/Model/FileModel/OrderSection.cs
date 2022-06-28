using System.Collections.Generic;

namespace Sam4sTillSystem.Model.FileModel
{
    public class OrderSection
    {
        public string SectionName { get; set; }
        public int Order { get; set; }
        public List<SellableItem> Items { get; set; } = new List<SellableItem>();
    }
}
