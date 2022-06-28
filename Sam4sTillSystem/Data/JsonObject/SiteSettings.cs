using Sam4sTillSystem.Data.JsonObject;
using Sam4sTillSystem.Data.JsonObject.SiteSetting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sam4sTillSystem.Data.JsonObject
{
    public class SiteSettings
    {
        public int NumberOfTables { get; set; } = 26;
        public List<UserLogin> UserLogins { get; set; } = new List<UserLogin>();
        public OpeningHoursConfig OpeningHoursConfig { get; set; } = new OpeningHoursConfig();
    }
}
