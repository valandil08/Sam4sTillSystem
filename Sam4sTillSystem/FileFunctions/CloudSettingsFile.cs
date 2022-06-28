using Newtonsoft.Json;
using Sam4sTillSystem.Data.JsonObject;
using System.IO;

namespace Sam4sTillSystem.FileFunctions
{
    public static class CloudSettingsFile
    {
        private static string fileLocation = @"./AppSettings/SystemSettings/CloudSettings.json";

        public static void LoadSettings()
        {
            if (!File.Exists(fileLocation))
            {
                CloudSettings settings = new CloudSettings();
                settings.Guid = null;
                settings.AuthorisationKey = null;

                File.WriteAllText(fileLocation, JsonConvert.SerializeObject(settings));
            }

            string fileData = File.ReadAllText(fileLocation);

            GlobalData.CloudSettings = JsonConvert.DeserializeObject<CloudSettings>(fileData);
        }

        public static void SaveSettings()
        {
            File.WriteAllText(fileLocation, JsonConvert.SerializeObject(GlobalData.CloudSettings));
        }
    }
}
