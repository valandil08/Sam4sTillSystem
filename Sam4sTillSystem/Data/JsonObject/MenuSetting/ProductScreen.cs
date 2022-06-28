namespace Sam4sTillSystem.Data.JsonObject.MenuSetting
{
    public class ProductScreen
    {
        public int ProductScreenId { get; set; }
        public string ScreenTitle { get; set; }
        public int MinItemsRequired { get; set; } = 0;
        public int MaxItemsAllowed { get; set; } = 1;
        public bool IsMandatory { get; set; } = false;
        public int[] ProductScreenControlIds { get; set; }
    }
}
