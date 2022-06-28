using Sam4sTillSystem.Data.JsonObject.SystemSetting;

namespace Sam4sTillSystem.Data.JsonObject
{
    public class SystemSettings
    {
        public CloudSettings CloudSettings = null;

        public HardwareSettings HardwareSettings = null;

        public DeveloperSettings DeveloperSettings = new DeveloperSettings();
    }
}
