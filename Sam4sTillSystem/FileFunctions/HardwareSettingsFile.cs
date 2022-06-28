using Newtonsoft.Json;
using Sam4sTillSystem.Data.JsonObject;
using System.IO;

namespace Sam4sTillSystem.FileFunctions
{
    public static class HardwareSettingsFile
    {
        private static string fileLocation = @"./AppSettings/SystemSettings/HardwareSettings.json";

        public static void LoadSettings()
        {
            if (!File.Exists(fileLocation))
            {
                HardwareSettings settings = new HardwareSettings();
                settings.ReceiptPrinterName = "None";
                settings.BackupReceiptPrinterOneName = "None";
                settings.BackupReceiptPrinterTwoName = "None";

                settings.ChefsPrinterMainName = "None";
                settings.BackupChefsPrinterOneName = "None";
                settings.BackupChefsPrinterTwoName = "None";


                File.WriteAllText(fileLocation, JsonConvert.SerializeObject(settings));
            }

            string fileData = File.ReadAllText(fileLocation);

            GlobalData.HardwareSettings = JsonConvert.DeserializeObject<HardwareSettings>(fileData);
        }

        public static void SaveSettings()
        {
            File.WriteAllText(fileLocation, JsonConvert.SerializeObject(GlobalData.HardwareSettings));
        }
    }
}
