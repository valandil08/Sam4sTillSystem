namespace Sam4sTillSystem.Data.JsonObject.MenuSetting
{
    public class ProductButton
    {
        public int ProductButtonId { get; set; }
        public int BasePriceInPence { get; set; }
        public string ButtonText { get; set; }
        public string ReceiptText { get; set; }
        public string ChefNoteText { get; set; }
        public string ButtonColour { get; set; }
        public bool IncludeInChefsNotes { get; set; }
        public int MenuCategoryId { get; set; }
        public int[] ProductScreenIds { get; set; } = new int[0]; // if 0 in length just add to order immediately
        public int[] AdditionalPrinterLinkIds { get; set; } = new int[0];
        public int[] AdjustmentControlIds { get; set; } = new int[0];
    }
}