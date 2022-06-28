using Sam4sTillSystem.Data.JsonObject.SiteSetting;
using Sam4sTillSystem.Enum;
using Sam4sTillSystem.Model.FileModel;
using Sam4sTillSystem.State;
using System.Collections.Generic;

namespace Sam4sTillSystem.Model
{
    public class PageState
    {
        public YesNoPopupModel YesNoPopupModel { get; set; } = new YesNoPopupModel();
        public AddItemPipeline ItemPipeLine { get; set; } = new AddItemPipeline();
        public List<AddItemPipeline> ItemsInOrder { get; set; } = new List<AddItemPipeline>();

        public int Total { get; set; } = 0;
        public long ItemId { get; set; } = 0;
        public string SelectedItemName { get; set; } = "";
        public int SelectedItemCost { get; set; } = 0;
        public Dictionary<string, int> Extras { get; set; } = new Dictionary<string, int>();

        public string SelectedNavMenu { get; set; } = "";
        public int SelectedTableNumber { get; set; } = 0;
        public string PaymentAmountText { get; set; } = "";
        public PaymentMethod? PaymentMethod { get; set; } = null;

        public int ChangeDue = 0;
        public Screen CurrentScreen { get; set; }  = Screen.BlankScreen;
        public Screen PreviousScreen { get; set; } = Screen.BlankScreen;

        public DailyStats DailyStats { get; set; } = null;

        public string CurrentItemOptionsScreen { get; set; } = null;
        public SellableItem SelectedItem { get; set; } = null;
        public MandatoryItemGroup SelectedMandatoryItemGroup { get; set; }  = null;
        public OptionalItemGroup SelectedOptionalItemGroup { get; set; } = null;
        public List<RemovableItem> SelectedRemovableItems { get; set; } = null;
        public List<string> SelectedSwapForItems { get; set; } = null;
        
        public Dictionary<string, int> ItemBreakdownStats { get; set; } = null;
        public int OfferBasedDiscounts { get; set; } = 0;

        public UserLogin UserLogin = null;
    }
}
