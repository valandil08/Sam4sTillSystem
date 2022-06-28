
namespace Sam4sTillSystem.Model
{
    public class OrderItem
    {
        public string ItemName { get; set; }
        public int PriceInPence { get; set; }
        public int Qty { get; set; }
        public int Total { get; set; }
        public bool IncludeInChefsNotes { get; set; }

        public static bool operator ==(OrderItem item1, OrderItem item2)
        {
            if (item1.ItemName != item2.ItemName)
            {
                return false;
            }

            if (item1.PriceInPence != item2.PriceInPence)
            {
                return false;
            }

            if (item1.Qty != item2.Qty)
            {
                return false;
            }

            if (item1.Total != item2.Total)
            {
                return false;
            }

            return true;
        }

        public static bool operator !=(OrderItem item1, OrderItem item2)
        {

            return !(item1 == item2);
        }
    }
}
