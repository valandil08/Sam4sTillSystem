using Sam4sTillSystem.Data;
using Sam4sTillSystem.Data.JsonObject;
using Sam4sTillSystem.Data.JsonObject.SiteSetting;

namespace Sam4sTillSystem
{
    public static class GlobalData
    {
        public static bool TestModeEnabled { get; set; } = false;

        public static int ChangeDue { get; set; } = 0;

        public static UserLogin UserLogin { get; set; } = null;

        public static OrderInfo OrderInfo = new OrderInfo();

        // put in System Settings
        public static CloudSettings CloudSettings = null;

        // put in System Settings
        public static HardwareSettings HardwareSettings = null;

        public static MenuSettings MenuSettings = null;

        public static SiteSettings SiteSettings = null;

        public static SystemSettings SystemSettings = new SystemSettings();
    }
}
