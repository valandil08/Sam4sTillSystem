using Sam4sTillSystem.Data;
using Sam4sTillSystem.Data.EventData;
using Sam4sTillSystem.Data.PopupData;
using Sam4sTillSystem.State;
using System;
using System.Collections.Generic;

namespace Sam4sTillSystem
{
    public static class GlobalEvents
    {
        public static EventHandler<ChangeScreenData> ChangeScreenEvent;

        public static EventHandler<string> ChangeScreenTitle;

        public static EventHandler<int> ChangeTableNumber;

        public static EventHandler<int> ChangeOrderDiscount;

        public static EventHandler<bool> TestModeChangedEvent;

        public static EventHandler<OrderInfo> SavedOrderSelectedEvent;

        public static EventHandler<bool> SetLoggedInStatusEvent;

        public static EventHandler<SetQuantityEventData> ShowQuantitySelectGridEvent;

        public static EventHandler<List<CostBreakdownData>> ShowCostBreakdownGridEvent;

        public static EventHandler<GirdAmountSelectedData> QuantitySelectGridAmountSelectedEvent;

        public static EventHandler<OrderItemInfo> AddOrUpdateOrderItem;

        public static EventHandler<Guid> LoadItemSummaryForOrderItem;

        public static EventHandler UpdateOrderTotal;

        public static EventHandler ClearOrderDisplay;

        // might remove in future
        public static EventHandler<string> SaveNewOrderSelectedEvent;
        public static EventHandler ReloadConfigEvent;
    }

    public class GirdAmountSelectedData
    {
        public Guid guid { get; set; }
        public int amount { get; set; }
    }
}
