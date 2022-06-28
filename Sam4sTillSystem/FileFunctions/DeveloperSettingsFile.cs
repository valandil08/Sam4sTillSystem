using Newtonsoft.Json;
using Sam4sTillSystem.Data.JsonObject;
using Sam4sTillSystem.Data.JsonObject.SiteSetting;
using Sam4sTillSystem.Data.JsonObject.SystemSetting;
using System.Collections.Generic;
using System.IO;

namespace Sam4sTillSystem.FileFunctions
{
    public static class DeveloperSettingsFile
    {
        private static string fileLocation = @"./AppSettings/SystemSettings/DeveloperSettings.json";

        public static void LoadSettings()
        {
            if (!File.Exists(fileLocation))
            {
                DeveloperSettings settings = new DeveloperSettings();
                settings.DebugMode = false;

                File.WriteAllText(fileLocation, JsonConvert.SerializeObject(settings));
            }

            string fileData = File.ReadAllText(fileLocation);

            GlobalData.SystemSettings.DeveloperSettings = JsonConvert.DeserializeObject<DeveloperSettings>(fileData);
        }

        public static void SaveSettings()
        {
            File.WriteAllText(fileLocation, JsonConvert.SerializeObject(GlobalData.SystemSettings.DeveloperSettings));
        }
    }
}
