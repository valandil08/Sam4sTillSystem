namespace Sam4sTillSystem.Data.OrderOptions
{
    public class MultiSelectListScreenData : ScreenData
    {
        public MultiSelectListScreenData(string screenName) : base(screenName, OrderOptionsScreenType.MultiSlectList)
        {

        }

        public override int GetItemTotalInPence()
        {
            return 0;
        }
    }
}
