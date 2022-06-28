using Newtonsoft.Json;
using Sam4sTillSystem.Data.JsonObject;
using Sam4sTillSystem.Data.JsonObject.MenuSetting;
using System.Collections.Generic;
using System.IO;

namespace Sam4sTillSystem.FileFunctions
{
    public static class MenuConfigFile
    {
        private static string fileLocation = @"./AppSettings/MenuConfig.json";

        public static void LoadSettings()
        {
            if (!File.Exists(fileLocation))
            {
                MenuSettings settings = new MenuSettings();
                settings.Categories = new List<MenuCategory>();
                settings.ProductButtons = new List<ProductButton>();
                settings.ProductScreens = new List<ProductScreen>();
                settings.ProductScreenControls = new List<ProductScreenControl>();

                File.WriteAllText(fileLocation, JsonConvert.SerializeObject(settings));
            }

            string fileData = File.ReadAllText(fileLocation);

            GlobalData.MenuSettings = JsonConvert.DeserializeObject<MenuSettings>(fileData);
        }

        public static void SaveSettings()
        {
            File.WriteAllText(fileLocation, JsonConvert.SerializeObject(GlobalData.MenuSettings));
        }
    }
}
