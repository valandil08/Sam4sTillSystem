namespace Sam4sTillSystem.Data.JsonObject.MenuSetting
{
    public class MenuCategory
    {
        public int MenuCategoryId { get; set; }
        public int RowNum { get; set; }
        public int RowSize { get; set; }
        public int ColNum { get; set; }
        public int ColSize { get; set; }
        public string Text { get; set; }
        public string ButtonColour { get; set; }
        public int[] AdditionalPrinterLinkIds { get; set; }
    }
}