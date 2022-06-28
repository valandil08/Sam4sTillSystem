using System.Collections.Generic;
using Sam4sTillSystem.Data.JsonObject.SiteSetting;
using Sam4sTillSystem.Model.ConfigurablePart;
using Sam4sTillSystem.Model.FileModel;

namespace Sam4sTillSystem.Model
{
    public class TillConfiguration
    {
        public List<string> UpperMenuSections { get; set; } = new List<string>();
        public List<string> LowerMenuSections { get; set; } = new List<string>();
        public List<OrderSection> OrderSections { get; set; } = new List<OrderSection>();
        public int NumberOfTables { get; set; }
        public string VatNumber { get; set; }
        public float VatRateInPercent { get; set; }
        public OpeningHoursConfig OpeningHours { get; set; }
    }
}
