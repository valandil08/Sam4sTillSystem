using Sam4sTillSystem.Data.JsonObject.MenuSetting;
using System.Collections.Generic;

namespace Sam4sTillSystem.Data.JsonObject
{
    public class MenuSettings
    {
        public List<MenuCategory> Categories { get; set; }
        public List<ProductButton> ProductButtons { get; set; }
        public List<ProductScreenControl> ProductScreenControls { get; set; }
        public List<ProductScreen> ProductScreens { get; set; }
        public int[] GenericExtraScreenControls { get; set; }   
        public PrinterLink[] Printers { get; set; }
        public double VatInPercent { get; set; }
        public string VatNumber { get; set; }
    }
}
