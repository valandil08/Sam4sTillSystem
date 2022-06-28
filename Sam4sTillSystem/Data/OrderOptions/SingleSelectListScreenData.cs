namespace Sam4sTillSystem.Data.OrderOptions
{
    public class SingleSelectListScreenData : ScreenData
    {
        public SingleSelectListScreenData(string screenName) : base(screenName, OrderOptionsScreenType.SingleSelectList)
        {

        }

        public override int GetItemTotalInPence()
        {
            return 0;
        }
    }
}
