namespace Sam4sTillSystem.Data.JsonObject.MenuSetting
{
    public class ProductScreenControl
    {
        public int ProductScreenControlId { get; set; }
        public string ButtonText { get; set; }
        public string ReceiptText { get; set; }
        public string ChefNoteText { get; set; }
        public bool IsQuantitySetter { get; set; }
        public string ButtonColour { get; set; }
        public int CostPerExtraItemInPence { get; set; }
        public int DiscountPerRemovedItemInPence { get; set; }
        public int StartingQuantity { get; set; }
        public int minimumQuantity { get; set; }
        public int maximumQuantity { get; set; }
    }
}
