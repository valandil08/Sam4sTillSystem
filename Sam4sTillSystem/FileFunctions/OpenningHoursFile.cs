
using Newtonsoft.Json;
using Sam4sTillSystem.Data.JsonObject.SiteSetting;
using System.IO;

namespace Sam4sTillSystem.FileFunctions
{
    public static class OpenningHoursFile
    {
        private static string fileLocation = @"./AppSettings/SiteSettings/OpenningHours.json";

        public static void LoadSettings()
        {
            if (!File.Exists(fileLocation))
            {
                OpeningHoursConfig settings = new OpeningHoursConfig();

                File.WriteAllText(fileLocation, JsonConvert.SerializeObject(settings));
            }

            string fileData = File.ReadAllText(fileLocation);

            GlobalData.SiteSettings.OpeningHoursConfig = JsonConvert.DeserializeObject<OpeningHoursConfig>(fileData);
        }

        public static void SaveSettings()
        {
            File.WriteAllText(fileLocation, JsonConvert.SerializeObject(GlobalData.SiteSettings.OpeningHoursConfig));
        }
    }
}
