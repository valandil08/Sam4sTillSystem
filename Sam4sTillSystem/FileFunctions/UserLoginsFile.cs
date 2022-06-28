using Newtonsoft.Json;
using Sam4sTillSystem.Data.JsonObject;
using Sam4sTillSystem.Data.JsonObject.SiteSetting;
using System.Collections.Generic;
using System.IO;

namespace Sam4sTillSystem.FileFunctions
{
    public static class UserLoginsFile
    {
        private static string fileLocation = @"./AppSettings/SiteSettings/UserLogins.json";

        public static void LoadSettings()
        {
            if (!File.Exists(fileLocation))
            {
                SiteSettings settings = new SiteSettings();
                settings.UserLogins = new List<UserLogin>();

                File.WriteAllText(fileLocation, JsonConvert.SerializeObject(settings));
            }

            string fileData = File.ReadAllText(fileLocation);

            GlobalData.SiteSettings = JsonConvert.DeserializeObject<SiteSettings>(fileData);
        }

        public static void SaveSettings()
        {
            File.WriteAllText(fileLocation, JsonConvert.SerializeObject(GlobalData.SiteSettings));
        }
    }
}
