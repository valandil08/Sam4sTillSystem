namespace Sam4sTillSystem.Data.OrderOptions
{
    public class ItemQuantitySetterScreenData : ScreenData
    {
        public ItemQuantitySetterScreenData(string screenName) : base(screenName, OrderOptionsScreenType.ItemQuantitySetter)
        {

        }

        public override int GetItemTotalInPence()
        {
            return 0;
        }
    }
}
