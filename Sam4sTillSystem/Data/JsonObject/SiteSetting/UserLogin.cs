using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sam4sTillSystem.Data.JsonObject.SiteSetting
{
    public class UserLogin
    {
        public string Name { get; set; } = "";
        public string Pin { get; set; } = "";

        public bool ManualEntryAllowed { get; set; } = false;
        public bool FixedDiscountAllowed { get; set; } = false;
        public bool RefundAllowed { get; set; } = false;
        public bool NoSaleAllowed { get; set; } = false;
        public bool NoSaleActivationsAllowed { get; set; } = false;
        public bool TodaysStatsAllowed { get; set; } = false;
        public bool OrderHistoryAllowed { get; set; } = false;
        public bool RegularOrdersAllowed { get; set; } = false;
        public bool MenuConfigAllowed { get; set; } = false;
        public bool SecurityConfigAllowed { get; set; } = false;
        public bool SystemConfigAllowed { get; set; } = false;
        public bool HardwareSettingsAllowed { get; set; } = false;
        public bool CloseAppAllowed { get; set; } = false;
        public bool ReloadConfigAllowed { get; set; } = false;
        public bool RestoreLastWorkingConfigAllowed { get; set; } = false;
        public bool EnableTestModeAllowed { get; set; } = false;

        public bool CloudUplinkAllowed { get; set; }
        public bool CheckForUpdateAllowed { get; set; }
        public bool DataSyncSettingsAllowed { get; set; }
    }
}
