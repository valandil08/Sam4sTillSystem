namespace Sam4sTillSystem.Data.OrderOptions
{
    public abstract class ScreenData
    {
        public string ScreenName { get; } = "";

        public OrderOptionsScreenType ScreenType { get; }

        public int ItemTotalInPence { get { return GetItemTotalInPence(); } }

        public ScreenData(string screenName, OrderOptionsScreenType screenType)
        {
            ScreenName = screenName;
            ScreenType = screenType;
        }

        public abstract int GetItemTotalInPence();
    }
}
