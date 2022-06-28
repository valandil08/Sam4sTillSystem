using System.Collections.Generic;

namespace Sam4sTillSystem.Model.FileModel
{
    public class SellableItem
    {
        public string ItemName { get; set; }
        public int PriceInPence { get; set; }
        public bool IncludeInChefsNotes { get; set; } = false;
        public bool IsVegetarian { get; set; } = false;
        public bool IsGlutenFree { get; set; } = false;

        public Dictionary<string, int> Extras { get; set; } = new Dictionary<string, int>();
        public List<OptionalItemGroup> OptionalItemGroups { get; set; } = new List<OptionalItemGroup>();
        public List<MandatoryItemGroup> MandatoryItemsGroups { get; set; } = new List<MandatoryItemGroup>();
        public List<RemovableItem> RemovableItems { get; set; } = new List<RemovableItem>();
        public List<string> SwapForItems { get; set; } = new List<string>();
        public List<string> ScreenDisplayOrder { get; set; } = new List<string>();
    }
}