using Sam4sTillSystem.Enum;

namespace Sam4sTillSystem.Data.EventData
{
    public class ChangeScreenData
    {
        public Screen Screen { get; set; } 
        public string ScreenName { get; set; } = null;
        public object Data { get; set; } = null;
    }
}
